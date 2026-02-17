using Microsoft.EntityFrameworkCore;
using VoiceAgent.Domain.Aggregates;
using VoiceAgent.Domain.Entities;

namespace VoiceAgent.Infrastructure.Persistence;

public sealed class VoiceAgentDbContext(DbContextOptions<VoiceAgentDbContext> options) : DbContext(options)
{
    public DbSet<CallSession> CallSessions => Set<CallSession>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CallSession>(builder =>
        {
            builder.HasKey("Id");
            builder.Property(x => x.State).HasConversion<string>();
            builder.Property(x => x.ProviderCallReference).HasMaxLength(150);
            builder.OwnsMany(typeof(ConversationTurn), "_turns", turns =>
            {
                turns.WithOwner().HasForeignKey("CallSessionId");
                turns.Property<long>("Id");
                turns.HasKey("Id");
                turns.Property("SpeakerRole").HasMaxLength(32);
                turns.Property("Content").HasMaxLength(8000);
            });
            builder.OwnsMany(typeof(ToolExecution), "_toolExecutions", tools =>
            {
                tools.WithOwner().HasForeignKey("CallSessionId");
                tools.Property<long>("Id");
                tools.HasKey("Id");
                tools.Property("ToolName").HasMaxLength(120);
                tools.Property("ArgumentsJson").HasMaxLength(8000);
                tools.Property("ResultJson").HasMaxLength(8000);
            });
            builder.OwnsMany(typeof(StateTransition), "_stateTransitions", transitions =>
            {
                transitions.WithOwner().HasForeignKey("CallSessionId");
                transitions.Property<long>("Id");
                transitions.HasKey("Id");
                transitions.Property("FromState").HasConversion<string>();
                transitions.Property("ToState").HasConversion<string>();
                transitions.Property("Reason").HasMaxLength(500);
            });
        });
    }
}
