---
title: InferRouter Part 2: Inference Strategies
slug: posts/inferrouter-02.html
layout: page
description: How InferRouter decides which provider handles each request — three configurable inference strategies, the ProviderOrchestrator execution loop, and the local fallback that is always last.
date: 2026-06-03
series: InferRouter
part: 2
---

:::section intro

:::lang en
## Part 2: Inference Strategies

[Part 1](/posts/inferrouter-01.html) described the concept: one proxy, one endpoint, strategy-based routing across any number of OpenAI-compatible providers. This part goes into how that routing actually works.

InferRouter does not have a hardcoded fallback chain. It has an inference strategy. The strategy is responsible for one thing: given the current state of the configured providers, return an ordered list of candidates to try. Everything else — executing attempts, handling errors, logging, rate limit tracking — belongs to the execution loop.

The local LlamaSharp model is not part of any strategy. It is the guaranteed fallback that the execution loop appends after the strategy's list is exhausted.
:::lang

:::lang hu
## 2. rész: Inferencia stratégiák

Az [1. rész](/posts/inferrouter-01.html) leírta a koncepciót: egy proxy, egy végpont, stratégia alapú routing bármennyi OpenAI-kompatibilis provideren át. Ez a rész arról szól, hogyan működik ez a routing valójában.

Az InferRouternek nincs beégetett fallback lánca. Inferencia stratégiája van. A stratégia egyetlen dologért felelős: az aktuális provider állapot alapján visszaad egy sorrendezett jelölt listát, amelyet meg kell próbálni. Minden más — kísérletek végrehajtása, hibakezelés, naplózás, rate limit követés — a végrehajtási ciklushoz tartozik.

A helyi LlamaSharp modell nem része egyetlen stratégiának sem. Ez a garantált fallback, amelyet a végrehajtási ciklus hozzáfűz a stratégia listájának kimerítése után.
:::lang

:::section

:::section architecture

:::lang en
## The Abstraction

The routing policy is expressed through a single interface:

```csharp
public interface IRoutingStrategy
{
    // Returns ordered cloud providers to attempt for this request.
    // LocalGguf providers are never included — held in reserve by ProviderOrchestrator.
    IReadOnlyList<ILlmProvider> GetOrderedProviders();
}
```

`ProviderOrchestrator` calls `GetOrderedProviders()` once at the start of each request. It appends the LlamaSharp provider to the end, then works through the full list until one succeeds or all fail.

Separating the strategy from the execution loop has a concrete consequence: adding a new routing behaviour means implementing `IRoutingStrategy`. The orchestrator, the error handling, the logging — none of it changes.

The strategy is selected at startup via a single config field:

```json
{
  "RoutingStrategy": "WeightedRoundRobin",
  "Providers": [ ... ]
}
```

If the field is absent or unrecognised, `ChainOfResponsibility` is the silent default. Existing deployments do not break on upgrade.
:::lang

:::lang hu
## Az absztrakció

A routing policy egyetlen interfészen keresztül fejezhető ki:

```csharp
public interface IRoutingStrategy
{
    // Visszaadja a megpróbálandó sorrendezett cloud provider listát ehhez a kéréshez.
    // LocalGguf providerek soha nem szerepelnek — a ProviderOrchestrator tartalékban tartja.
    IReadOnlyList<ILlmProvider> GetOrderedProviders();
}
```

A `ProviderOrchestrator` minden kérés elején egyszer hívja meg a `GetOrderedProviders()`-t. Hozzáfűzi a LlamaSharp providert a végéhez, majd végigdolgozza a teljes listát, amíg valamelyik sikerül, vagy mindegyik meghibásodik.

A stratégia és a végrehajtási ciklus szétválasztásának konkrét következménye van: egy új routing viselkedés hozzáadása az `IRoutingStrategy` implementálását jelenti. Az orchestrator, a hibakezelés, a naplózás — semmi nem változik.

A stratégiát indításkor egyetlen config mezőn keresztül választják ki:

```json
{
  "RoutingStrategy": "WeightedRoundRobin",
  "Providers": [ ... ]
}
```

