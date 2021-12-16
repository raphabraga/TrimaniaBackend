using System.Threading.Tasks;
using System;
using System.Linq;
using Backend.Data;
using Backend.Models;

namespace Backend.Services
{
    public class OrderService
    {
        private readonly ApplicationContext _applicationContext;

        public OrderService(ApplicationContext context)
        {
            _applicationContext = context;
        }

        public Order GetOpenOrder(User user)
        {
            return _applicationContext.Orders.FirstOrDefault(order => order.Client == user &&
                order.Status == OrderStatus.OPEN);
        }
        public Order CreateOrder(User user)
        {
            if (GetOpenOrder(user) != null)
                return null;
            Order order = new Order()
            {
                Client = user,
                Status = OrderStatus.OPEN,
                CreationDate = DateTime.Now,
            };
            return order;
        }
        public Product AddToChart(User user, Product product)
        {
            Order order = CreateOrder(user);
            if (order == null)
                order = GetOpenOrder(user);
            order.Items.Add(product);
            order.TotalValue += product.Price * product.Quantity;
            return product;
        }

        public bool RemoveFromChart(User user, int id)
        {
            Order order = GetOpenOrder(user);
            if (order == null)
                return false;
            Product product = order.Items.FirstOrDefault(item => item.Id == id);
            if (product == null)
                return false;
            else
            {
                order.Items.Remove(product);
                order.TotalValue -= product.Price * product.Quantity;
                return true;
            }
        }

        public bool IncreaseItemQuantity(User user, int id)
        {
            Order order = GetOpenOrder(user);
            if (order == null)
                return false;
            Product product = order.Items.FirstOrDefault(item => item.Id == id);
            if (product == null)
                return false;
            else
            {
                product.Quantity++;
                order.TotalValue += product.Price;
                return true;
            }
        }
        public bool DecreaseItemQuantity(User user, int id)
        {
            Order order = GetOpenOrder(user);
            if (order == null)
                return false;
            Product product = order.Items.FirstOrDefault(item => item.Id == id);
            if (product == null)
                return false;
            else
            {
                if (product.Quantity < 1)
                    return false;
                product.Quantity--;
                order.TotalValue -= product.Price;
                return true;
            }
        }

        public bool CancelOrder(User user)
        {
            Order order = GetOpenOrder(user);
            if (order == null)
                return false;
            order.Status = OrderStatus.CANCELLED;
            return true;
        }

        public bool CheckoutOrder(User user, PaymentMethod payment)
        {
            Order order = GetOpenOrder(user);
            if (order == null)
                return false;
            order.Status = OrderStatus.IN_PROGRESS;
            ProcessPurchase(payment, order);
            return true;
        }

        public async void ProcessPurchase(PaymentMethod payment, Order order)
        {
            int processingTime = 0;
            switch (payment)
            {
                case PaymentMethod.IN_CASH:
                    processingTime = 0; // instant processing
                    break;
                case PaymentMethod.CREDIT_CARD:
                    processingTime = 1 * 60 * 1000; // 1min (in ms) processing
                    break;
                case PaymentMethod.BANK_SLIP:
                    processingTime = 10 * 60 * 1000; // 10min (in ms) processing
                    break;
            }

            await Task.Delay(processingTime).ContinueWith(_ =>
            {
                order.Status = OrderStatus.COMPLETED;
            });
        }
    }
}