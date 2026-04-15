---
title: VCS as Proof Store
title_hu: VCS mint proof tár
slug: posts/sourcerag-01.html
description: Why duplicate source content into a proof database when source control already tracks everything? SourceRAG uses the VCS itself as its proof store — no content duplication, full traceability.
layout: page
series: SourceRAG
part: 2
---

:::section intro

:::lang en
# VCS as Proof Store

Every RAG system I've built or studied follows the same pattern. You have a source repository. You extract the content. You store it in a proof database. You embed it. You search it. Two copies of everything, an ongoing sync obligation, and a proof store you have to maintain yourself.

I kept coming back to the same question: the overhead is real, but is it necessary?

Git already knows every file, every method, every author, every commit message — with exact timestamps and full revision history. If I'm already on source control, I'm already maintaining a proof store. I just haven't been querying it directly.

SourceRAG is my attempt to test this hypothesis. This is a concept project — README and architecture decisions are in place, implementation is underway. I'm writing about it as I build.
:::lang

:::lang hu
# VCS mint proof tár

Minden RAG rendszer, amit eddig építettem vagy tanulmányoztam, ugyanazt a mintát követi. Van egy source repository. Kinyerjük a tartalmat. Betároljuk egy proof adatbázisba. Embedeljük. Keresünk benne. Minden adat két helyen él, folyamatos szinkronizációs kötelezettséggel, és egy proof tárral, amit magunknak kell karbantartani.

Mindig visszatértem ugyanahhoz a kérdéshez: az overhead valós, de valóban szükséges?

A git már tudja az összes fájlt, minden metódust, minden szerzőt, minden commit üzenetet — pontos időbélyeggel és teljes revíziótörténettel. Ha már source control alatt van a kódom, már fenntartok egy proof tárat. Csak nem kérdeztem le közvetlenül.

A SourceRAG ennek a hipotézisnek a tesztelése. Ez egy koncepcióprojekt — a README és az architektúrális döntések megvannak, az implementáció folyamatban van. Menet közben írok róla.
:::lang

:::section

:::section problem

:::lang en
## The duplication cost

Traditional RAG over a codebase looks roughly like this:

1. A source file is committed to version control
2. A background process extracts its content and stores it in a relational proof database
3. Chunks are embedded and written to a vector store
4. At query time, results come back as text chunks — detached from their origin

The proof database is what makes answers auditable. You can trace a result back to a row, and that row tells you what file it came from and when it was indexed. The problem is that the database is a snapshot — it drifts from the repository the moment someone pushes a commit. You need incremental indexing, re-sync logic, and schema migrations to keep it current.

And the content itself still lives in the repository. The database is a derivative. You're maintaining a derived copy of something that already exists and is already versioned.
:::lang

:::lang hu
## A duplikáció ára

A kódbázis feletti hagyományos RAG nagyjából így néz ki:

1. Egy forrást commitolnak a verziókezelőbe
2. Egy háttérfolyamat kinyeri a tartalmát és betárolja egy relációs proof adatbázisba
3. A chunkokat embedeljük és a vektortárba írjuk
4. Lekérdezéskor szövegcsomagok jönnek vissza — leválasztva az eredetüktől

A proof adatbázis teszi az eredményeket auditálhatóvá. Egy találatot vissza lehet vezetni egy sorra, és az a sor megmondja, melyik fájlból származik és mikor lett indexelve. A probléma az, hogy az adatbázis egy pillanatkép — azonnal eltér a repositorytól, amint valaki pusholunk egy commitot. Inkrementális indexelés, újraszinkronizálási logika és séma-migrációk szükségesek ahhoz, hogy aktuális maradjon.

A tartalom maga viszont még mindig a repositoryban él. Az adatbázis egy leszármaztatott. Egy olyan dolog leszármaztatott másolatát tartjuk fenn, amely már létezik és már verziózott.
:::lang

:::section

:::section principle

:::lang en
## The core decision: VCS is the proof store

**ADR-002** captures this as a hard architectural constraint: no source content is stored outside the repository. The vector store holds only embeddings and metadata. At query time, actual file content is reconstructed on-demand from the VCS using a revision reference.

