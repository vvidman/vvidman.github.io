---
title: Designing a minimal DSL for bilingual Markdown content
title_hu: Minimális DSL tervezése kétnyelvű Markdown tartalomhoz
slug: posts/post-03.html
description: One file. Two languages. No duplication. Here's the syntax that makes it work.
layout: page
date: 2026-03-02
series: SiteGenerator
part: 2
---

:::section title
:::lang en
### Designing a minimal DSL for bilingual Markdown content
> One file. Two languages. No duplication. Here's the syntax that makes it work.
:::lang

:::lang hu
### Minimális DSL tervezése kétnyelvű Markdown tartalomhoz
> Egy fájl. Két nyelv. Duplikáció nélkül. Ez a szintaxis, ami működővé teszi.
:::lang
:::section

:::section intro
:::lang en
The first design decision was the hardest to explain: every article lives in a single file.
Not one file per language. Not a source file with a translation sidecar. One file — and the
generator figures out the rest.

This sounds obvious until you try to make it work. Plain Markdown has no concept of language
blocks. You need a way to tell the parser: *this part is English, that part is Hungarian,
and they belong to the same section of the same article.*

That's where the DSL comes in.
:::lang
:::lang hu
Az első tervezési döntés volt a legnehezebben megmagyarázható: minden cikk egyetlen fájlban él.
Nem egy fájl nyelvenként. Nem forrásfájl mellé fordítási melléklet. Egy fájl — a generátor
kitalálja a többit.

Ez nyilvánvalónak hangzik, amíg meg nem próbálod megvalósítani. A sima Markdown nem ismeri
a nyelvi blokkok fogalmát. Kell valami, amivel el lehet mondani a parsernek: *ez a rész
angol, az a rész magyar, és ugyanannak a cikknek ugyanabba a szekciójába tartoznak.*

Erre való a DSL.
:::lang
:::section

:::section details
:::lang en
**What the file actually looks like**

Here's the real structure of an article file — this is the source of the post you're reading now:

```
---
title: Why I built my own static site generator
slug: posts/post-02.html
layout: page
description: Why I built my own static site generator — not because the others are bad
---

:::section intro
:::lang en
Most articles like this start with: "I tried X and hated it."
This isn't that article.
:::lang

:::lang hu
A legtöbb ilyen cikk úgy kezdődik: „Kipróbáltam X-et, és utáltam."
Ez nem olyan cikk.
:::lang
:::section
```

The file has two parts: a YAML frontmatter block at the top, followed by an ordered sequence
of sections. Each section contains one or more language blocks. Each language block is plain
Markdown.

**Why `:::`?**

The `:::` prefix was the first candidate and it stayed. The reasoning was simple:

- it doesn't appear in normal prose or code
- it's visually distinct from Markdown syntax
- it reads as a block delimiter without needing an end-tag on the same line
- it's easy to parse with a line-by-line scanner

An alternative would have been HTML comments (`<!-- section:intro -->`), but that would mix
HTML into Markdown source, which felt wrong. YAML frontmatter extensions were another option,
but putting structural content into metadata quickly becomes unreadable.

The `:::` felt like the right amount of ceremony — enough to be explicit, not enough to get
in the way.

**Two levels, not one**

The DSL has two nesting levels:

1. `:::section name` — a named content block within the page
2. `:::lang en` / `:::lang hu` — a language-specific block within a section

Sections give the content explicit structure: `title`, `intro`, `details`, `conclusion` —
whatever makes sense for the article. This matters for rendering, but also for long-term
maintainability: when you open a file six months later, you know exactly where each part is.

Language blocks are always paired inside a section. Both languages live together, which means
if you update a section, you see both versions side by side. No drift between translations.

**What the template receives**

The generator renders each language block independently into HTML, then injects the result
into the page template via simple placeholders:

```html
<div class="lang-content" id="content-en">
  {{content_en}}
</div>
<div class="lang-content" id="content-hu">
  {{content_hu}}
</div>
```

Both language versions exist in the DOM at the same time. A small JavaScript snippet handles
switching — no server round-trips, no re-renders, no framework needed. The active language
is shown, the other is hidden.

This also means both languages are indexable by search engines, and the page works without
JavaScript enabled.

**What the parser does**

The parser is a pure C# component — input is a file path, output is a structured document
model. It processes the file line by line:

- YAML frontmatter → `Metadata`
- `:::section name` → opens a new `Section`
- `:::lang en` → opens a `LanguageBlock` within the current section
- `:::lang` (closing) → closes the current language block
- `:::section` (closing) → closes the current section
- anything else → content line, appended to the current language block

No regex. No recursive descent. A simple state machine, because the grammar doesn't need
anything more complex.

**What I'd change**

The DSL works well for articles. If I ever needed nested sections, or per-section metadata,
the syntax would need to evolve — but that's a problem I don't have yet. Extending it would
be straightforward: the parser is isolated, the model is typed, and the renderer doesn't
care about grammar details.

