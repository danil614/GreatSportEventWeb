using GreatSportEventWeb.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
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
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<SportEvent> SportEvents { get; set; } = null!;
    public DbSet<Team> Teams { get; set; } = null!;
    public DbSet<Position> Positions { get; set; } = null!;
    public DbSet<Athlete> Athletes { get; set; } = null!;
    public DbSet<ParticipationEvent> ParticipationEvents { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.LogTo(Console.WriteLine);
        optionsBuilder.UseLazyLoadingProxies();
        //optionsBuilder.ConfigureWarnings(warnings => warnings.Ignore(CoreEventId.LazyLoadOnDisposedContextWarning));
    }
}