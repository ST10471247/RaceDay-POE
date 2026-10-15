using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RaceDay.API.DTOs;
using RaceDay.API.Helpers;
using RaceDay.API.Services;

namespace RaceDay.API.Controllers
{
    [Route("api/")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly CategoryService _categoryService;

        public CategoriesController(CategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        // GET: api/events/{eventId}/categories
        [HttpGet("events/{eventId}/categories")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCategoriesByEvent(int eventId)
        {
            var categories = await _categoryService.GetCategoriesByEventAsync(eventId);
            return Ok(categories);
        }

        // POST: api/events/{eventId}/categories
        [HttpPost("events/{eventId}/categories")]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> CreateCategory(int eventId, [FromBody] CreateCategoryDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var organiserId = User.GetUserId();
            var created = await _categoryService.CreateCategoryAsync(eventId, organiserId, dto);

            if (created == null)
                return NotFound(new { message = "Event not found or you don't have permission" });

            return CreatedAtAction(nameof(GetCategoriesByEvent), new { eventId = eventId }, created);
        }

        // PUT: api/categories/{id}
        [HttpPut("categories/{id}")]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] CreateCategoryDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var organiserId = User.GetUserId();
            var updated = await _categoryService.UpdateCategoryAsync(id, organiserId, dto);

            if (updated == null)
                return NotFound(new { message = "Category not found or you don't have permission" });

            return Ok(updated);
        }

        // DELETE: api/categories/{id}
        [HttpDelete("categories/{id}")]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var organiserId = User.GetUserId();
            var result = await _categoryService.DeleteCategoryAsync(id, organiserId);

            if (!result)
                return NotFound(new { message = "Category not found or you don't have permission" });

            return NoContent();
        }
    }
}
