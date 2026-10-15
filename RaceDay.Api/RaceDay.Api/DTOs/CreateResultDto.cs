using System.ComponentModel.DataAnnotations;

namespace RaceDay.API.DTOs
{
    public class CreateResultDto
    {
        [Required]
        public int EnrolmentId { get; set; }

        public TimeSpan? FinishTime { get; set; }

        public int? Position { get; set; }
    }
}