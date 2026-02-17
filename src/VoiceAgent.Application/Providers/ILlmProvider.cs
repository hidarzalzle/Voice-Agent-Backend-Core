namespace VoiceAgent.Application.Providers;

public interface ILlmProvider
{
    string Name { get; }
    Task<string> CompleteAsync(string prompt, CancellationToken cancellationToken);
}
