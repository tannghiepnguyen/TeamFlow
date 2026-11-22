namespace TeamFlow.Infrastructure.Persistence;

public class ProjectIssueSequence
{
  public Guid ProjectId { get; set; }
  public int NextNumber { get; set; } = 1;
}
