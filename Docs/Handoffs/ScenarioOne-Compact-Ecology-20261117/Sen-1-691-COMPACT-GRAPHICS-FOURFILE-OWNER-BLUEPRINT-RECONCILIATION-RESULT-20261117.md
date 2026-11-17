# Sen-1 #691 — Compact30 graphics four-file DRAFT reconciliation: author result

**AUTHOR HANDOFF — READY FOR INDEPENDENT DIFFERENT-FAMILY DOCUMENT REVIEW. SPEC STATUS: DRAFT — NOT APPROVED.**
No assets, Unity, C#, scene, Prefs, saves, Ops, Windows, `main` or live-board writes. The author does not approve the specification.

## 1. Registration and exact identity

| Field | Value |
| --- | --- |
| Claim | [#691](https://github.com/jermur788/CCF/issues/691); [pre-edit RUNNING registration](https://github.com/jermur788/CCF/issues/691#issuecomment-6960474035) |
| Packet | [Complete #691 handoff](https://github.com/jermur788/CCF/blob/docs/sen-1-675-689-base-decision-a0-688-asset-blueprint-verdict-20261117-v1/Docs/Handoffs/ScenarioOne-Compact-Ecology-20261117/Sen-1-691-COMPACT-GRAPHICS-DRAFT-CONVERGENCE-PACKET-20261117.md), branch tip `92fdb99e` |
| Actual worker | Anthropic **Claude Opus 5.5** (`claude-opus-5-5`), Claude Code harness in T3 Code, Linux host `jer-laptop`; direct user dispatch. Packet named GPT-6.1 Sol / DeepSeek as best suited; the user assigned this worker. No delegates. |
| Lane | Docs-only worktree `/home/jer/ccf-worktrees/sen-1-691-graphics`; no Unity project opened, no Ops lease, #652 untouched |
| Branch | `docs/sen-1-691-graphics-fourfile-owner-blueprint-reconciliation-20261117-v1` |
| Starting HEAD | `d36ce779224e418c400f461c16edd4f37630c5d0` (tip of `docs/sen-1-684-cgtrig-signed-compact-draft-author-20261117-v1`) |
| Spec commit | `8f03eda2181676704d9ae541e439379bc1417a27` (four graphics files only); this result is the following commit |
| Live board at start | `origin/main` `ce5c944e`: #691 READY / NOT RUNNING; no other writer of `specs/scenario-one-compact-graphics-v1/**` |
| Coordination anchor | `/media/jer/Files/CCF-Primary` on `ops/workflow-foundation-v1` `46247486`, pre-existing modified `AGENTS.md` and `Docs/Operations/StorageArchitecture.md` preserved untouched |

### Baseline choice (reviewer: please check)

The packet pins graphics at `2a76f77b` (tree `acf8dc9f`, #590) and says to start from the *verified current graphics DRAFT tip*. Remote inspection found two later lines:

| Line | Graphics tree | Content |
| --- | --- | --- |
| #592 tidy → **#657 D-084 convergence** → unchanged through #663/#665/#667/#684 | `42388b4a` (latest by date, carried by the #684 tip) | GA-1…GA-5 D-084 reading; GQ-07 still "open" |
| #592 → #593 owner ratification → #594 independent review → #599 precision | `6d3d6448` | Owner 16 Nov ~12:09 UTC "approve all" design direction: separate scene, W direction, see-through/vented guards, no fresh stumps, minimal boundary, neutral daylight |

#657 imported #592 rather than #593/#599, so the #593 owner direction dropped out of the latest line. **Chosen base: `42388b4a`** (latest, Manager-accepted through #684). The #593 owner directions are **restored as recorded design direction** with citations ([#593 result](https://github.com/jermur788/CCF/blob/ff7b1f1e3b132040acbed8fef6bea4b45bf84b8a/Docs/Handoffs/ScenarioOne-Graphics/Sen-1-593-OWNER-APPROVED-COMPACT-LOOK-SCENE-SPEC-AMENDMENT-RESULT-20261116.md), #594 verdict APPROVED for owner-design draft quality, #599 Manager acceptance `dbe03559`). #593 wording was **not** merged wholesale. Its separate-scene choice is now also canonical in D-085 OD-13.

## 2. Inputs read (pinned)

- Canonical `main` `ce5c944e`: Decision Log D-032, D-083, D-084, **D-085** (`d435292b`); active-tasks board.
- Owner tree route A receipt: [#675 comment 6957589672](https://github.com/jermur788/CCF/issues/675#issuecomment-6957589672) (17 Nov 18:45 UTC).
- [#688 Stage 0 blueprint](https://github.com/jermur788/CCF/blob/2a625018d04d13339cafe480bdccf1a670e5f9d2/Docs/Handoffs/ScenarioOne-Compact-Ecology-20261117/Sen-1-688-NATURAL-ENTRANCE-FOOTPATH-ASSET-BUILD-BLUEPRINT-20261117.md) (blob `9bdfb224`) and [Manager #688/#689 verdict](https://github.com/jermur788/CCF/blob/docs/sen-1-675-689-base-decision-a0-688-asset-blueprint-verdict-20261117-v1/Docs/Handoffs/ScenarioOne-Compact-Ecology-20261117/Sen-1-675-MANAGER-688-689-STAGE0-VERDICTS-AND-NEXT-SOURCE-GATES-20261117.md), accepted as static planning only.
- [#698 Stage 0 blueprint](https://github.com/jermur788/CCF/blob/7a8e575ec1e6dc6157f9dda4710c006564faf0f5/Docs/Handoffs/ScenarioOne-Compact-Ecology-20261117/Sen-1-698-OAK-BEECH-JUVENILE-AND-SHELTER-ART-BUILD-BLUEPRINT-20261117.md) (blob `db4feccc`) and [Manager #698 verdict](https://github.com/jermur788/CCF/blob/aebcd9c69a8137039d6b8b3472b7583ea6166e0b/Docs/Handoffs/ScenarioOne-Compact-Ecology-20261117/Sen-1-675-MANAGER-698-JUVENILE-SHELTER-STAGE0-ACCEPTANCE-AND-STAGE1-GATES-20261117.md), accepted as static planning only.
- Frozen #676 candidate (`8617587e`) and [#685 dossier](https://github.com/jermur788/CCF/blob/75b3d8eadcd0ed09eb1357c3ecabc854737ff3e7/Docs/Handoffs/ScenarioOne-Compact-Ecology-20261117/Sen-1-685-ASSET-RENDER-LOD-PROVENANCE-AND-WORLD-BLOCKER-CLOSURE-RESULT-20261117.md), via the #688/#698 citations and direct read of #685 rows 10/12/30/44/73–75/83.

## 3. Changed paths and blobs

`git diff --name-status d36ce779..<result commit>`: **M** ×4 graphics files, **A** ×1 this result. Nothing else.

| File | Before blob (`42388b4a`) | After blob (`8f03eda2`) |
| --- | --- | --- |
| `specs/scenario-one-compact-graphics-v1/spec.md` | `50d86b77dda08552f9a88eb8b60b4054fd6100d2` | `d581f6f16246fd78dfef679e3232516427b345e3` |
| `specs/scenario-one-compact-graphics-v1/plan.md` | `0e5b58fbe1c921809aa11d7bbfe28d1ac6a698e0` | `e1d1f30479b6e9d3984d8e56e7f981742c5d0437` |
| `specs/scenario-one-compact-graphics-v1/tasks.md` | `42d9c49d14c0cb5ed702039af8ff8544a7af5d00` | `fbc7c2457ba633bace2334b83a8116a3065c7a41` |
| `specs/scenario-one-compact-graphics-v1/checklists/requirements.md` | `fb0b63218f752aeec549f2caaea43d97c27a71a6` | `734f4573229c4201eef2a94df44ef2fab61326b7` |

Graphics tree after: `eccd12d7c0dd7b65ac3c3079dfec62a7332b03e5`. Parent (115) and planting (33) specs: **untouched** (not in diff).

## 4. Corpus preservation (packet §3.7)

Static checks run on the committed files:

- Checklist: **66 unchecked / 0 checked** before and after. The ordered ID list FR-01…FR-20, WF-01…06, SA-01…05, AC-01…12, G573-M1…M7, NEG-01…08, V-01…08 (20+6+5+12+7+8+8 = 66) is **byte-identical in order**: `diff` of extracted IDs is empty. No ID dropped, duplicated or added.
- `spec.md`: FR-01…FR-20 and AC-01…AC-12 each appear exactly once as requirement bullets. `spec.md`, `plan.md`, `tasks.md` contain zero checkboxes.
- All four headers still say **DRAFT / NOT APPROVED / NO IMPLEMENTATION AUTHORITY**. T-01 stays TODO (docs/review only), T-02…T-09 stay BLOCKED; nothing RUNNING or verified.
- `git diff --check` clean.

### The six checklist entries whose text changed

| ID | Old meaning | New meaning | Reason |
| --- | --- | --- | --- |
| FR-04 | 1.2 m was a source-only candidate blocked by old 2.6 m planks; 1.8 m / 2.4 m listed alongside | Owner-directed 1.2 m core + ≤0.20 m visual-only verge per side on a signed curve (C0, or T1 with a new 0.190 m cap — owner-pending) to a signed northern endpoint (A8 unsigned). Planks excluded by the separate scene. Failed shelter still conflicts. 1.8 m / 2.4 m kept as rejected comparators | D-084, D-085 OD-08/OD-10/OD-13; #688 §5. The "1.8 m candidate" framing was stale |
| FR-05 | "No unproved 0.20 m permanent edge" | Verge ≤0.20 m visual-only, never a collider or ecological edge. Footprints may be analytic regions on one plane only if GQ-08 accepts. O1/O2 intrusion figures unchanged | D-085 OD-08 makes 0.20 m an owner maximum. #688 one-plane method recorded as pending, not waived |
| FR-09 | GQ-07 A (preferred) **or** B renderer-only | Owner-selected separate Compact scene, plus an approved parent source bridge. B not used without new owner instruction | D-085 OD-13; owner 16 Nov |
| FR-16 | W/N2 owner look to be selected | W direction owner-chosen. Exact look (the #688 palette or an owner revision) accepted only from native views at 1.2 m + ≤0.20 m | Owner 16 Nov (#593/#594/#599); #688 palette is PROPOSED |
| WF-06 | Staged small truck / closed-visitor state | No parked or permanent vehicle, yard, gate or road. Any occasional collection stage/mode event is separately scoped, with no drivable footpath | D-085 OD-06; owner 17 Nov receipt |
| G573-M2 | Separate scene **or** renderer-only alternative | Separate scene excludes planks, clones and consumers. Renderer-only alternative unselected | D-085 OD-13 |

All six keep their ID, position and unchecked box. Other entries are interpreted through a new non-checkbox `#691` reading table at the top of the checklist.

## 5. Packet item crosswalk

| Packet item | Where addressed | Result |
| --- | --- | --- |
| 1 Stale product prose | spec §#691 R-1…R-4; product intent; non-goals; workflow A; FR-02/04/06/09/17; save section; AC-02/05; stop conditions; GQ-05/06/07; plan World/Historical/Phase 0/integration boundaries; tasks T-02/T-04; checklist FR-04/09/WF-06/G573-M2 | **"D-083 unpublished":** already corrected on the `42388b4a` base; D-085 added. **H1 parking / closed yard / visitor car and truck:** withdrawn; H1 is natural with no stumps, vehicles, gate, road, yard or kerb; collection is only an OD-06 stage/mode event with no V1 asset. **Shared scene:** separate Compact scene recorded as chosen; option B kept as unselected history. **1.8 m vs path width:** now 1.2 m + ≤0.20 m owner direction, with 1.8/2.4 m as rejected comparators. Player may walk off-path. Zero planar G1 displacement and the unsigned 175 roster are recorded. Historical E80 plank/forest logic preserved |
| 2 Tree route A | spec R-5, FR-12, authority §6; plan Sitka row and risk; tasks T-03 and §#691 table; checklist reading | Four existing detailed Sitka families recorded as owner artistic direction. Rights, all-LOD H/DBH/crown honesty, URP shader/script/GUID import and collision are **NOT PASSED** (#685 rows 10/12). Primitive stand-ins and other fallbacks are **BLOCKED pending a new explicit owner decision** |
| 3 Footpath/clearing kit | spec R-6, FR-05, FR-13, FR-16, GQ-08; plan §#691 and Phase 3; tasks §#691 | #688 roles recorded as **PROPOSED**: no-collider two-triangle plane, H1 inward soft mask, 1.2 m core + ≤0.20 m fringe with round north cap, one Compact-only material, original macro/detail textures, no copied art, truck, rack or canal. Method, palette W and the 20.5 MiB budget await D-032 and owner acceptance. **One-plane vs FR-05 separate footprints and FR-07 soft-rectangle edge recorded as GQ-08 owner/Manager pending, not waived** |
| 4 Curvature/A8 | spec R-7, R-8, FR-04, GQ-09, GQ-10; plan Route/G1 row | C0 radius ≥0.625284 m, deviation 0.094074 m ≤ 0.095 m. T1 m6 = m7 = −0.24, radius ≥1.488681 m, deviation 0.188148 m, **new 0.190 m cap** plus independent full world review needed. A8 (−2.10, 13.50) unsigned. Neither curve is look-accepted. Guard static margins O1 +0.185 / O2 +0.391 / B1 +0.280 / B2 +0.103 m. O1 failed 0.141 m intrusion stays a known FAIL. No tree removal or endpoint adoption |
| 5 Evidence and tests | spec R-11, FR-19; plan Tier 1/Tier 2; checklist reading FR-19/20 | Tier 1 static (planning only) is separated from Tier 2 Unity: 720p/1080p eye-height H1↔A8 walk with HUD, grayscale, no collider, package/import/texture-mask, old-world controls, all guard states and overlays, per-stage Sitka LOD fidelity, no fabricated FPS or memory. #652 HELD; nothing run. No new broad suite; old assertions not weakened |
| 6 D-032 OPEN | spec R-10, FR-18, GQ-03, GQ-12; plan asset row; checklist reading | Named author, hashes/provenance, compiled-beta policy, raw-source limits, URP custody/notices, full GUID record, budgets. Old soil/E80 assets are not automatic donors. No blanket legal inference |
| 7 Corpus preservation | §4 above | 66 / 0, IDs identical, FR/AC unchanged, DRAFT, no task RUNNING |

## 6. Owner and Manager decisions still open (none inferred)

GQ-01 exact W palette/material · **GQ-08** one-plane method, FR-05/FR-07 readings and mip/alias risk · **GQ-09** C0 vs T1 + 0.190 m cap · **GQ-10** A8 endpoint · **GQ-11** shelter look A/B, juvenile and shelter silhouettes, 4–6 m cue · **GQ-03/GQ-12** D-032 rights, storage and byte budgets, Sitka route A rights/fidelity · parent scene/source/save bridge (GQ-07 remainder) · signed #676 successor world bytes (0/80/8, digest null) · OD-16 deferred.

## 7. Verification boundary

**Done:** Git/GitHub reads of pinned sources; text edits to four Markdown files; static ID/count/diff checks above.
**Not done / not claimed:** no Unity, Blender, asset bake, render, collision, performance, save/load or native test; #688/#698 static calculations **not re-run**; no rights determination; no world signature; no board, dashboard or Drive update (Manager #675 owns those; mirrors NOT VERIFIED).

## 8. Independent review request

Prospective reviewer claim and branch `review/sen-1-691-graphics-fourfile-independent-20261117-v1`, sole writable path `Docs/Handoffs/ScenarioOne-Compact-Ecology-20261117/Sen-1-691-INDEPENDENT-COMPACT-GRAPHICS-FOURFILE-REVIEW-20261117.md`. **The reviewer must be a non-Anthropic model family**, because the author is Claude. Review the exact four changed graphics files at the spec commit against D-085, the owner route A receipt, #676/#688/#698 and §4 ID preservation. Verdict: APPROVED / NEEDS CHANGES / REJECTED. Specific checks requested:

1. The baseline choice in §1 and the restoration of the #593 owner direction (especially **W direction** as owner-chosen while the palette stays PROPOSED).
2. Whether any residual stale wording survives (vehicle, shared scene, W/N2 vote).
3. Whether GQ-08 correctly isolates the one-plane vs FR-05/FR-07 question without silently waiving it.
4. That no owner-pending item is worded as accepted.

A document-fidelity PASS does **not** approve the spec. Manager #675 must still issue a separate four-file exact-byte approval after the owner decisions above.
