using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TeamFlow.Domain.Entities;
using TeamFlow.Infrastructure.Identity;

namespace TeamFlow.Infrastructure.Persistence.Configurations;

public sealed class CommentConfiguration : IEntityTypeConfiguration<Comment>
{
  public void Configure(EntityTypeBuilder<Comment> builder)
  {
    builder.HasKey(comment => comment.Id);
    builder.Property(comment => comment.Body).IsRequired();
    builder.ToTable(table => table.HasCheckConstraint("CK_Comment_BodyLength", """char_length("Body") <= 10000"""));

    builder.HasOne<Issue>()
      .WithMany()
      .HasForeignKey(comment => new { comment.WorkspaceId, comment.IssueId })
      .HasPrincipalKey(issue => new { issue.WorkspaceId, issue.Id })
      .OnDelete(DeleteBehavior.Restrict);

    builder.HasOne<ApplicationUser>()
      .WithMany()
      .HasForeignKey(comment => comment.AuthorUserId)
      .OnDelete(DeleteBehavior.Restrict);
  }
}
