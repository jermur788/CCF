"""L4 write-observation analyzer for `strace -f -y -qq` output (#643 m-02, #646/#649 fail-closed repair).

Finds every traced mutating file operation and classifies it, failing CLOSED:
  HIT          a successful (or outcome-unknown) mutation whose resolved target lies under the protected directory;
  UNRESOLVED   a successful mutation for which no reliable absolute pathname could be derived — for example a bare,
               undecoded descriptor (`ftruncate(9, 0) = 0`), a relative path with an undecoded dirfd or unknown cwd, or a
               mutator with no recognised path argument at all;
  FAILED_PROTECTED_ATTEMPT  a failed mutating call aimed at the protected directory: not a confirmed write, but reported
               and fatal, because P1 should never even try;
  clean        a successful mutation whose every target resolves outside the protected directory, or a failed call
               elsewhere.
Exit 0 only when there are no HIT, UNRESOLVED or FAILED_PROTECTED_ATTEMPT records.

How targets are derived (one explicit rule per traced mutator, see ARG_SPEC):
  * fd-only mutators (ftruncate, fallocate, fchmod, fchown, fsetxattr, fremovexattr, utimensat(fd, NULL, ...)) use the
    path strace -y decodes for the descriptor (`9</abs/path>`); a bare or non-path descriptor (`9`, `9<pipe:[1]>`) is
    UNRESOLVED;
  * *at calls pair each path string with the dirfd argument immediately before it; a relative path needs a dirfd decoded
    to an absolute path, or bare AT_FDCWD with a known tracked cwd;
  * legacy path calls (mkdir("x"), rename("a","b"), ...) resolve relative paths against each process's tracked cwd
    (chdir/fchdir, fork/clone inheritance) — strace does not decode the cwd for these;
  * write-intent opens also add the kernel-resolved path of the returned descriptor, which catches routes through
    symlinks.

Remaining limitations (also written to the JSON): syscalls outside the traced set (e.g. io_uring, writes through a
descriptor opened before tracing) and processes outside the traced tree are not seen; legacy relative paths are joined
lexically (a symlinked intermediate component is not resolved); a chdir by one thread is applied to that thread id only.

Usage:
  python3 -I -B l4_analyze.py <trace> <protected-dir> <initial-cwd> [--json out.json] [--expect expected.txt]
  python3 -I -B l4_analyze.py --self-test
--expect: every line `<syscall> <path relative to protected-dir>` must match at least one distinct HIT (identity =
syscall + resolved target); a missing identity fails. Used for the decoy self-test, where hits are expected.
"""
import json
import os
import re
import sys
import tempfile

# ---------------------------------------------------------------------------------------------------------- rules
# For each traced mutator, the argument positions that name what is mutated:
#   ("fd", i)          argument i is a file descriptor (target = its decoded path)
#   ("path", i)        argument i is a path string resolved against the process cwd (legacy calls)
#   ("at", d, i)       argument i is a path string relative to dirfd argument d
#   ("ret",)           only the returned descriptor's decoded path identifies the target
ARG_SPEC = {
    "open": [("path", 0)], "creat": [("path", 0)], "mkdir": [("path", 0)], "mknod": [("path", 0)],
    "rmdir": [("path", 0)], "unlink": [("path", 0)], "truncate": [("path", 0)], "chmod": [("path", 0)],
    "chown": [("path", 0)], "lchown": [("path", 0)], "utime": [("path", 0)], "utimes": [("path", 0)],
    "setxattr": [("path", 0)], "lsetxattr": [("path", 0)], "removexattr": [("path", 0)], "lremovexattr": [("path", 0)],
    "rename": [("path", 0), ("path", 1)], "link": [("path", 0), ("path", 1)], "symlink": [("path", 1)],
    "openat": [("at", 0, 1)], "openat2": [("at", 0, 1)], "mkdirat": [("at", 0, 1)], "mknodat": [("at", 0, 1)],
    "unlinkat": [("at", 0, 1)], "fchmodat": [("at", 0, 1)], "fchmodat2": [("at", 0, 1)], "fchownat": [("at", 0, 1)],
    "futimesat": [("at", 0, 1)],
    "renameat": [("at", 0, 1), ("at", 2, 3)], "renameat2": [("at", 0, 1), ("at", 2, 3)],
    "linkat": [("at", 0, 1), ("at", 2, 3)], "symlinkat": [("at", 1, 2)],
    "ftruncate": [("fd", 0)], "fallocate": [("fd", 0)], "fchmod": [("fd", 0)], "fchown": [("fd", 0)],
    "fsetxattr": [("fd", 0)], "fremovexattr": [("fd", 0)],
    "open_by_handle_at": [("ret",)],
    # utimensat is special-cased: (fd, NULL) form is fd-only, otherwise ("at", 0, 1)
    "utimensat": [("at", 0, 1)],
}
OPEN_LIKE = {"open", "openat", "openat2", "open_by_handle_at"}
WRITE_FLAGS = re.compile(r"O_WRONLY|O_RDWR|O_CREAT|O_TRUNC|O_APPEND|O_TMPFILE")
NON_MUTATING_CONTROL = {"chdir", "fchdir", "clone", "clone3", "fork", "vfork"}

