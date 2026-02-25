namespace VVidman.BlogBuild.Models;
public sealed class PageMetadata
{
    public string Title { get; init; } = string.Empty;
    public string Slug { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string Layout { get; init; } = "page";
}
