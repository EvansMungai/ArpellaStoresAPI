using ArpellaStores.Data;
using ArpellaStores.Helpers;
using ArpellaStores.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "ARPELLA STORES API", Description = "Building an ecommerce store", Version = "v1" });
});

var connectionString = builder.Configuration.GetConnectionString("arpellaDB");
builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options => options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
builder.Services.AddDbContext<ArpellaContext>(options => options.UseMySql(connectionString, ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("arpellaDB"))));
builder.Services.AddTransient<ICategoriesService, CategoriesService>();    
builder.Services.AddTransient<ISubcategoriesServices, SubcategoriesService>();    
builder.Services.AddTransient<IProductsService, ProductsService>();
builder.Services.AddTransient<IInventoryService, InventoryService>();
builder.Services.AddTransient<IFinalPriceService, FinalPriceService>();
builder.Services.AddTransient<IDiscountService, DiscountService>();
builder.Services.AddTransient<ICouponService, CouponService>();
builder.Services.AddTransient<IFlashsaleService, FlashsaleService>();
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


