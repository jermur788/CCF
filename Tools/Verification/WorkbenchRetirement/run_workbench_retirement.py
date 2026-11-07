#!/usr/bin/env python3
"""Stage one disposable gate under an existing exact Unity lease; fail closed and always write receipts.

Supply --expected-head, --expected-branch, --owner, --lock-id and --pass-token explicitly.
The caller acquires/releases the lease. Saves/prefs use an isolated XDG_CONFIG_HOME on Files.
Licence copies are deleted; only SHA prefixes of regular files (never their contents) enter receipts.
Only the three known gates run, each with its own exact terminal PASS marker (TERMINAL_PASS).
After the Editor exits and the staged harness/.meta are removed, the lane must be exactly clean again:
any tracked or untracked drift (for example an Editor rewrite of Packages/manifest.json) is a FAIL,
recorded with paths and integrity hashes in both receipts. Nothing is restored or cleaned automatically.
"""
import argparse
import hashlib
import json
import os
from pathlib import Path
import re
import shutil
import subprocess
import sys
import time

ROOT = Path(__file__).resolve().parents[3]
UNITY = "/media/jer/ZX20/Unity/6000.6.0f1/Editor/Unity"
LOCK = "/media/jer/ZX20/CCF-Ops/scripts/unity-lock"
EVIDENCE = Path("/media/jer/Files/CCF/Verification")
# Each gate's final line on success. Per-check markers such as RACKV2PERSIST_CHECK are not terminal.
TERMINAL_PASS = {
    "WorkbenchRetirementVerification": "WORKBENCH_RETIREMENT_VERIFY_PASS",
    "Enlarged80Verification": "ENLARGED80_VERIFY_PASS",
    "RackV2PersistenceVerification": "RACKV2PERSIST_VERIFY_PASS",
}


def output(cmd, cwd=None):
    return subprocess.check_output(cmd, cwd=cwd, text=True).strip()


def verify_identity(args, clean):
    head = output(["git", "rev-parse", "HEAD"], ROOT)
    branch = output(["git", "branch", "--show-current"], ROOT)
    status = json.loads(output([sys.executable, "-B", LOCK, "status", "--json"]))
    meta = status.get("metadata") or {}
    if (head != args.expected_head or branch != args.expected_branch
            or status.get("status") != "held" or status.get("unity_editor_pids")
            or meta.get("head") != head or meta.get("branch") != branch
            or Path(meta.get("lane", "/")).resolve() != ROOT
            or meta.get("task_id") != args.task_id or meta.get("owner") != args.owner
            or meta.get("lock_id") != args.lock_id):
        raise RuntimeError("OWNERSHIP CONFLICT: actual lease/branch/HEAD does not match the requested identity")
    if clean and output(["git", "status", "--porcelain=v1", "-uall"], ROOT):
        raise RuntimeError("DIRTY LANE: refuse staging")
    return dict(head=head, branch=branch, lock_id=meta["lock_id"], task_id=meta["task_id"], owner=meta["owner"])


def lane_status():
    # -z keeps leading status columns and unusual path bytes intact (plain output() strips the first space).
    return subprocess.check_output(["git", "status", "--porcelain=v1", "-uall", "-z"], cwd=ROOT)


def postrun_drift():
    """Every path git reports after the run, with hashes only (never file contents)."""
    fields = lane_status().decode("utf-8", "surrogateescape").split("\0")
    entries = []
    i = 0
    while i < len(fields):
        item = fields[i]
        i += 1
        if not item:
            continue
        code, path = item[:2], item[3:]
        entry = dict(status=code, path=path)
        if "R" in code or "C" in code:  # porcelain -z gives the rename/copy source as the next field
            entry["from"] = fields[i]
            i += 1
        local = ROOT / path
        if local.is_symlink():
            entry["worktree"] = "symlink"
            entry["worktree_sha256"] = hashlib.sha256(os.readlink(local).encode("utf-8", "surrogateescape")).hexdigest()
        elif local.is_file():
            entry["worktree"] = "file"
            entry["worktree_sha256"] = hashlib.sha256(local.read_bytes()).hexdigest()
        elif local.is_dir():  # e.g. a nested repository; its contents are not read
            entry["worktree"] = "directory"
            entry["worktree_sha256"] = None
        else:
            entry["worktree"] = "absent"
            entry["worktree_sha256"] = None
        head = subprocess.run(["git", "rev-parse", "--verify", "--quiet", "HEAD:" + path],
                              cwd=ROOT, capture_output=True, text=True)
        entry["head_blob"] = head.stdout.strip() if head.returncode == 0 else None
        entries.append(entry)
    return entries


