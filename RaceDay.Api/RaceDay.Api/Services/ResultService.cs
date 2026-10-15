using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;
using RaceDay.API.DTOs;
using RaceDay.API.Models;

namespace RaceDay.API.Services
{
    public class ResultService
    {
        private readonly RaceDayDbContext _context;

        public ResultService(RaceDayDbContext context)
        {
            _context = context;
        }

        public async Task<List<ResultDto>> GetEventResultsAsync(int eventId)
        {
            var results = await _context.Results
                .Include(r => r.Participant)
                .Include(r => r.Category)
                .Where(r => r.Enrolment!.EventID == eventId)
                .OrderBy(r => r.Position)
                .ToListAsync();

            return results.Select(MapToDto).ToList();
        }

        public async Task<List<ResultDto>> GetParticipantResultsAsync(int participantId)
        {
            var results = await _context.Results
                .Include(r => r.Participant)
                .Include(r => r.Category)
                .Where(r => r.ParticipantID == participantId)
                .ToListAsync();

            return results.Select(MapToDto).ToList();
        }

        public async Task<List<ResultDto>> CreateResultsAsync(int eventId, int organiserId, List<CreateResultDto> dtos)
        {
            // Verify event belongs to organiser
            var eventEntity = await _context.Events.FindAsync(eventId);
            if (eventEntity == null || eventEntity.OrganiserID != organiserId)
                return new List<ResultDto>();

            var createdResults = new List<Result>();

            foreach (var dto in dtos)
            {
                // Get the enrolment
                var enrolment = await _context.Enrolments
                    .FirstOrDefaultAsync(e => e.EnrolmentID == dto.EnrolmentId
                        && e.EventID == eventId);

                if (enrolment == null) continue;

                // Check if result already exists
                var existing = await _context.Results
                    .FirstOrDefaultAsync(r => r.EnrolmentID == dto.EnrolmentId);

                if (existing != null) continue;

                var result = new Result
                {
                    ParticipantID = enrolment.ParticipantID,
                    EnrolmentID = dto.EnrolmentId,
                    CategoryID = enrolment.CategoryID,
                    FinishTime = dto.FinishTime,
                    Position = dto.Position,
                    Status = "Completed",
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                _context.Results.Add(result);
                createdResults.Add(result);

                // Update enrolment status
                enrolment.Status = "Completed";
            }

            await _context.SaveChangesAsync();

            // Reload with nav properties
            var resultIds = createdResults.Select(r => r.ResultID).ToList();
            var finalResults = await _context.Results
                .Include(r => r.Participant)
                .Include(r => r.Category)
                .Where(r => resultIds.Contains(r.ResultID))
                .ToListAsync();

            return finalResults.Select(MapToDto).ToList();
        }

        private ResultDto MapToDto(Result r)
        {
            return new ResultDto
            {
                ResultID = r.ResultID,
                ParticipantID = r.ParticipantID,
                ParticipantName = r.Participant != null
                    ? $"{r.Participant.FirstName} {r.Participant.LastName}"
                    : string.Empty,
                EnrolmentID = r.EnrolmentID,
                CategoryID = r.CategoryID,
                CategoryName = r.Category?.Name ?? string.Empty,
                FinishTime = r.FinishTime,
                Position = r.Position,
                Status = r.Status
            };
        }
    }
}