using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using GreatSportEventWeb.Models;
using Type = System.Type;

namespace GreatSportEventWeb.Data;

public static class DatabaseScripts<T> where T : class
{
    private static readonly Dictionary<Type, string[]> IncludePropertiesMap = new()
    {
        { typeof(Location), new[] { "City", "Type" } },
        { typeof(SportEvent), new[] { "Location", "Location.City", "Type" } },
        { typeof(Team), new[] { "Location", "Location.City" } },
        { typeof(Athlete), new[] { "Gender", "Team", "Position" } },
        {
            typeof(ParticipationEvent),
            new[] { "SportEvent", "SportEvent.Type", "SportEvent.Location", "SportEvent.Location.City", "Team" }
        },
        { typeof(Training), new[] { "Location", "Location.City", "Team" } },
        { typeof(Employee), new[] { "Gender", "Team", "Position" } }
    };

    public static IQueryable<T> GetCachedData(ApplicationContext context, IMemoryCache cache)
    {
        cache.TryGetValue(typeof(T), out IQueryable<T>? data);

        if (data != null) return data;

        var query = context.Set<T>().AsQueryable();

        data = IncludePropertiesMap.TryGetValue(typeof(T), out var includeProperties)
            ? IncludeProperties(query, includeProperties).ToList().AsQueryable()
            : query.ToList().AsQueryable();

        var cacheEntryOptions = new MemoryCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromMinutes(10));

        cache.Set(typeof(T), data, cacheEntryOptions);

        return data;
    }

    private static IQueryable<T> IncludeProperties(IQueryable<T> query, string[] includeProperties)
    {
        foreach (var includeProperty in includeProperties)
        {
            query = query.Include(includeProperty);
        }

        return query;
    }

}