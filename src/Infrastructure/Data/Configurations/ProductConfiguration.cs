using CRN.ProductAPI.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRN.ProductAPI.Infrastructure.Data.Configurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("Product");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id)
                   .ValueGeneratedNever();

            builder.Property(p => p.ProductName)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.HasIndex(p => p.ProductName)
                   .IsUnique();

            builder.Property(p => p.CreatedBy)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(p => p.CreatedOn)
                   .IsRequired();

            builder.Property(p => p.ModifiedBy)
                   .HasMaxLength(100);

            builder.Property(p => p.ModifiedOn);

            builder.HasMany(p => p.Item)
                   .WithOne(i => i.Product)
                   .HasForeignKey(i => i.ProductId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
