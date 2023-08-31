using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using API.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Tokens;
using API.Entities;
using Microsoft.AspNetCore.Identity;
using API.Data;

namespace API.Extensions
{
    public  static class IdentityServiceExtension
    {
        public static IServiceCollection  AddIdentityServices(this IServiceCollection services,IConfiguration config)
        {
            services.AddIdentityCore<AppUser>(opt=>{
                opt.Password.RequireNonAlphanumeric=false;
                
            })
            .AddRoles<AppRole>()
            .AddRoleManager<RoleManager<AppRole>>()
            .AddEntityFrameworkStores<DataContext>();
            
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(Options =>
            {
            Options.TokenValidationParameters = new TokenValidationParameters
                {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"])),
            ValidateIssuer = false,
            ValidateAudience = false,

             };  
             Options.Events=new JwtBearerEvents
             {
                OnMessageReceived = context=>{
                    var accessToken=context.Request.Query["access_token"];
                    var path=context.HttpContext.Request.Path;
                    if(!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
                    {
                        context.Token=accessToken;

                    }
                    return Task.CompletedTask;
                }
             };
            });
            services.AddAuthorization(opt=>
            {
            opt.AddPolicy("RequireAdminRole",policy =>policy.RequireRole("Admin"));
            opt.AddPolicy("ModeratePhotoRole",policy =>policy.RequireRole("Admin","Moderator"));

            });



            return services;    
        }
    }
}