def write_receipt(path, body):
    # Write a temporary file and rename it, so a failed write never leaves a partial receipt behind.
    temporary = path.with_name(path.name + ".partial")
    try:
        temporary.write_text(body)
        os.replace(temporary, path)
    except Exception:
        try:
            temporary.unlink(missing_ok=True)
        except Exception:
            pass
        raise


def licence_hashes(lic):
    # Unity may create licenses/packages/. Hash regular files recursively, never read a directory or symlink.
    return [hashlib.sha256(p.read_bytes()).hexdigest()[:12]
            for p in sorted(lic.rglob("*")) if not p.is_symlink() and p.is_file()]


def verdict(code, text, pass_token):
    failures = []
    if code != 0:
        failures.append("editor_exit_nonzero")
    # Exactly one line must start with the exact terminal token followed by whitespace or end of line.
    terminal = re.findall(r"^" + re.escape(pass_token) + r"(?=[ \t]|$)", text, re.MULTILINE)
    if not terminal:
        failures.append("missing_terminal_pass")
    elif len(terminal) > 1:
        failures.append("duplicate_terminal_pass")
    if pass_token.endswith("_PASS") and pass_token[:-len("_PASS")] + "_FAIL" in text:
        failures.append("gate_fail_marker")
    if re.search(r"\b(?:[A-Z][A-Z0-9_]*_)?FAIL\b", text):
        failures.append("fail_marker")
    if re.search(r"error CS\d+|compilation failed|scripts have compiler errors", text, re.IGNORECASE):
        failures.append("compiler_error")
    return failures


