using System.Text;
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
        // This class is used to interact with the AWS Bedrock API. 

        private string knowledgeBaseIdManual;
        private string knowledgeBaseIdTicket;
        private string guardrailIdentifier;
        private string guardrailVersion;
        private string modelId;
        private AmazonBedrockAgentRuntimeClient client;
        private AmazonBedrockRuntimeClient runtimeClient;

        public AwsHelper(string modId, string region, string kbIdManual, string kbIdTicket, string awsAccessKeyId, string awsSecretAccessKey, string guardrailIdentifier, string guardrailVersion)
        {
            modelId = modId;
            knowledgeBaseIdManual = kbIdManual;
            knowledgeBaseIdTicket = kbIdTicket;
            this.guardrailIdentifier = guardrailIdentifier;
            this.guardrailVersion = guardrailVersion;

            client = new AmazonBedrockAgentRuntimeClient(awsAccessKeyId, awsSecretAccessKey, RegionEndpoint.GetBySystemName(region));
            runtimeClient = new AmazonBedrockRuntimeClient(awsAccessKeyId, awsSecretAccessKey, RegionEndpoint.GetBySystemName(region));
        }

        public async Task<List<string>> RetrieveAsync(string query, int numberOfResultsManuals, int numberOfResultsTickets)
        {
            //this function retrieves the relevant contexts from AWS based on the query, that is the tickets and manuals (documents).

            RetrieveRequest retrieveRequestManual = new RetrieveRequest
            {
                RetrievalConfiguration = new KnowledgeBaseRetrievalConfiguration
                {
                    VectorSearchConfiguration = new KnowledgeBaseVectorSearchConfiguration
                    {
                        NumberOfResults = numberOfResultsManuals,
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
                        NumberOfResults = numberOfResultsTickets,
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
            //this function sends a single prompt to the LLM (large language model), it cannot send a complete conversation. It is used to generate a response to a single query.

            InvokeModelRequest invokeModelRequest = CreateInvokeModelRequest(query);
            InvokeModelResponse response = await runtimeClient.InvokeModelAsync(invokeModelRequest);

            using (StreamReader reader = new StreamReader(response.Body))
            {
                string responseBody = reader.ReadToEnd();
                InvokeModelResult result = InvokeModelResult.FromJson(responseBody);
                return result;
            }
        }

        public async Task<ConversationResponse> GenerateConversationResponseAsync(List<ChatMessage> chatMessages, string prompt)
        {
            //this function sends a complete conversation to the LLM (large language model) and gets a response from it.

            ConverseRequest converseRequest = CreateConverseRequest(chatMessages, prompt);
            ConverseResponse response = await runtimeClient.ConverseAsync(converseRequest);

            ChatMessage assistantResponse = ChatMessage.CreateFromAssistant(response.Output.Message.Content[0].Text);

            bool guardail_intervened = false;

            if (response.StopReason.Value == "guardrail_intervened")
                guardail_intervened = true;

            ConversationResponse conversationResponse= new ConversationResponse(assistantResponse, response.Usage.OutputTokens, response.Usage.InputTokens, guardail_intervened);

            return conversationResponse;
        }

        private ConverseRequest CreateConverseRequest(List<ChatMessage> chatMessages, string prompt)
        {
            //this function creates a ConverseRequest object from a list of chat messages and a prompt to be sent to the conversation API (.converseAsync).

            SystemContentBlock systemPrompt = new SystemContentBlock
            {
                Text = prompt,
            };

            Amazon.BedrockRuntime.Model.InferenceConfiguration inferenceConfig = new Amazon.BedrockRuntime.Model.InferenceConfiguration
            {
                MaxTokens = 4096,
                Temperature = .5f,
            };

            Amazon.BedrockRuntime.Model.GuardrailConfiguration guardrailConfig = new Amazon.BedrockRuntime.Model.GuardrailConfiguration
            {
                GuardrailIdentifier = guardrailIdentifier,
                GuardrailVersion = guardrailVersion
            };

            ConverseRequest converseRequest = new ConverseRequest
            {
                ModelId = modelId,
                System = new List<SystemContentBlock> { systemPrompt },
                Messages = chatMessages.ToAwsMessages(),
                InferenceConfig = inferenceConfig,
                GuardrailConfig = guardrailConfig,
            };

            return converseRequest;
        }

        private InvokeModelRequest CreateInvokeModelRequest(string query, int maxTokens = 4096, double temp = 0.5)
        {
            //this function creates an InvokeModelRequest object from a query to be sent to the invoke API (.invokeModelAsync).

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