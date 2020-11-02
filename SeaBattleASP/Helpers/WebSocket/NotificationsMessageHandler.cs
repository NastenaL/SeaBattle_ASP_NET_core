﻿namespace SeaBattleASP.Helpers.WebSocket
{
    using System;
    using System.Net.WebSockets;
    using System.Threading.Tasks;

    public class NotificationsMessageHandler : WebSocketHandler
    {
        public NotificationsMessageHandler(ConnectionManager webSocketConnectionManager) : base(webSocketConnectionManager)
        {
        }

        public override Task ReceiveAsync(WebSocket socket, WebSocketReceiveResult result, byte[] buffer)
        {
            throw new NotImplementedException();
        }
    }
}