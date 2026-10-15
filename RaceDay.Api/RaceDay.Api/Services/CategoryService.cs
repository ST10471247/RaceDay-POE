using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;
using RaceDay.API.DTOs;
using RaceDay.API.Models;

namespace RaceDay.API.Services
{
    public class CategoryService
    {
        private readonly RaceDayDbContext _context;

        public CategoryService(RaceDayDbContext context)
        {
            _context = context;
        }

        public async Task<List<CategoryDto>> GetCategoriesByEventAsync(int eventId)
        {
            var categories = await _context.Categories
                .Include(c => c.Enrolments)
                .Where(c => c.EventID == eventId)
                .ToListAsync();

            return categories.Select(MapToDto).ToList();
        }

        public async Task<CategoryDto?> GetCategoryByIdAsync(int id)
        {
            var category = await _context.Categories
                .Include(c => c.Enrolments)
                .FirstOrDefaultAsync(c => c.CategoryID == id);

            if (category == null) return null;

            return MapToDto(category);
        }

        public async Task<CategoryDto?> CreateCategoryAsync(int eventId, int organiserId, CreateCategoryDto dto)
        {
            // Verify event belongs to organiser
            var eventEntity = await _context.Events.FindAsync(eventId);
            if (eventEntity == null || eventEntity.OrganiserID != organiserId)
                return null;

            var category = new Category
            {
                EventID = eventId,
                Name = dto.Name,
                Distance = dto.Distance,
                StartTime = dto.StartTime,
                MaxParticipants = dto.MaxParticipants,
                Fee = dto.Fee,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return MapToDto(category);
        }

        public async Task<CategoryDto?> UpdateCategoryAsync(int id, int organiserId, CreateCategoryDto dto)
        {
            var category = await _context.Categories
                .Include(c => c.Event)
                .Include(c => c.Enrolments)
                .FirstOrDefaultAsync(c => c.CategoryID == id);

            if (category == null) return null;
            if (category.Event?.OrganiserID != organiserId) return null;

            category.Name = dto.Name;
            category.Distance = dto.Distance;
            category.StartTime = dto.StartTime;
            category.MaxParticipants = dto.MaxParticipants;
            category.Fee = dto.Fee;
            category.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            return MapToDto(category);
        }

        public async Task<bool> DeleteCategoryAsync(int id, int organiserId)
        {
            var category = await _context.Categories
                .Include(c => c.Event)
                .FirstOrDefaultAsync(c => c.CategoryID == id);

            if (category == null) return false;
            if (category.Event?.OrganiserID != organiserId) return false;

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return true;
        }

        private CategoryDto MapToDto(Category category)
        {
            return new CategoryDto
            {
                CategoryID = category.CategoryID,
                EventID = category.EventID,
                Name = category.Name,
                Distance = category.Distance,
                StartTime = category.StartTime,
                MaxParticipants = category.MaxParticipants,
                Fee = category.Fee,
                CurrentEnrolments = category.Enrolments?.Count(e => e.Status == "Active") ?? 0
            };
        }
    }
}
