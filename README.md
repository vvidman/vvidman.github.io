# vvidman.github.io

Source of [vvidman.github.io](https://vvidman.github.io) — the engineering blog of Viktor Vidman, software architect (.NET, AI-augmented development).

The site is bilingual (English / Hungarian) and built with a small custom C# static site generator. No Jekyll, no CI: a local build writes static HTML into `docs/`, which GitHub Pages serves as-is. Every page — including the home page, the posts index, `sitemap.xml`, `robots.txt`, `feed.xml`, `404.html` and the redirect stubs for renamed URLs — is generated.

## What is written here

- AI-augmented development workflows — *the human orchestrates, the AI executes*
- Local-first LLM infrastructure in .NET (InferRouter, RagLab, SourceRAG)
- Multi-agent experiments (ChaosForge) and human-in-the-loop pipelines (Scaffold Protocol)
- The blog's own generator, and occasional home lab notes

## Repository layout

| Path | Purpose |
|------|---------|
| `content/` | Source pages in bilingual Markdown DSL (`posts/`, `about/`, `certifications/`) |
| `blog-build/` | The generator (.NET 10, Markdig, Markdown.ColorCode): DSL parser, template engine, renderers, SEO files |
| `blog-build/templates/` | HTML templates (`page.html`, `home.html`, `posts-index.html`, `redirect.html`) and `partials/` |
| `blog-build.Tests/` | xUnit tests (parser, rendering, links, CSS sanity, content rules) |
| `infographics/` | Infographic sources (`src/`), shared stylesheet (`_shared/`) and `export.ps1` |
| `docs/` | Generated site + static assets (CSS, JS, fonts, images) — served by GitHub Pages |
| `plans/` | Design spec and implementation plans (not published) |
| `preview.ps1` | Build + local preview server |

## Content format

Each page has YAML-style front matter and content split into sections, each with one block per language. Post bodies start at `##`; the title is rendered from front matter only.

```markdown
---
title: Building a RAG Pipeline from Scratch
title_hu: RAG építése az alapoktól
slug: posts/raglab-01.html
description: One-line summary used for meta tags and the post index.
description_hu: Egysoros összefoglaló magyarul.
layout: page
date: 2026-04-05
series: RagLab
part: 1
topic: llm-infra
image: /assets/images/infographics/raglab-01-square.png
og_image: /assets/images/infographics/raglab-01-og.png
---

:::section intro

:::lang en
## Problem statement
...
:::lang

:::lang hu
## Probléma megfogalmazás
...
:::lang

:::section
```

### Front matter keys

| Key | Meaning |
|-----|---------|
| `title`, `title_hu` | Page title; a missing Hungarian value falls back to English |
| `slug` | Output path under `docs/` (also the public URL) |
| `description`, `description_hu` | Meta description, post index and lead paragraph |
| `layout` | Template name (`page`) |
| `date` | `yyyy-MM-dd`; posts are ordered by it |
| `tags` | Comma-separated pills on the post header |
| `series`, `part` | Groups posts into a series with a navigation tree and pager |
| `topic` | Required on posts: `ai-workflow`, `llm-infra` or `tooling`. Groups the posts index; unknown value is a build error, missing value a warning (post listed under "Other"). All parts of a series must share one topic |
| `image` | Square infographic (1200×1200), shown in the post and as the list thumbnail |
| `og_image` | 1200×630 social image for `og:image` / `twitter:image` (fallback: profile photo) |
| `redirect_from` | Comma-separated old slugs that get a redirect stub |

Unknown keys are a build error.

## Build, preview and test

Requires the .NET 10 SDK. All commands work from any working directory.

```powershell
# build docs/ and serve it on http://localhost:8080
./preview.ps1 -Build

# or only build
dotnet run --project blog-build

# run the tests (includes a build of the whole site and an internal link check)
dotnet test
```

## Infographics

Every post has two infographics generated from the same content: a square one (1200×1200, in the post and for LinkedIn) and a 1200×630 one (link previews). They use the dark theme tokens and the self-hosted fonts of the site.

1. Write `infographics/src/<id>.notes.md`: every node, card and callout with the post paragraph it comes from. Only statements from the post's own text go on an infographic.
2. Build `infographics/src/<id>-square.html` and `<id>-og.html` with the components of `infographics/_shared/infographic.css`. `<id>` is the post's slug file name without `.html`.
3. Export (Edge headless, Chrome fallback; the script checks the exact PNG size):

   ```powershell
   ./infographics/export.ps1 -Id <id>   # one post
   ./infographics/export.ps1 -All       # every post
   ```

   PNGs are written to `docs/assets/images/infographics/`.
4. Set `image` / `og_image` in the post's front matter and rebuild.

## License

[MIT](LICENSE)
