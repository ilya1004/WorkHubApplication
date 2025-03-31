using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IdentityService.DAL.Configurations;

public class AppUserConfiguration : IEntityTypeConfiguration<AppUser>
{
    public void Configure(EntityTypeBuilder<AppUser> builder)
    {
        builder.ToTable("Users");

        builder.Property(u => u.UserName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(u => u.RegisteredAt)
            .IsRequired();

        builder.Property(u => u.ImageUrl)
            .HasMaxLength(500);

        builder.Property(u => u.RefreshToken)
            .HasMaxLength(200);

        builder.HasOne(u => u.FreelancerProfile)
            .WithOne(f => f.User)
            .HasForeignKey<FreelancerProfile>(f => f.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(u => u.EmployerProfile)
            .WithOne(e => e.User)
            .HasForeignKey<EmployerProfile>(e => e.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(u => u.Role)
            .WithMany()
            .HasForeignKey(u => u.RoleId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}