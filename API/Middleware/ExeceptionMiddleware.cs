using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using API.Errors;

namespace API.Middleware
{
    public class ExeceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExeceptionMiddleware> _logger;
        private readonly IHostEnvironment _env;

        public ExeceptionMiddleware(RequestDelegate next,ILogger<ExeceptionMiddleware> logger,
        IHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }
    
        public async Task InvokeAsync(HttpContext context)
        {
            try{
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,ex.Message);
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                var response=_env.IsDevelopment()
                ? new (context.Response.StatusCode,ex.Message,ex.StackTrace?.ToString())
                :new ApiException(context.Response.StatusCode,ex.Message, "Internal server error");

                var options = new JsonSerializerOptions{PropertyNamingPolicy = JsonNamingPolicy.CamelCase};
                 var json=JsonSerializer.Serialize(response, options);
                await context.Response.WriteAsync(json);
                }
        }
    }
}