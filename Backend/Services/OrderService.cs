using System.Threading.Tasks;
using System;
using System.Linq;
using Backend.Data;
using Backend.Models;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Backend.Models.ViewModels;
using Backend.Interfaces;
using Backend.Models.Exceptions;
using Backend.Utils;
using Backend.Models.Enums;

namespace Backend.Services
{
    public class OrderService : IOrderService
    {
        private readonly ApplicationContext _applicationContext;
        private readonly IProductService _productService;
        public OrderService(ApplicationContext context, IProductService service)
        {
            _productService = service;
            _applicationContext = context;
            try
            {
                _applicationContext.Database.EnsureCreated();
            }
            catch (InvalidOperationException e)
            {
                System.Console.WriteLine(e.Message);
            }
        }

        public List<Order> GetOrders(User user, string sort, int? queryPage)
        {
            int perPage = 5;
            try
            {
                List<Order> orders = _applicationContext.Orders.Include(order => order.Client).Include(order => order.Items).ThenInclude(item => item.Product)
                .Where(order => order.Client.Id == user.Id).ToList();
                if (sort == "asc")
                    orders = orders.OrderBy(order => order.CreationDate).ToList();
                else if (sort == "desc")
                    orders = orders.OrderByDescending(order => order.CreationDate).ToList();
                if (queryPage == -1)
                    return orders;
                int page = queryPage.GetValueOrDefault(1) == 0 ? 1 : queryPage.GetValueOrDefault(1);
                orders = orders.Skip(perPage * (page - 1)).Take(perPage).ToList();
                return orders;
            }
            catch (InvalidOperationException)
            {
                throw;
            }
        }
        public Order GetOrderById(int id)
        {
            try
            {
                return _applicationContext.Orders.Include(order => order.Client).Include(order => order.Items).ThenInclude(item => item.Product)
                .FirstOrDefault(order => order.Id == id);
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
                return GetOrders(user, "desc", -1).FirstOrDefault(order =>
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
                return GetOrders(user, "desc", -1).Where(order =>
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
                _applicationContext.Orders.Add(order);
                _applicationContext.SaveChanges();
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
                _applicationContext.Update(order);
                _applicationContext.SaveChanges();
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
                        _applicationContext.Orders.Remove(order);
                    _productService.UpdateProductQuantity(id, -item.Quantity);
                    _applicationContext.SaveChanges();
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
                    _applicationContext.SaveChanges();
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
                _applicationContext.SaveChanges();
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
                _applicationContext.SaveChanges();
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
                    using var context = new ApplicationContext();
                    order.Status = OrderStatus.Finished;
                    order.FinishingDate = DateTime.Now;
                    context.Update(order);
                    context.SaveChanges();
                }
                catch (InvalidOperationException)
                {
                    throw;
                }
            });
        }
    }
}