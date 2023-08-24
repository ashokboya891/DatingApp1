using API.Data;
using API.Services;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;
using API.Helpers;

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
           services.AddScoped<IUserRepository,UserRepository>();
           services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
           services.Configure<CloudinarySettings>(config.GetSection("CloudinarySettings"));
           services.AddScoped<IPhotoService,PhotoService>();
            services.AddScoped<LogUserActivity>();
            
           return services;

        }
    }
}