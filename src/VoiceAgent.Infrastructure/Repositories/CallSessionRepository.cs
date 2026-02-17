using Microsoft.EntityFrameworkCore;
using VoiceAgent.Application.Abstractions;
using VoiceAgent.Domain.Aggregates;
using VoiceAgent.Domain.ValueObjects;
using VoiceAgent.Infrastructure.Persistence;

namespace VoiceAgent.Infrastructure.Repositories;

public sealed class CallSessionRepository(VoiceAgentDbContext dbContext) : ICallSessionRepository
{
    public async Task<CallSession?> GetByIdAsync(CallId callId, CancellationToken cancellationToken)
    {
        return await dbContext.CallSessions
            .Include("_turns")
            .Include("_toolExecutions")
            .FirstOrDefaultAsync(x => x.Id == callId, cancellationToken);
    }

    public Task AddAsync(CallSession session, CancellationToken cancellationToken)
    {
        dbContext.CallSessions.Add(session);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken)
        => dbContext.SaveChangesAsync(cancellationToken);
}
