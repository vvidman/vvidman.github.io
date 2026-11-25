using System.Text.RegularExpressions;
using VVidman.BlogBuild;

namespace BlogBuild.Tests;

public class PageRenderingTests : IClassFixture<SiteFixture>, IClassFixture<NoHuSiteFixture>
{
    private readonly SiteFixture _site;
    private readonly NoHuSiteFixture _noHu;

    public PageRenderingTests(SiteFixture site, NoHuSiteFixture noHu) { _site = site; _noHu = noHu; }

    [Fact]
    public void Real_content_builds_without_errors() => Assert.True(_site.Report.Errors == 0, _site.Log);

    [Fact]
    public void Series_post_has_tree_pager_and_meta()
    {
        var html = _site.Read("posts/inferrouter-02.html");
        Assert.Contains("series-tree", html);
        Assert.Contains("aria-current=\"page\"", html);
        Assert.Contains("class=\"pager\"", html);
        Assert.Contains("2/4", html);
        Assert.Contains("min read", html);
        Assert.Contains("post-layout has-series", html);
    }

    [Fact]
    public void Single_part_series_has_no_tree_pill_or_pager()
    {
        var html = _site.Read("posts/raglab-01.html");
        Assert.DoesNotContain("series-tree", html);
        Assert.DoesNotContain("1/1", html);
        Assert.DoesNotContain("class=\"pager\"", html);
        Assert.DoesNotContain("has-series", html);
    }

    [Fact]
    public void About_page_has_no_post_meta()
    {
        var html = _site.Read("about/index.html");
        Assert.DoesNotContain("post-meta", html);
        Assert.DoesNotContain("min read", html);
        Assert.DoesNotContain("pill", html);
        Assert.DoesNotContain("series-tree", html);
        Assert.Contains("page-layout", html);
    }

    [Fact]
    public void Missing_hu_title_falls_back_to_en()
    {
        var html = _noHu.Read("posts/nohu.html");
        var en = Regex.Match(html, "data-title-en=\"([^\"]*)\"").Groups[1].Value;
        var hu = Regex.Match(html, "data-title-hu=\"([^\"]*)\"").Groups[1].Value;
        Assert.NotEmpty(en);
        Assert.Equal(en, hu);
        Assert.Contains("&quot;quoted&quot; &amp; title", en);
        Assert.DoesNotContain("data-l=\"hu\"></span>", html);
        var h1 = Regex.Match(html, "<h1[^>]*>(.*?)</h1>", RegexOptions.Singleline).Groups[1].Value;
        Assert.DoesNotContain("data-l=", h1);
    }

    [Fact]
    public void Missing_hu_section_falls_back_to_en_content()
    {
        var html = _noHu.Read("posts/nohu.html");
        var hu = html.Substring(html.IndexOf("<div class=\"prose\" data-l=\"hu\">", StringComparison.Ordinal));
        Assert.Contains("Some English text.", hu);
    }

    [Fact]
    public void Literal_placeholders_in_posts_survive()
    {
        var slug = MarkdownDslParser.Parse(Path.Combine(TestPaths.RepoRoot, "content", "posts", "post_dslForSiteGenerator.md")).Metadata.Slug;
        var html = _site.Read(slug);
        Assert.Equal(0, _site.Report.Errors);
        Assert.Contains("&#123;&#123;", html);
    }

    [Fact]
    public void Html_lang_attributes_present()
    {
        var html = _site.Read("posts/inferrouter-02.html");
        Assert.Matches("<html[^>]*lang=\"en\"[^>]*data-lang=\"en\"[^>]*data-title-en=\"", html);
    }

    [Fact]
    public void Every_generated_page_has_an_h1()
    {
        foreach (var file in Directory.GetFiles(_site.Output, "*.html", SearchOption.AllDirectories)
                     .Where(f => !f.EndsWith(Path.Combine("posts", "index.html"))))
        {
            var html = File.ReadAllText(file);
            if (html.Contains("http-equiv=\"refresh\"")) continue; // redirect stubs have no content
            Assert.True(Regex.Matches(html, "<h1[ >]").Count >= 1, file);
        }
    }
}
