using System.Text.RegularExpressions;
using VVidman.BlogBuild;

namespace BlogBuild.Tests;

public class PostIndexTests : IClassFixture<SiteFixture>
{
    private readonly SiteFixture _site;
    public PostIndexTests(SiteFixture site) => _site = site;

    private string Index => _site.Read("posts/index.html");

    [Fact]
    public void Lists_every_post_exactly_once()
    {
        var posts = Directory.GetFiles(Path.Combine(_site.Output, "posts"), "*.html")
            .Select(Path.GetFileName)
            .Where(n => n != "index.html")
            .Where(n => !File.ReadAllText(Path.Combine(_site.Output, "posts", n!)).Contains("http-equiv=\"refresh\"")) // redirect stubs are not posts
            .ToList();
        Assert.NotEmpty(posts);

        foreach (var name in posts)
        {
            var count = Regex.Matches(Index, $"href=\"/posts/{Regex.Escape(name!)}\"").Count;
            Assert.True(count == 1, $"{name} listed {count} times");
        }
    }

    [Fact]
    public void Series_block_lists_parts_in_order()
    {
        var hrefs = Enumerable.Range(1, 4).Select(i => Index.IndexOf($"href=\"/posts/inferrouter-0{i}.html\"", StringComparison.Ordinal)).ToList();
        Assert.All(hrefs, h => Assert.True(h > 0));
        Assert.Equal(hrefs.OrderBy(h => h), hrefs);
        Assert.Contains("class=\"series-block\"", Index);
        Assert.Contains("class=\"part-badge\">#1<", Index);
        Assert.Contains("4 parts", Index);
        Assert.Contains("4 rész", Index);
    }

    [Fact]
    public void Standalone_block_comes_after_series_blocks()
    {
        var standalone = Index.IndexOf("Standalone posts", StringComparison.Ordinal);
        Assert.True(standalone > 0);
        Assert.True(Index.IndexOf("inferrouter-04.html", StringComparison.Ordinal) < standalone);
        Assert.Contains("Egyéb bejegyzések", Index);
    }

    [Fact]
    public void Hungarian_title_is_inside_hu_span()
    {
        var parsed = MarkdownDslParser.Parse(Path.Combine(TestPaths.RepoRoot, "content", "posts", "raglab-01.md"));
        Assert.Contains($"<span data-l=\"hu\">{VVidman.BlogBuild.Renderers.I18n.Encode(parsed.Metadata.TitleHu)}</span>", Index);
    }

    [Fact]
    public void Index_page_is_marked_as_blog_and_has_title()
    {
        Assert.Contains("href=\"/posts/\" aria-current=\"page\"", Index);
        Assert.Contains("<h1 class=\"post-title\"><span data-l=\"en\">Posts</span><span data-l=\"hu\">Bejegyzések</span></h1>", Index);
    }
}
