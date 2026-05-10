---
title: Closing the Loop
title_hu: Zárd le a hurkot
slug: posts/post-chaosforge-02.html
description: A practical workflow for pairing a reasoning model with an agentic coding assistant — and closing the structural gap between planning and implementation.
layout: page
date: 2026-03-26
series: ChaosForge
part: 2
---

:::section intro

:::lang en

# Closing the Loop

This workflow came out of a concrete frustration.

I was building a hobby project using two AI tools side by side: a conversational AI for planning and architecture, and an agentic coding assistant that works directly in the repository. Each tool is excellent at its job. Together, they have a structural gap that neither one solves on its own.

The gap is not tool-specific. Any workflow that separates **planning** from **implementation** across two AI systems will run into the same problem. This post describes that problem — and a practical pattern for closing it.

The pattern is tool-agnostic. Examples use Claude Desktop and Claude Code, but the underlying structure applies wherever a reasoning model and an agentic coding assistant are used together.

:::lang

:::lang hu

# Zárd le a hurkot

Ez a workflow egy konkrét frusztrációból született.

Egy hobbi projekten dolgoztam két AI eszközzel egymás mellett: egy társalgó AI-t használtam tervezéshez és architektúrához, és egy ágentikus kód-asszisztenst, ami közvetlenül a repositoryban dolgozik. Mindkét eszköz kiváló a maga feladatában. Együtt azonban van egy strukturális résük, amit egyik sem old meg önmagában.

A rés nem eszközspecifikus. Bármely workflow, ami **tervezést** és **implementációt** két AI rendszerre bont szét, ugyanebbe a problémába fog ütközni. Ez a poszt azt a problémát írja le – és egy praktikus mintát a megoldásra.

A minta eszközfüggetlen. A példák Claude Desktopot és Claude Code-ot használnak, de az alapstruktúra bárhol alkalmazható, ahol reasoning modell és ágentikus kód-asszisztens együtt fut.

:::lang

:::section

:::section two_roles

:::lang en

## Two Roles, Two Strengths

For clarity, the workflow uses two role names throughout.

**Reasoning partner** — the AI used for planning, architecture decisions, and design iteration. It operates conversationally, with context that can span multiple sessions. Its value is in the *reasoning*.

**Implementation agent** — the AI used to turn plans into code. It operates agentically: reads the full repository, writes and edits files, runs tests, and self-corrects. Its value is in the *execution*.

These two roles have genuinely different strengths. The workflow below doesn't try to collapse them into one — it makes the handoff between them explicit.

:::lang

:::lang hu

## Két szerepkör, két erősség

Az egyértelműség kedvéért a workflow két szerepkörrel dolgozik.

**Reasoning partner** — az AI, amit tervezésre, architektúrális döntésekre és design iterációra használok. Társalgó módban működik, kontextusa több munkamenetre is kiterjedhet. Az értéke az *érvelésben* rejlik.

**Implementációs ágens** — az AI, ami a terveket kóddá alakítja. Ágentikusan működik: beolvassa a teljes repositoryt, fájlokat ír és szerkeszt, teszteket futtat, és önjavít. Az értéke a *végrehajtásban* rejlik.

Ennek a két szerepkörnek valóban eltérő erősségei vannak. Az alábbi workflow nem próbálja egybesűríteni őket – hanem az átadást teszi explicitté közöttük.

:::lang

:::section

:::section problem

:::lang en

## Where the Loop Breaks

The handoff from reasoning partner to implementation agent works reasonably well by default. Write a spec, point the implementation agent at it, get code. The spec is the contract.

The return direction is where the loop breaks.

After implementation, the reasoning partner has no idea what actually happened. It doesn't know which files changed, which on-the-fly decisions were made — and most importantly — whether its own architecture documents are still accurate. The next planning session starts from a gradually outdating mental model.

Three specific gaps drive this drift:

