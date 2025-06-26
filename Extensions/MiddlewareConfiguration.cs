using ArpellaStores.Extensions.RouteHandlers;
using RouteBuilder = ArpellaStores.Extensions.RouteHandlers.RouteBuilder;
namespace ArpellaStores.Extensions;

public static class MiddlewareConfiguration
{
    public static void ConfigureMiddleware(this WebApplication app)
    {
        app.UseCors();
        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "ARPELLA STORES API V1");
            });
        }

        using var scope = app.Services.CreateScope();
        var routeBuilder = scope.ServiceProvider.GetService<RouteBuilder>();
        routeBuilder?.RegisterRoutes(app);

    }
}

