---
title: About
slug: about/index.html
layout: page
description: Software architect with 11+ years in .NET backend development and a growing focus on AI-augmented engineering workflows.
---

:::section me
:::lang en
![Viktor Vidman](/assets/images/vv-profile.png){width=380}
:::lang
:::lang hu
![Vidman Viktor](/assets/images/vv-profile.png){width=380}
:::lang
:::section

:::section whoiam
:::lang en
## Who I am

I am a **software architect** with over sixteen years of professional experience in backend development,
primarily in **C# and .NET**. The last eleven of those years I've spent at a single employer — not by accident,
but because the problems kept getting more interesting. I was promoted to architect based on what I built,
not how long I waited.

My domain is systems that have to keep working while you improve them: existing users, existing data,
existing code. Most real-world engineering happens inside those constraints, and I find that more
interesting than greenfield projects.

Alongside that, I've spent the past two years developing a second area of focus:
**AI-augmented development workflows** — not as a curiosity, but as a serious design problem.
:::lang

:::lang hu
## Ki vagyok

**Szoftver architekt** vagyok, tizenhat évnyi szakmai tapasztalattal, elsősorban **C# és .NET** technológiákban.
Az utolsó tizenegy évet egyetlen munkahelyen töltöttem — nem véletlenül, hanem mert a problémák egyre érdekesebbé váltak.
Architektté nem a várakozással kerültem, hanem azzal, amit felépítettem.

A szakterületem: rendszerek, amelyeknek működniük kell, miközben fejlesztjük őket.
Meglévő felhasználók, meglévő adatok, meglévő kód. A valódi mérnöki munka nagy része
ezeken a korlátok között zajlik — és ezt érdekesebbnek találom a zöldmezős projekteknél.

Az elmúlt két évben egy második fókuszterületet is kialakítottam:
**AI-augmented fejlesztési workflow-k** — nem kíváncsiságból, hanem komoly tervezési kérdésként kezelve.
:::lang
:::section

:::section focus
:::lang en
## What I focus on

- Designing backend architectures that remain maintainable over years, not just at launch
- Evolving legacy systems safely — without disrupting users or corrupting data
- Human-in-the-loop AI workflow design: how to integrate AI into a delivery process without losing engineering discipline
- Structured output validation and error-driven refinement in AI-assisted pipelines
- Multi-agent system architecture and the question of when to trust the agent and when to intervene
- Containerized development and deployment workflows
- CI/CD pipeline design with Jenkins and GitHub Actions

My core technology stack is **C# / .NET** and **ASP.NET Core**, with working knowledge of C++, Python, and PowerShell.
On the AI side: Claude API, OpenAI API, Groq, LlamaSharp, and local LLM setups for privacy-first workflows.
:::lang

:::lang hu
## Mire fókuszálok

- Backend architektúrák tervezése, amelyek évek múlva is karbantarthatók maradnak, nem csak az induláskor
- Legacy rendszerek biztonságos fejlesztése — a felhasználók zavarása és az adatok sérülése nélkül
- Human-in-the-loop AI workflow tervezés: hogyan integrálható az AI egy fejlesztési folyamatba anélkül, hogy elveszítenénk a mérnöki fegyelmet
- Strukturált output validáció és error-driven refinement AI-asszisztált pipeline-okban
- Multi-agent rendszer architektúra: mikor bízzunk meg az ágensben, és mikor avatkozzunk be
- Konténerizált fejlesztési és deployment workflow-k
- CI/CD pipeline tervezés Jenkins és GitHub Actions környezetben

Elsődleges technológiai stackem a **C# / .NET** és az **ASP.NET Core**, kiegészítve C++, Python és PowerShell ismeretekkel.
AI oldalon: Claude API, OpenAI API, Groq, LlamaSharp, és lokális LLM konfigurációk privacy-first workflow-khoz.
:::lang
:::section

