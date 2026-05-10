---
title: Making a 2M-line legacy codebase queryable by AI
title_hu: Legacy kódbázis, amit az AI is ismer
slug: posts/sourcerag-00.html
description: I designed a system to make a 2M+ line on-premise legacy codebase queryable by any AI tool. It was presented, never implemented. Here is the design — and the question it left open.
layout: page
date: 2026-04-13
series: SourceRAG
part: 1
---

:::section intro

:::lang en

AI coding assistants are increasingly capable. But their usefulness depends entirely on context — and context requires knowledge. If your codebase lives on-premise, spans over two million lines, and has no public documentation, AI tools know nothing about it. They can suggest code patterns, but they cannot reason about your system.

This is a design document for a system I never built. I called it CaKB — Codebase-aware Knowledge Base. I designed it, presented it internally, and it was never implemented. But the thinking behind it did not disappear — it became SourceRAG.

:::lang

:::lang hu

Az AI kódoló asszisztensek egyre képesebbek. De hasznosságuk teljes egészében a kontextustól függ — a kontextus pedig tudást igényel. Ha a kódbázisod on-premise van, több mint kétmillió sorból áll, és nincs nyilvános dokumentációja, az AI eszközök semmit sem tudnak róla. Tudnak kódmintákat javasolni, de nem tudnak a rendszeredről gondolkodni.

Ez egy rendszer tervdokumentuma, amelyet soha nem építettem meg. CaKB-nak neveztem — Codebase-aware Knowledge Base. Megterveztem, belső körben bemutattam, és nem valósult meg. De a mögötte lévő gondolkodás nem tűnt el — SourceRAG-gá vált.

:::lang

:::section

:::section problem

:::lang en

## The problem

A large on-premise legacy codebase presents a specific challenge: no AI tool can query it without explicit retrieval infrastructure. The codebase is not indexed by any LLM. It is not in any cloud. And it changes with every build.

The requirement was straightforward to state: make the codebase queryable by any AI tool — not just one. A proprietary integration built for a single assistant creates a dependency that needs rebuilding every time the tooling changes. The solution had to be tool-agnostic by design.

:::lang

:::lang hu

## A probléma

Egy nagy on-premise legacy kódbázis sajátos kihívást jelent: egyetlen AI eszköz sem tudja lekérdezni explicit retrieval infrastruktúra nélkül. A kódbázist nem indexelte semmilyen LLM. Nincs felhőben. És minden builddel változik.

A követelmény egyszerűen megfogalmazható volt: tegyük a kódbázist lekérdezhető bármely AI eszköz számára — ne csak egynek. Egyetlen asszisztensre szabott integráció olyan függőséget teremt, amelyet minden alkalommal újra kell írni, ha az eszközkészlet változik. A megoldásnak tervezési elvből tool-agnosticnak kellett lennie.

:::lang

:::section

:::section design

:::lang en

## The design

CaKB is built around three responsibilities: indexing, storage, and retrieval.

When a build completes, a CI/CD hook — Jenkins is a common choice in enterprise environments — triggers the Indexer Service. The Indexer processes changed source files, generates vector embeddings, and updates two stores: Qdrant for semantic search, and a SQLite Manifest DB that tracks index state — which files are indexed, at which commit, and when.

The Retrieval API sits in front of these stores. It is an ASP.NET Core service that accepts queries from any AI tool and returns relevant code context. It does not know — and does not care — who is asking. A Copilot Extension is one possible consumer. A custom CLI tool is another. The API contract is the only integration boundary that matters.

:::lang

:::lang hu

## A terv

A CaKB három felelősség köré épül: indexelés, tárolás és visszakeresés.

Amikor egy build befejeződik, egy CI/CD hook — a Jenkins elterjedt választás vállalati környezetekben — elindítja az Indexer Service-t. Az Indexer feldolgozza a megváltozott forrásfájlokat, vektoros beágyazásokat generál, és frissíti a két tárolót: a Qdrant-ot szemantikus kereséshez, és egy SQLite Manifest DB-t, amely nyomon követi az index állapotát — mely fájlok vannak indexelve, milyen commiton, és mikor.

A Retrieval API ezek előtt helyezkedik el. Ez egy ASP.NET Core szolgáltatás, amely fogadja a lekérdezéseket bármely AI eszköztől, és visszaadja a releváns kódkontextust. Nem tudja — és nem is érdekli — ki kérdez. Egy Copilot Extension az egyik lehetséges fogyasztó. Egy egyedi CLI eszköz a másik. Az API kontraktusa az egyetlen integrációs határ, ami számít.

:::lang

:::section

:::section rationale

:::lang en

## Component rationale

**Qdrant** — purpose-built for vector similarity search, Docker-deployable, with no cloud dependency. It runs entirely on-premise alongside the system it indexes.

**SQLite Manifest DB** — a lightweight, file-based tracking layer. At any point it answers one question: what is indexed, at which version, and is it still current? No separate database server, no operational overhead.

