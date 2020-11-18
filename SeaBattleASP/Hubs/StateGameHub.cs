namespace SeaBattleASP.Hubs
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.SignalR;

    public class StateGameHub : Hub
    {
        public async Task SendMessage(string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", 
                                        message);
        }
    }
}
