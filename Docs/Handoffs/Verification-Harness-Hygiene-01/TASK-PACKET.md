# Verification Harness Hygiene 01 — Task Packet

**Class:** bounded verification-tooling hygiene only  
**Parent backlog:** #28  
**Purpose:** repair two known verification-tool blind spots without changing game/product behaviour, production arithmetic, accepted evidence thresholds or Unity qualification rules.

## Status / routing

- **Released:** READY — OFFLINE / NO UNITY
- **Difficulty:** **2/5** — bounded tooling work with meaningful negative controls, but no gameplay/Unity implementation.
- **Best suited:** OpenAI Codex / Sol-class local code-verification worker. Ultra Code is unnecessary; keep the strongest Unity-capable slot on MFM-08 #156.
- **Recommended branch:** `verify/harness-hygiene-v1`
- **Recommended worktree:** `/media/jer/Videos/CCF-HARNESS-HYGIENE`
- **Exact start:** the main commit that publishes this packet (worker must refresh `main`, #28, its execution claim, and `Docs/Project/coordination/active-tasks.md` before editing).
- **Unity:** forbidden for this task; do not acquire the machine-wide Unity lock.
- **Windows:** forbidden.

Before edits, post `[RUNNING — OFFLINE]` on the substantive execution claim with provider/model, branch/worktree and exact starting HEAD.

## Problem A — CloudTypeCheck method-body blindness

Current repository tooling under `Tools/Verification/CloudTypeCheck/` uses an older approximation surface. PARITY-04 showed that four declaration-level `Painter2D` errors can stop `csc` before method-body binding, so a deliberate body error in `BroadleafAdultGrowthVerification.cs` was not reported. A scratch external project with a narrow `Painter2D` compatibility stub restored body binding and then caught injected `CS0029` / `CS1061` errors.

Repair the repository tool so its normal tooling/type-check mode cannot silently claim useful body-level checking while declaration-level compatibility gaps prevent method-body binding.

Required outcome:

1. The normal repository CloudTypeCheck path reaches method-body binding for the relevant verification/tool sources despite the known Unity-6 `Painter2D` surface gap.
2. A deliberate body-level type error in a scratch/mutated copy of a verification harness is detected by the repository tool.
3. The clean source does not gain new unexplained errors from the hygiene change.
4. The tool remains explicitly an approximation and must not be relabelled as a substitute for Unity import/compile.
5. Do not hide errors by blanket suppression, excluding the affected bodies/files, or deleting/rebaselining diagnostics merely to get green.
6. If baseline files genuinely require refresh because the tool can now see previously hidden body-level diagnostics, classify every changed baseline entry and preserve durable before/after evidence. Unexpected production/tool defects must be reported, not normalized away.

## Problem B — SiteInfrastructure verification seam allow-list

`Tools/Verification/SiteInfrastructure/OfflineLogic/SiteInfrastructureOfflineCheck.cs` currently asserts that **no other verification tool** references the site-seam types. That was correct for the dormant-infrastructure packet, but it makes an explicitly reviewed verification consumer impossible without weakening/removing the production non-use guard.

Replace the all-or-nothing verification-tool guard with an explicit narrow allow-list mechanism.

Required outcome:

1. **Production remains zero-reference.** `H_NO_PRODUCTION_FILE_REFERENCES_THE_NEW_TYPES_OR_RESOLVER` (or equivalent stronger check) must remain strict and unchanged in meaning.
2. Verification references are allowed only through an explicit, reviewable path allow-list. Do not permit directory-wide or pattern-wide arbitrary consumers.
3. The currently authorised adult verification harness path may be represented as an allowed consumer if needed: `Tools/Verification/ThreeSpecies/BroadleafAdultGrowthVerification.cs`. Do not edit that harness under this task unless a separate Manager instruction explicitly widens scope.
4. Add a positive/negative control proving an allow-listed verification consumer is accepted while an unlisted synthetic verification consumer is rejected.
5. Preserve all existing site-data numerical/semantic assertions and the isolated compile.

## Writable surface

Primary allowed files:

- `Tools/Verification/CloudTypeCheck/run_typecheck.py`
- `Tools/Verification/CloudTypeCheck/UnityEditorStub.cs`
- `Tools/Verification/CloudTypeCheck/InputSystemStub.cs`
- `Tools/Verification/CloudTypeCheck/baseline-prod.txt` and/or `baseline-tools.txt` **only with evidence-backed classification**
- `Tools/Verification/SiteInfrastructure/run_site_check.py`
- `Tools/Verification/SiteInfrastructure/OfflineLogic/SiteInfrastructureOfflineCheck.cs`
- narrow new verification-only helper/test files inside those two verification directories
- `Docs/Handoffs/Verification-Harness-Hygiene-01/HANDOFF.md`

No file under `Assets/ForestPrototype/` may change. No species assets, save/schema, scenes, prefabs, calibration, accepted anchors, gameplay tests or feature qualification criteria may change.

If a production/API change appears necessary, stop and return to Manager.

## Verification required

Run and record:

- CloudTypeCheck current clean run(s) for relevant modes, before and after;
- body-error mutation/negative control proving the tool now binds and diagnoses the injected body error;
- any changed baseline classification, line by line or grouped by proven common cause;
- SiteInfrastructure offline gate, preserving its existing assertions;
- allow-listed-consumer positive control and unlisted-consumer negative control;
- repeat/determinism run where practical;
- changed-file boundary and clean worktree.

Do **not** use Unity merely to validate this tooling task. Unity remains authoritative for Unity compilation, but this task is specifically to improve the offline approximation and source-policy tooling.

## Stop conditions

Stop and report if:

- a production file must change;
- an accepted gameplay/forestry value or verification threshold would need to change;
- errors must be suppressed or tests weakened to obtain a pass;
- another active writer owns either verification directory;
- the proposed change would cause the offline tool to masquerade as authoritative Unity compilation.

## Handoff

Publish:

`Docs/Handoffs/Verification-Harness-Hygiene-01/HANDOFF.md`

Include exact start/head, provider/model, branch/worktree, changed files, before/after diagnostics, mutation evidence, SiteInfrastructure allow-list controls, regression results, limitations and clean return state.

Success token:

`VERIFICATION HARNESS HYGIENE QUALIFIED — READY FOR MANAGER REVIEW`

No Unity. No Windows build.
