using Bitirme.Models;
using Microsoft.AspNetCore.SignalR;
namespace Bitirme.Hubs
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(Message message) =>
            await Clients.All.SendAsync("receiveMessage", message);
    }
}
