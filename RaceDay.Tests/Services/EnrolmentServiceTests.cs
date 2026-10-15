using FluentAssertions;
using RaceDay.API.DTOs;
using RaceDay.API.Services;
using RaceDay.Tests.Helpers;

namespace RaceDay.Tests.Services
{
    public class EnrolmentServiceTests
    {
        [Fact]
        public async Task EnrolAsync_WithValidData_ShouldCreateEnrolment()
        {
            // Arrange
            var context = TestDbContextFactory.Create();
            TestDbContextFactory.SeedTestData(context);
            var service = new EnrolmentService(context);

            var dto = new CreateEnrolmentDto { CategoryId = 1 };

            // Act
            var result = await service.EnrolAsync(2, 1, dto);

            // Assert
            result.Should().NotBeNull();
            result!.ParticipantID.Should().Be(2);
            result.EventID.Should().Be(1);
            result.CategoryID.Should().Be(1);
            result.Status.Should().Be("Active");
        }

        [Fact]
        public async Task EnrolAsync_WithDuplicateEnrolment_ShouldReturnNull()
        {
            // Arrange
            var context = TestDbContextFactory.Create();
            TestDbContextFactory.SeedTestData(context);
            var service = new EnrolmentService(context);

            var dto = new CreateEnrolmentDto { CategoryId = 1 };

            // Act
            await service.EnrolAsync(2, 1, dto);
            var result = await service.EnrolAsync(2, 1, dto);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task EnrolAsync_WithInvalidEvent_ShouldReturnNull()
        {
            // Arrange
            var context = TestDbContextFactory.Create();
            TestDbContextFactory.SeedTestData(context);
            var service = new EnrolmentService(context);

            var dto = new CreateEnrolmentDto { CategoryId = 1 };

            // Act
            var result = await service.EnrolAsync(2, 999, dto);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task CancelEnrolmentAsync_WithValidParticipant_ShouldCancel()
        {
            // Arrange
            var context = TestDbContextFactory.Create();
            TestDbContextFactory.SeedTestData(context);
            var service = new EnrolmentService(context);

            var dto = new CreateEnrolmentDto { CategoryId = 1 };
            var enrolment = await service.EnrolAsync(2, 1, dto);

            // Act
            var result = await service.CancelEnrolmentAsync(
                enrolment!.EnrolmentID, 2, "Participant");

            // Assert
            result.Should().NotBeNull();
            result!.Status.Should().Be("Cancelled");
        }
    }
}