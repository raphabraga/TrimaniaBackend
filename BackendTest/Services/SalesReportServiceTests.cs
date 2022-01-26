using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backend.Dtos;
using Backend.Models;
using Backend.Models.Enums;
using BackendTest.Fixtures;
using Xunit;

namespace BackendTest.Services
{
    public class SalesReportServiceTests : IClassFixture<UnitOfWorkFixture>
    {
        private readonly UnitOfWorkFixture _fixture;
        public SalesReportServiceTests(UnitOfWorkFixture fixture)
        {
            _fixture = fixture;
        }
        [Fact]
        public async Task GenerateReport_ByAdministratorAndWithNullReportRequest_ReturnsAllOrders()
        {
            // Given
            User user = new User()
            {
                Role = "Administrator"
            };

            ReportRequest reportRequest = new ReportRequest()
            {
                UserFilter = null,
                StatusFilter = null,
                StartDate = null,
                EndDate = null
            };
            // When
            var report = await _fixture.SalesReportService.GenerateReport(user, reportRequest);
            // Then
            Assert.Equal(_fixture.Context.Orders.ToList().Count, report.Orders.Count);
        }

        [Theory]
        [InlineData(10)]
        [InlineData(20)]
        [InlineData(40)]
        public async Task GenerateReport_ByCustomerAndWithNullReportRequest_ReturnsOnlyUserOrders(int userId)
        {
            // Given
            var num = _fixture.Context.Users.Count();
            User user = _fixture.Context.Users.Find(userId);

            ReportRequest reportRequest = new ReportRequest()
            {
                UserFilter = null,
                StatusFilter = null,
                StartDate = null,
                EndDate = null
            };
            // When
            var report = await _fixture.SalesReportService.GenerateReport(user, reportRequest);
            // Then
            Assert.Equal(_fixture.Context.Orders.Where(order => order.ClientId == user.Id).Count(), report.Orders.Count);
        }

        [Theory]
        [InlineData(OrderStatus.Open, OrderStatus.Finished)]
        [InlineData(OrderStatus.Cancelled, OrderStatus.InProgress)]
        [InlineData(OrderStatus.Finished, OrderStatus.Cancelled)]
        public async Task GenerateReport_WithStatusFilter_ReturnsOnlyOrdersWithSpecifiedStatus(OrderStatus firstStatus, OrderStatus secondStatus)
        {
            // Given
            User user = new User()
            {
                Role = "Administrator"
            };

            ReportRequest reportRequest = new ReportRequest()
            {
                UserFilter = null,
                StatusFilter = new List<OrderStatus>() {
                        firstStatus, secondStatus
                    },
                StartDate = null,
                EndDate = null
            };
            // When
            var report = await _fixture.SalesReportService.GenerateReport(user, reportRequest);
            // Then
            Assert.Equal(_fixture.Context.Orders.Where(order => (order.Status == firstStatus)
            || (order.Status == secondStatus)).Count(), report.Orders.Count);
        }

        [Theory]
        [InlineData(3, 10, 15)]
        [InlineData(2, 19, 13)]
        [InlineData(8, 28, 35)]
        public async Task GenerateReport_WithUserFilter_ReturnsOnlyOrdersOfSpecifiedUsers(int firstUserId, int secondUserId, int thirdUserId)
        {
            // Given
            User user = new User()
            {
                Role = "Administrator"
            };

            User firstUser = _fixture.Context.Users.Find(firstUserId);
            User secondUser = _fixture.Context.Users.Find(secondUserId);
            User thirdUser = _fixture.Context.Users.Find(thirdUserId);

            ReportRequest reportRequest = new ReportRequest()
            {
                UserFilter = new List<string> {
                    firstUser.Login,
                    secondUser.Login,
                    thirdUser.Login
                },
                StatusFilter = null,
                StartDate = null,
                EndDate = null
            };

            // When
            var report = await _fixture.SalesReportService.GenerateReport(user, reportRequest);
            // Then
            Assert.Equal(_fixture.Context.Orders.Where(order => (order.ClientId == firstUser.Id)
            || (order.ClientId == secondUser.Id) || (order.ClientId == thirdUser.Id)).Count(), report.Orders.Count);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(50)]
        [InlineData(100)]
        public async Task GenerateReport_WithStartDateFilter_ReturnsOnlyOrdersInLaterDates(int orderId)
        {
            // Given
            User user = new User()
            {
                Role = "Administrator"
            };


            ReportRequest reportRequest = new ReportRequest()
            {
                UserFilter = null,
                StatusFilter = null,
                StartDate = _fixture.Context.Orders.Find(orderId).CreationDate,
                EndDate = null
            };

            // When
            var report = await _fixture.SalesReportService.GenerateReport(user, reportRequest);
            // Then
            Assert.Equal(_fixture.Context.Orders.Where(order => order.CreationDate > reportRequest.StartDate
            ).Count(), report.Orders.Count);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(50)]
        [InlineData(100)]
        public async Task GenerateReport_WithEndDateFilter_ReturnsOnlyOrdersInSoonerDates(int orderId)
        {
            // Given
            User user = new User()
            {
                Role = "Administrator"
            };


            ReportRequest reportRequest = new ReportRequest()
            {
                UserFilter = null,
                StatusFilter = null,
                StartDate = null,
                EndDate = _fixture.Context.Orders.Find(orderId).CreationDate
            };

            // When
            var report = await _fixture.SalesReportService.GenerateReport(user, reportRequest);
            // Then
            Assert.Equal(_fixture.Context.Orders.Where(order => order.CreationDate < reportRequest.EndDate
            ).Count(), report.Orders.Count);
        }
    }
}