using GreatSportEventWeb.Data;
using GreatSportEventWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Type = GreatSportEventWeb.Models.Type;

namespace GreatSportEventWeb.Controllers;

[Authorize]
public class LocationsController : Controller
{
    private readonly IMemoryCache _cache;
    private readonly ApplicationContext _context;

    public LocationsController(ApplicationContext context, IMemoryCache cache)
    {
        _context = context;
        _cache = cache;
    }

    [HttpGet]
    [Authorize(Roles = "Seller")]
    public IActionResult Index()
    {
        return View(DatabaseScripts<Location>.GetCachedData(_context, _cache));
    }

    [HttpGet]
    public IActionResult GetSortedData(string sortBy, string sortDirection, bool clearCache)
    {
        if (clearCache) _cache.Remove(typeof(Location));

        var data = DatabaseScripts<Location>.GetCachedData(_context, _cache);

        data = sortBy switch
        {
            "name" => sortDirection == "asc" 
                ? data.OrderBy(item => item.Name)
                : data.OrderByDescending(item => item.Name),
            "city" => sortDirection == "asc"
                ? data.OrderBy(item => item.City!.Name)
                : data.OrderByDescending(item => item.City!.Name),
            "address" => sortDirection == "asc" 
                ? data.OrderBy(item => item.Address)
                : data.OrderByDescending(item => item.Address),
            "type" => sortDirection == "asc"
                ? data.OrderBy(item => item.Type!.Name)
                : data.OrderByDescending(item => item.Type!.Name),
            "capacity" => sortDirection == "asc"
                ? data.OrderBy(item => item.Capacity)
                : data.OrderByDescending(item => item.Capacity),
            _ => data
        };

        return PartialView("_Table", data);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public IActionResult DeleteItem(int id)
    {
        var item = _context.Locations.FirstOrDefault(item => item.Id == id);
        if (item == null) return NotFound(); // Если запись не найдена, возвращаем ошибку 404

        _context.Locations.Remove(item);
        var rowsAffected = _context.SaveChanges();

        // При удалении записи из базы данных, очищаем кэш
        _cache.Remove(typeof(Location));

        return rowsAffected > 0 ? Ok() : StatusCode(StatusCodes.Status500InternalServerError);
    }

    [HttpGet]
    public IActionResult GetItem(int id)
    {
        var item = _context.Locations.FirstOrDefault(item => item.Id == id);
        if (item == null) return NotFound(); // Если запись не найдена, возвращаем ошибку 404

        ViewBag.Cities = DatabaseScripts<City>.GetCachedData(_context, _cache).OrderBy(city => city.Name);
        ViewBag.Types = DatabaseScripts<Type>.GetCachedData(_context, _cache).OrderBy(type => type.Name);
        ViewBag.Edit = true;

        return PartialView("Form", item);
    }

    [HttpGet]
    public IActionResult CreateItem()
    {
        var item = new Location();

        ViewBag.Cities = DatabaseScripts<City>.GetCachedData(_context, _cache).OrderBy(city => city.Name);
        ViewBag.Types = DatabaseScripts<Type>.GetCachedData(_context, _cache).OrderBy(type => type.Name);
        ViewBag.Edit = false;

        return PartialView("Form", item);
    }

    [HttpPost]
    public IActionResult SaveItem(Location item)
    {
        if (ModelState.IsValid)
        {
            _context.Locations.Update(item);
            var rowsAffected = _context.SaveChanges();

            // При обновлении записи в базе данных, очищаем кэш
            _cache.Remove(typeof(Location));

            return rowsAffected > 0 ? Redirect("/Locations") : StatusCode(StatusCodes.Status500InternalServerError);
        }

        var errors = ModelState.Values.SelectMany(v => v.Errors);
        var errorMessage = "";

        foreach (var error in errors) errorMessage += error.ErrorMessage + "\n";
        
        return StatusCode(StatusCodes.Status500InternalServerError, new { errorMessage });
    }

    [HttpGet]
    public FileContentResult ExportToExcel()
    {
        var data = DatabaseScripts<Location>.GetCachedData(_context, _cache).ToList();
        string[] columns = { "Name", "City", "Address", "Type", "Capacity", "Description" };
        var fileContent = ExcelExport.ExportExcel(data, columns, true);
        return File(fileContent ?? Array.Empty<byte>(), ExcelExport.ExcelContentType, "Locations.xlsx");
    }
}