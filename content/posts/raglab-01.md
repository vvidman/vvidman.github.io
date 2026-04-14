---
title: Building a RAG Pipeline from Scratch
title_hu: RAG építése az alapoktól
slug: posts/raglab-01.html
description: What I learned by building a RAG pipeline in .NET without a framework — every component written from scratch, every decision made consciously.
layout: page
series: RAGLab
part: 1
---

:::section intro

:::lang en
## Problem statement

There is a pattern I keep seeing in the developer community around AI tooling. Someone discovers LangChain or LlamaIndex, chains together a few abstractions, gets a working demo in an afternoon, and walks away feeling like they understand Retrieval-Augmented Generation. Then someone asks them why their retrieval is returning irrelevant results, or why swapping the embedding model broke everything, and the answer is silence.

I did not want that to be me. So I built RagLab — a hand-crafted RAG pipeline in .NET/C#, with no high-level framework magic. Every component written from scratch, every decision made consciously. This is what I learned.
:::lang

:::lang hu
## Probléma megfogalmazás

Egy mintát látok újra és újra az AI eszközök körüli fejlesztői közösségben. Valaki felfedezi a LangChain-t vagy a LlamaIndex-et, összefűz néhány absztrakciót, egy délután alatt kap egy működő demót — és azzal a meggyőződéssel megy el, hogy érti a Retrieval-Augmented Generation-t. Aztán valaki megkérdezi, miért ad vissza irreleváns eredményeket a retrieval, vagy miért tört el minden, amikor kicserélte az embedding modellt. Csönd a válasz.

Én nem akartam ilyen lenni. Ezért építettem meg a RagLab-et — egy kézzel írt RAG pipeline-t .NET/C#-ban, framework varázslat nélkül. Minden komponens nulláról megírva, minden döntés tudatosan meghozva. Ez az, amit megtanultam.
:::lang

:::section

:::section what-is-rag

:::lang en
## What is RAG, and why does it matter?

Large language models are powerful, but their knowledge is frozen at training time. Ask a model about something that happened last month, or about a document on your company's internal server, and it either hallucinates or admits it does not know.

RAG solves this by separating two concerns: **knowing** and **generating**.

Instead of asking the model to know everything, you build a searchable knowledge base from your own documents. When a question comes in, you first retrieve the relevant pieces from that knowledge base, then hand those pieces — and only those pieces — to the model as context. The model's job is just to synthesize an answer from what you give it. The knowledge comes from your data, not from the model's parameters.

The practical consequence is significant: you can give the model access to any document, updated at any time, without retraining anything.
:::lang

:::lang hu
## Mi az a RAG, és miért fontos?

A nagy nyelvi modellek erősek, de tudásuk befagy a tanítás időpontjában. Kérdezz egy modellt valamiről, ami múlt hónapban történt, vagy a céged belső szerverén lévő dokumentumról — és vagy hallucinálja a választ, vagy bevallja, hogy nem tudja.

A RAG ezt két szempont szétválasztásával oldja meg: **tudás** és **generálás**.

Ahelyett, hogy mindent tudnia kellene a modellnek, egy kereshető tudásbázist építesz a saját dokumentumaidból. Amikor kérdés érkezik, először lekéred a releváns részeket ebből a tudásbázisból, majd csak ezeket adod át a modellnek kontextusként. A modell feladata csupán annyi, hogy szintetizáljon egy választ abból, amit kap. A tudás a te adataidból származik, nem a modell paramétereiből.

A gyakorlati következmény jelentős: hozzáférést adhatsz a modellnek bármilyen dokumentumhoz, bármikor frissítve — anélkül, hogy bármit újra kellene tanítani.
:::lang

:::section

:::section the-pipeline

:::lang en
## The Two Flows

A RAG system has two distinct flows. Understanding the difference between them is essential.

**Indexing** runs once, or whenever your documents change. This is the preparation phase.

```
File → Loader → Document → Chunker → Chunks → Embedder → Vectors → Vector Store
```

**Querying** runs for every question a user asks.

```
Question → Embedder → Vector → Search → Top Chunks → Generator → Answer
```

Notice that the embedder appears in both flows. This is not a coincidence — it is one of the most important architectural constraints in any RAG system. The same embedding model must be used during indexing and querying. Vectors produced by different models live in incompatible mathematical spaces. Swap the embedding model and you have to re-index everything from scratch.
:::lang

