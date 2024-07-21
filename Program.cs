using Microsoft.EntityFrameworkCore;
using Project_RentACar.DAO;
using Project_RentACar.Hubs;
using Project_RentACar.Models;

var builder = WebApplication.CreateBuilder(args);
// Add-start
builder.Services.AddControllersWithViews();
builder.Services.AddSignalR();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddDbContext<CarRentalDBContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MyCnn")));
builder.Services.AddScoped<AccountDAO>();
builder.Services.AddScoped<FinanceDAO>();
builder.Services.AddScoped<CarDAO>();
builder.Services.AddSession();
var app = builder.Build();
// update-Start
//app.MapGet("/", () => "Hello World!");
app.MapControllerRoute(name: "default",
    pattern: "{controller=Car}/{action=LoadCar}/{id?}");
// update end
app.UseStaticFiles();
app.UseSession();
app.MapHub<SignalR_Hub>("/signalR_Hub");
app.Run();