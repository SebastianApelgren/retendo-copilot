using System.Text.Json.Serialization;
using ThirdParty.Json.LitJson;

namespace RetendoCopilotApi.Models
{
    public class OutputBody
    {
        [JsonPropertyName("response")]
        public string Response { get; set; }
        [JsonPropertyName("context")]
        public string Context { get; set; }

        [JsonConstructor]
        public OutputBody(string response, string context)
        {
            Response = response;
            Context = context;
        }
    }
}
