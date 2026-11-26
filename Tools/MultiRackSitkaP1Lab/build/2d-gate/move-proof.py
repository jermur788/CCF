#!/usr/bin/env python3
"""
SPEC-multi-2d-s0-seam-extraction-V1 §5.4–§5.6 & §6.2–§6.3 — Mechanical move-proof & S0b Advance extraction verifier
(Python 3 stdlib only).

Proves zero logic delta for S0a mechanical splits and S0b AnnualPipeline extraction:
  (a) Exact non-empty line-multiset equality between T0 source file(s) and HEAD target files for all mechanically moved
      blocks, accounting ONLY for the permitted spec §5.2 (i)-(iv) additions/removals (and, when S0b Advance extraction
      is active, isolating the Advance member span for dedicated M1/M2 verification).
  (b) Contiguous verbatim block moves preserving original relative order within every target file.
  (c) UI static-field ordering invariant: all static readonly fields and constants stay in MultiRoutes.cs
      in original T0 order, with zero static readonly fields in MultiApi.*.cs.
  (d) Core root invariant: all fields, constructors, Advance, and Save/Load stay in MultiAreaContinuation.cs,
      with zero field declarations in MultiAreaContinuation.Commands.*.cs.
  (e) S0b Advance invariant: either Advance is byte-identical to T0 (RETAINED), or Advance routes through the fixed
      6-step compile-time AnnualPipeline.Steps using ONLY relocations M1 (deadwood hoist) and M2 (year-record placement),
      with both reserved steps provably empty no-ops and MultiAreaContinuation remaining sole holder of sync and root.
"""
import argparse
import collections
import json
import os
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


def extract_advance_slice(lines: list[str], start_marker: str, end_marker: str, label: str) -> tuple[int, int, list[str]]:
    start_idx = next((i for i, ln in enumerate(lines) if ln == start_marker), -1)
    end_idx = next((i for i, ln in enumerate(lines) if ln == end_marker), -1)
    if start_idx == -1 or end_idx == -1 or end_idx <= start_idx:
        sys.exit(f"FAIL [{label}]: unable to locate Advance span between {start_marker!r} and {end_marker!r}")
    return start_idx, end_idx, lines[start_idx:end_idx]


