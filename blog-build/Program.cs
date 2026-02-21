using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;
using Markdig;
using VVidman.BlogBuild.Models;

// crafting the configuration builder
var config = new ConfigurationBuilder()
    .AddJsonFile(
        Path.Combine(
            Directory.GetParent(AppContext.BaseDirectory)!.FullName,
            "appsettings.json"),
        optional: false)
    .Build();

// parsing the configuration
var buildConfig = config.Get<BuildConfig>()
    ?? throw new InvalidOperationException("Failed to bind configuration");


var source = File.ReadAllText(buildConfig.Paths.Input);

// --- Front matter ---
var frontMatterMatch = Regex.Match(
    source,
    @"^---\s*(.*?)\s*---\s*(.*)$",
    RegexOptions.Singleline);

var frontMatter = frontMatterMatch.Groups[1].Value;
var body = frontMatterMatch.Groups[2].Value;

string GetMeta(string key) =>
    Regex.Match(frontMatter, $@"{key}:\s*(.+)").Groups[1].Value.Trim();

var title = GetMeta("title");

// --- Language blocks ---
string ExtractLang(string markdown, string lang)
{
    var match = Regex.Match(
        markdown,
        $@":::lang {lang}\s*(.*?)\s*:::",
        RegexOptions.Singleline);

    return match.Success ? match.Groups[1].Value : "";
}

var enMd = ExtractLang(body, "en");
var huMd = ExtractLang(body, "hu");

// --- Markdown to HTML ---
var pipeline = new MarkdownPipelineBuilder()
    .UseAdvancedExtensions()
    .Build();

var enHtml = Markdown.ToHtml(enMd, pipeline);
var huHtml = Markdown.ToHtml(huMd, pipeline);

// --- Two-column layout ---
var combinedContent = $@"
<div class=""lang-grid"">
  <div class=""lang-col"">
    <h2>English</h2>
    {enHtml}
  </div>
  <div class=""lang-col"">
    <h2>Magyar</h2>
    {huHtml}
  </div>
</div>
";

// --- Apply template ---
var template = File.ReadAllText(buildConfig.Paths.Templates);

var finalHtml = template
    .Replace("{{title}}", title)
    .Replace("{{lang}}", "en")
    .Replace("{{content}}", combinedContent);

// --- Write output ---
Directory.CreateDirectory(Path.GetDirectoryName(buildConfig.Paths.Output)!);
File.WriteAllText(buildConfig.Paths.Output, finalHtml);

Console.WriteLine("✔ About page generated.");