using ArpellaStores.Extensions;
using ArpellaStores.Extensions.ServiceHandlers;
using DotNetEnv;


Env.Load();
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
var env = builder.Environment;
//Configure HTTPS
if (!env.IsDevelopment())
{
    builder.WebHost.ConfigureKestrel(options =>
    {
        options.ConfigureCustomHttps();
    });
}

// Register application services
builder.Services.RegisterServices(builder.Configuration);

var app = builder.Build();
// Configure the HTTP request pipeline.
app.ConfigureMiddleware();

app.Run();


