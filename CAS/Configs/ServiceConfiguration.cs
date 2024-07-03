using Microservices.CAS.Business;
using Microservices.CAS.Db;
using Microservices.CAS.Db.Repository;
using Microservices.CAS.Db.Repository.Interfaces;
using Microsoft.AspNetCore.Routing.Constraints;
using StackExchange.Redis;

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
            var cache = serviceProvider.GetRequiredService<ICacheRepository>();
            return new ContentAddressableStorage(
                configuration["StoragePath"]?? throw new InvalidOperationException("Set a StoragePath"), repository, cache);
        });


        services.AddSingleton<CASFileRepository>();
        services.AddSingleton<MongoDBContext>();

        var redisConnectionString = "localhost";
        services.AddSingleton<IDatabase>(sp =>
        {
            var db = ConnectionMultiplexer.Connect(redisConnectionString);
            return db.GetDatabase();
        });

        services.AddSingleton<ICacheRepository, RedisRepository>();
        
        services.AddHealthChecks();
    }
}