def verify_advance_extraction(
    repo: str,
    base_rev: str,
    head_rev: str,
    root_path: str,
    head_root_lines: list[str],
    t0_lines: list[str],
    ext_cfg: dict,
    allowed_relocations_cli: set[str] | None = None,
) -> dict:
    mode = ext_cfg["mode"]
    start_marker = ext_cfg["startMarker"]
    end_marker = ext_cfg["endMarker"]
    s0, e0, head_adv = extract_advance_slice(head_root_lines, start_marker, end_marker, "core-advance-head")

    t0_s = ext_cfg["t0StartLine"]
    t0_e = ext_cfg["t0EndLine"]
    t0_adv = t0_lines[t0_s - 1 : t0_e]

    # Also compare non-Advance regions of MultiAreaContinuation.cs against base_rev when checking a member directly.
    base_blob, base_root_lines = git_blob_lines(repo, base_rev, root_path)
    if base_rev != ext_cfg.get("t0Commit"):
        bs0, be0, base_adv = extract_advance_slice(base_root_lines, start_marker, end_marker, "core-advance-base")
        if base_root_lines[:bs0] != head_root_lines[:s0]:
            sys.exit("FAIL [advance]: lines before Advance in MultiAreaContinuation.cs differ from base")
        if base_root_lines[be0:] != head_root_lines[e0:]:
            sys.exit("FAIL [advance]: lines after Advance in MultiAreaContinuation.cs differ from base")
    else:
        base_adv = t0_adv

    if mode == "retained":
        if find_subsequence(head_root_lines, t0_adv, 0) == -1:
            sys.exit(f"FAIL [advance]: mode=retained but Advance in {root_path} is not byte-identical to T0")
        return {
            "mode": "retained",
            "advanceByteIdenticalToT0": True,
            "baseBlobId": base_blob,
        }

    if mode != "extracted-m1-m2":
        sys.exit(f"FAIL [advance]: unsupported advanceExtraction mode {mode!r}")

    required_relocs = set(ext_cfg.get("allowedRelocations", ["M1", "M2"]))
    if required_relocs != {"M1", "M2"}:
        sys.exit(f"FAIL [advance]: spec §6.2 permits ONLY M1 and M2 relocations, got {sorted(required_relocs)}")
    if allowed_relocations_cli is not None and not required_relocs.issubset(allowed_relocations_cli):
        sys.exit(
            f"FAIL [advance]: extraction requires relocations {sorted(required_relocs)} "
            f"but --allow only permitted {sorted(allowed_relocations_cli)}"
        )
    if allowed_relocations_cli is not None and not allowed_relocations_cli.issubset({"M1", "M2"}):
        sys.exit(f"FAIL [advance]: unsupported relocation requested in --allow: {sorted(allowed_relocations_cli)}")

    adv_text = "\n".join(head_adv)
    # 1. MultiAreaContinuation.Advance retains single lock, all guards, pipeline loop, beforeCommit, and single root commit.
    required_adv_snippets = [
        "lock (sync)",
        "RequireFence(fence);",
        'throw new P1Refusal("WorkPendingConsentRequired",',
        'throw new P1Refusal("ContinuationNoLivingStems",',
        "foreach (var step in AnnualPipeline.Steps)",
        "step.Run(progress);",
        "beforeCommit?.Invoke();",
        "root = root with",
        "return WithView(null, new MultiAreaAnnualResult(year, checked(year + 1), Array.AsReadOnly(progress.Reports)));",
    ]
    for snip in required_adv_snippets:
        if snip not in adv_text:
            sys.exit(f"FAIL [advance]: MultiAreaContinuation.Advance missing required invariant snippet: {snip!r}")

    # Verify single root assignment in Advance.
    root_assigns = re.findall(r"\broot\s*=\s*", adv_text)
    if len(root_assigns) != 1:
        sys.exit(f"FAIL [advance]: expected exactly 1 root assignment in Advance, found {len(root_assigns)}")

    # 2. Verify AnnualPipeline.cs structure, fixed compile-time step order, no locks/root/reflection, and provable no-ops.
    pipe_path = ext_cfg["pipelineFile"]
    pipe_blob, pipe_lines = git_blob_lines(repo, head_rev, pipe_path)
    pipe_text = "\n".join(pipe_lines)

    for forbidden in ("lock (", "lock(", "root =", "System.Reflection", "Activator.", "Assembly.", "Register("):
        if forbidden in pipe_text:
            sys.exit(f"FAIL [advance]: forbidden construct {forbidden!r} found in {pipe_path}")

    steps_m = re.search(
        r"internal\s+static\s+readonly\s+IAnnualStep\[\]\s+Steps\s*=\s*\{(.*?)\};",
        pipe_text,
        re.DOTALL,
    )
    if not steps_m:
        sys.exit(f"FAIL [advance]: AnnualPipeline.Steps compile-time array missing in {pipe_path}")
    actual_steps = [
        tok.strip().rstrip(",")
        for tok in steps_m.group(1).splitlines()
        if tok.strip() and not tok.strip().startswith("//")
    ]
    expected_steps = ext_cfg["expectedSteps"]
    if actual_steps != expected_steps:
        sys.exit(
            f"FAIL [advance]: AnnualPipeline.Steps mismatch:\n"
            f"  actual={actual_steps}\n  expected={expected_steps}"
        )

    # 3. Verify reserved steps are provably empty no-ops (T6).
    for cls_name in ext_cfg["reservedNoOpSteps"]:
        cls_m = re.search(
            rf"internal\s+sealed\s+class\s+{re.escape(cls_name)}\s*:\s*IAnnualStep\s*\{{(.*?)^\}}",
            pipe_text,
            re.DOTALL | re.MULTILINE,
        )
        if not cls_m:
            sys.exit(f"FAIL [advance]: reserved step class {cls_name} not found in {pipe_path}")
        if not re.search(r"public\s+void\s+Run\s*\(\s*YearInProgress\s+\w+\s*\)\s*\{\s*\}", cls_m.group(1)):
            sys.exit(f"FAIL [advance]: reserved step {cls_name}.Run is not a provably empty no-op")

    # 4. Verify M1 (DeadwoodStep) and M2 (YearRecordStep) bodies exist intact in their named step classes.
    m1_m = re.search(
        r"internal\s+sealed\s+class\s+DeadwoodStep\s*:\s*IAnnualStep\s*\{(.*?)^\}",
        pipe_text,
        re.DOTALL | re.MULTILINE,
    )
    if not m1_m or "FallenDeadwoodRecord.IdFor(progress.NextDeadwood++)" not in m1_m.group(1) or "foreach (var death in report.Deaths)" not in m1_m.group(1):
        sys.exit("FAIL [advance]: M1 DeadwoodStep missing canonical deadwood recording loop")

    m2_m = re.search(
        r"internal\s+sealed\s+class\s+YearRecordStep\s*:\s*IAnnualStep\s*\{(.*?)^\}",
        pipe_text,
        re.DOTALL | re.MULTILINE,
    )
    if not m2_m or "new MultiAreaYearRecord(progress.Year, checked(progress.Year + 1), progress.Revision," not in m2_m.group(1):
        sys.exit("FAIL [advance]: M2 YearRecordStep missing canonical MultiAreaYearRecord construction")

    # 5. Verify AdultGrowthStep no longer duplicates deadwood recording and RegenerationStep retains Stage5/Phase2 logic.
    adult_m = re.search(
        r"internal\s+sealed\s+class\s+AdultGrowthStep\s*:\s*IAnnualStep\s*\{(.*?)^\}",
        pipe_text,
        re.DOTALL | re.MULTILINE,
    )
    if not adult_m or "Phase2MixedDensity.StepRack(" not in adult_m.group(1) or "FallenDeadwoodRecord" in adult_m.group(1):
        sys.exit("FAIL [advance]: AdultGrowthStep structure invalid (must run adult steps without inline deadwood)")

    regen_m = re.search(
        r"internal\s+sealed\s+class\s+RegenerationStep\s*:\s*IAnnualStep\s*\{(.*?)^\}",
        pipe_text,
        re.DOTALL | re.MULTILINE,
    )
    if not regen_m or "Phase2Annual.Step(" not in regen_m.group(1) or "Stage5Annual.Step(" not in regen_m.group(1):
        sys.exit("FAIL [advance]: RegenerationStep missing Phase2Annual.Step / Stage5Annual.Step")

    return {
        "mode": mode,
        "member": ext_cfg["member"],
        "relocations": sorted(required_relocs),
        "pipelineFile": pipe_path,
        "pipelineBlobId": pipe_blob,
        "stepOrder": actual_steps,
        "reservedStepsProvablyNoOp": ext_cfg["reservedNoOpSteps"],
        "singleLockAndRootCommitInContinuation": True,
        "nonAdvanceLinesIdenticalToBase": True,
        "baseAdvanceLineCount": len(base_adv),
        "headAdvanceLineCount": len(head_adv),
    }


