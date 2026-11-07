#!/usr/bin/env python3
"""Stage one disposable gate under an existing exact Unity lease; fail closed and always write receipts.

Supply --expected-head, --expected-branch, --owner, --lock-id and --pass-token explicitly.
The caller acquires/releases the lease. Saves/prefs use an isolated XDG_CONFIG_HOME on Files.
Licence copies are deleted; only SHA prefixes of regular files (never their contents) enter receipts.
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


def licence_hashes(lic):
    # Unity may create licenses/packages/. Hash regular files recursively, never read a directory or symlink.
    return [hashlib.sha256(p.read_bytes()).hexdigest()[:12]
            for p in sorted(lic.rglob("*")) if not p.is_symlink() and p.is_file()]


def verdict(code, text, pass_token):
    failures = []
    if code != 0:
        failures.append("editor_exit_nonzero")
    if not re.search(r"^" + re.escape(pass_token) + r"\b", text, re.MULTILINE):
        failures.append("missing_terminal_pass")
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
    try:
        identity = verify_identity(args, clean=True)
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
        try:
            text = log.read_text(errors="replace") if log.exists() else ""
        except Exception as error:
            text = ""
            failures.append("log_read_error:" + type(error).__name__)
        failures.extend(verdict(code, text, args.pass_token))
        lines = [line for line in text.splitlines() if any(token in line for token in
                 ("_PASS", "_FAIL", "WORKBENCH_", "error CS", "HASH", "ENLARGED80", "MFM08A"))]
        result = dict(gate=args.gate, editor_exit_code=code, exit_code=1 if failures else 0,
                      seconds=round(time.monotonic() - started, 1), identity=identity,
                      failures=failures, licence_sha_prefixes=hashes)
        # Attempt both receipts independently, even if one cannot be written.
        for path, body in ((out / (args.gate + ".markers.txt"), "\n".join(lines) + "\n"),
                           (out / (args.gate + ".json"), json.dumps(result, indent=2) + "\n")):
            try:
                path.write_text(body)
            except Exception as error:
                failures.append("receipt_write_error:" + type(error).__name__)
                result["exit_code"] = 1
        print(json.dumps(result), flush=True)
    return result["exit_code"]


if __name__ == "__main__":
    sys.exit(main())
