using System.Text;
using Microservices.CAS.Business;

namespace CAS.Tests;

public class HashHelperTests
{
    [Fact]
    public void ComputeHash_ReturnsExpectedHash_ForNonEmptyData()
    {
        // Setup
        var data = "Hello World"u8.ToArray();
        var expectedHash = "a591a6d40bf420404a011733cfb7b190d62c65bf0bcda32b57b277d9ad9f146e";

        // Act
        var result = HashHelper.ComputeHash(data);

        // Assert
        Assert.Equal(expectedHash, result);
    }

    [Fact]
    public void ComputeHash_ReturnsDifferentHashes_ForDifferentData()
    {
        // Setup
        var data1 = Encoding.UTF8.GetBytes("Hello World");
        var data2 = Encoding.UTF8.GetBytes("Goodbye World");

        // Act
        var hash1 = HashHelper.ComputeHash(data1);
        var hash2 = HashHelper.ComputeHash(data2);

        // Assert
        Assert.NotEqual(hash1, hash2);
    }

    [Fact]
    public void ComputeHash_ReturnsConsistentHash_ForSameData()
    {
        // Setup
        var data = Encoding.UTF8.GetBytes("Consistent Data");

        // Act
        var hash1 = HashHelper.ComputeHash(data);
        var hash2 = HashHelper.ComputeHash(data);

        // Assert
        Assert.Equal(hash1, hash2);
    }

    [Fact]
    public void ComputeHash_ReturnsValidHash_ForEmptyData()
    {
        // Setup
        var data = Encoding.UTF8.GetBytes("");
        var expectedHash = "e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855";

        // Act
        var result = HashHelper.ComputeHash(data);

        // Assert
        Assert.Equal(expectedHash, result);
    }
}
