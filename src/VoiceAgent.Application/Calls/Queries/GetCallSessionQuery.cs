using MediatR;
using VoiceAgent.Application.Abstractions;

namespace VoiceAgent.Application.Calls.Queries;

public sealed record GetCallSessionQuery(string CallId) : IRequest<CallSessionDto?>;
public sealed record CallSessionDto(string CallId, string State, int TranscriptCount, int ToolExecutions);

public sealed class GetCallSessionQueryHandler(ICallSessionRepository repository)
    : IRequestHandler<GetCallSessionQuery, CallSessionDto?>
{
    public async Task<CallSessionDto?> Handle(GetCallSessionQuery request, CancellationToken cancellationToken)
    {
        var session = await repository.GetByIdAsync(new Domain.ValueObjects.CallId(request.CallId), cancellationToken);
        return session is null
            ? null
            : new CallSessionDto(session.Id.Value, session.State.ToString(), session.Turns.Count, session.ToolExecutions.Count);
    }
}
