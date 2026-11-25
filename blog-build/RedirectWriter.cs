using VVidman.BlogBuild.Renderers;

namespace VVidman.BlogBuild;

/// <summary>Renders the redirect stub written at an old slug (spec §7).</summary>
public static class RedirectWriter
{
    public static string Render(TemplateEngine engine, string targetSlug) =>
        engine.Render("redirect", new Dictionary<string, string>
        {
            ["target_path"] = I18n.Encode(Urls.Path(targetSlug)),
            ["target_url"]  = I18n.Encode(Urls.Absolute(targetSlug)),
        });
}
