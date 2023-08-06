
using Solid.Ecommerce.Application.Interfaces.Repositories;
using System.Linq.Expressions;
namespace Solid.Ecommerce.Infrastructure.Extensions;

public static class RepositoryExtensions
{
    /*
     1. Ham loc ra cac doi tuong thoa man mot tieu chi nao do
     */
    public static  IQueryable<T> Where<T>(this IRepository<T> repository, Expression<Func<T, bool>> predicate) where T : class
        => repository.Entities.Where(predicate);

    /*
     * 2. ToListAsync
     */
    public static async Task<List<T>> ToListAsync<T>(this IRepository<T> repository) where T : class
       =>  await repository.ToListAsync();

    public static async Task<List<T>> ToListAsync<T>(this IRepository<T> repository, Expression<Func<T, bool>> predicate) where T : class
       => await repository
            .Where(predicate)
            .ToListAsync();
    /*
     3. Return 1 list entity T nao do ma co sap sep theo mot tieu chi nao do
     */
    public static IOrderedQueryable<T> OrderBy<T, Tkey>(this IRepository<T> repository, Expression<Func<T, Tkey>> keySelector) where T: class
        => repository.Entities.OrderBy(keySelector);

    /*
     4. Return the first element
     */
    public static async Task<T> FirstOrDefaultAsync<T>(this IRepository<T> repository, Expression<Func<T, bool>> predicate) where T : class
        => await repository.Entities.FirstOrDefaultAsync(predicate);
    /*
     5. Check xem co ton tai mot so entity thoa man mot tieu chi nao do hay khong
     */
    
    public static async Task<bool> AnySync<T>(this IRepository<T> repository, Expression<Func<T, bool>> predicate) where T : class
        =>await repository.Entities.AnyAsync(predicate);
}