**Gap 1: No structured handoff from implementation back to planning.**
After a feature is built, the only option is to manually copy source files into the reasoning partner's context. This doesn't scale, and it floods the conversation with boilerplate that isn't relevant to architectural discussion.

**Gap 2: No living record of what's actually been built.**
Architecture documents describe intended design. After a few feature branches, intended and actual diverge silently. The reasoning partner plans the next feature from the original design — not the real codebase.

**Gap 3: Implementation decisions don't flow back into documentation.**
The implementation agent makes real decisions during development. An interface gets split. An invariant is enforced in a different layer than the spec said. A new field appears on an entity. These decisions are correct, they pass tests, they get merged — and then they disappear. No ADR is written. No architecture doc is updated.

:::lang

:::lang hu

## Ahol a hurok megszakad

Az átadás a reasoning partnertől az implementációs ágens felé alapértelmezés szerint elég jól működik. Megírod a specifikációt, rámutatod az ágensre, kód lesz belőle. A spec a kontraktus.

A visszairány az, ahol a hurok megszakad.

Az implementáció után a reasoning partnernek fogalma sincs, mi történt valójában. Nem tudja, melyik fájlok változtak, milyen menetközbeni döntések születtek – és ami a legfontosabb – a saját architektúra dokumentumai még pontosak-e. A következő tervezési munkamenet egy lassan elavuló mentális modellből indul.

Három konkrét rés hajtja ezt a sodródást:

**1. rés: Nincs strukturált visszacsatolás az implementációból a tervezésbe.**
Egy feature megépítése után az egyetlen lehetőség, hogy manuálisan bemásolod a forrásfájlokat a reasoning partner kontextusába. Ez nem skálázható, és olyan boilerplate-tel árasztja el a társalgást, ami nem releváns az architektúrális vitában.

**2. rés: Nincs élő nyilvántartás arról, mi épült meg ténylegesen.**
Az architektúra dokumentumok a tervezett designt írják le. Néhány feature branch után a tervezett és a tényleges csendben eltér egymástól. A reasoning partner a következő feature-t az eredeti designból tervezi – nem a valódi kódbázisból.

**3. rés: Az implementációs döntések nem áramlanak vissza a dokumentációba.**
Az implementációs ágens valódi döntéseket hoz fejlesztés közben. Egy interfész kettéválik. Egy invariánst más rétegben kényszerítenek ki, mint ahogy a spec mondta. Egy új mező jelenik meg egy entitáson. Ezek a döntések helyesek, átmennek a teszteken, bekerülnek a mainbe – aztán eltűnnek. Nincs ADR írva. Nincs architektúra doc frissítve.

:::lang

:::section

:::section solution

:::lang en

## Three Additions to Close the Loop

### Addition 1: The review file

After each implementation task, the implementation agent generates a single Markdown file in `docs/review/`. This is the *only* thing dropped into the reasoning partner's context for a review session.

It is not a diff. It is not a source dump. It contains exactly what a reviewer needs:

- A short summary of what was built and which spec it followed
- A table of changed files by architectural layer — structure only, no code
- Every independent decision the agent made, with its reasoning and the rejected alternative
- A small number of critical code excerpts — only what requires human judgment
- Any deviations from the original spec, each explained
- Open questions that require a human decision before the next feature can proceed
- Proposed updates to living documentation

The last two points are the key insight. The review file doesn't just describe the past — it actively surfaces what needs to happen next.

### Addition 2: `docs/progress/` — ground truth of what's been built

Two files, two audiences:

`docs/progress/index.md` — feature-level completions, in-progress branches, and a deviations table: every implementation decision that differs from the original design, with its reason and reference to any resulting ADR.

`docs/progress/design-state.md` — cross-cutting architectural state: which ADRs exist, which design constraints are active, which parts of the original architecture have evolved. This is what the architect role reads, not the feature list.

Individual feature progress lives in `docs/progress/features/` — one file per branch, additive only. The index is regenerated from these files on every `/review-ready` run. Merge conflicts become structurally impossible.

