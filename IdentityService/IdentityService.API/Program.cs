using IdentityService.API;
using IdentityService.API.Middlewares;
using IdentityService.DAL;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

services.AddAPI();

services.AddDAL(builder.Configuration);

var app = builder.Build();


app.UseRouting();

app.UseMiddleware<GlobalExceptionHandlingMiddleware>();


app.MapControllers();

app.Run();
