using GreatSportEventWeb.Data;
using GreatSportEventWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Type = GreatSportEventWeb.Models.Type;

namespace GreatSportEventWeb.Controllers;

[Authorize]
public class SportEventsController : Controller
{
    private readonly IMemoryCache _cache;
    private readonly ApplicationContext _context;

    public SportEventsController(ApplicationContext context, IMemoryCache cache)
    {
        _context = context;
        _cache = cache;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View(DatabaseScripts<SportEvent>.GetCachedData(_context, _cache));
    }

    [HttpGet]
    public IActionResult GetSortedData(string sortBy, string sortDirection, bool clearCache)
    {
        if (clearCache) _cache.Remove(typeof(SportEvent));

        var data = DatabaseScripts<SportEvent>.GetCachedData(_context, _cache);

        data = sortBy switch
        {
            "type" => sortDirection == "asc" 
                ? data.OrderBy(item => item.Type!.ToString())
                : data.OrderByDescending(item => item.Type!.ToString()),
            "location" => sortDirection == "asc"
                ? data.OrderBy(item => item.Location!.ToString())
                : data.OrderByDescending(item => item.Location!.ToString()),
            "datetime" => sortDirection == "asc" 
                ? data.OrderBy(item => item.DateTime)
                : data.OrderByDescending(item => item.DateTime),
            "duration" => sortDirection == "asc"
                ? data.OrderBy(item => item.Duration)
                : data.OrderByDescending(item => item.Duration),
            _ => data
        };

        return PartialView("_Table", data);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public IActionResult DeleteItem(int id)
    {
        var item = _context.SportEvents.FirstOrDefault(item => item.Id == id);
        if (item == null) return NotFound(); // Если запись не найдена, возвращаем ошибку 404

        _context.SportEvents.Remove(item);
        var rowsAffected = _context.SaveChanges();

        // При удалении записи из базы данных, очищаем кэш
        _cache.Remove(typeof(SportEvent));

        return rowsAffected > 0 ? Ok() : StatusCode(StatusCodes.Status500InternalServerError);
    }

    [HttpGet]
    public IActionResult GetItem(int id)
    {
        var item = _context.SportEvents.FirstOrDefault(item => item.Id == id);
        if (item == null) return NotFound(); // Если запись не найдена, возвращаем ошибку 404

        ViewBag.Locations = DatabaseScripts<Location>.GetCachedData(_context, _cache).OrderBy(location => location.ToString());
        ViewBag.Types = DatabaseScripts<Type>.GetCachedData(_context, _cache).OrderBy(type => type.Name)
            .Where(type => type.TypeType == TypeType.SportEvent);
        ViewBag.Edit = true;

        return PartialView("Form", item);
    }

    [HttpGet]
    public IActionResult CreateItem()
    {
        var item = new SportEvent();

        ViewBag.Locations = DatabaseScripts<Location>.GetCachedData(_context, _cache).OrderBy(location => location.ToString());
        ViewBag.Types = DatabaseScripts<Type>.GetCachedData(_context, _cache).OrderBy(type => type.Name)
            .Where(type => type.TypeType == TypeType.SportEvent);
        ViewBag.Edit = false;

        return PartialView("Form", item);
    }

    [HttpPost]
    public IActionResult SaveItem(SportEvent item)
    {
        if (ModelState.IsValid)
        {
            _context.SportEvents.Update(item);
            var rowsAffected = _context.SaveChanges();

            // При обновлении записи в базе данных, очищаем кэш
            _cache.Remove(typeof(SportEvent));

            return rowsAffected > 0 ? Redirect("/SportEvents") : StatusCode(StatusCodes.Status500InternalServerError);
        }

        var errors = ModelState.Values.SelectMany(v => v.Errors);
        var errorMessage = "";

        foreach (var error in errors) errorMessage += error.ErrorMessage + "\n";
        
        return StatusCode(StatusCodes.Status500InternalServerError, new { errorMessage });
    }

    [HttpGet]
    public FileContentResult ExportToExcel()
    {
        var data = DatabaseScripts<SportEvent>.GetCachedData(_context, _cache).ToList();
        string[] columns = { "Type", "Location", "DateTime", "Duration", "Description" };
        var fileContent = ExcelExport.ExportExcel(data, columns, true);
        return File(fileContent ?? Array.Empty<byte>(), ExcelExport.ExcelContentType, "SportEvents.xlsx");
    }
    
    [HttpPost]
    public IActionResult CheckUnique([FromBody] SportEvent? item)
    {
        if (item == null) return Json(new { isUnique = true, isValid = false });
        
        var isUnique = !_context.SportEvents.Any(source =>
            source.Id != item.Id &&
            source.TypeId == item.TypeId &&
            source.LocationId == item.LocationId &&
            source.DateTime == item.DateTime);

        return Json(new { isUnique, isValid = ModelState.IsValid });
    }
}