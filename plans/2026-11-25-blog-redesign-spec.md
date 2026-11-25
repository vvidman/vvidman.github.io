# vvidman.github.io — Redesign & Generator Spec

Date: 2026-11-25 · Owner: Viktor Vidman · Status: approved for implementation

This spec consolidates the decisions taken in the 2026-11-25 review session. The implementation plan
(`plans/2026-11-25-blog-redesign.md`) argues from this document.

## 1. Goals

1. Replace the current look with the approved design **"D"** (layout and typography of mockup C, colours of mockup A), light-first with a dark mode.
2. Move every page to generated output (home page included), with shared partials instead of copy-pasted headers.
3. Add the reader-facing features the current site lacks: post date, reading time, series navigation, syntax highlighting, per-post social image.
4. Fix structural problems: descriptive slugs with redirects from the old URLs, sitemap/robots/RSS/404, cwd-independent build.
5. Regenerate every infographic in the new identity (dark), one per post, in two sizes.

Mockup canvas (visual reference): https://claude.ai/artifact/JAZtFfh8pUHnfMfM62f1VQ — artboards **D-Home** and **D-Post** are the target.

## 2. Out of scope (deferred, do not implement)

- Site positioning / tagline change (Software Architect vs AI Platform Architect) — copy stays as specified in §6.
- `/work-with-me/` page and the pillar taxonomy from `blog-restructure-spec.md` — both depend on positioning.
- Separate URLs per language (`/hu/...`). Language stays a client-side switch on one URL.
- Removing the photographer's mark from the profile photo (ask the photographer for a clean export instead).

## 3. Visual identity "D"

### 3.1 Fonts (self-hosted, no external requests)

| Role | Family | Weights | Files (fontsource-variable 5.3.0) |
|---|---|---|---|
| Display / headings | Space Grotesk (variable) | 500–700 | `space-grotesk-latin-wght-normal.woff2`, `space-grotesk-latin-ext-wght-normal.woff2` |
| Body / UI | Inter (variable) | 400–600 | `inter-latin-wght-normal.woff2`, `inter-latin-ext-wght-normal.woff2` |
| Mono / code / meta | JetBrains Mono (variable) | 400–500 | `jetbrains-mono-latin-wght-normal.woff2`, `jetbrains-mono-latin-ext-wght-normal.woff2` |

Download URL pattern: `https://cdn.jsdelivr.net/npm/@fontsource-variable/<family>@5.3.0/files/<file>` (family = `inter`, `space-grotesk`, `jetbrains-mono`). Store under `docs/assets/fonts/` with the package `LICENSE` (OFL) copied as `docs/assets/fonts/OFL-<family>.txt`.
`latin-ext` is mandatory (Hungarian ő/ű). Unicode ranges:
- latin: `U+0000-00FF,U+0131,U+0152-0153,U+02BB-02BC,U+02C6,U+02DA,U+02DC,U+0304,U+0308,U+0329,U+2000-206F,U+20AC,U+2122,U+2191,U+2193,U+2212,U+2215,U+FEFF,U+FFFD`
- latin-ext: `U+0100-02BA,U+02BD-02C5,U+02C7-02CC,U+02CE-02D7,U+02DD-02FF,U+0304,U+0308,U+0329,U+1D00-1DBF,U+1E00-1E9F,U+1EF2-1EFF,U+2020,U+20A0-20AB,U+20AD-20C0,U+2113,U+2C60-2C7F,U+A720-A7FF`

### 3.2 Colour tokens

| Token | Light | Dark |
|---|---|---|
| `--bg` | `#F4F5F2` | `#0B121B` |
| `--surface` | `#FFFFFF` | `#111B27` |
| `--code-bg` | `#ECEEEB` | `#0E1824` |
| `--ink` | `#111C27` | `#E3E9F0` |
| `--muted` | `#4F5D6B` | `#9AA8B7` |
| `--line` | `#D5D9DD` | `#243243` |
| `--line-no` | `#7E8A96` | `#5E6E80` |
| `--accent` | `#1D5FD1` | `#6EA3FF` |
| `--accent-ink` | `#FFFFFF` | `#0B121B` |
| `--accent-soft` | `rgba(29,95,209,0.08)` | `rgba(110,163,255,0.12)` |
| `--tk-kw` (keyword) | `#1D5FD1` | `#7FB2FF` |
| `--tk-ty` (type) | `#0F766E` | `#6FD6C3` |
| `--tk-str` (string) | `#B4501A` | `#F2B47E` |
| `--tk-num` (number) | `#1D5FD1` | `#F08A4B` |
| `--tk-com` (comment) | `#5E6B78` | `#8796A8` |
| `--tk-fn` (member/attr) | `#8A4B0F` | `#F2B47E` |

