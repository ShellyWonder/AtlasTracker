using System.Security.Claims;
using System.Security.Principal;

namespace AtlasTracker.Extensions
{
    public static class IdentityExtensions
    {
        //Creating only ONE INSTANCE of the class
        public static int GetCompanyId(this IIdentity identity)
        {
            Claim claim = ((ClaimsIdentity)identity).FindFirst("CompanyId")!;
            return  int.Parse(claim.Value);
        }
    }
}
