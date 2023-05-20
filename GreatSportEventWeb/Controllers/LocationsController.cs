using GreatSportEventWeb.Data;
using GreatSportEventWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace GreatSportEventWeb.Controllers;

public class LocationsController : Controller
{
    private readonly ApplicationContext _context;
    private readonly IMemoryCache _cache;

    public LocationsController(ApplicationContext context, IMemoryCache cache)
    {
        _context = context;
        _cache = cache;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View(DatabaseScripts<Location>.GetCachedData(_context, _cache, Location.TypeName));
    }
    
    [HttpGet]
    public IActionResult GetSortedData(string sortBy, string sortDirection, bool clearCache)
    {
        if (clearCache)
        {
            _cache.Remove(Location.TypeName);
        }

        var data = DatabaseScripts<Location>.GetCachedData(_context, _cache, Location.TypeName);

        data = sortBy switch
        {
            "name" => sortDirection == "asc" ? data.OrderBy(l => l.Name) : data.OrderByDescending(l => l.Name),
            "city" => sortDirection == "asc" ? data.OrderBy(l => l.City.Name) : data.OrderByDescending(l => l.City.Name),
            "address" => sortDirection == "asc" ? data.OrderBy(l => l.Address) : data.OrderByDescending(l => l.Address),
            "type" => sortDirection == "asc" ? data.OrderBy(l => l.Type.Name) : data.OrderByDescending(l => l.Type.Name),
            "capacity" => sortDirection == "asc" ? data.OrderBy(l => l.Capacity) : data.OrderByDescending(l => l.Capacity),
            _ => data
        };

        return PartialView("_Table", data);
    }
    
    [HttpPost]
    public IActionResult DeleteItem(int id)
    {
        var item = _context.Locations.FirstOrDefault(item => item.Id == id);
        if (item == null)
        {
            return NotFound(); // Если запись не найдена, возвращаем ошибку 404
        }

        _context.Locations.Remove(item);
        var rowsAffected = _context.SaveChanges();
        
        // При удалении записи из базы данных, очищаем кэш
        _cache.Remove(Location.TypeName);

        return rowsAffected > 0 ? Ok() : StatusCode(500);
    }

    [HttpGet]
    public IActionResult GetItem(int id)
    {
        var item = _context.Locations.FirstOrDefault(item => item.Id == id);
        if (item == null)
        {
            return NotFound(); // Если запись не найдена, возвращаем ошибку 404
        }

        ViewBag.Cities = _context.Cities; // -----------------------------
        ViewBag.Types = _context.Types;

        return PartialView("Form", item);
    }

    [HttpPost]
    public IActionResult SaveItem(Location item)
    {
        if (ModelState.IsValid)
        {
            return Redirect("Index");
        }
        else
        {
            return PartialView("Form", item);
        }
    }
}