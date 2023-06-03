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
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<SportEvent> SportEvents { get; set; } = null!;
    public DbSet<Team> Teams { get; set; } = null!;
    public DbSet<Position> Positions { get; set; } = null!;
    public DbSet<Athlete> Athletes { get; set; } = null!;
    public DbSet<ParticipationEvent> ParticipationEvents { get; set; } = null!;
    public DbSet<Training> Trainings { get; set; } = null!;
    public DbSet<Employee> Employees { get; set; } = null!;
    public DbSet<OrganisationEvent> OrganisationEvents { get; set; } = null!;
    public DbSet<Viewer> Viewers { get; set; } = null!;
    public DbSet<Seat> Seats { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.LogTo(Console.WriteLine);
    }
}