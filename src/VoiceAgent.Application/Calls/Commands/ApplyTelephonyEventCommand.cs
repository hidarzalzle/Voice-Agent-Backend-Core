using MediatR;
using VoiceAgent.Application.Abstractions;
using VoiceAgent.Application.Memory;
using VoiceAgent.Domain.ValueObjects;

namespace VoiceAgent.Application.Calls.Commands;

public sealed record ApplyTelephonyEventCommand(
    string CallId,
    string EventType,
    string? Transcript,
    string? Speaker,
    DateTimeOffset OccurredAtUtc) : IRequest;

public sealed class ApplyTelephonyEventCommandHandler(
    ICallSessionRepository repository,
    IMemoryManager memoryManager,
    IDomainEventDispatcher domainEventDispatcher,
    ICallMonitoringNotifier monitoringNotifier)
    : IRequestHandler<ApplyTelephonyEventCommand>
{
    public async Task Handle(ApplyTelephonyEventCommand request, CancellationToken cancellationToken)
    {
        var callId = new CallId(request.CallId);
        var session = await repository.GetByIdAsync(callId, cancellationToken)
            ?? throw new InvalidOperationException("Call session not found.");

        switch (request.EventType)
        {
            case "call.started":
                session.TransitionTo(SessionState.Ringing);
                await monitoringNotifier.NotifyStateChangedAsync(session.Id.Value, session.State.ToString(), cancellationToken);
                break;
            case "call.connected":
                session.TransitionTo(SessionState.Connected);
                await monitoringNotifier.NotifyStateChangedAsync(session.Id.Value, session.State.ToString(), cancellationToken);
                session.TransitionTo(SessionState.Listening);
                await monitoringNotifier.NotifyStateChangedAsync(session.Id.Value, session.State.ToString(), cancellationToken);
                break;
            case "transcript.received":
                var sequence = session.Turns.Count + 1;
                var chunk = new TranscriptChunk(request.Transcript ?? string.Empty, request.Speaker ?? "unknown", request.OccurredAtUtc, sequence);
                session.AppendTranscript(chunk);
                await memoryManager.IngestTurnAsync(callId, chunk, cancellationToken);
                await monitoringNotifier.NotifyTranscriptAsync(session.Id.Value, chunk.Speaker, chunk.Content, cancellationToken);
                break;
            case "call.completed":
                session.TransitionTo(SessionState.Completed);
                await monitoringNotifier.NotifyStateChangedAsync(session.Id.Value, session.State.ToString(), cancellationToken);
                break;
            case "call.failed":
                session.TransitionTo(SessionState.Failed, "Provider failure event.");
                await monitoringNotifier.NotifyStateChangedAsync(session.Id.Value, session.State.ToString(), cancellationToken);
                break;
            default:
                throw new NotSupportedException($"Unsupported event type {request.EventType}.");
        }

        await repository.SaveChangesAsync(cancellationToken);
        await domainEventDispatcher.DispatchAsync(session.DomainEvents, cancellationToken);
        session.ClearDomainEvents();
    }
}
