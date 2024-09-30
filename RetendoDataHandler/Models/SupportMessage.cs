using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace RetendoDataHandler.Models
{
    public class SupportMessage
    {
        //This class is used to create support messages.

        [JsonPropertyName("role")]
        public string Role { get; set; }
        [JsonPropertyName("text")]
        public string Text { get; set; }

        [JsonConstructor]
        public SupportMessage(string role, string text)
        {
            Role = role;
            Text = text;
        }

        public override string ToString()
        {
            return $"{Role}: {Text}";
        }
    }
}
