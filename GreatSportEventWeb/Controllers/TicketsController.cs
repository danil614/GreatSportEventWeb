using System.Security.Claims;
using GreatSportEventWeb.Data;
using GreatSportEventWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace GreatSportEventWeb.Controllers;

[Authorize]
public class TicketsController : Controller
{
    private readonly IMemoryCache _cache;
    private readonly ApplicationContext _context;

    public TicketsController(ApplicationContext context, IMemoryCache cache)
    {
        _context = context;
        _cache = cache;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View(DatabaseScripts<Ticket>.GetCachedData(_context, _cache));
    }

    [HttpGet]
    public IActionResult GetSortedData(string sortBy, string sortDirection, bool clearCache)
    {
        if (clearCache) _cache.Remove(typeof(Ticket));

        var data = DatabaseScripts<Ticket>.GetCachedData(_context, _cache);

        data = sortBy switch
        {
            "viewer" => sortDirection == "asc"
                ? data.OrderBy(item => item.Viewer!.ToString())
                : data.OrderByDescending(item => item.Viewer!.ToString()),
            "sport-event" => sortDirection == "asc"
                ? data.OrderBy(item => item.Seat!.SportEvent!.DateTime)
                : data.OrderByDescending(item => item.Seat!.SportEvent!.DateTime),
            "seat" => sortDirection == "asc"
                ? data.OrderBy(item => item.Seat!.Name)
                : data.OrderByDescending(item => item.Seat!.Name),
            "price" => sortDirection == "asc"
                ? data.OrderBy(item => item.Seat!.Price)
                : data.OrderByDescending(item => item.Seat!.Price),
            "employee" => sortDirection == "asc"
                ? data.OrderBy(item => item.Employee)
                : data.OrderByDescending(item => item.Employee),
            "datetime" => sortDirection == "asc"
                ? data.OrderBy(item => item.DateTime)
                : data.OrderByDescending(item => item.DateTime),
            _ => data
        };

        return PartialView("_Table", data);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public IActionResult DeleteItem(int id)
    {
        var item = _context.Tickets.FirstOrDefault(item => item.Id == id);
        if (item == null) return NotFound(); // Если запись не найдена, возвращаем ошибку 404

        _context.Tickets.Remove(item);
        var rowsAffected = _context.SaveChanges();

        // При удалении записи из базы данных, очищаем кэш
        _cache.Remove(typeof(Ticket));

        return rowsAffected > 0 ? Ok() : StatusCode(StatusCodes.Status500InternalServerError);
    }

    [HttpGet]
    public IActionResult GetItem(int id)
    {
        var item = _context.Tickets.FirstOrDefault(item => item.Id == id);
        if (item == null) return NotFound(); // Если запись не найдена, возвращаем ошибку 404
        
        GetDataForm();
        ViewBag.Edit = true;

        return PartialView("Form", item);
    }

    [HttpGet]
    public IActionResult CreateItem()
    {
        var item = new Ticket();

        item.EmployeeId = GetEmployeeIdByUser();
        item.Employee = DatabaseScripts<Employee>.GetCachedData(_context, _cache)
            .FirstOrDefault(e => e.Id == item.EmployeeId);
        
        item.DateTime = DateTime.Now;
        item.DateTime = item.DateTime.AddSeconds(-item.DateTime.Second);
        item.DateTime = item.DateTime.AddMilliseconds(-item.DateTime.Millisecond);

        GetDataForm();
        ViewBag.Edit = false;

        return PartialView("Form", item);
    }

    private void GetDataForm()
    {
        ViewBag.Viewers = DatabaseScripts<Viewer>.GetCachedData(_context, _cache)
            .OrderBy(v => v.ToString()).Prepend(
                new Viewer
                {
                    Id = -1,
                    Surname = "",
                    Name = "",
                    Patronymic = ""
                });
        ViewBag.Seats = DatabaseScripts<Seat>.GetCachedData(_context, _cache).OrderBy(s => s.SportEvent!.DateTime)
            .Prepend(
                new Seat
                {
                    Id = -1,
                    SportEvent = null,
                    Name = ""
                });
    }

    private int GetEmployeeIdByUser()
    {
        var employeeId = User.FindFirstValue("EmployeeId");

        if (int.TryParse(employeeId, out var result))
        {
            return result;
        }

        return -1;
    }

    [HttpPost]
    public IActionResult SaveItem(Ticket item)
    {
        if (ModelState.IsValid)
        {
            _context.Tickets.Update(item);
            var rowsAffected = _context.SaveChanges();
            
            // При обновлении записи в базе данных, очищаем кэш
            _cache.Remove(typeof(Ticket));

            return rowsAffected > 0 ? Redirect("/Tickets") : StatusCode(StatusCodes.Status500InternalServerError);
        }

        var errors = ModelState.Values.SelectMany(v => v.Errors);
        var errorMessage = "";

        foreach (var error in errors) errorMessage += error.ErrorMessage + "\n";

        return StatusCode(StatusCodes.Status500InternalServerError, new { errorMessage });
    }

    [HttpGet]
    public FileContentResult ExportToExcel()
    {
        var data = DatabaseScripts<Ticket>.GetCachedData(_context, _cache).ToList();
        string[] columns = { "Viewer", "Employee", "DateTime", "Seat" };
        var fileContent = ExcelExport.ExportExcel(data, columns, true);
        return File(fileContent ?? Array.Empty<byte>(), ExcelExport.ExcelContentType, "Tickets.xlsx");
    }

    [HttpPost]
    public IActionResult CheckUnique([FromBody] Ticket? item)
    {
        if (item == null) return Json(new { isUnique = true, isValid = false });

        var isUnique = !_context.Tickets.Any(source =>
            source.Id != item.Id &&
            source.SeatId == item.SeatId);

        return Json(new { isUnique, isValid = ModelState.IsValid });
    }
}