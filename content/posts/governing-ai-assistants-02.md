---
title: Choosing Models Without an Eval Lab
title_hu: Modellválasztás saját mérőkörnyezet nélkül
slug: posts/governing-ai-assistants-02-choosing-models.html
description: How to choose the models for each tier when you have no internal evaluation lab: public benchmarks as a filter, five decision criteria, short trials and an eval library that grows out of real decisions.
description_hu: Hogyan válassz modellt az egyes szintekre belső mérőkörnyezet nélkül: publikus benchmarkok szűrőként, öt döntési szempont, rövid próbaidőszak és egy eval-könyvtár, ami valódi döntésekből nő ki.
layout: page
date: 2026-11-25
series: Governing AI Coding Assistants
part: 2
topic: ai-workflow
image: /assets/images/infographics/governing-ai-assistants-02-choosing-models-square.png
og_image: /assets/images/infographics/governing-ai-assistants-02-choosing-models-og.png
---

:::section intro

:::lang en
## The lab you don't have

[Part 1](/posts/governing-ai-assistants-01-model-tiers.html) gave every tier a fixed number of slots: one default, at most two alternatives. It did not say which model deserves a slot.

The textbook answer is an internal evaluation suite: your own tasks, your own codebase, repeatable scores. The honest answer is that most organisations don't have one when the decision is due, and won't have one for a while.

Waiting is not a neutral option. If the organisation doesn't decide, every developer decides alone, and you are back to the free-for-all from part 1.

So decide with what you have, and make the decision honest: state what it rests on, and what would change it.
:::lang

:::lang hu
## A labor, ami nincs

Az [1. rész](/posts/governing-ai-assistants-01-model-tiers.html) minden szintnek fix számú helyet adott: egy alapértelmezettet és legfeljebb két alternatívát. Azt nem mondta meg, melyik modell érdemli meg a helyet.

A tankönyvi válasz egy belső mérőkörnyezet: saját feladatok, saját kódbázis, megismételhető pontszámok. Az őszinte válasz az, hogy a legtöbb szervezetnek nincs ilyen, amikor dönteni kell, és egy ideig nem is lesz.

A várakozás nem semleges lehetőség. Ha a szervezet nem dönt, minden fejlesztő egyedül dönt, és visszajutottál az 1. rész „mindenki azt használ, amit akar” állapotához.

Dönts tehát abból, ami van, de becsületesen: mondd ki, mire épül a döntés, és mi változtatna rajta.
:::lang

:::section

:::section benchmarks

:::lang en
## Public benchmarks: a filter, not a verdict

Public benchmark results and the sites that aggregate them are the cheapest evidence you have. They are good at a few things:

- **narrowing the field:** a model far behind on coding tasks rarely belongs in the frontier tier;
- **spotting large gaps** between generations or price classes;
- **tracking movement:** how quickly a model family improves, or how quickly a cheaper model catches up;
- **comparing price and speed** on the same scale.

They are bad at others:

- the tasks are not your tasks, your languages or your codebase;
- scores move with every model version, and older snapshots linger;
- an aggregate score hides where a model is strong and where it is weak;
- small differences are mostly noise.

The rule that follows: use benchmarks to shortlist and to exclude clearly weaker candidates. Never choose between two close candidates on a benchmark delta alone.
:::lang

:::lang hu
## Publikus benchmarkok: szűrő, nem ítélet

A publikus benchmark-eredmények és az ezeket összesítő oldalak a legolcsóbb bizonyítékok, amelyek a rendelkezésedre állnak. Néhány dologra jók:

- **szűkítésre:** egy kódolási feladatokban messze lemaradó modell ritkán való a frontier szintre;
- **nagy különbségek észrevételére** generációk vagy árkategóriák között;
- **a mozgás követésére:** milyen gyorsan fejlődik egy modellcsalád, vagy milyen gyorsan zárkózik fel egy olcsóbb modell;
- **ár és sebesség összevetésére** ugyanazon a skálán.

Másra viszont nem:

- a feladatok nem a te feladataid, nem a te nyelveid, nem a te kódbázisod;
- a pontszámok minden modellverzióval változnak, a régi mérések pedig sokáig keringenek;
- egy összesített pontszám elfedi, hol erős és hol gyenge egy modell;
- a kis különbségek többnyire zajok.

