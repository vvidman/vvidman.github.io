using VVidman.BlogBuild.Renderers;

namespace BlogBuild.Tests;

public class HelpersTests
{
    private static string Words(int n) => string.Join(' ', Enumerable.Repeat("word", n));

    [Fact]
    public void I18n_encodes_and_falls_back_to_en()
    {
        Assert.Equal("A &amp; B", I18n.Text("A & B", null));
        Assert.Equal("A", I18n.Text("A", ""));
        Assert.Equal("A", I18n.Text("A", "  "));
        Assert.Equal("A", I18n.Text("A", "A"));
    }

    [Fact]
    public void I18n_emits_both_spans_and_encodes_hu()
    {
        var html = I18n.Text("Next", "Következő <b>");
        Assert.Equal("<span data-l=\"en\">Next</span><span data-l=\"hu\">Következő &lt;b&gt;</span>", html);
    }

    [Theory]
    [InlineData("latest_posts", "Latest posts", "Legfrissebb")]
    [InlineData("all_posts", "all posts", "összes")]
    [InlineData("series", "Series", "Sorozat")]
    [InlineData("standalone", "Standalone posts", "Egyéb bejegyzések")]
    [InlineData("min_read", "min read", "perc olvasás")]
    [InlineData("previous", "Previous", "Előző")]
    [InlineData("next", "Next", "Következő")]
    [InlineData("copy", "Copy", "Másolás")]
    public void I18n_labels_match_spec(string key, string en, string hu) =>
        Assert.Equal(I18n.Text(en, hu), I18n.Label(key));

    [Fact]
    public void I18n_unknown_label_throws() =>
        Assert.Throws<ArgumentException>(() => I18n.Label("nope"));

    [Fact]
    public void ReadingTime_ignores_fenced_code_and_rounds_up()
    {
        var md = Words(441) + "\n\n```csharp\n" + Words(500) + "\n```\n";
        Assert.Equal(3, ReadingTime.Minutes(md));
    }

    [Theory]
    [InlineData("")]
    [InlineData("one two")]
    public void ReadingTime_minimum_is_one(string md) => Assert.Equal(1, ReadingTime.Minutes(md));

    [Fact]
    public void ReadingTime_exact_multiple_does_not_round_up() =>
        Assert.Equal(1, ReadingTime.Minutes(Words(220)));

    [Theory]
    [InlineData("about/index.html", "/about/")]
    [InlineData("index.html", "/")]
    [InlineData("posts/raglab-01.html", "/posts/raglab-01.html")]
    public void Urls_path(string slug, string expected) => Assert.Equal(expected, Urls.Path(slug));

    [Fact]
    public void Urls_absolute() =>
        Assert.Equal("https://vvidman.github.io/about/", Urls.Absolute("about/index.html"));
}
