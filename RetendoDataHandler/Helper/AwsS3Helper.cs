using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using RetendoCopilotChatbot.Models;
using RetendoDataHandler.Models;
using System.Text.Json;


namespace RetendoDataHandler.Helper
{
    public class AwsS3Helper
    {
        //This class is used to interact with AWS S3. It can upload support tickets to a specified bucket and create folders in the bucket.

        private string region;
        private IAmazonS3 client;
        private string bucketName;
        private string pathInS3Bucket = "support-tickets/";

        public AwsS3Helper(string region, string awsAccessKey, string awsSecretAccessKey, string bucketName)
        {
            this.region = region;

            AmazonS3Config clientConfig = new AmazonS3Config
            {
                RegionEndpoint = Amazon.RegionEndpoint.GetBySystemName(region),
                LogResponse = true,
                LogMetrics = true,
            };

            client = new AmazonS3Client(awsAccessKey, awsSecretAccessKey, RegionEndpoint.GetBySystemName(region));
            this.bucketName = bucketName;
        }

        public async Task<UploadTicketResult> UploadToS3(List<SupportTicket> tickets)
        {
            //This function uploads the support tickets to the specified bucket in AWS S3 if the ticket isn't already uploaded.

            HashSet<string> objectNames = await GetAllObjectNamesAsync();
            List<Tuple<string, SupportTicket>> ticketsToUpload = new List<Tuple<string, SupportTicket>>();

            int uploads = 0;

            foreach (SupportTicket ticket in tickets)
            {
                string content = JsonSerializer.Serialize(ticket);
                string hash = content.ToHash();
                if (!objectNames.Contains(hash))
                {
                    objectNames.Add(hash);
                    ticketsToUpload.Add(new Tuple<string, SupportTicket>(hash, ticket));
                    uploads++;
                }
            }

            List<Task<PutObjectResponse>> tasks = new List<Task<PutObjectResponse>>();
            List<PutObjectResponse> results = new List<PutObjectResponse>();

            int counter = 0;

            foreach (Tuple<string, SupportTicket> ticket in ticketsToUpload)
            {
                counter++;
                if (counter % 100 == 0)
                {
                    results.AddRange(await Task.WhenAll(tasks));
                }
                string content = JsonSerializer.Serialize(ticket.Item2);

                PutObjectRequest putRequest = new PutObjectRequest
                {
                    BucketName = bucketName,
                    ContentBody = content,
                    Key = $"{pathInS3Bucket}{ticket.Item1}",
                    BucketKeyEnabled = true,
                };

                tasks.Add(client.PutObjectAsync(putRequest));
            }

            results.AddRange(await Task.WhenAll(tasks));

            int uploaded = 0;

            foreach (PutObjectResponse result in results)
            {
                if (result.HttpStatusCode == System.Net.HttpStatusCode.OK)
                {
                    uploaded++;
                }
            }

            return new UploadTicketResult(true, $"Successfully uploaded {uploaded}/{ticketsToUpload.Count} tickets. Skipped {tickets.Count - ticketsToUpload.Count} tickets that were sent as they are doublicates.");
        }

        public async Task CreateS3Folder(string path, string name)
        {
            //This function creates a folder in the specified bucket and path in bucket in AWS S3.

            if (path != "" && !path.EndsWith("/"))
            {
                path += "/";
            }

            path += name;

            PutObjectRequest putRequest = new PutObjectRequest
            {
                BucketName = bucketName,
                Key = $"{path}/",
            };

            await client.PutObjectAsync(putRequest);
        }

        public async Task<bool> FolderExists(string path, string name)
        {
            //This function checks if a folder exists in the specified bucket and path in bucket in AWS S3.

            ListObjectsRequest request = new ListObjectsRequest()
            {
                BucketName = bucketName,
                Prefix = path,
            };

            ListObjectsResponse response = await client.ListObjectsAsync(request);

            if (path != "" && !path.EndsWith("/"))
            {
                path += "/";
            }

            foreach (S3Object obj in response.S3Objects)
            {
                if (obj.Key == $"{path}{name}/")
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<HashSet<string>> GetAllObjectNamesAsync()
        {
            //this class gets all object names in the specified bucket in AWS S3. It only gets the last 16 characters of the object name, which is a hash of the content.

            HashSet<string> objectNames = new HashSet<string>();

            string? marker = null;
            ListObjectsResponse response;

            do
            {
                ListObjectsRequest request = new ListObjectsRequest()
                {
                    BucketName = bucketName,
                };

                if (marker != null)
                {
                    request.Marker = marker;
                }

                response = await client.ListObjectsAsync(request);

                foreach (S3Object obj in response.S3Objects)
                {
                    string hash = obj.Key.TakeLast(16);

                    objectNames.Add(hash);
                }

                marker = response.NextMarker;

            } while (response.IsTruncated);

            return objectNames;
        }
    }
}
