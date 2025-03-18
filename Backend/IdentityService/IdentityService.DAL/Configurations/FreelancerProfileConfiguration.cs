using IdentityService.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IdentityService.DAL.Configurations;

public class FreelancerProfileConfiguration : IEntityTypeConfiguration<FreelancerProfile>
{
    public void Configure(EntityTypeBuilder<FreelancerProfile> builder)
    {
        builder.ToTable("FreelancerProfiles");

        builder.HasIndex(f => f.UserId)
            .IsUnique();

        builder.Property(f => f.FirstName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(f => f.LastName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(f => f.About)
            .HasMaxLength(1000);

        builder.HasMany(f => f.Skills)
            .WithMany(s => s.FreelancerProfiles);

        builder.Property(f => f.StripeAccountId)
            .HasMaxLength(50);
    }
}