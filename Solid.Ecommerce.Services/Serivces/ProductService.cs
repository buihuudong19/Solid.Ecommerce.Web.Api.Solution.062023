using Microsoft.EntityFrameworkCore;
using Solid.Ecommerce.Application.Interfaces.Repositories;
using Solid.Ecommerce.Application.Interfaces.Services;
using Solid.Ecommerce.Services.BaseServices;
using Solid.Ecommerce.WebApi.Shared;
using Solid.Ecommerce.Caching.Common;
using System.Linq.Expressions;
using Solid.Ecommerce.Caching;
using Solid.Ecommerce.Caching.Extensions;


namespace Solid.Ecommerce.Services.Serivces;
public class ProductService : DataServiceBase<Product>, IProductService
{
    private readonly IDataCached dataCached;
    //Nho la khong co constructor KHONG THAM SO
    public ProductService(IUnitOfWork unitOfWork, IDataCached dataCached) : base(unitOfWork)
    {
        this.dataCached = dataCached;

    }

    public override async Task AddAsync(Product entity)
    {
        try
        {
            string key = null;

            await UnitOfWork.BeginTransactionAsync();
                //Working with caching after...
                await UnitOfWork.Repository <Product>()
                    .InsertAsync(entity,false);

                //luu vao cache: solid.ecommerce.product.id.1890
                key = string.Format(CachingCommonDefaults.CacheKey, typeof(Product).Name,
                entity.ProductId);

                dataCached.Set(key, entity, CachingCommonDefaults.CacheTime);

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

            /*<remove product in cached*/
            string key = string.Format(CachingCommonDefaults.CacheKey, typeof(Product).Name,
                product.ProductId);

            dataCached.Remove(key);


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
    /*
    public override async Task<IList<Product>> GetAllAsync()
        => await UnitOfWork.Repository<Product>()
                .Entities
                .ToListAsync();
    */
    public override async Task<IList<Product>> GetAllAsync()
    {
        string name = typeof(Product).Name.ToLower(); //"product"
        string keyAll = string.Format(CachingCommonDefaults.AllCacheKey, name);//solid.ecommerce.product.all
        /*1. check xem toan bo du lieu Products duoc cached chua?*/
        if (!dataCached.IsSet(keyAll) || !(bool)dataCached.Get(keyAll))
        {
            /*load all product tu db => cached. after that set value true cho keyall*/
            await _loadAllProductToCached();
            dataCached.Set(keyAll, true, CachingCommonDefaults.CacheTime);
        }
        /*2. getall data tu cached ra ngoai: solid.ecommerce.product.id*/
        string pattern = string.Format(CachingCommonDefaults.CacheKeyHeader, name);

        var result = dataCached.GetValues<Product>(pattern);

        return await Task.FromResult(result);
    }


    public override async Task<IList<Product>> GetAllAsync(Expression<Func<Product, bool>> predicate)
        => await UnitOfWork.Repository<Product>()
                 .Entities
                 .Where (predicate).ToListAsync();


    public override async Task<Product> GetOneAsync(int? id)
    {
        /*check trong cached neu co => lay tu cache, ko co thi load tu DB */
        try
        {
            //solid.ecommerce.product.5 (id = 5)
            string key = string.Format(CachingCommonDefaults.CacheKey, typeof(Product).Name.ToLower(), id);

            //neu nhu key ma da ton tai trong cached => return value
            if (dataCached.IsSet(key))
                return await Task.FromResult(dataCached.Get<Product>(key));


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

            //update lai cached
            string key = string.Format(CachingCommonDefaults.CacheKey, typeof(Product).Name,
                entity.ProductId);

            dataCached.Set<Product>(key, pro, CachingCommonDefaults.CacheTime);

            await UnitOfWork.CommitTransactionAsync();



        }
        catch ( Exception ex )
        {
            await UnitOfWork.RolebackTransactionAsync();
            throw;
        }

    }

    private async Task _loadAllProductToCached()
    {
        /*goi tu db len*/
        var products = await UnitOfWork.Repository<Product>()
                             .Entities
                             .ToListAsync();

        string key = string.Empty;//""

        foreach (var p in products)
        {
            /*Lam sao sinh chuoi ky tu doi tuong p o tren?: solid.ecommerce.product.1*/
            key = dataCached.GetKey(p, p => p.ProductId).ToLower();


            /*check xem key da duoc cached chua? neu chua thi set vao cache*/
            if (!dataCached.IsSet(key))
                dataCached.Set(key, p, CachingCommonDefaults.CacheTime);

        }
    }
}
