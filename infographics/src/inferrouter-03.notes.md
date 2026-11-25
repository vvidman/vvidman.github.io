# inferrouter-03 — InferRouter Part 3: Secret Management
Source: content/posts/inferrouter-03.md · Pattern: 3 cards + flow + callout (square); 3 cards + callout (og)

| Element | Text | Source |
|---|---|---|
| Eyebrow | InferRouter · Part 3 | front matter |
| Title | Secret management | title |
| Subtitle (square) | Where API keys live — and why the key is read fresh on every request. | "why not environment variables, why not config files, and why the key is read fresh on every single request" (Next section) |
| Card Environment variables | Visible in docker inspect. A container restart is needed to rotate. | "It is visible in `docker inspect` output… requires a container restart to rotate." (Three Places a Key Could Live) |
| Card Config files | One careless git add away from being public. | "one careless `git add` away from being public" |
| Card Docker Secrets (key) | Mounted as files at /run/secrets/. Never in environment variables or config files. | "mounted as files at `/run/secrets/`… never appears in environment variables, Docker metadata, or any config file." — "The choice is Docker Secrets." |
| Flow 1 | **Every request** — SecretReader reads /run/secrets/ directly. | "On every `CompleteAsync` call… it reads from `/run/secrets/` directly." (Per-Request Key Reading) |
| Flow 2 (key) | **Key lives on the stack** — Never stored in a field. | "The key is never stored in a field. It exists only on the stack frame of the request" |
| Flow 3 | **Discarded** — When the call completes. | "discarded when the call completes" |
| Callout (square) | Overwrite the file on the host, and the updated key is read without restarting the container. | same sentence |
| Callout (og) | The key is read fresh on every request. | Next section: "the key is read fresh on every single request" |
