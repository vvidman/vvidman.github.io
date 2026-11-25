---
title: Not Everyone Needs the Frontier Model
title_hu: Nem kell mindenkinek a legerősebb modell
slug: posts/governing-ai-assistants-01-model-tiers.html
description: Why the models behind an organisation's AI coding assistants should be tiered by type of work, with one default and at most two alternatives per tier.
description_hu: Miért érdemes egy szervezetben az AI kódoló asszisztensek modelljeit munkatípus szerint szintekbe sorolni, szintenként egy alapértelmezett és legfeljebb két alternatív modellel.
layout: page
date: 2026-11-25
series: Governing AI Coding Assistants
part: 1
topic: ai-workflow
image: /assets/images/infographics/governing-ai-assistants-01-model-tiers-square.png
og_image: /assets/images/infographics/governing-ai-assistants-01-model-tiers-og.png
---

:::section intro

:::lang en
## The question nobody owns

When a development organisation adopts an AI coding assistant, the first visible decision is the licence. The second one is quieter and lasts much longer: which models are developers allowed to use, and for what?

In most organisations this question has no owner. The assistant ships with a model picker, the list grows every month, and every developer makes their own call. Usually by habit, sometimes by hype.

This series is about giving that question an owner and a method. Not a policy document nobody reads, but a small set of decisions that keep the human in control at the level of the organisation, not just at the level of a single prompt.

Part 1 starts with the structure everything else hangs on: model tiers.
:::lang

:::lang hu
## A kérdés, aminek nincs gazdája

Amikor egy fejlesztői szervezet bevezet egy AI kódoló asszisztenst, az első látható döntés a licenc. A második csendesebb, és sokkal tovább hat: milyen modelleket használhatnak a fejlesztők, és mire?

A legtöbb szervezetben ennek a kérdésnek nincs gazdája. Az asszisztens egy modellválasztóval érkezik, a lista hónapról hónapra hosszabb, és mindenki maga dönt. Többnyire megszokásból, néha a hype alapján.

Ez a sorozat arról szól, hogyan kap ez a kérdés gazdát és módszert. Nem egy szabályzatot, amit senki nem olvas el, hanem néhány döntést, amelyekkel az irányítás az embernél marad, és nem csak egy-egy prompt szintjén, hanem az egész szervezetén.

Az 1. rész azzal a szerkezettel kezdi, amire minden más épül: a modellek szintekbe sorolásával.
:::lang

:::section

:::section two-extremes

:::lang en
## Two ways to get it wrong

**Free for all.** Every model in the picker is available to everyone. It looks like freedom. In practice:

- cost becomes unpredictable, because premium models are often billed at a multiple of the standard rate;
- results stop being comparable, because two developers solving the same kind of task are using different engines;
- nobody can tell which choices were deliberate and which were accidental.

**One size fits all.** One model for everyone, for everything. Easy to govern, but either you pay frontier prices for renaming variables, or you hand a mid-range model a cross-cutting refactor and blame the developer when it fails.

Both extremes skip the same step: matching the model to the kind of work.
:::lang

:::lang hu
## Két rossz véglet

**Mindenki azt használ, amit akar.** A választó összes modellje mindenkinek elérhető. Szabadságnak tűnik. A gyakorlatban:

- a költség kiszámíthatatlanná válik, mert a prémium modellek használata gyakran a normál díj többszöröse;
- az eredmények nem összehasonlíthatók, mert két fejlesztő ugyanazt a feladattípust más motorral oldja meg;
- senki nem tudja megmondani, melyik választás volt tudatos és melyik véletlen.

**Mindenkinek ugyanaz.** Egy modell mindenkinek, mindenre. Könnyű irányítani, de vagy frontier árat fizetsz egy változó átnevezéséért, vagy egy közepes modellre bízol egy több rétegen átnyúló refaktort, és utána a fejlesztőt hibáztatod, amikor elbukik.

Mindkét véglet ugyanazt a lépést hagyja ki: a modell hozzáillesztését a munka típusához.
:::lang

:::section

:::section tiers

:::lang en
## A tier is a class of work, not a model

Start from the work, not from the model list. Four classes cover most of what developers ask an assistant to do:

| Tier | Typical work | What matters most |
|---|---|---|
| Frontier | multi-step changes, architecture-level reasoning, unfamiliar code | capability |
| Workhorse | daily development: features, fixes, tests, reviews | balance of quality, speed and cost |
| Fast iterative | small, repetitive steps: renames, formatting, boilerplate, quick questions | latency and cost |
| Specialised | a workflow built around one specific model's behaviour | stability of that one model |

The tier names describe work. Models move between tiers as they improve or get cheaper; the tiers themselves stay stable. That is the point: developers learn "use the workhorse tier for this", not a model name that will be deprecated next quarter.

