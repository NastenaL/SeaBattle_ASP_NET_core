namespace SeaBattleASP.Hubs
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.SignalR;

    public class ChatHub : Hub
    {
        public async Task SendMessage(string playerId, string shipId, string stepType)
        {
            await Clients.All.SendAsync("ReceiveMessage", playerId, shipId, stepType);
        }
    }
}
