using GreatSportEventWeb.Data;
using GreatSportEventWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Memory;

namespace GreatSportEventWeb.Controllers;

public class LocationsController : Controller
{
    private const string CacheKey = "Locations";
    
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
        return View(GetCachedData());
    }
    
    [HttpGet]
    public IActionResult GetSortedData(string sortBy, string sortDirection, bool clearCache)
    {
        if (clearCache)
        {
            _cache.Remove(CacheKey);
        }
        
        var data = GetCachedData();

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
        _cache.Remove(CacheKey);

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

        return PartialView("Form", item);
    }
    
    private IQueryable<Location> GetCachedData()
    {
        _cache.TryGetValue(CacheKey, out IQueryable<Location>? data);
        
        if (data == null)
        {
            //data = new List<Location>().AsQueryable();
            data = _context.Locations.ToList().AsQueryable();

            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromMinutes(10)); // Устанавливаем время жизни кэша

            _cache.Set(CacheKey, data, cacheEntryOptions);
        }

        return data;
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