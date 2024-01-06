using Microsoft.EntityFrameworkCore;
using FC.Codeflix.Catalog.Domain.Category;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FC.Codeflix.Catalog.Infra.Data.EF.Configurations;

internal class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.HasKey(category => category.Id);
        builder.Property(category => category.Name)
            .HasMaxLength(255)
            .IsRequired();
        builder.Property(category => category.Description)
            .HasMaxLength(4_000);
        builder.Property(category => category.IsActive)
            .HasColumnName("is_active");
        builder.Property(category => category.CreatedAt)
            .HasColumnName("created_at");
        builder.Property(category => category.UpdatedAt)
            .HasColumnName("updated_at");
        builder.Property(category => category.DeletedAt)
            .HasColumnName("deleted_at");
    }
}