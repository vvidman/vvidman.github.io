---
title: The AI Executes. You Decide.
title_hu: AI végrehajt. Te döntesz.
slug: posts/post-sp01.html
layout: page
description: How Scaffold Protocol keeps the human as orchestrator while using AI as a transformation engine — with a real validation layer and error-driven refinement.
date: 2026-03-20
series: ScaffoldProtocol
part: 1
---

:::section title
:::lang en
### Scaffold Protocol - The AI Executes. You Decide.
> The birth of an AI-powered development protocol - one that fits my obsession with control.
:::lang
:::lang hu
### Scaffold Protocol - AI végrehajt. Te döntesz.
> Egy AI használó fejlesztési protokoll születése - ami megfelel az irányítás mániámnak.
:::lang
:::section

:::section intro

:::lang en
It started with smaller things. Refactoring legacy code into something unit-testable. Analyzing a bug report with a log file and a happy-path trace. Writing and updating unit tests. These were tasks I'd been doing with AI assistance for a while, and they worked well — when I was driving.

The pattern I noticed: the AI was most useful when the task was narrowly defined and the output format was predictable. It fell apart when the scope was open-ended, when it had to make architectural decisions, or when I couldn't immediately verify whether the output was correct.

So the question became: what if I built a system that enforced those conditions deliberately? Not just for one task — but for an entire development workflow?

I wanted to use AI in my development workflow. But I didn't want the AI to drive it.

That's the tension Scaffold Protocol was built to resolve. Not "how do I get the AI to do more?" — but "how do I keep control while using AI to move faster?"

The answer turned out to be architectural. The system has two distinct roles: a **human orchestrator** who decides what happens next, and an **AI transformation engine** that executes exactly one task at a time. Nothing more.

This article is about how that architecture works, where it breaks down, and what five real test runs taught me about the boundary between automation and judgment.
:::lang

:::lang hu
Kisebb dolgokkal kezdődött. Legacy kód refaktorálása unit tesztelhetőre. Bug report elemzése naplófájllal és happy path mentén. Unit tesztek írása és aktualizálása. Ezeket a feladatokat már egy ideje AI segítséggel csináltam, és jól működtek — amikor én vezéreltem a folyamatot.

A minta amit észrevettem: az AI akkor volt a leghasznosabb, amikor a feladat szűken volt definiálva és az output formátuma kiszámítható volt. Szétesett, amikor nyílt volt a hatókör, amikor architekturális döntéseket kellett hozni, vagy amikor nem tudtam azonnal ellenőrizni hogy az output helyes-e.

A kérdés tehát az lett: mi lenne, ha felépítenék egy rendszert, amely ezeket a feltételeket szándékosan kikényszeríti? Nem csak egy feladathoz — hanem egy teljes fejlesztési workflow-hoz?

AI-t akartam használni a fejlesztési folyamatomban. De nem akartam, hogy az AI vezesse azt.

Ez az a feszültség, amit a Scaffold Protocol feloldani próbál. Nem az volt a kérdés, hogy "hogyan csináltassak többet az AI-val?" — hanem az, hogy "hogyan tartsam meg az irányítást, miközben az AI-t használom a gyorsabb haladáshoz?"

A válasz architekturális lett. A rendszernek két elkülönített szerepe van: egy **human orchestrator**, aki eldönti mi következik, és egy **AI transzformációs motor**, amely egyetlen feladatot hajt végre egyszerre. Nem többet.

Ez a cikk arról szól, hogyan működik ez az architektúra, hol törik el, és mit tanított öt éles tesztfutás az automatizáció és az ítéletalkotás határáról.
:::lang

:::section

:::section principle

:::lang en
## The Human Is the Orchestrator

The AI has one job: given a defined input, produce a defined output in a specific format. It doesn't plan. It doesn't orchestrate. It doesn't decide whether this output should trigger the next step.

That's my job.

This is a deliberate design decision — not a concession to the model's limitations. A capable LLM could probably make reasonable architectural decisions. But *probably reasonable* is not a property I want in a development tool. I want predictability. I want to know exactly when the AI acted and what it produced. I want to be able to reject an output and understand why.

The moment the AI starts driving the process, you lose that. You get emergent behavior instead of designed behavior. That might be fine for exploration. It's not what I want when I'm building something I'll maintain.

