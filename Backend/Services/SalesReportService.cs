using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Backend.ViewModels;
using Backend.Interfaces.Services;
using Backend.Interfaces.Repositories;
using Backend.Models;
using Backend.Models.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Threading.Tasks;
using Backend.Dtos;

namespace Backend.Services
{
    public class SalesReportService : ISalesReportService
    {
        private readonly IUnitOfWork _unitOfWork;
        public SalesReportService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<SalesReport> GenerateReport(User requestingUser, ReportRequest reportRequest)
        {
            // TODO: Improve this method
            try
            {
                if (reportRequest.StartDate == null)
                    reportRequest.StartDate = DateTime.MinValue;
                if (reportRequest.EndDate == null)
                    reportRequest.EndDate = DateTime.MaxValue;
                Expression<Func<Order, bool>> filter = (order => order.CreationDate > reportRequest.StartDate && order.CreationDate < reportRequest.EndDate);
                Func<IQueryable<Order>, IIncludableQueryable<Order, object>> includes =
                order => order.Include(order => order.Client).Include(order => order.Items).ThenInclude(item => item.Product);
                var iNumerableOrders = await _unitOfWork.OrderRepository.Get(filter, orderBy: null, includes, page: null);
                var orders = iNumerableOrders.ToList();
                List<Order> OrdersFiltered = new List<Order>();
                if (requestingUser.Role != "Administrator")
                    reportRequest.UserFilter = new List<string> { requestingUser.Login };
                if (reportRequest.UserFilter != null)
                {
                    reportRequest.UserFilter.ForEach(userLogin => OrdersFiltered = OrdersFiltered.Union(
                        orders.Where(order => order.Client.Login == userLogin).ToList()
                    ).ToList());
                    orders = OrdersFiltered;
                    OrdersFiltered = new List<Order>();
                }
                if (reportRequest.StatusFilter != null)
                {
                    reportRequest.StatusFilter.ForEach(status => OrdersFiltered = OrdersFiltered.Union(
                        orders.Where(order => order.Status == status).ToList()
                    ).ToList());
                    orders = OrdersFiltered;
                }
                int amountFinished = orders.Aggregate(0, (sum, order) =>
                order.Status == OrderStatus.Finished ? sum + 1 : sum);
                int amountCancelled = orders.Aggregate(0, (sum, order) =>
                order.Status == OrderStatus.Cancelled ? sum + 1 : sum);
                decimal ordersTotal = orders.Aggregate(0m, (sum, order) => sum + order.TotalValue);
                return new SalesReport
                {
                    FinishedOrdersAmount = amountFinished,
                    CancelledOrdersAmount = amountCancelled,
                    OrdersTotalValue = ordersTotal,
                    Orders = orders.Select(order => new ViewOrder(order)).ToList()
                };
            }
            catch (InvalidOperationException)
            {
                throw;
            }
        }
    }
}