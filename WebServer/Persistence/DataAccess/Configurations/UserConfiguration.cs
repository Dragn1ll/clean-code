using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Persistence.DataAccess.Entities;

namespace Persistence.DataAccess.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<UserEntity>
{
    public void Configure(EntityTypeBuilder<UserEntity> builder)
    {
        builder.HasKey(u => u.Id);
        
        builder
            .HasOne(u => u.Role)
            .WithMany(r => r.Users)
            .HasForeignKey(u => u.RoleId);

        builder
            .HasMany(u => u.AuthoredDocuments)
            .WithOne(d => d.Author)
            .HasForeignKey(d => d.AuthorId);
        
        builder
            .HasMany(u => u.AssignedPermissions)
            .WithOne(p => p.User)
            .HasForeignKey(p => p.UserId);
    }
}