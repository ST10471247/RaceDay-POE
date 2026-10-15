using FluentAssertions;
using Microsoft.Extensions.Configuration;
using RaceDay.API.DTOs;
using RaceDay.API.Helpers;
using RaceDay.API.Services;
using RaceDay.Tests.Helpers;

namespace RaceDay.Tests.Services
{
    public class AuthServiceTests
    {
        private readonly JwtHelper _jwtHelper;

        public AuthServiceTests()
        {
            var inMemorySettings = new Dictionary<string, string?>
            {
                { "Jwt:Key", "ThisIsMySecretKeyForRaceDayApplication2026!@#$%" },
                { "Jwt:Issuer", "RaceDayAPI" },
                { "Jwt:Audience", "RaceDayClients" },
                { "Jwt:ExpiryInMinutes", "60" }
            };

            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            _jwtHelper = new JwtHelper(configuration);
        }

        [Fact]
        public async Task RegisterAsync_WithNewEmail_ShouldReturnToken()
        {
            // Arrange
            var context = TestDbContextFactory.Create();
            var authService = new AuthService(context, _jwtHelper);

            var dto = new RegisterDto
            {
                Email = "newuser@test.com",
                Password = "Password123",
                FirstName = "New",
                LastName = "User",
                Role = "Participant"
            };

            // Act
            var result = await authService.RegisterAsync(dto);

            // Assert
            result.Should().NotBeNull();
            result!.Token.Should().NotBeNullOrEmpty();
            result.User.Email.Should().Be("newuser@test.com");
            result.User.Role.Should().Be("Participant");
        }

        [Fact]
        public async Task RegisterAsync_WithExistingEmail_ShouldReturnNull()
        {
            // Arrange
            var context = TestDbContextFactory.Create();
            TestDbContextFactory.SeedTestData(context);
            var authService = new AuthService(context, _jwtHelper);

            var dto = new RegisterDto
            {
                Email = "organiser@test.com",
                Password = "Password123",
                FirstName = "Duplicate",
                LastName = "User",
                Role = "Participant"
            };

            // Act
            var result = await authService.RegisterAsync(dto);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task LoginAsync_WithValidCredentials_ShouldReturnToken()
        {
            // Arrange
            var context = TestDbContextFactory.Create();
            TestDbContextFactory.SeedTestData(context);
            var authService = new AuthService(context, _jwtHelper);

            var dto = new LoginDto
            {
                Email = "organiser@test.com",
                Password = "Password123"
            };

            // Act
            var result = await authService.LoginAsync(dto);

            // Assert
            result.Should().NotBeNull();
            result!.Token.Should().NotBeNullOrEmpty();
            result.User.Email.Should().Be("organiser@test.com");
        }

        [Fact]
        public async Task LoginAsync_WithWrongPassword_ShouldReturnNull()
        {
            // Arrange
            var context = TestDbContextFactory.Create();
            TestDbContextFactory.SeedTestData(context);
            var authService = new AuthService(context, _jwtHelper);

            var dto = new LoginDto
            {
                Email = "organiser@test.com",
                Password = "WrongPassword"
            };

            // Act
            var result = await authService.LoginAsync(dto);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task LoginAsync_WithNonExistentEmail_ShouldReturnNull()
        {
            // Arrange
            var context = TestDbContextFactory.Create();
            var authService = new AuthService(context, _jwtHelper);

            var dto = new LoginDto
            {
                Email = "nonexistent@test.com",
                Password = "Password123"
            };

            // Act
            var result = await authService.LoginAsync(dto);

            // Assert
            result.Should().BeNull();
        }
    }
}
