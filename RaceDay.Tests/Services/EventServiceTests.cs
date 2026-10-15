using FluentAssertions;
using RaceDay.API.DTOs;
using RaceDay.API.Services;
using RaceDay.Tests.Helpers;

namespace RaceDay.Tests.Services
{
    public class EventServiceTests
    {
        [Fact]
        public async Task GetAllEventsAsync_ShouldReturnAllEvents()
        {
            // Arrange
            var context = TestDbContextFactory.Create();
            TestDbContextFactory.SeedTestData(context);
            var service = new EventService(context);

            // Act
            var result = await service.GetAllEventsAsync();

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(1);
            result[0].Name.Should().Be("Test Event");
        }

        [Fact]
        public async Task GetEventByIdAsync_WithValidId_ShouldReturnEvent()
        {
            // Arrange
            var context = TestDbContextFactory.Create();
            TestDbContextFactory.SeedTestData(context);
            var service = new EventService(context);

            // Act
            var result = await service.GetEventByIdAsync(1);

            // Assert
            result.Should().NotBeNull();
            result!.Name.Should().Be("Test Event");
            result.Location.Should().Be("Test Location");
        }

        [Fact]
        public async Task GetEventByIdAsync_WithInvalidId_ShouldReturnNull()
        {
            // Arrange
            var context = TestDbContextFactory.Create();
            var service = new EventService(context);

            // Act
            var result = await service.GetEventByIdAsync(999);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task CreateEventAsync_ShouldCreateNewEvent()
        {
            // Arrange
            var context = TestDbContextFactory.Create();
            TestDbContextFactory.SeedTestData(context);
            var service = new EventService(context);

            var dto = new CreateEventDto
            {
                Name = "New Test Event",
                Description = "A new event",
                EventDate = DateTime.Now.AddDays(60),
                Location = "New Location",
                MaxParticipants = 200
            };

            // Act
            var result = await service.CreateEventAsync(1, dto);

            // Assert
            result.Should().NotBeNull();
            result.Name.Should().Be("New Test Event");
            result.Location.Should().Be("New Location");
            result.Status.Should().Be("Active");
        }

        [Fact]
        public async Task UpdateEventAsync_WithValidOrganiser_ShouldUpdateEvent()
        {
            // Arrange
            var context = TestDbContextFactory.Create();
            TestDbContextFactory.SeedTestData(context);
            var service = new EventService(context);

            var dto = new UpdateEventDto { Name = "Updated Event Name" };

            // Act
            var result = await service.UpdateEventAsync(1, 1, dto);

            // Assert
            result.Should().NotBeNull();
            result!.Name.Should().Be("Updated Event Name");
        }

        [Fact]
        public async Task UpdateEventAsync_WithWrongOrganiser_ShouldReturnNull()
        {
            // Arrange
            var context = TestDbContextFactory.Create();
            TestDbContextFactory.SeedTestData(context);
            var service = new EventService(context);

            var dto = new UpdateEventDto { Name = "Hacked!" };

            // Act
            var result = await service.UpdateEventAsync(1, 999, dto);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task DeleteEventAsync_WithValidOrganiser_ShouldSoftDelete()
        {
            // Arrange
            var context = TestDbContextFactory.Create();
            TestDbContextFactory.SeedTestData(context);
            var service = new EventService(context);

            // Act
            var result = await service.DeleteEventAsync(1, 1);

            // Assert
            result.Should().BeTrue();
            var ev = await context.Events.FindAsync(1);
            ev!.Status.Should().Be("Cancelled");
        }
    }
}