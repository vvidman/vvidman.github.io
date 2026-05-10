---
title: Building the .NET AI Stack from First Principles
title_hu: A .NET AI Stack felépítése az alapoktól
slug: posts/dotnet-ai-stack-01.html
description: What I learned by wiring together LlamaSharp, Microsoft.Extensions.AI, and Semantic Kernel — and why the documentation does not tell the whole story.
layout: page
date: 2026-05-10
series: .NET Agentic AI
part: 1
---

:::section intro

:::lang en
## Problem statement

Every AI tutorial starts the same way. You add the OpenAI NuGet package, paste in an API key, call `CompleteAsync`, and you have a working chat app in ten minutes. What you also have, without realising it, is a hard dependency on a single provider baked into every layer of your code.

I wanted to understand the stack properly before building anything on top of it. Not just which packages to install, but why the layers exist, what each one is responsible for, and what actually breaks when the documentation is wrong. This is Module 1 of my .NET Agentic AI curriculum — setting up the foundation that everything else will run on.
:::lang

:::lang hu
## Probléma megfogalmazás

Minden AI tutorial ugyanúgy kezdődik. Hozzáadod az OpenAI NuGet csomagot, beilleszted az API kulcsot, meghívod a `CompleteAsync`-ot, és tíz perc alatt van egy működő chat appod. Amid szintén van — anélkül, hogy észrevennéd — az egy egyetlen providertől való kemény függőség, ami beégett a kódod minden rétegébe.

Szerettem volna rendesen megérteni a stack-et, mielőtt bármit rá építek. Nem csak azt, hogy melyik csomagot kell telepíteni, hanem azt, hogy miért léteznek a rétegek, mindegyik miért felelős, és mi törik el valójában, amikor a dokumentáció nem mondja az egészet. Ez a .NET Agentic AI curriculumom 1. modulja — az alap felállítása, amelyre minden más épülni fog.
:::lang

:::section

:::section the-three-layers

:::lang en
## Why three layers?

The stack has three distinct responsibilities, and keeping them separate is the whole point.

```
┌──────────────────────────────────────────────────────┐
│                  YOUR CODE                           │
│         (business logic, agent workflows)            │
├──────────────────────────────────────────────────────┤
│             SEMANTIC KERNEL                          │
│   "What should the agent do?"                        │
│   Plugins, Memory, Planner, AgentGroupChat           │
├──────────────────────────────────────────────────────┤
│         MICROSOFT.EXTENSIONS.AI                      │
│   "How should it talk to any LLM?"                   │
│   IChatClient — one interface for every provider     │
├──────────────────────────────────────────────────────┤
│            LLM PROVIDER (swappable)                  │
│   LlamaSharp │ Azure OpenAI │ OpenAI │ Ollama │ ...  │
└──────────────────────────────────────────────────────┘
```

**Microsoft.Extensions.AI** defines `IChatClient` — a single interface that sits above every LLM provider. Think of it as `ILogger` for language models. You do not care whether logs go to the console, a file, or Seq. You do not care whether inference runs locally on LlamaSharp or remotely on Azure OpenAI. Swapping providers is one line.

**Semantic Kernel** is the agent framework. It uses M.E.AI internally — they are not competitors, they are layers. SK handles plugins, memory, planning, and multi-agent orchestration. M.E.AI handles the actual LLM communication.

**LlamaSharp** runs a GGUF model locally. No API key, no cloud dependency, no cost per token. For development and for understanding the stack, this is ideal.
:::lang

:::lang hu
## Miért három réteg?

A stacknek három különálló felelőssége van, és ezek elkülönítése az egész lényege.

```
┌──────────────────────────────────────────────────────┐
│                  A TE KÓDOD                          │
│         (business logic, agent workflows)            │
├──────────────────────────────────────────────────────┤
│             SEMANTIC KERNEL                          │
│   "Mit csináljon az agent?"                          │
│   Plugins, Memory, Planner, AgentGroupChat           │
├──────────────────────────────────────────────────────┤
│         MICROSOFT.EXTENSIONS.AI                      │
│   "Hogyan kommunikáljon bármely LLM-mel?"            │
│   IChatClient — egy interfész minden providerre      │
├──────────────────────────────────────────────────────┤
│            LLM PROVIDER (cserélhető)                 │
│   LlamaSharp │ Azure OpenAI │ OpenAI │ Ollama │ ...  │
└──────────────────────────────────────────────────────┘
```

A **Microsoft.Extensions.AI** definiálja az `IChatClient`-et — egy egységes interfészt, amely minden LLM provider fölé kerül. Gondolj rá úgy mint az `ILogger`-re, csak nyelvi modelleknek. Nem érdekel, hogy a logok konzolra, fájlba vagy Seq-be mennek. Nem érdekel, hogy az inferencia lokálisan fut LlamaSharp-on vagy távolról az Azure OpenAI-on. A provider csere egyetlen sor.

