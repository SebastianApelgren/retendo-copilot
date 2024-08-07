using RetendoCopilotChatbot.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetendoCopilotChatbot
{
    public class Copilot
    {
        private AwsHelper awsHelper;

        public Copilot(AwsHelper awsHelper)
        {
            this.awsHelper = awsHelper;
        }

        public async Task<string> GetChatResponseAsync(string query, List<ChatMessage> chatMessages)
        {
            string prompt = Prompts.ChatbotSystemPrompt;

            bool shouldUseContexts = await GetShouldSearchInDocumentationAsync(query);

            List<string> contexts = new List<string>();

            if (shouldUseContexts) contexts = await awsHelper.RetrieveAsync(query, 5);

            chatMessages.Add(ChatMessage.CreateFromUser(query, shouldUseContexts ? contexts.CreateContextChunk() : null));

            ChatMessage response = await awsHelper.GenerateConversationResponseAsync(chatMessages, prompt);

            chatMessages.Add(response);

            return response.Content;
        }

        public async Task<bool> GetShouldSearchInDocumentationAsync(string query)
        {
            string prompt= string.Format(Prompts.DocumentNeededPrompt, query);

            InvokeModelResult result = await awsHelper.GenerateResponseAsync(prompt);

            return !(result.MessageText == "nej");
        }
    }
}