using System.Net;
using System.Text;
using VVidman.BlogBuild.Models;

namespace VVidman.BlogBuild;

public static class PostIndexGenerator
{
    // Az infografika palettájából — sorozatonként automatikusan kiosztva
    private static readonly string[] SeriesPalette =
    [
        "#e8a24a", // amber
        "#4a749c", // blue
        "#bd6450", // terra
        "#6ec99a", // green
        "#7b68c8", // violet
        "#5fb3d4", // sky
    ];

    public static string Generate(
        IReadOnlyList<PageDocument> posts,
        string templateHtml,
        IReadOnlyList<string> languages)
    {
        var seriesGroups = posts
            .Where(p => !string.IsNullOrWhiteSpace(p.Metadata.Series))
            .GroupBy(p => p.Metadata.Series!, StringComparer.OrdinalIgnoreCase)
            .Select((g, i) => (
                Name:  g.Key,
                Color: SeriesPalette[i % SeriesPalette.Length],
                Posts: g.OrderBy(p => p.Metadata.Part ?? int.MaxValue).ToList()
            ))
            .ToList();

        var standalones = posts
            .Where(p => string.IsNullOrWhiteSpace(p.Metadata.Series))
            .ToList();

        var renderedByLang = new Dictionary<string, string>();
        foreach (var lang in languages)
            renderedByLang[lang] = BuildContent(seriesGroups, standalones, lang);

        return templateHtml
            .Replace("{{title}}",       "Posts")
            .Replace("{{lang}}",        languages.FirstOrDefault() ?? "en")
            .Replace("{{slug}}",        "posts/index.html")
            .Replace("{{description}}", "Articles on .NET, software architecture, and AI-augmented development.")
            .Replace("{{content_en}}",  renderedByLang.GetValueOrDefault("en", ""))
            .Replace("{{content_hu}}",  renderedByLang.GetValueOrDefault("hu", ""));
    }

    private static string BuildContent(
        IReadOnlyList<(string Name, string Color, List<PageDocument> Posts)> seriesGroups,
        IReadOnlyList<PageDocument> standalones,
        string lang)
    {
        var sb = new StringBuilder();
        var isHu = lang == "hu";

        if (seriesGroups.Count > 0)
        {
            sb.AppendLine($"<section class=\"post-index-section\">");
            sb.AppendLine($"  <h2>{(isHu ? "Sorozatok" : "Series")}</h2>");

            foreach (var (name, color, posts) in seriesGroups)
            {
                sb.AppendLine($"  <div class=\"series-group\" style=\"--series-color: {color}\">");
                sb.AppendLine($"    <div class=\"series-header\">");
                sb.AppendLine($"      <span class=\"series-stripe\"></span>");
                sb.AppendLine($"      <span class=\"series-name\">{WebUtility.HtmlEncode(name)}</span>");
                sb.AppendLine($"    </div>");
                sb.AppendLine($"    <ul class=\"series-post-list\">");

                foreach (var post in posts)
                {
                    var title = isHu && !string.IsNullOrWhiteSpace(post.Metadata.TitleHu)
                        ? post.Metadata.TitleHu
                        : post.Metadata.Title;

                    var partBadge = post.Metadata.Part.HasValue
                        ? $"<span class=\"part-badge\">#{post.Metadata.Part}</span>"
                        : "<span class=\"part-badge\"></span>";

                    sb.AppendLine($"      <li>");
                    sb.AppendLine($"        {partBadge}");
                    sb.AppendLine($"        <a href=\"/{post.Metadata.Slug}\">{WebUtility.HtmlEncode(title)}</a>");
                    sb.AppendLine($"      </li>");
                }

                sb.AppendLine($"    </ul>");
                sb.AppendLine($"  </div>");
            }

            sb.AppendLine($"</section>");
        }

        if (standalones.Count > 0)
        {
            sb.AppendLine($"<section class=\"post-index-section\">");
            sb.AppendLine($"  <h2>{(isHu ? "Egyéb bejegyzések" : "Standalone posts")}</h2>");
            sb.AppendLine($"  <ul class=\"post-list\">");

            foreach (var post in standalones)
            {
                var title = isHu && !string.IsNullOrWhiteSpace(post.Metadata.TitleHu)
                    ? post.Metadata.TitleHu
                    : post.Metadata.Title;

                sb.AppendLine($"    <li><a href=\"/{post.Metadata.Slug}\">{WebUtility.HtmlEncode(title)}</a></li>");
            }

            sb.AppendLine($"  </ul>");
            sb.AppendLine($"</section>");
        }

        return sb.ToString();
    }
}