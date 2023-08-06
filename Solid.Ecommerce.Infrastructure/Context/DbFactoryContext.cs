using Solid.Ecommerce.Shared.EntityModels.SqlServer.DBContext;

namespace Solid.Ecommerce.Infrastructure.Context;
public class DbFactoryContext
{
    //fields/data
    private DbContext _dbContext;//SolidStoreContext db= new();
    private Func<SolidStoreContext> _instanceFunc;
    //properties


    public DbContext DbContext => _dbContext ??
       (_dbContext = _instanceFunc.Invoke());

    //public DbContext DbContext => new SolidStoreContext();

    public DbFactoryContext(Func<SolidStoreContext> instanceFunc)
    {
        
        _instanceFunc = instanceFunc; // new SolidStoreContext();
    }
}
