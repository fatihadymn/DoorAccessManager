using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DoorAccessManager.Items.Entities;

namespace DoorAccessManager.Data.Configurations
{
    public class BaseEntityConfiguration<TEntity> : IEntityTypeConfiguration<TEntity> where TEntity : BaseEntity
    {
        public virtual void Configure(EntityTypeBuilder<TEntity> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.CreatedOn)
                   .IsRequired();

            builder.Property(x => x.UpdatedOn)
                   .IsRequired(false);
        }
    }
}
