# Voice Agent Backend Core (Production Foundation)

A production-oriented ASP.NET Core (.NET 8) backend for Voice AI orchestration with Clean Architecture, DDD, CQRS, PostgreSQL persistence, Redis idempotency support, SignalR monitoring, background workers, vector retrieval, and provider abstractions.

## Complete Project Structure

```text
VoiceAgent.Backend.Core.sln
src/
  VoiceAgent.Domain/
    Aggregates/CallSession.cs
    Entities/{ConversationTurn,ToolExecution,StateTransition}.cs
    ValueObjects/{CallId,SessionState,TranscriptChunk}.cs
    Events/{CallSessionStateChangedDomainEvent,TranscriptReceivedDomainEvent}.cs
    Exceptions/DomainException.cs
    Common/{Entity,AggregateRoot,IDomainEvent}.cs
  VoiceAgent.Application/
    Abstractions/{ICallSessionRepository,IEventBus,IDomainEventDispatcher,ICallMonitoringNotifier}.cs
    Calls/Commands/{StartCallSessionCommand,ApplyTelephonyEventCommand}.cs
    Calls/Queries/GetCallSessionQuery.cs
    Tools/{ITool,ToolRegistry,ToolExecutionEngine}.cs
    Memory/{IMemoryManager,ContextMemoryManager}.cs
    Search/{IVectorStore,VectorSearchModels}.cs
    Providers/{ILlmProvider,ITelephonyProvider}.cs
    Common/Behaviors/{LoggingBehavior,ValidationBehavior}.cs
  VoiceAgent.Infrastructure/
    Persistence/VoiceAgentDbContext.cs
    Repositories/CallSessionRepository.cs
    Search/{PgVectorStore,PineconeVectorStore}.cs
    Providers/{FailoverLlmClient,Telephony/*}.cs
    Eventing/{InMemoryEventBus,DomainEventDispatcher,NoOpCallMonitoringNotifier}.cs
    Webhooks/{IdempotencyStore,RedisIdempotencyStore}.cs
    Background/CallOrchestrationWorker.cs
    DependencyInjection/InfrastructureServiceCollectionExtensions.cs
  VoiceAgent.Api/
    Program.cs
    Controllers/{WebhooksController,CallControlController}.cs
    Contracts/TelephonyWebhookRequest.cs
    Hubs/CallMonitoringHub.cs
    Realtime/SignalRCallMonitoringNotifier.cs
```

## Domain: Core Models + State Machine

`CallSession` is the aggregate root and contains:
- state machine (`SessionState`) with explicit allowed transitions
- transcript persistence via `ConversationTurn`
- tool execution persistence via `ToolExecution`
- state transition audit log via `StateTransition`
- domain events for state and transcript changes

Illegal transitions throw `DomainException`.

## Application: CQRS + Orchestration

- `StartCallSessionCommand` and `ApplyTelephonyEventCommand`
- `GetCallSessionQuery`
- MediatR pipeline behaviors (logging + validation)
- `ToolExecutionEngine` supports parsing inputs, validating tools, timeout, retries (Polly), fallback tool, and context writeback.
- `ContextMemoryManager` maintains:
  1. short-term memory window
  2. long-term vector memory
  3. session metadata hooks
  4. tool-result memory

## Infrastructure: Persistence + Event-Driven Building Blocks

- EF Core (Npgsql/PostgreSQL) repository implementation
- Redis idempotency store support
- pluggable vector providers (`IVectorStore`): pgvector-style and Pinecone abstraction
- LLM provider failover client (primary -> secondary)
- telephony provider abstraction with Twilio/Vonage/SIP adapters
- domain event dispatch to event bus
- background orchestration worker for operational tasks

## API: Webhooks, Control, Real-time Monitoring

- Webhook endpoint accepts and handles:
  - `call.started`
  - `call.connected`
  - `transcript.received`
  - `call.completed`
  - `call.failed`
- Signature validation through telephony provider abstraction
- Idempotency gate before processing
- SignalR hub publishes live transcript, state changes, and tool execution logs
- AuthN/AuthZ enabled for control and monitoring endpoints

## Example Call Lifecycle Flow

1. `call.started` webhook -> `Initiated -> Ringing`
2. `call.connected` webhook -> `Ringing -> Connected -> Listening`
3. `transcript.received` -> transcript persisted + embedded + semantic memory updated
4. orchestrator may move `Listening -> Processing -> WaitingForTool -> Processing -> Responding`
5. tool results appended to memory context
6. `Responding -> Listening` loop continues per turn
7. terminal event moves to `Completed` or `Failed`
8. state/transcript/tool events broadcast through SignalR

## Production Notes

- This repository provides a strong orchestration core and clean extension points.
- Replace in-memory placeholders (event bus, pgvector store internals) with concrete managed services in deployment.
- Recommended next step: add integration tests and outbox pattern for guaranteed event delivery.
