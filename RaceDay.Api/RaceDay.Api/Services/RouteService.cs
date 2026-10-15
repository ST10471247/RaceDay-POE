using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;
using RaceDay.API.DTOs;
using RaceDay.API.Models;

namespace RaceDay.API.Services
{
    public class RouteService
    {
        private readonly RaceDayDbContext _context;

        public RouteService(RaceDayDbContext context)
        {
            _context = context;
        }

        public async Task<RouteInfoDto?> GetRouteByEventAsync(int eventId)
        {
            var route = await _context.RouteInformation
                .FirstOrDefaultAsync(r => r.EventID == eventId);

            if (route == null) return null;

            return MapToDto(route);
        }

        public async Task<RouteInfoDto?> UpdateRouteAsync(int eventId, int organiserId, RouteInfoDto dto)
        {
            // Verify event belongs to organiser
            var eventEntity = await _context.Events.FindAsync(eventId);
            if (eventEntity == null || eventEntity.OrganiserID != organiserId)
                return null;

            var route = await _context.RouteInformation
                .FirstOrDefaultAsync(r => r.EventID == eventId);

            if (route == null)
            {
                route = new RouteInformation
                {
                    EventID = eventId,
                    Description = dto.Description,
                    Distance = dto.Distance,
                    ElevationGain = dto.ElevationGain,
                    Waypoints = dto.Waypoints,
                    MapUrl = dto.MapUrl,
                    UpdatedAt = DateTime.Now
                };
                _context.RouteInformation.Add(route);
            }
            else
            {
                route.Description = dto.Description;
                route.Distance = dto.Distance;
                route.ElevationGain = dto.ElevationGain;
                route.Waypoints = dto.Waypoints;
                route.MapUrl = dto.MapUrl;
                route.UpdatedAt = DateTime.Now;
            }

            await _context.SaveChangesAsync();
            return MapToDto(route);
        }

        private RouteInfoDto MapToDto(RouteInformation r)
        {
            return new RouteInfoDto
            {
                RouteID = r.RouteID,
                EventID = r.EventID,
                Description = r.Description,
                Distance = r.Distance,
                ElevationGain = r.ElevationGain,
                Waypoints = r.Waypoints,
                MapUrl = r.MapUrl,
                UpdatedAt = r.UpdatedAt
            };
        }
    }
}