def verify_part(repo: str, base_rev: str, head_rev: str, part_name: str, cfg: dict, t0_rev: str) -> dict:
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

    adv_cfg = cfg.get("advanceExtraction")
    excluded_t0_lines: set[int] = set()
    if adv_cfg and adv_cfg.get("mode") == "extracted-m1-m2":
        for ln in range(adv_cfg["t0StartLine"], adv_cfg["t0EndLine"] + 1):
            excluded_t0_lines.add(ln)

    # Apply permitted §5.2(i) line replacements on a working copy of T0 kept lines.
    adjusted_t0 = list(t0_lines)
    for rep in cfg.get("replacedLines", []):
        ln = rep["t0Line"]
        if adjusted_t0[ln - 1] != rep["old"]:
            sys.exit(
                f"FAIL [{part_name}]: T0 line {ln} mismatch for replacement:\n"
                f"  actual={adjusted_t0[ln - 1]!r}\n  expected={rep['old']!r}"
            )
        adjusted_t0[ln - 1] = rep["new"]

    # S0c P6: when s0cModuleRoutes is active on part=ui, expand the two single-line T0 placeholders
    # (MultiPageJs field at staticFieldT0Line and routes.Add("multi.js", ...) at routeAddT0Line) into
    # their exact permitted P6 multi-line replacements, keeping every other line of MultiRoutes.cs & MultiApi.*.cs verbatim.
    s0c_routes_cfg = cfg.get("s0cModuleRoutes")
    expanded_t0_lines: dict[int, list[str]] = {}
    if s0c_routes_cfg and s0c_routes_cfg.get("mode") == "s0c-module-routes":
        expanded_t0_lines[s0c_routes_cfg["staticFieldT0Line"]] = s0c_routes_cfg["newStaticFieldLines"]
        expanded_t0_lines[s0c_routes_cfg["routeAddT0Line"]] = s0c_routes_cfg["newRouteAddLines"]

    def effective_lines(start_ln: int, end_ln: int) -> list[str]:
        out: list[str] = []
        for ln in range(start_ln, end_ln + 1):
            if ln in expanded_t0_lines:
                out.extend(expanded_t0_lines[ln])
            else:
                out.append(adjusted_t0[ln - 1])
        return out

    # Verify blocks partition every non-empty line in keptLineRange (excluding S0b extracted Advance span) without overlap or gap.
    blocks = cfg["blocks"]
    covered = set()
    blocks_by_target: dict[str, list[dict]] = collections.defaultdict(list)
    for b in blocks:
        s, e = b["t0StartLine"], b["t0EndLine"]
        if s < kept_start or e > kept_end or s > e:
            sys.exit(f"FAIL [{part_name}]: invalid block range {b['id']} [{s}, {e}]")
        for ln in range(s, e + 1):
            if ln in covered or ln in excluded_t0_lines:
                sys.exit(f"FAIL [{part_name}]: overlapping block coverage at T0 line {ln}")
            covered.add(ln)
        blocks_by_target[b["target"]].append(b)

    uncovered_nonempty = [
        ln
        for ln in range(kept_start, kept_end + 1)
        if ln not in covered and ln not in excluded_t0_lines and adjusted_t0[ln - 1].strip() != ""
    ]
    if uncovered_nonempty:
        sys.exit(
            f"FAIL [{part_name}]: uncovered non-empty T0 lines: {uncovered_nonempty[:10]}"
        )

    target_lines_map: dict[str, list[str]] = {}
    target_blobs: dict[str, str] = {}
    block_proofs = []

    expected_multiset = collections.Counter()
    actual_multiset = collections.Counter()

    for ln in range(kept_start, kept_end + 1):
        if ln in excluded_t0_lines:
            continue
        for line in (expanded_t0_lines[ln] if ln in expanded_t0_lines else [adjusted_t0[ln - 1]]):
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

        # For primaryTarget when S0b Advance extraction is active, exclude ONLY the delimited Advance span from the
        # S0a verbatim line multiset (that span is verified separately by verify_advance_extraction).
        if adv_cfg and adv_cfg.get("mode") == "extracted-m1-m2" and target == cfg["primaryTarget"]:
            s_adv, e_adv, _ = extract_advance_slice(t_lines, adv_cfg["startMarker"], adv_cfg["endMarker"], f"{part_name}-adv")
            multiset_lines = t_lines[:s_adv] + t_lines[e_adv:]
        else:
            multiset_lines = t_lines

        for line in multiset_lines:
            if line != "":
                actual_multiset[line] += 1

        pos = 0
        if target in file_headers:
            hdr = file_headers[target]
            if t_lines[: len(hdr)] != hdr:
                sys.exit(f"FAIL [{part_name}]: header mismatch in {target}")
            pos = len(hdr)

        t_blocks = blocks_by_target[target]
        prev_t0_start = 0
        for b in t_blocks:
            s, e = b["t0StartLine"], b["t0EndLine"]
            if s <= prev_t0_start:
                sys.exit(f"FAIL [{part_name}]: blocks for {target} not in increasing T0 order at {b['id']}")
            prev_t0_start = s
            needle = effective_lines(s, e)
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

    extra = actual_multiset - expected_multiset
    missing = expected_multiset - actual_multiset
    if extra or missing:
        sys.exit(
            f"FAIL [{part_name}]: line-multiset mismatch!\n"
            f"  extra ({sum(extra.values())}): {list(extra.items())[:10]}\n"
            f"  missing ({sum(missing.values())}): {list(missing.items())[:10]}"
        )

    invariants = {}
    if part_name == "ui":
        static_re = re.compile(r"^\s+private\s+(?:const|static\s+readonly)\s+.*?\b([A-Za-z_][A-Za-z0-9_]*)\s*=")
        t0_statics = [
            m.group(1)
            for ln in range(kept_start, kept_end + 1)
            for line in (expanded_t0_lines[ln] if ln in expanded_t0_lines else [adjusted_t0[ln - 1]])
            if (m := static_re.match(line))
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
        if adv_cfg:
            adv_cfg_with_t0 = dict(adv_cfg, t0Commit=t0_rev)
            invariants["advanceExtraction"] = verify_advance_extraction(
                repo, t0_rev, head_rev, root_path, target_lines_map[root_path], t0_lines, adv_cfg_with_t0
            )
        else:
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
    ap = argparse.ArgumentParser(description="Verify mechanical S0a file splits and S0b Advance extraction.")
    ap.add_argument("--repo", help="Repository root (defaults to git toplevel from script location)")
    ap.add_argument("--base", required=True, help="Base commit SHA (T0 for S0a splits, or S0a trunk tip for --member)")
    ap.add_argument("--head", default="HEAD", help="Target revision (default HEAD)")
    ap.add_argument("--table", help="Path to s0a-move-table.json (defaults beside this script)")
    ap.add_argument("--part", choices=("all", "ui", "core"), default="all")
    ap.add_argument("--member", help="Verify a single extracted member (e.g. MultiAreaContinuation.Advance)")
    ap.add_argument("--allow", help="Comma-separated permitted relocations for --member (e.g. M1,M2)")
    ap.add_argument("--report", help="Optional path to write JSON move-proof report")
    args = ap.parse_args()

    script_dir = os.path.dirname(os.path.abspath(__file__))
    table_path = args.table or os.path.join(script_dir, "s0a-move-table.json")
    repo = args.repo or subprocess.check_output(
        ["git", "-C", script_dir, "rev-parse", "--show-toplevel"], text=True
    ).strip()

    table = json.load(open(table_path, "r", encoding="utf-8"))
    t0_rev = table["t0Commit"]

    if args.member:
        core_cfg = table["parts"]["core"]
        adv_cfg = core_cfg.get("advanceExtraction")
        if not adv_cfg or adv_cfg.get("member") != args.member:
            sys.exit(f"FAIL: unsupported --member {args.member!r}")
        allowed_cli = {x.strip() for x in args.allow.split(",") if x.strip()} if args.allow else set()
        root_path = core_cfg["primaryTarget"]
        _, t0_lines = git_blob_lines(repo, t0_rev, core_cfg["sourcePath"])
        _, head_root_lines = git_blob_lines(repo, args.head, root_path)
        adv_res = verify_advance_extraction(
            repo,
            args.base,
            args.head,
            root_path,
            head_root_lines,
            t0_lines,
            dict(adv_cfg, t0Commit=t0_rev),
            allowed_relocations_cli=allowed_cli,
        )
        report = {
            "base": args.base,
            "head": subprocess.check_output(["git", "-C", repo, "rev-parse", args.head], text=True).strip(),
            "member": args.member,
            "result": adv_res,
        }
        if args.report:
            with open(args.report, "w", encoding="utf-8", newline="\n") as f:
                json.dump(report, f, indent=2)
                f.write("\n")
        print(
            f"MOVE-PROOF MEMBER PASS: {args.member} mode={adv_res['mode']} "
            f"relocations={','.join(adv_res.get('relocations', []))} "
            f"steps={len(adv_res.get('stepOrder', []))}"
        )
        return 0

    parts_cfg = table["parts"]
    selected = list(parts_cfg.keys()) if args.part == "all" else [args.part]
    for p in selected:
        if p not in parts_cfg:
            sys.exit(f"FAIL: requested part {p!r} not present in {table_path}")

    results = {}
    for p in selected:
        results[p] = verify_part(repo, args.base, args.head, p, parts_cfg[p], t0_rev)

    report = {
        "base": args.base,
        "head": subprocess.check_output(["git", "-C", repo, "rev-parse", args.head], text=True).strip(),
        "table": table_path,
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
