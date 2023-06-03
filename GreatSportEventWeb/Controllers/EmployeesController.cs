using GreatSportEventWeb.Data;
using GreatSportEventWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace GreatSportEventWeb.Controllers;

[Authorize]
public class EmployeesController : Controller
{
    private readonly IMemoryCache _cache;
    private readonly ApplicationContext _context;

    public EmployeesController(ApplicationContext context, IMemoryCache cache)
    {
        _context = context;
        _cache = cache;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View(DatabaseScripts<Employee>.GetCachedData(_context, _cache));
    }

    [HttpGet]
    public IActionResult GetSortedData(string sortBy, string sortDirection, bool clearCache)
    {
        if (clearCache) _cache.Remove(typeof(Employee));

        var data = DatabaseScripts<Employee>.GetCachedData(_context, _cache);

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
            "position" => sortDirection == "asc"
                ? data.OrderBy(item => item.Position)
                : data.OrderByDescending(item => item.Position),
            "team" => sortDirection == "asc"
                ? data.OrderBy(item => item.Team)
                : data.OrderByDescending(item => item.Team),
            "birth-date" => sortDirection == "asc"
                ? data.OrderBy(item => item.BirthDate)
                : data.OrderByDescending(item => item.BirthDate),
            _ => data
        };

        return PartialView("_Table", data);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public IActionResult DeleteItem(int id)
    {
        var item = _context.Employees.FirstOrDefault(item => item.Id == id);
        if (item == null) return NotFound(); // Если запись не найдена, возвращаем ошибку 404

        _context.Employees.Remove(item);
        var rowsAffected = _context.SaveChanges();

        // При удалении записи из базы данных, очищаем кэш
        _cache.Remove(typeof(Employee));

        return rowsAffected > 0 ? Ok() : StatusCode(StatusCodes.Status500InternalServerError);
    }

    [HttpGet]
    public IActionResult GetItem(int id)
    {
        var item = _context.Employees.FirstOrDefault(item => item.Id == id);
        if (item == null) return NotFound(); // Если запись не найдена, возвращаем ошибку 404
        
        item.SelectedEventIds = DatabaseScripts<OrganisationEvent>.GetCachedData(_context, _cache)
            .Where(oe => oe.EmployeeId == id)
            .Select(oe => oe.SportEventId).ToList();

        ViewBag.Gender = DatabaseScripts<Gender>.GetCachedData(_context, _cache).OrderBy(gender => gender.Name);
        ViewBag.Positions = DatabaseScripts<Position>.GetCachedData(_context, _cache).OrderBy(position => position.Name);
        ViewBag.Teams = DatabaseScripts<Team>.GetCachedData(_context, _cache).OrderBy(team => team.Name).ToList()
            .Prepend(
                new Team
                {
                    Id = -1,
                    Name = ""
                });
        ViewBag.SportEvents =
            DatabaseScripts<SportEvent>.GetCachedData(_context, _cache).OrderBy(team => team.DateTime);
        ViewBag.Edit = true;

        return PartialView("Form", item);
    }

    [HttpGet]
    public IActionResult CreateItem()
    {
        var item = new Employee();

        ViewBag.Gender = DatabaseScripts<Gender>.GetCachedData(_context, _cache).OrderBy(gender => gender.Name);
        ViewBag.Positions = DatabaseScripts<Position>.GetCachedData(_context, _cache).OrderBy(position => position.Name);
        ViewBag.Teams = DatabaseScripts<Team>.GetCachedData(_context, _cache).OrderBy(team => team.Name).ToList()
            .Prepend(
                new Team
                {
                    Id = -1,
                    Name = ""
                });
        ViewBag.SportEvents =
            DatabaseScripts<SportEvent>.GetCachedData(_context, _cache).OrderBy(team => team.DateTime);
        ViewBag.Edit = false;

        return PartialView("Form", item);
    }

    [HttpPost]
    public IActionResult SaveItem(Employee item)
    {
        if (ModelState.IsValid)
        {
            item.TeamId = item.TeamId == -1 ? null : item.TeamId;
            _context.Employees.Update(item);
            var rowsAffected = _context.SaveChanges();
            
            item.SelectedEventIds ??= new List<int>();
            UpdateOrganisationEvents(item.SelectedEventIds, item.Id);

            // При обновлении записи в базе данных, очищаем кэш
            _cache.Remove(typeof(Employee));

            return rowsAffected > 0 ? Redirect("/Employees") : StatusCode(StatusCodes.Status500InternalServerError);
        }

        var errors = ModelState.Values.SelectMany(v => v.Errors);
        var errorMessage = "";

        foreach (var error in errors) errorMessage += error.ErrorMessage + "\n";

        return StatusCode(StatusCodes.Status500InternalServerError, new { errorMessage });
    }
    
    private void UpdateOrganisationEvents(List<int> selectedEventIds, int employeeId)
    {
        var existingEvents = _context.OrganisationEvents.Where(oe => oe.EmployeeId == employeeId)
            .Select(oe => oe.SportEventId).ToList();

        var onDeleteEvents = existingEvents.Except(selectedEventIds);
        var onAddEvents = selectedEventIds.Except(existingEvents);

        foreach (var eventId in onDeleteEvents)
        {
            var organisationEvent =
                _context.OrganisationEvents.FirstOrDefault(
                    oe => oe.SportEventId == eventId && oe.EmployeeId == employeeId);

            if (organisationEvent != null)
            {
                _context.OrganisationEvents.Remove(organisationEvent);
            }
        }

        foreach (var eventId in onAddEvents)
        {
            var organisationEvent = new OrganisationEvent
            {
                SportEventId = eventId,
                EmployeeId = employeeId
            };

            _context.OrganisationEvents.Add(organisationEvent);
        }

        // Сохранение изменений в базе данных
        _context.SaveChanges();
        _cache.Remove(typeof(OrganisationEvent));
    }

    [HttpGet]
    public FileContentResult ExportToExcel()
    {
        var data = DatabaseScripts<Employee>.GetCachedData(_context, _cache).ToList();
        string[] columns =
        {
            "Surname", "Name", "Patronymic", "Gender", "PhoneNumber", "BirthDate",
            "Team", "Position"
        };
        var fileContent = ExcelExport.ExportExcel(data, columns, true);
        return File(fileContent ?? Array.Empty<byte>(), ExcelExport.ExcelContentType, "Employees.xlsx");
    }

    [HttpPost]
    public IActionResult CheckUnique([FromBody] Employee? item)
    {
        if (item == null) return Json(new { isUnique = true, isValid = false });

        var isUnique = !_context.Employees.Any(source =>
            source.Id != item.Id &&
            source.Surname == item.Surname &&
            source.Name == item.Name &&
            source.Patronymic == item.Patronymic &&
            source.GenderId == item.GenderId &&
            source.BirthDate == item.BirthDate);

        return Json(new { isUnique, isValid = ModelState.IsValid });
    }
}