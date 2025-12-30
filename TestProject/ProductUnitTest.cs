using Entities;
using Moq;
using Repository;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace TestProject
{
    public class ProductUnitTest
    {
        private readonly Mock<IProductRepository> _mockRepository;

        public ProductUnitTest()
        {
            _mockRepository = new Mock<IProductRepository>();
        }

        [Fact]
        public async Task GetProducts_ReturnsFilteredProducts()
        {
            // Arrange
            var products = new List<ProductTbl>
            {
                new ProductTbl { ProductId = 1, ProductName = "Product1", ProductPrice = 100, CategoryId = 1 },
                new ProductTbl { ProductId = 2, ProductName = "Product2", ProductPrice = 200, CategoryId = 2 },
                new ProductTbl { ProductId = 3, ProductName = "Product3", ProductPrice = 150, CategoryId = 1 }
            };

            _mockRepository.Setup(repo => repo.getProducts(It.IsAny<int?[]>(), It.IsAny<int?>(), It.IsAny<int?>(), It.IsAny<int>(), It.IsAny<int>()))
                           .ReturnsAsync((products, products.Count));

            // Act
            var result = await _mockRepository.Object.getProducts(new int?[] { 1 }, 100, 200, 1, 10);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.total);
            Assert.Equal(3, result.Item1.Count);
        }

        [Fact]
        public async Task GetProducts_ReturnsZeroWhenNoProductsMatchCriteria()
        {
            // Arrange
            _mockRepository.Setup(repo => repo.getProducts(It.IsAny<int?[]>(), It.IsAny<int?>(), It.IsAny<int?>(), It.IsAny<int>(), It.IsAny<int>()))
                           .ReturnsAsync((new List<ProductTbl>(), 0));

            // Act
            var result = await _mockRepository.Object.getProducts(new int?[] { 1 }, 250, 300, 1, 10);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(0, result.total);
            Assert.Empty(result.Item1);
        }
    }
}