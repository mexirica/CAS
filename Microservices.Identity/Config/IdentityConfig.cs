using Microservices.Identity.DB;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NetDevPack.Identity.Jwt;
using NetDevPack.Security.PasswordHasher.Core;

namespace Microservices.Identity.Config;

public static class IdentityConfig
{
    public static IServiceCollection AddIdentityConfiguration(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<UserContext>(opts =>
        {
            opts.UseNpgsql(configuration.GetConnectionString("Postgres"));
        });
        
        services.AddMemoryCache()
            .AddDataProtection();

        services.AddJwtConfiguration(configuration, "AppSettings")
            .AddNetDevPackIdentity<IdentityUser>()
            .PersistKeysToDatabaseStore<UserContext>();

        services.AddIdentity<IdentityUser, IdentityRole>(o =>
            {
                o.Password.RequireDigit = false;
                o.Password.RequireLowercase = false;
                o.Password.RequireNonAlphanumeric = false;
                o.Password.RequireUppercase = false;
                o.Password.RequiredUniqueChars = 0;
                o.Password.RequiredLength = 8;
            })
            .AddEntityFrameworkStores<UserContext>()
            .AddDefaultTokenProviders();

        services.UpgradePasswordSecurity()
            .WithStrengthen(PasswordHasherStrength.Moderate)
            .UseArgon2<IdentityUser>();

        return services;
    }
}
