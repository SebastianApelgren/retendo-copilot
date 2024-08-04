using Amazon.BedrockAgentRuntime.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace RetendoCopilotChatbot
{
    public static class ExtensionMethods
    {
        public static string GetText(this KnowledgeBaseRetrievalResult result)
        {
            return result.Content.Text;
        }

        public static string CreateContextChunk(this List<string> contexts)
        {
            return string.Join("\n\r-----------\n\r", contexts);
        }
    }
}
