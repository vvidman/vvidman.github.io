# homelab-docker-server — Home Lab Docker Server and Deploy Workflow
Source: content/posts/homelab-docker-server.md (slug `posts/homelab-docker-server.html`, standalone: no series/part) · Pattern: compare + two flows + callout (square); compact flow + callout (og)

| Element | Text | Source |
|---|---|---|
| Eyebrow | Home Lab | title "Home Lab Docker Server and Deploy Workflow" (post has no series) |
| Title | Docker server and deploy workflow | title without the "Home Lab" prefix (moved to eyebrow) |
| Subtitle (square) | An old machine or WSL2 as a personal Docker host, with a deploy workflow that works identically on both. | description |
| Compare head 1 | Dedicated Linux machine | "Option A: Dedicated Linux Machine" |
| Compare 1 | At least 4 GB RAM and an SSD | "at least **4 GB RAM and an SSD** is workable" |
| Compare 1 | Recommended OS: Debian 13 | "**Recommended OS:** Debian 13" |
| Compare 1 | A dedicated, always-on host that doesn't compete with your development machine | "a dedicated, always-on host that doesn't compete with your development machine for resources" |
| Compare head 2 (key) | WSL2 on Windows | "Option B: WSL2 on Windows" |
| Compare 2 | Docker Engine runs natively inside WSL2 | "Docker Engine runs natively inside WSL2" |
| Compare 2 | Competes with Windows for RAM | "your Docker host competes with Windows for RAM" |
| Compare 2 | Goes down whenever your development machine goes down | "it goes down whenever your development machine goes down" |
| Label 1 | Developer machine | deploy workflow diagram column "Developer machine" |
| Flow 1 | docker build → docker save \| gzip → scp | diagram steps 1–2 and "→ scp →" |
| Label 2 | Host (Linux or WSL2) | diagram column "Host (Linux or WSL2)" |
| Flow 2 | docker load → Portainer stack deploy → Nginx Proxy Manager (optional) | diagram steps 3–5 |
| Callout (square) | Dedicated Linux machine or WSL2 — the commands are identical. | "works whether your host is a dedicated Linux machine or WSL2 — the commands are identical" |
| og flow | docker build → docker save → scp → docker load → Portainer | same steps; `| gzip` and "stack deploy" dropped for width |
| Callout (og) | The same workflow on a Linux machine and on WSL2. | "deploy to either of them using the exact same workflow" (intro) |
