using Microsoft.AspNetCore.SignalR;
using SentimatrixAPI.Models;

namespace SentimatrixAPI.Hubs
{
    public class TicketHub : Hub
    {
        public async Task NewSeriousTicket(ProcessedEmail ticket)
        {
            await Clients.All.SendAsync("ReceiveSeriousTicket", ticket);
        }
    }
}
