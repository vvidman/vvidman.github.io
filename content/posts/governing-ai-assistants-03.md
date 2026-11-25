---
title: The Model Register Is a Living Document
title_hu: A modellregiszter élő dokumentum
slug: posts/governing-ai-assistants-03-model-register.html
description: How to keep an organisation's AI model choices current: what a model register records, the five states a model moves through, two review rhythms, and how to handle a model that goes away.
description_hu: Hogyan maradnak naprakészek egy szervezet AI-modelldöntései: mit rögzít a modellregiszter, milyen öt állapoton megy át egy modell, két felülvizsgálati ritmus, és mi a teendő, ha egy modell megszűnik.
layout: page
date: 2026-11-25
series: Governing AI Coding Assistants
part: 3
topic: ai-workflow
image: /assets/images/infographics/governing-ai-assistants-03-model-register-square.png
og_image: /assets/images/infographics/governing-ai-assistants-03-model-register-og.png
---

:::section intro

:::lang en
## Snapshots go stale

[Part 1](/posts/governing-ai-assistants-01-model-tiers.html) built a tier map. [Part 2](/posts/governing-ai-assistants-02-choosing-models.html) filled its slots with decisions that each carry a date. Both are snapshots.

The models behind them don't stand still. New ones appear, prices change, behaviour shifts between versions, and vendors retire models on their own schedule, not yours.

A register that isn't maintained is worse than none. Developers notice the first time it recommends a model that no longer exists, and from then on they stop reading it. Everything from the first two parts depends on the register staying worth trusting.
:::lang

:::lang hu
## A pillanatképek elavulnak

Az [1. rész](/posts/governing-ai-assistants-01-model-tiers.html) felépített egy szinttérképet. A [2. rész](/posts/governing-ai-assistants-02-choosing-models.html) a helyeit olyan döntésekkel töltötte fel, amelyek mindegyikének dátuma van. Mindkettő pillanatkép.

A mögöttük álló modellek viszont nem állnak meg. Újak jelennek meg, az árak változnak, a viselkedés verzióról verzióra eltolódik, és a gyártók a saját ütemezésük szerint vezetik ki a modelleket, nem a tiéd szerint.

Egy karbantartatlan regiszter rosszabb, mint ha nem is lenne. A fejlesztők az első alkalommal észreveszik, amikor egy már nem létező modellt ajánl, és onnantól nem olvassák. Az első két rész minden eredménye azon múlik, hogy a regiszterben meg lehessen bízni.
:::lang

:::section

:::section contents

:::lang en
## What the register records

Keep it to one table that fits on a screen. Each row is one model in one slot:

| Field | Content |
|---|---|
| Model | name and version, as the tooling shows it |
| Tier and slot | for example: workhorse, default |
| Status | one of the five states below |
| Reason | one sentence: why it is in this slot |
| Evidence | link to the decision record from part 2 |
| Owner | who answers questions about this row |
| Next review | a date, not "when needed" |
| Notes | known weaknesses, workflows that depend on it |

Excluded models live in a separate list with their own reasons, as part 1 said. Mixing "not allowed" with "allowed, not chosen" makes both lists harder to read.
:::lang

:::lang hu
## Mit rögzít a regiszter

Maradjon egyetlen táblázat, ami elfér egy képernyőn. Minden sor egy modell egy helyen:

| Mező | Tartalom |
|---|---|
| Modell | név és verzió, ahogy az eszköz mutatja |
| Szint és hely | például: igásló, alapértelmezett |
| Állapot | a lenti öt állapot egyike |
| Indoklás | egy mondat: miért van ezen a helyen |
| Bizonyíték | link a 2. részben leírt döntési feljegyzésre |
| Gazda | ki válaszol a sorral kapcsolatos kérdésekre |
| Következő felülvizsgálat | egy dátum, nem „ha szükséges” |
| Megjegyzés | ismert gyengeségek, tőle függő workflow-k |

A kizárt modellek külön listán vannak, saját indoklással, ahogy az 1. rész mondta. Ha a „nem engedélyezett” és az „engedélyezett, de nem választott” keveredik, mindkét lista nehezebben olvasható.
:::lang

:::section

