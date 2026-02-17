namespace VoiceAgent.Application.Search;

public interface IVectorStore
{
    Task UpsertAsync(VectorDocument document, CancellationToken cancellationToken);
    Task<IReadOnlyList<VectorMatch>> SimilaritySearchAsync(string callId, float[] embedding, int topK, CancellationToken cancellationToken);
}