This has a specific consequence: every answer is traceable not just to a file, but to a specific revision of that file, to the author of that revision, and to the commit message that explains why the change was made. The proof isn't a database row — it's a commit in your repository's history.

The overhead moves from "maintain a copy" to "query what you already have." Whether that trade-off holds in practice is exactly what this project is testing.
:::lang

:::lang hu
## Az alapdöntés: a VCS a proof tár

Az **ADR-002** ezt kemény architektúrális korlátként rögzíti: semmiféle forrástartalmat nem tárolunk a repositoryn kívül. A vektortár csak embeddingeket és metaadatokat tárol. Lekérdezéskor a tényleges fájltartalom igény szerint rekonstruálódik a VCS-ből, egy revízió-referencia alapján.

Ennek konkrét következménye van: minden válasz nem csupán egy fájlra, hanem annak egy konkrét revíziójára vezethető vissza — a revízió szerzőjére és arra a commit üzenetre, amely elmagyarázza, miért történt a változás. A proof nem egy adatbázissor — hanem egy commit a repository történetében.

Az overhead "másolat karbantartása"-ról "amit már megvan, azt lekérdezem"-re tolódik. Hogy ez a csere a gyakorlatban is megállja-e a helyét, pontosan ezt teszteli ez a projekt.
:::lang

:::section

:::section architecture

:::lang en
## Solution structure

The solution follows Clean Architecture across six projects, with a hard dependency rule: Domain has no dependencies, Application depends only on Domain, Infrastructure implements Domain interfaces, and the two host projects are isolated from each other.

```
SourceRAG.sln
├── src/
│   ├── SourceRAG.Domain/          # Entities, interfaces, enums
│   ├── SourceRAG.Application/     # Use cases, MediatR handlers
│   ├── SourceRAG.Infrastructure/  # VCS, Embedding, Chunking, Qdrant
│   ├── SourceRAG.Api/             # ASP.NET Core minimal API (REST)
│   ├── SourceRAG.McpHost/         # MCP server host
│   └── SourceRAG.Web/             # Blazor Web UI
└── tests/
    ├── SourceRAG.Domain.Tests/
    ├── SourceRAG.Application.Tests/
    └── SourceRAG.Infrastructure.Tests/
```

The two hosting projects — `SourceRAG.Api` and `SourceRAG.McpHost` — share the same Application layer via MediatR. Neither knows about the other. This means the same use cases are available over REST and over MCP without any duplication of business logic.

All provider choices — which VCS, which embedding model — are runtime-configurable via `appsettings.json`. No code change is needed to switch between Git and SVN, or between a local LLamaSharp model and an API-based embedding provider.

```json
{
  "SourceRAG": {
    "VcsProvider": "Git",
    "EmbeddingProvider": "Local",
    "RepositoryPath": "/path/to/repo",
    "Branch": "main"
  }
}
```
:::lang

:::lang hu
## Solutionstruktúra

A solution Clean Architecture-t követ, hat projekten át, kemény függőségi szabállyal: a Domainnek nincs függősége, az Application csak a Domaintől függ, az Infrastructure implementálja a Domain interfészeket, a két host projekt egymástól elszigetelt.

```
SourceRAG.sln
├── src/
│   ├── SourceRAG.Domain/          # Entitások, interfészek, enumok
│   ├── SourceRAG.Application/     # Use case-ek, MediatR handlerek
│   ├── SourceRAG.Infrastructure/  # VCS, Embedding, Chunking, Qdrant
│   ├── SourceRAG.Api/             # ASP.NET Core minimal API (REST)
│   ├── SourceRAG.McpHost/         # MCP szerver host
│   └── SourceRAG.Web/             # Blazor Web UI
└── tests/
    ├── SourceRAG.Domain.Tests/
    ├── SourceRAG.Application.Tests/
    └── SourceRAG.Infrastructure.Tests/
```

A két host projekt — `SourceRAG.Api` és `SourceRAG.McpHost` — ugyanazt az Application réteget osztja MediatR-on keresztül. Egyik sem tud a másikról. Ez azt jelenti, hogy ugyanazok a use case-ek elérhetők REST-en és MCP-n is, üzleti logika duplikáció nélkül.

