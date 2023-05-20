using Microsoft.Extensions.Caching.Memory;

namespace GreatSportEventWeb.Data;

public static class DatabaseScripts<T> where T : class
{
    public static IQueryable<T> GetCachedData(ApplicationContext context, IMemoryCache cache)
    {
        cache.TryGetValue(typeof(T), out IQueryable<T>? data);

        if (data != null) return data;
        
        data = context.Set<T>().ToList().AsQueryable();

        var cacheEntryOptions = new MemoryCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromMinutes(10)); // Устанавливаем время жизни кэша

        cache.Set(typeof(T), data, cacheEntryOptions);

        return data;
    }
}