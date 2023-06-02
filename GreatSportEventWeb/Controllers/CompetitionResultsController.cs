using GreatSportEventWeb.Data;
using GreatSportEventWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace GreatSportEventWeb.Controllers;

[Authorize]
public class CompetitionResultsController : Controller
{
    private readonly IMemoryCache _cache;
    private readonly ApplicationContext _context;

    public CompetitionResultsController(ApplicationContext context, IMemoryCache cache)
    {
        _context = context;
        _cache = cache;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View(DatabaseScripts<ParticipationEvent>.GetCachedData(_context, _cache));
    }

    [HttpGet]
    public IActionResult GetSortedData(string sortBy, string sortDirection, bool clearCache)
    {
        if (clearCache) _cache.Remove(typeof(ParticipationEvent));

        var data = DatabaseScripts<ParticipationEvent>.GetCachedData(_context, _cache);

        data = sortBy switch
        {
            "sport-event" => sortDirection == "asc"
                ? data.OrderBy(item => item.SportEvent!.ToString())
                : data.OrderByDescending(item => item.SportEvent!.ToString()),
            "team" => sortDirection == "asc"
                ? data.OrderBy(item => item.Team!.ToString())
                : data.OrderByDescending(item => item.Team!.ToString()),
            "score" => sortDirection == "asc"
                ? data.OrderBy(item => item.Score)
                : data.OrderByDescending(item => item.Score),
            _ => data
        };

        return PartialView("_Table", data);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public IActionResult DeleteItem(int sportEventId, int teamId)
    {
        var item = _context.ParticipationEvents.FirstOrDefault(item =>
            item.SportEventId == sportEventId && item.TeamId == teamId);
        if (item == null) return NotFound(); // Если запись не найдена, возвращаем ошибку 404

        _context.ParticipationEvents.Remove(item);
        var rowsAffected = _context.SaveChanges();

        // При удалении записи из базы данных, очищаем кэш
        _cache.Remove(typeof(ParticipationEvent));

        return rowsAffected > 0 ? Ok() : StatusCode(StatusCodes.Status500InternalServerError);
    }

    [HttpGet]
    public IActionResult GetItem(int sportEventId, int teamId)
    {
        var item = DatabaseScripts<ParticipationEvent>.GetCachedData(_context, _cache)
            .FirstOrDefault(item => item.SportEventId == sportEventId && item.TeamId == teamId);
        
        if (item == null) return NotFound(); // Если запись не найдена, возвращаем ошибку 404
        
        ViewBag.Edit = true;

        return PartialView("Form", item);
    }

    [HttpPost]
    public IActionResult SaveItem(ParticipationEvent item)
    {
        if (ModelState.IsValid)
        {
            _context.ParticipationEvents.Update(item);
            var rowsAffected = _context.SaveChanges();
            
            // При обновлении записи в базе данных, очищаем кэш
            _cache.Remove(typeof(ParticipationEvent));

            return rowsAffected > 0 ? Redirect("/CompetitionResults") : StatusCode(StatusCodes.Status500InternalServerError);
        }

        var errors = ModelState.Values.SelectMany(v => v.Errors);
        var errorMessage = "";

        foreach (var error in errors) errorMessage += error.ErrorMessage + "\n";

        return StatusCode(StatusCodes.Status500InternalServerError, new { errorMessage });
    }

    [HttpGet]
    public FileContentResult ExportToExcel()
    {
        var data = DatabaseScripts<ParticipationEvent>.GetCachedData(_context, _cache).ToList();
        string[] columns = { "SportEvent", "Team", "Score" };
        var fileContent = ExcelExport.ExportExcel(data, columns, true);
        return File(fileContent ?? Array.Empty<byte>(), ExcelExport.ExcelContentType, "CompetitionResults.xlsx");
    }

    [HttpPost]
    public IActionResult CheckUnique([FromBody] ParticipationEvent? item)
    {
        return Json(item == null
            ? new { isUnique = true, isValid = false }
            : new { isUnique = true, isValid = ModelState.IsValid });
    }
}