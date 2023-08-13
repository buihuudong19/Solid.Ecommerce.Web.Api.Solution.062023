using Solid.Ecommerce.Application.Interfaces.Common;
using Solid.Ecommerce.Application.Interfaces.Repositories;

namespace Solid.Ecommerce.Infrastructure.Repositories;
public class Repository<T> : IRepository<T> where T : class
{
    public IApplicationDbContext ApplicationDbContext { get; private set; }

    public Repository(IApplicationDbContext applicationDbContext)
    {
        ApplicationDbContext = applicationDbContext;
    }

    public DbSet<T> Entities 
        => ApplicationDbContext.DbContext.Set<T>(); //return mot collect cac T

    public async Task DeleteAsync(T entity, bool saveChanges = true)
    {
        Entities.Remove(entity);
        if (saveChanges)
        {
            await ApplicationDbContext.DbContext.SaveChangesAsync(); //
        }
    }

    public async Task DeleteAsync(int id, bool saveChanges = true)
    {
        //1. Lay obj tu db thong qua id
        var entity = await Entities.FindAsync(id);
        DeleteAsync(entity);
        if (saveChanges)
        {
            await ApplicationDbContext.DbContext.SaveChangesAsync(); 
        }

    }

    public async Task DeleteRangeAsync(IEnumerable<T> entities, bool saveChanges = true)
    {
        Entities.RemoveRange(entities);
        if (saveChanges)
        {
            await ApplicationDbContext.DbContext.SaveChangesAsync(); 
        }
    }

    public T Find(params object[] keyValues)
        => Entities.Find(keyValues);

    public async Task<T> FindAsync(params object[] keyValues)
    => await Entities.FindAsync(keyValues);

    public async Task<IList<T>> GetAllAsync()
        => await Entities.ToListAsync();

    public async Task<T> InsertAsync(T entity, bool saveChanges = true)
    {
       var p = await Entities.AddAsync(entity); //van nam tren cached cua EFC
        if (saveChanges)
        {
            await ApplicationDbContext.DbContext.SaveChangesAsync(); //apply giao dich dang tren cached cua EFC => DB
        }
        return entity;
    }

    public async Task InsertRangeAsync(IEnumerable<T> entities, bool saveChanges = true)
    {
        await ApplicationDbContext.DbContext.AddRangeAsync(entities);
        if(saveChanges)
        {
            await ApplicationDbContext.DbContext.SaveChangesAsync();
        }

    }

    public async Task UpdateAsync(T entity, bool saveChanges = true)
    {
        Entities.Update(entity);//Moi tren RAM (Entity Framework)
        if(saveChanges)
        {
            await ApplicationDbContext.DbContext.SaveChangesAsync();
        }
    }

    public async Task UpdateRangeAsync(IEnumerable<T> entities, bool saveChanges = true)
    {
        Entities.UpdateRange(entities);
        if (saveChanges)
        {
            await ApplicationDbContext.DbContext.SaveChangesAsync();
        }
    }
}
