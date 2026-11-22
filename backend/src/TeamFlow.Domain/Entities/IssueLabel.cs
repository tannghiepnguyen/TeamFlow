namespace TeamFlow.Domain.Entities;

public class IssueLabel
{
  public Guid WorkspaceId { get; set; }
  public Guid IssueId { get; set; }
  public Guid LabelId { get; set; }
}