:::lang hu
## A két folyamat

Egy RAG rendszernek két különálló folyamata van. A kettő közötti különbség megértése elengedhetetlen.

Az **indexelés** egyszer fut le, vagy amikor a dokumentumaid megváltoznak. Ez az előkészítési fázis.

```
Fájl → Loader → Document → Chunker → Chunks → Embedder → Vektorok → Vector Store
```

A **lekérdezés** minden felhasználói kérdésnél lefut.

```
Kérdés → Embedder → Vektor → Keresés → Top Chunks → Generator → Válasz
```

Figyeld meg, hogy az embedder mindkét folyamatban szerepel. Ez nem véletlen — ez az egyik legfontosabb architektúrális megszorítás bármely RAG rendszerben. Ugyanazt az embedding modellt kell használni az indexeléskor és a lekérdezéskor. Különböző modellek által előállított vektorok inkompatibilis matematikai terekben élnek. Cseréld ki az embedding modellt, és mindent újra kell indexelned nulláról.
:::lang

:::section

:::section component-loader

:::lang en
## The Loader

The loader's job is to read a file and produce a clean, normalized internal representation. Nothing more, nothing less.

"RAG-ready" is not a file format. It is a set of properties: clean text, consistent encoding, preserved structure where it matters, attached metadata. A PDF, a Markdown file, and a Word document all need to go through different parsing logic, but they all produce the same thing at the end: a `Document` with content and metadata.

This is a textbook application of the adapter pattern. Each loader adapts a specific format into a common representation, and the rest of the pipeline never has to care about where the document came from.

```csharp
public interface IDocumentLoader
{
    bool CanLoad(string filePath);
    Task<Document> LoadAsync(string filePath, CancellationToken ct = default);
}
```

The `CanLoad` method is the key: the pipeline asks each registered loader in turn whether it can handle a given file. The first one that says yes takes the job. Adding a new format means implementing this interface — nothing else changes.
:::lang

:::lang hu
## A Loader

A loader feladata: beolvas egy fájlt, és egy tiszta, normalizált belső reprezentációt állít elő. Nem több, nem kevesebb.

A "RAG-ready" nem fájlformátum. Tulajdonságok halmaza: tiszta szöveg, konzisztens kódolás, megőrzött struktúra ahol számít, csatolt metaadatok. Egy PDF, egy Markdown fájl és egy Word dokumentum mind különböző parsing logikán megy keresztül, de a végén ugyanazt adja vissza: egy `Document`-et, tartalommal és metaadatokkal.

Ez az adapter pattern tankönyvi alkalmazása. Minden loader adaptál egy specifikus formátumot egy közös reprezentációra — a pipeline többi része soha nem törődik azzal, honnan jött a dokumentum.

```csharp
public interface IDocumentLoader
{
    bool CanLoad(string filePath);
    Task<Document> LoadAsync(string filePath, CancellationToken ct = default);
}
```

A `CanLoad` metódus a kulcs: a pipeline megkérdezi az összes regisztrált loadert, hogy kezelni tudja-e az adott fájlt. Az első, amelyik igent mond, elvégzi a munkát. Új formátum hozzáadása annyit jelent, hogy implementálod ezt az interface-t — más nem változik.
:::lang

:::section

:::section component-chunker

:::lang en
## The Chunker

You cannot give an entire document to a language model. Context windows are finite, and even when they are large, flooding the model with irrelevant text degrades the quality of its answers. The chunker splits a document into manageable pieces.

RagLab uses fixed-size character-based chunking with overlap. The overlap is the part that surprises people: if you split text at hard boundaries, you risk cutting a sentence — or a concept — in half. With overlap, every semantic unit is guaranteed to appear whole in at least one chunk.

**Why characters and not tokens?**

Tokens are the unit language models use internally. Token-based chunking would be more precise — you could guarantee that no chunk exceeds a model's context limit. But every model family has its own tokenizer. Tying the chunker to a specific tokenizer creates a coupling that makes the system brittle when you swap models. Character-based chunking is a deliberate simplification: modestly less precise, but completely model-independent.

The right way to handle this in production is to treat the tokenizer as part of the model provider configuration, not part of the chunker. RagLab encodes this through the vertical slice pattern — each model provider registers its own chunk size and overlap tuned to its context window.
:::lang

:::lang hu
## A Chunker

