# CCF #857 — blank Unity/3D scenario starter: final orchestration result

**Parent:** [#857](https://github.com/jermur788/CCF/issues/857) · **Spec:** [#856](https://github.com/jermur788/CCF/issues/856) · **Spec review:** [#858](https://github.com/jermur788/CCF/issues/858) · **Binding packet:** [BLANK-STARTER-END-TO-END-ORCHESTRATOR-AFTER-SPEC-DISPATCH-20261120.md](https://github.com/jermur788/CCF/blob/Dashboard-CCF-Blank-Starter-End-To-End-Orchestrator-Dispatch-20261120-v1/Docs/Handoffs/CCF-Blank-Scenario-Starter-20261120/Dashboard-CCF-BLANK-STARTER-END-TO-END-ORCHESTRATOR-AFTER-SPEC-DISPATCH-20261120.md)

Date: 20 November 2026. Orchestrator: `opencode-go/deepseek-v4.1-flash` (coordination only), host `jer-laptop`.

---

## 1. Status — the six categories are separate

| Status | Result |
|---|---|
| **SPEC APPROVED** | **YES, conditionally** — owner G5 + owner-delegated G4, 2026-11-20, for the corrected four-file blob at `23da9850a7245a8623244ff2e2e76fbf46c33f0c`. The documents themselves still read `DRAFT / NOT APPROVED / NO IMPLEMENTATION AUTHORITY`; acceptance is recorded as an owner decision, not by editing them. |
| **SOURCE IMPLEMENTED** | **YES** — 26 paths on `feat/ccf-857-blank-starter-20261120-v1`, published at `511032f0124e28fdf61a8d1b676c81330f7fa5c0`. Production bytes are those of `c5e0547a`; the two later commits change verification tooling and documentation only. |
| **UNITY VERIFIED** | **PARTIAL** — Tier 1 and most of Tier 2 PASS on Unity `6000.6.0f1`. Two gates are not closed: a fully clean three-witness access trace (§6, R-01 caveat) and the Enlarged80 deterministic anchor (§6, R-02). |
| **INDEPENDENT REVIEWED** | **YES for the candidate, verdict NEEDS CHANGES** — [review of `c5e0547a`](https://github.com/jermur788/CCF/blob/32a028c753f877b9d35eebbac2ee0c45640ce246/Docs/Handoffs/CCF-Blank-Scenario-Starter-20261120/Dashboard-CCF-857-BLANK-STARTER-INDEPENDENT-CANDIDATE-REVIEW-20261120.md) by GPT-6.1 Terra. The bounded repair is **not** delta re-reviewed; see §7. |
| **INTEGRATED** | **NO** — never attempted. No merge was authorised, and #844 was not reachable. |
| **OWNER ACCEPTED** | **NO** — the owner has not played or accepted the starter. |

**Honest one-line summary:** the blank starter exists, compiles, renders and is largely qualified; it is independently reviewed with open findings, one of which is an inherited limitation of a stale historical harness rather than a defect in this work.

---

## 2. What was delivered

Branch `feat/ccf-857-blank-starter-20261120-v1`, 26 changed paths (+2532/−56 at the reviewed head):

- `Assets/Scenes/BlankStarter.unity` — flat y=0 ground, 30 × 30 m, collider, player spawn inside the boundary, existing `ForestPlayer` and camera, no world actors.
- `Assets/Scenes/BlankStarter45x60.unity` — TEST ONLY copy, 45 × 60 m, duplicated scenario-owned data only.
- `Assets/ForestPrototype/BlankStarter/BlankStarterPreview.cs` — one scene-local component (footprint, spawn, DEV text slots, preview marker). No manager, no ecology, no registry, no singleton.
- Minimum empty-safe seam in `ScenarioOneUiRoot`, `WorkPlanView`, `AnnualReviewView`, `LearningObjectivesView`, `StandMapView`, `StandMapSource`, `StandMapModel`, `StandMapTransform`, `WalkingHudView`, `HudMinimapView`, `ForestPlayer`, `ForestSaveController`, `StandaloneSessionMenu` and its `Startup` partial.
- Fail-closed save fence: one predicate `BlankStarterPreview.BlocksHistoricalSave(scene)`, tested at the head of `Save`, `Load`, `CanLoad` and before every `File.Exists` in the F10 and startup paths; `Restart` is fenced by the marker **and** `buildIndex < 0` (Unity reports `1` for the unlisted scene, so the marker does the real work).
- Rectangular map by **uniform** pixels-per-metre from the larger extent, letterboxed — exact 3:4 boundary, no distortion, no second coordinate system, historical square map unchanged.
- Four disposable verification files under `Tools/Verification/BlankStarter/`.

**Protected and verified unchanged** (re-hashed by the orchestrator): `Assets/Scenes/ForestTest.unity`, `ProjectSettings/EditorBuildSettings.asset`, `Packages/manifest.json`, `Packages/packages-lock.json`. The diff contains no `ProjectSettings`, `Packages`, save-schema or ecology-geometry path.

---

## 3. Evidence

Root: `/media/jer/Files/CCF/Verification/CCF-857-BlankStarter-20261120/`

| Subdirectory | Contents |
|---|---|
| `isolated-final/`, `isolation-retry/` | original compile, 1280 × 720 captures `01-…09-…` and `slot-*.png`, 109-check clean run |
| `historical-final/` | old-world positive: 17 checks, Legacy40 override 64 cells / 336 trees, v20 Enlarged80 load with 1,344 mapped trees, five rack routes, round trip |
| `native-r1/` | the first live-path run and the host-directory incident record |
| `review-repair/` | **the R-01/R-02 repair**: `negative/` (three staged witnesses, 115 checks, trace, positive control, screenshots), `determinism/` (reference continuation and Enlarged80 logs), `exit-codes.txt`, `repair-tree-identities.txt` |

**Note for readers of the implementation handoff:** its reproduction block names `$e/reproduce/…`. The evidence actually lives in `$e/review-repair/…`. This document is authoritative on paths; the handoff's prose and its per-criterion table remain accurate.

`repair-tree-identities.txt` and the orchestrator's own check agree: the `Assets` tree is `91a41532be014be91539f6d9aafec226b80a7a15` at both the reviewed head `c5e0547a` and the repair commit — the reviewed production bytes did not move.

---

## 4. Unity and lease record

| Item | Value |
|---|---|
| Editor | `6000.6.0f1`, `/media/jer/ZX20/Unity/6000.6.0f1`, launched inside the Flatpak Unity Hub sandbox (licence smoke PASS) |
| Lane | `/media/jer/ZX20/CCF-Sen1-857-BlankStarter-20261120`, registered as `Sen1-857-BlankStarter` |
| Leases | `f736db4d50914aafbf82f920d707ef8c`, `860e848a47914b06903e85a2bb03cb9b`, `0c7756aeee204f0683b79e282f5811df` — all released **normally**, no `--force`, no Editor left running |
| Lock at close | **free**, no Editor PIDs |

---

## 5. Acceptance summary

PASS below means supported by the cited evidence for the inspected candidate, not independently re-run by the reviewer.

| ID | Result | Basis |
|---|---|---|
| AC-01 / N-13 | PASS | import, true bounds, walk/look, invalid footprint rejection on both scenes |
| AC-02 / N-01 | PASS | shared rendered UI, four headings, no phantom tree, no cloned UI |
| AC-03 / N-02 | PASS | seven layers empty, no rack editing/help/tool, square-map regression 176/176 |
| AC-04 / N-03 | PASS | exact Work Plan text, safe controls, no year/work/economy mutation |
| AC-05 / N-04 | PASS | exact review/objectives strings, no historic lesson or year |
| AC-06 / N-05–N-08 | **PASS with a trace caveat** | three witnesses staged and preserved byte/metadata-identical, 115 checks, positive control covers all three paths; the trace also contains one `21465 ???( <detached ...>` record that names no path (§6) |
| AC-06 / N-09 | **FAIL for full host-directory restoration** | save and sidecars preserved; the first live-path run rewrote non-save Unity housekeeping files |
| AC-07 / N-10 | PASS | true 45 × 60 rectangle in world and map, shared GUIDs, no copied C# |
| AC-08 / N-11 | PASS | per-slot empty → distinctive value → cleared observed for all three slots, banner always visible |
| AC-09 / N-12 | **NOT TESTED in full** | Reference continuation gate PASS (exit 0); Enlarged80 gate FAIL at a pre-existing v23-only assertion (§6); historical load/render/round trip PASS |
| AC-10 | **PARTIAL** | candidate independently reviewed; repaired tooling not delta re-reviewed; no integration |
| FR-01…FR-07, FR-09…FR-11, FR-13 | PASS | per the review's own matrix |
| FR-08, FR-12 | PARTIAL | see AC-06 and AC-09 |

---

## 6. Two open technical items, stated plainly

**R-01 caveat — the access trace is not byte-empty.** The repair staged all three existing witnesses (`forest-save.json` 1,299,807 B, `.bak` 68 B, `.tmp` 52 B) and the harness passed 115 checks with zero runtime errors. The `strace -f -P` output for the run is 26 bytes containing exactly `21465 ???( <detached ...>`: a detached-process placeholder emitted by `strace -f` for a short-lived child, not a record naming a save path. The wrapper requires an empty trace, so it exited 1. The positive control (1,876 bytes) proves `stat`/read records *are* captured for all three paths, and the earlier 109-check run had a genuinely zero-byte trace but staged only the main file. **Conclusion:** no access to the save paths was identified in any run; a single-run, zero-byte, three-witness trace was not produced.

**R-02 — Enlarged80 determinism is blocked by an inherited stale gate.** `Enlarged80Verification` aborts with `an Enlarged80 capture is v23 + geometry 1` before reaching the Year-3 anchor. This is the same v23-vs-v24 staleness recorded as baseline debt in #830; it is not a regression from this candidate, whose diff touches no simulation, ecology or geometry code. Fixing it would mean editing a historical verification gate, which the spec forbids ("do not weaken baselines for green"). It is therefore recorded as **NOT TESTED** with the named blocker, not as a pass.

**Host-directory incident, corrected record.** The first live-path run rewrote `prefs`, `Unity/*/Editor/Analytics/*` and `Unity/*/Insights/*` under `/home/jer/.config/unity3d/DefaultCompany/CCF/`; the directory was **not** restored byte-for-byte and the original `prefs` bytes were not retained. My first adjudication claimed "no `PlayerPrefs` writer exists in production or harness code" — **that was wrong and is retracted**: `UI/LearningObjectivesView.cs:91-114` and `UI/MenuHelpView.cs:64-71,154-155` do write `PlayerPrefs`. Corrected statement: those writers exist but were not identified as reachable in the exercised preview path, which is not a byte-level proof. The save slot itself is unaffected: `forest-save.json` absent before and after, no `.tmp`, `.bak` SHA-256 unchanged. Future qualification must use an isolated XDG config root.

---

## 7. Review trail and the open review gap

1. **Spec review #858** (GPT-6.1 Sol) — NEEDS CHANGES: F-01 undefined production-content gate, F-02 conditional review wording; F-03–F-05 factual.
2. **Spec delta review** (GPT-6.1 Terra) — NEEDS CHANGES: D-01 stale package links, D-02 unproven slot binding, D-03 unlabelled proposals; F-03–F-05 confirmed resolved.
3. **Bounded corrections** — `c4d75f8e` (Sonnet 5.5) then `23da9850` (GPT-6.2 Sol). The final three fixes were verified by the orchestrator's direct inspection of the full compare, not by a third independent review.
4. **Candidate review** (GPT-6.1 Terra) — NEEDS CHANGES: R-01 sidecar witnesses, R-02 determinism, R-03 allowlist count, R-04 orchestrator overstatement.
5. **Bounded repair** — `40182dca` + `511032f0`: three witnesses staged, determinism gates attempted, R-03 clarified (26 paths, four verification files), R-04 retracted in the record.

**Open gap:** a delta re-review of the repaired tooling and evidence could not be obtained. Every provider route reached its usage limit during this session (`anthropic` rejected its key; the `opencode` pool disconnected; three subagent attempts terminated at "usage limit has been reached"). The repair was therefore verified by the orchestrator's direct read-only inspection of the evidence files, which is weaker than independent review and is recorded as such. The production bytes under review did not change, so the candidate review's scope is unaffected.

---

## 8. What is not done, and the exact next steps

1. **Delta re-review of the repaired evidence** — one fresh nonauthor reviewer over `review-repair/`, `run-native.sh` and the corrected handoff.
2. **Enlarged80 determinism** — either repair the stale `Enlarged80Verification` v23 expectation under a separately authorised historical-gate task, or accept AC-09 as permanently partial for this starter.
3. **Clean single-run trace** — one isolated run staging all three witnesses with a genuinely empty trace, if a byte-empty trace is required literally.
4. **Integration** — #844 (3D source manager) must ratify the `ForestPlayer.cs` amendment and decide integration; no merge is authorised and none was attempted.
5. **Owner acceptance** — the owner has not played or reviewed the starter.
6. **Windows/Player build** — explicitly not authorised and not attempted.

---

## 9. Routing record

Routing decisions are in [`orchestration/routing-ledger.jsonl`](https://github.com/jermur788/CCF/blob/orchestration/ccf-857-blank-starter-end-to-end-20261120-v1/orchestration/routing-ledger.jsonl) on the orchestration branch: `d857-01` spec review, `d857-02` correction 1, `d857-03` delta review, `d857-04` correction 2 (two failed launches recorded), `d857-05`/`d857-06` implementation and the `ForestPlayer.cs` G4 amendment, `d857-07` candidate review and my retraction, `d857-08` bounded repair.

Every provider substitution is recorded with its reason. The preferred Opus 5.5 route was never available in this session and was not silently replaced.

---

## 10. Limits of this document

This is a coordination record. It does not grant integration, merge, release or owner acceptance. It does not claim a rendered witness that was not captured, a test that was not run, or a review that was not performed. Where evidence is partial, it is labelled partial.
