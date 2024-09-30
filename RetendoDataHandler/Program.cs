using RetendoCopilotChatbot;
using RetendoDataHandler;
using RetendoDataHandler.Helper;

namespace RetendoDataHandler
{
    internal class Program
    {
        private static string region = "eu-west-2";

        static async Task Main(string[] args)
        {
            AwsS3Helper awsS3Helper = new AwsS3Helper(region, SecretManager.GetAwsAccessKey(), SecretManager.GetAwsSecretAccessKey());

            // Define the local data path and S3 folder name
            string path = "Data";

            // Create an instance of DataHandler
            DataHandler dataHandler = new DataHandler(awsS3Helper);

            // Upload support tickets using DataHandler
            bool success = await dataHandler.UploadTickets(path);

            // Output the result of the upload operation
            if (!success)
            {
                Console.WriteLine($"Failed to upload support tickets in {path}/ to S3");
            }
            else
            {
                Console.WriteLine($"Support tickets in {path}/ uploaded to S3");
            }
        }
    }
}
