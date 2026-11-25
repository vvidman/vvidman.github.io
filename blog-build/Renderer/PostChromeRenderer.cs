using System.Text;
using VVidman.BlogBuild.Models;

namespace VVidman.BlogBuild.Renderers;

/// <summary>Markup around the article body: header block, series tree, pager, figure.</summary>
public static class PostChromeRenderer
{
    public static bool IsPost(PageDocument doc) =>
        doc.Metadata.Slug.StartsWith("posts/", StringComparison.OrdinalIgnoreCase);

    public static string Header(PageDocument doc, SeriesContext? series, int minutesEn, int minutesHu)
    {
        var m = doc.Metadata;
        var sb = new StringBuilder();
        sb.Append(IsPost(doc) ? "<header class=\"post-head\">\n" : "<div class=\"post-head\">\n");

        var pills = new List<string>();
        if (series is not null)
            pills.Add($"<span class=\"pill pill-accent\">{I18n.Encode(series.Name)} · {series.Index + 1}/{series.Parts.Count}</span>");
        pills.AddRange(m.Tags.Select(t => $"<span class=\"pill\">{I18n.Encode(t)}</span>"));
        if (pills.Count > 0)
            sb.Append("<div class=\"pills\">").Append(string.Concat(pills)).Append("</div>\n");

        sb.Append("<h1 class=\"post-title\">").Append(I18n.Text(m.Title, m.TitleHu)).Append("</h1>\n");

        if (!string.IsNullOrWhiteSpace(m.Description))
            sb.Append("<p class=\"post-lead\">").Append(I18n.Text(m.Description, m.DescriptionHu)).Append("</p>\n");

        var meta = new List<string>();
        if (m.Date is { } date)
            meta.Add(date.ToString("yyyy-MM-dd"));
        if (IsPost(doc))
        {
            meta.Add(I18n.Text($"{minutesEn} min read", $"{minutesHu} perc olvasás"));
            if (doc.Sections.Any(s => s.HasLanguage("hu")))
                meta.Add("EN / HU");
        }
        if (meta.Count > 0)
            sb.Append("<p class=\"post-meta\">").Append(string.Join(" · ", meta)).Append("</p>\n");

        sb.Append(IsPost(doc) ? "</header>" : "</div>");
        return sb.ToString();
    }

    public static string SeriesTree(SeriesContext? s)
    {
        if (s is null)
            return string.Empty;

        var sb = new StringBuilder();
        // `open` keeps it expanded without JS; site.js closes it on narrow screens only.
        sb.Append("<details class=\"series-tree\" open>\n");
        sb.Append("<summary><span class=\"st-name\">").Append(I18n.Encode(s.Name))
          .Append("</span><span class=\"st-count\">").Append(s.Index + 1).Append('/').Append(s.Parts.Count)
          .Append("</span></summary>\n");
        for (var i = 0; i < s.Parts.Count; i++)
        {
            var p = s.Parts[i];
            var number = (p.Metadata.Part ?? i + 1).ToString("00");
            var current = i == s.Index ? " aria-current=\"page\"" : "";
            sb.Append($"<a href=\"{Urls.Path(p.Metadata.Slug)}\"{current}><span class=\"n\">{number}</span>{I18n.Text(p.Metadata.Title, p.Metadata.TitleHu)}</a>\n");
        }
        sb.Append("</details>");
        return sb.ToString();
    }

    public static string Pager(SeriesContext? s)
    {
        if (s is null || (s.Previous is null && s.Next is null))
            return string.Empty;

        var sb = new StringBuilder();
        sb.Append("<nav class=\"pager\" aria-label=\"Series navigation\">\n");
        if (s.Previous is not null)
            sb.Append(Card(s.Previous, "previous", "prev", "← ", ""));
        if (s.Next is not null)
            sb.Append(Card(s.Next, "next", "next", "", " →"));
        sb.Append("</nav>");
        return sb.ToString();
    }

    private static string Card(PageDocument target, string labelKey, string rel, string before, string after)
    {
        var label = I18n.Label(labelKey);
        return $"<a href=\"{Urls.Path(target.Metadata.Slug)}\" rel=\"{rel}\"><span class=\"dir\">{before}{label}{after}</span>{I18n.Text(target.Metadata.Title, target.Metadata.TitleHu)}</a>\n";
    }

    public static string Figure(PageDocument doc)
    {
        if (string.IsNullOrWhiteSpace(doc.Metadata.Image))
            return string.Empty;

        var alt = I18n.Encode(doc.Metadata.Title + " — infographic");
        return $"<figure><img src=\"{I18n.Encode(doc.Metadata.Image)}\" alt=\"{alt}\" loading=\"lazy\" width=\"1200\" height=\"1200\"></figure>";
    }
}
