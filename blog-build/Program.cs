using Microsoft.Extensions.Configuration;
using VVidman.BlogBuild.Models;
using VVidman.BlogBuild.Renderers;

namespace VVidman.BlogBuild;

public class Program
{
    private static BuildConfig s_Config = null!;

    public static void Main(string[] args)
    {
        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false)
            .Build();

        s_Config = config.Get<BuildConfig>()
            ?? throw new InvalidOperationException("Failed to bind configuration");

        var templateCache = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        var allPages      = new List<PageDocument>();

        // Fázis 1: minden .md parse-olása
        CollectPages(s_Config.Paths.Input, allPages);

        // Fázis 2: egyedi oldalak renderelése és kiírása
        foreach (var page in allPages)
            RenderAndWrite(page, templateCache);

        // Fázis 3: posts/index.html generálása
        GeneratePostsIndex(allPages, templateCache);

        Console.WriteLine("Processing completed");
    }

    private static void CollectPages(string currentPath, List<PageDocument> pages)
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
                Console.Error.WriteLine($"[ERROR] Failed to parse '{file}': {ex.Message}");
            }
        }

        foreach (var dir in Directory.EnumerateDirectories(currentPath))
            CollectPages(dir, pages);
    }

    private static void RenderAndWrite(PageDocument page, Dictionary<string, string> templateCache)
    {
        try
        {
            var layoutName = string.IsNullOrWhiteSpace(page.Metadata.Layout)
                ? s_Config.Defaults.Layout
                : page.Metadata.Layout;

            if (!templateCache.TryGetValue(layoutName, out var templateHtml))
            {
                var templatePath = Path.Combine(s_Config.Paths.Templates, $"{layoutName}.html");
                templateHtml = File.ReadAllText(templatePath);
                templateCache[layoutName] = templateHtml;
            }

            var html = PageRenderer.Render(page, templateHtml, s_Config.Defaults.Languages);

            var pagePath = Path.Combine(s_Config.Paths.Output, page.Metadata.Slug);
            var pageDir  = Path.GetDirectoryName(pagePath)
                ?? throw new InvalidOperationException(
                    $"Cannot determine output directory for slug: '{page.Metadata.Slug}'");

            Directory.CreateDirectory(pageDir);
            File.WriteAllText(pagePath, html);
            Console.WriteLine($"Rendered: {page.Metadata.Slug}");
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"[ERROR] Failed to render '{page.Metadata.Slug}': {ex.Message}");
        }
    }

    private static void GeneratePostsIndex(
        List<PageDocument> allPages,
        Dictionary<string, string> templateCache)
    {
        try
        {
            // Csak a posts/ slug-ú oldalak kerülnek be az indexbe
            var posts = allPages
                .Where(p => p.Metadata.Slug.StartsWith("posts/", StringComparison.OrdinalIgnoreCase))
                .ToList();

            const string indexLayout = "posts-index";
            if (!templateCache.TryGetValue(indexLayout, out var templateHtml))
            {
                var templatePath = Path.Combine(s_Config.Paths.Templates, $"{indexLayout}.html");
                templateHtml = File.ReadAllText(templatePath);
                templateCache[indexLayout] = templateHtml;
            }

            var html       = PostIndexGenerator.Generate(posts, templateHtml, s_Config.Defaults.Languages);
            var outputPath = Path.Combine(s_Config.Paths.Output, "posts", "index.html");

            Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);
            File.WriteAllText(outputPath, html);
            Console.WriteLine("Generated: posts/index.html");
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"[ERROR] Failed to generate posts index: {ex.Message}");
        }
    }
}