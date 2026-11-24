using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TeamFlow.Domain.Entities;
using TeamFlow.Infrastructure.Identity;

namespace TeamFlow.Infrastructure.Persistence.Configurations;

public sealed class IssueAttachmentConfiguration : IEntityTypeConfiguration<IssueAttachment>
{
  public void Configure(EntityTypeBuilder<IssueAttachment> builder)
  {
    builder.ToTable(nameof(IssueAttachment));
    builder.HasKey(attachment => attachment.Id);

    builder.Property(attachment => attachment.OriginalFileName).IsRequired().HasMaxLength(255);
    builder.Property(attachment => attachment.ObjectKey).IsRequired();
    builder.Property(attachment => attachment.ContentType).IsRequired().HasMaxLength(120);
    builder.ToTable(table => table.HasCheckConstraint("CK_IssueAttachment_Size", "\"SizeBytes\" >= 0"));

    builder.HasOne<Issue>()
      .WithMany()
      .HasForeignKey(attachment => new { attachment.WorkspaceId, attachment.IssueId })
      .HasPrincipalKey(issue => new { issue.WorkspaceId, issue.Id })
      .OnDelete(DeleteBehavior.Restrict);

    builder.HasOne<ApplicationUser>()
      .WithMany()
      .HasForeignKey(attachment => attachment.UploadedByUserId)
      .OnDelete(DeleteBehavior.Restrict);
  }
}
