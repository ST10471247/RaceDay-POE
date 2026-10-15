namespace RaceDay.API.DTOs
{
    public class EventDto
    {
        public int EventID { get; set; }
        public int OrganiserID { get; set; }
        public string OrganiserName { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime EventDate { get; set; }
        public string Location { get; set; } = string.Empty;
        public string? RouteInfo { get; set; }
        public int MaxParticipants { get; set; }
        public string Status { get; set; } = string.Empty;
        public int CurrentEnrolments { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}