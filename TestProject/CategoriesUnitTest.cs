using Entities;
using Moq;
using Repository;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace TestProject
{
    public class CategoryUnitTest
    {
        private readonly Mock<ICategoryRepository> _mockRepository;

        public CategoryUnitTest()
        {
            _mockRepository = new Mock<ICategoryRepository>();
        }

        [Fact]
        public async Task GetCategories_ReturnsAllCategories()
        {
            // Arrange
            var categories = new List<CategoriesTbl>
            {
                new CategoriesTbl { CategoryId = 1, CategoryName = "Electronics" },
                new CategoriesTbl { CategoryId = 2, CategoryName = "Clothing" }
            };

            _mockRepository.Setup(repo => repo.getCategories()).ReturnsAsync(categories);

            // Act
            var result = await _mockRepository.Object.getCategories();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Contains(result, c => c.CategoryName == "Electronics");
        }
        [Fact]
        public async Task GetCategories_ReturnsEmptyListWhenNoCategoriesExist()
        {
            // Arrange
            var categories = new List<CategoriesTbl>(); // אין קטגוריות

            _mockRepository.Setup(repo => repo.getCategories()).ReturnsAsync(categories);

            // Act
            var result = await _mockRepository.Object.getCategories();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }
    }
}
