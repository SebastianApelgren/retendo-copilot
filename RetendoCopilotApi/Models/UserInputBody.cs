using System.Text.Json.Serialization;

namespace RetendoCopilotApi.Models
{
    public class UserInputBody
    {
        [JsonPropertyName("userMessage")]
        public string UserMessage { get; set; }
        [JsonPropertyName("context")]
        public string Context { get; set; }

        [JsonConstructor]
        public UserInputBody(string userMessage, string context)
        {
            UserMessage = userMessage;
            Context = context;
        }
    }
}
