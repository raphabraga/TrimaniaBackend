using System;
using System.Collections.Generic;
using System.Linq;
using Backend.Data;
using Backend.Interfaces;
using Backend.Models;
using Backend.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services
{
    public class SalesReportService : ISalesReportService
    {
        private readonly ApplicationContext _applicationContext;

        public SalesReportService(ApplicationContext context)
        {
            _applicationContext = context;
            try
            {
                _applicationContext.Database.EnsureCreated();
            }
            catch (InvalidOperationException e)
            {
                System.Console.WriteLine(e.Message);
            }
        }
        public SalesReport GenerateReport(DateTime startDate, DateTime endDate,
        List<string> userFilter, List<OrderStatus> statusFilter)
        {
            try
            {
                endDate = endDate == DateTime.MinValue ? DateTime.MaxValue : endDate;
                List<Order> orders = _applicationContext.Orders.Include(order => order.Client).
                Include(order => order.Items).ThenInclude(item => item.Product).Where
                (order => order.CreationDate > startDate && order.CreationDate < endDate).ToList();
                List<Order> OrdersFiltered = new List<Order>();
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