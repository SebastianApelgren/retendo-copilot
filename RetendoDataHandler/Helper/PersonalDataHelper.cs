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
            //This function reads all support tickets from the given path and returns a list of SupportTicket objects.
            //It also removes some personal data from the messages.

            List<SupportTicket> tickets = new List<SupportTicket>();

            HashSet<string> names = new HashSet<string>() { "jens", "apelgren" };

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

                        //adds the requester (users) name to the names list
                        foreach (string name in requesterName)
                        {
                            string nameLower = name.ToLower();
                            nameLower = Regex.Replace(nameLower, @"[\p{P}-[.]]", ""); //removes punctuation

                            //checks if the name is already in the list
                            if (!names.Contains(nameLower))
                            {
                                //checks if the name is valid (more than 2 letters, not only numbers and not "retendo")
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

            RemoveNames(tickets, names.ToList());

            return tickets;
        }


        public static void RemoveNames(List<SupportTicket> tickets, List<string> names)
        {
            //this function removes the names from the messages in the support tickets. The names are all the names of users sending support tickets.

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
