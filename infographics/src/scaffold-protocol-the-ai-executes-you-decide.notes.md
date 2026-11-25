# scaffold-protocol-the-ai-executes-you-decide — The AI Executes. You Decide.
Source: content/posts/post_scaffoldProtocol.md (slug `posts/scaffold-protocol-the-ai-executes-you-decide.html`, series Scaffold Protocol, part 1) · Pattern: flow + compare + callout (square); compact flow + callout (og)

| Element | Text | Source |
|---|---|---|
| Eyebrow | Scaffold Protocol · Part 1 | front matter |
| Title | The AI executes. You decide. | title |
| Subtitle (square) | A human orchestrator who decides what happens next, and an AI transformation engine that executes exactly one task at a time. | "two distinct roles: a **human orchestrator** who decides what happens next, and an **AI transformation engine** that executes exactly one task at a time." |
| Node 1 | **Human orchestrator** — Decides what happens next. | same sentence; "That's my job." |
| Node 2 (key) | **AI step** — Given a defined input, produces a defined output in a specific format. | "The AI has one job: given a defined input, produce a defined output in a specific format." |
| Node 3 | **Validator** — Finds the certainly wrong outputs. | "The validator's job is not to find the perfect output. Its job is to find the certainly wrong ones." |
| Node 4 | **Human decision** — Validator passes: the output might be good, the human decides. | "Validator returns pass → the output *might* be good. Human decides." |
| Compare head 1 | Retry | "**Retry** means…" |
| Compare 1 | Sends the same request again and hopes for a better output. | "sending the same request again and hoping for a better output" |
| Compare 1 | It gives the model no information about what went wrong. | "it gives the model no information about what went wrong" |
| Compare head 2 (key) | Error-driven refinement | "Error-Driven Refinement" |
| Compare 2 | The specific violation is mapped to a targeted error message. | "The specific violation is extracted from the validator result, mapped to a targeted error message" |
| Compare 2 | The message is injected into a new prompt that says exactly what to fix. | "injected into a new prompt that tells the model exactly what to fix" |
| Callout (square) | Run 5: accepted after one automatic refinement. "I'd do it differently, but I can't fault it." | Runs table: Run 5, STOP_TOKEN_LEAKED, "Auto × 1", "**Accept**"; "My note at the time: *"I'd do it differently, but I can't fault it."*" |
| Callout (og) | Validator fails: certainly wrong. Validator passes: the human decides. | "Validator returns fail → the output is certainly wrong. Automated rejection. / Validator returns pass → … Human decides." |
