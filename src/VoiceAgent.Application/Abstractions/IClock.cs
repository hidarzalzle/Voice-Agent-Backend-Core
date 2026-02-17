namespace VoiceAgent.Application.Abstractions;

public interface IClock
{
    DateTimeOffset UtcNow { get; }
}
