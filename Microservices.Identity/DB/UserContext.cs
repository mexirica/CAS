using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NetDevPack.Security.Jwt.Core.Model;
using NetDevPack.Security.Jwt.Store.EntityFrameworkCore;

namespace Microservices.Identity.DB;

public class UserContext : IdentityDbContext, ISecurityKeyContext
{
    public UserContext(DbContextOptions<UserContext> opts) : base(opts) { }
    
    public DbSet<KeyMaterial> SecurityKeys { get; set; }
}
