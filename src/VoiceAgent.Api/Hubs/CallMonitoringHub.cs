using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace VoiceAgent.Api.Hubs;

[Authorize]
public sealed class CallMonitoringHub : Hub
{
    public Task Subscribe(string callId)
        => Groups.AddToGroupAsync(Context.ConnectionId, callId);

    public Task Unsubscribe(string callId)
        => Groups.RemoveFromGroupAsync(Context.ConnectionId, callId);
}