:::section thinking
:::lang en
## How I think about software

Good software is shaped by constraints. The question is never "what would we build from scratch?" —
it's "what can we change, safely, given what already exists?" That shift in framing
changes almost every design decision that follows.

I hold the same view about AI-assisted development. The question isn't how to get the AI to do more.
It's how to keep the human in the right position: setting direction, validating outputs,
owning outcomes. **The human orchestrates. The AI executes.**

That principle is behind both personal projects I've built in this space —
[Scaffold Protocol](/posts/post-sp01.html) and [ChaosForge](/posts/post-chaosforge-01.html) —
and it's the lens I apply to my daily work.

I have a formal engineering background (MSc) and I rely on it more than I expected:
structured decomposition, validation boundaries, and the discipline to separate
"this is certainly wrong" from "this requires judgment" turn out to be exactly
the skills that make AI-augmented workflows reliable rather than unpredictable.
:::lang

:::lang hu
## Hogyan gondolkodom a szoftverekről

A jó szoftvert a korlátok formálják. A kérdés soha nem az, hogy "mit építenénk nulláról?" —
hanem az, hogy "mit változtathatunk meg, biztonságosan, a meglévő keretek között?"
Ez a szemléletváltás szinte minden rákövetkező tervezési döntést megváltoztat.

Az AI-asszisztált fejlesztésről ugyanígy gondolkodom. Nem az a kérdés, hogyan csináltassunk
többet az AI-val. Hanem az, hogyan tartsuk az embert a megfelelő pozícióban:
irányítja a folyamatot, validálja az outputot, felel az eredményért.
**Az ember orchestrál. Az AI végrehajtja.**

Ez az elv áll mindkét személyes projektem mögött —
[Scaffold Protocol](/posts/post-sp01.html) és [ChaosForge](/posts/post-chaosforge-01.html) —
és ugyanezt a szemléletet alkalmazom a napi munkámra is.

Formális mérnöki háttérrel rendelkezem (MSc), és jobban támaszkodok rá, mint vártam:
a strukturált dekompozíció, a validációs határok meghúzása, és az a fegyelem, hogy szétválasszuk
a "ez biztosan rossz" és a "ez emberi ítéletet igényel" kategóriákat — pontosan ezek azok
a képességek, amelyek az AI-augmented workflow-kat kiszámíthatóvá teszik, nem kiszámíthatatlanná.
:::lang
:::section

:::section about
:::lang en
## About this blog

This blog documents what I'm actually building and thinking — not polished retrospectives,
but decisions made in real time, with the reasoning that went into them.

The format is bilingual by design: English for reach, Hungarian because that's the language
I think in when the problem is hard.

The blog itself is built on a **custom C# static site generator** I wrote — which means
the infrastructure is also a project, with its own ADRs and its own bugs.
That felt more honest than using something off the shelf.
:::lang

:::lang hu
## A blogról

Ez a blog azt dokumentálja, amit ténylegesen építek és amiről gondolkozom —
nem csiszolt visszatekintések, hanem valós idejű döntések, a mögöttük álló érveléssel együtt.

A formátum szándékosan kétnyelvű: angol a szélesebb elérés miatt, magyar mert nehéz problémáknál
ezen a nyelven gondolkozom.

A blog egy általam írt **egyedi C# statikus oldalgenerátoron** fut — ami azt jelenti,
hogy maga az infrastruktúra is projekt, saját ADR-ekkel és saját bugokkal.
Ez őszintébbnek tűnt, mint egy kész megoldás használata.
:::lang
:::section

:::section certs
:::lang en
## Licenses & certifications
Here you can see the trainings I have completed recently:
[Licenses & certifications](/posts/post-01.html)
:::lang
:::lang hu
## Képzések és tanusítványok
Itt láthatod a képzéseket miket elvégeztem mostanában:
[Képzések és tanusítványok](/posts/post-01.html)
:::lang
:::section