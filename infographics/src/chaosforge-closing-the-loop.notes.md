# chaosforge-closing-the-loop — Closing the Loop
Source: content/posts/chaosforge-02-closing-the-loop.md (slug `posts/chaosforge-closing-the-loop.html`, series ChaosForge, part 2) · Pattern: flow + 3 cards + callout (square); compact flow + callout (og)

| Element | Text | Source |
|---|---|---|
| Eyebrow | ChaosForge · Part 2 | front matter |
| Title | Closing the loop | title |
| Subtitle (square) | Closing the structural gap between planning and implementation. | description: "closing the structural gap between planning and implementation" |
| Subtitle (og) | Closing the structural gap between planning and implementation. | same as square subtitle (description) |
| Node 1 | **Reasoning partner** — Plans and writes the spec. | "the AI used for planning, architecture decisions, and design iteration"; full-loop diagram "write spec → docs/specs/feature-name.md" |
| Node 2 | **Implementation agent** — Implements, then runs /review-ready. | "The AI used to turn plans into code"; diagram "implement └─► /review-ready" |
| Node 3 (key) | **Review file + progress** — docs/review/ and docs/progress/ updated. | diagram "docs/review/feature-name-review.md" and "docs/progress/ updated" |
| Node 4 | **Reasoning partner, new session** — Reads the progress index and review file. | diagram: "reads docs/progress/index.md first", "reads review file as context" |
| Card The review file | The only file dropped into the reasoning partner's context for a review session. | Addition 1: "This is the *only* thing dropped into the reasoning partner's context for a review session." |
| Card docs/progress/ | Ground truth of what's been built. | Addition 2 heading: "`docs/progress/` — ground truth of what's been built" |
| Card Proposed doc updates | Every review file lists which living documents need updating. The human decides what gets incorporated. | Addition 3: "Every review file includes an explicit section listing which living documents need updating… The human decides what gets incorporated." |
| Callout (square) | Every arrow is explicit and documented. Nothing is carried in anyone's head. | "Every arrow is explicit and documented. Nothing is carried in anyone's head." |
| Callout (og) | Where the loop breaks: after implementation, the reasoning partner has no idea what actually happened. | "The return direction is where the loop breaks. After implementation, the reasoning partner has no idea what actually happened." |
