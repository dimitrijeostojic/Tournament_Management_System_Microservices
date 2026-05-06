using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configuration;

internal sealed class TeamStandingEntityConfiguration
    : IEntityTypeConfiguration<TeamStanding>
{
    public void Configure(EntityTypeBuilder<TeamStanding> builder)
    {
        builder.ToTable("TeamStandings");
        builder.HasKey(c => c.Id);
        builder.Property(c => c.TeamPublicId)
            .IsRequired();
        builder.HasIndex(c => c.TeamPublicId)
            .IsUnique();
        builder.Property(c => c.Played)
            .IsRequired()
            .HasDefaultValue(0);
        builder.Property(c => c.Wins)
            .IsRequired()
            .HasDefaultValue(0);
        builder.Property(c => c.Losses)
            .IsRequired()
            .HasDefaultValue(0);
        builder.Property(c => c.PointsFor)
            .IsRequired()
            .HasDefaultValue(0);
        builder.Property(c => c.PointsAgainst)
            .IsRequired()
            .HasDefaultValue(0);
        builder.Property(c => c.StandingPoints)
            .IsRequired()
            .HasDefaultValue(0);
    }
}
