using VoiceAgent.Application.Providers;

namespace VoiceAgent.Infrastructure.Providers.Telephony;

public sealed class SipTelephonyProvider : ITelephonyProvider
{
    public string ProviderName => "sip";

    public Task<bool> ValidateWebhookSignatureAsync(string payload, string signature, CancellationToken cancellationToken)
    {
        return Task.FromResult(!string.IsNullOrWhiteSpace(signature));
    }
}
