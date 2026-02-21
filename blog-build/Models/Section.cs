namespace VVidman.BlogBuild.Models;
public sealed class Section
{
    public string Key { get; init; } = string.Empty;

    /// <summary>
    /// Language code -> LanguageBlock
    /// Order does NOT matter inside a section
    /// </summary>
    public IReadOnlyDictionary<string, LanguageBlock> Languages
        => _languages;

    private readonly Dictionary<string, LanguageBlock> _languages = new();

    public void AddLanguage(LanguageBlock block)
    {
        if (_languages.ContainsKey(block.Language))
            throw new InvalidOperationException(
                $"Duplicate language '{block.Language}' in section '{Key}'");

        _languages[block.Language] = block;
    }

    public bool HasLanguage(string lang)
        => _languages.ContainsKey(lang);

    public LanguageBlock GetLanguage(string lang)
        => _languages[lang];
}