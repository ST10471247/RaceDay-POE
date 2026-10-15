using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RaceDay.API.DTOs;
using RaceDay.API.Helpers;
using RaceDay.API.Services;

namespace RaceDay.API.Controllers
{
    [Route("api/")]
    [ApiController]
    public class RouteController : ControllerBase
    {
        private readonly RouteService _routeService;

        public RouteController(RouteService routeService)
        {
            _routeService = routeService;
        }

        // GET: api/events/{eventId}/route
        [HttpGet("events/{eventId}/route")]
        [AllowAnonymous]
        public async Task<IActionResult> GetRoute(int eventId)
        {
            var route = await _routeService.GetRouteByEventAsync(eventId);

            if (route == null)
                return NotFound(new { message = "Route information not found for this event" });

            return Ok(route);
        }

        // PUT: api/events/{eventId}/route
        [HttpPut("events/{eventId}/route")]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> UpdateRoute(int eventId, [FromBody] RouteInfoDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var organiserId = User.GetUserId();
            var updated = await _routeService.UpdateRouteAsync(eventId, organiserId, dto);

            if (updated == null)
                return NotFound(new { message = "Event not found or you don't have permission" });

            return Ok(updated);
        }
    }
}