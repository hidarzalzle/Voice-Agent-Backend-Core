using VoiceAgent.Domain.Aggregates;
using VoiceAgent.Domain.ValueObjects;

namespace VoiceAgent.Application.Abstractions;

public interface ICallSessionRepository
{
    Task<CallSession?> GetByIdAsync(CallId callId, CancellationToken cancellationToken);
    Task AddAsync(CallSession session, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}
