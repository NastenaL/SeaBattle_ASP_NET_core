namespace SeaBattleASP.Hubs
{
    using Microsoft.AspNetCore.SignalR;
    using System.Threading.Tasks;

    public class ChatHub : Hub
    {
        public async Task SendMessage(string userId, string shipId, string stepType)
        {
            await Clients.All.SendAsync("ReceiveMessage", userId, shipId, stepType);
        }
        public async Task SendShortMessage(string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", message);
        }

    }
}
