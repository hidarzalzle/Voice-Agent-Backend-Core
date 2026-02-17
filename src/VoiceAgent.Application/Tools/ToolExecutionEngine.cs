using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using VoiceAgent.Application.Memory;
using VoiceAgent.Domain.ValueObjects;

namespace VoiceAgent.Application.Tools;

public sealed class ToolExecutionEngine
{
    private readonly ToolRegistry _registry;
    private readonly IMemoryManager _memoryManager;
    private readonly ILogger<ToolExecutionEngine> _logger;
    private readonly AsyncRetryPolicy _retryPolicy;

    public ToolExecutionEngine(ToolRegistry registry, IMemoryManager memoryManager, ILogger<ToolExecutionEngine> logger)
    {
        _registry = registry;
        _memoryManager = memoryManager;
        _logger = logger;
        _retryPolicy = Policy.Handle<Exception>()
            .WaitAndRetryAsync(3, attempt => TimeSpan.FromMilliseconds(200 * attempt));
    }

    public async Task<string> ExecuteAsync(
        CallId callId,
        string toolName,
        string argumentsJson,
        string? fallbackToolName,
        TimeSpan timeout,
        CancellationToken cancellationToken)
    {
        if (!_registry.TryGet(toolName, out var tool))
        {
            if (!string.IsNullOrWhiteSpace(fallbackToolName) && _registry.TryGet(fallbackToolName, out var fallbackTool))
            {
                return await RunToolAsync(callId, fallbackTool, argumentsJson, timeout, cancellationToken);
            }

            throw new InvalidOperationException($"Unknown tool '{toolName}'.");
        }

        try
        {
            return await _retryPolicy.ExecuteAsync(async ct =>
                await RunToolAsync(callId, tool, argumentsJson, timeout, ct), cancellationToken);
        }
        catch (Exception ex) when (!string.IsNullOrWhiteSpace(fallbackToolName) && _registry.TryGet(fallbackToolName, out var fallback))
        {
            _logger.LogWarning(ex, "Primary tool failed, trying fallback {FallbackTool}", fallbackToolName);
            return await RunToolAsync(callId, fallback, argumentsJson, timeout, cancellationToken);
        }
    }

    private async Task<string> RunToolAsync(CallId callId, ITool tool, string argumentsJson, TimeSpan timeout, CancellationToken cancellationToken)
    {
        using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeoutCts.CancelAfter(timeout);

        var result = await tool.ExecuteAsync(argumentsJson, timeoutCts.Token);
        await _memoryManager.StoreToolResultAsync(callId, tool.Name, result, cancellationToken);
        return result;
    }
}
