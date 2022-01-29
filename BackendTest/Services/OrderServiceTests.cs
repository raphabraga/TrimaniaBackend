using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backend.Dtos;
using Backend.Models;
using Backend.Models.Enums;
using Backend.Models.Exceptions;
using BackendTest.Fixtures;
using Xunit;

namespace BackendTest.Services
{
    public class OrderServiceTests : IClassFixture<OrderFixture>
    {
        private readonly OrderFixture _fixture;
        public OrderServiceTests(OrderFixture fixture)
        {
            _fixture = fixture;
        }

        [Theory]
        [InlineData(20)]
        public async Task GetOrders_WhenCalledByCustomer_ReturnsCustomerOrders(int userId)
        {
            // Given
            User user = _fixture.Context.Users.Find(userId);
            // When
            List<Order> userOrders = await _fixture.OrderService.GetOrders(user, "asc", null);
            // Then
            Assert.True(Enumerable.SequenceEqual(userOrders, _fixture.Context.Orders.Where(order => order.ClientId == userId).OrderBy(order => order.CreationDate)));
        }

        [Fact]
        public async Task GetOrders_WhenCalledByAdministrator_ReturnsAllOrders()
        {
            // Given
            User adminUser = _fixture.Context.Users.FirstOrDefault(user => user.Role == "Administrator");
            // When
            List<Order> orders = await _fixture.OrderService.GetOrders(adminUser, sort: null, queryPage: null);
            // Then
            Assert.Equal(_fixture.Context.Orders.ToList(), orders);
        }

        [Theory]
        [InlineData(10, "asc")]
        [InlineData(20, "desc")]
        public async Task GetOrders_WithSorting_ReturnsUserOrdersOrderedByCreationDate(int userId, string sortBy)
        {
            // Given
            User user = _fixture.Context.Users.Find(userId);
            // When
            List<Order> userOrders = await _fixture.OrderService.GetOrders(user, sortBy, null);
            // Then
            if (sortBy == "asc")
                Assert.True(Enumerable.SequenceEqual(userOrders, _fixture.Context.Orders.Where(order => order.ClientId == userId).OrderBy(order => order.CreationDate)));
            else
                Assert.True(Enumerable.SequenceEqual(userOrders, _fixture.Context.Orders.Where(order => order.ClientId == userId).OrderByDescending(order => order.CreationDate)));
        }

        [Theory]
        [InlineData(10, 1)]
        [InlineData(30, 2)]
        public async Task GetOrders_WithPage_ReturnsTenSpecificUserOOrders(int userId, int page)
        {
            // Given
            User user = _fixture.Context.Users.Find(userId);
            // When
            var userOrders = await _fixture.OrderService.GetOrders(user, null, page);
            // Then
            Assert.Equal<List<Order>>(_fixture.Context.Orders.Where(order => order.ClientId == userId).Skip(10 * (page - 1)).Take(10).ToList(), userOrders);
        }

        [Theory]
        [InlineData(30)]
        [InlineData(80)]
        public async Task GetOrderById_WhenCalledByCustomerWithOrderIdWhichBelongsToHim_ReturnsOrderWithSpecifiedId(int orderId)
        {
            // Given
            var userId = _fixture.Context.Orders.Find(orderId).ClientId;
            var customerUser = _fixture.Context.Users.Find(userId);
            // When
            Order order = await _fixture.OrderService.GetOrderById(customerUser, orderId);
            // Then
            Assert.Equal(orderId, order.Id);
        }

        [Theory]
        [InlineData(30)]
        [InlineData(80)]
        public async Task GetOrderById_WhenCalledByAdministrator_ReturnsOrderWithSpecifiedId(int orderId)
        {
            // Given
            User adminUser = _fixture.Context.Users.FirstOrDefault(user => user.Role == "Administrator");
            // When
            Order order = await _fixture.OrderService.GetOrderById(adminUser, orderId);
            // Then
            Assert.Equal(orderId, order.Id);
        }

