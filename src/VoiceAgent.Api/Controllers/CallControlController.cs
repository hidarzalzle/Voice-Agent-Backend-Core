using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VoiceAgent.Application.Calls.Commands;
using VoiceAgent.Application.Calls.Queries;

namespace VoiceAgent.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/calls")]
public sealed class CallControlController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<string>> StartCall([FromQuery] string providerReference, CancellationToken cancellationToken)
    {
        var callId = await mediator.Send(new StartCallSessionCommand(providerReference), cancellationToken);
        return Ok(callId);
    }

    [HttpGet("{callId}")]
    public async Task<ActionResult<CallSessionDto>> Get(string callId, CancellationToken cancellationToken)
    {
        var dto = await mediator.Send(new GetCallSessionQuery(callId), cancellationToken);
        return dto is null ? NotFound() : Ok(dto);
    }
}
