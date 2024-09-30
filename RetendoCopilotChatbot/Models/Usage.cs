using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace RetendoCopilotChatbot.Models
{
    public partial class Usage
    {
        //not relevant
        
        [JsonPropertyName("input_tokens")]
        public long InputTokens { get; set; }

        [JsonPropertyName("output_tokens")]
        public long OutputTokens { get; set; }

        [JsonConstructor]
        public Usage(long inputTokens, long outputTokens)
        {
            InputTokens = inputTokens;
            OutputTokens = outputTokens;
        }
    }
}
