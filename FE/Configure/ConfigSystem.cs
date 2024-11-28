using FE.Services;

namespace FE.Configure
{
    public static class ConfigSystem
    {
        public static void ConfigureSystem(this WebApplicationBuilder builder)
        {
            // Add CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigin",
                    builder =>
                    {
                        builder.WithOrigins("*").AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .SetIsOriginAllowed(origin => true) // allow any origin 
                        .SetPreflightMaxAge(TimeSpan.FromSeconds(2520));
                    });
            });

            builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            // Appsetting
            builder.Configuration
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();

            // Add Service
            AddService(builder);
        }

        public static void AddService(WebApplicationBuilder builder)
        {
            builder.Services.AddTransient<ICONSUMEAPIService, CONSUMEAPIService>();
        }
    }
}
