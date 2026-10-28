# RACK-V2 OPEN-3 — CANONICAL AUTHORITY SYNC HANDOFF

```text
DOCUMENTATION HANDOFF
Task: RACK-V2 OPEN-3 — Canonical Authority Sync (claim #50)
Worker: OpenCode / opencode-go/deepseek-v4.1-flash
Role: Primary documentation writer
Branch: docs/rack-v2-open3-authority-sync-v1
Worktree: /media/jer/ZX20/CCF-Open3Sync
Starting HEAD: 3b103626d5499aaa968c10ee3c6a78c0139c29d6 (origin/main)
Final tip: the commit that adds this file (docs only)
Exit: OPEN-3 AUTHORITY SYNCED — READY FOR MANAGER REVIEW
```

## Scope

Canonical documentation only. No production code, no Unity run, no save/schema change, no generated context, mirror or dashboard edit.

The accepted OPEN-3 boundary recorded here is exactly the direct-user decision of 2026-10-28 (`accept open 3 rec`), already implemented and qualified through RACK-V2-05 and RACK-V2-07:

- a rack with **zero executed removals** may be reopened/edited through the authoritative rack reconciliation/cancellation path;
- reopening itself cancels nothing;
- where editing would cancel approved-but-unexecuted work, the player receives a warning and explicit consent is required before cancellation/reconciliation;
- any rack with **one or more executed removals** is geometry-locked and represented as harvested;
- executed removals are never reversed;
- only racks whose geometry changes lose geometry-bound entrance-mark state; unrelated racks retain theirs.

No additional reopen policy was invented.

## Changed files

1. `Docs/Project/decision-log.md`
2. `specs/scenario-one-objective-1-rack-planning-field-marking-v2/spec.md`
3. `specs/scenario-one-objective-1-rack-planning-field-marking-v2/tasks.md`
4. `specs/scenario-one-objective-1-rack-planning-field-marking-v2/plan.md` (minimal same-spec consistency edit)
5. `specs/scenario-one-objective-1-rack-planning-field-marking-v2/checklists/requirements.md` (minimal same-spec consistency edit)

The last two files were changed only because they still stated OPEN-3 as undefined/unresolved, which the packet's directory-wide search rule covers.

## Before / after OPEN-3 references

### 1. `Docs/Project/decision-log.md` — D-064

**Before:** `OPEN-3 (what may be reopened once a confirmed rack's orders are approved or executed) remains open and is not to be implemented before it is decided.`

**After:** `OPEN-3 accepted 2026-10-28 by direct user authority:` followed by the accepted boundary above. D-064's other product rules, evidence labels, implementation caveat and save/Reference Future statements were left unchanged.

### 2. `specs/scenario-one-objective-1-rack-planning-field-marking-v2/spec.md`

- **Provenance (line 15).** Before: `OPEN-3 remains open.` After: `OPEN-3 was accepted by direct user authority on 2026-10-28 (see §39.7).`
- **§39 introduction (line 946).** Before: `OPEN-3 is raised for a decision and is not assumed.` After: `OPEN-3 was accepted by direct user authority on 2026-10-28 (D-064).`
- **§39.6 stop condition 7.** Before: `OPEN-3 (§39.7) is reached before it is resolved.` After: removed; the remaining stop conditions are unchanged.
- **§39.7 introduction (line 1008).** Before: `OPEN-3 remains open.` After: `OPEN-3 was accepted by direct user authority on 2026-10-28 (D-064).`
- **§39.7 OPEN-3 block.** Before: `OPEN-3 — Reopening a confirmed plan after its work has progressed: OPEN` with an untested recommendation, `Next step: ... Do not implement OPEN-3 yet`, and `Needed before: RACK-V2-05 begins`. After: `ACCEPTED — ZERO EXECUTED REMOVALS MAY REOPEN; EXECUTED RACKS LOCKED` with the accepted boundary and links to the RACK-V2-05/RACK-V2-07 Manager evidence.
- **Header.** `Last updated` advanced from 2026-10-26 to 2026-10-28.

