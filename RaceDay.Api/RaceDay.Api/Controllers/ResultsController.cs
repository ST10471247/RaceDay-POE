using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RaceDay.API.DTOs;
using RaceDay.API.Helpers;
using RaceDay.API.Services;

namespace RaceDay.API.Controllers
{
    [Route("api/")]
    [ApiController]
    public class ResultsController : ControllerBase
    {
        private readonly ResultService _resultService;

        public ResultsController(ResultService resultService)
        {
            _resultService = resultService;
        }

        // GET: api/events/{eventId}/results
        [HttpGet("events/{eventId}/results")]
        [AllowAnonymous]
        public async Task<IActionResult> GetEventResults(int eventId)
        {
            var results = await _resultService.GetEventResultsAsync(eventId);
            return Ok(results);
        }

        // GET: api/participants/{id}/results
        [HttpGet("participants/{id}/results")]
        [Authorize]
        public async Task<IActionResult> GetParticipantResults(int id)
        {
            var currentUserId = User.GetUserId();
            var role = User.GetUserRole();

            if (role != "Organiser" && currentUserId != id)
                return Forbid();

            var results = await _resultService.GetParticipantResultsAsync(id);
            return Ok(results);
        }

        // POST: api/events/{eventId}/results
        [HttpPost("events/{eventId}/results")]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> CaptureResults(int eventId, [FromBody] List<CreateResultDto> dtos)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var organiserId = User.GetUserId();
            var results = await _resultService.CreateResultsAsync(eventId, organiserId, dtos);

            if (results.Count == 0)
                return BadRequest(new { message = "No results were captured. Check event ownership and enrolment IDs." });

            return CreatedAtAction(nameof(GetEventResults), new { eventId = eventId }, results);
        }
    }
}