One more thing worth naming: Scaffold Protocol is **not a pipeline**. A pipeline implies that input flows through a chain of steps automatically. Here, every step is a deliberate human decision. The steps affect each other — the output of one becomes the input of the next — but the human decides when and whether each step runs.
:::lang

:::lang hu
## A Human az Orchestrator

Az AI egyetlen feladata: adott inputból adott formátumú outputot generálni. Nem tervez. Nem orchestrál. Nem dönti el, hogy az output triggerel-e egy következő lépést.

Ez az én dolgom.

Ez tudatos tervezési döntés — nem a modell képességeinek hiányából fakad. Egy alkalmas LLM valószínűleg elfogadható architekturális döntéseket is hozhatna. De a *valószínűleg elfogadható* nem az a tulajdonság, amit egy fejlesztési eszközben keresek. Kiszámíthatóságot keresek. Azt akarom, hogy pontosan tudjam mikor lépett az AI, és mit produkált. Azt akarom, hogy el tudjam utasítani egy outputot, és értsem miért.

Amint az AI kezdi vinni a folyamatot, ezt elveszíted. Tervezett viselkedés helyett emergáló viselkedést kapsz. Ez talán jó explorációra. Nem ezt akarom, amikor olyat építek, amit majd karbantartok.

Érdemes még egy dolgot nevesíteni: a Scaffold Protocol **nem pipeline**. A pipeline azt implikálja, hogy az input automatikusan folyik végig egy láncon. Itt minden lépés tudatos emberi döntés. A lépések hatással vannak egymásra — az egyik outputja a másik inputja lesz — de a human dönti el mikor és hogy lefut-e az adott lépés.
:::lang

:::section

:::section step_anatomy

:::lang en
## How a Step Works

Every step in Scaffold Protocol is defined by a YAML configuration file. The config contains everything the AI needs for that step — and nothing more.

```yaml
name: task_breakdown
description: Decompose a feature description into a structured task list

system_prompt: |
  You are a task decomposition assistant for a .NET project.
  Given a feature description, produce a structured YAML task list.
  Each task must have: id, title, affected_files, and estimated_complexity.
  End your response with the stop token: ---END---
  Do not include any commentary outside the YAML structure.

input:
  schema: feature_description
  format: plain_text

output:
  schema: task_list
  format: yaml
  stop_token: "---END---"

constraints:
  max_tasks: 10
  forbidden_affected_files:
    - Program.cs
    - appsettings.json
```

Each step has its own system prompt, its own input and output schema, and its own constraints. A step doesn't know about other steps. It doesn't have access to project history beyond what the orchestrator explicitly provides. It sees exactly what I decide to give it.

This isolation is intentional. Steps that know too much about surrounding context start making implicit assumptions. And implicit assumptions in AI output are the hardest kind to catch.
:::lang

:::lang hu
## Hogyan Épül Fel Egy Step?

A Scaffold Protocol minden lépése egy YAML konfigurációs fájllal van definiálva. A konfig tartalmaz mindent, amire az AI-nak szüksége van az adott lépéshez — és nem többet.

```yaml
name: task_breakdown
description: Egy feature leírás lebontása strukturált task listává

system_prompt: |
  You are a task decomposition assistant for a .NET project.
  Given a feature description, produce a structured YAML task list.
  Each task must have: id, title, affected_files, and estimated_complexity.
  End your response with the stop token: ---END---
  Do not include any commentary outside the YAML structure.

input:
  schema: feature_description
  format: plain_text

output:
  schema: task_list
  format: yaml
  stop_token: "---END---"

constraints:
  max_tasks: 10
  forbidden_affected_files:
    - Program.cs
    - appsettings.json
```

Minden lépésnek saját system promptja van, saját input és output sémája, és saját constraintjei. Egy step nem tud a többi lépésről. Nincs hozzáférése a projekt előzményeihez azon túl, amit az orchestrator explicit módon megad. Pontosan azt látja, amit én odaadok neki.

Ez az izoláció szándékos. Az olyan lépések, amelyek túl sokat tudnak a körülöttük lévő kontextusról, implicit feltételezésekbe kezdenek. Az AI outputban lévő implicit feltételezések pedig a legnehezebben elkapható hibák.
:::lang

:::section

:::section validation

:::lang en
## The Validation Layer

The validator's job is not to find the perfect output. Its job is to find the certainly wrong ones.

