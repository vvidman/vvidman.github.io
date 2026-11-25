---
title: From Tier Map to Agent Team in GitHub Copilot
title_hu: Szinttérképből agent-csapat GitHub Copilotban
slug: posts/copilot-tier-agent-team.html
description: How to turn a model tier map into a model-independent agent team in GitHub Copilot: roles bound to tiers, tiers bound to models in one register, and where Copilot Fusion fits next to it.
description_hu: Hogyan lesz egy modell-szinttérképből modellfüggetlen agent-csapat GitHub Copilotban: a szerepek szintekhez, a szintek egyetlen regiszterben modellekhez kötve, és hol a helye mellette a Copilot Fusionnek.
layout: page
date: 2026-11-25 20:00
topic: ai-workflow
image: /assets/images/infographics/copilot-tier-agent-team-square.png
og_image: /assets/images/infographics/copilot-tier-agent-team-og.png
---

:::section intro

:::lang en
## The step a tier map leaves out

A [tier map](/posts/governing-ai-assistants-01-model-tiers.html) answers one question: which models are allowed, and for what kind of work. It does not say how the tiers work together on a single task.

In practice the developer fills that gap by hand. Plan with the frontier model, switch to the workhorse for the implementation, switch again for the repetitive part, and remember to switch back for the review. Most people stop switching after the second day.

GitHub Copilot now offers one answer to this: Fusion, a router that picks a model for every prompt. This post describes a second answer, an explicit team of agents where each role runs on a fixed tier, and explains why the two belong next to each other rather than in competition.

A note on status: what follows is a designed template, not a measured result. The last section lists what a pilot has to prove before anyone should trust it.
:::lang

:::lang hu
## A lépés, amit a szinttérkép kihagy

Egy [szinttérkép](/posts/governing-ai-assistants-01-model-tiers.html) egy kérdésre válaszol: mely modellek engedélyezettek, és milyen munkára. Azt nem mondja meg, hogyan dolgoznak együtt a szintek egyetlen feladaton.

A gyakorlatban ezt a rést a fejlesztő tölti ki kézzel. Tervezés a frontier modellel, váltás az igáslóra az implementációhoz, újabb váltás az ismétlődő részhez, és nem szabad elfelejteni visszaváltani a review-hoz. A legtöbben a második napon abbahagyják a váltogatást.

A GitHub Copilot erre ma egy választ kínál: a Fusiont, egy routert, amely promptonként választ modellt. Ez a poszt egy második választ ír le: egy explicit agent-csapatot, amelyben minden szerep rögzített szinten fut. És azt is, miért egymás mellett van a helyük, nem egymással szemben.

Egy megjegyzés a státuszról: ami következik, az egy megtervezett template, nem mért eredmény. Az utolsó szakasz felsorolja, mit kell egy pilotnak bizonyítania, mielőtt bárki megbízna benne.
:::lang

:::section

:::section origin

:::lang en
## Where the idea came from

