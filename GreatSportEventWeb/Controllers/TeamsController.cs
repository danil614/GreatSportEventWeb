using GreatSportEventWeb.Data;
using GreatSportEventWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace GreatSportEventWeb.Controllers;

[Authorize]
public class TeamsController : Controller
{
    private readonly IMemoryCache _cache;
    private readonly ApplicationContext _context;

    public TeamsController(ApplicationContext context, IMemoryCache cache)
    {
        _context = context;
        _cache = cache;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View(DatabaseScripts<Team>.GetCachedData(_context, _cache));
    }

    [HttpGet]
    public IActionResult GetSortedData(string sortBy, string sortDirection, bool clearCache)
    {
        if (clearCache) _cache.Remove(typeof(Team));

        var data = DatabaseScripts<Team>.GetCachedData(_context, _cache);

        data = sortBy switch
        {
            "name" => sortDirection == "asc" 
                ? data.OrderBy(item => item.Name)
                : data.OrderByDescending(item => item.Name),
            "location" => sortDirection == "asc"
                ? data.OrderBy(item => item.Location!.ToString())
                : data.OrderByDescending(item => item.Location!.ToString()),
            "come-from" => sortDirection == "asc" 
                ? data.OrderBy(item => item.ComeFrom)
                : data.OrderByDescending(item => item.ComeFrom),
            "rating" => sortDirection == "asc"
                ? data.OrderBy(item => item.Rating)
                : data.OrderByDescending(item => item.Rating),
            _ => data
        };

        return PartialView("_Table", data);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public IActionResult DeleteItem(int id)
    {
        var item = _context.Teams.FirstOrDefault(item => item.Id == id);
        if (item == null) return NotFound(); // Если запись не найдена, возвращаем ошибку 404

        _context.Teams.Remove(item);
        var rowsAffected = _context.SaveChanges();

        // При удалении записи из базы данных, очищаем кэш
        _cache.Remove(typeof(Team));

        return rowsAffected > 0 ? Ok() : StatusCode(StatusCodes.Status500InternalServerError);
    }

    [HttpGet]
    public IActionResult GetItem(int id)
    {
        var item = _context.Teams.FirstOrDefault(item => item.Id == id);
        if (item == null) return NotFound(); // Если запись не найдена, возвращаем ошибку 404

        ViewBag.Locations = DatabaseScripts<Location>.GetCachedData(_context, _cache).OrderBy(location => location.ToString());
        ViewBag.Edit = true;

        return PartialView("Form", item);
    }

    [HttpGet]
    public IActionResult CreateItem()
    {
        var item = new Team();

        ViewBag.Locations = DatabaseScripts<Location>.GetCachedData(_context, _cache).OrderBy(location => location.ToString());
        ViewBag.Edit = false;

        return PartialView("Form", item);
    }

    [HttpPost]
    public IActionResult SaveItem(Team item)
    {
        if (ModelState.IsValid)
        {
            _context.Teams.Update(item);
            var rowsAffected = _context.SaveChanges();

            // При обновлении записи в базе данных, очищаем кэш
            _cache.Remove(typeof(Team));

            return rowsAffected > 0 ? Redirect("/Teams") : StatusCode(StatusCodes.Status500InternalServerError);
        }

        var errors = ModelState.Values.SelectMany(v => v.Errors);
        var errorMessage = "";

        foreach (var error in errors) errorMessage += error.ErrorMessage + "\n";
        
        return StatusCode(StatusCodes.Status500InternalServerError, new { errorMessage });
    }

    [HttpGet]
    public FileContentResult ExportToExcel()
    {
        var data = DatabaseScripts<Team>.GetCachedData(_context, _cache).ToList();
        string[] columns = { "Name", "Location", "ComeFrom", "Rating", "Description" };
        var fileContent = ExcelExport.ExportExcel(data, columns, true);
        return File(fileContent ?? Array.Empty<byte>(), ExcelExport.ExcelContentType, "Teams.xlsx");
    }
    
    [HttpPost]
    public IActionResult CheckUnique([FromBody] Team? item)
    {
        if (item == null) return Json(new { isUnique = true, isValid = false });
        
        var isUnique = !_context.Teams.Any(source =>
            source.Id != item.Id &&
            source.Name == item.Name &&
            source.LocationId == item.LocationId &&
            source.ComeFrom == item.ComeFrom);

        return Json(new { isUnique, isValid = ModelState.IsValid });
    }
}