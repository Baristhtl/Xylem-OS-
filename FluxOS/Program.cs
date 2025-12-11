using Microsoft.EntityFrameworkCore;
using FluxOS.Models;

var builder = WebApplication.CreateBuilder(args);

// Servisler
builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSession();

// Veritabanı
builder.Services.AddDbContext<FluxOsContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Ayarlar
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

// --- BURASI ARTIK TERTEMİZ ---
app.UseStaticFiles();
// -----------------------------

app.UseRouting();
app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();