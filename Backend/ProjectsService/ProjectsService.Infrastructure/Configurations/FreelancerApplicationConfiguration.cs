using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ProjectsService.Infrastructure.Configurations;

public class FreelancerApplicationConfiguration : IEntityTypeConfiguration<FreelancerApplication>
{
    public void Configure(EntityTypeBuilder<FreelancerApplication> builder)
    {
        builder.ToTable("FreelancerApplications");

        builder.HasKey(fa => fa.Id);

        builder.Property(fa => fa.CreatedAt)
            .IsRequired();

        builder.Property(fa => fa.Status)
            .IsRequired();

        builder.Property(fa => fa.FreelancerId)
            .IsRequired();
    }
}
