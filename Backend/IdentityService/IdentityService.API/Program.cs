using IdentityService.API;
using IdentityService.API.Middlewares;
using IdentityService.BLL;
using IdentityService.DAL;
using IdentityService.DAL.Services.DbInitializer;
using System.Text.Json.Serialization;
using IdentityService.DAL.Abstractions.DbInitializer;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

services.AddControllers()
    .AddJsonOptions(options => options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

services.AddTransient<GlobalExceptionHandlingMiddleware>();
services.AddHttpContextAccessor();

services.AddAPI(builder.Configuration);
services.AddBLL(builder.Configuration);
services.AddDAL(builder.Configuration);

services.AddEndpointsApiExplorer()
    .AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
    var dbInitializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
    await dbInitializer.InitializeDb();
}

app.UseRouting();

app.UseHttpsRedirection();

app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();