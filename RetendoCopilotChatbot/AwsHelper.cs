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

namespace RetendoCopilotChatbot
{
    public class AwsHelper
    {
        private string knowledgeBaseId;
        private string modelId;
        private string modelArn;
        private KnowledgeBaseConfiguration knowledgeBaseConfiguration;
        private AmazonBedrockAgentRuntimeClient client;
        private AmazonBedrockRuntimeClient runtimeClient;

        public AwsHelper(string modId, string region, string kbId, string awsAccessKeyId, string awsSecretAccessKey)
        {
            modelId = modId;
            modelArn = $"arn:aws:bedrock:{region}::foundation-model/{modelId}";
            knowledgeBaseId = kbId;
            knowledgeBaseConfiguration = new KnowledgeBaseConfiguration
            {
                KnowledgeBaseId = kbId,
            };

            client = new AmazonBedrockAgentRuntimeClient(awsAccessKeyId, awsSecretAccessKey, RegionEndpoint.GetBySystemName(region));
            runtimeClient = new AmazonBedrockRuntimeClient(awsAccessKeyId, awsSecretAccessKey, RegionEndpoint.GetBySystemName(region));
        }

        public async Task<List<string>> RetrieveAsync(string query, int numberOfResults)
        {
            try
            {
                RetrieveRequest retrieveRequest = new RetrieveRequest
                {
                    RetrievalConfiguration = new KnowledgeBaseRetrievalConfiguration
                    {
                        VectorSearchConfiguration = new KnowledgeBaseVectorSearchConfiguration
                        {
                            NumberOfResults = numberOfResults,
                        },
                    },
                    KnowledgeBaseId = knowledgeBaseId,
                    RetrievalQuery = new KnowledgeBaseQuery
                    {
                        Text = query,
                    },
                };

                RetrieveResponse response = await client.RetrieveAsync(retrieveRequest);

                if (response.HttpStatusCode != System.Net.HttpStatusCode.OK)
                    throw new Exception($"Error: {response.HttpStatusCode}");

                List<string> contexts = new List<string>();

                for (int i = 0; i < response.RetrievalResults.Count; i++)
                {
                    contexts.Add(response.RetrievalResults[i].GetText());
                }

                return contexts;
            }
            catch (AmazonBedrockAgentRuntimeException ex)
            {
                // Handle specific exceptions related to AWS SDK
                throw;
            }
            catch (Exception ex)
            {
                // Handle other potential exceptions
                throw;
            }
        }

        public async Task<InvokeModelResult> GenerateResponseAsync(List<ChatMessage> chatMessages, int numberOfResults = 5)
        {
            InvokeModelRequest invokeModelRequest = CreateInvokeModelRequest(chatMessages);
            InvokeModelResponse response = await runtimeClient.InvokeModelAsync(invokeModelRequest);

            using (StreamReader reader = new StreamReader(response.Body))
            {
                string responseBody = reader.ReadToEnd();
                InvokeModelResult result = InvokeModelResult.FromJson(responseBody);
                return result;
            }
        }

        public async Task<ChatMessage> GenerateConversationResponseAsync(List<ChatMessage> chatMessages, string prompt, int numberOfResults = 5)
        {
            ConverseRequest converseRequest = CreateConverseRequest(chatMessages, prompt);
            ConverseResponse response = await runtimeClient.ConverseAsync(converseRequest);

            ChatMessage assistantResponse = ChatMessage.CreateFromAssistant(response.Output.Message.Content[0].Text);

            return assistantResponse;
        }

        private ConverseRequest CreateConverseRequest(List<ChatMessage> chatMessages, string prompt, int numberOfResults = 5)
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

        private InvokeModelRequest CreateInvokeModelRequest(List<ChatMessage> chatMessages, int maxTokens = 4096, double temp = 0.5)
        {
            string nativeRequest = JsonSerializer.Serialize(new
            {
                anthropic_version = "bedrock-2023-05-31",
                max_tokens = maxTokens,
                temperature = temp,
                messages = chatMessages.ToArray(),
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
