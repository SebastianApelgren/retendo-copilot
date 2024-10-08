﻿// <auto-generated />
//
// To parse this JSON data, add NuGet 'System.Text.Json' then do:
//
//    using RetendoDataHandler.Models;
//
//    var supportTicket = SupportTicket.FromJson(jsonString);
#nullable enable
#pragma warning disable CS8618
#pragma warning disable CS8601
#pragma warning disable CS8603

namespace RetendoDataHandler.Models
{
    using System;
    using System.Collections.Generic;

    using System.Text.Json;
    using System.Text.Json.Serialization;
    using System.Globalization;

    public partial class SupportTicketRaw
    {
        //JSON dezerilization for the support ticket raw data.

        [JsonPropertyName("subject")]
        public string Subject { get; set; }

        [JsonPropertyName("raw_subject")]
        public string RawSubject { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("submitter")]
        public Assignee Submitter { get; set; }

        [JsonPropertyName("requester")]
        public Assignee Requester { get; set; }

        [JsonPropertyName("assignee")]
        public Assignee Assignee { get; set; }

        [JsonPropertyName("collaborator")]
        public List<object> Collaborator { get; set; }

        [JsonPropertyName("recipient")]
        public string Recipient { get; set; }

        [JsonPropertyName("comments")]
        public List<Comment> Comments { get; set; }
    }

    public partial class Assignee
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("role")]
        public string Role { get; set; }

        [JsonPropertyName("ticket_restriction")]
        public string TicketRestriction { get; set; }
    }

    public partial class Comment
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("author_id")]
        public long AuthorId { get; set; }

        [JsonPropertyName("body")]
        public string Body { get; set; }

        [JsonPropertyName("html_body")]
        public string HtmlBody { get; set; }

        [JsonPropertyName("plain_body")]
        public string PlainBody { get; set; }

        [JsonPropertyName("public")]
        public bool Public { get; set; }

        [JsonPropertyName("attachments")]
        public List<Attachment> Attachments { get; set; }

        [JsonPropertyName("audit_id")]
        public long AuditId { get; set; }

        [JsonPropertyName("ticket_id")]
        public long TicketId { get; set; }

        [JsonPropertyName("via")]
        public CommentVia Via { get; set; }

        [JsonPropertyName("created_at")]
        public DateTimeOffset CreatedAt { get; set; }
    }

    public partial class Attachment
    {
        [JsonPropertyName("url")]
        public Uri Url { get; set; }

        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("file_name")]
        public string FileName { get; set; }

        [JsonPropertyName("content_url")]
        public Uri ContentUrl { get; set; }

        [JsonPropertyName("mapped_content_url")]
        public Uri MappedContentUrl { get; set; }

        [JsonPropertyName("content_type")]
        public string ContentType { get; set; }

        [JsonPropertyName("size")]
        public long Size { get; set; }

        [JsonPropertyName("width")]
        public long? Width { get; set; }

        [JsonPropertyName("height")]
        public long? Height { get; set; }

        [JsonPropertyName("inline")]
        public bool Inline { get; set; }

        [JsonPropertyName("deleted")]
        public bool Deleted { get; set; }

        [JsonPropertyName("malware_access_override")]
        public bool MalwareAccessOverride { get; set; }

        [JsonPropertyName("malware_scan_result")]
        public string MalwareScanResult { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("thumbnails")]
        public List<Attachment> Thumbnails { get; set; }
    }

    public partial class CommentVia
    {
        [JsonPropertyName("channel")]
        public string Channel { get; set; }

        [JsonPropertyName("source")]
        public PurpleSource Source { get; set; }
    }

    public partial class PurpleSource
    {
        [JsonPropertyName("from")]
        public PurpleFrom From { get; set; }

        [JsonPropertyName("to")]
        public ToClass To { get; set; }

        [JsonPropertyName("rel")]
        public object Rel { get; set; }
    }

    public partial class PurpleFrom
    {
        [JsonPropertyName("address")]
        public string Address { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("original_recipients")]
        public List<string> OriginalRecipients { get; set; }
    }

    public partial class ToClass
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("address")]
        public string Address { get; set; }
    }

    public partial class SupportTicketRaw
    {
        public static SupportTicketRaw FromJson(string json) => JsonSerializer.Deserialize<SupportTicketRaw>(json, RetendoDataHandler.Models.Converter.Settings);
    
        public SupportTicket? ToSupportTicket()
        {
            if (this.Requester.Role != "end-user")
            {
                return null;
            }
            List<SupportMessage> messages = new List<SupportMessage>();

            long authorId = this.Requester.Id;

            foreach (Comment comment in this.Comments)
            {
                if (comment.AuthorId == authorId)
                {
                    messages.Add(new SupportMessage("User", comment.Body));
                }
                else
                {
                    messages.Add(new SupportMessage("Assistant", comment.Body));
                }
            }

            return new SupportTicket(this.Subject, messages);
        }
    
    }

    public static class Serialize
    {
        public static string ToJson(this SupportTicketRaw self) => JsonSerializer.Serialize(self, RetendoDataHandler.Models.Converter.Settings);
    }
}
#pragma warning restore CS8618
#pragma warning restore CS8601
#pragma warning restore CS8603
