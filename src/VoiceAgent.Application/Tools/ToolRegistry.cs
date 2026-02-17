namespace VoiceAgent.Application.Tools;

public sealed class ToolRegistry(IEnumerable<ITool> tools)
{
    private readonly Dictionary<string, ITool> _tools = tools.ToDictionary(t => t.Name, StringComparer.OrdinalIgnoreCase);

    public bool TryGet(string toolName, out ITool tool) => _tools.TryGetValue(toolName, out tool!);

    public IReadOnlyCollection<string> List() => _tools.Keys.ToArray();
}
