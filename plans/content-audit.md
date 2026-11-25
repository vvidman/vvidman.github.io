# Content audit

> Run: python tools/content_audit.py

## content/about/about.md
- title: About | title_hu: 
- description: Software architect with 16+ years in .NET backend development and a growing focus on AI-augmented engineering workflows.
- description_hu: (missing)
- min heading level: 2
  - [whoiam/en] line 19: h2 `Who I am`
  - [whoiam/hu] line 37: h2 `Ki vagyok`
  - [focus/en] line 56: h2 `What I focus on`
  - [focus/hu] line 73: h2 `Mire fókuszálok`
  - [thinking/en] line 92: h2 `How I think about software`
  - [thinking/hu] line 123: h2 `Hogyan gondolkodom a szoftverekről`
  - [about/en] line 158: h2 `About this blog`
  - [about/hu] line 172: h2 `A blogról`
  - [certs/en] line 188: h2 `Licenses & certifications`
  - [certs/hu] line 193: h2 `Képzések és tanusítványok`

## content/posts/chaosforge-01.md
- title: ChaosForge - Let Go of the Reins | title_hu: ChaosForge – Engedd el a gyeplőt
- description: From Scaffold Protocol to ChaosForge – why I stopped trying to control every AI step, and what I built instead.
- description_hu: A Scaffold Protocoltól a ChaosForge-ig – miért hagytam abba az AI minden lépésének kontrollálását, és mit építettem helyette.
- min heading level: 2
  - [scaffold_context/en] line 64: h2 `Where This Came From`
  - [scaffold_context/hu] line 78: h2 `Honnan jött ez az ötlet?`
  - [the_idea/en] line 96: h2 `A Development Team That Doesn't Sleep`
  - [the_idea/hu] line 125: h2 `Egy fejlesztőcsapat, ami nem alszik`
  - [butterfly_effect/en] line 158: h2 `The Butterfly Effect Is the Feature`
  - [butterfly_effect/hu] line 174: h2 `A pillangó effektus nem mellékhatás – ez a feature`
  - [design_decisions/en] line 194: h2 `The Decisions That Mattered`
  - [design_decisions/hu] line 208: h2 `A döntések, amik számítottak`
  - [closing/en] line 226: h2 `Where It Stands`
  - [closing/hu] line 240: h2 `Ahol most tart`
- fig sections: fig-overview

## content/posts/chaosforge-02-closing-the-loop.md
- title: Closing the Loop | title_hu: Zárd le a hurkot
- description: A practical workflow for pairing a reasoning model with an agentic coding assistant — and closing the structural gap between planning and implementation.
- description_hu: Gyakorlati workflow egy gondolkodó (reasoning) modell és egy agentic kódolóasszisztens párosításához — és a tervezés és a megvalósítás közötti strukturális rés bezárásához.
- min heading level: 2
  - [two_roles/en] line 43: h2 `Two Roles, Two Strengths`
  - [two_roles/hu] line 57: h2 `Két szerepkör, két erősség`
  - [problem/en] line 75: h2 `Where the Loop Breaks`
  - [problem/hu] line 98: h2 `Ahol a hurok megszakad`
  - [solution/en] line 125: h2 `Three Additions to Close the Loop`
  - [solution/hu] line 163: h2 `Három kiegészítés a hurok bezárásához`
  - [closed_loop/en] line 205: h2 `The Full Closed Loop`
  - [closed_loop/hu] line 240: h2 `A teljes zárt hurok`
  - [context_window/en] line 292: h2 `Managing the Context Window as the Project Grows`
  - [context_window/hu] line 317: h2 `A kontextusablak kezelése ahogy a projekt növekszik`
  - [scaling_and_limits/en] line 346: h2 `Scaling and Limits`
  - [scaling_and_limits/hu] line 365: h2 `Skálázás és korlátok`
  - [files_and_templates/en] line 388: h2 `Files, Conventions, and Templates`
  - [files_and_templates/hu] line 417: h2 `Fájlok, konvenciók és sablonok`
