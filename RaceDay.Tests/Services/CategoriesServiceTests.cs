using FluentAssertions;
using RaceDay.API.DTOs;
using RaceDay.API.Services;
using RaceDay.Tests.Helpers;

namespace RaceDay.Tests.Services
{
    public class CategoryServiceTests
    {
        [Fact]
        public async Task GetCategoriesByEventAsync_ShouldReturnCategories()
        {
            // Arrange
            var context = TestDbContextFactory.Create();
            TestDbContextFactory.SeedTestData(context);
            var service = new CategoryService(context);

            // Act
            var result = await service.GetCategoriesByEventAsync(1);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(1);
            result[0].Name.Should().Be("Test Category");
        }

        [Fact]
        public async Task CreateCategoryAsync_WithValidOrganiser_ShouldCreate()
        {
            // Arrange
            var context = TestDbContextFactory.Create();
            TestDbContextFactory.SeedTestData(context);
            var service = new CategoryService(context);

            var dto = new CreateCategoryDto
            {
                Name = "New Category",
                Distance = 21.1m,
                StartTime = TimeSpan.FromHours(8),
                MaxParticipants = 100,
                Fee = 200.00m
            };

            // Act
            var result = await service.CreateCategoryAsync(1, 1, dto);

            // Assert
            result.Should().NotBeNull();
            result!.Name.Should().Be("New Category");
            result.Distance.Should().Be(21.1m);
        }

        [Fact]
        public async Task DeleteCategoryAsync_WithValidOrganiser_ShouldDelete()
        {
            // Arrange
            var context = TestDbContextFactory.Create();
            TestDbContextFactory.SeedTestData(context);
            var service = new CategoryService(context);

            // Act
            var result = await service.DeleteCategoryAsync(1, 1);

            // Assert
            result.Should().BeTrue();
            var category = await context.Categories.FindAsync(1);
            category.Should().BeNull();
        }
    }
}