### Addition 3: Proposed doc updates as a first-class output

Every review file includes an explicit section listing which living documents need updating. The implementation agent — which has full context about what changed — proposes the updates. The human decides what gets incorporated.

Documentation maintenance becomes a step in the review process, not a task that accumulates and gets skipped.

:::lang

:::lang hu

## Három kiegészítés a hurok bezárásához

### 1. kiegészítés: A review fájl

Minden implementációs task után az implementációs ágens egyetlen Markdown fájlt generál a `docs/review/` mappába. Ez az *egyetlen* dolog, amit a reasoning partner kontextusába dobsz egy review munkamenethez.

Nem diff. Nem forrásdump. Pontosan azt tartalmazza, amire egy reviewernek szüksége van:

- Rövid összefoglaló arról, mi épült meg és melyik specet követte
- Módosított fájlok táblázata architektúrális réteg szerint – csak struktúra, kód nélkül
- Minden önálló döntés amit az ágens hozott, az indoklással és az elvetett alternatívával
- Kevés kritikus kódrészlet – csak ami emberi megítélést igényel
- Az eredeti spectől való eltérések, mindegyik magyarázattal
- Nyitott kérdések, amelyek emberi döntést igényelnek a következő feature előtt
- Javasolt frissítések az élő dokumentációhoz

Az utolsó két pont a kulcsfelismerés. A review fájl nem csak a múltat írja le – aktívan felszínre hozza, mi szükséges a következő lépésben.

### 2. kiegészítés: `docs/progress/` — a megépített valóság

Két fájl, két közönség:

`docs/progress/index.md` — feature szintű befejezések, folyamatban lévő branch-ek, és egy eltérési táblázat: minden implementációs döntés, ami eltér az eredeti designtól, az indoklással és az ebből következő ADR hivatkozással.

`docs/progress/design-state.md` — cross-cutting architektúrális állapot: mely ADR-ek léteznek, mely design kényszerek aktívak, az eredeti architektúra mely részei fejlődtek. Ezt olvassa az architekt szerepkör, nem a feature listát.

Az egyedi feature haladás a `docs/progress/features/` mappában él – branch-enként egy fájl, csak addíció. Az indexet minden `/review-ready` futáson újragenerálja az ágens. A merge conflict strukturálisan lehetetlenné válik.

### 3. kiegészítés: Javasolt doc frissítések mint első osztályú output

Minden review fájl tartalmaz egy explicit szekciót, ami felsorolja, mely élő dokumentumokat kell frissíteni. Az implementációs ágens – akinek teljes kontextusa van arról, mi változott – javasolja a frissítéseket. Az ember dönti el, mi kerül be.

A dokumentáció karbantartása a review folyamat egyik lépése lesz, nem egy felhalmozódó és kihagyott feladat.

:::lang

:::section

:::section closed_loop

:::lang en

## The Full Closed Loop

```
Reasoning partner
  └─► write spec → docs/specs/feature-name.md
                          │
                          ▼
                  Implementation agent
                    └─► implement
                          └─► /review-ready
                                ├─► docs/review/feature-name-review.md
                                └─► docs/progress/ updated
                                          │
                                          ▼
                              Reasoning partner (new session)
                                ├─► reads docs/progress/index.md first
                                ├─► reads review file as context
                                ├─► decides on proposed doc updates
                                ├─► writes new ADRs if needed
                                └─► updates architecture.md / domain-model.md
                                          │
                                          ▼
                              Human commits everything
                              (spec + review + updated docs + source)
                                          │
                                          ▼
                              PR merged → next feature
```

Every arrow is explicit and documented. Nothing is carried in anyone's head.

:::lang

:::lang hu

## A teljes zárt hurok