This is the most important design decision in the system. A validator that tries to judge output *quality* will fail — quality is contextual, subjective, architectural. Those judgments require a human. But a validator that checks structural correctness, constraint compliance, and format requirements — that's automatable.

```yaml
validators:
  - type: stop_token_present
    token: "---END---"
    error: STOP_TOKEN_MISSING

  - type: stop_token_not_leaked
    token: "---END---"
    error: STOP_TOKEN_LEAKED

  - type: yaml_valid
    error: INVALID_YAML_STRUCTURE

  - type: no_forbidden_affected_files
    files: "{{constraints.forbidden_affected_files}}"
    error: FORBIDDEN_AFFECTED_FILE
```

The boundary is clear:
- **Validator returns fail** → the output is certainly wrong. Automated rejection.
- **Validator returns pass** → the output *might* be good. Human decides.

This is what keeps the human's attention on the real questions. Not "does this output have the correct stop token?" — but "is this the right decomposition for this feature?" The validator handles the mechanical layer so that I can focus on the judgment layer.
:::lang

:::lang hu
## A Validációs Réteg

A validator feladata nem a tökéletes output megtalálása. A feladata a biztosan rosszak kiszűrése.

Ez a rendszer legfontosabb tervezési döntése. Egy validator, amely output *minőséget* próbál megítélni, kudarcot vall — a minőség kontextuális, szubjektív, architekturális. Ezek az ítéletek embert igényelnek. De egy validator, amely strukturális helyességet, constraint megfelelést és formátum követelményeket ellenőriz — az automatizálható.

```yaml
validators:
  - type: stop_token_present
    token: "---END---"
    error: STOP_TOKEN_MISSING

  - type: stop_token_not_leaked
    token: "---END---"
    error: STOP_TOKEN_LEAKED

  - type: yaml_valid
    error: INVALID_YAML_STRUCTURE

  - type: no_forbidden_affected_files
    files: "{{constraints.forbidden_affected_files}}"
    error: FORBIDDEN_AFFECTED_FILE
```

A határ egyértelmű:
- **Validator fail-t ad** → az output biztosan rossz. Automatikus elutasítás.
- **Validator pass-t ad** → az output *lehet* jó. A human dönti el.

Ez az, ami az ember figyelmét a valódi kérdésekre tartja. Nem arra, hogy "megvan-e a stop token az outputban?" — hanem arra, hogy "ez a helyes lebontás ehhez a featurehoz?" A validator kezeli a mechanikus réteget, hogy én a judgment rétegre tudok fókuszálni.
:::lang

:::section

:::section refinement

:::lang en
## Error-Driven Refinement

When validation fails, the system has two options: retry or refine.

**Retry** means sending the same request again and hoping for a better output. This sometimes works, by accident. But it gives the model no information about what went wrong. It's randomness dressed up as iteration.

**Refinement** is different. The specific violation is extracted from the validator result, mapped to a targeted error message, and injected into a new prompt that tells the model exactly what to fix.

```
Validator result:  STOP_TOKEN_LEAKED
Violation detail:  Stop token "---END---" found at line 8 inside the YAML content body.

Refinement prompt injected:
  Your previous response was rejected for the following reason:
  STOP_TOKEN_LEAKED — The stop token "---END---" must appear only as
  the final line of your response. It was found at line 8 inside the
  YAML content body. Please regenerate the task list without embedding
  the stop token in the content.
```

The model gets back exactly what it did wrong. Not "try again" — but "this specific line, this specific rule, this specific fix."

The refinement loop runs automatically for a configurable number of attempts. If it exhausts its budget, it escalates to me. I then decide whether to intervene manually, adjust the prompt, or reject the step entirely.
:::lang

:::lang hu
## Error-Driven Refinement

Amikor a validáció sikertelen, a rendszernek két lehetősége van: retry vagy refinement.

A **retry** ugyanazt a kérést küldi újra, remélve egy jobb outputot. Ez néha véletlenszerűen működik. De a modellnek semmi információt nem ad arról, mi ment rosszul. Véletlenszerűség, ami iterációnak látszik.

A **refinement** más. A konkrét violation kiemelésre kerül a validator eredményből, egy célzott hibaüzenetre van leképezve, és egy új promptba injektálva, amely pontosan megmondja a modellnek mit kell javítani.

