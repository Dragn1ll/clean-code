using Core.Enum;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Persistence.DataAccess.Entities;

namespace Persistence.DataAccess.Configurations;

public class PermissionConfiguration : IEntityTypeConfiguration<PermissionEntity>
{
    public void Configure(EntityTypeBuilder<PermissionEntity> builder)
    {
        builder.HasKey(p => p.Id);
        
        builder.HasMany(p => p.UsersDocuments)
            .WithOne(a => a.Permission)
            .HasForeignKey(a => a.PermissionId)
            .OnDelete(DeleteBehavior.Cascade);
        
        var permissions = Enum
            .GetValues<Permissions>()
            .Select(p => new PermissionEntity
            {
                Id = (int)p,
                Name = p.ToString()
            });

        builder.HasData(permissions);
    }
}