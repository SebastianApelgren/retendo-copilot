using RetendoDataHandler.Models;
using System.Text.Json.Serialization;
using ThirdParty.Json.LitJson;

namespace RetendoCopilotApi.Models
{
    public class UserInputBodyTickets
    {

        [JsonPropertyName("supportTickets")]
        public List<SupportTicketRaw> SupportTickets { get; set; }

        [JsonConstructor]
        public UserInputBodyTickets(List<SupportTicketRaw> supportTickets)
        {
            SupportTickets = supportTickets;
        }
    }
}
