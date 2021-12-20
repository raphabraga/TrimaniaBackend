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
            public abstract Order GetOpenOrInProgressOrder(User user);
            public abstract Order GetInProgressOrder(User user);
            public abstract Order CreateOrder(User user);
            public abstract Product AddToChart(User user, Product product);
            public abstract bool RemoveFromChart(User user, int id);
            public abstract bool IncreaseItemQuantity(User user, int id);
            public abstract bool DecreaseItemQuantity(User user, int id);
            public abstract bool CancelOrder(User user);
            public abstract bool CheckoutOrder(User user, Payment payment);
            public abstract void ProcessPurchase(PaymentMethod payment, Order order);
        }
    }
}