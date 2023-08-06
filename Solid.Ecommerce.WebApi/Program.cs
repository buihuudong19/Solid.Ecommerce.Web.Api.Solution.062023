using Solid.Ecommerce.WebApi;

//Khoi tao application server (webserver)
Host.CreateDefaultBuilder(args)
    .ConfigureWebHostDefaults(webBuilder =>
    {
        webBuilder
        .UseKestrel() //iis
        .UseContentRoot(Directory.GetCurrentDirectory())
        .UseIISIntegration()
        .UseWebRoot(Directory.GetCurrentDirectory())//"wwwroot"
        .UseStartup<Startup>();
    })
    .Build()
    .Run();