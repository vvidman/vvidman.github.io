using System.Text.RegularExpressions;

namespace BlogBuild.Tests;

/// <summary>
/// A single missing '}' makes the browser silently drop every later rule,
/// so the stylesheet is checked structurally.
/// </summary>
public class CssSanityTests
{
    private static readonly string CssPath = Path.Combine(TestPaths.RepoRoot, "docs", "assets", "css", "site.css");

    // Class names of the Task 7 contract (templates and renderers rely on them).
    private static readonly string[] ContractClasses =
    {
        "wrap", "wrap-wide", "wrap-narrow", "site-header", "brand", "nav", "toggles", "seg", "icon-btn", "site-footer",
        "hero", "hero-text", "prompt", "accent", "lead", "cta", "btn", "btn-primary",
        "panel", "panel-head", "panel-body", "k", "s", "n",
        "post-list", "post-row", "thumb", "row-main", "row-title", "row-desc", "row-meta", "pill",
        "post-layout", "series-tree", "article", "post-head", "pills", "pill-accent",
        "post-title", "post-lead", "post-meta", "prose", "pager", "dir",
        "code-head", "tk-kw", "tk-ty", "tk-str", "tk-num", "tk-com", "tk-fn",
        "series-block", "series-head", "part-badge",
        "topic-block", "topic-head", "topic-blurb", "topic-nav",
    };

    internal static string StripComments(string css) => Regex.Replace(css, @"/\*.*?\*/", "", RegexOptions.Singleline);

    internal static bool BracesBalanced(string css, out string problem)
    {
        var depth = 0;
        var line = 1;
        foreach (var c in StripComments(css))
        {
            if (c == '\n') line++;
            if (c == '{') depth++;
            else if (c == '}' && --depth < 0)
            {
                problem = $"unmatched '}}' near line {line}";
                return false;
            }
        }
        problem = depth == 0 ? "" : $"{depth} unclosed '{{' at end of file";
        return depth == 0;
    }

    /// <summary>Selector text = everything before each '{' (also covers selectors nested in @media).</summary>
    internal static HashSet<string> DefinedClasses(string css)
    {
        var set = new HashSet<string>();
        foreach (Match prelude in Regex.Matches(StripComments(css), @"([^{}]+)\{"))
            foreach (Match cls in Regex.Matches(prelude.Groups[1].Value, @"\.([A-Za-z_][\w-]*)"))
                set.Add(cls.Groups[1].Value);
        return set;
    }

    [Fact]
    public void Braces_are_balanced()
    {
        Assert.True(BracesBalanced(File.ReadAllText(CssPath), out var problem), problem);
    }

    [Fact]
    public void Every_contract_class_is_defined()
    {
        var defined = DefinedClasses(File.ReadAllText(CssPath));
        var missing = ContractClasses.Where(c => !defined.Contains(c)).ToList();
        Assert.True(missing.Count == 0, "Missing selectors: " + string.Join(", ", missing));
    }

    [Fact]
    public void Brace_check_detects_a_missing_closing_brace()
    {
        const string broken = ".a { color: red;\n.b { color: blue; }\n";
        Assert.False(BracesBalanced(broken, out var problem));
        Assert.Contains("unclosed", problem);
        Assert.False(BracesBalanced("}", out _));
        Assert.True(BracesBalanced("/* { */ .a { x: y; }", out _));
    }
}
