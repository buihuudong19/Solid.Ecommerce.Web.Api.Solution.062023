using Solid.Ecommerce.Application.Interfaces.Common;

namespace Solid.Ecommerce.Infrastructure.Context;
public class ApplicationDbContext : IApplicationDbContext
{
    private DbFactoryContext _dbFactoryContext;

    public ApplicationDbContext(DbFactoryContext dbFactoryContext)
    {
        _dbFactoryContext = dbFactoryContext;
    }

    //properties
    public DbContext DbContext => _dbFactoryContext.DbContext;
}