Minden provider-választás — melyik VCS, melyik embedding modell — runtime-on konfigurálható `appsettings.json`-ban. Git és SVN, vagy lokális LLamaSharp modell és API-alapú embedding provider között váltáshoz nincs szükség kódmódosításra.

```json
{
  "SourceRAG": {
    "VcsProvider": "Git",
    "EmbeddingProvider": "Local",
    "RepositoryPath": "/path/to/repo",
    "Branch": "main"
  }
}
```
:::lang

:::section

:::section vcs_providers

:::lang en
## VCS providers: Git and SVN as vertical slices

**ADR-001** defines VCS support as paired vertical slices: each provider implements both a repository reader and a blame provider, encapsulated together. Adding a new VCS means adding a new slice — not modifying existing code.

In practice this means Git uses LibGit2Sharp and SVN uses SharpSvn, but both expose the same domain interface. The revision concept is mapped consistently: Git uses commit SHA, SVN uses revision number. Re-index detection works by diffing the current head against the last indexed revision — `HEAD..lastIndexed` for Git, `GetLog(fromRev, HEAD)` for SVN.

**ADR-005** deliberately limits indexing scope to a single branch — `main` for Git, `trunk` for SVN. Multi-branch support is a scope decision, not a missing capability. The assumption is that the canonical branch contains what matters for knowledge retrieval.

**ADR-010** addresses credentials. VCS operations run under a read-only service role. Credentials are resolved through environment variables via `IVcsCredentialProvider` — no credentials in config files, no hardcoded values in code.
:::lang

:::lang hu
## VCS providerek: Git és SVN mint vertikális szeletek

Az **ADR-001** a VCS-támogatást párosított vertikális szeletekként definiálja: minden provider egyszerre implementál egy repository olvasót és egy blame providert, egységbe zárva. Új VCS hozzáadása új szeletet jelent — nem a meglévő kód módosítását.

A gyakorlatban ez azt jelenti, hogy a Git LibGit2Sharp-ot, az SVN SharpSvn-t használ, de mindkettő ugyanazt a domain interfészt expozálja. A revízió-fogalom következetesen van leképezve: Git commit SHA-t, SVN revision számot használ. Az újraindexelés-detektálás az aktuális head és az utolsó indexelt revízió összehasonlításával működik — Gitnél `HEAD..lastIndexed`, SVN-nél `GetLog(fromRev, HEAD)`.

Az **ADR-005** szándékosan egyetlen ágra korlátozza az indexelési hatókört — Gitnél `main`, SVN-nél `trunk`. A több-ágú támogatás hatóköri döntés, nem hiányzó képesség. Az feltételezés az, hogy a kanonikus ág tartalmazza azt, ami a tudáslekérdezés szempontjából fontos.

Az **ADR-010** a hitelesítési adatokkal foglalkozik. A VCS műveletek read-only service role-ban futnak. A hitelesítési adatok környezeti változókon keresztül kerülnek feloldásra `IVcsCredentialProvider`-en át — nem kerülnek config fájlba, és nem hardcodeoltak a kódban.
:::lang

:::section

:::section chunking

:::lang en
## Chunking: Roslyn first, plain text fallback

**ADR-003** defines chunking as a Chain of Responsibility. The first handler that can process a file claims it; the rest are skipped.

For C# files, the primary handler uses Roslyn. It parses the syntax tree and extracts chunks at meaningful semantic boundaries — methods, classes, properties. This means a chunk corresponds to something a developer would reason about, not an arbitrary line window. Symbol name and type are known at chunking time and go directly into the vector store payload.

For everything else — configuration files, documentation, SQL scripts, any non-C# source — a plain text handler takes over. It applies a sliding window with overlap to avoid cutting context at hard boundaries.

The chain is ordered and extensible. Adding support for another language means inserting a new handler into the chain before the plain text fallback, with no changes to the rest of the pipeline.
:::lang

:::lang hu
## Chunkolás: Roslyn elsőként, plain text fallback

Az **ADR-003** a chunkolást Chain of Responsibility mintaként definiálja. Az első handler, amely feldolgozni tudja a fájlt, "foglalja le" — a többi kihagyásra kerül.

