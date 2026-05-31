---
title: The Legacy BIOS + GPT Trap
slug: posts/homelab-legacy-bios.html
description: Why older machines refuse to boot after a successful Linux installation, and how to fix it with Debian's classic installer.
layout: page
---

:::section intro

:::lang en
# The Legacy BIOS + GPT Trap

If you're trying to install Linux on an older machine and keep hitting **"Boot Device Not Found"** after a seemingly successful installation, you're probably caught in the Legacy BIOS + GPT trap. This post documents the problem, how to diagnose it, and how to solve it — so you don't spend hours on what I did.

> **This is a companion post to:** [Home Lab Docker Server and Deploy Workflow](/posts/homelab-docker-server.html)
:::lang

:::lang hu
# A Legacy BIOS + GPT csapda

Ha Linuxot próbálsz telepíteni egy régebbi gépre, és az egyébként sikeres telepítés után újra és újra **"Boot Device Not Found"** üzenettel találkozol, valószínűleg a Legacy BIOS + GPT csapdába estél. Ez a post dokumentálja a problémát, a diagnosztikát és a megoldást — hogy ne kelljen órákig keresnod azt, amit én már megtaláltam.

> **Ez egy companion post ehhez:** [Otthoni Docker szerver és deploy workflow](/posts/homelab-docker-server.html)
:::lang

:::section

:::section problem

:::lang en
## The Problem in Plain English

Modern Linux installers — especially Ubuntu Server with its Subiquity installer — default to creating a **GPT partition table**, even when the system is booting in Legacy BIOS mode.

GPT + Legacy BIOS boot is technically valid. GRUB supports it through a dedicated 1 MB `bios_grub` partition where it stores its core image. The MBR gets a small boot stub that hands off execution to that partition. On most modern hardware this works fine.

The problem is that **some older BIOSes in Legacy mode don't reliably execute this boot chain from a GPT disk.** The BIOS reads the protective MBR, finds a valid `0x55AA` boot signature, starts to hand off — and then silently fails. You get a blank screen flash and land back at "Boot Device Not Found."

The frustrating part: GRUB is correctly installed. The MBR has a valid signature. The `bios_grub` partition exists with the right flag. Everything looks right. The system just won't boot.
:::lang

:::lang hu
## A probléma egyszerűen

A modern Linux telepítők — különösen az Ubuntu Server Subiquity telepítője — alapértelmezetten **GPT partíciós táblát** hoznak létre, még akkor is, ha a rendszer Legacy BIOS módban bootol.

A GPT + Legacy BIOS kombináció technikailag érvényes. A GRUB egy dedikált 1 MB-os `bios_grub` partíción keresztül támogatja ezt, ahol a core image-t tárolja. Az MBR egy kis boot stubot kap, amely átadja a vezérlést ennek a partíciónak. A legtöbb modern hardveren ez működik.

A probléma az, hogy **egyes régebbi BIOS-ok Legacy módban nem hajtják végre megbízhatóan ezt a boot láncot GPT lemezről.** A BIOS beolvassa a protective MBR-t, talál egy érvényes `0x55AA` boot szignatúrát, elkezdi az átadást — majd csendben meghibásodik. Egy pillanatra fekete képernyő, aztán vissza a "Boot Device Not Found" üzenethez.

A frusztráló rész: a GRUB helyesen van telepítve. Az MBR érvényes szignatúrával rendelkezik. A `bios_grub` partíció létezik a megfelelő flaggel. Minden rendbennek látszik. A rendszer egyszerűen nem bootol.
:::lang

:::section

:::section who-is-affected

:::lang en
## Who Is Affected

This tends to happen on laptops and desktops manufactured between roughly **2010 and 2016** — old enough to have a Legacy BIOS (or UEFI with Legacy/CSM mode), but still capable hardware for a home server. Specifically:

- Machines where **UEFI boot from USB doesn't work** reliably, forcing Legacy mode for installation
- Any machine where the BIOS firmware was never updated past its original factory version

The typical scenario: you're repurposing an old laptop as a Docker host, and you want to avoid the complexity of UEFI setup on aging hardware.
:::lang

:::lang hu
## Kit érint