Ha a mező hiányzik vagy ismeretlen, a `ChainOfResponsibility` az alapértelmezett. A meglévő telepítések frissítéskor nem törnek el.
:::lang

:::section

:::section strategies

:::lang en
## Three Strategies

**ChainOfResponsibility** — providers are tried in the order they appear in config. Every request goes to the first provider. If it is rate-limited or failing, the next one takes over. The routing order is exactly what the operator wrote.

Use this when providers have a clear priority: one is preferred for latency, cost, or reliability, and others are fallbacks. The most predictable and debuggable option.

**WeightedRoundRobin** — requests are distributed across providers in proportion to their `DailyRequestLimit`. A provider with a higher daily quota receives proportionally more traffic. Weights are derived automatically — no separate config field is needed.

Providers with `DailyRequestLimit: 0` are excluded from weighted selection. A zero value means no local quota is configured, so there is no basis for deriving a weight.

Use this when multiple providers are at comparable quality and spreading load across their combined quotas matters more than a fixed priority order.

**LeastUsed** — at each routing decision, selects the provider with the lowest utilisation ratio (`DailyCount / DailyLimit`). This normalises across providers with different quota sizes: a provider at 500 of 1000 requests used (50%) is considered more constrained than one at 200 of 2000 (10%), even though it has handled fewer absolute requests.

Tiebreaks are random. When multiple providers have the same ratio — including at the start of a day when all counters are zero — random selection spreads load without imposing a hidden ordering.

Use this when avoiding early exhaustion of any single provider's daily quota is the priority. Keeps the full provider pool available later in the day.
:::lang

:::lang hu
## Három stratégia

**ChainOfResponsibility** — a providerek a configban megjelenő sorrendben kerülnek megpróbálásra. Minden kérés az első providerre kerül. Ha rate-limitelt vagy hibás, a következő veszi át. A routing sorrend pontosan az, amit az operátor leírt.

Akkor alkalmazandó, ha a providereknek egyértelmű prioritásuk van: az egyik preferált késleltetés, költség vagy megbízhatóság szempontjából, a többiek fallbackek. A legjobban előrejelezhető és debuggolható lehetőség.

**WeightedRoundRobin** — a kérések a `DailyRequestLimit` arányában kerülnek elosztásra a providerek között. A nagyobb napi kvótával rendelkező provider arányosan több forgalmat kap. A súlyok automatikusan kerülnek levezetésre — nincs szükség külön config mezőre.

A `DailyRequestLimit: 0` értékű providerek ki vannak zárva a súlyozott kiválasztásból. A nulla érték azt jelenti, hogy nincs helyi kvóta konfigurálva, ezért nincs alapja a súly levezetésének.

Akkor alkalmazandó, ha több összehasonlítható minőségű provider áll rendelkezésre, és a kombinált kvótájuk között való terhelosztás fontosabb a fix prioritási sorrendnél.

**LeastUsed** — minden routing döntésnél a legalacsonyabb kihasználtsági arányú (`DailyCount / DailyLimit`) providert választja. Ez normalizál a különböző kvótaméretű providerek között: egy 1000-ből 500 kérést elhasznált provider (50%) korlátosabbnak tekintendő, mint egy 2000-ből 200-at elhasznált (10%), bár kevesebb abszolút kérést kezelt.

A holtverseny véletlenszerűen dől el. Ha több providernek ugyanolyan az aránya — beleértve a nap elejét, amikor minden számláló nullán van — a véletlenszerű kiválasztás elosztja a terhelést egy rejtett sorrend bevezetése nélkül.

Akkor alkalmazandó, ha az elsődleges szempont, hogy egyetlen provider napi kvótája se merüljön ki korán. A teljes provider poolt a nap végéig elérhetővé tartja.
:::lang

:::section

:::section execution

:::lang en
## The Execution Loop

Once the strategy returns its ordered list, `ProviderOrchestrator` appends the local LlamaSharp provider and iterates:

