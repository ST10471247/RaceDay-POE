using System.ComponentModel.DataAnnotations;

namespace RaceDay.API.DTOs
{
    public class CreateCategoryDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Range(0.1, 1000)]
        public decimal Distance { get; set; }

        [Required]
        public TimeSpan StartTime { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int MaxParticipants { get; set; }

        [Required]
        [Range(0, 100000)]
        public decimal Fee { get; set; }
    }
}