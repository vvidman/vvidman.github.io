---
title: From .NET Dev to AI Engineer — Why I Started This Series
title_hu: .NET fejlesztőből AI Engineer — Miért kezdtem el ezt a sorozatot
slug: posts/dotnet-ai-stack-00.html
description: I completed a strong theoretical course on Agentic AI. Then I realised I could not build anything with it. This is what I did about that.
layout: page
date: 2026-05-10
series: .NET Agentic AI
part: 0
---

:::section intro

:::lang en
## The gap between knowing and building

I completed a course. A good one — [Agentic AI Architectures with Patterns, Frameworks and MCP](https://www.udemy.com/course/agentic-ai-architectures-with-patterns-frameworks-and-mcp/) by Mehmet Ozkaya on Udemy. The theory was solid. Agentic patterns, orchestration vs choreography, RAG pipelines, MCP, HITL — I understood the concepts. I could talk about them.

What I could not do was build anything.

That is not a criticism of the course. The theory is the theory, and it is well taught. But understanding a concept and implementing it are different cognitive events. Several other students said the same thing in the reviews. I agreed with them, and I decided to do something about it.
:::lang

:::lang hu
## A tudás és az építés közötti szakadék

Teljesítettem egy kurzust. Egy jót — Mehmet Ozkaya [Agentic AI Architectures with Patterns, Frameworks and MCP](https://www.udemy.com/course/agentic-ai-architectures-with-patterns-frameworks-and-mcp/) kurzusát az Udemy-n. Az elmélet szilárd volt. Agentic patternek, orchestration kontra choreography, RAG pipeline-ok, MCP, HITL — értettem a koncepciókat. Tudtam róluk beszélni.

Amit nem tudtam, az az, hogy bármit is felépítsek.

Ez nem a kurzus kritikája. Az elmélet az elmélet, és jól van tanítva. De egy fogalmat megérteni és implementálni két különböző kognitív esemény. Több más hallgató is ugyanezt mondta az értékelésekben. Egyet értettem velük, és úgy döntöttem, teszek valamit ez ellen.
:::lang

:::section

:::section what-this-is

:::lang en
## What this series is

Seven modules. Seven blog posts. One GitHub repository with one branch per module, each building on the previous.

This is not a tutorial. I am not going to walk you through setting up a project from scratch and explain every line. What I will do is show you what I built, document the decisions I made and why, and be honest about what broke, what surprised me, and what I would do differently.

The code is in [dotnet-ai-labs](https://github.com/vvidman/dotnet-ai-labs). The implementation choices reflect my own thinking — this is not an official companion to the Udemy course, and it does not cover the same ground in the same order. The course was the inspiration. The labs are my interpretation.

If you are a .NET developer who wants to understand Agentic AI by building it — not just reading about it — this series is for you.
:::lang

:::lang hu
## Mi ez a sorozat

Hét modul. Hét blogbejegyzés. Egy GitHub repository, modulonként egy branch-csel, mindegyik az előzőre épülve.

Ez nem egy tutorial. Nem fogom végigvezetni, hogyan kell felállítani egy projektet az alapoktól, és megmagyarázni minden sort. Amit csinálok: megmutatom, mit építettem, dokumentálom a döntéseket és azok indokait, és őszinte leszek azzal kapcsolatban, mi tört el, mi lepett meg, és mit csinálnék másképp.

A kód a [dotnet-ai-labs](https://github.com/vvidman/dotnet-ai-labs) repositoryban van. Az implementációs döntések a saját gondolkodásomat tükrözik — ez nem az Udemy kurzus hivatalos kísérője, és nem ugyanazt az anyagot fedi le ugyanabban a sorrendben. A kurzus volt az inspiráció. A laborok az én értelmezésem.

Ha .NET fejlesztő vagy, aki az Agentic AI-t azzal akarja megérteni, hogy megépíti — nem csak olvas róla — ez a sorozat neked szól.
:::lang

:::section

:::section the-stack

:::lang en
## The stack

Everything runs locally. No cloud provider, no API key, no cost per token.

```
┌──────────────────────────────────────────────────────┐
│                   YOUR CODE                          │
│         (business logic, agent workflows)            │
├──────────────────────────────────────────────────────┤
│             SEMANTIC KERNEL                          │
│   "What should the agent do?"                        │
│   Plugins · Memory · Planner · AgentGroupChat        │
├──────────────────────────────────────────────────────┤
│         MICROSOFT.EXTENSIONS.AI                      │
│   "How to talk to any LLM?"                          │
│   IChatClient — unified interface for all providers  │
├──────────────────────────────────────────────────────┤
│            LLM PROVIDER (swappable)                  │
│   LlamaSharp │ Azure OpenAI │ OpenAI │ Ollama │ ...  │
└──────────────────────────────────────────────────────┘
```

The model is [Phi-3-mini-4k-instruct](https://huggingface.co/microsoft/Phi-3-mini-4k-instruct-gguf) — a 3.8B parameter model from Microsoft, quantised to Q4, around 2.3 GB. It runs on CPU with 8 GB of RAM. No GPU required.

Phi-3 is lightweight, well-documented, and linkable directly from Hugging Face. It is a Microsoft model, which fits the stack. It is not the most capable model for tool calling — and that will become visible in later modules. I will document it when it does.

The runtime is **.NET 10** throughout.
:::lang

:::lang hu
## A stack

Minden lokálisan fut. Nincs felhőszolgáltató, nincs API kulcs, nincs tokenalapú költség.

```
┌──────────────────────────────────────────────────────┐
│                  A TE KÓDOD                          │
│         (business logic, agent workflows)            │
├──────────────────────────────────────────────────────┤
│             SEMANTIC KERNEL                          │
│   "Mit csináljon az agent?"                          │
│   Plugins · Memory · Planner · AgentGroupChat        │
├──────────────────────────────────────────────────────┤
│         MICROSOFT.EXTENSIONS.AI                      │
│   "Hogyan kommunikáljon bármely LLM-mel?"            │
│   IChatClient — egységes interfész minden providerre │
├──────────────────────────────────────────────────────┤
│            LLM PROVIDER (cserélhető)                 │
│   LlamaSharp │ Azure OpenAI │ OpenAI │ Ollama │ ...  │
└──────────────────────────────────────────────────────┘
```

A modell a [Phi-3-mini-4k-instruct](https://huggingface.co/microsoft/Phi-3-mini-4k-instruct-gguf) — egy 3.8B paraméteres modell a Microsofttól, Q4-re kvantálva, körülbelül 2.3 GB. CPU-n fut, 8 GB RAM-mal. GPU nem szükséges.

A Phi-3 lightweight, jól dokumentált, és közvetlenül linkelhető a Hugging Face-ről. Microsoft modell, ami illik a stackhez. Nem a legképesebb modell tool calling szempontjából — ez a későbbi modulokban láthatóvá válik. Dokumentálom majd, amikor megtörténik.

A runtime végig **.NET 10**.
:::lang

:::section

:::section the-modules

:::lang en
## The seven modules

| # | Topic | Key technologies |
|---|-------|-----------------|
| 1 | [The .NET AI Stack](dotnet-ai-stack-01.html) | M.E.AI · IChatClient · LlamaSharp · SK Kernel |
| 2 | Tool Calling & Function Calling | KernelFunction · Auto/Manual tool loop |
| 3 | Memory Architecture (STM/LTM) | ChatHistory · Vector DB · Embeddings |
| 4 | RAG → Agentic RAG | Passive pipeline → Active retrieval · Qdrant |
| 5 | Multi-Agent Orchestration | AgentGroupChat · Orchestration vs Choreography |
| 6 | MCP (Model Context Protocol) | MCP Server · MCP Client · REST vs MCP |
| 7 | Security, HITL & Guardrails | Least Privilege · HITL · Prompt injection · Audit log |

Links will be added as each post is published.
:::lang

:::lang hu
## A hét modul

| # | Téma | Kulcstechnológiák |
|---|------|-------------------|
| 1 | [A .NET AI Stack](dotnet-ai-stack-01.html) | M.E.AI · IChatClient · LlamaSharp · SK Kernel |
| 2 | Tool Calling & Function Calling | KernelFunction · Auto/Manual tool loop |
| 3 | Memory Architecture (STM/LTM) | ChatHistory · Vector DB · Embeddings |
| 4 | RAG → Agentic RAG | Passzív pipeline → Aktív retrieval · Qdrant |
| 5 | Multi-Agent Orchesztráció | AgentGroupChat · Orchestration vs Choreography |
| 6 | MCP (Model Context Protocol) | MCP Server · MCP Client · REST vs MCP |
| 7 | Biztonság, HITL & Guardrails | Least Privilege · HITL · Prompt injection · Audit log |

A linkek a bejegyzések publikálásakor kerülnek hozzáadásra.
:::lang

:::section

:::section repo-structure

:::lang en
## How the repository is structured

The repository is [dotnet-ai-labs](https://github.com/vvidman/dotnet-ai-labs) on GitHub. Each module has its own branch, built on top of the previous one:

```
main
 └── modul-01
      └── modul-02
           └── modul-03
                └── ...
                     └── modul-07
```

Every branch contains a `MODULE-SUMMARY.md` — written at the end of each module, after the code is working. It covers what I learned, what was hard, the decisions I made and their tradeoffs. That document is the direct source material for each blog post.

If you want to follow along, start from `modul-01` and read the `MODULE-SUMMARY.md` before the blog post. The code and the writing are the same experience, just in different formats.
:::lang

:::lang hu
## Hogyan épül fel a repository

A repository a [dotnet-ai-labs](https://github.com/vvidman/dotnet-ai-labs) GitHubon. Minden modulnak saját branch-e van, az előzőre épülve:

```
main
 └── modul-01
      └── modul-02
           └── modul-03
                └── ...
                     └── modul-07
```

Minden branch tartalmaz egy `MODULE-SUMMARY.md` fájlt — amelyet minden modul végén írok, miután a kód működik. Lefedi, amit tanultam, ami nehéz volt, a döntéseket és azok tradeoff-jait. Ez a dokumentum az egyes blogbejegyzések közvetlen forrásanyaga.

Ha követni szeretnéd a folyamatot, kezdj a `modul-01`-gyel, és olvasd el a `MODULE-SUMMARY.md`-t a blogbejegyzés előtt. A kód és az írás ugyanaz az élmény, csak különböző formátumban.
:::lang

:::section

:::section start

:::lang en
## Start here

Module 1 is published: [Building the .NET AI Stack from First Principles](dotnet-ai-stack-01.html).

It covers why three layers are needed, what the documentation gets wrong about LlamaSharp and Semantic Kernel, and how short-term memory actually works. No magic, just code.
:::lang

:::lang hu
## Kezdj itt

Az 1. modul publikálva: [A .NET AI Stack felépítése az alapoktól](dotnet-ai-stack-01.html).

Lefedi, hogy miért szükséges három réteg, mit mond tévesen a dokumentáció a LlamaSharp-ról és a Semantic Kernelről, és hogyan működik valójában a short-term memory. Nincs varázslat, csak kód.
:::lang

:::section
