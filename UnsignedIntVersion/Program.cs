using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.OData.Abstracts;
using Microsoft.AspNetCore.OData.Formatter.Serialization;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using UnsignedIntVersion.VersionFeature;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddControllers()
    .AddOData(options =>
    {
        options.AddRouteComponents("default", GetDefaultModel());
        options.AddRouteComponents("custom", GetCustomModel(), collection =>
        {
            collection.AddSingleton<IODataSerializerProvider, VersionSerializerProvider>();
            collection.AddSingleton<VersionTypeSerializer>();
            collection.AddSingleton<IETagHandler, VersionETagHandler>();
        });
    });

var app = builder.Build();

app.UseODataRouteDebug().UseRouting().UseEndpoints(routeBuilder => routeBuilder.MapControllers());

app.Run();

static IEdmModel GetDefaultModel()
{
    var builder = new ODataConventionModelBuilder();
    var entityType = builder.EntitySet<Customer>("Customers").EntityType;
    entityType.HasKey(x => x.ID);
    entityType.Property(x => x.Version).IsConcurrencyToken();
    return builder.GetEdmModel();
}

static IEdmModel GetCustomModel()
{
    var builder = new ODataConventionModelBuilder
    {
        Namespace = "Custom"
    };
    var entityType = builder.EntitySet<CustomCustomer>("CustomCustomers").EntityType;
    entityType.HasKey(x => x.ID);
    entityType.Ignore(x => x.Version);
    
    var model = (EdmModel)builder.GetEdmModel();
    Feature.AddVersionSupport(model);
    return model;
}

public class Customer
{
    public int ID { get; set; }
    public string Name { get; set; }
    // EF Core supports ulong as a concurrency token
    // https://docs.microsoft.com/en-us/ef/core/modeling/value-conversions?tabs=data-annotations#use-ulong-for-timestamprowversion
    public ulong Version { get; set; }
}

public class CustomCustomer
{
    public int ID { get; set; }
    public string Name { get; set; }
    public ulong Version { get; set; }
}

public class CustomersController : ODataController
{
    public IQueryable<Customer> Get()
    {
        return new List<Customer> { new()
        {
            ID = 1, 
            Name = "default",
            // Causes an OverflowException
            // Version = ulong.MaxValue
            Version = long.MaxValue
        } }.AsQueryable();
    }
}

public class CustomCustomersController : ODataController
{
    public IQueryable<CustomCustomer> Get()
    {
        return new List<CustomCustomer> { new()
        {
            ID = 2, 
            Name = "custom",
            Version = ulong.MaxValue
        } }.AsQueryable();
    }
}