using Solid.Ecommerce.Caching.Extensions;
using Solid.Ecommerce.Services.ExtensionsServices;
using Solid.Ecommerce.WebApi.Security;

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

        /*call manual cached*/
        services.AddCacheServices();


        /*For security*/
        services.AddCors();

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

        app.UseMiddleware<SecurityHeaders>();

        app.UseHttpsRedirection();
        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints => 
        { 
            endpoints.MapControllers(); 
        }
        );
        /*For security*/
        app.UseCors(configurePolicy: options =>
        {
            options.WithMethods("GET", "POST", "PUT", "DELETE");
            options.WithOrigins(
            "https://localhost:7282" // allow requests from the client (diff domain)
            );
        });
    }

}
