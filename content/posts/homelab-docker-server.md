---
title: Home Lab Docker Server and Deploy Workflow
slug: posts/homelab-docker-server.html
description: How to turn an old machine or WSL2 into a personal Docker host, and set up a deploy workflow that works identically on both.
layout: page
---

:::section intro

:::lang en
# Home Lab Docker Server and Deploy Workflow

So you want a personal Docker host. Somewhere to run side projects, self-hosted services, or AI experiments without paying for cloud infrastructure. You have two realistic options: an old PC or laptop running Linux, or WSL2 on your Windows machine. This post covers both — and more importantly, how to deploy to either of them using the exact same workflow.

> **Companion post:** If you run into boot problems during Linux installation on older hardware, see [The Legacy BIOS + GPT Trap](/posts/homelab-legacy-bios.html).
:::lang

:::lang hu
# Otthoni Docker szerver és deploy workflow

Egy személyes Docker hoszt — hely ahol mellékprojekteket, self-hosted szolgáltatásokat vagy AI kísérleteket futtathatsz anélkül, hogy felhőinfrastruktúráért fizetnél. Két reális opció van: egy régi gép Linuxszal, vagy WSL2 Windows alatt. Ez a post mindkettőt lefedi — és ami fontosabb, egy egységes deploy workflow-t mutat be, ami mindkét esetben ugyanúgy működik.

> **Kapcsolódó post:** Ha régi hardverre telepítés közben bootolási problémákba ütközöl, lásd: [A Legacy BIOS + GPT csapda](/posts/homelab-legacy-bios.html).
:::lang

:::section

:::section choosing-host

:::lang en
## Choosing Your Host

### Option A: Dedicated Linux Machine

Any machine from the last 10–15 years with at least **4 GB RAM and an SSD** is workable. The more RAM, the more services you can run simultaneously.

Good candidates: an old laptop, a retired office desktop, or a mini PC (Intel NUC, Beelink, etc.).

The advantage is a dedicated, always-on host that doesn't compete with your development machine for resources. The disadvantage is setup time — particularly on machines old enough to have Legacy BIOS quirks.

**Recommended OS:** Debian 13 — stable, minimal, excellent Docker support, and the installer handles older hardware well.

### Option B: WSL2 on Windows

If you don't have a spare machine, WSL2 is a legitimate alternative. Docker Engine runs natively inside WSL2, SSH access works, and the deploy workflow described in this post is identical — same commands, same tools, same result.

The trade-offs: your Docker host competes with Windows for RAM, and it goes down whenever your development machine goes down. For personal projects and learning, this is usually acceptable.
:::lang

:::lang hu
## A hoszt kiválasztása

### A opció: Dedikált Linux gép

Bármely 10–15 évnél nem régebbi gép, amelyen van legalább **4 GB RAM és SSD**, megfelel. Minél több RAM, annál több szolgáltatást lehet egyszerre futtatni.

Jó jelöltek: egy fiókban porosodó régi laptop, nyugdíjazott irodai asztali gép, vagy mini PC (Intel NUC, Beelink, stb.).

Az előnye: dedikált, mindig bekapcsolt hoszt, amely nem verseng a fejlesztői géppel az erőforrásokért. A hátránya a telepítési idő — különösen régi, Legacy BIOS-os gépeken.

**Ajánlott OS:** Debian 13 — stabil, minimális, kiváló Docker-támogatás, és a telepítő jól kezeli a régebbi hardvert.

### B opció: WSL2 Windows alatt

Ha nincs tartalék gép, a WSL2 valódi alternatíva. A Docker Engine natívan fut WSL2 alatt, az SSH-hozzáférés működik, és a post-ban leírt deploy workflow azonos — ugyanazok a parancsok, ugyanazok az eszközök, ugyanaz az eredmény.

A kompromisszumok: a Docker hoszt verseng a Windowsszal a RAM-ért, és leáll, ha a fejlesztői gép leáll. Személyes projektekhez és tanuláshoz ez általában elfogadható.
:::lang

