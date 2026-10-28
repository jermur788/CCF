# RACK-V2-07 — MANAGER REVIEW

**Task:** `RACK-V2-07 — Confirmation + Minimap Implementation / Qualification`  
**Task claim:** #47 (`[HANDED-OFF]`, closed)  
**Worker handoff:** `Docs/Handoffs/RACK-V2-07/HANDOFF.md`  
**Exact Unity-tested candidate:** `38366c8ba31fa7d42d87e8e7f61bd387310ca9dc`  
**Published branch tip:** `d2ff1dbc9ce47f4d27b17721955a8a64e05a131f`  
**Integration:** PR #48 / merge `c5a0532ac0d6a3729baeca0a1bf6d1d85df46fb9`  
**Manager verdict:** **APPROVED / INTEGRATED — CONFIRMED PLAN PROPAGATES**

## Accepted evidence

- RACK-V2-07 confirm/minimap Unity gate: PASS 213 checks / 21 captures at 1280×720 and 1920×1080.
- Preservation gates: RACK-V2-03 runtime PASS 14; RACK-V2-04 map PASS 142; RACK-V2-05/06 editing+spacing PASS 344.
- Offline gates: editing 61, spacing 11, suggested layout 147, Stand Map 171; focused static check 10.
- Runtime source compile and Editor-harness compile: 0 errors.
- Exact candidate used the machine-wide Unity lock only while free; locks were released normally and U1 ended clean.
- The first spacing preservation attempt aborted on a known black first-capture display flake before an assertion; the complete gate was rerun on the same candidate and passed. Both logs are retained.

## Accepted implementation boundary

RACK-V2-07 adds the valid-five `Confirm Rack Plan` gate, authoritative plan-confirmation delegation, reopen warning/consent UI, per-rack mark invalidation, the planned-to-marked counter transition, and minimap propagation from the same authoritative rack sources. Confirm creates pending work only and does not approve, execute or mark. Reconfirming an unchanged plan is a no-op. Reopening by itself cancels nothing.

The accepted OPEN-3 boundary is preserved: racks with zero executed removals may be reopened/edited through the authoritative reconciliation/cancellation path, with explicit consent where approved work would be cancelled; any rack with an executed removal is geometry-locked/harvested; executed removals are never reversed. Geometry changes clear only the affected rack's entrance-mark state.

## G3 — protected assertion review

**PASS / ACCEPTED.** Two protected RACK-V2-04 offline assertions stated that V2 rack presentation did not expand to the minimap. RACK-V2-07's approved task requirement explicitly requires the minimap to show dashed guidance before confirmation and the solid confirmed plan after confirmation, including harvested/locked presentation. The old assertions therefore encoded a superseded requirement.

The worker did not weaken the checks merely to obtain green results. The two obsolete assertions were replaced by stronger checks that require the new minimap contract: dashed suggested racks, solid confirmed racks, matching Stand Map/minimap symbols and colours, harvested-rack visibility, draft-rack exclusion and confirmation/version transitions. The independent protected Unity assertion that suggestions must not appear in the normal `Racks` collection outside planning mode was kept byte-identical; the new minimap-only guidance uses the separate `MiniGuidance` path. Existing RACK-V2-03/04/05/06 Unity gate sources were not rebaselined.

This is a deliberate requirement supersession with stronger measurement coverage, not test weakening.

## Known limitations / not claimed

- First-time minimap teaching note is session-only until persistence work in RACK-V2-11.
- The lower HUD hint still contains the old first-person rack-author wording; removal belongs to RACK-V2-08.
- Player spraying/field marking is not implemented or qualified here (RACK-V2-09).
- Help expansion, persistence/save qualification, whole Objective-1 qualification and final rendered review remain later tasks.
- No Windows/Player build or user acceptance.

**Verdict:** `APPROVED / INTEGRATED — CONFIRMED PLAN PROPAGATES`

**DASHBOARD SYNC: NOT AVAILABLE**  
**Published dashboard reflects this task: NO**
