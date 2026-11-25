using System.Text.RegularExpressions;
using VVidman.BlogBuild;

namespace BlogBuild.Tests;

public class HomeTests : IClassFixture<SiteFixture>
{
    private readonly SiteFixture _site;
    public HomeTests(SiteFixture site) => _site = site;

    private string Home => _site.Read("index.html");

    [Fact]
    public void Hero_copy_is_present_in_both_languages()
    {
        Assert.Contains("The human orchestrates.", Home);
        Assert.Contains("The AI executes.", Home);
        Assert.Contains("Az ember orchestrál.", Home);
        Assert.Contains("Az AI végrehajt.", Home);
        Assert.Contains("Read the blog →", Home);
        Assert.Contains("Irány a blog →", Home);
        Assert.Contains("$ whoami", Home);
    }

    [Fact]
    public void Shows_profile_panel_with_latest_post_title()
    {
        Assert.Contains("profile.json", Home);
        Assert.Contains("\"Software Architect\"", Home);
        var firstTitle = Regex.Match(Home, "class=\"row-title\"><span data-l=\"en\">([^<]*)</span>|class=\"row-title\">([^<]*)<").Groups;
        var title = firstTitle[1].Success ? firstTitle[1].Value : firstTitle[2].Value;
        Assert.Contains($"<span class=\"s\">\"{title}\"</span>", Home);
    }

    [Fact]
    public void Lists_exactly_five_rows_newest_first()
    {
        Assert.Equal(5, Regex.Matches(Home, "class=\"post-row\"").Count);

        var dates = new Dictionary<string, DateOnly>();
        foreach (var file in Directory.GetFiles(Path.Combine(TestPaths.RepoRoot, "content", "posts"), "*.md"))
        {
            var m = MarkdownDslParser.Parse(file).Metadata;
            if (m.Date is { } d) dates["/" + m.Slug] = d;
        }

        var listed = Regex.Matches(Home, "class=\"post-row\" href=\"([^\"]+)\"").Select(m => m.Groups[1].Value).ToList();
        var listedDates = listed.Select(h => dates[h]).ToList();
        Assert.Equal(listedDates.OrderByDescending(d => d), listedDates);
    }

    [Fact]
    public void Each_series_appears_once_with_its_newest_part()
    {
        var rows = Regex.Matches(Home, "class=\"post-row\" href=\"([^\"]+)\"").Select(m => m.Groups[1].Value).ToList();
        Assert.Equal(rows.Count, rows.Distinct().Count());
        Assert.Single(rows, r => r.Contains("inferrouter"));
        Assert.Contains("/posts/inferrouter-04.html", rows);
        Assert.Contains("InferRouter · 4/4", Home);
        Assert.DoesNotContain("InferRouter · 1/4", Home);
    }

    [Theory]
    [InlineData("InferRouter", "IR")]
    [InlineData("Scaffold Protocol", "SP")]
    [InlineData(".NET Agentic AI", "NA")]
    [InlineData("ChaosForge", "CF")]
    [InlineData("Homelab", "HO")]
    public void Monogram(string name, string expected) => Assert.Equal(expected, HomeGenerator.Monogram(name));

    [Fact]
    public void Profile_latest_is_newest_post()
    {
        Assert.Contains("<span class=\"s\">\"The Model Register Is a Living Document\"</span>", Home);
    }

    [Fact]
    public void Nav_has_no_current_page_on_home() => Assert.DoesNotContain("aria-current", Home);

    private const string ThesisEn = "How to put AI into a real engineering process without losing control: workflows, guardrails, and the infrastructure underneath.";
    private const string ThesisHu = "Hogyan kerül az AI egy valós fejlesztési folyamatba úgy, hogy közben nem veszítjük el az irányítást: workflow-k, korlátok és az alattuk lévő infrastruktúra.";
    private const string Focus = "AI-augmented engineering · LLM infrastructure in .NET";

    [Fact]
    public void Lead_is_the_thesis_in_both_languages()
    {
        Assert.Contains($"<p class=\"lead\"><span data-l=\"en\">{ThesisEn}</span><span data-l=\"hu\">{ThesisHu}</span></p>", Home);
        Assert.DoesNotContain("16 years in .NET. I write about", Home);
        Assert.DoesNotContain("16 éve .NET-ben. AI-támogatott fejlesztésről", Home);
    }

    [Fact]
    public void Focus_line_sits_under_whoami()
    {
        var whoami = Home.IndexOf("$ whoami", StringComparison.Ordinal);
        var line = Home.IndexOf($"<p class=\"focus-line\">Software Architect · {Focus}</p>", StringComparison.Ordinal);
        Assert.True(whoami > 0 && line > whoami);
        Assert.True(line < Home.IndexOf("<h1>", StringComparison.Ordinal));
    }

    [Fact]
    public void Profile_panel_has_focus_after_role()
    {
        var role = Home.IndexOf("\"role\"", StringComparison.Ordinal);
        var focus = Home.IndexOf("\"focus\"", StringComparison.Ordinal);
        var years = Home.IndexOf("\"years\"", StringComparison.Ordinal);
        Assert.True(role > 0 && role < focus && focus < years);
        Assert.Contains("<span class=\"s\">\"AI-augmented engineering\"</span>, <span class=\"s\">\"LLM infrastructure in .NET\"</span>", Home);
    }

    [Fact]
    public void Title_and_description_carry_the_positioning()
    {
        const string title = "Viktor Vidman – Software Architect · AI-augmented engineering · LLM infrastructure in .NET";
        Assert.Contains($"<title>{title}</title>", Home);
        Assert.Contains($"<meta property=\"og:title\" content=\"{title}\">", Home);
        Assert.Contains("content=\"Viktor Vidman, software architect. How to put AI into a real engineering process without losing control: workflows, guardrails, and the LLM infrastructure underneath, in .NET.\"", Home);
    }

    [Fact]
    public void Json_ld_knows_about_the_new_topics()
    {
        Assert.Contains("\"knowsAbout\":[\"C#\",\".NET\",\"Software Architecture\",\"AI-Augmented Engineering\",\"LLM Infrastructure\",\"Retrieval-Augmented Generation\",\"Human-in-the-Loop Workflows\"]", Home);
    }
}
