using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TeamFlow.Infrastructure.Outbox;

namespace TeamFlow.Infrastructure.Persistence.Configurations;

public sealed class ProcessedEventConfiguration : IEntityTypeConfiguration<ProcessedEvent>
{
  public void Configure(EntityTypeBuilder<ProcessedEvent> builder)
  {
    builder.ToTable(nameof(ProcessedEvent));
    builder.HasKey(processedEvent => new { processedEvent.ConsumerName, processedEvent.EventId });

    builder.Property(processedEvent => processedEvent.ConsumerName).IsRequired().HasMaxLength(100);
  }
}
