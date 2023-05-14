using GreatSportEventWeb.Models;
using Microsoft.EntityFrameworkCore;

namespace GreatSportEventWeb.Data;

public class ApplicationContext : DbContext
{
    public DbSet<Location> Locations { get; set; } = null!;
    
    public ApplicationContext()
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseMySQL(DatabaseConnection.GetConnectionString());
        optionsBuilder.LogTo(Console.WriteLine);
    }
}