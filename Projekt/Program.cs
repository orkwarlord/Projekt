using Microsoft.EntityFrameworkCore;
using Projekt.Data;
using Projekt.Models;

var builder = WebApplication.CreateBuilder(args);

//MVC + Views
builder.Services.AddControllersWithViews();

//DbContext (SQL Server)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//Identity (login/register)
builder.Services.AddDefaultIdentity<AppUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>();

//Razor Pages
builder.Services.AddRazorPages();

var app = builder.Build();

//Konfiguracja HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

//Identity UI endpoints
app.MapRazorPages();

app.Run();
