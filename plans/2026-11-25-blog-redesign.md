# Blog Redesign & Generator Extension Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Ship design "D" on vvidman.github.io with a fully generated site (home included), reader features (date, reading time, series nav, syntax highlighting), descriptive URLs with redirects, SEO files, and a new set of dark infographics.

**Architecture:** The C# generator (`blog-build/`) gets a small template engine with partials, a `SiteIndex` that knows posts and series, focused renderers for post chrome / index / home / SEO files, and a `SiteBuilder` entry point that both `Program` and an xUnit integration test call. Styling is one token-based `site.css` with light/dark themes; bilingual text is switched client-side via `data-l` attributes.

**Tech Stack:** .NET 10, Markdig 0.45.0, Markdown.ColorCode 3.0.1, xUnit, vanilla CSS/JS, GitHub Pages (`docs/`), Edge/Chrome headless for infographic export.

**Spec:** `plans/2026-11-25-blog-redesign-spec.md` — read it before starting; section numbers below (§) refer to it. Visual target: mockup canvas artboards D-Home and D-Post (link in spec §1).

**Repo root:** `D:\Work\vvidman.github.io`. Plans live in `plans/` (NOT under `docs/` — `docs/` is the published site).

## Global Constraints

- .NET 10 (`net10.0`), Markdig 0.45.0; only new packages: `Markdown.ColorCode` 3.0.1 (generator), `xunit`, `xunit.runner.visualstudio`, `Microsoft.NET.Test.Sdk` (tests).
- Generator namespaces: `VVidman.BlogBuild`, `VVidman.BlogBuild.Models`, `VVidman.BlogBuild.Renderers` (folder `Renderer/` keeps its name).
- Zero indentation for HTML inside markdown DSL files (Markdig 4-space code-block gotcha).
- `{{...}}` inside rendered page content is never substituted (single-pass template rendering, content inserted last).
- No runtime requests to third-party hosts: fonts self-hosted, no CDN scripts, no analytics.
- Every user-visible string exists in EN and HU (§6); missing HU falls back to EN, never to empty.
- Colour values only from the token table in §3.2; no hard-coded colours elsewhere in CSS.
- Touch targets ≥44px; text contrast ≥4.5:1; real `<a>`/`<button>` elements only.
- Keep `docs/.nojekyll`. Do not hand-edit generated files in `docs/` (except assets).
- One commit per task, message prefix `feat:`, `fix:`, `refactor:`, `test:`, `content:` or `chore:`. Viktor reviews diffs; do not push.
- Viktor's content rule: no invented facts or narratives in posts, descriptions or infographics — only what the post text says.

## Review Focus

1. **Literal `{{placeholders}}` in posts** (Site Generator series shows template syntax in code blocks) must render verbatim and must not make the template engine throw — test in Task 3 and Task 9.
2. **Pages without optional metadata** (about, certifications, 404: no date/series/description_hu/image) must render without empty pills, stray separators or "0 min" — test in Task 9.
3. **Missing Hungarian content** (no `title_hu`, a section without `:::lang hu`) falls back to English text instead of an empty block — test in Task 4 and Task 9.
4. **Code blocks in unsupported languages** (bash, yaml, ini, md) and without a language stay framed, readable and copyable — test in Task 6.
5. **JavaScript disabled or `localStorage` blocked** (private mode): page shows English, theme follows the OS, nothing throws — manual check in Task 8 (documented steps).

---

## File Structure

```
blog-build/
  BlogBuild.csproj                (modify: + Markdown.ColorCode)
  Program.cs                      (modify: thin wrapper over SiteBuilder)
  SiteBuilder.cs                  (create: orchestrates parse → render → write)
  PathResolver.cs                 (create: cwd-independent paths)
  SiteIndex.cs                    (create: posts, series context, latest)
  PostIndexGenerator.cs           (rewrite: new markup)
  HomeGenerator.cs                (create)
  SeoGenerator.cs                 (create: sitemap, robots, rss, 404 vars)
  RedirectWriter.cs               (create)
  MarkdownDSLParser.cs            (modify: new keys)
  Models/PageMetadata.cs          (modify: new props)
  Models/ResolvedPaths.cs         (create)
  Renderer/TemplateEngine.cs      (create)
  Renderer/I18n.cs                (create)
  Renderer/ReadingTime.cs         (create)
  Renderer/Urls.cs                (create)
  Renderer/BlogCodeStyles.cs      (create)
  Renderer/MarkdownRenderer.cs    (modify: ColorCode)
  Renderer/PageRenderer.cs        (rewrite: uses TemplateEngine)
  Renderer/PostChromeRenderer.cs  (create)
  templates/partials/_head.html   (create)
  templates/partials/_header.html (create)
  templates/partials/_footer.html (create)
  templates/page.html             (rewrite)
  templates/posts-index.html      (rewrite)
  templates/home.html             (create)
  templates/redirect.html         (create)
blog-build.Tests/
  BlogBuild.Tests.csproj          (create)
  Fixtures/…                      (create: small .md fixtures)
  *Tests.cs                       (create per task)
docs/assets/css/site.css          (rewrite)
docs/assets/javascript/site.js    (rewrite)
docs/assets/fonts/*.woff2, OFL-*.txt (add)
docs/assets/images/vv-profile.jpg (add, 640px)
infographics/                     (create, phase 3: _shared/, src/, export.ps1)
content/**                        (modify: front matter, headings, slugs)
```

