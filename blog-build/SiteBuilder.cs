using VVidman.BlogBuild.Models;
using VVidman.BlogBuild.Renderers;

namespace VVidman.BlogBuild;

public sealed record BuildReport(int Errors, IReadOnlyList<string> WrittenFiles);

public static class SiteBuilder
{
    public static BuildReport Build(BuildConfig cfg, ResolvedPaths paths)
    {
        var errors        = 0;
        var written       = new List<string>();
        var templateCache = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        var allPages      = new List<PageDocument>();

        // Fázis 1: minden .md parse-olása
        CollectPages(paths.Input, allPages, ref errors);

        var index  = new SiteIndex(allPages);
        var engine = new TemplateEngine(paths.Templates);

        // Fázis 2: egyedi oldalak renderelése és kiírása
        foreach (var page in allPages)
            RenderAndWrite(cfg, paths, page, index, engine, written, ref errors);

        // Fázis 3: posts/index.html generálása
        GeneratePostsIndex(cfg, paths, allPages, templateCache, written, ref errors);

        Console.WriteLine("Processing completed");
        return new BuildReport(errors, written);
    }

    private static string Template(ResolvedPaths paths, Dictionary<string, string> cache, string name)
    {
        if (!cache.TryGetValue(name, out var html))
        {
            html = File.ReadAllText(Path.Combine(paths.Templates, $"{name}.html"));
            cache[name] = html;
        }
        return html;
    }

    private static void CollectPages(string currentPath, List<PageDocument> pages, ref int errors)
    {
        foreach (var file in Directory.EnumerateFiles(currentPath, "*.md"))
        {
            try
            {
                pages.Add(MarkdownDslParser.Parse(file));
                Console.WriteLine($"Parsed: {file}");
            }
            catch (Exception ex)
            {
                errors++;
                Console.Error.WriteLine($"[ERROR] Failed to parse '{file}': {ex.Message}");
            }
        }

        foreach (var dir in Directory.EnumerateDirectories(currentPath))
            CollectPages(dir, pages, ref errors);
    }

    private static void RenderAndWrite(
        BuildConfig cfg, ResolvedPaths paths, PageDocument page,
        SiteIndex index, TemplateEngine engine, List<string> written, ref int errors)
    {
        try
        {
            var html = PageRenderer.Render(page, index, engine, cfg.Defaults.Languages);

            var pagePath = Path.Combine(paths.Output, page.Metadata.Slug);
            var pageDir  = Path.GetDirectoryName(pagePath)
                ?? throw new InvalidOperationException(
                    $"Cannot determine output directory for slug: '{page.Metadata.Slug}'");

            Directory.CreateDirectory(pageDir);
            File.WriteAllText(pagePath, html);
            written.Add(pagePath);
            Console.WriteLine($"Rendered: {page.Metadata.Slug}");
        }
        catch (Exception ex)
        {
            errors++;
            Console.Error.WriteLine($"[ERROR] Failed to render '{page.Metadata.Slug}': {ex.Message}");
        }
    }

    private static void GeneratePostsIndex(
        BuildConfig cfg, ResolvedPaths paths, List<PageDocument> allPages,
        Dictionary<string, string> templateCache, List<string> written, ref int errors)
    {
        try
        {
            // Csak a posts/ slug-ú oldalak kerülnek be az indexbe
            var posts = allPages
                .Where(p => p.Metadata.Slug.StartsWith("posts/", StringComparison.OrdinalIgnoreCase))
                .ToList();

            var html       = PostIndexGenerator.Generate(posts, Template(paths, templateCache, "posts-index"), cfg.Defaults.Languages);
            var outputPath = Path.Combine(paths.Output, "posts", "index.html");

            Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);
            File.WriteAllText(outputPath, html);
            written.Add(outputPath);
            Console.WriteLine("Generated: posts/index.html");
        }
        catch (Exception ex)
        {
            errors++;
            Console.Error.WriteLine($"[ERROR] Failed to generate posts index: {ex.Message}");
        }
    }
}
