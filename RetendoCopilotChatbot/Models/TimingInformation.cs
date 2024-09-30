using System.Diagnostics;
using System.Text.Json.Serialization;

namespace RetendoCopilotChatbot.Models
{
    public class TimingInformation
    {
        //class to store timing information for the chat response, used in copilot getChatResponseAsync function.

        [JsonPropertyName("timings")]
        public List<TimingEntry> Timings { get; set; } = new List<TimingEntry>();

        public void RegisterTiming(Stopwatch stopwatch, string taskName)
        {
            stopwatch.Stop();
            Timings.Add(new TimingEntry(taskName, stopwatch.ElapsedMilliseconds));
        }
    }
}
