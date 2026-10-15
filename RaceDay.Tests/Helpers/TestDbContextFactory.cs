using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;
using RaceDay.API.Models;

namespace RaceDay.Tests.Helpers
{
    public static class TestDbContextFactory
    {
        public static RaceDayDbContext Create()
        {
            var options = new DbContextOptionsBuilder<RaceDayDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var context = new RaceDayDbContext(options);
            context.Database.EnsureCreated();
            return context;
        }

        public static void SeedTestData(RaceDayDbContext context)
        {
            // Add Organiser
            context.Users.Add(new User
            {
                UserID = 1,
                Email = "organiser@test.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123"),
                FirstName = "Test",
                LastName = "Organiser",
                Role = "Organiser",
                CreatedAt = DateTime.Now
            });

            // Add Participant
            context.Users.Add(new User
            {
                UserID = 2,
                Email = "participant@test.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123"),
                FirstName = "Test",
                LastName = "Participant",
                Role = "Participant",
                CreatedAt = DateTime.Now
            });

            // Add Event
            context.Events.Add(new Event
            {
                EventID = 1,
                OrganiserID = 1,
                Name = "Test Event",
                Description = "A test event",
                EventDate = DateTime.Now.AddDays(30),
                Location = "Test Location",
                MaxParticipants = 100,
                Status = "Active",
                CreatedAt = DateTime.Now
            });

            // Add Category
            context.Categories.Add(new Category
            {
                CategoryID = 1,
                EventID = 1,
                Name = "Test Category",
                Distance = 10.0m,
                StartTime = TimeSpan.FromHours(7),
                MaxParticipants = 50,
                Fee = 100.00m,
                CreatedAt = DateTime.Now
            });

            context.SaveChanges();
        }
    }
}