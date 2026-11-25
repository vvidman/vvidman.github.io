# inferrouter-04 — InferRouter Part 4: Observability
Source: content/posts/inferrouter-04.md · Pattern: log node + 3 endpoint cards + callout (square); 3 cards + callout (og)

| Element | Text | Source |
|---|---|---|
| Eyebrow | InferRouter · Part 4 | front matter |
| Title | Observability | title |
| Subtitle (square) | How do you know what is actually happening inside the running proxy? | "once the proxy is running, how do you know what is actually happening inside it?" (intro) |
| Node (key) | **Operation log** — A structured JSONL file, one per day. Events: infer_started · infer_ordering · infer_completed · infer_fallback · infer_failed · rate_limit_hit | "a structured JSONL file that captures every inference event"; "a per-day file"; "Six event types are defined: …" |
| Card GET /health/providers | Is a given provider reachable right now? | "is a given provider reachable right now" |
| Card GET /stats/live | How much quota remains for the day? | "how much quota remains for the day" |
| Card GET /stats/history | What happened over a specific day's worth of requests? | "what happened over a specific day's worth of requests" |
| Callout (square) | The log records events, not providers. | "the log records **events**, not providers" |
| Callout (og) | Every inference event goes into a provider-agnostic JSONL operation log. | intro of Part 4 + Part 3 Next section: "a provider-agnostic JSONL file that records every inference event" |
