using Microsoft.EntityFrameworkCore;
using Solid.Ecommerce.Application.Interfaces.Repositories;
using Solid.Ecommerce.Application.Interfaces.Services;
using Solid.Ecommerce.Services.BaseServices;
using Solid.Ecommerce.WebApi.Shared;
using System.Linq.Expressions;

namespace Solid.Ecommerce.Services.Serivces;
public class ProductService : DataServiceBase<Product>, IProductService
{
    //Nho la khong co constructor KHONG THAM SO
    public ProductService(IUnitOfWork unitOfWork) : base(unitOfWork)
    {
    }

    public override async Task AddAsync(Product entity)
    {
        try
        {
            await UnitOfWork.BeginTransactionAsync();
             //Working with caching after...
                await UnitOfWork.Repository <Product>()
                    .InsertAsync(entity,false);
                
            await UnitOfWork.CommitTransactionAsync();

        }catch (Exception ex)
        {
            await UnitOfWork.RolebackTransactionAsync();
            throw;
        }
    }

    public override async Task DeleteAsync(int? id)
    {
        try
        {
            await UnitOfWork.BeginTransactionAsync();
            //Working with caching after...
            var product = await UnitOfWork.Repository <Product>()
                            .FindAsync(id);

            if(product is null)
                throw new KeyNotFoundException(nameof(product));

            await DeleteAsync(product);


            await UnitOfWork.CommitTransactionAsync();

        }
        catch (Exception ex)
        {
            await UnitOfWork.RolebackTransactionAsync();
            throw;
        }
    }

    public override async Task DeleteAsync(Product entity)
    {
        try
        {
            await UnitOfWork.BeginTransactionAsync();
            //Working with caching after...
            await UnitOfWork.Repository<Product>()
                .DeleteAsync(entity, false);

            await UnitOfWork.CommitTransactionAsync();

        }
        catch (Exception ex)
        {
            await UnitOfWork.RolebackTransactionAsync();
            throw;
        }
    }

    public override async Task<IList<Product>> GetAllAsync()
        => await UnitOfWork.Repository<Product>()
                .Entities
                .ToListAsync();
    
    public override async Task<IList<Product>> GetAllAsync(Expression<Func<Product, bool>> predicate)
        => await UnitOfWork.Repository<Product>()
                 .Entities
                 .Where (predicate).ToListAsync();


    public override async Task<Product> GetOneAsync(int? id)
    {
        try
        {
            Product?  product = await UnitOfWork.Repository<Product>()
                .FindAsync(id);

            return await Task.FromResult(product); //cho phep return bat dong bo

        }catch ( Exception ex)
        {
            return null!;
        }
    }

    public override async Task UpdateAsync(Product entity)
    {
        try
        {
            await UnitOfWork.BeginTransactionAsync();
            //Working with caching after...
            //1. lay product ra tu db theo id cua entity
            var pro = await UnitOfWork.Repository<Product>().FindAsync(entity.ProductId);
            if (pro is null)
                throw new KeyNotFoundException();
            pro.ProductName = entity.ProductName;
            pro.UnitPrice = entity.UnitPrice;
            pro.SupplierId = entity.SupplierId;
            pro.CategoryId = entity.CategoryId;

            await UnitOfWork.CommitTransactionAsync();

        }
        catch ( Exception ex )
        {
            await UnitOfWork.RolebackTransactionAsync();
            throw;
        }
    }
}
