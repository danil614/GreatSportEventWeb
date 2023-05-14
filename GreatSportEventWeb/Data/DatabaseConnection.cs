namespace GreatSportEventWeb.Data;

public class DatabaseConnection
{
    /// <summary>
    ///     Получает строку подключения к базе данных.
    /// </summary>
    public static string GetConnectionString()
    {
        const string host = "***REMOVED***";
        const string database = "great_sport_event_db";
        const string username = "app_user_db";
        const string password = "***REMOVED***";

        var connectionString =
            $"server={host};user={username};database={database};password={password};CharSet=utf8;convert zero datetime=True";
        return connectionString;
    }
}