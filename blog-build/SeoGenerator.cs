using System.Globalization;
using System.Xml.Linq;
using VVidman.BlogBuild.Models;
using VVidman.BlogBuild.Renderers;

namespace VVidman.BlogBuild;

/// <summary>sitemap.xml, robots.txt, feed.xml and the 404 page (spec §7).</summary>
public static class SeoGenerator
{
    public const string NotFoundSlug = "404.html";
    public const int FeedLimit = 20;

    private static readonly XNamespace Sm = "http://www.sitemaps.org/schemas/sitemap/0.9";

    /// <summary>All real pages plus the generated home and posts index. Redirect stubs and 404 are never listed.</summary>
    public static string Sitemap(IEnumerable<PageDocument> pages)
    {
        var entries = new List<(string Loc, DateOnly? LastMod)>
        {
            (Urls.Absolute(HomeGenerator.Slug), null),
            (Urls.Absolute("posts/index.html"), null),
        };

        foreach (var page in pages)
        {
            var slug = page.Metadata.Slug;
            if (slug is NotFoundSlug or HomeGenerator.Slug or "posts/index.html")
                continue;
            entries.Add((Urls.Absolute(slug), page.Metadata.Date));
        }

        var urlset = new XElement(Sm + "urlset",
            entries.DistinctBy(e => e.Loc).Select(e => new XElement(Sm + "url",
                new XElement(Sm + "loc", e.Loc),
                e.LastMod is { } d ? new XElement(Sm + "lastmod", d.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)) : null)));

        return Serialize(new XDocument(new XDeclaration("1.0", "utf-8", null), urlset));
    }

    public static string Robots() =>
        "User-agent: *\nAllow: /\n\nSitemap: " + Urls.Origin + "/sitemap.xml\n";

    /// <summary>RSS 2.0, English titles and descriptions. <paramref name="latest"/> must already be newest first.</summary>
    public static string Rss(IEnumerable<PageDocument> latest)
    {
        var items = latest
            .Where(p => p.Metadata.Date is not null)
            .Take(FeedLimit)
            .Select(p =>
            {
                var url = Urls.Absolute(p.Metadata.Slug);
                var date = p.Metadata.Date!.Value.ToDateTime(p.Metadata.Time ?? TimeOnly.MinValue, DateTimeKind.Utc);
                return new XElement("item",
                    new XElement("title", p.Metadata.Title),
                    new XElement("link", url),
                    new XElement("guid", new XAttribute("isPermaLink", "true"), url),
                    new XElement("pubDate", date.ToString("R", CultureInfo.InvariantCulture)),
                    new XElement("description", p.Metadata.Description));
            });

        var channel = new XElement("channel",
            new XElement("title", "Viktor Vidman"),
            new XElement("link", Urls.Origin + "/"),
            new XElement("description", "Software architecture, AI-augmented delivery and local-first LLM infrastructure."),
            new XElement("language", "en"),
            items);

        return Serialize(new XDocument(new XDeclaration("1.0", "utf-8", null),
            new XElement("rss", new XAttribute("version", "2.0"), channel)));
    }

    /// <summary>In-code page rendered through the generic page layout.</summary>
    public static PageDocument NotFoundPage()
    {
        var doc = new PageDocument
        {
            Metadata = new PageMetadata
            {
                Title = "Page not found",
                TitleHu = "Az oldal nem található",
                Slug = NotFoundSlug,
                Layout = "page",
            },
        };

        var body = new Section { Key = "body" };
        body.AddLanguage(new LanguageBlock
        {
            Language = "en",
            Markdown = "The page you are looking for does not exist or has moved.\n\n[Back to the home page](/) · [All posts](/posts/)",
        });
        body.AddLanguage(new LanguageBlock
        {
            Language = "hu",
            Markdown = "A keresett oldal nem létezik, vagy átkerült máshová.\n\n[Vissza a kezdőlapra](/) · [Összes bejegyzés](/posts/)",
        });
        doc.AddSection(body);
        return doc;
    }

    private static string Serialize(XDocument doc) =>
        doc.Declaration + "\n" + doc.Root!.ToString() + "\n";
}
