namespace VVidman.BlogBuild.Renderers;

public static class Urls
{
    public const string Origin = "https://vvidman.github.io";

    public static string Path(string slug)
    {
        var path = "/" + slug.TrimStart('/');
        return path.EndsWith("/index.html", StringComparison.Ordinal)
            ? path[..^"index.html".Length]
            : path;
    }

    public static string Absolute(string slug) => Origin + Path(slug);
}