Nem adhatsz egy teljes dokumentumot egy nyelvi modellnek. A context window véges, és még ha nagy is, az irreleváns szöveggel való árasztás rontja a válaszok minőségét. A chunker feldarabolja a dokumentumot kezelhető darabokra.

A RagLab fix méretű, karakter-alapú chunking-ot használ, átfedéssel. Az átfedés az a rész, ami meglepetést okoz: ha kemény határon osztod el a szöveget, félbevághatod a mondatot — vagy egy koncepciót. Az átfedéssel garantálható, hogy minden szemantikai egység legalább egy chunkban teljes egészében megjelenik.

**Miért karakterek és nem tokenek?**

A tokenek azok az egységek, amelyeket a nyelvi modellek belül használnak. A token-alapú chunking pontosabb lenne — garantálhatnád, hogy egyetlen chunk sem lépi túl a modell context limitjét. De minden modellcsaládnak saját tokenizere van. Ha a chunkert egy specifikus tokenizerhez kötöd, törékeny rendszert kapsz, amikor modellt cserélsz. A karakter-alapú chunking tudatos egyszerűsítés: kicsit kevésbé pontos, de teljesen modell-független.

A helyes megközelítés az, hogy a tokenizert a modell provider konfigurációjának részeként kezeled, nem a chunkeren belül. A RagLab ezt a vertical slice pattern segítségével kódolja — minden model provider regisztrálja a saját chunk méretét és átfedési értékét, a saját context windowjára hangolva.
:::lang

:::section

:::section component-embedder

:::lang en
## The Embedder

This is where the demystification really begins.

An embedding model takes a piece of text and produces a list of numbers — a vector. For the model used in RagLab (`nomic-embed-text`), that list is 768 numbers long. What those numbers represent is the *meaning* of the text, encoded as a position in a 768-dimensional space.

Texts with similar meanings end up close to each other in that space. "The dog barked" and "The hound made noise" will produce vectors that point in roughly the same direction. "The stock market fell" will point somewhere completely different.

The key insight — and this took me a while to internalize — is that **vectors are not passed to the language model**. They exist exclusively to make the knowledge base searchable. The language model never sees a vector. It sees text — specifically, the text of the chunks the search retrieved.

The embedder is also where the system becomes model-specific. Vectors produced by `nomic-embed-text` live in a completely different mathematical space than vectors produced by OpenAI's `text-embedding-ada-002`. They are incompatible. If you swap the embedding model, you have to re-index your entire knowledge base from scratch. The original documents are the source of truth; the vectors are a derived artifact.
:::lang

:::lang hu
## Az Embedder

Itt kezdődik a valódi demisztifikáció.

Egy embedding modell fogja a szöveget, és előállít egy számlistát — egy vektort. A RagLab-ban használt modell (`nomic-embed-text`) esetén ez a lista 768 szám hosszú. Amit ezek a számok reprezentálnak, az a szöveg *jelentése*, 768 dimenziós térben kódolva.

A hasonló jelentésű szövegek közel kerülnek egymáshoz ebben a térben. "A kutya ugatott" és "Az eb hangot adott ki" nagyjából azonos irányba mutató vektorokat fog produkálni. "A tőzsde esett" teljesen más irányba mutat.

A kulcsgondolat — és ezt egy ideig tartott belsővé tennem — az, hogy **a vektorok nem kerülnek át a nyelvi modellhez**. Kizárólag arra léteznek, hogy kereshetővé tegyék a tudásbázist. A nyelvi modell soha nem lát vektort. Szöveget lát — pontosabban azoknak a chunkoknak a szövegét, amelyeket a keresés visszaadott.

Az embedder az a pont, ahol a rendszer modell-specifikussá válik. A `nomic-embed-text` által előállított vektorok teljesen más matematikai térben élnek, mint az OpenAI `text-embedding-ada-002` által előállítottak. Inkompatibilisek. Ha kicseréled az embedding modellt, a teljes tudásbázisodat nulláról kell újraindexelned. Az eredeti dokumentumok az igazság forrása; a vektorok levezetett artifaktok.
:::lang

:::section

:::section component-vectorstore

:::lang en
## The Vector Store

The vector store holds all the embedded chunks and answers one question: given a query vector, which stored vectors are closest to it?

RagLab implements this in memory using cosine similarity. The intuition is simple: two vectors can be compared by the angle between them. A small angle means they point in the same direction — similar meaning. Cosine similarity measures the cosine of that angle, producing a score between -1 and 1.

