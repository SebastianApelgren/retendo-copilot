using System.Text.Json.Serialization;

namespace RetendoCopilotChatbot.Models
{
    public class TimingEntry
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("duration")]
        public long Duration { get; set; }

        public TimingEntry(string name, long duration)
        {
            Name = name;
            Duration = duration;
        }

        public override string ToString()
        {
            return $"{Name}: {Duration}ms";
        }
    }
}
