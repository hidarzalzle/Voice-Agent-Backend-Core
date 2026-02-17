using VoiceAgent.Domain.Common;
using VoiceAgent.Domain.ValueObjects;

namespace VoiceAgent.Domain.Events;

public sealed record CallSessionStateChangedDomainEvent(
    CallId CallId,
    SessionState Previous,
    SessionState Current,
    DateTime OccurredAtUtc) : IDomainEvent;
