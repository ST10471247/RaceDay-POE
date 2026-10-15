using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDay.API.Models
{
    public class Result
    {
        [Key]
        public int ResultID { get; set; }

        [Required]
        public int ParticipantID { get; set; }

        [Required]
        public int EnrolmentID { get; set; }

        [Required]
        public int CategoryID { get; set; }

        public TimeSpan? FinishTime { get; set; }

        public int? Position { get; set; }

        [MaxLength(50)]
        public string Status { get; set; } = "Pending";

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        // Navigation properties
        [ForeignKey("ParticipantID")]
        public User? Participant { get; set; }

        [ForeignKey("EnrolmentID")]
        public Enrolment? Enrolment { get; set; }

        [ForeignKey("CategoryID")]
        public Category? Category { get; set; }
    }
}