Sections 1–38 were not rewritten.

### 3. `specs/scenario-one-objective-1-rack-planning-field-marking-v2/tasks.md`

- **Top decisions note.** Before: `OPEN-3 ... remains open and gates RACK-V2-05. RACK-V2-01 reports the safest boundary for OPEN-3 but does not implement it. A task that reaches OPEN-3 unresolved stops and reports.` After: OPEN-3 accepted 2026-10-28, with the exact boundary; no unresolved-stop instruction remains.
- **RACK-V2-07 stop conditions.** Before: `Reopen requires touching executed work (OPEN-3 unresolved).` After: `Reopen would require reversing an executed removal or creating a second removal mechanism, rather than locking that rack as harvested (OPEN-3 accepted 2026-10-28).`
- **Task summary dependency.** Before: `RACK-V2-04; OPEN-3`. After: `RACK-V2-04 (OPEN-3 resolved 2026-10-28)`.
- **Header.** `Last updated` advanced from 2026-10-26 to 2026-10-28.

Task scope, dependencies other than the resolved OPEN-3 marker, and every exit token were preserved. Historical RACK-V2-01/RACK-V2-05 references that describe work already performed (for example `report the safest boundary for OPEN-3` and `OPEN-3 resolved`) were left as historical text; they do not state that OPEN-3 is still open.

### 4. `specs/scenario-one-objective-1-rack-planning-field-marking-v2/plan.md` — RK-6

**Before:** `reopening a plan after orders are approved or executed is still undefined (OPEN-3).`

**After:** `reopening a plan after orders are approved or executed is bounded by accepted OPEN-3 (2026-10-28): zero-executed racks may reopen through the authoritative reconciliation/cancellation path; any rack with an executed removal is geometry-locked and never reversed.`

### 5. `specs/scenario-one-objective-1-rack-planning-field-marking-v2/checklists/requirements.md` — decision status

**Before:** `- [ ] OPEN-3 ... resolved and recorded before RACK-V2-05 begins — decision and date ...:`

**After:** `- [x] OPEN-3 ...: ACCEPTED — ZERO EXECUTED REMOVALS MAY REOPEN; EXECUTED RACKS LOCKED — evidence: direct user authority 2026-10-28, Decision Log D-064; implemented/qualified through RACK-V2-05 and RACK-V2-07.`

## Verification

- `git diff --name-only` contains exactly the five documentation/spec files above. No production code, Unity asset, scene, prefab, material, package, ProjectSettings, save/schema, `.meta`, generated context, mirror or dashboard file changed.
- Directory-wide search of the approved Rack V2 spec directory plus `Docs/Project/decision-log.md` finds no remaining current-authority statement that OPEN-3 is open, unresolved, undefined or gating a task. Remaining literal references are historical task text, accepted-boundary text, or unrelated OPEN-3 identifiers in other specs.
- `Docs/Project/coordination/active-tasks.md` was not edited: it is live coordination outside the canonical-authority scope, and its old `OPEN-3 remains open` phrase appears only in a historical RACK-V2-COMBINED-REPAIR-01 restriction row. No canonical authority claims that status.
- The recorded boundary is textually identical to the direct-user decision and to the accepted RACK-V2-05/RACK-V2-07 Manager reviews; no new behaviour or policy was added.
- No Unity run and no production-code verification were required or performed.

## Known limitations / not claimed

- No Unity, build, runtime, save/load or production-code verification; this is a documentation-only change.
- No generated context, Drive mirror, dashboard, platform-attachment or published-dashboard update was performed or claimed.
- The D-064 row's pre-existing `Not implemented or verified; no Windows build` caveat was left unchanged because it is outside OPEN-3 wording; whole-feature completion remains a separate status matter.
- Historical coordination rows and other specs were not rewritten.

**Ready for Manager review: Yes.**

`OPEN-3 AUTHORITY SYNCED — READY FOR MANAGER REVIEW`
