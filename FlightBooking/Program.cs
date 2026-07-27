using System.Reflection;
using FlightBooking.AgentServices;
using FlightBooking.Services.AccountServices;
using FlightBooking.Services.BookingServices;
using Microsoft.AspNetCore.Authentication.Cookies;
using FlightBooking.Services.CheckInServices;
using FlightBooking.Services.FlightServices;
using FlightBooking.Services.NoShowServices;
using FlightBooking.Settings;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Gizli anahtarlari tutan yerel ayar dosyasi (git'e gitmez, opsiyonel)
builder.Configuration.AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: true);

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

// Rezervasyon servisini DI'a kaydet
builder.Services.AddScoped<IBookingService, BookingService>();

// Check-in servisini DI'a kaydet
builder.Services.AddScoped<ICheckInService, CheckInService>();

// Uyelik/giris servisi + cerez tabanli kimlik dogrulama
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/Login";
    });

// No-Show / Overbooking servisi
builder.Services.AddScoped<INoShowService, NoShowService>();

// ML.NET no-show tahmin servisi (model bir kez egitilir -> Singleton)
builder.Services.AddSingleton<FlightBooking.Services.MachineLearningServices.NoShowMlService>();

// ML.NET ucus talep tahmin servisi
builder.Services.AddSingleton<FlightBooking.Services.MachineLearningServices.FlightDemandMlService>();

// Gemini (AI asistan) ayarlari ve servisi
builder.Services.Configure<GeminiSettings>(builder.Configuration.GetSection("Gemini"));
builder.Services.AddHttpClient<IGeminiService, GeminiService>();

// AI Agent: hava durumu araci (tool) + orkestra servisi
builder.Services.AddHttpClient<IWeatherTool, WeatherTool>();
builder.Services.AddScoped<ITravelAgentService, TravelAgentService>();

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

app.UseAuthentication();
app.UseAuthorization();

// Klasik statik dosya sunumu (wwwroot altindaki css/js/img) — en guvenilir yontem
app.UseStaticFiles();

// Admin gibi alanlarin (Area) yonlendirmesi — default route'tan once gelmeli
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Default}/{action=Index}/{id?}");


app.Run();
