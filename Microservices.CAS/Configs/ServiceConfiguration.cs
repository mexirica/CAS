// ServiceConfigurations.cs

using Microservices.CAS.Business;
using Microservices.CAS.Db;
using Microsoft.AspNetCore.Routing.Constraints;

public static class ServiceConfigurations
{
    public static void AddCustomServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<RouteOptions>(options =>
        {
            options.SetParameterPolicy("regex", typeof(RegexInlineRouteConstraint));
        });

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.Configure<RouteOptions>(options => options.LowercaseUrls = true);
        services.Configure<MongoDBSettings>(configuration.GetSection("MongoDB"));

        services.AddSingleton<ContentAddressableStorage>(serviceProvider =>
        {
            var repository = serviceProvider.GetRequiredService<CASFileRepository>();
            return new ContentAddressableStorage(
                configuration["StoragePath"]?? throw new InvalidOperationException("Set a StoragePath"), repository);
        });

        services.AddSingleton<CASFileRepository>();
        services.AddSingleton<MongoDBContext>();
    }
}
