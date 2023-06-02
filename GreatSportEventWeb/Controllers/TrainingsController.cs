using GreatSportEventWeb.Data;
using GreatSportEventWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace GreatSportEventWeb.Controllers;

[Authorize]
public class TrainingsController : Controller
{
    private readonly IMemoryCache _cache;
    private readonly ApplicationContext _context;

    public TrainingsController(ApplicationContext context, IMemoryCache cache)
    {
        _context = context;
        _cache = cache;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View(DatabaseScripts<Training>.GetCachedData(_context, _cache));
    }

    [HttpGet]
    public IActionResult GetSortedData(string sortBy, string sortDirection, bool clearCache)
    {
        if (clearCache) _cache.Remove(typeof(Training));

        var data = DatabaseScripts<Training>.GetCachedData(_context, _cache);

        data = sortBy switch
        {
            "team" => sortDirection == "asc"
                ? data.OrderBy(item => item.Team!.ToString())
                : data.OrderByDescending(item => item.Team!.ToString()),
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
        var item = _context.Trainings.FirstOrDefault(item => item.Id == id);
        if (item == null) return NotFound(); // Если запись не найдена, возвращаем ошибку 404

        _context.Trainings.Remove(item);
        var rowsAffected = _context.SaveChanges();

        // При удалении записи из базы данных, очищаем кэш
        _cache.Remove(typeof(Training));

        return rowsAffected > 0 ? Ok() : StatusCode(StatusCodes.Status500InternalServerError);
    }

    [HttpGet]
    public IActionResult GetItem(int id)
    {
        var item = _context.Trainings.FirstOrDefault(item => item.Id == id);
        if (item == null) return NotFound(); // Если запись не найдена, возвращаем ошибку 404
        
        ViewBag.Locations = DatabaseScripts<Location>.GetCachedData(_context, _cache)
            .OrderBy(location => location.ToString());
        ViewBag.Teams = DatabaseScripts<Team>.GetCachedData(_context, _cache).OrderBy(team => team.Name);
        
        ViewBag.Edit = true;

        return PartialView("Form", item);
    }

    [HttpGet]
    public IActionResult CreateItem()
    {
        var item = new Training();

        ViewBag.Locations = DatabaseScripts<Location>.GetCachedData(_context, _cache)
            .OrderBy(location => location.ToString());
        ViewBag.Teams = DatabaseScripts<Team>.GetCachedData(_context, _cache).OrderBy(team => team.Name);
        
        ViewBag.Edit = false;

        return PartialView("Form", item);
    }

    [HttpPost]
    public IActionResult SaveItem(Training item)
    {
        if (ModelState.IsValid)
        {
            _context.Trainings.Update(item);
            var rowsAffected = _context.SaveChanges();
            
            // При обновлении записи в базе данных, очищаем кэш
            _cache.Remove(typeof(Training));

            return rowsAffected > 0 ? Redirect("/Trainings") : StatusCode(StatusCodes.Status500InternalServerError);
        }

        var errors = ModelState.Values.SelectMany(v => v.Errors);
        var errorMessage = "";

        foreach (var error in errors) errorMessage += error.ErrorMessage + "\n";

        return StatusCode(StatusCodes.Status500InternalServerError, new { errorMessage });
    }

    [HttpGet]
    public FileContentResult ExportToExcel()
    {
        var data = DatabaseScripts<Training>.GetCachedData(_context, _cache).ToList();
        string[] columns = { "Team", "Location", "DateTime", "Duration", "Description" };
        var fileContent = ExcelExport.ExportExcel(data, columns, true);
        return File(fileContent ?? Array.Empty<byte>(), ExcelExport.ExcelContentType, "Trainings.xlsx");
    }

    [HttpPost]
    public IActionResult CheckUnique([FromBody] Training? item)
    {
        if (item == null) return Json(new { isUnique = true, isValid = false });

        var isUnique = !_context.Trainings.Any(source =>
            source.Id != item.Id &&
            source.TeamId == item.TeamId &&
            source.LocationId == item.LocationId &&
            source.DateTime == item.DateTime);

        return Json(new { isUnique, isValid = ModelState.IsValid });
    }
}