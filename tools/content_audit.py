#!/usr/bin/env python3
"""Audit content/ for Task 12 (duplicate titles, heading levels, description_hu).

Writes plans/content-audit.md.   Run: python tools/content_audit.py
(plans/, not docs/: docs/ is the published site.)
"""
import re, sys, pathlib

root = pathlib.Path(__file__).resolve().parent.parent
out = root / "plans" / "content-audit.md"
files = sorted((root / "content").rglob("*.md"))

def norm(s):
    return re.sub(r"[^a-z0-9]+", " ", s.lower()).strip()

lines_out = ["# Content audit", "", "> Run: python tools/content_audit.py", ""]
for f in files:
    text = f.read_text(encoding="utf-8")
    lines = text.splitlines()
    meta, i = {}, 1
    while lines[i].strip() != "---":
        k, _, v = lines[i].partition(":")
        meta[k.strip()] = v.strip()
        i += 1
    rel = f.relative_to(root).as_posix()
    lines_out.append(f"## {rel}")
    lines_out.append(f"- title: {meta.get('title')} | title_hu: {meta.get('title_hu','')}")
    lines_out.append(f"- description: {meta.get('description','')}")
    lines_out.append(f"- description_hu: {meta.get('description_hu','(missing)')}")
    # walk blocks
    section = lang = None
    fence = False
    block_headings = {}
    for n, ln in enumerate(lines[i + 1:], start=i + 2):
        s = ln.strip()
        if s.startswith("```"):
            fence = not fence
        if fence or s.startswith("```"):
            continue
        m = re.match(r":::section (\S+)", s)
        if m: section = m.group(1); continue
        if s == ":::section": section = None; continue
        m = re.match(r":::lang (\w+)", s)
        if m: lang = m.group(1); continue
        if s == ":::lang": lang = None; continue
        h = re.match(r"(#{1,6})\s+(.*)", s)
        if h and lang:
            block_headings.setdefault((section, lang), []).append((n, len(h.group(1)), h.group(2)))
    levels = [lv for hs in block_headings.values() for _, lv, _ in hs]
    lines_out.append(f"- min heading level: {min(levels) if levels else '-'}")
    first = {}
    for (sec, lg), hs in block_headings.items():
        n, lv, t = hs[0]
        ref = meta.get("title_hu") if lg == "hu" and meta.get("title_hu") else meta.get("title")
        dup = norm(t) == norm(ref or "")
        first[(sec, lg)] = (n, lv, t, dup)
    for (sec, lg), (n, lv, t, dup) in sorted(first.items(), key=lambda kv: kv[1][0]):
        lines_out.append(f"  - [{sec}/{lg}] line {n}: h{lv} `{t}`" + (" **== title**" if dup else ""))
    figs = re.findall(r":::section (fig-\S+)", text)
    if figs: lines_out.append(f"- fig sections: {', '.join(figs)}")
    lines_out.append("")
out.write_text("\n".join(lines_out), encoding="utf-8")
print(f"wrote {out}")
