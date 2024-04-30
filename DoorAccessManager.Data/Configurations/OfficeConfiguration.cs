using DoorAccessManager.Items.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DoorAccessManager.Data.Configurations
{
    public class OfficeConfiguration : BaseEntityConfiguration<Office>
    {
        public override void Configure(EntityTypeBuilder<Office> builder)
        {
            base.Configure(builder);

            builder.Property(x => x.Name)
                   .IsRequired();

            builder.Property(x => x.IsActive)
                   .IsRequired();

            builder.HasMany(x => x.Doors)
                   .WithOne(x => x.Office)
                   .HasForeignKey(x => x.OfficeId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.Users)
                   .WithOne(x => x.Office)
                   .HasForeignKey(x => x.OfficeId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