        [Theory]
        [InlineData(30)]
        [InlineData(80)]
        public async Task GetOrderById_WhenCalledByCustomerWithOrderIdWhichDoesNotBelongsToHim_ThrowsUnauthorizedAccessException(int otherCustomerOrderId)
        {
            // Given
            var userId = _fixture.Context.Orders.Find(otherCustomerOrderId).ClientId;
            var customerUser = _fixture.Context.Users.First(user => user.Id != userId && user.Role == "Customer");
            // When
            var act = async () => await _fixture.OrderService.GetOrderById(customerUser, otherCustomerOrderId);
            // Then
            await Assert.ThrowsAsync<UnauthorizedAccessException>(act);
        }

        [Theory]
        [InlineData(20)]
        [InlineData(40)]
        public async Task GetOrderById_WhenCalledWithNonExistingOrderId_ThrowsRegisterNotFoundException(int userId)
        {
            // Given
            var user = _fixture.Context.Users.Find(userId);
            var nonExistingOrderId = _fixture.Context.Orders.Count() + 1;
            // When
            var act = async () => await _fixture.OrderService.GetOrderById(user, nonExistingOrderId);
            // Then
            await Assert.ThrowsAsync<RegisterNotFoundException>(act);
        }

        [Theory]
        [InlineData(32)]
        public async Task GetOpenOrder_WhenCalled_ReturnsUserOpenOrder(int userId)
        {
            // Given
            var user = _fixture.Context.Users.Find(userId);
            var order = new Order()
            {
                Id = _fixture.Context.Orders.Count() + 1,
                ClientId = user.Id,
                Status = OrderStatus.Open
            };
            _fixture.Context.Orders.Add(order);
            _fixture.Context.SaveChanges();
            // When
            var userOpenOrder = await _fixture.OrderService.GetOpenOrder(user);
            // Then
            Assert.Equal<Order>(_fixture.Context.Orders.FirstOrDefault(order => order.ClientId == user.Id && order.Status == OrderStatus.Open), userOpenOrder);
        }

        [Theory]
        [InlineData(43)]
        public async Task GetOpenOrder_WhenCalledByUserWithoutOpenOrder_ThrowsRegisterNotFoundException(int userId)
        {
            // Given
            var user = _fixture.Context.Users.Find(userId);
            // When
            var act = async () => await _fixture.OrderService.GetOpenOrder(user);
            // Then
            await Assert.ThrowsAsync<RegisterNotFoundException>(act);
        }

        [Theory]
        [InlineData(26)]
        public async Task GetInProgressOrder_WhenCalled_ReturnsUserInProgressOrders(int userId)
        {
            // Given
            var user = _fixture.Context.Users.Find(userId);

            var inProgressOrders = new List<Order>();
            for (int i = 0; i < 5; i++)
            {
                var order = new Order()
                {
                    Id = _fixture.Context.Orders.Count() + i + 1,
                    ClientId = user.Id,
                    Status = OrderStatus.InProgress
                };
                _fixture.Context.Orders.Add(order);
                inProgressOrders.Add(order);
            }
            _fixture.Context.SaveChanges();
            // When
            var userInProgressOrders = await _fixture.OrderService.GetInProgressOrders(user);
            // Then
            Assert.True(Enumerable.SequenceEqual(_fixture.Context.Orders.Where(order => order.ClientId == user.Id && order.Status == OrderStatus.InProgress).OrderBy(order => order.CreationDate), userInProgressOrders.OrderBy(order => order.CreationDate)));
        }

        [Theory]
        [InlineData(46, 30)]
        public async Task AddToChart_WithUserHavingAnOpenOrder_AddItemToChartInTheOpenOrder(int userId, int productId)
        {
            // Given
            var addToChartRequest = new AddToChartRequest()
            {
                ProductId = productId,
                Quantity = _fixture.Context.Products.Find(productId).StockQuantity - 1,
            };

            var user = _fixture.Context.Users.Find(userId);

            var order = new Order()
            {
                Id = _fixture.Context.Orders.Count() + 1,
                ClientId = user.Id,
                Status = OrderStatus.Open
            };
            _fixture.Context.Orders.Add(order);
            _fixture.Context.SaveChanges();

            // When
            var item = await _fixture.OrderService.AddToChart(user, addToChartRequest);
            // Then
            Assert.True(_fixture.Context.Orders.Find(order.Id).Items.Contains(item));
        }

