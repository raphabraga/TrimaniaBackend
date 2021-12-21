using System.Threading.Tasks;
using System;
using System.Linq;
using Backend.Data;
using Backend.Models;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Backend.Models.ViewModels;

namespace Backend.Services
{
    public class OrderService
    {
        private readonly ApplicationContext _applicationContext;
        private readonly ProductService _productService;

        public OrderService(ApplicationContext context, ProductService service)
        {
            _productService = service;
            _applicationContext = context;
            _applicationContext.Database.EnsureCreated();
        }

        public List<Order> GetOrders(User user)
        {
            return _applicationContext.Orders.Include(order => order.Items).Where(order => order.Client.Id == user.Id).ToList();
        }

        public Order GetOpenOrInProgressOrder(User user)
        {
            return _applicationContext.Orders.Include(order => order.Items)
            .FirstOrDefault(order => order.Client.Id == user.Id &&
                (order.Status == OrderStatus.Open || order.Status == OrderStatus.InProgress));
        }
        public Order GetInProgressOrder(User user)
        {
            return _applicationContext.Orders.Include(order => order.Items).FirstOrDefault(order => order.Client.Id == user.Id &&
                order.Status == OrderStatus.InProgress);
        }
        public Order CreateOrder(User user)
        {
            if (GetOpenOrInProgressOrder(user) != null)
                return null;
            Order order = new Order()
            {
                Client = user,
                Status = OrderStatus.Open,
                CreationDate = DateTime.Now,
                Items = new List<ChartItem>()
            };
            return order;
        }
        public ChartItem AddToChart(User user, int productId, int quantity)
        {
            Product product = _productService.UpdateProductQuantity(productId, quantity);
            if (product == null)
                return null;
            Order order = CreateOrder(user);
            var isCreated = (order == null);
            if (isCreated)
                order = GetOpenOrInProgressOrder(user);
            order.TotalValue += product.Price * quantity;
            ChartItem item = order.Items.FirstOrDefault(item => item.ProductId == productId);
            if (item == null)
            {
                item = new ChartItem
                {
                    ProductId = product.Id,
                    Price = product.Price,
                    Quantity = quantity
                };
                order.Items.Add(item);
            }
            item.Quantity += quantity;
            if (isCreated)
                _applicationContext.Update(order);
            else
                _applicationContext.Add(order);
            _applicationContext.SaveChanges();
            return item;
        }

        public bool RemoveFromChart(User user, int id)
        {
            Order order = GetOpenOrInProgressOrder(user);
            if (order == null)
                return false;
            ChartItem item = order.Items.FirstOrDefault(item => item.ProductId == id);
            if (item == null)
                return false;
            else
            {
                order.Items.Remove(item);
                order.TotalValue -= item.Price * item.Quantity;
                _applicationContext.SaveChanges();
                return true;
            }
        }

        public bool IncreaseItemQuantity(User user, int id)
        {
            Order order = GetOpenOrInProgressOrder(user);
            if (order == null)
                return false;
            ChartItem item = order.Items.FirstOrDefault(item => item.ProductId == id);
            if (item == null)
                return false;
            else
            {
                item.Quantity++;
                order.TotalValue += item.Price;
                _applicationContext.SaveChanges();
                return true;
            }
        }
        public bool DecreaseItemQuantity(User user, int id)
        {
            Order order = GetOpenOrInProgressOrder(user);
            if (order == null)
                return false;
            ChartItem item = order.Items.FirstOrDefault(item => item.ProductId == id);
            if (item == null)
                return false;
            else
            {
                if (item.Quantity < 1)
                    return false;
                if (item.Quantity == 1)
                    RemoveFromChart(user, id);
                item.Quantity--;
                order.TotalValue -= item.Price;
                _applicationContext.SaveChanges();
                return true;
            }
        }

        public bool CancelOrder(User user)
        {
            Order order = GetOpenOrInProgressOrder(user);
            if (order == null)
                return false;
            order.Status = OrderStatus.Cancelled;
            order.CancellationDate = DateTime.Now;
            _applicationContext.SaveChanges();
            return true;
        }

        public bool CheckoutOrder(User user, Payment payment)
        {
            Order order = GetOpenOrInProgressOrder(user);
            if (order == null)
                return false;
            order.Status = OrderStatus.InProgress;
            _applicationContext.SaveChanges();
            ProcessPurchase(payment.PaymentMethod, order);
            return true;
        }

        public void ProcessPurchase(PaymentMethod payment, Order order)
        {
            int processingTime = 0;
            switch (payment)
            {
                case PaymentMethod.InCash:
                    processingTime = 0; // instant processing
                    break;
                case PaymentMethod.CreditCard:
                    processingTime = 5 * 1000; // 5s processing
                    break;
                case PaymentMethod.BankSlip:
                    processingTime = 10 * 1000; // 10s processing
                    break;
            }
            Task.Delay(processingTime).Wait();
            order.Status = OrderStatus.Finished;
            order.FinishingDate = DateTime.Now;
            _applicationContext.SaveChanges();
        }
    }
}