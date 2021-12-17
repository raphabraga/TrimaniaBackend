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

        public OrderService(ApplicationContext context)
        {
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
                (order.Status == OrderStatus.OPEN || order.Status == OrderStatus.IN_PROGRESS));
        }
        public Order GetInProgressOrder(User user)
        {
            return _applicationContext.Orders.Include(order => order.Items).FirstOrDefault(order => order.Client.Id == user.Id &&
                order.Status == OrderStatus.IN_PROGRESS);
        }
        public Order CreateOrder(User user)
        {
            if (GetOpenOrInProgressOrder(user) != null)
                return null;
            Order order = new Order()
            {
                Client = user,
                Status = OrderStatus.OPEN,
                CreationDate = DateTime.Now,
                Items = new List<Product>()
            };
            return order;
        }
        public Product AddToChart(User user, Product product)
        {
            Order order = CreateOrder(user);
            var isCreated = order == null;
            if (isCreated)
                order = GetOpenOrInProgressOrder(user);
            order.Items.Add(product);
            order.TotalValue += product.Price * product.Quantity;
            if (isCreated)
                _applicationContext.Update(order);
            else
                _applicationContext.Add(order);
            _applicationContext.SaveChanges();
            return product;
        }

        public bool RemoveFromChart(User user, int id)
        {
            Order order = GetOpenOrInProgressOrder(user);
            if (order == null)
                return false;
            Product product = order.Items.FirstOrDefault(item => item.Id == id);
            if (product == null)
                return false;
            else
            {
                order.Items.Remove(product);
                order.TotalValue -= product.Price * product.Quantity;
                _applicationContext.SaveChanges();
                return true;
            }
        }

        public bool IncreaseItemQuantity(User user, int id)
        {
            Order order = GetOpenOrInProgressOrder(user);
            if (order == null)
                return false;
            Product product = order.Items.FirstOrDefault(item => item.Id == id);
            if (product == null)
                return false;
            else
            {
                product.Quantity++;
                order.TotalValue += product.Price;
                _applicationContext.SaveChanges();
                return true;
            }
        }
        public bool DecreaseItemQuantity(User user, int id)
        {
            Order order = GetOpenOrInProgressOrder(user);
            if (order == null)
                return false;
            Product product = order.Items.FirstOrDefault(item => item.Id == id);
            if (product == null)
                return false;
            else
            {
                if (product.Quantity < 1)
                    return false;
                if (product.Quantity == 1)
                    RemoveFromChart(user, id);
                product.Quantity--;
                order.TotalValue -= product.Price;
                _applicationContext.SaveChanges();
                return true;
            }
        }

        public bool CancelOrder(User user)
        {
            Order order = GetOpenOrInProgressOrder(user);
            if (order == null)
                return false;
            order.Status = OrderStatus.CANCELLED;
            order.CancelDate = DateTime.Now;
            _applicationContext.SaveChanges();
            return true;
        }

        public bool CheckoutOrder(User user, Payment payment)
        {
            Order order = GetOpenOrInProgressOrder(user);
            if (order == null)
                return false;
            order.Status = OrderStatus.IN_PROGRESS;
            _applicationContext.SaveChanges();
            ProcessPurchase(payment.PaymentMethod, order);
            return true;
        }

        public void ProcessPurchase(PaymentMethod payment, Order order)
        {
            int processingTime = 0;
            switch (payment)
            {
                case PaymentMethod.IN_CASH:
                    processingTime = 0; // instant processing
                    break;
                case PaymentMethod.CREDIT_CARD:
                    processingTime = 5 * 1000; // 5s processing
                    break;
                case PaymentMethod.BANK_SLIP:
                    processingTime = 10 * 1000; // 10s processing
                    break;
            }
            Task.Delay(processingTime).Wait();
            order.Status = OrderStatus.COMPLETED;
            order.FinishedDate = DateTime.Now;
            _applicationContext.SaveChanges();
        }
    }
}