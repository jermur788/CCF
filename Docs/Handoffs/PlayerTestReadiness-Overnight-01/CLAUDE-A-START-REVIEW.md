# CLAUDE-A START REVIEW — PLAYER-TEST-OVERNIGHT-01 (#91)

Read-only programme/readiness audit. Nothing was edited apart from this file. I did not move, delete or clean anything, did not take the Unity lock, did not launch Unity, and did not commit or push.

## 1. Reviewer identity (recorded as observed)

| Field | Value |
|---|---|
| Role | Claude A, mandatory first delegate |
| Model | **Claude Opus 5.5** (`claude-opus-5-5`) |
| Provider / harness | Anthropic, via Claude Code (Claude Agent SDK) running inside T3 Code |
| Effort | **I cannot check this from inside the session.** The launching harness sets the effort level, and I cannot see it as a named "medium". The orchestrator must check the effort it actually requested in its `delegate_task`/launch record. If that record does not say Opus **medium**, the packet's first-delegate condition is not met. In that case, re-run this review at medium, or record the substitution with the Manager. Do not record "medium" for this review unless the launch record shows it. |
| Audit date | 2026-10-29 |

## 2. Inspected SHAs

| Ref | SHA |
|---|---|
| Orchestration worktree HEAD (`orchestration/scenario-one-player-test-ready-v1`) | `4df2a45333aee03820fc8c9f83da78e9d0cf7a02`. This is the actual starting main plus one change to `active-tasks.md` marking #91 RUNNING. |
| GitHub `origin/main` (checked live with `ls-remote`) | `f76eb20334bdeaec933fbec654726d29c57d7464` |
| Packet's observed main | `775947a`, which is an ancestor of f76eb20. No authority conflict. |
| Context commit | `d05b27ad376ce02748ee4703429ad3ff66a65542` (exists; "docs: correct milestone reconciliation date") |
| Bare store `refs/heads/main` | `d72beb8`. This ref is **stale**: it is an ancestor of f76eb20, from 2026-10-27. Never use the bare store's `main` as the integration base. Use `origin/main`. |
| S1-D T-06 reviewed candidate | `29bb6cb`: merge-base `e6ee199`, **39 commits behind** main |
| S1-C T-03 accepted static / tip | `859f74e` / `cf92b9c`: merge-base `b56af1a`, **75 commits behind** main |
| RACK-V2-09 branch `feature/scenario-one-rack-v2-field-marking-v1` | `63e95ba`. It exists **on GitHub only**, not in the local bare store, and is behind f76eb20. Fast-forward it to current main before work starts. |

## 3. Machine and storage result

- **Unity lock:** `python3 -B /media/jer/ZX20/CCF-Ops/scripts/unity-lock status --json` returned `{"status":"free","unity_editor_pids":[]}`. Under `python3 -I` the script fails, because the `_ccf` import needs the script directory on the path. Use `-B` without `-I` for this script.
- **Unity/UnityHub processes:** none, apart from matches on my own shell.
- **ZX20 free space:**
  - **12 GiB** at audit start (`df -BG`).
  - **15,440 MiB (≈15.1 GiB)** mid-audit.
  - **20,732 MiB (≈20.2 GiB)** at the end of the audit.
- **Concurrent relocation was observed during this audit.** The orchestrator, or another actor, was already moving worktrees while I inspected:
  - `CCF-RackV2-01`, `CCF-RackV2-03A`, `CCF-RackV2-04A` and `CCF-RackV2-02B-R` left ZX20 and now live in `/media/jer/Videos/CCF-ZX20-overflow/worktrees/`.
  - `git worktree list` already points at the new paths, so the repair has been done.
  - Earlier relocations (`04V2`, `05`, `06`, `07`) and `CCF-Temp` scratch directories were already in the overflow area.
  - These moves were not mine. Record them in the orchestrator's own storage receipt with the copy/checksum evidence.

### Candidate assessment (packet rules: clean, inactive, merged, unowned)

| Worktree | HEAD | Clean | In `origin/main` | Live claim / owner reference | Verdict |
|---|---|---|---|---|---|
| CCF-RackV2-01 | `089dbeb` (detached) | yes | yes | none | SAFE; already relocated |
| CCF-RackV2-03A | `089dbeb` (detached) | yes | yes | none | SAFE; already relocated |
| CCF-RackV2-02B-R | `fef344d` | yes | yes | none | SAFE; already relocated |
| CCF-RackV2-04A | `d45e326` | yes, but a **sparse checkout** (952 of 5070 files present by design; `git status` empty) | yes | none | SAFE; already relocated. The sparse state is original, not a truncated copy. |
| CCF-RackV2-03C | `098e187` | yes | yes | none | SAFE (≈1.8 GiB on exFAT); not yet moved |
| CCF-CloudPacketPub | `0ebfb0c` | yes | yes. Its upstream `a928c31` is also merged, so the local copy is merely behind. | none | SAFE (≈1.8 GiB); not yet moved |