A **Semantic Kernel** az agent framework. Belsőleg az M.E.AI-t használja — nem versenytársak, hanem rétegek. Az SK kezeli a pluginokat, a memóriát, a tervezést és a multi-agent orkesztrációt. Az M.E.AI kezeli a tényleges LLM kommunikációt.

A **LlamaSharp** lokálisan futtat egy GGUF modellt. Nincs API kulcs, nincs felhőfüggőség, nincs token-alapú költség. Fejlesztéshez és a stack megértéséhez ez ideális.
:::lang

:::section

:::section what-the-docs-miss

:::lang en
## What the documentation does not tell you

The module description I started from referenced a class called `LLamaSharpChatClient`. It does not exist.

The actual class in `LLamaSharp.SemanticKernel` is `LLamaSharpChatCompletion`, which implements SK's `IChatCompletionService` — not M.E.AI's `IChatClient`. To bridge between them, you call `.AsChatClient()`:

```csharp
var llamaChat = new LLamaSharpChatCompletion(executor);
IChatClient chatClient = llamaChat.AsChatClient();
```

Before you can create the `executor`, you need to build the full LlamaSharp object chain. This was also missing from the original description:

```csharp
var modelParams = new ModelParams(modelPath)
{
    ContextSize = contextSize,
    GpuLayerCount = gpuLayers
};

using var model   = LLamaWeights.LoadFromFile(modelParams);
using var context = model.CreateContext(modelParams);
var executor      = new InteractiveExecutor(context);
```

The `model` and `context` objects are `IDisposable`. In a DI factory, they are never disposed — something to address in production with a hosted service wrapper.

The second issue was with Semantic Kernel registration. The module code called `.Build()` on the result of `AddKernel()`. That throws:

```
System.InvalidOperationException: Build is not permitted on instances
returned from AddKernel. Resolve the Kernel from the service provider.
```

The correct pattern is to register everything in a `ServiceCollection` and resolve `Kernel` from the built provider:

```csharp
services.AddSingleton<IChatClient>(sp =>
{
    var loggerFactory = sp.GetRequiredService<ILoggerFactory>();
    return new ChatClientBuilder(LLamaSharpChatClientFactory(config))
        .UseLogging(loggerFactory)
        .UseFunctionInvocation()
        .Build();
});

services.AddKernel();

var provider = services.BuildServiceProvider();
var kernel   = provider.GetRequiredService<Kernel>();
```

Registering `IChatClient` as a factory lambda solves the dependency ordering problem: `loggerFactory` is available from the container when the factory runs, so there is no chicken-and-egg issue.
:::lang

:::lang hu
## Amit a dokumentáció nem mond el

A kiindulópontul szolgáló modul leírás hivatkozott egy `LLamaSharpChatClient` nevű osztályra. Ilyen nem létezik.

A `LLamaSharp.SemanticKernel`-ben lévő tényleges osztály a `LLamaSharpChatCompletion`, amely az SK `IChatCompletionService`-ét implementálja — nem az M.E.AI `IChatClient`-jét. A kettő közötti bridge a `.AsChatClient()` hívás:

```csharp
var llamaChat = new LLamaSharpChatCompletion(executor);
IChatClient chatClient = llamaChat.AsChatClient();
```

Mielőtt az `executor`-t létrehoznád, fel kell építeni a teljes LlamaSharp objektumláncot. Ez szintén hiányzott az eredeti leírásból:

```csharp
var modelParams = new ModelParams(modelPath)
{
    ContextSize = contextSize,
    GpuLayerCount = gpuLayers
};

using var model   = LLamaWeights.LoadFromFile(modelParams);
using var context = model.CreateContext(modelParams);
var executor      = new InteractiveExecutor(context);
```

A `model` és `context` objektumok `IDisposable`-ek. DI factory-ban soha nem kerülnek dispose-ra — ezt production kódban hosted service wrapperrel kell kezelni.

A második probléma a Semantic Kernel regisztrációjával volt. A modul kódja `.Build()`-et hívott az `AddKernel()` visszatérési értékén. Ez kivételt dob:

```
System.InvalidOperationException: Build is not permitted on instances
returned from AddKernel. Resolve the Kernel from the service provider.
```

A helyes pattern: mindent `ServiceCollection`-ben regisztrálni, és a `Kernel`-t a felépített providerből feloldani:

```csharp
services.AddSingleton<IChatClient>(sp =>
{
    var loggerFactory = sp.GetRequiredService<ILoggerFactory>();
    return new ChatClientBuilder(LLamaSharpChatClientFactory(config))
        .UseLogging(loggerFactory)
        .UseFunctionInvocation()
        .Build();
});

services.AddKernel();

var provider = services.BuildServiceProvider();
var kernel   = provider.GetRequiredService<Kernel>();
```

