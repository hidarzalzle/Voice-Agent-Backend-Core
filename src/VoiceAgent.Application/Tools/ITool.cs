namespace VoiceAgent.Application.Tools;

public interface ITool
{
    string Name { get; }
    Task<string> ExecuteAsync(string argumentsJson, CancellationToken cancellationToken);
}
