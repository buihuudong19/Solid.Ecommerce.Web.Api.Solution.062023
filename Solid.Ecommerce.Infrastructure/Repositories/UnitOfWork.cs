
using Microsoft.EntityFrameworkCore.Storage;
using Solid.Ecommerce.Application.Interfaces.Common;
using Solid.Ecommerce.Application.Interfaces.Repositories;
using Solid.Ecommerce.Infrastructure.Context;
using System.Data;

namespace Solid.Ecommerce.Infrastructure.Repositories;
public class UnitOfWork : IUnitOfWork
{
    //Cau truc se luu cac repository tuong ung voi moi entity (customers => CustomerRepo,...)
    private Dictionary<string, Object> Repositories { get; }
    private IDbContextTransaction _transaction;
    private IsolationLevel? _isolationLevel;

    public IApplicationDbContext ApplicationDbContext { get; private set; } 

    public UnitOfWork(IApplicationDbContext applicationDbContext)
    {
        ApplicationDbContext = applicationDbContext;
        Repositories = new Dictionary<string, dynamic>();
    }

    public async Task BeginTransactionAsync()
    {
        if (_transaction == null)
        {
            _transaction = _isolationLevel.HasValue ?
                await ApplicationDbContext.DbContext.Database.BeginTransactionAsync(_isolationLevel.GetValueOrDefault()) :
                await ApplicationDbContext.DbContext.Database.BeginTransactionAsync();
        }
    }

    public async Task CommitTransactionAsync()
    {
        //apply toan bo transactions => database
        await ApplicationDbContext.DbContext.SaveChangesAsync();
        if (_transaction is null) return;
        await _transaction.CommitAsync(); //clean toan bo hanh dong da duoc apply vao db
        await _transaction.DisposeAsync(); //nothing vung nho dem (neu co)
        _transaction = null;
    
    }
    
    public IRepository<T> Repository<T>() where T : class
    {
        //lam sao de return 1 repository cua mot entity T nao do?
        var type = typeof(T);//Muon lay kieu cua entity T
        var typeName = type.Name;
        //lock: giup tuan tu hoa neu nhu co nhieu loi de tao repo
        lock (Repositories)
        {
            if(Repositories.ContainsKey(typeName))
                return (IRepository<T>)Repositories[typeName];
            //tao moi repo tuong ung cho kieu T di vao
            var repository= new Repository<T>(ApplicationDbContext);
            //add vo cached 
            Repositories.Add(typeName, repository); 

            return repository;
        }
    }

    public async Task RolebackTransactionAsync()
    {
        if (_transaction is null) return;
        await _transaction.RollbackAsync();
        await _transaction.DisposeAsync();
        _transaction = null;
    }

    public async Task<int> SaveChangeAsync(CancellationToken cancellationToken = default)
        => await ApplicationDbContext.DbContext.SaveChangesAsync(cancellationToken);


    public void Dispose()
    {
        //nothing: connection db, nothing all objects
        if (ApplicationDbContext == null) return;
        //close connection
        if(ApplicationDbContext.DbContext.Database.GetDbConnection().State == ConnectionState.Open)
            ApplicationDbContext.DbContext.Database.GetDbConnection().Close();

        ApplicationDbContext.DbContext.Dispose();

        ApplicationDbContext = null;

        GC.SuppressFinalize(this); //nothing luon UnitOfWork (khong con dung duoc nua)

    }


}
