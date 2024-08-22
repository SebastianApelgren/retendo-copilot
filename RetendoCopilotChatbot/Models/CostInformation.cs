using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace RetendoCopilotChatbot.Models
{
    public class CostInformation
    {
        [JsonPropertyName("inputTokens")]
        public double InputTokens { get; set; }

        [JsonPropertyName("outputTokens")]
        public double OutputTokens { get; set; }

        [JsonPropertyName("cost")]
        public double Cost => Math.Round((InputTokens*0.003 + OutputTokens*0.015)/1000,4);
        
        public CostInformation(double inputToken = 0, double outputToken = 0)
        {
            InputTokens = inputToken;
            OutputTokens = outputToken;
        }

        public void AddTokens(double inputToken, double outputToken)
        {
            InputTokens += inputToken;
            OutputTokens += outputToken;
        }

        public void CountAndAddTokens(string input, string output)
        {
            InputTokens += Math.Ceiling(input.Count()/4.5);
            OutputTokens += Math.Ceiling(output.Count()/4.5);
        }

        public override string ToString()
        {
            string print = $"Input Tokens: {InputTokens}\nOutput Tokens: {OutputTokens}\nCost: {Cost}";
            return print;
        }
    }
}
