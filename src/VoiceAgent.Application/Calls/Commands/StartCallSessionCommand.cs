using MediatR;
using VoiceAgent.Application.Abstractions;
using VoiceAgent.Domain.Aggregates;
using VoiceAgent.Domain.ValueObjects;

namespace VoiceAgent.Application.Calls.Commands;

public sealed record StartCallSessionCommand(string ProviderCallReference) : IRequest<string>;

public sealed class StartCallSessionCommandHandler(ICallSessionRepository repository)
    : IRequestHandler<StartCallSessionCommand, string>
{
    public async Task<string> Handle(StartCallSessionCommand request, CancellationToken cancellationToken)
    {
        var session = new CallSession(CallId.New(), request.ProviderCallReference);
        await repository.AddAsync(session, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);
        return session.Id.Value;
    }
}
