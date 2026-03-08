using System.Text;
using VVidman.BlogBuild.Models;

namespace VVidman.BlogBuild.Renderers;

public static class PageRenderer
{
    public static string Render(
        PageDocument document,
        string templateHtml,
        IReadOnlyList<string> languages)
    {
        var renderedByLang = new Dictionary<string, string>();

        foreach (var lang in languages)
        {
            var sb = new StringBuilder();

            foreach (var section in document.Sections)
            {
                sb.AppendLine(
                    SectionRenderer.RenderForLanguage(section, lang));
            }

            // Escape any {{ }} sequences in rendered content before
            // injecting into the template, to prevent accidental
            // placeholder substitution inside code examples.
            renderedByLang[lang] = EscapeTemplateLiterals(sb.ToString());
        }

        var defaultLang = languages.FirstOrDefault() ?? "en";

        return templateHtml
            .Replace("{{title}}", document.Metadata.Title)
            .Replace("{{lang}}", defaultLang)
            .Replace("{{slug}}", document.Metadata.Slug)
            .Replace("{{description}}", document.Metadata.Description)
            .Replace("{{content_en}}", renderedByLang.GetValueOrDefault("en", ""))
            .Replace("{{content_hu}}", renderedByLang.GetValueOrDefault("hu", ""));
    }

    /// <summary>
    /// Escapes {{ and }} in rendered HTML content to prevent accidental
    /// template placeholder substitution inside code blocks.
    /// The browser renders &#123; and &#125; as { and } respectively.
    /// </summary>
    private static string EscapeTemplateLiterals(string renderedHtml)
        => renderedHtml
            .Replace("{{", "&#123;&#123;")
            .Replace("}}", "&#125;&#125;");
}