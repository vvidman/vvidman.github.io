using System.Text.RegularExpressions;

namespace BlogBuild.Tests;

public class LinkCheckTests : IClassFixture<SiteFixture>
{
    private readonly SiteFixture _site;
    public LinkCheckTests(SiteFixture site) => _site = site;


    [Fact]
    public void All_internal_links_resolve()
    {
        Assert.True(_site.Report.Errors == 0, _site.Log);

        var missing = new List<string>();
        foreach (var file in Directory.GetFiles(_site.Output, "*.html", SearchOption.AllDirectories))
        {
            var html = File.ReadAllText(file);
            foreach (Match m in Regex.Matches(html, "(?:href|src)=\"(/[^\"]*)\""))
            {
                var target = m.Groups[1].Value;
                if (target.StartsWith("//")) continue;
                var path = target.Split('#', '?')[0];
                if (!Resolves(path))
                    missing.Add($"{Path.GetRelativePath(_site.Output, file)} -> {target}");
            }
        }
        Assert.True(missing.Count == 0, "Broken internal links:\n" + string.Join("\n", missing.Distinct()));
    }

    // Generated pages live in the temp output; static assets are authored in docs/assets.
    private bool Resolves(string path)
    {
        var rel = path.TrimStart('/');
        if (path.EndsWith('/') || rel.Length == 0)
            rel = Path.Combine(rel, "index.html");

        if (File.Exists(Path.Combine(_site.Output, rel))) return true;
        return path.StartsWith("/assets/") && File.Exists(Path.Combine(TestPaths.RepoRoot, "docs", rel));
    }
}
