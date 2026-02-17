using Polly;
using VoiceAgent.Application.Providers;

namespace VoiceAgent.Infrastructure.Providers;

public sealed class FailoverLlmClient(ILlmProvider primary, ILlmProvider secondary)
{
    private readonly ResiliencePipeline _pipeline = new ResiliencePipelineBuilder()
        .AddRetry(new Polly.Retry.RetryStrategyOptions
        {
            MaxRetryAttempts = 3,
            Delay = TimeSpan.FromMilliseconds(250)
        })
        .Build();

    public async Task<string> CompleteAsync(string prompt, CancellationToken cancellationToken)
    {
        try
        {
            return await _pipeline.ExecuteAsync(async ct => await primary.CompleteAsync(prompt, ct), cancellationToken);
        }
        catch
        {
            return await secondary.CompleteAsync(prompt, cancellationToken);
        }
    }
}
