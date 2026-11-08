# CCF #340 — F1–F3 raw-byte repair of the exact-dirty Unity lease proposal: SOURCE REPAIR (not deployed, not reviewed)

**Date:** 8 November 2026
**Status:** SOURCE REPAIR, tested offline on disposable fixtures. **Not deployed, not independently reviewed, not approved.** No lease, Unity, registered-lane, deployed-CCF-Ops or ZX20 change was made. **No source acceptance without #341 review.**
**Task:** [#340](https://github.com/jermur788/CCF/issues/340), bounded safety-source repair. Binding packet: `MANAGER-339-ADVERSE-SOURCE-REVIEW-AND-F1-F3-REPAIR-PACKET-20261108.md` (read in full, including the stale-launch clarification).

```text
IMPLEMENTATION HANDOFF
Task: #340 — repair F1/F2/F3 false accepts in the #338 exact-dirty lease proposal (source only)
Worker: Anthropic Claude Sonnet 5.5 (claude-sonnet-5-5), Claude Code under T3 Code. One author, no subagents, no self-review.
Role: Source author (single writer)
Branch: fix/unity-lease-f1-f3-raw-byte-guard-20261108-v1  (worktree /media/jer/Files/CCF/Operations/CCF-UnityLeaseRawByteRepair-20261108, common store /media/jer/Files/CCF/Repository.git)
Base: 949cc0c19bed09ba3fad4c319e235575ac75f6a8 (#338 tested source). Provenance anchors: report head 280682a4b16af8b7d2cd9d19c5d8580458456de7, original base 7b9aedd14c04dc28cad061bdef24b2d590b21c61, #339 review 1257474f532094c5bda8dc90896efbe7f70898a0
Context commit: 4790c20918ca2a667b04d80262142d9b4151b60e (latest change to the canonical context set on both 949cc0c1 and origin/main 05e14182)
TESTED SOURCE COMMIT: 6b111b123dc5d03258e0481c499b0ca634b11a1d
HEAD (publication): given in the #340 HANDED-OFF comment and the final message (a file cannot contain its own commit SHA)
GITHUB HANDOFF PATH: Docs/Handoffs/ScenarioOne-Overnight-Continuation-20261108/UNITY-LOCK-F1-F3-SOURCE-REPAIR-RESULT.md
EXTERNAL EVIDENCE ARCHIVE: /media/jer/Files/CCF/Verification/CCF-UnityLeaseRawByteRepair-20261108/ (SHA256SUMS.txt)
```

**Reasoning effort:** the harness reports only the numeric value `10` (same unmappable scale as #338). It is **not confirmed equal to the packet's "High"**; weigh accordingly. The `[RUNNING]` receipt ([#340 comment 6722052487](https://github.com/jermur788/CCF/issues/340#issuecomment-6722052487)) was posted after a real preflight and before any source edit or test execution. Preflight reads (git/gh/df and reading the packet, the #339 report and the source) preceded it and are disclosed there.

## 1. Result in one paragraph

All three #339 false accepts reproduce **RED on the exact old source** (`38018c6f…`, exit 0, lock created, `exact-admission` event recorded) and **fail closed on the repair** (exit 2, no `lock.json`, no event). F1 is refused by a canonical-object-graph guard; F2 and F3 are refused by a new **authoritative raw-byte proof** that does not use filters, the index, stat data or Git's status/diff. The strict default clean path, the pinned eight-file profile, the #318 manifest, the lane/store/branch/old-HEAD pins, the exclusive flock, PID/stale/malformed handling, release/status and the error exit are byte-identical or untouched. The baseline 77-test suite (unchanged file) passes 77/77 and the new targeted suite passes 40/40, each run once on the final source.

## 2. What changed

Only `Tools/Verification/UnityLeaseExactAdmission/` (three files; baseline copies and the original test file are byte-identical).

| Change | Where | Why |
|---|---|---|
| `git_exact` now sets `GIT_NO_REPLACE_OBJECTS=1` and `-c core.useReplaceRefs=false` | `git_exact` | **F1.** Every admission Git call reads real objects. |
| `require_canonical_object_graph` | new, runs right after the evidence check | **F1.** Refuses `refs/replace/*`, `info/grafts`, shallow repositories and unsupported object formats; re-derives `HEAD^{commit}` unreplaced and requires it to equal the pin. |
| `parse_ls_tree`, `blob_object_id`, `prove_raw_working_tree` | new, runs after the existing pinned-byte loop | **F2/F3.** The authoritative proof (below). |
| `head_tree` and `raw_proof` (counts, bytes, seconds, object format) in the audit record | `admit_exact_dirty` return | Lets a reviewer see what was proved. |
| Comment block describing what an admission proves; `import io, time` | header | Explains the new contract. |
| Matching unified patch regenerated | `unity-lock-exact-admission.patch` | Still removes exactly the one original clean-gate line (asserted by the existing test). |
| Targeted tests | `test_raw_byte_admission.py` (new) | Section 5. |

**The raw-byte proof.** `ls-tree -r -l -z --full-tree HEAD` (unreplaced) lists the canonical blobs: it reads tree objects only, so no index, stat data, status or filter is involved. The working tree is then walked with `O_NOFOLLOW` directory descriptors. Every tracked path **outside the pinned dirty set** is read as raw bytes and its Git object id (`blob <size>\0` + bytes, SHA-1 or SHA-256 per the repository's object format, streamed in 1 MiB chunks) must equal the tree's id. A size mismatch only refuses sooner; equality is never inferred from size, mtime or the index. Anything in the working tree that is not a tracked path is refused. The pinned paths keep their existing exact sha256/size/mode and HEAD-blob checks.

| Situation | Behaviour (fail closed unless stated) |
|---|---|
| Tracked file bytes differ, missing, or replaced by a directory | refuse |
| Symlink (tracked as a regular file, or untracked) | refuse; a tracked symlink (none exist in the real tree) is accepted only if a real symlink with identical target bytes, otherwise refuse |
| FIFO/socket/device, tracked or not | refuse, never opened (no blocking) |
| Untracked file anywhere outside the allowed ignored entries | refuse, independent of Git status, `info/exclude` and the untracked cache |
| Allowed ignored entries (`Library/`, `Logs/`, `UserSettings/`, the three root files) | not walked; type must match (`Library/` must be a real directory). A tracked path *inside* one is refused as ambiguous |
| Root `.git` | accepted only as a regular file (worktree pointer); a `.git` directory is refused |
| Staged changes, unmerged, assume-unchanged, skip-worktree | unchanged existing checks (`status`, `diff-index --cached`, `ls-files -v`) |
| Submodule/gitlink, unusual mode, non-UTF-8 or repeated path, unparseable `ls-tree` | refuse |
| Any Git error, `OSError` (unreadable file, EIO), file length changing while read | refuse (`OSError` is converted; the main wrapper already exits 2) |
| Empty untracked directories | **tolerated** (Git cannot see them, they hold no bytes) |
| Unix mode of unrelated tracked files | **not compared** (exFAT reports one mode; the pinned eight keep the strict mode check) |

Git's status/diff/index checks are kept as a cheap pre-filter and for unchanged error messages. They are no longer what admission rests on.

## 3. Red → green

| Case | Old source `38018c6f` | Repaired source | Refused by |
|---|---|---|---|
| F1 replace ref substitutes pinned HEAD tree (#339 fixture) | **FALSE ACCEPT** (exit 0, lock, event) | exit 2, no lock, no event | graph guard; with the guard removed, still refused by unreplaced Git (`unexpected Git status entry`) **and** by the raw proof alone |
| F2 repo-local clean filter normalises drift (#339 fixture) | **FALSE ACCEPT** | exit 2, no lock, no event | raw proof (Git view blind, `status` shows nothing) |
| F3 non-racy same-size edit, restored mtime, `trustctime=false`/`checkStat=minimal` (#339 fixture) | **FALSE ACCEPT** | exit 2, no lock, no event | raw proof |
| Variant: `core.autocrlf=true` CRLF working bytes | **FALSE ACCEPT** | refused | raw proof |
| Variant: `info/grafts` forges required ancestry (post-transition fixture) | **FALSE ACCEPT** | refused | graft refusal |
| Sentinel: plain unrelated edit | refused | refused | existing status check |

Each case first admits and releases the uncontaminated fixture (positive control), and every preceding Git precondition ("ordinary status is fooled") is asserted. Evidence: `red-old-source-final.log` (5 FALSE ACCEPT failures, 0 precondition failures), `failclosed-old.json`, `failclosed-repaired.json`.

**Correction recorded.** I first drafted two further variants (a `git replace` of a *blob*, and `autocrlf` without re-adding the file). Both failed their own "Git is fooled" precondition on the old source, so they were never false accepts: Git compares index and HEAD object ids literally, and without `git add` the CRLF file shows stat-dirty. The blob variant was dropped; the autocrlf variant was fixed to follow the same fixture-only `git add` step the #339 reviewer used for F2. My first red count ("six") was wrong; the correct count is **five**.

## 4. Verification

| Check | Result |
|---|---|
| F1/F2/F3 red on old source (disposable fixtures, dummy lock) | **PASS** (reproduced: 5 genuine false accepts) |
| F1/F2/F3 fail closed on repair (exit 2, no lock.json, no event) | **PASS** (all five + three reviewer fixtures) |
| Clean default admission (identical to deployed, no new fields) | **PASS** (existing `DefaultStrictClean`, 4 tests) |
| Exact old-eight positive, audit record, release, stat-noise-only tolerated | **PASS** (existing `ExactPreTransition`, 5 tests; new audit/counter test) |
| One unrelated raw change rejected | **PASS** |
| Git-error never accepts (every new Git call failing; `OSError` mid-proof) | **PASS** |
| Owner / exclusive-lock sentinels (held, stale, malformed, two-process race ×4, branch conflict, unregistered lane, Editor PID) | **PASS** (existing `LeaseSemantics`; new sentinel: held lock refused before any raw work; default path never calls the new proof) |
| Baseline suite `test_exact_admission.py` (file unchanged), one complete pass on final source | **PASS 77/77** (10.1 s) |
| New suite `test_raw_byte_admission.py`, final source | **PASS 40/40** (6.2 s) |
| Patch reproduces proposed exactly; removes only the original clean-gate line | **PASS** (existing test + manual `patch`/`cmp`) |
| Deployed `unity-lock` / `_ccf.py` hashes (read-only) still the pinned baseline | **PASS** (`54cf0a95…`, `ab4343ef…`) |
| Repeated 77+27 mutation campaigns | **NOT RUN** (packet forbids; no precision claimed from mutation counts) |
| Proposed function against the real registered lane (even read-only) | **NOT RUN, by design** |
| exFAT behaviour, real device throughput, real lane contents | **NOT RUN** (fixtures are ext4) |
| Unity / real lease / deployment / R1 / B3 / integration | **NOT RUN** (out of scope) |

## 5. New tests (40)

`FalseAcceptReproductions` and `GraftOnPostFixture` (6 black-box tests through `main()`, red on old). `RawProofBehaviour` covers: unreplaced reads, replace/graft/shallow/object-format/HEAD refusals; counts and audit record; **the proof ignores a corrupted index and never runs configured filters**; one flipped byte in each unrelated file; size changes; symlink and symlinked-directory substitution (also with `core.symlinks=false`); tracked-symlink target comparison; FIFOs (alarm-guarded, no blocking); untracked files under `info/exclude`, and a new file under a directory whose mtime was restored with the untracked cache enabled (the test asserts the raw proof refuses; it does not claim Git status is fooled there); tolerated empty directories and allowed-ignored contents; tracked path inside an ignored entry; type confusion on `Library`; unreadable file; stream length disagreeing with `fstat`; `ls-tree` parser refusals; object ids equal `git hash-object --no-filters` across 1 MiB chunk boundaries (SHA-1 and SHA-256 repositories); every new Git call failing; and the sentinels above. `RawProofPostTransition` proves the clean-pinned package JSONs against the new HEAD.

## 6. Cost and resource implications

The pinned old HEAD `4d95e452` tree (from the object store, not the lane) is **5,732 regular files, 1.138 GiB**, largest 25.3 MiB, **no symlinks, no gitlinks, no `.gitattributes`**. Disposable data with the same paths/sizes (random bytes, ext4, `lane/.git` a pointer file), via the repaired `prove_raw_working_tree`:

| Run | Seconds | Throughput |
|---|---|---|
| Warm cache ×3 | 2.09–2.12 | ≈ 585 MB/s |
| Pages evicted (`posix_fadvise DONTNEED`) ×2 | 35.7, 21.6 | 34–57 MB/s on this disk |

Peak RSS ≈ 71 MiB for the whole process including file generation (the proof streams 1 MiB chunks). Comparison: the pre-filter `git status` was 0.03 s warm. Real cost ≈ 1.14 GiB ÷ the ZX20 cold-read rate (illustratively ~12 s at 100 MB/s, ~40 s at 30 MB/s); **not measured on ZX20** (forbidden). Implications: the proof runs inside the exclusive guard, so `status`/`release` callers wait for it; a failing or stalled device means refusal (or an indefinite block on a hung read — there is no timeout; I did not add one); `MemoryError` is not caught by the main wrapper (would exit 1 with a traceback, but no lock or event is written before the proof completes).

## 7. Must-not-change anchors

Unchanged and asserted by the existing suite: eight protected sha256/HEAD-blob/size values and `0o100755` mode, #318 manifest `9fb80596…`/16 files, lane `WorkbenchTier2Reg287`, store, branch, old HEAD `4d95e452…`, post-transition `head: None` (UNSUPPORTED) and its six-file/must-descend pin, flock/guard/process-identity/release/status text, the `Unity lane is dirty` message and single acquire site, five-option activation and acquire-only scope. **Intentionally changed:** `unity-lock.proposed` (`38018c6f…` → `b0720660…`), the patch (`deb541e7…` → `6c3b1f55…`), and the new test file.

| Path | SHA-256 at tested commit `6b111b12` |
|---|---|
| `unity-lock.proposed` | `b0720660528cdf9c215ea8627b033aecf4e318c79483c7031c8c07cdaf50bddc` |
| `unity-lock-exact-admission.patch` | `6c3b1f55069cbde9bc2c66358e79c70372d8a73c1c14485e83d585aaaafddbc2` |
| `test_raw_byte_admission.py` (new) | `2b612105b6faa00266e105f99a0645e630400ac9b5554ddfb64273caf57fee66` |
| `test_exact_admission.py` (unchanged) | `7b9122f370b723638e0a67d8f5d28e97f21291303e307b8daeb6417cdcc95afd` |
| `unity-lock.deployed-54cf0a95.txt`, `_ccf.deployed-ab4343ef.txt` (unchanged) | `54cf0a95…048e5`, `ab4343ef…de001` |

## 8. Known limits and decisions for the Manager

- **Object-id trust.** Equality is proved through Git's own object id (SHA-1 here), not a second byte comparison. This is the same collision-resistance assumption as trusting the pinned HEAD commit id. A tampered object database is out of scope.
- **Mode.** Unix mode of unrelated tracked files is not part of the proof (bytes only); empty untracked directories are tolerated. Say so if either should be refused.
- **Tracked symlinks / `core.symlinks=false` checkouts are unsupported** (refused). The real tree has none today.
- **Deployed default clean path is unchanged by design** (strict-default preservation) and therefore remains exposed to the same F1–F3 concealment techniques for ordinary clean-lane admission. Out of this packet's scope; flagged, not fixed.
- **Post-admission drift** is not addressed (as in #338); admission is per-invocation, not a consumed token.
- **ext4 only.** exFAT inode noise, case-insensitivity and real throughput are untested; a cold-cache run on ZX20 may take tens of seconds.
- **Expiry** (`2026-11-11`) and strict ignored/mode pins are untouched and **not adopted as policy**; post-transition stays UNSUPPORTED; no allow-dirty or R1 relaxation was added.
- **Process notes.** Effort value `10` unverified as High. Nothing was applied to the real lane. Superseded interim logs are retained in the evidence directory and labelled.

**Worker assessment — Ready for review: Conditional** (source only; #341 must independently falsify F1–F3 and probe for new bypasses, in particular the walk/allowed-ignored logic and the tolerated cases above). **Recommended next step:** start #341; do not deploy, lease or run Unity on this source before a Manager ruling.
