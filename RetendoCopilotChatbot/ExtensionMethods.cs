using Amazon.BedrockAgentRuntime.Model;
using Amazon.BedrockRuntime.Model;
using Microsoft.VisualBasic;
using RetendoCopilotChatbot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace RetendoCopilotChatbot
{
    public static class ExtensionMethods
    {
        public static string GetText(this KnowledgeBaseRetrievalResult result)
        {
            return result.Content.Text;
        }

        public static string CreateContextChunk(this List<string> contexts)
        {
            return string.Join("\r\n-----------\r\n", contexts);
        }

        public static List<Message> ToAwsMessages(this List<ChatMessage> chatMessages)
        {
            List<Message> messages = new List<Message>();
            foreach (ChatMessage chatMessage in chatMessages)
            {
                if (chatMessage.Role == "user")
                {
                    byte[] byteArray = Encoding.UTF8.GetBytes(chatMessage.Documents);
                    //byte[] byteArray = Encoding.ASCII.GetBytes(contents);
                    MemoryStream stream = new MemoryStream(byteArray);

                    DocumentSource documentSource = new DocumentSource
                    {
                        Bytes = stream,
                    };

                    messages.Add(new Message
                    {
                        Content = new List<ContentBlock>
                        {
                            new ContentBlock
                            {
                                
                                Text = chatMessage.Content,
                            },
                            new ContentBlock
                            {
                                Document = new DocumentBlock
                                {
                                    Name = $"Dokumentation till ",
                                    Format = "txt",
                                    Source = documentSource,
                                },
                                
                            }
                        },
                        Role = chatMessage.Role,
                    });
                } else {
                    messages.Add(new Message
                    {
                        Content = new List<ContentBlock>
                        {
                            new ContentBlock
                            {
                                Text = chatMessage.Content,
                            }
                        },
                        Role = chatMessage.Role,
                    });
                }
            }
            return messages;
        }
    }
}
