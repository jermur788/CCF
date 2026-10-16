# Scenario One Tier-3 preflight (static, Non-Unity)

Verification-only location. This directory contains the static preflight, the
exact Tier-3 execution runbook and the preflight evidence manifest for the
single upcoming Tier-3 Unity qualification.

**Unity was NOT RUN. No production file was changed.**

## Identity

| Item | Value |
|---|---|
| Task | CCF Tier-3 static preflight |
| Route | OpenCode / DeepSeek (Non-Unity only) |
| Base (candidate under test) | `971c5f8cc75e571a159a5f38415d0005fdf30ae4` |
| Branch | `ops/tier3-final-preflight` |
| Worktree | `/media/jer/ZX20/CCF-Tier3Preflight` |
| Candidate branch at base | `integration/scenario-one-tier3-candidate` |
| Production invariant | `git diff c20c6dc..HEAD -- Assets Packages ProjectSettings` = **EMPTY** (verified) |
| Static suites | 16 run: 14 PASS, 1 expected-retired FAIL, 1 stale-tool FAIL (both documented) |

## Status: **READY — with prerequisites; density R2 misses adjudicated as an accepted known C-level limitation**

The candidate tooling is internally consistent enough for a one-shot Tier-3
run **provided the prerequisites in `Tier3Runbook.md` are met** and the
`READY`/`NEEDS FIX` items in `StaticPreflightFindings.md` are accepted or
resolved *before* launch.

- **READY (no code change required):** the Tier-3 Unity gates, the Linux
  Player build/smoke, all runner self-tests, all offline suites, the type-check
  and the source/fixture/growth/site checks behave exactly as the Phase-2
  handoff documented at this SHA.
- **NEEDS FIX / PREREQUISITE (must be true before launch):** run in a *full*
  worktree; hold a machine-wide Unity lock; seed the licence for every
  XDG-isolated runner; do not use the EXCLUDED tools `check_sources.py`,
  `check_production_v1.py` and `with_demo_hdr_quarantined.py` (see §6 of
  `StaticPreflightFindings.md`), nor `run_unity_gates.py` cold or
  `run_v3_remaining.sh`.
- **ADJUDICATED — ACCEPTED KNOWN C-LEVEL LIMITATION:** the reduced-model composite
  density offline suite still fails **3/107** deterministic R2 screening checks:
  L40 `B_BEECH_DENSE` 0.423, L40 `F_BEECH_YOUNG` 0.407, E80 `F_BEECH_YOUNG`
  0.417, against the unchanged 0.40 criterion. These remain
  failures/non-passes in evidence and no threshold or production value was
  altered to remove them.
- **PENDING (review/re-pin):** the model-2 new-game branch now exists at
  `40b2419` (`feat: start new Scenario One games on growth model 2`) and its
  focused Unity gate passed, but the implementation remains pending independent
  review / re-pin; see "Current state" below.

## Operating conditions (required)

All four are **required** before the Tier-3 Unity qualification; later
outstanding work must wait on the final re-pinned candidate.

1. **Full checkout required.** Run in a **full (non-sparse) worktree** at the
   candidate SHA. The sparse Non-Unity worktree lacks `Assets/` and produces a
   false `run_typecheck.py` NEW_ERRORS cascade and unusable offline/build
   runners. (Findings §B1.)
2. **External machine-wide Unity lock required.** Hold an **external**
   machine-wide lock (e.g. `flock` on a ZX20 lock file) plus a no-Unity
   assertion for the whole sequence; `build_linux.py` has no editor guard and
   the other runners guard only per-project/per-install. (Runbook §2.)
3. **Explicit licence seeding required.** Every XDG-isolated runner must copy
   the licence **explicitly** (`--license-dir`, or a pre-seeded config);
   `run_unity_gates.py`, `run_model2.py` and the regression runners never seed
   one. (Runbook §3.)
