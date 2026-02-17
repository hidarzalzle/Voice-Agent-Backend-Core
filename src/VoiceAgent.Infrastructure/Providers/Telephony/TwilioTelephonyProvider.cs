using System.Security.Cryptography;
using System.Text;
using VoiceAgent.Application.Providers;

namespace VoiceAgent.Infrastructure.Providers.Telephony;

public sealed class TwilioTelephonyProvider : ITelephonyProvider
{
    private readonly string _webhookSecret;
    public string ProviderName => "twilio";

    public TwilioTelephonyProvider(string webhookSecret)
    {
        _webhookSecret = webhookSecret;
    }

    public Task<bool> ValidateWebhookSignatureAsync(string payload, string signature, CancellationToken cancellationToken)
    {
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_webhookSecret));
        var computed = Convert.ToHexString(hmac.ComputeHash(Encoding.UTF8.GetBytes(payload)));
        return Task.FromResult(string.Equals(computed, signature, StringComparison.OrdinalIgnoreCase));
    }
}