- fig sections: fig-closed-loop

## content/posts/dotnet-ai-stack-00.md
- title: From .NET Dev to AI Engineer — Why I Started This Series | title_hu: .NET fejlesztőből AI Engineer — Miért kezdtem el ezt a sorozatot
- description: I completed a strong theoretical course on Agentic AI. Then I realised I could not build anything with it. This is what I did about that.
- description_hu: Elvégeztem egy erős, elméleti Agentic AI kurzust. Aztán rájöttem, hogy semmit sem tudok építeni vele. Ezt tettem ellene.
- min heading level: 2
  - [intro/en] line 16: h2 `The gap between knowing and building`
  - [intro/hu] line 26: h2 `A tudás és az építés közötti szakadék`
  - [what-this-is/en] line 40: h2 `What this series is`
  - [what-this-is/hu] line 52: h2 `Mi ez a sorozat`
  - [the-stack/en] line 68: h2 `The stack`
  - [the-stack/hu] line 98: h2 `A stack`
  - [the-modules/en] line 132: h2 `The seven modules`
  - [the-modules/hu] line 148: h2 `A hét modul`
  - [repo-structure/en] line 168: h2 `How the repository is structured`
  - [repo-structure/hu] line 187: h2 `Hogyan épül fel a repository`
  - [start/en] line 210: h2 `Start here`
  - [start/hu] line 218: h2 `Kezdj itt`

## content/posts/dotnet-ai-stack-01.md
- title: Building the .NET AI Stack from First Principles | title_hu: A .NET AI Stack felépítése az alapoktól
- description: What I learned by wiring together LlamaSharp, Microsoft.Extensions.AI, and Semantic Kernel — and why the documentation does not tell the whole story.
- description_hu: Amit a LlamaSharp, a Microsoft.Extensions.AI és a Semantic Kernel összekötésével tanultam — és miért nem mond el mindent a dokumentáció.
- min heading level: 2
  - [intro/en] line 16: h2 `Problem statement`
  - [intro/hu] line 24: h2 `Probléma megfogalmazás`
  - [the-three-layers/en] line 36: h2 `Why three layers?`
  - [the-three-layers/hu] line 66: h2 `Miért három réteg?`
  - [what-the-docs-miss/en] line 100: h2 `What the documentation does not tell you`
  - [what-the-docs-miss/hu] line 156: h2 `Amit a dokumentáció nem mond el`
  - [middleware-pipeline/en] line 216: h2 `The middleware pipeline`
  - [middleware-pipeline/hu] line 233: h2 `A middleware pipeline`
  - [the-final-code/en] line 254: h2 `The conversation loop`
  - [the-final-code/hu] line 291: h2 `A conversation loop`
  - [what-i-learned/en] line 332: h2 `What I actually learned`
  - [what-i-learned/hu] line 344: h2 `Amit tényleg megtanultam`
  - [whats-next/en] line 360: h2 `What comes next`
  - [whats-next/hu] line 368: h2 `Mi jön ezután`

## content/posts/homelab-docker-server.md
- title: Home Lab Docker Server and Deploy Workflow | title_hu: Otthoni Docker szerver és deploy workflow
- description: How to turn an old machine or WSL2 into a personal Docker host, and set up a deploy workflow that works identically on both.
- description_hu: Hogyan lehet egy régi gépből vagy a WSL2-ből személyes Docker hosztot csinálni, és olyan deploy workflow-t felállítani, amely mindkettőn ugyanúgy működik.
- min heading level: 2
  - [choosing-host/en] line 30: h2 `Choosing Your Host`
  - [choosing-host/hu] line 50: h2 `A hoszt kiválasztása`
  - [setup-linux/en] line 74: h2 `Setting Up the Linux Host (Debian 13)`
  - [setup-linux/hu] line 118: h2 `A Linux hoszt beállítása (Debian 13)`
  - [setup-wsl2/en] line 166: h2 `Setting Up WSL2`
  - [setup-wsl2/hu] line 196: h2 `WSL2 beállítása`
  - [docker-install/en] line 230: h2 `Installing Docker`
  - [docker-install/hu] line 271: h2 `Docker telepítése`
  - [portainer-npm/en] line 316: h2 `Portainer and Nginx Proxy Manager`
  - [portainer-npm/hu] line 359: h2 `Portainer és Nginx Proxy Manager`
  - [deploy-workflow/en] line 406: h2 `The Deploy Workflow`
  - [deploy-workflow/hu] line 451: h2 `A deploy workflow`
  - [deploy-script/en] line 500: h2 `Deploy Script`
  - [deploy-script/hu] line 541: h2 `Deploy szkript`
  - [secrets-volumes/en] line 586: h2 `Secrets and Volumes`
  - [secrets-volumes/hu] line 629: h2 `Titkok és volume-ok`
  - [checklists/en] line 676: h2 `Checklists`
  - [checklists/hu] line 710: h2 `Ellenőrzőlisták`

