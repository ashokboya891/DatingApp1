

using System.Security.Claims;

namespace API.Extensions
{
    public  static class ClaimsPrincipalExtension
    {
        public static string GetUsername(this ClaimsPrincipal User)
        {
            //   return  user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
             return  User.FindFirst(ClaimTypes.Name)?.Value;
             
        }

          public static int GetUserId(this ClaimsPrincipal User)
        {
            //   return  user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
             return int .Parse( User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
             
        }
        
    }
}