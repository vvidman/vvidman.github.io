# sourcerag-01 — VCS as Proof Store
Source: content/posts/sourcerag-01.md (front matter: series SourceRAG, part 2) · Pattern: compare + flow + callout (square); compare + callout (og)

| Element | Text | Source |
|---|---|---|
| Eyebrow | SourceRAG · Part 2 | front matter |
| Title | VCS as proof store | title "VCS as Proof Store" |
| Subtitle (square) | Why duplicate source content into a proof database when source control already tracks everything? | description |
| Compare head 1 | Traditional RAG over a codebase | "Traditional RAG over a codebase looks roughly like this" |
| Compare 1, item 1 (og: same) | Two copies of everything | intro: "Two copies of everything, an ongoing sync obligation, and a proof store you have to maintain yourself." |
| Compare 1, item 2 | An ongoing sync obligation | same sentence |
| Compare 1, item 3 | A proof store you have to maintain yourself | same sentence |
| Compare head 2 (key) | SourceRAG | — |
| Compare 2, item 1 (og: "No source content outside the repository") | No source content is stored outside the repository. | ADR-002: "no source content is stored outside the repository" |
| Compare 2, item 2 (og: "Vector store: embeddings and metadata only") | The vector store holds only embeddings and metadata. | "The vector store holds only embeddings and metadata." |
| Compare 2, item 3 (og: "Content reconstructed on demand from the VCS") | File content is reconstructed on demand from the VCS. | "actual file content is reconstructed on-demand from the VCS using a revision reference" |
| Flow 1 | **Revision + file path** — Taken from the vector store payload. | "At query time, the system takes `revision` and `file_path`" |
| Flow 2 (key) | **VCS call** — Asked for that revision of the file. | "calls the VCS" |
| Flow 3 | **Exact content** — As it existed at that commit. | "reconstructs the exact content as it existed at that commit" |
| Callout (square) | The proof isn't a database row — it's a commit in your repository's history. | "The proof isn't a database row — it's a commit in your repository's history." |
| Callout (og) | Every answer is traceable to a specific revision, its author and its commit message. | "every answer is traceable not just to a file, but to a specific revision of that file, to the author of that revision, and to the commit message" |
