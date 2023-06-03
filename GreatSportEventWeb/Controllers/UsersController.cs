using GreatSportEventWeb.Data;
using GreatSportEventWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace GreatSportEventWeb.Controllers;

[Authorize]
public class UsersController : Controller
{
    private readonly IMemoryCache _cache;
    private readonly ApplicationContext _context;

    public UsersController(ApplicationContext context, IMemoryCache cache)
    {
        _context = context;
        _cache = cache;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View(DatabaseScripts<User>.GetCachedData(_context, _cache));
    }

    [HttpGet]
    public IActionResult GetSortedData(string sortBy, string sortDirection, bool clearCache)
    {
        if (clearCache) _cache.Remove(typeof(User));

        var data = DatabaseScripts<User>.GetCachedData(_context, _cache);

        data = sortBy switch
        {
            "login" => sortDirection == "asc"
                ? data.OrderBy(item => item.Login)
                : data.OrderByDescending(item => item.Login),
            "type" => sortDirection == "asc"
                ? data.OrderBy(item => item.AccessMode.GetDisplayName())
                : data.OrderByDescending(item => item.AccessMode.GetDisplayName()),
            "full-name" => sortDirection == "asc"
                ? data.OrderBy(item => item.Person)
                : data.OrderByDescending(item => item.Person),
            _ => data
        };

        return PartialView("_Table", data);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public IActionResult DeleteItem(string id)
    {
        var item = _context.Users.FirstOrDefault(item => item.Login == id);
        if (item == null) return NotFound(); // Если запись не найдена, возвращаем ошибку 404

        _context.Users.Remove(item);
        var rowsAffected = _context.SaveChanges();

        // При удалении записи из базы данных, очищаем кэш
        _cache.Remove(typeof(User));

        return rowsAffected > 0 ? Ok() : StatusCode(StatusCodes.Status500InternalServerError);
    }

    [HttpGet]
    public IActionResult GetItem(string id)
    {
        var item = _context.Users.FirstOrDefault(item => item.Login == id);
        if (item == null) return NotFound(); // Если запись не найдена, возвращаем ошибку 404

        GetDataForm();
        item.IsEdit = 1;

        return PartialView("Form", item);
    }

    [HttpGet]
    public IActionResult CreateItem()
    {
        var item = new User();

        GetDataForm();
        item.IsEdit = 0;

        return PartialView("Form", item);
    }

    private void GetDataForm()
    {
        ViewBag.Athletes = DatabaseScripts<Athlete>.GetCachedData(_context, _cache).OrderBy(a => a.ToString())
            .Prepend(
                new Athlete
                {
                    Id = -1,
                    Surname = "",
                    Name = "",
                    Patronymic = ""
                });
        ViewBag.Employees = DatabaseScripts<Employee>.GetCachedData(_context, _cache).OrderBy(e => e.ToString())
            .Prepend(
                new Employee
                {
                    Id = -1,
                    Surname = "",
                    Name = "",
                    Patronymic = ""
                });
        ViewBag.Viewers = DatabaseScripts<Viewer>.GetCachedData(_context, _cache).OrderBy(v => v.ToString())
            .Prepend(
                new Viewer
                {
                    Id = -1,
                    Surname = "",
                    Name = "",
                    Patronymic = ""
                });
        
        ViewBag.AccessModes = EnumHelper.GetEnumDropdownList<UserType>();
    }

    [HttpPost]
    public IActionResult SaveItem(User item)
    {
        if (ModelState.IsValid)
        {
            item.AthleteId = item.AthleteId == -1 ? null : item.AthleteId;
            item.EmployeeId = item.EmployeeId == -1 ? null : item.EmployeeId;
            item.ViewerId = item.ViewerId == -1 ? null : item.ViewerId;

            item.Password = HashPassword.GetHash(item.Password);
            
            if (item.IsEdit == 1)
                _context.Users.Update(item);
            else
                _context.Users.Add(item);

            var rowsAffected = _context.SaveChanges();

            // При обновлении записи в базе данных, очищаем кэш
            _cache.Remove(typeof(User));

            return rowsAffected > 0 ? Redirect("/Users") : StatusCode(StatusCodes.Status500InternalServerError);
        }

        var errors = ModelState.Values.SelectMany(v => v.Errors);
        var errorMessage = "";

        foreach (var error in errors) errorMessage += error.ErrorMessage + "\n";

        return StatusCode(StatusCodes.Status500InternalServerError, new { errorMessage });
    }

    [HttpGet]
    public FileContentResult ExportToExcel()
    {
        var data = DatabaseScripts<User>.GetCachedData(_context, _cache).ToList();
        string[] columns = { "Login", "AccessMode", "Athlete", "Employee", "Viewer" };
        var fileContent = ExcelExport.ExportExcel(data, columns, true);
        return File(fileContent ?? Array.Empty<byte>(), ExcelExport.ExcelContentType, "Users.xlsx");
    }

    [HttpPost]
    public IActionResult CheckUnique([FromBody] User? item)
    {
        if (item == null) return Json(new { isUnique = true, isValid = false });

        var isUnique = item.IsEdit == 1 || !_context.Users.Any(source => source.Login == item.Login);
        
        return Json(new { isUnique, isValid = ModelState.IsValid });
    }
}