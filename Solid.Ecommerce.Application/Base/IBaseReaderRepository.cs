
namespace Solid.Ecommerce.Application.Base;
public interface IBaseReaderRepository<T>:IBaseRepo<T> where T : class
{
    /// <summary>
    /// Get all items of an entity by asychronous
    /// </summary>
    /// <returns>
    /// A list entity of T
    /// </returns>
    Task<IList<T>> GetAllAsync();
    T Find(params object[] keyValues);
    Task<T> FindAsync(params object[] keyValues); 
    
}



