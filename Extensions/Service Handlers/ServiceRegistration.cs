using ArpellaStores.Data;
using ArpellaStores.Features.Payment_Management.Models;
using ArpellaStores.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;

namespace ArpellaStores.Extensions;

public static class ServiceRegistration
{
    public static void RegisterServices(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("arpellaDB");

        // Add API Explorer & Swagger for documentation
        serviceCollection.AddEndpointsApiExplorer();
        serviceCollection.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "ARPELLA STORES API", Description = "Building an ecommerce store", Version = "v1" });
        });

        //Configure DBContext
        serviceCollection.AddDbContext<ArpellaContext>(options => options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));
        serviceCollection.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options => options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

        // Configure Authentication and authorization services
        serviceCollection.AddIdentity<User, IdentityRole>().AddEntityFrameworkStores<ArpellaContext>().AddDefaultTokenProviders();
        serviceCollection.AddAuthentication();
        serviceCollection.AddAuthorization(options =>
        {
            options.AddPolicy("AdminPolicy", policy => policy.RequireRole("Admin"));
            options.AddPolicy("CustomerPolicy", policy => policy.RequireRole("Customer"));
            options.AddPolicy("InventoryManagerPolicy", policy => policy.RequireRole("Inventory Manager", "Admin"));
            options.AddPolicy("OrderManager", policy => policy.RequireRole("Order Manager", "Admin"));
            options.AddPolicy("DeliveryGuy", policy => policy.RequireRole("DeliveryGuy", "Admin"));
            options.AddPolicy("Accountant", policy => policy.RequireRole("Accountant", "Admin"));
        });
        serviceCollection.AddMemoryCache();

        serviceCollection.AddCors(options =>
        {
            options.AddDefaultPolicy(builder =>
            {
                builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
            });
            //options.AddPolicy("clientOrigin", builder => builder.WithOrigins("https://localhost:3000").AllowAnyHeader().AllowAnyMethod());
        });
        serviceCollection.Configure<MpesaSettings>(configuration.GetSection("MpesaConfig"));

        // Register application services
        serviceCollection.RegisterFeatureServices();

        // Register application endpoints services
        serviceCollection.RegisterRouteRegistrars();

        // Register RouteBuilder
        serviceCollection.AddScoped<RouteBuilder>();
    }
}
