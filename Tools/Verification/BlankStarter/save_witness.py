#!/usr/bin/env python3
"""Run the #857 blank trace with a fixture only if the authorised live slot is absent."""

import hashlib
import json
import os
from pathlib import Path
import stat
import subprocess
import sys

SAVE_DIR = Path("/home/jer/.config/unity3d/DefaultCompany/CCF")
LIVE = SAVE_DIR / "forest-save.json"
BACKUP = SAVE_DIR / "save-backup-20261104/forest-save.json"
SLOT_NAMES = ("forest-save.json", "forest-save.json.tmp", "forest-save.json.bak")
EVIDENCE = Path("/media/jer/Files/CCF/Verification/CCF-857-BlankStarter-20261120")


def read_noatime(path):
    with os.fdopen(os.open(path, os.O_RDONLY | os.O_NOATIME | os.O_NOFOLLOW), "rb") as stream:
        return stream.read()


def snapshot():
    result = {}
    for path in sorted(SAVE_DIR.rglob("*")):
        info = path.lstat()
        row = {"mode": info.st_mode, "mtime_ns": info.st_mtime_ns, "uid": info.st_uid, "gid": info.st_gid}
        if stat.S_ISREG(info.st_mode):
            row["sha256"] = hashlib.sha256(read_noatime(path)).hexdigest()
        elif stat.S_ISLNK(info.st_mode):
            row["target"] = os.readlink(path)
        result[str(path.relative_to(SAVE_DIR))] = row
    return result


def record(path, value):
    path.write_text(json.dumps(value, sort_keys=True, indent=2) + "\n", encoding="utf-8")


def shadow_slots(slot):
    slot = slot.resolve(strict=True)
    if EVIDENCE not in slot.parents:
        sys.exit("STOP: this operation requires an isolated witness directory.")
    result = {}
    for name in SLOT_NAMES:
        path = slot / name
        info = path.lstat()
        if not stat.S_ISREG(info.st_mode):
            sys.exit(f"STOP: {name} is not a regular witness file.")
        result[name] = {"sha256": hashlib.sha256(read_noatime(path)).hexdigest(),
                        "size": info.st_size, "mode": f"{stat.S_IMODE(info.st_mode):04o}",
                        "mtime_ns": info.st_mtime_ns}
    if len({v["sha256"] for v in result.values()}) != len(SLOT_NAMES):
        sys.exit("STOP: the three witnesses must have distinct contents.")
    return result


def main():
    output = Path(sys.argv[1]).resolve(strict=True)
    if len(sys.argv) == 5 and sys.argv[2] == "--shadow-slots":
        if EVIDENCE not in output.parents or sys.argv[4] not in ("before", "after"):
            sys.exit("STOP: invalid evidence directory or snapshot phase.")
        record(output / f"shadow-slots-{sys.argv[4]}.json", shadow_slots(Path(sys.argv[3])))
        return 0
    if sys.argv[2:] == ["--snapshot"]:
        record(output / "host-directory-snapshot.json", snapshot())
        return 0
    if os.environ.get("CCF857_LIVE_CUSTODY_APPROVED") != "byte-exact-backup-and-isolation":
        sys.exit("STOP: do not repeat the original host launch. Use run-native.sh exact-shadow.")
    if sys.argv[2] != "--" or len(sys.argv) < 4:
        sys.exit("Usage: save_witness.py EVIDENCE_DIR -- <Flatpak strace + Unity command>")
    if LIVE.exists() or LIVE.is_symlink():
        sys.exit("STOP: the original live save slot is not absent.")
    baseline = snapshot()
    directory_times = SAVE_DIR.stat()
    fixture = read_noatime(BACKUP)
    preserved = output / "historical-v20-fixture.json"
    if preserved.exists():
        if preserved.read_bytes() != fixture:
            sys.exit("STOP: preserved historical fixture no longer matches its source.")
    else:
        with preserved.open("xb") as stream:
            stream.write(fixture)
    record(output / "save-prior.json", baseline)
    # Exclusive create: never replace an existing file, even if it appeared after the prior-state check.
    with LIVE.open("xb") as stream:
        stream.write(fixture)
        witness_inode = os.fstat(stream.fileno()).st_ino
    staged = snapshot()
    record(output / "save-staged.json", staged)
    try:
        completed = subprocess.run(sys.argv[3:], check=False)
    finally:
        after = snapshot()
        record(output / "save-after-unity.json", after)
        # Never unlink an unknown replacement or a file with changed bytes.
        if LIVE.is_file() and LIVE.lstat().st_ino == witness_inode and read_noatime(LIVE) == fixture:
            LIVE.unlink()
            os.utime(SAVE_DIR, ns=(directory_times.st_atime_ns, directory_times.st_mtime_ns))
        restored = snapshot()
        record(output / "save-restored.json", restored)
        if staged != after or baseline != restored:
            sys.exit("STOP: filesystem witness changed. Inspect the recorded snapshots before proceeding.")
    trace = output / "blank-save-access.strace"
    if not trace.is_file() or trace.stat().st_size != 0:
        sys.exit("STOP: the path-filtered trace is missing or records access; inspect it before proceeding.")
    return completed.returncode


if __name__ == "__main__":
    sys.exit(main())
