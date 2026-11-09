using System.ComponentModel.DataAnnotations;

namespace RaceDay.MVC.Models.ViewModels
{
    public class CreateEventViewModel
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required]
        public DateTime EventDate { get; set; }

        [Required]
        public string Location { get; set; } = string.Empty;

        public string? RouteInfo { get; set; }

        [Required]
        public int MaxParticipants { get; set; }

        // The file upload for Azure
        public IFormFile? ImageFile { get; set; }
    }
}