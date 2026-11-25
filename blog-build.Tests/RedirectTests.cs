using System.Text.RegularExpressions;
using VVidman.BlogBuild;

namespace BlogBuild.Tests;

public class RedirectTests : IClassFixture<SiteFixture>, IClassFixture<CollisionSiteFixture>
{
    private readonly SiteFixture _site;
    private readonly CollisionSiteFixture _collision;

    public RedirectTests(SiteFixture site, CollisionSiteFixture collision) { _site = site; _collision = collision; }

    // spec §7: old slug -> new slug
    public static TheoryData<string, string> Renames => new()
    {
        { "posts/post-02.html",           "posts/why-i-built-my-own-static-site-generator.html" },
        { "posts/post-03.html",           "posts/bilingual-markdown-dsl.html" },
        { "posts/post-04.html",           "posts/generator-pipeline-to-github-pages.html" },
        { "posts/post-sp01.html",         "posts/scaffold-protocol-the-ai-executes-you-decide.html" },
        { "posts/post-chaosforge-01.html", "posts/chaosforge-let-go-of-the-reins.html" },
        { "posts/post-chaosforge-02.html", "posts/chaosforge-closing-the-loop.html" },
        { "posts/post-01.html",           "certifications/index.html" },
    };

    [Fact]
    public void Real_content_builds_without_errors() => Assert.True(_site.Report.Errors == 0, _site.Log);

    [Theory]
    [MemberData(nameof(Renames))]
    public void Old_slug_is_a_redirect_stub_to_the_new_page(string oldSlug, string newSlug)
    {
        var stub = _site.Read(oldSlug);
        var path = newSlug.EndsWith("/index.html") ? "/" + newSlug[..^"index.html".Length] : "/" + newSlug;
        Assert.Contains($"url={path}\"", stub);
        Assert.Contains($"<link rel=\"canonical\" href=\"https://vvidman.github.io{path}\">", stub);
        Assert.Contains("name=\"robots\" content=\"noindex\"", stub);
        Assert.Contains($"<a href=\"{path}\">", stub);
        Assert.True(File.Exists(Path.Combine(_site.Output, newSlug)), $"{newSlug} not generated");
    }

    [Fact]
    public void Stub_target_is_a_page_not_a_stub()
    {
        var real = _site.Read("posts/why-i-built-my-own-static-site-generator.html");
        Assert.DoesNotContain("http-equiv=\"refresh\"", real);
    }

    [Fact]
    public void Stub_path_colliding_with_real_page_is_a_build_error()
    {
        Assert.True(_collision.Report.Errors == 1, _collision.Log);
        // the real page must survive untouched
        Assert.Contains("Page B", _collision.Read("posts/b.html"));
    }

    [Fact]
    public void Content_has_no_links_to_old_slugs()
    {
        var link = new Regex(@"(\]\(|href="")/posts/(post-0\d|post-sp01|post-chaosforge-\d+)\.html");
        var bad = Directory.GetFiles(Path.Combine(TestPaths.RepoRoot, "content"), "*.md", SearchOption.AllDirectories)
            .Where(f => link.IsMatch(File.ReadAllText(f)))
            .Select(Path.GetFileName)
            .ToList();
        Assert.True(bad.Count == 0, "Old-slug links in: " + string.Join(", ", bad));
    }
}