The specialised tier deserves a note. Some workflows, such as an agent setup, a review pipeline or a prompt library, are tuned to one model's quirks. Swapping the model silently breaks them. Giving them their own tier makes that dependency visible instead of accidental.
:::lang

:::lang hu
## A szint munkatípus, nem modell

A munkából indulj ki, ne a modellistából. Négy munkatípus lefedi a legtöbbet abból, amire a fejlesztők egy asszisztenst használnak:

| Szint | Jellemző munka | Mi számít leginkább |
|---|---|---|
| Frontier | többlépéses változtatások, architektúra-szintű gondolkodás, ismeretlen kód | képesség |
| Igásló | napi fejlesztés: funkciók, javítások, tesztek, review | minőség, sebesség és költség egyensúlya |
| Gyors-iteratív | kis, ismétlődő lépések: átnevezés, formázás, boilerplate, gyors kérdések | késleltetés és költség |
| Specializált | egy workflow, ami egy konkrét modell viselkedésére épül | annak az egy modellnek a stabilitása |

A szintek neve a munkát írja le. A modellek vándorolnak a szintek között, ahogy jobbak vagy olcsóbbak lesznek, maguk a szintek viszont stabilak. Pont ez a lényeg: a fejlesztő azt tanulja meg, hogy „erre az igásló szint való”, nem egy modellnevet, amit a következő negyedévben kivezetnek.

A specializált szint külön említést érdemel. Bizonyos workflow-k, például egy agent-összeállítás, egy review pipeline vagy egy prompt-könyvtár, egy modell sajátosságaira vannak hangolva. Ha a modellt csendben lecseréled, eltörnek. Ha saját szintet kapnak, ez a függőség láthatóvá válik, és nem véletlenül derül ki.
:::lang

:::section

:::section defaults

:::lang en
## One default, at most two alternatives

Each tier gets exactly one default and at most two alternatives.

- **The default** is what developers should reach for without thinking. Most of the work happens here, which makes results comparable and support possible.
- **The alternatives** exist for a stated reason: a different strength, a fallback when the default is slow or unavailable, or a candidate under evaluation.
- **Three is the ceiling.** Beyond that, the picker becomes a menu again, and you are back to free for all with extra paperwork.

The limit has a useful side effect. Every time someone wants to add a model, it forces the honest question: which one does it replace?
:::lang

:::lang hu
## Egy alapértelmezett, legfeljebb két alternatíva

Minden szint pontosan egy alapértelmezett és legfeljebb két alternatív modellt kap.

- **Az alapértelmezett** az, amihez a fejlesztő gondolkodás nélkül nyúl. A munka nagy része itt történik, így az eredmények összehasonlíthatók, és a támogatás is megoldható.
- **Az alternatíváknak** kimondott oka van: más erősség, tartalék arra az esetre, ha az alapértelmezett lassú vagy elérhetetlen, vagy egy éppen értékelés alatt álló jelölt.
- **A plafon három.** Ennél több esetén a választó újra étlappá válik, és visszajutottál a „mindenki azt használ, amit akar” állapotba, csak több papírmunkával.

A korlátnak van egy hasznos mellékhatása. Valahányszor valaki új modellt akar felvenni, kikényszeríti az őszinte kérdést: melyiket váltja le?
:::lang

:::section

:::section where-to-decide

:::lang en
## Decide centrally, start simple

Tiers can be assigned per team, per repository or for the whole organisation. Start at the organisation level with a single tier map. Per-team differences are real, but they are a second-order optimisation, and every split doubles the number of decisions you have to review later.

Split only when a team can name the class of work the shared map gets wrong.

Who decides matters as much as where. A small group with a clear mandate, such as the architects or whoever owns technical standards, beats a wide committee. The goal is not consensus on every model, but a decision that is visible, explained and reversible.
:::lang

:::lang hu
## Központi döntés, egyszerű kezdés

A szinteket lehet csapatonként, repónként vagy az egész szervezetre beállítani. Kezdd szervezeti szinten, egyetlen szinttérképpel. A csapatok közötti különbségek valósak, de ez másodrendű optimalizálás, és minden szétválasztás megduplázza a később felülvizsgálandó döntések számát.

Akkor válaszd szét, ha egy csapat meg tudja nevezni azt a munkatípust, amit a közös térkép rosszul kezel.

Az, hogy ki dönt, ugyanannyira fontos, mint az, hogy hol. Egy kis csoport világos felhatalmazással, például az architektek vagy akik a technikai szabványokért felelnek, jobb, mint egy széles bizottság. A cél nem az, hogy minden modellről konszenzus legyen, hanem egy látható, megindokolt és visszafordítható döntés.
:::lang

:::section

:::section in-practice

:::lang en
## From map to practice

A tier map that lives in a document nobody opens changes nothing. Three things make it part of daily work.

**Find the classes of work from real usage, not from a whiteboard.** Ask a few developers from different teams to list what they used the assistant for last week. The four classes above are a starting point; rename or merge them if your list says otherwise. A tier nobody can map their work to is a tier too many.