## content/posts/homelab-legacy-bios.md
- title: The Legacy BIOS + GPT Trap | title_hu: A Legacy BIOS + GPT csapda
- description: Why older machines refuse to boot after a successful Linux installation, and how to fix it with Debian's classic installer.
- description_hu: Miért nem bootolnak a régebbi gépek egy sikeres Linux-telepítés után, és hogyan javítható ez a Debian klasszikus telepítőjével.
- min heading level: 2
  - [problem/en] line 30: h2 `The Problem in Plain English`
  - [problem/hu] line 42: h2 `A probléma egyszerűen`
  - [who-is-affected/en] line 58: h2 `Who Is Affected`
  - [who-is-affected/hu] line 69: h2 `Kit érint`
  - [diagnosis/en] line 84: h2 `Diagnosis: Is This Your Problem?`
  - [diagnosis/hu] line 118: h2 `Diagnosztika: ez a te problémád?`
  - [why-grub-repair-wont-fix/en] line 156: h2 `Why GRUB Repair Won't Fix It`
  - [why-grub-repair-wont-fix/hu] line 166: h2 `Miért nem oldja meg a GRUB javítás?`
  - [solution/en] line 180: h2 `The Solution: MBR Partition Table via Debian`
  - [solution/hu] line 201: h2 `A megoldás: MBR partíciós tábla Debiannal`
  - [uefi-note/en] line 226: h2 `What About UEFI Mode?`
  - [uefi-note/hu] line 236: h2 `Mi a helyzet UEFI móddal?`
  - [reference/en] line 250: h2 `Quick Reference`
  - [reference/hu] line 270: h2 `Gyors referencia`

## content/posts/inferrouter-01.md
- title: InferRouter: One Endpoint, Any Provider | title_hu: InferRouter: egy végpont, bármilyen provider
- description: Why managing LLM providers per project breaks down — and how a single self-hosted proxy with strategy-based inference routing solves it.
- description_hu: Miért omlik össze az LLM providerek projektenkénti kezelése — és hogyan oldja meg ezt egyetlen self-hosted proxy stratégia alapú inferencia routinggal.
- min heading level: 2
  - [problem/en] line 54: h2 `What Was Actually Broken`
  - [problem/hu] line 70: h2 `Mi volt valójában elrontva?`
  - [concept/en] line 90: h2 `The Solution Shape`
  - [concept/hu] line 102: h2 `A megoldás formája`
  - [ollama/en] line 118: h2 `What About Ollama?`
  - [ollama/hu] line 132: h2 `Mi a helyzet az Ollamával?`
  - [series/en] line 150: h2 `This Series`
  - [series/hu] line 159: h2 `A sorozatról`
- fig sections: fig-overview

