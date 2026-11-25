using global::Markdown.ColorCode;
using Markdig;

namespace VVidman.BlogBuild.Renderers;

public static class MarkdownRenderer
{
    private static readonly MarkdownPipeline Pipeline =
        new MarkdownPipelineBuilder()
            .UseAdvancedExtensions()
            .UseColorCode(HtmlFormatterType.Css, BlogCodeStyles.Create())
            .Build();

    // The ColorCode extension keeps per-renderer writer state; two builds rendering at the same
    // time (parallel test classes) otherwise hit "Cannot write to a closed TextWriter".
    private static readonly object Gate = new();

    public static string Render(string markdown)
    {
        if (string.IsNullOrWhiteSpace(markdown))
            return string.Empty;

        lock (Gate)
            return Markdig.Markdown.ToHtml(markdown, Pipeline);
    }
}