---

## Phase 1 — Foundations and redesign

### Task 0: Test project

**Files:**
- Create: `blog-build.Tests/BlogBuild.Tests.csproj`, `blog-build.Tests/SmokeTests.cs`
- Modify: `vvidman.github.io.sln` (add both projects if `BlogBuild.csproj` is missing from it)

**Interfaces:**
- Produces: test project referencing `../blog-build/BlogBuild.csproj`; `TestPaths.RepoRoot` static helper (walks up from `AppContext.BaseDirectory` to the folder containing `vvidman.github.io.sln`).

- [ ] **Step 1:** Create the xUnit project (`net10.0`), project reference to BlogBuild, add to the solution: `dotnet new xunit -o blog-build.Tests -n BlogBuild.Tests; dotnet sln vvidman.github.io.sln add blog-build.Tests/BlogBuild.Tests.csproj blog-build/BlogBuild.csproj; dotnet add blog-build.Tests reference blog-build/BlogBuild.csproj`
- [ ] **Step 2:** Write `SmokeTests.Parses_every_existing_content_file`: every `*.md` under `content/` parses with `MarkdownDslParser.Parse` without exception.
- [ ] **Step 3:** Run `dotnet test` from repo root. Expected: PASS (1 test).
- [ ] **Step 4:** Commit `test: add xUnit test project with content smoke test`.

### Task 1: Content model — new front-matter keys

**Files:**
- Modify: `blog-build/Models/PageMetadata.cs`, `blog-build/MarkdownDSLParser.cs`
- Test: `blog-build.Tests/ParserMetadataTests.cs`, `blog-build.Tests/Fixtures/full-metadata.md`

**Interfaces:**
- Produces on `PageMetadata`: `string DescriptionHu` (default `""`), `string? Image`, `string? OgImage`, `IReadOnlyList<string> RedirectFrom` (default empty; comma-split, trimmed).

- [ ] **Step 1: Failing tests**
```csharp
[Fact] public void Parses_new_keys() {
    var doc = MarkdownDslParser.Parse(Fixture("full-metadata.md"));
    Assert.Equal("Magyar leírás", doc.Metadata.DescriptionHu);
    Assert.Equal("/assets/images/infographics/x-square.png", doc.Metadata.Image);
    Assert.Equal("/assets/images/infographics/x-og.png", doc.Metadata.OgImage);
    Assert.Equal(new[] { "posts/post-02.html", "posts/old.html" }, doc.Metadata.RedirectFrom);
}
[Fact] public void Unknown_key_still_throws() { /* fixture with `foo: bar` → InvalidOperationException */ }
```
- [ ] **Step 2:** `dotnet test --filter ParserMetadataTests` → FAIL (unknown metadata key).
- [ ] **Step 3:** Add keys `description_hu`, `image`, `og_image`, `redirect_from` to `ParseMetadata`.
- [ ] **Step 4:** Tests PASS, smoke test still PASS.
- [ ] **Step 5:** Commit `feat: parse description_hu, image, og_image, redirect_from`.

### Task 2: Path resolution and SiteBuilder entry point

**Files:**
- Create: `blog-build/PathResolver.cs`, `blog-build/Models/ResolvedPaths.cs`, `blog-build/SiteBuilder.cs`
- Modify: `blog-build/Program.cs`, `preview.ps1` (drop the Push-Location workaround: `dotnet run --project (Join-Path $PSScriptRoot 'blog-build/BlogBuild.csproj')`)
- Test: `blog-build.Tests/PathResolverTests.cs`

**Interfaces:**
- Produces: `public sealed record ResolvedPaths(string ProjectDir, string Input, string Templates, string Output);`
- Produces: `PathResolver.FindProjectDir(string startDir) : string` — walks up until a directory containing `BlogBuild.csproj`; throws `DirectoryNotFoundException` if none.
- Produces: `PathResolver.Resolve(BuildConfig cfg, string projectDir) : ResolvedPaths` — `Path.GetFullPath(Path.Combine(projectDir, cfg.Paths.X))`.
- Produces: `SiteBuilder.Build(BuildConfig cfg, ResolvedPaths paths) : BuildReport` with `public sealed record BuildReport(int Errors, IReadOnlyList<string> WrittenFiles);` — contains today's three phases (collect, render pages, posts index) unchanged in behaviour; later tasks add phases here.

- [ ] **Step 1:** Tests: `Resolve` turns `../content` into `<repo>/content` for projectDir `<repo>/blog-build`; `FindProjectDir(<repo>/blog-build/bin/Debug/net10.0)` returns `<repo>/blog-build`.
- [ ] **Step 2:** Run → FAIL.
- [ ] **Step 3:** Implement; `Program.Main` = load config from `AppContext.BaseDirectory`, `FindProjectDir(AppContext.BaseDirectory)`, `Resolve`, `SiteBuilder.Build`, exit code 1 when `Errors > 0`.
- [ ] **Step 4:** Tests PASS. Run `dotnet run --project blog-build` from repo root AND `cd blog-build; dotnet run`: both regenerate `docs/` with no diff in `git status docs/` beyond nothing (identical output).
- [ ] **Step 5:** Commit `refactor: cwd-independent paths and SiteBuilder entry point`.

