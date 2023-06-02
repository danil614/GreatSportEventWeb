using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using GreatSportEventWeb.Models;
using Type = System.Type;

namespace GreatSportEventWeb.Data;

public static class DatabaseScripts<T> where T : class
{
    private static readonly Dictionary<Type, List<string>> IncludePropertiesMap = new()
    {
        //{ typeof(Location), new List<string> { "Property1", "Property2" } },
        { typeof(Team), new List<string> { "Location", "Location.City" } }//,
        //{ typeof(Athlete), new List<string> { "Property5", "Property6" } }
        // Добавьте другие классы и свойства, если необходимо
    };

    public static IQueryable<T> GetCachedData(ApplicationContext context, IMemoryCache cache)
    {
        cache.TryGetValue(typeof(T), out IQueryable<T>? data);

        if (data != null) return data;

        var query = context.Set<T>().AsQueryable();

        if (IncludePropertiesMap.TryGetValue(typeof(T), out var includeProperties))
        {
            data = IncludeProperties(query, includeProperties).ToList().AsQueryable();
        }
        else
        {
            data = query.ToList().AsQueryable();
        }

        var cacheEntryOptions = new MemoryCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromMinutes(10));

        cache.Set(typeof(T), data, cacheEntryOptions);

        return data;
    }

    private static IQueryable<T> IncludeProperties(IQueryable<T> query, List<string> includeProperties)
    {
        foreach (var includeProperty in includeProperties)
        {
            query = query.Include(includeProperty);
        }

        return query;
    }

}