using VVidman.BlogBuild;
using VVidman.BlogBuild.Models;

namespace BlogBuild.Tests;

public class PathResolverTests
{
    private static readonly string ProjectDir = Path.Combine(TestPaths.RepoRoot, "blog-build");

    [Fact]
    public void Resolve_makes_paths_absolute_against_project_dir()
    {
        var cfg = new BuildConfig
        {
            Paths = new PathConfig { Input = "../content", Templates = "templates", Output = "../docs" }
        };
        var p = PathResolver.Resolve(cfg, ProjectDir);
        Assert.Equal(Path.Combine(TestPaths.RepoRoot, "content"), p.Input);
        Assert.Equal(Path.Combine(TestPaths.RepoRoot, "docs"), p.Output);
        Assert.Equal(Path.Combine(ProjectDir, "templates"), p.Templates);
        Assert.Equal(ProjectDir, p.ProjectDir);
    }

    [Fact]
    public void FindProjectDir_walks_up_from_bin_folder()
    {
        var start = Path.Combine(ProjectDir, "bin", "Debug", "net10.0");
        Assert.Equal(ProjectDir, PathResolver.FindProjectDir(start));
    }

    [Fact]
    public void FindProjectDir_throws_when_none()
    {
        var root = Path.GetPathRoot(Path.GetTempPath())!;
        Assert.Throws<DirectoryNotFoundException>(() => PathResolver.FindProjectDir(root));
    }
}
