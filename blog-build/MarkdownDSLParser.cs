using System.Text;
using VVidman.BlogBuild.Models;

namespace VVidman.BlogBuild;

public static class MarkdownDslParser
{
    /// <summary>
    /// Parse markdown DSL file and return the parsed <see cref="PageDocument"/> instance.
    /// </summary>
    /// <param name="filePath">The markdown DSL file relative path</param>
    /// <returns></returns>
    /// <exception cref="FileNotFoundException">Occures when the markdown file does not exist</exception>
    /// <exception cref="InvalidOperationException">Occures when the parser found unexpected content</exception>
    public static PageDocument Parse(string filePath)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException(filePath);

        var lines = File.ReadAllLines(filePath);
        var index = 0;

        // --- Parse metadata ---
        var metadata = ParseMetadata(lines, ref index);

        var document = new PageDocument
        {
            Metadata = metadata
        };

        // --- Parse sections ---
        while (index < lines.Length)
        {
            if (IsEmpty(lines[index]))
            {
                index++;
                continue;
            }

            if (IsSectionStart(lines[index], out var sectionKey))
            {
                var section = ParseSection(lines, ref index, sectionKey);
                document.AddSection(section);
                continue;
            }

            throw new InvalidOperationException(
                $"Unexpected content at line {index + 1}: {lines[index]}");
        }

        return document;
    }

    // ============================
    // Metadata
    // ============================

    private static PageMetadata ParseMetadata(string[] lines, ref int index)
    {
        if (index >= lines.Length || lines[index].Trim() != "---")
            throw new InvalidOperationException("Missing front-matter start '---'");

        index++; // skip '---'

        string title = "", slug = "", layout = "";

        while (index < lines.Length && lines[index].Trim() != "---")
        {
            var line = lines[index].Trim();
            index++;

            if (string.IsNullOrWhiteSpace(line))
                continue;

            var parts = line.Split(':', 2);
            if (parts.Length != 2)
                throw new InvalidOperationException($"Invalid metadata line: {line}");

            var key = parts[0].Trim();
            var value = parts[1].Trim();

            if(key == "title")
                title = value;
            else if (key == "slug")
                slug = value;
            else if (key == "layout")
                layout = value;
            else 
                throw new InvalidOperationException($"Unknown metadata key: {key}");
        }

        if (index >= lines.Length || lines[index].Trim() != "---")
            throw new InvalidOperationException("Missing front-matter end '---'");

        index++; // skip closing '---'
        return new() { Title = title, Layout = "", Slug = slug };
    }

    // ============================
    // Section
    // ============================

    private static Section ParseSection(
        string[] lines,
        ref int index,
        string sectionKey)
    {
        var section = new Section
        {
            Key = sectionKey
        };

        index++; // skip :::section <key>

        while (index < lines.Length)
        {
            if (IsSectionEnd(lines[index]))
            {
                index++; // skip :::section
                return section;
            }

            if (IsEmpty(lines[index]))
            {
                index++;
                continue;
            }

            if (IsLangStart(lines[index], out var lang))
            {
                var block = ParseLanguageBlock(lines, ref index, lang);
                section.AddLanguage(block);
                continue;
            }

            throw new InvalidOperationException(
                $"Unexpected content in section '{sectionKey}' at line {index + 1}");
        }

        throw new InvalidOperationException(
            $"Section '{sectionKey}' was not closed with :::section");
    }

    // ============================
    // Language block
    // ============================

    private static LanguageBlock ParseLanguageBlock(
        string[] lines,
        ref int index,
        string lang)
    {
        index++; // skip :::lang <code>

        var sb = new StringBuilder();

        while (index < lines.Length)
        {
            if (IsLangEnd(lines[index]))
            {
                index++; // skip :::lang
                return new LanguageBlock
                {
                    Language = lang,
                    Markdown = sb.ToString().TrimEnd()
                };
            }

            sb.AppendLine(lines[index]);
            index++;
        }

        throw new InvalidOperationException(
            $"Language block '{lang}' was not closed with :::lang");
    }

    // ============================
    // Helpers
    // ============================

    private static bool IsSectionStart(string line, out string key)
    {
        line = line.Trim();
        key = string.Empty;

        if (!line.StartsWith(":::section "))
            return false;

        key = line.Substring(":::section ".Length).Trim();
        if (string.IsNullOrEmpty(key))
            throw new InvalidOperationException("Section key cannot be empty");

        return true;
    }

    private static bool IsSectionEnd(string line)
        => line.Trim() == ":::section";

    private static bool IsLangStart(string line, out string lang)
    {
        line = line.Trim();
        lang = string.Empty;

        if (!line.StartsWith(":::lang "))
            return false;

        lang = line.Substring(":::lang ".Length).Trim();
        if (string.IsNullOrEmpty(lang))
            throw new InvalidOperationException("Language code cannot be empty");

        return true;
    }

    private static bool IsLangEnd(string line)
        => line.Trim() == ":::lang";

    private static bool IsEmpty(string line)
        => string.IsNullOrWhiteSpace(line);
}