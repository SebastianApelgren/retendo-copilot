using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace RetendoDataHandler.Models
{
    public class UploadTicketResult
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonConstructor]
        public UploadTicketResult(bool success, string message)
        {
            Success = success;
            Message = message;
        }
    }
}
