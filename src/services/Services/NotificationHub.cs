using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

public class NotificationHub : Hub { }

public class CustomUserIdProvider : IUserIdProvider
{
    public string GetUserId(HubConnectionContext connection)
    {
        // Prioriza a claim "id" e cai para NameIdentifier por compatibilidade.
        return connection.User?.FindFirst("id")?.Value
            ?? connection.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? string.Empty;
    }
}