Az `IChatClient` lambda factory-ként való regisztrálása megoldja a függőségi sorrend problémát: a `loggerFactory` elérhető a konténerből, amikor a factory fut, tehát nincs chicken-and-egg probléma.
:::lang

:::section

:::section middleware-pipeline

:::lang en
## The middleware pipeline

One of the most useful things M.E.AI provides is a composable middleware pipeline for `IChatClient`. It works exactly like ASP.NET Core middleware — layers wrap layers, running from outside in:

```csharp
IChatClient chatClient = new ChatClientBuilder(innerClient)
    .UseLogging(loggerFactory)   // logs every request and response
    .UseFunctionInvocation()     // executes tool calls automatically
    .Build();
```

This matters for two reasons. First, cross-cutting concerns — logging, telemetry, retry — are declared once and apply to every LLM call automatically. Second, `.UseFunctionInvocation()` is where automatic tool execution lives. When the model decides to call a tool, the middleware intercepts the response, invokes the function, and sends the result back — without any code in your business logic.

The warning that comes with this: automatic function invocation means the model can trigger side effects without explicit approval. That is fine in development. In production, it is the reason Module 7 (HITL, guardrails) exists.
:::lang

:::lang hu
## A middleware pipeline

Az egyik leghasznosabb dolog, amit az M.E.AI nyújt, egy kompozálható middleware pipeline az `IChatClient`-hez. Pontosan úgy működik mint az ASP.NET Core middleware — rétegek wrap-elik egymást, kívülről befelé futnak:

```csharp
IChatClient chatClient = new ChatClientBuilder(innerClient)
    .UseLogging(loggerFactory)   // logol minden kérést és választ
    .UseFunctionInvocation()     // automatikusan végrehajtja a tool hívásokat
    .Build();
```

Ez két okból fontos. Először, a cross-cutting concern-ök — logging, telemetria, retry — egyszer kerülnek deklarálásra, és automatikusan érvényesek minden LLM hívásra. Másodszor, a `.UseFunctionInvocation()` itt él az automatikus tool végrehajtás. Amikor a modell úgy dönt, hogy tool-t hív, a middleware lehallgatja a választ, meghívja a függvényt, és visszaküldi az eredményt — anélkül, hogy bármi kód lenne a business logikádban.

A figyelmeztetés, ami ezzel jár: az automatikus function invocation azt jelenti, hogy a modell jóváhagyás nélkül tud mellékhatásokat kiváltani. Ez fejlesztés közben rendben van. Production-ban ez az oka annak, hogy létezik a 7. modul (HITL, guardrails).
:::lang

:::section

:::section the-final-code

:::lang en
## The conversation loop

The mini challenge in Module 1 was building a multi-turn conversation with memory. The key insight: there is no magic. Memory is a `List<ChatMessage>` that you pass to every call. The model sees the entire history each time. That is all short-term memory is.

```csharp
var chatClient = provider.GetRequiredService<IChatClient>();

var messages = new List<ChatMessage>
{
    new(ChatRole.System, "You are a helpful .NET expert assistant.")
};

using var cts = new CancellationTokenSource();
Console.CancelKeyPress += (_, e) => { e.Cancel = true; cts.Cancel(); };

while (!cts.Token.IsCancellationRequested)
{
    Console.Write("Next message: ");
    var input = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(input)) continue;

    messages.Add(new ChatMessage(ChatRole.User, input));

    var result = await chatClient.GetResponseAsync(
        messages, cancellationToken: cts.Token);

    messages.Add(new ChatMessage(ChatRole.Assistant, result.Text));
    Console.WriteLine($"\nAnswer: {result.Text}\n");
}
```

Notice that `IChatClient` is resolved directly from the provider — not through the kernel. The kernel is registered and available, but at this stage, it is not doing anything yet. The agent capabilities come in Module 2 when plugins enter the picture.

The choice to use `IChatClient` with `List<ChatMessage>` rather than SK's `IChatCompletionService` with `ChatHistory` was deliberate. The middleware pipeline runs on the `IChatClient`. Keeping everything at the M.E.AI layer in Module 1 means the logging and function invocation middleware is active, without needing to wire up an additional SK bridge.
:::lang

:::lang hu
## A conversation loop

A Modul 1 mini kihívása egy többkörös, memóriával rendelkező párbeszéd megépítése volt. A kulcsfontosságú felismerés: nincs varázslat. A memória egy `List<ChatMessage>`, amelyet minden hívásnak átadsz. A modell minden alkalommal látja a teljes előzményt. Ez minden, ami a short-term memory.

