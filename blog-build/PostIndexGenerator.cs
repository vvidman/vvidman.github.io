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
        // Series are assigned to a topic through their parts; all parts must agree.
        var seriesByTopic = new Dictionary<string, List<(string Name, IReadOnlyList<PageDocument> Parts)>>();
        foreach (var series in index.Series)
        {
            var keys = series.Parts.Select(p => TopicKey(p)).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
            if (keys.Count > 1)
                throw new InvalidOperationException(
                    $"Series '{series.Name}' mixes topics: {string.Join(", ", keys.Select(k => k.Length == 0 ? "(none)" : k))}. All parts of a series must share one topic.");

            if (!seriesByTopic.TryGetValue(keys[0], out var list))
                seriesByTopic[keys[0]] = list = new();
            list.Add(series);
        }

        var standaloneByTopic = index.Posts
            .Where(p => string.IsNullOrWhiteSpace(p.Metadata.Series))
            .GroupBy(p => TopicKey(p), StringComparer.OrdinalIgnoreCase)
            .ToDictionary(g => g.Key, g => g.ToList(), StringComparer.OrdinalIgnoreCase);

        var sections = new List<(string Key, string Name, string? Blurb, string Content)>();
        foreach (var topic in Topics.All)
        {
            var content = TopicContent(topic.Key, seriesByTopic, standaloneByTopic);
            if (content.Length > 0)
                sections.Add((topic.Key, I18n.Text(topic.NameEn, topic.NameHu), I18n.Text(topic.BlurbEn, topic.BlurbHu), content));
        }
        var other = TopicContent("", seriesByTopic, standaloneByTopic);
        if (other.Length > 0)
            sections.Add(("other", I18n.Text("Other", "Egyéb"), null, other));

        var body = new StringBuilder();
        body.Append("<nav class=\"topic-nav\" aria-label=\"Topics\">");
        foreach (var sec in sections)
            body.Append("<a href=\"#").Append(sec.Key).Append("\">").Append(sec.Name).Append("</a>");
        body.Append("</nav>\n");

        foreach (var sec in sections)
        {
            body.Append("<section class=\"topic-block\" id=\"").Append(sec.Key).Append("\">\n");
            body.Append("<header class=\"topic-head\"><h2>").Append(sec.Name).Append("</h2>");
            if (sec.Blurb is not null)
                body.Append("<p class=\"topic-blurb\">").Append(sec.Blurb).Append("</p>");
            body.Append("</header>\n").Append(sec.Content).Append("</section>\n");
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

    private static string TopicKey(PageDocument post) => post.Metadata.Topic?.Trim() ?? "";

    private static string TopicContent(
        string key,
        Dictionary<string, List<(string Name, IReadOnlyList<PageDocument> Parts)>> seriesByTopic,
        Dictionary<string, List<PageDocument>> standaloneByTopic)
    {
        var sb = new StringBuilder();

        if (seriesByTopic.TryGetValue(key, out var seriesList))
            foreach (var (name, parts) in seriesList)
            {
                sb.Append("<section class=\"series-block\">\n<div class=\"series-head\">");
                sb.Append("<h2>").Append(I18n.Encode(name)).Append("</h2>");
                sb.Append("<span>").Append(I18n.Text(
                    parts.Count == 1 ? "1 part" : $"{parts.Count} parts",
                    $"{parts.Count} rész")).Append("</span></div>\n<ul class=\"post-list\">\n");

                for (var i = 0; i < parts.Count; i++)
                    sb.Append(Row(parts[i], $"#{parts[i].Metadata.Part ?? i + 1}"));

                sb.Append("</ul>\n</section>\n");
            }

        if (standaloneByTopic.TryGetValue(key, out var standalone))
        {
            sb.Append("<ul class=\"post-list\">\n");
            foreach (var post in standalone)
                sb.Append(Row(post, badge: null));
            sb.Append("</ul>\n");
        }

        return sb.ToString();
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