:::section states

:::lang en
## Five states

A model moves through five states. Each transition has a rule, so nobody has to argue about it from scratch.

1. **Candidate.** Shortlisted from benchmarks, not yet tried on real work.
2. **Approved.** Passed a time-boxed trial and its eval cases; holds an alternative slot.
3. **Default.** The tier's first choice. A model becomes default by replacing the previous one, never by being added next to it.
4. **Deprecating.** The vendor announced a retirement date, or a better option was approved. A successor is named, and a migration window starts.
5. **Removed.** The migration window is over; the row moves to the history.

Two rules keep the states honest. A model never skips from candidate to default. And nothing stays in deprecating without an end date.
:::lang

:::lang hu
## Öt állapot

Egy modell öt állapoton megy keresztül. Minden átmenetnek szabálya van, így senkinek nem kell minden alkalommal elölről vitatkoznia róla.

1. **Jelölt.** Benchmarkok alapján rövid listára került, valódi munkán még nem próbálták.
2. **Jóváhagyott.** Átment egy időkorlátos próbán és az eval-eseteken; alternatív helyet foglal el.
3. **Alapértelmezett.** A szint első választása. Egy modell úgy lesz alapértelmezett, hogy leváltja az előzőt, soha nem úgy, hogy mellé kerül.
4. **Kivezetés alatt.** A gyártó bejelentette a megszüntetés dátumát, vagy egy jobb lehetőséget hagytak jóvá. Kijelölt utódja van, és elindul egy átállási időszak.
5. **Eltávolított.** Az átállási időszak lejárt; a sor a történetbe kerül.

Két szabály tartja tisztán az állapotokat. Egy modell soha nem ugrik jelöltből egyenesen alapértelmezetté. És semmi nem marad végdátum nélkül kivezetés alatt.
:::lang

:::section

:::section rhythms

:::lang en
## Two review rhythms

**A scheduled review, monthly.** Short, with the same agenda every time: new candidates, vendor announcements since last time, open issues from developers, and rows whose next-review date has passed. Most months the outcome is "no change", and that is fine. The point is that someone looked.

**An event-triggered review.** Vendors publish changelogs and release notes, many of them as feeds you can subscribe to. Watch them. A new model, a price change or a deprecation notice triggers an out-of-cycle review, but only if it touches a slot in the register. Everything else waits for the monthly review.

The two rhythms protect each other. The schedule guarantees attention even in quiet months. The trigger guarantees that a retirement notice doesn't sit unread for four weeks.
:::lang

:::lang hu
## Két felülvizsgálati ritmus

**Ütemezett felülvizsgálat, havonta.** Rövid, mindig ugyanazzal a napirenddel: új jelöltek, a legutóbbi óta megjelent gyártói bejelentések, a fejlesztőktől érkezett nyitott kérdések, és azok a sorok, amelyeknél lejárt a következő felülvizsgálat dátuma. A legtöbb hónapban az eredmény „nincs változás”, és ez így rendben van. A lényeg, hogy valaki ránézett.

**Eseményvezérelt felülvizsgálat.** A gyártók changelogokat és kiadási jegyzeteket tesznek közzé, sokszor feliratkozható feedként. Kövesd őket. Egy új modell, egy árváltozás vagy egy kivezetési értesítés soron kívüli felülvizsgálatot indít, de csak akkor, ha a regiszter valamelyik helyét érinti. Minden más várhat a havi felülvizsgálatig.

A két ritmus védi egymást. Az ütemezés a csendes hónapokban is garantálja a figyelmet. Az esemény pedig azt, hogy egy megszüntetési értesítés ne várjon négy hétig olvasatlanul.
:::lang

:::section

:::section deprecation

:::lang en
## When a model you rely on goes away

Retirements are the moment a register earns its keep. Handle them the same way every time:

1. **Find what depends on it.** The register's notes and the specialised tier from part 1 tell you where it will hurt.
2. **Name a successor through the normal process.** Same criteria, same trial, same eval cases as in part 2, just on a deadline.
3. **Set a migration window** that ends before the vendor's date, not on it.
4. **Tell developers what changes and when,** and what they need to do. If shared instructions reference tiers instead of model names, usually nothing.

