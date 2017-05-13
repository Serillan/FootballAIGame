using System.Security.Cryptography;
using System.Text;

namespace FootballAIGame.DbModel.Utilities
{
    /// <summary>
    /// Provides the method for generating an access key.
    /// </summary>
    public static class AccessKeyGenerator
    {
        /// <summary>
        /// The access key length.
        /// </summary>
        public const int AccessKeyLength = 8;

        /// <summary>
        /// Generates a new access key by using cryptographically secure pseudo-random number generator.
        /// </summary>
        /// <returns>The new access key.</returns>
        public static string Generate()
        {
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            var bytes = new byte[AccessKeyLength];

            using (var randomGenerator = new RNGCryptoServiceProvider())
            {
                randomGenerator.GetBytes(bytes);
            }

            var stringBuilder = new StringBuilder(AccessKeyLength);
            foreach (var b in bytes)
            {
                stringBuilder.Append(chars[b % (chars.Length)]);
            }

            return stringBuilder.ToString();
        }
    }
}