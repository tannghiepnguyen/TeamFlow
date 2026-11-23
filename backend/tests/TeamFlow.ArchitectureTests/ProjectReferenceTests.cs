using System.Xml.Linq;

namespace TeamFlow.ArchitectureTests;

public class ProjectReferenceTests
{
  [Theory]
  [InlineData("Domain", new string[] { })]
  [InlineData("Application", new[] { "Domain" })]
  [InlineData("Infrastructure", new[] { "Application" })]
  [InlineData("API", new[] { "Application", "Infrastructure" })]
  [InlineData("Worker", new[] { "Application", "Infrastructure" })]
  public void DirectProjectReferencesMatchArchitecture(string project, string[] expectedReferences)
  {
    var projectRoot = Path.Combine(AppContext.BaseDirectory, "ProjectFiles");
    var projectFile = ProjectFilePath(projectRoot, project);
    var projectDirectory = Path.GetDirectoryName(projectFile)!;

    var actualReferences = XDocument.Load(projectFile)
      .Descendants("ProjectReference")
      .Select(reference => (string?)reference.Attribute("Include"))
      .OfType<string>()
      .Select(include => Path.GetFullPath(include.Replace('\\', '/'), projectDirectory))
      .Order(StringComparer.Ordinal)
      .ToArray();

    var expectedPaths = expectedReferences
      .Select(reference => ProjectFilePath(projectRoot, reference))
      .Order(StringComparer.Ordinal)
      .ToArray();

    Assert.Equal(expectedPaths, actualReferences);
  }

  private static string ProjectFilePath(string projectRoot, string project)
  {
    var projectName = $"TeamFlow.{project}";
    return Path.Combine(projectRoot, projectName, $"{projectName}.csproj");
  }
}
