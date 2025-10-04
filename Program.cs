using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PortalAcademico.Data;
using PortalAcademico.Models;

var builder = WebApplication.CreateBuilder(args);

// Conexión a SQLite
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Identity con soporte para roles
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
    options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

// Configurar cookies de Identity (opcional, pero buena práctica)
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login";
    options.LogoutPath = "/Identity/Account/Logout";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
});

builder.Services.AddControllersWithViews();

// Agregar servicios de sesión
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
// Habilitar sesiones
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

// Ruta por defecto: ir al catálogo de cursos
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

// Sembrar datos iniciales (3 cursos + Coordinador)
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await SeedData.Initialize(services);
}

app.Run();