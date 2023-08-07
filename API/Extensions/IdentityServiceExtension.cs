using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using API.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Tokens;

namespace API.Extensions
{
    public  static class IdentityServiceExtension
    {
        public static IServiceCollection  AddIdentityServices(this IServiceCollection services,IConfiguration config)
        {
            
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(Options =>
            {
            Options.TokenValidationParameters = new TokenValidationParameters
                {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"])),
            ValidateIssuer = false,
            ValidateAudience = false,

             };  
            });
            return services;    
        }
    }
}