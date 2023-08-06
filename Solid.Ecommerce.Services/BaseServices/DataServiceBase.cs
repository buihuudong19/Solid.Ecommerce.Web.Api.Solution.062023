

using Solid.Ecommerce.Application.Interfaces.Repositories;
using Solid.Ecommerce.Application.Interfaces.Services;
using System.Linq.Expressions;

namespace Solid.Ecommerce.Services.BaseServices;

public abstract class DataServiceBase<TEntity>:IDataService<TEntity> where TEntity : class, new()
{
    /*
     * Muc dich:
        - Chua cac members chung cho all lop con (ProductService, *Service)
        - Thuc thi giup 1 phan cac chuc nang de cho cac *Service khong phai
          thuc thi nua ma su dung luon
     */
    //property
    protected IUnitOfWork UnitOfWork { get; set; }
    //khong tao constructor KHONG THAM SO o day (:
    protected DataServiceBase(IUnitOfWork unitOfWork)
    {
        UnitOfWork = unitOfWork;
    }

    public abstract Task<IList<TEntity>> GetAllAsync();
    public abstract Task<IList<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> predicate);
    public abstract Task<TEntity> GetOneAsync(int? id);
    public abstract Task UpdateAsync(TEntity entity);
    public abstract Task DeleteAsync(int? id);
    public abstract Task DeleteAsync(TEntity entity);
    public abstract Task AddAsync(TEntity entity);

    //manual methods cua abstract class
}


