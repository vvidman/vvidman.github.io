using System.Text.RegularExpressions;
using VVidman.BlogBuild;
using VVidman.BlogBuild.Models;

namespace BlogBuild.Tests;

/// <summary>Spec §5: post titles come from front matter only; body headings start at '##'.</summary>
public class ContentRulesTests
{
    private static IEnumerable<(string File, PageDocument Doc)> Posts() =>
        Directory.GetFiles(Path.Combine(TestPaths.RepoRoot, "content", "posts"), "*.md")
            .Select(f => (Path.GetFileName(f), MarkdownDslParser.Parse(f)));

    /// <summary>Heading levels in a markdown block, ignoring fenced code.</summary>
    private static IEnumerable<int> HeadingLevels(string markdown)
    {
        var fence = false;
        foreach (var line in markdown.Split('\n'))
        {
            if (line.TrimStart().StartsWith("```")) { fence = !fence; continue; }
            if (fence) continue;
            var m = Regex.Match(line, @"^(#{1,6})\s");
            if (m.Success) yield return m.Groups[1].Length;
        }
    }

    private static IEnumerable<(string File, string Section, string Lang, int Level)> AllHeadings() =>
        Posts().SelectMany(p => p.Doc.Sections.SelectMany(s => s.Languages.Values
            .SelectMany(b => HeadingLevels(b.Markdown).Select(l => (p.File, s.Key, b.Language, l)))));

    [Fact]
    public void No_post_body_contains_h1()
    {
        var bad = AllHeadings().Where(h => h.Level == 1).Select(h => $"{h.File} [{h.Section}/{h.Lang}]").ToList();
        Assert.True(bad.Count == 0, "H1 in body: " + string.Join(", ", bad));
    }

    [Fact]
    public void Every_post_has_description_hu()
    {
        var bad = Posts().Where(p => string.IsNullOrWhiteSpace(p.Doc.Metadata.DescriptionHu)).Select(p => p.File).ToList();
        Assert.True(bad.Count == 0, "Missing description_hu: " + string.Join(", ", bad));
    }

    [Fact]
    public void Smallest_heading_in_each_post_is_h2()
    {
        var bad = AllHeadings().GroupBy(h => h.File)
            .Where(g => g.Min(h => h.Level) != 2)
            .Select(g => $"{g.Key} (min h{g.Min(h => h.Level)})").ToList();
        Assert.True(bad.Count == 0, "Heading levels not normalised: " + string.Join(", ", bad));
    }

    [Fact]
    public void No_description_repeats_the_title()
    {
        var bad = Posts().Where(p => string.Equals(p.Doc.Metadata.Description.Trim(), p.Doc.Metadata.Title.Trim(), StringComparison.OrdinalIgnoreCase))
            .Select(p => p.File).ToList();
        Assert.True(bad.Count == 0, "Description equals title: " + string.Join(", ", bad));
    }

    [Fact]
    public void Every_post_has_a_known_topic()
    {
        var bad = Posts().Where(p => p.Doc.Metadata.Topic is null || !Topics.TryGet(p.Doc.Metadata.Topic, out _))
            .Select(p => p.File).ToList();
        Assert.True(bad.Count == 0, "Missing or unknown topic: " + string.Join(", ", bad));
    }
}
