---
title: Why I built my own static site generator — not because the others are bad
slug: posts/post-02.html
layout: page
description: Why I built my own static site generator — not because the others are bad
---

:::section title
:::lang en
### Why I built my own static site generator — not because the others are bad
#### I never had a problem with Jekyll. I never tried it. That was intentional.
:::lang

:::lang hu
### Miért írtam saját statikus oldalgenerátort? (Nem azért, mert a többi rossz.)
#### Nem volt bajom a Jekyll-lel. Soha nem is próbáltam. Szándékos döntés volt.
:::lang
:::section

:::section intro
:::lang en
Most articles like this start with: "I tried X and hated it." This isn't that article.

When I started building this blog, I never got to the question of which generator to choose. The first question I asked myself was different: *what is this, exactly?*

A blog. Static HTML pages. Bilingual content. Hosted on GitHub Pages. From a software engineering perspective, this is a transformation pipeline: structured text goes in, HTML comes out. That's a programming problem — not a content management problem.

If you look at it that way, the question isn't *which generator to pick*. The question is: *why would you delegate that decision* to a general-purpose tool that knows nothing about your specific constraints?
:::lang
:::lang hu
A legtöbb ilyen cikk úgy kezdődik: „Kipróbáltam X-et, és utáltam." Ez nem olyan cikk.

Amikor elkezdtem ezt a blogot felépíteni, el sem jutottam odáig, hogy melyik generátort válasszam. Az első kérdés, amit feltettem magamnak, más volt: *mi ez valójában?*

Egy blog, statikus HTML oldalakból, kétnyelvű tartalommal, GitHub Pages-en. Szoftvermérnöki szempontból ez egy transzformációs pipeline: strukturált szöveg bemegy, HTML kijön. Ez egy programozási feladat – nem egy tartalomkezelési probléma.

Ha így nézel rá, a kérdés nem az, hogy *melyik generátort* válaszd. A kérdés az, hogy *miért delegálnád ezt a döntést* egy általános célú eszközre, amelynek fogalma sincs a te korlátaidról?
:::lang
:::section

:::section details
:::lang en
**The actual constraints**

In my case, the main constraint was bilingual content — but not in the way most tools handle it. I didn't want two separate Markdown files for the same article. No translation keys, no locale mappings, no duplicated directory trees. I wanted one file per article, from which the generator knows what's Hungarian and what's English.

That's not a standard feature in any general-purpose generator. You can solve it with plugins and template hacks — but then your solution is built on top of someone else's conventions. When that tool changes, your content suffers.

**The simpler path is the custom tool**

This sounds counterintuitive, but it's true: a custom tool is simpler in the long run when your problem is small and well-defined. Mine was:

- Read Markdown input
- Use a DSL to mark structure (sections, languages)
- Render HTML from a template
- Write output to `docs/`, served directly by GitHub Pages

That's it. No plugin system, no theme engine, no configuration mysticism. I wrote it in C# because that's the tool I know best — and because if someone wants to understand how I think, the code I write shows them.

**This isn't a manifesto against Jekyll**

This isn't a "build your own, it's better" manifesto. If someone wants to start a simple blog tomorrow, Hugo is highly recommended. Fast, well-documented, great community.

But when you have specific constraints and know exactly what you want — a custom tool isn't a hobby project. It's an engineering decision.

In the next part, I'll show the DSL I designed and why it ended up looking the way it does.
:::lang
:::lang hu
**A konkrét korlátok**

Az én esetemben a legfontosabb megkötés a kétnyelvűség volt – de nem úgy, ahogy a legtöbb eszköz kezeli. Nem akarok két külön Markdown fájlt ugyanahhoz a tartalomhoz. Nem akarok fordítási kulcsokat, locale-mappingeket, duplikált könyvtárstruktúrát. Azt akarom, hogy egy cikk egy fájl legyen, amelyből a generátor tudja, mi a magyar és mi az angol rész.

Ez egyetlen általános generátorban sem alapfunkció. Pluginnel, template-hackkel meg lehet oldani – de ekkor már egy más rendszer konvencióira épül a megoldásod, és ha az a rendszer változik, a te tartalmad szenved.

**Az egyszerűbb út a saját megoldás**

Ez furán hangzik, de igaz: a saját eszköz hosszú távon egyszerűbb, ha a problémád pici és jól definiált. Az enyém az volt:

- Markdown bemenetet olvasok
- DSL-lel jelölöm a struktúrát (szekciók, nyelvek)
- HTML-t generálok egy template alapján
- A kimenet a `docs/` mappába kerül, amit a GitHub Pages közvetlenül kiszolgál

Ennyi. Nincs plugin-rendszer, nincs témarendszer, nincs konfigurációs misztikum. C#-ban írtam, mert az az eszköz, amelyet a legjobban ismerek – és mert ha egy fejlesztőt az érdekel, hogy hogyan gondolkodom, akkor az általam írt kód megmutatja.

**Nem versenyezni akarok a Jekyll-lel**

Ez nem egy "csináld magad, mert jobb" kiáltvány. Ha holnap valaki egy egyszerű blogot akar indítani, ajánlom neki a Hugo-t. Gyors, jól dokumentált, nagy közösség áll mögötte.

De ha valakinek specifikus korlátai vannak, és tudja pontosan, mit akar – a saját eszköz nem hobby-projekt. Ez mérnöki döntés.

A következő részben megmutatom, hogyan néz ki a DSL, amit terveztem, és miért pont ilyen lett.
:::lang
:::section