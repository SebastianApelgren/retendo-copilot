﻿using RetendoCopilotChatbot.Models;
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

            string shouldUseContexts = await GetShouldSearchInDocumentationOrShouldAnswerAsync(query);

            List<string> contexts = new List<string>();

            int numberOfResults = 5;

            if (shouldUseContexts == "olämpligt")
            {
                return "Jag kan inte svara på den frågan. Vänligen kontakta Retendo's kundtjänst direkt så kan dem hjälpa dig.";
                //chatMessages.Add(ChatMessage.CreateFromUser(query, null, null));
            }
            else if (shouldUseContexts == "ja")
            {
                chatMessages.RemoveDocumentsAndTickets();
                contexts = await awsHelper.RetrieveAsync(query, numberOfResults);
                chatMessages.Add(ChatMessage.CreateFromUser(query, contexts.GetRange(0, numberOfResults).CreateContextChunk(), contexts.GetRange(numberOfResults, numberOfResults).CreateContextChunk()));
            }
            else
                chatMessages.Add(ChatMessage.CreateFromUser(query, null, null));

            ChatMessage response = await awsHelper.GenerateConversationResponseAsync(chatMessages, prompt);

            string oldResponse = response.Content;

            response.Content = await RewriteResponseAsync(response.Content);

            chatMessages.Add(response);

            Console.WriteLine($"Before rewriting: {oldResponse}");
            Console.WriteLine();

            return response.Content;
        }

        public async Task<string> GetChatResponseTimedAsync(string query, List<ChatMessage> chatMessages)
        {
            string prompt = Prompts.ChatbotSystemPrompt;

            var watch = Stopwatch.StartNew();
            var watch2 = Stopwatch.StartNew();

            watch.Start();
            watch2.Start();

            string shouldUseContexts = await GetShouldSearchInDocumentationOrShouldAnswerAsync(query);

            watch2.Stop();
            Console.WriteLine($"Time to get shouldUseContexts: {watch2.ElapsedMilliseconds} ms");

            watch2.Restart();
            watch2.Start();

            List<string> contexts = new List<string>();

            int numberOfResults = 5;

            if(shouldUseContexts == "olämpligt")
            {
                return "Jag kan inte svara på den frågan. Vänligen kontakta Retendo's kundtjänst direkt så kan dem hjälpa dig.";
                //chatMessages.Add(ChatMessage.CreateFromUser(query, null, null));
            }
            else if (shouldUseContexts == "ja")
            {
                chatMessages.RemoveDocumentsAndTickets();
                contexts = await awsHelper.RetrieveAsync(query, numberOfResults);
                chatMessages.Add(ChatMessage.CreateFromUser(query, contexts.GetRange(0, numberOfResults).CreateContextChunk(), contexts.GetRange(numberOfResults, numberOfResults).CreateContextChunk()));
            }
            else
                chatMessages.Add(ChatMessage.CreateFromUser(query, null, null));

            watch2.Stop();
            Console.WriteLine($"Time to retrieve contexts: {watch2.ElapsedMilliseconds} ms");

            watch2.Restart();
            watch2.Start();

            ChatMessage response = await awsHelper.GenerateConversationResponseAsync(chatMessages, prompt);

            string oldResponse = response.Content;

            watch2.Stop();
            Console.WriteLine($"Time to generate response: {watch2.ElapsedMilliseconds} ms");

            watch2.Restart();
            watch2.Start();

            response.Content = await RewriteResponseAsync(response.Content);

            watch2.Stop();
            Console.WriteLine($"Time to rewrite response: {watch2.ElapsedMilliseconds} ms");

            chatMessages.Add(response);

            watch.Stop();
            Console.WriteLine($"Total time for query: {watch.ElapsedMilliseconds} ms");

            Console.WriteLine();
            Console.WriteLine($"Before rewriting: {oldResponse}");
            Console.WriteLine();

            return response.Content;
        }

        public async Task<string> GetShouldSearchInDocumentationOrShouldAnswerAsync(string query)
        {
            string prompt = string.Format(Prompts.DocumentNeededPrompt, query);

            InvokeModelResult result = await awsHelper.GenerateResponseAsync(prompt);

            return result.MessageText;
        }

        public async Task<string> RewriteResponseAsync(string response)
        {
            string prompt = string.Format(Prompts.RewriteResponsePrompt2, response);

            InvokeModelResult result = await awsHelper.GenerateResponseAsync(prompt);

            return result.MessageText;
        }

    }
}