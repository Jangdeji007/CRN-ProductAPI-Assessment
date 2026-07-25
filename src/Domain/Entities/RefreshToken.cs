namespace CRN.ProductAPI.Domain.Entities
{
    public class RefreshToken
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public string TokenHash { get; set; } = string.Empty;

        public DateTime ExpiresOn { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime? RevokedOn { get; set; }

        public string? ReplacedByTokenHash { get; set; }

        public User User { get; set; } = null!;

        public bool IsExpired => DateTime.UtcNow >= ExpiresOn;

        public bool IsRevoked => RevokedOn.HasValue;

        public bool IsActive => !IsRevoked && !IsExpired;
    }
}