```
Validator result:  STOP_TOKEN_LEAKED
Violation detail:  Stop token "---END---" found at line 8 inside the YAML content body.

Refinement prompt injected:
  Your previous response was rejected for the following reason:
  STOP_TOKEN_LEAKED — The stop token "---END---" must appear only as
  the final line of your response. It was found at line 8 inside the
  YAML content body. Please regenerate the task list without embedding
  the stop token in the content.
```

A modell pontosan azt kapja vissza, amit rosszul csinált. Nem "próbáld újra" — hanem "ez a konkrét sor, ez a konkrét szabály, ez a konkrét javítás."

A refinement loop automatikusan fut egy konfigurálható számú próbálkozásig. Ha kimerül a keret, eszkalál hozzám. Én döntöm el, hogy manuálisan beavatkozok, módosítom a promptot, vagy teljesen elutasítom a lépést.
:::lang

:::section

:::section real_data

:::lang en
## Five Runs. Real Numbers.

One note on the model choice before the numbers. The test runs used a local Qwen2.5-7B-Instruct model running on my personal hardware — an AMD Ryzen 5 with integrated graphics, no dedicated GPU. The reasons were deliberate: privacy (no code leaves the machine), offline operation, and zero cost per run. The hardware constraint also sets a real ceiling: a 7B model is the practical limit on this setup. Response times run 30–60 seconds on CPU. In a human-in-the-loop workflow, that's acceptable — there's a review step between every run anyway.

The following runs were recorded during the first live test of the `task_breakdown` step.

| Run | Auto-reject reason | Refinement | Result |
|-----|-------------------|------------|--------|
| 1 | — (no validator yet) | Human reject | Lesson: prompt calibration needed |
| 2 | — | Human reject | Truncation + constraint violation |
| 3 | — | Human (edit suggested) | Stop token leaking into content |
| 4 | FORBIDDEN_AFFECTED_FILE | Auto × 3 | Validator bug identified (false positive) |
| 5 | STOP_TOKEN_LEAKED | Auto × 1 | **Accept** |

Run 4 deserves more attention. The `FORBIDDEN_AFFECTED_FILE` violation was a false positive — the validator was incorrectly parsing the output structure and flagging a file that wasn't actually in the `affected_files` list. The auto-refinement loop ran three times before I caught it and identified the bug.

That run wasn't a failure of the system. It was the system doing exactly what it was supposed to do — surfacing a problem that needed fixing. The validator bug was real; I just had the wrong thing under test. Runs 1–3, without a validator, were far noisier.

Run 5: the output was accepted. My note at the time: *"I'd do it differently, but I can't fault it."*

That is the target state. Not "the AI produced what I would have produced." But "the AI produced something I cannot reject on objective grounds." Stylistic disagreement is not a validator problem — and it's not a reason to reject an output that meets the spec.
:::lang

:::lang hu
## Öt Futás. Valós Számok.

Egy megjegyzés a modellválasztásról a számok előtt. A tesztfutások lokális Qwen2.5-7B-Instruct modellt használtak, a saját gépemen — AMD Ryzen 5, integrált grafika, dedikált GPU nélkül. Az okok szándékosak: privacy (semmi nem hagyja el a gépet), offline működés, és nulla futási cost. A hardver korlát egy valós plafont is meghatároz: a 7B modell a gyakorlati határ ezen a konfiguráción. A válaszidők CPU-n 30–60 másodpercek. Human-in-the-loop workflow-ban ez elfogadható — minden futás után úgyis van egy review lépés.

Az alábbi futások a `task_breakdown` step első éles tesztjén kerültek rögzítésre.

| Futás | Auto-reject oka | Refinement | Eredmény |
|-------|-----------------|------------|----------|
| 1 | — (validator még nem volt) | Human reject | Tanulság: prompt kalibrálás szükséges |
| 2 | — | Human reject | Truncation + constraint sértés |
| 3 | — | Human (edit javasolt) | Stop token szivárgás a tartalomba |
| 4 | FORBIDDEN_AFFECTED_FILE | Auto × 3 | Validator bug azonosítva (false positive) |
| 5 | STOP_TOKEN_LEAKED | Auto × 1 | **Accept** |

A 4. futás külön figyelmet érdemel. A `FORBIDDEN_AFFECTED_FILE` violation false positive volt — a validator helytelenül értelmezte az output struktúrát, és olyan fájlt jelölt tiltottnak, amely valójában nem szerepelt az `affected_files` listában. Az auto-refinement loop háromszor futott le, mielőtt elkaptam és azonosítottam a hibát.

