using System.Collections.Generic;
using System.Threading.Tasks;
using Backend.Dtos;
using Backend.Models;

namespace Backend.Interfaces.Services
{
    public interface IOrderService
    {
        public abstract Task<List<Order>> GetOrders(User user, string sort, int? page);
        public abstract Task<Order> GetOrderById(User requestingUser, int id);
        public abstract Task<Order> GetOpenOrder(User user);
        public abstract Task<List<Order>> GetInProgressOrders(User user);
        public abstract Task<ChartItem> AddToChart(User user, AddToChartRequest request);
        public abstract Task<ChartItem> RemoveFromChart(User user, int id);
        public abstract Task<ChartItem> ChangeItemQuantity(User user, int id, string sign);
        public abstract Task<Order> CancelOrder(User user);
        public abstract Task<Order> CheckoutOrder(User user, PaymentRequest payment);
    }
}
