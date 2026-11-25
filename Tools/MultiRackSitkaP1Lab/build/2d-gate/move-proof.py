#!/usr/bin/env python3
"""
SPEC-multi-2d-s0-seam-extraction-V1 §5.4–§5.6 — Mechanical move-proof verifier (Python 3 stdlib only).

Proves zero logic delta for S0a mechanical splits:
  (a) Exact non-empty line-multiset equality between T0 source file(s) and HEAD target files,
      accounting ONLY for the permitted spec §5.2 (i)-(iv) additions/removals.
  (b) Contiguous verbatim block moves preserving original relative order within every target file.
  (c) UI static-field ordering invariant: all static readonly fields and constants stay in MultiRoutes.cs
      in original T0 order, with zero static readonly fields in MultiApi.*.cs.
  (d) Core root invariant: all fields, constructors, Advance, and Save/Load stay in MultiAreaContinuation.cs,
      with Advance byte-identical to T0 and zero field declarations in MultiAreaContinuation.Commands.*.cs.
"""
import argparse
import collections
import json
import re
import subprocess
import sys


def git_blob_lines(repo: str, rev: str, path: str) -> tuple[str, list[str]]:
    blob_id = subprocess.check_output(
        ["git", "-C", repo, "rev-parse", f"{rev}:{path}"], text=True
    ).strip()
    raw = subprocess.check_output(["git", "-C", repo, "show", f"{rev}:{path}"])
    if raw.startswith(b"\xef\xbb\xbf"):
        sys.exit(f"FAIL: UTF-8 BOM in {rev}:{path}")
    if b"\r" in raw:
        sys.exit(f"FAIL: CR byte in {rev}:{path}")
    text = raw.decode("utf-8", errors="strict")
    lines = text.split("\n")
    if lines and lines[-1] == "":
        lines = lines[:-1]
    return blob_id, lines


def find_subsequence(haystack: list[str], needle: list[str], start_pos: int) -> int:
    n = len(needle)
    limit = len(haystack) - n + 1
    for i in range(start_pos, limit):
        if haystack[i : i + n] == needle:
            return i
    return -1