### Task 3: Template engine with partials

**Files:**
- Create: `blog-build/Renderer/TemplateEngine.cs`
- Test: `blog-build.Tests/TemplateEngineTests.cs` (uses a temp templates dir)

**Interfaces:**
- Produces: `public sealed class TemplateEngine(string templatesDir)` with `string Render(string templateName, IReadOnlyDictionary<string, string> vars)`.
  - Loads `<templatesDir>/<templateName>.html` (cached).
  - Expands `{{> name}}` from `<templatesDir>/partials/_<name>.html`, recursively, max depth 5 (deeper → `TemplateException`).
  - Then ONE regex pass `\{\{\s*([a-z0-9_]+)\s*\}\}` over the expanded template replacing from `vars`; values are inserted verbatim and never rescanned. Unknown name → `TemplateException("Unknown placeholder 'x' in template 'y'")`.
- Produces: `public sealed class TemplateException : Exception`.

- [ ] **Step 1: Failing tests**
```csharp
[Fact] public void Replaces_vars_and_partials() // "<h>{{> nav}}</h>{{title}}" + _nav "N{{x}}" → "<h>N1</h>T"
[Fact] public void Inserted_values_are_not_rescanned() // vars["content"]="{{title}}" → output contains literal "{{title}}"
[Fact] public void Unknown_placeholder_throws()
[Fact] public void Missing_partial_throws()
```
- [ ] **Step 2:** Run → FAIL. **Step 3:** Implement. **Step 4:** PASS.
- [ ] **Step 5:** Commit `feat: template engine with partials and single-pass substitution`.

### Task 4: I18n, reading time, URLs

**Files:**
- Create: `blog-build/Renderer/I18n.cs`, `blog-build/Renderer/ReadingTime.cs`, `blog-build/Renderer/Urls.cs`
- Test: `blog-build.Tests/HelpersTests.cs`

**Interfaces:**
- `I18n.Text(string en, string? hu) : string` — HTML-encodes; if `hu` is null/blank or equal to `en` returns encoded `en`; else `<span data-l="en">{en}</span><span data-l="hu">{hu}</span>`.
- `I18n.Label(string key) : string` — returns `I18n.Text` for the fixed labels of spec §6, keys: `latest_posts`, `all_posts`, `series`, `standalone`, `min_read`, `previous`, `next`, `copy` (copy/copied are JS-side, keep key for template use).
- `ReadingTime.Minutes(string markdown) : int` — whitespace-split word count of lines outside ``` fences, `ceil(words / 220.0)`, min 1.
- `Urls.Path(string slug) : string` — `/` + slug, with trailing `index.html` stripped (`about/index.html` → `/about/`).
- `Urls.Absolute(string slug) : string` — `https://vvidman.github.io` + `Path(slug)`.

- [ ] **Step 1:** Tests: `I18n.Text("A & B", null) == "A &amp; B"`; `I18n.Text("Next","Következő")` contains both spans; `ReadingTime.Minutes` of 441 prose words + a 500-word fenced block == 3; empty → 1; `Urls.Path("about/index.html") == "/about/"`; `Urls.Path("posts/raglab-01.html") == "/posts/raglab-01.html"`.
- [ ] **Step 2–4:** FAIL → implement → PASS.
- [ ] **Step 5:** Commit `feat: i18n, reading time and url helpers`.

### Task 5: SiteIndex — posts, series, latest

**Files:**
- Create: `blog-build/SiteIndex.cs`
- Test: `blog-build.Tests/SiteIndexTests.cs`

**Interfaces:**
- `public sealed class SiteIndex(IReadOnlyList<PageDocument> pages)`
  - `IReadOnlyList<PageDocument> Posts` — slug starts with `posts/`; ordered by `Date` desc (null last), then `Title`.
  - `IReadOnlyList<PageDocument> Latest(int count)` — first `count` of `Posts` with a date.
  - `IReadOnlyList<(string Name, IReadOnlyList<PageDocument> Parts)> Series` — grouped case-insensitively by `Metadata.Series`, parts ordered by `Part` (null last); series ordered by newest post date desc.
  - `SeriesContext? ContextFor(PageDocument post)`
- `public sealed record SeriesContext(string Name, IReadOnlyList<PageDocument> Parts, int Index, PageDocument? Previous, PageDocument? Next);`

- [ ] **Step 1:** Tests with in-memory `PageDocument`s: InferRouter part 2 of 4 → `Index == 1`, `Previous` part 1, `Next` part 3; first part `Previous == null`; standalone post → `ContextFor == null`; `Latest(2)` skips undated pages; `about/index.html` not in `Posts`.
- [ ] **Step 2–4:** FAIL → implement → PASS.
- [ ] **Step 5:** Commit `feat: SiteIndex with series context`.

### Task 6: Build-time syntax highlighting

