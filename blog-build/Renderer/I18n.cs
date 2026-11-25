namespace VVidman.BlogBuild.Renderers;

public static class I18n
{
    private static readonly Dictionary<string, (string En, string Hu)> Labels = new()
    {
        ["latest_posts"] = ("Latest posts", "Legfrissebb"),
        ["all_posts"]    = ("all posts", "összes"),
        ["series"]       = ("Series", "Sorozat"),
        ["standalone"]   = ("Standalone posts", "Egyéb bejegyzések"),
        ["min_read"]     = ("min read", "perc olvasás"),
        ["previous"]     = ("Previous", "Előző"),
        ["next"]         = ("Next", "Következő"),
        ["copy"]         = ("Copy", "Másolás"),
    };

    /// <summary>
    /// HTML-encoded bilingual fragment. Falls back to plain English when the Hungarian
    /// text is missing or identical.
    /// </summary>
    public static string Text(string en, string? hu)
    {
        var encEn = Encode(en);
        if (string.IsNullOrWhiteSpace(hu) || hu == en)
            return encEn;

        return $"<span data-l=\"en\">{encEn}</span><span data-l=\"hu\">{Encode(hu)}</span>";
    }

    /// <summary>Encodes only the HTML-significant characters; keeps accented letters readable.</summary>
    public static string Encode(string text) => text
        .Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;")
        .Replace("\"", "&quot;").Replace("'", "&#39;");

    public static string Label(string key) =>
        Labels.TryGetValue(key, out var l)
            ? Text(l.En, l.Hu)
            : throw new ArgumentException($"Unknown label '{key}'", nameof(key));
}
