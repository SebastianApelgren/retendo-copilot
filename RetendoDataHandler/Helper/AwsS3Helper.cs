using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using RetendoDataHandler.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace RetendoDataHandler.Helper
{
    public class AwsS3Helper
    {
        //This class is used to interact with AWS S3. It can upload support tickets to a specified bucket and create folders in the bucket.

        private string region;
        private IAmazonS3 client;

        public AwsS3Helper(string region, string awsAccessKey, string awsSecretAccessKey)
        {
            this.region = region;

            AmazonS3Config clientConfig = new AmazonS3Config
            {
                RegionEndpoint = Amazon.RegionEndpoint.GetBySystemName(region),
                LogResponse = true,
                LogMetrics = true,
            };

            client = new AmazonS3Client(awsAccessKey, awsSecretAccessKey, RegionEndpoint.GetBySystemName(region));
        }

        public async Task<bool> UploadToS3(string bucketName, string pathInBucket, List<SupportTicket> tickets)
        {
            //This function uploads the support tickets to the specified bucket and path in bucket in AWS S3.

            if (pathInBucket != "" && !pathInBucket.EndsWith("/"))
            {
                pathInBucket += "/";
            }

            foreach (SupportTicket ticket in tickets)
            {
                string content = JsonSerializer.Serialize(ticket);
                string fileName = $"{ticket.Subject}_{Guid.NewGuid()}.txt";

                PutObjectRequest putRequest = new PutObjectRequest
                {
                    BucketName = bucketName,
                    Key = $"{pathInBucket}{fileName}",
                    ContentBody = content,
                    BucketKeyEnabled = true,
                };

                try
                {
                    PutObjectResponse response = await client.PutObjectAsync(putRequest);

                    if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
                        continue;
                    else
                        return false;
                }
                catch (AmazonS3Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }

            return true;
        }

        public async Task CreateS3Folder(string bucketName, string path, string name)
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

        public async Task<bool> FolderExists(string bucketName, string path, string name)
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
    }
}