**Files:**
- Modify: `blog-build/BlogBuild.csproj` (`<PackageReference Include="Markdown.ColorCode" Version="3.0.1" />`), `blog-build/Renderer/MarkdownRenderer.cs`
- Create: `blog-build/Renderer/BlogCodeStyles.cs`
- Test: `blog-build.Tests/HighlightingTests.cs`

**Interfaces:**
- `BlogCodeStyles.Create() : ColorCode.Styling.StyleDictionary` — one `Style` per scope with `ReferenceName` and no colours. Mapping (`ColorCode.Common.ScopeName` → class): Keyword, ControlKeyword, PreprocessorKeyword, PowerShellOperator → `tk-kw`; ClassName, Type, TypeVariable, NameSpace, PowerShellType → `tk-ty`; String, StringCSharpVerbatim, StringEscape, HtmlAttributeValue, XmlAttributeValue → `tk-str`; Number → `tk-num`; Comment, XmlDocComment, XmlDocTag, HtmlComment, XmlComment → `tk-com`; PowerShellVariable, PowerShellParameter, HtmlAttributeName, XmlAttribute, HtmlElementName, XmlName, BuiltinFunction → `tk-fn`. If a listed constant does not exist in the package, drop it (compile error tells you) — do not invent new ones.
- Pipeline: `.UseAdvancedExtensions().UseColorCode(HtmlFormatterType.Css, BlogCodeStyles.Create())`.

- [ ] **Step 1: Failing tests**
```csharp
[Fact] public void CSharp_keywords_get_token_class() {
    var html = MarkdownRenderer.Render("```csharp\npublic interface IFoo { }\n```");
    Assert.Contains("class=\"tk-kw\"", html);
    Assert.Contains("<pre", html);
}
[Fact] public void Unsupported_language_renders_plain_pre() {
    var html = MarkdownRenderer.Render("```bash\necho hi\n```");
    Assert.Contains("<pre", html); Assert.Contains("echo hi", html);
}
[Fact] public void Fence_without_language_renders_plain_pre() // "```\nx\n```" → contains "<pre"
[Fact] public void Placeholder_text_in_code_survives() // "```html\n{{content_en}}\n```" → contains "content_en"
```
- [ ] **Step 2:** FAIL. **Step 3:** Implement. **Step 4:** PASS. Build the site; open `docs/posts/inferrouter-02.html` source and record the actual wrapper markup ColorCode emits for csharp (expected `<div class="csharp"><pre>…`) and for bash (expected `<pre><code class="language-bash">`). Write both shapes as a comment at the top of `BlogCodeStyles.cs` — Task 7's CSS and Task 8's JS target exactly these.
- [ ] **Step 5:** Commit `feat: build-time syntax highlighting with token classes`.

### Task 7: Design tokens, fonts and stylesheet

**Files:**
- Add: `docs/assets/fonts/` — the six woff2 files and three `OFL-*.txt` (spec §3.1, download URLs there)
- Rewrite: `docs/assets/css/site.css`
- Add: `docs/assets/images/vv-profile.jpg` (640px wide, quality 82, from `vv-profile.png`; e.g. `magick vv-profile.png -resize 640x -quality 82 vv-profile.jpg`, or PowerShell + System.Drawing)

