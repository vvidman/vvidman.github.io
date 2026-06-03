---
title: InferRouter: One Endpoint, Any Provider
slug: posts/inferrouter-01.html
layout: page
description: Why managing LLM providers per project breaks down — and how a single self-hosted proxy with strategy-based inference routing solves it.
date: 2026-06-03
series: InferRouter
part: 1
---

:::section intro

:::lang en
## InferRouter: One Endpoint, Any Provider

Running multiple AI projects in parallel means every one of them needs LLM inference. For a while, every one of them handled it separately — which provider to call, how to manage the API key, what to do when a quota runs out.

The duplication wasn't incidental. It was structural: provider management was treated as application logic rather than infrastructure. The fix is to move it one layer up.

**InferRouter** is a self-hosted proxy that exposes a single OpenAI-compatible HTTP endpoint. Behind it, a configurable inference strategy routes each request across one or more cloud providers. When all of them are unavailable or exhausted, a local quantized model — running in-process via LlamaSharp — handles the call. The caller sees none of this.

Available on Github: [InferRouter](https://github.com/vvidman/InferRouter)

:::lang

:::lang hu
## InferRouter: egy végpont, bármilyen provider

Párhuzamosan több AI projekten dolgozni azt jelenti, hogy mindegyiknek LLM inferenciára van szüksége. Egy ideig mindegyik külön kezelte — melyik providert hívja, hogyan kezelje az API kulcsot, mit tegyen, ha elfogy a kvóta.

A duplikáció nem véletlen volt. Strukturális volt: a provider menedzsmentet alkalmazáslogikának tekintették, nem infrastruktúrának. A megoldás az, hogy egy réteggel feljebb toljuk.

Az **InferRouter** egy self-hosted proxy, amely egyetlen OpenAI-kompatibilis HTTP végpontot exponál. Mögötte egy konfigurálható inferencia stratégia irányítja az egyes kéréseket egy vagy több cloud provider között. Ha mindegyik elérhetetlen vagy kimerült, egy helyi kvantált modell — amely in-process fut LlamaSharp segítségével — kezeli a hívást. A hívó fél ebből semmit nem érzékel.

Elérhető a Githubon: [InferRouter](https://github.com/vvidman/InferRouter)
:::lang

:::section

:::section problem

:::lang en
## What Was Actually Broken

The visible symptom was duplicated code. The underlying problem was coupling: every project knew about its own providers, held its own keys, and implemented its own fallback behaviour independently.

Three things broke consistently:

**API keys scattered.** Each project stored credentials differently. No single place to rotate a key, no single place to audit access.

**Routing logic reimplemented.** Deciding which provider to try, in what order, under what conditions — this is not application logic. Writing it once per project means maintaining it N times.

**Provider changes cascade.** Swapping a provider, adjusting a rate limit, or pointing to a new endpoint meant touching every project that used it.

Provider management is infrastructure. Infrastructure belongs in one place.
:::lang

:::lang hu
## Mi volt valójában elrontva?

A látható tünet duplikált kód volt. A mögöttes probléma a csatolás volt: minden projekt ismerte a saját providereit, tárolta a saját kulcsait, és egymástól függetlenül implementálta a saját fallback viselkedését.

Három dolog tört el következetesen:

**Szétszórt API kulcsok.** Minden projekt más módszerrel tárolta a hitelesítő adatokat. Nincs egyetlen hely a kulcs rotáláshoz, nincs egyetlen hely az elérés auditáláshoz.

**Routing logika újraimplementálva.** Eldönteni, melyik providert próbálja meg, milyen sorrendben, milyen feltételek mellett — ez nem alkalmazáslogika. Projektenként egyszer megírni azt jelenti: N-szer karbantartani.

**Provider csere kaszkád hatással jár.** Egy provider cseréje, rate limit módosítása, vagy új endpoint beállítása minden érintett projektet érintett.

A provider menedzsment infrastruktúra. Az infrastruktúra egy helyen van.
:::lang

:::section

:::section concept

:::lang en
## The Solution Shape

InferRouter sits between callers and providers. Callers speak the OpenAI chat completions API — the same interface they already use. InferRouter applies a configurable inference strategy to select which provider handles each request.

The only requirements for a provider entry in the chain: an OpenAI-compatible HTTP endpoint and a valid config block. Any cloud inference service that speaks the protocol qualifies. The chain can hold one provider or many — using more than one makes the system resilient to individual provider outages and quota exhaustion.

When the inference strategy has no eligible provider left — because all are rate-limited, failing, or exhausted — LlamaSharp takes over. It runs a local GGUF model in-process, with no network dependency. It is never part of strategy selection; its role is to guarantee a response regardless of what the network is doing.

The project is publicly available on GitHub with a full README, unit and integration tests, and ADRs 001–007 documenting every key design decision.
:::lang

:::lang hu
## A megoldás formája

Az InferRouter a hívók és a providerek között helyezkedik el. A hívók az OpenAI chat completions API-t használják — ugyanazt az interfészt, amelyet már ismernek. Az InferRouter egy konfigurálható inferencia stratégiát alkalmaz, hogy meghatározza, melyik provider kezeli az egyes kéréseket.

A lánc provider bejegyzéseinek egyetlen követelménye: OpenAI-kompatibilis HTTP végpont és érvényes config blokk. Bármely cloud inferencia szolgáltatás, amely a protokollt ismeri, megfelel. A lánc tartalmazhat egy vagy több providert — egynél több használata ellenállóvá teszi a rendszert az egyes provider kimaradásokkal és kvóta kimerüléssel szemben.

Ha az inferencia stratégiának már nincs elérhető providere — mert mindegyik rate-limitelt, hibás vagy kimerült — a LlamaSharp veszi át. In-process futtat egy helyi GGUF modellt, hálózati függőség nélkül. Soha nem része a stratégia kiválasztásnak; szerepe az, hogy garantáljon választ, függetlenül attól, mi történik a hálózaton.

A projekt nyilvánosan elérhető GitHubon, teljes README-vel, unit és integrációs tesztekkel, valamint ADR 001–007 dokumentumokkal, amelyek minden főbb tervezési döntést rögzítenek.
:::lang

:::section

:::section ollama

:::lang en
## What About Ollama?

Ollama is the obvious question. It already exposes an OpenAI-compatible HTTP endpoint and runs local models. Why build something else?

Because Ollama solves a different problem. It is a local model server — excellent at what it does, but scoped to a single machine and a single model at a time. It has no concept of multiple cloud providers, no rate limit tracking across services, no API key management, and no routing strategy.

InferRouter's problem domain is managing inference across multiple providers — some cloud, some local — with a configurable strategy for how requests are distributed, a structured log of every call, and secrets handled safely.

The more important point: Ollama is a valid InferRouter provider. Add it to the config as an `openai_compatible` entry pointing at `http://localhost:11434/v1` and it participates in the inference strategy like any other provider. Run Ollama on the same host, point InferRouter at it, and the local model becomes part of the routing pool alongside whatever cloud providers are configured.

InferRouter and Ollama are not alternatives. They solve adjacent problems, and they compose.
:::lang

:::lang hu
## Mi a helyzet az Ollamával?

Az Ollama kézenfekvő kérdés. Már exponál egy OpenAI-kompatibilis HTTP végpontot és helyi modelleket futtat. Miért kellett valami mást építeni?

Mert az Ollama egy másik problémát old meg. Helyi modell szerver — kiváló abban amit csinál, de egyetlen gépre és egyetlen modellre egyszerre korlátozódik. Nincs fogalma több cloud providerről, nincsenek kereszt-szolgáltatói rate limit nyomkövetési képességei, nincs API kulcs menedzsment, és nincs routing stratégia.

Az InferRouter problématerülete az inferencia kezelése több provider között — néhány cloud, néhány helyi — konfigurálható stratégiával arra vonatkozóan, hogyan kerülnek elosztásra a kérések, minden hívás strukturált naplójával, és biztonságosan kezelt titkokkal.

A fontosabb pont: az Ollama érvényes InferRouter provider. Add hozzá a confighoz `openai_compatible` bejegyzésként, a `http://localhost:11434/v1`-re mutatva, és úgy vesz részt az inferencia stratégiában, mint bármely más provider. Futtasd az Ollamát ugyanazon a hoston, mutass rá az InferRouterrel, és a helyi modell a routing pool részévé válik bármely konfigurált cloud provider mellett.

Az InferRouter és az Ollama nem alternatívák egymásnak. Szomszédos problémákat oldanak meg, és összerakhatók.
:::lang

:::section

:::section series

:::lang en
## This Series

- **Part 1 (this post):** The problem and the solution shape
- **[Part 2:](/posts/inferrouter-02.html)** Inference strategies — how ProviderOrchestrator selects and routes requests
- **[Part 3:](/posts/inferrouter-03.html)** Secret management — where API keys live and why it matters
- **[Part 4:](/posts/inferrouter-04.html)** Observability — the operation log and live state endpoints
:::lang

:::lang hu
## A sorozatról

- **1. rész (ez a poszt):** A probléma és a megoldás formája
- **[2. rész:](/posts/inferrouter-02.html)** Inferencia stratégiák — hogyan választja ki és irányítja a kéréseket a ProviderOrchestrator
- **[3. rész:](/posts/inferrouter-03.html)** Titkos kulcsok kezelése — hol éljenek az API kulcsok és miért számít
- **[4. rész:](/posts/inferrouter-04.html)** Megfigyelhetőség — a műveleti napló és az élő állapot végpontok
:::lang

:::section
