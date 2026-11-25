# bilingual-markdown-dsl — Designing a minimal DSL for bilingual Markdown content
Source: content/posts/post_dslForSiteGenerator.md (slug `posts/bilingual-markdown-dsl.html`, series Site Generator, part 2) · Pattern: code block + 3 stacked cards + callout (square); code block + 2 cards (og)

| Element | Text | Source |
|---|---|---|
| Eyebrow | Site Generator · Part 2 | front matter |
| Title | Designing a minimal DSL for bilingual Markdown content | title |
| Subtitle (square) | One file. Two languages. No duplication. | description |
| Code block | `:::section intro` / `:::lang en` / `Markdown` / `:::lang` / `:::lang hu` / `Markdown` / `:::lang` / `:::section` | the post's own example file ("What the file actually looks like"): delimiters `:::section intro`, `:::lang en`, `:::lang hu`, closing `:::lang`, `:::section`; "Each language block is plain Markdown." (language text replaced by the word Markdown) |
| Card One file per article | Not one file per language. Not a source file with a translation sidecar. | "every article lives in a single file. Not one file per language. Not a source file with a translation sidecar." |
| Card Two levels, not one | :::section name — a named content block. :::lang en / :::lang hu — a language block inside a section. | "The DSL has two nesting levels: 1. `:::section name` — a named content block… 2. `:::lang en` / `:::lang hu` — a language-specific block within a section" |
| Card A simple state machine | The parser reads line by line. No regex. No recursive descent. | "It processes the file line by line"; "No regex. No recursive descent. A simple state machine" |
| Callout (square) | Explicit over clever. | "Explicit over clever. That was the principle." |
