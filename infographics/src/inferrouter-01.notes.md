# inferrouter-01 — InferRouter: One Endpoint, Any Provider
Source: content/posts/inferrouter-01.md · Pattern: flow + fallback node + 3 cards + callout (square); flow + callout (og)

| Element | Text on infographic | Source in post |
|---|---|---|
| Eyebrow | InferRouter · Part 1 | front matter `series: InferRouter`, `part: 1` |
| Title | One endpoint, any provider | title "InferRouter: One Endpoint, Any Provider" |
| Subtitle (square) | A single self-hosted proxy with strategy-based inference routing. | description: "a single self-hosted proxy with strategy-based inference routing solves it" |
| Node 1 | **Callers** — Speak the OpenAI chat completions API they already use. | "Callers speak the OpenAI chat completions API — the same interface they already use." (The Solution Shape) |
| Node 2 (key) | **InferRouter** — One OpenAI-compatible endpoint. A configurable strategy routes each request. | "a self-hosted proxy that exposes a single OpenAI-compatible HTTP endpoint. Behind it, a configurable inference strategy routes each request across one or more cloud providers." (intro) |
| Node 3 | **Cloud providers** — One or more. Any OpenAI-compatible HTTP endpoint qualifies. | "The chain can hold one provider or many"; "an OpenAI-compatible HTTP endpoint and a valid config block. Any cloud inference service that speaks the protocol qualifies." |
| Fallback node (square) | **Local model · LlamaSharp** — Runs in-process. Handles the call when all providers are unavailable or exhausted. | "When all of them are unavailable or exhausted, a local quantized model — running in-process via LlamaSharp — handles the call." |
| Card 1 | **API keys scattered** — No single place to rotate a key or audit access. | "No single place to rotate a key, no single place to audit access." (What Was Actually Broken) |
| Card 2 | **Routing logic reimplemented** — Writing it once per project means maintaining it N times. | "Writing it once per project means maintaining it N times." |
| Card 3 | **Provider changes cascade** — Swapping a provider meant touching every project that used it. | "Swapping a provider, adjusting a rate limit, or pointing to a new endpoint meant touching every project that used it." |
| Callout (square) | Provider management is infrastructure. Infrastructure belongs in one place. | same two sentences, What Was Actually Broken |
| Callout (og) | When every provider is unavailable or exhausted, a local model handles the call. | same sentence as Fallback node (intro) |
