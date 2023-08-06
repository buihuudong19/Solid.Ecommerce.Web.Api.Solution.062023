using Solid.Ecommerce.Application.Interfaces.Common;

namespace Solid.Ecommerce.Application.Interfaces.Repositories;
public interface IUnitOfWork
{
    //Van de: Lam the nao de gom all transaction (db) => 1 transaction => DB
    IApplicationDbContext ApplicationDbContext { get; }
    IRepository<T> Repository<T>() where T : class;//Generic o muc phuong thuc

    Task<int> SaveChangeAsync(CancellationToken cancellationToken = default);
    //begin tran
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RolebackTransactionAsync();

    
}