LINE = re.compile(r"^(\d+)\s+(.*)$")
CALL = re.compile(r"^([a-z0-9_]+)\((.*)\)\s+=\s+(-?\d+|\?)(?:<([^>]*)>)?(.*)$")
UNFINISHED = re.compile(r"^([a-z0-9_]+)\((.*)\s<unfinished \.\.\.>$")
RESUMED = re.compile(r"^<\.\.\. ([a-z0-9_]+) resumed>(.*)$")
FD_ARG = re.compile(r"^(AT_FDCWD|-?\d+)(?:<(.*)>)?$")
STRING_ARG = re.compile(r'^"((?:[^"\\]|\\.)*)"(?:\.\.\.)?$')

LIMITATIONS = [
    "syscalls outside the traced set (e.g. io_uring, write through an fd opened before tracing) are not seen",
    "processes not descended from the traced command are not seen",
    "legacy relative paths are joined to the tracked cwd lexically; a symlinked intermediate component is not resolved",
    "a chdir by one thread is applied to that thread id only",
]


def split_args(args):
    """Split a strace argument list at top-level commas (quotes, (), [], {}, and <...> decorations are kept whole)."""
    out, depth, cur, i, in_str = [], 0, [], 0, False
    while i < len(args):
        c = args[i]
        if in_str:
            cur.append(c)
            if c == "\\" and i + 1 < len(args):
                cur.append(args[i + 1]); i += 2; continue
            if c == '"':
                in_str = False
        elif c == '"':
            in_str = True; cur.append(c)
        elif c in "([{<":
            depth += 1; cur.append(c)
        elif c in ")]}>":
            depth -= 1; cur.append(c)
        elif c == "," and depth == 0:
            out.append("".join(cur).strip()); cur = []
        else:
            cur.append(c)
        i += 1
    if cur or out:
        out.append("".join(cur).strip())
    return out


def calls(lines):
    pending = {}
    for raw in lines:
        m = LINE.match(raw.rstrip("\n"))
        if not m:
            continue
        pid, rest = int(m.group(1)), m.group(2)
        u = UNFINISHED.match(rest)
        if u:
            pending[pid] = (u.group(1), u.group(2))
            yield pid, u.group(1), u.group(2), None, None, False
            continue
        r = RESUMED.match(rest)
        if r:
            name, args0 = pending.pop(pid, (r.group(1), ""))
            rest = f"{name}({args0}{r.group(2)}"
        c = CALL.match(rest)
        if c:
            yield pid, c.group(1), c.group(2), c.group(3), c.group(4), True


def decoded_abs(arg):
    """Absolute path that strace -y decoded for a descriptor argument, or None (bare fd, pipe/socket, AT_FDCWD w/o path)."""
    m = FD_ARG.match(arg or "")
    if m and m.group(2) and m.group(2).startswith("/"):
        return m.group(2)
    return None


