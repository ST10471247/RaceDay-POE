using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDay.API.Models
{
    public class Enrolment
    {
        [Key]
        public int EnrolmentID { get; set; }

        [Required]
        public int ParticipantID { get; set; }

        [Required]
        public int EventID { get; set; }

        [Required]
        public int CategoryID { get; set; }

        public DateTime EnrolmentDate { get; set; } = DateTime.Now;

        [MaxLength(50)]
        public string Status { get; set; } = "Active";

        [MaxLength(50)]
        public string PaymentStatus { get; set; } = "Pending";

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        // Navigation properties
        [ForeignKey("ParticipantID")]
        public User? Participant { get; set; }

        [ForeignKey("EventID")]
        public Event? Event { get; set; }

        [ForeignKey("CategoryID")]
        public Category? Category { get; set; }

        public Result? Result { get; set; }
    }
}
