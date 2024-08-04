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

        private async Task<List<string>> RetrieveAsync(string query, int numberOfResults)
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

        public async Task<InvokeModelResult> GenerateResponseAsync(string query, int numberOfResults = 5)
        {
            List<string> contexts = new List<string>();
            contexts = await RetrieveAsync(query, 10);
            string prompt = @$"Mål: Du är en kundtjänstchattbot som är utformad för att korrekt besvara frågor baserat på den tillhandahållna dokumentationen. Ditt mål är att ge hjälpsamma och korrekta svar på kundförfrågningar.

            Instruktioner:
            - Studera noggrant den tillhandahållna dokumentationen för att förstå vilken information som finns tillgänglig.
            - När en kund ställer en fråga, sök i dokumentationen efter relevant information för att formulera ett svar.
            - Om du hittar ett klart och fullständigt svar i dokumentationen, ge det svaret ordagrant utan att modifiera eller lägga till något.
            - Skriv aldrig ut några namn, personnummer eller annan identifierande information om sådan finns i dokumentationen.
            - Om dokumentationen inte innehåller ett fullständigt svar, meddela vänligt kunden att du inte har tillräcklig information för att fullt ut besvara deras fråga.
            - Hitta aldrig på information eller spekulera bortom vad som anges i dokumentationen.
            - Bibehåll en professionell och vänlig ton i dina svar.
            - Om en kunds fråga är oklar eller för bred, be vänligen om förtydligande innan du försöker svara.
            - Gör inga citationer från dokumentationen utan svara direkt på kundens fråga.
            - Skriv inte Enligt dokumentationen eller något liknande i dina svar.
            
            Här kommer dokumentationen listat från mest tillförlitlig till minst tillförlitlig:
            
            <dokumentationen>
            {contexts.CreateContextChunk()}
            </dokumentationen>
            
            Här kommer kundfrågan:
            
            <kundfråga>
            {query}
            </kundfråga>
            
            Din kunskap kommer enbart från den tillhandahållna dokumentationen. Du har ingen ytterligare information utöver vad som finns i de dokumenten. Svara sanningsenligt baserat på dokumentationen och försök aldrig att hitta på eller gissa svar.
            
            Assistant:";

            InvokeModelRequest invokeModelRequest = CreateModelRequest(prompt);
            InvokeModelResponse response = await runtimeClient.InvokeModelAsync(invokeModelRequest);

            using(StreamReader reader = new StreamReader(response.Body))
            {
                string responseBody = reader.ReadToEnd();
                InvokeModelResult result = InvokeModelResult.FromJson(responseBody);
                return result;
            }
        }

        private InvokeModelRequest CreateModelRequest(string prompt, int maxTokens = 512, double temperature = 0.5)
        {
            string nativeRequest = JsonSerializer.Serialize(new
            {
                anthropic_version = "bedrock-2023-05-31",
                max_tokens = 512,
                temperature = 0.5,
                messages = new[]
                {
                    new { role = "user", content = prompt }
                }
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
