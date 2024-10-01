using RetendoCopilotChatbot;
using RetendoDataHandler;
using RetendoDataHandler.Helper;
using RetendoDataHandler.Models;

namespace RetendoDataHandler
{
    internal class Program
    {
        private static string region = "eu-west-2";
        private static string bucketName = "retendo-support-tickets";
        private static string path = "Data";

        static async Task Main(string[] args)
        {
            AwsS3Helper awsS3Helper = new AwsS3Helper(region, SecretManager.GetAwsAccessKey(), SecretManager.GetAwsSecretAccessKey(), bucketName);

            // Define the local data path and S3 folder name
            
            DataHandler dataHandler = new DataHandler(awsS3Helper);

            // Upload the support tickets to S3
            UploadTicketResult result = await dataHandler.UploadTickets(dataPath: path);

        }
    }
}
