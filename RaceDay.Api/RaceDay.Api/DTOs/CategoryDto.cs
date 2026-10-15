namespace RaceDay.API.DTOs
{
    public class CategoryDto
    {
        public int CategoryID { get; set; }
        public int EventID { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Distance { get; set; }
        public TimeSpan StartTime { get; set; }
        public int MaxParticipants { get; set; }
        public decimal Fee { get; set; }
        public int CurrentEnrolments { get; set; }
    }
}
