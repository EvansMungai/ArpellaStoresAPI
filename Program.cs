using ArpellaStores.Data;
using ArpellaStores.Helpers;
using ArpellaStores.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "ARPELLA STORES API", Description = "Building an ecommerce store", Version = "v1" });
});
var connectionString = builder.Configuration.GetConnectionString("arpella");
builder.Services.AddDbContext<ArpellaContext>(options => options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));
builder.Services.AddTransient<IProductManagement, ProductManagement>();
builder.Services.AddTransient<IRouteResolutionHelper, RouteResolutionHelper>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "ARPELLA STORES API V1");
    });
}

var resolutionHelper = app.Services.CreateScope().ServiceProvider.GetService<IRouteResolutionHelper>();
resolutionHelper.addMappings(app);

app.Run();


