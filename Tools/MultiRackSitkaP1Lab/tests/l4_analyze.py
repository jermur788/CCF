"""L4 write-observation analyzer for `strace -f -y -qq` output (#643 m-02).

Finds every traced mutating file operation whose target lies under a protected directory. Compared with a plain text
grep it also covers:
  * legacy relative-path calls (mkdir("x"), unlink("x"), rename("a","b"), ...), which strace does NOT decode, by tracking
    each process's working directory through chdir/fchdir and fork/clone inheritance;
  * *at calls via the dirfd strace -y decodes (AT_FDCWD</cwd>, 3</dir>);
  * fd-based mutators (ftruncate, fchmod, fallocate, ...) via the decoded descriptor path;
  * write-intent opens via the kernel-resolved path of the RETURNED descriptor (so a route through a symlink is caught).

Conservative: a mutating call whose relative path cannot be resolved (unknown cwd) is reported as UNRESOLVED and makes
the result fail. Limitations that remain are printed with the result (syscalls outside the traced set, processes outside
the traced tree, symlinked intermediate components in legacy relative paths, per-thread cwd changes).

Usage: python3 -I -B l4_analyze.py <trace> <protected-dir> <initial-cwd> [--json out.json]
Exit: 0 no protected hits and no unresolved calls; 1 otherwise.
"""
import json
import os
import re
import sys

MUTATING_ALWAYS = {
    "creat", "rename", "renameat", "renameat2", "unlink", "unlinkat", "rmdir", "mkdir", "mkdirat", "mknod", "mknodat",
    "link", "linkat", "symlink", "symlinkat", "truncate", "ftruncate", "fallocate", "chmod", "fchmod", "fchmodat",
    "fchmodat2", "chown", "fchown", "lchown", "fchownat", "utime", "utimes", "utimensat", "futimesat", "setxattr",
    "lsetxattr", "fsetxattr", "removexattr", "lremovexattr", "fremovexattr",
}
OPEN_LIKE = {"open", "openat", "openat2", "open_by_handle_at"}
WRITE_FLAGS = re.compile(r"O_WRONLY|O_RDWR|O_CREAT|O_TRUNC|O_APPEND|O_TMPFILE")
# Which string arguments are paths to check (index among the string literals of the call). None = all.
PATH_STRINGS = {"symlink": [1], "symlinkat": [1], "setxattr": [0], "lsetxattr": [0], "removexattr": [0], "lremovexattr": [0]}

LINE = re.compile(r"^(\d+)\s+(.*)$")
CALL = re.compile(r"^([a-z0-9_]+)\((.*)\)\s+=\s+(-?\d+|\?)(?:<([^>]*)>)?(.*)$")
UNFINISHED = re.compile(r"^([a-z0-9_]+)\((.*)\s<unfinished \.\.\.>$")
RESUMED = re.compile(r"^<\.\.\. ([a-z0-9_]+) resumed>(.*)$")
TOKEN = re.compile(r'(AT_FDCWD|-?\d+)<([^>]*)>|"((?:[^"\\]|\\.)*)"')


def calls(path):
    pending = {}
    with open(path, encoding="utf-8", errors="replace") as f:
        for raw in f:
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


def main():
    trace, protected, initial = sys.argv[1], os.path.normpath(sys.argv[2]), os.path.normpath(sys.argv[3])
    cwd = {}
    first_pid = None
    open_clones = {}  # parent pid -> True while a clone/fork is unfinished (its child may print before the return)
    hits, unresolved, mutating = [], [], 0

    def under(p):
        p = os.path.normpath(p)
        return p == protected or p.startswith(protected + os.sep)

    for pid, name, args, ret, retpath, finished in calls(trace):
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
        ok = ret not in (None, "?") and not ret.startswith("-")
        tokens = TOKEN.findall(args)
        if name == "chdir":
            if ok and tokens and tokens[0][2]:
                p, base = tokens[0][2], cwd.get(pid)
                if p.startswith("/"):
                    cwd[pid] = os.path.normpath(p)
                elif base:
                    cwd[pid] = os.path.normpath(os.path.join(base, p))
                else:
                    cwd.pop(pid, None)  # unknown from now on: later relative writes become UNRESOLVED
            continue
        if name == "fchdir":
            if ok and tokens and tokens[0][1]:
                cwd[pid] = os.path.normpath(tokens[0][1])
            continue

        is_open = name in OPEN_LIKE
        if is_open and not WRITE_FLAGS.search(args):
            continue
        if not is_open and name not in MUTATING_ALWAYS:
            continue
        mutating += 1
        targets, unknown = [], False
        last_dir = None
        string_index = -1
        wanted = PATH_STRINGS.get(name)
        for fd, fdpath, s in tokens:
            if fdpath:
                last_dir = fdpath
                if name in ("ftruncate", "fallocate", "fchmod", "fchown", "fsetxattr", "fremovexattr") or (name == "utimensat" and args.split(",")[1].strip() == "NULL"):
                    targets.append(fdpath)
                continue
            string_index += 1
            if wanted is not None and string_index not in wanted:
                last_dir = None
                continue
            if s.startswith("/"):
                targets.append(s)
            elif name.endswith("at") or name in ("openat2", "renameat2", "fchmodat2", "open_by_handle_at"):
                if last_dir:
                    targets.append(os.path.join(last_dir, s))
                else:
                    unknown = True
            else:
                base = cwd.get(pid)
                if base:
                    targets.append(os.path.join(base, s))
                else:
                    unknown = True
            last_dir = None
        if retpath and is_open:
            targets.append(retpath)
        record = {"pid": pid, "call": name, "ret": ret, "targets": sorted(set(os.path.normpath(t) for t in targets)),
                  "args": args[:300]}
        if any(under(t) for t in targets):
            hits.append(record)
        elif unknown:
            unresolved.append(record)

    result = {"protected": protected, "mutatingCallsSeen": mutating, "protectedHits": hits, "unresolved": unresolved,
              "limitations": [
                  "syscalls outside the traced set (e.g. io_uring, write through an fd opened before tracing) are not seen",
                  "processes not descended from the traced command are not seen",
                  "legacy relative paths are joined to the tracked cwd lexically; a symlinked intermediate component is not resolved",
                  "a chdir by one thread is applied to that thread id only",
              ]}
    if "--json" in sys.argv:
        with open(sys.argv[sys.argv.index("--json") + 1], "w", encoding="utf-8") as out:
            json.dump(result, out, indent=1)
    print(f"mutating calls seen={mutating} protected hits={len(hits)} unresolved={len(unresolved)}")
    for h in hits[:20]:
        print("HIT", h["call"], h["targets"])
    for u in unresolved[:20]:
        print("UNRESOLVED", u["call"], u["args"][:160])
    return 0 if not hits and not unresolved else 1


if __name__ == "__main__":
    sys.exit(main())
