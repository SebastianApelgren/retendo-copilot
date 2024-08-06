using RetendoCopilotChatbot.Models;
using System;
using System.Collections.Generic;
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
            string prompt = CreatePromptAsync(query);

            bool shouldUseContexts = await GetShouldSearchInDocumentationAsync(query);
            List<string> contexts = new List<string>();

            if (shouldUseContexts) contexts = await awsHelper.RetrieveAsync(query, 5);

            chatMessages.Add(ChatMessage.CreateFromUser(query, shouldUseContexts ? contexts.CreateContextChunk() : null));
            //InvokeModelResult result = await awsHelper.GenerateResponseAsync(chatMessages);

            ChatMessage response = await awsHelper.GenerateConversationResponseAsync(chatMessages, prompt);

            chatMessages.Add(response);

            //string response = result.MessageText;

            //chatMessages.Add(ChatMessage.CreateFromAssistant(response));
            return response.Content;
        }

        private string CreatePromptAsync(string query)
        {
            string prompt = @$"Du är nu en kundtjänstchattbot. Din uppgift är att hjälpa användare genom att svara på deras frågor baserat på både den tillhandahållna dokumentationen och tidigare meddelanden i konversationen. Svara alltid så exakt som möjligt och sammanfatta informationen från dokumentationen och konversationen med dina egna ord. Om en fråga inte kan besvaras med den tillgängliga dokumentationen eller informationen från konversationen, meddela användaren att du inte har den informationen just nu.

När du svarar på frågor, följ dessa riktlinjer:

Exakthet: Svara endast baserat på den tillhandahållna dokumentationen och tidigare meddelanden i konversationen. Gissa inte eller spekulera.
Tydlighet: Formulera dina svar tydligt och koncist med dina egna ord.
Sammanfattning: Sammanfatta informationen från dokumentationen och konversationen istället för att hänvisa direkt till dem.
Begränsning: Om dokumentationen och konversationen inte täcker frågan, informera användaren att du för närvarande inte har den informationen.

Kom ihåg att alltid hålla dig till den information som finns i dokumentationen och konversationen. Om du behöver mer information, fråga mig om den specifika dokumentationen.";
            return prompt;
        }

        public async Task<bool> GetShouldSearchInDocumentationAsync(string query)
        {
            string prompt = $@"Du hjälper till att avgöra om en chatbot behöver söka i dokumentationen för att svara på en användarfråga.
Oftast ska dokumentationen användas men om användaren bara refererar till ett tidigare meddelande eller säger något som inte är en fråga behöver inte dokumentationen användas.
Om du tror att svaret på användarens fråga finns i dokumentationen, svara 'ja'. Annars, svara 'nej'. Svara bara med ett av de två orden.
<användarens fråga>
{query}
</användarens fråga>";

            InvokeModelResult result = await awsHelper.GenerateResponseAsync(prompt);

            return !(result.MessageText == "nej");
        }
    }
}