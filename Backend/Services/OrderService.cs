using System.Threading.Tasks;
using System;
using System.Linq;
using Backend.Data;
using Backend.Models;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Backend.Models.ViewModels;
using Backend.Interfaces.Services;
using Backend.Models.Exceptions;
using Backend.Utils;
using Backend.Models.Enums;
using Backend.Interfaces.UnitOfWork;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;

namespace Backend.Services
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IProductService _productService;
        public OrderService(IUnitOfWork unitOfWork, IProductService service)
        {
            _productService = service;
            _unitOfWork = unitOfWork;
        }

        public List<Order> GetOrders(User user, string sort, int? queryPage)
        {
            try
            {
                Expression<Func<Order, bool>> predicateFilter = order => order.Client.Id == user.Id;
                Func<IQueryable<Order>, IOrderedQueryable<Order>> orderBy = null;
                Func<IQueryable<Order>, IIncludableQueryable<Order, object>> includes = order =>
                order.Include(order => order.Client).Include(order => order.Items).ThenInclude(item => item.Product);
                if (sort == "asc")
                    orderBy = q => q.OrderBy(order => order.CreationDate);
                else if (sort == "desc")
                    orderBy = q => q.OrderByDescending(order => order.CreationDate);

                return _unitOfWork.OrderRepository.Get(predicateFilter, orderBy, includes, queryPage).ToList();
            }
            catch (InvalidOperationException)
            {
                throw;
            }
        }
        public Order GetOrderById(User requestingUser, int id)
        {
            try
            {
                Func<IQueryable<Order>, IIncludableQueryable<Order, object>> includes = order =>
                order.Include(order => order.Client).Include(order => order.Items).ThenInclude(item => item.Product);

                Order order = _unitOfWork.OrderRepository.Get(filter: null, orderBy: null, includes, page: null)
                .FirstOrDefault(order => order.Id == id);
                if (requestingUser.Role != "Administrator" && order?.Client?.Id != requestingUser.Id)
                    throw new UnauthorizedAccessException(ErrorMessage.GetMessage(ErrorType.NotAuthorized));
                return order;
            }
            catch (InvalidOperationException)
            {
                throw;
            }
        }
        public Order GetOpenOrder(User user)
        {
            try
            {
                return GetOrders(user, "desc", null).FirstOrDefault(order =>
                order.Status == OrderStatus.Open);
            }
            catch (InvalidOperationException)
            {
                throw;
            }
        }
        public List<Order> GetInProgressOrders(User user)
        {
            try
            {
                return GetOrders(user, "desc", null).Where(order =>
                order.Status == OrderStatus.InProgress).ToList();
            }
            catch (InvalidOperationException)
            {
                throw;
            }
        }
        public Order CreateOrder(User user)
        {
            try
            {
                Order order = new Order()
                {
                    Client = user,
                    Status = OrderStatus.Open,
                    CreationDate = DateTime.Now,
                    Items = new List<ChartItem>()
                };
                _unitOfWork.OrderRepository.Insert(order);
                _unitOfWork.Commit();
                return order;
            }
            catch (InvalidOperationException)
            {
                throw;
            }
        }
        public ChartItem AddToChart(User user, AddToChartRequest request)
        {
            try
            {
                if (_productService.GetProductById(request.ProductId.Value) == null)
                    throw new RegisterNotFoundException(ErrorMessage.GetMessage(ErrorType.ProductIdNotFound));
                Order order = GetOpenOrder(user);
                if (order == null)
                    order = CreateOrder(user);
                Product product = _productService.UpdateProductQuantity(request.ProductId.Value, request.Quantity.Value);
                order.TotalValue += product.Price.Value * request.Quantity.Value;
                ChartItem item = order.Items.FirstOrDefault(item => item?.Product?.Id == request.ProductId.Value);
                if (item == null)
                {
                    item = new ChartItem
                    {
                        Product = product,
                        Price = product.Price.Value,
                        Quantity = request.Quantity.Value
                    };
                    order.Items.Add(item);
                }
                else
                    item.Quantity += request.Quantity.Value;
                _unitOfWork.OrderRepository.Update(order);
                _unitOfWork.Commit();
                return item;
            }
            catch (InvalidOperationException)
            {
                throw;
            }
            catch (OutOfStockException)
            {
                throw;
            }
        }

        public ChartItem RemoveFromChart(User user, int id)
        {
            try
            {
                Order order = GetOpenOrder(user);
                if (order == null)
                    throw new RegisterNotFoundException(ErrorMessage.GetMessage(ErrorType.RemoveItemFromEmptyChart));
                ChartItem item = order.Items.FirstOrDefault(item => item.Product.Id == id);
                if (item == null)
                    throw new RegisterNotFoundException(ErrorMessage.GetMessage(ErrorType.ProductIdNotFound));
                else
                {
                    order.Items.Remove(item);
                    order.TotalValue -= item.Price * item.Quantity;
                    if (order.Items.Count == 0)
                        _unitOfWork.OrderRepository.Delete(order.Id);
                    _productService.UpdateProductQuantity(id, -item.Quantity);
                    _unitOfWork.Commit();
                    return item;
                }
            }
            catch (InvalidOperationException)
            {
                throw;
            }
        }

        public ChartItem ChangeItemQuantity(User user, int id, string sign)
        {
            try
            {
                Order order = GetOpenOrder(user);
                if (order == null)
                    throw new RegisterNotFoundException(ErrorMessage.GetMessage(ErrorType.ChangeItemFromEmptyChart));
                ChartItem item = order.Items.FirstOrDefault(item => item.Product.Id == id);
                if (item == null)
                    throw new RegisterNotFoundException(ErrorMessage.GetMessage(ErrorType.ChangeItemNotInChart));
                else
                {
                    if (sign == "Increase")
                    {
                        _productService.UpdateProductQuantity(id, 1);
                        item.Quantity++;
                        order.TotalValue += item.Price;
                    }
                    else
                    {
                        if (item.Quantity == 1)
                        {
                            RemoveFromChart(user, id);
                            item.Quantity--;
                        }
                        else
                        {
                            item.Quantity--;
                            order.TotalValue -= item.Price;
                            _productService.UpdateProductQuantity(id, -1);
                        }
                    }
                    _unitOfWork.Commit();
                    return item;
                }
            }
            catch (InvalidOperationException)
            {
                throw;
            }
            catch (OutOfStockException)
            {
                throw;
            }
        }
        public Order CancelOrder(User user)
        {
            try
            {
                Order order = GetOpenOrder(user);
                if (order == null)
                    throw new RegisterNotFoundException(ErrorMessage.GetMessage(ErrorType.CancelEmptyChart));
                order.Status = OrderStatus.Cancelled;
                order.CancellationDate = DateTime.Now;
                order.Items.ForEach(item =>
                {
                    _productService.UpdateProductQuantity(item.Product.Id, -item.Quantity);
                });
                _unitOfWork.Commit();
                return order;
            }
            catch (InvalidOperationException)
            {
                throw;
            }
        }

        public Order CheckoutOrder(User user, Payment payment)
        {
            try
            {
                Order order = GetOpenOrder(user);
                if (order == null)
                    throw new RegisterNotFoundException(ErrorMessage.GetMessage(ErrorType.CheckoutEmptyChart));
                order.Status = OrderStatus.InProgress;
                _unitOfWork.Commit();
                ProcessPurchase(order, payment);
                return order;
            }
            catch (InvalidOperationException)
            {
                throw;
            }
        }

        public void ProcessPurchase(Order order, Payment payment)
        {
            // TODO: Improve this method
            int processingTime = 0;
            switch (payment.PaymentMethod)
            {
                case PaymentMethod.InCash:
                    processingTime = 0; // instant processing
                    break;
                case PaymentMethod.CreditCard:
                    processingTime = 1 * 1000 * 60; // 1min processing
                    break;
                case PaymentMethod.BankSlip:
                    processingTime = 5 * 1000 * 60; // 5min processing
                    break;
            }
            Task.Delay(processingTime).ContinueWith(_ =>
            {
                try
                {
                    // TODO: Fix this part (disposing before persisting data in DB)
                    using var unitOfWork = _unitOfWork;
                    order.Status = OrderStatus.Finished;
                    order.FinishingDate = DateTime.Now;
                    _unitOfWork.OrderRepository.Update(order);
                    _unitOfWork.Commit();
                }
                catch (InvalidOperationException)
                {
                    throw;
                }
            });
        }
    }
}