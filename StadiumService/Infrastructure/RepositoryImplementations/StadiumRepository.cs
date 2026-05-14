using Domain.Entities;
using Domain.RepositoryInterfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.RepositoryImplementations;

public sealed class StadiumRepository(MongoDbContext context) : IStadiumRepository
{
    private readonly MongoDbContext _context = context ?? throw new ArgumentNullException(nameof(context));

    public async Task<List<Stadium>> GetAllStadiumsAsync(CancellationToken cancellationToken)
    {
        return await _context.Stadiums.ToListAsync(cancellationToken);
    }

    public async Task<Stadium?> GetByPublicIdAsync(Guid stadiumPublicId, CancellationToken cancellationToken)
    {
        return await _context.Stadiums
            .FirstOrDefaultAsync(s => s.PublicId == stadiumPublicId, cancellationToken);
    }
}
