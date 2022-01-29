using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backend.Dtos;
using Backend.Models;
using Backend.Models.Enums;
using Backend.Models.Exceptions;
using Backend.Repositories;
using Backend.Services;
using BackendTest.Mocks;
using Xunit;

namespace BackendTest.Services
{
    public class OrderServiceTests : IDisposable
    {
        private readonly OrderService _orderService;
        private readonly ApplicationContextMoq _context;
        public OrderServiceTests()
        {
            _context = new ApplicationContextMoq();
            DbSeedingMock.Seeding(_context);
            var unitOfWork = new UnitOfWork(_context);
            var productService = new ProductService(unitOfWork);
            _orderService = new OrderService(unitOfWork, productService);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
        }

        [Theory]
        [InlineData(20)]
        public async Task GetOrders_WhenCalledByCustomer_ReturnsCustomerOrders(int userId)
        {
            // Given
            User user = _context.Users.Find(userId);
            // When
            List<Order> userOrders = await _orderService.GetOrders(user, "asc", null);
            // Then
            Assert.True(Enumerable.SequenceEqual(userOrders, _context.Orders.Where(order => order.ClientId == userId).OrderBy(order => order.CreationDate)));
        }

        [Fact]
        public async Task GetOrders_WhenCalledByAdministrator_ReturnsAllOrders()
        {
            // Given
            User adminUser = _context.Users.FirstOrDefault(user => user.Role == "Administrator");
            // When
            List<Order> orders = await _orderService.GetOrders(adminUser, sort: null, queryPage: null);
            // Then
            Assert.Equal(_context.Orders.ToList(), orders);
        }

        [Theory]
        [InlineData(10, "asc")]
        [InlineData(20, "desc")]
        public async Task GetOrders_WithSorting_ReturnsUserOrdersOrderedByCreationDate(int userId, string sortBy)
        {
            // Given
            User user = _context.Users.Find(userId);
            // When
            List<Order> userOrders = await _orderService.GetOrders(user, sortBy, null);
            // Then
            if (sortBy == "asc")
                Assert.True(Enumerable.SequenceEqual(userOrders, _context.Orders.Where(order => order.ClientId == userId).OrderBy(order => order.CreationDate)));
            else
                Assert.True(Enumerable.SequenceEqual(userOrders, _context.Orders.Where(order => order.ClientId == userId).OrderByDescending(order => order.CreationDate)));
        }

        [Theory]
        [InlineData(10, 1)]
        [InlineData(30, 2)]
        public async Task GetOrders_WithPage_ReturnsTenSpecificUserOOrders(int userId, int page)
        {
            // Given
            User user = _context.Users.Find(userId);
            // When
            var userOrders = await _orderService.GetOrders(user, null, page);
            // Then
            Assert.Equal<List<Order>>(_context.Orders.Where(order => order.ClientId == userId).Skip(10 * (page - 1)).Take(10).ToList(), userOrders);
        }

        [Theory]
        [InlineData(30)]
        [InlineData(80)]
        public async Task GetOrderById_WhenCalledByCustomerWithOrderIdWhichBelongsToHim_ReturnsOrderWithSpecifiedId(int orderId)
        {
            // Given
            var userId = _context.Orders.Find(orderId).ClientId;
            var customerUser = _context.Users.Find(userId);
            // When
            Order order = await _orderService.GetOrderById(customerUser, orderId);
            // Then
            Assert.Equal(orderId, order.Id);
        }

        [Theory]
        [InlineData(30)]
        [InlineData(80)]
        public async Task GetOrderById_WhenCalledByAdministrator_ReturnsOrderWithSpecifiedId(int orderId)
        {
            // Given
            User adminUser = _context.Users.FirstOrDefault(user => user.Role == "Administrator");
            // When
            Order order = await _orderService.GetOrderById(adminUser, orderId);
            // Then
            Assert.Equal(orderId, order.Id);
        }

