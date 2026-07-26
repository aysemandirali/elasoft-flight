using System.Reflection;
using FlightBooking.Services.FlightServices;
using FlightBooking.Settings;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// MongoDB ayarlarini appsettings.json'daki "DatabaseSettingsKey" bolumune bagla
builder.Services.Configure<DatabaseSettings>(builder.Configuration.GetSection("DatabaseSettingsKey"));

// Servislerde IDatabaseSettings isteyince ayarlarin degerini dondur
builder.Services.AddScoped<IDatabaseSettings>(sp =>
{
    return sp.GetRequiredService<IOptions<DatabaseSettings>>().Value;
});

// AutoMapper profillerini (GeneralMapping) uygulamaya tani
builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());

// Ucus servisini DI'a kaydet
builder.Services.AddScoped<IFlightService, FlightService>();

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

// Admin gibi alanlarin (Area) yonlendirmesi — default route'tan once gelmeli
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
