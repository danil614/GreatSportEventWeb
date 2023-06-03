using System.Security.Claims;

namespace GreatSportEventWeb.Data;

public static class UserTabManager
{
// Создание словаря вкладок
    private static readonly Dictionary<string, TabItem> Tabs = new Dictionary<string, TabItem>
    {
        { "Home", new TabItem("Главная", "Home", "Index") },
        { "Locations", new TabItem("Места расположения", "Locations", "Index") },
        { "Types", new TabItem("Типы", "Types", "Index") },
        { "Cities", new TabItem("Города", "Cities", "Index") },
        { "SportEvents", new TabItem("Спортивные мероприятия", "SportEvents", "Index") },
        { "Teams", new TabItem("Команды", "Teams", "Index") },
        { "Positions", new TabItem("Должности", "Positions", "Index") },
        { "Athletes", new TabItem("Спортсмены", "Athletes", "Index") },
        { "CompetitionResults", new TabItem("Результаты соревнований", "CompetitionResults", "Index") },
        { "Trainings", new TabItem("Тренировки", "Trainings", "Index") },
        { "Employees", new TabItem("Сотрудники", "Employees", "Index") },
        { "Viewers", new TabItem("Зрители", "Viewers", "Index") },
        { "Users", new TabItem("Пользователи", "Users", "Index") },
        { "Seats", new TabItem("Посадочные места", "Seats", "Index") },
        { "Tickets", new TabItem("Билеты", "Tickets", "Index") }
    };

    public static UserType? GetUserType(IEnumerable<Claim> claims)
    {
        var roleClaim = claims.FirstOrDefault(c => c.Type == ClaimsIdentity.DefaultRoleClaimType);

        if (roleClaim != null && Enum.TryParse<UserType>(roleClaim.Value, out var userType))
        {
            return userType;
        }
        else
        {
            return null;
        }
    }
    
    /// <summary>
    /// Метод для формирования списка вкладок на основе UserType.
    /// </summary>
    public static List<TabItem> GetTabsForUserType(UserType? userType)
    {
        var userTabs = new List<TabItem>();

        switch (userType)
        {
            case UserType.Admin:
                userTabs.AddRange(new[]
                {
                    Tabs["Home"],
                    Tabs["Locations"],
                    Tabs["Types"],
                    Tabs["Cities"],
                    Tabs["SportEvents"],
                    Tabs["Teams"],
                    Tabs["Positions"],
                    Tabs["Athletes"],
                    Tabs["CompetitionResults"],
                    Tabs["Trainings"],
                    Tabs["Employees"],
                    Tabs["Viewers"],
                    Tabs["Users"],
                    Tabs["Seats"],
                    Tabs["Tickets"]
                });
                break;
            case UserType.Seller:
                userTabs.AddRange(new[]
                {
                    Tabs["Home"],
                    Tabs["Viewers"],
                    Tabs["Seats"],
                    Tabs["Tickets"]
                });
                break;
            case UserType.Organizer:
                userTabs.AddRange(new[]
                {
                    Tabs["Home"],
                    Tabs["Locations"],
                    Tabs["Types"],
                    Tabs["Cities"],
                    Tabs["SportEvents"],
                    Tabs["Teams"],
                    Tabs["Positions"],
                    Tabs["Athletes"],
                    Tabs["CompetitionResults"],
                    Tabs["Trainings"]
                });
                break;
            case UserType.Hr:
                userTabs.AddRange(new[]
                {
                    Tabs["Home"],
                    Tabs["SportEvents"],
                    Tabs["Positions"],
                    Tabs["Athletes"],
                    Tabs["Employees"],
                    Tabs["Users"]
                });
                break;
            case UserType.Trainer:
                userTabs.AddRange(new[]
                {
                    Tabs["Home"],
                    Tabs["SportEvents"],
                    Tabs["Teams"],
                    Tabs["Athletes"],
                    Tabs["Trainings"]
                });
                break;
            case UserType.Athlete:
                userTabs.AddRange(new[]
                {
                    Tabs["Home"],
                    Tabs["SportEvents"],
                    Tabs["Teams"],
                    Tabs["Athletes"],
                    Tabs["Trainings"],
                    Tabs["CompetitionResults"]
                });
                break;
            default:
                userTabs.Add(Tabs["Home"]);
                break;
        }

        return userTabs;
    }
}