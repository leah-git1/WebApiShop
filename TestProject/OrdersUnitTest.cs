using Entities;
using Moq;
using Repository;
using System;
using System.Threading.Tasks;
using Xunit;

namespace TestProject
{
    public class OrderUnitTest
    {
        private readonly Mock<IOrderRepository> _mockRepository;

        public OrderUnitTest()
        {
            _mockRepository = new Mock<IOrderRepository>();
        }

        [Fact]
        public async Task GetOrderById_ReturnsOrderWhenOrderExists()
        {
            // Arrange
            var order = new OrdersTbl { OrderId = 1, OrderDate = DateOnly.FromDateTime(DateTime.Now), OrderSum = 100.0, UserId = 1 };
            _mockRepository.Setup(repo => repo.getOrderById(1)).ReturnsAsync(order);

            // Act
            var result = await _mockRepository.Object.getOrderById(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.OrderId);
            Assert.Equal(order.OrderDate, result.OrderDate);
            Assert.Equal(order.OrderSum, result.OrderSum);
            Assert.Equal(order.UserId, result.UserId);
        }

        [Fact]
        public async Task GetOrderById_ReturnsNullWhenOrderDoesNotExist()
        {
            // Arrange
            _mockRepository.Setup(repo => repo.getOrderById(2)).ReturnsAsync((OrdersTbl)null);

            // Act
            var result = await _mockRepository.Object.getOrderById(2);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task AddOrder_ReturnsAddedOrder()
        {
            // Arrange
            var order = new OrdersTbl { OrderId = 3, OrderDate = DateOnly.FromDateTime(DateTime.Now), OrderSum = 200.0, UserId = 1 };
            _mockRepository.Setup(repo => repo.AddOrder(order)).ReturnsAsync(order);

            // Act
            var result = await _mockRepository.Object.AddOrder(order);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.OrderId);
            Assert.Equal(order.OrderDate, result.OrderDate);
            Assert.Equal(order.OrderSum, result.OrderSum);
            Assert.Equal(order.UserId, result.UserId);
        }
    }
}
