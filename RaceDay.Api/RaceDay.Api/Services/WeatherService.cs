using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;
using RaceDay.API.DTOs;
using RaceDay.API.Models;

namespace RaceDay.API.Services
{
    public class WeatherService
    {
        private readonly RaceDayDbContext _context;

        public WeatherService(RaceDayDbContext context)
        {
            _context = context;
        }

        public async Task<WeatherDto?> GetWeatherByEventAsync(int eventId)
        {
            var weather = await _context.Weather
                .FirstOrDefaultAsync(w => w.EventID == eventId);

            if (weather == null) return null;

            return MapToDto(weather);
        }

        public async Task<WeatherDto?> UpdateWeatherAsync(int eventId, int organiserId, WeatherDto dto)
        {
            // Verify event belongs to organiser
            var eventEntity = await _context.Events.FindAsync(eventId);
            if (eventEntity == null || eventEntity.OrganiserID != organiserId)
                return null;

            var weather = await _context.Weather
                .FirstOrDefaultAsync(w => w.EventID == eventId);

            if (weather == null)
            {
                weather = new Weather
                {
                    EventID = eventId,
                    ForecastDate = dto.ForecastDate,
                    Temperature = dto.Temperature,
                    Conditions = dto.Conditions,
                    WindSpeed = dto.WindSpeed,
                    Humidity = dto.Humidity,
                    UpdatedAt = DateTime.Now
                };
                _context.Weather.Add(weather);
            }
            else
            {
                weather.ForecastDate = dto.ForecastDate;
                weather.Temperature = dto.Temperature;
                weather.Conditions = dto.Conditions;
                weather.WindSpeed = dto.WindSpeed;
                weather.Humidity = dto.Humidity;
                weather.UpdatedAt = DateTime.Now;
            }

            await _context.SaveChangesAsync();
            return MapToDto(weather);
        }

        private WeatherDto MapToDto(Weather w)
        {
            return new WeatherDto
            {
                WeatherID = w.WeatherID,
                EventID = w.EventID,
                ForecastDate = w.ForecastDate,
                Temperature = w.Temperature,
                Conditions = w.Conditions,
                WindSpeed = w.WindSpeed,
                Humidity = w.Humidity,
                UpdatedAt = w.UpdatedAt
            };
        }
    }
}
