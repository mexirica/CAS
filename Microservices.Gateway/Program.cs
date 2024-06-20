using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateSlimBuilder(args);

var app = builder.Build();

builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);
builder.Services.AddOcelot(builder.Configuration);

app.UseOcelot();

app.Run();
