---
title: The generator pipeline — from Markdown to GitHub Pages
slug: posts/post-04.html
layout: page
description: No CI/CD. No build server. A local command, a docs/ folder, and a git push. Here's the full picture.
---

:::section title
:::lang en
### The generator pipeline — from Markdown to GitHub Pages
> No CI/CD. No build server. A local command, a docs/ folder, and a git push. Here's the full picture.
:::lang
:::lang hu
### A generátor pipeline — Markdowntól a GitHub Pages-ig
> Nincs CI/CD. Nincs build szerver. Egy lokális parancs, egy docs/ mappa, és egy git push. Ez az egész.
:::lang
:::section

:::section intro
:::lang en
The first two parts covered the *why* and the *what* — why a custom generator, and what the
DSL looks like. This part is the *how*: what actually happens when the build runs, what comes
out the other end, and how it ends up on GitHub Pages.
:::lang
:::lang hu
Az első két rész a *miérttel* és a *mivel* foglalkozott — miért saját generátor, és hogyan
néz ki a DSL. Ez a rész a *hogyan*: mi történik valójában, amikor a build lefut, mi kerül
a kimenetbe, és hogyan jut el a GitHub Pages-re.
:::lang
:::section

:::section details
:::lang en
**The full pipeline in one sentence**

The generator reads a configuration file, walks the content directory, parses each Markdown
DSL file into a structured document model, renders language-specific HTML, applies the page
template, and writes the result into the `docs/` folder.

That's it. No watch mode, no hot reload, no dev server. A single command, a single pass,
deterministic output.

**Configuration: appsettings.json**

The generator is a standard .NET console application. Configuration lives in
`appsettings.json`, which keeps the build logic decoupled from file system assumptions:

```json
{
  "Paths": {
    "Input": "../content",
    "Templates": "templates",
    "Output": "../docs"
  },

  "Defaults": {
    "Languages": [ "en", "hu" ],
    "Layout": "page"
  }
}
```

Nothing is hardcoded. If the content directory moves, or the output structure changes, the
configuration changes — the generator doesn't.

**What the generator does, step by step**

1. Load configuration from `appsettings.json`
2. Read the HTML page template into memory
3. Walk the `content/` directory and collect all `.md` files
4. For each file: parse → render → write
5. Copy static assets from `assets/` to `docs/assets/`

Step 4 is where the work happens. The parser produces a `PageDocument`, the renderer
produces a finished HTML string, and the writer places it in `docs/` at the path defined
by the file's `slug` metadata field.

The slug determines the output path directly:

```
slug: posts/post-02.html  →  docs/posts/post-02.html
```

This means the URL structure is explicit and author-controlled — not derived from the
file system layout of the content directory.

**The docs/ folder structure**

GitHub Pages serves whatever is in the `docs/` folder of the repository. The output
mirrors the final public URL structure exactly:

```
docs/
├── index.html
├── about/
│   └── index.html
├── posts/
│   ├── index.html
│   ├── post-01.html
│   └── post-02.html
└── assets/
    ├── css/
    │   └── site.css
    ├── javascript/
    │   └── site.js
    └── icons/
        └── favicon.ico
```

The `docs/` folder is committed alongside the source. GitHub Pages serves it directly —
no build step on the server side, no Jekyll processing, no `.nojekyll` complications
beyond placing an empty `.nojekyll` file in the repo root to disable GitHub's default
processing.

**Local preview workflow**

Before pushing, the output is always previewed locally using `preview.ps1` — a small
PowerShell script with two modes:

```powershell
# Build and serve in one command
.\preview.ps1 -Build

# Serve the existing output without rebuilding
.\preview.ps1
```

The script runs the generator, then serves the `docs/` folder over a local HTTP server —
Python's built-in `http.server` if available, with a fallback to `dotnet-serve`:

