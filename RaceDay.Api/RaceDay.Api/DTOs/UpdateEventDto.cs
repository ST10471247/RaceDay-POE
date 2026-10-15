using System.ComponentModel.DataAnnotations;

namespace RaceDay.API.DTOs
{
    public class UpdateEventDto
    {
        [MaxLength(200)]
        public string? Name { get; set; }

        public string? Description { get; set; }

        public DateTime? EventDate { get; set; }

        [MaxLength(500)]
        public string? Location { get; set; }

        public string? RouteInfo { get; set; }

        [Range(1, int.MaxValue)]
        public int? MaxParticipants { get; set; }

        public string? Status { get; set; }
    }
}