        [Theory]
        [InlineData(18, 60)]
        public async Task AddToChart_WithUserNotHavingOpenOrder_CreatesAnOrderAndAddItemToIt(int userId, int productId)
        {
            // Given
            var addToChartRequest = new AddToChartRequest()
            {
                ProductId = productId,
                Quantity = _fixture.Context.Products.Find(productId).StockQuantity - 1,
            };

            var user = _fixture.Context.Users.Find(userId);
            // When
            var item = await _fixture.OrderService.AddToChart(user, addToChartRequest);
            // Then
            Assert.True(_fixture.Context.Orders.FirstOrDefault(order => order.Status == OrderStatus.Open && order.ClientId == user.Id).Items.Contains(item));
        }

        [Theory]
        [InlineData(6, 45, 3)]
        public async Task AddToChart_WithUserHavingAnOpenOrderWithTheProductAlreadyInTheChart_IncreaseProductQuantityInTheOpenOrder(int userId, int productId, int itemQuantity)
        {
            // Given
            var product = _fixture.Context.Products.Find(productId);
            var addToChartRequest = new AddToChartRequest()
            {
                ProductId = productId,
                Quantity = product.StockQuantity - 1,
            };

            var user = _fixture.Context.Users.Find(userId);
            var order = new Order()
            {
                Id = _fixture.Context.Orders.Count() + 1,
                ClientId = user.Id,
                Status = OrderStatus.Open,
                Items = new List<ChartItem>()
            };
            var item = new ChartItem()
            {
                Id = _fixture.Context.Items.Count() + 1,
                ProductId = productId,
                Price = product.Price.Value,
                Quantity = itemQuantity,
            };
            order.Items.Add(item);
            _fixture.Context.Orders.Add(order);
            _fixture.Context.SaveChanges();

            // When
            await _fixture.OrderService.AddToChart(user, addToChartRequest);
            // Then
            Assert.True(_fixture.Context.Items.Find(item.Id).Quantity == itemQuantity + addToChartRequest.Quantity);
        }

        [Theory]
        [InlineData(11, 75, 10)]
        public async Task RemoveFromChart_WithOthersItemsInTheChart_RemoveItemFromChartAndLetOrderOpen(int userId, int productId, int itemQuantity)
        {
            // Given
            var product = _fixture.Context.Products.Find(productId);

            var user = _fixture.Context.Users.Find(userId);
            var order = new Order()
            {
                Id = _fixture.Context.Orders.Count() + 1,
                ClientId = user.Id,
                Status = OrderStatus.Open,
                Items = new List<ChartItem>()
            };
            var firstItem = new ChartItem()
            {
                Id = _fixture.Context.Items.Count() + 1,
                ProductId = productId,
                Price = product.Price.Value,
                Quantity = itemQuantity,
            };
            order.Items.Add(firstItem);
            var secondItem = new ChartItem()
            {
                Id = _fixture.Context.Items.Count() + 2,
                ProductId = productId + 1,
                Price = product.Price.Value,
                Quantity = 1
            };
            order.Items.Add(secondItem);
            _fixture.Context.Orders.Add(order);
            _fixture.Context.SaveChanges();
            // When
            await _fixture.OrderService.RemoveFromChart(user, productId);
            // Then
            Assert.True(!_fixture.Context.Orders.Find(order.Id).Items.Contains(firstItem));
        }

        [Theory]
        [InlineData(29, 75, 10)]
        public async Task RemoveFromChart_WithoutItemsInTheChart_RemoveOrderFromDb(int userId, int productId, int itemQuantity)
        {
            // Given
            var product = _fixture.Context.Products.Find(productId);

            var user = _fixture.Context.Users.Find(userId);

            var order = new Order()
            {
                Id = _fixture.Context.Orders.Count() + 1,
                ClientId = user.Id,
                Status = OrderStatus.Open,
                Items = new List<ChartItem>()
            };
            var item = new ChartItem()
            {
                Id = _fixture.Context.Items.Count() + 1,
                ProductId = productId,
                Price = product.Price.Value,
                Quantity = itemQuantity,
            };
            order.Items.Add(item);
            _fixture.Context.Orders.Add(order);
            _fixture.Context.SaveChanges();
            // When
            await _fixture.OrderService.RemoveFromChart(user, productId);
            // Then
            Assert.True(_fixture.Context.Orders.Find(order.Id) == null);
        }

