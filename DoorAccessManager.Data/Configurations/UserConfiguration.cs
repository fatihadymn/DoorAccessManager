using DoorAccessManager.Items.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DoorAccessManager.Data.Configurations
{
    public class UserConfiguration : BaseEntityConfiguration<User>
    {
        public override void Configure(EntityTypeBuilder<User> builder)
        {
            base.Configure(builder);

            builder.Property(x => x.Name)
                   .IsRequired();

            builder.Property(x => x.Username)
                   .IsRequired();

            builder.HasIndex(x => x.Username)
                   .IsUnique();

            builder.Property(x => x.PasswordHash)
                   .IsRequired();

            builder.Property(x => x.IsActive)
                   .IsRequired();

            builder.HasMany(x => x.DoorAccessLogs)
                   .WithOne(x => x.User)
                   .HasForeignKey(x => x.UserId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
