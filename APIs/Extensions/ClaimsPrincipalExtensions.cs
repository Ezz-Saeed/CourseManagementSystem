using System.Security.Claims;

namespace API.Extensions
{
    public static class ClaimsPincipalExtensions
    {
        public static string GetUserId(this ClaimsPrincipal principal)
        {
            return principal.FindFirst(c => c.Type == ClaimTypes.NameIdentifier)!.Value;
        }

        public static string GetUserEmail(this ClaimsPrincipal principal)
        {
            return principal.FindFirst(c => c.Type == ClaimTypes.Email)!.Value;
        }

        public static string GetUserName(this ClaimsPrincipal principal)
        {
            return principal.FindFirst(c => c.Type == ClaimTypes.Name)!.Value;
        }
    }
}
