using RetendoDataHandler.Models;
using RetendoDataHandler.Helper;
using RetendoCopilotChatbot;
using System.Diagnostics.Tracing;

namespace RetendoDataHandler
{
    public class DataHandler
    {
        private string region = "eu-west-2";
        private string bucketName = "retendo-support-tickets";
        private AwsS3Helper awsS3Helper;
        private string s3Folder = "support-tickets";

        public DataHandler(AwsS3Helper awsS3Helper)
        {
            this.awsS3Helper = awsS3Helper;
        }

        public async Task<bool> UploadTickets(string dataPath = null, List<SupportTicketRaw> ticketsRaw = null)
        {
            //this function uploads the support tickets to the specified bucket and path in bucket in AWS S3.

            if (dataPath == null && ticketsRaw == null)
            {
                return false;
            }

            List<SupportTicket> tickets;

            if (!(dataPath == null))
            {
                tickets = PersonalDataHelper.GetSupportTicketsFromFile(dataPath);
            }
            else
            {
                tickets = PersonalDataHelper.GetSupportTicketsFromList(ticketsRaw);
            }

            // Check if the S3 folder exists, if not, create it (commented out because I just add all support tickets to the same folder in the bucket)
            // bool folderExists = await awsS3Helper.FolderExists(bucketName, "", s3Folder);
            // if (!folderExists)
            // {
            //     await awsS3Helper.CreateS3Folder(bucketName, "", s3Folder);
            // }

            // Upload the tickets to S3
            bool success = await awsS3Helper.UploadToS3(bucketName, s3Folder, tickets);
            return success;
        }
    }
}
