using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Amazon;
using Amazon.BedrockRuntime;
using Amazon.BedrockRuntime.Model;
using Amazon.BedrockAgentRuntime;
using Amazon.BedrockAgentRuntime.Model;
using System.Text.Json;
using RetendoCopilotChatbot.Models;
using System.Security.Cryptography;

namespace RetendoCopilotChatbot
{
    public class AwsHelper
    {
        private string knowledgeBaseIdManual;
        private string knowledgeBaseIdTicket;
        private string modelId;
        private AmazonBedrockAgentRuntimeClient client;
        private AmazonBedrockRuntimeClient runtimeClient;

        public AwsHelper(string modId, string region, string kbIdManual, string kbIdTicket, string awsAccessKeyId, string awsSecretAccessKey)
        {
            modelId = modId;
            knowledgeBaseIdManual = kbIdManual;
            knowledgeBaseIdTicket = kbIdTicket;

            client = new AmazonBedrockAgentRuntimeClient(awsAccessKeyId, awsSecretAccessKey, RegionEndpoint.GetBySystemName(region));
            runtimeClient = new AmazonBedrockRuntimeClient(awsAccessKeyId, awsSecretAccessKey, RegionEndpoint.GetBySystemName(region));
        }

        public async Task<List<string>> RetrieveAsync(string query, int numberOfResults = 5)
        {
            RetrieveRequest retrieveRequestManual = new RetrieveRequest
            {
                RetrievalConfiguration = new KnowledgeBaseRetrievalConfiguration
                {
                    VectorSearchConfiguration = new KnowledgeBaseVectorSearchConfiguration
                    {
                        NumberOfResults = numberOfResults,
                    },
                },
                KnowledgeBaseId = knowledgeBaseIdManual,
                RetrievalQuery = new KnowledgeBaseQuery
                {
                    Text = query,
                },
            };

            RetrieveRequest retrieveRequestTicket = new RetrieveRequest
            {
                RetrievalConfiguration = new KnowledgeBaseRetrievalConfiguration
                {
                    VectorSearchConfiguration = new KnowledgeBaseVectorSearchConfiguration
                    {
                        NumberOfResults = numberOfResults,
                    },
                },
                KnowledgeBaseId = knowledgeBaseIdTicket,
                RetrievalQuery = new KnowledgeBaseQuery
                {
                    Text = query,
                },
            };

            Task<RetrieveResponse> responseManualTask = client.RetrieveAsync(retrieveRequestManual);
            Task<RetrieveResponse> responseTicketTask = client.RetrieveAsync(retrieveRequestTicket);

            await Task.WhenAll(responseManualTask, responseTicketTask);

            RetrieveResponse responseManual = responseManualTask.Result;
            RetrieveResponse responseTicket = responseTicketTask.Result;

            List<string> contexts = new List<string>();

            for (int i = 0; i < responseManual.RetrievalResults.Count; i++)
            {
                contexts.Add(responseManual.RetrievalResults[i].GetText());
            }

            for (int i = 0; i < responseTicket.RetrievalResults.Count; i++)
            {
                contexts.Add(responseTicket.RetrievalResults[i].GetText());
            }

            return contexts;
        }

        public async Task<InvokeModelResult> GenerateResponseAsync(string query)
        {
            InvokeModelRequest invokeModelRequest = CreateInvokeModelRequest(query);
            InvokeModelResponse response = await runtimeClient.InvokeModelAsync(invokeModelRequest);

            using (StreamReader reader = new StreamReader(response.Body))
            {
                string responseBody = reader.ReadToEnd();
                InvokeModelResult result = InvokeModelResult.FromJson(responseBody);
                return result;
            }
        }

        public async Task<ChatMessage> GenerateConversationResponseAsync(List<ChatMessage> chatMessages, string prompt)
        {
            ConverseRequest converseRequest = CreateConverseRequest(chatMessages, prompt);
            ConverseResponse response = await runtimeClient.ConverseAsync(converseRequest);

            ChatMessage assistantResponse = ChatMessage.CreateFromAssistant(response.Output.Message.Content[0].Text);

            return assistantResponse;
        }

        private ConverseRequest CreateConverseRequest(List<ChatMessage> chatMessages, string prompt)
        {
            SystemContentBlock systemPrompt = new SystemContentBlock
            {
                Text = prompt,
            };

            Amazon.BedrockRuntime.Model.InferenceConfiguration inferenceConfig = new Amazon.BedrockRuntime.Model.InferenceConfiguration
            {
                MaxTokens = 4096,
                Temperature = .5f,
            };

            ConverseRequest converseRequest = new ConverseRequest
            {
                ModelId = modelId,
                System = new List<SystemContentBlock> { systemPrompt },
                Messages = chatMessages.ToAwsMessages(),
                InferenceConfig = inferenceConfig,
            };

            return converseRequest;
        }

        private InvokeModelRequest CreateInvokeModelRequest(string query, int maxTokens = 4096, double temp = 0.5)
        {
            string nativeRequest = JsonSerializer.Serialize(new
            {
                anthropic_version = "bedrock-2023-05-31",
                max_tokens = maxTokens,
                temperature = temp,
                messages = new[] { new { role = "user", content = query } },
            });

            InvokeModelRequest invokeModelRequest = new InvokeModelRequest
            {
                ModelId = modelId,
                Body = new MemoryStream(Encoding.UTF8.GetBytes(nativeRequest)),
                ContentType = "application/json",
            };

            return invokeModelRequest;
        }
    }
}