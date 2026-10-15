namespace RaceDay.API.DTOs
{
    public class RouteInfoDto
    {
        public int RouteID { get; set; }
        public int EventID { get; set; }
        public string? Description { get; set; }
        public decimal Distance { get; set; }
        public int? ElevationGain { get; set; }
        public string? Waypoints { get; set; }
        public string? MapUrl { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
