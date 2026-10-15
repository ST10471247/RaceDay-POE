using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;
using RaceDay.API.DTOs;
using RaceDay.API.Models;

namespace RaceDay.API.Services
{
    public class EventService
    {
        private readonly RaceDayDbContext _context;

        public EventService(RaceDayDbContext context)
        {
            _context = context;
        }

        public async Task<List<EventDto>> GetAllEventsAsync()
        {
            var events = await _context.Events
                .Include(e => e.Organiser)
                .Include(e => e.Enrolments)
                .OrderBy(e => e.EventDate)
                .ToListAsync();

            return events.Select(MapToDto).ToList();
        }

        public async Task<EventDto?> GetEventByIdAsync(int id)
        {
            var ev = await _context.Events
                .Include(e => e.Organiser)
                .Include(e => e.Enrolments)
                .FirstOrDefaultAsync(e => e.EventID == id);

            if (ev == null) return null;

            return MapToDto(ev);
        }

        public async Task<EventDto> CreateEventAsync(int organiserId, CreateEventDto dto)
        {
            var ev = new Event
            {
                OrganiserID = organiserId,
                Name = dto.Name,
                Description = dto.Description,
                EventDate = dto.EventDate,
                Location = dto.Location,
                RouteInfo = dto.RouteInfo,
                MaxParticipants = dto.MaxParticipants,
                Status = "Active",
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            _context.Events.Add(ev);
            await _context.SaveChangesAsync();

            // Reload with organiser
            var created = await _context.Events
                .Include(e => e.Organiser)
                .Include(e => e.Enrolments)
                .FirstOrDefaultAsync(e => e.EventID == ev.EventID);

            return MapToDto(created!);
        }

        public async Task<EventDto?> UpdateEventAsync(int id, int organiserId, UpdateEventDto dto)
        {
            var ev = await _context.Events.FindAsync(id);
            if (ev == null) return null;

            if (ev.OrganiserID != organiserId) return null;

            if (!string.IsNullOrEmpty(dto.Name)) ev.Name = dto.Name;
            if (dto.Description != null) ev.Description = dto.Description;
            if (dto.EventDate.HasValue) ev.EventDate = dto.EventDate.Value;
            if (!string.IsNullOrEmpty(dto.Location)) ev.Location = dto.Location;
            if (dto.RouteInfo != null) ev.RouteInfo = dto.RouteInfo;
            if (dto.MaxParticipants.HasValue) ev.MaxParticipants = dto.MaxParticipants.Value;
            if (!string.IsNullOrEmpty(dto.Status)) ev.Status = dto.Status;

            ev.UpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();

            var updated = await _context.Events
                .Include(e => e.Organiser)
                .Include(e => e.Enrolments)
                .FirstOrDefaultAsync(e => e.EventID == id);

            return MapToDto(updated!);
        }

        public async Task<bool> DeleteEventAsync(int id, int organiserId)
        {
            var ev = await _context.Events.FindAsync(id);
            if (ev == null) return false;
            if (ev.OrganiserID != organiserId) return false;

            // Soft delete
            ev.Status = "Cancelled";
            ev.UpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();

            return true;
        }

        private EventDto MapToDto(Event ev)
        {
            return new EventDto
            {
                EventID = ev.EventID,
                OrganiserID = ev.OrganiserID,
                OrganiserName = ev.Organiser != null
                    ? $"{ev.Organiser.FirstName} {ev.Organiser.LastName}"
                    : string.Empty,
                Name = ev.Name,
                Description = ev.Description,
                EventDate = ev.EventDate,
                Location = ev.Location,
                RouteInfo = ev.RouteInfo,
                MaxParticipants = ev.MaxParticipants,
                Status = ev.Status,
                CurrentEnrolments = ev.Enrolments?.Count(e => e.Status == "Active") ?? 0,
                CreatedAt = ev.CreatedAt
            };
        }
    }
}