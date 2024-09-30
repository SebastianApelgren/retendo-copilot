using System.Text.Json.Serialization;

namespace RetendoCopilotApi.Models
{
    public class UserInputBody
    {
        //Input object for the API. Contains the user message, context, and number of results for manuals and tickets.

        [JsonPropertyName("userMessage")]
        public string UserMessage { get; set; }

        [JsonPropertyName("numberOfResultsManuals")]
        public int NumberOfResultsManuals { get; set; }

        [JsonPropertyName("numberOfResultsTickets")]
        public int NumberOfResultsTickets { get; set; }

        [JsonPropertyName("context")]
        public string Context { get; set; }

        [JsonConstructor]
        public UserInputBody(string userMessage, string context, int numberOfResultsManuals = 3, int numberOfResultsTickets = 5)
        {
            UserMessage = userMessage;
            Context = context;
            NumberOfResultsManuals = numberOfResultsManuals;
            NumberOfResultsTickets = numberOfResultsTickets;
        }
    }
}