A szabály ebből: a benchmarkokat rövid lista készítésére és az egyértelműen gyengébb jelöltek kiszűrésére használd. Két közeli jelölt között soha ne dönts pusztán egy benchmark-különbség alapján.
:::lang

:::section

:::section criteria

:::lang en
## Capability is one column of five

A model that wins on capability can still be the wrong choice for a slot. Five criteria cover most decisions:

| Criterion | Question | Evidence |
|---|---|---|
| Capability | Does it handle this class of work well? | benchmarks to shortlist, your own trial to decide |
| Cost | Does its price fit the tier? | the vendor's pricing, including any premium multiplier |
| Latency | Is it fast enough for how this tier is used? | your own trial |
| Context | Can it hold the code and history the work needs? | documented context window, your own trial |
| Lifecycle risk | How likely is it to change or disappear soon? | preview status, announced deprecations, the vendor's track record |

The weights come from part 1. The frontier tier weighs capability first. The fast iterative tier weighs latency and cost. The specialised tier weighs lifecycle risk above everything, because the workflow depends on that one model staying the same.
:::lang

:::lang hu
## A képesség csak egy oszlop az ötből

Egy modell, ami képességben nyer, egy adott helyre mégis rossz választás lehet. Öt szempont lefedi a döntések többségét:

| Szempont | Kérdés | Bizonyíték |
|---|---|---|
| Képesség | Jól kezeli ezt a munkatípust? | benchmark a szűkítéshez, saját próba a döntéshez |
| Költség | Az ára illik a szinthez? | a gyártó árazása, a prémium szorzóval együtt |
| Késleltetés | Elég gyors ahhoz, ahogy ezt a szintet használják? | saját próba |
| Kontextus | Elfér benne a kód és az előzmény, ami a munkához kell? | dokumentált kontextusablak, saját próba |
| Életciklus-kockázat | Mennyire valószínű, hogy hamarosan változik vagy megszűnik? | preview státusz, bejelentett kivezetések, a gyártó eddigi gyakorlata |

A súlyokat az 1. rész adja. A frontier szinten a képesség az első. A gyors-iteratív szinten a késleltetés és a költség. A specializált szinten az életciklus-kockázat mindennél fontosabb, mert a workflow arra épül, hogy az az egy modell ugyanolyan marad.
:::lang

:::section

:::section trial

:::lang en
## A short trial beats a long debate

The alternative slots from part 1 exist for exactly this. Put the candidate into one of them for a fixed period, and let real work decide.

- **Time-box it.** Two to four weeks is usually enough to see a pattern without the trial quietly becoming permanent.
- **Pick a few volunteers from different teams,** so the result is not one team's taste.
- **Use work they would do anyway.** No synthetic exercises; the point is your tasks, in your codebase.
- **Collect short, structured notes:** class of work, outcome, how much rework was needed, anything surprising.
- **End with a decision:** promote, keep as an alternative, or drop. A trial without a decision date is just another free-for-all.

The notes are not statistics, and they should not pretend to be. They are evidence that you can show and explain, which is more than a benchmark delta gives you.
:::lang

:::lang hu
## Egy rövid próba többet ér egy hosszú vitánál

Az 1. rész alternatív helyei pontosan erre valók. Tedd a jelöltet az egyikbe egy meghatározott időre, és hagyd, hogy a valódi munka döntsön.

- **Legyen időkorlátja.** Két-négy hét általában elég ahhoz, hogy kirajzolódjon egy minta, anélkül hogy a próba csendben véglegessé válna.
- **Válassz néhány önkéntest különböző csapatokból,** hogy az eredmény ne egyetlen csapat ízlését tükrözze.
- **Olyan munkán próbáljátok, amit amúgy is elvégeznének.** Nem mesterséges feladatokon; a lényeg a ti feladataitok, a ti kódbázisotokban.
- **Gyűjts rövid, strukturált jegyzeteket:** munkatípus, eredmény, mennyi utómunka kellett, mi volt meglepő.
- **Döntéssel zárd:** előléptetés, alternatívaként marad, vagy kikerül. Egy döntési határidő nélküli próba csak egy újabb „mindenki azt használ, amit akar”.

A jegyzetek nem statisztikák, és nem is kell annak tűnniük. Olyan bizonyítékok, amelyeket meg tudsz mutatni és meg tudsz magyarázni, és ez több, mint amit egy benchmark-különbség ad.
:::lang

:::section

:::section eval-library

:::lang en
## Grow the eval library from decisions