## content/posts/inferrouter-02.md
- title: InferRouter Part 2: Inference Strategies | title_hu: InferRouter 2. rész: Inferencia stratégiák
- description: How InferRouter decides which provider handles each request — three configurable inference strategies, the ProviderOrchestrator execution loop, and the local fallback that is always last.
- description_hu: Hogyan dönti el az InferRouter, melyik provider kezel egy kérést — három konfigurálható inferencia stratégia, a ProviderOrchestrator végrehajtási ciklusa, és a helyi fallback, amely mindig az utolsó.
- min heading level: 2
  - [architecture/en] line 49: h2 `The Abstraction`
  - [architecture/hu] line 79: h2 `Az absztrakció`
  - [strategies/en] line 113: h2 `Three Strategies`
  - [strategies/hu] line 133: h2 `Három stratégia`
  - [execution/en] line 157: h2 `The Execution Loop`
  - [execution/hu] line 172: h2 `A végrehajtási ciklus`
  - [local_fallback/en] line 191: h2 `The Local Fallback`
  - [local_fallback/hu] line 201: h2 `A helyi fallback`
  - [teaser/en] line 215: h2 `Next: Secret Management`
  - [teaser/hu] line 223: h2 `Következő: Titkos kulcsok kezelése`
- fig sections: fig-overview

## content/posts/inferrouter-03.md
- title: InferRouter Part 3: Secret Management | title_hu: InferRouter 3. rész: Titkos kulcsok kezelése
- description: Where API keys live in InferRouter — why not environment variables, why not config files, and what per-request key reading gives you in practice.
- description_hu: Hol élnek az API kulcsok az InferRouterben — miért nem környezeti változókban, miért nem konfigurációs fájlokban, és mit ad a gyakorlatban a kérésenkénti kulcsolvasás.
- min heading level: 2
  - [comparison/en] line 45: h2 `Three Places a Key Could Live`
  - [comparison/hu] line 57: h2 `Három hely, ahol egy kulcs élhetne`
  - [secretreader/en] line 73: h2 `Per-Request Key Reading`
  - [secretreader/hu] line 87: h2 `Kérésenkénti kulcs olvasás`
  - [teaser/en] line 105: h2 `Next: Observability`
  - [teaser/hu] line 113: h2 `Következő: Megfigyelhetőség`
- fig sections: fig-overview

## content/posts/inferrouter-04.md
- title: InferRouter Part 4: Observability | title_hu: InferRouter 4. rész: Megfigyelhetőség
- description: How InferRouter makes its internal state visible — a provider-agnostic JSONL operation log and three HTTP endpoints for live health, quota stats, and log history.
- description_hu: Hogyan teszi láthatóvá az InferRouter a belső állapotát — provider-független JSONL műveleti napló és három HTTP végpont az élő állapothoz, a kvótastatisztikákhoz és a naplótörténethez.
- min heading level: 2
  - [log/en] line 45: h2 `The Operation Log`
  - [log/hu] line 72: h2 `A műveleti napló`
  - [endpoints/en] line 103: h2 `Live State Endpoints`
  - [endpoints/hu] line 117: h2 `Élő állapot végpontok`
  - [status/en] line 135: h2 `Where It Stands`
  - [status/hu] line 143: h2 `Aktuális állapot`
- fig sections: fig-overview

## content/posts/post_dslForSiteGenerator.md
- title: Designing a minimal DSL for bilingual Markdown content | title_hu: Minimális DSL tervezése kétnyelvű Markdown tartalomhoz
- description: One file. Two languages. No duplication. Here's the syntax that makes it work.
- description_hu: Egy fájl. Két nyelv. Duplikáció nélkül. Ez a szintaxis, ami működővé teszi.
- min heading level: -

## content/posts/post_generatorPipeline.md
- title: The generator pipeline — from Markdown to GitHub Pages | title_hu: A generátor pipeline — Markdowntól a GitHub Pages-ig
- description: No CI/CD. No build server. A local command, a docs/ folder, and a git push. Here's the full picture.
- description_hu: Nincs CI/CD. Nincs build szerver. Egy lokális parancs, egy docs/ mappa, és egy git push. Ez az egész.
- min heading level: -

## content/posts/post_licensAndCerts.md
- title: Licenses & certifications | title_hu: Képzések és tanúsítványok
- description: My licenses and certifications
- description_hu: Képzéseim és tanúsítványaim.
- min heading level: -