Theme switch: `<html data-theme="light|dark">`. Initial value: stored choice (`localStorage.theme`), else `prefers-color-scheme`. Toggle button in the header.

### 3.3 Layout

- Header (all pages): brand `~/vvidman` (mono, `~/` in accent) · nav `/blog`, `/about` (mono, current page gets a surface pill) · EN/HU segmented switch · theme toggle (44×44). Bottom border `--line`.
- Footer: `© {year} Viktor Vidman · built with a custom C# generator` (HU: `saját C# generátorral építve`) · links github, linkedin, rss (mono, lowercase).
- Container widths: home and lists 1160px, post layout 1240px with article column max 760px, generic pages 760px. Side padding 24px.
- Radii: 6px (pills), 8px (buttons), 10px (code, cards), 12px (panels, images).
- Every interactive element is a real `<a>` or `<button>`, ≥44px tall touch target, visible focus ring (`2px solid var(--accent)`, offset 2px).
- Text contrast ≥ 4.5:1 in both themes (tokens above satisfy this).
- Mobile (<760px): layouts stack; series tree moves above the article; nav wraps.

### 3.4 Page anatomy

**Home** (generated `docs/index.html`): hero (left: `$ whoami` mono line, H1 two lines with line 2 in accent, lead paragraph, two CTAs; right: `profile.json` panel) → "Latest posts" list of the 5 newest posts (git-log rows: 72px thumbnail, title, description, series pill, date, reading minutes) → footer.

**Post** (`page.html` with post chrome): two columns — left series tree (only for posts in a series: series name, parts as `NN-<slug-ish title>` links, current part highlighted with accent inset bar, GitHub repo link if the post defines one — not in scope, omit) and right article: pills (`<Series> · <part>/<total>` in accent-soft, plus tags if any) → H1 (Space Grotesk 600, ~54px desktop) → lead (description, 20px muted) → meta line (`date · N min read · EN / HU`, mono) → content → pager (prev/next part cards).
Posts not in a series use a single centred 760px column with the same header block.

**Posts index**: page title, then one block per series (series name, part count, rows), then "Standalone posts". Row = part badge, title, date, minutes.

**Generic page** (about): title + content in a 760px column, no post meta.

### 3.5 Code blocks

Syntax highlighting at build time with `Markdown.ColorCode` 3.0.1 in CSS-class mode with our own class names (`tk-kw`, `tk-ty`, `tk-str`, `tk-num`, `tk-com`, `tk-fn`). Languages without ColorCode support (bash, yaml, ini, md, json if unsupported) render as plain monospace blocks with the same frame. JS adds a header bar with the language label and a Copy button to every code block (progressive enhancement; without JS the block is still readable).

## 4. Bilingual behaviour

- One URL per page; both languages are in the HTML. `<html data-lang="en|hu" lang="en|hu">`.
- Every bilingual fragment is marked `data-l="en"` / `data-l="hu"`; CSS hides the inactive one.
- Initial language: stored choice (`localStorage.lang`), else `navigator.language` starting with `hu` → `hu`, else `en`.
- Switching also sets `document.title` from `data-title-en` / `data-title-hu` on `<html>`.
- An inline script in `<head>` sets `data-lang` and `data-theme` before first paint (no flash). All `localStorage` access is wrapped in try/catch.
- Missing Hungarian value (title, description, section) falls back to English — never an empty element.

## 5. Content model (front matter)

Existing keys: `title`, `title_hu`, `slug`, `description`, `layout`, `date`, `tags`, `series`, `part`.
New keys:
- `description_hu` — Hungarian description (all posts get one; drafts written by the implementer, reviewed by Viktor).
- `image` — square infographic, site-relative path (`/assets/images/infographics/<id>-square.png`); used in-post and as list thumbnail.
- `og_image` — 1200×630 social image path; used for `og:image` / `twitter:image`. Fallback: `/assets/images/vv-profile.jpg`.
- `redirect_from` — comma-separated list of old slugs (e.g. `posts/post-02.html`) that get a redirect stub.

