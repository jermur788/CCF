# Multi #759 — H0-A D5 protected-P0 physical read authority: PHASE0 result

**18 November 2026 · claim [#759](https://github.com/jermur788/CCF/issues/759) · Manager [#730](https://github.com/jermur788/CCF/issues/730) · packet [`f8923ba8`](https://github.com/jermur788/CCF/blob/f8923ba8b636be8e23ef1346a7df8c396dfc2e49/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-2D-SITKA-759-H0-A-D5-P0-BASELINE-PHASE0-READ-AUTHORITY-PACKET-20261118.md).**

**Status: BLOCKED FOR PHYSICAL BASELINE.** Phase0 is complete. No actor-specific Manager grant and no owner/security read authority exist for any physical P0 tree or for the P0 pin objects in a local store. So I took no physical P0 read, traversal or hash, and I make **no integrity claim**. This document only **proposes** a custodian, a path set, commands and a budget (§5–§7). It grants nothing.

| | |
| --- | --- |
| Worker | Anthropic **Claude Opus 5.5** (`claude-opus-5-5`), Claude Code 2.1.327 in T3 Code, native Linux on host `jer-laptop` (Linux 7.0.0-42-generic), session `d1530702` |
| Registration | [RUNNING comment 6987386340](https://github.com/jermur788/CCF/issues/759#issuecomment-6987386340), 18:14Z |
| Fresh main | `34185b8b71ba280c89091290345e537ef107bdf4` (`git ls-remote`, 18:14:10Z) |
| Branch | `docs/multi-759-d5-p0-baseline-authority-result-20261118-v1`, created at that main through the GitHub API. No local branch, worktree or ref was made in any CCF store. |
| Only output | this one Markdown file |
| **Independence disclosure** | An earlier Claude Opus 5.5 session wrote [#756](https://github.com/jermur788/CCF/blob/7aaa7a90fd5134850f3eeda5696410e20c82f286/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-2D-SITKA-756-H0-A-FROZEN-CONTRACT-AND-D5-PROTECTION-PREFLIGHT-DRAFT-RESULT-20261118.md) (including its D5 route), #639 P1-F0 and the #567 P0 handoff `a7f41983` itself. The packet's worker default is "NOT #756 contract author". At 18:13Z the owner expressly overrode that default for this session, after being told about the authorship. This result therefore **re-checks author-side work and is not independent verification of it.** I do **not** propose myself, or any Claude Opus 5.5 session, as custodian. |

---

## 1. Inputs read in full (public/canonical only)

| Input | Ref |
| --- | --- |
| #759 packet | `f8923ba8` (14 lines) and #759 issue body + Manager comment [6987129802](https://github.com/jermur788/CCF/issues/759#issuecomment-6987129802) |
| #756 DRAFT result (§4 D5) | `7aaa7a90` |
| #757 independent GPT-6 review (B-2: metadata is not byte integrity) | `docs/multi-730-757-independent-contract-review-20261118-v1` |
| #753 custody preflight (lane-by-lane HEADs observed 2026-11-18) | `f3a15e3e` |
| P1 owner-approved spec §9.2 (L0–L4), REQ-B07, m02, m05 | `docs/multi-p1-637-owner-approved-four-file-20261116-v1` @ `90583811` |
| D-091 (no arbitrary time limits; existing leases not mechanically changed) | main `34185b8b` `decision-log.md` |

## 2. Physical Ops metadata (read-only, 18:14:38Z)

| Observation | Result |
| --- | --- |
| `ZX20/CCF-Ops/board/active.md` | SHA-256 `4fa750eba64a8a072feb305a7db2cce17ddedfa62dbd276305116c03ab853994`, 110,616 B, mtime 17:28:31.52Z. **Equal to the #755 after-hash and the #756 observation.** 141 lines, 39 `CCF-` rows. |
| `ZX20/CCF-Ops/lanes.json` | SHA-256 `c1d147196e1b8d57008e44545db3ba7f3fecea0ae12094dbf3e05f1c782b7d9e`, 12,179 B. **Unchanged.** 25 lanes; `git_stores` = `Files-canonical` (`/media/jer/Files/CCF/Repository.git`) and `legacy-SSD`. |
| `ZX20/CCF-Ops/deployment.json` | SHA-256 `8c66b290c247708846cb8b59f83626a10a3fa1d052397a3316dc5f23fc157b18`, unchanged since 2026-11-15. |
| Locks/processes | `state/unity` holds only `guard` (0 B) and `history.jsonl`. `ps` shows no `dotnet` process and no live `git` process (4 defunct zombies only). There is no board lock holder to report. |
| Board/lanes mentions of #759 | **none.** No row, lane, lock or reservation for #759 or a D5 custodian exists. |
| Free space | Files ext4: 34 GB free (93 % used). ZX20: 47 GB free. |
| Ops scripts | **none run**, and no lock taken. I hashed the three files and parsed them in RAM only. |

## 3. Public authority for the protected object (GitHub API only)

The P0 pin is **public** on GitHub. I read only commit and tree **metadata** (object IDs, path names, modes, sizes). I fetched no blob content.

| Fact | Value |
| --- | --- |
| P0 pin commit (Manager #589, spec §9.2 m05) | `a7f41983c2f2f233b8fe999d1a5b10e72bdd2866`, commit tree `dfbf5f9df729f2433e1836dbf6c6ee3b33a7043e` |
| Protected subtree `Tools/MultiRackSitkaLab` | `f212c8f64fb28b83aa870c7b5b7c72e03051e46a`; not truncated; **52 blobs, all mode 100644**, 16 subdirectories, **777,548 B** in total. The largest is `fixtures/synthetic/sitka-flat-perf-2000-v1.sitkalab.json` (324,929 B). |
| On main `34185b8b` | `Tools/` has **no** `MultiRackSitka*` entry. Absence from main proves nothing about physical copies (#756 §4.2). |
| #639 public tip `e302f15b` | carries `MultiRackSitkaLab` = **`f212c8f6`** (identical committed bytes) plus `MultiRackSitkaP1Lab` = `2c017956` |
| `ce7bfeb6` (#687), `5aa7b6ef` (#674), `719ccbf8` (#697) | **404 on GitHub**: local-only commits. Whether their trees still carry `f212c8f6` can be learned **only by a granted local read**. |
| SHA-256 of the sorted public path/mode/size list (Appendix A) | `899075b0db872169a7dcc0755f728547559e505ce8415ad4ad9f12669dbffcdc`. This is a list fingerprint, **not** a content hash. |

**The canonical expected value per file is its Git blob ID in tree `f212c8f6`.** A future custodian can compare each physical file against it by computing `sha1("blob <len>\0" + bytes)` and recording SHA-256 as well. That answers #757 B-2: there is a source-approved byte-level expectation for every tracked file, without trusting P0's own `MANIFEST.sha256`. Ignored and untracked files have **no** source expectation, so they get a pre/post byte hash only.

## 4. Who owns each candidate tree, and whether a read collides

The access classes are **G** (a canonical Git-object read from a store: object IDs and committed bytes), **P** (a physical working-tree file read: contents, modes, ignored output) and **O** (Ops metadata). The HEADs come from #753's observation (≈16:45Z today). **I did not re-observe them.**

| ID | Tree / object | Physical path (from Ops/#753) | Recorded holder / owner | Board state, expiry text | Collision if read | Class | Proposal |
| --- | --- | --- | --- | --- | --- | --- | --- |
| **B-1** | Canonical P0 pin objects | `/media/jer/Files/CCF/Repository.git`, read only `a7f41983^{commit}`, `a7f41983:Tools/MultiRackSitkaLab`, plus a refs-list hash | owner `jermur788`. This is a shared store with no lane. | — | None for a read-only `--no-optional-locks` object read. Other agents push here, so a moving refs hash is reconciled against GitHub and is **not** a hit. | G | **include** |
| **B-2** | H0 store cannot carry P0 | `/media/jer/Files/CCF/Operations/CCF-H0A-Year0Viewer-20261118/git/CCF-H0A-Year0Viewer.git` | Manager #730 + owner; setup custodian Claude Sonnet 5.5 (#755); writer UNASSIGNED | READY, NO EXPIRY (D-091) | None. Lanes are NOT RUNNING. It must be taken **before** a G2 writer claim. | G | **include** |
| **B-3a** | #639 P1-F0 worktree | `/home/jer/ccf-worktrees/multi-p1-639-f0-30m` (cited by #753 only) | **Not recorded in physical Ops** (no lane, no row). Author: Claude Opus 5.5; owner `jermur788` | none | **Unknown custody.** It sits under `/home`, outside the Files/ZX20 storage policy. Its public tip carries `f212c8f6`. | P | include **only after the owner confirms custody** and that no writer is active |
| **B-3b** | #687 child A P0 current-main worktree | `/media/jer/Files/CCF/Operations/CCF-687-P0P1-Integration-20261117/ccf-687-child-a` (HEAD `ce7bfeb6`, clean) | **Exclusive lease: OpenAI Codex GPT-6.2 Sol Medium**, retained for review | VERIFY; "through 2026-11-19T18:00:00Z" | Yes: an exclusive lease. A read needs **lease-holder or owner consent** with read-only terms like #683's. | P | include with consent |
| **B-3c** | #674 ancestry scratch worktree | `/media/jer/Files/CCF/Operations/CCF-MultiP0-AncestryProof-20261117/ccf-multi-p0-674` (HEAD `5aa7b6ef`, **dirty: 8 untracked doc paths**) | **Reserved: opencode-go DeepSeek V4.1 Flash**; #683 Codex read-only grant ended 2026-11-18T12:00Z | READY; "expires 2026-11-18T18:00:00Z", **already passed at this observation** | Yes, and the lease status is ambiguous (see C-2). Record it as it is and **never clean it**. | P | include **only after the custodian reconciles the lease** |
| B-3d | #697 child B worktree | `/media/jer/Files/CCF/Operations/CCF-697-P1-Child-B-20261117/ccf-697-child-b` (HEAD `719ccbf8`, clean per #753) | Exclusive reservation: Codex GPT-6.2 Sol Medium | BLOCKED; "through 2026-11-19T18:00:00Z" | Yes: exclusive. It is also the **source of the #710 store copy**. | P | **exclude by default.** It is not known to carry P0 bytes, and reading it adds a lease collision. Manager may add it. |
| B-4 | #710 R custody, #725 retention | `CCF-710-ModeRepair-20261117`, `CCF-725-F0Annual-20261118` | owner `jermur788` custodian | BLOCKED / DONE-PENDING-LEDGER | — | none | **EXCLUDED** (packet). Only the Ops text already covered by B-5. |
| **B-5** | Ops | `board/active.md`, `lanes.json`, `deployment.json` | physical Ops custodian | — | None. Hash only, no lock. | O | **include** |
| — | Isolated stores of #674/#687/#697 | their `git/*.git` | as above | — | lease collision | G | **exclude.** Committed P0 bytes are proved once by B-1. Worktree reads (B-3) cover the physical copies. |
| — | `CCF-Primary`, legacy store, Unity lanes, H0 worktrees' file contents | — | various | — | — | — | **exclude.** No P0 is expected there, and the H0 trees are proved by B-2 at Git level. |

## 5. Proposed custodian (one, named by role; it must register before acting)

**OpenAI Codex GPT-6.2 Sol High, a NEW native T3 Code session on `jer-laptop`.** It registers its own session/thread ID on #759, or on a successor claim, before any read.

It must **not** be any of the following:
- any Claude Opus 5.5 session (author of #756, #639 and P0 `a7f41983`);
- the #757 reviewer;
- the #687/#697 Sol Medium lease holder;
- the #725 Sol High reservation session;
- the #674 DeepSeek holder;
- any future H0 Core/A writer, packager or H0 reviewer.

Why Codex: it matches the packet's "best worker" and keeps provider independence from every Claude-authored input.

**Fallback (Manager choice only):** the #755 G1 setup custodian Claude **Sonnet 5.5** session, which already holds H0 physical setup custody. Its weaknesses: it is the same provider as the #756 author, and it is not independent of B-2's own setup.

## 6. Proposed actor-specific grant (paste-ready; NOT issued; needs Manager #730 AND owner/security)

> **GRANT (proposed): D5 PRE-baseline, READ-ONLY, custodian `<provider/model/session-id/host>` only.**
> **Scope:**
> - B-1: `git --no-optional-locks -C /media/jer/Files/CCF/Repository.git` with `cat-file -t a7f41983…`, `rev-parse a7f41983^{tree}` (= `dfbf5f9d…`), `rev-parse a7f41983:Tools/MultiRackSitkaLab` (= `f212c8f6…`), `ls-tree -r --full-tree a7f41983 -- Tools/MultiRackSitkaLab` (52 entries == Appendix A), and SHA-256 of `for-each-ref --format='%(objectname) %(refname)'`.
> - B-2: in the H0 bare store, `cat-file -e a7f41983` **must fail**, and `ls-tree -r <each H0 branch> -- Tools/MultiRackSitkaLab` must be empty.
> - B-3a/B-3b/B-3c, **only the subtree `<tree>/Tools/MultiRackSitkaLab`**, and only after the consents below:
>   - `git --no-optional-locks rev-parse HEAD` and `HEAD:Tools/MultiRackSitkaLab`;
>   - `git --no-optional-locks status --porcelain=v1 -z --ignored --untracked-files=all -- Tools/MultiRackSitkaLab`;
>   - one `python3 -I -B` walker (script published first and hashed in the receipt). It does not follow symlinks and opens files with `O_RDONLY|O_NOFOLLOW`. For every entry it records path, `lstat` type/mode/size/`mtime_ns`/inode, and for regular files SHA-256 plus the Git blob ID. For tracked files it compares the blob ID to Appendix A.
> - B-5: SHA-256 of the three Ops files.
>
> **Environment:** `GIT_OPTIONAL_LOCKS=0`.
>
> **Forbidden:** fetch, gc, index refresh, checkout, clean, reset, restore, chmod, rm, and copying any file content out. No #710/#725, no #697 unless added, no lane/board/lease edit, no dotnet/node/npm/Unity.
>
> **Evidence:** only `/media/jer/Files/CCF/Verification/CCF-H0A-Year0Viewer-20261118/d5/pre/` (manifest TSV + `SHA256SUMS` + `walker.py` + command log). The GitHub receipt is hash-only and publishes no source text.
>
> **No expiry (D-091).** Custody ends at documented hand-back.
>
> **Required consents, each recorded verbatim in the grant:**
> - (i) owner `jermur788` for B-1, B-3a and the overall scope;
> - (ii) the #687 lease holder, or the owner overriding it, for B-3b;
> - (iii) the custodian's reconciliation of the #674 lease plus owner consent, for B-3c;
> - (iv) **owner/security** sign-off that physical reads of P0 trees are acceptable.
>
> **Any consent missing:** drop that tree, record it as NOT BASELINED, and never substitute metadata.

The **post** baseline repeats the same commands after A's handoff and before review, and runs a byte-diff of the two manifests. A tree that was dirty at pre (B-3c) is compared to its own pre state, not to clean.

## 7. Budget and partial-evidence risk

| | Estimate (not measured) |
| --- | --- |
| Tracked bytes to hash | 777,548 B × up to 3 trees ≈ 2.3 MB, taking seconds |
| Ignored/untracked bytes (earlier P0 `bin/obj/TestResults/.lab-data/evidence`) | **unknown** until a granted read. Proposed hard cap: STOP and report if a tree has > 20,000 entries or > 2 GiB. |
| Wall time | under 10 min for all of pre (B-1 ≈ seconds; refs hash ≈ seconds) |
| Evidence space | under 20 MB on Files (34 GB free) |

**Residual risks, true even after a full grant:**
- `atime` updates on ext4 are not content changes.
- Same-content rewrites are invisible to hashes, but harmless to bytes.
- Writes outside the named subtree, or by a non-sandboxed process, are not observed (#756 §4.4).
- A tree dropped for missing consent is **unprotected by measurement**.
- #710/#725 integrity rests on non-access only.
- L4 (write tracing) is **not** part of this baseline. Default **.NET L4 FAIL38 stays UNWAIVED**.

## 8. Contradictions and blockers found

| # | Finding |
| --- | --- |
| **BLK-1** | **No grant exists.** No Manager actor-specific grant and no owner/security read authority covers B-1 or B-3. This is the stop reason. |
| **C-1** | The #639 worktree (`/home/jer/ccf-worktrees/multi-p1-639-f0-30m`) carries P0 bytes according to its public tip, but it has **no Ops lane or board row** and lies outside the storage policy. Its custodian and whether it holds a writer are unrecorded. |
| **C-2** | #674's lease text "expires 2026-11-18T18:00:00Z" has **passed**. D-091 forbids timer-based stops and reclaim and says existing leases "are not mechanically changed". So #674 is neither released nor live by rule. The custodian must reconcile it under the board lock before any read. |
| **C-3** | `lanes.json` text is stale against #753 physical observation. #687 says "ALLOCATED but UNCREATED" yet it exists at `ce7bfeb6`. #697 says "Worktree uncreated" yet it exists at `719ccbf8`. The board HEAD is "TBD" for #674 and #697. |
| **C-4** | #687/#697 ("2026-11-19T18:00Z") and #710/#725 still carry UTC expiries. The #732 row says #725 runs "to 2026-11-20T18:00:00Z" while its lane says "2026-11-27T18:00:00Z". These are D-091 compatibility items, noted only. |
| **C-5** | **#757 B-2 is satisfiable for tracked files** through the public blob IDs (§3). For ignored files only pre/post equality is possible, never "matches source". |
| **OPEN** | **Owner/security decision on substituting JS-scoped B-1…B-5 for the .NET L0–L4 ladder (#756 DN-5, #753 OPEN-5) remains OPEN.** This result does not decide it. |

## 9. Decisions needed (routing)

1. Manager #730 + owner: accept or amend the custodian in §5 and the tree set in §4 (include B-3a? B-3d?).
2. Owner: confirm custody of the #639 worktree (C-1), or decide to drop it.
3. The physical custodian reconciles #674 (C-2), and records the #687 holder's consent.
4. Owner/security: approve physical P0 reads, and separately decide the JS-only substitution (OPEN).
5. Issue the §6 grant verbatim to the registered custodian session. Only then may the pre-baseline run.

## 10. What I did not do

- I did not open, list, `stat`, hash or traverse any physical P0 tree, the #639 worktree, the #674/#687/#697 worktrees or stores, or the canonical store's P0 objects.
- I touched no #710/#725 store, not even metadata.
- I made no Ops script run, lock, board, lanes or lease change, and no cleanup.
- I wrote no source, imported no fixture, and ran no tests, build, browser, Node, .NET, Unity, Windows or Host.
- I made no main edit, pushed no public code, and gave no L4 waiver.
- `CCF-Primary` (dirty, branch `ops/workflow-foundation-v1`) was used only for read-only `git fetch`/`git show`. Its working files were not touched.
- Dashboard/Drive: unverified.

## Appendix A — protected tracked files at `a7f41983:Tools/MultiRackSitkaLab` (`f212c8f6`), from public GitHub tree metadata

Paths are relative to `Tools/MultiRackSitkaLab/`. 52 files, 777,548 B.

| Path | Mode | Bytes |
| --- | --- | --- |
| `.gitattributes` | 100644 | 121 |
| `.gitignore` | 100644 | 358 |
| `Directory.Build.props` | 100644 | 432 |
| `MultiRackSitkaLab.sln` | 100644 | 3873 |
| `README.md` | 100644 | 13494 |
| `evidence/README.md` | 100644 | 324 |
| `fixtures/synthetic/MANIFEST.sha256` | 100644 | 301 |
| `fixtures/synthetic/oracle-quarter-ha.sitkalab.json` | 100644 | 969 |
| `fixtures/synthetic/sitka-flat-demo-v1.sitkalab.json` | 100644 | 26675 |
| `fixtures/synthetic/sitka-flat-perf-2000-v1.sitkalab.json` | 100644 | 324929 |
| `global.json` | 100644 | 68 |
| `schemas/lab-file-v1.md` | 100644 | 7358 |
| `schemas/lab-file-v1.schema.json` | 100644 | 4719 |
| `src/Core/CanonicalJson.cs` | 100644 | 10026 |
| `src/Core/Engine.cs` | 100644 | 14167 |
| `src/Core/FixtureCatalog.cs` | 100644 | 3049 |
| `src/Core/Geometry.cs` | 100644 | 4915 |
| `src/Core/LabDataStore.cs` | 100644 | 8776 |
| `src/Core/LabFile.cs` | 100644 | 16444 |
| `src/Core/LabSession.cs` | 100644 | 12256 |
| `src/Core/Metrics.cs` | 100644 | 3615 |
| `src/Core/Models.cs` | 100644 | 6732 |
| `src/Core/NoFollowFiles.cs` | 100644 | 18537 |
| `src/Core/SitkaLab.Core.csproj` | 100644 | 264 |
| `src/Core/SnapshotValidator.cs` | 100644 | 7330 |
| `src/Core/Views.cs` | 100644 | 7643 |
| `src/Host/LabHost.cs` | 100644 | 16292 |
| `src/Host/Program.cs` | 100644 | 1192 |
| `src/Host/SitkaLab.Host.csproj` | 100644 | 748 |
| `src/Ui/wwwroot/app.js` | 100644 | 23386 |
| `src/Ui/wwwroot/index.html` | 100644 | 8088 |
| `src/Ui/wwwroot/styles.css` | 100644 | 7069 |
| `tests/Core.Tests/FileFormatTests.cs` | 100644 | 13347 |
| `tests/Core.Tests/Fx.cs` | 100644 | 2691 |
| `tests/Core.Tests/LabDataStoreTests.cs` | 100644 | 42015 |
| `tests/Core.Tests/LayeredGuardTests.cs` | 100644 | 9983 |
| `tests/Core.Tests/MetricsAndGeometryTests.cs` | 100644 | 9826 |
| `tests/Core.Tests/OperationFuzzTests.cs` | 100644 | 5358 |
| `tests/Core.Tests/PersistenceAndReplayTests.cs` | 100644 | 10474 |
| `tests/Core.Tests/Program.cs` | 100644 | 73 |
| `tests/Core.Tests/SitkaLab.Core.Tests.csproj` | 100644 | 390 |
| `tests/Core.Tests/StateMachineTests.cs` | 100644 | 17054 |
| `tests/Host.Tests/HostTests.cs` | 100644 | 39896 |
| `tests/Host.Tests/Program.cs` | 100644 | 73 |
| `tests/Host.Tests/SitkaLab.Host.Tests.csproj` | 100644 | 523 |
| `tests/Shared/MiniTest.cs` | 100644 | 4134 |
| `tests/Ui/cdp.mjs` | 100644 | 6561 |
| `tests/Ui/run-browser-tests.mjs` | 100644 | 38259 |
| `tests/check-schema.py` | 100644 | 4300 |
| `tests/run-mutation-controls.py` | 100644 | 13241 |
| `tools/FixtureTool/Program.cs` | 100644 | 4875 |
| `tools/FixtureTool/SitkaLab.FixtureTool.csproj` | 100644 | 325 |
