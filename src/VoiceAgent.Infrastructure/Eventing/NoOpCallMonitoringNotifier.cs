using VoiceAgent.Application.Abstractions;

namespace VoiceAgent.Infrastructure.Eventing;

public sealed class NoOpCallMonitoringNotifier : ICallMonitoringNotifier
{
    public Task NotifyTranscriptAsync(string callId, string speaker, string text, CancellationToken cancellationToken) => Task.CompletedTask;
    public Task NotifyStateChangedAsync(string callId, string state, CancellationToken cancellationToken) => Task.CompletedTask;
    public Task NotifyToolExecutionAsync(string callId, string toolName, bool success, CancellationToken cancellationToken) => Task.CompletedTask;
}
