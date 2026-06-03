---
title: InferRouter Part 4: Observability
slug: posts/inferrouter-04.html
layout: page
description: How InferRouter makes its internal state visible — a provider-agnostic JSONL operation log and three HTTP endpoints for live health, quota stats, and log history.
date: 2026-06-03
series: InferRouter
part: 4
---

:::section intro

:::lang en
## Part 4: Observability

[Part 3](/posts/inferrouter-03.html) covered secret management. The core concerns — inference strategy, execution loop, key handling — are now closed. This part covers the remaining question: once the proxy is running, how do you know what is actually happening inside it?

InferRouter answers this in two layers. The first is the operation log: a structured JSONL file that captures every inference event in a provider-agnostic format. The second is three HTTP endpoints that expose live internal state without requiring log file access.
:::lang

:::lang hu
## 4. rész: Megfigyelhetőség

A [3. rész](/posts/inferrouter-03.html) a titkos kulcsok kezelését tárgyalta. Az alapvető szempontok — inferencia stratégia, végrehajtási ciklus, kulcs kezelés — most már le vannak zárva. Ez a rész a fennmaradó kérdéssel foglalkozik: ha a proxy fut, hogyan tudod, mi történik valójában benne?

Az InferRouter erre két rétegen válaszol. Az első a műveleti napló: egy strukturált JSONL fájl, amely minden inferencia eseményt provider-agnosztikus formátumban rögzít. A második három HTTP végpont, amelyek az élő belső állapotot teszik láthatóvá naplófájl-hozzáférés nélkül.
:::lang

:::section

:::section log

:::lang en
## The Operation Log

Every inference call produces one or more JSONL entries. Log entries are written to a per-day file — `operations-{yyyy-MM-dd}.jsonl` — and a new file is started automatically at UTC midnight.

The schema is built on one design decision: the log records **events**, not providers. The provider is an attribute on the event, not a structural element of the schema.

This matters when providers change. If the schema were provider-centric — separate fields or log shapes per provider — adding or removing a provider would require a schema migration. Any downstream consumer would need updating. With provider as an attribute, the schema stays stable regardless of how the provider chain evolves.

A successful request produces one entry:

```json
{"ts":"2026-05-25T10:00:00Z","request_id":"uuid","event":"infer_completed","provider":"provider-a","model":"model-name","prompt_tokens":120,"completion_tokens":340,"latency_ms":310,"fallback":false,"status":"ok"}
```

When the strategy falls through to a second provider, two entries are written — one for the transition, one for the completion:

```json
{"ts":"2026-05-25T10:00:05Z","request_id":"uuid","event":"infer_fallback","from_provider":"provider-a","to_provider":"provider-b","reason":"rate_limit"}
{"ts":"2026-05-25T10:00:06Z","request_id":"uuid","event":"infer_completed","provider":"provider-b","model":"model-name","prompt_tokens":120,"completion_tokens":340,"latency_ms":890,"fallback":true,"status":"ok"}
```

The `request_id` field ties all entries for a single request together. Reconstructing the full routing trace — including multiple provider transitions — is straightforward from the flat log without any joins.

Six event types are defined: `infer_started`, `infer_ordering` (records the provider order the active strategy resolved), `infer_completed`, `infer_fallback`, `infer_failed`, `rate_limit_hit`.
:::lang

:::lang hu
## A műveleti napló

Minden inferencia hívás egy vagy több JSONL bejegyzést produkál. A naplóbejegyzések napi fájlba kerülnek — `operations-{yyyy-MM-dd}.jsonl` — és UTC éjfélkor automatikusan új fájl indul.

A séma egy tervezési döntésre épül: a napló **eseményeket** rögzít, nem providereket. A provider az esemény egy attribútuma, nem a séma strukturális eleme.

Ez akkor számít, amikor a providerek változnak. Ha a séma provider-centrikus lenne — külön mezők vagy naplóformák providerenként — egy provider hozzáadása vagy eltávolítása sémamigrációt igényelne. Minden downstream fogyasztót frissíteni kellene. Ha a provider attribútum, a séma stabil marad, függetlenül attól, hogyan fejlődik a provider lánc.

Egy sikeres kérés egy bejegyzést produkál:

```json
{"ts":"2026-05-25T10:00:00Z","request_id":"uuid","event":"infer_completed","provider":"provider-a","model":"model-name","prompt_tokens":120,"completion_tokens":340,"latency_ms":310,"fallback":false,"status":"ok"}
```

Amikor a stratégia egy második providerre esik át, két bejegyzés íródik — egy az átmenetért, egy a befejezésért:

```json
{"ts":"2026-05-25T10:00:05Z","request_id":"uuid","event":"infer_fallback","from_provider":"provider-a","to_provider":"provider-b","reason":"rate_limit"}
{"ts":"2026-05-25T10:00:06Z","request_id":"uuid","event":"infer_completed","provider":"provider-b","model":"model-name","prompt_tokens":120,"completion_tokens":340,"latency_ms":890,"fallback":true,"status":"ok"}
```

