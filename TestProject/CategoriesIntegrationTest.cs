using Entities;
using Microsoft.EntityFrameworkCore;
using Repository;
using System.Threading.Tasks;
using Xunit;

namespace TestProject
{
    public class CategoriesIntegrationTest : IClassFixture<DatabaseFixture>
    {
        private readonly CategoryRepository _categoryRepository;
        private readonly DatabaseFixture _fixture;

        public CategoriesIntegrationTest(DatabaseFixture fixture)
        {
            _fixture = fixture;
            _categoryRepository = new CategoryRepository(_fixture.Context);
        }

        [Fact]
        public async Task GetCategories_ReturnsAllCategories()
        {
            // Arrange
            _fixture.Context.CategoriesTbls.AddRange(
                new CategoriesTbl { CategoryName = "Electronics" },
                new CategoriesTbl { CategoryName = "Clothing" }
            );
            await _fixture.Context.SaveChangesAsync();

            // Act
            var categories = await _categoryRepository.getCategories();

            // Assert
            Assert.NotNull(categories);
            Assert.Equal(2, categories.Count);
            Assert.Contains(categories, c => c.CategoryName == "Electronics");
        }

        [Fact]
        public async Task GetCategories_ReturnsEmptyListWhenNoCategoriesExist()
        {
            // Act
            var categories = await _categoryRepository.getCategories();

            // Assert
            Assert.NotNull(categories);
            Assert.Empty(categories);
        }
    }
}