namespace Microservices.CAS.Business
{
    /// <summary>
    /// Provides utility methods for file management.
    /// </summary>
    public static class FileManager
    {
        /// <summary>
        /// Ensures that a directory exists at the specified path. 
        /// If the directory does not exist, it is created.
        /// </summary>
        /// <param name="path">The path where the directory should exist.</param>
        public static void EnsureDirectoryExists(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        /// <summary>
        /// Reads all bytes from a file at the specified path.
        /// </summary>
        /// <param name="filePath">The path of the file to read.</param>
        /// <returns>A byte array containing the contents of the file.</returns>
        public static async ValueTask<byte[]> ReadFromFile(string filePath)
        {
            return await File.ReadAllBytesAsync(filePath);
        }

        /// <summary>
        /// Writes the specified byte array to a file at the specified path.
        /// If the file already exists, it is overwritten.
        /// </summary>
        /// <param name="filePath">The path of the file to write to.</param>
        /// <param name="content">The byte array to write to the file.</param>
        public static async ValueTask WriteToFile(string filePath, byte[] content)
        {
           await File.WriteAllBytesAsync(filePath, content);
        }

        /// <summary>
        /// Checks whether a file exists at the specified path.
        /// </summary>
        /// <param name="filePath">The path of the file to check.</param>
        /// <returns>true if the file exists; otherwise, false.</returns>
        public static bool FileExists(string filePath)
        {
            return File.Exists(filePath);
        }

        /// <summary>
        /// Deletes the file at the specified path.
        /// If the file does not exist, no exception is thrown.
        /// </summary>
        /// <param name="filePath">The path of the file to delete.</param>
        public static async ValueTask DeleteFile(string filePath)
        {
            if (FileExists(filePath))
            {
							await Task.Run(() => File.Delete(filePath));
            }
        }
    }
}
