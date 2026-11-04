# Objective guidance spec handoff — #258

**Status: DRAFT / NOT APPROVED / NOT IMPLEMENTED.** 2026-11-04. Related [#258](https://github.com/jermur788/CCF/issues/258) and [#254](https://github.com/jermur788/CCF/issues/254).

## Result and ownership

Created only `specs/scenario-one-next-action-guidance-v1/{spec.md,plan.md,tasks.md,checklists/requirements.md}` and this handoff. Starting branch `docs/overnight-objective-guidance-spec-20261104`, clean HEAD `8f0f230190830855112e3e7a1d7544db94a7f591`; assigned lane `/media/jer/Files/CCF/Operations/CCF-Overnight-ObjectiveSpec-20261104`. No other lane touched. Context-source latest commit observed `9067554795b662966e243e09bb724aca9bd15649`; canonical context read in assigned snapshot. No coordination/ledger edits.

## Observed facts versus proposals

Spec F1–F9 contains file:line citations separated by main/candidate revision. Both sources highlight Back to forest; main V2 wording omits entrance precedence while candidate RackV2Instruction supplies it. Current lesson selection uses TeachingFacts/ScenarioOneTeachingSequence, not GuidedTopicIndex. Map symbols include patterned rack lines, entrance square and round handle; actual tree-mark renderer uses rings. Map cells are non-focusable and global Tab opens Work Plan; full keyboard rack creation was not established.

Proposal: one transient cue using existing state/validators and wording, Stand Map then Plan Racks emphasis, confirmed-plan return for physical marking, then Work Plan review. Static map-like approved parallel example retains canopy and explicitly labels proposed/unexecuted status. No save schema change. No new backend, guidance engine or auto-navigation.

## Decisions and next owner

Manager/user must resolve Q1 modal keyboard policy (coordinate #257), Q2 static versus live figure and verified reference/standing-row count, Q3 later-lesson action mapping/first slice, Q4 base and #255/#256 reconciliation. Recommend static labelled approved parallel example and Objective 1 first slice. Independent design review and explicit Manager approval remain required; no self-approval or merge. DPC should route review and publish outcome through its own ledger; #200 retains coordination ownership.

## Verification and limits

Read both refs with git show without checkout; inspected existing spec conventions and relevant UI/Map sources. Documentation-only checks: owned-path diff, whitespace, required files/status/citations and no production change. No Unity, compiler, build, screenshots, real input, player review or saved-progress runtime checks performed. Test matrix specifies checks, resolutions and roles; all future acceptance boxes remain unchecked. Geometry/scale and keyboard policy need qualification before production. Dashboard publication/synchronization was not verified and remains an operational handoff item for DPC/Manager.

## V2 Orchestration Decision Appendix

- **Journal event:** EVENT 012 (binding dispatch supplied by parent); pool **codex**, runtime **gpt-6.1-sol**, effort **medium**.
- **Selection:** assigned design sub-agent; medium effort sufficient for bounded source-backed docs. No delegates, provider switch, escalation, quota wait or retries claimed.
- **Alternatives:** other provider availability/capacity UNKNOWN; worker did not select or launch another provider. Independent cross-provider review remains pending.
- **Quota/reset/context/tokens/elapsed:** UNKNOWN (no provider telemetry receipt). No fabricated numeric capacity claims.
- **Provenance:** main `8f0f230190830855112e3e7a1d7544db94a7f591`, candidate `72b4efbd1c653fa2231f632a79030a83ff76f833`; source ledger F1–F9 records exact paths/lines. Five-rack/16–20 m/seven-row statements retain their distinct site/legacy semantics; no new measured geometry or research coefficients reported.
- **Outcome:** four-file DRAFT and handoff ready for external review, not production implementation or acceptance. Parent DPC alone appends ledger event outcome.

## Review response

- **P1 FIXED:** FR-2 now resolves `editor.ConsentPending`, `editor.ResetPending`, `reopenPending`, then other blocking modals before every background row; only the active scope has primary emphasis/activation. Safe decline/cancel wins, with no-primary fallback when no safe modal choice exists. AC-1 tests each prompt against competing planning/confirmed states and re-derivation after dismissal.
- **P2 FIXED:** FR-3 makes candidate `RackV2Instruction()` authoritative for Objective 1 wording/stage after modal handling, projects its first unfinished step onto the screen, then applies validators. Corrections use a fixed first-eligible allowlist (map, planning, reason-matched undo, over-target selected unlocked delete); otherwise reveal the same instruction/reason. No arbitrary enabled-control selection or second instruction source. AC-6 tests helper branches, ties and ambiguous reasons.
- **P3 FIXED:** F6 was re-derived with `git show` and numbered lines at main `8f0f230190830855112e3e7a1d7544db94a7f591`; every symbol citation now names `StandMapSymbols.Build()` and its `MapSymbol` entry. Racks :86–97, navigation/grid :99–104, tree marks :106–107; not candidate 72b4efbd.

DRAFT remains unchanged. Planned canopy/no-clearing semantics, shape/pattern/text accessibility and saved-state derivation without schema changes remain required. Docs-only; no Unity or production verification claimed.

Revision starts from clean `f98d9ee8948d54494c5cc882fdb332e2ede832f9` on the same assigned branch/worktree; PR #265 remains the review destination. Independent re-review remains pending. Dashboard synchronization remains unverified and belongs to DPC/Manager.

Revision verification: `git diff --check` passed; programmatic exact-commit checks matched all 17 named symbol entries and the Build definition; all four spec statuses remain DRAFT; only the five owned paths changed. FR-5/FR-6 and the save-compatibility section were compared with the prior HEAD and remain byte-identical. No runtime gates run.
