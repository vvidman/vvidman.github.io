using VVidman.BlogBuild;
using VVidman.BlogBuild.Models;

namespace BlogBuild.Tests;

public class SiteIndexTests
{
    private static PageDocument Page(string slug, string title, string? date = null, string? series = null, int? part = null, string? time = null) =>
        new()
        {
            Metadata = new PageMetadata
            {
                Slug = slug,
                Title = title,
                Date = date is null ? null : DateOnly.Parse(date),
                Time = time is null ? null : TimeOnly.Parse(time),
                Series = series,
                Part = part
            }
        };

    private static (SiteIndex Index, PageDocument[] Ir) Build()
    {
        var ir = new[]
        {
            Page("posts/ir-3.html", "IR 3", "2026-03-03", "InferRouter", 3),
            Page("posts/ir-1.html", "IR 1", "2026-03-01", "InferRouter", 1),
            Page("posts/ir-4.html", "IR 4", "2026-03-04", "inferrouter", 4),
            Page("posts/ir-2.html", "IR 2", "2026-03-02", "InferRouter", 2),
        };
        var pages = new List<PageDocument>(ir)
        {
            Page("posts/solo.html", "Solo", "2026-01-01"),
            Page("posts/undated.html", "Undated"),
            Page("about/index.html", "About"),
        };
        pages.Add(Page("posts/sp.html", "SP", "2025-12-01", "Scaffold", 1));
        return (new SiteIndex(pages), ir);
    }

    [Fact]
    public void Context_for_middle_part()
    {
        var (index, _) = Build();
        var post = index.Posts.Single(p => p.Metadata.Title == "IR 2");
        var ctx = index.ContextFor(post)!;
        Assert.Equal(1, ctx.Index);
        Assert.Equal("IR 1", ctx.Previous!.Metadata.Title);
        Assert.Equal("IR 3", ctx.Next!.Metadata.Title);
        Assert.Equal(4, ctx.Parts.Count);
        Assert.Equal("InferRouter", ctx.Name);
    }

    [Fact]
    public void First_part_has_no_previous_last_has_no_next()
    {
        var (index, _) = Build();
        Assert.Null(index.ContextFor(index.Posts.Single(p => p.Metadata.Title == "IR 1"))!.Previous);
        Assert.Null(index.ContextFor(index.Posts.Single(p => p.Metadata.Title == "IR 4"))!.Next);
    }

    [Fact]
    public void Standalone_post_has_no_context()
    {
        var (index, _) = Build();
        Assert.Null(index.ContextFor(index.Posts.Single(p => p.Metadata.Title == "Solo")));
    }

    [Fact]
    public void Single_part_series_has_no_context()
    {
        var (index, _) = Build();
        Assert.Null(index.ContextFor(index.Posts.Single(p => p.Metadata.Title == "SP")));
        Assert.Contains(index.Series, s => s.Name == "Scaffold");
    }

    [Fact]
    public void Latest_skips_undated_and_orders_newest_first()
    {
        var (index, _) = Build();
        Assert.Equal(new[] { "IR 4", "IR 3" }, index.Latest(2).Select(p => p.Metadata.Title));
        Assert.DoesNotContain(index.Latest(100), p => p.Metadata.Title == "Undated");
    }

    [Fact]
    public void Posts_exclude_non_post_pages_and_put_undated_last()
    {
        var (index, _) = Build();
        Assert.DoesNotContain(index.Posts, p => p.Metadata.Slug.StartsWith("about/"));
        Assert.Equal("Undated", index.Posts[^1].Metadata.Title);
    }

    [Fact]
    public void Series_grouped_case_insensitively_and_ordered_by_newest()
    {
        var (index, _) = Build();
        Assert.Equal(new[] { "InferRouter", "Scaffold" }, index.Series.Select(s => s.Name));
        Assert.Equal(new[] { 1, 2, 3, 4 }, index.Series[0].Parts.Select(p => p.Metadata.Part!.Value));
    }

    [Fact]
    public void Same_date_orders_by_part_descending_then_title()
    {
        var pages = new List<PageDocument>
        {
            Page("posts/a1.html", "A1", "2026-06-03", "A", 1),
            Page("posts/a3.html", "A3", "2026-06-03", "A", 3),
            Page("posts/a2.html", "A2", "2026-06-03", "A", 2),
            Page("posts/zz.html", "ZZ", "2026-06-03"),
            Page("posts/aa.html", "AA", "2026-06-03"),
        };
        var index = new SiteIndex(pages);
        Assert.Equal(new[] { "A3", "A2", "A1", "AA", "ZZ" }, index.Latest(5).Select(p => p.Metadata.Title));
    }

    [Fact]
    public void LatestPerSeries_keeps_only_newest_part_of_each_series()
    {
        var pages = new List<PageDocument>
        {
            Page("posts/ir-1.html", "IR 1", "2026-06-03", "InferRouter", 1),
            Page("posts/ir-4.html", "IR 4", "2026-06-03", "inferrouter", 4),
            Page("posts/ir-2.html", "IR 2", "2026-06-03", "InferRouter", 2),
            Page("posts/solo.html", "Solo", "2026-05-01"),
            Page("posts/sp.html", "SP", "2026-04-01", "Scaffold", 1),
            Page("posts/undated.html", "Undated"),
        };
        var index = new SiteIndex(pages);
        Assert.Equal(new[] { "IR 4", "Solo", "SP" }, index.LatestPerSeries(10).Select(p => p.Metadata.Title));
        Assert.Equal(new[] { "IR 4", "Solo" }, index.LatestPerSeries(2).Select(p => p.Metadata.Title));
    }

    [Fact]
    public void Same_day_posts_are_ordered_by_publish_time()
    {
        var index = new SiteIndex(new[]
        {
            Page("posts/g-3.html", "Series 3", "2026-11-25", "Governing", 3),
            Page("posts/solo.html", "Solo", "2026-11-25", time: "20:00"),
            Page("posts/g-2.html", "Series 2", "2026-11-25", "Governing", 2),
        });
        Assert.Equal(new[] { "Solo", "Series 3", "Series 2" }, index.Posts.Select(p => p.Metadata.Title));
        Assert.Equal("Solo", index.LatestPerSeries(5)[0].Metadata.Title);
    }
}
