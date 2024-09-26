using RetendoCopilotChatbot.Models;
using System.Diagnostics;

namespace RetendoCopilotChatbot
{
    public class Copilot
    {
        private AwsHelper awsHelper;

        public Copilot(AwsHelper awsHelper)
        {
            this.awsHelper = awsHelper;
        }

        public async Task<ChatResponse> GetChatResponseAsync(string query, List<ChatMessage> chatMessages, int numberOfResultsManuals = 3, int numberOfResultsTickets = 5)
        {
            // This function is responsible for generating a response to a user query. It first determines if the query should be answered by the chatbot or if it should be searched in the documentation.
            // If the query should be answered, it retrieves the relevant contexts from AWS and generates a response based on those contexts.

            TimingInformation timingInformation = new TimingInformation();
            Stopwatch totalTimeWatch = Stopwatch.StartNew();

            CostInformation costInformation = new CostInformation();

            string prompt = Prompts.ChatbotSystemPrompt;

            Stopwatch queryVerdictStopwatch = Stopwatch.StartNew();
            string queryVerdict = await GetShouldSearchInDocumentationOrShouldAnswerAsync(query, costInformation);
            timingInformation.RegisterTiming(queryVerdictStopwatch, "query initial validation");

            List<string> contexts = new List<string>();

            if (queryVerdict.Contains("olämpligt"))
            {
                return new ChatResponse("Jag kan inte svara på den frågan. Vänligen kontakta Retendo's kundtjänst direkt så kan de hjälpa dig.", timingInformation, costInformation);
            }
            else if (queryVerdict == "ja")
            {
                Stopwatch retrieveContextTime = Stopwatch.StartNew();

                //removes old documents and tickets as only 5 documents/tickets can be sent at a time.
                chatMessages.RemoveDocumentsAndTickets();
                contexts = await awsHelper.RetrieveAsync(query, numberOfResultsManuals, numberOfResultsTickets);

                try
                {
                    List<string> documents = contexts.GetRange(0, numberOfResultsManuals);
                    List<string> tickets = contexts.GetRange(numberOfResultsManuals, numberOfResultsTickets);

                    chatMessages.Add(ChatMessage.CreateFromUser(query, documents.CreateContextChunk(), tickets.CreateContextChunk()));
                }
                catch(IndexOutOfRangeException)
                {
                    throw new InvalidDataException($"Tried to get documents and tickets but the list of fetched contexts did not contain them the expected amount of documents/tickets. Expected amount: {numberOfResultsManuals + numberOfResultsTickets}, actual amount: {contexts.Count}. Something may be wrong when fetching contexts or the appropriate contexts are not in aws.");
                }
                
                timingInformation.RegisterTiming(retrieveContextTime, "retrieve context");
            }
            else if (queryVerdict == "nej")
                chatMessages.Add(ChatMessage.CreateFromUser(query, null, null));
            else
                return new ChatResponse("Jag kan inte svara på den frågan. Vänligen kontakta Retendo's kundtjänst direkt så kan de hjälpa dig.", timingInformation, costInformation);

            Stopwatch generateResponseWatch = Stopwatch.StartNew();
            ConversationResponse response = await awsHelper.GenerateConversationResponseAsync(chatMessages, prompt);
            timingInformation.RegisterTiming(generateResponseWatch, "generate response");
            costInformation.AddTokens(response.inputTokens, response.outputTokens);

            if (response.guardrailIntervened)
                chatMessages.RemoveAt(chatMessages.Count - 1);
            else
                chatMessages.Add(response.ChatMessage);

            timingInformation.RegisterTiming(totalTimeWatch, "total time");

            return new ChatResponse(response.ChatMessage.Content, timingInformation, costInformation);
        }

        public async Task<string> GetShouldSearchInDocumentationAsync(string query, CostInformation costInformation)
        {
            string prompt = string.Format(Prompts.DocumentNeededPrompt, query);

            InvokeModelResult result = await awsHelper.GenerateResponseAsync(prompt);

            costInformation.CountAndAddTokens(prompt, result.MessageText);

            return result.MessageText;
        }

        public async Task<string> GetShouldSearchInDocumentationOrShouldAnswerAsync(string query, CostInformation costInformation)
        {
            string prompt = string.Format(Prompts.DocumentNeededAndRelevantQuestionPrompt, query);

            InvokeModelResult result = await awsHelper.GenerateResponseAsync(prompt);

            costInformation.CountAndAddTokens(prompt, result.MessageText);

            return result.MessageText;
        }
    }
}