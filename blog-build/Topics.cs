namespace VVidman.BlogBuild;

public sealed record Topic(string Key, string NameEn, string NameHu, string BlurbEn, string BlurbHu);

/// <summary>Fixed registry of post topics, in display order.</summary>
public static class Topics
{
    public static IReadOnlyList<Topic> All { get; } = new[]
    {
        new Topic("ai-workflow",
            "AI-augmented workflow", "AI-támogatott munkafolyamat",
            "How to work with AI without losing control.", "Hogyan dolgozz AI-jal úgy, hogy az irányítás nálad marad."),
        new Topic("llm-infra",
            "LLM infrastructure in .NET", "LLM infrastruktúra .NET-ben",
            "What runs underneath: routing, retrieval, observability.", "Ami alatta fut: routing, keresés, megfigyelhetőség."),
        new Topic("tooling",
            "Engineering tools", "Mérnöki eszközök",
            "The tools I build and run my work with.", "Az eszközök, amikkel építek és dolgozom."),
    };

    public static bool TryGet(string key, out Topic topic)
    {
        var found = All.FirstOrDefault(t => string.Equals(t.Key, key?.Trim(), StringComparison.OrdinalIgnoreCase));
        topic = found!;
        return found is not null;
    }
}
