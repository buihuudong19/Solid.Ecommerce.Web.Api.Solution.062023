namespace Solid.Ecommerce.Caching.Common;

public static partial class CachingCommonDefaults
{
    public static int CacheTime => 60; //by min 

    /// <summary>
    /// Gets a key for caching of products
    /// </summary>
    public static string AllCacheKey => "solid.ecommerce.{0}.all";
    //solid.ecommerce.product.all

    /// <summary>
    /// Gets a key for caching
    /// </summary>
    /// <remarks>
    /// {0} : id of entity
    /// {1} : name of entity
    /// </remarks>
    public static string CacheKey => "solid.ecommerce.{0}.id.{1}"; 
    //solid.ecommerce.product.1
    public static string CacheKeyHeader => "solid.ecommerce.{0}.id";
}
