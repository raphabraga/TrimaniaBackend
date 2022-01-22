using System.Threading.Tasks;
using System;
using System.Linq;
using Backend.Models;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Backend.Interfaces.Services;
using Backend.Models.Exceptions;
using Backend.Utils;
using Backend.Models.Enums;
using Backend.Interfaces.Repositories;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;
using Backend.Dtos;

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

        public async Task<List<Order>> GetOrders(User user, string sort, int? queryPage)
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
                var orders = await _unitOfWork.OrderRepository.Get(predicateFilter, orderBy, includes, queryPage);
                return orders.ToList();
            }
            catch (InvalidOperationException)
            {
                throw;
            }
        }
        public async Task<Order> GetOrderById(User requestingUser, int id)
        {
            try
            {
                Func<IQueryable<Order>, IIncludableQueryable<Order, object>> includes = order =>
                order.Include(order => order.Client).Include(order => order.Items).ThenInclude(item => item.Product);

                var userOrder = await _unitOfWork.OrderRepository.Get(filter: null, orderBy: null, includes, page: null);
                var order = userOrder.FirstOrDefault(order => order.Id == id);
                if (requestingUser.Role != "Administrator" && order?.Client?.Id != requestingUser.Id)
                    throw new UnauthorizedAccessException(ErrorUtils.GetMessage(ErrorType.NotAuthorized));
                return order;
            }
            catch (InvalidOperationException)
            {
                throw;
            }
        }
        public async Task<Order> GetOpenOrder(User user)
        {
            try
            {
                var userOrders = await GetOrders(user, "desc", null);
                return userOrders.FirstOrDefault(order => order.Status == OrderStatus.Open);
            }
            catch (InvalidOperationException)
            {
                throw;
            }
        }
        public async Task<List<Order>> GetInProgressOrders(User user)
        {
            try
            {
                var userOrders = await GetOrders(user, "desc", null);
                return userOrders.Where(order => order.Status == OrderStatus.InProgress).ToList();
            }
            catch (InvalidOperationException)
            {
                throw;
            }
        }
        public async Task<Order> CreateOrder(User user)
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
                await _unitOfWork.Commit();
                return order;
            }
            catch (InvalidOperationException)
            {
                throw;
            }
        }
        public async Task<ChartItem> AddToChart(User user, AddToChartRequest request)
        {
            try
            {
                if (await _productService.GetProductById(request.ProductId.Value) == null)
                    throw new RegisterNotFoundException(ErrorUtils.GetMessage(ErrorType.ProductIdNotFound));
                Order order = await GetOpenOrder(user);
                if (order == null)
                    order = await CreateOrder(user);
                Product product = await _productService.UpdateProductQuantity(request.ProductId.Value, request.Quantity.Value);
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
                await _unitOfWork.Commit();
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

        public async Task<ChartItem> RemoveFromChart(User user, int id)
        {
            try
            {
                Order order = await GetOpenOrder(user);
                if (order == null)
                    throw new RegisterNotFoundException(ErrorUtils.GetMessage(ErrorType.RemoveItemFromEmptyChart));
                ChartItem item = order.Items.FirstOrDefault(item => item.Product.Id == id);
                if (item == null)
                    throw new RegisterNotFoundException(ErrorUtils.GetMessage(ErrorType.ProductIdNotFound));
                else
                {
                    order.Items.Remove(item);
                    order.TotalValue -= item.Price * item.Quantity;
                    if (order.Items.Count == 0)
                        _unitOfWork.OrderRepository.Delete(order.Id);
                    await _productService.UpdateProductQuantity(id, -item.Quantity);
                    await _unitOfWork.Commit();
                    return item;
                }
            }
            catch (InvalidOperationException)
            {
                throw;
            }
        }

        public async Task<ChartItem> ChangeItemQuantity(User user, int id, string sign)
        {
            try
            {
                Order order = await GetOpenOrder(user);
                if (order == null)
                    throw new RegisterNotFoundException(ErrorUtils.GetMessage(ErrorType.ChangeItemFromEmptyChart));
                ChartItem item = order.Items.FirstOrDefault(item => item.Product.Id == id);
                if (item == null)
                    throw new RegisterNotFoundException(ErrorUtils.GetMessage(ErrorType.ChangeItemNotInChart));
                else
                {
                    if (sign == "Increase")
                    {
                        await _productService.UpdateProductQuantity(id, 1);
                        item.Quantity++;
                        order.TotalValue += item.Price;
                    }
                    else
                    {
                        if (item.Quantity == 1)
                        {
                            await RemoveFromChart(user, id);
                            item.Quantity--;
                        }
                        else
                        {
                            item.Quantity--;
                            order.TotalValue -= item.Price;
                            await _productService.UpdateProductQuantity(id, -1);
                        }
                    }
                    await _unitOfWork.Commit();
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
        public async Task<Order> CancelOrder(User user)
        {
            try
            {
                Order order = await GetOpenOrder(user);
                if (order == null)
                    throw new RegisterNotFoundException(ErrorUtils.GetMessage(ErrorType.CancelEmptyChart));
                order.Status = OrderStatus.Cancelled;
                order.CancellationDate = DateTime.Now;
                order.Items.ForEach(async item =>
                {
                    await _productService.UpdateProductQuantity(item.Product.Id, -item.Quantity);
                });
                await _unitOfWork.Commit();
                return order;
            }
            catch (InvalidOperationException)
            {
                throw;
            }
        }

        public async Task<Order> CheckoutOrder(User user, PaymentRequest payment)
        {
            try
            {
                Order order = await GetOpenOrder(user);
                if (order == null)
                    throw new RegisterNotFoundException(ErrorUtils.GetMessage(ErrorType.CheckoutEmptyChart));
                order.Status = OrderStatus.InProgress;
                await _unitOfWork.Commit();
                await ProcessPurchase(order, payment);
                return order;
            }
            catch (InvalidOperationException)
            {
                throw;
            }
        }

        public async Task ProcessPurchase(Order order, PaymentRequest payment)
        {
            // TODO: Improve this method
            int processingTime = 0;
            switch (payment.PaymentMethod)
            {
                case PaymentMethod.InCash:
                    processingTime = 0; // instant processing
                    break;
                case PaymentMethod.CreditCard:
                    processingTime = 1 * 1000 * 30; // 30s processing
                    break;
                case PaymentMethod.BankSlip:
                    processingTime = 1 * 1000 * 60; // 1min processing
                    break;
            }
            await Task.Delay(processingTime).ContinueWith(_ =>
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