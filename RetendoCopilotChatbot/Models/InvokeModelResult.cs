#nullable enable
#pragma warning disable CS8618
#pragma warning disable CS8601
#pragma warning disable CS8603
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Globalization;

namespace RetendoCopilotChatbot.Models
{
    public partial class InvokeModelResult
    {
        //This class is an output from the copilot InvokeModelAsync function.

        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("role")]
        public string Role { get; set; }

        [JsonPropertyName("model")]
        public string Model { get; set; }

        [JsonPropertyName("content")]
        public List<Content> Content { get; set; }

        [JsonPropertyName("stop_reason")]
        public string StopReason { get; set; }

        [JsonPropertyName("stop_sequence")]
        public object StopSequence { get; set; }

        [JsonPropertyName("usage")]
        public Usage Usage { get; set; }

        [JsonIgnore]
        public string MessageText => Content[0].Text;

        public static InvokeModelResult FromJson(string json) => JsonSerializer.Deserialize<InvokeModelResult>(json, Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this InvokeModelResult self) => JsonSerializer.Serialize(self, Converter.Settings);
    }
}
#pragma warning restore CS8618
#pragma warning restore CS8601
#pragma warning restore CS8603
