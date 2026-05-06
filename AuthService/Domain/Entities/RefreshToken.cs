using System.Security.Cryptography;

namespace Domain.Entities;

public sealed class RefreshToken
{
    public int Id { get; private set; }
    public string? Token { get; private set; }
    public string? UserId { get; private set; }
    public DateTime ExpiresAt { get; private set; }
    public bool IsRevoked { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public User? User { get; private set; }

    public static RefreshToken Create(string userId)
    {
        return new RefreshToken
        {
            UserId = userId,
            Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            IsRevoked = false,
            CreatedAt = DateTime.UtcNow
        };
    }

    public RefreshToken Revoke()
    {
        IsRevoked = true;
        return this;
    }
}