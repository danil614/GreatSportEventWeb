using GreatSportEventWeb.Data;
using GreatSportEventWeb.Models;
using Microsoft.AspNetCore.Mvc;
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

    public IActionResult Index()
    {
        return View(GetCachedLocations());
    }
    
    public IActionResult GetSortedData(string sortBy, string sortDirection)
    {
        var locations = GetCachedLocations();

        locations = sortBy switch
        {
            "name" => sortDirection == "asc" ? locations.OrderBy(l => l.Name) : locations.OrderByDescending(l => l.Name),
            "city" => sortDirection == "asc" ? locations.OrderBy(l => l.City.Name) : locations.OrderByDescending(l => l.City.Name),
            "address" => sortDirection == "asc" ? locations.OrderBy(l => l.Address) : locations.OrderByDescending(l => l.Address),
            "type" => sortDirection == "asc" ? locations.OrderBy(l => l.Type.Name) : locations.OrderByDescending(l => l.Type.Name),
            "capacity" => sortDirection == "asc" ? locations.OrderBy(l => l.Capacity) : locations.OrderByDescending(l => l.Capacity),
            _ => locations
        };

        return PartialView("_LocationTable", locations);
    }
    
    public IActionResult DeleteLocation(int id)
    {
        var location = _context.Locations.FirstOrDefault(l => l.Id == id);
        if (location == null)
        {
            return NotFound(); // Если запись не найдена, возвращаем ошибку 404
        }

        _context.Locations.Remove(location);
        var rowsAffected = _context.SaveChanges();
        
        // При удалении записи из базы данных, очищаем кэш
        _cache.Remove(CacheKey);

        return rowsAffected > 0 ? Ok() : StatusCode(500);
    }
    
    private IQueryable<Location> GetCachedLocations()
    {
        _cache.TryGetValue(CacheKey, out IQueryable<Location>? locations);
        
        if (locations == null)
        {
            locations = _context.Locations.ToList().AsQueryable();

            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromMinutes(10)); // Устанавливаем время жизни кэша

            _cache.Set(CacheKey, locations, cacheEntryOptions);
        }

        return locations;
    }
}