A retirement handled this way is a routine change. Handled without a register, it is a surprise that lands on whoever happened to depend on the model.
:::lang

:::lang hu
## Amikor megszűnik egy modell, amire építesz

A kivezetéseknél derül ki, mennyit ér egy regiszter. Kezeld őket mindig ugyanúgy:

1. **Keresd meg, mi függ tőle.** A regiszter megjegyzései és az 1. rész specializált szintje megmutatják, hol fog fájni.
2. **Jelölj ki utódot a szokásos folyamattal.** Ugyanazok a szempontok, ugyanaz a próba, ugyanazok az eval-esetek, mint a 2. részben, csak határidővel.
3. **Állíts be átállási időszakot,** ami a gyártó dátuma előtt ér véget, nem pont akkor.
4. **Mondd el a fejlesztőknek, mi változik és mikor,** és mit kell tenniük. Ha a közös instrukciók szintekre hivatkoznak, nem modellnevekre, többnyire semmit.

Az így kezelt kivezetés rutinváltozás. Regiszter nélkül meglepetés, ami azon csattan, aki éppen függött a modelltől.
:::lang

:::section

:::section communication

:::lang en
## Tell developers what changed

Keep a short changelog next to the register: one line per change, with the reason. Post it where developers already read, not in a channel created for it.

One rule matters more than the rest: never change a default silently. Developers who notice a different model behind the same tier without an explanation stop trusting the whole setup, and they will be right to.
:::lang

:::lang hu
## Mondd el a fejlesztőknek, mi változott

Vezess egy rövid változásnaplót a regiszter mellett: változásonként egy sor, indoklással. Oda tedd közzé, ahol a fejlesztők amúgy is olvasnak, ne egy erre létrehozott csatornán.

Egy szabály mindennél fontosabb: alapértelmezett modellt soha ne cserélj csendben. Ha a fejlesztők magyarázat nélkül másik modellt vesznek észre ugyanazon szint mögött, elveszítik a bizalmukat az egész rendszerben, és jogosan.
:::lang

:::section

:::section checklist

:::lang en
## Checklist

To check that your register is alive, make sure you can answer these:

1. Does every row have a status, an owner and a next-review date?
2. When did the last monthly review happen, and what did it decide?
3. Which vendor feeds are we watching, and who reads them?
4. Which models are deprecating, who is the successor, and when does the window end?
5. Which workflows depend on a single model?
6. Where can a developer see what changed, and why?
:::lang

:::lang hu
## Ellenőrzőlista

Hogy lásd, él-e a regisztered, legyen válaszod ezekre:

1. Minden sornak van állapota, gazdája és következő felülvizsgálati dátuma?
2. Mikor volt az utolsó havi felülvizsgálat, és mit döntött?
3. Mely gyártói feedeket követjük, és ki olvassa őket?
4. Mely modellek vannak kivezetés alatt, ki az utódjuk, és mikor ér véget az átállási időszak?
5. Mely workflow-k függenek egyetlen modelltől?
6. Hol látja egy fejlesztő, mi változott és miért?
:::lang

:::section

:::section closing

:::lang en
## Governance is not prohibition

Across the three parts, the goal was never to restrict developers. It was to make the organisation's choices about AI models visible, explained and reversible: a tier map that matches models to work, decisions that state their evidence, and a register that keeps both current.

That is what "the human orchestrates" means at the level of an organisation. Not choosing every model by hand, but owning the structure in which those choices are made.
:::lang

:::lang hu
## Az irányítás nem tiltás

A három rész során a cél soha nem a fejlesztők korlátozása volt, hanem az, hogy a szervezet AI-modellekkel kapcsolatos döntései láthatók, megindokoltak és visszafordíthatók legyenek: egy szinttérkép, ami a modelleket a munkához illeszti, döntések, amelyek kimondják, mire épülnek, és egy regiszter, ami mindkettőt naprakészen tartja.

Szervezeti szinten ezt jelenti az, hogy „az ember orchestrál”. Nem azt, hogy minden modellt kézzel választasz ki, hanem azt, hogy tiéd a keret, amiben ezek a döntések születnek.
:::lang

:::section
