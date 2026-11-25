namespace VVidman.BlogBuild.Renderers;

public static class ReadingTime
{
    private const double WordsPerMinute = 220.0;

    /// <summary>Words outside fenced code blocks / 220, rounded up, minimum 1.</summary>
    public static int Minutes(string markdown)
    {
        var words = 0;
        var inFence = false;

        foreach (var line in markdown.Split('\n'))
        {
            if (line.TrimStart().StartsWith("```"))
            {
                inFence = !inFence;
                continue;
            }
            if (inFence)
                continue;

            words += line.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries).Length;
        }

        return Math.Max(1, (int)Math.Ceiling(words / WordsPerMinute));
    }
}
