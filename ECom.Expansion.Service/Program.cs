using ApplicationServices.Implementation;
using ApplicationServices.Implementation.Utils;
using ApplicationServices.Interfaces;
using Basket.Ecom;
using Basket.Ecom.Clients;
using Basket.Interfaces;
using Basket.Interfaces.Clients;
using Basket.Interfaces.Clients.Options;
using Catalog.Ecom;
using Catalog.Ecom.Clients;
using Catalog.Ecom.Options;
using Catalog.Interfaces;
using Collecting.Controllers;
using Collecting.Fozzy;
using Collecting.Fozzy.Clients;
using Collecting.Fozzy.Clients.Options;
using Collecting.Interfaces;
using Collecting.Interfaces.Clients;
using Collecting.UseCases;
using DataAccess.Interfaces;
using DataAccess.MsSql;
using DataAccess.MsSql.Utils;
using DataAccess.Proxy;
using DomainServices.Implementation;
using DomainServices.Interfaces;
using ECom.Entities.Models;
using ECom.Expansion.Service.Utils;
using ECom.Expansion.WebApi;
using Elastic.Apm.NetCoreAll;
using Entities.Consts;
using Entities.Models.Expansion;
using Expansion.Ecom;
using Expansion.Ecom.Clients;
using Expansion.Ecom.Clients.Options;
using Expansion.Interfaces;
using Expansion.Interfaces.Clients;
using HealthCheck.Controllers;
using HealthCheck.UseCases.Database.Queries.CheckDbConnection;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using NLog.Config;
using NLog.Web;
using ReadOnlyDataAccess.Proxy;
using Swashbuckle.AspNetCore.Filters;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;
using WebSite.Presta;
using WebSite.Presta.Clients;
using WebSite.Presta.Clients.Options;
using WebSiteService.Interfaces;
using WebSiteService.Interfaces.Clients;
using Workflow.Controllers;
using Workflow.Ecom;
using Workflow.Ecom.Clients;
using Workflow.Ecom.Clients.Options;
using Workflow.Interfaces;
using Workflow.Interfaces.Clients;
using Workflow.UseCases.Order;

NLog.Logger? logger = null;
IConfiguration Configuration;
string? _swaggerTitle = "Unknown";
try
{

    var builder = WebApplication.CreateBuilder(args);

    ConfigureConfiguration(builder.Configuration);
    ConfigureServices(builder.Services, NLog.LogManager.GetLogger("logger for retry policies"));
    ConfigureSwagger(builder.Services, builder.Environment);
    ConfigureWithWebHost(builder.WebHost);
    

    var app = builder.Build();
    

    ConfigureMiddleware(app, app.Environment);
    //ConfigureEndpoints(app, app.Services, app.Environment);

    app.Run();
}
catch (Exception ex)
{
    logger?.Fatal(ex, "{msg}", "Service stopped because of exception");
    Console.WriteLine(ex.Message);
}
finally
{
    NLog.LogManager.Shutdown();
}

void ConfigureWithWebHost(IWebHostBuilder webHostBuilder)
{
    webHostBuilder
   .ConfigureLogging((h, l) =>
    {
        ConfigurationItemFactory.Default.LayoutRenderers.RegisterDefinition("error-type", typeof(ErrorTypeLayoutRenderer));
        var env = h.HostingEnvironment.EnvironmentName;

        var factory = NLogBuilder.ConfigureNLog(Path.Combine(Directory.GetCurrentDirectory(), $"nlog.{env}.config"));
        logger = factory.GetCurrentClassLogger();

        l.ClearProviders();
        l.SetMinimumLevel(LogLevel.Trace);
    })
    .UseNLog(new NLogAspNetCoreOptions
    {
        IncludeScopes = true
    });
}

void ConfigureConfiguration(ConfigurationManager configuration)
{
    Configuration = configuration;
}

