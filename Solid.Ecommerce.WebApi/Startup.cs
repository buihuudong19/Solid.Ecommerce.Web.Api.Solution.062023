using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Solid.Ecommerce.Caching.Extensions;
using Solid.Ecommerce.IdentityJWT.Authentication;
using Solid.Ecommerce.Services.ExtensionsServices;
using Solid.Ecommerce.WebApi.Security;
using System.Text;

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


       
        /*<Integerated authentiacate by Dongbh*/
        /*Init conn*/
        services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("ConnStrSQLServerDB")));
        /*For Identity*/
        services.AddIdentity<IdentityUser, IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        /*Add authentication*/
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;

        })
        .AddJwtBearer(options =>
        {
            options.SaveToken = true;
            options.RequireHttpsMetadata = false;
            options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidAudience = Configuration["JWT:ValidAudience"],
                ValidIssuer = Configuration["JWT:ValidIssuer"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JWT:Secret"]))
            };
        });
        /*</Integerated authentiacate by Dongbh*/
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
        //<Authenticate by Dongbh
        app.UseAuthentication();
        app.UseAuthorization();
        //</Authenticate by Dongbh
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
