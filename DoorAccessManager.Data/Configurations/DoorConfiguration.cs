using DoorAccessManager.Items.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DoorAccessManager.Data.Configurations
{
    public class DoorConfiguration : BaseEntityConfiguration<Door>
    {
        public override void Configure(EntityTypeBuilder<Door> builder)
        {
            base.Configure(builder);

            builder.Property(x => x.Name)
                   .IsRequired();

            builder.Property(x => x.IsActive)
                   .IsRequired();

            builder.HasMany(x => x.DoorRoles)
                   .WithOne(x => x.Door)
                   .HasForeignKey(x => x.DoorId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.DoorAccessLogs)
                   .WithOne(x => x.Door)
                   .HasForeignKey(x => x.DoorId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
