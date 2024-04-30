using DoorAccessManager.Items.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DoorAccessManager.Data.Configurations
{
    public class DoorRoleConfiguration : BaseEntityConfiguration<DoorRole>
    {
        public override void Configure(EntityTypeBuilder<DoorRole> builder)
        {
            base.Configure(builder);
        }
    }
}
