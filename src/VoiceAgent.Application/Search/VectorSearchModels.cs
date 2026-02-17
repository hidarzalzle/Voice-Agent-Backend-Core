namespace VoiceAgent.Application.Search;

public sealed record VectorDocument(
    string Id,
    string CallId,
    string Content,
    float[] Embedding,
    DateTimeOffset Timestamp,
    IDictionary<string, string>? Metadata = null);

public sealed record VectorMatch(string DocumentId, string Content, float Score, IDictionary<string, string>? Metadata);
