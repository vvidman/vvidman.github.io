using VVidman.BlogBuild.Models;

namespace VVidman.BlogBuild;

public sealed record SeriesContext(
    string Name,
    IReadOnlyList<PageDocument> Parts,
    int Index,
    PageDocument? Previous,
    PageDocument? Next);

public sealed class SiteIndex
{
    public IReadOnlyList<PageDocument> Posts { get; }
    public IReadOnlyList<(string Name, IReadOnlyList<PageDocument> Parts)> Series { get; }

    public SiteIndex(IReadOnlyList<PageDocument> pages)
    {
        Posts = pages
            .Where(p => p.Metadata.Slug.StartsWith("posts/", StringComparison.OrdinalIgnoreCase))
            .OrderBy(p => p.Metadata.Date is null)
            .ThenByDescending(p => p.Metadata.Date)
            .ThenByDescending(p => p.Metadata.Part)
            .ThenBy(p => p.Metadata.Title, StringComparer.OrdinalIgnoreCase)
            .ToList();

        Series = Posts
            .Where(p => !string.IsNullOrWhiteSpace(p.Metadata.Series))
            .GroupBy(p => p.Metadata.Series!.Trim(), StringComparer.OrdinalIgnoreCase)
            .Select(g =>
            {
                var parts = (IReadOnlyList<PageDocument>)g
                    .OrderBy(p => p.Metadata.Part is null)
                    .ThenBy(p => p.Metadata.Part)
                    .ThenBy(p => p.Metadata.Date)
                    .ToList();
                // Display name: spelling used by the first part
                return (Name: parts[0].Metadata.Series!.Trim(), Parts: parts, Newest: g.Max(p => p.Metadata.Date));
            })
            .OrderBy(s => s.Newest is null)
            .ThenByDescending(s => s.Newest)
            .Select(s => (s.Name, s.Parts))
            .ToList();
    }

    public IReadOnlyList<PageDocument> Latest(int count) =>
        Posts.Where(p => p.Metadata.Date is not null).Take(count).ToList();

    /// <summary>Newest dated posts, at most one per series (its newest part); standalone posts always count.</summary>
    public IReadOnlyList<PageDocument> LatestPerSeries(int count)
    {
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        return Posts
            .Where(p => p.Metadata.Date is not null)
            .Where(p => string.IsNullOrWhiteSpace(p.Metadata.Series) || seen.Add(p.Metadata.Series.Trim()))
            .Take(count)
            .ToList();
    }

    public SeriesContext? ContextFor(PageDocument post)
    {
        var name = post.Metadata.Series;
        if (string.IsNullOrWhiteSpace(name))
            return null;

        foreach (var (seriesName, parts) in Series)
        {
            if (!string.Equals(seriesName, name.Trim(), StringComparison.OrdinalIgnoreCase))
                continue;

            var idx = -1;
            for (var i = 0; i < parts.Count; i++)
                if (ReferenceEquals(parts[i], post)) { idx = i; break; }
            if (idx < 0)
                return null;

            return new SeriesContext(
                seriesName, parts, idx,
                idx > 0 ? parts[idx - 1] : null,
                idx < parts.Count - 1 ? parts[idx + 1] : null);
        }
        return null;
    }
}
