# generator-pipeline-to-github-pages — The generator pipeline — from Markdown to GitHub Pages
Source: content/posts/post_generatorPipeline.md (slug `posts/generator-pipeline-to-github-pages.html`, series Site Generator, part 3) · Pattern: two flows + 3 cards + callout (square); compact flow + callout (og)

| Element | Text | Source |
|---|---|---|
| Eyebrow | Site Generator · Part 3 | front matter |
| Title | The generator pipeline — from Markdown to GitHub Pages | title |
| Subtitle (square) | No CI/CD. No build server. A local command, a docs/ folder, and a git push. | description |
| Label 1 | Build · a single command, a single pass | "A single command, a single pass, deterministic output." |
| Flow 1 node 1 | **Configuration** — Loaded from appsettings.json. | step 1: "Load configuration from `appsettings.json`" |
| Flow 1 node 2 | **Parse** — Produces a PageDocument. | "The parser produces a `PageDocument`" |
| Flow 1 node 3 | **Render** — Produces a finished HTML string. | "the renderer produces a finished HTML string" |
| Flow 1 node 4 (key) | **Write** — Places it in docs/ at the path of the slug. | "the writer places it in `docs/` at the path defined by the file's `slug` metadata field" |
| Label 2 | Publish · a deliberate human step | "Why manual, and why that's a deliberate choice" |
| Flow 2 | Write → Build → Review on desktop → Review on mobile → Push | "The flow is: write, build, review on desktop, review on mobile, push." |
| Card The docs/ folder | GitHub Pages serves it directly. No build step on the server side. | "GitHub Pages serves it directly — no build step on the server side" |
| Card Local preview | preview.ps1 serves the exact same output that GitHub Pages will. | "The preview serves the exact same output that GitHub Pages will" |
| Card Why manual | Automating the review would mean trusting the build output without looking at it. | "Automating it would mean trusting the build output without looking at it." (about the review step) |
| Callout (square) | What you see locally is what ships. | "What you see locally is what ships." |
| Callout (og) | A local command, a docs/ folder, and a git push. | description / blockquote |