def main(argv=None):
    ap = argparse.ArgumentParser(description=__doc__)
    ap.add_argument("gate")
    ap.add_argument("source", type=Path)
    ap.add_argument("out", type=Path)
    for name in ("expected-head", "expected-branch", "owner", "lock-id", "pass-token"):
        ap.add_argument("--" + name, required=True)
    ap.add_argument("--task-id", default="287")
    ap.add_argument("--env", nargs="*", default=[])
    args = ap.parse_args(argv)
    if not re.fullmatch(r"[0-9a-f]{40}", args.expected_head):
        ap.error("expected-head must be a full SHA")
    if not re.fullmatch(r"[A-Za-z][A-Za-z0-9_]*", args.gate):
        ap.error("gate must be a simple class name")
    if args.gate == "WorkbenchRetirementVerification" and args.pass_token != "WORKBENCH_RETIREMENT_VERIFY_PASS":
        ap.error("Workbench requires its terminal WORKBENCH_RETIREMENT_VERIFY_PASS marker")
    if args.gate not in TERMINAL_PASS:
        ap.error("unknown gate: only " + ", ".join(sorted(TERMINAL_PASS)) + " have a known terminal PASS marker")
    if args.pass_token != TERMINAL_PASS[args.gate]:
        ap.error(args.gate + " requires its exact terminal " + TERMINAL_PASS[args.gate] + " marker")
    out = args.out.resolve()
    if not out.is_relative_to(EVIDENCE.resolve()):
        ap.error("evidence must live under /media/jer/Files/CCF/Verification/")
    out.mkdir(parents=True, exist_ok=False)
    config = out / "config"
    lic = config / "unity3d/Unity/licenses"
    target = ROOT / "Assets/ForestPrototype" / args.source.name
    log = out / (args.gate + ".log")
    started = time.monotonic()
    code = None
    failures, hashes = [], []
    identity = None
    staged = False
    admitted = False
    drift, drift_state = [], "not_checked_no_clean_admission"
    try:
        identity = verify_identity(args, clean=True)
        admitted = True  # the lane was exactly clean here, so any later status output is drift from this run
        source = args.source.resolve()
        if not source.is_relative_to(ROOT / "Tools/Verification") or source.suffix != ".cs":
            raise RuntimeError("source must be candidate verifier C# under Tools/Verification")
        output(["git", "ls-files", "--error-unmatch", str(source.relative_to(ROOT))], ROOT)
        if target.exists() or Path(str(target) + ".meta").exists():
            raise RuntimeError("Existing staged harness or meta")
        extra = dict(item.split("=", 1) for item in args.env)
        if set(extra) & {"XDG_CONFIG_HOME", "HOME", "XDG_DATA_HOME"}:
            raise RuntimeError("environment must not override isolated save/prefs roots")
        choices = [Path.home() / ".config/unity3d/Unity/licenses",
                   Path.home() / ".var/app/com.unity.UnityHub/config/unity3d/Unity/licenses"]
        available = [p for p in choices if (p / "UnityEntitlementLicense.xml").is_file()]
        if not available:
            raise RuntimeError("No Unity licence file available")
        selected = max(available, key=lambda p: (p / "UnityEntitlementLicense.xml").stat().st_mtime)
        shutil.copytree(selected, lic)
        env = dict(os.environ, **extra)
        env.update(XDG_CONFIG_HOME=str(config), DISPLAY=os.environ.get("DISPLAY", ":0"))
        for key in ("CCF_STAND_GEOMETRY", "TMPDIR", "TMP", "TEMP", "XDG_CACHE_HOME"):
            if key not in extra:
                env.pop(key, None)
        cmd = [UNITY, "-projectPath", str(ROOT), "-job-worker-count", "2", "-executeMethod", args.gate + ".Begin",
               "-batchmode", "-nographics", "-logFile", str(log)]
        staged = True  # also remove a partial copy if copy2 fails
        shutil.copy2(source, target)
        try:
            code = subprocess.run(cmd, cwd=ROOT, env=env, timeout=2700).returncode
        except subprocess.TimeoutExpired:
            code = 124
        verify_identity(args, clean=False)
    except Exception as error:
        # Exception text from licensing/filesystem libraries could contain sensitive data. Store type only.
        failures.append("execution_or_admission_error:" + type(error).__name__)
    finally:
        if staged:
            for path in (target, Path(str(target) + ".meta")):
                try:
                    path.unlink(missing_ok=True)
                except Exception as error:
                    failures.append("harness_cleanup_error:" + type(error).__name__)
        try:
            hashes = licence_hashes(lic)
        except Exception as error:
            failures.append("licence_hash_error:" + type(error).__name__)
        # Hashing failure must never bypass deletion or receipts.
        try:
            if lic.exists():
                shutil.rmtree(lic)
        except Exception as error:
            failures.append("licence_cleanup_error:" + type(error).__name__)
        # Post-run provenance: after removing only what this runner staged, the lane must be clean again.
        # Report drift; never restore, clean or waive it here.
        if admitted:
            try:
                drift = postrun_drift()
                drift_state = "drift" if drift else "clean"
                if drift:
                    failures.append("postrun_lane_drift:" + str(len(drift)))
            except Exception as error:
                drift_state = "error"
                failures.append("postrun_status_error:" + type(error).__name__)
        try:
            text = log.read_text(errors="replace") if log.exists() else ""
        except Exception as error:
            text = ""
            failures.append("log_read_error:" + type(error).__name__)
        failures.extend(verdict(code, text, args.pass_token))
        lines = [line for line in text.splitlines() if any(token in line for token in
                 ("_PASS", "_FAIL", "WORKBENCH_", "error CS", "HASH", "ENLARGED80", "MFM08A"))]
        lines.append("POSTRUN_LANE_STATUS " + drift_state + " entries=" + str(len(drift)))
        lines.extend("POSTRUN_LANE_DRIFT " + item["status"] + " path=" + json.dumps(item["path"])
                     + (" from=" + json.dumps(item["from"]) if "from" in item else "")
                     + " worktree=" + item["worktree"] + " sha256=" + str(item["worktree_sha256"])
                     + " head_blob=" + str(item["head_blob"]) for item in drift)
        result = dict(gate=args.gate, editor_exit_code=code, exit_code=1 if failures else 0,
                      seconds=round(time.monotonic() - started, 1), identity=identity,
                      failures=failures, licence_sha_prefixes=hashes,
                      postrun_lane_status=drift_state, postrun_drift=drift)
        # Attempt both receipts independently. The JSON is serialized only after the markers attempt,
        # so it always states the final exit code and failures, including a markers write failure.
        markers = out / (args.gate + ".markers.txt")
        try:
            write_receipt(markers, "\n".join(lines) + "\n")
        except Exception as error:
            failures.append("receipt_write_error:markers:" + type(error).__name__)
        result["exit_code"] = 1 if failures else 0
        try:
            write_receipt(out / (args.gate + ".json"), json.dumps(result, indent=2) + "\n")
        except Exception as error:
            failures.append("receipt_write_error:json:" + type(error).__name__)
            result["exit_code"] = 1
            # Best effort only: if markers exist, say that the JSON receipt is missing. Invent nothing else.
            try:
                if markers.is_file():
                    with markers.open("a") as handle:
                        handle.write("RUNNER_RECEIPT_ERROR json:" + type(error).__name__ + " runner_exit=1\n")
            except Exception:
                pass
        print(json.dumps(result), flush=True)
    return result["exit_code"]


if __name__ == "__main__":
    sys.exit(main())
