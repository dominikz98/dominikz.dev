using dominikz.Common.Enumerations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace dominikz.Api.Models.Configs
{
    public class ActivityConfig : IEntityTypeConfiguration<Activity>
    {
        public void Configure(EntityTypeBuilder<Activity> builder)
        {
            builder.ToTable("activities");

            builder.HasKey(x => x.Id);
            builder.Property(e => e.Id)
                .ValueGeneratedOnAdd();

            builder.Property(e => e.Title)
                .IsRequired();

            builder.Property(e => e.Release)
                .IsRequired();

            builder.Property(e => e.Description)
                .IsRequired();

            builder.Property(e => e.Category)
                .IsRequired()
                .HasConversion(e => (int)e, e => (ActivityCategory)e);

            builder.HasMany(e => e.Tags)
                .WithMany(e => e.Activities);
        }
    }
}