You don't need an evaluation project to start an evaluation library. You need a habit: every disputed decision becomes a test case.

A case is small:

- **the task,** written the way a developer would ask it;
- **the context,** a minimal, sanitised slice of code or documentation that the task needs;
- **what good looks like,** the properties a correct answer must have;
- **how to judge it,** an automated check where possible, a short rubric otherwise.

Start with five to ten cases, kept next to the model register. Rerun them whenever a new candidate appears or a default changes. Over time this library becomes the internal lab you didn't have, built from real disagreements instead of from a project plan.
:::lang

:::lang hu
## Az eval-könyvtár a döntésekből nőjön ki

Eval-könyvtárat indítani nem kell külön projekt. Egy szokás kell hozzá: minden vitatott döntésből teszteset lesz.

Egy eset kicsi:

- **a feladat,** úgy megfogalmazva, ahogy egy fejlesztő kérdezné;
- **a kontextus,** a feladathoz szükséges kód vagy dokumentáció minimális, megtisztított részlete;
- **milyen a jó válasz,** azok a tulajdonságok, amelyekkel egy helyes válasznak rendelkeznie kell;
- **hogyan ítéled meg,** ahol lehet, automatikus ellenőrzéssel, máskor egy rövid értékelési szemponttal.

Kezdd öt-tíz esettel, a modellregiszter mellett tárolva. Futtasd újra őket, valahányszor új jelölt érkezik vagy változik egy alapértelmezett modell. Idővel ebből a könyvtárból lesz a belső labor, ami eddig hiányzott, és valódi vitákból épül, nem egy projekttervből.
:::lang

:::section

:::section decision-record

:::lang en
## Write the decision down

A slot assignment deserves the same treatment as an architecture decision. One short record per decision:

- the model, the tier and the slot;
- the evidence used: benchmark snapshot and its date, trial notes, eval cases;
- known weaknesses, stated plainly;
- what would trigger a review.

The record does two jobs. It explains the decision to developers today, and it lets you revisit it later without reconstructing why it was made. Part 3 is about that revisiting.
:::lang

:::lang hu
## Írd le a döntést

Egy hely kiosztása ugyanazt a bánásmódot érdemli, mint egy architektúra-döntés. Döntésenként egy rövid feljegyzés:

- a modell, a szint és a hely;
- a felhasznált bizonyíték: benchmark-mérés és annak dátuma, próbajegyzetek, eval-esetek;
- az ismert gyengeségek, nyíltan kimondva;
- mi indítana felülvizsgálatot.

A feljegyzés két feladatot lát el. Ma elmagyarázza a döntést a fejlesztőknek, később pedig lehetővé teszi, hogy újragondold, anélkül hogy rekonstruálnod kellene, miért született. A 3. rész erről az újragondolásról szól.
:::lang

:::section

:::section checklist

:::lang en
## Checklist

Before a model takes a slot, make sure you can answer these:

1. Which benchmarks did we use to shortlist, and from what date?
2. How does the candidate score on all five criteria, with the tier's weights?
3. Did it go through a time-boxed trial on real work?
4. Which eval cases did we run, and what did they show?
5. What are its known weaknesses?
6. What would make us review this decision?
:::lang

:::lang hu
## Ellenőrzőlista

Mielőtt egy modell helyet kap, legyen válaszod ezekre:

1. Mely benchmarkokat használtuk a szűkítéshez, és milyen dátumú mérésből?
2. Hogyan teljesít a jelölt mind az öt szempont szerint, a szint súlyaival?
3. Átment egy időkorlátos próbán valódi munkán?
4. Mely eval-eseteket futtattuk, és mit mutattak?
5. Mik az ismert gyengeségei?
6. Mi indítaná el a döntés felülvizsgálatát?
:::lang

:::section

:::section next

:::lang en
## Next: keeping it alive

Every decision in this post has a date on it. Models improve, prices change, vendors retire versions on their own schedule. Part 3 is about the model register: what it records, the states a model moves through, and the two review rhythms that keep the register worth trusting.
:::lang

:::lang hu
## Következik: életben tartás

Ebben a posztban minden döntésnek dátuma van. A modellek fejlődnek, az árak változnak, a gyártók a saját ütemezésük szerint vezetnek ki verziókat. A 3. rész a modellregiszterről szól: mit rögzít, milyen állapotokon megy át egy modell, és melyik az a két felülvizsgálati ritmus, amitől a regiszter megbízható marad.
:::lang

:::section
