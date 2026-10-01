using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace AwesomeNetwork.Hubs
{
    public class CustomUserIdProvider : IUserIdProvider
    {
        public virtual string GetUserId(HubConnectionContext connection)
        {
            // Извлекаем Id текущего авторизованного пользователя
            return connection.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                   ?? connection.User?.FindFirst("sub")?.Value;
        }
    }
}
