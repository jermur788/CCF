# Multi #810 — desktop WSL2/ext4 development worktree restore: RESULT

**19 November 2026. One native desktop orchestrator. Status: `READY FOR SOURCE DEVELOPMENT` / `BUILD ENVIRONMENT NOT QUALIFIED`.**

Restoration only. No code changes, no builds, no tests, no mutants, no accessibility sweep, no Unity, no main integration, no artifact deletion.

- **Issue:** [#810](https://github.com/jermur788/CCF/issues/810) → `HANDED-OFF`
- **Manager handoff:** [#730](https://github.com/jermur788/CCF/issues/730)
- **Source (read-only, untouched):** `C:\CCF-Archives\Multi-20261119` (= `/mnt/c/CCF-Archives/Multi-20261119`)
- **Authority followed:** the archive's own `RESTORE.md` + `MANIFEST.txt`, with #810's stricter staged-import rule

---

## 1. Environment (actual, observed)

| Item | Value |
|---|---|
| Host | Windows 11 Pro `DESKTOP-DL1BQI7` |
| Orchestrator | DeepSeek V4.1 Flash (`opencode-go`), native desktop, one run |
| GitHub identity | `jermur788` — WSL `gh` authenticated (gist, read:org, repo, workflow) |
| WSL | Ubuntu **26.04.1 LTS**, distro `Ubuntu`, state `Running` |
| Kernel | `6.18.58.1-microsoft-standard-WSL2` (WSL2) |
| Filesystem | `/dev/sdd` **ext4** mounted `/` — verified by `df -T`, not assumed |
| Capacity | 1007 G total, 34 G used, **922 G free** before; **919 G free** after |
| Memory | 7.7 GiB total, 7.2 GiB available; no concurrent heavy agent |
| Ownership | `jer` uid=1000 gid=1000 (sudo present); all restored files `jer:jer` |
| Collision | `~/CCF-Dev/Multi-20261119` did **not** exist — new, unique, single-owner |
| Extraction FS | all worktrees on `/dev/sdd ext4`; nothing executable under `/mnt/c` |

WSL/ext4 was present, capacity ample and permissions known, so no owner decision was required and no OS environment was built or resized.

## 2. Archive verification — **PASS**

`sha256sum -c SHA256SUMS` in the copied NTFS archive: **9/9 `OK`** (34.6 s).

```
9e6e2904…77097d  viewer/ccf-year0-viewer-20261119.gitbundle
d17856e9…de43c  viewer/ccf-year0-viewer-evidence-20261119.tar
964f0209…b90aa4  viewer/ccf-year0-viewer-snapshot-20261119.tar
4733de42…5e2e80  engine/ccf-y2-10-engine-20261119.gitbundle
7a1fb72b…c393ee  engine/ccf-y2-10-evidence-20261119.tar
e6b89e57…fe5c5c  engine/ccf-y2-10-lane-snapshot-20261119.tar
324d4c5b…3dc7cd  engine/evidence/HANDOFF.md
fc939e3d…b3a0ac  MANIFEST.txt
62df9614…7f6fc   RESTORE.md
```

## 3. Bundle verification and clone — **PASS**

```
viewer bundle: <bundle> is okay — records a complete history — sha1
engine bundle: <bundle> is okay — records a complete history — sha1
```

`git bundle list-heads` carried exactly the expected refs (the corrected, complete-history viewer bundle — not the broken shallow one):

| Ref | SHA |
|---|---|
| `refs/heads/feature/multi-h0a-core-year0-admission-20261118-v1` | `c4124b63252fb563dc7d84c9fc7fc4e3e0d8d2d1` |
| `refs/heads/feature/multi-h0a-a-year0-viewer-20261118-v1` | `f3537fe37305199a55a06882e32a81487c53e540` |
| `refs/heads/local/h0a-base` | `34185b8b71ba280c89091290345e537ef107bdf4` |
| `refs/heads/feature/multi-789-year2-10-engine-20261119-v1` | `6f9d2e990201bc47e361c3562be9390ed221baf6` |

## 4. Restored worktrees — three actual HEADs **PASS**

```
~/CCF-Dev/Multi-20261119/
├── core/       feature/multi-h0a-core-year0-admission-20261118-v1   c4124b63252fb563dc7d84c9fc7fc4e3e0d8d2d1
├── viewer/     feature/multi-h0a-a-year0-viewer-20261118-v1         f3537fe37305199a55a06882e32a81487c53e540
├── y2engine/   feature/multi-789-year2-10-engine-20261119-v1        6f9d2e990201bc47e361c3562be9390ed221baf6
├── artifacts/  packaged HTML, manifest, contract, reference bundle
├── evidence/   viewer/ + engine/ evidence units
└── _staging/   full staged extractions (kept, see §6)
```

- `core` is the Git store; `viewer` is a **real** `git worktree` of it (`git worktree list` shows both, `.git` pointer regenerated to `/home/jer/CCF-Dev/Multi-20261119/core/.git/worktrees/viewer` — no archived pointer text copied).
- No shared `.git` pointer files from the original host were reused; a fresh store was created from the bundle.
- History preserved: no reset, no rebase, no flattening, no substitution of public `main`.

| Store | `git fsck --full` | Shallow? | Commits reachable |
|---|---|---|---|
| core (viewer store) | **rc=0** | NO | 2082 |
| y2engine | **rc=0** | NO | 14 |

`fsck` prints only `dangling commit` lines in the viewer store — unreferenced objects that a `git bundle` clone legitimately leaves behind (the bundle advertises far more refs than the three checked out). No missing objects, no corruption, no missing prerequisite.

## 5. Snapshot/TAR safety — **PASS**

All four tars inspected before extraction:

| Tar | Members | Absolute | `../` | Links/special | Case collisions | Verdict |
|---|---:|---:|---:|---:|---:|---|
| viewer snapshot | 106 | 0 | 0 | 0 | 0 | SAFE |
| viewer evidence | 1253 | 0 | 0 | 1 symlink | 0 | SAFE |
| engine snapshot | 153 | 0 | 0 | 0 | 0 | SAFE |
| engine evidence | 19 | 0 | 0 | 0 | 0 | SAFE |

The single symlink is `…/784-standard-20261118/q-parent/prof-ff/lock -> 127.0.1.1:+6902`, a Chromium profile lock **inside evidence** — relative sibling name, no absolute path, no traversal, cannot escape its directory. It is not imported into any worktree.

Extraction went to **isolated `_staging/` directories**, never over a Git checkout. Every snapshot file that exists in the checkouts was byte-compared to its `HEAD` blob:

| Tree | Tracked files compared | Byte mismatches | Untracked entries |
|---|---:|---:|---|
| viewer snapshot → `core` | 12 | **0** | pinned fixture, `.git` text |
| viewer snapshot → `viewer` | 17 | **0** | `.git` text |
| engine snapshot → `y2engine` (excl. `bin/`, `obj/`, `.p0ref/`) | 32 | **0** | none |

Only then were authorised items imported. The archived `.git` pointer text was **not** copied; `bin/` and `obj/` build caches were **not** imported into the engine worktree.

## 6. Protected inputs — exact hashes and modes **PASS**

| File | Size | Expected | SHA-256 | Mode |
|---|---:|---|---|---|
| `artifacts/20261119T003202Z/year0-viewer.html` | 127,831 | 127,831 ✅ | `0b69cc1dcf07687c1e56a204797bcd8f32425d1c6a249e4efa09fadce72a67b8` ✅ | 0664 |
| `core/Tools/MultiRackSitkaYear0Viewer/fixtures/p1d-30m-sitka-adult-v1.sitkap1fx.json` | 32,567 | 32,567 ✅ | `bcaaa5c3d9b3d36ad8319ffcc361a6225a02b90281cef8435de7c52f8ff757c4` ✅ | **0444** ✅ |
| `y2engine/Tools/MultiRackSitkaP1Lab/.p0ref/SitkaLab.Core.dll` | 120,832 | 120,832 ✅ | `9a99f78131a348032a0342b2ec3dc2b671c0455ab60fee504761e0e82e1934b3` ✅ | **0600** ✅ |
| `.p0ref/SitkaLab.Core.dll.sha256` | 84 | — | self-check `sha256sum -c` → **OK** | **0600** ✅ |
| `.p0ref/provenance.txt` | 1,569 | — | records `p0QualifiedCommit=a7f41983…`, `dllSha256` matching | **0600** ✅ |

- P0 `.p0ref/` is ignored by design: `Tools/MultiRackSitkaP1Lab/.gitignore:8:.p0ref/` — confirmed by `git check-ignore`.
- The pinned fixture is **untracked by design** — confirmed by `git ls-files --error-unmatch` (not tracked at `HEAD`).
- Engine's own derived fixture `…/fixtures/derived/p1d-30m-sitka-adult-v1.sitkap1fx.json` hashes to the same `bcaaa5c3…` (32,567 B), tracked and byte-identical to `HEAD`.
- Year1 contract receipt confirmed in restored evidence: `golden1.stdout`, `golden2.stdout`, `parent-golden.stdout` all **45,980 B** / `574d87461af8ff2cb3559723c62f376f4339971728b0613c6aa069fe8bb5dd53`, exactly as `RESTORE.md` §4 states. (The 22,857 B `ccf.h0a.core-read-v1.0.0.md` is the separate Core read contract, not that receipt.)

## 7. Worktree cleanliness

```
core:      ?? Tools/MultiRackSitkaYear0Viewer/fixtures/    ← the authorised untracked fixture only
viewer:    CLEAN
y2engine:  CLEAN
```

No tracked modifications anywhere. The single `core` entry is the deliberately untracked, mode-0444 pinned fixture — not an unexpected edit, not a dirty merge of someone else's work.

## 8. Explicitly NOT RUN

| Omitted | Later gate |
|---|---|
| Full regression / test suites | #801 D0–D6 matrices, future authorised run |
| Mutation testing | future qualification |
| `.NET` build and P0 rebuild | requires V-01 public read/build-input grant (#804) |
| Unity build / import | not a Unity project set |
| Browser accessibility matrix | future independent qualification |
| Full 113/78+50 test matrices | #801 acceptance work |
| Browser visual smoke | **NOT RUN** — optional only, skipped deliberately |

Source restoration is **not** desktop runtime qualification.

## 9. Status and honest limits

**`READY FOR SOURCE DEVELOPMENT`** — the three source trees, their real unpushed Git histories, the pinned fixture, the P0 reference DLL and the packaged HTML are present and verified on native ext4.

**`BUILD ENVIRONMENT NOT QUALIFIED`** — as required by #804 / #810:
- **Missing P1 public compile-linked files:** `Assets/ForestPrototype/SitkaGrowthModel.cs` and `Assets/ForestPrototype/SimulationRandom.cs` are **not present** (confirmed absent). They were deliberately **not** staged; #804 requires a prospective **V-01 read/build-input grant** before that runnable build configuration exists.
- No .NET SDK/runtime check was performed, no dependency closure established.
- **L4 FAIL38 remains UNWAIVED.** #804 is **BLOCKED FOR MAIN**; Manager disposition is **PARK FOR DESKTOP** — unchanged by this restore.
- This is the Multi standalone Viewer/Core + engine set, **not** a complete usable CCF Unity project.
- Dashboard/Drive sync **NOT VERIFIED**.

## 10. Preservation

- `C:\CCF-Archives\Multi-20261119` unchanged: all 9 checksums re-verified on the destination; no file written, moved or deleted there.
- ZX20 `D:\` and laptop originals untouched.
- Original archives kept intact; the full staged extractions were retained under `_staging/` rather than discarded.
- No GitHub `main` push; this document is the only published change.
- Local repo `CCF-HP` had pre-existing modified files before this task (asset `.fbx`/`.png`, `Tools/`, `Docs/Verification`); none were touched or committed.

## 11. Handoff

**#730:** the desktop now holds usable, integrity-checked WSL2/ext4 development worktrees.

```
core/      c4124b63  feature/multi-h0a-core-year0-admission-20261118-v1
viewer/    f3537fe3  feature/multi-h0a-a-year0-viewer-20261118-v1
y2engine/  6f9d2e99  feature/multi-789-year2-10-engine-20261119-v1
```

Next work should start in **new isolated branches under explicit grants**. A future runnable build needs the V-01 build-input decision above; nothing in this restore authorises a build, main integration or release claim.

**#810 result: `READY FOR SOURCE DEVELOPMENT`, `BUILD ENVIRONMENT NOT QUALIFIED`, all minimum checks PASS, no archive or source file modified.**
