using System.Text.RegularExpressions;
using VVidman.BlogBuild;

namespace BlogBuild.Tests;

public class PostIndexTests : IClassFixture<SiteFixture>, IClassFixture<MixedTopicSiteFixture>,
    IClassFixture<OneTopicSiteFixture>, IClassFixture<NoTopicSiteFixture>
{
    private readonly SiteFixture _site;
    private readonly MixedTopicSiteFixture _mixed;
    private readonly OneTopicSiteFixture _one;
    private readonly NoTopicSiteFixture _none;

    public PostIndexTests(SiteFixture site, MixedTopicSiteFixture mixed, OneTopicSiteFixture one, NoTopicSiteFixture none)
    {
        _site = site; _mixed = mixed; _one = one; _none = none;
    }

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

    private static int At(string html, string needle) => html.IndexOf(needle, StringComparison.Ordinal);

    [Fact]
    public void Topics_appear_in_registry_order()
    {
        var ids = new[] { "ai-workflow", "llm-infra", "tooling" }
            .Select(k => At(Index, $"<section class=\"topic-block\" id=\"{k}\">")).ToList();
        Assert.All(ids, i => Assert.True(i > 0));
        Assert.Equal(ids.OrderBy(i => i), ids);
    }

    [Fact]
    public void Topic_head_has_name_and_blurb()
    {
        Assert.Contains("<header class=\"topic-head\"><h2><span data-l=\"en\">LLM infrastructure in .NET</span><span data-l=\"hu\">LLM infrastruktúra .NET-ben</span></h2>", Index);
        Assert.Contains("<p class=\"topic-blurb\"><span data-l=\"en\">What runs underneath: routing, retrieval, observability.</span>", Index);
    }

    [Fact]
    public void InferRouter_block_is_inside_llm_infra()
    {
        var start = At(Index, "id=\"llm-infra\"");
        var end = At(Index, "id=\"tooling\"");
        for (var i = 1; i <= 4; i++)
        {
            var pos = At(Index, $"href=\"/posts/inferrouter-0{i}.html\"");
            Assert.InRange(pos, start, end);
        }
    }

    [Fact]
    public void Home_lab_posts_are_inside_tooling()
    {
        var start = At(Index, "id=\"tooling\"");
        foreach (var slug in new[] { "homelab-docker-server", "homelab-legacy-bios" })
            Assert.True(At(Index, $"href=\"/posts/{slug}.html\"") > start, slug);
    }

    [Fact]
    public void Series_blocks_come_before_standalone_posts_within_a_topic()
    {
        // tooling: Site Generator series first, then the standalone home lab posts
        var series = At(Index, "href=\"/posts/why-i-built-my-own-static-site-generator.html\"");
        var standalone = At(Index, "href=\"/posts/homelab-docker-server.html\"");
        Assert.True(series > 0 && series < standalone);
    }

    [Fact]
    public void Topic_nav_links_to_every_topic()
    {
        foreach (var k in new[] { "ai-workflow", "llm-infra", "tooling" })
            Assert.Contains($"<a href=\"#{k}\">", Index);
        Assert.Contains("class=\"topic-nav\"", Index);
    }

    [Fact]
    public void No_other_section_when_every_post_has_a_topic()
    {
        Assert.DoesNotContain("id=\"other\"", Index);
        Assert.DoesNotContain("Egyéb</span>", Index);
    }

    [Fact]
    public void Post_without_topic_lands_in_other_and_is_still_listed()
    {
        var html = _none.Read("posts/index.html");
        Assert.Contains("<section class=\"topic-block\" id=\"other\">", html);
        Assert.Contains("href=\"/posts/none.html\"", html);
        Assert.Contains("Egyéb", html);
    }

    [Fact]
    public void Topic_without_posts_renders_nothing()
    {
        var html = _one.Read("posts/index.html");
        Assert.Contains("id=\"tooling\"", html);
        Assert.DoesNotContain("id=\"ai-workflow\"", html);
        Assert.DoesNotContain("id=\"llm-infra\"", html);
        Assert.DoesNotContain("href=\"#ai-workflow\"", html);
        Assert.DoesNotContain("id=\"other\"", html);
    }

    [Fact]
    public void Series_with_mixed_topics_is_a_build_error()
    {
        Assert.Equal(1, _mixed.Report.Errors);
        Assert.Contains("Mixed", _mixed.Log);
        Assert.Contains("topic", _mixed.Log);
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
