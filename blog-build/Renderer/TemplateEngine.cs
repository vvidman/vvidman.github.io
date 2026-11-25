using System.Collections.Concurrent;
using System.Text.RegularExpressions;

namespace VVidman.BlogBuild.Renderers;

public sealed class TemplateException(string message) : Exception(message);

/// <summary>
/// Minimal template engine: <c>{{> name}}</c> partials (expanded first, max depth 5),
/// then a single <c>{{var}}</c> substitution pass. Substituted values are never rescanned.
/// </summary>
public sealed partial class TemplateEngine(string templatesDir)
{
    private const int MaxPartialDepth = 5;

    private readonly ConcurrentDictionary<string, string> _cache = new();

    [GeneratedRegex(@"\{\{>\s*([a-zA-Z0-9_-]+)\s*\}\}")]
    private static partial Regex PartialRegex();

    [GeneratedRegex(@"\{\{\s*([a-z0-9_]+)\s*\}\}")]
    private static partial Regex VarRegex();

    public string Render(string templateName, IReadOnlyDictionary<string, string> vars)
    {
        var expanded = ExpandPartials(Load(Path.Combine(templatesDir, templateName + ".html"), templateName), 0);

        return VarRegex().Replace(expanded, m =>
        {
            var name = m.Groups[1].Value;
            return vars.TryGetValue(name, out var value)
                ? value
                : throw new TemplateException($"Unknown placeholder '{name}' in template '{templateName}'");
        });
    }

    private string ExpandPartials(string text, int depth)
    {
        if (!PartialRegex().IsMatch(text))
            return text;

        if (depth >= MaxPartialDepth)
            throw new TemplateException($"Partial nesting deeper than {MaxPartialDepth} levels");

        return PartialRegex().Replace(text, m =>
        {
            var name = m.Groups[1].Value;
            var partial = Load(Path.Combine(templatesDir, "partials", $"_{name}.html"), $"partial '{name}'");
            return ExpandPartials(partial, depth + 1);
        });
    }

    private string Load(string path, string label) =>
        _cache.GetOrAdd(path, p => File.Exists(p)
            ? File.ReadAllText(p)
            : throw new TemplateException($"Template file not found for {label}: {p}"));
}