Explicit over clever. That was the principle. I think it held.

In the next part: the generator pipeline — how the parser output becomes a finished HTML file,
and how the build integrates with GitHub Pages.
:::lang
:::lang hu
**Hogyan néz ki valójában egy fájl**

Ez az egyik valódi cikkfájl struktúrája — pontosan az, amit most olvasol forrásként:

```
---
title: Miért írtam saját statikus oldalgenerátort?
slug: posts/post-02.html
layout: page
description: Miért írtam saját statikus oldalgenerátort? (Nem azért, mert a többi rossz.)
---

:::section intro
:::lang en
Most articles like this start with: "I tried X and hated it."
This isn't that article.
:::lang

:::lang hu
A legtöbb ilyen cikk úgy kezdődik: „Kipróbáltam X-et, és utáltam."
Ez nem olyan cikk.
:::lang
:::section
```

A fájlnak két része van: felül egy YAML frontmatter blokk, utána egy szekciókból álló,
rendezett struktúra. Minden szekció egy vagy több nyelvi blokkot tartalmaz. Minden nyelvi
blokk sima Markdown.

**Miért `:::`?**

A `:::` prefix volt az első jelölt, és maradt is. Az indoklás egyszerű volt:

- nem fordul elő normál szövegben vagy kódban
- vizuálisan elkülönül a Markdown szintaxistól
- blokk-határolóként olvasható anélkül, hogy ugyanabban a sorban záró tag-re lenne szükség
- egyszerűen értelmezhető soros scannerrel

Alternatíva lett volna a HTML komment (`<!-- section:intro -->`), de HTML-t keverni Markdown
forrásba pontatlannak éreztem. YAML frontmatter kiterjesztés is szóba jött, de strukturális
tartalmat metaadatba tenni gyorsan olvashatatlanná válik.

A `:::` pont annyi ceremóniának érződött, amennyi szükséges — elég explicit, de nem tolakodó.

**Két szint, nem egy**

A DSL-nek két egymásba ágyazott szintje van:

1. `:::section name` — egy elnevezett tartalmi blokk az oldalon belül
2. `:::lang en` / `:::lang hu` — egy nyelv-specifikus blokk a szekción belül

A szekciók explicit struktúrát adnak a tartalomnak: `title`, `intro`, `details`,
`conclusion` — ami az adott cikknek illik. Ez a renderelés szempontjából fontos, de
hosszú távú karbantarthatóság miatt is: ha hat hónap múlva megnyitom a fájlt, pontosan
tudom, hol van minden rész.

A nyelvi blokkok mindig párban vannak egy szekción belül. Mindkét nyelv egymás mellett él,
ami azt jelenti, hogy ha frissíted a szekciót, egyszerre látod mindkét verziót.
Nem csúszik el a fordítás.

**Mit kap a template**

A generátor minden nyelvi blokkot önállóan renderel HTML-be, majd az eredményt egyszerű
placeholder-eken keresztül injektálja az oldalsablonba:

```html
<div class="lang-content" id="content-en">
  {{content_en}}
</div>
<div class="lang-content" id="content-hu">
  {{content_hu}}
</div>
```

Mindkét nyelvi verzió egyszerre él a DOM-ban. Egy kis JavaScript snippet kezeli a váltást —
nincs szerver-kommunikáció, nincs újrarenderelés, nem kell framework. Az aktív nyelv látható,
a másik el van rejtve.

Ez azt is jelenti, hogy mindkét nyelv indexálható a keresők számára, és az oldal JavaScript
nélkül is működik.

**Mit csinál a parser**

A parser egy tiszta C# komponens — bemenet egy fájlútvonal, kimenet egy strukturált
dokumentummodell. Soronként dolgozza fel a fájlt:

- YAML frontmatter → `Metadata`
- `:::section name` → új `Section` nyílik
- `:::lang en` → `LanguageBlock` nyílik az aktuális szekción belül
- `:::lang` (záró) → az aktuális nyelvi blokk lezárul
- `:::section` (záró) → az aktuális szekció lezárul
- minden más → tartalomsor, az aktuális nyelvi blokkhoz hozzáfűzve

Nincs regex. Nincs rekurzív leszállás. Egyszerű állapotgép, mert a grammatika nem igényel
ennél bonyolultabbat.

**Mit változtatnék**

A DSL cikkekre jól működik. Ha valaha egymásba ágyazott szekciókra vagy szekció-szintű
metaadatokra lenne szükség, a szintaxisnak fejlődnie kellene — de ez még nem megoldandó
probléma. A kiterjesztés egyértelmű lenne: a parser izolált, a modell típusos, a renderer
nem foglalkozik grammatikai részletekkel.

Explicit a trükkös helyett. Ez volt az elv. Azt hiszem, tartotta magát.

A következő részben: a generátor pipeline — hogyan lesz a parser kimenetéből kész HTML fájl,
és hogyan illeszkedik a build a GitHub Pages-be.
:::lang
:::section