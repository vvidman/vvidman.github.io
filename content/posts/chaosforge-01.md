---
title: ChaosForge - Let Go of the Reins
slug: posts/post-chaosforge-01.html
description: From Scaffold Protocol to ChaosForge – why I stopped trying to control every AI step, and what I built instead.
layout: page
---

:::section intro

:::lang en

# ChaosForge - Let Go of the Reins

There's a moment in every engineering project where you stop and ask: *am I solving the right problem?*

For me, that moment came midway through building Scaffold Protocol – a human-in-the-loop, step-by-step AI workflow tool for .NET development. The system worked. Agents produced output. Validators caught bad results. The human reviewed everything. Controlled. Predictable.

Too predictable.

I was watching my validation layer generate refinement loop after refinement loop on outputs that were, by any reasonable measure, *fine*. The system was busy. It just wasn't going anywhere.

That's when I asked the real question: **what if the goal isn't to control every step – but to trust the process?**

This is ChaosForge.

:::lang

:::lang hu

# ChaosForge - Engedd el a gyeplőt

Minden mérnöki projektben eljön az a pillanat, amikor megállsz és megkérdezed magadtól: *tényleg a jó problémát oldom meg?*

Nekem ez a pillanat a Scaffold Protocol fejlesztésének közepén érkezett – egy human-in-the-loop, lépésenkénti AI workflow eszköz .NET fejlesztéshez. A rendszer működött. Az ágensek outputot adtak. A validátor kiszűrte a rosszakat. Az ember minden lépést átnézett. Kontrollált. Kiszámítható.

Túl kiszámítható.

Néztem, ahogy a validációs rétegem refinement kört generál refinement kör után olyan outputokon, amik – minden ésszerű mércével mérve – *rendben voltak*. A rendszer dolgozott. Csak nem haladt.

Akkor tettem fel a valódi kérdést: **mi van ha a cél nem minden lépés kontrollálása, hanem a folyamatban való bizalom?**

Ez a ChaosForge.

:::lang

:::section

:::section scaffold_context

:::lang en

## Where This Came From

Scaffold Protocol was built on a simple principle: the AI transforms, the human decides. Every step had its own YAML configuration, its own system prompt, its own validation rules. The human reviewed output at each checkpoint. Nothing moved forward without approval.

It worked well for deterministic, structured tasks. The problem emerged when I tried to make the validation layer do too much – not just catch *clearly wrong* outputs, but enforce *stylistic correctness* that only I could judge. The result was a loop that felt productive but wasn't.

The insight was simple: **I was trying to make the machine do the human's job.**

ChaosForge inverts the assumption. Instead of a tightly controlled single-agent pipeline, it's a multi-agent team working in parallel – with human review gates at the *right* moments, not every moment.

:::lang

:::lang hu

## Honnan jött ez az ötlet?

A Scaffold Protocol egy egyszerű elven alapult: az AI transzformál, az ember dönt. Minden lépésnek volt saját YAML konfigurációja, saját system promptja, saját validációs szabályai. Az ember minden ellenőrzési ponton átnézte az outputot. Semmi sem mozdult előre jóváhagyás nélkül.

Jól működött determinisztikus, strukturált feladatoknál. A probléma akkor jött elő, amikor a validációs réteggel túl sokat akartam elvégeztetni – nemcsak a *nyilvánvalóan rossz* outputokat szűrni, hanem olyan *stílusbeli helyességet* kikényszeríteni, amit csak én tudtam megítélni. Az eredmény egy olyan kör lett, ami produktívnak tűnt, de nem volt az.

A felismerés egyszerű volt: **azt próbáltam a géppel elvégeztetni, ami az ember feladata.**

A ChaosForge megfordítja az alapfeltevést. Egy szorosan kontrollált, single-agent pipeline helyett párhuzamosan dolgozó multi-agent csapat – emberi revíziós kapukkal a *megfelelő* pillanatokban, nem minden pillanatban.

:::lang

:::section

:::section the_idea

:::lang en

## A Development Team That Doesn't Sleep