**Make the default the path of least resistance.** If the tooling allows it, set the workhorse default as the preselected model. Reference tiers, not model names, in shared instructions, templates and onboarding material. Then the model behind a tier can change without rewriting every guide.

**Publish the reasoning, not just the result.** For each slot, one sentence on why this model is there and what would make you replace it. Developers accept a constraint they understand. They work around one they don't.

None of this requires new infrastructure. A short page, a clear owner and a predictable review rhythm are enough to start.
:::lang

:::lang hu
## A térképtől a gyakorlatig

Egy szinttérkép, ami egy soha meg nem nyitott dokumentumban él, semmin nem változtat. Három dolog teszi a napi munka részévé.

**A munkatípusokat a valódi használatból keresd meg, ne a táblánál.** Kérj meg néhány fejlesztőt különböző csapatokból, hogy írják össze, mire használták az asszisztenst az elmúlt héten. A fenti négy típus kiindulópont; nevezd át vagy vond össze őket, ha a lista mást mutat. Ha egy szinthez senki nem tudja a munkáját hozzárendelni, az eggyel több a kelleténél.

**Az alapértelmezett legyen a legkisebb ellenállás útja.** Ha az eszköz engedi, az igásló szint alapértelmezett modellje legyen előre kiválasztva. A közös instrukciókban, sablonokban és az onboarding anyagokban szintekre hivatkozz, ne modellnevekre. Így a szint mögötti modell lecserélhető anélkül, hogy minden útmutatót át kellene írni.

**Az indoklást tedd közzé, ne csak az eredményt.** Minden helyhez egy mondat arról, miért ez a modell van ott, és mi miatt cserélnéd le. A fejlesztők elfogadják a korlátot, ha értik. Ha nem értik, megkerülik.

Ehhez nem kell új infrastruktúra. Egy rövid oldal, egy egyértelmű gazda és kiszámítható felülvizsgálati ritmus elég az induláshoz.
:::lang

:::section

:::section what-tiers-are-not

:::lang en
## What tiers are not

- **Not a ranking of people.** Junior developers are not restricted to cheap models. The class of work decides, not seniority.
- **Not a ban list.** Excluding models, for licensing, data-handling or quality reasons, is a separate decision with its own criteria. Tiers organise what is already allowed.
- **Not permanent.** A tier map is a snapshot. Part 3 of this series covers how to keep it alive.
:::lang

:::lang hu
## Mi nem a szint

- **Nem az emberek rangsora.** A junior fejlesztők nincsenek az olcsó modellekre korlátozva. A munka típusa dönt, nem a tapasztalat.
- **Nem tiltólista.** Modellek kizárása licenc, adatkezelés vagy minőség miatt külön döntés, saját szempontokkal. A szintek azt rendezik, ami már engedélyezett.
- **Nem végleges.** A szinttérkép egy pillanatkép. A sorozat 3. része arról szól, hogyan tartsd életben.
:::lang

:::section

:::section checklist

:::lang en
## Checklist

Before you publish a tier map, make sure you can answer these:

1. Which classes of work do our developers actually use the assistant for?
2. For each class: what matters most, capability, latency, cost or stability?
3. Which model is the default in each tier, and why?
4. Which alternatives exist, and what is each one for?
5. Which workflows depend on one specific model?
6. Who owns the map, and how does someone propose a change?
:::lang

:::lang hu
## Ellenőrzőlista

Mielőtt közzéteszel egy szinttérképet, legyen válaszod ezekre:

1. Valójában milyen munkatípusokra használják a fejlesztőink az asszisztenst?
2. Az egyes típusoknál mi számít leginkább: képesség, késleltetés, költség vagy stabilitás?
3. Melyik modell az alapértelmezett az egyes szinteken, és miért?
4. Milyen alternatívák vannak, és mire való mindegyik?
5. Mely workflow-k függenek egy konkrét modelltől?
6. Kié a térkép, és hogyan javasolhat valaki változtatást?
:::lang

:::section

:::section next

:::lang en
## Next: choosing the models

A tier map only says where a model goes. It does not say which model deserves the slot. Most organisations have to make that call without an internal evaluation lab. Part 2 is about doing it honestly anyway: public benchmarks as a filter rather than a verdict, and an evaluation library that grows out of real decisions.
:::lang

:::lang hu
## Következik: a modellek kiválasztása

A szinttérkép csak azt mondja meg, hová kerül egy modell. Azt nem, hogy melyik modell érdemli meg a helyet. A legtöbb szervezetnek ezt saját mérőkörnyezet nélkül kell eldöntenie. A 2. rész arról szól, hogyan lehet ezt mégis becsületesen csinálni: a publikus benchmarkok szűrőként, nem ítéletként, és egy eval-könyvtár, ami valódi döntésekből nő ki.
:::lang

:::section
