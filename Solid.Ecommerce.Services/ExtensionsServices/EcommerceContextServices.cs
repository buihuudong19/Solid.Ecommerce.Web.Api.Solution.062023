namespace Solid.Ecommerce.Services.ExtensionsServices;
public static class EcommerceContextServices
{
    
    public static IServiceCollection EcommerceInfrastructureDatabase(this IServiceCollection services,IConfiguration config)
    {
        //1. Bien ket noi xuong db thanh services (extension methods)
        services.AddDbContext<SolidStoreContext>(options =>
        {
            options.UseSqlServer(config.GetConnectionString("SolidEcommerceDbConn"),sqlOptions => sqlOptions.CommandTimeout(60));
        });

        //2. Bien DbFactory thanh services
        services.AddScoped<Func<SolidStoreContext>>(
            provider => () => provider.GetService<SolidStoreContext>()
         );
        services.AddScoped<DbFactoryContext>();
        //3. IApplicationDbContext, IUnitOfWork => services
        services.AddScoped<IApplicationDbContext, ApplicationDbContext>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
       
        return services;
    }

    //2. data service => khong can phai new XXservice();
    /// <summary>
    /// Add custom Data service...
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddDataServices(this IServiceCollection services)
    {
        services.AddScoped<IProductService, ProductService>();
        /*Neu muon add cac Services khac thi add at here...*/
        return services;
    }


}