def verify_part(repo: str, base_rev: str, head_rev: str, part_name: str, cfg: dict) -> dict:
    src_path = cfg["sourcePath"]
    src_blob_id, t0_lines = git_blob_lines(repo, base_rev, src_path)
    if cfg.get("sourceBlobId") and src_blob_id != cfg["sourceBlobId"]:
        sys.exit(f"FAIL [{part_name}]: T0 blob id {src_blob_id} != expected {cfg['sourceBlobId']}")

    # Verify removed source file is gone if required (e.g. PlayerMultiApi.cs after git mv to MultiRoutes.cs).
    if cfg.get("sourceMustBeDeleted", False):
        chk = subprocess.run(
            ["git", "-C", repo, "cat-file", "-e", f"{head_rev}:{src_path}"],
            capture_output=True,
        )
        if chk.returncode == 0:
            sys.exit(f"FAIL [{part_name}]: old source file still exists at {head_rev}:{src_path}")

    kept_start, kept_end = cfg["keptLineRange"]
    removed_range = cfg.get("removedRange")
    if removed_range:
        rem_start, rem_end = removed_range
        if rem_start != kept_end + 1 or rem_end != len(t0_lines):
            sys.exit(f"FAIL [{part_name}]: removedRange {removed_range} does not align with keptLineRange {cfg['keptLineRange']} and file length {len(t0_lines)}")
    else:
        if kept_start != 1 or kept_end != len(t0_lines):
            sys.exit(f"FAIL [{part_name}]: keptLineRange {cfg['keptLineRange']} != [1, {len(t0_lines)}]")

    # Apply permitted §5.2(i) line replacements on a working copy of T0 kept lines.
    adjusted_t0 = list(t0_lines)
    replaced_map: dict[int, tuple[str, str]] = {}
    for rep in cfg.get("replacedLines", []):
        ln = rep["t0Line"]
        if adjusted_t0[ln - 1] != rep["old"]:
            sys.exit(
                f"FAIL [{part_name}]: T0 line {ln} mismatch for replacement:\n"
                f"  actual={adjusted_t0[ln - 1]!r}\n  expected={rep['old']!r}"
            )
        adjusted_t0[ln - 1] = rep["new"]
        replaced_map[ln] = (rep["old"], rep["new"])

    # Verify blocks partition every non-empty line in keptLineRange without overlap or gap.
    blocks = cfg["blocks"]
    covered = set()
    blocks_by_target: dict[str, list[dict]] = collections.defaultdict(list)
    for b in blocks:
        s, e = b["t0StartLine"], b["t0EndLine"]
        if s < kept_start or e > kept_end or s > e:
            sys.exit(f"FAIL [{part_name}]: invalid block range {b['id']} [{s}, {e}]")
        for ln in range(s, e + 1):
            if ln in covered:
                sys.exit(f"FAIL [{part_name}]: overlapping block coverage at T0 line {ln}")
            covered.add(ln)
        blocks_by_target[b["target"]].append(b)

    # Every non-empty line in kept_start..kept_end (except closing '}' accounted in closingLine) must be covered.
    uncovered_nonempty = [
        ln
        for ln in range(kept_start, kept_end + 1)
        if ln not in covered and adjusted_t0[ln - 1].strip() != ""
    ]
    if uncovered_nonempty:
        sys.exit(
            f"FAIL [{part_name}]: uncovered non-empty T0 lines: {uncovered_nonempty[:10]}"
        )

    # Read all HEAD target files and verify contiguous block presence + increasing T0 order.
    target_lines_map: dict[str, list[str]] = {}
    target_blobs: dict[str, str] = {}
    block_proofs = []

    expected_multiset = collections.Counter()
    actual_multiset = collections.Counter()

    # Build expected multiset from T0 kept lines (with permitted replacements) + permitted helper + permitted headers/footers.
    for ln in range(kept_start, kept_end + 1):
        line = adjusted_t0[ln - 1]
        if line != "":
            expected_multiset[line] += 1

    added_helper = cfg.get("addedHelper")
    if added_helper:
        for line in added_helper["lines"]:
            if line != "":
                expected_multiset[line] += 1

    file_headers = cfg.get("fileHeaders", {})
    file_footers = cfg.get("fileFooters", {})
    for target, hdr_lines in file_headers.items():
        for line in hdr_lines:
            if line != "":
                expected_multiset[line] += 1
    for target, ftr_lines in file_footers.items():
        for line in ftr_lines:
            if line != "":
                expected_multiset[line] += 1

    for target in sorted(blocks_by_target.keys()):
        t_blob, t_lines = git_blob_lines(repo, head_rev, target)
        target_blobs[target] = t_blob
        target_lines_map[target] = t_lines
        for line in t_lines:
            if line != "":
                actual_multiset[line] += 1

        # Verify declared file header (for newly created partial files) is at exact top of file.
        pos = 0
        if target in file_headers:
            hdr = file_headers[target]
            if t_lines[: len(hdr)] != hdr:
                sys.exit(f"FAIL [{part_name}]: header mismatch in {target}")
            pos = len(hdr)

        # Verify declared blocks appear verbatim, contiguously, and in strictly increasing T0 order.
        t_blocks = blocks_by_target[target]
        prev_t0_start = 0
        for b in t_blocks:
            s, e = b["t0StartLine"], b["t0EndLine"]
            if s <= prev_t0_start:
                sys.exit(f"FAIL [{part_name}]: blocks for {target} not in increasing T0 order at {b['id']}")
            prev_t0_start = s
            needle = adjusted_t0[s - 1 : e]
            found_at = find_subsequence(t_lines, needle, pos)
            if found_at == -1:
                sys.exit(
                    f"FAIL [{part_name}]: contiguous block {b['id']} (T0 lines {s}..{e}) not found in order in {target} from line {pos + 1}"
                )
            block_proofs.append(
                {
                    "id": b["id"],
                    "target": target,
                    "t0Lines": [s, e],
                    "headLines": [found_at + 1, found_at + len(needle)],
                    "lineCount": len(needle),
                    "members": b.get("members", []),
                }
            )
            pos = found_at + len(needle)

            if added_helper and added_helper["target"] == target and added_helper["afterBlockId"] == b["id"]:
                h_needle = added_helper["lines"]
                h_at = find_subsequence(t_lines, h_needle, pos)
                if h_at == -1:
                    sys.exit(f"FAIL [{part_name}]: addedHelper not found after {b['id']} in {target}")
                pos = h_at + len(h_needle)

        if target in file_footers:
            ftr = file_footers[target]
            if t_lines[-len(ftr) :] != ftr:
                sys.exit(f"FAIL [{part_name}]: footer mismatch in {target}")

    # Exact multiset comparison (zero extra, zero missing non-empty lines).
    extra = actual_multiset - expected_multiset
    missing = expected_multiset - actual_multiset
    if extra or missing:
        sys.exit(
            f"FAIL [{part_name}]: line-multiset mismatch!\n"
            f"  extra ({sum(extra.values())}): {list(extra.items())[:10]}\n"
            f"  missing ({sum(missing.values())}): {list(missing.items())[:10]}"
        )

    # Part-specific structural invariants.
    invariants = {}
    if part_name == "ui":
        # Spec §5.4: every static readonly field & const stays in MultiRoutes.cs in original order; zero in MultiApi.*.cs.
        static_re = re.compile(r"^\s+private\s+(?:const|static\s+readonly)\s+.*?\b([A-Za-z_][A-Za-z0-9_]*)\s*=")
        t0_statics = [
            m.group(1)
            for ln in range(kept_start, kept_end + 1)
            if (m := static_re.match(adjusted_t0[ln - 1]))
        ]
        routes_path = cfg["primaryTarget"]
        routes_statics = [
            m.group(1)
            for line in target_lines_map[routes_path]
            if (m := static_re.match(line))
        ]
        if routes_statics != t0_statics:
            sys.exit(
                f"FAIL [ui]: static/const field list or order changed in {routes_path}:\n"
                f"  T0={t0_statics}\n  HEAD={routes_statics}"
            )
        for target, t_lines in target_lines_map.items():
            if target == routes_path:
                continue
            bad = [line.strip() for line in t_lines if static_re.match(line)]
            if bad:
                sys.exit(f"FAIL [ui]: forbidden static/const field(s) in {target}: {bad}")
        invariants["staticFieldsInPrimaryOrder"] = routes_statics
        invariants["staticFieldsInOtherPartials"] = 0

    elif part_name == "core":
        # Spec §5.5: zero field declarations, ctors, Advance, or Save/Load in command partials; Advance byte-identical to T0.
        root_path = cfg["primaryTarget"]
        field_re = re.compile(r"^\s+private\s+(?:readonly\s+)?(?!static\s+\w+\s*\()[\w<>,\[\]\?\.\s]+\s+[a-z_][A-Za-z0-9_]*\s*(?:=|;)")
        for target, t_lines in target_lines_map.items():
            if target == root_path:
                continue
            bad_fields = [line.strip() for line in t_lines if field_re.match(line)]
            if bad_fields:
                sys.exit(f"FAIL [core]: forbidden field declaration in {target}: {bad_fields}")
            for forbidden_member in ("internal MultiAreaContinuation(", "Advance(", "CaptureSave(", "RestoreSave("):
                if any(forbidden_member in line for line in t_lines):
                    sys.exit(f"FAIL [core]: forbidden root member {forbidden_member!r} in {target}")
        # Verify Advance lines 985..1155 exist verbatim in root_path.
        adv_slice = t0_lines[984:1155]
        if find_subsequence(target_lines_map[root_path], adv_slice, 0) == -1:
            sys.exit(f"FAIL [core]: Advance member in {root_path} is not byte-identical to T0")
        invariants["advanceByteIdenticalToT0"] = True
        invariants["fieldsInCommandPartials"] = 0

    return {
        "part": part_name,
        "sourcePath": src_path,
        "sourceBlobId": src_blob_id,
        "targetCount": len(target_lines_map),
        "targetBlobs": target_blobs,
        "blockCount": len(block_proofs),
        "nonEmptyLinesVerified": sum(actual_multiset.values()),
        "multisetEqual": True,
        "contiguityAndOrderVerified": True,
        "invariants": invariants,
        "blocks": block_proofs,
    }


