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
            //used to get the context from the knowledge base retrieval result

            return result.Content.Text;
        }

        public static string CreateContextChunk(this List<string> contexts)
        {
            //used to create a context chunk from the contexts

            return string.Join("\r\n-----------\r\n", contexts);
        }

        public static List<Message> ToAwsMessages(this List<ChatMessage> chatMessages)
        {
            //used to convert chat messages to messages that can be sent to AWS.

            List<Message> messages = new List<Message>();
            foreach (ChatMessage chatMessage in chatMessages)
            {
                if (chatMessage.Role == "user")
                {
                    List<ContentBlock> content = new List<ContentBlock>()
                    {
                        new ContentBlock
                        {
                            Text = chatMessage.Content,
                        }
                    };

                    //adds the documents and tickets to the message
                    if (chatMessage.Documents != null)
                    {
                        byte[] byteArray = Encoding.UTF8.GetBytes(chatMessage.Documents);

                        using (MemoryStream stream = new MemoryStream(byteArray))
                        {
                            DocumentSource documentSource = new DocumentSource
                            {
                                Bytes = stream,
                            };

                            content.Add(new ContentBlock
                            {
                                Document = new DocumentBlock
                                {
                                    Name = $"Relevant dokumentation (id {Guid.NewGuid()})",
                                    Format = "txt",
                                    Source = documentSource,
                                },
                            });
                        }
                    }
                    if (chatMessage.Tickets != null)
                    {
                        byte[] byteArray = Encoding.UTF8.GetBytes(chatMessage.Tickets);

                        using (MemoryStream stream = new MemoryStream(byteArray))
                        {
                            DocumentSource documentSource = new DocumentSource
                            {
                                Bytes = stream,
                            };

                            content.Add(new ContentBlock
                            {
                                Document = new DocumentBlock
                                {
                                    Name = $"Relevanta kundarenden (id {Guid.NewGuid()})",
                                    Format = "txt",
                                    Source = documentSource,
                                },
                            });
                        }
                    }

                    messages.Add(new Message
                    {
                        Content = content,
                        Role = chatMessage.Role,
                    });
                }
                else
                {
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

        public static List<ChatMessage> RemoveDocumentsAndTickets(this List<ChatMessage> chatMessages)
        {
            //used to remove documents and tickets from chat messages

            foreach (ChatMessage chatMessage in chatMessages)
            {
                chatMessage.Documents = null;
                chatMessage.Tickets = null;
            }
            return chatMessages;
        }
    }
}