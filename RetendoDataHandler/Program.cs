using RetendoDataHandler.Models;
using RetendoCopilotChatbot;
using RetendoDataHandler.Helper;

namespace RetendoDataHandler
{
    internal class Program
    {
        private static string region = "eu-west-2";
        private static string bucketName = "retendo-support-tickets";

        static async Task Main(string[] args)
        {
            //run this program to upload all support tickets in the Data folder to S3.

            string path = "Data";
            string s3Folder = "support-tickets";

            List<SupportTicket> tickets = PersonalDataHelper.GetSupportTickets(path);

            bool success = await UploadTickets(tickets, s3Folder);

            if (!success)
            {
                Console.WriteLine($"Failed to upload support tickets in {path}/ to S3");
            }
            else
            {
                Console.WriteLine($"Support tickets in {path}/ uploaded to S3");
            }
        }

        private async static Task<bool> UploadTickets(List<SupportTicket> tickets, string s3Folder)
        {
            //this function uploads the support tickets to the specified bucket and path in bucket in AWS S3.

            string awsAccessKey = SecretManager.GetAwsAccessKey();
            string awsSecretAccessKey = SecretManager.GetAwsSecretAccessKey();

            AwsS3Helper awsHelper = new AwsS3Helper(region, awsAccessKey, awsSecretAccessKey);

            bool folderExists = await awsHelper.FolderExists(bucketName, "", s3Folder);

            if (!folderExists)
            {
                await awsHelper.CreateS3Folder(bucketName, "", s3Folder);
            }

            bool success = await awsHelper.UploadToS3(bucketName, s3Folder, tickets);
            return success;
        }
    }
}