ChaosForge simulates a real Scrum team – except every team member is an AI agent. You define the project, the use cases, the team composition. Then you hand it off.

The Business Analyst reads your use cases and produces User Requirements Specifications. The Architect turns those into Software Requirements and breaks them into implementable tasks. The Scrum Master builds the sprint plan. Then developers, testers, reviewers, and technical writers run in parallel – pulling tasks, implementing, reviewing, testing, documenting.

The human's role is the judge. Not the manager of every micro-decision – the judge at three revision gates. Three moments where everything stops and you decide: **accept, edit and accept, or reject.**

```yaml
# Agent configuration example
agent:
  role: business_analyst
  model_provider: groq
  model: llama-3.3-70b
  system_prompt: |
    You are a senior Business Analyst.
    Input: a user-defined use case.
    Output: a structured URS document.
  input_from: use_cases
  output_format: markdown
  output_to: urs_documents
```

Each agent role has its own configuration – its own model, its own system prompt, its own input and output contract. The Architect runs on a large cloud model for deep reasoning. Developers run locally for fast, repetitive generation. The infrastructure doesn't care which provider is behind the interface.

:::lang

:::lang hu

## Egy fejlesztőcsapat, ami nem alszik

A ChaosForge egy valódi Scrum csapatot szimulál – azzal a különbséggel, hogy minden csapattag AI ágens. Te definiálod a projektet, a use case-eket, a csapat összetételét. Aztán átadod.

A Business Analyst elolvassa a use case-eket és User Requirements Specificationöket állít elő. Az Architect ezekből Software Requirementseket csinál, és lebontja implementálható taskokra. A Scrum Master sprint tervet épít. Aztán a developerek, tesztelők, reviewerek és technical writerek párhuzamosan futnak – taskokat vesznek fel, implementálnak, reviewznak, tesztelnek, dokumentálnak.

Az ember szerepe a bíróé. Nem minden mikrodöntés menedzsere – a bíró három revíziós kapunál. Három pillanat, ahol minden megáll és te döntesz: **elfogad, szerkeszt és elfogad, vagy visszadob.**

```yaml
# Ágens konfiguráció példa
agent:
  role: business_analyst
  model_provider: groq
  model: llama-3.3-70b
  system_prompt: |
    Senior Business Analyst vagy.
    Input: felhasználó által definiált use case.
    Output: strukturált URS dokumentum.
  input_from: use_cases
  output_format: markdown
  output_to: urs_documents
```

Minden ágens szerepkörnek saját konfigurációja van – saját modellje, saját system promptja, saját input és output kontraktusa. Az Architect nagy felhős modellen fut a mély gondolkodáshoz. A developerek lokálisan futnak a gyors, ismétlő generáláshoz. Az infrastruktúra nem érdekli, melyik provider van az interfész mögött.

:::lang

:::section

:::section butterfly_effect

:::lang en

## The Butterfly Effect Is the Feature

The system is named ChaosForge for a reason. At each revision gate, you have three choices: accept the output unchanged, reject it with a reason, or – most powerfully – **edit and accept**.

Modify the URS. Adjust the SRS. Rewrite a sprint task.

That single edit ripples through everything downstream. A modified URS produces different SRS documents. Different SRS documents produce different tasks. Different tasks produce a different sprint plan. By the time the developer agents pick up their first task, the project is already shaped by your intervention – not just approved, but *directed*.

This is what Scaffold Protocol was missing. In a step-by-step single-agent system, an edit is a correction. In a multi-agent team, it's a strategic decision with consequences.

The butterfly effect isn't a risk to be managed. It's the design.

:::lang

:::lang hu

## A pillangó effektus nem mellékhatás – ez a feature

A rendszer neve ChaosForge, és ennek oka van. Minden revíziós kapunál három választásod van: elfogadod az outputot változatlanul, visszadobod indoklással, vagy – a legerősebb opció – **szerkesztesz és elfogadsz**.

Módosítod az URS-t. Igazítasz az SRS-en. Átírsz egy sprint taskot.

