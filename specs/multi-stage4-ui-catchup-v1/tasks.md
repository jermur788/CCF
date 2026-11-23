# Multi Stage 1–4 `/multi` UI Catch-up Bounded Tasks

- **Spec ID:** SPEC-multi-stage4-ui-catchup-V1
- **Spec status:** APPROVED
- **Task list owner:** Implementation sub-agent (sole writer)
- **Last updated:** 2026-11-23 (statuses at candidate 541bb898; T-06 done with the J-drift exception in the handoff §9)

Tasks are implementation units, not new product decisions. Each stays inside the approved spec and plan. A material product ambiguity stops the task and returns to the owner.

## T-01 — Spec quartet

- **Task ID:** T-01
- **Status:** DONE
- **Goal:** Commit the APPROVED quartet before any tracked source edit.
- **Scope:** `specs/multi-stage4-ui-catchup-v1/**`.
- **Exclusions:** any `src/**` edit.
- **Owner / routing:** implementation sub-agent (opencode / claude-opus-5-5); routing ids d-900-ui-catchup-impl-1, att-20261123-02.
- **Dependencies:** —
- **Verification:** commit ordering visible in `git log`.
- **Stop conditions:** owner authority unclear.
- **Completion state:** quartet committed as its own commit.

## T-02 — Execute-time Prune revalidation test (deliverable A)

- **Task ID:** T-02
- **Status:** DONE
- **Goal:** Prove whole-plan atomic refusal when a Prune guard turns false between Approve and Execute.
- **Scope:** new Core test; production fix only if a defect is exposed.
- **Exclusions:** weakening any assertion.
- **Owner / routing:** as T-01.
- **Dependencies:** T-01.
- **Verification:** the test fails if any tree radius, lift record, work event, year, revision or approval changes; refusal code stable.
- **Stop conditions:** production defect needing a change outside the grant.
- **Completion state:** test passes in the full suite with all anchors unchanged.

## T-03 — Core read-out, pruning schedule, adapter (deliverable B, Core part)

- **Task ID:** T-03
- **Status:** DONE
- **Goal:** Engine-authoritative Stage 4 / canopy / ground-cell read-out from one root; scheduled Prune target; `OpenSynthetic` default 1/1/1 with explicit 0/0/0.
- **Scope:** `MultiAreaUiReadout.cs` (new), `MultiAreaContinuation.cs`, `MultiAreaWorkPlanAdapter.cs`, `CanopyLightEvaluator.cs` (internal helpers), `Stage4Records.cs`; tests.
- **Exclusions:** changing `Evaluate` output, any frozen path.
- **Owner / routing:** as T-01.
- **Dependencies:** T-01.
- **Verification:** contributors reproduce each cell light index bit for bit; cover fraction against an independent sampler; 1/1/1 default; 0/0/0 path unchanged; MA-W12 extended; anchors unchanged.
- **Stop conditions:** anchor drift.
- **Completion state:** suite green, anchors unchanged.

## T-04 — `/multi` API exposure and Prune routes (deliverable B, Ui part)

- **Task ID:** T-04
- **Status:** DONE
- **Goal:** Additive `stage4` payload member; `api/multi/prune/add|remove` following the Fell pattern.
- **Scope:** `src/Ui/PlayerMultiApi.cs` (C# part).
- **Exclusions:** `PlayerApi.cs`, `Program.cs`, renaming/removing payload members.
- **Owner / routing:** as T-01.
- **Dependencies:** T-03.
- **Verification:** Ui build 0/0; smoke W section unchanged; manual route checks (guards, 409 fresh view).
- **Stop conditions:** a guard or policy change needed in `PlayerApi.cs`.
- **Completion state:** routes reachable behind unchanged guards.

## T-05 — `/multi` page (deliverable C)

- **Task ID:** T-05
- **Status:** DONE
- **Goal:** Rack readouts + model badge, prune inspector + plan lists + confirmation, Light Guide + ramp legend + ground inspector, fallen-log markers + legend, preserving the #885 baseline.
- **Scope:** `MultiAreaPage` constants.
- **Exclusions:** any forestry/canopy/light/RD/eligibility arithmetic in JS; new images; inline script/style.
- **Owner / routing:** as T-01.
- **Dependencies:** T-04.
- **Verification:** `node --check`; real Edge over CDP at 1920×1080 and 1280×720.
- **Stop conditions:** layout requiring CSP/page-policy change.
- **Completion state:** spec AC-5..AC-8 observed.

## T-06 — Harness triage and Tier 2 gates

- **Task ID:** T-06
- **Status:** DONE
- **Goal:** J01/J99 root cause; stand-in `cloneNode` alignment only; full gate run.
- **Scope:** `tests/ui-smoke-multiarea.sh` stand-in.
- **Exclusions:** weakening any check.
- **Owner / routing:** as T-01.
- **Dependencies:** T-05.
- **Verification:** gate log with anchors, P0 sha256 before/after, listeners alive.
- **Stop conditions:** any drift.
- **Completion state:** results recorded honestly.

## T-07 — Screenshots and handoff

- **Task ID:** T-07
- **Status:** DONE
- **Goal:** Eight PNGs and the implementation result handoff.
- **Scope:** `Docs/Handoffs/MultiRack-Simulation-20261116/900-ui-catchup/**`.
- **Exclusions:** capability tokens in images or docs.
- **Owner / routing:** as T-01.
- **Dependencies:** T-06.
- **Verification:** sizes, token-free, table in handoff.
- **Stop conditions:** —
- **Completion state:** pushed branch, handoff committed.

## Task summary

| Task | Status | Owner / routing | Depends on |
|---|---|---|---|
| T-01 | DONE | implementation sub-agent | — |
| T-02 | DONE | implementation sub-agent | T-01 |
| T-03 | DONE | implementation sub-agent | T-01 |
| T-04 | DONE | implementation sub-agent | T-03 |
| T-05 | DONE | implementation sub-agent | T-04 |
| T-06 | DONE | implementation sub-agent | T-05 |
| T-07 | DONE | implementation sub-agent | T-06 |
