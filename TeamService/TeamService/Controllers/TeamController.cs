using Application.Team.Create;
using Application.Team.Delete;
using Application.Team.GetAll;
using Application.Team.GetById;
using Application.Team.Update;
using Core;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TeamService.Extensions;

namespace TeamService.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class TeamController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));

    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetAllTeamsRequest(), cancellationToken);
        return result.ToActionResult();
    }

    [AllowAnonymous]
    [HttpGet("{publicId:guid}")]
    public async Task<IActionResult> GetById(Guid publicId, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new GetTeamByIdRequest { PublicId = publicId }, cancellationToken);
        return result.ToActionResult();
    }

    [AllowAnonymous]
    [HttpGet("{publicId:guid}/exists")]
    public async Task<IActionResult> Exists(Guid publicId, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new GetTeamByIdRequest { PublicId = publicId }, cancellationToken);
        return result.ToActionResult();
    }

    [Authorize(Roles = $"{Roles.Admin},{Roles.Manager}")]
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateTeamRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);
        return result.ToActionResult();
    }

    [Authorize(Roles = $"{Roles.Admin},{Roles.Manager}")]
    [HttpPut("{publicId:guid}")]
    public async Task<IActionResult> Update(
        Guid publicId,
        [FromBody] UpdateTeamRequest request,
        CancellationToken cancellationToken)
    {
        request.TeamPublicId = publicId;
        var result = await _mediator.Send(request, cancellationToken);
        return result.ToActionResult();
    }

    [Authorize(Roles = Roles.Admin)]
    [HttpDelete("{publicId:guid}")]
    public async Task<IActionResult> Delete(Guid publicId, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new DeleteTeamRequest { PublicId = publicId }, cancellationToken);
        return result.ToActionResult();
    }
}