- If a provider's quota is locally known to be exhausted, it is skipped immediately and a `rate_limit_hit` event is logged.
- On a successful response, the request counter is updated and `infer_completed` is logged.
- On a rate limit error (`429` or mapped equivalent), the provider is marked exhausted and `infer_fallback` is logged.
- On a server error, one retry is attempted inline before falling through.
- On an auth error, the provider is skipped permanently for this request — a misconfigured key is not a transient condition.
- If all providers in the list fail, `infer_failed` is logged and an error is returned to the caller.

The strategy never sees errors or results. It only answers one question at the start of the request. The execution loop handles everything that happens after.
:::lang

:::lang hu
## A végrehajtási ciklus

Amint a stratégia visszaadja a sorrendezett listát, a `ProviderOrchestrator` hozzáfűzi a helyi LlamaSharp providert, és iterál:

- Ha egy provider kvótája helyileg ismerten kimerült, azonnal kihagyásra kerül, és `rate_limit_hit` esemény kerül naplózásra.
- Sikeres válasz esetén a kérésszámláló frissül, és `infer_completed` kerül naplózásra.
- Rate limit hiba esetén (`429` vagy megfeleltetett egyenértékű), a provider kimerültként jelölődik és `infer_fallback` kerül naplózásra.
- Szerver hiba esetén egy retry kerül megkísérlésre inline, mielőtt továbblép.
- Auth hiba esetén a provider véglegesen kihagyásra kerül ennél a kérésnél — egy helytelenül konfigurált kulcs nem tranziens állapot.
- Ha a lista összes providere meghibásodik, `infer_failed` kerül naplózásra és hiba kerül visszaadásra a hívónak.

A stratégia soha nem lát hibákat vagy eredményeket. Csak egy kérdésre válaszol a kérés elején. A végrehajtási ciklus kezeli mindazt, ami ezután történik.
:::lang

:::section

:::section local_fallback

:::lang en
## The Local Fallback

LlamaSharp runs a GGUF model in-process. It is registered as the same `ILlmProvider` interface as every cloud provider, but it is never included in strategy selection. `ProviderOrchestrator` appends it unconditionally as the last entry in the attempt list.

This means LlamaSharp is reached only when the inference strategy has no remaining eligible cloud provider — all are rate-limited, failing, or unreachable. It is not a routing preference. It is a guarantee: as long as the model file exists on disk and the process is running, some response will be returned.

The model is loaded lazily on first use to avoid paying the memory cost during periods when cloud providers are healthy.
:::lang

:::lang hu
## A helyi fallback

A LlamaSharp in-process futtat egy GGUF modellt. Ugyanazon `ILlmProvider` interfészen van regisztrálva, mint minden cloud provider, de soha nem szerepel a stratégia kiválasztásban. A `ProviderOrchestrator` feltétel nélkül hozzáfűzi utolsó bejegyzésként a próbálkozási listához.

Ez azt jelenti, hogy a LlamaSharp csak akkor kerül elérésre, ha az inferencia stratégiának nincs maradék elérhető cloud providere — mindegyik rate-limitelt, hibás, vagy elérhetetlen. Ez nem routing preferencia. Ez egy garancia: amíg a modell fájl létezik a lemezen és a folyamat fut, valamiféle válasz visszaadásra kerül.

A modell lazán töltődik be az első használatkor, hogy elkerülje a memória költség kifizetését olyan időszakokban, amikor a cloud providerek egészségesek.
:::lang

:::section

:::section teaser

:::lang en
## Next: Secret Management

The inference strategy is in place. What it still does not know is the API keys for each provider. That question has a non-obvious answer.

[Part 3](/posts/inferrouter-03.html) covers the Docker Secrets decision — why not environment variables, why not config files, and why the key is read fresh on every single request.
:::lang

:::lang hu
## Következő: Titkos kulcsok kezelése

Az inferencia stratégia a helyén van. Amit még nem tud: minden provider API kulcsai. Ennek a kérdésnek nem magától értetődő a válasza.

A [3. rész](/posts/inferrouter-03.html) a Docker Secrets döntést tárgyalja — miért nem environment variable, miért nem config fájl, és miért olvasódik a kulcs frissen minden egyes kérésnél.
:::lang

:::section
