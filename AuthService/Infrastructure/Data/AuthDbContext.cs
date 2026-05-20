using Domain.Abstraction;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data;

namespace Infrastructure.Data;

public class AuthDbContext(DbContextOptions<AuthDbContext> options) : IdentityDbContext<User>(options), IUnitOfWork
{
    public DbSet<RefreshToken> RefreshTokens { get; set; }


    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        var ManagerRoleId = "83d0736a-f899-4b65-8b43-73be201e670e";
        var AdminRoleId = "8ec281a0-c9bf-4c16-9346-062afbc90466";

        var roles = new List<IdentityRole>
            {
                new()
                {
                    Id = ManagerRoleId,
                    ConcurrencyStamp = ManagerRoleId,
                    Name="Manager",
                    NormalizedName="Manager".ToUpper()
                },new()
                {
                    Id = AdminRoleId,
                    ConcurrencyStamp = AdminRoleId,
                    Name="Admin",
                    NormalizedName="Admin".ToUpper()
                }
            };

        builder.Entity<IdentityRole>().HasData(roles);

        builder.Entity<RefreshToken>()
            .HasOne(r => r.User)
            .WithMany()
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await base.SaveChangesAsync(cancellationToken);
    }

    public async Task ExecuteInTransactionAsync(Func<Task> action, CancellationToken cancellationToken = default)
    {
        using var transaction = await base.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            await action();
            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);

        }
    }

    public IDbTransaction BeginTransaction()
    {
        return Database.BeginTransaction().GetDbTransaction();
    }
}
