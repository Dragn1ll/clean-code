using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Persistence.DataAccess.Entities;

namespace Persistence.DataAccess.Configurations;

public class AccessConfiguration : IEntityTypeConfiguration<AccessEntity>
{
    public void Configure(EntityTypeBuilder<AccessEntity> builder)
    {
        builder.HasKey(a => new { a.UserId, a.DocumentId, a.PermissionId });

        builder.HasOne(a => a.User)
            .WithMany(u => u.Accesses)
            .HasForeignKey(a => a.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(a => a.Document)
            .WithMany(d => d.Accesses)
            .HasForeignKey(a => a.DocumentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(a => a.Permission)
            .WithMany(p => p.UsersDocuments)
            .HasForeignKey(a => a.PermissionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}