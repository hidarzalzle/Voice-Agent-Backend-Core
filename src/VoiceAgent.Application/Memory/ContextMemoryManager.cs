using System.Collections.Concurrent;
using System.Text;
using VoiceAgent.Application.Search;
using VoiceAgent.Domain.ValueObjects;

namespace VoiceAgent.Application.Memory;

public sealed class ContextMemoryManager(IVectorStore vectorStore) : IMemoryManager
{
    private readonly ConcurrentDictionary<string, Queue<string>> _shortTerm = new();
    private readonly ConcurrentDictionary<string, List<string>> _toolResults = new();

    public Task IngestTurnAsync(CallId callId, TranscriptChunk chunk, CancellationToken cancellationToken)
    {
        var queue = _shortTerm.GetOrAdd(callId.Value, _ => new Queue<string>());
        queue.Enqueue($"[{chunk.Timestamp:O}] {chunk.Speaker}: {chunk.Content}");

        while (queue.Count > 25)
        {
            queue.Dequeue();
        }

        return vectorStore.UpsertAsync(
            new VectorDocument(
                Guid.NewGuid().ToString("N"),
                callId.Value,
                chunk.Content,
                FakeEmbed(chunk.Content),
                chunk.Timestamp,
                new Dictionary<string, string> { ["speaker"] = chunk.Speaker }),
            cancellationToken);
    }

    public async Task<string> BuildContextWindowAsync(CallId callId, int tokenBudget, CancellationToken cancellationToken)
    {
        var sb = new StringBuilder();
        if (_shortTerm.TryGetValue(callId.Value, out var queue))
        {
            foreach (var line in queue)
            {
                sb.AppendLine(line);
            }
        }

        var semantic = await vectorStore.SimilaritySearchAsync(callId.Value, FakeEmbed("latest context"), 5, cancellationToken);
        foreach (var match in semantic)
        {
            sb.AppendLine($"[memory:{match.Score:F2}] {match.Content}");
        }

        if (_toolResults.TryGetValue(callId.Value, out var toolMemory))
        {
            foreach (var entry in toolMemory.TakeLast(5))
            {
                sb.AppendLine($"[tool] {entry}");
            }
        }

        var context = sb.ToString();
        return context.Length <= tokenBudget ? context : context[^tokenBudget..];
    }

    public Task StoreToolResultAsync(CallId callId, string toolName, string result, CancellationToken cancellationToken)
    {
        var memory = _toolResults.GetOrAdd(callId.Value, _ => new List<string>());
        memory.Add($"{toolName}: {result}");
        return Task.CompletedTask;
    }

    private static float[] FakeEmbed(string text) => text.Take(64).Select(c => (float)c / 255f).DefaultIfEmpty(0f).ToArray();
}
