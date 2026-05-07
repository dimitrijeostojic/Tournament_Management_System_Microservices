using Application.Stadium.CheckAvailability;
using Application.Stadium.GetAll;
using Application.Stadium.GetByPublicId;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StadiumService.Extensions;

namespace StadiumService.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class StadiumController : ControllerBase
{
    private readonly IMediator _mediator;

    public StadiumController(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> GetAllStadiums(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetAllStadiumsRequest(), cancellationToken);
        return result.ToActionResult();
    }

    [AllowAnonymous]
    [HttpGet("{publicId:guid}")]
    public async Task<IActionResult> GetByPublicId(Guid publicId, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetStadiumByPublicIdRequest { PublicId = publicId }, cancellationToken);
        return result.ToActionResult();
    }

    [AllowAnonymous]
    [HttpGet("{publicId:guid}/exists")]
    public async Task<IActionResult> CheckExists(Guid publicId, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new CheckStadiumAvailabilityRequest { PublicId = publicId }, cancellationToken);
        return result.ToActionResult();
    }
}