Ez az egy szerkesztés végiggyűrűzik mindenen, ami utána következik. Egy módosított URS más SRS dokumentumokat eredményez. Más SRS dokumentumok más taskokat. Más taskok más sprint tervet. Mire a developer ágensek felveszik az első taskjukat, a projektet már alakította a beavatkozásod – nem csak jóváhagytad, *irányítottad*.

Ezt hiányoltam a Scaffold Protocolból. Egy lépésenkénti, single-agent rendszerben egy szerkesztés javítás. Egy multi-agent csapatban stratégiai döntés, következményekkel.

A pillangó effektus nem kezelt kockázat. Ez a terv.

:::lang

:::section

:::section design_decisions

:::lang en

## The Decisions That Mattered

**Singleton roles at the domain level.** The Business Analyst, Architect, and Scrum Master are singletons – enforced by the domain model, not just convention. Having two Business Analysts produce competing URS documents isn't parallelism; it's noise.

**TaskAttempt as a first-class entity.** Every development cycle – implement, review, test – is stored as a separate record. When a developer picks up a rejected task, they see the previous attempt's output and the reviewer's exact comment. The full context of failure is preserved.

**Provider abstraction from day one.** The LLM provider is swappable per agent role. The Architect runs on a 70B cloud model for deep reasoning. Developers run on LlamaSharp locally for fast, repetitive code generation. This isn't premature abstraction – it's a hardware constraint turned into a design principle.

**No automatic pipeline.** Errors accumulate in automated chains. If the Architect produces a flawed SRS, every downstream task is built on that flaw. Human revision gates aren't ceremony – they're the guarantee that each phase rests on reviewed ground.

:::lang

:::lang hu

## A döntések, amik számítottak

**Singleton szerepkörök domain szinten.** A Business Analyst, az Architect és a Scrum Master singleton – nem konvenció alapján, hanem a domain modell által kikényszerítve. Ha két Business Analyst versengő URS dokumentumokat produkál, az nem párhuzamosság – az zaj.

**TaskAttempt mint első osztályú entitás.** Minden fejlesztési kör – implementálás, review, teszt – külön rekordként tárolódik. Amikor egy developer felvesz egy visszadobott taskot, látja az előző attempt outputját és a reviewer pontos megjegyzését. A kudarc teljes kontextusa megmarad.

**Provider absztrakció az első naptól.** Az LLM provider ágensenként cserélhető. Az Architect 70B-es felhős modellen fut a mély gondolkodáshoz. A developerek LlamaSharp-on futnak lokálisan a gyors, ismétlő kódgeneráláshoz. Ez nem korai absztrakció – egy hardverkényszert design elvvé fordítottam.

**Nincs automatikus pipeline.** A hibák akkumulálódnak az automatikus láncokban. Ha az Architect hibás SRS-t produkál, minden downstream task arra a hibára épül. Az emberi revíziós kapuk nem ceremónia – ezek a garancia arra, hogy minden fázis reviewzott alapra épül.

:::lang

:::section

:::section closing

:::lang en

## Where It Stands

ChaosForge is a hobby project. A learning project. There's no product pressure and no deadline.

The domain model is taking shape – entities, state machine, revision gates, TaskAttempt audit trail. The LLM provider abstraction is designed. The agent worker architecture is sketched.

What I don't know yet: what a multi-agent development team actually produces when you let it run. That's the experiment.

*Next in the series: domain model design and the task lifecycle state machine.*

:::lang

:::lang hu

## Ahol most tart

A ChaosForge hobbi projekt. Tanulási projekt. Nincs product kényszer, nincs határidő.

A domain modell formát ölt – entitások, state machine, revíziós kapuk, TaskAttempt audit trail. Az LLM provider absztrakció megtervezett. Az agent worker architektúra felvázolt.

Amit még nem tudok: egy multi-agent fejlesztőcsapat mit produkál valójában, ha hagyjuk futni. Ez a kísérlet.

*Sorozat következő része: domain modell tervezés és a task életciklus state machine.*

:::lang

:::section
