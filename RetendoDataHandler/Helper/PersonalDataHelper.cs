using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using RetendoDataHandler.Models;

namespace RetendoDataHandler.Helper
{
    public class PersonalDataHelper
    {
        public static List<SupportTicket> GetSupportTickets(string path)
        {
            List<SupportTicket> tickets = new List<SupportTicket>();

            List<string> names = new List<string>() { "jens", "apelgren" };

            foreach (string fileName in Directory.GetFiles(path))
            {
                foreach (string row in File.ReadAllLines(fileName))
                {
                    SupportTicketRaw ticket = SupportTicketRaw.FromJson(row);

                    SupportTicket? supportTicket = ticket.ToSupportTicket();

                    if (supportTicket != null)
                    {
                        tickets.Add(supportTicket);

                        List<string> requesterName = ticket.Requester.Name.Split(" ").ToList();

                        foreach (string name in requesterName)
                        {
                            string nameLower = name.ToLower();
                            nameLower = Regex.Replace(nameLower, @"[\p{P}-[.]]", "");

                            if (!names.Contains(nameLower))
                            {
                                if (nameLower.Length < 3 || Regex.IsMatch(nameLower, @"^\d+$") || nameLower == "retendo")
                                {
                                    continue;
                                }
                                else
                                {
                                    names.Add(nameLower);
                                }
                            }
                        }
                    }
                }
            }
            RemoveNames(tickets, names);

            return tickets;
        }


        public static void RemoveNames(List<SupportTicket> tickets, List<string> names)
        {
            foreach (SupportTicket ticket in tickets)
            {
                foreach (SupportMessage message in ticket.Messages)
                {
                    foreach (string name in names)
                    {
                        message.Text = Regex.Replace(message.Text, $@"\b{name}\b", "REDACTED", RegexOptions.IgnoreCase);
                    }
                }
            }
        }
    }
}
