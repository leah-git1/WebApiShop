using Entities;
using Microsoft.EntityFrameworkCore;
using Repository;
using System;
using System.Threading.Tasks;
using Xunit;

namespace TestProject
{
    public class OrderIntegrationTest : IClassFixture<DatabaseFixture>
    {
        private readonly OrderRepository _orderRepository;
        private readonly DatabaseFixture _fixture;

        public OrderIntegrationTest(DatabaseFixture fixture)
        {
            _fixture = fixture;
            _orderRepository = new OrderRepository(_fixture.Context);
        }

        [Fact]
        public async Task AddOrder_SavesOrderToDatabase()
        {
            // Arrange
            var user = new User
            {
                UserName = "TestUser@w",
                Password = "TestPassword", // שדה חובה
                                           // הוסף שדות נוספים אם יש 
            };

            _fixture.Context.Users.Add(user);
            await _fixture.Context.SaveChangesAsync(); // שמירה של המשתמש

            var order = new OrdersTbl
            {
                OrderDate = DateOnly.FromDateTime(DateTime.Now),
                OrderSum = 150.0,
                UserId = user.UserId
            };

            // Act
            var result = await _orderRepository.AddOrder(order);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(order.OrderSum, result.OrderSum);
            Assert.Equal(order.UserId, result.UserId);

            var savedOrder = await _fixture.Context.OrdersTbls.FirstOrDefaultAsync(x => x.OrderId == result.OrderId);
            Assert.NotNull(savedOrder);
            Assert.Equal(order.OrderSum, savedOrder.OrderSum);
            Assert.Equal(order.UserId, savedOrder.UserId);
        }

        [Fact]
        public async Task GetOrderById_ReturnsOrderWhenExists()
        {
            // Arrange
            var user = new User
            {
                UserName = "TestUser",
                Password = "TestPassword"
                // הוסף שדות נוספים אם יש
            };

            _fixture.Context.Users.Add(user);
            await _fixture.Context.SaveChangesAsync(); // שמור את המשתמש

            var order = new OrdersTbl
            {
                OrderDate = DateOnly.FromDateTime(DateTime.Now),
                OrderSum = 150.0,
                UserId = user.UserId // השתמש במזהה המשתמש שיצרת
            };

            await _orderRepository.AddOrder(order); // הוספת ההזמנה

            // Act
            var fetchedOrder = await _orderRepository.getOrderById(order.OrderId);

            // Assert
            Assert.NotNull(fetchedOrder);
            Assert.Equal(order.OrderId, fetchedOrder.OrderId);
            Assert.Equal(order.UserId, fetchedOrder.UserId);
        }

        [Fact]
        public async Task GetOrderById_ReturnsNullWhenOrderDoesNotExist()
        {
            // Act
            var result = await _orderRepository.getOrderById(99); // Assuming 99 does not exist

            // Assert
            Assert.Null(result);
        }
    }
}
