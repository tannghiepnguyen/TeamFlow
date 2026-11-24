using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TeamFlow.Domain.Entities;
using TeamFlow.Infrastructure.Identity;
using TeamFlow.Infrastructure.Outbox;

namespace TeamFlow.Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options)
    : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>(options)
{
  public DbSet<ActivityLog> ActivityLogs => Set<ActivityLog>();
  public DbSet<Comment> Comments => Set<Comment>();
  public DbSet<Issue> Issues => Set<Issue>();
  public DbSet<IssueAttachment> IssueAttachments => Set<IssueAttachment>();
  public DbSet<IssueLabel> IssueLabels => Set<IssueLabel>();
  public DbSet<Label> Labels => Set<Label>();
  public DbSet<Notification> Notifications => Set<Notification>();
  public DbSet<NotificationPreference> NotificationPreferences => Set<NotificationPreference>();
  public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();
  public DbSet<ProcessedEvent> ProcessedEvents => Set<ProcessedEvent>();
  public DbSet<Project> Projects => Set<Project>();
  public DbSet<ProjectIssueSequence> ProjectIssueSequences => Set<ProjectIssueSequence>();
  public DbSet<Sprint> Sprints => Set<Sprint>();
  public DbSet<Workspace> Workspaces => Set<Workspace>();
  public DbSet<WorkspaceInvitation> WorkspaceInvitations => Set<WorkspaceInvitation>();
  public DbSet<WorkspaceMember> WorkspaceMembers => Set<WorkspaceMember>();

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);
    modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
  }
}
