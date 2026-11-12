# #438 rack-entrance identity: native read-only witness result

**Date:** 12 November 2026 (run 23:37–23:40 UTC). **Task:** #438, run as G2 of [#443](https://github.com/jermur788/CCF/issues/443) (not a second writer).
**Packet:** [Manager #424 witness packet](https://github.com/jermur788/CCF/blob/docs/manager-424-ux440-handoff-review-20261112-v1/Docs/Handoffs/ScenarioOne-PlayerTest-Release-20261112/MANAGER-424-RACK-ENTRANCE-IDENTITY-NATIVE-SAFETY-WITNESS-PACKET-20261112.md).
**Worker:** Anthropic Claude Opus 5.5 (`claude-opus-5-5`), T3 Code on the Linux host. The packet names Sol/Codex GPT-6.1 Sol High. Sol hit its usage limit before doing any work, and the user then explicitly chose Claude for this task.
**File name:** kept as the packet names it (`SOL-438-…`), although the worker is Claude.

## Classification: **A — presentation only, for the tested fixture**

| Question | Result |
|---|---|
| Wrong mark ↔ rack ↔ tree ↔ work association (C) | **Not observed. Disproven for the tested fixture:** 5/5 sprays each marked exactly the rack that owns the sprayed tree. |
| Stable rack ID order vs physical south→north order (B) | **Equal in the tested fixture** (IDs 1–5 = south→north). The player's "Rack 1 at the far/upper end after the first mark" was **not reproduced.** |
| First target before any spray (A) | Source target, HUD direction box and 3D cue all point to rack 1 (southernmost) **before** the first spray. The **minimap shows no rack destination at all**, and at spawn the 3D cue is 53 m away. |
| Work Plan orders | 149 Pending (30+18+34+36+31). Every rack's orders equal its frozen removal set, and every entrance tree's order belongs to its own rack. Nothing was approved or advanced. |
| Evidence reliable enough to classify (D) | Yes for this fixture: native input, isolated profile, non-black captures at 1280×720 and 1920×1080, raw log. One harness measurement defect is disclosed below. |

**PASS / FAIL / NOT VERIFIED:**
- Association safety for the tested fixture: PASS (0 mismatches).
- The player's own D-073 world/sequence: **NOT VERIFIED**. Their save was protected and not read.
- Generalisation to edited or hand-drawn plans: **NOT VERIFIED**.

## Fixture and custody

- **Candidate:** `integration/scenario-one-personal-playtest-d073-20261112-v1` = `55863f1b7719011bba16b3f3ab377e379c0b988a`, unmodified. **Lane:** `/media/jer/ZX20/CCF-S1-Objective1UX-20261112` on branch `feature/scenario-one-objective1-ux-repair-20261112-v1` at the same HEAD. The lane was clean before the run and was restored clean after it.
- **Unity lease:** `unity-lock` was held for task `#443` (lock `f69f4c9a…`) for the run, then released after it. The status is now `free`, with no Editor PIDs. An earlier lease (`15889069…`) recorded a short-lived PID and went stale, so I released it myself (exact owner and lock id) and re-acquired. Receipts are in `/media/jer/Files/CCF/Verification/CCF-443-Objective1UX-20261112/ops/`.
- **G0:** user-attested ("im telling you its released") plus agent-completed cleanup of the personal-launcher run. This is **not** a launcher terminal receipt (see #443).
- **Profile:** throwaway `XDG_CONFIG_HOME` under the evidence dir. The harness refuses to run unless `persistentDataPath` is inside it, and the save slot started and ended empty (`saveWritten=false`). The licence copy was removed after the run (`licence-copy-removed.json`). Host saves/prefs, the personal-playtest lane and the profile, Reference v1 and Legacy40 were all untouched.
- **No production source edits.** The disposable harness was staged into `Assets/ForestPrototype/` by the runner and removed with its `.meta` (`leftover: []`). The Editor's six known side-effect paths (`.vscode/settings.json` and five SectionFive `.mat`) were archived to `editor-drift.patch` and restored by exact path.
- **World:** fresh Enlarged80 Rack Planning V2 from the real startup screen (Start New Scenario → three premise pages). Native M → Plan Racks → Reset to suggested layout → Confirm Rack Plan. The map buttons were invoked through their own `Clickable`, as in RACK-V2-12.
- **Fixture (stated):** before each aim the player was placed beside the entrance tree (as in RACK-V2-12). P, left-click, M, Tab and Esc were real OS events on a private Xephyr display. The route walk itself was not reproduced.

## Five-row identity table (raw: `evidence-438/witness-receipts.json`)

| Visit (S→N) | Stable rack ID | Entrance (x, z) | Entrance tree | Tree pos (x, z) | Sprayed tree → rack recorded | Orders (all Pending) = removal set | Cue before this spray |
|---|---|---|---|---|---|---|---|
| 1 (only rack below the feeder track, z −27.4) | 1 | −35.4, −35.271 | PO0203 | −33.815, −35.271 | PO0203 → **1** | 30 = 30 ✓ | Rack 1 (before any spray: HUD "Rack 1 entrance · 53 m") |
| 2 | 2 | −35.4, −16.422 | PO1204 | −31.603, −16.422 | PO1204 → **2** | 18 = 18 ✓ | Rack 2 (immediately after spray 1) |
| 3 | 3 | −35.4, 0.238 | PO2003 | −33.052, 0.238 | PO2003 → **3** | 34 = 34 ✓ | Rack 3 |
| 4 | 4 | −35.4, 15.794 | PO2803 | −33.25, 15.794 | PO2803 → **4** | 36 = 36 ✓ | Rack 4 |
| 5 | 5 | −35.4, 35.232 | PO3803 | −33.783, 35.232 | PO3803 → **5** | 31 = 31 ✓ | Rack 5; at 5/5 the cue is gone and the HUD says review and approve in the Work Plan [Tab] |

The geometry, removal-set and work-order fingerprint was `307C1ED538EA0D3E` at confirm and at 5/5 (unchanged). Spawn was (−0.43, 5.25); distances along the access from the feeder junction (−35.4, −27.4) to each entrance are 7.9 / 11.0 / 27.6 / 43.2 / 62.6 m.

**Mechanism (source):** `ForestTreeMarkingManager.TryGetRackFieldTarget` returns the first unmarked, unlocked rack in `ConfirmedRacks` order, and the HUD/prompt label is `Rack {stable Id}`. `ScenarioOneSuggestedRackLayout` returns routes south→north, and a fresh plan assigns `nextRackId` 1..5 in that order, so the labels match visit order **only** for a plan created by Reset on an empty plan.

**Inferred, not witnessed:** after edits (delete and re-create a rack gives ID 6, or racks drawn by hand in another order), the stable-ID order can differ from physical order. That would produce exactly the player's "Rack 1 at the far end" with correct association, which is class **B**. It needs no ID change, only a presentation visit index.

## Minimap and HUD measurements (real screen pixels)

| Resolution | Minimap panel | Minimap map surface |
|---|---|---|
| 1280×720 | 165×164 at (1104, 546) | 157×156 |
| 1920×1080 | 247×248 at (1656, 818) | 235×236 |

`HudMinimapView.SidePx = 196` UI units. The minimap draws only a Stand-Map waypoint (`StandMapSource` `HasWaypoint`), never the rack field target, so `minimapHasDestination=false` throughout. A ~2.5× **perceived** increase as area means ~1.58× linear (≈248 px surface at 720p). That conflicts with the bottom-right column stack and needs measuring in G5. No resize was made here.

## Disclosed harness defect

The harness's `hudPanelVisible` field read `false` (and its `hudWaypoint` bounds `null`) in every receipt, but the captures show the upper-left direction box **visible** with the correct text (`w03`, `w05`). The field's ancestor-visibility test was wrong for that panel. The classification relies on the captures and the label text (`hudLine`), which agree with the source target. The HUD bounding box was therefore **not measured** and moves to G5.

## Captures (1280×720 here; 1920×1080 and the full set on local evidence)

- `evidence-438/w01-map-confirmed-1280x720.png`: confirmed five-rack plan.
- `evidence-438/w02-tree-marking-intro-1280x720.png`: Introducing Tree Marking on the HUD (before Esc).
- `evidence-438/w03-before-first-spray-1280x720.png`: after Esc and **before any spray**. HUD "Rack 1 entrance · 53 m" with arrow; minimap has no target marker.
- `evidence-438/w05-after-first-spray-1280x720.png`: after spraying rack 1. Counter 1/5; HUD "Rack 2 entrance · 19 m".
- `evidence-438/w07-work-plan-pending-not-approved-1280x720.png`: Work Plan, 149 pending · 0 approved (nothing approved).
- Harness source: `evidence-438/Rack438EntranceWitness.cs.txt`.

Local durable evidence: `/media/jer/Files/CCF/Verification/CCF-443-Objective1UX-20261112/g2-witness-run1/`. It holds the raw log (sha256 prefix `6393982f2e70b207`), `result.json`, `summary-lines.txt`, `driver/native-actions.jsonl`, the 14 captures, `witness-receipts.json` (sha256 prefix `3d6a0115cb2fdb8f`), `work-plan-visible-text.txt`, `licence-copy-removed.json` and `editor-drift.patch`.

## Consequence

Class A, with B possible only for edited plans (inferred). Per the packet, no stop: proceed to #443 G3. The bounded spec includes a minimap rack-target marker, a first-target cue, and a presentation-only south→north visit index for the label and the next-target order. **No stored rack ID, geometry, save or Work Plan change.** The Manager should examine the original evidence and issue APPROVED / NEEDS CHANGES / REJECTED. If the player's specific "far Rack 1" observation must be explained before any fix, that needs the player's own plan sequence (read-only), which I have not touched.
