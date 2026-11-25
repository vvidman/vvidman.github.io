using VVidman.BlogBuild.Models;

namespace VVidman.BlogBuild;

public static class PathResolver
{
    /// <summary>Walks up from <paramref name="startDir"/> to the folder containing BlogBuild.csproj.</summary>
    public static string FindProjectDir(string startDir)
    {
        var dir = new DirectoryInfo(Path.GetFullPath(startDir));
        while (dir is not null)
        {
            if (File.Exists(Path.Combine(dir.FullName, "BlogBuild.csproj")))
                return dir.FullName;
            dir = dir.Parent;
        }
        throw new DirectoryNotFoundException($"BlogBuild.csproj not found above '{startDir}'.");
    }

    public static ResolvedPaths Resolve(BuildConfig cfg, string projectDir) => new(
        projectDir,
        Full(projectDir, cfg.Paths.Input),
        Full(projectDir, cfg.Paths.Templates),
        Full(projectDir, cfg.Paths.Output));

    private static string Full(string baseDir, string relative) =>
        Path.GetFullPath(Path.Combine(baseDir, relative));
}