```csharp
var chatClient = provider.GetRequiredService<IChatClient>();

var messages = new List<ChatMessage>
{
    new(ChatRole.System, "You are a helpful .NET expert assistant.")
};

using var cts = new CancellationTokenSource();
Console.CancelKeyPress += (_, e) => { e.Cancel = true; cts.Cancel(); };

while (!cts.Token.IsCancellationRequested)
{
    Console.Write("Next message: ");
    var input = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(input)) continue;

    messages.Add(new ChatMessage(ChatRole.User, input));

    var result = await chatClient.GetResponseAsync(
        messages, cancellationToken: cts.Token);

    messages.Add(new ChatMessage(ChatRole.Assistant, result.Text));
    Console.WriteLine($"\nAnswer: {result.Text}\n");
}
```

Figyeld meg, hogy az `IChatClient` közvetlenül a providerből kerül feloldásra — nem a kernelen keresztül. A kernel regisztrálva és elérhető, de ebben a szakaszban még nem csinál semmit. Az agent képességek a 2. modulban jönnek, amikor a pluginek belépnek a képbe.

Az a döntés, hogy az `IChatClient`-et `List<ChatMessage>`-dzsel használom az SK `IChatCompletionService`-e és `ChatHistory`-ja helyett, szándékos volt. A middleware pipeline az `IChatClient`-en fut. Azzal, hogy mindent az M.E.AI rétegen tartunk a Modul 1-ben, a logging és a function invocation middleware aktív — anélkül, hogy egy additional SK bridge-et kellene bekötni.
:::lang

:::section

:::section what-i-learned

:::lang en
## What I actually learned

**The abstraction is worth the extra wiring.** Setting up `IChatClient` instead of instantiating `LlamaSharpChatCompletion` directly takes more code. But once it is done, the rest of the application genuinely does not know or care which provider is behind it. That is the point.

**Documentation describes the happy path.** The class names, method signatures, and code samples in module descriptions reflect an ideal. What breaks — `LLamaSharpChatClient` not existing, `AddKernel()` not supporting `.Build()`, `IChatCompletionService` not being auto-registered — only surfaces when you actually run the code. Implementation is the verification step.

**Short-term memory is just state management.** There is no hidden mechanism. Passing the full message list on every call is the entire implementation. The interesting problems — context window limits, what to keep and what to drop — come in Module 3.

**The middleware pipeline is where production concerns live.** Logging, telemetry, rate limiting, HITL checkpoints — all of these are middleware. The pattern is the same one ASP.NET Core developers already know. The only thing new is the domain.
:::lang

:::lang hu
## Amit tényleg megtanultam

**Az absztrakció megéri a plusz bekötést.** Az `IChatClient` felállítása a `LlamaSharpChatCompletion` közvetlen példányosítása helyett több kódot igényel. De ha egyszer kész, az alkalmazás többi része valóban nem tudja és nem is érdekli, hogy melyik provider áll mögötte. Ez a lényeg.

**A dokumentáció a happy path-et írja le.** Az osztálynevek, metódus-szignatúrák és kódminták a modul leírásokban egy ideált tükröznek. Ami eltörik — a `LLamaSharpChatClient` nem létezik, az `AddKernel()` nem támogatja a `.Build()`-et, az `IChatCompletionService` nincs automatikusan regisztrálva — csak akkor kerül felszínre, amikor ténylegesen futtatod a kódot. Az implementáció az ellenőrzési lépés.

**A short-term memory csak állapotkezelés.** Nincs rejtett mechanizmus. A teljes üzenetlista átadása minden hívásban maga az egész implementáció. Az érdekes problémák — context window korlátok, mit tartsunk meg és mit dobjunk el — a 3. modulban jönnek.

**A middleware pipeline az, ahol a production concern-ök élnek.** Logging, telemetria, rate limiting, HITL checkpointok — ezek mind middleware. A pattern ugyanaz, amelyet az ASP.NET Core fejlesztők már ismernek. Az egyetlen új dolog a domain.
:::lang

:::section

:::section whats-next

:::lang en
## What comes next

Module 1 set up the plumbing. The `IChatClient` is wired, the middleware pipeline is running, the DI container is in place. The `.UseFunctionInvocation()` middleware is already registered — it just has nothing to invoke yet.

Module 2 changes that. `[KernelFunction]` plugins, the difference between auto and manual tool loops, and production error handling for tool calls. That is where an LLM becomes an agent.
:::lang

:::lang hu
## Mi jön ezután

A Modul 1 felállította a vízvezetékeket. Az `IChatClient` be van kötve, a middleware pipeline fut, a DI konténer a helyén van. A `.UseFunctionInvocation()` middleware már regisztrálva van — csak még nincs mit meghívnia.

A 2. modul ezt változtatja meg. `[KernelFunction]` pluginek, az auto és manuális tool loop közötti különbség, és production hibakezelés a tool hívásokhoz. Ez az a pont, ahol egy LLM-ből agent lesz.
:::lang

:::section
