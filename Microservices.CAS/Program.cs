using Microservices.CAS.Business;
using Microservices.CAS.Configs;
using Microservices.CAS.Middleware;
using Microservices.CAS.Routes;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);

builder.Host.AddLog();

builder.Services.AddCustomServices(builder.Configuration);

var app = builder.Build();

app.UseMetricServer();
app.UseMiddleware<EndpointLoggingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();
app.MapFileRoutes(app.Services.GetRequiredService<ContentAddressableStorage>());

app.Run();
