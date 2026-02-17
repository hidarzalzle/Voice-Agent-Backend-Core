using VoiceAgent.Application.Providers;

namespace VoiceAgent.Infrastructure.Providers.Telephony;

public sealed class VonageTelephonyProvider : ITelephonyProvider
{
    public string ProviderName => "vonage";

    public Task<bool> ValidateWebhookSignatureAsync(string payload, string signature, CancellationToken cancellationToken)
    {
        // Production implementation should validate Vonage-specific signature scheme.
        return Task.FromResult(!string.IsNullOrWhiteSpace(signature));
    }
}
