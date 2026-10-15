using System.Security.Claims;

namespace RaceDay.API.Helpers
{
    public static class UserExtensions
    {
        public static int GetUserId(this ClaimsPrincipal user)
        {
            var claim = user.FindFirst(ClaimTypes.NameIdentifier);
            if (claim == null) return 0;

            return int.TryParse(claim.Value, out int id) ? id : 0;
        }

        public static string GetUserRole(this ClaimsPrincipal user)
        {
            var claim = user.FindFirst(ClaimTypes.Role);
            return claim?.Value ?? string.Empty;
        }
    }
}