def targets_for(name, argv, retpath, cwd):
    """Returns (targets, reason). reason is non-None when some named target could not be resolved reliably."""
    spec = ARG_SPEC[name]
    if name == "utimensat" and len(argv) > 1 and argv[1] == "NULL":
        spec = [("fd", 0)]
    targets, reasons = [], []
    for rule in spec:
        kind = rule[0]
        if kind == "fd":
            p = decoded_abs(argv[rule[1]] if rule[1] < len(argv) else None)
            if p: targets.append(p)
            else: reasons.append(f"fd argument {rule[1]} not decoded to an absolute path")
        elif kind == "ret":
            pass  # handled below via retpath
        else:
            idx = rule[2] if kind == "at" else rule[1]
            sm = STRING_ARG.match(argv[idx]) if idx < len(argv) else None
            if not sm:
                reasons.append(f"path argument {idx} is not a string")
                continue
            s = sm.group(1)
            if s.startswith("/"):
                targets.append(s)
            elif kind == "at":
                d = argv[rule[1]] if rule[1] < len(argv) else ""
                base = decoded_abs(d)
                if base is None and d == "AT_FDCWD":
                    base = cwd
                if base: targets.append(os.path.join(base, s))
                else: reasons.append(f"relative path with undecoded dirfd '{d}'")
            else:
                if cwd: targets.append(os.path.join(cwd, s))
                else: reasons.append("relative legacy path with unknown cwd")
    if retpath:
        if retpath.startswith("/"):
            targets.append(retpath)
    return [os.path.normpath(t) for t in targets], ("; ".join(reasons) or None)


def analyze(lines, protected, initial):
    protected = os.path.normpath(protected)
    cwd, first_pid, open_clones = {}, None, {}
    hits, unresolved, failed_protected, mutating, failed_other = [], [], [], 0, 0

    def under(p):
        return p == protected or p.startswith(protected + os.sep)

    for pid, name, args, ret, retpath, finished in calls(lines):
        if first_pid is None:
            first_pid = pid
            cwd[pid] = initial
        if pid not in cwd:
            parents = [p for p in open_clones if p in cwd]
            if len(parents) == 1:
                cwd[pid] = cwd[parents[0]]
        if name in ("clone", "clone3", "fork", "vfork"):
            if not finished:
                open_clones[pid] = True
                continue
            open_clones.pop(pid, None)
            if ret and ret.isdigit() and int(ret) > 0 and pid in cwd:
                cwd.setdefault(int(ret), cwd[pid])
            continue
        if not finished:
            continue
        failed = ret is not None and ret.startswith("-")   # "?" (outcome unknown) is treated as success: fail closed
        argv = split_args(args)
        if name == "chdir":
            sm = STRING_ARG.match(argv[0]) if argv else None
            if not failed and sm:
                p, base = sm.group(1), cwd.get(pid)
                if p.startswith("/"): cwd[pid] = os.path.normpath(p)
                elif base: cwd[pid] = os.path.normpath(os.path.join(base, p))
                else: cwd.pop(pid, None)  # unknown from now on: later relative writes become UNRESOLVED
            continue
        if name == "fchdir":
            if not failed:
                p = decoded_abs(argv[0] if argv else None)
                if p: cwd[pid] = os.path.normpath(p)
                else: cwd.pop(pid, None)
            continue
        if name in NON_MUTATING_CONTROL or name not in ARG_SPEC:
            continue
        if name in OPEN_LIKE and not WRITE_FLAGS.search(args):
            continue  # read-only open: not a mutation

        targets, reason = targets_for(name, argv, retpath if name in OPEN_LIKE else None, cwd.get(pid))
        record = {"pid": pid, "call": name, "ret": ret, "targets": sorted(set(targets)), "reason": reason,
                  "args": args[:300]}
        if failed:
            failed_other += 1
            if any(under(t) for t in targets):
                failed_protected.append(record)
            continue
        mutating += 1
        if any(under(t) for t in targets):
            hits.append(record)
        elif reason or not targets:
            record["reason"] = reason or "no reliable pathname for a successful mutation"
            unresolved.append(record)

    return {"protected": protected, "mutatingCallsSucceeded": mutating, "failedMutatingCalls": failed_other,
            "protectedHits": hits, "unresolved": unresolved, "failedProtectedAttempts": failed_protected,
            "limitations": LIMITATIONS}


