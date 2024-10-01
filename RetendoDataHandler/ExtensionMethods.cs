using Amazon.S3.Model;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;

namespace RetendoDataHandler
{
    public static class ExtensionMethods
    {
        // Method to generate SHA256 hash of a given string
        public static string ToHash(this string input)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
                return BitConverter.ToString(bytes).Replace("-", "").Substring(0, 16); // Shorten to 16 characters
            }
        }

        public static string TakeLast(this string input, int lenght)
        {
            if (lenght >= input.Length)
                return input;
            return input.Substring(input.Length - lenght);
        }
    }
}
