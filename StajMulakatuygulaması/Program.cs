using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using Radzen;
using StajMulakatuygulaması.Components;
using StajMulakatuygulamasi.Models;
using StajMulakatuygulaması.Models;
using StajMulakatuygulamasi.Services;

var builder = WebApplication.CreateBuilder(args);

// --- 1. SERVİSLER ---

// Blazor Servisleri
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Radzen
builder.Services.AddRadzenComponents();

// --- KİMLİK DOĞRULAMA (AUTH) AYARLARI ---

// Yetkilendirme Çekirdeği (Middleware gerektirmez)
builder.Services.AddAuthorizationCore();
builder.Services.AddAuthentication();
builder.Services.AddCascadingAuthenticationState();

// BİZİM ÖZEL AUTH PROVIDER (Standart Cookie yerine bunu kullanıyoruz)
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();

// --- DİĞER SERVİSLER ---
builder.Services.AddScoped<SystemSettingService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<HocaMusaitlikService>();
builder.Services.AddScoped<SchedulingDataService>();
builder.Services.AddScoped<SchedulingService>();
builder.Services.AddScoped<OgrenciService>();
builder.Services.AddScoped<BildirimService>();

// Veritabanı
var connectionString = "Server=localhost\\SQLEXPRESS;Database=StajMulakatDB;Trusted_Connection=True;TrustServerCertificate=True;";
builder.Services.AddDbContextFactory<StajMulakatDbContext>(options =>
    options.UseSqlServer(connectionString));

var app = builder.Build();

// --- 2. MIDDLEWARE (BORU HATTI) ---

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();


// --- 3. UÇ NOKTA ---

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();