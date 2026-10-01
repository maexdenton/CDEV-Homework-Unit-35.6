using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace AwesomeNetwork.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        // Хаб отвечает за маршрутизацию сообщений через WebSockets
    }
}