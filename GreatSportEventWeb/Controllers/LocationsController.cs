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

    public IActionResult Index()
    {
        var locations = _context.Locations.ToListAsync();
        return View(locations);
    }
}