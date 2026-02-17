using System.Collections.Concurrent;
using VoiceAgent.Application.Search;

namespace VoiceAgent.Infrastructure.Search;

public sealed class PgVectorStore : IVectorStore
{
    private readonly ConcurrentDictionary<string, List<VectorDocument>> _data = new();

    public Task UpsertAsync(VectorDocument document, CancellationToken cancellationToken)
    {
        var bucket = _data.GetOrAdd(document.CallId, _ => new List<VectorDocument>());
        bucket.Add(document);
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<VectorMatch>> SimilaritySearchAsync(string callId, float[] embedding, int topK, CancellationToken cancellationToken)
    {
        if (!_data.TryGetValue(callId, out var docs))
        {
            return Task.FromResult<IReadOnlyList<VectorMatch>>(Array.Empty<VectorMatch>());
        }

        var matches = docs
            .Select(d => new VectorMatch(d.Id, d.Content, Cosine(embedding, d.Embedding), d.Metadata))
            .OrderByDescending(m => m.Score)
            .Take(topK)
            .ToArray();

        return Task.FromResult<IReadOnlyList<VectorMatch>>(matches);
    }

    private static float Cosine(float[] a, float[] b)
    {
        var len = Math.Min(a.Length, b.Length);
        if (len == 0) return 0;

        double dot = 0, aMag = 0, bMag = 0;
        for (var i = 0; i < len; i++)
        {
            dot += a[i] * b[i];
            aMag += a[i] * a[i];
            bMag += b[i] * b[i];
        }

        var denominator = Math.Sqrt(aMag) * Math.Sqrt(bMag);
        return denominator == 0 ? 0 : (float)(dot / denominator);
    }
}
