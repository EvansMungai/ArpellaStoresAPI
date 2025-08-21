using ArpellaStores.Extensions;
using ArpellaStores.Extensions.ServiceHandlers;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

// Add environment variables
builder.Configuration.AddEnvironmentVariables();
DotNetEnv.Env.Load();


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