C# fájloknál az elsődleges handler Roslyt használ. Elemzi a szintaxisfát, és értelmes szemantikai határokhoz — metódusokhoz, osztályokhoz, property-khez — igazodva nyeri ki a chunkokat. Ez azt jelenti, hogy egy chunk olyasminek felel meg, amiben egy fejlesztő gondolkodna, nem egy önkényes sorablakonak. A szimbólum neve és típusa chunkoláskor ismert, és közvetlenül a vektortár payload-jába kerül.

Minden máshoz — konfigurációs fájlok, dokumentáció, SQL scriptek, nem-C# forrás — a plain text handler veszi át az irányítást. Csúszó ablakot alkalmaz átfedéssel, hogy elkerülje a kontextus kemény határon való elvágását.

A lánc rendezett és bővíthető. Egy másik nyelv támogatásának hozzáadása egy új handler beillesztését jelenti a láncba a plain text fallback elé, a pipeline többi részének módosítása nélkül.
:::lang

:::section

:::section embedding

:::lang en
## Embedding: local or API, no code change required

**ADR-004** defines embedding as a config-driven provider choice. `EmbeddingProvider: "Local"` routes through LlamaSharp with a locally hosted GGUF model — offline, no API key, no data leaving the machine. `EmbeddingProvider: "Api"` routes through the Anthropic API.

Both providers implement the same `IEmbeddingProvider` interface. The Application layer calls the interface. It has no knowledge of which implementation is active.

For the current project phase, the local path is the default. The rationale is the same as in Scaffold Protocol: hardware-constrained offline-first development, with API-based providers as an option if a production context emerges. The difference here is that SourceRAG is designed as a multi-user system from the start — the API path matters more as scale increases.
:::lang

:::lang hu
## Embedding: lokális vagy API, kódmódosítás nélkül

Az **ADR-004** az embeddinget config-vezérelt provider-választásként definiálja. Az `EmbeddingProvider: "Local"` LlamaSharpot használ, egy lokálisan futtatott GGUF modellel — offline, API kulcs nélkül, az adatok nem hagyják el a gépet. Az `EmbeddingProvider: "Api"` az Anthropic API-on keresztül fut.

Mindkét provider ugyanazt az `IEmbeddingProvider` interfészt implementálja. Az Application réteg az interfészt hívja. Nincs tudomása arról, melyik implementáció aktív.

A jelenlegi projektfázisban a lokális útvonal az alapértelmezett. Az indok ugyanaz, mint a Scaffold Protocolnál: hardver-korlátozott, offline-first fejlesztés, API-alapú providerekkel mint opcióval, ha éles kontextus alakul ki. A különbség itt az, hogy a SourceRAG eleve többfelhasználós rendszernek van tervezve — az API útvonal fontosabbá válik, ahogy a skála növekszik.
:::lang

:::section

:::section proof_payload

:::lang en
## The chunk proof payload

**ADR-006** defines what gets stored in Qdrant for each indexed chunk. The point ID is deterministic: `sha256(repoPath + filePath + symbolName + revision)`. The same chunk indexed twice produces the same ID — which means re-indexing is idempotent by construction.

The payload is the proof structure:

```json
{
  "file_path": "src/Core/ImageProcessor.cs",
  "symbol_name": "ProcessTile",
  "symbol_type": "Method",
  "revision": "a3f9c12e",
  "author": "vvidman",
  "commit_message": "Fix OOM on large WSI files",
  "timestamp": "2024-11-03T14:22:00Z",
  "branch": "main",
  "start_line": 42,
  "end_line": 78
}
```

No source code is in this payload. At query time, the system takes `revision` and `file_path`, calls the VCS, and reconstructs the exact content as it existed at that commit. The proof isn't a stored copy — it's a pointer into version history.

This is what makes the approach interesting: the payload is small, the vector store stays lean, and the auditability is richer than what a traditional proof database would give you. You get authorship and change intent, not just file content.
:::lang

:::lang hu
## A chunk proof payload

Az **ADR-006** azt határozza meg, mi kerül a Qdrantba minden indexelt chunk esetén. A pont azonosítója determinisztikus: `sha256(repoPath + filePath + symbolName + revision)`. Ugyanaz a chunk kétszer indexelve ugyanazt az azonosítót adja — ami azt jelenti, hogy az újraindexelés konstrukciónál fogva idempotens.

