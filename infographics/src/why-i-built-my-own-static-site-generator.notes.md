# why-i-built-my-own-static-site-generator — Why I built my own static site generator — not because the others are bad
Source: content/posts/post_siteGenerator.md (slug `posts/why-i-built-my-own-static-site-generator.html`, series Site Generator, part 1) · Pattern: flow + 3 cards + callout (square); compact flow + callout (og)

| Element | Text | Source |
|---|---|---|
| Eyebrow | Site Generator · Part 1 | front matter |
| Title | Why I built my own static site generator | title (the "— not because the others are bad" part moves to the subtitle) |
| Subtitle (square) | Not because the others are bad. | title: "…— not because the others are bad" |
| Node 1 | **Markdown input** — Structured text goes in. | "Read Markdown input"; "structured text goes in, HTML comes out" |
| Node 2 (key) | **Custom generator** — A DSL marks sections and languages. HTML is rendered from a template. | "Use a DSL to mark structure (sections, languages)"; "Render HTML from a template" |
| Node 3 | **docs/** — Served directly by GitHub Pages. | "Write output to `docs/`, served directly by GitHub Pages" |
| Card Not a content management problem | A blog is a transformation pipeline: structured text goes in, HTML comes out. | "this is a transformation pipeline: structured text goes in, HTML comes out. That's a programming problem — not a content management problem." |
| Card The actual constraint | Bilingual content: one file per article. No translation keys, no duplicated directory trees. | "No translation keys, no locale mappings, no duplicated directory trees. I wanted one file per article" |
| Card Simpler in the long run | A custom tool is simpler when your problem is small and well-defined. | "a custom tool is simpler in the long run when your problem is small and well-defined" |
| Callout (square) | A custom tool isn't a hobby project. It's an engineering decision. | "a custom tool isn't a hobby project. It's an engineering decision." |
| Callout (og) | The question isn't which generator to pick. | "the question isn't *which generator to pick*" |
