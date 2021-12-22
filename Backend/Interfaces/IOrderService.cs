using System.Collections.Generic;
using Backend.Models;
using Backend.Models.ViewModels;

namespace Backend.Interfaces
{
    public class IOrderService
    {
        public interface OrderService
        {
            public abstract List<Order> GetOrders(User user);
            public Order GetOrderById(int id);
            public abstract Order GetOpenOrder(User user);
            public abstract Order GetInProgressOrder(User user);
            public abstract Order CreateOrder(User user);
            public abstract Product AddToChart(Order order, int productId, int quantity);
            public abstract bool RemoveFromChart(Order order, int id);
            public abstract bool ChangeItemQuantity(Order order, int id, string sign);
            public abstract bool CancelOrder(Order order);
            public abstract bool CheckoutOrder(Order order, Payment payment);
            public abstract void ProcessPurchase(Order order, PaymentMethod payment);
        }
    }
}