A payload a proof-struktúra:

```json
{
  "file_path": "src/Core/ImageProcessor.cs",
  "symbol_name": "ProcessTile",
  "symbol_type": "Method",
  "revision": "a3f9c12e",
  "author": "vvidman",
  "commit_message": "Fix OOM on large WSI files",
  "timestamp": "2024-11-03T14:22:00Z",
  "branch": "main",
  "start_line": 42,
  "end_line": 78
}
```

Ebben a payloadban nincs forráskód. Lekérdezéskor a rendszer veszi a `revision`-t és a `file_path`-et, meghívja a VCS-t, és rekonstruálja a tartalmat pontosan úgy, ahogy az adott commitban létezett. A proof nem egy tárolt másolat — hanem egy mutató a verzióhistóriába.

Ez teszi az megközelítést érdekessé: a payload kicsi, a vektortár karcsú marad, az auditálhatóság pedig gazdagabb, mint amit egy hagyományos proof adatbázis adna. Nem csak fájltartalmat kapsz — szerzőséget és változtatási szándékot is.
:::lang

:::section

:::section hosting

:::lang en
## Dual hosting: REST and MCP over a shared Application layer

**ADR-008** is the most structurally significant decision in the solution: the same Application layer is shared by two independent host projects. `SourceRAG.Api` exposes a REST interface consumed by the Blazor web client. `SourceRAG.McpHost` exposes an MCP server consumed by AI agents — Claude Desktop, GitHub Copilot, or any MCP-compatible client.

Neither host project contains business logic. Both dispatch to the Application layer via MediatR. Adding a third host — a CLI, a gRPC service — would require no changes to Application or Infrastructure.

The MCP server exposes three tools:

```md
| Tool | Description |
|---|---|
| `search_codebase` | Semantic search over the indexed repository |
| `index_repository` | Trigger full or incremental reindex |
| `get_index_status` | Return current index state, last revision, chunk count |
```

**ADR-009** defines the Blazor client as a typed `HttpClient` targeting the REST API. The web client has no direct dependency on Application or Infrastructure — it's a consumer of the API, not a host.

**ADR-011** covers authentication. Both the Blazor web UI and the MCP server authenticate via Azure AD / Entra ID using OAuth 2.0. The decision to use Entra ID was driven by the multi-user design assumption — this isn't a single-developer local tool. Authentication is a first-class concern from the start.
:::lang

:::lang hu
## Dual hosting: REST és MCP egy közös Application réteg felett

Az **ADR-008** a solution leglényegesebb strukturális döntése: ugyanazt az Application réteget osztja két független host projekt. A `SourceRAG.Api` REST interfészt expozál, amelyet a Blazor web kliens fogyaszt. A `SourceRAG.McpHost` MCP szervert expozál, amelyet AI ügynökök fogyasztanak — Claude Desktop, GitHub Copilot, vagy bármely MCP-kompatibilis kliens.

Egyik host projekt sem tartalmaz üzleti logikát. Mindkettő MediatR-on keresztül diszpécsel az Application rétegnek. Egy harmadik host hozzáadása — CLI, gRPC szolgáltatás — nem igényelne módosítást az Application vagy az Infrastructure oldalán.

Az MCP szerver három toolt expozál:

```md
| Tool | Leírás |
|---|---|
| `search_codebase` | Szemantikus keresés az indexelt repositoryban |
| `index_repository` | Teljes vagy inkrementális újraindexelés indítása |
| `get_index_status` | Aktuális indexállapot, utolsó revízió, chunk-szám |
```

Az **ADR-009** a Blazor klienst typed `HttpClient`-ként definiálja, amely a REST API-t célozza. A web kliens nem függ közvetlenül az Application vagy az Infrastructure rétegektől — az API fogyasztója, nem hostja.

Az **ADR-011** a hitelesítést fedi le. A Blazor web UI és az MCP szerver is Azure AD / Entra ID-n keresztül hitelesít OAuth 2.0 használatával. Az Entra ID melletti döntést a többfelhasználós tervezési feltételezés vezérelte — ez nem egy egyfejlesztős lokális eszköz. A hitelesítés első osztályú elvárás a kezdetektől.
:::lang

:::section

:::section observability

