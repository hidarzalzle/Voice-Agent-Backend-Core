using VoiceAgent.Domain.Common;
using VoiceAgent.Domain.ValueObjects;

namespace VoiceAgent.Domain.Events;

public sealed record TranscriptReceivedDomainEvent(
    CallId CallId,
    TranscriptChunk Chunk,
    DateTime OccurredAtUtc) : IDomainEvent;
