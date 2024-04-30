using DoorAccessManager.Items.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DoorAccessManager.Data.Configurations
{
    public class DoorAccessLogConfiguration : BaseEntityConfiguration<DoorAccessLog>
    {
        public override void Configure(EntityTypeBuilder<DoorAccessLog> builder)
        {
            base.Configure(builder);

            builder.Property(x => x.IsSuccess)
                   .IsRequired();

            builder.Property(x => x.Description)
                   .IsRequired();
        }
    }
}
