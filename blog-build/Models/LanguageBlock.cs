namespace VVidman.BlogBuild.Models;

public sealed class LanguageBlock
{
    public string Language { get; init; } = string.Empty;

    /// <summary>
    /// Raw markdown content inside the lang block
    /// </summary>
    public string Markdown { get; init; } = string.Empty;
}