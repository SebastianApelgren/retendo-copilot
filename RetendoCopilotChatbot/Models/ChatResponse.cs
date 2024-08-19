using System.Text;
using System.Text.Json.Serialization;

namespace RetendoCopilotChatbot.Models
{
    public class ChatResponse
    {
        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("timingInformation")]
        public TimingInformation TimingInformation { get; set; }

        public ChatResponse(string message, TimingInformation timingInformation)
        {
            Message = message;
            TimingInformation = timingInformation;
        }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine(Message);
            stringBuilder.AppendLine();

            stringBuilder.AppendLine("<timing information>");
            foreach(TimingEntry timingEntry in TimingInformation.Timings)
            {
                stringBuilder.AppendLine(timingEntry.ToString());
            }
            stringBuilder.AppendLine("</timing information>");

            return stringBuilder.ToString();
        }
    }
}
