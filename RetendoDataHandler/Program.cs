using RetendoDataHandler.Models;
using System.Text.RegularExpressions;
using RetendoCopilotChatbot;
using RetendoDataHandler.Helper;
using System.Text.Json;
using Amazon.S3.Model;
using System.Reflection.Metadata;

namespace RetendoDataHandler
{
    internal class Program
    {
        private static string region = "eu-west-2";
        private static string bucketName = "retendo-support-tickets";

        static async Task Main(string[] args)
        {
            string path = "Data";
            string s3Folder = "support-tickets";

            bool success = await UploadTickets(path, s3Folder);

            if (!success)
            {
                Console.WriteLine($"Failed to upload support tickets in {path}/ to S3");
            }
            else
            {
                Console.WriteLine($"Support tickets in {path}/ uploaded to S3");
            }
        }

        private async static Task<bool> UploadTickets(string path, string s3Folder)
        {
            List<SupportTicket> tickets = PersonalDataHelper.GetSupportTickets(path);

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


/*
 * Todo:
 *  - lägg till metod i awsHelper så att den laddar direkt upp i S3 bucket
 *  - lägg till så att modelen tar 5 sup ärenden och 5 artiklar och gör ett svar.
 *  - KLAR // lägg till så att den tar bort namn från supporttickets. 
 * 
 */