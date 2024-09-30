using System.Text.Json.Serialization;

namespace RetendoCopilotChatbot.Models
{
    public class ConversationResponse
    {
        //This class is an output from the copilot GetConversationResponseAsync function.

        [JsonPropertyName("chatMessage")]
        public ChatMessage ChatMessage { get; set; }

        [JsonPropertyName("timingInformation")]
        public double outputTokens { get; set; }

        [JsonPropertyName("costInformation")]
        public double inputTokens { get; set; }

        [JsonPropertyName("guardailIntervened")]
        public bool guardrailIntervened { get; set; }

        public ConversationResponse(ChatMessage chatMessage, double outputTokens, double inputTokens, bool guardrailIntervened)
        {
            ChatMessage = chatMessage;
            this.outputTokens = outputTokens;
            this.inputTokens = inputTokens;
            this.guardrailIntervened = guardrailIntervened;
        }

    }
}
