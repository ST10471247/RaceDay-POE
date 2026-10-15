using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RaceDay.API.DTOs;
using RaceDay.API.Helpers;
using RaceDay.API.Services;

namespace RaceDay.API.Controllers
{
    [Route("api/")]
    [ApiController]
    [Authorize]
    public class EnrolmentsController : ControllerBase
    {
        private readonly EnrolmentService _enrolmentService;

        public EnrolmentsController(EnrolmentService enrolmentService)
        {
            _enrolmentService = enrolmentService;
        }

        // GET: api/enrolments
        [HttpGet("enrolments")]
        public async Task<IActionResult> GetAllEnrolments()
        {
            var userId = User.GetUserId();
            var role = User.GetUserRole();
            var enrolments = await _enrolmentService.GetAllEnrolmentsAsync(userId, role);
            return Ok(enrolments);
        }

        // POST: api/events/{eventId}/enrol
        [HttpPost("events/{eventId}/enrol")]
        [Authorize(Roles = "Participant")]
        public async Task<IActionResult> Enrol(int eventId, [FromBody] CreateEnrolmentDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var participantId = User.GetUserId();
            var result = await _enrolmentService.EnrolAsync(participantId, eventId, dto);

            if (result == null)
                return BadRequest(new { message = "Unable to enrol. Event may be full, already enrolled, or invalid category." });

            return CreatedAtAction(nameof(GetAllEnrolments), result);
        }

        // PUT: api/enrolments/{id}/cancel
        [HttpPut("enrolments/{id}/cancel")]
        public async Task<IActionResult> CancelEnrolment(int id)
        {
            var userId = User.GetUserId();
            var role = User.GetUserRole();
            var result = await _enrolmentService.CancelEnrolmentAsync(id, userId, role);

            if (result == null)
                return NotFound(new { message = "Enrolment not found or you don't have permission" });

            return Ok(result);
        }

        // GET: api/events/{eventId}/participants
        [HttpGet("events/{eventId}/participants")]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> GetEventParticipants(int eventId)
        {
            var organiserId = User.GetUserId();
            var participants = await _enrolmentService.GetEventParticipantsAsync(eventId, organiserId);
            return Ok(participants);
        }
    }
}