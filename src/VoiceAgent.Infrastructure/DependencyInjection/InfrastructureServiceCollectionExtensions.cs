using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using VoiceAgent.Application.Abstractions;
using VoiceAgent.Application.Providers;
using VoiceAgent.Application.Search;
using VoiceAgent.Infrastructure.Background;
using VoiceAgent.Infrastructure.Eventing;
using VoiceAgent.Infrastructure.Persistence;
using VoiceAgent.Infrastructure.Providers.Telephony;
using VoiceAgent.Infrastructure.Repositories;
using VoiceAgent.Infrastructure.Search;
using VoiceAgent.Infrastructure.Webhooks;

namespace VoiceAgent.Infrastructure.DependencyInjection;

public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<VoiceAgentDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("Postgres") ?? "Host=localhost;Database=voice-agent;Username=postgres;Password=postgres"));

        services.AddSingleton<IConnectionMultiplexer>(_ =>
            ConnectionMultiplexer.Connect(configuration["Redis:Configuration"] ?? "localhost:6379"));

        services.AddScoped<ICallSessionRepository, CallSessionRepository>();
        services.AddSingleton<IEventBus, InMemoryEventBus>();
        services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();
        services.AddSingleton<IdempotencyStore>();
        services.AddSingleton<RedisIdempotencyStore>();

        services.AddSingleton<ITelephonyProvider>(sp => new TwilioTelephonyProvider(configuration["Telephony:Twilio:WebhookSecret"] ?? "dev-secret"));
        services.AddSingleton<ITelephonyProvider, VonageTelephonyProvider>();
        services.AddSingleton<ITelephonyProvider, SipTelephonyProvider>();

        services.AddSingleton<IVectorStore, PgVectorStore>();
        services.AddHostedService<CallOrchestrationWorker>();

        return services;
    }
}
