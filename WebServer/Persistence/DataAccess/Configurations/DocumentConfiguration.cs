using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Persistence.DataAccess.Entities;

namespace Persistence.DataAccess.Configurations;

public class DocumentConfiguration : IEntityTypeConfiguration<DocumentEntity>
{
    public void Configure(EntityTypeBuilder<DocumentEntity> builder)
    {
        builder.HasKey(d => d.Id);

        builder
            .HasOne(d => d.Author)
            .WithMany(a => a.AuthoredDocuments)
            .HasForeignKey(d => d.AuthorId);
        
        builder
            .HasMany(d => d.Users)
            .WithOne(u => u.Document)
            .HasForeignKey(u => u.DocumentId);
    }
}