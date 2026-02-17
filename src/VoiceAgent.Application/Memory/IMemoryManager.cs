using VoiceAgent.Domain.ValueObjects;

namespace VoiceAgent.Application.Memory;

public interface IMemoryManager
{
    Task IngestTurnAsync(CallId callId, TranscriptChunk chunk, CancellationToken cancellationToken);
    Task<string> BuildContextWindowAsync(CallId callId, int tokenBudget, CancellationToken cancellationToken);
    Task StoreToolResultAsync(CallId callId, string toolName, string result, CancellationToken cancellationToken);
}
