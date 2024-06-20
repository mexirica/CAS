using Microservices.CAS.Db;
using System.Text;
using Microservices.CAS.Models;

namespace Microservices.CAS.Business
{
    /// <summary>
    /// Provides a class for managing content in a content-addressable storage system.
    /// </summary>
    public class ContentAddressableStorage
    {
        private readonly string _storagePath;
        private const int SubdirectoryDepth = 2;
        private const int SubdirectoryLength = 2;
        private readonly CASFileRepository _repository;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentAddressableStorage"/> class.
        /// </summary>
        /// <param name="storagePath">The path to the storage directory.</param>
        /// <param name="repository">The repository for managing CASFile entities.</param>
        public ContentAddressableStorage(string storagePath, CASFileRepository repository)
        {
            _repository = repository;
            _storagePath = storagePath;

            FileManager.EnsureDirectoryExists(storagePath);
        }

        /// <summary>
        /// Stores the given content in the storage system and returns its hash.
        /// </summary>
        /// <param name="content">The content to store.</param>
        /// <param name="name">The name of the file.</param>
        /// <returns>The hash of the stored content.</returns>
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

        /// <summary>
        /// Retrieves the content with the given name from the storage system.
        /// </summary>
        /// <param name="name">The name of the file.</param>
        /// <returns>The content of the file.</returns>
        public async Task<byte[]> RetrieveBytes(string name)
        {
            var file = await _repository.GetByNameAsync(name);
            if (file == null || file.Hash == null) throw new ArgumentException($"File {name} not found.");
            string filePath = GetFilePath(file.Hash);

            if (FileManager.FileExists(filePath))
            {
                return FileManager.ReadFromFile(filePath);
            }

            return null;
        }

        /// <summary>
        /// Deletes the content with the given hash from the storage system.
        /// </summary>
        /// <param name="hash">The hash of the content to delete.</param>
        /// <returns>true if the content was deleted; otherwise, false.</returns>
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

        /// <summary>
        /// Gets the file path for the given hash.
        /// </summary>
        /// <param name="hash">The hash of the file.</param>
        /// <returns>The file path as a string.</returns>
        private string GetFilePath(string hash)
        {
            string subdirectoryPath = GetSubdirectoryPath(hash);
            return Path.Combine(subdirectoryPath, hash);
        }

        /// <summary>
        /// Gets the subdirectory path for the given hash.
        /// The subdirectory path is constructed by appending a portion of the hash to the storage path.
        /// The number of appended portions is determined by the SubdirectoryDepth constant.
        /// </summary>
        /// <param name="hash">The hash of the file.</param>
        /// <returns>The subdirectory path as a string.</returns>
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