void ConfigureServices(IServiceCollection services, NLog.Logger logger)
{

    services.AddCorsPolicies(Configuration);

    services.AddAutoMapper(typeof(MappingProfile), typeof(ViewMappingProfile), typeof(HealthcheckControllerMapping));
    services.AddMediatR(
       typeof(CommonAppService),
       //typeof(FozzyShopUpdateQtyReplacementsCommand),

       typeof(CollectingController),
       typeof(UpdateCollectableOrderCommand),

       typeof(WorkflowController),
       typeof(AcceptReplacementsCommand),

       typeof(HealthcheckController),
       typeof(CheckReadOnlyDbConnectionQuery)
       );

    //Domain
    services.AddScoped<ITransformService, TransformService>(); 
    services.AddScoped<IDateTimeService, DateTimeService>();

    // Infrastructure
    services.AddScoped<IBasketService, BasketService>();
    services.AddScoped<ICollectingService, FozzyCollectingService>();
    services.AddScoped<IWebSiteService, PrestaWebSiteService>();
    services.AddScoped<IWorkflowService, EComWorkflowService>();
    services.AddScoped<ICommonAppService, CommonAppService>();
    services.AddScoped<ICatalogService, EComCatalogService>();
    services.AddScoped<IExpansionService, ExpansionService>();
    

    //services.AddScoped<IInterceptorService, InterceptorService>(); 
    services.AddSingleton<IEntityMappingService, EntityMapper>();

    // Use Cases
    services.AddScoped<ICommonAppService, CommonAppService>();
    //services.AddSingleton<IFozzyShopAndCollectOrderDifferenceCalculator, FozzyShopAndCollectOrderDifferenceCalculator>();
    //services.AddSingleton<IEComAndCollectOrderDifferenceCalculator, EComAndCollectOrderDifferenceCalculator>();

    // controllers
    //..


    //clients

    // get local options sample
    //var resiliencePoliciesOptions = new ResiliencePoliciesOptions()
    //Configuration.GetSection(nameof(ResiliencePoliciesOptions)).Bind(resiliencePoliciesOptions)
    
    //  catalog
    services.Configure<CatalogApiOptions>(Configuration.GetSection(nameof(CatalogApiOptions)));
    services.AddHttpClient<ICatalogApiClient, CatalogApiClient>((serviceProvider, httpClient) =>
    {
        var options = serviceProvider
                        .GetRequiredService<IOptions<CatalogApiOptions>>()
                        .Value;

        httpClient.BaseAddress = new Uri(options.BaseUrl);
        httpClient.Timeout = options.Timeout;
    })
    ;
    
    services.Configure<BasketServiceOptions>(Configuration.GetSection(nameof(BasketServiceOptions)));
    services.AddHttpClient<IBasketServiceClient, BasketApiClient> ((serviceProvider, httpClient) =>
    {
        var options = serviceProvider
                        .GetRequiredService<IOptions<BasketServiceOptions>>()
                        .Value;

        httpClient.BaseAddress = new Uri(options.BaseUrl);
        httpClient.Timeout = options.Timeout;
    })
    ;

    
   

    services.Configure<FozzyShopCollectingServiceOptions>(Configuration.GetSection(nameof(FozzyShopCollectingServiceOptions)));
    services.AddHttpClient<IFozzyCollectingServiceClient, FozzyCollectingServiceClient>((serviceProvider, httpClient) =>
    {
        var options = serviceProvider
                        .GetRequiredService<IOptions<FozzyShopCollectingServiceOptions>>()
                        .Value;

        httpClient.BaseAddress = new Uri(options.BaseUrl);
        httpClient.Timeout = options.Timeout;
    })
    ;

    services.Configure<FozzyShopStaffServiceOptions>(Configuration.GetSection(nameof(FozzyShopStaffServiceOptions)));
    services.AddHttpClient<IFozzyStaffServiceClient, FozzyStaffServiceClient>((serviceProvider, httpClient) =>
    {
        var options = serviceProvider
                        .GetRequiredService<IOptions<FozzyShopStaffServiceOptions>>()
                        .Value;

        httpClient.BaseAddress = new Uri(options.BaseUrl);
        httpClient.Timeout = options.Timeout;
    })
    ;

    services.Configure<FozzyShopSiteOptions>(Configuration.GetSection(nameof(FozzyShopSiteOptions)));
    services.AddScoped<IFozzyShopSiteServiceClient, FozzyShopSiteServiceClient>(); // TODO add custom policy fpr REST Client!

    services.Configure<EComWorkflowServiceOptions>(Configuration.GetSection(nameof(EComWorkflowServiceOptions)));
    services.AddHttpClient<IWorkflowServiceClient, EComWorkflowServiceClient>((serviceProvider, httpClient) =>
    {
        var options = serviceProvider
                        .GetRequiredService<IOptions<EComWorkflowServiceOptions>>()
                        .Value;

        httpClient.BaseAddress = new Uri(options.BaseUrl);
        httpClient.Timeout = options.Timeout;
    })
    ;

    
    services.Configure<WorkflowToExpansionOptions>(Configuration.GetSection(nameof(WorkflowToExpansionOptions)));
    services.AddHttpClient<IWorkflowToExpansionClient, WorkflowToExpansionClient> ((serviceProvider, httpClient) =>
    {
        var options = serviceProvider
                        .GetRequiredService<IOptions<WorkflowToExpansionOptions>>()
                        .Value;

        httpClient.BaseAddress = new Uri(options.BaseUrl);
        httpClient.Timeout = options.Timeout;
    })
    ;

    services.Configure<CollectingToExpansionOptions>(Configuration.GetSection(nameof(CollectingToExpansionOptions)));
    services.AddHttpClient<ICollectingToExpansionClient, CollectingToExpansionClient > ((serviceProvider, httpClient) =>
    {
        var options = serviceProvider
                        .GetRequiredService<IOptions<CollectingToExpansionOptions>>()
                        .Value;

        httpClient.BaseAddress = new Uri(options.BaseUrl);
        httpClient.Timeout = options.Timeout;
    })
    ;


    services.AddDbContext<IDataAccess, AppDbContext>(builder =>
         builder.UseSqlServer(Configuration.GetConnectionString("MsSql")));

    services.AddDbContext<IReadOnlyDataAccess, ReadOnlyAppDbContext>(builder =>
         builder.UseSqlServer(Configuration.GetConnectionString("MsSql")));

    services.AddScoped<IDataAccessProxy, DataAccessProxy>();
    services.AddScoped<IReadOnlyDataAccessProxy, ReadOnlyDataAccessProxy>();

    services.AddMvc()
        .AddXmlSerializerFormatters();

    //services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ApmTraceBehavior<,>)) 

    services.AddControllers()
        .AddApplicationPart(typeof(CollectingController).Assembly)
        .AddApplicationPart(typeof(WorkflowController).Assembly)
        .AddApplicationPart(typeof(HealthcheckController).Assembly)
        .AddControllersAsServices()
        .AddNewtonsoftJson();
}

    void ConfigureSwagger(IServiceCollection services, IWebHostEnvironment env)
    {
        var host = System.Net.Dns.GetHostName();
        var ver = Assembly.GetExecutingAssembly().GetName().Version?.ToString();
        var swaggerDescr = $"* Environment: {env.EnvironmentName}\n* Assembly version: {ver}\n* Host: {host}";
        _swaggerTitle = Assembly.GetExecutingAssembly().GetName().Name;
    

        services.AddSwaggerGen(c =>
        {
            c.ExampleFilters();
            c.SwaggerDoc(ExpansionConsts.Common.App.Groups.Expansion.GroupNameVersionOne, new OpenApiInfo
            {
                Title = PrepareTitle(ExpansionConsts.Common.App.Groups.Expansion.Description),
                Version = ver,
                Description = swaggerDescr
            });

            c.SwaggerDoc(ExpansionConsts.Common.App.Groups.Workflow.GroupNameVersionOne, new OpenApiInfo
            {
                Title = PrepareTitle(ExpansionConsts.Common.App.Groups.Workflow.Description),
                Version = ver,
                Description = swaggerDescr
            });

            c.SwaggerDoc(ExpansionConsts.Common.App.Groups.FozzyCollecting.GroupNameVersionOne, new OpenApiInfo
            {
                Title = PrepareTitle(ExpansionConsts.Common.App.Groups.FozzyCollecting.Description),
                Version = ver,
                Description = swaggerDescr
            });

            c.CustomSchemaIds(x => x.FullName);

            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

            AddXmlDocumentation(c, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml");
            AddXmlDocumentation(c, $"{typeof(CatalogInfo).Assembly.GetName().Name}.xml");
            AddXmlDocumentation(c, $"{typeof(TraceableOrder).Assembly.GetName().Name}.xml");
            AddXmlDocumentation(c, $"{typeof(HealthcheckController).Assembly.GetName().Name}.xml");
            AddXmlDocumentation(c, $"{typeof(CollectingController).Assembly.GetName().Name}.xml");
            AddXmlDocumentation(c, $"{typeof(WorkflowController).Assembly.GetName().Name}.xml");

        });
        services.AddSwaggerGenNewtonsoftSupport();
    services.AddSwaggerExamplesFromAssemblies(
        typeof(CollectingController).Assembly,
        typeof(WorkflowController).Assembly,
        typeof(HealthcheckController).Assembly);

}

void AddXmlDocumentation(SwaggerGenOptions options, string xmlFile)
{
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (!File.Exists(xmlPath))
    {
        Console.WriteLine($"File {xmlPath} not found");
    }
    options.IncludeXmlComments(xmlPath);
}


void ConfigureMiddleware(IApplicationBuilder app, IWebHostEnvironment env)
{
    app.UseRouting();

    app.UseCors("CorsPolicy");

    if (env.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
    }
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint($"/swagger/{ExpansionConsts.Common.App.Groups.Expansion.GroupNameVersionOne}/swagger.json",
            PrepareTitle2(ExpansionConsts.Common.App.Groups.Expansion.Description));
        c.SwaggerEndpoint($"/swagger/{ExpansionConsts.Common.App.Groups.Workflow.GroupNameVersionOne}/swagger.json",
            PrepareTitle2(ExpansionConsts.Common.App.Groups.Workflow.Description));
        c.SwaggerEndpoint($"/swagger/{ExpansionConsts.Common.App.Groups.FozzyCollecting.GroupNameVersionOne}/swagger.json",
            PrepareTitle2(ExpansionConsts.Common.App.Groups.FozzyCollecting.Description));
        c.DisplayOperationId();
        c.RoutePrefix = string.Empty;
    });


    app.UseCommonMiddleware();
    app.UseMiddleware<ServiceHeaders>();
    //app.UseMiddleware<ConcurentDbUpdateHandler>();

    app.UseAllElasticApm();

    app.UseExceptionMiddleware(); // exception handling 

    app.UseHttpsRedirection();

    app.UseStaticFiles();

    //var runtimeVersion = typeof(Startup).GetTypeInfo().Assembly.GetCustomAttribute<AssemblyFileVersionAttribute>().Version;
    //logger.LogInformation($"ReleaseVersion {runtimeVersion}");

    app.UseEndpoints(endpoints =>
    {
        endpoints.MapControllers();
    });
}

//void ConfigureEndpoints(IEndpointRouteBuilder app, IServiceProvider services, IWebHostEnvironment env)
//{
//    app.UseEndpoints(endpoints =>
//    {
//        endpoints.MapControllers();
//    });
//}

string? PrepareTitle(string title)
{
    return (title == ExpansionConsts.Common.App.Groups.Expansion.Description) 
        ? _swaggerTitle 
        : $"Calls from {title} ({_swaggerTitle})";
}

string PrepareTitle2(string title)
{
    return title;
}
