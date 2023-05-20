using GreatSportEventWeb.Models;
using Microsoft.EntityFrameworkCore;
using Type = GreatSportEventWeb.Models.Type;

namespace GreatSportEventWeb.Data;

public class ApplicationContext : DbContext
{
    public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
    {
    }

    public DbSet<Location> Locations { get; set; } = null!;
    public DbSet<City> Cities { get; set; } = null!;
    public DbSet<Type> Types { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.LogTo(Console.WriteLine);
        optionsBuilder.UseLazyLoadingProxies();
    }
}