**Interfaces — class contract (templates and C# renderers in Tasks 8–12 use exactly these):**
- Layout: `.wrap` (max 1160), `.wrap-wide` (max 1240), `.wrap-narrow` (max 760), `.site-header`, `.brand`, `.nav`, `.toggles`, `.seg` + `.seg button[aria-pressed="true"]`, `.icon-btn`, `.site-footer`.
- Bilingual/theme: `[data-lang="en"] [data-l="hu"], [data-lang="hu"] [data-l="en"] { display:none !important }`; tokens on `:root` (light) and `[data-theme="dark"]`.
- Home: `.hero`, `.hero-text`, `.prompt`, `.hero h1 .accent`, `.lead`, `.cta`, `.btn`, `.btn-primary`, `.panel`, `.panel-head`, `.panel-body` (`.k` key, `.s` string, `.n` number), `.post-list`, `.post-row`, `.thumb`, `.row-main`, `.row-title`, `.row-desc`, `.row-meta`, `.pill`.
- Post: `.post-layout` (flex-wrap; with series: aside + article), `.series-tree`, `.series-tree a[aria-current="page"]`, `.article`, `.post-head`, `.pills`, `.pill-accent`, `.post-title`, `.post-lead`, `.post-meta`, `.prose`, `.pager`, `.pager a`, `.pager .dir`.
- Prose: headings h2–h4 in Space Grotesk, `.prose h2::before { content:"# "; color:var(--accent) }`, paragraphs 17.5px/1.75, links accent underline, `img` max-width 100% radius 12px border `--line`, tables, blockquote (accent-soft background, 3px accent left bar), inline `code` (surface + line border).
- Code: frame for both ColorCode wrapper and plain `pre` (bg `--code-bg`, border `--line`, radius 10px, mono 14px, overflow-x auto); `.code-head` (label + copy button, added by JS); `.tk-kw/.tk-ty/.tk-str/.tk-num/.tk-com/.tk-fn` → the `--tk-*` tokens.
- Index: `.series-block`, `.series-head`, `.part-badge`.
- Focus: `:focus-visible { outline:2px solid var(--accent); outline-offset:2px }`.
- Responsive breakpoint 760px: hero stacks, `.post-layout` stacks with `.series-tree` first, `.row-meta` wraps under title.

- [ ] **Step 1:** Add fonts and `@font-face` (6 rules: family names `Space Grotesk`, `Inter`, `JetBrains Mono`; `font-weight` ranges 300 700 / 100 900 / 100 800; `font-display: swap`; unicode ranges from spec §3.1). Remove the Google Fonts `@import`.
- [ ] **Step 2:** Write the token blocks exactly from spec §3.2 and the classes above. Delete every old rule (`#forkme_banner`, `#linked_ref`, `.btn-icon-en`, `.lang-grid`, old `.post-list`, `.series-*` …) except the `.cert-*` rules, which are re-pointed to tokens (certifications page keeps its card markup).
- [ ] **Step 2b:** Add `blog-build.Tests/CssSanityTests.cs`: `Braces_are_balanced` (strip `/* */` comments, running `{`/`}` depth never negative and ends at 0) and `Every_contract_class_is_defined` (each class in the contract list above appears as a selector in site.css). A single missing `}` silently drops every later rule in the browser — this test is the guard. Run → PASS.
- [ ] **Step 3:** Verify: `Select-String -Path docs/assets/css/site.css -Pattern '#[0-9a-fA-F]{3,6}'` matches only inside the two token blocks; no `googleapis` string anywhere in `docs/`.
- [ ] **Step 4:** Commit `feat: design D tokens, self-hosted fonts, new stylesheet`.

### Task 8: Partials, head init script, site.js

**Files:**
- Create: `blog-build/templates/partials/_head.html`, `_header.html`, `_footer.html`
- Rewrite: `docs/assets/javascript/site.js`

**Interfaces:**
- `_head.html` vars: `title_en`, `title_hu`, `description`, `canonical`, `og_image`, `og_type`, `json_ld`. Contains: charset, viewport, `<title>`, description, author, robots, `theme-color` (`#F4F5F2` light / `#0B121B` dark via `media`), canonical, OG (`og:title`, `og:description`, `og:url`, `og:image`, `og:type`, `og:locale` en_US + alternate hu_HU), `twitter:card=summary_large_image`, JSON-LD, `<link rel="preload">` for `inter-latin-wght-normal.woff2` and `space-grotesk-latin-wght-normal.woff2`, stylesheet, favicon/apple-touch-icon, `<link rel="alternate" type="application/rss+xml" href="/feed.xml">`, `site.js` with `defer`, and this inline script FIRST in `<head>`:
```html
<script>(function(){var d=document.documentElement,l,t;try{l=localStorage.getItem('lang');t=localStorage.getItem('theme')}catch(e){}
if(l!=='en'&&l!=='hu'){l=(navigator.language||'').toLowerCase().indexOf('hu')===0?'hu':'en'}
if(t!=='light'&&t!=='dark'){t=window.matchMedia&&matchMedia('(prefers-color-scheme: dark)').matches?'dark':'light'}
d.setAttribute('data-lang',l);d.setAttribute('lang',l);d.setAttribute('data-theme',t)})();</script>
```
- The `<html>` element (in each page template, not the partial): `<html lang="en" data-lang="en" data-theme="light" data-title-en="{{title_en}}" data-title-hu="{{title_hu}}">`.
- `_header.html` vars: `nav_blog`, `nav_about` (each `""` or ` aria-current="page"`). Markup per spec §3.3; EN/HU buttons `data-set-lang="en|hu"`, theme button `data-toggle-theme` with `aria-label` "Toggle dark mode" and two inline SVG icons (sun shown in dark, moon in light via CSS).
- `_footer.html` vars: `year`.
- `site.js` (no globals except an IIFE): binds `[data-set-lang]` (set attribute + `lang` + `aria-pressed` + `document.title` from `data-title-<l>` + persist), `[data-toggle-theme]` (flip + persist), and on DOMContentLoaded wraps every `.prose pre` in a frame with `.code-head` (label from ColorCode wrapper class or `language-x` class, else `text`; Copy button using `navigator.clipboard.writeText`, label switches to Copied/Másolva for 1.5s). All storage calls in try/catch.

- [ ] **Step 1:** Write the three partials and site.js (templates consume them in Tasks 9–11, so no generator change yet).
- [ ] **Step 2:** Manual check after Task 9 builds (recorded here, executed then): (a) DevTools → disable JavaScript → reload a post: English content visible, theme matches OS; (b) Firefox private window with `dom.storage.enabled=false` or Chrome with site data blocked: toggles still switch, console has no errors; (c) switching to HU changes the tab title.
- [ ] **Step 3:** Commit `feat: shared partials, no-flash init script, theme/lang/copy JS`.

### Task 9: Post page and generic page rendering

**Files:**
- Rewrite: `blog-build/templates/page.html`, `blog-build/Renderer/PageRenderer.cs`
- Create: `blog-build/Renderer/PostChromeRenderer.cs`
- Modify: `blog-build/Renderer/SectionRenderer.cs` (wrap per-language output in `<div class="prose" data-l="xx">` at page level, see below), `blog-build/SiteBuilder.cs`
- Test: `blog-build.Tests/PageRenderingTests.cs`

**Interfaces:**
- Consumes: `TemplateEngine`, `SiteIndex`, `SeriesContext`, `I18n`, `ReadingTime`, `Urls`.
- `PostChromeRenderer.Header(PageDocument doc, SeriesContext? series, int minutesEn, int minutesHu) : string` — pills (series pill `"{Name} · {Index+1}/{Parts.Count}"` only with series; one `.pill` per tag), `<h1 class="post-title">` via `I18n.Text(Title, TitleHu)`, `.post-lead` via `I18n.Text(Description, DescriptionHu)` (omitted when Description empty), `.post-meta` built only from present parts: date `yyyy-MM-dd`, `N min read`/`N perc olvasás` (only for `posts/`), `EN / HU`; separators ` · ` only between present parts.
- `PostChromeRenderer.SeriesTree(SeriesContext? s) : string` — `""` when null; `<aside class="series-tree">` with `NN` (two-digit `Part`, fallback index) + title per part, `aria-current="page"` on the current one.
- `PostChromeRenderer.Pager(SeriesContext? s) : string` — `""` when null or no neighbours; `<nav class="pager" aria-label="Series navigation">` with Previous/Next cards.
- `PostChromeRenderer.Figure(PageDocument doc) : string` — `""` unless `Image` set; `<figure>` with `<img src alt loading="lazy" width="1200" height="1200">`, alt = `I18n`-less EN title + " — infographic". (Phase 3 switches posts from in-content figure sections to this.)
- `PageRenderer.Render(PageDocument doc, SiteIndex index, TemplateEngine engine, IReadOnlyList<string> languages) : string` — vars: `title_en`, `title_hu` (fallback en), `description`, `canonical` (`Urls.Absolute`), `og_image` (absolute; `OgImage` ?? `/assets/images/vv-profile.jpg`), `og_type` (`article` for posts else `website`), `json_ld` (posts: `BlogPosting` with headline, datePublished, author Person "Viktor Vidman", url, image; others: `Person` block copied from current home page), `nav_blog`/`nav_about`, `year`, `layout_class` (`post-layout has-series` / `post-layout` / `page-layout`), `post_header` (posts only, else `<h1 class="post-title">` + title), `series_tree`, `figure`, `pager`, `content_en`, `content_hu`. Content strings get the existing `EscapeTemplateLiterals` before insertion; missing HU section content falls back to the EN section.
- Markup of the content area: `<div class="prose" data-l="en">…</div><div class="prose" data-l="hu">…</div>`.

- [ ] **Step 1: Failing tests** (render real files from `content/` through `SiteBuilder` into a temp output dir, then inspect HTML):
```csharp
[Fact] public void Series_post_has_tree_pager_and_meta() // inferrouter-02: contains "series-tree", aria-current, "pager", "2/4", "min read"
[Fact] public void About_page_has_no_post_meta() // about: no "post-meta", no "min read", no "pill"
[Fact] public void Missing_hu_title_falls_back_to_en() // fixture without title_hu: data-title-hu equals EN title, no empty data-l="hu" span
[Fact] public void Literal_placeholders_in_posts_survive() // post_dslForSiteGenerator renders, output contains "&#123;&#123;" or "{{" inside <code>, build has 0 errors
[Fact] public void Html_lang_attributes_present() // <html … data-lang="en" … data-title-en=
```
- [ ] **Step 2:** FAIL. **Step 3:** Implement and wire into `SiteBuilder` (replace the old Replace-chain). **Step 4:** PASS.
- [ ] **Step 5:** `./preview.ps1 -Build`, open http://localhost:8080/posts/inferrouter-02.html and /about/ at 1280px and 375px, both themes, both languages; compare with mockup D-Post. Run Task 8 Step 2 checks.
- [ ] **Step 6:** Commit `feat: design D post and page templates with series navigation`.

### Task 10: Posts index

**Files:**
- Rewrite: `blog-build/PostIndexGenerator.cs`, `blog-build/templates/posts-index.html`
- Test: `blog-build.Tests/PostIndexTests.cs`

**Interfaces:**
- `PostIndexGenerator.Generate(SiteIndex index, TemplateEngine engine) : string` — vars as Task 9 (`title_en` "Posts", `title_hu` "Bejegyzések", `nav_blog` current) plus `index_body`: one `.series-block` per `SiteIndex.Series` (head: name + `N parts`/`N rész`), rows `.post-row` with `.part-badge` (`#N`), title `I18n.Text`, `.row-meta` date + minutes; then a "Standalone posts" block for posts without series. The certifications page is no longer a post after Task 14; until then it appears under Standalone.

- [ ] **Step 1:** Test: generated index contains every post slug once; InferRouter block lists parts 1→4 in order; HU title of `raglab-01` present inside `data-l="hu"`.
- [ ] **Step 2–4:** FAIL → implement → PASS.
- [ ] **Step 5:** Visual check at /posts/ (desktop, mobile, both themes). Commit `feat: design D posts index`.

### Task 11: Generated home page

**Files:**
- Create: `blog-build/HomeGenerator.cs`, `blog-build/templates/home.html`
- Modify: `blog-build/SiteBuilder.cs` (new phase writes `docs/index.html`)
- Delete from tracking: nothing — `docs/index.html` is now overwritten by the generator
- Test: `blog-build.Tests/HomeTests.cs`

**Interfaces:**
- `HomeGenerator.Generate(SiteIndex index, TemplateEngine engine) : string` — vars: shared head/header/footer vars (`title_en` "Viktor Vidman – Software Architect · .NET · AI-Augmented Development", same for HU, description = current home meta description), `latest_title` (EN title of `Latest(1)`), `latest_rows` (5 `.post-row`s: thumbnail = `Image` as background, else a mono monogram of the series initials; title `I18n.Text`; description `I18n.Text(Description, DescriptionHu)`; series pill; date; minutes).
- `home.html` contains the hero copy and `profile.json` panel verbatim from spec §6 using `I18n`-style `data-l` spans written directly in the template.

- [ ] **Step 1:** Test: output contains `The human orchestrates.`, `Az ember orchestrál.`, exactly 5 `post-row`, newest post first, `profile.json`.
- [ ] **Step 2–4:** FAIL → implement → PASS.
- [ ] **Step 5:** Visual check against mockup D-Home. Commit `feat: generated design D home page`.

### Task 12: Content cleanup — titles, headings, Hungarian descriptions

**Files:**
- Modify: all 18 posts in `content/posts/`, `content/about/about.md`

**Rules (spec §5):**
- Remove the first heading of a post when it repeats the post title (both languages). Affected: chaosforge-01, chaosforge-02, homelab ×2, inferrouter-01…04, post_dslForSiteGenerator, post_generatorPipeline, post_licensAndCerts, post_scaffoldProtocol, post_siteGenerator, sourcerag-01. If that leaves a section with only empty lang blocks (the `title` sections), delete the whole section.
- Then shift headings per file so the smallest remaining level is `##` (outside code fences only).
- Add `description_hu` to every post: a faithful Hungarian translation of `description`, matching the post's own Hungarian wording where the post already phrases it. Also fix `post_siteGenerator.md` whose `description` duplicates its title — replace it with a one-sentence summary taken from its intro paragraph.
- About page: replace `vv-profile.png` with `vv-profile.jpg`.

- [ ] **Step 1:** Add test `ContentRulesTests.No_post_body_contains_h1` and `Every_post_has_description_hu` (iterate `content/posts`). Run → FAIL.
- [ ] **Step 2:** Apply the rules. **Step 3:** Tests PASS, full build 0 errors.
- [ ] **Step 4:** Produce `plans/description_hu-review.md` listing each post's EN description and new HU description for Viktor's review.
- [ ] **Step 5:** Commit `content: remove duplicate titles, normalise headings, add description_hu`. **Stop for Viktor's review of the HU descriptions before Phase 2.**

---

## Phase 2 — URLs and SEO

### Task 13: Descriptive slugs and redirect stubs

**Files:**
- Create: `blog-build/RedirectWriter.cs`, `blog-build/templates/redirect.html`
- Modify: the 7 files in spec §7 (slug + `redirect_from`), move `content/posts/post_licensAndCerts.md` → `content/certifications/index.md`, every internal link to an old slug in `content/` (about.md, post_generatorPipeline.md and any other `grep` hit), `blog-build/SiteBuilder.cs`
- Test: `blog-build.Tests/RedirectTests.cs`

**Interfaces:**
- `RedirectWriter.Render(TemplateEngine engine, string targetSlug) : string` — `redirect.html` vars `target_path`, `target_url`; content per spec §7.
- `SiteBuilder` writes one stub per `RedirectFrom` entry; a stub path that collides with a real page slug is a build error.

- [ ] **Step 1:** Tests: building writes `posts/post-02.html` containing `url=/posts/why-i-built-my-own-static-site-generator.html` and `noindex`; collision fixture → `Errors == 1`; `grep` over `content/` finds no `/posts/post-0`, `post-sp01`, `post-chaosforge-` links.
- [ ] **Step 2–4:** FAIL → implement + content moves → PASS.
- [ ] **Step 5:** Build; delete orphaned old HTML files in `docs/posts/` that are now stubs (they are overwritten) and nothing else. Commit `feat: descriptive slugs with redirect stubs`.

### Task 14: sitemap, robots, RSS, 404, link check

**Files:**
- Create: `blog-build/SeoGenerator.cs`
- Modify: `blog-build/SiteBuilder.cs`
- Test: `blog-build.Tests/SeoTests.cs`, `blog-build.Tests/LinkCheckTests.cs`

**Interfaces:**
- `SeoGenerator.Sitemap(IEnumerable<PageDocument> pages) : string` — `<urlset>` with `Urls.Absolute(slug)` and `<lastmod>` = date when present; includes `/` and `/posts/`; excludes stubs and 404.
- `SeoGenerator.Robots() : string` — `User-agent: *`, `Allow: /`, `Sitemap: https://vvidman.github.io/sitemap.xml`.
- `SeoGenerator.Rss(IEnumerable<PageDocument> latest) : string` — RSS 2.0, channel title `Viktor Vidman`, link home, items: title, link, guid (absolute URL), pubDate RFC 1123, description.
- 404: rendered through `page.html` with generic layout from an in-code `PageDocument` (EN "Page not found" / HU "Az oldal nem található", body links to `/` and `/posts/`), written to `docs/404.html`.

- [ ] **Step 1:** Tests: sitemap is valid XML, contains `/posts/raglab-01.html`, not `post-02.html`; RSS parses with `XDocument`, ≤20 items, newest first; `docs/404.html` exists.
- [ ] **Step 2:** `LinkCheckTests.All_internal_links_resolve`: build into temp dir, for every `href`/`src` starting with `/` in every `.html`, the target file exists (directory → `index.html`). Run → FAIL until both are done.
- [ ] **Step 3–4:** Implement → PASS.
- [ ] **Step 5:** Commit `feat: sitemap, robots, RSS feed, 404 page and link check`.

---

## Phase 3 — Infographics

### Task 15: Infographic template system and export

**Files:**
- Create: `infographics/_shared/infographic.css`, `infographics/_shared/fonts` (relative link to `../../docs/assets/fonts`, use `../../docs/assets/fonts/...` URLs in `@font-face`), `infographics/src/_example-square.html`, `infographics/src/_example-og.html`, `infographics/export.ps1`

**Interfaces:**
- CSS: dark tokens from spec §3.2; canvas classes `.ig-square` (1200×1200, padding 72px) and `.ig-og` (1200×630, padding 56px); components `.ig-eyebrow` (mono 22px accent, `SERIES · PART N`), `.ig-title` (Space Grotesk 600, 76px square / 60px og, max 2 lines), `.ig-sub` (Inter 26px muted), `.ig-flow` + `.ig-node` + `.ig-node.is-key` (accent border) + `.ig-arrow`, `.ig-cards` (2–5 cards, `.ig-card` surface + line border, radius 12px, card title Space Grotesk 28px, body Inter 20px), `.ig-compare` (two columns), `.ig-callout` (accent-soft background), `.ig-footer` (`Viktor Vidman · vvidman.github.io`, mono 18px muted, bottom). SVG icons only, stroke style, no emoji, no watermark.
- `export.ps1 -Id <id>`: renders `src/<id>-square.html` and `src/<id>-og.html` with Edge (fallback Chrome) headless: `--headless=new --hide-scrollbars --force-device-scale-factor=1 --window-size=1200,1200 --screenshot=<out>` into `docs/assets/images/infographics/<id>-square.png` / `<id>-og.png`; `-All` exports every pair.

- [ ] **Step 1:** Build the example pair; export; check PNG dimensions are exactly 1200×1200 and 1200×630 (`[System.Drawing.Image]::FromFile(...).Size`).
- [ ] **Step 2:** Show both PNGs to Viktor. **Stop until he approves the look.**
- [ ] **Step 3:** Commit `feat: infographic template and export script`.

### Task 16: Per-post infographics

**Files:**
- Create: `infographics/src/<id>-square.html`, `<id>-og.html` for each post (id = post slug file name without `.html`, after Task 13 renames)
- Add: 36 PNGs in `docs/assets/images/infographics/`
- Modify: front matter `image` / `og_image` of all 18 posts; remove every `:::section fig-*` block added on 2026-11-25 (the post template now renders `figure`)
- Delete: the 12 old PNGs in `docs/assets/images/` (`chaosforge-infographic-01.png`, `closed-loop-infographics.png`, `inferrouter-0[1-4]-infograph.png`, `post-sp01-infographics.png`, `raglab-infographic-0[1-3].png`, `sourcerag-00.png`, `sourcerag_part1.png`)

**Batches (one approval stop per batch):** InferRouter (4) · RagLab + SourceRAG (3) · ChaosForge + Scaffold (3) · .NET Agentic AI (2) · Site Generator (3) · Home Lab (2).

- [ ] **Step 1 (per batch):** For each post write a content outline in `infographics/src/<id>.notes.md`: eyebrow, title, subtitle, chosen pattern (flow / cards / compare), each node/card text, and for every statement the post paragraph it comes from. RagLab merges the old three into one (two flows + five components).
- [ ] **Step 2 (per batch):** Build the HTML pair from the outline, export, and present PNGs + notes to Viktor. **Stop until approved**; apply corrections.
- [ ] **Step 3 (after all batches):** Set `image`/`og_image`, remove `fig-*` sections, delete old PNGs, build. `LinkCheckTests` and full test suite PASS; `git grep -n "infograph\|infographic-0\|sourcerag_part1" content docs/posts` returns nothing.
- [ ] **Step 4:** Commit per batch `content: infographics for <series>`, final `chore: remove legacy infographics`.

---

## Final verification (after Task 16)

- [ ] `dotnet test` — all green.
- [ ] `./preview.ps1 -Build` — 0 errors; visit `/`, `/posts/`, one series post, one standalone post, `/about/`, `/certifications/`, `/posts/post-02.html` (redirects), `/nope` (404) at 1280px and 375px, light/dark, EN/HU.
- [ ] Share a post URL in LinkedIn Post Inspector after publishing to confirm the og image (Viktor).
- [ ] Update `README.md` (front-matter keys table, test command, infographic workflow).
