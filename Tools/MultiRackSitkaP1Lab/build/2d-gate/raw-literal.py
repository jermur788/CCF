#!/usr/bin/env python3
"""
SPEC-multi-2d-s0-seam-extraction-V1 §5.3 — Deterministic C# raw-string-literal extractor for /multi assets.
Python 3 standard library only. Reads Git blob bytes directly via `git show`, independent of checkout autocrlf.
"""
import argparse
import hashlib
import json
import subprocess
import sys

DEFAULT_GIT_PATH = "Tools/MultiRackSitkaP1Lab/src/Ui/PlayerMultiApi.cs"
EXPECTED_ORDER = ("Html", "Css", "Js")


def read_blob(repo: str, rev: str, git_path: str) -> tuple[str, bytes]:
    blob_id = subprocess.check_output(
        ["git", "-C", repo, "rev-parse", f"{rev}:{git_path}"],
        text=True,
    ).strip()
    raw = subprocess.check_output(["git", "-C", repo, "show", f"{rev}:{git_path}"])
    return blob_id, raw


def parse_raw_literals(raw: bytes) -> dict[str, dict]:
    if raw.startswith(b"\xef\xbb\xbf"):
        sys.exit("REFUSED: source Git blob starts with UTF-8 BOM")
    if b"\r" in raw:
        sys.exit("REFUSED: source Git blob contains CR (0x0d) byte(s)")
    try:
        text = raw.decode("utf-8", errors="strict")
    except UnicodeDecodeError as exc:
        sys.exit(f"REFUSED: source Git blob is not valid UTF-8: {exc}")

    lines = text.split("\n")
    if lines and lines[-1] == "":
        lines = lines[:-1]

    # Locate all three constants and enforce exact order, exact opener, no $, and col-0 closing delimiter.
    found: dict[str, dict] = {}
    decl_positions: list[tuple[int, str]] = []

    for name in EXPECTED_ORDER:
        exact_opener = f'    internal const string {name} = """'
        # Also refuse interpolated or indented/variant forms for this constant name.
        hits = []
        for idx, line in enumerate(lines):
            if f"internal const string {name} =" in line:
                if "$" in line:
                    sys.exit(f"REFUSED: interpolated raw literal prefix ($) found on line {idx + 1} for {name}")
                if line != exact_opener:
                    sys.exit(f"REFUSED: unexpected declaration shape on line {idx + 1} for {name}: {line!r}")
                hits.append(idx)
        if len(hits) != 1:
            sys.exit(f"REFUSED: expected exactly 1 declaration for {name}, found {len(hits)}")
        decl_idx = hits[0]
        decl_positions.append((decl_idx, name))

        close_idx = None
        for j in range(decl_idx + 1, len(lines)):
            stripped = lines[j].lstrip(" \t")
            if stripped == '""";':
                if lines[j] != '""";':
                    sys.exit(
                        f"REFUSED: closing delimiter for {name} on line {j + 1} has leading whitespace "
                        f"(non-zero dedent): {lines[j]!r}"
                    )
                close_idx = j
                break
        if close_idx is None:
            sys.exit(f"REFUSED: closing '\"\"\";' delimiter not found for {name}")

        content_lines = lines[decl_idx + 1 : close_idx]
        # C# raw-string-literal rule (col-0 closing delimiter): dedent = 0, LF join, NO trailing LF.
        value_bytes = "\n".join(content_lines).encode("utf-8")
        sha256 = hashlib.sha256(value_bytes).hexdigest()
        found[name] = {
            "name": name,
            "declLine": decl_idx + 1,
            "contentStartLine": decl_idx + 2,
            "contentEndLine": close_idx,
            "closeLine": close_idx + 1,
            "lineCount": len(content_lines),
            "bytes": len(value_bytes),
            "sha256": sha256,
            "lastByteHex": f"0x{value_bytes[-1]:02x}" if value_bytes else "",
            "_bytes": value_bytes,
        }

    actual_order = tuple(n for _, n in sorted(decl_positions))
    if actual_order != EXPECTED_ORDER:
        sys.exit(f"REFUSED: constant order mismatch: got {actual_order}, expected {EXPECTED_ORDER}")

    return found


def main() -> int:
    ap = argparse.ArgumentParser(description="Extract C# raw string literals verbatim from a Git blob.")
    ap.add_argument("--repo", required=True, help="Git repository root")
    ap.add_argument("--rev", required=True, help="Git revision (e.g. T0 commit SHA)")
    ap.add_argument("--path", default=DEFAULT_GIT_PATH, help="Repo-relative path to PlayerMultiApi.cs")
    ap.add_argument("--name", choices=EXPECTED_ORDER, help="Constant name to extract")
    ap.add_argument("--out-sha256", action="store_true", help="Print SHA-256 of extracted bytes")
    ap.add_argument("--out-file", help="Write exact extracted bytes to path (no trailing LF, no BOM)")
    ap.add_argument("--meta-json", action="store_true", help="Print JSON metadata for --name or all constants")
    args = ap.parse_args()

    blob_id, raw = read_blob(args.repo, args.rev, args.path)
    parsed = parse_raw_literals(raw)

    if args.meta_json:
        if args.name:
            rec = {k: v for k, v in parsed[args.name].items() if k != "_bytes"}
            out = {"rev": args.rev, "path": args.path, "sourceBlobId": blob_id, "asset": rec}
        else:
            out = {
                "rev": args.rev,
                "path": args.path,
                "sourceBlobId": blob_id,
                "assets": {
                    k.lower(): {kk: vv for kk, vv in v.items() if kk != "_bytes"}
                    for k, v in parsed.items()
                },
            }
        print(json.dumps(out, indent=2))
        return 0

    if not args.name:
        sys.exit("USAGE ERROR: specify --name Html|Css|Js (or --meta-json)")

    rec = parsed[args.name]
    if args.out_file:
        with open(args.out_file, "wb") as f:
            f.write(rec["_bytes"])
    if args.out_sha256:
        print(rec["sha256"])
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
