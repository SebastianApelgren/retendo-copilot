using RetendoCopilotChatbot.Models;
using System.Text.Json;
using System.Text;

namespace RetendoCopilotApi.Helpers

{
    public class ContextHelper
    {
        public static string SerializeAndConvertToBase64(List<ChatMessage> chatMessages)
        {
            //Converts the chat history to a base64 string.

            string jsonString = JsonSerializer.Serialize(chatMessages);
            string context = Convert.ToBase64String(Encoding.UTF8.GetBytes(jsonString));

            return context;
        }

        public static List<ChatMessage> DeserializeAndConvertToString(string context)
        {
            //Converts the base64 string to a chat history.

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
