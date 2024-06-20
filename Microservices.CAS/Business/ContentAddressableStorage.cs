using System.Text.Json;
using Microservices.CAS.Db;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Text;

namespace Microservices.CAS.Business
{
    public class ContentAddressableStorage
    {
        private readonly string _storagePath;
        private const int SubdirectoryDepth = 2;
        private const int SubdirectoryLength = 2;
        private readonly CASFileRepository _repository;
        
        public ContentAddressableStorage(string storagePath, CASFileRepository repository)
        {
            _repository = repository;
            _storagePath = storagePath;

            FileManager.EnsureDirectoryExists(storagePath);
        }

        public async Task<string> Store(byte[] content, string name)
        {
            string hash = HashHelper.ComputeHash(content);
            string filePath = GetFilePath(hash);

            if (!FileManager.FileExists(filePath))
            {
                FileManager.EnsureDirectoryExists(Path.GetDirectoryName(filePath));
                FileManager.WriteToFile(filePath, content);
                await _repository.CreateAsync(new CASFile { Hash = hash, Name = name });
            }

            return hash;
        }

        public async Task<byte[]> RetrieveBytes(string name)
        {
            var file = await _repository.GetByNameAsync(name);
            if (file == null) throw new ArgumentException($"File {name} not found.");
            string filePath = GetFilePath(file.Hash);

            if (FileManager.FileExists(filePath))
            {
                return FileManager.ReadFromFile(filePath);
            }

            return null;
        }

        public async Task<FileStream> RetrieveStream(string name)
        {
            var file = await _repository.GetByNameAsync(name);
            if (file == null) throw new ArgumentException("File not found.");
            string filePath = GetFilePath(file.Hash);
            return File.OpenRead(filePath);
        }

        public bool Delete(string hash)
        {
            string filePath = GetFilePath(hash);

            if (FileManager.FileExists(filePath))
            {
                FileManager.DeleteFile(filePath);
                return true;
            }

            return false;
        }

        private string GetFilePath(string hash)
        {
            string subdirectoryPath = GetSubdirectoryPath(hash);
            return Path.Combine(subdirectoryPath, hash);
        }

        private string GetSubdirectoryPath(string hash)
        {
            StringBuilder subdirectoryPath = new StringBuilder(_storagePath);

            for (int i = 0; i < SubdirectoryDepth; i++)
            {
                subdirectoryPath.Append(Path.DirectorySeparatorChar);
                subdirectoryPath.Append(hash.Substring(i * SubdirectoryLength, SubdirectoryLength));
            }

            return subdirectoryPath.ToString();
        }
    }
}
