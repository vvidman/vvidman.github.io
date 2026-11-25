using VVidman.BlogBuild;
using VVidman.BlogBuild.Models;

namespace BlogBuild.Tests;

/// <summary>Builds a content folder into a temp output dir once per test class.</summary>
public class SiteFixture : IDisposable
{
    public string Output { get; } = Path.Combine(Path.GetTempPath(), "site-" + Guid.NewGuid().ToString("N"));
    public BuildReport Report { get; }
    public string Log { get; }

    public SiteFixture() : this(Path.Combine(TestPaths.RepoRoot, "content")) { }

    protected SiteFixture(string input)
    {
        var projectDir = Path.Combine(TestPaths.RepoRoot, "blog-build");
        var cfg = new BuildConfig
        {
            Defaults = new DefaultConfig { Languages = new[] { "en", "hu" }, Layout = "page" }
        };
        var paths = new ResolvedPaths(projectDir, input, Path.Combine(projectDir, "templates"), Output);
        var log = new StringWriter();
        Report = SiteBuilder.Build(cfg, paths, log);
        Log = log.ToString();
    }

    public string Read(string slug) => File.ReadAllText(Path.Combine(Output, slug));

    public void Dispose() { if (Directory.Exists(Output)) Directory.Delete(Output, true); }
}

public sealed class NoHuSiteFixture()
    : SiteFixture(Path.Combine(TestPaths.RepoRoot, "blog-build.Tests", "Fixtures", "nohu"));

public sealed class CollisionSiteFixture()
    : SiteFixture(Path.Combine(TestPaths.RepoRoot, "blog-build.Tests", "Fixtures", "collision"));

public sealed class BadTopicSiteFixture()
    : SiteFixture(Path.Combine(TestPaths.RepoRoot, "blog-build.Tests", "Fixtures", "badtopic"));

public sealed class NoTopicSiteFixture()
    : SiteFixture(Path.Combine(TestPaths.RepoRoot, "blog-build.Tests", "Fixtures", "notopic"));

public sealed class MixedTopicSiteFixture()
    : SiteFixture(Path.Combine(TestPaths.RepoRoot, "blog-build.Tests", "Fixtures", "mixedtopic"));

public sealed class OneTopicSiteFixture()
    : SiteFixture(Path.Combine(TestPaths.RepoRoot, "blog-build.Tests", "Fixtures", "onetopic"));
