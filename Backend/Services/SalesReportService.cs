using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Backend.Data;
using Backend.Interfaces.Services;
using Backend.Interfaces.UnitOfWork;
using Backend.Models;
using Backend.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace Backend.Services
{
    public class SalesReportService : ISalesReportService
    {
        private readonly IUnitOfWork _unitOfWork;
        public SalesReportService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public SalesReport GenerateReport(User requestingUser, DateTime startDate, DateTime endDate,
        List<string> userFilter, List<OrderStatus> statusFilter)
        {
            // TODO: Improve this method
            try
            {
                endDate = endDate == DateTime.MinValue ? DateTime.MaxValue : endDate;
                Expression<Func<Order, bool>> filter = (order => order.CreationDate > startDate && order.CreationDate < endDate);
                Func<IQueryable<Order>, IIncludableQueryable<Order, object>> includes =
                order => order.Include(order => order.Client).Include(order => order.Items).ThenInclude(item => item.Product);
                List<Order> orders = _unitOfWork.OrderRepository.Get(filter, orderBy: null, includes, page: null).ToList();
                List<Order> OrdersFiltered = new List<Order>();
                if (requestingUser.Role != "Administrator")
                    userFilter = new List<string> { requestingUser.Login };
                if (userFilter != null)
                {
                    userFilter.ForEach(userLogin => OrdersFiltered = OrdersFiltered.Union(
                        orders.Where(order => order.Client.Login == userLogin).ToList()
                    ).ToList());
                    orders = OrdersFiltered;
                    OrdersFiltered = new List<Order>();
                }
                if (statusFilter != null)
                {
                    statusFilter.ForEach(status => OrdersFiltered = OrdersFiltered.Union(
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