**Storage verdict:**
- The candidate set is **safe** under the packet rules.
- The relocations already applied have restored ≈20.2 GiB, which is above both the 15 GiB floor and the 20 GiB headroom target.
- If more headroom is needed, move `CCF-RackV2-03C` and `CCF-CloudPacketPub` next (≈3.6 GiB).
- Second-wave candidates, all merged, clean when scanned and with no open claim: `CCF-RackV2-04`, `CCF-RackV2-08`, `CCF-RackV2-Combined`, `CCF-Reconcile`, `CCF-S1C-Impl01`, `CCF-S1C-Impl02`, `CCF-S1D-T01`, `CCF-Open3Sync`, `CCF-MFM02`, `CCF-ModelCost`, `CCF-ModelCostClaude`. Re-check each one's status immediately before moving it.

**Do not relocate:**
- `CCF-Integration`: dirty (`?? Scenario1V1/`).
- `CCF-Primary`: dirty with user work (5 entries).
- `CCF-S1C-Impl03`: claim #19's retained lane.
- `CCF-Q10`: #74's lane; the Unity phase is still pending.
- `CCF-S1D-T06`, `CCF-S1D-T06Review` (exact `29bb6cb`) and `CCF-S1D-T06IndependentConfirm`: evidence and candidate lineage for #88.
- `CCF-U1`, `CCF-U2`, `CCF-U3`: Unity lanes; status scan did not finish.
- Unmerged branches: `CCF-RackV2-03B`, `CCF-RackV2Layout`, `CCF-Parity01/02/03/05`, `CCF-MFM01`, `CCF-ContextRefresh20261029`. They may be fully published, but they are not ancestors of main, so prove a remote copy exists before treating them as inactive.
- `CCF-Ops`: lock tooling.

**First operational gate:** run a fresh `df` and `unity-lock status` immediately before the first lock acquisition. Free space must be ≥15 GiB; ≥20 GiB was observed at the end of this audit.

## 4. Claim and worktree inventory summary

### Open claims (`gh issue list`)

| Claim | Status | Note |
|---|---|---|
| #91 | RUNNING | Parent |
| #90 | WAITING (storage) | RACK-V2-09 |
| #88 | WAITING | S1-D T-06 Unity |
| #74 | WAITING: UNITY | Static Q10 handed off at `3383e76` (`CCF-Q10`); controller measurement remains |
| #70 | WAITING | PARITY-05 Unity |
| #39 | WAITING | MFM-03 |
| #19 | WAITING | S1-C T-03 rendered |
| #28 | BACKLOG | Not a dependency |

**Claim text is stale on four claims.** The bodies of #88, #70, #39 and #19 still say "behind #49", but #49 (RACK-V2-08) is closed and integrated through PR #89. Live `active-tasks.md` correctly overrides this. When each lane is released, update its claim body so a worker does not stall on a closed dependency.

**Worktrees:** 47 registered with the shared store. Clean except `CCF-Integration` and `CCF-Primary`; the U1–U3 scan was incomplete when I stopped it.

## 5. Completed vs outstanding (from evidence)

### Task lists that still say TODO despite published evidence