        [Theory]
        [InlineData(9)]
        public async Task RemoveFromChart_PassingNonExistingChartItemId_ThrowsRegisterNotFoundException(int userId)
        {
            // Given
            var user = _fixture.Context.Users.Find(userId);
            var order = new Order()
            {
                Id = _fixture.Context.Orders.Count() + 1,
                ClientId = user.Id,
                Status = OrderStatus.Open,
            };
            _fixture.Context.Orders.Add(order);
            _fixture.Context.SaveChanges();
            // When
            var act = async () => await _fixture.OrderService.RemoveFromChart(user, _fixture.Context.Products.Count() + 1);
            // Then
            await Assert.ThrowsAsync<RegisterNotFoundException>(act);
        }

        [Theory]
        [InlineData(19, "Increase", 55, 5)]
        [InlineData(41, "Decrease", 65, 3)]
        public async Task ChangeItemQuantity_WithSign_UpdateChartItemAccordingly(int userId, string sign, int productId, int itemQuantity)
        {
            // Given
            var product = _fixture.Context.Products.Find(productId);

            var user = _fixture.Context.Users.Find(userId);

            var order = new Order()
            {
                Id = _fixture.Context.Orders.Count() + 1,
                ClientId = user.Id,
                Status = OrderStatus.Open,
                TotalValue = itemQuantity * product.Price.Value,
                Items = new List<ChartItem>()
            };
            var item = new ChartItem()
            {
                Id = _fixture.Context.Items.Count() + 1,
                ProductId = productId,
                Price = product.Price.Value,
                Quantity = itemQuantity,
            };
            int itemQuantityBefore = item.Quantity;
            int productQuantityBefore = product.StockQuantity.Value;
            decimal totalValueBefore = order.TotalValue;
            order.Items.Add(item);

            _fixture.Context.Orders.Add(order);
            _fixture.Context.SaveChanges();

            // When
            int itemChange;
            if (sign == "Increase")
                itemChange = 1;
            else
                itemChange = -1;
            await _fixture.OrderService.ChangeItemQuantity(user, productId, sign);
            // Then
            var changedItem = _fixture.Context.Items.Find(item.Id);
            Assert.True(changedItem.Quantity == itemQuantityBefore + itemChange && product.StockQuantity == productQuantityBefore - itemChange && order.TotalValue == totalValueBefore + itemChange * product.Price);
        }

        [Theory]
        [InlineData(17, "increase")]
        [InlineData(31, "decrease")]
        public async Task ChangeItemQuantity_WithNonExistingProductId_ThrowsRegisterNotFoundException(int userId, string sign)
        {
            // Given
            var user = _fixture.Context.Users.Find(userId);
            var order = new Order()
            {
                Id = _fixture.Context.Orders.Count() + 1,
                ClientId = user.Id,
                Status = OrderStatus.Open,
            };
            _fixture.Context.Orders.Add(order);
            _fixture.Context.SaveChanges();

            var nonExistingProductId = _fixture.Context.Products.OrderByDescending(product => product.Id).First().Id + 1;
            // When
            var act = async () => await _fixture.OrderService.ChangeItemQuantity(user, nonExistingProductId, sign);
            // Then
            await Assert.ThrowsAsync<RegisterNotFoundException>(act);
        }

