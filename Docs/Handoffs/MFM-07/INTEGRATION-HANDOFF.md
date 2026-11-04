# MFM-07 integration handoff — claim #125

MFM-07 INTEGRATED — READY FOR MFM-08 / S1-D T-05 DEPENDENCIES

Integration decision: **APPROVED** for this bounded MFM-07 integration (G4); required post-merge qualification **PASS** on the exact merged candidate. No MFM-08, MFM-09, save-schema/validator change or Windows build occurred.

## Exact identity and authority

- Executor: T3 Code / OpenCode harness, `opencode-go/deepseek-v4.1-flash` (DeepSeek, Opencode Go), sole integration writer.
- Packet: `Docs/Handoffs/MFM-07/INTEGRATION-TASK-PACKET.md` at `origin/main` commit `1d046408`.
- Integration worktree: `/media/jer/Videos/CCF-PlayerTestIntegration`.
- Branch: `integration/scenario-one-player-test-ready-v1`.
- Freshly inspected pre-integration umbrella HEAD: **`7608f02cb8da1fabd8b78fd5aaf45b0c32086f67`** (clean; documentation-only successor of the MFM-06 qualified production merge `b971f4da0540e606bfee9702b0c99f81a0a89355`).
- Exact reviewed MFM-07 production input: **`d4188b33e58b0fe67504a87167692d01b226c84e`** (`feature/mfm-v1-proposal-approval-boundary-v1`). Published docs-only successor `01755c47d537202e9cf479545be527d43121298f` was inspected and **not** used, so no unreviewed production/verification change was absorbed.
- Exact merged and Unity-qualified umbrella candidate: **`c8cfae8c4025cec6e960a050edf4bfde2a981de5`**, save schema v23.
- Method: `git merge --no-ff d4188b33e58b0fe67504a87167692d01b226c84e`. Clean `ort` merge, **no conflicts**. Merge parents are the two full SHAs above.
- This handoff is a subsequent documentation-only commit; the tested candidate remains the merge SHA above.

## Change and protected MFM-07 behaviour

The merged delta is the reviewed MFM-07 input: 9 files, +2125/−30.

Production (all **new**; no existing production file was modified):

- `Assets/ForestPrototype/ScenarioOne/ScenarioOnePrescriptionApproval.cs` (+`.meta`) — pure proposal/validation/`Build` logic, no `UnityEngine`.
- `Assets/ForestPrototype/ScenarioOne/ScenarioOneManager.PrescriptionApproval.cs` (+`.meta`) — the only writer: `CreatePrescription`, `PreviewPrescription`, `CheckPrescriptionLink`, `ApprovePrescription`.

Verification tooling: `Tools/Verification/MfmApprovalBoundary/` (offline check + runner, Unity gate, and a copy of the MFM-06 runner with the added `approval` gate).