A `request_id` mező összeköti egyetlen kérés összes bejegyzését. A teljes routing trace rekonstruálása — beleértve a több provider átmenetet — egyértelmű a sík naplóból, join-ok nélkül.

Hat eseménytípus van definiálva: `infer_started`, `infer_ordering` (rögzíti az aktív stratégia által feloldott provider sorrendet), `infer_completed`, `infer_fallback`, `infer_failed`, `rate_limit_hit`.
:::lang

:::section

:::section endpoints

:::lang en
## Live State Endpoints

Three HTTP endpoints expose the internal state of the running proxy in real time.

**`GET /health/providers`** contacts every configured provider with a minimal one-token request and returns its current status. The endpoint always returns `200 OK` — provider status is expressed in the response body, not the HTTP status code. Status values mirror the internal error taxonomy: `ok`, `rate_limit`, `auth_error`, `server_error`, `model_unavailable`, `unknown_error`.

**`GET /stats/live`** returns the current rate limit state for every configured provider: daily limit, daily count, RPM limit, RPM window count, and whether the provider is currently marked exhausted. This reads directly from the in-memory `RateLimitTracker` — no log file access involved.

**`GET /stats/history`** returns the raw JSONL content of a day's operation log. Without a query parameter it returns today's log (UTC). A `?date=yyyy-MM-dd` parameter retrieves a specific day. Returns `404` if the log file does not exist for the requested date.

The three endpoints answer three distinct questions: is a given provider reachable right now, how much quota remains for the day, and what happened over a specific day's worth of requests. Together with the operation log, they provide full visibility into the proxy's behaviour without requiring access to the host filesystem.
:::lang

:::lang hu
## Élő állapot végpontok

Három HTTP végpont teszi valós időben láthatóvá a futó proxy belső állapotát.

A **`GET /health/providers`** minden konfigurált providert megkeres egy minimális egy-token kéréssel, és visszaadja az aktuális státuszát. A végpont mindig `200 OK`-t ad vissza — a provider státuszt a válasz törzse tartalmazza, nem a HTTP státuszkód. A státusz értékek a belső hiba taxonómiát tükrözik: `ok`, `rate_limit`, `auth_error`, `server_error`, `model_unavailable`, `unknown_error`.

A **`GET /stats/live`** minden konfigurált provider aktuális rate limit állapotát adja vissza: napi limit, napi darabszám, RPM limit, RPM ablak darabszám, és hogy az adott provider jelenleg kimerültként van-e jelölve. Ez közvetlenül az in-memory `RateLimitTracker`-ből olvas — nem igényel naplófájl-hozzáférést.

A **`GET /stats/history`** egy nap műveleti naplójának nyers JSONL tartalmát adja vissza. Query paraméter nélkül a mai napló (UTC) kerül visszaadásra. `?date=yyyy-MM-dd` paraméterrel egy meghatározott nap kérhető le. `404`-et ad vissza, ha a naplófájl nem létezik a kért dátumra.

A három végpont három különböző kérdésre válaszol: elérhető-e egy adott provider most, mennyi kvóta maradt a napból, és mi történt egy adott nap kérései során. A műveleti naplóval együtt teljes láthatóságot biztosítanak a proxy viselkedésébe anélkül, hogy hozzáférés szükséges lenne a gazdagép fájlrendszeréhez.
:::lang

:::section

:::section status

:::lang en
## Where It Stands

InferRouter is implemented and publicly available on GitHub. The complete stack — `ProviderOrchestrator` with three pluggable inference strategies, `RateLimitTracker`, `ErrorNormalizer`, `OperationLogger`, `ProviderHealthChecker`, `StatsService`, `SecretReader` — is complete, unit and integration tested. The Docker Compose stack runs with file-based secrets and a local model volume.

The project ships with a README and ADRs 001–007. It is production-ready: stable, tested, and deployable as a drop-in OpenAI-compatible inference backend.
:::lang

:::lang hu
## Aktuális állapot

Az InferRouter implementálva van és nyilvánosan elérhető GitHubon. A teljes stack — `ProviderOrchestrator` három csatlakoztatható inferencia stratégiával, `RateLimitTracker`, `ErrorNormalizer`, `OperationLogger`, `ProviderHealthChecker`, `StatsService`, `SecretReader` — kész, unit és integrációs tesztekkel lefedett. A Docker Compose stack file-based titkokkal és helyi modell kötettel fut.

A projekt README-vel és ADR 001–007 dokumentumokkal érkezik. Production-ready: stabil, tesztelt, és telepíthető drop-in OpenAI-kompatibilis inferencia backendként.
:::lang

:::section
