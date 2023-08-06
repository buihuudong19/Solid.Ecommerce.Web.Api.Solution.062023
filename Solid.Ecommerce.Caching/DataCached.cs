using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;
using Solid.Ecommerce.Caching.Common;
using System.Collections.Concurrent;
using System.Text.RegularExpressions;


namespace Solid.Ecommerce.Caching;

public class DataCached : IDataCached
{
    private readonly IMemoryCache _memoryCache;
    /// <summary>
    /// All keys of cache
    /// </summary>
    /// <remarks>Dictionary value indicating whether a key still exists in cache</remarks> 
    private static readonly ConcurrentDictionary<string, bool> _allKeys;
    /// <summary>
    /// Cancellation token for clear cache
    /// </summary>
    protected CancellationTokenSource _cancellationTokenSource;
    static DataCached()
    {
        _allKeys = new ConcurrentDictionary<string, bool>();
    }

    public DataCached(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
        _cancellationTokenSource = new CancellationTokenSource();
    }

    /// <summary>
    /// Add key to dictionary
    /// </summary>
    /// <param name="key">Key of cached item</param>
    /// <returns>Itself key</returns>
    protected string AddKey(string key)
    {
        _allKeys.TryAdd(key, true);
        return key;
    }
    /// <summary>
    /// Remove key from dictionary
    /// </summary>
    /// <param name="key">Key of cached item</param>
    /// <returns>Itself key</returns>
    protected string RemoveKey(string key)
    {
        TryRemoveKey(key);
        return key;
    }
    /// <summary>
    /// Try to remove a key from dictionary, or mark a key as not existing in cache
    /// </summary>
    /// <param name="key">Key of cached item</param>
    protected void TryRemoveKey(string key)
    {
        //try to remove key from dictionary
        if (!_allKeys.TryRemove(key, out _))
            //if not possible to remove key from dictionary, then try to mark key as not existing in cache
            _allKeys.TryUpdate(key, false, true);
    }
    protected MemoryCacheEntryOptions GetMemoryCacheEntryOptions(TimeSpan cacheTime)
    {
        var options = new MemoryCacheEntryOptions()
            // add cancellation token for clear cache
            .AddExpirationToken(new CancellationChangeToken(_cancellationTokenSource.Token))
                .SetSize(0);

        //set cache time
        options.AbsoluteExpirationRelativeToNow = cacheTime;

        return options;
    }

    public IList<string> GetKeys() => _allKeys.Keys.ToList();
    public IList<T> GetValues<T>(string pattern)
    {
        IList<T> values = new List<T>();

        //get cache keys that matches pattern
        var regex = new Regex(pattern, RegexOptions.Singleline | RegexOptions.Compiled | RegexOptions.IgnoreCase);

        var matchesKeys = _allKeys
                .Where(p => p.Value)
                .Select(p => p.Key)
                .Where(key => regex.IsMatch(key)).ToList();

        //remove matching values
        foreach (var key in matchesKeys)
        {
            _memoryCache.TryGetValue(key, out T? value);
            if (value != null)
                values.Add(value);

        }

        return values;

    }
    public virtual void Set<T>(string key, T? value, int cacheTime)
    {
        
        if(value != null)
        {
            _memoryCache.Set(AddKey(key), value, GetMemoryCacheEntryOptions(TimeSpan.FromMinutes(cacheTime)));
        }
    }
  
    /// <summary>
    /// Get a cached item. If it's not in the cache yet, then load and cache it
    /// </summary>
    /// <typeparam name="T">Type of cached item</typeparam>
    /// <param name="key">Cache key</param>
    /// <param name="acquire">Function to load item if it's not in the cache yet</param>
    /// <param name="cacheTime">Cache time in minutes; pass 0 to do not cache; pass null to use the default time</param>
    /// <returns>The cached value associated with the specified key</returns>
    public virtual T Get<T>(string key, Func<T> acquire, int? cacheTime = null)
    {
        //item already is in cache, so return it
        if (_memoryCache.TryGetValue(key, out T? value))
            return value!;

        //or create it using passed function
        var result = acquire!();

        //and set in cache (if cache time is defined)
        if ((cacheTime ?? CachingCommonDefaults.CacheTime) > 0)
            Set(key, result, cacheTime ?? CachingCommonDefaults.CacheTime);

        return result;
    }

    public T Get<T>(string key)
    {
        if (_memoryCache.TryGetValue(key, out T? value))
            return value!;

        return default(T)!;
    }
    public void Remove(string key)
    {
        _memoryCache.Remove(RemoveKey(key));
    }

    public bool IsSet(string key)
        => _memoryCache.TryGetValue(key, out object _);


    public void Clear()
    {
        //send cancellation request
        _cancellationTokenSource.Cancel();

        //releases all resources used by this cancellation token
        _cancellationTokenSource.Dispose();

        //recreate cancellation token
        _cancellationTokenSource = new CancellationTokenSource();
    }

    public object Get(string key)
    {
        if (_memoryCache.TryGetValue(key, out object? value))
            return value!;
        return null!;
    }
}