using IdentityService.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IdentityService.DAL.Configurations;

public class EmployerIndustryConfiguration : IEntityTypeConfiguration<EmployerIndustry>
{
    public void Configure(EntityTypeBuilder<EmployerIndustry> builder)
    {
        builder.ToTable("EmployerIndustries");

        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.NormalizedName)
            .IsRequired()
            .HasMaxLength(200);
    }
}