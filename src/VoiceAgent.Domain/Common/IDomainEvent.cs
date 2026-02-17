namespace VoiceAgent.Domain.Common;

public interface IDomainEvent
{
    DateTime OccurredAtUtc { get; }
}