```
Reasoning partner
  └─► spec írása → docs/specs/feature-name.md
                          │
                          ▼
                  Implementációs ágens
                    └─► implementál
                          └─► /review-ready
                                ├─► docs/review/feature-name-review.md
                                └─► docs/progress/ frissítve
                                          │
                                          ▼
                              Reasoning partner (új munkamenet)
                                ├─► beolvassa a docs/progress/index.md-t először
                                ├─► beolvassa a review fájlt kontextusként
                                ├─► dönt a javasolt doc frissítésekről
                                ├─► szükség esetén új ADR-eket ír
                                └─► frissíti az architecture.md / domain-model.md-t
                                          │
                                          ▼
                              Az ember commitol mindent
                              (spec + review + frissített doc-ok + forrás)
                                          │
                                          ▼
                              PR merged → következő feature
```

Minden nyíl explicit és dokumentált. Senki fejében nincs semmi elrejtve.

:::lang

:::section

:::section context_window

:::lang en

## Managing the Context Window as the Project Grows

Every file the implementation agent reads consumes context window tokens. As a project grows — more features merged, more ADRs written, more review files generated — naive loading strategies will eventually degrade session quality.

Five principles keep this under control:

**CLAUDE.md contains behavior, not knowledge.**
The project memory file loaded every session should contain only behavioral rules: what the agent must and must not do, where to find things, how to behave under uncertainty. Technical content — entity definitions, layer rules, naming conventions — belongs in `docs/` only. If the same rule appears in both places, it will drift. Keep one source of truth per fact.

**Template files are never read by the agent.**
All files prefixed with `_` are human-facing scaffolding. A single rule in CLAUDE.md and every slash command prevents the agent from loading explanatory comments and fill-in guides that have no instructional value during implementation.

**Docs are loaded on demand, not upfront.**
The implementation agent does not need `docs/architecture.md` to answer a question about test naming conventions. It does not need `docs/domain-model.md` to fix a validator. Each slash command specifies exactly which files to load for that task. Everything else stays on disk until it is needed.

**ADRs are loaded selectively via a tagged index.**
`docs/decisions/index.md` lists every ADR with layer and topic tags. The agent reads the index, identifies which tags overlap with the current feature's scope, and loads only those ADR files. A project with 20 ADRs does not load all 20 for a feature that touches only the API layer.

**`docs/progress/features/` is never loaded wholesale.**
Individual feature files are read only when a command explicitly requires a specific one. The `docs/progress/index.md` summary is the entry point — if it is kept concise (archived beyond 10 completed entries), it stays cheap to load.

:::lang

:::lang hu

## A kontextusablak kezelése ahogy a projekt növekszik

Minden fájl, amit az implementációs ágens beolvas, kontextusablak tokeneket fogyaszt. Ahogy a projekt növekszik – több mergelt feature, több megírt ADR, több review fájl – a naiv betöltési stratégiák előbb-utóbb rontani fogják a munkamenet minőségét.

Öt elv tartja ezt kézben:

**A CLAUDE.md viselkedést tartalmaz, nem tudást.**
A minden munkamenetben betöltött projektmemória fájlnak csak viselkedési szabályokat szabad tartalmaznia: mit kell és mit nem szabad az ágensnek, hol találja a dolgokat, hogyan viselkedjen bizonytalanság esetén. A technikai tartalom – entitás definíciók, réteg szabályok, névkonvenciók – csak a `docs/`-ban legyen. Ha ugyanaz a szabály mindkét helyen szerepel, el fog térni egymástól. Minden tényhez egyetlen igazságforrás.

**A sablonfájlokat az ágens soha nem olvassa be.**
Minden `_` előtaggal rendelkező fájl ember-szemközti vázlat. Egyetlen szabály a CLAUDE.md-ben és minden slash commandban megakadályozza, hogy az ágens magyarázó kommenteket és kitöltési útmutatókat töltsön be, amelyeknek nincs instruktív értékük implementáció során.