The starting point was an [article written for Claude Code](https://medium.com/@altairaisolutions/fable-5-2-prompts-for-most-dangerous-llm-in-history-that-will-f-cking-change-you-full-c1b40a15c285). Its tone is loud, but the technical core holds up: the most expensive model pays off when it does not do everything, only thinks, and directs a team of cheaper models.

The article splits the work into four roles. A **brain** plans, splits the work, writes the briefs and judges the results. A **builder** implements from a clear brief. A **checker** verifies independently and changes nothing. A **worker** does repetitive, easily verified work and writes no code. Around them sit a few written delegation rules and an **advisor** that gives a second opinion at named moments.

Every mechanism in it is specific to one tool and tied to four named models of one vendor. Removing that binding was the actual work.
:::lang

:::lang hu
## Honnan jött az ötlet

A kiindulópont egy [Claude Code-ra írt cikk](https://medium.com/@altairaisolutions/fable-5-2-prompts-for-most-dangerous-llm-in-history-that-will-f-cking-change-you-full-c1b40a15c285) volt. A hangneme harsány, de a technikai magja megállja a helyét: a legdrágább modell akkor éri meg, ha nem mindent az végez, hanem csak gondolkodik, és olcsóbb modellekből álló csapatot irányít.

A cikk négy szerepre bontja a munkát. A **brain** tervez, feloszt, brifel és értékel. A **builder** egyértelmű brief alapján implementál. A **checker** függetlenül ellenőriz, és semmit nem módosít. A **worker** ismétlődő, jól ellenőrizhető munkát végez, és nem ír kódot. Mellettük néhány leírt delegálási szabály áll, és egy **advisor**, amely megnevezett pontokon ad második véleményt.

Minden mechanizmusa egy eszközhöz és egy gyártó négy konkrét modelljéhez kötött. Ennek a kötésnek a levétele volt a valódi munka.
:::lang

:::section

:::section principle

:::lang en
## Roles bind to tiers, tiers bind to models

The principle behind the template fits in one sentence: **split the work into roles, bind each role to a tier, and bind the tiers to models in exactly one place.** Then replacing a model is a register update, not a rewrite of every instruction.

Six rules follow from it:

1. **Role, not model.** An agent definition says which tier a role runs on, "workhorse", never which model. Model names live in the central register.
2. **One register, one truth.** One default and at most two alternatives per tier, the same register that governs which models are allowed at all. The team cannot use a model the organisation has not approved.
3. **Direction belongs to the top tier.** Planning, splitting, root causes and final decisions belong to the orchestrator. It does not do repetitive work.
4. **Independent verification.** The checker is not the one who wrote the work, and it does not change anything. Work is done only after the check.
5. **A second opinion at named moments.** The advisor is asked before choosing an approach, on a recurring failure, and before closing.
6. **The human approves, the AI executes.** The plan starts only after human approval. Risky steps, such as deleting, pushing, changing CI or touching anything outside the repository, need permission by name. Safe steps don't.
:::lang

:::lang hu
## A szerep szinthez kötött, a szint modellhez

A template mögötti elv egy mondatba belefér: **a munkát szerepekre bontjuk, a szerepeket szintekhez kötjük, a szinteket pedig egyetlen helyen modellekhez.** Így a modellcsere egy regiszterfrissítés, nem az összes instrukció átírása.

Ebből hat szabály következik:

1. **Szerep, nem modell.** Az agent-definíció azt mondja ki, hogy egy szerep melyik szinten fut („igásló”), soha nem azt, hogy melyik modellen. A modellnevek a központi regiszterben vannak.
2. **Egy regiszter, egy igazság.** Szintenként egy alapértelmezett és legfeljebb két alternatíva, ugyanabban a regiszterben, amely azt is szabályozza, mely modellek engedélyezettek egyáltalán. A csapat nem használhat olyan modellt, amelyet a szervezet nem hagyott jóvá.
3. **Az irányítás a legfelső szinté.** Tervezés, felosztás, gyökérok és végső döntés az orchestratoré. Ismétlődő munkát nem végez.
4. **Független ellenőrzés.** Az ellenőr nem az, aki a munkát írta, és nem módosít semmit. A munka csak az ellenőrzés után kész.
5. **Második vélemény megnevezett pontokon.** Az advisort a megközelítés kiválasztása előtt, visszatérő hibánál és a lezárás előtt kell megkérdezni.
6. **Az ember jóváhagy, az AI végrehajt.** A terv csak emberi jóváhagyás után indul. A kockázatos lépésekhez, mint a törlés, a push, a CI módosítása vagy bármi a repozitóriumon kívül, név szerinti engedély kell. A biztonságos lépésekhez nem.
:::lang

:::section

:::section left-out

:::lang en
## What I deliberately left out

The article has an instruction block tuned to keep the model going: don't stop halfway, don't offer to wait, finish the task. For an autonomous demo that makes sense. In an environment where auditability and human sign-off matter, it works directly against the approval points in rule 6, so it stayed out.

The shape of its approval rules came through, though. The exception is written into the rule itself ("ask before you do X"), not into a separate "never" line. Newer models follow instructions more literally, and an absolute "never" without its exception is exactly the kind of line that produces surprising behaviour after a model upgrade.
:::lang

:::lang hu
## Amit szándékosan kihagytam

A cikkben van egy instrukciós blokk, amely arra van hangolva, hogy a modell ne álljon meg: ne hagyja félbe, ne ajánlja fel, hogy vár, vigye végig a feladatot. Egy autonóm demóban ennek van értelme. Olyan környezetben viszont, ahol az auditálhatóság és az emberi jóváhagyás számít, egyenesen a 6. szabály jóváhagyási pontjai ellen dolgozik, ezért kimaradt.

A jóváhagyási szabályok formája viszont átjött. A kivétel magában a szabályban áll („kérdezz, mielőtt X-et csinálod”), nem egy külön „soha” sorban. Az újabb modellek szó szerintebben követik az instrukciókat, és egy kivétel nélküli, abszolút „soha” pontosan az a fajta sor, amely egy modellváltás után meglepő viselkedést okoz.
:::lang

:::section

:::section team

:::lang en
## The team

The developer talks to one agent only: the orchestrator. It briefs the other four, reads their reports and decides what happens next. Which model each of them runs on comes from the register.

| Role | Tier | Job | What it cannot do |
|---|---|---|---|
| Orchestrator | Frontier | plans, delegates, decides, judges | — |
| Builder | Workhorse | implements from a brief | start other agents |
| Checker | Workhorse | tests, reads the diff, reports | edit files |
| Worker | Fast iterative | repetitive, verifiable work | write code |
| Advisor | Frontier | second opinion | edit files or run commands |

The last column matters more than it looks. Copilot custom agents take a tool list, so a role is enforced by what the agent is given, not by what its prompt asks for. A checker without edit tools cannot "just fix" the problem it found.

The delegation rules are short:

- **Delegate only independent, high-volume work.** Every subagent starts from an empty context and pays for everything it reads. Small tasks stay with the orchestrator.
- **No two agents edit the same file at the same time.**
- **A part that failed twice goes back to the orchestrator,** instead of a third attempt.

The team runs in two phases. First the orchestrator only plans, and the plan goes to the human. After approval it executes that plan. If execution would change the scope, it stops and reports instead of quietly rewriting the plan.
:::lang

:::lang hu
## A csapat

A fejlesztő egyetlen agenttel beszél: az orchestratorral. Ő brifeli a másik négyet, olvassa a jelentéseiket, és dönt a következő lépésről. Hogy melyikük melyik modellen fut, azt a regiszter határozza meg.

| Szerep | Szint | Feladat | Amit nem tehet |
|---|---|---|---|
| Orchestrator | Frontier | tervez, delegál, dönt, értékel | — |
| Builder | Igásló | brief alapján implementál | más agentet indítani |
| Checker | Igásló | tesztel, diffet olvas, jelent | fájlt módosítani |
| Worker | Gyors-iteratív | ismétlődő, ellenőrizhető munka | kódot írni |
| Advisor | Frontier | második vélemény | fájlt módosítani vagy parancsot futtatni |

Az utolsó oszlop fontosabb, mint amilyennek látszik. A Copilot custom agentek eszközlistát kapnak, így a szerepet az határozza meg, amit az agent megkap, nem az, amit a promptja kér. Egy szerkesztési eszköz nélküli checker nem tudja „csak gyorsan megjavítani” a hibát, amit talált.

A delegálási szabályok rövidek:

- **Csak független, nagy volumenű munkát adj ki.** Minden subagent üres kontextusból indul, és fizetni kell mindenért, amit elolvas. A kis feladat az orchestratornál marad.
- **Két agent nem szerkesztheti egyszerre ugyanazt a fájlt.**
- **A kétszer elbukott rész visszakerül az orchestratorhoz,** harmadik próbálkozás helyett.

A csapat két fázisban dolgozik. Először az orchestrator csak tervez, és a terv az emberhez kerül. A jóváhagyás után azt a tervet hajtja végre. Ha a végrehajtás megváltoztatná a hatókört, megáll és jelent, nem írja át csendben a tervet.
:::lang

:::section

:::section fusion

:::lang en
## Fusion and the team

[Copilot Fusion](https://docs.github.com/en/copilot/concepts/models/copilot-fusion) became generally available on [28 October 2026](https://github.blog/changelog/2026-10-28-route-and-orchestrate-models-with-github-copilot-fusion). It is not a model but a runtime layer: the user picks an effort level (Standard, High or Max), and Fusion chooses a model and an execution pattern for every prompt. The patterns are single (one model does the task), cascade (an efficient model drafts, a judge accepts or escalates) and critique (one model drafts, others review, the drafter revises). It only chooses from models the organisation allows.

That makes Fusion and the team two answers to different questions. Fusion decides at runtime which model works. The team decides in advance which role runs on which tier.

| Aspect | Copilot Fusion | Tier-based agent team |
|---|---|---|
| Who picks the model | Fusion, per prompt | the register, per role, in advance |
| Reproducibility | low: pattern and model can change between runs | high: same role, same model |
| What an audit can show | which models were used | which role, which model, which register version |
| How it starts | from the model picker, can be the default | manually, through one agent |
| Subagents | does not start them | built on them |
| Good for | everyday, single-session work | large, multi-step, verified tasks |

The division of labour follows from the table: Fusion for everyday work, where it can be the default; the orchestrator and its team for large, multi-step tasks; and a fixed-model agent for the specialised tier, but only where a real use case exists.

The orchestrator runs on a fixed model, not on Fusion, for two reasons. The documentation says Fusion does not start subagents, and it does not say what happens when a Fusion session calls a fixed-model agent. And for the role that makes the plan, being able to reproduce a run is worth more than the routing's cost advantage. A Fusion-based orchestrator remains an option for a pilot, not the default.
:::lang

:::lang hu
## A Fusion és a csapat

A [Copilot Fusion](https://docs.github.com/en/copilot/concepts/models/copilot-fusion) [2026. október 28-án](https://github.blog/changelog/2026-10-28-route-and-orchestrate-models-with-github-copilot-fusion) lett általánosan elérhető. Nem modell, hanem futtatási réteg: a felhasználó effort szintet választ (Standard, High vagy Max), a Fusion pedig promptonként választ modellt és végrehajtási mintát. A minták: single (egy modell végzi a feladatot), cascade (egy hatékony modell készít vázlatot, egy bíráló elfogadja vagy erősebb modellhez továbbítja) és critique (egy modell vázlatot ír, mások bírálják, a szerző javít). Csak a szervezet által engedélyezett modellek közül választ.

A Fusion és a csapat így két különböző kérdésre ad választ. A Fusion futás közben dönti el, melyik modell dolgozzon. A csapat előre rögzíti, melyik szerep melyik szinten fut.

| Szempont | Copilot Fusion | Szint alapú agent-csapat |
|---|---|---|
| Ki választ modellt | a Fusion, promptonként | a regiszter, szerepenként, előre |
| Reprodukálhatóság | alacsony: a minta és a modell futásonként változhat | magas: azonos szerep, azonos modell |
| Mit mutat ki egy audit | mely modelleket használta | melyik szerep, melyik modell, melyik regiszterverzió |
| Indítás | a modellválasztóból, lehet alapértelmezett | kézzel, egy agenten keresztül |
| Subagentek | nem indít subagentet | rájuk épül |
| Mire jó | mindennapi, együléses munka | nagy, többlépéses, ellenőrzött feladat |

A munkamegosztás a táblázatból következik: a mindennapi munkára a Fusion, ahol alapértelmezett is lehet; nagy, többlépéses feladatra az orchestrator és a csapata; a specializált szintre pedig rögzített modellű agent, de csak ott, ahol van valódi használati eset.

Az orchestrator két okból fut rögzített modellen, és nem Fusionön. A dokumentáció szerint a Fusion nem indít subagentet, és arról sem ír, mi történik, ha egy Fusion-session rögzített modellű agentet hív. Emellett annál a szerepnél, amely a tervet készíti, többet ér a futás reprodukálhatósága, mint a routing költségelőnye. A Fusion-alapú orchestrator pilot-opció marad, nem alapértelmezés.
:::lang

:::section

:::section generator

:::lang en
## Keeping it model-independent

Copilot has no tier alias: an agent file names a concrete model. Left alone, that would bring the vendor binding straight back, one hand-edited file at a time.

A small generator keeps the binding in one place. The register lists the models per tier. The agent templates reference tiers only. The generator renders the finished agent files for each surface, in the format that surface expects. Where a surface accepts a priority list, the default comes first and the alternatives follow as fallbacks, which maps the "one default, at most two alternatives" rule directly.

The generator refuses to write anything if the register breaks its own rules: a template points to a tier that does not exist, a tier has more than three models, or a placeholder is left unresolved. Every generated file is marked as generated, with the template and the register version it came from, and a manifest records which tier and model each agent got.

That manifest is the machine-readable form of a rule from [Part 3](/posts/governing-ai-assistants-03-model-register.html) of the governance series: never change a default silently. When the register changes, the manifest diff shows exactly which agent now runs on a different model.
:::lang

:::lang hu
## Hogyan marad modellfüggetlen

A Copilotban nincs szint-alias: egy agent-fájl konkrét modellt nevez meg. Ha ezt magára hagyjuk, a gyártóhoz kötés azonnal visszajön, egyenként kézzel szerkesztett fájlok formájában.

Egy kis generátor egy helyen tartja a kötést. A regiszter szintenként sorolja fel a modelleket. Az agent-sablonok csak szintekre hivatkoznak. A generátor felületenként előállítja a kész agent-fájlokat, abban a formában, amit az adott felület vár. Ahol a felület prioritási listát fogad, ott elöl az alapértelmezett áll, utána tartalékként az alternatívák, ami egy az egyben leképezi az „egy alapértelmezett, legfeljebb két alternatíva” szabályt.

A generátor semmit nem ír ki, ha a regiszter megsérti a saját szabályait: egy sablon nem létező szintre mutat, egy szinten háromnál több modell van, vagy feloldatlan placeholder maradt. Minden generált fájl generáltként van megjelölve, azzal a sablonnal és regiszterverzióval együtt, amelyből készült, egy manifest pedig rögzíti, melyik agent melyik szintet és modellt kapta.

Ez a manifest a governance sorozat [3. részében](/posts/governing-ai-assistants-03-model-register.html) leírt szabály gépi formája: alapértelmezett modellt soha ne cserélj csendben. Amikor a regiszter változik, a manifest diffje pontosan megmutatja, melyik agent fut ezentúl más modellen.
:::lang

:::section

:::section risks

:::lang en
## What can still go wrong

The biggest risk is not technical but about control: a system of several agents steps over human decision points more easily, and costs more than it looks.

- **Cost overrun.** Every subagent starts empty, rereads files and is billed at its own model's rate. The delegation rules exist for this, and a pilot has to measure whether they hold.
- **Nested agents.** When the team is started through a skill, the orchestrator is itself a subagent that starts further subagents. Whether each surface allows that is not documented everywhere, and some block it by default. It has to be tested per surface.
- **Correlated blind spots.** If the builder and the checker come from the same model family, they tend to miss the same mistakes. Giving the checker a model from another vendor is a cheap register change.
- **Prompt injection.** Agents read repository files and issues, and an instruction hidden in them reaches an agent that can run commands. Narrow tool lists and approval rules reduce this. Running the team on repositories from untrusted sources should simply not happen.
- **A moving platform.** Several agent configuration fields are still in preview, and the documentation changes weekly. Keeping every field in the templates means fixing it in one place.

None of this replaces human code review on a pull request. The team produces a change; a person still signs it off.
:::lang

:::lang hu
## Mi romolhat még el

A legnagyobb kockázat nem technikai, hanem az irányításé: egy több agentből álló rendszer könnyebben lép át emberi döntési pontokat, és drágább, mint amilyennek látszik.

- **Költség-túlfutás.** Minden subagent üresen indul, újraolvassa a fájlokat, és a saját modellje díján számlázódik. A delegálási szabályok erre valók, és egy pilotnak kell megmérnie, hogy tartják-e magukat.
- **Beágyazott agentek.** Ha a csapatot egy skill indítja, maga az orchestrator is subagent, amely további subagenteket indít. Hogy ezt az egyes felületek megengedik-e, nincs mindenhol dokumentálva, és van, ahol alapból tiltott. Felületenként ki kell próbálni.
- **Korrelált vakfoltok.** Ha a builder és a checker ugyanabból a modellcsaládból jön, hajlamosak ugyanazokat a hibákat elnézni. Ha a checker másik gyártó modelljét kapja, az egy olcsó regisztermódosítás.
- **Prompt injection.** Az agentek repozitóriumi fájlokat és issue-kat olvasnak, és egy bennük elrejtett utasítás eljuthat egy parancsfuttatásra képes agenthez. A szűk eszközlisták és a jóváhagyási szabályok ezt csökkentik. Megbízhatatlan forrású repozitóriumon a csapat egyszerűen ne fusson.
- **Mozgó platform.** Az agent-konfiguráció több mezője még előzetes (preview), a dokumentáció pedig heti ütemben változik. Ha minden mező a sablonokban van, egy helyen kell javítani.

Mindez nem váltja ki az emberi code review-t a pull requesten. A csapat elkészít egy változást, de jóváhagyni továbbra is egy embernek kell.
:::lang

:::section

:::section pilot

:::lang en
## What a pilot has to show

Today the choice between Fusion and the team rests on impressions. A pilot can replace them with numbers. Four scenarios cover the interesting cases:

1. **A multi-file feature,** where splitting the work should pay off.
2. **Bulk repetitive work,** where the fast iterative tier should carry most of the load.
3. **A recurring bug,** where the advisor and the "twice failed" rule are tested.
4. **A small control task,** where the right behaviour is no delegation at all.

Each scenario runs once with Fusion and once with the team. Record the credits used, the elapsed time, the number of human interventions, the defects the checker caught, and whether the result was accepted.

The same scenarios do a second job. Written down as repeatable cases, they are the first entries of the evaluation library from [Part 2](/posts/governing-ai-assistants-02-choosing-models.html): tier decisions measured on your own codebase instead of on public benchmarks.
:::lang

:::lang hu
## Mit kell egy pilotnak megmutatnia

Ma a Fusion és a csapat közötti választás benyomásokon alapul. Egy pilot számokkal válthatja ki ezeket. Négy forgatókönyv lefedi az érdekes eseteket:

1. **Többfájlos feature,** ahol a munka felosztásának meg kell térülnie.
2. **Tömeges ismétlődő munka,** ahol a terhelés nagyját a gyors-iteratív szintnek kell vinnie.
3. **Visszatérő hiba,** ahol az advisor és a „kétszer elbukott” szabály próbára kerül.
4. **Kis kontrollfeladat,** ahol a helyes viselkedés az, hogy egyáltalán nincs delegálás.

Minden forgatókönyv egyszer Fusionnel, egyszer a csapattal fut. Rögzíteni kell a felhasznált krediteket, az eltelt időt, az emberi beavatkozások számát, a checker által talált hibákat, és azt, hogy elfogadták-e az eredményt.

Ugyanezek a forgatókönyvek egy második feladatot is ellátnak. Megismételhető esetként leírva ők a governance sorozat [2. részében](/posts/governing-ai-assistants-02-choosing-models.html) leírt eval-könyvtár első elemei: szintdöntések, amelyeket a saját kódbázisodon mérsz, nem publikus benchmarkokon.
:::lang

:::section

:::section closing

:::lang en
## The stable layer is the role

Models change monthly. Tools add and rename fields weekly. What stays stable is the description of the work: who plans, who builds, who checks, and where a human has to say yes.

Put that description in roles, bind the roles to tiers, and keep the models in one register. Then a new model is a reviewed change to one file, and the team keeps working the same way it did yesterday. That is "the human orchestrates, the AI executes", written down as configuration.
:::lang

:::lang hu
## A stabil réteg a szerep

A modellek havonta változnak. Az eszközök hetente kapnak új mezőket vagy neveznek át régieket. Ami stabil marad, az a munka leírása: ki tervez, ki épít, ki ellenőriz, és hol kell egy embernek igent mondania.

Ezt a leírást tedd szerepekbe, a szerepeket kösd szintekhez, a modelleket pedig tartsd egyetlen regiszterben. Akkor egy új modell egyetlen fájl átnézett módosítása, és a csapat ugyanúgy dolgozik tovább, mint tegnap. Ez az „az ember orchestrál, az AI végrehajt”, konfigurációként leírva.
:::lang

:::section
