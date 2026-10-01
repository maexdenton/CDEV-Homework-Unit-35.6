using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;

namespace AwesomeNetwork.Hubs
{
    public class CustomUserIdProvider : IUserIdProvider
    {
        public virtual string GetUserId(HubConnectionContext connection)
        {
            // Берем Id текущего пользователя из Claim-ов авторизации
            return connection.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }
    }
}
