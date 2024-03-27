using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace Basics_Advanced.Transformation
{
    public class ClaimsTransformation : IClaimsTransformation
    {

        // This function is called every time an authorization call is made
        public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            var hasFriendClaim = principal.Claims.Any(x => x.Type == "Friend");

            if(!hasFriendClaim)
            {
                ((ClaimsIdentity)principal.Identity).AddClaim(new Claim("Friend", "Bad"));
            }

            return Task.FromResult(principal);
        }
    }
}
