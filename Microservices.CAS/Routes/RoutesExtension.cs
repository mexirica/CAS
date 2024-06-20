using Microservices.CAS.Business;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Microservices.CAS.Routes
{
    /// <summary>
    /// Provides extension methods for mapping file routes.
    /// </summary>
    public static class RoutesExtension
    {
        /// <summary>
        /// Maps file routes for uploading and retrieving files.
        /// </summary>
        /// <param name="endpoints">The IEndpointRouteBuilder instance.</param>
        /// <param name="cas">The ContentAddressableStorage instance.</param>
        /// <returns>The IEndpointRouteBuilder instance with mapped routes.</returns>
        public static IEndpointRouteBuilder MapFileRoutes(this IEndpointRouteBuilder endpoints, ContentAddressableStorage cas)
        {
            var fileRoutes = endpoints.MapGroup("/v1");

            // Map POST route for file upload
            fileRoutes.MapPost("/upload/{filename}", async (string filename, HttpContext context) =>
            {
                try
                {
                    using var memoryStream = new MemoryStream();
                    await context.Request.Body.CopyToAsync(memoryStream);
                    var content = memoryStream.ToArray();
                    await cas.Store(content, filename);
                    return Results.Created();
                }
                catch (Exception)
                {
                    return Results.Problem(detail: "Error uploading file", statusCode: 500);
                }
            });

            // Map GET route for file retrieval
            fileRoutes.MapGet("/{contentType}/{filename}", async (string filename, string contentType) =>
            {
                try
                {
                    if (contentType != "download" && contentType != "stream")
                    {
                        return Results.BadRequest(
                            "Invalid path. Use 'download' or 'stream' as content type. Example: stream/coding.mp4");
                    }

                    var retrievedContent = await cas.RetrieveBytes(filename);

                    if (!retrievedContent.Any()) return Results.NotFound($"File '{filename}' not found.");

                    var fileResult = contentType == "download"
                        ? Results.File(retrievedContent, "application/octet-stream", filename)
                        : Results.File(retrievedContent, "video/mp4", enableRangeProcessing: true);

                    return fileResult;
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

            return endpoints;
        }
    }
}
