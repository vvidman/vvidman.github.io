# raglab-01 — Building a RAG Pipeline from Scratch
Source: content/posts/raglab-01.md · Merges the three old images (two flows + five components). Pattern: two compact flows + 5 component cards + callout (square); two compact flows + callout (og)

| Element | Text | Source |
|---|---|---|
| Eyebrow | RagLab · Part 1 | front matter |
| Title | Building a RAG pipeline from scratch | title |
| Subtitle (square) | Every component written from scratch, every decision made consciously. | "Every component written from scratch, every decision made consciously." (Problem statement) |
| Label 1 | Indexing — runs once, or whenever your documents change | "**Indexing** runs once, or whenever your documents change." (The Two Flows) |
| Flow 1 | File → Loader → Chunker → Embedder → Vector store | code block: "File → Loader → Document → Chunker → Chunks → Embedder → Vectors → Vector Store" (intermediate data nouns left out) |
| Label 2 | Querying — runs for every question | "**Querying** runs for every question a user asks." |
| Flow 2 | Question → Embedder → Search (top chunks) → Generator → Answer | code block: "Question → Embedder → Vector → Search → Top Chunks → Generator → Answer" |
| Embedder nodes (key) | accent border in both flows | "the embedder appears in both flows… one of the most important architectural constraints" |
| Card Loader | Reads a file into a clean, normalized Document. | "read a file and produce a clean, normalized internal representation"; "a `Document` with content and metadata" |
| Card Chunker | Splits a document into manageable pieces. | "The chunker splits a document into manageable pieces." |
| Card Embedder | Turns text into a vector that encodes its meaning. | "takes a piece of text and produces a list of numbers — a vector… the *meaning* of the text" |
| Card Vector store | Given a query vector, finds the closest stored vectors. | "answers one question: given a query vector, which stored vectors are closest to it?" |
| Card Generator | The only component that involves a generative language model. | "The generator is the only component that involves a generative language model." |
| Callout | The same embedding model must be used during indexing and querying. | "The same embedding model must be used during indexing and querying." |
