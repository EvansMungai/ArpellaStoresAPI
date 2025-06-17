namespace ArpellaStores.Extensions.Service_Handlers;

public static class CorsServiceRegistration
{
    public static void ConfigureCors(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddCors(options =>
        {
            options.AddDefaultPolicy(builder =>
            {
                builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
            });
            //options.AddPolicy("clientOrigin", builder => builder.WithOrigins("https://localhost:3000").AllowAnyHeader().AllowAnyMethod());
        });
    }
}