Ez a futás nem a rendszer kudarca volt. A rendszer pontosan azt csinálta, amit kellett — felszínre hozott egy megoldandó problémát. A validator bug valós volt; csak a téves dolgot teszteltem. Az 1–3. futások, validator nélkül, sokkal zajosabbak voltak.

5. futás: az output elfogadva. Az akkori megjegyzésem: *„én máshogy csinálnám, de nem tudok belekötni."*

Ez a célállapot. Nem az, hogy "az AI azt produkálta, amit én produkáltam volna." Hanem az, hogy "az AI olyat produkált, amit objektív alapon nem tudok elutasítani." Az ízlésbeli különbség nem validator kérdés — és nem ok egy spec-megfelelő output elutasítására.
:::lang

:::section

:::section workflow_overview

:::lang en
## The Bigger Picture

The test runs described above cover only the first step. The intended workflow has four:

```
[User input]
     ↓
[1. Task breakdown]        →  human validation
     ↓
[2. Production code]       →  human validation
     ↓
[3. Documentation]         →  human validation
     ↓
[4. Unit test design]      →  human validation
```

Each step is independent. Each has its own YAML config, its own validator rules, its own output format. The human decides whether each step runs, when it runs, and whether to carry its output forward into the next step.

Step 2 is worth being explicit about: the goal is not scaffolding or placeholder code — it's production-ready output that can ship. The validator enforces structural correctness; the human reviews for architectural fit. Between the two, the bar is set at something that could actually go into the codebase.
:::lang

:::lang hu
## A Teljes Kép

A fentebb leírt tesztfutások csak az első lépést fedik le. A tervezett workflow négy lépésből áll:

```
[User input]
     ↓
[1. Feladatfelbontás]       →  emberi validáció
     ↓
[2. Production kód]         →  emberi validáció
     ↓
[3. Dokumentáció]           →  emberi validáció
     ↓
[4. Unit teszt tervezés]    →  emberi validáció
```

Minden lépés független. Mindegyiknek saját YAML configja van, saját validator szabályai, saját output formátuma. A human dönti el, hogy melyik lépés fut, mikor fut, és hogy az outputja bekerül-e a következő lépés inputjaként.

A 2. lépésnél érdemes explicit lenni: a cél nem scaffold vagy placeholder kód — hanem production-ready output, ami be tud kerülni a codebase-be. A validator strukturális helyességet kényszerít ki; a human architektúrális megfelelőséget értékel. A kettő együtt egy olyan mércét állít, amely ténylegesen szállítható kódot céloz.
:::lang

:::section

:::section closing

:::lang en
## Where It Stands

Scaffold Protocol is not a finished tool. The validator coverage is partial. The refinement loop handles common patterns well, but has edge cases I haven't mapped yet. The step library is small.

But the architecture is stable, and that's what matters at this stage.

Human orchestrator. AI transformation engine. Validation boundary that filters certainly wrong outputs. Error-driven refinement that gives the model precise feedback. Human decision at every accepted output.

The most important property isn't the automation — it's that I always know exactly why an output was accepted or rejected. There are no black boxes. Every decision in the system is either mine, or traceable to a specific rule I wrote.

That's not a constraint on what the system can do.

That *is* what the system does.

**I'll continue soon. Are you interested?**
:::lang

:::lang hu
## Ahol Most Tart

A Scaffold Protocol még nincs kész. A validator lefedettség részleges. A refinement loop jól kezeli a tipikus eseteket, de vannak edge case-ek, amelyeket még nem térképeztem fel. A step library kicsi.

De az architektúra stabil, és ez a fontos ezen a ponton.

Human orchestrator. AI transzformációs motor. Validációs határ, amely kiszűri a biztosan rossz outputokat. Error-driven refinement, amely pontos visszajelzést ad a modellnek. Human döntés minden elfogadott outputnál.

A legfontosabb tulajdonság nem az automatizáció — hanem az, hogy mindig pontosan tudom, miért lett egy output elfogadva vagy elutasítva. Nincsenek fekete dobozok. A rendszer minden döntése vagy az enyém, vagy visszavezethető egy általam írt konkrét szabályra.

Ez nem korlát arra, hogy mit tud a rendszer.

Ez *az*, amit a rendszer csinál.

**Nemsokára folytatom. Érdekel?**
:::lang

:::section