## content/posts/post_scaffoldProtocol.md
- title: The AI Executes. You Decide. | title_hu: AI végrehajt. Te döntesz.
- description: How Scaffold Protocol keeps the human as orchestrator while using AI as a transformation engine — with a real validation layer and error-driven refinement.
- description_hu: Hogyan tartja a Scaffold Protocol az embert orchestratorként, miközben az AI transzformációs motorként dolgozik — valódi validációs réteggel és error-driven refinement-tel.
- min heading level: 2
  - [principle/en] line 74: h2 `The Human Is the Orchestrator`
  - [principle/hu] line 88: h2 `A Human az Orchestrator`
  - [step_anatomy/en] line 106: h2 `How a Step Works`
  - [step_anatomy/hu] line 143: h2 `Hogyan Épül Fel Egy Step?`
  - [validation/en] line 184: h2 `The Validation Layer`
  - [validation/hu] line 216: h2 `A Validációs Réteg`
  - [refinement/en] line 252: h2 `Error-Driven Refinement`
  - [refinement/hu] line 278: h2 `Error-Driven Refinement`
  - [real_data/en] line 308: h2 `Five Runs. Real Numbers.`
  - [real_data/hu] line 332: h2 `Öt Futás. Valós Számok.`
  - [workflow_overview/en] line 360: h2 `The Bigger Picture`
  - [workflow_overview/hu] line 382: h2 `A Teljes Kép`
  - [closing/en] line 408: h2 `Where It Stands`
  - [closing/hu] line 426: h2 `Ahol Most Tart`
- fig sections: fig-overview

## content/posts/post_siteGenerator.md
- title: Why I built my own static site generator — not because the others are bad | title_hu: Miért írtam saját statikus oldalgenerátort? (Nem azért, mert a többi rossz.)
- description: Why I wrote my own static site generator for this blog: I never compared generators — the first question I asked was what a blog actually is.
- description_hu: Miért írtam saját statikus oldalgenerátort ehhez a bloghoz: nem hasonlítottam össze generátorokat — az első kérdésem az volt, hogy mi is valójában egy blog.
- min heading level: 2
  - [title/en] line 15: h2 `I never had a problem with Jekyll. I never tried it. That was intentional.`
  - [title/hu] line 19: h2 `Nem volt bajom a Jekyll-lel. Soha nem is próbáltam. Szándékos döntés volt.`

## content/posts/raglab-01.md
- title: Building a RAG Pipeline from Scratch | title_hu: RAG építése az alapoktól
- description: What I learned by building a RAG pipeline in .NET without a framework — every component written from scratch, every decision made consciously.
- description_hu: Amit egy RAG pipeline keretrendszer nélküli, .NET-ben történő felépítéséből tanultam — minden komponens az alapoktól megírva, minden döntés tudatosan meghozva.
- min heading level: 2
  - [intro/en] line 16: h2 `Problem statement`
  - [intro/hu] line 24: h2 `Probléma megfogalmazás`
  - [what-is-rag/en] line 49: h2 `What is RAG, and why does it matter?`
  - [what-is-rag/hu] line 61: h2 `Mi az a RAG, és miért fontos?`
  - [the-pipeline/en] line 77: h2 `The Two Flows`
  - [the-pipeline/hu] line 97: h2 `A két folyamat`
  - [component-loader/en] line 147: h2 `The Loader`
  - [component-loader/hu] line 167: h2 `A Loader`
  - [component-chunker/en] line 191: h2 `The Chunker`
  - [component-chunker/hu] line 205: h2 `A Chunker`
  - [component-embedder/en] line 223: h2 `The Embedder`
  - [component-embedder/hu] line 237: h2 `Az Embedder`
  - [component-vectorstore/en] line 255: h2 `The Vector Store`
  - [component-vectorstore/hu] line 283: h2 `A Vector Store`
  - [component-generator/en] line 315: h2 `The Generator`
  - [component-generator/hu] line 338: h2 `A Generator`
  - [architectural-decisions/en] line 365: h2 `Architectural Decisions`
  - [architectural-decisions/hu] line 394: h2 `Architektúrális döntések`
  - [what-i-learned/en] line 427: h2 `What I Actually Learned`
  - [what-i-learned/hu] line 439: h2 `Amit tényleg megtanultam`
  - [whats-next/en] line 455: h2 `What Comes Next`
  - [whats-next/hu] line 469: h2 `Mi jön ezután`
