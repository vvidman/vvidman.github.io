using VVidman.BlogBuild.Models;
using VVidman.BlogBuild.Renderers;

namespace VVidman.BlogBuild;

public sealed record BuildReport(int Errors, IReadOnlyList<string> WrittenFiles);

public static class SiteBuilder
{
    public static BuildReport Build(BuildConfig cfg, ResolvedPaths paths, TextWriter? log = null)
    {
        log ??= Console.Out;
        var errors        = 0;
        var written       = new List<string>();
        var allPages      = new List<PageDocument>();

        // Fázis 1: minden .md parse-olása
        CollectPages(paths.Input, allPages, log, ref errors);

        var index  = new SiteIndex(allPages);
        var engine = new TemplateEngine(paths.Templates);

        // Fázis 2: egyedi oldalak renderelése és kiírása
        foreach (var page in allPages)
            RenderAndWrite(cfg, paths, page, index, engine, written, log, ref errors);

        // Fázis 3: posts/index.html generálása
        GeneratePostsIndex(paths, index, engine, written, log, ref errors);

        // Fázis 4: home (docs/index.html)
        GenerateHome(paths, index, engine, written, log, ref errors);

        log.WriteLine("Processing completed");
        return new BuildReport(errors, written);
    }

    private static void CollectPages(string currentPath, List<PageDocument> pages, TextWriter log, ref int errors)
    {
        foreach (var file in Directory.EnumerateFiles(currentPath, "*.md"))
        {
            try
            {
                pages.Add(MarkdownDslParser.Parse(file));
                log.WriteLine($"Parsed: {file}");
            }
            catch (Exception ex)
            {
                errors++;
                log.WriteLine($"[ERROR] Failed to parse '{file}': {ex.Message}");
            }
        }

        foreach (var dir in Directory.EnumerateDirectories(currentPath))
            CollectPages(dir, pages, log, ref errors);
    }

    private static void RenderAndWrite(
        BuildConfig cfg, ResolvedPaths paths, PageDocument page,
        SiteIndex index, TemplateEngine engine, List<string> written, TextWriter log, ref int errors)
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
            log.WriteLine($"Rendered: {page.Metadata.Slug}");
        }
        catch (Exception ex)
        {
            errors++;
            log.WriteLine($"[ERROR] Failed to render '{page.Metadata.Slug}': {ex.Message}");
        }
    }

    private static void GeneratePostsIndex(
        ResolvedPaths paths, SiteIndex index, TemplateEngine engine,
        List<string> written, TextWriter log, ref int errors)
    {
        try
        {
            var html       = PostIndexGenerator.Generate(index, engine);
            var outputPath = Path.Combine(paths.Output, "posts", "index.html");

            Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);
            File.WriteAllText(outputPath, html);
            written.Add(outputPath);
            log.WriteLine("Generated: posts/index.html");
        }
        catch (Exception ex)
        {
            errors++;
            log.WriteLine($"[ERROR] Failed to generate posts index: {ex.Message}");
        }
    }

    private static void GenerateHome(
        ResolvedPaths paths, SiteIndex index, TemplateEngine engine,
        List<string> written, TextWriter log, ref int errors)
    {
        try
        {
            var outputPath = Path.Combine(paths.Output, HomeGenerator.Slug);
            Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);
            File.WriteAllText(outputPath, HomeGenerator.Generate(index, engine));
            written.Add(outputPath);
            log.WriteLine("Generated: index.html");
        }
        catch (Exception ex)
        {
            errors++;
            log.WriteLine($"[ERROR] Failed to generate home page: {ex.Message}");
        }
    }
}
