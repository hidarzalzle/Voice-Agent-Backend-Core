using VoiceAgent.Application.Abstractions;
using VoiceAgent.Domain.Common;

namespace VoiceAgent.Infrastructure.Eventing;

public sealed class DomainEventDispatcher(IEventBus eventBus) : IDomainEventDispatcher
{
    public async Task DispatchAsync(IEnumerable<IDomainEvent> events, CancellationToken cancellationToken)
    {
        foreach (var domainEvent in events)
        {
            await eventBus.PublishAsync(domainEvent, cancellationToken);
        }
    }
}
