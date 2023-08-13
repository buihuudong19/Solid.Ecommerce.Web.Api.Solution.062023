
using System.Linq.Expressions;

namespace Solid.Ecommerce.Application.Interfaces.Services;
public interface IDataService<TEntity> where TEntity : class, new()
{
    Task<IList<TEntity>> GetAllAsync();
    Task<IList<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> predicate);
    Task<TEntity> GetOneAsync(int? id);
    Task<TEntity> AddAsync(TEntity entity);
    Task UpdateAsync(TEntity entity);
    Task DeleteAsync(int? id);
    Task DeleteAsync(TEntity entity);
}
