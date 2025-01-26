using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Persistence.DataAccess.Entities;

namespace Persistence.DataAccess.Configurations;

public class DocumentConfiguration : IEntityTypeConfiguration<DocumentEntity>
{
    public void Configure(EntityTypeBuilder<DocumentEntity> builder)
    {
        builder
            .HasOne(d => d.Author)
            .WithMany(u => u.Documents)
            .HasForeignKey(d => d.AuthorId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}