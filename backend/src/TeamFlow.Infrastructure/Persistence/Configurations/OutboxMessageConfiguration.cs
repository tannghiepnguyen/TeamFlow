using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TeamFlow.Domain.Entities;
using TeamFlow.Infrastructure.Outbox;

namespace TeamFlow.Infrastructure.Persistence.Configurations;

public sealed class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
{
  public void Configure(EntityTypeBuilder<OutboxMessage> builder)
  {
    builder.HasKey(message => message.Id);

    builder.Property(message => message.EventType).IsRequired().HasMaxLength(150);
    builder.Property(message => message.AggregateType).IsRequired().HasMaxLength(80);
    builder.Property(message => message.Payload).HasColumnType("jsonb");
    builder.Property(message => message.LockedBy).HasMaxLength(100);
    builder.Property(message => message.Attempts).HasDefaultValue(0);

    builder.HasIndex(message => new { message.AvailableAt, message.Id })
      .HasFilter("\"ProcessedAt\" IS NULL");

    builder.HasOne<Workspace>()
      .WithMany()
      .HasForeignKey(message => message.WorkspaceId)
      .OnDelete(DeleteBehavior.Restrict);
  }
}
