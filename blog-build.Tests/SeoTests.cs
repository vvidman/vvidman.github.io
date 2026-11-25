using System.Globalization;
using System.Xml.Linq;

namespace BlogBuild.Tests;

public class SeoTests : IClassFixture<SiteFixture>
{
    private readonly SiteFixture _site;
    public SeoTests(SiteFixture site) => _site = site;

    private static readonly XNamespace Sm = "http://www.sitemaps.org/schemas/sitemap/0.9";

    [Fact]
    public void Real_content_builds_without_errors() => Assert.True(_site.Report.Errors == 0, _site.Log);

    [Fact]
    public void Sitemap_lists_real_pages_but_not_stubs_or_404()
    {
        var doc = XDocument.Parse(_site.Read("sitemap.xml"));
        var locs = doc.Descendants(Sm + "loc").Select(e => e.Value).ToList();

        Assert.Contains("https://vvidman.github.io/", locs);
        Assert.Contains("https://vvidman.github.io/posts/", locs);
        Assert.Contains("https://vvidman.github.io/about/", locs);
        Assert.Contains("https://vvidman.github.io/certifications/", locs);
        Assert.Contains("https://vvidman.github.io/posts/raglab-01.html", locs);
        Assert.DoesNotContain(locs, l => l.EndsWith("/posts/post-02.html"));
        Assert.DoesNotContain(locs, l => l.Contains("404"));
        Assert.Equal(locs.Count, locs.Distinct().Count());
    }

    [Fact]
    public void Sitemap_has_lastmod_for_dated_posts()
    {
        var doc = XDocument.Parse(_site.Read("sitemap.xml"));
        var url = doc.Descendants(Sm + "url")
            .Single(u => (string)u.Element(Sm + "loc")! == "https://vvidman.github.io/posts/raglab-01.html");
        Assert.Matches(@"^\d{4}-\d{2}-\d{2}$", (string)url.Element(Sm + "lastmod")!);
    }

    [Fact]
    public void Robots_allows_all_and_points_to_sitemap()
    {
        var robots = _site.Read("robots.txt");
        Assert.Contains("User-agent: *", robots);
        Assert.Contains("Allow: /", robots);
        Assert.Contains("Sitemap: https://vvidman.github.io/sitemap.xml", robots);
    }

    [Fact]
    public void Rss_is_valid_newest_first_and_capped()
    {
        var doc = XDocument.Parse(_site.Read("feed.xml"));
        Assert.Equal("2.0", (string?)doc.Root!.Attribute("version"));
        Assert.Equal("Viktor Vidman", (string)doc.Root.Element("channel")!.Element("title")!);

        var items = doc.Descendants("item").ToList();
        Assert.InRange(items.Count, 1, 20);

        var dates = items.Select(i => DateTime.ParseExact((string)i.Element("pubDate")!, "R", CultureInfo.InvariantCulture)).ToList();
        Assert.Equal(dates.OrderByDescending(d => d), dates);

        foreach (var item in items)
        {
            var link = (string)item.Element("link")!;
            Assert.StartsWith("https://vvidman.github.io/posts/", link);
            Assert.Equal(link, (string)item.Element("guid")!);
            Assert.False(string.IsNullOrWhiteSpace((string)item.Element("title")!));
        }
        Assert.DoesNotContain(items, i => ((string)i.Element("link")!).EndsWith("post-02.html"));
    }

    [Fact]
    public void NotFound_page_exists_and_is_bilingual()
    {
        var html = _site.Read("404.html");
        Assert.Contains("Page not found", html);
        Assert.Contains("Az oldal nem található", html);
        Assert.Contains("href=\"/\"", html);
        Assert.Contains("href=\"/posts/\"", html);
        Assert.Contains("<h1", html);
    }
}