:::lang en
## Observability across the pipeline

**ADR-007** adds AiObservability instrumentation to both the indexing and query pipelines. Every significant operation — a chunking pass, an embedding call, a vector store write, a semantic search — produces a span. The spans are structured and carry context: which file was chunked, which revision was being indexed, how many chunks were produced, how long the embedding took.

This matters in a RAG system because failures aren't always obvious. A file that produces zero chunks is a silent omission. An embedding call that times out degrades retrieval quality without throwing an exception. Observability turns these into visible, queryable events.

AiObservability is a companion library I maintain separately. SourceRAG integrates it — this is one of the related projects listed in the README.
:::lang

:::lang hu
## Megfigyelhetőség a pipeline-on át

Az **ADR-007** AiObservability instrumentálást ad hozzá az indexelési és lekérdezési pipeline-okhoz egyaránt. Minden lényeges művelet — egy chunkolási menet, egy embedding hívás, egy vektortár-írás, egy szemantikus keresés — spant generál. A spanok strukturáltak és kontextust hordoznak: melyik fájl lett chunkolva, melyik revíziót indexelték, hány chunk keletkezett, mennyi ideig tartott az embedding.

Ez egy RAG rendszerben azért fontos, mert a hibák nem mindig nyilvánvalók. Egy fájl, amely nulla chunkot produkál, csendes kihagyás. Egy embedding hívás, amely timeout-ol, rontja a lekérdezési minőséget anélkül, hogy kivételt dobna. A megfigyelhetőség ezeket látható, lekérdezhető eseményekké alakítja.

Az AiObservability egy kísérő könyvtár, amelyet külön tartok karban. A SourceRAG integrálja — ez az egyik kapcsolódó projekt, amely a README-ben szerepel.
:::lang

:::section

:::section repo_link

:::lang en
## Source

The repository is available on GitHub: [SourceRAG](https://github.com/vvidman/SourceRAG)
:::lang

:::lang hu
## Forrás

A repository elérhető GitHubon: [SourceRAG](https://github.com/vvidman/SourceRAG)
:::lang

:::section

:::section status

:::lang en
## Where this stands

SourceRAG is at concept and README stage. The architecture decisions are locked, the solution structure is defined, and the core interfaces are sketched. Implementation is beginning.

The question I'm actually testing is simple: does removing the proof store duplication produce a system that is meaningfully simpler to operate? The expected gains are clear on paper — no sync logic, no schema drift, richer traceability for free. Whether those gains survive contact with real repositories, real commit histories, and real query patterns is what the implementation will answer.

I'll write about what I find — including what doesn't work. If you've built something similar, or if the VCS-as-proof-store idea surfaces a problem I haven't considered, I'd like to hear it.

*Related projects: [RagLab](https://github.com/vvidman/RagLab) · [AiObservability](https://github.com/vvidman/AiObservability) · [Scaffold Protocol](https://github.com/vvidman/ScaffoldProtocol)*
:::lang

:::lang hu
## Ahol most tart

A SourceRAG koncepció és README szinten van. Az architektúrális döntések rögzítve vannak, a solution struktúra definiált, az alapvető interfészek felvázolva. Az implementáció kezdődik.

A kérdés, amelyet valójában tesztelek, egyszerű: a proof tár duplikáció eltávolítása egy érdemben egyszerűbben üzemeltetható rendszert produkál-e? A várható nyereségek papíron világosak — nincs szinkronizálási logika, nincs séma-drift, gazdagabb visszakövethetőség ingyen. Hogy ezek a nyereségek túlélik-e a találkozást valódi repositorykkal, valódi commithistóriával és valódi lekérdezési mintákkal — erre az implementáció fog válaszolni.

Írok majd arról, amit tapasztalok — beleértve azt is, ami nem működik. Ha valami hasonlót építettél, vagy ha a VCS-as-proof-store ötlet olyan problémát vet fel, amelyet nem vettem figyelembe, szívesen hallom.

*Kapcsolódó projektek: [RagLab](https://github.com/vvidman/RagLab) · [AiObservability](https://github.com/vvidman/AiObservability) · [Scaffold Protocol](https://github.com/vvidman/ScaffoldProtocol)*
:::lang

:::section
