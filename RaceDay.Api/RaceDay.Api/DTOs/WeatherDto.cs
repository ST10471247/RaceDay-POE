namespace RaceDay.API.DTOs
{
    public class WeatherDto
    {
        public int WeatherID { get; set; }
        public int EventID { get; set; }
        public DateTime ForecastDate { get; set; }
        public decimal? Temperature { get; set; }
        public string? Conditions { get; set; }
        public int? WindSpeed { get; set; }
        public int? Humidity { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}