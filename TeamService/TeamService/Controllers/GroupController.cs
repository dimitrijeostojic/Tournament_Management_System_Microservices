using Application.Group.Create;
using Application.Group.GetAll;
using Application.Group.GetGroupStandings;
using Core;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TeamService.Extensions;

namespace TeamService.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class GroupController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));

    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetAllGroupsRequest(), cancellationToken);
        return result.ToActionResult();
    }

    [AllowAnonymous]
    [HttpGet("{groupPublicId:guid}/standings")]
    public async Task<IActionResult> GetStandings([FromRoute] Guid groupPublicId, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetGroupStandingsRequest { GroupPublicId = groupPublicId }, cancellationToken);
        return result.ToActionResult();
    }

    [Authorize(Roles = Roles.Admin)]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateGroupRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);
        return result.ToActionResult();
    }
}
