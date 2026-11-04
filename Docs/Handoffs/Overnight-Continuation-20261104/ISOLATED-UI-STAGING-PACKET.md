# #269 — Isolate reviewed overnight UI deltas; preserve active playtest

**Status:** READY — source-only Git staging and independent review. **NO production integration/Unity authorisation.**  
**Date:** 2026-11-04. **Claim:** https://github.com/jermur788/CCF/issues/269  
**Purpose:** bring the reviewed bounded UI/copy/verification deltas into one exact-candidate lineage for **later** Unity verification without accidentally importing hundreds of unrelated changes from divergent Git histories.  
**Difficulty:** Medium–High — Git provenance, whitelist-only replay and combined-test semantics; no forestry implementation.  
**Orchestrator:** DeepSeek DPC #203/#254. **Worker:** T3 Code (Claude Sonnet 5.5 Medium or GPT-6.1 Medium by observed availability); reviewer from the other provider. User permits warranted escalation to higher effort or Opus 5.5, but document the reason and capacity in the ledger; default Medium.
**Published candidate source:** `integration/scenario-one-player-test-ready-v1` last observed at `72b4efbd...`. Freshly resolve actual candidate and confirm it has not moved; do not infer this was the exact local build under the user's earlier playtest.
**Conditional new integration-preparation lane:** branch **`integration/scenario-one-overnight-ui-qualification-v1`**, worktree **`/media/jer/Files/CCF/Operations/CCF-Overnight-UIQualification-20261104`**, created **from published candidate only** if branch/path are empty, writer unclaimed and storage safe. If occupied or dirty, STOP and request Manager reassignment; no cleaning, stashing or overwriting. **No Unity Library or Editor.** All modifications belong to this isolated worktree and branch.

## Why PR #261/#262/#264 must not merge directly

GitHub compares these PRs **against `main`**, which has diverged from the published candidate by hundreds of commits. Each PR incorrectly appears to change about 290 files. A direct PR merge could absorb unrelated simulation, save, model, UI and test history. By contrast, a compare of their exact PR heads **against `integration/scenario-one-player-test-ready-v1`** shows only intended bounded files. Use the latter comparison and independently enumerate every changed path before writing.

## Exact allowed source paths

