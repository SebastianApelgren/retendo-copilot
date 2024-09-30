using RetendoCopilotChatbot.Models;
using System.Text.Json.Serialization;

namespace RetendoCopilotApi.Models
{
    public class OutputBody
    {
        //Output object for the API. Contains the response message, cost information, timings, and context.

        [JsonPropertyName("responseMessage")]
        public string ResponseMessage { get; set; }

        [JsonPropertyName("costInformation")]
        public CostInformation CostInformation { get; set; }

        [JsonPropertyName("timings")]
        public List<TimingEntry> Timings { get; set; }

        [JsonPropertyName("context")]
        public string Context { get; set; }

        [JsonConstructor]
        public OutputBody(string responseMessage, List<TimingEntry> timings, string context, CostInformation costInformation)
        {
            ResponseMessage = responseMessage;
            Timings = timings;
            Context = context;
            CostInformation = costInformation;
        }

        public OutputBody(ChatResponse response, string context)
        {
            ResponseMessage = response.Message;
            Timings = response.TimingInformation.Timings;
            CostInformation = response.CostInformation;
            Context = context;
        }
    }
}
