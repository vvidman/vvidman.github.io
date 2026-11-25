using VVidman.BlogBuild;

namespace BlogBuild.Tests;

public class TopicTests : IClassFixture<BadTopicSiteFixture>, IClassFixture<NoTopicSiteFixture>
{
    private readonly BadTopicSiteFixture _bad;
    private readonly NoTopicSiteFixture _none;

    public TopicTests(BadTopicSiteFixture bad, NoTopicSiteFixture none) { _bad = bad; _none = none; }

    [Fact]
    public void Parser_reads_topic()
    {
        var doc = MarkdownDslParser.Parse(Path.Combine(TestPaths.RepoRoot, "blog-build.Tests", "Fixtures", "topic.md"));
        Assert.Equal("llm-infra", doc.Metadata.Topic);
    }

    [Fact]
    public void Registry_has_three_topics_in_order() =>
        Assert.Equal(new[] { "ai-workflow", "llm-infra", "tooling" }, Topics.All.Select(t => t.Key));

    [Fact]
    public void TryGet_finds_known_and_rejects_unknown()
    {
        Assert.True(Topics.TryGet("tooling", out var t));
        Assert.Equal("Engineering tools", t.NameEn);
        Assert.False(Topics.TryGet("nonsense", out _));
    }

    [Fact]
    public void Unknown_topic_is_a_build_error_naming_the_file()
    {
        Assert.Equal(1, _bad.Report.Errors);
        Assert.Contains("Unknown topic 'nonsense' in", _bad.Log);
        Assert.Contains("bad.md", _bad.Log);
    }

    [Fact]
    public void Missing_topic_warns_but_does_not_fail()
    {
        Assert.Equal(0, _none.Report.Errors);
        Assert.Contains("[WARN]", _none.Log);
        Assert.Contains("none.md", _none.Log);
    }
}
