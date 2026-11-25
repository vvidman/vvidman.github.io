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

        var vars = BaseVars(
            titleEn: DocumentTitle(m.Title),
            titleHu: DocumentTitle(string.IsNullOrWhiteSpace(m.TitleHu) ? m.Title : m.TitleHu),
            description: m.Description,
            slug: m.Slug,
            ogImage: string.IsNullOrWhiteSpace(m.OgImage) ? DefaultOgImage : m.OgImage,
            ogType: isPost ? "article" : "website",
            jsonLd: JsonLd(doc, string.IsNullOrWhiteSpace(m.OgImage) ? DefaultOgImage : m.OgImage),
            navBlog: isPost,
            navAbout: m.Slug.StartsWith("about/", StringComparison.OrdinalIgnoreCase));

        vars["layout_class"] = series is not null ? "post-layout has-series" : isPost ? "post-layout" : "page-layout";
        vars["post_header"]  = PostChromeRenderer.Header(doc, series, Minutes(doc, "en"), Minutes(doc, "hu"));
        vars["series_tree"]  = PostChromeRenderer.SeriesTree(series);
        vars["figure"]       = PostChromeRenderer.Figure(doc);
        vars["pager"]        = PostChromeRenderer.Pager(series);
        vars["content_en"]   = rendered.GetValueOrDefault("en", "");
        vars["content_hu"]   = rendered.GetValueOrDefault("hu", "");

        var layout = string.IsNullOrWhiteSpace(m.Layout) ? "page" : m.Layout;
        return engine.Render(layout, vars);
    }

    /// <summary>Variables every full page needs for the head, header and footer partials.</summary>
    internal static Dictionary<string, string> BaseVars(
        string titleEn, string titleHu, string description, string slug,
        string ogImage, string ogType, string jsonLd, bool navBlog, bool navAbout) => new()
    {
        ["title_en"]    = I18n.Encode(titleEn),
        ["title_hu"]    = I18n.Encode(titleHu),
        ["description"] = I18n.Encode(description),
        ["canonical"]   = Urls.Absolute(slug),
        ["og_image"]    = AbsoluteUrl(ogImage),
        ["og_type"]     = ogType,
        ["json_ld"]     = jsonLd,
        ["nav_blog"]    = navBlog ? CurrentPage : "",
        ["nav_about"]   = navAbout ? CurrentPage : "",
        ["year"]        = DateTime.UtcNow.Year.ToString(),
    };

    internal static string AbsoluteUrl(string pathOrUrl) =>
        pathOrUrl.StartsWith("http", StringComparison.OrdinalIgnoreCase) ? pathOrUrl : Urls.Origin + pathOrUrl;

    internal static int Minutes(PageDocument doc, string lang) =>
        ReadingTime.Minutes(string.Join("\n\n", doc.Sections
            .Select(s => SectionRenderer.ResolveBlock(s, lang)?.Markdown)
            .Where(md => md is not null)));

    internal static string PersonJsonLd() => JsonSerializer.Serialize(new Dictionary<string, object>
    {
        ["@context"] = "https://schema.org",
        ["@type"] = "Person",
        ["name"] = SiteName,
        ["url"] = Urls.Origin,
        ["jobTitle"] = "Software Architect",
        ["knowsAbout"] = new[] { "C#", ".NET", "Software Architecture", "AI-Augmented Engineering", "LLM Infrastructure", "Retrieval-Augmented Generation", "Human-in-the-Loop Workflows" },
        ["sameAs"] = new[] { "https://github.com/vvidman", "https://www.linkedin.com/in/viktor-vidman-55a2a197" },
    });

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
            return PersonJsonLd();

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
