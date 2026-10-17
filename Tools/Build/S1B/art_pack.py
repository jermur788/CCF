#!/usr/bin/env python3
"""Explicit, verified local art-pack dependency for the Linux Player build (Tier-3 tooling).

The shipped scene references prefabs from the Unity Asset Store pack "Ultimate Nature - Starter"
(Innerverse Interactive). Per Assets/ForestPrototype/Docs/UltimateNatureSetup.md the pack is a LOCALLY
ACQUIRED dependency: it is gitignored and must never be committed or redistributed. Without it the
scene has missing prefab instances and the Player build reports errors.

This module makes the dependency explicit and checkable without storing any pack content:

  * art-pack-manifest.json (committed, next to this file) lists exactly the pack files the project
    needs (the GUID-reference closure of the project's own assets), with sha256 and Unity GUID, the
    Asset Store provenance Unity recorded (product id, package version, upload id) and a checksum of
    the whole pack's non-.meta content.
  * verify_pack() refuses a missing or different pack with a precise message.
  * prepare() verifies the pack already installed at Assets/InnerverseInteractive, or verifies the
    pack the caller names with --art-pack / $CCF_ART_PACK_DIR and installs ONLY the required files
    (and their .meta) into that gitignored folder. No machine-specific path is stored anywhere.

Commands:
  art_pack.py verify   --pack DIR [--manifest FILE]              check a pack folder against the manifest
  art_pack.py generate --project ROOT --pack DIR [--out FILE]    regenerate the manifest (maintainer tool)
  art_pack.py generate --project ROOT --pack DIR --check         fail if the committed manifest is stale
"""
import argparse
import hashlib
import json
import os
import re
import shutil
import sys
from pathlib import Path

HERE = Path(__file__).resolve().parent
DEFAULT_MANIFEST = HERE / "art-pack-manifest.json"
PROJECT_PACK_REL = "Assets/InnerverseInteractive"
SCHEMA = 1

GUID_RE = re.compile(rb"[0-9a-f]{32}")
# Text-serialised asset kinds that can reference other assets by GUID (shaders/includes are scanned too inside the pack).
TEXT_EXTS = {".unity", ".prefab", ".asset", ".mat", ".controller", ".anim", ".playable", ".signal", ".lighting",
             ".physicmaterial", ".flare", ".rendertexture", ".shadergraph", ".shadersubgraph", ".uxml", ".uss",
             ".inputactions", ".json", ".preset", ".mixer", ".overridecontroller", ".terrainlayer", ".spriteatlas"}
PACK_TEXT_EXTS = TEXT_EXTS | {".shader", ".cginc", ".hlsl", ".txt"}


class ArtPackError(Exception):
    """A precise, user-facing reason the art pack cannot be accepted."""


def sha256_file(path):
    h = hashlib.sha256()
    with open(path, "rb") as f:
        for block in iter(lambda: f.read(1 << 20), b""):
            h.update(block)
    return h.hexdigest()


def read_guid(meta_path):
    m = re.search(r"^guid: ([0-9a-f]{32})\s*$", Path(meta_path).read_text(encoding="utf-8", errors="replace"), re.M)
    return m.group(1) if m else None


def parse_origin(meta_path):
    """Asset Store provenance the Package Manager writes into a .meta (AssetOrigin block), or None."""
    text = Path(meta_path).read_text(encoding="utf-8", errors="replace")
    m = re.search(r"^AssetOrigin:\n((?:[ \t]+.*\n?)+)", text, re.M)
    if not m:
        return None
    fields = {}
    for key, value in re.findall(r"^[ \t]+(productId|packageName|packageVersion|uploadId): (.*)$", m.group(1), re.M):
        value = value.strip()
        fields[key] = json.loads(value) if value.startswith('"') else value
    return fields


def digest_lines(lines):
    return hashlib.sha256("".join(lines).encode("utf-8")).hexdigest()


def required_set_digest(entries):
    return digest_lines(f"{e['path']}\0{e['sha256']}\0{e['guid']}\n" for e in sorted(entries, key=lambda e: e["path"]))


def full_content(pack_root):
    """Count, bytes and sha256 over every non-.meta file (.meta files differ harmlessly between imports)."""
    root = Path(pack_root)
    rows, total = [], 0
    for path in sorted(p for p in root.rglob("*") if p.is_file() and p.suffix != ".meta"):
        rel = path.relative_to(root).as_posix()
        rows.append(f"{rel}\0{sha256_file(path)}\n")
        total += path.stat().st_size
    return {"files": len(rows), "bytes": total, "sha256": digest_lines(rows)}


# ----------------------------------------------------------------------------- manifest generation