        [Theory]
        [InlineData(30)]
        [InlineData(80)]
        public async Task GetOrderById_WhenCalledByCustomerWithOrderIdWhichDoesNotBelongsToHim_ThrowsUnauthorizedAccessException(int otherCustomerOrderId)
        {
            // Given
            var userId = _context.Orders.Find(otherCustomerOrderId).ClientId;
            var customerUser = _context.Users.First(user => user.Id != userId && user.Role == "Customer");
            // When
            var act = async () => await _orderService.GetOrderById(customerUser, otherCustomerOrderId);
            // Then
            await Assert.ThrowsAsync<UnauthorizedAccessException>(act);
        }

        [Theory]
        [InlineData(20)]
        [InlineData(40)]
        public async Task GetOrderById_WhenCalledWithNonExistingOrderId_ThrowsRegisterNotFoundException(int userId)
        {
            // Given
            var user = _context.Users.Find(userId);
            var nonExistingOrderId = _context.Orders.Count() + 1;
            // When
            var act = async () => await _orderService.GetOrderById(user, nonExistingOrderId);
            // Then
            await Assert.ThrowsAsync<RegisterNotFoundException>(act);
        }

        [Fact]
        public async Task GetOpenOrder_WhenCalled_ReturnsUserOpenOrder()
        {
            // Given
            var user = new User()
            {
                Id = _context.Users.Count() + 1,
                Name = "User WithOpenOrder",
                Login = "user_withopenorder",
                Email = "userwithopenorder@mail.com",
                Birthday = DateTime.Now,
                Password = "UserWithOpenOrderPassword",
                Cpf = "UserWithOpenOrderCpf"
            };
            _context.Users.Add(user);
            _context.SaveChanges();
            var order = new Order()
            {
                Id = _context.Orders.Count() + 1,
                ClientId = user.Id,
                Status = OrderStatus.Open
            };
            _context.Orders.Add(order);
            _context.SaveChanges();
            // When
            var userOpenOrder = await _orderService.GetOpenOrder(user);
            // Then
            Assert.Equal<Order>(_context.Orders.FirstOrDefault(order => order.ClientId == user.Id && order.Status == OrderStatus.Open), userOpenOrder);
        }

        [Fact]
        public async Task GetOpenOrder_WhenCalledByUserWithoutOpenOrder_ThrowsRegisterNotFoundException()
        {
            // Given
            var user = new User()
            {
                Id = _context.Users.Count() + 1,
                Name = "User WithoutOpenOrder",
                Login = "user_withoutopenorder",
                Email = "userwithoutopenorder@mail.com",
                Birthday = DateTime.Now,
                Password = "UserWithoutOpenOrderPassword",
                Cpf = "UserWithoutOpenOrderCpf"
            };
            _context.Users.Add(user);
            _context.SaveChanges();

            // When
            var act = async () => await _orderService.GetOpenOrder(user);
            // Then
            await Assert.ThrowsAsync<RegisterNotFoundException>(act);
        }

        [Fact]
        public async Task GetInProgressOrder_WhenCalled_ReturnsUserInProgressOrders()
        {
            // Given
            var user = new User()
            {
                Id = _context.Users.Count() + 1,
                Name = "User WithInProgressOrders",
                Login = "user_withinprogressorders",
                Email = "userwithinprogressorders@mail.com",
                Birthday = DateTime.Now,
                Password = "UserWithInProgressOrdersPassword",
                Cpf = "UserWithInProgressOrdersCpf"
            };
            _context.Users.Add(user);
            _context.SaveChanges();

            var inProgressOrders = new List<Order>();
            for (int i = 0; i < 5; i++)
            {
                var order = new Order()
                {
                    Id = _context.Orders.Count() + i + 1,
                    ClientId = user.Id,
                    Status = OrderStatus.InProgress
                };
                _context.Orders.Add(order);
                inProgressOrders.Add(order);
            }
            _context.SaveChanges();
            // When
            var userInProgressOrders = await _orderService.GetInProgressOrders(user);
            // Then
            Assert.True(Enumerable.SequenceEqual(_context.Orders.Where(order => order.ClientId == user.Id && order.Status == OrderStatus.InProgress).OrderBy(order => order.CreationDate), userInProgressOrders.OrderBy(order => order.CreationDate)));
        }

