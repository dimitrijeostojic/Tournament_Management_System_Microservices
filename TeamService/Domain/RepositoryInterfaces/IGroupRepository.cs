using Domain.Entities;

namespace Domain.RepositoryInterfaces;

public interface IGroupRepository
{
    Task<List<Group>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Group?> GetByPublicIdAsync(Guid publicId, CancellationToken cancellationToken = default);
    Task<int> CountAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Group group, CancellationToken cancellationToken = default);
    void Delete(Group group);
}