Ez általában **2010 és 2016 között** gyártott laptopokon és asztali gépeken fordul elő — elég régiek ahhoz, hogy Legacy BIOS-uk legyen (vagy UEFI Legacy/CSM móddal), de még mindig alkalmas hardver egy otthoni szerverhez. Konkrétan:

- Gépek, ahol **az UEFI USB bootolás nem működik** megbízhatóan, és Legacy módot kell használni a telepítéshez
- Bármely gép, amelynek BIOS firmware-e soha nem lett frissítve a gyári verzión túl

A tipikus helyzet: egy régi laptopot szeretnél Docker hosztként hasznosítani, és el akarod kerülni a UEFI beállítás komplexitását elavult hardveren.
:::lang

:::section

:::section diagnosis

:::lang en
## Diagnosis: Is This Your Problem?

Work through this before spending time on GRUB repair attempts.

**Step 1 — Can you boot the USB installer?**

If the USB appears and boots but the system won't boot after install, continue below. If the USB doesn't appear in the boot menu at all, this is a USB formatting issue, not a GPT problem. Recreate with Rufus using **MBR partition scheme + BIOS or UEFI-CSM target**.

**Step 2 — Check the partition table.**

Boot the installer again, drop to a shell (`Ctrl+Alt+F2`), and run:

```bash
sudo parted /dev/sda print
```

If you see `Partition Table: gpt` with a 1 MB partition flagged `bios_grub` — GRUB was set up for Legacy BIOS on a GPT disk. This is the trap.

**Step 3 — Verify the MBR boot signature.**

```bash
dd if=/dev/sda bs=1 count=2 skip=510 2>/dev/null | hexdump
```

Expected output: `0000000 aa55` — a valid boot sector is present.

> **Note:** GRUB 2's `boot.img` does not contain the ASCII string "GRUB" in the MBR sector. A grep-based check returns nothing even on a correctly installed system — don't use it as a diagnostic.

**Step 4 — Confirm the BIOS sees the disk.**

Enter your BIOS setup and check the storage or device configuration section. If your disk is listed there, the hardware is fine. The issue is entirely in the boot chain handoff between the BIOS and GRUB over a GPT disk.
:::lang

:::lang hu
## Diagnosztika: ez a te problémád?

Menj végig ezeken, mielőtt GRUB-javítással próbálkozol.

**1. lépés — Bootol az USB telepítő?**

Ha az USB megjelenik és bootol, de a telepítés utáni rendszer nem indul, folytasd az alábbiakkal. Ha az USB egyáltalán nem jelenik meg a boot menüben, ez USB formázási probléma, nem GPT-probléma. Hozd létre újra Rufusszal: **MBR partíciós séma + BIOS vagy UEFI-CSM target**.

**2. lépés — Ellenőrizd a partíciós táblát.**

Indítsd el újra a telepítőt, lépj shellbe (`Ctrl+Alt+F2`), és futtasd:

```bash
sudo parted /dev/sda print
```

Ha `Partition Table: gpt` jelenik meg egy `bios_grub` flaggel jelölt 1 MB-os partícióval — a GRUB Legacy BIOS-ra lett beállítva GPT lemezen. Ez a csapda.

**3. lépés — Ellenőrizd az MBR boot szignatúrát.**

```bash
dd if=/dev/sda bs=1 count=2 skip=510 2>/dev/null | hexdump
```

Várt kimenet: `0000000 aa55` — érvényes boot szektor.

> **Megjegyzés:** A GRUB 2 `boot.img`-je nem tartalmazza a "GRUB" ASCII szöveget az MBR szektorban. Egy grep-alapú ellenőrzés akkor is üres eredményt ad, ha a telepítés helyes — ne használd diagnosztikai eszközként.

**4. lépés — Ellenőrizd, hogy a BIOS látja-e a lemezt.**

Lépj be a BIOS beállításaiba, és nézd meg a tárolóeszközök vagy eszközök konfigurációs szekciót. Ha a lemez ott szerepel, a hardver rendben van. A probléma kizárólag a BIOS és a GRUB közötti boot lánc átadásában van GPT lemez esetén.
:::lang

:::section

:::section why-grub-repair-wont-fix

:::lang en
## Why GRUB Repair Won't Fix It

If you've already tried `chroot + grub-install /dev/sda`, possibly multiple times, and it runs without errors but the system still won't boot — here's why.