def main() -> int:
    ap = argparse.ArgumentParser(description="Verify mechanical S0a file splits against T0.")
    ap.add_argument("--repo", required=True)
    ap.add_argument("--base", required=True, help="T0 commit SHA")
    ap.add_argument("--head", default="HEAD", help="Target revision (default HEAD)")
    ap.add_argument("--table", required=True, help="Path to s0a-move-table.json")
    ap.add_argument("--part", choices=("all", "ui", "core"), default="all")
    ap.add_argument("--report", help="Optional path to write JSON move-proof report")
    args = ap.parse_args()

    table = json.load(open(args.table, "r", encoding="utf-8"))
    parts_cfg = table["parts"]
    selected = list(parts_cfg.keys()) if args.part == "all" else [args.part]
    for p in selected:
        if p not in parts_cfg:
            sys.exit(f"FAIL: requested part {p!r} not present in {args.table}")

    results = {}
    for p in selected:
        results[p] = verify_part(args.repo, args.base, args.head, p, parts_cfg[p])

    report = {
        "base": args.base,
        "head": subprocess.check_output(["git", "-C", args.repo, "rev-parse", args.head], text=True).strip(),
        "table": args.table,
        "parts": results,
    }
    if args.report:
        with open(args.report, "w", encoding="utf-8", newline="\n") as f:
            json.dump(report, f, indent=2)
            f.write("\n")

    summary_bits = [
        f"{p}({r['targetCount']} files, {r['blockCount']} blocks, {r['nonEmptyLinesVerified']} non-empty lines)"
        for p, r in results.items()
    ]
    print("MOVE-PROOF PASS: " + "; ".join(summary_bits))
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
