using Authenticator.API.Core.Domain.Authentication;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;

namespace Authenticator.API.Infrastructure.Helpers
{
    public static class ClaimsMapper
    {
        public static AuthenticatedUser? ToAuthenticatedUser(this ClaimsPrincipal user)
        {
            if (user == null || !user.Identity!.IsAuthenticated)
                return null;

            return new AuthenticatedUser
            {
                TenantId = Guid.TryParse(user.Claims.FirstOrDefault(c => c.Type == "tenant_id")?.Value, out var guid)? guid : Guid.Empty,
                TenantSlug = user.Claims.FirstOrDefault(c => c.Type == "tenant_slug")?.Value,
                TenantName = user.Claims.FirstOrDefault(c => c.Type == "tenant_name")?.Value,
                UserId = user.Claims.FirstOrDefault(c => c.Type == "user_id")?.Value,
                Username = user.Claims.FirstOrDefault(c => c.Type == "username")?.Value,
                FullName = user.Claims.FirstOrDefault(c => c.Type == "full_name")?.Value,
                Email = user.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")?.Value,
                Roles = user.Claims
                    .Where(c => c.Type == ClaimTypes.Role || c.Type == "role")
                    .Select(c => c.Value)
                    .ToList()
            };
        }
    }
}
