using Application.Common;
using Core;
using Domain.Abstractions;
using Domain.RepositoryInterfaces;
using MediatR;

namespace Application.Group.Create;

internal sealed class CreateGroupRequestHandler(IGroupRepository groupRepository, IUnitOfWork unitOfWork)
    : IRequestHandler<CreateGroupRequest, Result<CreateGroupResponse>>
{
    private const int MaxGroups = 4;

    private readonly IGroupRepository _groupRepository = groupRepository ?? throw new ArgumentNullException(nameof(groupRepository));
    private readonly IUnitOfWork _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

    public async Task<Result<CreateGroupResponse>> Handle(CreateGroupRequest request, CancellationToken cancellationToken)
    {
        var count = await _groupRepository.CountAsync(cancellationToken);
        if (count >= MaxGroups)
            return Result<CreateGroupResponse>.Failure(ApplicationErrors.MaxGroupsReached);

        var group = Domain.Entities.Group.Create(request.GroupName);
        if (group == null)
        {
            return Result<CreateGroupResponse>.Failure(new Error("Failed to create group"));
        }

        await _groupRepository.AddAsync(group, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var response = new CreateGroupResponse
        {
            PublicId = group.PublicId,
            GroupName = group.GroupName
        };

        return Result<CreateGroupResponse>.Success(response);

    }
}
