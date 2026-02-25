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

            renderedByLang[lang] = sb.ToString();
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
}