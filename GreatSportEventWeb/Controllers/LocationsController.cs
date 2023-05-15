using GreatSportEventWeb.Data;
using GreatSportEventWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GreatSportEventWeb.Controllers;

public class LocationsController : Controller
{
    private readonly ApplicationContext _context;

    public LocationsController(ApplicationContext context)
    {
        _context = context;
    }

    public IActionResult Index(string sort)
    {
        IQueryable<Location> locations = _context.Locations;

        locations = sort switch
        {
            "name" => locations.OrderBy(l => l.Name),
            "city" => locations.OrderBy(l => l.City.Name),
            "address" => locations.OrderBy(l => l.Address),
            "type" => locations.OrderBy(l => l.Type.Name),
            "capacity" => locations.OrderBy(l => l.Capacity),
            _ => locations
        };

        return View(locations.ToList());
    }
}