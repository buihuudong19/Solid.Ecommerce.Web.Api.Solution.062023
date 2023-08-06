using Solid.Ecommerce.Services.ExtensionsServices;

namespace Solid.Ecommerce.WebApi;

public class Startup
{
    public IConfiguration Configuration { get;}
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }
    //Chua all service ma co the cua nguoi dung hoac he thong
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddAuthorization();
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new() { 
                Title = "Ecommerce Services API",
                Version = "v1"
            } 
            );
        });
        services.AddMemoryCache();
        /*Call services*/
        services.EcommerceInfrastructureDatabase(Configuration);
        services.AddDataServices();


    }
    //Chua all nhung cau hinh (chi lam 1 lan)
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        else
        {
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints => 
        { 
            endpoints.MapControllers(); 
        }
        );


    }

}
