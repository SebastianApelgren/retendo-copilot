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

        [JsonPropertyName("costInformation")]
        public CostInformation CostInformation { get; set; }

        public ChatResponse(string message, TimingInformation timingInformation, CostInformation costInformation)
        {
            Message = message;
            TimingInformation = timingInformation;
            CostInformation = costInformation;
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

            stringBuilder.AppendLine();

            stringBuilder.AppendLine("<cost information>");
            stringBuilder.AppendLine(CostInformation.ToString());
            stringBuilder.AppendLine("</cost information>");

            return stringBuilder.ToString();
        }
    }
}
