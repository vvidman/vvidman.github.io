using Microsoft.Extensions.Configuration;
using VVidman.BlogBuild.Models;
using VVidman.BlogBuild.Renderers;

namespace VVidman.BlogBuild;

public class Program
{
    private static BuildConfig s_Config;

    public static void Main(string[] args)
    {
        #region First we read the config

        // crafting the configuration builder
        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false)
            .Build();

        s_Config = config.Get<BuildConfig>()
                    ?? throw new InvalidOperationException("Failed to bind configuration");
        #endregion

        // Process markdowns, starting from the input root folder
        ScanDirectory(s_Config.Paths.Input);

        Console.WriteLine("Processing completed");
    }

    /// <summary>
    /// Iterate through the folders and search for markdowns
    /// </summary>
    /// <param name="currentPath">Top level folder</param>
    private static void ScanDirectory(string currentPath)
    {
        // search for markdowns on the same level
        foreach (var file in Directory.EnumerateFiles(currentPath, "*.md"))
        {
            ProcessMarkdownFile(file);
        }

        // iterate the sub-level folders
        foreach (var dir in Directory.EnumerateDirectories(currentPath))
        {
            ScanDirectory(dir);
        }
    }

    private static void ProcessMarkdownFile(string filePath)
    {
        // ---- load template ----
        var templatePath = Path.Combine(s_Config.Paths.Templates, $"{s_Config.Defaults.Layout}.html");
        var templateHtml = File.ReadAllText(templatePath);

        // ---- parse markdown ----        
        var page = MarkdownDslParser.Parse(filePath);

        // ---- render ----
        var html = PageRenderer.Render(page, templateHtml, s_Config.Defaults.Languages);        

        // ---- write output ----
        string mdPath = Path.GetDirectoryName(filePath);
        string relativePath = Path.GetRelativePath(s_Config.Paths.Input, mdPath);  
        var outputDir = Path.Combine(s_Config.Paths.Output, relativePath);
        
        Directory.CreateDirectory(outputDir);

        File.WriteAllText(Path.Combine(outputDir, $"{page.Metadata.Slug}.html"), html);             

        Console.WriteLine($"Markdown processed: {filePath}");
    }    
}