**A doc-ok igény szerint töltődnek be, nem előre.**
Az implementációs ágensnek nincs szüksége a `docs/architecture.md`-re, hogy megválaszoljon egy teszt elnevezési konvencióra vonatkozó kérdést. Nincs szüksége a `docs/domain-model.md`-re egy validátor javításához. Minden slash command pontosan meghatározza, melyik fájlokat töltse be az adott feladathoz. Minden más a lemezen marad, amíg szükség nincs rá.

**Az ADR-ek szelektíven töltődnek be egy tagelt indexen keresztül.**
A `docs/decisions/index.md` minden ADR-t felsorol réteg és témacímkékkel. Az ágens beolvassa az indexet, azonosítja, mely címkék fednek át az aktuális feature hatókörével, és csak azokat az ADR fájlokat tölti be. Egy 20 ADR-rel rendelkező projekt nem tölt be mind a 20-at egy olyan feature-höz, ami csak az API réteget érinti.

**A `docs/progress/features/` soha nem töltődik be teljes egészében.**
Az egyedi feature fájlokat csak akkor olvassa be, ha egy parancs explicit módon igényli az adott fájlt. A `docs/progress/index.md` összefoglaló a belépési pont – ha tömören van tartva (10 befejezett bejegyzésen túl archiválva), olcsón betölthető marad.

:::lang

:::section

:::section scaling_and_limits

:::lang en

## Scaling and Limits

The single-file progress approach doesn't scale to a team, but the underlying structure does — with two adjustments.

**Role separation.** An architect needs ADR state, design constraints, and architectural deviations. A developer needs feature completions, active branches, and dependencies. Keeping these in separate files with separate audiences avoids both noise and false authority.

**Branch-additive tracking.** Each feature branch adds a file to `docs/progress/features/` and never modifies an existing one. The index is regenerated from these files on every `/review-ready` run. Merge conflicts become structurally impossible.

What this workflow does *not* solve:

- **It assumes human judgment at the review boundary.** If the human skips review and merges directly, the loop doesn't close. The workflow creates structure for good handoffs — it cannot enforce them.
- **Long-running features need intermediate checkpoints.** One `/review-ready` run per logical implementation chunk, not just at branch completion.
- **The reasoning partner's context window is finite.** The review file format deliberately restricts what's included, but large features may still require splitting across multiple sessions.
- **It does not verify that implementation decisions were good.** The review file surfaces decisions for human inspection — it does not substitute for architectural judgment.

:::lang

:::lang hu

## Skálázás és korlátok

Az egyfájlos haladáskövetés nem skálázódik csapatra, de az alapstruktúra igen – két kiegészítéssel.

**Szerepkör szétválasztás.** Egy architektnek ADR állapotra, design kényszerekre és architektúrális eltérésekre van szüksége. Egy developernek feature befejezésekre, aktív branch-ekre és függőségekre. Ha ezeket külön fájlokban, külön közönségnek tartjuk, elkerülhetjük a zajt és a hamis autoritást.

**Branch-additív követés.** Minden feature branch hozzáad egy fájlt a `docs/progress/features/` mappához, és soha nem módosít egy meglévőt. Az indexet minden `/review-ready` futáson újragenerálja az ágens. A merge conflict strukturálisan lehetetlenné válik.

Amit ez a workflow *nem* old meg:

- **Emberi megítélést feltételez a review határon.** Ha az ember kihagyja a review-t és közvetlenül mergel, a hurok nem zárul be. A workflow strukturát teremt a jó átadásokhoz – de nem tudja kikényszeríteni azokat.
- **A hosszan futó feature-ök közbülső checkpointokat igényelnek.** Egy `/review-ready` futás logikai implementációs egységenként, nem csak branch befejezésekor.
- **A reasoning partner kontextusablaka véges.** A review fájl formátuma szándékosan korlátozza a bekerülőt, de nagy feature-ök esetén több munkamenetre bontás lehet szükséges.
- **Nem ellenőrzi, hogy az implementációs döntések jók voltak-e.** A review fájl emberi vizsgálat elé tárja a döntéseket – nem helyettesíti az architektúrális megítélést.

