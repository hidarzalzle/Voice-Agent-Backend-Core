using VoiceAgent.Application.Search;

namespace VoiceAgent.Infrastructure.Search;

public sealed class PineconeVectorStore : IVectorStore
{
    public Task UpsertAsync(VectorDocument document, CancellationToken cancellationToken)
    {
        throw new NotImplementedException("Pinecone integration implemented via provider SDK in production deployment.");
    }

    public Task<IReadOnlyList<VectorMatch>> SimilaritySearchAsync(string callId, float[] embedding, int topK, CancellationToken cancellationToken)
    {
        throw new NotImplementedException("Pinecone integration implemented via provider SDK in production deployment.");
    }
}
