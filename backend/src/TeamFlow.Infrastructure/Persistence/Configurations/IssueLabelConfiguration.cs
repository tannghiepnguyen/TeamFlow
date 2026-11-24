using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TeamFlow.Domain.Entities;

namespace TeamFlow.Infrastructure.Persistence.Configurations;

public sealed class IssueLabelConfiguration : IEntityTypeConfiguration<IssueLabel>
{
  public void Configure(EntityTypeBuilder<IssueLabel> builder)
  {
    builder.ToTable(nameof(IssueLabel));
    builder.HasKey(issueLabel => new { issueLabel.IssueId, issueLabel.LabelId });

    builder.HasOne<Issue>()
      .WithMany()
      .HasForeignKey(issueLabel => new { issueLabel.WorkspaceId, issueLabel.IssueId })
      .HasPrincipalKey(issue => new { issue.WorkspaceId, issue.Id })
      .OnDelete(DeleteBehavior.Restrict);

    builder.HasOne<Label>()
      .WithMany()
      .HasForeignKey(issueLabel => new { issueLabel.WorkspaceId, issueLabel.LabelId })
      .HasPrincipalKey(label => new { label.WorkspaceId, label.Id })
      .OnDelete(DeleteBehavior.Restrict);
  }
}
