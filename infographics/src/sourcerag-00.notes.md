# sourcerag-00 — Making a 2M-line legacy codebase queryable by AI
Source: content/posts/sourcerag-00.md (front matter: series SourceRAG, part 1) · Pattern: flow + 3 rationale cards + callout (square); compact flow + callout (og)

| Element | Text | Source |
|---|---|---|
| Eyebrow | SourceRAG · Part 1 | front matter |
| Title | Making a 2M-line legacy codebase queryable by AI | title |
| Subtitle (square) | Designed and presented internally, never implemented. | "I designed it, presented it internally, and it was never implemented." |
| Node 1 | **CI/CD hook** — Triggered when a build completes. | "When a build completes, a CI/CD hook… triggers the Indexer Service." (The design) |
| Node 2 | **Indexer Service** — Processes changed source files and generates vector embeddings. | "The Indexer processes changed source files, generates vector embeddings, and updates two stores" |
| Node 3 | **Qdrant + SQLite Manifest DB** — Semantic search, and index state. | "Qdrant for semantic search, and a SQLite Manifest DB that tracks index state" |
| Node 4 (key) | **Retrieval API** — ASP.NET Core. Accepts queries from any AI tool. | "an ASP.NET Core service that accepts queries from any AI tool and returns relevant code context" |
| Card Qdrant | Purpose-built for vector similarity search, Docker-deployable, no cloud dependency. | Component rationale: "purpose-built for vector similarity search, Docker-deployable, with no cloud dependency" |
| Card SQLite Manifest DB | A lightweight, file-based tracking layer. No separate database server. | "a lightweight, file-based tracking layer"; "No separate database server, no operational overhead." |
| Card Tool-agnostic Retrieval API | Decouples every consumer from the storage layer. | "decouples every consumer from the storage layer" |
| Callout | Does the proof store need to be built — or does it already exist? | "Does the proof store need to be built — or does it already exist?" (One question left open) |