def crawl_required(project_root, pack_root):
    """Pack files (relative to pack_root) the project's own assets need: the GUID-reference closure."""
    project, pack = Path(project_root), Path(pack_root)
    guid_to_path = {}
    for meta in pack.rglob("*.meta"):
        g = read_guid(meta)
        if g:
            guid_to_path[g.encode()] = Path(str(meta)[:-5])
    pack_abs = os.path.abspath(pack)
    direct = set()
    for d, _, files in os.walk(project / "Assets"):
        if os.path.abspath(d).startswith(pack_abs):
            continue
        for name in files:
            p = Path(d) / name
            if p.suffix.lower() in TEXT_EXTS and p.stat().st_size < 80_000_000:
                for g in set(GUID_RE.findall(p.read_bytes())):
                    if g in guid_to_path:
                        direct.add(g)
    required, queue = set(), list(direct)
    while queue:
        asset = guid_to_path[queue.pop()]
        if asset in required:
            continue
        required.add(asset)
        scan = [Path(str(asset) + ".meta")]
        if asset.is_file() and asset.suffix.lower() in PACK_TEXT_EXTS:
            scan.append(asset)
        for p in scan:
            if p.is_file():
                for g in set(GUID_RE.findall(p.read_bytes())):
                    if g in guid_to_path and guid_to_path[g] not in required:
                        queue.append(g)
    return sorted(p.relative_to(pack).as_posix() for p in required if p.is_file())


def generate_manifest(project_root, pack_root):
    pack = Path(pack_root)
    entries, origins = [], set()
    for rel in crawl_required(project_root, pack):
        meta = pack / (rel + ".meta")
        if not meta.is_file():
            raise ArtPackError("pack file without .meta: " + rel)
        origin = parse_origin(meta)
        if not origin:
            raise ArtPackError("no Asset Store AssetOrigin recorded in " + rel + ".meta")
        origins.add(json.dumps(origin, sort_keys=True))
        entries.append({"path": rel, "bytes": (pack / rel).stat().st_size, "sha256": sha256_file(pack / rel), "guid": read_guid(meta)})
    if not entries:
        raise ArtPackError("the project references no pack file; nothing to pin")
    if len(origins) != 1:
        raise ArtPackError("required files record more than one Asset Store origin: " + "; ".join(sorted(origins)))
    origin = json.loads(next(iter(origins)))
    return {
        "schema": SCHEMA,
        "name": origin["packageName"],
        "publisher": "Innerverse Interactive",
        "source": "Unity Asset Store (product id %s). Locally acquired dependency; NOT stored in this repository and never to be committed or redistributed." % origin["productId"],
        "assetStoreProductId": origin["productId"],
        "packageVersion": origin["packageVersion"],
        "uploadId": origin["uploadId"],
        "installFolder": PROJECT_PACK_REL,
        "setupDoc": "Assets/ForestPrototype/Docs/UltimateNatureSetup.md",
        "identityPolicy": ("required files: exact sha256 + Unity GUID (enforced). fullContent: sha256 over every non-.meta file of the whole "
                           "pack (recorded and compared; a mismatch is a warning because the build only consumes the required files)."),
        "required": entries,
        "requiredSetSha256": required_set_digest(entries),
        "fullContent": full_content(pack),
    }


# ----------------------------------------------------------------------------- verification / installation

def load_manifest(path=DEFAULT_MANIFEST):
    data = json.loads(Path(path).read_text(encoding="utf-8"))
    if data.get("schema") != SCHEMA or not data.get("required"):
        raise ArtPackError("art-pack manifest is missing or has an unsupported schema: " + str(path))
    if required_set_digest(data["required"]) != data["requiredSetSha256"]:
        raise ArtPackError("art-pack manifest is internally inconsistent (requiredSetSha256 does not match its entries)")
    return data


def resolve_pack_root(path):
    """Accept the InnerverseInteractive folder itself or its parent (e.g. a project's Assets folder)."""
    p = Path(path)
    if (p / "InnerverseInteractive").is_dir():
        return p / "InnerverseInteractive"
    return p


def verify_pack(pack_root, manifest, label="art pack"):
    """Raise ArtPackError unless every required file is present with the exact bytes and GUID. Returns a summary."""
    root = Path(pack_root)
    if not root.is_dir():
        raise ArtPackError("%s folder not found: %s" % (label, root))
    problems = []
    for e in manifest["required"]:
        f, meta = root / e["path"], root / (e["path"] + ".meta")
        if not f.is_file():
            problems.append("missing " + e["path"])
            continue
        if f.stat().st_size != e["bytes"] or sha256_file(f) != e["sha256"]:
            problems.append("different bytes " + e["path"])
        if not meta.is_file():
            problems.append("missing .meta " + e["path"])
        elif read_guid(meta) != e["guid"]:
            problems.append("different GUID " + e["path"])
    if problems:
        raise ArtPackError("%s does not match %s %s (%d of %d required files): %s" % (
            label, manifest["name"], manifest["packageVersion"], len(problems), len(manifest["required"]), "; ".join(problems[:6])))
    return {"requiredFiles": len(manifest["required"]), "requiredSetSha256": manifest["requiredSetSha256"]}


