using System.Text;
using VVidman.BlogBuild.Models;
using VVidman.BlogBuild.Renderers;

namespace VVidman.BlogBuild;

public static class PostIndexGenerator
{
    public const string Slug = "posts/index.html";

    private const string Description = "Articles on .NET, software architecture, and AI-augmented development.";

    public static string Generate(SiteIndex index, TemplateEngine engine)
    {
        var body = new StringBuilder();

        foreach (var (name, parts) in index.Series)
        {
            body.Append("<section class=\"series-block\">\n<div class=\"series-head\">");
            body.Append("<h2>").Append(I18n.Encode(name)).Append("</h2>");
            body.Append("<span>").Append(I18n.Text(
                parts.Count == 1 ? "1 part" : $"{parts.Count} parts",
                $"{parts.Count} rész")).Append("</span></div>\n<ul class=\"post-list\">\n");

            for (var i = 0; i < parts.Count; i++)
                body.Append(Row(parts[i], $"#{parts[i].Metadata.Part ?? i + 1}"));

            body.Append("</ul>\n</section>\n");
        }

        var standalone = index.Posts.Where(p => string.IsNullOrWhiteSpace(p.Metadata.Series)).ToList();
        if (standalone.Count > 0)
        {
            body.Append("<section class=\"series-block\">\n<div class=\"series-head\"><h2>")
                .Append(I18n.Label("standalone")).Append("</h2></div>\n<ul class=\"post-list\">\n");
            foreach (var post in standalone)
                body.Append(Row(post, badge: null));
            body.Append("</ul>\n</section>\n");
        }

        var vars = PageRenderer.BaseVars(
            titleEn: PageRenderer.DocumentTitle("Posts"),
            titleHu: PageRenderer.DocumentTitle("Bejegyzések"),
            description: Description,
            slug: Slug,
            ogImage: "/assets/images/vv-profile.jpg",
            ogType: "website",
            jsonLd: PageRenderer.PersonJsonLd(),
            navBlog: true,
            navAbout: false);
        vars["title_heading"] = I18n.Text("Posts", "Bejegyzések");
        vars["index_body"] = body.ToString();

        return engine.Render("posts-index", vars);
    }

    private static string Row(PageDocument post, string? badge)
    {
        var m = post.Metadata;
        var sb = new StringBuilder();
        sb.Append("<li><a class=\"post-row\" href=\"").Append(Urls.Path(m.Slug)).Append("\">");

        if (badge is not null)
            sb.Append("<span class=\"part-badge\">").Append(I18n.Encode(badge)).Append("</span>");

        sb.Append("<span class=\"row-main\"><span class=\"row-title\">")
          .Append(I18n.Text(m.Title, m.TitleHu))
          .Append("</span></span>");

        var meta = new List<string>();
        if (m.Date is { } date)
            meta.Add($"<span>{date:yyyy-MM-dd}</span>");
        meta.Add("<span>" + I18n.Text(
            $"{PageRenderer.Minutes(post, "en")} min read",
            $"{PageRenderer.Minutes(post, "hu")} perc olvasás") + "</span>");

        sb.Append("<span class=\"row-meta\">").Append(string.Concat(meta)).Append("</span>");
        sb.Append("</a></li>\n");
        return sb.ToString();
    }
}
