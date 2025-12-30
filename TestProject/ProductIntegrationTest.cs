using Entities;
using Microsoft.EntityFrameworkCore;
using Repository;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace TestProject
{
    public class ProductRepositoryIntegrationTests : IClassFixture<DatabaseFixture>
    {
        private readonly ProductRepository _productRepository;
        private readonly DatabaseFixture _fixture;

        public ProductRepositoryIntegrationTests(DatabaseFixture fixture)
        {
            _fixture = fixture;
            _productRepository = new ProductRepository(_fixture.Context);
        }

        private async Task<CategoriesTbl> CreateCategory(string categoryName)
        {
            var category = new CategoriesTbl { CategoryName = categoryName };
            await _fixture.Context.CategoriesTbls.AddAsync(category);
            await _fixture.Context.SaveChangesAsync();
            return category;
        }

        [Fact]
        public async Task GetProducts_ReturnsFilteredProductsFromDatabase()
        {
            // Arrange
            var category = await CreateCategory("Category1");
            var product1 = new ProductTbl { ProductName = "Product1", ProductPrice = 100, CategoryId = category.CategoryId };
            var product2 = new ProductTbl { ProductName = "Product2", ProductPrice = 200, CategoryId = category.CategoryId };

            await _fixture.Context.ProductTbls.AddRangeAsync(product1, product2);
            await _fixture.Context.SaveChangesAsync();

            // Act
            var result = await _productRepository.getProducts(new int?[] { category.CategoryId }, 100, 200, 1, 10);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.total);
            Assert.Equal(2, result.Item1.Count);
        }

        [Fact]
        public async Task GetProducts_ReturnsEmptyListWhenNoProductsMatchCriteria()
        {
            // Arrange
            var category = await CreateCategory("Category1");
            var product = new ProductTbl { ProductName = "Product1", ProductPrice = 100, CategoryId = category.CategoryId };
            await _fixture.Context.ProductTbls.AddAsync(product);
            await _fixture.Context.SaveChangesAsync();

            // Act
            var result = await _productRepository.getProducts(new int?[] { 2 }, 200, 300, 1, 10);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(0, result.total);
            Assert.Empty(result.Item1);
        }

        [Fact]
        public async Task GetProducts_ReturnsAllProductsWhenNoFiltersApplied()
        {
            // Arrange
            var category1 = await CreateCategory("Category1");
            var category2 = await CreateCategory("Category2");
            var product1 = new ProductTbl { ProductName = "Product1", ProductPrice = 100, CategoryId = category1.CategoryId };
            var product2 = new ProductTbl { ProductName = "Product2", ProductPrice = 200, CategoryId = category2.CategoryId };

            await _fixture.Context.ProductTbls.AddRangeAsync(product1, product2);
            await _fixture.Context.SaveChangesAsync();

            // Act
            var result = await _productRepository.getProducts(new int?[] {}, null, null, 1, 10);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.total);
            Assert.Equal(2, result.Item1.Count);
        }
    }
}