using GreatSportEventWeb.Data;
using GreatSportEventWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace GreatSportEventWeb.Controllers;

[Authorize]
public class CitiesController : Controller
{
    private readonly IMemoryCache _cache;
    private readonly ApplicationContext _context;

    public CitiesController(ApplicationContext context, IMemoryCache cache)
    {
        _context = context;
        _cache = cache;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View(DatabaseScripts<City>.GetCachedData(_context, _cache));
    }

    [HttpGet]
    public IActionResult GetSortedData(string sortBy, string sortDirection, bool clearCache)
    {
        if (clearCache) _cache.Remove(typeof(City));

        var data = DatabaseScripts<City>.GetCachedData(_context, _cache);

        data = sortBy switch
        {
            "id" => sortDirection == "asc"
                ? data.OrderBy(item => item.Id) 
                : data.OrderByDescending(item => item.Id),
            "name" => sortDirection == "asc"
                ? data.OrderBy(item => item.Name) 
                : data.OrderByDescending(item => item.Name),
            _ => data
        };

        return PartialView("_Table", data);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public IActionResult DeleteItem(int id)
    {
        var item = _context.Cities.FirstOrDefault(item => item.Id == id);
        if (item == null) return NotFound(); // Если запись не найдена, возвращаем ошибку 404

        _context.Cities.Remove(item);
        var rowsAffected = _context.SaveChanges();

        // При удалении записи из базы данных, очищаем кэш
        _cache.Remove(typeof(City));

        return rowsAffected > 0 ? Ok() : StatusCode(StatusCodes.Status500InternalServerError);
    }

    [HttpGet]
    public IActionResult GetItem(long id)
    {
        var item = _context.Cities.FirstOrDefault(item => item.Id == id);
        if (item == null) return NotFound(); // Если запись не найдена, возвращаем ошибку 404
        ViewBag.Edit = true;
        
        return PartialView("Form", item);
    }

    [HttpGet]
    public IActionResult CreateItem()
    {
        var item = new City();
        ViewBag.Edit = false;
        
        return PartialView("Form", item);
    }

    [HttpPost]
    public IActionResult SaveItem(City item, bool isNew)
    {
        if (ModelState.IsValid)
        {
            if (isNew)
            {
                _context.Cities.Add(item);
            }
            else
            {
                _context.Cities.Update(item);
            }

            var rowsAffected = _context.SaveChanges();

            // При обновлении записи в базе данных, очищаем кэш
            _cache.Remove(typeof(City));

            return rowsAffected > 0 ? Redirect("/Cities") : StatusCode(StatusCodes.Status500InternalServerError);
        }
        
        var errors = ModelState.Values.SelectMany(v => v.Errors);
        var errorMessage = "";

        foreach (var error in errors) errorMessage += error.ErrorMessage + "\n";
        
        return StatusCode(StatusCodes.Status500InternalServerError, new { errorMessage });
    }

    [HttpGet]
    public FileContentResult ExportToExcel()
    {
        var data = DatabaseScripts<City>.GetCachedData(_context, _cache).ToList();
        string[] columns = { "Id", "Name" };
        var fileContent = ExcelExport.ExportExcel(data, columns, true);
        return File(fileContent ?? Array.Empty<byte>(), ExcelExport.ExcelContentType, "Cities.xlsx");
    }
    
    [HttpPost]
    public IActionResult CheckUnique([FromBody] City? item)
    {
        if (item == null) return Json(new { isUnique = true, isValid = false });
        
        var isUnique = !_context.Cities.Any(source =>
            source.Id != item.Id &&
            source.Name == item.Name);

        return Json(new { isUnique, isValid = ModelState.IsValid });
    }
}