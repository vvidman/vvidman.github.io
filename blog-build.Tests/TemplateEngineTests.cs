using VVidman.BlogBuild.Renderers;

namespace BlogBuild.Tests;

public sealed class TemplateEngineTests : IDisposable
{
    private readonly string _dir = Path.Combine(Path.GetTempPath(), "tpl-" + Guid.NewGuid().ToString("N"));

    public TemplateEngineTests() => Directory.CreateDirectory(Path.Combine(_dir, "partials"));
    public void Dispose() => Directory.Delete(_dir, true);

    private void Tpl(string name, string body) => File.WriteAllText(Path.Combine(_dir, name + ".html"), body);
    private void Partial(string name, string body) => File.WriteAllText(Path.Combine(_dir, "partials", "_" + name + ".html"), body);

    [Fact]
    public void Replaces_vars_and_partials()
    {
        Tpl("t", "<h>{{> nav}}</h>{{title}}");
        Partial("nav", "N{{x}}");
        var html = new TemplateEngine(_dir).Render("t", new Dictionary<string, string> { ["x"] = "1", ["title"] = "T" });
        Assert.Equal("<h>N1</h>T", html);
    }

    [Fact]
    public void Inserted_values_are_not_rescanned()
    {
        Tpl("t", "<main>{{content}}</main>{{title}}");
        var html = new TemplateEngine(_dir).Render("t", new Dictionary<string, string> { ["content"] = "{{title}} {{> nav}}", ["title"] = "T" });
        Assert.Equal("<main>{{title}} {{> nav}}</main>T", html);
    }

    [Fact]
    public void Unknown_placeholder_throws()
    {
        Tpl("t", "{{nope}}");
        var ex = Assert.Throws<TemplateException>(() => new TemplateEngine(_dir).Render("t", new Dictionary<string, string>()));
        Assert.Contains("Unknown placeholder 'nope' in template 't'", ex.Message);
    }

    [Fact]
    public void Missing_partial_throws()
    {
        Tpl("t", "{{> ghost}}");
        Assert.Throws<TemplateException>(() => new TemplateEngine(_dir).Render("t", new Dictionary<string, string>()));
    }

    [Fact]
    public void Recursive_partials_beyond_depth_5_throw()
    {
        Tpl("t", "{{> loop}}");
        Partial("loop", "{{> loop}}");
        Assert.Throws<TemplateException>(() => new TemplateEngine(_dir).Render("t", new Dictionary<string, string>()));
    }
}
