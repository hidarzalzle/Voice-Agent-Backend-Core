using MediatR;
using Microsoft.AspNetCore.Mvc;
using VoiceAgent.Api.Contracts;
using VoiceAgent.Application.Calls.Commands;
using VoiceAgent.Application.Providers;
using VoiceAgent.Infrastructure.Webhooks;

namespace VoiceAgent.Api.Controllers;

[ApiController]
[Route("api/webhooks/telephony")]
public sealed class WebhooksController(
    IMediator mediator,
    IdempotencyStore idempotencyStore,
    IEnumerable<ITelephonyProvider> telephonyProviders) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Receive([FromBody] TelephonyWebhookRequest request, CancellationToken cancellationToken)
    {
        if (!idempotencyStore.TryMarkProcessed(request.EventId))
        {
            return Ok(new { status = "ignored_duplicate" });
        }

        var provider = telephonyProviders.FirstOrDefault(x =>
            x.ProviderName.Equals(request.Provider, StringComparison.OrdinalIgnoreCase));

        if (provider is null)
        {
            return BadRequest(new { error = "Unsupported telephony provider." });
        }

        var signatureValid = await provider.ValidateWebhookSignatureAsync(request.RawPayload, request.Signature, cancellationToken);
        if (!signatureValid)
        {
            return Unauthorized();
        }

        await mediator.Send(
            new ApplyTelephonyEventCommand(
                request.CallId,
                request.EventType,
                request.Transcript,
                request.Speaker,
                request.OccurredAtUtc),
            cancellationToken);

        return Accepted(new { status = "processed" });
    }
}
