using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RaceDay.API.DTOs;
using RaceDay.API.Helpers;
using RaceDay.API.Services;

namespace RaceDay.API.Controllers
{
    [Route("api/")]
    [ApiController]
    public class WeatherController : ControllerBase
    {
        private readonly WeatherService _weatherService;

        public WeatherController(WeatherService weatherService)
        {
            _weatherService = weatherService;
        }

        // GET: api/events/{eventId}/weather
        [HttpGet("events/{eventId}/weather")]
        [AllowAnonymous]
        public async Task<IActionResult> GetWeather(int eventId)
        {
            var weather = await _weatherService.GetWeatherByEventAsync(eventId);

            if (weather == null)
                return NotFound(new { message = "Weather information not found for this event" });

            return Ok(weather);
        }

        // PUT: api/events/{eventId}/weather
        [HttpPut("events/{eventId}/weather")]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> UpdateWeather(int eventId, [FromBody] WeatherDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var organiserId = User.GetUserId();
            var updated = await _weatherService.UpdateWeatherAsync(eventId, organiserId, dto);

            if (updated == null)
                return NotFound(new { message = "Event not found or you don't have permission" });

            return Ok(updated);
        }
    }
}