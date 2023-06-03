using GreatSportEventWeb.Data;
using GreatSportEventWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace GreatSportEventWeb.Controllers;

[Authorize]
public class SeatsController : Controller
{
    private readonly IMemoryCache _cache;
    private readonly ApplicationContext _context;

    public SeatsController(ApplicationContext context, IMemoryCache cache)
    {
        _context = context;
        _cache = cache;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View(DatabaseScripts<Seat>.GetCachedData(_context, _cache));
    }

    [HttpGet]
    public IActionResult GetSortedData(string sortBy, string sortDirection, bool clearCache)
    {
        if (clearCache) _cache.Remove(typeof(Seat));

        var data = DatabaseScripts<Seat>.GetCachedData(_context, _cache);

        data = sortBy switch
        {
            "sport-event" => sortDirection == "asc"
                ? data.OrderBy(item => item.SportEvent!.DateTime)
                : data.OrderByDescending(item => item.SportEvent!.DateTime),
            "name" => sortDirection == "asc"
                ? data.OrderBy(item => item.Name)
                : data.OrderByDescending(item => item.Name),
            "price" => sortDirection == "asc"
                ? data.OrderBy(item => item.Price)
                : data.OrderByDescending(item => item.Price),
            "is-occupied" => sortDirection == "asc"
                ? data.OrderBy(item => item.IsOccupied)
                : data.OrderByDescending(item => item.IsOccupied),
            _ => data
        };

        return PartialView("_Table", data);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public IActionResult DeleteItem(int id)
    {
        var item = _context.Seats.FirstOrDefault(item => item.Id == id);
        if (item == null) return NotFound(); // Если запись не найдена, возвращаем ошибку 404

        _context.Seats.Remove(item);
        var rowsAffected = _context.SaveChanges();

        // При удалении записи из базы данных, очищаем кэш
        _cache.Remove(typeof(Seat));

        return rowsAffected > 0 ? Ok() : StatusCode(StatusCodes.Status500InternalServerError);
    }

    [HttpGet]
    public IActionResult GetItem(int id)
    {
        var item = _context.Seats.FirstOrDefault(item => item.Id == id);
        if (item == null) return NotFound(); // Если запись не найдена, возвращаем ошибку 404

        ViewBag.SportEvents = DatabaseScripts<SportEvent>.GetCachedData(_context, _cache).OrderBy(se => se.DateTime);
        ViewBag.Edit = true;

        return PartialView("Form", item);
    }

    [HttpGet]
    public IActionResult CreateItem()
    {
        var item = new Seat();

        ViewBag.SportEvents = DatabaseScripts<SportEvent>.GetCachedData(_context, _cache).OrderBy(se => se.DateTime);
        ViewBag.Edit = false;

        return PartialView("Form", item);
    }

    [HttpPost]
    public IActionResult SaveItem(Seat item)
    {
        if (ModelState.IsValid)
        {
            _context.Seats.Update(item);
            var rowsAffected = _context.SaveChanges();

            // При обновлении записи в базе данных, очищаем кэш
            _cache.Remove(typeof(Seat));

            return rowsAffected > 0 ? Redirect("/Seats") : StatusCode(StatusCodes.Status500InternalServerError);
        }

        var errors = ModelState.Values.SelectMany(v => v.Errors);
        var errorMessage = "";

        foreach (var error in errors) errorMessage += error.ErrorMessage + "\n";

        return StatusCode(StatusCodes.Status500InternalServerError, new { errorMessage });
    }

    [HttpGet]
    public FileContentResult ExportToExcel()
    {
        var data = DatabaseScripts<Seat>.GetCachedData(_context, _cache).ToList();
        string[] columns = { "SportEvent", "Name", "Price", "IsOccupied" };
        var fileContent = ExcelExport.ExportExcel(data, columns, true);
        return File(fileContent ?? Array.Empty<byte>(), ExcelExport.ExcelContentType, "Seats.xlsx");
    }

    [HttpPost]
    public IActionResult CheckUnique([FromBody] Seat? item)
    {
        if (item == null) return Json(new { isUnique = true, isValid = false });

        var isUnique = !_context.Seats.Any(source =>
            source.Id != item.Id &&
            source.Name == item.Name &&
            source.SportEventId == item.SportEventId);

        return Json(new { isUnique, isValid = ModelState.IsValid });
    }
}