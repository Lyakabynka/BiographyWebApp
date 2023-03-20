using BiographyWebApp.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

namespace BiographyWebApp.Services
{
    public class AuthService
    {
        public ClaimsPrincipal GenerateClaimsPrincipal(User user, string AuthenticationScheme)
        {
            List<Claim> claims = new List<Claim>()
            {
                    new Claim(ClaimTypes.NameIdentifier, Convert.ToString(user.Id)),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role.ToString())
            };
            ClaimsIdentity identity = new ClaimsIdentity(claims, AuthenticationScheme);
            ClaimsPrincipal principal = new ClaimsPrincipal(identity);

            return principal;
        }
    }
}