GRUB is not the problem. `grub-install --target=i386-pc --recheck /dev/sda` correctly writes `boot.img` to the MBR and `core.img` to the `bios_grub` partition. "Installation finished. No error reported." is accurate — the installation succeeded. The BIOS simply doesn't reliably hand off execution from the protective MBR of a GPT disk into that boot chain.

Running GRUB repair again won't change this. The problem lives in the BIOS firmware, not in the bootloader.
:::lang

:::lang hu
## Miért nem oldja meg a GRUB javítás?

Ha már megpróbáltad a `chroot + grub-install /dev/sda` parancsot, esetleg többször is, és hiba nélkül lefutott, de a rendszer még mindig nem bootol — íme a magyarázat.

A GRUB nem a probléma. A `grub-install --target=i386-pc --recheck /dev/sda` helyesen írja a `boot.img`-t az MBR-be és a `core.img`-t a `bios_grub` partícióra. Az "Installation finished. No error reported." üzenet pontos — a telepítés sikerült. A BIOS egyszerűen nem hajtja végre megbízhatóan a boot lánc átadását egy GPT lemez protective MBR-jéből.

A GRUB javítás újrafuttatása ezt nem változtatja meg. A probléma a BIOS firmware-ben van, nem a bootloaderben.
:::lang

:::section

:::section solution

:::lang en
## The Solution: MBR Partition Table via Debian

Ubuntu Server's Subiquity installer (from at least version 22.04 onwards) **only creates GPT partition tables**, even in custom storage layout mode. There is no option to choose MBR/DOS, and wiping the disk doesn't change this — the installer will recreate GPT.

**Debian's classic installer (debian-installer) supports MBR/DOS partition tables explicitly.** When booting in Legacy BIOS mode and selecting guided partitioning, it creates an MBR partition table by default. GRUB installs to the MBR gap between the partition table and the first partition, and older BIOSes boot from it without issues.

### Steps

