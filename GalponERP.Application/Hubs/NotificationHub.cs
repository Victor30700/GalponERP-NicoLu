using Microsoft.AspNetCore.SignalR;

namespace GalponERP.Application.Hubs;

public class NotificationHub : Hub
{
    public async Task SendNotification(string user, string message)
    {
        await Clients.All.SendAsync("ReceiveNotification", user, message);
    }

    public async Task JoinLoteGroup(string loteId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"Lote_{loteId}");
    }

    public async Task LeaveLoteGroup(string loteId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Lote_{loteId}");
    }
}
