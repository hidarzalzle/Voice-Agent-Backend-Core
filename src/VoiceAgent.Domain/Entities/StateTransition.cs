using VoiceAgent.Domain.ValueObjects;

namespace VoiceAgent.Domain.Entities;

public sealed class StateTransition
{
    public long Id { get; private set; }
    public SessionState FromState { get; private set; }
    public SessionState ToState { get; private set; }
    public DateTimeOffset TransitionedAtUtc { get; private set; }
    public string? Reason { get; private set; }

    private StateTransition() { }

    public StateTransition(SessionState fromState, SessionState toState, DateTimeOffset transitionedAtUtc, string? reason)
    {
        FromState = fromState;
        ToState = toState;
        TransitionedAtUtc = transitionedAtUtc;
        Reason = reason;
    }
}