- **Rack V2 `tasks.md`:** all 14 tasks still say `TODO`. RACK-V2-01…08 are integrated (PRs #48/#80/#89; v21 save foundation `cf84d24`). Correct the list at RACK-V2-14 or before; do not redo the work.
- **S1-C `tasks.md`:** T-01…T-08 all `TODO`. T-01 and T-02 are integrated per `current-milestone.md`. T-03 is static-accepted only and not integrated.
- **S1-D `tasks.md` summary:** T-01 and T-02 are still listed as `AUTHORISED`, but both are integrated (T-02 via PR #76 / `9fcb5ee`). T-06 is static/executable-review PASS with Unity pending. T-07 is cancelled.
- **Parity `tasks.md`:**
  - PARITY-03 still says "independent static review #9 remains open", but #9 is CLOSED / HANDED-OFF.
  - PARITY-05 still says "conformance patch DISPATCHED", but PR #69 is integrated; only the Unity run under #70 remains.
- **MFM `tasks.md`:** no status fields. MFM-02 is integrated (PR #38 / `07eee3d`). The MFM-01 handoff branch `8991418` is not in main; check that its qualification was accepted (the milestone treats MFM-02 as having followed it). MFM-00 coordination sync is reflected through D-065 and the milestone.

### Genuinely outstanding work

| Spec | Outstanding |
|---|---|
| Rack | 09, 10, 11, 12, 13, 14 |
| Parity | 05 (Unity only), 06, 07, 08, 09; Q10 controller phase (non-blocking) |
| S1-C | T-03 (rendered → integrate), T-04, T-05, T-06, T-07, T-08 |
| S1-D | T-03, T-04, T-05, T-06 (Unity + integrate), T-08, T-09 |
| MFM | 03 to 12 |
| Final | Phase G Tier-3 |

## 6. Dependency DAG

```
STORAGE GATE (≥15 GiB, fresh df + lock status)
 │
 ├─► R09 Rack field marking (#90) ─┬─► R10 Help ─┐
 │                                 └─► R11 Persistence (v21 qualification; no new schema expected)
 │                                                  └─► R12 Functional ─► R13 Rendered ─► R14 Integration  ══► RACK EXIT
 │
 ├─► #88 S1-D T-06 Unity @29bb6cb ─► integrate T-06 (ForestSaveValidation.cs) ─► post-merge compile/save gates
 ├─► #19 S1-C T-03 rendered @cf92b9c ─► integrate T-03
 ├─► #70 PARITY-05 Unity (no code)
 └─► #74 Q10 controller (non-blocking evidence)

Parity:  P05 ─► P06 (broadleaf timber/economy) ─► P07 (player truth) ─► P08 (save/determinism, NO schema bump) ─► P09  ══► PARITY VERIFIED

S1-C:   T-03 ─► T-04 ; T-03 ─► T-06 ; T-01 ─► T-07
        T-05 (schema v22) requires: T-06(S1-D) integrated + Rack R11 settled (v21 final)
        all ─► T-08

S1-D:   T-03 (cap decouple)                      (independent of S1-C UI)
        T-04 (guide monotonicity) ──must land before──► MFM-09
        T-05 (prev vs proposed) after S1-C T-03/T-04 in AnnualReviewView/WorkPlanView
        T-06 integrated ─► T-08 (3-cycle witness) ─► T-09 (independent review)

MFM:    RACK EXIT ─► M03 (static now; Unity part queued) ─► M04 ─► M05 ─► M06 (schema v23 = next after S1-C v22)
        ─► M07 ─► M08 ─► M09 (after S1-D T-04) ─► M10 (after PARITY VERIFIED) ─► M11 ─► M12  ══► MFM EXIT

All five exits ─► Phase G: candidate branch ─► Tier-3 ─► independent final review (Claude A if independent) ─► FINAL-HANDOFF
```

### Recommended execution order

Heavy Unity work is serialized. Static and offline work runs in parallel where the files do not overlap.

1. **Storage gate:** confirm it (already ≈20 GiB).
2. **Unity:** #90 R09.
   - In parallel, offline: MFM-03 static measurement (#39). It touches only research/measurement tooling.
   - In parallel, offline: PARITY-06 inspection and design (read-only until its writer is assigned).
3. **Unity:** #88 T-06, then integrate.
   - Do this early. Every later schema bump edits `ForestSaveValidation.cs`, which T-06 also changes, so landing T-06 first removes a three-way conflict.
4. **Unity:** R10 and R11, then R12, R13, R14 → Rack exit.
   - #19 T-03 rendered and #70 PARITY-05 Unity can be fitted between Rack Unity runs, but **integrate T-03 only after R14**, or rebase it onto R14. Both touch shared HUD/Stand Map regions: the T-03 diff against main shows `StandMapView`/`WalkingHudView` drift because its base is old.
5. **Parity chain:** P06 → P07 → P08 → P09, alongside S1-D T-03.
6. **S1-C, then S1-D (one UI writer at a time on `AnnualReviewView`/`WorkPlanView`):**
   - S1-C T-04, then T-05 (v22), T-06, T-07, T-08.
   - Then S1-D T-04 and T-05, then T-08 and T-09.
7. **MFM:** M04 → M12, with M06 at the next schema after S1-C T-05 and M10 after P09.
8. **Phase G.**

## 7. Overlap and schema/version risks

1. **Save version sequencing.**
   - `ForestSaveData.CurrentVersion = 21`, owned by Rack V2 (`cf84d24`).
   - `RackSaveData.entranceMarked` and `entranceTreeId` already exist in v21. So RACK-V2-09 and RACK-V2-11 should need **no new schema**: R11 qualifies v21.
   - If R09 or R11 finds a new field is needed, stop. The R09 packet already says so (`RACK-V2-09 PERSISTENCE DEPENDENCY — ROUTE TO RACK-V2-11`). A new version would also need G5.
   - S1-C T-05 takes **v22**, which S1-C `tasks.md` names explicitly if Rack integrates first.
   - MFM-06 takes the **next available version after that (expected v23)**. The MFM plan forbids hard-coding the number.
   - PARITY-08 and S1-D: **no schema bump** (T-07 is cancelled).
   - **The orchestrator must fix this order and record it in `active-tasks.md`** before either T-05 or MFM-06 starts. Both are additive `ScenarioOneSaveData` changes and must not be developed concurrently.
2. **World-hash compatibility path.**
   - The v21 fields are deliberately "declared last and adjacent". The archive and compatibility hash code (`ScenarioReferenceArchive.cs:256`, `ScenarioOneWorkOrder.cs:58`) strips exactly those bytes to reproduce pre-v21 hashes.
   - Every later additive field (v22 `objectiveAchievements`, v23 MFM prescription state) must extend that strip path. Otherwise protected determinism/lifecycle hashes move.
   - This is the highest-risk area for silently rebaselining an anchor. It needs a cross-provider independent review at T-05 and at MFM-06.
3. **`ForestSaveValidation.cs`** is written by S1-D T-06 (required blob `28b786d…`), S1-C T-05, MFM-06 and PARITY-08 (unknown-species rejection before mutation). These must be serialized in the order T-06 → T-05 → MFM-06, with PARITY-08 verification re-run after each.
4. **`ScenarioOneManager.cs`** (incl. `EvaluateProgress`) is written by R09 (spray/mark interaction), S1-D T-03 (footprint cap), S1-C T-05 (achievement append) and MFM-07. One writer at a time. The MFM plan §18 makes the same rule.
5. **`UI/AnnualReviewView.cs`, `ScenarioManagementHistory.cs`, `ManagementHistoryText.cs`, `WorkPlanView.cs`** are written by S1-C T-03…T-06, S1-D T-05 and MFM-07/09 (provenance and history linkage). S1-D T-05 must use the S1-C surfaces; no second history UI.
6. **Guide/curriculum files** (`LearningObjectivesView.Curriculum.cs`/`.Sequence.cs`, `MenuHelpView.Text.cs`) are written by R10 Help, S1-D T-04 and MFM-09. Order: R10 → S1-D T-04 → MFM-09.
7. **`StandMapView` and map model** are written by Rack R09/R10/R13 and MFM-08. MFM-08 strictly follows R14.
8. **Stale candidate bases.**
   - #88 qualifies `29bb6cb`, 39 commits behind; #19 qualifies at a base 75 behind.
   - Their own file sets do not overlap anything changed on main since their bases, so a textual merge should be clean.
   - But a Unity pass on an old base does not qualify the merged result. After each merge, require an import/compile and save-gate rerun at the integrated HEAD. This applies most strongly to T-03, because its UI interacts with Combined HUD layout. Memory notes say Combined HUD failures were pre-existing and flagged after RACK-V2-08; check whether those are now fixed on main before blaming T-03.
9. **The parity numeric mapping (#31 M1/M2/M3).** The PARITY-05 preflight required Manager decisions. Parity `tasks.md` records "Manager mapping resolved" and PR #69 is integrated, so I treat this as resolved. Confirm the decision record before PARITY-06/07 reuse those numbers.

## 8. Blockers

**I found no genuine product or authority blocker:**
- Schema use is authorised: S1-C §15 accepts the additive v22; D-065 authorises the MFM next-available schema.
- Rack v21 already exists.
- No two active writers are currently assigned to the same subsystem.

**Conditions the orchestrator must satisfy. These are not product decisions:**

- **G0:** confirm and record that this delegate ran as Opus **medium** (see §1). If it did not, re-run this review or record the substitution with the Manager before substantive work.
- **G1:** take a fresh storage and lock preflight before the first Unity acquisition.
- **G2:** record the schema order (Rack v21 final → S1-C T-05 v22 → MFM-06 next) and the single-writer order in `active-tasks.md`.
- **G3:** use cross-provider independent review for T-06 integration, T-05, MFM-06, PARITY-08 and any change to the hash-compatibility path.

**Potential stops to watch for. Do not invent fixes:**
- R09 or R11 needing a v22 field.
- An extension to the hash strip path that cannot keep protected anchors stable.
- Completion or solvency becoming unreachable at MFM-11.

The packet's hard-stop rules apply.

## 9. Verdict

`OVERNIGHT PROGRAMME READY TO EXECUTE`

**Conditions on this verdict:**
- Gates G0–G3 above. G1 is the first operational gate.
- Storage remediation was already observed at ≈20.2 GiB, using a safe candidate set.
- No Windows build.
