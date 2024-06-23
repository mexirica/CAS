using Microservices.Identity.DB;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.AddDbContext<UserContext>(opts =>
{
    opts.UseNpgsql(builder.Configuration.GetConnectionString("Postgres"));
});

builder.Services.AddMemoryCache().AddDataProtection();



var app = builder.Build(); 


app.Run();