From [Objective copy #255 / PR #262](https://github.com/jermur788/CCF/pull/262): two player-facing copy files:
- `Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs`
- `Assets/ForestPrototype/UI/MenuHelpView.Text.cs`

plus specifically reviewed **test corrections** (not an authorisation to suppress tests):
- `Tools/Verification/RackV2HelpTeaching/RackV2HelpTeachingOfflineCheck.cs`
- `Tools/Verification/RackV2HelpTeaching/run_offline.py`
- `Tools/Verification/RackV2Objective1/RackV2Objective1EndToEnd.cs`
- `Docs/Handoffs/Overnight-Continuation-20261104/OBJECTIVE-COPY-HANDOFF.md`

From [rack handle #256 / PR #261](https://github.com/jermur788/CCF/pull/261):
- `Assets/ForestPrototype/UI/Map/StandMapRackEditor.cs`
- `Assets/ForestPrototype/UI/StandMapView.cs`
- `Docs/Handoffs/Overnight-Continuation-20261104/RACK-HANDLE-HANDOFF.md`

From [source-static baseline #260 / PR #264](https://github.com/jermur788/CCF/pull/264):
- `Tools/Verification/OvernightPlaytestFeedback/README.md`
- `Tools/Verification/OvernightPlaytestFeedback/baseline-output.txt`
- `Tools/Verification/OvernightPlaytestFeedback/check_baseline.py`
- `Docs/Handoffs/Overnight-Continuation-20261104/BASELINE-VERIFICATION-HANDOFF.md`

**13 exact files.** Do not import other changed files, including forest/simulation/save data/geometry, frozen references or `active-tasks.md`. Files not present at exact candidate base must be created only where the PR's candidate-relative diff adds them. Keep existing user playtest and staged intro branches unmoved.

## Replay and verification

1. Refresh live ownership and all three claims, validate current branch/HEAD, and compare each PR **head against the exact published candidate** (not against main). Record file paths and patch/commit provenance; stop if diffs no longer match the whitelist.
2. In the new safe isolated source-only lane, transplant **exact intended deltas** by controlled cherry-pick or indexed patch without bringing unrelated history. Do not rewrite unrelated file content. Make one clear stage commit or a logical sequence with meaningful review diffs. Ensure no unrequested text/rule changes.
3. Inspect the two test updates from #255: source assertion labels adjust to **exact** requested copy and `UnitStyle` only normalises the one approved unit string. Do not remove checks, expand allow-lists broadly, accept substring-only equivalence, or suppress red tests. Verify source copy explicitly preserves current semantics of orange rack stripes, red Fell *label*, blue Crop *label*, while #259's unresolved world/map colour discrepancy stays open.
4. Run executable **offline** checks supported by the source-only environment. Reconcile the #255 97-check pass, #256's focused six snap cases (the unrelated previous 10/10 static gate does NOT cover physical drag), and combined #260 `PASS=9 FAIL=1 NOT-TESTABLE=2`. The remaining FAIL is expected **Objective 1 CTA still points to Back to forest**, which #258 has *only drafted*; do not make the checker green by relaxing it. Historic S10 branch-scope allow-list may fail due combined changed-file footprint; label correctly as scope failure without weakening tests.
5. Independently review all exact file deltas, source compile plausibility, test assertions, and preserved rights. No claims of Unity compilation/input/renders/scene playback or user acceptance. For runtime acceptance, later packet must allocate exclusive Unity and verify actual final staged ref.
6. Publish a **separate PR against the correct candidate lineage** (not `main`) with a compact delta and handoff `Docs/Handoffs/Overnight-Continuation-20261104/ISOLATED-UI-STAGING-HANDOFF.md`, then mark #269 [HANDED-OFF]. If GitHub PR target restrictions make this impossible, publish exact branch plus a GitHub handoff and report limitation; never retarget existing 290-file PRs without reconciliation.

## Unity and operational gates

DPC's overnight receipt reported ~19 GiB free on ZX20. Cold Unity Library import would consume about 10–12 GiB while the machine-wide post-admission floor is 15 GiB; **do not start cold Unity at that headroom**. Do not delete unrelated caches, borrow the user's warm `CCF-U1` Library, or bypass the lock. A distinct manager-approved Unity qualification packet and fresh lock/disk/process preflight are required when enough storage is available.

The current user's Scenario One playtest is not completed/accepted; absolutely no Windows/Player build without completion and explicit post-playtest authorisation. No Rack V2/MFM, accessibility-settings, mark-visual semantics or CTA implementation is approved through this claim.

## Required V2 ledger and handoff

DeepSeek logs actual model selection, provider capacity/context/quota where exposed (else UNKNOWN), retries, branches, checks, cross-provider review and outcome in the [overnight routing ledger](ROUTING-LEDGER.md), as an append-only serial writer; use a separate source-provenance annex in the handoff if needed. **Re-extract** any published number from actual output, including file counts/test totals. At HAND-OFF deliver branch and all relevant PR links and state distinctions: IMPLEMENTED vs REVIEWED vs STAGED vs VERIFIED vs ACCEPTED.

## Paste-ready DPC dispatch

> DeepSeek DPC: execute source-only Git staging claim #269 using https://github.com/jermur788/CCF/blob/main/Docs/Handoffs/Overnight-Continuation-20261104/ISOLATED-UI-STAGING-PACKET.md . The original PRs #261/#262/#264 target divergent main with roughly 290 unrelated files; **do not merge them**. Create the exact isolated candidate-based branch/worktree only after fresh unused-lane/ownership/disk checks. Transplant only the 13 documented approved-scope file deltas; run offline 97-check gate, six focused snapping cases and combined baseline checker, preserving the expected CTA failure. Obtain independent other-provider review, append real V2 routing events and publish ISOLATED-UI-STAGING-HANDOFF.md with a small target-candidate PR. No Unity, save/persistence changes, unrelated cleanup, release integration or Windows build.

**User action required to execute locally:** forward this full handoff to the DeepSeek DPC chat; no user product decision is needed to stage reviewed bytes for qualification.