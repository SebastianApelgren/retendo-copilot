using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace RetendoDataHandler.Models
{
    public class SupportTicket
    {
        //An object for each support ticket. It contains a subject and a list of messages.

        [JsonPropertyName("Subject")]
        public string Subject { get; set; }

        [JsonPropertyName("Messages")]
        public List<SupportMessage> Messages { get; set; }

        [JsonConstructor]
        public SupportTicket(string subject, List<SupportMessage> messages)
        {
            Subject = subject;
            Messages = messages;
        }
    }
}