4. **U4 is authoritative for actual 1280×720 verification.** `U4GuidanceSmoke`
   hard-asserts the real Game view is 1280×720; the broad `UxQualification`
   harness does **not**, so the broad UX run alone is not proof of actual
   1280×720. (Runbook §7.)

## Current state (Manager-reported, 2026-10-16 follow-up)

- Corrected F10 Unity delivery **passed on model-2 branch `40b2419`**.
- **Native Windows F10 remains outstanding.**
- Model-2 implementation **passed focused Unity but remains pending independent
  review / re-pin** (branch `feature/scenario-one-model2-default` at `40b2419`;
  gate `ModelDefaultVerification.Begin`, marker `MODEL_DEFAULT_VERIFY_PASS`).
- The **cue-order failure remains an unresolved review item**.
- The **component-BA stream is not yet accepted/integrated**.
- **Linux Player build/smoke waits for the final re-pinned candidate.**

These are the Manager's current-state statements as supplied in the follow-up;
the preflight itself was not re-run.

## Files

| File | Contents |
|---|---|
| `README.md` | This overview and handoff. |
| `Tier3Runbook.md` | Exact ordered Tier-3 commands, expected evidence, shutdown/crash classification, cleanup. |
| `StaticPreflightFindings.md` | Static audit findings (defects, stale tools, risks), non-Unity test results and the accepted-limitations vs blockers classification. |
| `PreflightManifest.json` | Tested HEAD, participant tool SHA-256, exact static commands, results and evidence inputs. |

## Final preservation record

Bounded documentation corrections applied before commit (no preflight rerun, no
production change):

1. **Manager adjudication — ACCEPTED KNOWN C-LEVEL LIMITATION.** The three
   reduced-model R2 misses remain recorded exactly as observed and remain
   failures/non-passes in evidence: L40 `B_BEECH_DENSE` **0.423**, L40
   `F_BEECH_YOUNG` **0.407**, E80 `F_BEECH_YOUNG` **0.417**, against the
   unchanged **0.40** criterion. No threshold or production value was altered.
2. **EXCLUDED from Tier-3 (explicit):** `check_sources.py`,
   `check_production_v1.py`, `with_demo_hdr_quarantined.py` (and
   `run_v3_remaining.sh` is not a Tier-3 gate). See `StaticPreflightFindings.md` §6
   and `PreflightManifest.json` `excluded_from_tier3`.
3. **Full preservation identity:** tested HEAD
   `971c5f8cc75e571a159a5f38415d0005fdf30ae4`; required **full** (non-sparse)
   worktree at that SHA (B1); the complete Tier-3 command sequence is in
   `Tier3Runbook.md`; the complete participant-tool SHA-256 inventory is in
   `PreflightManifest.json`. Production invariant EMPTY; Unity NOT RUN.

## Handoff

```text
IMPLEMENTATION HANDOFF — STATIC PREFLIGHT
Task:               CCF Tier-3 static preflight (Non-Unity)
Worker:             OpenCode / DeepSeek
Role:               Primary (preflight only)
Branch:             ops/tier3-final-preflight
Base:               971c5f8cc75e571a159a5f38415d0005fdf30ae4
Tested HEAD:        971c5f8cc75e571a159a5f38415d0005fdf30ae4 (candidate; unchanged)
Production invariant: EMPTY (Assets/Packages/ProjectSettings unchanged from c20c6dc)
Unity:              NOT RUN
Static test results: unit 22/22; F10 13/13+2; history 74; N3 88; UX 176;
                     density 91; composite density 107 with 3 known FAIL;
                     timber 71; typecheck prod 7/7 CLEAN, tools 13 vs baseline 43 CLEAN;
                     build_linux 22/22; fixture controls 9+7; growth 360/7; site 63;
                     P2 PASS; P3 PASS; composite mutations all KILLED (control shows same 3 FAIL)
Status:             READY (prerequisites) / 3 reduced-model R2 misses adjudicated: ACCEPTED KNOWN C-LEVEL LIMITATION / model-2 at 40b2419 passed focused Unity, pending review+re-pin
```
