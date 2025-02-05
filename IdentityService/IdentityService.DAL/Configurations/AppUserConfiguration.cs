using IdentityService.DAL.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.DAL.Configurations;

public class AppUserConfiguration : IEntityTypeConfiguration<AppUser>
{
    public void Configure(EntityTypeBuilder<AppUser> builder)
    {
        builder.ToTable("Users");

        builder.Property(u => u.RegisteredAt)
            .IsRequired();

        builder.Property(u => u.ImageUrl)
            .HasMaxLength(500);

        builder.HasOne(u => u.FreelancerProfile)
            .WithOne()
            .HasForeignKey<AppUser>(u => u.FreelancerProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(u => u.EmployerProfile)
            .WithOne()
            .HasForeignKey<AppUser>(u => u.EmployerProfileId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
