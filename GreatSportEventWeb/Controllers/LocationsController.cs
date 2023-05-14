using GreatSportEventWeb.Data;
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
        return View();
    }
    
    public ActionResult GetLocations()
    {
        return Json(_context.Locations.ToList());
    }
}