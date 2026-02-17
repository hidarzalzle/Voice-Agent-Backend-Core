using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using VoiceAgent.Application.Calls.Queries;

namespace VoiceAgent.Infrastructure.Background;

public sealed class CallOrchestrationWorker(IServiceScopeFactory scopeFactory, ILogger<CallOrchestrationWorker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = scopeFactory.CreateScope();
            var mediator = scope.ServiceProvider.GetRequiredService<MediatR.IMediator>();
            _ = await mediator.Send(new GetCallSessionQuery("healthcheck"), stoppingToken);
            logger.LogDebug("Call orchestration heartbeat tick.");
            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
        }
    }
}
