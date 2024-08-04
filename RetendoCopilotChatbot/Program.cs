using RetendoCopilotChatbot.Models;

namespace RetendoCopilotChatbot
{
    public class Program
    {
        private const string modelId = "anthropic.claude-3-sonnet-20240229-v1:0";
        private const string region = "eu-west-2";
        private const string kbId = "CJHYRDAGBS";

        public async static Task Main(string[] args)
        {
            string awsAccessKeyId = SecretManager.GetAwsAccessKey();
            string awsSecretAccessKey = SecretManager.GetAwsSecretAccessKey();

            while (true)
            {
                Console.Clear();

                Console.WriteLine("Hur kan jag hjälpa dig?");

                string query = Console.ReadLine()!;

                if (query == "exit")
                    break;

                AwsHelper awsHelper = new AwsHelper(modelId, region, kbId, awsAccessKeyId, awsSecretAccessKey);
                InvokeModelResult response = await awsHelper.GenerateResponseAsync(query);

                Console.WriteLine(response.MessageText);

                Console.ReadLine();
            }
        }
    }
}