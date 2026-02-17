using VoiceAgent.Application.Abstractions;

namespace VoiceAgent.Infrastructure.Eventing;

public sealed class InMemoryEventBus : IEventBus
{
    public Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