        [Theory]
        [InlineData(3, 40, 85)]
        public async Task ChangeItemQuantity_WithDecreaseSignAndOneItemInTheChart_RemoveItemFromChart(int userId, int firstProductId, int secondProductId)
        {
            // Given
            var firstProduct = _fixture.Context.Products.Find(firstProductId);
            var secondProduct = _fixture.Context.Products.Find(secondProductId);

            var user = _fixture.Context.Users.Find(userId);

            var order = new Order()
            {
                Id = _fixture.Context.Orders.Count() + 1,
                ClientId = user.Id,
                Status = OrderStatus.Open,
                Items = new List<ChartItem>()
            };
            var firstItem = new ChartItem()
            {
                Id = _fixture.Context.Items.Count() + 1,
                ProductId = firstProductId,
                Price = firstProduct.Price.Value,
                Quantity = 1,
            };
            order.Items.Add(firstItem);
            var secondItem = new ChartItem()
            {
                Id = _fixture.Context.Items.Count() + 2,
                ProductId = secondProductId,
                Price = secondProduct.Price.Value,
                Quantity = 1,
            };
            order.Items.Add(secondItem);

            _fixture.Context.Orders.Add(order);
            _fixture.Context.SaveChanges();

            // When
            await _fixture.OrderService.ChangeItemQuantity(user, firstProductId, "decrease");

            // Then
            Assert.True(!_fixture.Context.Orders.Find(order.Id).Items.Contains(firstItem));
        }

        [Theory]
        [InlineData(25, 30, 4, 70, 5)]
        public async Task CancelOrder_WhenCalled_ClosesOrderAccordingly(int userId, int firstProductId, int firstItemQuantity, int secondProductId, int secondItemQuantity)
        {
            // Given  
            var user = _fixture.Context.Users.Find(userId);
            var firstProduct = _fixture.Context.Products.Find(firstProductId);
            var secondProduct = _fixture.Context.Products.Find(secondProductId);

            var order = new Order()
            {
                Id = _fixture.Context.Orders.Count() + 1,
                ClientId = userId,
                Status = OrderStatus.Open,
                CreationDate = DateTime.Now,
                Items = new List<ChartItem>()
            };
            var firstItem = new ChartItem()
            {
                Id = _fixture.Context.Items.Count() + 1,
                ProductId = firstProductId,
                Price = firstProduct.Price.Value,
                Quantity = firstItemQuantity,
            };
            order.Items.Add(firstItem);
            var secondItem = new ChartItem()
            {
                Id = _fixture.Context.Items.Count() + 2,
                ProductId = secondProductId,
                Price = secondProduct.Price.Value,
                Quantity = secondItemQuantity,
            };
            order.Items.Add(secondItem);

            _fixture.Context.Orders.Add(order);
            _fixture.Context.SaveChanges();

            var quantityFirstProductBefore = firstProduct.StockQuantity;
            var quantitySecondProductBefore = secondProduct.StockQuantity;

            // When
            await _fixture.OrderService.CancelOrder(user);
            // Then
            var cancelledOrder = _fixture.Context.Orders.Find(order.Id);

            Assert.True(cancelledOrder.CancellationDate > cancelledOrder.CreationDate &&
            order.Status == OrderStatus.Cancelled &&
            firstProduct.StockQuantity == quantityFirstProductBefore + firstItemQuantity &&
            secondProduct.StockQuantity == quantitySecondProductBefore + secondItemQuantity);
        }

        [Theory]
        [InlineData(15, PaymentMethod.InCash)]
        [InlineData(25, PaymentMethod.BankSlip)]
        [InlineData(35, PaymentMethod.CreditCard)]
        public async Task CheckoutOrder_WhenCalled_FinishesOrderAccordingly(int userId, PaymentMethod paymentMethod)
        {
            // Given  
            var user = _fixture.Context.Users.Find(userId);

            var order = new Order()
            {
                Id = _fixture.Context.Orders.Count() + 1,
                ClientId = userId,
                Status = OrderStatus.Open,
                CreationDate = DateTime.Now,
            };
            _fixture.Context.Orders.Add(order);
            _fixture.Context.SaveChanges();


            var payment = new PaymentRequest()
            {
                PaymentMethod = paymentMethod
            };

            // When
            await _fixture.OrderService.CheckoutOrder(user, payment);

            // Then
            var finishedOrder = _fixture.Context.Orders.Find(order.Id);
            Assert.True(finishedOrder.FinishingDate > finishedOrder.CreationDate &&
            order.Status == OrderStatus.Finished);
        }
    }
}