        [Theory]
        [InlineData(30)]
        public async Task AddToChart_WithUserHavingAnOpenOrder_AddItemToChartInTheOpenOrder(int productId)
        {
            // Given
            var addToChartRequest = new AddToChartRequest()
            {
                ProductId = productId,
                Quantity = _context.Products.Find(productId).StockQuantity - 1,
            };

            var user = new User()
            {
                Id = _context.Users.Count() + 1,
                Name = "User AddItemsToChartWithOpenOrder",
                Login = "user_additemstochartwithopenorder",
                Email = "useradditemstochartwithopenorder@mail.com",
                Birthday = DateTime.Now,
                Password = "UserAddItemsToChartWithOpenOrderPassword",
                Cpf = "UserAddItemsToChartWithOpenOrderCpf"
            };
            _context.Users.Add(user);
            _context.SaveChanges();
            var order = new Order()
            {
                Id = _context.Orders.Count() + 1,
                ClientId = user.Id,
                Status = OrderStatus.Open
            };
            _context.Orders.Add(order);
            _context.SaveChanges();

            // When
            var item = await _orderService.AddToChart(user, addToChartRequest);
            // Then
            Assert.True(_context.Orders.Find(order.Id).Items.Contains(item));
        }

        [Theory]
        [InlineData(60)]
        public async Task AddToChart_WithUserNotHavingOpenOrder_CreatesAnOrderAndAddItemToIt(int productId)
        {
            // Given
            var addToChartRequest = new AddToChartRequest()
            {
                ProductId = productId,
                Quantity = _context.Products.Find(productId).StockQuantity - 1,
            };

            var user = new User()
            {
                Id = _context.Users.Count() + 1,
                Name = "User AddItemsToChartWithoutOpenOrder",
                Login = "user_additemstochartwithoutopenorder",
                Email = "useradditemstochartwithoutopenorder@mail.com",
                Birthday = DateTime.Now,
                Password = "UserAddItemsToChartWithoutOpenOrderPassword",
                Cpf = "UserAddItemsToChartWithoutOpenOrderCpf"
            };
            _context.Users.Add(user);
            _context.SaveChanges();

            // When
            var item = await _orderService.AddToChart(user, addToChartRequest);
            // Then
            Assert.True(_context.Orders.FirstOrDefault(order => order.Status == OrderStatus.Open && order.ClientId == user.Id).Items.Contains(item));
        }

        [Theory]
        [InlineData(45, 3)]
        public async Task AddToChart_WithUserHavingAnOpenOrderWithTheProductAlreadyInTheChart_IncreaseProductQuantityInTheOpenOrder(int productId, int itemQuantity)
        {
            // Given
            var product = _context.Products.Find(productId);
            var addToChartRequest = new AddToChartRequest()
            {
                ProductId = productId,
                Quantity = product.StockQuantity - 1,
            };

            var user = new User()
            {
                Id = _context.Users.Count() + 1,
                Name = "User AddItemsToChartWithOpenOrderWithItem",
                Login = "user_additemstochartwithopenorderwithitem",
                Email = "useradditemstochartwithopenorderwithitem@mail.com",
                Birthday = DateTime.Now,
                Password = "UserAddItemsToChartWithOpenOrderWithItemPassword",
                Cpf = "UserAddItemsToChartWithOpenOrderWithItemCpf"
            };
            _context.Users.Add(user);
            _context.SaveChanges();
            var order = new Order()
            {
                Id = _context.Orders.Count() + 1,
                ClientId = user.Id,
                Status = OrderStatus.Open,
                Items = new List<ChartItem>()
            };
            var item = new ChartItem()
            {
                Id = _context.Items.Count() + 1,
                ProductId = productId,
                Price = product.Price.Value,
                Quantity = itemQuantity,
            };
            order.Items.Add(item);
            _context.Orders.Add(order);
            _context.SaveChanges();

            // When
            await _orderService.AddToChart(user, addToChartRequest);
            // Then
            Assert.True(_context.Items.Find(item.Id).Quantity == itemQuantity + addToChartRequest.Quantity);
        }
    }
}