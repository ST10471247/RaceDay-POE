using System.ComponentModel.DataAnnotations;

namespace RaceDay.API.DTOs
{
    public class CreateEventDto
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required]
        public DateTime EventDate { get; set; }

        [Required]
        [MaxLength(500)]
        public string Location { get; set; } = string.Empty;

        public string? RouteInfo { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int MaxParticipants { get; set; }
    }
}