1. Download the Debian netinstall ISO from [debian.org](https://debian.org) (~400 MB)
2. Create a bootable USB with Rufus: **MBR partition scheme + BIOS or UEFI-CSM target**
3. In BIOS: set boot mode to **Legacy**, make sure USB boot is enabled
4. Boot from USB — select it from the one-time boot menu if needed
5. In the Debian installer: choose `Install` (not Graphical installer)
6. At partitioning: `Guided - use entire disk` → `All files in one partition`
7. At software selection: deselect all desktop environments, keep SSH server and standard utilities
8. Install GRUB to `/dev/sda` when prompted

The system will boot on first try.
:::lang

:::lang hu
## A megoldás: MBR partíciós tábla Debiannal

Az Ubuntu Server Subiquity telepítője (legalább a 22.04-es verziótól) **kizárólag GPT partíciós táblát hoz létre**, még egyéni tárolóelrendezés módban is. Nincs lehetőség MBR/DOS választására, és a lemez törlése sem változtat ezen — a telepítő újra GPT-t hoz létre.

**A Debian klasszikus telepítője (debian-installer) explicit módon támogatja az MBR/DOS partíciós táblákat.** Legacy BIOS módban indítva, az irányított particionálást választva, alapértelmezetten MBR partíciós táblát hoz létre. A GRUB az MBR résbe települ a partíciós tábla és az első partíció között, és a régebbi BIOS-ok probléma nélkül bootolnak belőle.

### Lépések

1. Töltsd le a Debian netinstall ISO-t a [debian.org](https://debian.org) oldalról (~400 MB)
2. Hozz létre bootolható USB-t Rufusszal: **MBR partíciós séma + BIOS vagy UEFI-CSM target**
3. BIOS-ban: állítsd be a boot módot **Legacy**-re, és győződj meg arról, hogy az USB boot engedélyezett
4. Bootolj USB-ről — ha szükséges, válaszd ki az egyszer használatos boot menüből
5. A Debian telepítőben: válaszd az `Install` opciót (nem Graphical installer)
6. Particionálásnál: `Guided - use entire disk` → `All files in one partition`
7. Szoftverválasztásnál: töröld az összes asztali környezetet, tartsd meg az SSH server és standard utilities opciókat
8. Telepítsd a GRUB-ot a `/dev/sda` eszközre, amikor a rendszer kéri

A rendszer első próbálkozásra bootolni fog.
:::lang

:::section

:::section uefi-note

:::lang en
## What About UEFI Mode?

UEFI is the correct long-term approach and avoids this problem entirely. If your machine supports booting a UEFI-formatted USB, use it — both Ubuntu and Debian handle UEFI installs cleanly, and the result is a GPT disk with a proper EFI System Partition.

The catch is that on some older machines, UEFI USB boot doesn't work reliably even when UEFI mode is enabled. If your UEFI-formatted USB doesn't appear in the boot device list, you're forced into Legacy mode — and then the GPT trap applies.

Before assuming UEFI won't work, try a BIOS firmware update if one is available. Newer firmware versions often improve UEFI USB boot compatibility.
:::lang

:::lang hu
## Mi a helyzet UEFI móddal?

A UEFI a helyes hosszú távú megközelítés, és teljesen elkerüli ezt a problémát. Ha a gép támogatja UEFI-formázású USB bootolását, használd azt — mind az Ubuntu, mind a Debian tisztán kezeli a UEFI telepítéseket, és az eredmény egy GPT lemez megfelelő EFI System Partícióval.

A buktató: egyes régebbi gépeken a UEFI USB boot nem működik megbízhatóan, még akkor sem, ha a BIOS-ban engedélyezett. Ha a UEFI-formázású USB nem jelenik meg a boot eszközök listájában, kénytelen vagy Legacy módot használni — és akkor életbe lép a GPT csapda.

Mielőtt azt feltételezed, hogy a UEFI nem fog működni, próbálj meg BIOS firmware frissítést keresni, ha elérhető. Az újabb firmware verziók gyakran javítják a UEFI USB boot kompatibilitást.
:::lang

:::section

:::section reference

:::lang en
## Quick Reference

| Symptom | Likely Cause | Fix |
|---|---|---|
| USB not in boot menu | USB not formatted for Legacy | Rufus: MBR + BIOS target |
| "Boot Device Not Found" after Ubuntu install | GPT + Legacy BIOS incompatibility | Switch to Debian |
| `grub-install` succeeds but system won't boot | Same root cause — GRUB is fine, BIOS isn't | Switch to Debian |
| UEFI USB not recognized | Old firmware / BIOS quirk | Update firmware, or use Legacy + Debian |
| `Partition Table: gpt` + `bios_grub` flag | Ubuntu Subiquity GPT-only behavior | Switch to Debian |

**My configuration for reference:**

- Machine: HP EliteBook 840 G2 (2015, repurposed as home lab server)
- Failed distro: Ubuntu Server 26.04 (Subiquity installer, GPT-only)
- Working distro: Debian 13 (classic installer, MBR partition table)
- BIOS quirk: UEFI-formatted USB not recognized in boot menu; Legacy mode required
- Final result: Fully functional headless Docker host — Debian 13, Docker Engine, Portainer CE, Nginx Proxy Manager
:::lang

:::lang hu
## Gyors referencia

| Tünet | Valószínű ok | Megoldás |
|---|---|---|
| USB nem szerepel a boot menüben | Az USB nincs Legacy-re formázva | Rufus: MBR + BIOS target |
| "Boot Device Not Found" Ubuntu telepítés után | GPT + Legacy BIOS inkompatibilitás | Váltás Debianra |
| `grub-install` sikeres, de a rendszer nem bootol | Ugyanaz az ok — a GRUB rendben, a BIOS nem | Váltás Debianra |
| UEFI USB nem ismeri fel | Régi firmware / BIOS hiba | Firmware frissítés, vagy Legacy + Debian |
| `Partition Table: gpt` + `bios_grub` flag | Ubuntu Subiquity GPT-only viselkedés | Váltás Debianra |

**Saját konfiguráció referenciának:**

- Gép: HP EliteBook 840 G2 (2015, otthoni lab szerverként hasznosítva)
- Sikertelen disztribúció: Ubuntu Server 26.04 (Subiquity telepítő, GPT-only)
- Működő disztribúció: Debian 13 (klasszikus telepítő, MBR partíciós tábla)
- BIOS sajátosság: UEFI-formázású USB nem ismerhető fel a boot menüben; Legacy módra volt szükség
- Végeredmény: Teljesen funkcionális headless Docker hoszt — Debian 13, Docker Engine, Portainer CE, Nginx Proxy Manager
:::lang

:::section
