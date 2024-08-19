﻿using RetendoCopilotChatbot.Models;
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

        public async Task<ChatResponse> GetChatResponseAsync(string query, List<ChatMessage> chatMessages)
        {
            TimingInformation timingInformation = new TimingInformation();
            Stopwatch totalTimeWatch = Stopwatch.StartNew();

            string prompt = Prompts.ChatbotSystemPrompt;

            Stopwatch queryVerdictStopwatch = Stopwatch.StartNew();
            string queryVerdict = await GetShouldSearchInDocumentationOrShouldAnswerAsync(query);
            timingInformation.RegisterTiming(queryVerdictStopwatch, "query initial validation");

            List<string> contexts = new List<string>();

            int numberOfResults = 5;

            if (queryVerdict.Contains("olämpligt"))
            {
                return new ChatResponse("Jag kan inte svara på den frågan. Vänligen kontakta Retendo's kundtjänst direkt så kan de hjälpa dig.", timingInformation);
            }
            else if (queryVerdict == "ja")
            {
                Stopwatch retrieveContextTime = Stopwatch.StartNew();
                
                chatMessages.RemoveDocumentsAndTickets(); //removes old documents and tickets as only 5 documents/tickets can be sent at a time.
                contexts = await awsHelper.RetrieveAsync(query, numberOfResults);

                try
                {
                    List<string> documents = contexts.GetRange(0, numberOfResults);
                    List<string> tickets = contexts.GetRange(numberOfResults, numberOfResults);

                    chatMessages.Add(ChatMessage.CreateFromUser(query, documents.CreateContextChunk(), tickets.CreateContextChunk()));
                }
                catch(IndexOutOfRangeException)
                {
                    throw new InvalidDataException($"Tried to get documents and tickets but the list of fetched contexts did not contain them the expected amount of documents/tickets. Expected amount: {numberOfResults * 2}, actual amount: {contexts.Count}. Something may be wrong when fetching contexts or the appropriate contexts are not in aws.");
                }
                
                timingInformation.RegisterTiming(retrieveContextTime, "retrieve context");
            }
            else
                chatMessages.Add(ChatMessage.CreateFromUser(query, null, null));

            Stopwatch generateResponseWatch = Stopwatch.StartNew();
            ChatMessage response = await awsHelper.GenerateConversationResponseAsync(chatMessages, prompt);
            timingInformation.RegisterTiming(generateResponseWatch, "generate response");

            Stopwatch isSensitiveWatch = Stopwatch.StartNew();
            bool isSensitive = await IsSensitiveInformation(response.Content);
            timingInformation.RegisterTiming(isSensitiveWatch, "is sensitive check");

            if (isSensitive)
            {
                chatMessages.RemoveAt(chatMessages.Count - 1);
                return new ChatResponse("Jag kan inte svara på den frågan. Vänligen kontakta Retendo's kundtjänst direkt så kan de hjälpa dig.", timingInformation);
            }

            chatMessages.Add(response);

            timingInformation.RegisterTiming(totalTimeWatch, "total time");

            return new ChatResponse(response.Content, timingInformation);
        }

        public async Task<string> GetShouldSearchInDocumentationOrShouldAnswerAsync(string query)
        {
            string prompt = string.Format(Prompts.DocumentNeededAndRelevantQuestionPrompt, query);

            InvokeModelResult result = await awsHelper.GenerateResponseAsync(prompt);

            return result.MessageText;
        }

        public async Task<bool> IsSensitiveInformation(string text)
        {
            string prompt = string.Format(Prompts.RewriteResponsePrompt3, text);

            InvokeModelResult result = await awsHelper.GenerateResponseAsync(prompt);

            return result.MessageText == "ja";
        }
    }
}