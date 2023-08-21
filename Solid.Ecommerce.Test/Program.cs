using Solid.Ecommerce.Application.Interfaces.Repositories;
using Solid.Ecommerce.WebApi.Shared;
using Solid.Ecommerce.Infrastructure.Repositories;
using Solid.Ecommerce.Application.Interfaces.Common;
using Solid.Ecommerce.Infrastructure.Context;
using Solid.Ecommerce.Shared.EntityModels.SqlServer.DBContext;
using Solid.Ecommerce.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;
using Solid.Ecommerce.Application.Interfaces.Services;
using Solid.Ecommerce.Services.Serivces;

DbFactoryContext dbFactoryContext = new DbFactoryContext(
   ()=> new SolidStoreContext()
    );


IApplicationDbContext applicationDbContext
    = new ApplicationDbContext(dbFactoryContext);
/*
IRepository<Product> repository = new Repository<Product>(applicationDbContext);
*/

IUnitOfWork unitOfWork = new UnitOfWork(applicationDbContext);
/*
var products = await unitOfWork.Repository<Product>()
    .Entities
    .ToListAsync();
*/
/*
IProductService productService = new ProductService(unitOfWork);
var products = await productService.GetAllAsync();

foreach (Product p in products)
{
    Console.WriteLine($"Product Id: {p.ProductId}, Name: {p.ProductName}, Price: {p.UnitPrice}" );
}
*/



