using Microsoft.Extensions.Configuration;
using VVidman.BlogBuild.Models;

namespace VVidman.BlogBuild;

public class Program
{
    public static int Main(string[] args)
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false)
            .Build();

        var cfg = config.Get<BuildConfig>()
            ?? throw new InvalidOperationException("Failed to bind configuration");

        var projectDir = PathResolver.FindProjectDir(AppContext.BaseDirectory);
        var paths      = PathResolver.Resolve(cfg, projectDir);

        var report = SiteBuilder.Build(cfg, paths);
        return report.Errors > 0 ? 1 : 0;
    }
}
