namespace VVidman.BlogBuild.Models;
public sealed class PageMetadata
{
    public string Title { get; init; } = string.Empty;
    public string TitleHu  { get; init; } = string.Empty;
    public string Slug { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string Layout { get; init; } = "page";
    public DateOnly? Date { get; init; }
    public IReadOnlyList<string> Tags { get; init; } = Array.Empty<string>();
    public string? Series { get; init; }   // pl. "ChaosForge"
    public int?    Part   { get; init; }   // pl. 1
}
