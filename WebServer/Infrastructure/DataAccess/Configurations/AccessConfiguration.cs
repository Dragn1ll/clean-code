using Infrastructure.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.DataAccess.Configurations;

public class AccessConfiguration : IEntityTypeConfiguration<AccessEntity>
{
    public void Configure(EntityTypeBuilder<AccessEntity> builder)
    {
        builder
            .HasOne(up => up.User)
            .WithMany(u => u.AssignedPermissions)
            .HasForeignKey(up => up.UserId);
        
        builder
            .HasOne(up => up.Permission)
            .WithMany(p => p.UsersDocuments)
            .HasForeignKey(up => up.PermissionId);
        
        builder
            .HasOne(up => up.Document)
            .WithMany(d => d.Users)
            .HasForeignKey(up => up.DocumentId);
    }
}