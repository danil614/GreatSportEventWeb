using GreatSportEventWeb.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Получаем строку подключения из файла конфигурации
var connection = builder.Configuration.GetConnectionString("DefaultConnection");

// Добавляем контекст ApplicationContext в качестве сервиса в приложение
builder.Services.AddDbContext<ApplicationContext>(options =>
{
    if (connection != null) options.UseMySQL(connection);
});

var app = builder.Build();

// app.MapGet("/api/locations", async (ApplicationContext db) => await db.Locations.ToListAsync());
// app.MapGet("/api/cities", async (ApplicationContext db) => await db.Cities.ToListAsync());

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    "default",
    "{controller=Home}/{action=Index}/{id?}");

app.Run();