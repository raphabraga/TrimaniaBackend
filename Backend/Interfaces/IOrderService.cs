using System.Collections.Generic;
using Backend.Models;
using Backend.Models.ViewModels;

namespace Backend.Interfaces
{
    public interface IOrderService
    {
        public abstract List<Order> GetOrders(User user, string sort, int? page);
        public abstract Order GetOrderById(int id);
        public abstract Order GetOpenOrder(User user);
        public abstract Order GetInProgressOrder(User user);
        public abstract Order CreateOrder(User user);
        public ChartItem AddToChart(Order order, int productId, int quantity);
        public abstract bool RemoveFromChart(Order order, int id);
        public abstract bool ChangeItemQuantity(Order order, int id, string sign);
        public abstract bool CancelOrder(Order order);
        public abstract bool CheckoutOrder(Order order, Payment payment);
        public abstract void ProcessPurchase(Order order, PaymentMethod payment);
    }
}
