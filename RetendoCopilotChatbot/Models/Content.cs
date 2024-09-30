using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace RetendoCopilotChatbot.Models
{
    public partial class Content
    {
        //Not relevant.

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("text")]
        public string Text { get; set; }

        [JsonConstructor]
        public Content(string type, string text)
        {
            Type = type;
            Text = text;
        }
    }
}
