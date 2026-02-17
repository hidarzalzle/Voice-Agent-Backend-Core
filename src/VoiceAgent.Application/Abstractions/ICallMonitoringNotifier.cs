namespace VoiceAgent.Application.Abstractions;

public interface ICallMonitoringNotifier
{
    Task NotifyTranscriptAsync(string callId, string speaker, string text, CancellationToken cancellationToken);
    Task NotifyStateChangedAsync(string callId, string state, CancellationToken cancellationToken);
    Task NotifyToolExecutionAsync(string callId, string toolName, bool success, CancellationToken cancellationToken);
}
