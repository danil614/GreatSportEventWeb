using GreatSportEventWeb.Data;
using GreatSportEventWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace GreatSportEventWeb.Controllers;

[Authorize]
public class ViewersController : Controller
{
    private readonly IMemoryCache _cache;
    private readonly ApplicationContext _context;

    public ViewersController(ApplicationContext context, IMemoryCache cache)
    {
        _context = context;
        _cache = cache;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View(DatabaseScripts<Viewer>.GetCachedData(_context, _cache));
    }

    [HttpGet]
    public IActionResult GetSortedData(string sortBy, string sortDirection, bool clearCache)
    {
        if (clearCache) _cache.Remove(typeof(Viewer));

        var data = DatabaseScripts<Viewer>.GetCachedData(_context, _cache);

        data = sortBy switch
        {
            "full-name" => sortDirection == "asc"
                ? data.OrderBy(item => item.ToString())
                : data.OrderByDescending(item => item.ToString()),
            "gender" => sortDirection == "asc"
                ? data.OrderBy(item => item.Gender)
                : data.OrderByDescending(item => item.Gender),
            "phone-number" => sortDirection == "asc"
                ? data.OrderBy(item => item.PhoneNumber)
                : data.OrderByDescending(item => item.PhoneNumber),
            "birth-date" => sortDirection == "asc"
                ? data.OrderBy(item => item.BirthDate)
                : data.OrderByDescending(item => item.BirthDate),
            "number-purchases" => sortDirection == "asc"
                ? data.OrderBy(item => item.NumberPurchases)
                : data.OrderByDescending(item => item.NumberPurchases),
            _ => data
        };

        return PartialView("_Table", data);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public IActionResult DeleteItem(int id)
    {
        var item = _context.Viewers.FirstOrDefault(item => item.Id == id);
        if (item == null) return NotFound(); // Если запись не найдена, возвращаем ошибку 404

        _context.Viewers.Remove(item);
        var rowsAffected = _context.SaveChanges();

        // При удалении записи из базы данных, очищаем кэш
        _cache.Remove(typeof(Viewer));

        return rowsAffected > 0 ? Ok() : StatusCode(StatusCodes.Status500InternalServerError);
    }

    [HttpGet]
    public IActionResult GetItem(int id)
    {
        var item = _context.Viewers.FirstOrDefault(item => item.Id == id);
        if (item == null) return NotFound(); // Если запись не найдена, возвращаем ошибку 404
        
        ViewBag.Gender = DatabaseScripts<Gender>.GetCachedData(_context, _cache).OrderBy(gender => gender.Name);
        ViewBag.Edit = true;

        return PartialView("Form", item);
    }

    [HttpGet]
    public IActionResult CreateItem()
    {
        var item = new Viewer();

        ViewBag.Gender = DatabaseScripts<Gender>.GetCachedData(_context, _cache).OrderBy(gender => gender.Name);
        ViewBag.Edit = false;

        return PartialView("Form", item);
    }

    [HttpPost]
    public IActionResult SaveItem(Viewer item)
    {
        if (ModelState.IsValid)
        {
            _context.Viewers.Update(item);
            var rowsAffected = _context.SaveChanges();

            // При обновлении записи в базе данных, очищаем кэш
            _cache.Remove(typeof(Viewer));

            return rowsAffected > 0 ? Redirect("/Viewers") : StatusCode(StatusCodes.Status500InternalServerError);
        }

        var errors = ModelState.Values.SelectMany(v => v.Errors);
        var errorMessage = "";

        foreach (var error in errors) errorMessage += error.ErrorMessage + "\n";

        return StatusCode(StatusCodes.Status500InternalServerError, new { errorMessage });
    }

    [HttpGet]
    public FileContentResult ExportToExcel()
    {
        var data = DatabaseScripts<Viewer>.GetCachedData(_context, _cache).ToList();
        string[] columns =
        {
            "Surname", "Name", "Patronymic", "Gender", "PhoneNumber", "BirthDate", "NumberPurchases"
        };
        var fileContent = ExcelExport.ExportExcel(data, columns, true);
        return File(fileContent ?? Array.Empty<byte>(), ExcelExport.ExcelContentType, "Viewers.xlsx");
    }

    [HttpPost]
    public IActionResult CheckUnique([FromBody] Viewer? item)
    {
        if (item == null) return Json(new { isUnique = true, isValid = false });

        var isUnique = !_context.Viewers.Any(source =>
            source.Id != item.Id &&
            source.Surname == item.Surname &&
            source.Name == item.Name &&
            source.Patronymic == item.Patronymic &&
            source.GenderId == item.GenderId &&
            source.BirthDate == item.BirthDate);

        return Json(new { isUnique, isValid = ModelState.IsValid });
    }
}