```powershell
param(
    [int]$Port = 8080,
    [switch]$Build
)

if ($Build) {
    dotnet run --project .\blog-build\blog-build.csproj
}

Push-Location (Join-Path $PSScriptRoot "docs")

try {
    python -m http.server $Port
}
catch {
    dotnet tool install --global dotnet-serve --version 1.11.0
    dotnet serve -p $Port
}

Pop-Location
```

The preview serves the exact same output that GitHub Pages will — same HTML, same assets,
same URL structure. What you see locally is what ships.

**Why manual, and why that's a deliberate choice**

Setting up a GitHub Actions workflow here would be straightforward — a YAML file that runs
`dotnet run` and commits the `docs/` folder back to the repository. I know how to do it.
I chose not to.

The reason is that I review every change before it goes live — not just new posts, but
CSS tweaks, SEO metadata updates, fixes to older articles. And I don't just check it on
desktop. I open it on mobile too, because that's where layout issues hide.

This is human orchestration, and for my own blog it's non-negotiable. A CI/CD pipeline
would remove the friction — but that friction is the review step. Automating it would
mean trusting the build output without looking at it. That's a trade-off I'm not willing
to make here.

The flow is: write, build, review on desktop, review on mobile, push. It takes a few
minutes. It catches things that automated checks never would.

**What this series covered**

This was the third and final part of the static site generator series:

- Part 1: [Why a custom generator — the constraints and the decision](/posts/post-02.html)
- Part 2: [The DSL — one file, two languages, explicit structure](/posts/post-03.html)
- Part 3: [The pipeline — configuration, build, output, local preview](/posts/post-04.html)

The generator itself is not a framework and not a product. It's a small, focused tool
that does exactly what this blog needs. That's the point.

**Next up:** 
> A different kind of project — one that's still being built.
:::lang
:::lang hu
**A teljes pipeline egy mondatban**

A generátor beolvassa a konfigurációs fájlt, végigjárja a tartalom mappát, minden Markdown
DSL fájlt strukturált dokumentummodelként olvas fel, rendereli a nyelv-specifikus HTML-t,
alkalmazza a web sablont, és az eredményt a `docs/` mappába írja.

Ennyi. Nincs watch mode, nincs hot reload, nincs dev szerver. Egyetlen parancs, egyetlen
menet, determinisztikus kimenet.

**Konfiguráció: appsettings.json**

A generátor egy standard .NET konzolalkalmazás. A konfiguráció `appsettings.json`-ban él,
ami elválasztja a build logikát a fájlrendszer-feltevésektől:

```json
{
  "Paths": {
    "Input": "../content",
    "Templates": "templates",
    "Output": "../docs"
  },

  "Defaults": {
    "Languages": [ "en", "hu" ],
    "Layout": "page"
  }
}
```

Semmi sincs hardcode-olva. Ha a tartalom mappa megváltozik, vagy a kimenet struktúrája
módosul, a konfiguráció változik — a generátor nem.

**Mit csinál a generátor, lépésről lépésre**

1. Beolvassa a konfigurációt az `appsettings.json`-ból
2. Betölti a HTML oldalsablont a memóriába
3. Végigjárja a `content/` mappát és összegyűjti az összes `.md` fájlt
4. Minden fájlra: parse → render → írás
5. Átmásolja a statikus asseteket az `assets/`-ből a `docs/assets/`-be

A 4. lépésben történik a munka. A parser előállít egy `PageDocument`-et, a renderer
egy kész HTML stringet, az író pedig a fájl `slug` metaadat mezője által meghatározott
helyre teszi a `docs/`-ba.

A slug közvetlenül meghatározza a kimeneti útvonalat:

```
slug: posts/post-02.html  →  docs/posts/post-02.html
```

Ez azt jelenti, hogy az URL struktúra explicit és szerző által kontrollált — nem a
tartalom mappa fájlrendszer-elrendezéséből van levezetva.

**A docs/ mappa struktúra**

A GitHub Pages a repository `docs/` mappájának tartalmát szolgálja ki. A kimenet
pontosan tükrözi a végső publikus URL struktúrát:

```
docs/
├── index.html
├── about/
│   └── index.html
├── posts/
│   ├── index.html
│   ├── post-01.html
│   └── post-02.html
└── assets/
    ├── css/
    │   └── site.css
    ├── javascript/
    │   └── site.js
    └── icons/
        └── favicon.ico
```

A `docs/` mappa a forrás mellett kerül commitolásra. A GitHub Pages közvetlenül kiszolgálja
— nincs szerver oldali build lépés, nincs Jekyll feldolgozás, a repo gyökerében lévő
`.nojekyll` fájl kikapcsolja a GitHub alapértelmezett feldolgozását.

**Lokális preview workflow**

Push előtt a kimenet mindig lokálisan ellenőrzöm a `preview.ps1` scripttel — egy kis
PowerShell eszközzel, ami két módban tud futni:

```powershell
# Build és kiszolgálás egyben
.\preview.ps1 -Build

# Csak kiszolgálás, újrabuild nélkül
.\preview.ps1
```

A script lefuttatja a generátort, majd egy lokális HTTP szerveren kiszolgálja a `docs/`
mappát — Python beépített `http.server`-ével, ha elérhető, különben `dotnet-serve`-vel:

```powershell
param(
    [int]$Port = 8080,
    [switch]$Build
)

if ($Build) {
    dotnet run --project .\blog-build\blog-build.csproj
}

Push-Location (Join-Path $PSScriptRoot "docs")

try {
    python -m http.server $Port
}
catch {
    dotnet tool install --global dotnet-serve --version 1.11.0
    dotnet serve -p $Port
}

Pop-Location
```

A preview pontosan azt szolgálja ki, amit a GitHub Pages fog — ugyanaz a HTML, ugyanazok
az assetek, ugyanaz az URL struktúra. Amit lokálisan látsz, az kerül ki élesbe.

**Miért manuális, és miért szándékos döntés**

Egy GitHub Actions workflow felállítása itt triviális lenne — egy YAML fájl, ami lefuttatja
a `dotnet run`-t és visszacommitálja a `docs/` mappát. Tudom, hogyan kell megcsinálni.
Szándékosan nem csináltam meg.

Az ok az, hogy minden változtatást átnézek, mielőtt élesbe kerül — nemcsak új posztokat,
hanem CSS módosításokat, SEO metaadat frissítéseket, régebbi cikkek javításait is. És nem
csak asztali gépen ellenőrzöm. Mobilon is megnézem, mert ott bújnak meg az elrendezési
hibák.

Ez humán orchestrálás, és a saját blogom esetén ez nem kompromisszumozható. Egy CI/CD
pipeline eltávolítaná a súrlódást — de az a súrlódás maga az ellenőrzési lépés.
Az automatizálás azt jelentené, hogy a build kimenetét megtekintés nélkül elfogadom.
Ez egy olyan trade-off, amit itt nem vagyok hajlandó megtenni.

A flow: írás, build, ellenőrzés asztalon, ellenőrzés mobilon, push. Néhány percet vesz
igénybe. Olyan dolgokat is megfog, amit automatizált ellenőrzés soha nem venne észre.

**Mit fedett le ez a sorozat**

Ez volt a statikus oldalgenerátor sorozat harmadik és egyben utolsó része:

- 1. rész: [Miért saját generátor — a korlátok és a döntés](/posts/post-02.html)
- 2. rész: [A DSL — egy fájl, két nyelv, explicit struktúra](/posts/post-03.html)
- 3. rész: [A pipeline — konfiguráció, build, kimenet, lokális preview](/posts/post-04.html)

Maga a generátor nem framework és nem termék. Egy kis, fókuszált eszköz, ami pontosan
azt csinálja, amire ennek a blognak szüksége van. Ez a lényeg.

**Következő:** 
> Egy másféle projekt — ami még épül.
:::lang
:::section