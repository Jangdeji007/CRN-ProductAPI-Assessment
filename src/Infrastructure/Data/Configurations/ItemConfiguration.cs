using CRN.ProductAPI.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRN.ProductAPI.Infrastructure.Data.Configurations
{
    public class ItemConfiguration : IEntityTypeConfiguration<Item>
    {
        public void Configure(EntityTypeBuilder<Item> builder)
        {
            builder.ToTable("Item");

            builder.HasKey(i => i.Id);

            builder.Property(i => i.Id)
                   .ValueGeneratedNever();

            builder.Property(i => i.Quantity)
                   .IsRequired();

            builder.Property(i => i.ProductId)
                   .IsRequired();

            builder.HasOne(i => i.Product)
                   .WithMany(p => p.Item)
                   .HasForeignKey(i => i.ProductId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
