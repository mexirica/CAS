using System.Text.Json;
using Microservices.CAS.Db;

namespace Microservices.CAS.Business;

public static class FileManager
{
    public static void EnsureDirectoryExists(string path)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
    }

    public static byte[] ReadFromFile(string filePath)
    {
        return File.ReadAllBytes(filePath);
    }
    
    public static void WriteToFile(string filePath, byte[] content)
    {
        File.WriteAllBytes(filePath, content);
    }

    public static bool FileExists(string filePath)
    {
        return File.Exists(filePath);
    }

    public static void DeleteFile(string filePath)
    {
        if (FileExists(filePath))
        {
            File.Delete(filePath);
        }
    }
}
