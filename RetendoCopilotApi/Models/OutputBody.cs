using RetendoCopilotChatbot.Models;
using System.Text.Json.Serialization;

namespace RetendoCopilotApi.Models
{
    public class OutputBody
    {
        [JsonPropertyName("responseMessage")]
        public string ResponseMessage { get; set; }

        [JsonPropertyName("timings")]
        public List<TimingEntry> Timings { get; set; }

        [JsonPropertyName("context")]
        public string Context { get; set; }

        [JsonConstructor]
        public OutputBody(string responseMessage, List<TimingEntry> timings, string context)
        {
            ResponseMessage = responseMessage;
            Timings = timings;
            Context = context;
        }

        public OutputBody(ChatResponse response, string context)
        {
            ResponseMessage = response.Message;
            Timings = response.TimingInformation.Timings;
            Context = context;
        }
    }
}