```csharp
private static float CosineSimilarity(float[] a, float[] b)
{
    float dot = 0, normA = 0, normB = 0;
    for (int i = 0; i < a.Length; i++)
    {
        dot   += a[i] * b[i];
        normA += a[i] * a[i];
        normB += b[i] * b[i];
    }
    return dot / (MathF.Sqrt(normA) * MathF.Sqrt(normB));
}
```

The division by the vector lengths is the key operation: it removes the effect of length and isolates direction. The same text at different lengths should mean the same thing — and with cosine similarity, it does.

The store returns the top-K most similar chunks. In RagLab, K defaults to 3. This is derived from the generation model's context window size minus the prompt template overhead. Too few chunks and you risk missing relevant information; too many and the model's attention gets diluted — a phenomenon sometimes called "lost in the middle."

RagLab uses a dual vector store: one for document knowledge, one for conversation history. Both are queried independently with separate topK allocations, preventing conversation history from pushing out relevant document chunks.
:::lang

:::lang hu
## A Vector Store

A vector store tárolja az összes beágyazott chunkot, és egyetlen kérdésre válaszol: adott egy lekérdezési vektor — melyik tárolt vektorok állnak a legközelebb hozzá?

A RagLab ezt memóriában implementálja, koszinusz hasonlóság (cosine similarity) segítségével. Az intuíció egyszerű: két vektort az általuk bezárt szög alapján lehet összehasonlítani. Kis szög azt jelenti, hogy azonos irányba mutatnak — hasonló a jelentésük. A cosine similarity ennek a szögnek a koszinuszát méri, -1 és 1 közötti értéket adva vissza.

```csharp
private static float CosineSimilarity(float[] a, float[] b)
{
    float dot = 0, normA = 0, normB = 0;
    for (int i = 0; i < a.Length; i++)
    {
        dot   += a[i] * b[i];
        normA += a[i] * a[i];
        normB += b[i] * b[i];
    }
    return dot / (MathF.Sqrt(normA) * MathF.Sqrt(normB));
}
```

A vektorhosszakkal való osztás a kulcsoperáció: eltávolítja a hossz hatását, és csak az irányt hagyja meg. Ugyanaz a szöveg különböző hosszban leírva ugyanazt kell hogy jelentse — és a cosine similarity esetén így is van.

A store visszaadja a top-K leghasonlóbb chunkot. A RagLab-ban K alapértelmezetten 3. Ez a generáló modell context window méretéből és a prompt template overhead-jéből következik. Túl kevés chunk: fennáll a veszély, hogy releváns információt hagysz ki. Túl sok: a modell figyelme szétoszlik — ezt a jelenséget néha "lost in the middle"-nek hívják.

A RagLab duális vector store-t használ: egy a dokumentumtudásnak, egy a conversation historynak. Mindkettőből külön, független topK allokációval történik a lekérdezés — ez megakadályozza, hogy a history kiszorítsa a releváns dokumentum chunkokat.
:::lang

:::section

:::section component-generator

:::lang en
## The Generator

The generator is the only component that involves a generative language model. Everything before it was preparation. Here, the assembled context and the original question are formatted into a prompt and sent to the model.

```
You are a helpful assistant.
Answer exclusively based on the provided context.
If the context does not contain relevant information, say so clearly.

Context:
[1] {chunk_1}
[2] {chunk_2}
[3] {chunk_3}

Question: {query}
```

The "answer exclusively based on the provided context" instruction is load-bearing. Without it, the model will blend its own parametric knowledge with the retrieved context, making it impossible to know whether an answer came from your documents or from the model's training data. This constraint is what makes RAG auditable.

The model itself is stateless. It has no memory of previous questions. The illusion of continuity in a conversation is always constructed by the application layer — by deciding what to include in the prompt for each call. This is not a limitation to work around; it is the correct mental model.
:::lang

:::lang hu
## A Generator

A generator az egyetlen komponens, amely generatív nyelvi modellt használ. Minden, ami előtte volt, előkészítés. Itt az összegyűjtött kontextus és az eredeti kérdés egy promptba formázódik, és elküldésre kerül a modellnek.

```
You are a helpful assistant.
Answer exclusively based on the provided context.
If the context does not contain relevant information, say so clearly.

Context:
[1] {chunk_1}
[2] {chunk_2}
[3] {chunk_3}

Question: {query}
```

