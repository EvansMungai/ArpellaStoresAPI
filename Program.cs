using ArpellaStores.Data;
using ArpellaStores.Helpers;
using ArpellaStores.Models;
using ArpellaStores.Services;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Identity;
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
builder.Services.AddDbContext<ArpellaContext>(options => options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));
builder.Services.AddIdentity<User, IdentityRole>().AddEntityFrameworkStores<ArpellaContext>().AddDefaultTokenProviders();
builder.Services.AddAuthentication();
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminPolicy", policy => policy.RequireRole("Admin"));
    options.AddPolicy("CustomerPolicy", policy => policy.RequireRole("Customer"));
    options.AddPolicy("InventoryManagerPolicy", policy => policy.RequireRole("Inventory Manager", "Admin"));
    options.AddPolicy("OrderManager", policy => policy.RequireRole("Order Manager", "Admin"));
    options.AddPolicy("DeliveryGuy", policy => policy.RequireRole("DeliveryGuy", "Admin"));
    options.AddPolicy("Accountant", policy => policy.RequireRole("Accountant", "Admin"));
});
builder.Services.AddTransient<ICategoriesService, CategoriesService>();
builder.Services.AddTransient<ISubcategoriesServices, SubcategoriesService>();
builder.Services.AddTransient<IProductsService, ProductsService>();
builder.Services.AddTransient<IInventoryService, InventoryService>();
builder.Services.AddTransient<IFinalPriceService, FinalPriceService>();
builder.Services.AddTransient<IDiscountService, DiscountService>();
builder.Services.AddTransient<ICouponService, CouponService>();
builder.Services.AddTransient<IFlashsaleService, FlashsaleService>();
builder.Services.AddTransient<ICloudinaryService, CloudinaryService>();
builder.Services.AddTransient<IUserManagementService, UserManagementService>();
builder.Services.AddTransient<IAuthenticationService, AuthenticationService>();
builder.Services.AddTransient<IRouteResolutionHelper, RouteResolutionHelper>();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
    //options.AddPolicy("clientOrigin", builder => builder.WithOrigins("https://localhost:3000").AllowAnyHeader().AllowAnyMethod());
});



var app = builder.Build();
// Configure the HTTP request pipeline.
//app.UseCors("clientOrigin");
app.UseCors();
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

var resolutionHelper = app.Services.CreateScope().ServiceProvider.GetService<IRouteResolutionHelper>();
resolutionHelper.addMappings(app);

app.Run();


