using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace RetendoCopilotChatbot.Models
{
    public class ChatMessage
    {
        [JsonPropertyName("content")]
        public string Content { get; set; }
        [JsonPropertyName("role")]
        public string Role { get; set; }

        [JsonConstructor]
        public ChatMessage(string content, string role)
        {
            Content = content;
            Role = role;
        }

        public static ChatMessage CreateFromAssistant(string content) => new ChatMessage(content, "assistant");
        public static ChatMessage CreateFromUser(string content) => new ChatMessage(content, "user");
        public static ChatMessage CreateFromSystem(string content) => new ChatMessage(content, "assistant");
    }
}
