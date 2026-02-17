using VoiceAgent.Domain.ValueObjects;

namespace VoiceAgent.Domain.Entities;

public sealed class ConversationTurn
{
    public long Id { get; private set; }
    public string SpeakerRole { get; private set; } = string.Empty;
    public string Content { get; private set; } = string.Empty;
    public DateTimeOffset Timestamp { get; private set; }
    public int Sequence { get; private set; }

    private ConversationTurn() { }

    public ConversationTurn(TranscriptChunk chunk)
    {
        SpeakerRole = chunk.Speaker;
        Content = chunk.Content;
        Timestamp = chunk.Timestamp;
        Sequence = chunk.Sequence;
    }
}
