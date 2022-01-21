using System.Collections.Generic;
using Backend.Dtos;
using Backend.Models;

namespace Backend.Interfaces.Services
{
    public interface IOrderService
    {
        public abstract List<Order> GetOrders(User user, string sort, int? page);
        public abstract Order GetOrderById(User requestingUser, int id);
        public abstract Order GetOpenOrder(User user);
        public abstract List<Order> GetInProgressOrders(User user);
        public abstract Order CreateOrder(User user);
        public ChartItem AddToChart(User user, AddToChartRequest request);
        public abstract ChartItem RemoveFromChart(User user, int id);
        public abstract ChartItem ChangeItemQuantity(User user, int id, string sign);
        public abstract Order CancelOrder(User user);
        public abstract Order CheckoutOrder(User user, PaymentRequest payment);
        public abstract void ProcessPurchase(Order order, PaymentRequest payment);
    }
}
