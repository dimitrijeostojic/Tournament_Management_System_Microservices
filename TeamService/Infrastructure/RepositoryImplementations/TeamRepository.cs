using Domain.Entities;
using Domain.RepositoryInterfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.RepositoryImplementations;

public sealed class TeamRepository(ApplicationDbContext context)
    : ITeamRepository
{
    private readonly ApplicationDbContext _context = context ?? throw new ArgumentNullException(nameof(context));

    public async Task AddAsync(Team team, CancellationToken cancellationToken = default)
    {
        await _context.Teams.AddAsync(team, cancellationToken);
    }

    public void Delete(Team team)
    {
        _context.Teams.Remove(team);
    }

    public async Task<List<Team>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Teams.Include(t => t.Group).ToListAsync(cancellationToken);
    }

    public async Task<List<Team>> GetByGroupPublicIdAsync(Guid groupPublicId, CancellationToken cancellationToken = default)
    {
        return await context.Teams
            .Include(t => t.Group)
            .Where(t => t.Group!.PublicId == groupPublicId)
            .ToListAsync(cancellationToken);
    }

    public async Task<Team?> GetByPublicIdAsync(Guid publicId, CancellationToken cancellationToken = default)
    {
        return await context.Teams
            .Include(t => t.Group)
            .FirstOrDefaultAsync(t => t.PublicId == publicId, cancellationToken);
    }
}
