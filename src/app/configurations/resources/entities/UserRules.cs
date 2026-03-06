using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using backend.app.models.core;

namespace backend.app.configurations.resources
{
       public class UserRules : IEntityTypeConfiguration<User>
       {
              public void Configure(EntityTypeBuilder<User> builder)
              {
                     builder.HasIndex(u => u.Email).IsUnique();
                     builder.HasIndex(u => u.Username).IsUnique();
                     builder.HasIndex(u => u.GoogleId).IsUnique();
                     builder.HasIndex(u => u.MicrosoftId).IsUnique();

                     builder.HasIndex(u => new { u.Status, u.Usertype });

                     builder.Property(u => u.Email)
                            .IsRequired()
                            .HasMaxLength(255);

                     builder.Property(u => u.Usertype)
                            .IsRequired();

                     builder.Property(u => u.Status)
                            .IsRequired();
                            
                     builder.HasQueryFilter(u => u.Status != UserStatus.SoftDelete);
              }
       }
}