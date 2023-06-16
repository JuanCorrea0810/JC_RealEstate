using RealEstate;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog();
var startup = new Startup(builder.Configuration);
startup.ConfigureServices(builder.Services);
var app = builder.Build();
startup.Configure(app, app.Environment);

app.Run();
