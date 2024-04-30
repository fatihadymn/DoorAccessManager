using DoorAccessManager.Items.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DoorAccessManager.Data.Configurations
{
    public class RoleConfiguration : BaseEntityConfiguration<Role>
    {
        public override void Configure(EntityTypeBuilder<Role> builder)
        {
            base.Configure(builder);

            builder.Property(x => x.Name)
                   .IsRequired();

            builder.Property(x => x.IsActive)
                   .IsRequired();

            builder.HasMany(x => x.Users)
                   .WithOne(x => x.Role)
                   .HasForeignKey(x => x.RoleId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x=>x.DoorRoles)
                   .WithOne(x => x.Role)
                   .HasForeignKey(x => x.RoleId)
                   .OnDelete(DeleteBehavior.Cascade);   
        }
    }
}
