using GreatSportEventWeb.Data;
using GreatSportEventWeb.Models;
using Microsoft.AspNetCore.Mvc;

namespace GreatSportEventWeb.Controllers;

public class LocationsController : Controller
{
    private readonly ApplicationContext _context;

    public LocationsController(ApplicationContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        return View(_context.Locations.ToList());
    }
    
    public IActionResult GetSortedData(string sortBy, string sortDirection)
    {
        IQueryable<Location> locations = _context.Locations;

        locations = sortBy switch
        {
            "name" => sortDirection == "asc" ? locations.OrderBy(l => l.Name) : locations.OrderByDescending(l => l.Name),
            "city" => sortDirection == "asc" ? locations.OrderBy(l => l.City.Name) : locations.OrderByDescending(l => l.City.Name),
            "address" => sortDirection == "asc" ? locations.OrderBy(l => l.Address) : locations.OrderByDescending(l => l.Address),
            "type" => sortDirection == "asc" ? locations.OrderBy(l => l.Type.Name) : locations.OrderByDescending(l => l.Type.Name),
            "capacity" => sortDirection == "asc" ? locations.OrderBy(l => l.Capacity) : locations.OrderByDescending(l => l.Capacity),
            _ => locations
        };

        return PartialView("_LocationTable", locations.ToList());
    }

    public void DeleteLocation(int id)
    {
        Console.WriteLine($"Удаление записи с id: {id}");
    }
}