# inferrouter-02 — InferRouter Part 2: Inference Strategies
Source: content/posts/inferrouter-02.md · Pattern: flow + 3 cards + callout (square); 3 cards + callout (og)

| Element | Text | Source |
|---|---|---|
| Eyebrow | InferRouter · Part 2 | front matter |
| Title | Inference strategies | title |
| Subtitle (square) | The strategy returns an ordered list of candidates. The execution loop does everything else. | "return an ordered list of candidates to try. Everything else — executing attempts, handling errors, logging, rate limit tracking — belongs to the execution loop." (intro) |
| Node 1 | **Strategy** — Returns an ordered list of candidate providers. | same paragraph; interface comment "Returns ordered cloud providers to attempt for this request." |
| Node 2 (key) | **Execution loop** — Executes attempts, handles errors, logs, tracks rate limits. | "executing attempts, handling errors, logging, rate limit tracking — belongs to the execution loop" |
| Node 3 | **LlamaSharp** — Local GGUF model, appended last as the guaranteed fallback. | "The local LlamaSharp model is not part of any strategy. It is the guaranteed fallback that the execution loop appends after the strategy's list is exhausted." |
| Card ChainOfResponsibility | Providers are tried in config order. The most predictable and debuggable option. | "providers are tried in the order they appear in config"; "The most predictable and debuggable option." |
| Card WeightedRoundRobin | Traffic is spread in proportion to each provider's DailyRequestLimit. | "requests are distributed across providers in proportion to their `DailyRequestLimit`" |
| Card LeastUsed | Picks the lowest DailyCount / DailyLimit ratio. Ties are random. | "selects the provider with the lowest utilisation ratio (`DailyCount / DailyLimit`)"; "Tiebreaks are random." |
| Callout (square) | The strategy never sees errors or results. | "The strategy never sees errors or results." (The Execution Loop) |
| Callout (og) | LlamaSharp is never part of a strategy — it is the guaranteed fallback. | "it is never included in strategy selection"; "It is the guaranteed fallback" |
