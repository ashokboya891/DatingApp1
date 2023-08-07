using API.Data;
using API.Services;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;
namespace API.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection addApplicationServices(this IServiceCollection services,IConfiguration config)   
        {

            services.AddDbContext<DataContext>(opt=>{
                opt.UseSqlite(config.GetConnectionString("DefaultConnection"));
            }    );

            services.AddScoped<ITokenService,TokenService>();
           services.AddCors();
           return services;

        }
    }
}