:::lang

:::section

:::section files_and_templates

:::lang en

## Files, Conventions, and Templates

| File | Maintained by | Read by | Purpose |
|---|---|---|---|
| `docs/specs/*.md` | Human (via reasoning partner) | Implementation agent | Implementation contract |
| `docs/decisions/ADR-*.md` | Human (via reasoning partner) | Implementation agent | Architecture decisions |
| `docs/review/*-review.md` | Implementation agent | Human + reasoning partner | Review bridge |
| `docs/progress/index.md` | Implementation agent | Human + reasoning partner | Feature-level ground truth |
| `docs/progress/design-state.md` | Human (after review) | Reasoning partner | Architectural ground truth |
| `docs/progress/features/*.md` | Implementation agent | Human | Per-branch additive record |
| `docs/architecture.md` | Human (after review) | Implementation agent | Living architecture reference |
| `docs/domain-model.md` | Human (after review) | Implementation agent | Living domain reference |

All files live in the repository and are committed with the feature branch. The PR contains not just source code but the full documentation trail: spec in, review out, progress updated, docs proposed.

Ready-to-use GitHub templates with all file stubs, fill-in guides, and slash command definitions:

**Claude Code** (reference implementation — tested):
[github.com/vvidman/claude-code-workflow-template](https://github.com/vvidman/claude-code-workflow-template)

**OpenAI Codex CLI** (structural equivalent — not yet tested in production):
[github.com/vvidman/codex-workflow-template](https://github.com/vvidman/codex-workflow-template)

The workflow pattern is tool-agnostic. The templates are tool-specific because file naming conventions, auto-load mechanisms, and command invocation differ between tools. The doc structure — specs, review files, progress tracking, ADRs — is identical in both.

:::lang

:::lang hu

## Fájlok, konvenciók és sablonok

| Fájl | Karbantartja | Olvassa | Célja |
|---|---|---|---|
| `docs/specs/*.md` | Ember (reasoning partneren keresztül) | Implementációs ágens | Implementációs kontraktus |
| `docs/decisions/ADR-*.md` | Ember (reasoning partneren keresztül) | Implementációs ágens | Architektúrális döntések |
| `docs/review/*-review.md` | Implementációs ágens | Ember + reasoning partner | Review híd |
| `docs/progress/index.md` | Implementációs ágens | Ember + reasoning partner | Feature szintű valóság |
| `docs/progress/design-state.md` | Ember (review után) | Reasoning partner | Architektúrális valóság |
| `docs/progress/features/*.md` | Implementációs ágens | Ember | Branch-enkénti additív rekord |
| `docs/architecture.md` | Ember (review után) | Implementációs ágens | Élő architektúra referencia |
| `docs/domain-model.md` | Ember (review után) | Implementációs ágens | Élő domain referencia |

Minden fájl a repositoryban él és a feature branch-csel commitolódik. A PR nem csak forráskódot tartalmaz, hanem a teljes dokumentációs nyomvonalat: spec be, review ki, haladás frissítve, doc-ok javasolva.

Kész GitHub sablonok fájl-stubokkal, kitöltési útmutatókkal és slash command definíciókkal:

**Claude Code** (referencia implementáció — tesztelve):
[github.com/vvidman/claude-code-workflow-template](https://github.com/vvidman/claude-code-workflow-template)

**OpenAI Codex CLI** (strukturális megfelelő — élesben még nem tesztelve):
[github.com/vvidman/codex-workflow-template](https://github.com/vvidman/codex-workflow-template)

A workflow minta eszközfüggetlen. A sablonok eszközspecifikusak, mert a fájlnevezési konvenciók, az auto-load mechanizmusok és a parancsok hívása eltér az eszközök között. A doc struktúra – specek, review fájlok, haladáskövetés, ADR-ek – mindkettőben azonos.

:::lang

:::section
