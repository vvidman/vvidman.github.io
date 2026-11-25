using VVidman.BlogBuild.Renderers;

namespace BlogBuild.Tests;

public class HighlightingTests
{
    [Fact]
    public void CSharp_keywords_get_token_class()
    {
        var html = MarkdownRenderer.Render("```csharp\npublic interface IFoo { }\n```");
        Assert.Contains("class=\"tk-kw\"", html);
        Assert.Contains("<pre", html);
    }

    [Fact]
    public void Unsupported_language_renders_plain_pre()
    {
        var html = MarkdownRenderer.Render("```bash\necho hi\n```");
        Assert.Contains("<pre", html);
        Assert.Contains("echo hi", html);
    }

    [Fact]
    public void Fence_without_language_renders_plain_pre()
    {
        var html = MarkdownRenderer.Render("```\nx\n```");
        Assert.Contains("<pre", html);
    }

    [Fact]
    public void Placeholder_text_in_code_survives()
    {
        var html = MarkdownRenderer.Render("```html\n{{content_en}}\n```");
        Assert.Contains("content_en", html);
    }
}
