using System.Threading.Tasks;
using System;
using System.Linq;
using Backend.Data;
using Backend.Models;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Backend.Models.ViewModels;
using Backend.Interfaces;

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
            _applicationContext.Database.EnsureCreated();
        }

        public List<Order> GetOrders(User user, string sort, int? queryPage)
        {
            int perPage = 5;
            List<Order> orders = _applicationContext.Orders.Include(order => order.Items).ThenInclude(item => item.Product)
            .Where(order => order.Client.Id == user.Id).ToList();
            if (sort == "asc")
                orders = orders.OrderBy(order => order.CreationDate).ToList();
            else if (sort == "des")
                orders = orders.OrderByDescending(order => order.CreationDate).ToList();
            if (queryPage == -1)
                return orders;
            int page = queryPage.GetValueOrDefault(1) == 0 ? 1 : queryPage.GetValueOrDefault(1);
            orders = orders.Skip(perPage * (page - 1)).Take(perPage).ToList();
            return orders;
        }
        public Order GetOrderById(int id)
        {
            return _applicationContext.Orders.Include(order => order.Items).ThenInclude(item => item.Product)
            .FirstOrDefault(order => order.Id == id);
        }
        public Order GetOpenOrder(User user)
        {
            return GetOrders(user, "des", -1).FirstOrDefault(order =>
            order.Status == OrderStatus.Open);
        }
        public Order GetInProgressOrder(User user)
        {
            return GetOrders(user, "des", -1).FirstOrDefault(order =>
            order.Status == OrderStatus.InProgress);
        }
        public Order CreateOrder(User user)
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
        public ChartItem AddToChart(Order order, int productId, int quantity)
        {
            Product product = _productService.UpdateProductQuantity(productId, quantity);
            if (product == null)
                return null;
            order.TotalValue += product.Price.Value * quantity;
            ChartItem item = order.Items.FirstOrDefault(item => item?.Product?.Id == productId);
            if (item == null)
            {
                item = new ChartItem
                {
                    Product = product,
                    Price = product.Price.Value,
                    Quantity = quantity
                };
                order.Items.Add(item);
            }
            else
                item.Quantity += quantity;
            _applicationContext.Update(order);
            _applicationContext.SaveChanges();
            return item;
        }

        public bool RemoveFromChart(Order order, int id)
        {
            ChartItem item = order.Items.FirstOrDefault(item => item.Product.Id == id);
            if (item == null)
                return false;
            else
            {
                order.Items.Remove(item);
                order.TotalValue -= item.Price * item.Quantity;
                _productService.UpdateProductQuantity(id, -item.Quantity);
                _applicationContext.SaveChanges();
                return true;
            }
        }

        public bool ChangeItemQuantity(Order order, int id, string sign)
        {
            ChartItem item = order.Items.FirstOrDefault(item => item.Product.Id == id);
            if (item == null)
                return false;
            else
            {
                if (sign == "Increase")
                {
                    if (_productService.GetProductById(id).StockQuantity > 1)
                    {
                        item.Quantity++;
                        order.TotalValue += item.Price;
                        _productService.UpdateProductQuantity(id, 1);
                    }
                    else
                        return false;
                }
                else
                {
                    if (item.Quantity < 1)
                        return false;
                    if (item.Quantity == 1)
                        RemoveFromChart(order, id);
                    item.Quantity--;
                    order.TotalValue -= item.Price;
                    _productService.UpdateProductQuantity(id, -1);
                }
                _applicationContext.SaveChanges();
                return true;
            }
        }
        public bool CancelOrder(Order order)
        {
            order.Status = OrderStatus.Cancelled;
            order.CancellationDate = DateTime.Now;
            order.Items.ForEach(item =>
            {
                _productService.UpdateProductQuantity(item.Product.Id, -item.Quantity);
            });
            _applicationContext.SaveChanges();
            return true;
        }

        public bool CheckoutOrder(Order order, Payment payment)
        {
            order.Status = OrderStatus.InProgress;
            _applicationContext.SaveChanges();
            ProcessPurchase(order, payment.PaymentMethod.Value);
            return true;
        }

        public void ProcessPurchase(Order order, PaymentMethod payment)
        {
            int processingTime = 0;
            switch (payment)
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
                using var context = new ApplicationContext();
                order.Status = OrderStatus.Finished;
                order.FinishingDate = DateTime.Now;
                context.Update(order);
                context.SaveChanges();
            });
        }
    }
}