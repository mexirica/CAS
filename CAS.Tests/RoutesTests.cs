using System.Net;
using System.Net.Http.Headers;
using System.Text;
using Microservices.CAS.Business;
using Microservices.CAS.Db.Repository.Interfaces;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace CAS.Tests
{
    //TODO: FIX THE TESTS
    public class RoutesTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly Mock<ContentAddressableStorage> _casMock;
        private readonly Mock<ICacheRepository> _cacheMock;

        public RoutesTests(WebApplicationFactory<Program> factory)
        {
            _casMock = new Mock<ContentAddressableStorage>(null, null, null); 
            _cacheMock = new Mock<ICacheRepository>();

            _client = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    services.AddSingleton(_casMock.Object);
                    services.AddSingleton(_cacheMock.Object);
                });
            }).CreateClient();
        }

        [Fact]
        public async Task UploadFile_ReturnsCreated_WhenFileIsUploaded()
        {
            // Arrange
            var filename = "testfile.txt";
            var content = new ByteArrayContent(Encoding.UTF8.GetBytes("This is a test file"));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            
            _cacheMock.Setup(c => c.GetByKeyAsync(filename)).ReturnsAsync((string)null);
            _casMock.Setup(c => c.Store(It.IsAny<byte[]>(), filename)).ReturnsAsync("hash");

            // Act
            var response = await _client.PostAsync($"/v1/upload/{filename}", content);

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        [Fact]
        public async Task UploadFile_ReturnsConflict_WhenFileAlreadyExists()
        {
            // Arrange
            var filename = "testfile.txt";
            var content = new ByteArrayContent("This is a test file"u8.ToArray());
            content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            
            _cacheMock.Setup(c => c.GetByKeyAsync(filename)).ReturnsAsync("existing-hash");

            // Act
            var response = await _client.PostAsync($"/v1/upload/{filename}", content);

            // Assert
            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        }

        [Fact]
        public async Task GetFile_ReturnsFile_WhenFileExists()
        {
            // Arrange
            var filename = "testfile.txt";
            var contentType = "download";
            var fileContent = Encoding.UTF8.GetBytes("This is a test file");
            
            _casMock.Setup(c => c.RetrieveBytes(filename)).ReturnsAsync(fileContent);

            // Act
            var response = await _client.GetAsync($"/v1/{contentType}/{filename}");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("application/octet-stream", response.Content.Headers.ContentType.ToString());
            var responseData = await response.Content.ReadAsByteArrayAsync();
            Assert.Equal(fileContent, responseData);
        }

        [Fact]
        public async Task GetFile_ReturnsNotFound_WhenFileDoesNotExist()
        {
            // Arrange
            var filename = "nonexistentfile.txt";
            var contentType = "download";
            
            _casMock.Setup(c => c.RetrieveBytes(filename)).ReturnsAsync(new byte[0]);

            // Act
            var response = await _client.GetAsync($"/v1/{contentType}/{filename}");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetFile_ReturnsBadRequest_WhenInvalidContentType()
        {
            // Arrange
            var filename = "testfile.txt";
            var contentType = "invalidcontenttype";

            // Act
            var response = await _client.GetAsync($"/v1/{contentType}/{filename}");

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}
