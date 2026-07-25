using CRN.ProductAPI.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRN.ProductAPI.Infrastructure.Data.Configurations
{
    public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.ToTable("RefreshToken");

            builder.HasKey(t => t.Id);

            builder.Property(t => t.Id)
                   .ValueGeneratedNever();

            builder.Property(t => t.TokenHash)
                   .IsRequired()
                   .HasMaxLength(128);

            builder.HasIndex(t => t.TokenHash);

            builder.Property(t => t.ExpiresOn)
                   .IsRequired();

            builder.Property(t => t.CreatedOn)
                   .IsRequired();

            builder.Property(t => t.ReplacedByTokenHash)
                   .HasMaxLength(128);

            builder.Ignore(t => t.IsExpired);
            builder.Ignore(t => t.IsRevoked);
            builder.Ignore(t => t.IsActive);
        }
    }
}
