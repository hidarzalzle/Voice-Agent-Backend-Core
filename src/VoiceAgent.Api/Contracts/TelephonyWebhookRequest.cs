namespace VoiceAgent.Api.Contracts;

public sealed record TelephonyWebhookRequest(
    string EventId,
    string Provider,
    string EventType,
    string CallId,
    string? Transcript,
    string? Speaker,
    DateTimeOffset OccurredAtUtc,
    string Signature,
    string RawPayload);
