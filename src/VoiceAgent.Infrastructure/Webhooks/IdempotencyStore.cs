using System.Collections.Concurrent;

namespace VoiceAgent.Infrastructure.Webhooks;

public sealed class IdempotencyStore
{
    private readonly ConcurrentDictionary<string, DateTimeOffset> _processedEvents = new();

    public bool TryMarkProcessed(string eventId)
        => _processedEvents.TryAdd(eventId, DateTimeOffset.UtcNow);
}
