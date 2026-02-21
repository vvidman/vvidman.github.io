namespace VVidman.BlogBuild.Models;

public sealed class PageDocument
{
    public PageMetadata Metadata { get; init; } = new();

    /// <summary>
    /// Sections in document order
    /// </summary>
    public IReadOnlyList<Section> Sections => _sections;

    private readonly List<Section> _sections = new();

    public void AddSection(Section section)
    {
        if (_sections.Any(s => s.Key == section.Key))
            throw new InvalidOperationException(
                $"Duplicate section key: {section.Key}");

        _sections.Add(section);
    }
}