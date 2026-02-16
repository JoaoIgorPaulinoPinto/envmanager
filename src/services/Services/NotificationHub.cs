using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

public class NotificationHub : Hub { }

public class CustomUserIdProvider : IUserIdProvider
{
    public string GetUserId(HubConnectionContext connection)
    {
        // Aqui mapeamos a claim "id" que você definiu no seu TokenFactory
        return connection.User?.FindFirst("id")?.Value;
    }
}