- fig sections: fig-overview, fig-two-flows, fig-components

## content/posts/sourcerag-00.md
- title: Making a 2M-line legacy codebase queryable by AI | title_hu: Legacy kódbázis, amit az AI is ismer
- description: I designed a system to make a 2M+ line on-premise legacy codebase queryable by any AI tool. It was presented, never implemented. Here is the design — and the question it left open.
- description_hu: Terveztem egy rendszert, amellyel egy 2M+ soros, on-premise legacy kódbázis bármely AI eszközzel lekérdezhetővé válik. Bemutatták, de soha nem valósult meg. Íme a terv — és a kérdés, amelyet nyitva hagyott.
- min heading level: 2
  - [problem/en] line 50: h2 `The problem`
  - [problem/hu] line 60: h2 `A probléma`
  - [design/en] line 74: h2 `The design`
  - [design/hu] line 86: h2 `A terv`
  - [rationale/en] line 102: h2 `Component rationale`
  - [rationale/hu] line 116: h2 `Komponens indoklások`
  - [cost/en] line 134: h2 `What this requires`
  - [cost/hu] line 148: h2 `Amit ez megkövetel`
  - [closing/en] line 166: h2 `One question left open`
  - [closing/hu] line 180: h2 `Egy nyitott kérdés`
- fig sections: fig-overview

## content/posts/sourcerag-01.md
- title: VCS as Proof Store | title_hu: VCS mint proof tár
- description: Why duplicate source content into a proof database when source control already tracks everything? SourceRAG uses the VCS itself as its proof store — no content duplication, full traceability.
- description_hu: Miért másoljuk a forrástartalmat egy proof adatbázisba, ha a verziókezelő már mindent követ? A SourceRAG magát a VCS-t használja proof tárként — tartalomduplikáció nélkül, teljes nyomon követhetőséggel.
- min heading level: 2
  - [problem/en] line 53: h2 `The duplication cost`
  - [problem/hu] line 68: h2 `A duplikáció ára`
  - [principle/en] line 87: h2 `The core decision: VCS is the proof store`
  - [principle/hu] line 97: h2 `Az alapdöntés: a VCS a proof tár`
  - [architecture/en] line 111: h2 `Solution structure`
  - [architecture/hu] line 147: h2 `Solutionstruktúra`
  - [vcs_providers/en] line 187: h2 `VCS providers: Git and SVN as vertical slices`
  - [vcs_providers/hu] line 199: h2 `VCS providerek: Git és SVN mint vertikális szeletek`
  - [chunking/en] line 215: h2 `Chunking: Roslyn first, plain text fallback`
  - [chunking/hu] line 227: h2 `Chunkolás: Roslyn elsőként, plain text fallback`
  - [embedding/en] line 243: h2 `Embedding: local or API, no code change required`
  - [embedding/hu] line 253: h2 `Embedding: lokális vagy API, kódmódosítás nélkül`
  - [proof_payload/en] line 267: h2 `The chunk proof payload`
  - [proof_payload/hu] line 294: h2 `A chunk proof payload`
  - [hosting/en] line 325: h2 `Dual hosting: REST and MCP over a shared Application layer`
  - [hosting/hu] line 347: h2 `Dual hosting: REST és MCP egy közös Application réteg felett`
  - [observability/en] line 373: h2 `Observability across the pipeline`
  - [observability/hu] line 383: h2 `Megfigyelhetőség a pipeline-on át`
  - [repo_link/en] line 397: h2 `Source`
  - [repo_link/hu] line 403: h2 `Forrás`
  - [status/en] line 413: h2 `Where this stands`
  - [status/hu] line 425: h2 `Ahol most tart`
- fig sections: fig-overview
