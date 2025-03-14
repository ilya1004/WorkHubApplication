using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ProjectsService.Infrastructure.Configurations;

public class LifecycleConfiguration : IEntityTypeConfiguration<Lifecycle>
{
    public void Configure(EntityTypeBuilder<Lifecycle> builder)
    {
        builder.ToTable("Lifecycles");

        builder.HasKey(l => l.Id);

        builder.Property(l => l.CreatedAt)
            .IsRequired();

        builder.Property(l => l.UpdatedAt)
            .IsRequired();

        builder.Property(l => l.ApplicationsStartDate)
            .IsRequired();

        builder.Property(l => l.ApplicationsDeadline)
            .IsRequired();

        builder.Property(l => l.WorkStartDate)
            .IsRequired();

        builder.Property(l => l.WorkDeadline)
            .IsRequired();
        
        builder.Property(l => l.AcceptanceRequested)
            .IsRequired()
            .HasDefaultValue(false);
        
        builder.Property(l => l.AcceptanceConfirmed)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(l => l.Status)
            .IsRequired();

        builder.Property(l => l.ProjectId)
            .IsRequired();
    }
}
