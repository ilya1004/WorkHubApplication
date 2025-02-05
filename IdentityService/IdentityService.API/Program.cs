using IdentityService.DAL;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

services.AddDataAccessLayer(builder.Configuration);

var app = builder.Build();


app.Run();
