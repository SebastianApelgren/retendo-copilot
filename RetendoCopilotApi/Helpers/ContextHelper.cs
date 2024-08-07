using RetendoCopilotChatbot.Models;
using System.Text.Json;
using System.Text;

namespace RetendoCopilotApi.Helpers

{
    public class ContextHelper
    {
        public static string SerializeAndConvertToBase64(List<ChatMessage> chatMessages)
        {
            string jsonString = JsonSerializer.Serialize(chatMessages);
            string context = Convert.ToBase64String(Encoding.UTF8.GetBytes(jsonString));
            
            return context;
        }

        public static List<ChatMessage> DeserializeAndConvertToString(string context)
        {
            if (string.IsNullOrEmpty(context))
                return new List<ChatMessage>();

            string jsonString = Encoding.UTF8.GetString(Convert.FromBase64String(context));
            List<ChatMessage>? chatMessages = JsonSerializer.Deserialize<List<ChatMessage>>(jsonString);

            if (chatMessages == null)
                return new List<ChatMessage>();

            return chatMessages;
        }
    }
}
