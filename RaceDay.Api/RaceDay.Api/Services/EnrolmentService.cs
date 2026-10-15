using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;
using RaceDay.API.DTOs;
using RaceDay.API.Models;

namespace RaceDay.API.Services
{
    public class EnrolmentService
    {
        private readonly RaceDayDbContext _context;

        public EnrolmentService(RaceDayDbContext context)
        {
            _context = context;
        }

        public async Task<List<EnrolmentDto>> GetAllEnrolmentsAsync(int userId, string role)
        {
            IQueryable<Enrolment> query = _context.Enrolments
                .Include(e => e.Participant)
                .Include(e => e.Event)
                .Include(e => e.Category);

            if (role == "Participant")
            {
                query = query.Where(e => e.ParticipantID == userId);
            }
            else if (role == "Organiser")
            {
                query = query.Where(e => e.Event!.OrganiserID == userId);
            }

            var enrolments = await query.ToListAsync();
            return enrolments.Select(MapToDto).ToList();
        }

        public async Task<EnrolmentDto?> EnrolAsync(int participantId, int eventId, CreateEnrolmentDto dto)
        {
            // Verify event exists and is active
            var eventEntity = await _context.Events.FindAsync(eventId);
            if (eventEntity == null || eventEntity.Status != "Active")
                return null;

            // Verify category exists and belongs to event
            var category = await _context.Categories.FindAsync(dto.CategoryId);
            if (category == null || category.EventID != eventId)
                return null;

            // Check if already enrolled
            var existing = await _context.Enrolments
                .FirstOrDefaultAsync(e => e.ParticipantID == participantId && e.EventID == eventId);

            if (existing != null)
                return null;

            // Check if event is full
            var currentCount = await _context.Enrolments
                .CountAsync(e => e.EventID == eventId && e.Status == "Active");

            if (currentCount >= eventEntity.MaxParticipants)
                return null;

            // Check if category is full
            var categoryCount = await _context.Enrolments
                .CountAsync(e => e.CategoryID == dto.CategoryId && e.Status == "Active");

            if (categoryCount >= category.MaxParticipants)
                return null;

            var enrolment = new Enrolment
            {
                ParticipantID = participantId,
                EventID = eventId,
                CategoryID = dto.CategoryId,
                EnrolmentDate = DateTime.Now,
                Status = "Active",
                PaymentStatus = "Pending",
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            _context.Enrolments.Add(enrolment);
            await _context.SaveChangesAsync();

            // Reload with navigation properties
            var created = await _context.Enrolments
                .Include(e => e.Participant)
                .Include(e => e.Event)
                .Include(e => e.Category)
                .FirstOrDefaultAsync(e => e.EnrolmentID == enrolment.EnrolmentID);

            return MapToDto(created!);
        }

        public async Task<EnrolmentDto?> CancelEnrolmentAsync(int id, int userId, string role)
        {
            var enrolment = await _context.Enrolments
                .Include(e => e.Participant)
                .Include(e => e.Event)
                .Include(e => e.Category)
                .FirstOrDefaultAsync(e => e.EnrolmentID == id);

            if (enrolment == null) return null;

            // Participant can only cancel their own
            if (role == "Participant" && enrolment.ParticipantID != userId)
                return null;

            // Organiser can only cancel for their own events
            if (role == "Organiser" && enrolment.Event?.OrganiserID != userId)
                return null;

            enrolment.Status = "Cancelled";
            enrolment.UpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();

            return MapToDto(enrolment);
        }

        public async Task<List<EnrolmentDto>> GetEventParticipantsAsync(int eventId, int organiserId)
        {
            var eventEntity = await _context.Events.FindAsync(eventId);
            if (eventEntity == null || eventEntity.OrganiserID != organiserId)
                return new List<EnrolmentDto>();

            var enrolments = await _context.Enrolments
                .Include(e => e.Participant)
                .Include(e => e.Event)
                .Include(e => e.Category)
                .Where(e => e.EventID == eventId)
                .ToListAsync();

            return enrolments.Select(MapToDto).ToList();
        }

        private EnrolmentDto MapToDto(Enrolment e)
        {
            return new EnrolmentDto
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
            };
        }
    }
}