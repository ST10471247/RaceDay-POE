using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDay.API.Models
{
    public class Weather
    {
        [Key]
        public int WeatherID { get; set; }

        [Required]
        public int EventID { get; set; }

        [Required]
        public DateTime ForecastDate { get; set; }

        public decimal? Temperature { get; set; }

        [MaxLength(100)]
        public string? Conditions { get; set; }

        public int? WindSpeed { get; set; }

        public int? Humidity { get; set; }

        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        // Navigation property
        [ForeignKey("EventID")]
        public Event? Event { get; set; }
    }
}