**CI/CD hook** — build-triggered indexing keeps the knowledge base current without a separate scheduling mechanism. When code changes, the index changes. The trigger already exists in the pipeline; it just needs to call the Indexer.

**Tool-agnostic Retrieval API** — decouples every consumer from the storage layer. Any AI tool integrates through a single, stable contract. The API absorbs the complexity so consumers do not have to.

:::lang

:::lang hu

## Komponens indoklások

**Qdrant** — vektoros hasonlóságkeresésre tervezett, Docker-elhető, felhő-függőség nélkül. Teljesen on-premise futtatható az általa indexelt rendszer mellett.

**SQLite Manifest DB** — könnyűsúlyú, fájlalapú nyomon követési réteg. Bármikor egyetlen kérdésre ad választ: mi van indexelve, milyen verzióban, és még érvényes-e? Nincs szükség külön adatbázis-szerverre, nincs üzemeltetési többlet.

**CI/CD hook** — build-triggerelt indexelés külön ütemezési mechanizmus nélkül tartja naprakészen a tudásbázist. Ha a kód változik, az index is változik. A trigger már megvan a pipeline-ban; csak az Indexert kell meghívnia.

**Tool-agnostic Retrieval API** — leválasztja az összes fogyasztót a tárolási rétegtől. Minden AI eszköz egyetlen, stabil kontraktuson keresztül integrálódik. Az API magába nyeli a komplexitást, hogy a fogyasztóknak ne kelljen.

:::lang

:::section

:::section cost

:::lang en

## What this requires

This is not a weekend project. A production-grade CaKB carries real operational weight.

Embedding generation has to be reliable at build time — a silently failing indexer that produces a stale knowledge base is worse than no indexer at all. The Retrieval API contract has to remain stable, or every consumer integration breaks simultaneously. The SQLite manifest has to handle concurrent index updates without corruption. And the whole system requires organizational alignment: the Indexer runs as part of the build pipeline, which means it is maintained alongside the build itself.

None of these are unsolvable. But the cumulative weight is real — and one component carries a disproportionate share of it.

The proof store — the mechanism that guarantees the index accurately reflects the codebase at any given point — is where most of the complexity lives. You need to know what is indexed, at which version, and whether the index is still valid. Building that correctly is harder than it looks.

:::lang

:::lang hu

## Amit ez megkövetel

Ez nem egy hétvégi projekt. Egy termelési szintű CaKB valós üzemeltetési súllyal jár.

Az embedding generálásnak build időben megbízhatónak kell lennie — egy csendben hibázó, elavult tudásbázist produkáló indexer rosszabb, mint ha nincs indexer egyáltalán. A Retrieval API kontraktusának stabilnak kell maradnia, különben minden fogyasztói integráció egyszerre törik el. Az SQLite manifestnek egyidejű index-frissítéseket kell kezelnie adatsérülés nélkül. Az egész rendszer szervezeti összehangolást igényel: az Indexer a build pipeline részeként fut, ami azt jelenti, hogy a builddel együtt kell karbantartani.

Ezek egyike sem megoldhatatlan. De a kumulatív súly valóságos — és az egyik komponens aránytalanul nagy részt visel belőle.

A bizonyítástár — az a mechanizmus, amely garantálja, hogy az index pontosan tükrözi a kódbázist bármely adott pillanatban — az, ahol a komplexitás nagy része él. Tudnod kell, mi van indexelve, milyen verzióban, és hogy az index még érvényes-e. Ezt helyesen megépíteni nehezebb, mint amilyennek látszik.

:::lang

:::section

:::section closing

:::lang en

## One question left open

Building CaKB as designed is possible. The architecture is sound. But the proof store is the most expensive part — and it raises a question worth sitting with.

You need to know what is indexed, at which version, and whether the index is still valid. If you already have a version control system, it already knows all of this. Every commit is a versioned snapshot. Every file change is tracked. The history is auditable.

Does the proof store need to be built — or does it already exist?

That question is where SourceRAG begins. [Let's see how it looks in practice →](/posts/sourcerag-01.html)

:::lang

:::lang hu

## Egy nyitott kérdés

A CaKB megépítése az ismertetett módon lehetséges. Az architektúra megalapozott. De a bizonyítástár a legköltségesebb rész — és felvet egy kérdést, amelyen érdemes elidőzni.

Tudnod kell, mi van indexelve, milyen verzióban, és hogy az index még érvényes-e. Ha már van verziókezelő rendszered, az már mindezeket tudja. Minden commit egy verzionált pillanatkép. Minden fájlváltozás nyomon van követve. Az előzmények auditálhatók.

A bizonyítástárat meg kell építeni — vagy már létezik?

Ez a kérdés az, ahol a SourceRAG elkezdődik. [Nézzük meg a gyakorlatban →](/posts/sourcerag-01.html)

:::lang

:::section
