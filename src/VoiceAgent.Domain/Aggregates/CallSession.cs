using VoiceAgent.Domain.Common;
using VoiceAgent.Domain.Entities;
using VoiceAgent.Domain.Events;
using VoiceAgent.Domain.Exceptions;
using VoiceAgent.Domain.ValueObjects;

namespace VoiceAgent.Domain.Aggregates;

public sealed class CallSession : AggregateRoot<CallId>
{
    private static readonly IReadOnlyDictionary<SessionState, SessionState[]> AllowedTransitions =
        new Dictionary<SessionState, SessionState[]>
        {
            [SessionState.Initiated] = [SessionState.Ringing, SessionState.Failed],
            [SessionState.Ringing] = [SessionState.Connected, SessionState.Failed],
            [SessionState.Connected] = [SessionState.Listening, SessionState.Completed, SessionState.Failed],
            [SessionState.Listening] = [SessionState.Processing, SessionState.Completed, SessionState.Failed],
            [SessionState.Processing] = [SessionState.WaitingForTool, SessionState.Responding, SessionState.Failed],
            [SessionState.WaitingForTool] = [SessionState.Processing, SessionState.Failed],
            [SessionState.Responding] = [SessionState.Listening, SessionState.Completed, SessionState.Failed],
            [SessionState.Completed] = Array.Empty<SessionState>(),
            [SessionState.Failed] = Array.Empty<SessionState>()
        };

    private readonly List<ConversationTurn> _turns = new();
    private readonly List<ToolExecution> _toolExecutions = new();
    private readonly List<StateTransition> _stateTransitions = new();
    private readonly Dictionary<string, string> _metadata = new(StringComparer.OrdinalIgnoreCase);

    public SessionState State { get; private set; }
    public string ProviderCallReference { get; private set; } = string.Empty;
    public string? FailureReason { get; private set; }
    public IReadOnlyCollection<ConversationTurn> Turns => _turns.AsReadOnly();
    public IReadOnlyCollection<ToolExecution> ToolExecutions => _toolExecutions.AsReadOnly();
    public IReadOnlyCollection<StateTransition> StateTransitions => _stateTransitions.AsReadOnly();
    public IReadOnlyDictionary<string, string> Metadata => _metadata;

    private CallSession() { }

    public CallSession(CallId callId, string providerCallReference)
    {
        Id = callId;
        ProviderCallReference = providerCallReference;
        State = SessionState.Initiated;
        _stateTransitions.Add(new StateTransition(SessionState.Initiated, SessionState.Initiated, DateTimeOffset.UtcNow, "Session created."));
    }

    public void TransitionTo(SessionState nextState, string? reason = null)
    {
        var current = State;
        if (!AllowedTransitions.TryGetValue(current, out var allowed) || !allowed.Contains(nextState))
        {
            throw new DomainException($"Illegal state transition from {current} to {nextState}.");
        }

        State = nextState;
        _stateTransitions.Add(new StateTransition(current, nextState, DateTimeOffset.UtcNow, reason));
        if (nextState == SessionState.Failed)
        {
            FailureReason = reason ?? "Unknown failure.";
        }

        RaiseDomainEvent(new CallSessionStateChangedDomainEvent(Id, current, nextState, DateTime.UtcNow));
    }

    public void AppendTranscript(TranscriptChunk chunk)
    {
        _turns.Add(new ConversationTurn(chunk));
        RaiseDomainEvent(new TranscriptReceivedDomainEvent(Id, chunk, DateTime.UtcNow));
    }

    public void RecordToolExecution(string toolName, string argsJson, string resultJson, bool success)
    {
        _toolExecutions.Add(new ToolExecution(toolName, argsJson, resultJson, success));
    }

    public void UpsertMetadata(string key, string value)
    {
        _metadata[key] = value;
    }
}
