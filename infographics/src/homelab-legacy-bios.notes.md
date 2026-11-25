# homelab-legacy-bios — The Legacy BIOS + GPT Trap
Source: content/posts/homelab-legacy-bios.md (slug `posts/homelab-legacy-bios.html`, standalone: no series/part, companion post of homelab-docker-server) · Pattern: flow + compare + callout (square); compact flow + callout (og)

| Element | Text | Source |
|---|---|---|
| Eyebrow | Home Lab · Companion post | intro: "This is a companion post to: Home Lab Docker Server and Deploy Workflow" |
| Title | The Legacy BIOS + GPT trap | title |
| Subtitle (square) | Why older machines refuse to boot after a successful Linux installation. | description: "Why older machines refuse to boot after a successful Linux installation" |
| Node 1 | **GPT partition table** — Modern installers default to GPT, even in Legacy BIOS mode. | "Modern Linux installers… default to creating a **GPT partition table**, even when the system is booting in Legacy BIOS mode." |
| Node 2 | **GRUB installs correctly** — boot.img in the MBR, core.img in the bios_grub partition. | "`grub-install` correctly writes `boot.img` to the MBR and `core.img` to the `bios_grub` partition." |
| Node 3 (key) | **BIOS hand-off fails** — Some older BIOSes don't reliably run this boot chain from a GPT disk. | "some older BIOSes in Legacy mode don't reliably execute this boot chain from a GPT disk" |
| Node 4 | **Boot Device Not Found** — After a seemingly successful installation. | "keep hitting **"Boot Device Not Found"** after a seemingly successful installation" |
| Compare head 1 | Ubuntu Server (Subiquity) | "Ubuntu Server's Subiquity installer" |
| Compare 1 | Only creates GPT partition tables | "**only creates GPT partition tables**, even in custom storage layout mode" |
| Compare 1 | No option to choose MBR/DOS | "There is no option to choose MBR/DOS" |
| Compare 1 | grub-install succeeds, the system still won't boot | "runs without errors but the system still won't boot" |
| Compare head 2 (key) | Debian classic installer | "Debian's classic installer (debian-installer)" |
| Compare 2 | Explicitly supports MBR/DOS partition tables | "supports MBR/DOS partition tables explicitly" |
| Compare 2 | GRUB installs to the MBR gap | "GRUB installs to the MBR gap between the partition table and the first partition" |
| Compare 2 | The system will boot on first try | "The system will boot on first try." |
| Callout (square) | GRUB is not the problem. The problem lives in the BIOS firmware, not in the bootloader. | "GRUB is not the problem." / "The problem lives in the BIOS firmware, not in the bootloader." |
| Callout (og) | The problem lives in the BIOS firmware, not in the bootloader. | same |
