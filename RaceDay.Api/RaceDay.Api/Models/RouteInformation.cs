using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDay.API.Models
{
    public class RouteInformation
    {
        [Key]
        public int RouteID { get; set; }

        [Required]
        public int EventID { get; set; }

        public string? Description { get; set; }

        [Required]
        public decimal Distance { get; set; }

        public int? ElevationGain { get; set; }

        public string? Waypoints { get; set; }

        [MaxLength(500)]
        public string? MapUrl { get; set; }

        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        // Navigation property
        [ForeignKey("EventID")]
        public Event? Event { get; set; }
    }
}