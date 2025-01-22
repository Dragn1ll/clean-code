using Infrastructure.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.DataAccess.Configurations;

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