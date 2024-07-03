using Microservices.CAS.Business;

namespace CAS.Tests;

public class FileManagerTests
{
    [Fact]
    public void EnsureDirectoryExists_CreatesDirectory_WhenItDoesNotExist()
    {
        // Setup
        var path = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

        // Act
        FileManager.EnsureDirectoryExists(path);

        // Assert
        Assert.True(Directory.Exists(path));

        // Cleanup
        Directory.Delete(path);
    }

    [Fact]
    public async Task ReadFromFile_ReturnsFileContent_WhenFileExists()
    {
        // Setup
        var filePath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        var expectedContent = new byte[] { 1, 2, 3, 4, 5 };
        await File.WriteAllBytesAsync(filePath, expectedContent);

        // Act
        var content = await FileManager.ReadFromFile(filePath);

        // Assert
        Assert.Equal(expectedContent, content);

        // Cleanup
        File.Delete(filePath);
    }

    [Fact]
    public async Task WriteToFile_CreatesAndWritesToFile_WhenFileDoesNotExist()
    {
        // Setup
        var filePath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        var contentToWrite = new byte[] { 5, 4, 3, 2, 1 };

        // Act
        await FileManager.WriteToFile(filePath, contentToWrite);

        // Assert
        Assert.True(File.Exists(filePath));
        var writtenContent = await File.ReadAllBytesAsync(filePath);
        Assert.Equal(contentToWrite, writtenContent);

        // Cleanup
        File.Delete(filePath);
    }

    [Fact]
    public void FileExists_ReturnsTrue_WhenFileExists()
    {
        // Setup
        var filePath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        File.Create(filePath).Dispose();

        // Act & Assert
        Assert.True(FileManager.FileExists(filePath));

        // Cleanup
        File.Delete(filePath);
    }

    [Fact]
    public void FileExists_ReturnsFalse_WhenFileDoesNotExist()
    {
        // Setup
        var filePath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

        // Act & Assert
        Assert.False(FileManager.FileExists(filePath));
    }

    [Fact]
    public async Task DeleteFile_DeletesFile_WhenFileExists()
    {
        // Setup
        var filePath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        File.Create(filePath).Dispose();

        // Act
        await FileManager.DeleteFile(filePath);

        // Assert
        Assert.False(File.Exists(filePath));
    }

    [Fact]
    public async Task DeleteFile_DoesNothing_WhenFileDoesNotExist()
    {
        // Setup
        var filePath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

        // Act
        await FileManager.DeleteFile(filePath);

        // Assert
        Assert.False(File.Exists(filePath));
    }
}