def check_expected(result, expected_lines):
    """Every expected (syscall, relative target) identity must appear among DISTINCT hits."""
    prot = result["protected"]
    seen = {}
    for h in result["protectedHits"]:
        for t in h["targets"]:
            if t == prot or t.startswith(prot + os.sep):
                key = (h["call"], os.path.relpath(t, prot))
                seen[key] = seen.get(key, 0) + 1
    expected = []
    for line in expected_lines:
        line = line.strip()
        if line and not line.startswith("#"):
            call, rel = line.split(None, 1)
            expected.append((call, os.path.normpath(rel)))
    missing = [e for e in expected if e not in seen]
    extra = sorted(k for k in seen if k not in expected)
    return {"expected": len(expected), "matched": len(expected) - len(missing), "missing": missing,
            "unexpectedIdentities": extra, "sightings": {f"{k[0]} {k[1]}": v for k, v in sorted(seen.items())}}


# ------------------------------------------------------------------------------------------- built-in synthetic tests
def self_test():
    P, O = "/scratch/protected", "/scratch/outside"
    cases = [
        # (label, trace lines, expected (hits, unresolved, failedProtected, mutatingSucceeded))
        ("bare ftruncate fd -> UNRESOLVED", ["100 ftruncate(9, 0) = 0"], (0, 1, 0, 1)),
        ("bare fallocate fd -> UNRESOLVED", ["100 fallocate(9, 0, 0, 4096) = 0"], (0, 1, 0, 1)),
        ("bare fchmod fd -> UNRESOLVED", ["100 fchmod(9, 0600) = 0"], (0, 1, 0, 1)),
        ("bare fchown/fsetxattr/fremovexattr fds -> 3 UNRESOLVED",
         ["100 fchown(9, 1000, 1000) = 0", '100 fsetxattr(9, "user.x", "v", 1, 0) = 0', '100 fremovexattr(9, "user.x") = 0'],
         (0, 3, 0, 3)),
        ("pipe/socket fd (not a path) -> UNRESOLVED", ["100 ftruncate(9<pipe:[12345]>, 0) = 0"], (0, 1, 0, 1)),
        ("bare utimensat(fd, NULL) -> UNRESOLVED", ["100 utimensat(9, NULL, NULL, 0) = 0"], (0, 1, 0, 1)),
        ("decoded protected fd -> HIT", [f"100 ftruncate(9<{P}/file>, 0) = 0"], (1, 0, 0, 1)),
        ("decoded protected utimensat(fd, NULL) -> HIT", [f"100 utimensat(9<{P}/file>, NULL, NULL, 0) = 0"], (1, 0, 0, 1)),
        ("decoded benign fd -> clean", [f"100 ftruncate(9<{O}/file>, 0) = 0"], (0, 0, 0, 1)),
        ("read-only controls -> no mutation",
         [f"100 read(9<{P}/file>, \"abc\", 3) = 3", f'100 openat(AT_FDCWD<{P}>, "file", O_RDONLY|O_CLOEXEC) = 9<{P}/file>',
          f'100 open("{P}/file", O_RDONLY) = 9<{P}/file>'],
         (0, 0, 0, 0)),
        ("relative *at with bare dirfd -> UNRESOLVED", ['100 mkdirat(9, "x", 0777) = 0'], (0, 1, 0, 1)),
        ("relative *at with decoded protected dirfd -> HIT", [f'100 mkdirat(9<{P}>, "x", 0777) = 0'], (1, 0, 0, 1)),
        ("second rename operand with bare dirfd -> UNRESOLVED",
         [f'100 renameat2(5<{O}>, "a", 7, "b", 0) = 0'], (0, 1, 0, 1)),
        ("legacy relative mkdir after chdir into protected -> HIT",
         [f'100 chdir("{P}") = 0', '100 mkdir("x", 0777) = 0'], (1, 0, 0, 1)),
        ("legacy relative mkdir by a pid with unknown cwd (never seen as a clone child) -> UNRESOLVED",
         ['100 chdir("/") = 0', '101 mkdir("x", 0777) = 0'], (0, 1, 0, 1)),
        ("successful open_by_handle_at, undecoded return -> UNRESOLVED",
         ["100 open_by_handle_at(5, {handle_bytes=8, handle_type=1}, O_WRONLY) = 6"], (0, 1, 0, 1)),
        ("successful mutator with no string path -> UNRESOLVED", ["100 mkdir(0x7ffd1234, 0777) = 0"], (0, 1, 0, 1)),
        ("write-open through symlink resolved by returned fd -> HIT",
         [f'100 openat(AT_FDCWD<{O}>, "via/f", O_WRONLY|O_CREAT|O_TRUNC, 0666) = 3<{P}/f>'], (1, 0, 0, 1)),
        ("failed protected write attempt -> FAILED_PROTECTED_ATTEMPT (not a hit)",
         [f'100 openat(AT_FDCWD<{P}>, "f", O_WRONLY|O_CREAT, 0666) = -1 EACCES (Permission denied)'], (0, 0, 1, 0)),
        ("failed write elsewhere -> clean", [f'100 mkdir("{O}/x", 0777) = -1 EEXIST (File exists)'], (0, 0, 0, 0)),
        ("unfinished/resumed bare ftruncate -> UNRESOLVED",
         ["100 ftruncate(9, 0 <unfinished ...>", "100 <... ftruncate resumed>) = 0"], (0, 1, 0, 1)),
        ("outcome unknown (?) on protected path -> HIT (fail closed)", [f'100 mkdir("{P}/y", 0777) = ?'], (1, 0, 0, 1)),
    ]
    failures = 0
    for label, lines, want in cases:
        r = analyze(lines, P, O)
        got = (len(r["protectedHits"]), len(r["unresolved"]), len(r["failedProtectedAttempts"]), r["mutatingCallsSucceeded"])
        ok = got == want
        failures += 0 if ok else 1
        print(f"{'PASS' if ok else 'FAIL'} {label}: hits/unresolved/failedProtected/mutating = {got} (want {want})")
    # --expect identity check: two sightings of ONE operation must not satisfy two expected identities
    r = analyze([f'100 mkdir("{P}/a", 0777) = 0', f'100 mkdir("{P}/a", 0777) = 0'], P, O)
    chk = check_expected(r, ["mkdir a", "mkdir b"])
    ok = chk["matched"] == 1 and chk["missing"] == [("mkdir", "b")] and chk["sightings"] == {"mkdir a": 2}
    failures += 0 if ok else 1
    print(f"{'PASS' if ok else 'FAIL'} duplicate sightings of one op do not satisfy a second expected identity: {chk}")
    print(f"self-test: {len(cases) + 1 - failures}/{len(cases) + 1} passed")
    return 0 if failures == 0 else 1


