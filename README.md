# vvidman.github.io

Source of [vvidman.github.io](https://vvidman.github.io) — the engineering blog of Viktor Vidman, software architect (.NET, AI-augmented development).

The site is bilingual (English / Hungarian) and built with a small custom C# static site generator. No Jekyll, no CI: a local build writes static HTML into `docs/`, which GitHub Pages serves as-is.

## What is written here

- AI-augmented development workflows — *the human orchestrates, the AI executes*
- Local-first LLM infrastructure in .NET (InferRouter, RagLab, SourceRAG)
- Multi-agent experiments (ChaosForge) and human-in-the-loop pipelines (Scaffold Protocol)
- The blog's own generator, and occasional home lab notes

## Repository layout

| Path | Purpose |
|------|---------|
| `content/` | Source pages in bilingual Markdown DSL (`posts/`, `about/`) |
| `blog-build/` | The generator (.NET 10, Markdig): DSL parser, renderers, post index |
| `blog-build/templates/` | HTML templates (`page.html`, `posts-index.html`) |
| `docs/` | Generated site + static assets (CSS, JS, images) — served by GitHub Pages |
| `preview.ps1` | Optional build + local preview server |

## Content format

Each page has YAML-style front matter and content split into sections, each with one block per language:

```markdown
---
title: Building a RAG Pipeline from Scratch
title_hu: RAG építése az alapoktól
slug: posts/raglab-01.html
description: One-line summary used for meta tags and the post index.
layout: page
date: 2026-04-05
series: RagLab
part: 1
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

## Build and preview

Requires the .NET 10 SDK.

```powershell
# build docs/ and serve it on http://localhost:8080
./preview.ps1 -Build

# or only build
cd blog-build
dotnet run
```

The generator resolves the paths in `blog-build/appsettings.json` relative to the working directory, so run it from `blog-build/`.

## License

[MIT](LICENSE)
