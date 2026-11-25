using System.Text;
using System.Text.RegularExpressions;
using VVidman.BlogBuild.Models;
using VVidman.BlogBuild.Renderers;

namespace VVidman.BlogBuild;

public static partial class HomeGenerator
{
    public const string Slug = "index.html";

    private const string Title = "Viktor Vidman – Software Architect · AI-augmented engineering · LLM infrastructure in .NET";

    private const string Description =
        "Viktor Vidman, software architect. How to put AI into a real engineering process without losing control: " +
        "workflows, guardrails, and the LLM infrastructure underneath, in .NET.";

    private const int LatestCount = 5;

    public static string Generate(SiteIndex index, TemplateEngine engine)
    {
        var latest = index.LatestPerSeries(LatestCount);

        var rows = new StringBuilder();
        foreach (var post in latest)
            rows.Append(Row(post, index.ContextFor(post)));

        var vars = PageRenderer.BaseVars(
            titleEn: Title,
            titleHu: Title,
            description: Description,
            slug: Slug,
            ogImage: "/assets/images/vv-profile.jpg",
            ogType: "website",
            jsonLd: PageRenderer.PersonJsonLd(),
            navBlog: false,
            navAbout: false);

        vars["latest_title"] = I18n.Encode(latest.Count > 0 ? latest[0].Metadata.Title : "");
        vars["latest_label"] = I18n.Label("latest_posts");
        vars["all_label"]    = I18n.Label("all_posts");
        vars["latest_rows"]  = rows.ToString();

        return engine.Render("home", vars);
    }

    private static string Row(PageDocument post, SeriesContext? series)
    {
        var m = post.Metadata;
        var sb = new StringBuilder();
        sb.Append("<li><a class=\"post-row\" href=\"").Append(Urls.Path(m.Slug)).Append("\">");

        if (!string.IsNullOrWhiteSpace(m.Image))
            sb.Append("<span class=\"thumb\" style=\"background-image:url('").Append(I18n.Encode(m.Image)).Append("')\" aria-hidden=\"true\"></span>");
        else
            sb.Append("<span class=\"thumb\" aria-hidden=\"true\">")
              .Append(I18n.Encode(Monogram(series?.Name ?? m.Title))).Append("</span>");

        sb.Append("<span class=\"row-main\"><span class=\"row-title\">")
          .Append(I18n.Text(m.Title, m.TitleHu)).Append("</span>");
        if (!string.IsNullOrWhiteSpace(m.Description))
            sb.Append("<span class=\"row-desc\">").Append(I18n.Text(m.Description, m.DescriptionHu)).Append("</span>");
        sb.Append("</span>");

        var meta = new List<string>();
        if (series is not null)
            meta.Add($"<span class=\"pill\">{I18n.Encode(series.Name)} · {series.Index + 1}/{series.Parts.Count}</span>");
        if (m.Date is { } date)
            meta.Add($"<span>{date:yyyy-MM-dd}</span>");
        meta.Add("<span>" + I18n.Text(
            $"{PageRenderer.Minutes(post, "en")} min read",
            $"{PageRenderer.Minutes(post, "hu")} perc olvasás") + "</span>");

        sb.Append("<span class=\"row-meta\">").Append(string.Concat(meta)).Append("</span>");
        sb.Append("</a></li>\n");
        return sb.ToString();
    }

    [GeneratedRegex(@"[A-Z]?[a-z]+|[A-Z]+(?![a-z])|\d+")]
    private static partial Regex WordRegex();

    /// <summary>Two-letter mono monogram: initials of the first two words, or the first two letters of a single word.</summary>
    internal static string Monogram(string name)
    {
        var words = WordRegex().Matches(name).Select(m => m.Value).ToList();
        if (words.Count == 0)
            return "··";

        var letters = words.Count >= 2
            ? $"{words[0][0]}{words[1][0]}"
            : words[0].Length >= 2 ? words[0][..2] : words[0];
        return letters.ToUpperInvariant();
    }
}