Az "answer exclusively based on the provided context" utasítás kritikus. Nélküle a modell összekeveri saját paraméteres tudását a visszakeresett kontextussal — és lehetetlenné válik megállapítani, hogy egy válasz a te dokumentumaidból vagy a modell tanítási adataiból származik. Ez a megszorítás teszi a RAG-ot auditálhatóvá.

A modell maga állapotmentes. Nincs emlékezete korábbi kérdésekre. A folyamatosság illúzióját mindig az alkalmazás rétege építi fel — azzal, hogy dönt: mi kerüljön bele az egyes hívások promptjába. Ez nem egy korlát, amit ki kell kerülni; ez a helyes mentális modell.
:::lang

:::section

:::section architectural-decisions

:::lang en
## Architectural Decisions

### Interfaces over implementations

Every component in RagLab is hidden behind an interface: `IDocumentLoader`, `IChunker`, `IEmbedder`, `IVectorStore`, `IGenerator`. The pipeline orchestrator depends on abstractions, not on concrete types.

This is not cargo-culting SOLID principles. It reflects a real constraint: the components that are easy to swap (document loaders, generators) and the components that are expensive to swap (the embedder, because of re-indexing) are different. Having clear interfaces makes those swap points explicit — even when the swap itself requires work.

### There is no general RAG, only model-specific RAG

This was the most important thing I internalized while building this. Chunk size, overlap, topK — these are all downstream consequences of which embedding model and generation model you use. They travel together. A system that pretends they are independent configuration knobs will be fragile in practice.

RagLab encodes this through the vertical slice pattern. Each model provider is a self-contained slice that owns its entire registration logic:

```csharp
// Program.cs — the only place a concrete type is instantiated
IModelSlice slice = new LlamaSlice();
// or: new ClaudeSlice();
slice.Register(builder.Services, builder.Configuration);
```

Each slice brings its own `IEmbedder`, `IGenerator`, chunk size, overlap, and recommended topK as a coherent unit. Switching providers means changing one line.

### Records for data, sealed classes for implementations

Domain models are C# records — immutable, value-semantic, no behavior. Implementations are sealed classes. This keeps the distinction between "data that flows through the pipeline" and "logic that transforms the data" structurally clear.
:::lang

:::lang hu
## Architektúrális döntések

### Interface-ek az implementációk felett

A RagLab minden komponense interface mögé van rejtve: `IDocumentLoader`, `IChunker`, `IEmbedder`, `IVectorStore`, `IGenerator`. A pipeline orchestrátora absztrakciókra támaszkodik, nem konkrét típusokra.

Ez nem SOLID elvek cargo-cult követése. Egy valódi megszorítást tükröz: a könnyen cserélhető komponensek (document loaderek, generátorok) és a drágán cserélhető komponensek (az embedder, az újraindexelés miatt) különbözők. Az egyértelmű interface-ek láthatóvá teszik ezeket a cserepontokat — akkor is, ha maga a csere munkát igényel.

### Nincs általános RAG, csak modell-specifikus RAG

Ez volt a legfontosabb dolog, amit az építés során belsővé tettem. A chunk méret, az átfedés, a topK — ezek mind az embedding és generáló modell megválasztásának következményei. Együtt járnak. Egy rendszer, amely úgy tesz, mintha független konfigurációs gombok lennének, törékeny lesz.

A RagLab ezt a vertical slice pattern segítségével kódolja. Minden model provider egy önálló szelet, amely a teljes regisztrációs logikájának tulajdonosa:

```csharp
// Program.cs — az egyetlen hely, ahol konkrét típus példányosodik
IModelSlice slice = new LlamaSlice();
// vagy: new ClaudeSlice();
slice.Register(builder.Services, builder.Configuration);
```

Minden szelet hozza a saját `IEmbedder`-ét, `IGenerator`-át, chunk méretét, átfedési értékét és ajánlott topK-ját — koherens egységként. A provider cseréje egyetlen sor módosítását jelenti.

### Record-ok az adatnak, sealed class-ok az implementációnak

A domain modellek C# record-ok — immutábilisek, értékszemantikájúak, viselkedés nélkül. Az implementációk sealed class-ok. Ez strukturálisan egyértelművé teszi a különbséget "a pipeline-on átfolyó adatok" és "az adatokat transzformáló logika" között.
:::lang

:::section

:::section what-i-learned

:::lang en
## What I Actually Learned