def install_required(src_root, dest_root, manifest):
    """Copy ONLY the required files and their .meta files; nothing else from the pack leaves its source."""
    for e in manifest["required"]:
        for suffix in ("", ".meta"):
            src, dst = Path(src_root) / (e["path"] + suffix), Path(dest_root) / (e["path"] + suffix)
            dst.parent.mkdir(parents=True, exist_ok=True)
            shutil.copy2(src, dst)


def prepare(project_root, art_pack=None, manifest_path=DEFAULT_MANIFEST):
    """Make sure the project has the pinned art pack. Returns provenance for build-identity.json."""
    manifest = load_manifest(manifest_path)
    dest = Path(project_root) / PROJECT_PACK_REL
    source_kind = None
    full = None
    if art_pack:
        src = resolve_pack_root(art_pack)
        verify_pack(src, manifest, "art pack given by --art-pack")
        full = full_content(src) if (src / "Ultimate Nature – Starter").is_dir() else None
        source_kind = "explicit --art-pack"
    if dest.exists():
        verify_pack(dest, manifest, "art pack installed in the project (%s)" % PROJECT_PACK_REL)
        source_kind = (source_kind + "; project folder already installed and verified") if source_kind else "project folder already installed and verified"
    elif art_pack:
        install_required(src, dest, manifest)
        verify_pack(dest, manifest, "art pack installed from --art-pack into the project")
        source_kind += "; required files installed into the gitignored project folder"
    else:
        raise ArtPackError(
            "the %s art pack is required but not available. Install it as described in %s, or pass its folder with "
            "--art-pack DIR (or set CCF_ART_PACK_DIR). Expected: Asset Store product %s, version %s, required files verified by sha256."
            % (manifest["name"], manifest["setupDoc"], manifest["assetStoreProductId"], manifest["packageVersion"]))
    summary = {
        "verified": True,
        "name": manifest["name"], "publisher": manifest["publisher"],
        "assetStoreProductId": manifest["assetStoreProductId"], "packageVersion": manifest["packageVersion"], "uploadId": manifest["uploadId"],
        "requiredFiles": len(manifest["required"]), "requiredSetSha256": manifest["requiredSetSha256"],
        "manifestSha256": sha256_file(manifest_path),
        "sourceKind": source_kind,
        "committedToRepository": False,
    }
    if full is not None:
        summary["fullContent"] = {"files": full["files"], "sha256": full["sha256"],
                                  "matchesManifest": full["sha256"] == manifest["fullContent"]["sha256"]}
    else:
        summary["fullContent"] = "not computed (source was not a complete pack)"
    return summary


# ----------------------------------------------------------------------------- CLI

def main(argv=None):
    p = argparse.ArgumentParser(description=__doc__, formatter_class=argparse.RawDescriptionHelpFormatter)
    sub = p.add_subparsers(dest="cmd", required=True)
    v = sub.add_parser("verify")
    v.add_argument("--pack", required=True)
    v.add_argument("--manifest", default=str(DEFAULT_MANIFEST))
    g = sub.add_parser("generate")
    g.add_argument("--project", required=True)
    g.add_argument("--pack", required=True)
    g.add_argument("--out", default=str(DEFAULT_MANIFEST))
    g.add_argument("--check", action="store_true", help="compare with the committed manifest instead of writing")
    a = p.parse_args(argv)
    try:
        if a.cmd == "verify":
            manifest = load_manifest(a.manifest)
            root = resolve_pack_root(a.pack)
            print(json.dumps(verify_pack(root, manifest), indent=2))
            if (root / "Ultimate Nature – Starter").is_dir():
                full = full_content(root)
                print("fullContent matches manifest:", full["sha256"] == manifest["fullContent"]["sha256"], "(files %d)" % full["files"])
            return 0
        fresh = generate_manifest(a.project, resolve_pack_root(a.pack))
        if a.check:
            committed = load_manifest(a.out)
            same = committed["required"] == fresh["required"] and committed["requiredSetSha256"] == fresh["requiredSetSha256"]
            print("manifest %s" % ("CURRENT" if same else "STALE: the project's pack references changed; regenerate and review"))
            return 0 if same else 1
        Path(a.out).write_text(json.dumps(fresh, indent=2, ensure_ascii=False) + "\n", encoding="utf-8")
        print("wrote %s: %d required files, requiredSetSha256 %s" % (a.out, len(fresh["required"]), fresh["requiredSetSha256"]))
        return 0
    except ArtPackError as error:
        print("STOP: " + str(error), file=sys.stderr)
        return 1


if __name__ == "__main__":
    sys.exit(main())
