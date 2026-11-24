using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TeamFlow.Domain.Entities;
using TeamFlow.Infrastructure.Identity;

namespace TeamFlow.Infrastructure.Persistence.Configurations;

public sealed class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
  public void Configure(EntityTypeBuilder<Notification> builder)
  {
    builder.ToTable(nameof(Notification));
    builder.HasKey(notification => notification.Id);

    builder.Property(notification => notification.Type).HasConversion<string>().HasMaxLength(60);
    builder.Property(notification => notification.Payload).HasColumnType("jsonb");

    builder.HasIndex(notification => new { notification.RecipientUserId, notification.SourceEventId, notification.Type })
      .IsUnique();
    builder.HasIndex(notification => new { notification.WorkspaceId, notification.RecipientUserId, notification.CreatedAt })
      .HasFilter("\"ReadAt\" IS NULL");

    builder.HasOne<Workspace>()
      .WithMany()
      .HasForeignKey(notification => notification.WorkspaceId)
      .OnDelete(DeleteBehavior.Restrict);

    builder.HasOne<ApplicationUser>()
      .WithMany()
      .HasForeignKey(notification => notification.RecipientUserId)
      .OnDelete(DeleteBehavior.Restrict);
  }
}
