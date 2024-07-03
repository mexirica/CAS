namespace Microservices.CAS.Business
{
    using System.Security.Cryptography;
    using System.Text;

    /// <summary>
    /// Provides a helper class for computing hashes.
    /// </summary>
    public static class HashHelper
    {
        /// <summary>
        /// Computes the SHA256 hash for the given data.
        /// </summary>
        /// <param name="data">The data to compute the hash for.</param>
        /// <returns>The computed hash as a hexadecimal string.</returns>
        public static string ComputeHash(byte[] data)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(data);
                StringBuilder hashString = new StringBuilder();
                foreach (byte b in hashBytes)
                {
                    hashString.Append(b.ToString("x2"));
                }
                return hashString.ToString();
            }
        }
    }
}