def main():
    if sys.argv[1:] == ["--self-test"]:
        return self_test()
    trace, protected, initial = sys.argv[1], sys.argv[2], os.path.normpath(sys.argv[3])
    with open(trace, encoding="utf-8", errors="replace") as f:
        result = analyze(f, protected, initial)
    rc = 0 if not (result["protectedHits"] or result["unresolved"] or result["failedProtectedAttempts"]) else 1
    if "--expect" in sys.argv:
        with open(sys.argv[sys.argv.index("--expect") + 1], encoding="utf-8") as f:
            result["expectCheck"] = check_expected(result, f.readlines())
    if "--json" in sys.argv:
        with open(sys.argv[sys.argv.index("--json") + 1], "w", encoding="utf-8") as out:
            json.dump(result, out, indent=1)
    print(f"mutating calls succeeded={result['mutatingCallsSucceeded']} failed={result['failedMutatingCalls']} "
          f"protected hits={len(result['protectedHits'])} unresolved={len(result['unresolved'])} "
          f"failed protected attempts={len(result['failedProtectedAttempts'])}")
    for h in result["protectedHits"][:20]:
        print("HIT", h["call"], h["targets"])
    for u in result["unresolved"][:20]:
        print("UNRESOLVED", u["call"], u["reason"], "|", u["args"][:160])
    for a in result["failedProtectedAttempts"][:20]:
        print("FAILED_PROTECTED_ATTEMPT", a["call"], a["targets"])
    if "expectCheck" in result:
        e = result["expectCheck"]
        print(f"expected identities matched={e['matched']}/{e['expected']} missing={e['missing']} unexpected={e['unexpectedIdentities']}")
        if e["missing"]:
            return 3
    return rc


if __name__ == "__main__":
    sys.exit(main())
