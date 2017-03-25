using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Security;

namespace FootballAIGame.Web.Helpers
{
    public static class AccessKeyGenerator
    {
        public const int AccessKeyLength = 8;

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