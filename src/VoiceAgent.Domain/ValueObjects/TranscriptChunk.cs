namespace VoiceAgent.Domain.ValueObjects;

public sealed record TranscriptChunk(
    string Content,
    string Speaker,
    DateTimeOffset Timestamp,
    int Sequence);
