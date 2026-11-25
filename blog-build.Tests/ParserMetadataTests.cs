using VVidman.BlogBuild;

namespace BlogBuild.Tests;

public class ParserMetadataTests
{
    private static string Fixture(string name) =>
        Path.Combine(TestPaths.RepoRoot, "blog-build.Tests", "Fixtures", name);

    [Fact]
    public void Parses_new_keys()
    {
        var doc = MarkdownDslParser.Parse(Fixture("full-metadata.md"));
        Assert.Equal("Magyar leírás", doc.Metadata.DescriptionHu);
        Assert.Equal("/assets/images/infographics/x-square.png", doc.Metadata.Image);
        Assert.Equal("/assets/images/infographics/x-og.png", doc.Metadata.OgImage);
        Assert.Equal(new[] { "posts/post-02.html", "posts/old.html" }, doc.Metadata.RedirectFrom);
    }

    [Fact]
    public void Unknown_key_still_throws()
    {
        var ex = Assert.Throws<InvalidOperationException>(() => MarkdownDslParser.Parse(Fixture("unknown-key.md")));
        Assert.Contains("Unknown metadata key", ex.Message);
    }

    [Fact]
    public void Date_with_time_sets_date_and_time()
    {
        var doc = MarkdownDslParser.Parse(Fixture("date-time.md"));
        Assert.Equal(new DateOnly(2026, 11, 25), doc.Metadata.Date);
        Assert.Equal(new TimeOnly(20, 0), doc.Metadata.Time);
    }
}
