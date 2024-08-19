using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.NetworkInformation;
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

        [JsonPropertyName("Documents")]
        public string? Documents { get; set; }

        [JsonPropertyName("tickets")]
        public string? Tickets { get; set; }

        [JsonConstructor]
        public ChatMessage(string content, string role, string? documents = null, string? tickets = null)
        {
            Content = content;
            Role = role;
            Documents = documents;
            Tickets = tickets;
        }

        public static ChatMessage CreateFromAssistant(string content) => new ChatMessage(content, "assistant");
        public static ChatMessage CreateFromUser(string content, string? documents, string? tickets)
        {
            return new ChatMessage(content, "user", documents, tickets);
        }

        public static ChatMessage CreateFromSystem(string content) => new ChatMessage(content, "assistant");
    }
}