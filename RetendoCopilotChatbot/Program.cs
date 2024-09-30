using Amazon.BedrockRuntime.Model;
using RetendoCopilotChatbot.Models;

namespace RetendoCopilotChatbot
{
    public class Program
    {
        //Class to run the chatbot in the console

        private const string modelId = "anthropic.claude-3-sonnet-20240229-v1:0";
        private const string region = "eu-west-2";
        private const string kbId_manual = "CJHYRDAGBS";
        private const string kbId_support_tickets = "9EUR2ST7ML";
        private const string guardrailIdentifier = "arn:aws:bedrock:eu-west-2:891377104334:guardrail/00lerbhrdat2";
        private const string guardrailVersion = "DRAFT";

        public async static Task Main(string[] args)
        {
            string awsAccessKeyId = SecretManager.GetAwsAccessKey();
            string awsSecretAccessKey = SecretManager.GetAwsSecretAccessKey();

            string startMessage = "Assistant: Hur kan jag hjälpa dig?";

            AwsHelper awsHelper = new AwsHelper(modelId, region, kbId_manual, kbId_support_tickets, awsAccessKeyId, awsSecretAccessKey, guardrailIdentifier, guardrailVersion);
            Copilot copilot = new Copilot(awsHelper);
            List<ChatMessage> chatMessages = new List<ChatMessage>();

            Console.WriteLine(startMessage);

            while (true)
            {
                Console.WriteLine();
                Console.Write("User: ");

                string query = Console.ReadLine()!;

                Console.WriteLine();

                if (query == "exit")
                    break;

                ChatResponse response = await copilot.GetChatResponseAsync(query, chatMessages);

                Console.WriteLine($"Assistant: {response}");
            }
        }

        public async static Task Test()
        {
            //test to send 1000 queries to the knowledge base to see if the OCU went up (this is not a real test). Will not be used by Retendo.

            string awsAccessKeyId = SecretManager.GetAwsAccessKey();
            string awsSecretAccessKey = SecretManager.GetAwsSecretAccessKey();

            AwsHelper awsHelper = new AwsHelper(modelId, region, kbId_manual, kbId_support_tickets, awsAccessKeyId, awsSecretAccessKey, guardrailIdentifier, guardrailVersion);

            for (int i = 1; i <= 1000; i++)
            {
                string query = "Jag kan inte logga in, hur ska jag ta mig åt?";
                List<ChatMessage> chatMessages = new List<ChatMessage>();

                List<string> _context = await awsHelper.RetrieveAsync(query, 3, 3);

                Console.WriteLine(i);
            }

            Console.WriteLine("Done");
        }
    }
}