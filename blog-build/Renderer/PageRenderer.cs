using System.Text;
using System.Text.Json;
using VVidman.BlogBuild.Models;

namespace VVidman.BlogBuild.Renderers;

public static class PageRenderer
{
    private const string SiteName = "Viktor Vidman";
    private const string DefaultOgImage = "/assets/images/vv-profile.jpg";
    private const string CurrentPage = " aria-current=\"page\"";

    /// <summary>Browser-tab / social title: "Viktor Vidman – {title}".</summary>
    public static string DocumentTitle(string title) => $"{SiteName} – {title}";

    public static string Render(
        PageDocument doc,
        SiteIndex index,
        TemplateEngine engine,
        IReadOnlyList<string> languages)
    {
        var m = doc.Metadata;
        var isPost = PostChromeRenderer.IsPost(doc);
        var series = isPost ? index.ContextFor(doc) : null;

        var rendered = new Dictionary<string, string>();
        foreach (var lang in languages)
        {
            var sb = new StringBuilder();
            foreach (var section in doc.Sections)
                sb.AppendLine(SectionRenderer.RenderForLanguage(section, lang));

            // {{ }} inside rendered content must never look like template syntax.
            rendered[lang] = EscapeTemplateLiterals(sb.ToString());
        }

        var titleEn = DocumentTitle(m.Title);
        var titleHu = DocumentTitle(string.IsNullOrWhiteSpace(m.TitleHu) ? m.Title : m.TitleHu);
        var ogImage = string.IsNullOrWhiteSpace(m.OgImage) ? DefaultOgImage : m.OgImage;

        var vars = new Dictionary<string, string>
        {
            ["title_en"]     = I18n.Encode(titleEn),
            ["title_hu"]     = I18n.Encode(titleHu),
            ["description"]  = I18n.Encode(m.Description),
            ["canonical"]    = Urls.Absolute(m.Slug),
            ["og_image"]     = AbsoluteUrl(ogImage),
            ["og_type"]      = isPost ? "article" : "website",
            ["json_ld"]      = JsonLd(doc, ogImage),
            ["nav_blog"]     = isPost ? CurrentPage : "",
            ["nav_about"]    = m.Slug.StartsWith("about/", StringComparison.OrdinalIgnoreCase) ? CurrentPage : "",
            ["year"]         = DateTime.UtcNow.Year.ToString(),
            ["layout_class"] = series is not null ? "post-layout has-series" : isPost ? "post-layout" : "page-layout",
            ["post_header"]  = PostChromeRenderer.Header(doc, series, Minutes(doc, "en"), Minutes(doc, "hu")),
            ["series_tree"]  = PostChromeRenderer.SeriesTree(series),
            ["figure"]       = PostChromeRenderer.Figure(doc),
            ["pager"]        = PostChromeRenderer.Pager(series),
            ["content_en"]   = rendered.GetValueOrDefault("en", ""),
            ["content_hu"]   = rendered.GetValueOrDefault("hu", ""),
        };

        var layout = string.IsNullOrWhiteSpace(m.Layout) ? "page" : m.Layout;
        return engine.Render(layout, vars);
    }

    internal static string AbsoluteUrl(string pathOrUrl) =>
        pathOrUrl.StartsWith("http", StringComparison.OrdinalIgnoreCase) ? pathOrUrl : Urls.Origin + pathOrUrl;

    private static int Minutes(PageDocument doc, string lang) =>
        ReadingTime.Minutes(string.Join("\n\n", doc.Sections
            .Select(s => SectionRenderer.ResolveBlock(s, lang)?.Markdown)
            .Where(md => md is not null)));

    private static string JsonLd(PageDocument doc, string ogImage)
    {
        var m = doc.Metadata;
        Dictionary<string, object> data;

        if (PostChromeRenderer.IsPost(doc))
        {
            data = new()
            {
                ["@context"] = "https://schema.org",
                ["@type"] = "BlogPosting",
                ["headline"] = m.Title,
                ["author"] = new Dictionary<string, object> { ["@type"] = "Person", ["name"] = SiteName },
                ["url"] = Urls.Absolute(m.Slug),
                ["image"] = AbsoluteUrl(ogImage),
            };
            if (m.Date is { } d)
                data["datePublished"] = d.ToString("yyyy-MM-dd");
        }
        else
        {
            data = new()
            {
                ["@context"] = "https://schema.org",
                ["@type"] = "Person",
                ["name"] = SiteName,
                ["url"] = Urls.Origin,
                ["jobTitle"] = "Software Architect",
                ["knowsAbout"] = new[] { "C#", ".NET", "Backend Architecture", "AI-Augmented Development", "Human-in-the-Loop Workflows", "Multi-Agent Systems", "Software Design" },
                ["sameAs"] = new[] { "https://github.com/vvidman", "https://www.linkedin.com/in/viktor-vidman-55a2a197" },
            };
        }

        // The default encoder escapes '<', '>' and '&', so the JSON cannot close the <script> element.
        return JsonSerializer.Serialize(data);
    }

    /// <summary>
    /// Escapes the double-brace sequences in rendered HTML content so they can never be
    /// mistaken for template placeholders. Browsers render the entities as the braces.
    /// </summary>
    private static string EscapeTemplateLiterals(string renderedHtml)
        => renderedHtml
            .Replace("{{", "&#123;&#123;")
            .Replace("}}", "&#125;&#125;");
}
