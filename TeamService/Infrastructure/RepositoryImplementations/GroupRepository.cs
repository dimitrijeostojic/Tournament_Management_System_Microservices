using Domain.Entities;
using Domain.RepositoryInterfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.RepositoryImplementations;

public sealed class GroupRepository(ApplicationDbContext context)
    : IGroupRepository
{
    private readonly ApplicationDbContext _context = context ?? throw new ArgumentNullException(nameof(context));

    public async Task AddAsync(Group group, CancellationToken cancellationToken = default)
    {
        await _context.Groups.AddAsync(group, cancellationToken);
    }

    public void Delete(Group group)
    {
        _context.Groups.Remove(group);
    }

    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
        => await _context.Groups.CountAsync(cancellationToken);

    public async Task<List<Group>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _context.Groups.Include(x => x.Teams).ToListAsync(cancellationToken);

    public async Task<Group?> GetByPublicIdAsync(Guid publicId, CancellationToken cancellationToken = default)
    {
        return await _context.Groups.FirstOrDefaultAsync(g => g.PublicId == publicId, cancellationToken);
    }
}