**Basic math is enough.** Cosine similarity is taught in secondary school. The only reason it feels exotic in ML contexts is the number of dimensions. The operation is identical whether you have 3 dimensions or 768.

**Simplicity wins.** Character-based chunking is less precise than token-based chunking. It is also trivially understandable, completely model-independent, and sufficient for the vast majority of use cases. The right question is never "what is the most sophisticated approach?" but "what is the simplest approach that solves the actual problem?"

**Understanding the limits of a pattern matters more than knowing the pattern.** RAG is not magic. It will return irrelevant results if the chunking is wrong. It will fail silently if you swap the embedding model without re-indexing. It will hallucinate if the prompt does not constrain the model to the provided context. Knowing *why* these failure modes exist is what lets you prevent and diagnose them.

**Frameworks hide the interesting parts.** LangChain and LlamaIndex are excellent tools for shipping. They are poor tools for understanding. Building from scratch forced me to make every decision explicitly — and every decision taught me something.
:::lang

:::lang hu
## Amit tényleg megtanultam

**Az alap matek elég.** A cosine similarity-t középiskolában tanítják. Az egyetlen ok, amiért egzotikusnak tűnik ML kontextusban, a dimenziók száma. A művelet azonos, akár 3, akár 768 dimenzióban.

**Az egyszerűség nyer.** A karakter-alapú chunking kevésbé pontos, mint a token-alapú. De triviálisan érthető, teljesen modell-független, és a használati esetek túlnyomó többségéhez elegendő. A helyes kérdés soha nem az, hogy "mi a legsofisztikáltabb megközelítés?", hanem: "mi a legegyszerűbb megközelítés, amely megoldja a valódi problémát?"

**A pattern korlátainak megértése fontosabb, mint a pattern ismerete.** A RAG nem mágia. Irreleváns eredményeket ad, ha a chunking rossz. Csendben sikertelen lesz, ha az embedding modellt cseréled újraindexelés nélkül. Hallucináció lép fel, ha a prompt nem szorítja a modellt a kapott kontextusra. Annak ismerete, *miért* léteznek ezek a hibamódok, az, ami lehetővé teszi megelőzésüket és diagnosztizálásukat.

**A frameworkök elrejtik az érdekes részeket.** A LangChain és a LlamaIndex kiváló eszközök a szállításhoz. Gyenge eszközök a megértéshez. A nulláról való építés arra kényszerített, hogy minden döntést expliciten hozzak meg — és minden döntés tanított valamit.
:::lang

:::section

:::section whats-next

:::lang en
## What Comes Next

RagLab is a living project. The planned next steps:

**Phase 2** replaces the in-memory vector store with Qdrant — a production-ready vector database — and adds a Claude API generator alongside the local LlamaSharp one. The Core interfaces remain unchanged; only new Infrastructure implementations are added.

**Phase 3** introduces hybrid search: combining semantic vector search with BM25 keyword search, plus reranking. This is where production RAG systems typically diverge from tutorial RAG systems.

A **vertical slice refactor** will enforce the coupling between model provider and pipeline parameters structurally, not just by convention. The goal is compile-time guarantees, not runtime surprises.

The code is available on [GitHub](https://github.com/vvidman/RagLab). Every component is intentionally minimal — the goal is always clarity over cleverness.
:::lang

:::lang hu
## Mi jön ezután

A RagLab egy élő projekt. A tervezett következő lépések:

A **2. fázis** lecseréli a memória-alapú vector store-t Qdrant-ra — egy production-ready vektoradatbázisra — és hozzáad egy Claude API generátort a helyi LlamaSharp mellé. A Core interface-ek változatlanok maradnak; csak új Infrastructure implementációk kerülnek hozzáadásra.

A **3. fázis** hibrid keresést vezet be: szemantikai vektor keresés és BM25 kulcsszó keresés kombinációja, plusz rerankinggal. Itt szoktak a production RAG rendszerek eltérni a tutorial RAG rendszerektől.

A **vertical slice refactor** strukturálisan kényszeríti ki a model provider és a pipeline paraméterek közötti összekapcsoltságot — nem csupán konvenció alapján. A cél a fordítási idejű garancia, nem a futásidejű meglepetés.

A kód elérhető a [GitHubon](https://github.com/vvidman/RagLab). Minden komponens szándékosan minimális — a cél mindig az érthetőség, nem az okosság.
:::lang

:::section