Reading time: words outside fenced code blocks ÷ 220, rounded up, minimum 1; computed per language.

Headings inside post bodies start at `##`. The post title is rendered from front matter only — content must not repeat it.

## 6. Copy (fixed text)

Hero H1 — EN: `The human orchestrates.` / `The AI executes.` · HU: `Az ember orchestrál.` / `Az AI végrehajt.`
Hero lead — EN: `Software architect, 16 years in .NET. I write about AI-augmented delivery, local-first LLM infrastructure and the engineering decisions behind them.` · HU: `Szoftver architekt vagyok, 16 éve .NET-ben. AI-támogatott fejlesztésről, local-first LLM infrastruktúráról és a mögöttük álló mérnöki döntésekről írok.`
CTAs — EN: `Read the blog →` (`/posts/`), `About me` (`/about/`) · HU: `Irány a blog →`, `Rólam`.
profile.json — `role: "Software Architect"`, `years: 16`, `core: ["C#", ".NET", "ASP.NET Core"]`, `ai: ["LlamaSharp", "Qdrant", "Ollama"]`, `latest: "<title of newest post>"` (generated), `lang: ["en", "hu"]`.
Section labels — Latest posts / Legfrissebb · all posts / összes · Series / Sorozat · Standalone posts / Egyéb bejegyzések · min read / perc olvasás · Previous / Előző · Next / Következő · Copy / Másolás · Copied / Másolva.

## 7. URLs

Renamed slugs (old → new). Every renamed page gets `redirect_from: <old>`; the generator writes a stub at the old path.

| Source file | Old slug | New slug |
|---|---|---|
| `post_siteGenerator.md` | `posts/post-02.html` | `posts/why-i-built-my-own-static-site-generator.html` |
| `post_dslForSiteGenerator.md` | `posts/post-03.html` | `posts/bilingual-markdown-dsl.html` |
| `post_generatorPipeline.md` | `posts/post-04.html` | `posts/generator-pipeline-to-github-pages.html` |
| `post_scaffoldProtocol.md` | `posts/post-sp01.html` | `posts/scaffold-protocol-the-ai-executes-you-decide.html` |
| `chaosforge-01.md` | `posts/post-chaosforge-01.html` | `posts/chaosforge-let-go-of-the-reins.html` |
| `chaosforge-02-closing-the-loop.md` | `posts/post-chaosforge-02.html` | `posts/chaosforge-closing-the-loop.html` |
| `post_licensAndCerts.md` → move to `content/certifications/index.md` | `posts/post-01.html` | `certifications/index.html` |

All other slugs stay. Redirect stub: `<meta http-equiv="refresh" content="0; url=/new">`, `<link rel="canonical" href="https://vvidman.github.io/new">`, `<meta name="robots" content="noindex">`, and a visible link.

Also generated: `docs/sitemap.xml` (all non-stub pages), `docs/robots.txt` (allow all + sitemap URL), `docs/feed.xml` (RSS 2.0, newest 20 posts, English titles/descriptions), `docs/404.html` (generic page layout, bilingual message + link home).

## 8. Infographics (phase 3)

- One per post (18 posts; the certifications page gets none). RagLab's three old images are merged into one.
- Two sizes from the same HTML source: **square 1200×1200** (in-post + LinkedIn) and **og 1200×630** (link previews).
- Dark theme tokens from §3.2 (dark column), fonts from §3.1 (loaded from the local woff2 files).
- No third-party marks or watermarks. Footer: `Viktor Vidman · vvidman.github.io` (no job title — positioning is undecided).
- Content rule: every statement on an infographic must be traceable to the post's text. Viktor approves each series' infographics before export.
- Old PNGs in `docs/assets/images/` that are replaced are deleted.

## 9. Engineering constraints

- .NET 10, Markdig 0.45.0. New package: `Markdown.ColorCode` 3.0.1. Tests: xUnit.
- HTML inside markdown DSL files has zero indentation (Markdig treats 4+ spaces as code).
- DSL tokens inside code fences are ignored (existing behaviour, keep).
- `{{ }}` sequences inside rendered content must never be substituted (posts about the generator contain literal placeholders).
- No runtime requests to third-party hosts (fonts, scripts, analytics).
- Build: `cd blog-build; dotnet run` and `./preview.ps1 -Build` must both work from any working directory.
- Every change is reviewed by Viktor on desktop and mobile before publishing.
