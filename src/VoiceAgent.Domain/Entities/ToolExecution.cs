namespace VoiceAgent.Domain.Entities;

public sealed class ToolExecution
{
    public long Id { get; private set; }
    public string ToolName { get; private set; } = string.Empty;
    public string ArgumentsJson { get; private set; } = "{}";
    public string ResultJson { get; private set; } = "{}";
    public bool Succeeded { get; private set; }
    public DateTimeOffset ExecutedAtUtc { get; private set; }

    private ToolExecution() { }

    public ToolExecution(string toolName, string argumentsJson, string resultJson, bool succeeded)
    {
        ToolName = toolName;
        ArgumentsJson = argumentsJson;
        ResultJson = resultJson;
        Succeeded = succeeded;
        ExecutedAtUtc = DateTimeOffset.UtcNow;
    }
}
