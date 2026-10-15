using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RaceDay.API.Helpers;
using RaceDay.API.Services;

namespace RaceDay.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly UserService _userService;

        public UsersController(UserService userService)
        {
            _userService = userService;
        }

        // GET: api/users/profile
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var userId = User.GetUserId();
            var profile = await _userService.GetProfileAsync(userId);

            if (profile == null)
                return NotFound(new { message = "User not found" });

            return Ok(profile);
        }

        // PUT: api/users/profile
        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] DTOs.UpdateUserDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = User.GetUserId();
            var updated = await _userService.UpdateProfileAsync(userId, dto);

            if (updated == null)
                return NotFound(new { message = "User not found or email already in use" });

            return Ok(updated);
        }

        // GET: api/users/{id}/enrolments
        [HttpGet("{id}/enrolments")]
        public async Task<IActionResult> GetUserEnrolments(int id)
        {
            var currentUserId = User.GetUserId();
            var currentUserRole = User.GetUserRole();

            // Only allow users to view their own enrolments, or organisers
            if (currentUserRole != "Organiser" && currentUserId != id)
                return Forbid();

            var enrolments = await _userService.GetUserEnrolmentsAsync(id);
            return Ok(enrolments);
        }
    }
}