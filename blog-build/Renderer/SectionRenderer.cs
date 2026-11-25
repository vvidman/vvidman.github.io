using VVidman.BlogBuild.Models;

namespace VVidman.BlogBuild.Renderers;

public static class SectionRenderer
{
    public const string FallbackLanguage = "en";

    /// <summary>The block shown for <paramref name="language"/>: its own, else the English one, else null.</summary>
    public static LanguageBlock? ResolveBlock(Section section, string language)
    {
        if (section.HasLanguage(language))
            return section.GetLanguage(language);

        return section.HasLanguage(FallbackLanguage) ? section.GetLanguage(FallbackLanguage) : null;
    }

    public static string RenderForLanguage(Section section, string language)
    {
        var block = ResolveBlock(section, language);
        if (block is null)
            return string.Empty;

        var html = MarkdownRenderer.Render(block.Markdown);

        return $"""
        <section class="page-section" data-key="{section.Key}">
          {html}
        </section>
        """;
    }
}
