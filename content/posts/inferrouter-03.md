---
title: InferRouter Part 3: Secret Management
slug: posts/inferrouter-03.html
layout: page
description: Where API keys live in InferRouter — why not environment variables, why not config files, and what per-request key reading gives you in practice.
date: 2026-06-03
series: InferRouter
part: 3
---

:::section intro

:::lang en
## Part 3: Secret Management

[Part 2](/posts/inferrouter-02.html) covered inference strategies: how ProviderOrchestrator selects providers and routes requests. The execution loop knows which providers to try. What it still needs at request time is the API key for each cloud provider.

This is a question that is easy to get wrong.
:::lang

:::lang hu
## 3. rész: Titkos kulcsok kezelése

A [2. rész](/posts/inferrouter-02.html) az inferencia stratégiákkal foglalkozott: hogyan választja ki a ProviderOrchestrator a providereket és irányítja a kéréseket. A végrehajtási ciklus tudja, melyik providereket próbálja meg. Amit kérés időpontban még szükséges: minden cloud provider API kulcsa.

Ez egy könnyen elrontható kérdés.
:::lang

:::section

:::section comparison

:::lang en
## Three Places a Key Could Live

**Environment variables** — straightforward to set, but the key becomes part of the process environment for the full container lifetime. It is visible in `docker inspect` output, appears in crash dumps and debug traces, and requires a container restart to rotate.

**Config files** — the risk here is accidental commit. A key placed in `appsettings.json` ends up adjacent to source code, one careless `git add` away from being public. Even with `.gitignore` in place, the key lives as a plain string in process memory for the entire application lifetime.

**Docker Secrets** — keys are mounted as files at `/run/secrets/` by Docker Compose using file-based secrets. No Swarm mode required. The key never appears in environment variables, Docker metadata, or any config file. The `secrets/` directory on the host is git-ignored; `secrets.example/` documents the expected file layout without containing real values.

The choice is Docker Secrets.
:::lang

:::lang hu
## Három hely, ahol egy kulcs élhetne

**Environment variable** — egyszerű beállítani, de a kulcs a teljes container élettartama alatt a process environment részévé válik. Látható a `docker inspect` kimenetében, megjelenik crash dump-okban és debug trace-ekben, és a rotáláshoz container újraindítás szükséges.

**Config fájl** — a kockázat itt a véletlen commit. Egy `appsettings.json`-ba helyezett kulcs a forráskód mellé kerül, egy gondatlan `git add`-tól arra, hogy nyilvánossá váljon. Még `.gitignore` esetén is, a kulcs plain stringként él a process memóriában a teljes alkalmazás élettartama alatt.

**Docker Secrets** — a kulcsok fájlokként vannak felcsatolva a `/run/secrets/` elérési úton a Docker Compose által, file-based secrets segítségével. Nincs szükség Swarm módra. A kulcs soha nem jelenik meg environment variable-kban, Docker metaadatokban, vagy bármilyen config fájlban. A gazdagépen lévő `secrets/` könyvtár git-ignored; a `secrets.example/` a várt fájlszerkezetet dokumentálja valódi értékek nélkül.

A döntés a Docker Secrets.
:::lang

:::section

:::section secretreader

:::lang en
## Per-Request Key Reading

Docker Secrets solve the storage problem. The implementation decision that matters equally: what happens when the key is actually needed.

`SecretReader` is an injectable singleton. On every `CompleteAsync` call — the method inside `OpenAiCompatibleProvider` that sends the HTTP request — it reads from `/run/secrets/` directly. The key is never stored in a field. It exists only on the stack frame of the request and is discarded when the call completes.

This was a deliberate refactor from an earlier static class design. Two problems forced the change. First, API keys living as plain strings in process memory for the full application lifetime is a worse security posture than a per-request read. Second, a static class creates a sequencing problem with the DI container's logger — the logger is unavailable before the host is built, but the static class needed it at the same time.

The injectable singleton solves both. And it introduces a useful side effect: Docker Secret rotation is picked up automatically on the next request. Overwrite the file on the host, and the updated key is read without restarting the container.

If a provider's secret file is missing or empty, `ReadApiKey` logs a warning and returns null. The provider remains in the chain but every attempt immediately produces an `AuthError`, which the execution loop skips — consistent with how a live `401` from the provider is handled. The fallback trace in the operation log makes the reason visible.
:::lang

:::lang hu
## Kérésenkénti kulcs olvasás

A Docker Secrets megoldja a tárolási problémát. Ugyanannyira fontos implementációs döntés: mi történik, amikor a kulcsra ténylegesen szükség van.

A `SecretReader` injectable singleton. Minden `CompleteAsync` hívásnál — az a metódus az `OpenAiCompatibleProvider`-ben, amely elküldi a HTTP kérést — közvetlenül a `/run/secrets/`-ből olvas. A kulcs soha nem kerül mezőbe. Kizárólag a kérés stack frame-jén él, és a hívás befejeztével eldobásra kerül.

Ez szándékos refaktor volt egy korábbi statikus osztály megközelítésből. Két probléma kényszerítette ki a változtatást. Először: az API kulcsok plain stringként a process memóriában a teljes alkalmazás élettartama alatt rosszabb biztonsági helyzet, mint egy kérésenkénti olvasás. Másodszor: egy statikus osztály sorrendiségi problémát okoz a DI container loggerével — a logger nem elérhető a host felépítése előtt, de a statikus osztálynak ugyanakkor volt rá szüksége.

Az injectable singleton mindkettőt megoldja. És bevezet egy hasznos mellékhatást: a Docker Secret rotáció automatikusan felvevődik a következő kérésnél. Írjuk felül a fájlt a gazdagépen, és a frissített kulcs container újraindítás nélkül olvasódik be.

Ha egy provider secret fájlja hiányzik vagy üres, a `ReadApiKey` figyelmeztetést naplóz és null értéket ad vissza. A provider a láncban marad, de minden kísérlet azonnal `AuthError`-t produkál, amelyet a végrehajtási ciklus kihagy — összhangban azzal, ahogy egy élő `401` kerül kezelésre. A műveleti napló fallback trace-e láthatóvá teszi az okot.
:::lang

:::section

:::section teaser

:::lang en
## Next: Observability

Provider selection is handled. Keys are managed. The remaining question is visibility: once the proxy is running, how do you know what is actually happening inside it?

[Part 4](/posts/inferrouter-04.html) covers the operation log — a provider-agnostic JSONL file that records every inference event — and three HTTP endpoints that expose live state without touching the log.
:::lang

:::lang hu
## Következő: Megfigyelhetőség

A provider kiválasztás kezelve. A kulcsok kezelve. A fennmaradó kérdés a láthatóság: ha a proxy fut, hogyan tudod, mi történik valójában benne?

A [4. rész](/posts/inferrouter-04.html) a műveleti naplóval foglalkozik — egy provider-agnosztikus JSONL fájllal, amely minden inferencia eseményt rögzít — és három HTTP végponttal, amelyek az élő állapotot teszik láthatóvá a naplófájlok érintése nélkül.
:::lang

:::section
