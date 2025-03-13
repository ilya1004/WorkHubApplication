using IdentityService.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IdentityService.DAL.Configurations;

public class EmployerProfileConfiguration : IEntityTypeConfiguration<EmployerProfile>
{
    public void Configure(EntityTypeBuilder<EmployerProfile> builder)
    {
        builder.ToTable("EmployerProfiles");

        builder.HasIndex(e => e.UserId)
            .IsUnique();

        builder.Property(e => e.CompanyName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.About)
            .HasMaxLength(1000);

        builder.HasOne(e => e.Industry)
            .WithMany(i => i.EmployerProfiles)
            .HasForeignKey(e => e.IndustryId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Property(e => e.StripeCustomerId)
            .HasMaxLength(50);
    }
}