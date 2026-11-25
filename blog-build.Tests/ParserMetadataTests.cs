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
}
