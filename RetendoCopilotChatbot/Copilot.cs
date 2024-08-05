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
            string prompt = await CreatePromptAsync(query);
            chatMessages.Add(ChatMessage.CreateFromUser(prompt));
            InvokeModelResult result = await awsHelper.GenerateResponseAsync(chatMessages);
            string response = result.MessageText;
            chatMessages.RemoveAt(chatMessages.Count - 1);
            chatMessages.Add(ChatMessage.CreateFromUser(query));   
            chatMessages.Add(ChatMessage.CreateFromAssistant(response));
            return response;
        }

        private async Task<string> CreatePromptAsync(string query)
        {
            List<string> contexts = new List<string>();
            contexts = await awsHelper.RetrieveAsync(query, 5);

            string prompt = @$"Mål: Du är en kundtjänstchattbot som är utformad för att korrekt besvara frågor baserat på den tillhandahållna dokumentationen och tidigare meddelanden i konversationen. Ditt mål är att ge hjälpsamma och korrekta svar på kundförfrågningar.

Instruktioner:
- Studera noggrant den tillhandahållna dokumentationen för att förstå vilken information som finns tillgänglig.
- När en kund ställer en fråga, sök i dokumentationen och tidigare meddelanden i konversationen efter relevant information för att formulera ett svar.
- Om du hittar ett klart och fullständigt svar i dokumentationen eller tidigare meddelanden, ge det svaret ordagrant utan att modifiera eller lägga till något.
- Skriv aldrig ut några namn, personnummer eller annan identifierande information om sådan finns i dokumentationen eller tidigare meddelanden.
- Om varken dokumentationen eller tidigare meddelanden innehåller ett fullständigt svar, meddela vänligt kunden att du inte har tillräcklig information för att fullt ut besvara deras fråga.
- Hitta aldrig på information eller spekulera bortom vad som anges i dokumentationen och tidigare meddelanden.
- Bibehåll en professionell och vänlig ton i dina svar.
- Om en kunds fråga är oklar eller för bred, be vänligen om förtydligande innan du försöker svara.
- Gör inga citationer från dokumentationen eller tidigare meddelanden utan svara direkt på kundens fråga.
- Skriv inte 'Enligt dokumentationen' eller något liknande i dina svar.

Här kommer dokumentationen listat från mest tillförlitlig till minst tillförlitlig:

<dokumentationen>
{contexts.CreateContextChunk()}
</dokumentationen>

Här kommer kundfrågan:

<kundfråga>
{query}
</kundfråga>

Din kunskap kommer enbart från den tillhandahållna dokumentationen och tidigare meddelanden i konversationen. Du har ingen ytterligare information utöver vad som finns i de dokumenten och tidigare meddelanden. Svara sanningsenligt baserat på dokumentationen och försök aldrig att hitta på eller gissa svar.

Assistant:";
            return prompt;
        }
    }
}
