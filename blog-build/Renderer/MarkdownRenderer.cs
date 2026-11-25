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

    public static string Render(string markdown)
    {
        if (string.IsNullOrWhiteSpace(markdown))
            return string.Empty;

        return Markdig.Markdown.ToHtml(markdown, Pipeline);
    }
}