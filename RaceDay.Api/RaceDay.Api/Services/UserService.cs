using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;
using RaceDay.API.DTOs;
using RaceDay.API.Models;

namespace RaceDay.API.Services
{
    public class UserService
    {
        private readonly RaceDayDbContext _context;

        public UserService(RaceDayDbContext context)
        {
            _context = context;
        }

        public async Task<UserDto?> GetProfileAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return null;

            return new UserDto
            {
                UserID = user.UserID,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = user.Role,
                CreatedAt = user.CreatedAt
            };
        }

        public async Task<UserDto?> UpdateProfileAsync(int userId, UpdateUserDto dto)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return null;

            if (!string.IsNullOrEmpty(dto.FirstName))
                user.FirstName = dto.FirstName;

            if (!string.IsNullOrEmpty(dto.LastName))
                user.LastName = dto.LastName;

            if (!string.IsNullOrEmpty(dto.Email))
            {
                // Check if email is already taken by another user
                var emailExists = await _context.Users
                    .AnyAsync(u => u.Email == dto.Email && u.UserID != userId);

                if (emailExists) return null;

                user.Email = dto.Email;
            }

            user.UpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();

            return new UserDto
            {
                UserID = user.UserID,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = user.Role,
                CreatedAt = user.CreatedAt
            };
        }

        public async Task<List<EnrolmentDto>> GetUserEnrolmentsAsync(int userId)
        {
            var enrolments = await _context.Enrolments
                .Include(e => e.Participant)
                .Include(e => e.Event)
                .Include(e => e.Category)
                .Where(e => e.ParticipantID == userId)
                .ToListAsync();

            return enrolments.Select(e => new EnrolmentDto
            {
                EnrolmentID = e.EnrolmentID,
                ParticipantID = e.ParticipantID,
                ParticipantName = e.Participant != null
                    ? $"{e.Participant.FirstName} {e.Participant.LastName}"
                    : string.Empty,
                EventID = e.EventID,
                EventName = e.Event?.Name ?? string.Empty,
                CategoryID = e.CategoryID,
                CategoryName = e.Category?.Name ?? string.Empty,
                EnrolmentDate = e.EnrolmentDate,
                Status = e.Status,
                PaymentStatus = e.PaymentStatus
            }).ToList();
        }
    }
}