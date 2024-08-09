using RetendoDataHandler.Models;

namespace RetendoDataHandler
{
    internal class Program
    {
        static void Main(string[] args)
        {
            List<SupportTicket> list = new List<SupportTicket>();

            foreach (string fileName in Directory.GetFiles("Data"))
            {
                foreach (string row in File.ReadAllLines(fileName))
                {
                    SupportTicketRaw ticket = SupportTicketRaw.FromJson(row);

                    SupportTicket? supportTicket = ticket.ToSupportTicket();

                    if (supportTicket != null)
                        list.Add(supportTicket);
                }
            }
        }
    }
}
