namespace VVidman.BlogBuild.Models;

public sealed class BuildConfig
{
    public PathConfig Paths { get; init; } = new();
    public DefaultConfig Defaults { get; init; } = new();
}

public sealed class PathConfig
{
    public string Input { get; init; } = string.Empty;
    public string Templates { get; init; } = string.Empty;
    public string Output { get; init; } = string.Empty;
}

public sealed class DefaultConfig
{
    public IReadOnlyList<string> Languages { get; init; } = Array.Empty<string>();
    public string Layout { get; init; } = "page";
}