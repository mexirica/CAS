using Microservices.CAS.Business;
using Microservices.CAS.Db;
using Microservices.CAS.Middleware;
using Microsoft.AspNetCore.Routing.Constraints;
using Prometheus;
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateSlimBuilder(args);


Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Warning() 
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .WriteTo.Console() 
    .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day) 
    .CreateLogger();

builder.Host.UseSerilog();

//builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.Configure<RouteOptions>(options => options.LowercaseUrls = true);
builder.Services.Configure<MongoDBSettings>(builder.Configuration.GetSection("MongoDB"));

builder.Services.Configure<RouteOptions>(options =>
{
    options.ConstraintMap["regex"] = typeof(RegexInlineRouteConstraint);
});
builder.Services.AddSingleton<ContentAddressableStorage>(sp =>
{
    var repository = sp.GetRequiredService<CASFileRepository>();
    return new ContentAddressableStorage(builder.Configuration["StoragePath"], repository);
});

builder.Services.AddSingleton<CASFileRepository>();
builder.Services.AddSingleton<MongoDBContext>();

var app = builder.Build();

app.UseMetricServer();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseMiddleware<EndpointLoggingMiddleware>();

app.MapPost("upload/{filename}", async (string filename, ContentAddressableStorage cas, HttpContext context) =>
{
    try
    {
        using var memoryStream = new MemoryStream();
        await context.Request.Body.CopyToAsync(memoryStream);
        var content = memoryStream.ToArray();
        var _ = await cas.Store(content, filename);
        return Results.Created();
    }
    catch (Exception)
    {
        return Results.Problem(detail: "Error uploading file", statusCode: 500);
    }
});

app.MapGet("{contentType}/{filename}", async (string filename, string contentType,ContentAddressableStorage cas) =>
{
    try
    {
        var retrievedContent = await cas.RetrieveBytes(filename);
        return retrievedContent.Any()
            ? contentType switch
            {
               "download" => Results.File(retrievedContent, "application/octet-stream", filename),
                "stream" => Results.File(retrievedContent, "video/mp4", enableRangeProcessing: true),
                _ => throw new ArgumentException("Invalid path.Use 'download' or 'stream' as content type ex:(stream/coding.mp4)")
            } 
            : Results.NotFound($"File '{filename}' not found.");
    }
    catch (ArgumentException ex)
    {
        return Results.NotFound(ex.Message);
    }
    catch (Exception e)
    {
        return Results.Problem(detail: e.Message, statusCode: 500);
    }
});

app.Run();
