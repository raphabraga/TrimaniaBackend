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

        private async Task<List<Order>> GetAllOrders(string sort)
        {
            Expression<Func<Order, bool>> predicateFilter = order => true;
            Func<IQueryable<Order>, IOrderedQueryable<Order>> orderBy = null;
            if (sort == "asc")
                orderBy = q => q.OrderBy(order => order.CreationDate);
            else if (sort == "desc")
                orderBy = q => q.OrderByDescending(order => order.CreationDate);
            Func<IQueryable<Order>, IIncludableQueryable<Order, object>> includes = order =>
            order.Include(order => order.Client).Include(order => order.Items).ThenInclude(item => item.Product);
            var orders = await _unitOfWork.OrderRepository.Get(predicateFilter, orderBy, includes, page: null);
            return orders.ToList();
        }

        public async Task<List<Order>> GetOrders(User user, string sort, int? queryPage)
        {
            int resultsPerPage = 10;
            var orders = await GetAllOrders(sort);
            if (user.Role != "Administrator")
                orders = orders.Where(order => order.ClientId == user.Id).ToList();
            if (queryPage == null)
                return orders;
            else
                return orders.Skip((queryPage.Value - 1) * resultsPerPage).Take(resultsPerPage).ToList();
        }
        public async Task<Order> GetOrderById(User requestingUser, int id)
        {
            var allOrders = await GetAllOrders("asc");
            var userOrders = await GetOrders(requestingUser, sort: "asc", queryPage: null);
            var order = allOrders.FirstOrDefault(order => order.Id == id);
            if (order == null)
                throw new RegisterNotFoundException(ErrorUtils.GetMessage(ErrorType.OrderIdNotFound));
            if (requestingUser.Role != "Administrator" && order.ClientId != requestingUser.Id)
                throw new UnauthorizedAccessException(ErrorUtils.GetMessage(ErrorType.NotAuthorized));
            return order;
        }
        public async Task<Order> GetOpenOrder(User user)
        {
            var userOrders = await GetOrders(user, "desc", null);
            var userOpenOrder = userOrders.FirstOrDefault(order => order.Status == OrderStatus.Open);
            if (userOpenOrder == null)
                throw new RegisterNotFoundException(ErrorUtils.GetMessage(ErrorType.NoOpenOrders));
            return userOpenOrder;
        }
        public async Task<List<Order>> GetInProgressOrders(User user)
        {
            var userOrders = await GetOrders(user, "desc", null);
            var userInProgressOrders = userOrders.Where(order => order.Status == OrderStatus.InProgress).ToList();
            return userInProgressOrders;
        }
        private async Task<Order> CreateOrder(User user)
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
        public async Task<ChartItem> AddToChart(User user, AddToChartRequest request)
        {
            var product = await _productService.GetProductById(request.ProductId.Value);
            Order order = new Order();
            try
            {
                order = await GetOpenOrder(user);
            }
            catch (RegisterNotFoundException)
            {
                order = null;
            }
            if (order == null)
                order = await CreateOrder(user);
            product = await _productService.UpdateProductQuantity(request.ProductId.Value, request.Quantity.Value);
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

        public async Task<ChartItem> RemoveFromChart(User user, int id)
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

        public async Task<ChartItem> ChangeItemQuantity(User user, int id, string sign)
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
        public async Task<Order> CancelOrder(User user)
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

        public async Task<Order> CheckoutOrder(User user, PaymentRequest payment)
        {
            Order order = await GetOpenOrder(user);
            if (order == null)
                throw new RegisterNotFoundException(ErrorUtils.GetMessage(ErrorType.CheckoutEmptyChart));
            order.Status = OrderStatus.InProgress;
            await _unitOfWork.Commit();
            await ProcessPurchase(order, payment);
            return order;
        }
        private async Task ProcessPurchase(Order order, PaymentRequest payment)
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
                // TODO: Fix this part (disposing before persisting data in DB)
                using var unitOfWork = _unitOfWork;
                order.Status = OrderStatus.Finished;
                order.FinishingDate = DateTime.Now;
                _unitOfWork.OrderRepository.Update(order);
                _unitOfWork.Commit();
            });
        }
    }
}