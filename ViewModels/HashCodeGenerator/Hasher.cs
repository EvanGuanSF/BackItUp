using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System.Diagnostics;

namespace BackItUp.ViewModels.HashCodeGenerator
{
    public static class Hasher
    {
        /// <summary>
        /// Generates a 32 byte hash for the given input and returns it as a hex string.
        /// </summary>
        /// <param name="inputString"></param>
        /// <returns>inputString's SHA-256 hash code as a hex string</returns>
        public static string StringHasher(string inputString)
        {
            if(inputString.Length < 1)
            {
                return null;
            }

            string hashResult = null;

            using(SHA256 hasher256 = SHA256.Create())
            {
                hashResult = BitConverter.ToString(hasher256.ComputeHash(Encoding.UTF8.GetBytes(inputString.ToLower()))).Replace("-", string.Empty);
            }

            Debug.WriteLine("Hashed: " + hashResult);

            return hashResult;
        }
    }
}
