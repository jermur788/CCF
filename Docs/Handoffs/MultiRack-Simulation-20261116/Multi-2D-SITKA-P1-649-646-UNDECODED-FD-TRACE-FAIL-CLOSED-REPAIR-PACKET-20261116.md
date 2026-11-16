# Multi — #649 tiny L4 undecoded file-descriptor fail-closed correction

**Date:** 16 November 2026  
**Management verdict:** #641 **NEEDS CHANGES** accepted on the [independent #646 result](https://github.com/jermur788/CCF/blob/review/multi-p1-646-644-file-safety-delta-20261116-v1/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-2D-SITKA-P1-646-INDEPENDENT-644-FILE-SAFETY-DELTA-RESULT-20261116.md).  
**Repair claim:** [#649](https://github.com/jermur788/CCF/issues/649), READY / NOT RUNNING until actual source author registers.  
**Existing source task:** [#639](https://github.com/jermur788/CCF/issues/639), [#644](https://github.com/jermur788/CCF/issues/644).  
**Accepted from #646 at source-review level:** B-01 generator no unsafe output path; M-01 build-root symlink/hardlink protection (trusted single-user non-racing case); M-02 catalog-bounded derived loader; m-01 duplicate XY; **frozen fixture and original P0 identities**, unmodified. No ecological year has run or is authorised.

## 1. Assignment and exact existing lane

**Purpose:** make one trace analyzer fail closed on a successfully traced file mutation whose descriptor is not annotated with a pathname, then prove each expected self-test mutation is observed. The #646 reviewer identified one MAJOR in *verification tooling*, not an implementation regression in the already-passed P1 safety fixes.

**Difficulty:** LOW–MEDIUM, narrow Python parsing and finite negative tests; careful fail-closed handling and proportional Ubuntu witness needed. **Best suited provider/model:** **same existing source worker Anthropic Claude Code Opus 5.5**, effort low, native Ubuntu laptop, NOT a new second writer.

**Existing exclusive branch:** `feature/multi-p1-639-f0-30m-foundation-20261116-v1`, published head at intake `33c0506990eefb6bea4277a553d2fe38408bb353`. **Exclusive worktree:** `/home/jer/ccf-worktrees/multi-p1-639-f0-30m`, host `jer-laptop`, native Ubuntu 24.04 x64. Original qualified P0 `Tools/MultiRackSitkaLab/**` remains read-only and must preserve its source/ignored output. The live main board has #639's single-writer reservation for `Tools/MultiRackSitkaP1Lab/**` and exactly the existing #639 result Markdown. #640 is the sole current live-board writer; #641 does not edit it.

**Writable allowlist:** `Tools/MultiRackSitkaP1Lab/tests/l4_analyze.py` and, **only as necessary** for precise synthetic cases/expected assertion, `Tools/MultiRackSitkaP1Lab/tests/p0-isolation.sh`. Write the durable repair handoff as **new §9 in existing reserved result** `Docs/Handoffs/MultiRack-Simulation-20261116/Multi-Sitka-P1-639-F0-THIRTY-METRE-FOUNDATION-IMPLEMENTATION-RESULT-20261116.md`. Do **not** use a new unreserved result document. No changes to production Core/CLI, build script, fixtures, catalogs, accepted specs, live coordination or P0.

**Mandatory preflight:** refresh current live main board and #639/#644/#649 issue comments; recheck actual model/provider, worktree clean-state including ignored protected P0, branch current HEAD and origin, original P0 tree, single writer and machine/Unity state. Post an actual **[RUNNING] #649 receipt** with provider, host, branch, worktree and starting head before editing. If any drift/conflict/dirty unrelated work, mark [BLOCKED]; never stash, clean, overwrite or discard unrelated work. No extra ownership requested for existing file reservation.

## 2. Exactly one blocking fix: unresolved descriptor handling

Affected [`tests/l4_analyze.py` lines 109–150](https://github.com/jermur788/CCF/blob/33c0506990eefb6bea4277a553d2fe38408bb353/Tools/MultiRackSitkaP1Lab/tests/l4_analyze.py#L109-L150). Current regex `TOKEN` only yields descriptors annotated like `9</path>` or quoted path literals; a trace entry such as `1234 ftruncate(9, 0) = 0` yields no target and leaves `unknown=False`, so L4 may erroneously return PASS. Same issue for `fallocate(9, 0, 0, 4096)`, `fchmod(9, 0600)`, other fd-only mutation operations. **Source-level review finding, not observed on actual strace**.

Implement explicit classification for every successful fd-only mutator in the currently covered trace list. If the descriptor target cannot be resolved to a reliable pathname, record **UNRESOLVED** and return non-zero. If a successful traced mutator of another form has no reliable pathname and no safe explicit target, likewise fail closed instead of silently passing. Preserve existing resolved-path P0 hit classification, legacy relative cwd handling, `*at` semantics, and read-only calls. Avoid false P0 hits from unresolvable paths; mark them UNRESOLVED. Error/failed syscall classification must be explicit (don't count failed nonmutating attempts as confirmed writes).

**Narrow required synthetic negative/control evidence:**
- Bare fd mutating calls `ftruncate(9, 0) = 0`, `fallocate(9, 0, 0, 4096) = 0`, `fchmod(9, 0600) = 0` produce `UNRESOLVED` and exit failure.
- Resolved protected fd `ftruncate(9</scratch/protected/file>, 0) = 0` is a protected **HIT**. Resolved benign fd `ftruncate(9</scratch/outside/file>, 0) = 0` is not a hit or unresolved.
- A read-only `read(9, ...)` or `openat(..., O_RDONLY)` control does not cause a false mutation.
- Self-test asserts **identity of each scripted expected write** (not merely `protectedHits >= 13`). The present 13/13 count must not be satisfied by duplicated sightings of one operation while missing another. Use a minimal self-contained deterministic trace/decoy fixture, never actual P0, and record how operation identity is certified. Preserve current 13-call decoy intent unless impractical, in which case justify exact alternative.
- Verify no accidental success for a successful path-based mutation lacking both a recognized target and a reliable pathname.

The analyzer's current declared limitations remain: only the traced syscall set and traced descendant processes, lexical intermediate symlink handling, and per-thread cwd limitations; deletion syscalls were not exercised by #644 and do not require dangerous new deletion tests. Do not exaggerate L4 PASS to global prevention. The builder's same-user race is a separately disclosed F0 local-host limitation, not part of this repair.

## 3. Proportional Ubuntu verification (same worker only)

Run focused Python analyzer self-tests with exact exit values, a small scratch-only synthetic trace, then **one** proportionate cold P0 L0–L4 witness through existing harness after the change. If a native trace contains undecodable descriptors that fail closed, preserve that diagnostic and report [BLOCKED] rather than whitelist/ignore them to green. No test weakening. Record counts, unique expected-operation coverage, examples of resolved/unresolved and P0 hits, 52 original P0 file hashes/ignored pre/post unchanged and the actual traced scope. If existing 33 Core tests or 8/8 builder/23/23 CLI tests are not affected, they need not all be rerun beyond what the existing cold harness does. No Tier 3/integration suite, no repeated fixture hash/source-data audit.

Confirm Git diff from `33c0506990eefb6bea4277a553d2fe38408bb353` touches only authorized analyzer/self-test files and the pre-reserved #639 Markdown. Independently unchanged at Git blob level: 30m fixture, `TrustTables.cs` (3 original P0 + one derived status all **PendingIndependentReview**), `ManifestGenerator.cs`, pins and all original P0. Do not edit or promote values. No annual engine or year.

Publish one durable **§9 #649 repair result** in existing #639 result document with actual worker identity, exact base/final commits, changes, test outputs/evidence directory, limitations and request for one narrow independent recheck. Post [HANDED-OFF] on #649, notify #641/#644/#639 with exact GitHub section link. Keep all source claims open until Manager disposition; do not self-approve.

## 4. Next independent recheck / Manager gate

Once result is published, #641 will create one **non-author** focused changed-lines recheck of `l4_analyze.py` and the precise self-test, using a separate GitHub-only reviewer branch and distinct result Markdown. The reviewer should check bare fd -> UNRESOLVED, decorated protected fd -> HIT, decorated benign fd -> no hit, read-only control -> not mutating, unique expected mutation identities and any undercount. It should be an independent static/code review of the exact post-repair delta with transparently unrun native tests.

The full 100-tree synthetic forest and three original-P0 SHA/snapshot records have already independently passed, as have B-01/M-01/M-02/m-01 code corrections at source level. **Do not redo those audits unless these files change.**

Manager #641 must issue a separate **APPROVED / NEEDS CHANGES / REJECTED** decision. First ecological year **remains HOLD** pending approved independent review, explicit Manager acceptance, and separate reviewed status promotion of the four pending records. No Unity C-6, Unity numerical parity, Windows build or original-P0 annual support.

## 5. Copy/paste dispatch

> Resume the existing exclusive Multi P1 Ubuntu source lane for **#649** as the same Claude Opus 5.5 author. Read the COMPLETE [Multi #649 bounded unresolved-file-descriptor repair packet](https://github.com/jermur788/CCF/blob/docs/multi-p1-649-l4-undecoded-fd-repair-20261116-v1/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-2D-SITKA-P1-649-646-UNDECODED-FD-TRACE-FAIL-CLOSED-REPAIR-PACKET-20261116.md). Refresh live ownership and your exact worktree/head, post [RUNNING] on #649, then change only the L4 analyzer and essential synthetic self-test to fail closed on bare mutating fds, assert unique expected operations, and rerun focused native L4 verification. Preserve all passing #644 source repairs and byte-identical 30m fixture/P0 trust values. Append §9 repair evidence to the already-reserved #639 result Markdown, hand off #649 for independent delta review. No first ecological year, trust promotion, P0/Unity edits or Windows build.

**User action:** paste this dispatch into the **existing Claude source chat** on the native Ubuntu laptop, not a separate worker or a second worktree. **Dashboard/Drive/physical Ops: NOT VERIFIED**.
