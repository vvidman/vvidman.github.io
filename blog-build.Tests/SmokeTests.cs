using VVidman.BlogBuild;

namespace BlogBuild.Tests;

public class SmokeTests
{
    [Fact]
    public void Parses_every_existing_content_file()
    {
        var files = Directory.GetFiles(Path.Combine(TestPaths.RepoRoot, "content"), "*.md", SearchOption.AllDirectories);
        Assert.NotEmpty(files);
        foreach (var file in files)
        {
            var ex = Record.Exception(() => MarkdownDslParser.Parse(file));
            Assert.True(ex is null, $"{file}: {ex?.Message}");
        }
    }
}
