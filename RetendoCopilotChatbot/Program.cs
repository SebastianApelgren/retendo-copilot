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

            string startMessage = "Assistant: Hur kan jag hjälpa dig?";

            AwsHelper awsHelper = new AwsHelper(modelId, region, kbId, awsAccessKeyId, awsSecretAccessKey);
            Copilot copilot = new Copilot(awsHelper);
            List<ChatMessage> chatMessages = new List<ChatMessage>();

            Console.WriteLine(startMessage);

            while (true)
            {
                string query = Console.ReadLine()!;

                if (query == "exit")
                    break;

                string response = await copilot.GetChatResponseAsync(query, chatMessages);

                Console.WriteLine(response);
            }
        }
    }
}