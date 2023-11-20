using Evaluation.Helpers;
using Evaluation.Repositories;
using Evaluation.Services;
using Microsoft.AspNetCore.Mvc.Infrastructure;


var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
// Add services to the container.
services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
});
services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
services.AddMemoryCache();
services.AddRazorPages();
services.AddMvc();
services.AddSingleton<DataContext>();
services.AddTransient<IVehicleRepository, VehicleRepository>();
services.AddTransient<IVehicleService, VehicleService>();
services.AddTransient<IBulkService, BulkService>();

services.AddControllers();
services.AddHttpContextAccessor();
services.AddTransient<IActionContextAccessor, ActionContextAccessor>();
services.AddScoped<IRazorRenderService, RazorRenderService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (!app.Environment.IsDevelopment())
//{

//}
app.UseMiddleware<AppExceptionHandlerMiddleware>();
app.UseExceptionHandler("/Error");
// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
app.UseHsts();
using var scope = app.Services.CreateScope();
var context = scope.ServiceProvider.GetRequiredService<DataContext>();
await context.Init();


app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();
app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
