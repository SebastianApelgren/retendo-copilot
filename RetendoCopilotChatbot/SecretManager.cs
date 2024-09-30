using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetendoCopilotChatbot
{
    public class SecretManager
    {
        //This class is used to retrieve the AWS access key and secret access key from the secret file. (will not be used by Retendo).

        public static string GetAwsAccessKey()
        {
            string secret = ReadSecretFile("AwsAccessKey.txt").Result;
            return secret;
        }

        public static string GetAwsSecretAccessKey()
        {
            string secret = ReadSecretFile("AwsSecretAccessKeys.txt").Result;
            return secret;
        }

        private static async Task<string> ReadSecretFile(string fileName)
        {
            if (!File.Exists(fileName))
            {
                File.Create(fileName);
                throw new FileNotFoundException($"{fileName} not found");
            }
            string secret = await File.ReadAllTextAsync(fileName);
            return secret;
        }
    }
}
