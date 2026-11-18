# Multi #753 — H0-A offline Year0 viewer: read-only physical custody and package preflight result

**18 November 2026 · worker: Anthropic Claude Sonnet 5.5 (`claude-sonnet-5-5`), Claude Code harness inside T3 Code, native Linux host `jer-laptop` · Manager: #730 · claim: [#753](https://github.com/jermur788/CCF/issues/753).**
**Registered [RUNNING]:** [comment 6985074140](https://github.com/jermur788/CCF/issues/753#issuecomment-6985074140) at 2026-11-18T16:42:59Z, against fresh GitHub main `34185b8b71ba280c89091290345e537ef107bdf4`.
**Branch:** `docs/multi-753-h0a-readonly-preflight-result-20261118-v1` (created from that exact main HEAD through the GitHub API; no local checkout, worktree or ref was created).

## 0. Verdict at a glance

**NO SOURCE CODE WRITTEN / NO TESTS / NO BUILD / NO UNITY / NO WINDOWS / NO HOST / NO BROWSER LAUNCHED / NO BOARD, LEASE, LANE OR WORKTREE CHANGE.** This is documentation of read-only observations only.

| Question | Answer |
| --- | --- |
| Can the real physical CCF-Ops be read from this machine? | **YES.** Live deployment `/media/jer/ZX20/CCF-Ops` and the Files ext4 lane tree were read directly. No substitution of GitHub data for Ops was needed. |
| Is there a collision for a *new* H0 lane? | **NO collision found.** No board row, lane, lock, branch or directory mentions H0, #753 or the Year0 viewer. Proposed names are unused. |
| Is the public Year0 fixture trustworthy against the pin? | **YES for the public GitHub bytes:** 32,567 bytes, SHA-256 equals the pin exactly. Native local-copy equality is **PENDING** (local stores deliberately not read). |
| Can a source writer start? | **NO. BLOCKED FOR SOURCE.** No lane, no lease, no actor-specific G1 permit, no new G2 scope, no frozen allowlist, no packaging-trust manifest. |
| Can preparation proceed? | **GO FOR PREPARATION:** custodian lane reservation steps, two bounded Core/A packets, one packaging route, listed in §§7-9. |

## 1. Observed environment (present facts, with timestamps)

Observation window **2026-11-18 ~16:38Z to ~16:50Z** (board/`lanes.json` content is as of 12:52Z). Facts are separated from assumptions in the right-hand column.

| Item | Observed | Status |
| --- | --- | --- |
| Worker | Claude Sonnet 5.5, Anthropic, Claude Code (Agent SDK) in T3 Code. Packet preferred Codex Sol High or Claude Opus 5.5; this run is as dispatched by the owner. | Fact (self-reported, as registered) |
| Host / OS | `jer-laptop`, Linux Mint 22.3 (Ubuntu 24.04 base), kernel `7.0.0-42-generic`, x86_64, 4 CPUs, 15 GiB RAM (about 9.8 GiB available), X11 session | Fact (`/etc/os-release`, `uname`, `free`) |
| User identity | Unprivileged local desktop user, uid 1000 (member of `sudo` group; **no privilege escalation used**). GitHub/git identity `jermur788`. | Fact |
| Root `/` | ext4 `/dev/sda2`, 234 GB, **93% used, 17 GB free** | Fact. Not used, per owner instruction. |
| ZX20 | exFAT `/dev/sdd2` mounted `/media/jer/ZX20`, 477 GB, 90% used, **50 GB free**; `lanes.json` floor is 15 GiB (16,106,127,360 B), so free space exceeds the floor | Fact (`df`; floor from `lanes.json`). I did not run `lane-status --storage-only`. |
| Files | ext4 `/dev/sdb4` mounted `/media/jer/Files`, 465 GB, 93% used, **35 GB free** | Fact |
| Scratch used by me | `/dev/shm/ccf753` (tmpfs, RAM-backed, **no disk written**). I first placed an 11 KB packet copy under the harness scratchpad on root, then deleted it. | Fact |
| Read-only tools used | `git 2.43.0` (`--no-optional-locks`, `GIT_OPTIONAL_LOCKS=0`), `gh 2.45.0`, `python3 3.12.3` (`-B -I`, stdlib only, read/parse only), `sha256sum` | Fact |
| Present but NOT executed | `node`/`npm` (managed under a `vite-plus` directory with runtime directories 24.22.1, 26.12.1, 26.13.0, 26.13.1; **which one is active is unverified**), `dotnet` SDK 8.0.133 and 10.0.114 (package metadata) | Fact (presence); runtime behaviour unverified |
| Desktop browsers detected | **Firefox 159.0** (deb, `/usr/lib/firefox`, BuildID 20261022150525), **Chromium 156.0.8078.79** (deb), **Brave 1.97.59** (flatpak). Versions read from package/flatpak/`application.ini` metadata; **no browser was executed or launched.** | Fact (metadata only) |
| Browsers not detected | Google Chrome, Microsoft Edge, Safari. Windows/macOS browsers cannot be observed from this host. | Fact |
| A browser is already running | A Firefox session (owner's OAuth sign-in tab) is live. **Not touched.** | Fact |
| Running Unity / .NET | `ps` shows no Unity, `dotnet`, `git fetch/gc/push/checkout/merge` or Blender process | Fact at ~16:39Z |
| Git credentials | Global `credential.helper` for `github.com` is `gh auth git-credential`; the `gh` token has scopes `gist, read:org, repo, workflow`. **Any git store on this host can authenticate a push to the public repo.** | Fact (see §5) |

### 1.1 Offline single-file HTML/Canvas constraints (design considerations, NOT browser-verified)

No page was opened and no runtime was exercised. These are constraints to check at packaging review, not measured results.

1. **Classic inline scripts only.** ES `import`/`type=module` and any `fetch`/XHR of a sibling file are unsafe from a `file://` document (origin/CORS). The fixture must therefore be embedded at packaging time, as the approved spec already requires. **Proposal:** embed the 32,567 bytes as **base64** in an inert `<script type="application/octet-stream">`/data block (about 43.4 K characters), decode to bytes, then hash. Base64 avoids JSON/`</script>`/line-ending ambiguity and preserves exact bytes.
2. **SHA-256 in the page.** `crypto.subtle` needs a secure context. Whether each target browser treats a local `file://` page as secure is **UNVERIFIED and must be tested** on the actual browsers. If it is not, Core would need its own dependency-free SHA-256. Pick one code path in the Core packet; do not ship both.
3. **CSP via `<meta http-equiv>`.** Hash-source CSP (`default-src 'none'; script-src 'sha256-…'; style-src 'sha256-…'; img-src data:; base-uri 'none'; form-action 'none'`) is expressible in a `<meta>` tag, but `meta` CSP cannot carry `frame-ancestors`, `sandbox` or reporting directives. Behaviour on `file://` in each target browser is **UNVERIFIED**. The assembler must compute script/style hashes from the exact final inline blocks.
4. **Storage.** `file://` origins are opaque in current Firefox/Chromium families, which is consistent with the spec's "no localStorage / no saved state". The page must not touch `localStorage`, `IndexedDB`, cookies or `File System Access`.
5. **Numerics.** `Math.fround`, `DataView.getFloat32/Uint32` and Number arithmetic reproduce the fixture rule `(float)((xMm-15000)/1000.0)` exactly in principle (|xMm−15000| ≤ 15,000 is exact in binary64). This is an expectation for H0-AC03, **not** something I computed or tested here.
6. **No external anything.** No CDN, web font, analytics, service worker or manifest; system font stack only; Canvas sized by `devicePixelRatio`.
7. **"Current desktop browsers" is not yet a testable matrix.** The #750 receipt itself says no named browser has passed. This host can cover Firefox 159, Chromium 156 and Brave 1.97 only.

## 2. Physical ownership and custody (Question 2)

**What was read.** `/media/jer/ZX20/CCF-Ops`: `board/active.md` (mtime **2026-11-18T12:52:39Z**), `lanes.json` (mtime 12:52:39Z), `deployment.json` (2026-11-15T23:18:25Z, `RECONCILED_OBSERVATION_NOT_SOURCE_ACCEPTANCE`), `README.md`, `environment.sh`, the `state/unity` directory listing and the tail of its history. Per-lane `git status`/`HEAD` used `git --no-optional-locks`, with no fetch, index refresh, checkout or ref write.
**What was NOT run.** `lane-status`, `packet-lint`, `unity-lock`, `self-test.py` or any Ops script (I read files directly to avoid any script side effect). A `lane-status --preflight` result is therefore **not** claimed.

### 2.1 Board and lock facts

| Fact | Evidence |
| --- | --- |
| 42 board rows, 42 unique IDs; states READY 11, DONE-PENDING-LEDGER 21, WAITING 5, BLOCKED 4, VERIFY 1; rows name 12 distinct lanes, of which **9 are in `lanes.json`** and 3 are not (`CCF-DensityInstrumentation`, `CCF-Growfor`, `CCF-Tier3Preflight`; older rows, unrelated to H0) | Parsed `board/active.md` (header row plus 42 data rows; my first parse skipped the first data row and was corrected) |
| **No board row, lane, branch or lock mentions #753, #750, #749, H0-A, "Year0 viewer" or HTML** | Search of all 42 rows and `lanes.json` |
| **No Unity lock record.** `state/unity` holds only the 0-byte `guard` and `history.jsonl` (299,478 B; last event a `release` at 2026-11-15T22:44:25Z). | `ls`/`find`/`tail` |
| No Unity, .NET or git-mutation process running | `ps` |
| `lanes.json` has no machine field for expiry. Dates appear only inside free-text `purpose` strings. A grep of `_ccf.py` and `lane-status` found **no expiry parsing**; only `unity-lock` handles an ETA (15 matches). | grep (full source not read, so "no tool enforces it" is **observed by grep, not proven**) |

### 2.2 Lane-by-lane physical state (metadata only)

| Lane / task | Physical HEAD and cleanliness | Board / `lanes.json` says | Note |
| --- | --- | --- | --- |
| #725 `MultiAnnualF0Child725` | branch `feature/multi-p1-f0-annual-725-20261118-v1` at `df467dcc`, **clean** | row DONE-PENDING-LEDGER, HEAD `df467dcc`, **custody to 2026-11-27T18:00Z**, immutable retention, no writer, no auto-purge | Matches. **Protected; contents not read.** |
| #710 `MultiP1ModeRepair710` | `repair/multi-p1-shell-modes-710-20261117-v1` at `35d26f5e`, **clean** | BLOCKED, custody retained, **to 2026-11-23T18:00Z**, no active writer | Matches. **Protected; contents not read.** |
| #690/#707 `Sen1SaveA0ZX20NonUnity707` (ZX20, exFAT) | `integration/sen-1-d073-shared-save-a0-20261117-v1` at `55863f1b`, clean | BLOCKED, writer assigned but SOURCE START HELD, advisory expiry 2026-11-19T00:00Z "NOT ENFORCED BY TOOL" | Matches. Unrelated to H0. |
| #687 `MultiP0P1Integ687` | `integrate/multi-p0-q-current-main-20261117-v1` at `ce7bfeb6`, clean | VERIFY, lease to 2026-11-19T18:00Z | Matches. Unrelated. |
| #697 `MultiP1ChildB697` | `integrate/multi-p1-h2-f-child-b-20261117-v1` at `719ccbf8`, clean | BLOCKED, reserved, expiry 2026-11-19T18:00Z | Board HEAD "TBD". Unrelated. |
| #674 `MultiP0Proof674` | `integrate/multi-p0-q-main-ancestry-proof-20261117-v1` at `5aa7b6ef`, **dirty: 8 untracked status lines (doc paths)** | READY / not running, **expires 2026-11-18T18:00Z (today, about 1h15 after observation)**; board HEAD "TBD" | Not an H0 collision (different root) but **flag for the custodian**: dirty tree plus a same-day expiry. Not touched. |
| #652 `ops/sen-1-652-unity-lease-readiness-20261116-v1` checkout | `4c7acc05`, clean | board row HEAD `a9f56d63` | **Board HEAD is a strict ancestor of the live HEAD** (`merge-base --is-ancestor` true). Row is stale but lineage-consistent; custodian housekeeping, not H0. |
| `CCF-NonUnity` lane | `tooling/linux-player-tier3` at `5010613f`, clean | idle | Unrelated branch. **Do not reuse for H0.** |
| `/media/jer/Files/CCF/Worktrees/CCF-Sen1-690-A0-20261117` (lane `Sen1SaveA0NonUnity690`) | **Path MISSING** | "Unassigned / NOT RUNNING" reservation | Reserved but never created. Informational. |

**Disclosure of one borderline step.** To verify custody I ran `git --no-optional-locks status` and `rev-parse` on the #725 and #710 worktrees. That `stat()`s tracked files but reads no file *contents*, wrote nothing and refreshed no index. If the custodian reads "do not read protected source trees" more strictly than metadata, treat this as a disclosed deviation. No file inside either tree was opened, listed or copied.

### 2.3 Canonical `active-tasks.md` versus physical Ops, and D-091

| Topic | Canonical main (`34185b8b`) | Physical Ops | Verdict |
| --- | --- | --- | --- |
| #710 R custody expiry | `active-tasks.md` line 131 at `34185b8b` (the #730 succession paragraph; line 129 at the earlier `55d4fb89`): **2026-11-19 18:00 UTC** | `lanes.json` and board: **2026-11-23T18:00Z** (extended by #732 receipt) | **Canonical text is stale.** The packet's Nov 23 matches physical. |
| #725 custody expiry | same paragraph: **2026-11-20 18:00 UTC** | `lanes.json` and #725 row: **2026-11-27T18:00Z**; but the **#732 row's Locks text still says "#725 source (to 2026-11-20T18:00:00Z)"** | Physical Ops is **internally inconsistent**; `lanes.json` and the #725 row agree with the packet (Nov 27). |
| #735 retention / #753 / H0-A | one passing #735 mention (Sen-1 #652 paragraph); no Multi H0 or retention row | no H0 row | Canonical coordination does not yet carry H0-A. Authorised editor, not me. |
| D-091 no arbitrary worker expiry (owner, 2026-11-18 11:49Z) | decision log row D-091; says existing physical leases are "not mechanically changed" and must be reconciled by their custodians | #674/#687/#697/#690/#710/#725 rows still carry dated expiries; #690 labels its date "ADVISORY / MANUALLY SUPERVISED" | Unreconciled. **For H0, do not write any UTC expiry** (see §9). |

**Collision conclusion: none for a new H0 lane.** Every existing lane root is different from the proposed one, and no existing row claims any H0, Year0-viewer or `Tools/MultiRackSitkaYear0Viewer` path. I did not kill, stash, clean, reset, re-lease, expire or create anything.

**Factual versus unverified custody.** Factual: the physical state above. Unverified: whether any custodian has since changed it (board is 12:52Z; observation ~16:40Z); the current Manager/owner grant status of any lane beyond what the rows say; #725/#710 contents and hashes (deliberately not read); any off-machine Dashboard/Drive copy (**NOT VERIFIED**).

## 3. Fixture trust and delivery (Question 3)

**Source read (GitHub only; no local copy touched).** Public repo (`private=false`), branch `feature/multi-p1-639-f0-30m-foundation-20261116-v1`, tip `e302f15bf6188605a2c2cbeddbd973b3d8ac0f66`, path `Tools/MultiRackSitkaP1Lab/fixtures/derived/p1d-30m-sitka-adult-v1.sitkap1fx.json`, git blob `8bf81a4364a5a6bacf104d72d354e74635d3b26c`. **The path is not on main** (404 on `main`). The bytes were fetched into RAM and hashed there.

| Check | Pin (spec/plan/tasks/#750 chain) | Observed | Result |
| --- | --- | --- | --- |
| Byte length | 32,567 | 32,567 | **match** |
| SHA-256 | `bcaaa5c3d9b3d36ad8319ffcc361a6225a02b90281cef8435de7c52f8ff757c4` | `bcaaa5c3d9b3d36ad8319ffcc361a6225a02b90281cef8435de7c52f8ff757c4` | **match** |
| `format` / `derivedSchemaVersion` / `sourceClass` | `ccf-sitka-p1-derived-fixture` / 1 / `Synthetic` | identical | match |
| `domain` / `derivedFixtureId` / `catalogVersion` | `DerivedP1ThirtyMetreExperimentV1` / `p1d-30m-sitka-adult-v1` / `p1-f0-derived-catalog-v1` | identical | match |
| Geometry | `P1DerivedSquare30m`, `sideMm` 30000, transform `p1-square30-corner-to-centred-xz-v1` | identical; `xzRule` text "exact int64 subtraction, then binary64 division, then one f32 cast" | match (declared path only) |
| Area | f32 bits `3db851ec` | `areaHa` = `{"k":"f32","b":"3db851ec"}` | match |
| Provenance values | seed 20261116, growth model 2, RNG model 1, `p1-f0-adult-sitka-30m-v1` | identical (separate `generatorSeed` 6390001 exists; do not confuse it with the simulation seed) | match |
| Roster | 100 unique IDs `P30-001`…`P30-100`, 100 living, age 30 | IDs equal that ordered list; 100 `live`; all `ageYears` 30; `treeCount` 100 | match |
| Below-170 mm stems | 12 | 12 rows with `dbhMm` < 170 | match |
| Domain | xMm/yMm in [0, 30000) | all 100 in range | match |
| Row shape | typed f32 envelopes | every row has the same 12 keys; `dbhCm`, `heightM`, `crownRadiusM`, `unityX`, `unityZ` are `{"k":"f32","b":"<8 hex>"}` | consistent with spec §4.2 |
| Bytes hygiene | no EOL rewriting | 0 CR bytes, no trailing newline; **main has no `.gitattributes`** | no conversion risk on main |

**What this does and does not prove.**
- It proves the **public GitHub bytes equal the pinned digest**. It is a consistency check against the documented pin, **not a new trust root**: the pin appears in spec, plan and tasks that share one authoring chain. The independent root still requires a non-author reviewer to re-hash and a Manager-published manifest (spec H0-R008).
- The file's own `contentHash` (`b7dee8a3…6627`) is self-declared. I did **not** recompute it (its canonicalisation is not documented in what I read) and it must never serve as the trust anchor.
- **Not done, by design (future H0-AC03/AC12):** recomputing the 100 forward f32 X/Z bit patterns, negative/tamper checks, or any schema fail-closed test.
- **Native validation PENDING.** I did not open any local copy of the fixture (the #639 worktree at `/home/jer/ccf-worktrees/multi-p1-639-f0-30m` and the protected #710/#725 stores were left alone). Local-copy equality is a custodian-run check after a read grant.
- There is **no file named `*catalog*`** under the public P1 lab tree; catalog identity appears as the in-file `catalogVersion` plus whatever the C# lab source holds (not read). The Core packet should pin `catalogVersion` as a string, not look for a catalog file.
- I made **no** regenerated JSON and **no** Year1 rows.

## 4. No-new-public-production-code containment (Question 4)

| Finding | Evidence | Consequence |
| --- | --- | --- |
| The CCF repository is **public** and the canonical store's `origin` is `https://github.com/jermur788/CCF.git` | `gh api` `visibility=public`; `git config` | Any branch in the canonical store is one `git push` from publication. |
| The host can push without a prompt | global helper `!/usr/bin/gh auth git-credential`; token scope `repo` | **Containment cannot rely on missing credentials.** It must be structural and procedural. |
| No non-sample git hooks in the canonical store | `ls hooks` | No technical guard exists there. |
| The #725/#687/#697/#674/#710 precedent is an **isolated bare store with `origin` removed** | board/`lanes.json` text | This is the pattern to copy. |
| My own checkout (`/media/jer/Files/CCF-Primary`) is a canonical-store worktree with a public origin | `worktree list`; remote | That is why I created **no** local branch and published this result **through the GitHub API only**. |

**STOP rule I applied:** any proposal that places H0 source in the canonical store (`Repository.git`) or in a worktree of it **would** risk publishing unpublished code, and is rejected here. The proposal in §7 does not do that.

**Proposed hardening (for the custodian to apply and read back; I applied none):** isolated bare store with **no remote**; local `credential.helper=` blanked; `push.default=nothing`; `remote.pushDefault` unset; a local `core.hooksPath` containing a `pre-push` hook that always exits 1; no upstream tracking on any branch; custodian runs `git remote -v` (expected: empty) and reads back the config before any writer starts. GitHub may carry only the [RUNNING]/[HANDED-OFF] text, hashes, path lists and test summaries. **No HTML, JS, packaged viewer or copy of the fixture embedded in HTML may be pushed.** (The standalone fixture is already public on the #639 branch; embedding it in a new HTML would be a *new* publication.)

## 5. Disjoint future source ownership (Question 5 — PROPOSAL ONLY)

Nothing below is created, claimed, leased or granted. Names were checked and are unused: no GitHub branch matches `feature/multi-h0a*` or `docs/multi-753*` before registration, no directory matches `*H0*`/`*Year0*` under Files `Operations`, `Worktrees` or ZX20, and none of the proposed repo paths exists on main (`Tools/` on main holds only `Build, Calibration, Diagnostics, ProjectContext, Verification`).

### 5.1 Where the lane lives and why

Files ext4, not ZX20: the packet requires a "POSIX/ext4" lane, ZX20 is exFAT (no symlinks, no POSIX modes), and the Multi non-Unity precedent (#674/#687/#697/#710/#725) is Files ext4. Root `/` is not used. The owner's "use ZX20" preference was applied to my own scratch. If the owner wants the *lane* on ZX20 instead, that conflicts with the packet and needs a decision (OPEN-3 in §10).

| Item | Proposed value |
| --- | --- |
| Lane root | `/media/jer/Files/CCF/Operations/CCF-H0A-Year0Viewer-20261118/` |
| Isolated bare store (no origin) | `<root>/git/CCF-H0A-Year0Viewer.git` |
| Base | local ref `local/h0a-base-20261118` at the **fresh main HEAD at G1 time** (today `34185b8b`; the custodian re-pins) |
| Core worktree / branch | `<root>/ccf-h0a-core` · `feature/multi-h0a-core-year0-admission-20261118-v1` |
| A worktree / branch | `<root>/ccf-h0a-viewer` · `feature/multi-h0a-a-year0-viewer-20261118-v1` |
| Packaged output (packager-owned, never committed) | `<root>/artifacts/<UTC-id>/` |
| Evidence | `/media/jer/Files/CCF/Verification/CCF-H0A-Year0Viewer-20261118/` |
| `lanes.json` names | `MultiH0ACore`, `MultiH0AViewer` (both `unity:false`) |
| Footprint | A `--no-hardlinks` clone of the canonical store is **4.1 GB** (Files has 35 GB free, 93% used). Cheaper option: `git init --bare` plus a fetch of only the main commit and the #639 fixture commit (size not measured). Custodian's choice. |

Two writer worktrees on two branches avoid a shared index and `git add -A` collisions, and the work is naturally sequential anyway (tasks T05 requires T04), so no concurrent edit of one tree occurs.

### 5.2 Exact file ownership (all inside `Tools/MultiRackSitkaYear0Viewer/`)

I tested nine representative paths (one per file role, plus a `dist/` output) against main's `.gitignore` in a throwaway RAM-only repo: **none is ignored**. A packaged `dist/` would not be ignored either, which is why output goes **outside the tree**.

| Owner | Exclusive files (allowlist) | Notes |
| --- | --- | --- |
| **Core writer (one)** | `core/year0-core.js`, `core/CONTRACT.md`, `fixtures/p1d-30m-sitka-adult-v1.sitkap1fx.json`, `build/assemble-year0-viewer.py`, `tests/core/**` | `year0-core.js` is a **classic script with one frozen global namespace** (no `import`/`export`) containing trust admission, strict schema, the single f32/transform codec, F1 availability envelopes, the full F3 key and typed refusal. The fixture is a **byte-identical, read-only (0444)** copy of blob `8bf81a43…`, size/hash asserted at commit. |
| **A writer (one)** | `viewer/shell.template.html`, `viewer/viewer.css`, `viewer/viewer-render.js`, `viewer/viewer-camera-hit.js`, `viewer/viewer-inspector.js`, `tests/viewer/**` | A owns the **only** HTML shell, Canvas renderer, camera, stem-centre hit test, all-100-ID list and inspector. A may merge the three `viewer-*.js` into fewer files but may not leave `viewer/**`. |
| **Packager (one; recommended = the Core writer, in a later sequential step)** | the assembler above; output only in `<root>/artifacts/<UTC-id>/` (`ccf-year0-viewer.html`, `manifest.json`) | Reads A's worktree **read-only at a recorded commit**; never edits A's files. A change needed in the template goes back to A via Manager. This is the single owner of output bytes. |
| **Read-only for everyone** | `specs/multirack-sitka-offline-year0-viewer-v1/**`, everything else in the repo | |
| **Forbidden** | `Assets/**`, `ProjectSettings/**`, `Packages/**`, the P0 lab and its fixtures/pins, the P1 lab `src/` and `tests/`, `package.json`, lockfiles, `node_modules`, any CDN/font/script reference, `.vscode/settings.json`, `Docs/Project/**` | Also: no protected #710/#725 import, no Year1 data, no `Library/Temp/Logs/obj`. |

### 5.3 One-way immutable transfer (Core to A) and no dual-writer

- **Frozen interface (Manager decision before A starts):** one global `CcfYear0Core.admit(bytes)` that returns either a **deeply frozen** observation (100 rows with raw mm integers, typed f32 bit strings, derived X/Z bits, availability envelopes, inventory 100/100, full F3 key) or a typed refusal. A never receives a mutable collection and never decodes f32. Selection leaves A as `(treeId, fullObservationKey)` only. These names are a **proposal**, not an existing API.
- **Placeholders (proposal):** `<!--CCF:STYLE-->`, `<!--CCF:CORE-->`, `<!--CCF:VIEWER-->`, `<!--CCF:FIXTURE-->`. The packager substitutes them and touches nothing else.
- **No duplicate f32 decoder** (Core owns the only one), **no second hit test** (A owns the only one), **no shell dual-writer** (A owns the template; the packager only substitutes).
- Review gate (mechanical): `git diff --name-only <base>..<branch>` for each branch must be a subset of that owner's allowlist; any overlap stops for Manager arbitration.

### 5.4 Packaging and build handoff (route, not a build)

A stdlib-only Python 3 assembler (3.12.3 is present; no pip, no npm, no network), run only after a future G2 permit:
1. Verify both worktrees clean and record their HEAD SHAs; run the allowlist diff check.
2. Read the fixture from the Core tree; assert **size 32,567 and SHA-256 equal to the pin**; base64-embed.
3. Substitute placeholders; compute SHA-256 of each inline `<script>`/`<style>` block and write them into the CSP `<meta>`; **fail closed** if the output contains any `http:`, `https:`, `//host`, `src=`/`href=` to a non-`data:` URL, `import`, `fetch(`, `XMLHttpRequest`, `WebSocket`, `localStorage` or `sendBeacon`.
4. Write `ccf-year0-viewer.html` plus `manifest.json` (input commits, input-file SHA-256s, output SHA-256, fixture digest, UTC time, assembler version) to `<root>/artifacts/<UTC-id>/`. Nothing is committed or pushed.
5. **Code-integrity root:** the output SHA-256 is recorded in a Manager-reviewed receipt **outside** the HTML (hash only, no code). A non-author reviewer re-hashes the file with `sha256sum` before opening it. A hash inside the same document is not tamper-proof (spec §6).
6. Use: open the local file in a desktop browser; **no server, Host, picker or network**.

## 6. G2 permit prerequisites and proportionate evidence (Question 6)

**Physical and claims to issue next (in order):**
1. **Custodian (physical, not a source writer)** creates the isolated store and two worktrees; applies and reads back the §4 hardening; adds two `lanes.json` entries and a board row with **no UTC expiry** (D-091: ownership ends on documented safe handoff/release); runs `lane-status --preflight` (READY, exit 0) and `packet-lint --board`; records the base commit and a before-hash of the protected P0/baseline set.
2. **Manager #730** issues, per writer: an **actor-specific** G1 physical source permit naming session, branch, worktree and the exact file allowlist; a **new** G2 non-release security scope for H0 (#694 does **not** cover it); separate GitHub claims for Core and for A (not #753).
3. Each writer posts a real **[RUNNING]** with fresh main, base commit and clean-tree readback; **no edit before that.**
4. Verify at start: P0 protection **L0–L3+** unchanged; protected-hit count stays **zero**; L4 stays **FAIL38 UNWAIVED** and is disclosed in every handoff; stop on any new protected write or safety finding.
5. A non-author independent reviewer and a manual browser reviewer are named in advance.

**Proposed test ceiling (all NOT RUN now; one pass per candidate commit, no repeated broad suites; no Unity, Windows, .NET or Year1):**

| Lane | Maximum evidence |
| --- | --- |
| Core | One zero-dependency checks run: exact size/SHA; altered same-length bytes; wrong size; duplicate, missing and unknown ID; unknown key/version/enum; wrong domain/catalog/format; NaN/Infinity/bad hex; wrong transform ID; **100 forward f32 X/Z bits equal to the fixture's `unityX/unityZ`**; area bits `3db851ec`; availability states with no fake zero; full-F3-key stale/other refusal; last-good-view retention. |
| A | One view-state checks run: 3 m × 20 CSS px/m = 60 px; nearest-centre/ordinal tie; overlap chooser; Fit/pan/zoom clamps; DPR/resize; all-100 keyboard reachability; **instrumented zero annual/mark/work/save/network calls**. |
| Rendered (human) | Firefox 159 and Chromium 156 (Brave as a third Chromium check) at 1280×720, 1920×1080, 360 CSS px and 200% text zoom, keyboard-only walk, network denied, captured with host/browser details. |
| Independent | One non-author review: changed-path allowlists, trust negatives, package integrity, rendered evidence. |

**BLOCKED right now:** the host/test-runner for the JS checks is undecided (Node's active version is unverified, and a Node runner is not an approved dependency; a browser-opened harness page is the alternative; OPEN-6).

## 7. Stop conditions — status at observation (Question 7)

| Stop condition | Status |
| --- | --- |
| Source rights conflict | **None found.** No H0 claim exists to conflict. |
| Modified protected P0 | **Not assessed** (no read of P0/#710/#725). Baseline hash is a custodian task. |
| Unknown allowed package source | **OPEN.** No manifest/CSP/browser matrix yet. |
| Dirty unrelated work | **Present but unrelated:** #674 worktree has 8 untracked lines. Not in the H0 path; flagged only. |
| No native credible ownership | **Not triggered:** a physical custodian (`jermur788`) is recorded in board rows, but no one is yet assigned to H0. |
| Need for Host/server to run local HTML | **Not triggered** by the H0-A design (embedded fixture, classic inline scripts). |
| Bundling with network or arbitrary input | **Not triggered** (stdlib assembler, offline, fixed inputs) — conditional on §5.4 fail-closed greps. |
| Public code push without consent | **Risk exists** (§4) and is **controlled only if** the isolated no-remote store is built and read back. |
| Changed H0 scope | **Not triggered.** |

## 8. Recommendation by gate

| Gate | Recommendation |
| --- | --- |
| G0 product/spec (H0-A, four-file spec) | **CLOSED** per #750 receipt. |
| G0 technical packaging (pins, manifest, CSP, browser matrix, code-integrity root) | **GO FOR PREPARATION / BLOCKED FOR SOURCE.** |
| G1 physical custody | **GO FOR PREPARATION** (readable Ops, no collision, names free) / **BLOCKED FOR SOURCE** until the custodian builds and reads back the lane and rows. |
| Fixture trust | **GO FOR PREPARATION.** Public bytes match the pin; native local equality and independent re-hash **PENDING**. |
| No-public-code containment | **GO FOR PREPARATION** / **BLOCKED FOR SOURCE** until hardening is applied and read back. |
| G2 local non-release scope | **BLOCKED.** No permit, no claim, no allowlist grant. |
| G3 Host/network/save | **Not required** by H0-A as scoped; **STOP** if any appears. |
| G4 main/public/release | **BLOCKED** (L4 FAIL38 UNWAIVED). Not waived here. |
| Canonical Multi decision ID/crosswalk | **BLOCKED for promotion only** (main D-084 is Scenario One). Does not block local preparation. |

## 9. Bounded implementation packet proposal for #730 (not dispatched)

- **Packet C (Core), after §6 steps 1-3:** tasks T02-T04 in one claim; allowlist §5.2 Core row; ceiling §6 Core row; deliverable: the frozen interface in `core/CONTRACT.md` plus `year0-core.js`; STOP on any need for a second decoder, Year1, arbitrary input or a protected import.
- **Packet A (viewer), only after Core's boundary is stable (T05-T07 depend on T04):** allowlist §5.2 A row; ceiling §6 A row; deliverable: the single shell/renderer/inspector; STOP on any independent bit decoding, health score, mark/work path or second shell.
- **Packet P (package + independent review):** assembler, manifest, human rendered captures, non-author review; the Manager decides who packages (recommended: the Core writer, sequentially).
- **No expiry** on any of these lanes (D-091). Custody ends on documented safe handoff/release.

## 10. Unresolved owner and canonical decisions

| ID | Decision needed | Owner |
| --- | --- | --- |
| OPEN-1 | Concrete **supported browser/OS matrix** for "current desktop browsers" (only Firefox 159, Chromium 156, Brave 1.97 are testable on this host; Windows/macOS are not). | Owner/Manager |
| OPEN-2 | Core in **browser JavaScript** (recommended: the digest check must happen at open time with no runtime other than the browser) versus a C#/.NET build-time projection. The existing lab is .NET; the JS Core would be a second implementation whose oracle is the fixture's own `unityX/unityZ` bits. | Manager |
| OPEN-3 | Lane location: Files ext4 (recommended, packet-aligned) versus the owner's ZX20 preference. | Owner |
| OPEN-4 | Code-integrity root: who reviews and records the output SHA-256, and where (docs-only receipt, hash only). | Manager |
| OPEN-5 | Meaning of **P0 "L0–L3+" for a JS-only tree** (the ladder is defined for the .NET lab). Proposal: before/after byte-hash of the protected P0 set plus zero-protected-hit, with no .NET run. | Manager/security |
| OPEN-6 | JS check host: browser-opened harness page versus Node (version unverified; Node is not an approved dependency). | Manager |
| OPEN-7 | Designated packager identity (recommended Core writer). | Manager |
| OPEN-8 | `crypto.subtle` on `file://` versus a dependency-free SHA-256 inside Core (one path only). | Core packet |
| OPEN-9 | Canonical **Multi decision ID/crosswalk** for the historical Multi D-084, and refresh of main `active-tasks.md` (stale #710/#725 dates, no #735/#753/H0 rows). | Authorised editor |
| OPEN-10 | Public-source policy for the future (this run only confirms none is granted). | Owner |
| NOTE-A | Physical Ops drift for the custodian: #732 row says #725 to Nov 20 (others say Nov 27); #674 dirty and expiring today; #652 board HEAD behind live HEAD; reserved `CCF-Sen1-690-A0-20261117` path under `Worktrees/` does not exist. None blocks H0. | Custodian |

## 11. What was and was not done

**Done (read-only):** read the complete packet, #750 receipt, four-file spec, #749 review, #748 consolidation, #694 policy, #753/#730 threads, main coordination docs; read physical `board/active.md`, `lanes.json`, `deployment.json`, README and Unity-lock state; read lane git metadata with no-optional-locks git; read browser/tool inventory from metadata; fetched and hashed the public fixture in RAM; tested ignore rules in a RAM-only throwaway repo; created one GitHub branch and one GitHub file; posted comments.
**Not done:** any Ops script; any board, `lanes.json`, lease, lock, lane or worktree change; any local branch or file under any CCF repository or lane; any read of #710/#725 file contents or of a local fixture copy; any browser launch, network service or page load; `dotnet`, `node`, `npm`, Unity, Windows or Host; tests, builds, source, installs; main edit or integration; public production-code push; L4 waiver.

**NO SOURCE CODE WRITTEN / NO TESTS/BUILD/UNITY/WINDOWS/HOST.** All 65 H0 requirements remain unverified. Dashboard/Drive sync: **NOT VERIFIED.** This result is not a G1/G2 grant, a lease, an allowlist or a start signal. Issue #753 stays OPEN for Manager #730 review.
