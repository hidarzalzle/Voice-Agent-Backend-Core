namespace VoiceAgent.Application.Providers;

public interface ITelephonyProvider
{
    string ProviderName { get; }
    Task<bool> ValidateWebhookSignatureAsync(string payload, string signature, CancellationToken cancellationToken);
}
