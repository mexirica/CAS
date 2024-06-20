using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

var mediaPath = builder.Configuration["MediaPath"];

app.MapGet("/stream/{filename}", (string filename) =>
{
    try
    {
        string filePath = Path.Combine(mediaPath, $"{filename}.mp4");

        // Verifique se o arquivo existe
        if (!File.Exists(filePath))
        {
            return Results.NotFound($"Arquivo 'coding.mp4' não encontrado.");
        }

        // Abra o arquivo e retorne como stream de resposta
        var stream = File.OpenRead(filePath);
        return Results.Stream(stream, "video/mp4", enableRangeProcessing: true);
    }
    catch (Exception e)
    {
        return Results.Problem(detail: e.Message, statusCode: 500);
    }
});

app.Run();
