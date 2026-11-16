# Multi — #644 bounded F0 file-safety correction after #643 independent review

**Date:** 16 November 2026  
**Manager:** OpenAI ChatGPT GPT-6, #641, browser-only.  
**Verdict on #643:** **NEEDS CHANGES accepted** (1 BLOCKER, 2 MAJOR, 3 MINOR). #639 stage-1 is source-implemented, independently validated for manifest/trust identity, **NOT ACCEPTED for first ecological-year execution**.
**Repair claim:** [#644](https://github.com/jermur788/CCF/issues/644) **READY / NOT RUNNING**; actual author must register [RUNNING] on #644 before source edits. **Original source #639 remains OPEN; its custody stays reserved.**  
**Independent review:** [#643 result](https://github.com/jermur788/CCF/blob/review/multi-p1-643-manifest-p0-trust-file-safety-20261116-v1/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-2D-SITKA-P1-643-INDEPENDENT-30M-MANIFEST-P0-TRUST-FILE-SAFETY-RESULT-20261116.md). **Full existing author result:** [#639 F0](https://github.com/jermur788/CCF/blob/0636ddc39cc5e8bc5c31f72ed9c6e81846f3c7f5/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-Sitka-P1-639-F0-THIRTY-METRE-FOUNDATION-IMPLEMENTATION-RESULT-20261116.md).

## 1. Exact ownership and starting target

**Best suited:** resume the **existing Anthropic Claude Code Opus 5.5** author on **native Ubuntu laptop**. Difficulty **MEDIUM–HIGH** because three tightly scoped hostile-filesystem write/read paths must be closed and verified without harming qualified P0; reuse actual single writer, do not start a second agent or relocate the worktree.

**Exclusive existing source lane:** `/home/jer/ccf-worktrees/multi-p1-639-f0-30m` on `jer-laptop`, branch `feature/multi-p1-639-f0-30m-foundation-20261116-v1`, published starting candidate **`0636ddc39cc5e8bc5c31f72ed9c6e81846f3c7f5`**. All production edits under existing `Tools/MultiRackSitkaP1Lab/**` reservation only. Existing #639 handoff exact path remains reserved if updated; prefer publish a **new result Markdown under this packet's path** only once coordinated with the live sole board writer as necessary; otherwise append a committed evidence section to existing #639 result without creating an unreserved write outside allowance. The **requested new result path** is `Docs/Handoffs/MultiRack-Simulation-20261116/Multi-2D-SITKA-P1-644-F0-FILE-SAFETY-REPAIR-RESULT-20261116.md`. This path is **not yet in the live board reservation**: before using it, obtain explicit prospective approval/reservation from the current board writer; do not silently widen #639. If the board writer cannot grant promptly, use the existing reserved #639 result document instead and link it on #644.

**Preflight required:** refresh live `main:Docs/Project/coordination/active-tasks.md`, #639/#644/#641 and #643; inspect actual laptop worktree branch/HEAD, full tracked/untracked/ignored state, author/collision/physical machine availability and original P0 protected subtree; register actual `[RUNNING]` claim on #644 with provider/model, host, branch/worktree and starting head before edits. STOP if dirty/unexpected or custody differs; preserve all unrelated work. The live board now says #640 Scenario One Manager holds sole board writer authority after a prospective transfer from #425; **#641 Multi manager must not write board directly**, and the worker must refresh any later transfer.

**Protected/no writes:** `Tools/MultiRackSitkaLab/**` including ignored/build products; `Assets/**`, `Packages/**`, `ProjectSettings/**`, game source, old saves, specs, live board, frozen original P0 and Reference Future. No Unity/editor/lease, Windows build, numerical Unity parity or first ecological year.

## 2. Frozen successfully reviewed values

Do **not modify** the 32,567-byte `fixtures/derived/p1d-30m-sitka-adult-v1.sitkap1fx.json`, its deterministic 100-tree generation, IDs/positions/age/DBH/height/crowns/f32 bits, content and file SHA, catalog digest or any of the three original-P0 immutable source fixtures/trust bytes, lengths, snapshot hashes or table digest. Both compiled status records remain **`PendingIndependentReview`**. No reviewer/Manager acceptance refs inserted. #643 independently passed these source/data values (not native security execution); do not repeat full manifest/hash review unless repair changes those inputs.

The starting relative density ~0.6098 is a **nonblocking experimental calibration**. Do not change the generator to create a higher-pressure stand. A later separately named synthetic stress fixture could be considered under a separate authorisation.

## 3. Scope of mandatory corrections

### B-01 BLOCKER — manifest generator's unsafe destination
`src/Cli/Program.cs` around lines 15–20: `generate-manifest <out>` currently passes a caller path to `File.WriteAllBytes`, which can overwrite qualified P0. Replace with an explicit P1-owned output policy or an independently validated external scratch output root **never under protected P0**, robust against traversal, symlinks in ancestor/leaf, hard-link aliases, and pre-existing files. Use descriptor-anchored no-follow creation or a comparably demonstrable safe approach; refuse overwrite and escape. Test a protected-looking path with **safe scratch stand-ins only**, including traversal, existing, symlink/hardlink and directory cases; never execute a destructive test against real P0. No path should write to P0 even if user passes a malicious/mistaken location.

### M-01 MAJOR — build-output symlink redirect
`build/build-p0-core-ref.sh` around lines 19–21 and 60–72: ignored `.p0ref` can be a pre-existing symlink to the protected P0 directory; `mkdir -p` accepts and `cp`/provenance/checksum writes follow it. Refuse symlinked or uncontrolled P1 output root before any copy/write and **anchor creation under a verified P1-only root**; verify safe root and file destinations with no-follow protections proportional to this local script. Preserve external temporary `git archive`/P0 unchanged behavior and deterministic DLL SHA contract. Add a safe `.p0ref` symlink-to-scratch-target negative and direct `build-p0-core-ref.sh` check, not only the clean cold-run harness. Do not clean or repurpose any existing ignored user folder except known newly created test-owned scratch.

### M-02 MAJOR — derived verification reads before safety checks
`src/Cli/Program.cs` around lines 22–25 and `src/Core/FixtureAccess.cs` around lines 101–114: `verify-derived` uses `File.ReadAllBytes` on the provided path before bounded verification, follows symlinks, may block on FIFO and allocate an oversized file. Reuse/adapt descriptor-relative no-follow bounded read policy with exact approved derived catalog length, opened-descriptor single-link regular-file verification, and strict canonical byte/digest checks **before processing**. Define safe root/admission rules explicitly and preserve fail-closed status/refusal, ordinary valid-file verification and domain separation. Add scratch-only negative tests for symlink ancestor/leaf, hardlink, FIFO, directory, oversized and wrong-domain data. Do not expand to Host, saves or other unavailable systems.

## 4. Two proportionate minor improvements, one deferred

- **m-01:** `src/Core/DerivedManifest.cs` add validation that 100-tree XY positions are unique (and a negative), **without changing existing fixture bytes/generator**.
- **m-02:** `tests/p0-isolation.sh` close or document traced syscall and relative-path visibility limitations; add focused coverage for safe simulated relative-path writes if feasible. Never claim a benign `strace` run proves every possible filesystem write impossible. The strongest claim must match the actual observed scope.
- **m-03:** density barely above mortality onset — **DEFERRED, NONBLOCKING**. Preserve reviewed data.

**Stop condition:** if secure output/read handling requires an unapproved architecture change or a source edit outside allowlist, report BLOCKED with precise options. Do not weaken existing tests to green.

## 5. Mandatory verification and handoff

Implement only scoped guards/tests. Run normal original targeted tests plus new focused negative tests, full cold P0 L0–L4 and an adversarial safe-scratch output-root/relative path suite on actual Ubuntu, with exact branch/HEAD and source/P0 tracked+ignored before/after checks. Explicitly exercise the documented builder and CLI paths, not just helpers. Compare full original-P0 52 tracked-file digests and ignored-file inventory before/after; no writes on actual P0. Record actual commands, test counts, nonzero refusal exits, stdout/stderr, negative-case effects and trace limitations. Any impossibility/unsupported platform stays explicit. Confirm fixture, derived catalog, three trust entries and pending statuses byte-identical; verify source diff against `0636ddc3...` is scoped. No need to run Unity, annual ecology, Host, save/replay, C-2b or a broad Tier 3 suite.

Publish durable GitHub result at approved exact path above (or updated existing reserved #639 path if new path lacks live writer approval) with actual [RUNNING]/[HANDED-OFF] #644 claim receipts, base/final candidate, path-by-path diffs, test logs/storage references, any remaining risks and declaration **NO ECOLOGICAL YEAR RUN**. Stop with trust records pending. #639 source issue remains open.

## 6. Subsequent review and Manager disposition

After source repair, Manager #641 inspects exact final candidate and opens **one independent narrow changed-lines/security delta check** (ideally reviewer #643, separate review result lane; no need for a repeat of all 100 trees and original hashes if immutable). This review must re-evaluate B-01, M-01 and M-02 and proportional m-01/m-02 tests against actual code and evidence; no automatic source merge/acceptance. Manager then decides **APPROVED / NEEDS CHANGES / REJECTED**. Only after APPROVED may Manager authorise a **separate reviewed acceptance-status commit** for both catalogs and later first ecological-year implementation; an approval of this repair is not itself a code/runtime/Unity-parity proof.

## 7. Paste-ready source-worker dispatch

> Resume existing Multi-Rack P1 source ownership on **#644** as the same Claude Opus 5.5 native Ubuntu laptop worker. Read the COMPLETE [Multi #644 bounded file-safety repair handoff](https://github.com/jermur788/CCF/blob/docs/multi-p1-644-f0-security-repair-20261116-v1/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-2D-SITKA-P1-644-643-FILE-SAFETY-BOUNDED-REPAIR-PACKET-20261116.md). Refresh live coordination and #639/#644, verify your existing worktree/branch/current HEAD/P0 untouched, register your actual [RUNNING] on #644 before editing. Correct B-01/M-01/M-02 and bounded m-01/m-02 only, preserve the fully validated manifest and all three original-P0 trust identities byte-identically, run focused native scratch-only negatives and P0 isolation checks, publish the approved exact GitHub result, then [HANDED-OFF]. No annual ecology, acceptance-status promotion, P0 writes, Unity, Windows build or second worker.

**User action:** send this exact dispatch to the **existing** Claude source session (not a new worker). No new product decision is needed. **Dashboard, Drive, physical Ops sync remain NOT VERIFIED**.
