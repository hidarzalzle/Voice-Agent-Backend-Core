using Microsoft.AspNetCore.SignalR;
using VoiceAgent.Api.Hubs;
using VoiceAgent.Application.Abstractions;

namespace VoiceAgent.Api.Realtime;

public sealed class SignalRCallMonitoringNotifier(IHubContext<CallMonitoringHub> hubContext) : ICallMonitoringNotifier
{
    public Task NotifyTranscriptAsync(string callId, string speaker, string text, CancellationToken cancellationToken)
        => hubContext.Clients.Group(callId).SendAsync("transcript.received", new { callId, speaker, text }, cancellationToken);

    public Task NotifyStateChangedAsync(string callId, string state, CancellationToken cancellationToken)
        => hubContext.Clients.Group(callId).SendAsync("call.state.changed", new { callId, state }, cancellationToken);

    public Task NotifyToolExecutionAsync(string callId, string toolName, bool success, CancellationToken cancellationToken)
        => hubContext.Clients.Group(callId).SendAsync("tool.execution", new { callId, toolName, success }, cancellationToken);
}
