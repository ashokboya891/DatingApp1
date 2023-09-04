using API.Data;
using API.Services;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;
using API.Helpers;
using API.SignalR;

namespace API.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection addApplicationServices(this IServiceCollection services,IConfiguration config)   
        {

            services.AddDbContext<DataContext>(opt=>{
                //use sqllite removed after postgress added
                opt.UseSqlite(config.GetConnectionString("DefaultConnection"));
            });

            services.AddScoped<ITokenService,TokenService>();
           services.AddCors();
           services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
           services.Configure<CloudinarySettings>(config.GetSection("CloudinarySettings"));
           services.AddScoped<IPhotoService,PhotoService>();
            services.AddScoped<LogUserActivity>();

        //after adding unitofwork this 3 commented
            // services.AddScoped<IUserRepository,UserRepository>();
            // services.AddScoped<ILikesRespository,LikesRepository>();
            // services.AddScoped<IMessageRepository,MessageRepository>();
            services.AddSignalR();
            services.AddSingleton<PresenceTracker>();
            services.AddScoped<IUnitOfWork,UnitOfWork>();
           return services;

        }
    }
}