:::section

:::section setup-linux

:::lang en
## Setting Up the Linux Host (Debian 13)

**Install the OS:** Download the Debian 13 netinstall ISO from [debian.org](https://debian.org), create a bootable USB with [Rufus](https://rufus.ie), and install with the text installer. At partitioning choose guided — entire disk, all files in one partition. At software selection: SSH server + Standard system utilities only — no desktop environment.

**First boot — install sudo:**

```bash
su -
apt install sudo -y
usermod -aG sudo <your-username>
exit
```

Log out and back in. `sudo` now works.

**Set a static IP.** Find your interface name with `ip a`, then edit `/etc/network/interfaces`:

```
auto enp0s25
iface enp0s25 inet static
    address 192.168.x.x
    netmask 255.255.255.0
    gateway 192.168.x.1
    dns-nameservers 8.8.8.8 1.1.1.1
```

Apply with `sudo systemctl restart networking`. Replace `enp0s25` with your actual interface name. Your gateway IP is visible on Windows via `ipconfig`.

**Firewall:**

```bash
sudo apt install ufw -y
sudo ufw allow OpenSSH
sudo ufw allow 9443/tcp   # Portainer
sudo ufw allow 80/tcp     # Nginx Proxy Manager
sudo ufw allow 81/tcp     # NPM admin
sudo ufw allow 443/tcp    # HTTPS
sudo ufw enable
```

**Timezone:** `sudo timedatectl set-timezone Europe/Budapest`
:::lang

:::lang hu
## A Linux hoszt beállítása (Debian 13)

**OS telepítése:** Töltsd le a Debian 13 netinstall ISO-t a [debian.org](https://debian.org) oldalról, készíts bootolható USB-t [Rufus](https://rufus.ie) segítségével, és telepíts a szöveges telepítővel. Particionálásnál válaszd a guided — teljes lemez, minden fájl egy partíción opciót. Szoftverválasztásnál: csak SSH server + Standard system utilities — semmilyen asztali környezet.

**Első indítás — sudo telepítése:**

```bash
su -
apt install sudo -y
usermod -aG sudo <felhasználónév>
exit
```

Jelentkezz ki és vissza. A `sudo` mostantól működik.

**Statikus IP beállítása.** Az interfész nevét az `ip a` paranccsal kérdezd le, majd szerkeszd az `/etc/network/interfaces` fájlt:

```
auto enp0s25
iface enp0s25 inet static
    address 192.168.x.x
    netmask 255.255.255.0
    gateway 192.168.x.1
    dns-nameservers 8.8.8.8 1.1.1.1
```

Alkalmazd: `sudo systemctl restart networking`. Az `enp0s25` helyére a saját interfészneved kerül. A gateway IP Windows alatt az `ipconfig` kimenetéből olvasható le.

**Tűzfal:**

```bash
sudo apt install ufw -y
sudo ufw allow OpenSSH
sudo ufw allow 9443/tcp   # Portainer
sudo ufw allow 80/tcp     # Nginx Proxy Manager
sudo ufw allow 81/tcp     # NPM admin
sudo ufw allow 443/tcp    # HTTPS
sudo ufw enable
```

**Időzóna:** `sudo timedatectl set-timezone Europe/Budapest`
:::lang

:::section

:::section setup-wsl2

:::lang en
## Setting Up WSL2

**Enable WSL2** from an Administrator PowerShell prompt:

```powershell
wsl --install
```

Restart when prompted. Ubuntu installs by default — that's fine.

**Static IP (optional):** WSL2 gets a dynamic IP by default. For a stable address, add this to `%USERPROFILE%\.wslconfig`:

```ini
[wsl2]
networkingMode=mirrored
```

This makes WSL2 share your Windows IP.

**Enable SSH access inside WSL2:**

```bash
sudo apt update && sudo apt install openssh-server -y
sudo service ssh start
```

To start SSH automatically, add `sudo service ssh start` to your `~/.bashrc`.
:::lang

:::lang hu
## WSL2 beállítása

**WSL2 engedélyezése** rendszergazdai PowerShell ablakból:

```powershell
wsl --install
```

Indítsd újra a gépet. Alapértelmezetten Ubuntu települ — ez megfelelő.

**Statikus IP (opcionális):** A WSL2 alapból dinamikus IP-t kap. Stabil cím érdekében add hozzá a `%USERPROFILE%\.wslconfig` fájlhoz:

```ini
[wsl2]
networkingMode=mirrored
```

Így a WSL2 a Windows IP-jét osztja meg.

**SSH-hozzáférés engedélyezése WSL2-n belül:**

```bash
sudo apt update && sudo apt install openssh-server -y
sudo service ssh start
```

Az automatikus indításhoz add a `sudo service ssh start` sort a `~/.bashrc` fájlhoz.
:::lang

:::section

:::section docker-install

:::lang en
## Installing Docker

The official Docker installation method works identically on Debian and Ubuntu/WSL2.

```bash
sudo apt-get install -y ca-certificates curl
sudo install -m 0755 -d /etc/apt/keyrings
sudo curl -fsSL https://download.docker.com/linux/debian/gpg \
  -o /etc/apt/keyrings/docker.asc
sudo chmod a+r /etc/apt/keyrings/docker.asc

echo \
  "deb [arch=$(dpkg --print-architecture) signed-by=/etc/apt/keyrings/docker.asc] \
  https://download.docker.com/linux/debian \
  $(. /etc/os-release && echo "$VERSION_CODENAME") stable" | \
  sudo tee /etc/apt/sources.list.d/docker.list > /dev/null

sudo apt-get update
sudo apt-get install -y docker-ce docker-ce-cli containerd.io \
  docker-buildx-plugin docker-compose-plugin
```

For Ubuntu/WSL2, replace `debian` with `ubuntu` in the repository URL and gpg path.

**Post-install — run Docker without sudo:**

```bash
sudo usermod -aG docker $USER
```

Log out and back in, then verify: `docker run hello-world`

On a dedicated Linux machine, enable Docker to start automatically:

```bash
sudo systemctl enable docker
sudo systemctl enable containerd
```
:::lang

:::lang hu
## Docker telepítése

A hivatalos Docker telepítési módszer Debianon és Ubuntu/WSL2-n azonos.

```bash
sudo apt-get install -y ca-certificates curl
sudo install -m 0755 -d /etc/apt/keyrings
sudo curl -fsSL https://download.docker.com/linux/debian/gpg \
  -o /etc/apt/keyrings/docker.asc
sudo chmod a+r /etc/apt/keyrings/docker.asc

echo \
  "deb [arch=$(dpkg --print-architecture) signed-by=/etc/apt/keyrings/docker.asc] \
  https://download.docker.com/linux/debian \
  $(. /etc/os-release && echo "$VERSION_CODENAME") stable" | \
  sudo tee /etc/apt/sources.list.d/docker.list > /dev/null

sudo apt-get update
sudo apt-get install -y docker-ce docker-ce-cli containerd.io \
  docker-buildx-plugin docker-compose-plugin
```

Ubuntu/WSL2 esetén a repository URL-ben és a gpg útvonalon a `debian` helyére `ubuntu` kerül.

**Telepítés utáni lépés — Docker futtatása sudo nélkül:**

```bash
sudo usermod -aG docker $USER
```

Jelentkezz ki és vissza, majd ellenőrizd: `docker run hello-world`

Dedikált Linux gépen engedélyezd az automatikus indítást:

```bash
sudo systemctl enable docker
sudo systemctl enable containerd
```
:::lang

:::section

:::section portainer-npm

:::lang en
## Portainer and Nginx Proxy Manager

### Portainer CE

```bash
docker volume create portainer_data

docker run -d \
  -p 8000:8000 \
  -p 9443:9443 \
  --name portainer \
  --restart=always \
  -v /var/run/docker.sock:/var/run/docker.sock \
  -v portainer_data:/data \
  portainer/portainer-ce:latest
```

Access at `https://<host-ip>:9443`. Accept the self-signed certificate warning — it's expected. Create an admin user and select **Get Started → local**.

### Nginx Proxy Manager

Create `~/stacks/npm/docker-compose.yml`:

```yaml
services:
  app:
    image: jc21/nginx-proxy-manager:latest
    restart: unless-stopped
    ports:
      - '80:80'
      - '81:81'
      - '443:443'
    volumes:
      - ./data:/data
      - ./letsencrypt:/etc/letsencrypt
```

Then: `docker compose up -d`

Access the admin UI at `http://<host-ip>:81`. Default credentials: `admin@example.com` / `changeme` — change them on first login.
:::lang

:::lang hu
## Portainer és Nginx Proxy Manager

### Portainer CE

```bash
docker volume create portainer_data

docker run -d \
  -p 8000:8000 \
  -p 9443:9443 \
  --name portainer \
  --restart=always \
  -v /var/run/docker.sock:/var/run/docker.sock \
  -v portainer_data:/data \
  portainer/portainer-ce:latest
```

Elérhető: `https://<host-ip>:9443`. Az önaláírt tanúsítványra vonatkozó figyelmeztetést el kell fogadni — ez várható. Hozz létre admin felhasználót, és válaszd a **Get Started → local** opciót.

### Nginx Proxy Manager

Hozd létre a `~/stacks/npm/docker-compose.yml` fájlt:

```yaml
services:
  app:
    image: jc21/nginx-proxy-manager:latest
    restart: unless-stopped
    ports:
      - '80:80'
      - '81:81'
      - '443:443'
    volumes:
      - ./data:/data
      - ./letsencrypt:/etc/letsencrypt
```

Majd: `docker compose up -d`

Az admin felület: `http://<host-ip>:81`. Alapértelmezett belépési adatok: `admin@example.com` / `changeme` — azonnal változtasd meg az első bejelentkezéskor.
:::lang

:::section

:::section deploy-workflow

:::lang en
## The Deploy Workflow

This is the core of the post. The workflow below works whether your host is a dedicated Linux machine or WSL2 — the commands are identical.

```
Developer machine                     Host (Linux or WSL2)
────────────────────────────          ──────────────────────────────
1. docker build
2. docker save | gzip        → scp →
                                       3. docker load
                                       4. Portainer stack deploy
                                       5. Nginx Proxy Manager (optional)
```

**Step 1 — Build the image:**

```bash
docker build -t <project>:1.0.0 -t <project>:latest .
```

**Step 2 — Export:**

```bash
docker save <project>:latest | gzip > <project>.tar.gz
```

**Step 3 — Upload (one-time directory setup):**

```bash
ssh user@<host-ip> "mkdir -p ~/images"
scp <project>.tar.gz user@<host-ip>:~/images/
```

**Step 4 — Load on the host:**

```bash
ssh user@<host-ip> "docker load < ~/images/<project>.tar.gz"
```

**Step 5 — Deploy via Portainer:** On first deploy, go to Portainer → Stacks → Add stack, paste your `docker-compose.yml`, set environment variables, and click Deploy. For updates: select the stack and click Update the stack. Portainer picks up the newly loaded `latest` image automatically.

**Optional — expose via Nginx Proxy Manager:** Add a Proxy Host with the service name as forward hostname and the container's internal port. Add the host IP to your `hosts` file on the development machine to reach it by local domain name.
:::lang

:::lang hu
## A deploy workflow

Ez a post lényege. Az alábbi workflow működik, akár dedikált Linux gép, akár WSL2 a hoszt — a parancsok azonosak.

```
Fejlesztői gép                        Hoszt (Linux vagy WSL2)
────────────────────────────          ──────────────────────────────
1. docker build
2. docker save | gzip        → scp →
                                       3. docker load
                                       4. Portainer stack deploy
                                       5. Nginx Proxy Manager (opcionális)
```

**1. lépés — Image build:**

```bash
docker build -t <projekt>:1.0.0 -t <projekt>:latest .
```

**2. lépés — Export:**

```bash
docker save <projekt>:latest | gzip > <projekt>.tar.gz
```

**3. lépés — Feltöltés (könyvtár egyszeri létrehozása):**

```bash
ssh user@<host-ip> "mkdir -p ~/images"
scp <projekt>.tar.gz user@<host-ip>:~/images/
```

**4. lépés — Betöltés a hoszton:**

```bash
ssh user@<host-ip> "docker load < ~/images/<projekt>.tar.gz"
```

**5. lépés — Deploy Portainerrel:** Első deploy esetén: Portainer → Stacks → Add stack, illeszd be a `docker-compose.yml` tartalmát, állítsd be a környezeti változókat, és kattints a Deploy gombra. Frissítésnél: válaszd ki a stacket, és kattints az Update the stack gombra. A Portainer automatikusan felveszi az újonnan betöltött `latest` image-t.

**Opcionálisan — Nginx Proxy Manageren keresztül:** Adj hozzá egy Proxy Hostot a service névvel mint forward hostname és a konténer belső portjával. Add hozzá a hoszt IP-jét a fejlesztői gép `hosts` fájljához, hogy lokális domain névvel érhesd el.
:::lang

:::section

:::section deploy-script

:::lang en
## Deploy Script

Put this `deploy.sh` in the root of each project repository:

```bash
#!/bin/bash
set -e

PROJECT="<project-name>"
VERSION="${1:-latest}"
SERVER="user@<host-ip>"
REMOTE_DIR="~/images"

echo "Building $PROJECT:$VERSION..."
docker build -t "$PROJECT:$VERSION" -t "$PROJECT:latest" .

echo "Exporting image..."
docker save "$PROJECT:latest" | gzip > "$PROJECT.tar.gz"

echo "Uploading to $SERVER..."
scp "$PROJECT.tar.gz" "$SERVER:$REMOTE_DIR/"

echo "Loading on server..."
ssh "$SERVER" "docker load < $REMOTE_DIR/$PROJECT.tar.gz"

echo "Cleaning up local tar..."
rm "$PROJECT.tar.gz"

echo "Done. Update the stack in Portainer: https://<host-ip>:9443"
```

Usage:

```bash
chmod +x deploy.sh
./deploy.sh           # deploys latest
./deploy.sh 1.2.0     # deploys with version tag
```
:::lang

:::lang hu
## Deploy szkript

Helyezd el ezt a `deploy.sh` fájlt minden projekt repository gyökérkönyvtárában:

```bash
#!/bin/bash
set -e

PROJECT="<projekt-neve>"
VERSION="${1:-latest}"
SERVER="user@<host-ip>"
REMOTE_DIR="~/images"

echo "Building $PROJECT:$VERSION..."
docker build -t "$PROJECT:$VERSION" -t "$PROJECT:latest" .

echo "Exporting image..."
docker save "$PROJECT:latest" | gzip > "$PROJECT.tar.gz"

echo "Uploading to $SERVER..."
scp "$PROJECT.tar.gz" "$SERVER:$REMOTE_DIR/"

echo "Loading on server..."
ssh "$SERVER" "docker load < $REMOTE_DIR/$PROJECT.tar.gz"

echo "Cleaning up local tar..."
rm "$PROJECT.tar.gz"

echo "Done. Update the stack in Portainer: https://<host-ip>:9443"
```

Használat:

```bash
chmod +x deploy.sh
./deploy.sh           # latest deploy
./deploy.sh 1.2.0     # verziócímkével
```
:::lang

:::section

:::section secrets-volumes

:::lang en
## Secrets and Volumes

### Secrets

For any project using API keys or credentials, create secret files on the host before the first deploy:

```bash
ssh user@<host-ip>
mkdir -p ~/secrets/<project-name>
echo "your-actual-api-key" > ~/secrets/<project-name>/api_key.txt
chmod 600 ~/secrets/<project-name>/*.txt
```

Reference them in `docker-compose.yml` by file path. Never commit the secrets directory — keep a `secrets.example/` folder in the repo with placeholder filenames only.

### Volumes

**Named volumes** (databases, persistent data) — Docker manages these, visible in Portainer:

```yaml
volumes:
  db_data:
    name: myproject_db_data
```

**Bind mounts** (models, uploads, logs) — create the directory on the host first:

```bash
ssh user@<host-ip> "mkdir -p /opt/<project-name>/models"
```

Then reference in compose:

```yaml
volumes:
  - type: bind
    source: /opt/<project-name>/models
    target: /models
    read_only: true
```
:::lang

:::lang hu
## Titkok és volume-ok

### Titkok

Minden API kulcsot vagy bejelentkezési adatot igénylő projektnél az első deploy előtt hozd létre a titkos fájlokat a hoszton:

```bash
ssh user@<host-ip>
mkdir -p ~/secrets/<projekt-neve>
echo "valódi-api-kulcs" > ~/secrets/<projekt-neve>/api_key.txt
chmod 600 ~/secrets/<projekt-neve>/*.txt
```

A `docker-compose.yml`-ben fájlútvonallal hivatkozz rájuk. A secrets könyvtárat soha ne commitold — a repo-ban csak egy `secrets.example/` mappa legyen, placeholder fájlnevekkel.

### Volume-ok

**Named volume-ok** (adatbázisok, perzisztens adatok) — a Docker kezeli ezeket, a Portainerben láthatóak:

```yaml
volumes:
  db_data:
    name: myproject_db_data
```

**Bind mountok** (modellek, feltöltések, logok) — előbb hozd létre a könyvtárat a hoszton:

```bash
ssh user@<host-ip> "mkdir -p /opt/<projekt-neve>/models"
```

Majd hivatkozz rá a compose fájlban:

```yaml
volumes:
  - type: bind
    source: /opt/<projekt-neve>/models
    target: /models
    read_only: true
```
:::lang

:::section

:::section checklists

:::lang en
## Checklists

### First Deploy

- `~/images/` directory exists on host
- Required directories under `/opt/<project>/` created
- Secrets files created under `~/secrets/<project>/`
- Image built and uploaded
- Image loaded: `docker load`
- Portainer stack created and deployed
- Service reachable on its internal port
- NPM proxy host configured (if needed)
- Smoke test done

### Update

- Image built and uploaded
- Image loaded: `docker load`
- Portainer → stack → **Update the stack**
- Smoke test done

### Useful Commands on the Host

```bash
docker ps                          # running containers
docker ps -a                       # all containers
docker images                      # image list
docker logs <container-name> -f    # follow container logs
docker image prune                 # remove unused images
docker system df                   # disk usage summary
```
:::lang

:::lang hu
## Ellenőrzőlisták

### Első deploy

- A `~/images/` könyvtár létezik a hoszton
- A szükséges könyvtárak létrehozva `/opt/<projekt>/` alatt
- Titkos fájlok létrehozva `~/secrets/<projekt>/` alatt
- Image build és feltöltve
- Image betöltve: `docker load`
- Portainer stack létrehozva és deployolva
- A szolgáltatás elérhető a belső portján
- NPM proxy host beállítva (ha szükséges)
- Smoke teszt elvégezve

### Frissítés

- Image build és feltöltve
- Image betöltve: `docker load`
- Portainer → stack → **Update the stack**
- Smoke teszt elvégezve

### Hasznos parancsok a hoszton

```bash
docker ps                          # futó konténerek
docker ps -a                       # összes konténer
docker images                      # image lista
docker logs <konténer-neve> -f     # konténer logok követése
docker image prune                 # nem használt image-ek törlése
docker system df                   # lemezhasználat összefoglaló
```
:::lang

:::section
