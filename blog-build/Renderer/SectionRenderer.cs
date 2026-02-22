using VVidman.BlogBuild.Models;

namespace VVidman.BlogBuild.Renderers;

public static class SectionRenderer
{
    public static string RenderForLanguage(Section section, string language)
    {
        if (!section.HasLanguage(language))
            return string.Empty;

        var block = section.GetLanguage(language);
        var html = MarkdownRenderer.Render(block.Markdown);

        return $"""
        <section class="page-section" data-key="{section.Key}">
          {html}
        </section>
        """;
    }
}