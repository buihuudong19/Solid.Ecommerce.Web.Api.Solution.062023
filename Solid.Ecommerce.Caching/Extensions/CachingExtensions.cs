using Microsoft.Extensions.DependencyInjection;
using Solid.Ecommerce.Caching.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solid.Ecommerce.Caching.Extensions;

public static class CachingExtensions
{
    public static IServiceCollection AddCacheServices(this IServiceCollection services)
    {
        services.AddScoped<IDataCached,DataCached>();
        return services;
    }
    /// <summary>
    /// Func<T,int id) => return id of entity T (T is passed from outsite)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dataCached"></param>
    /// <param name="entity"></param>
    /// <param name="acquire"></param>
    /// <returns></returns>
    public static string GetKey<T>(this IDataCached dataCached, T? entity, Func<T, int> acquire) where T : class
        =>  string.Format(CachingCommonDefaults.CacheKey,
            typeof(T).Name, acquire(entity!));
       
        
    
    public static void LoadAllDataToCached<T>(this IDataCached dataCached, 
        IList<T> data,
        Action<IList<T>> action ) where T : class
    {
        action(data);
    }
}
