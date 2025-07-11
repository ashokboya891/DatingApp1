﻿using API.Data;
using API.Entities;
using API.Extensions;
using API.Middleware;
using API.SignalR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.addApplicationServices(builder.Configuration);
builder.Services.AddSwaggerGen(); // ✅ Make sure this line is present

builder.Services.AddIdentityServices(builder.Configuration);

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
});
var app = builder.Build();

app.UseMiddleware<ExeceptionMiddleware>();
app.UseCors(builder=>builder.AllowAnyHeader().AllowAnyMethod()
.AllowCredentials()
.WithOrigins("https://localhost:4200"));
// ✅ FIX: Enable Swagger in Development mode only
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
    });
}
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

// app.UseDefaultFiles();
// app.UseStaticFiles();
app.MapControllers();
app.MapHub<PresenceHub>("hubs/presence");
app.MapHub<MessageHub>("hubs/message");
// app.MapFallbackToController("Index","Fallback");

using var scope=app.Services.CreateScope();
var services = scope.ServiceProvider;
try{
    var context = services.GetRequiredService<DataContext>();
    var userManager=services.GetRequiredService<UserManager<AppUser>>();
    var roleManager=services.GetRequiredService<RoleManager<AppRole>>();

    await context.Database.MigrateAsync();
    // await Seed.ClearConnections(context);
    await context.Database.ExecuteSqlRawAsync("DELETE FROM [Connections]");  //after adding migration  problem will raise so added new method in seed.cs so it got commented
    // context.connections.RemoveRange(context.connections);
    await Seed.SeedUsers(userManager,roleManager);

}
catch(Exception ex)
{
    var logger=services.GetService<ILogger<Program>>();
    logger.LogError(ex,"An Error occured druing migration");
}
app.Run();
