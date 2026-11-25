# dotnet-ai-stack-01 — Building the .NET AI Stack from First Principles
Source: content/posts/dotnet-ai-stack-01.md (series .NET Agentic AI, part 1) · Pattern: layer stack + 3 cards + callout (square); 3 cards + callout (og)

| Element | Text | Source |
|---|---|---|
| Eyebrow | .NET Agentic AI · Part 1 | front matter |
| Title | Building the .NET AI stack from first principles | title |
| Subtitle (square) | Wiring together LlamaSharp, Microsoft.Extensions.AI and Semantic Kernel — and why the documentation does not tell the whole story. | description |
| Layer 1 | Your code — business logic, agent workflows | stack diagram (Why three layers?) |
| Layer 2 (key) | Semantic Kernel — What should the agent do? | diagram: "SEMANTIC KERNEL "What should the agent do?"" |
| Layer 3 (key) | Microsoft.Extensions.AI — How should it talk to any LLM? | diagram: "MICROSOFT.EXTENSIONS.AI "How should it talk to any LLM?"" |
| Layer 4 | LLM provider (swappable) — LlamaSharp · Azure OpenAI · OpenAI · Ollama | diagram |
| Card LLamaSharpChatClient | Does not exist. The class is LLamaSharpChatCompletion, bridged with .AsChatClient(). | "referenced a class called `LLamaSharpChatClient`. It does not exist."; "To bridge between them, you call `.AsChatClient()`" |
| Card AddKernel().Build() | Throws InvalidOperationException. Resolve Kernel from the service provider. | "The module code called `.Build()` on the result of `AddKernel()`. That throws… Resolve the Kernel from the service provider." |
| Card Short-term memory | A List<ChatMessage> passed to every call. That is all. | "Memory is a `List<ChatMessage>` that you pass to every call… That is all short-term memory is." |
| Callout (square, og) | Implementation is the verification step. | "Implementation is the verification step." (What I actually learned) |