Coordination: `Docs/Project/coordination/active-tasks.md` (the reviewed input's #121-era edit). `ScenarioOneManager.cs`, the save data/validator, Work Plan and marking manager remain byte-identical to the pre-integration umbrella.

The merged production tree is byte-identical to reviewed `d4188b33` across `Assets`, `Tools`, `Packages` and `ProjectSettings`. The only whole-tree difference from `d4188b33` is the pre-existing `Docs/Handoffs/MFM-06/INTEGRATION-HANDOFF.md` already present on the umbrella (docs-only).

Protected behaviour preserved and exercised by the gates:

- proposals are transient plain data — never `TreeMarkType.Fell`, work orders or persisted proposal state;
- manual Fell marks, open orders and confirmed Crop Tree state win conflicts and are surfaced, not overwritten;
- prescription approval creates **Pending** ordinary felling work only; Work Plan approval remains the separate commitment gate;
- `Build` validates prescription + coverage unit + Crop Tree linkage, then the world is re-read and every link revalidated immediately before the first order write;
- stale/invalid linkage fails closed with no partial mutation; unexpected write failures roll back orders, events, allocators and approval state/year;
- v23 provenance fields are reused; **no schema/version bump and no save-validator edit** (unchanged v23).

## Post-merge verification

Baseline ran in clean detached U1 at **7608f02c**; candidate ran in the same clean detached U1 at **c8cfae8c**, Unity **6000.6.0f1**, under normal locks (each released normally). Every runner result is `pass:true`, normal Editor exit 0, no gate failure, `remaining_lane_changes` empty, host saves unchanged, staged harness/metas and copied licence removed.

| Gate | Result |
|---|---|
| Pre-merge baseline compile | `MFM06_COMPILE_PASS`, saveVersion=23, 103.91 s |
| Pre-merge baseline hash dump | `MFM06_HASHDUMP_PASS` lines=18, 197.41 s |
| **Import/compile (candidate)** | `MFM06_COMPILE_PASS`, saveVersion=23, 62.77 s |
| **MFM-07 approval runtime gate** | `MFM07_UNITY_PASS checks=106`, 120.11 s — includes Work Plan leakage/bypass negatives and real save→load provenance round trip |
| **MFM-06 save/load regression** | `MFM06_UNITY_PASS checks=159`, 133.48 s |
| **Protected hash dump** | `MFM06_HASHDUMP_PASS` lines=18, 69.28 s — **18/18 lines byte-identical** to the fresh pre-merge `7608f02c` baseline (and to the MFM-06 integration candidate dump); **no rebaseline** |
| **Work Plan / interaction regression** | `SCENARIO_ONE_INTERACTION_VERIFY_PASS`, 162.87 s |
| Additional offline boundary check (candidate) | `MFM07_APPROVAL_BOUNDARY_PASS checks=125 failed=0`; two processes byte-identical; F6 rollback and F7 no-schema-bump checks PASS |

The MFM-07 runtime gate drives marks/crop designations through `ForestTreeMarkingManager` and the real Work Plan manager APIs, not rendered UI or native input; rendered MFM-08 review remains future work.

## Reproduction and evidence

Durable root: `/media/jer/Files/CCF/Verification/MFM-07-Integration-20261029/`.

- `integration-provenance.json`, `hash-comparison.json`, `gate-summary.json`.
- `baseline/01-compile`, `baseline/02-hash-dump`, `candidate/00-offline`, `candidate/01-compile`, `candidate/02-approval`, `candidate/03-save`, `candidate/04-hash-dump`, `candidate/05-interaction` — each with `command.json`, `process-result.json`, `unity.log`, evidence folder and cleanup receipt.
- `hash-dump.txt` for both baseline and candidate.

To rerun: reserve clean detached U1 at the exact candidate SHA, acquire a fresh normal lock, then from the merged checkout:

```bash
python3 -B Tools/Verification/MfmApprovalBoundary/run_approval_boundary_offline.py --out /media/jer/Files/CCF/Verification/NEW/00-offline
python3 -B Tools/Verification/MfmApprovalBoundary/run_unity_gate.py --project /media/jer/ZX20/CCF-U1 \
  --out /media/jer/Files/CCF/Verification/NEW/01-compile --expected-head <SHA> \
  --lock-id <LOCK> --owner <OWNER> --task-id <TASK> --gate compile
```

Run `approval`, `save`, `hash-dump`, `interaction` at sibling output directories; `compile` must be first at `01-compile`. Repeat compile + hash dump on exact `7608f02c` under its own lock and compare the two 18-line dumps byte-for-byte.

## Cleanliness, coordination and publication

- At MFM-07 completion U1 was clean and detached at the exact qualified merged candidate `c8cfae8c`; both locks were released normally with no Editor process. The lane was then free and was subsequently acquired by #123 S1-C T-04 (`Claude-S1C-T04`) at 2026-10-29T20:33Z for its own gated work; MFM-07 retains no lane ownership and did not touch that work.
- Integration worktree clean before this handoff commit; no unrelated work touched. Primary's existing modified `AGENTS.md` / `active-tasks.md` and untracked documents were untouched.
- Live coordination `origin/main` was marked `#125 [RUNNING]` (DeepSeek / `deepseek-v4.1-flash`, integration worktree, starting HEAD `7608f02c`, input `d4188b33`) at main commit **`b7e67f7c539fe70acaed1b4641bc24860bf03a2a`** before integration. The umbrella merge also carries the reviewed input's #121-era `active-tasks.md`; the Manager should reconcile the live ledger to this integrated result.
- Merge and this handoff are published to `origin` on `integration/scenario-one-player-test-ready-v1`.

## Residuals and next dependency

Accepted residuals, unchanged and visible (do not block MFM-07 integration):

1. `TryReadPrescriptionWorld` still omits infrastructure masks and management features; the coverage-core contract accepts them but the production adapter passes empty arrays. Close this (#129) before shipped MFM-08/MFM-09 coverage UX relies on it.
2. Approval has no dedicated management-event enum; approval state/year plus per-order `prescriptionId` provenance is accepted for MFM-07.
3. A single player Fell mark can drop the whole coverage unit from player management; safe and conflict-surfaced, but player-facing treatment is an MFM-08 product question.
4. The runtime gate is API-driven, not rendered/native-input; MFM-08 owns rendered review.
5. Not run here by design: full Tier-3 sweep, N3, Frozen Reference, Windows.

Next dependency: **#129 MFM-08A coverage-world adapter** may now pin post-MFM-07 integrated umbrella `c8cfae8c`; **S1-D T-05** may consume the integrated pending-work/provenance API. MFM-08 UX follows #129 + integrated MFM-07 + free UI ownership. No MFM-08/MFM-09 implementation was done here.
