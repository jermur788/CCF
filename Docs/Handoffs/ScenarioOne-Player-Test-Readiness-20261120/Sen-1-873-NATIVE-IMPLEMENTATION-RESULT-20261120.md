# Sen-1 #873 — Compact30 Editor player-test candidate: native implementation result

**Date:** 2026-11-20
**Verdict:** **IMPLEMENTED + NATIVE-TESTED — NOT "EDITOR PLAYER-TEST CANDIDATE READY".**
One blocking playthrough defect (F-1, pre-existing base code × base scene scale) and one
protected-config deviation (D-1) remain. Not REVIEWED, not INTEGRATED, not OWNER-ACCEPTED.

| Status word | State |
|---|---|
| IMPLEMENTED | Yes — candidate `3841d4c1e11f9a11777082e74b72cea6a0856cde` (source), pushed |
| VERIFIED (native, bounded) | Partly — see §4; F-1 blocks steps 6 and 8 of the intended playthrough |
| REVIEWED | No — independent non-author review outstanding |
| INTEGRATED | No — main untouched; no G4 |
| OWNER-ACCEPTED | No |

## 1. Actor, authority and continuity

- Handoff: `docs/sen1-873-one-parent-player-test-finish-20261120-v1` → `Sen-1-873-ONE-PARENT-COMPACT30-PLAYABLE-EDITOR-CANDIDATE-END-TO-END-NATIVE-HANDOFF-20261120.md`; source grant eed9f68c owner-direct on #873; Manager #872.
- Host `jer-laptop`, UID 1000, non-root; Unity 6000.6.0f1 via authenticated Flatpak Hub sandbox (#862 mode A).
- **Actor, part 1 (registration → import → native gates → most of GUI):** OpenAI Codex gpt-6.1-sol high, T3 thread `0e61f428-7d88-40b9-b0b2-332da1bcb312`, Codex session `01a1f96d-7f75-7061-9419-064f487d2705`. That run stopped on a provider usage limit at ~15:56Z with the Editor still in Play.
- **Actor, part 2 (remaining GUI steps, Editor exit, restores, lease release, this result):** Anthropic Claude Opus 5.5, **same T3 thread** `0e61f428`, continuing at the owner's "Continue" instruction. No new lease was taken; the existing lease was released by its exact recorded identity (§3). This provider change is disclosed here; both actors are authors of this candidate and neither may review it.

## 2. Source

- Branch `integration/sen1-873-compact30-playtest-20261120-v1`; gameplay base `8cb22de077b93b4c7af076e19936f3170d6ab48b`; visual delta `0709d05a…` → `5cbc26a4073ffa60db0a3c442e237eaa473ad3ff`; context `7e24ba0eee05b9af1055c88e423e629fe409bbf1`.
- Source candidate `3841d4c1` — 80 paths vs base: 74 exact #836 visual paths (`Art/Sen1PathFence/**`, `Art/Sen1SpruceFloor/**`, `Scenes/ScenarioOneCompact30.unity`), plus `Assets/Editor.meta`, `Assets/Editor/Sen1CompactPlaytestEntry.cs(.meta)`, `Tools/Verification/Sen1CompactPlaytest/{README.md, verify_composition.py, Sen1CompactPlaytestVerification.cs}`. This result file is the 81st permanent path (registered allowlist line 80).
- Allowlist: `/media/jer/Files/CCF/Verification/Sen1-873-Compact30-Playtest-20261120/source-allowlist.txt` (85 lines = 81 permanent + 4 disposable harness paths), SHA-256 `adda3cea…cd8bb`.
- Final worktree before this commit: `git status --porcelain --untracked-files=all` empty at `3841d4c1`.

## 3. Physical lock, packages and cleanup

- Lease `af22f420473449f7ad5a8ab67f4571f7`, owner `Codex-873-0e61f428`, task `SEN1-873-COMPACT30-PLAYERTEST`, acquired normally 14:57:29Z.
- Editor exited via File → Exit ~16:08Z; no Unity/UPM/DataStore/Burst process remained.
- **Released** with exact owner/task/lock-id, `forced: false`; status afterwards `free`, `unity_editor_pids: []` (`unity-release.json`, `unity-after.json`).
- Packages: Unity's auto migration Linux SDK/sysroot/toolchain 1.1.0 → 1.1.1 was used transiently under the owner amendment. The final drift was compared line-for-line to the recorded `import-package-drift.patch` (identical), then `Packages/manifest.json` and `Packages/packages-lock.json` were restored; blob hashes equal HEAD (`ca552487…`, `8c0c0978…`). Not committed.
- `.vscode/settings.json`: Unity's IDE package rewrote `dotnet.defaultSolution` and a trailing comma; patch saved (`vscode-settings-drift-final.patch`), restored to HEAD blob `9b835a42…`.
- On Editor exit Unity re-serialised 7 base `Art/SectionFive/**` materials (URP validator syncing `_MainTex` from `_BaseMap`, `_AlphaToMask` 0→1). They are outside the allowlist; patch saved (`exit-material-reserialization.patch`), restored to HEAD. **Note for later lanes:** the base materials disagree with what URP 6000.6 writes; any future Editor session will dirty them again.
- Disposable harnesses (`Assets/ForestPrototype/Sen1CompactPlaytestVerification.cs`, `Sen1CompactE2ReuseVerification.cs` + metas): absent from the worktree.

## 4. Tests — actual outcomes

| Gate | Result | Evidence |
|---|---|---|
| Composition: 74 visual blobs byte-exact, 5891 remaining base blobs unchanged, 175 authored trees, new GUIDs resolve | **PASS** | `composition.json` |
| Offline Compact site rules | **PASS 36/36** | `offline-rules.txt` |
| Import/compile, Unity 6000.6.0f1 | **PASS** (0 `error CS`) | `import.log`, `targeted-native2.log` |
| Explicit Editor entry / scene structure gate | **PASS** `COMPACT30_PLAYTEST_ENTRY_PASS` | `targeted-native2.log` |
| Existing E2 management + negative gate, on composed candidate | **PASS 71/71** `COMPACT30_E2_VERIFY_PASS checks=71` | `targeted-native2.log` |
| GUI: CCF → Player Test → Play Compact30 opens no-rack Compact, 175 Sitka bound, 36-cell world | **PASS** | `captures/player-test-*.png`, `gui-play-start.png`, log line `175 living Sitka bound` |
| GUI: H1 start, winding path, perimeter fence, spruce floor rendered; WASD/mouse walk/look | **PASS** (observed) | `gui-walking.png`, `gui-stopped.png` |
| GUI: arrival Y, assessment J/Y | **PASS** | `gui-arrival-assessment.png`, log |
| GUI negative: designate without stock refused | **PASS** | log `No unreserved sessile-oak-sapling stock` |
| GUI: buy 2 Oak + 2 Beech, designate O1/O2/B1/B2 (contractor, no shelter) | **PASS** | `nursery-purchase.png`, `gui-four-orders.png` |
| GUI negative: re-designating approved site refused | **PASS** | log `O1 has Approved work…` |
| GUI: approve, advance; 4 plantings COMPLETED; Annual Review 1; covering review acknowledged | **PASS** | `gui-year1-review.png`, `gui-forest-year1.png` |
| GUI: Crop Tree selection (step 6) | **FAIL — F-1** (mark registers: P0209, HUD 1/3; visual covers the world; 3 not completed) | `gui-crop-trees.png`, `gui-crop-stepback.png`, `gui-after-esc.png` |
| GUI: N deliberate no-work after T1, advance, Annual Review Year 2 consequence | **PASS** (2 planted juveniles growing, 2 lost, 1 of 4 browsed) | log, `gui-workplan-year1.png`, `gui-year2-advance.png` |
| GUI: inspect juveniles in later year (step 8) | **BLOCKED by F-1** | `gui-year2-forest.png` |
| GUI session console | No exceptions/errors after scene start | `gui-isolated.log` |
| Protected saves (`forest-save.json` ×3 incl. backup/bak) and 35 other host witnesses | **PASS** — 38/39 unchanged | `saved-files-after-final.json` |
| Shared Flatpak Editor prefs `…/unity3d/DefaultCompany/CCF/prefs` | **DEVIATION — D-1** | `saved-files-change.json` |
| Legacy40, Enlarged80, Reference Future v1, deterministic anchors, historical save runtime | **NOT RUN** | — |
| #857 AC-06 zero-byte save trace | **NOT APPLICABLE / NOT RUN** — this candidate writes no save (Compact session is in-memory only) | — |
| Windows build | **NOT RUN** (out of scope) | — |

## 5. Open defects

**F-1 (BLOCKER for the intended playthrough; not a #873 regression).** Crop Tree and Fell
mark visuals are mis-scaled in Compact30. `ForestTreeMarkingManager.CreateMarker`
(`Assets/ForestPrototype/ForestTreeMarkingManager.cs`, base code, unchanged by #873) parents
the diamond/disc/trunk-band to `tree.transform` with `SetParent(…, false)` and unit-space
local scales. The Compact30 authored Sitka roots carry the normalised prefab scale (e.g.
`P0209_Sitka_Mature_01` `m_LocalScale` 12.009895 — present already in the #718/#820 base
scene). The 1.2-unit disc therefore becomes a ~14 m solid blue sheet ~0.6 m above ground
that hides the floor and juveniles across most of the 30 m world; the diamond and band are
similarly displaced. Repro: Play Compact30 → aim at a living Sitka → C. Fell (X) uses the
same path and is expected to behave the same (not separately exercised). A fix touches base
gameplay code outside this grant — return to #872 for a scoped repair (e.g. parent markers
to a non-scaled anchor or divide by `lossyScale`). Not repaired here, per packet.

**D-1 (protected-config deviation, needs owner/Manager disposition).** During the earlier
`-executeMethod` native gate (~15:20Z, Codex part), the Editor wrote the shared Flatpak
PlayerPrefs file `/home/jer/.var/app/com.unity.UnityHub/config/unity3d/DefaultCompany/CCF/prefs`
(SHA-256 `36d5e891…` → `f64398ca…`, 2876 bytes). The GUI run was then moved to a task-local
`XDG_CONFIG_HOME` (`launch-isolated.py`) and the file did not change again (hash still
`f64398ca…` after exit). No pre-run byte copy exists, so it cannot be restored. Keys are
learning/help progress (`CCF.Learning.v1.*`, `CCF.MenuHelp.v1.*`), `UnityGraphicsQuality`,
`ToolchainAutomaticallyInstallPackage` and Unity Connect session ids; it is not a game
save. Post-run copy: `shared-prefs-after-native.xml`.

**Observations (not defects, need design review):** the toast after the first C read
"Marked P0209 as Crop Tree (0 marked)" while the HUD read 1; the four contractor plantings
cost €3.00 in total (Year 1 net −€3.00) — calibration, not judged here.

## 6. Owner launch instructions (exact candidate)

The machine-wide `unity-lock` is FREE. Do not run this alongside another Editor.

1. Unity Hub → open `/media/jer/ZX20/CCF-Sen1-873-Compact30-Playtest-20261120` (Unity 6000.6.0f1). Not an older copy.
2. If Unity offers the Linux toolchain 1.1.1 update, it will dirty `Packages/`; that is expected locally and must not be committed.
3. Menu **CCF → Player Test → Play Compact30**, then click the Game view.
4. WASD/mouse walk/look; Esc releases the mouse. Y acknowledges arrival; J opens the assessment, Y acknowledges, J closes.
5. Tab → Work Plan → Nursery: buy 2 sessile Oak + 2 European Beech. Back in the forest, 1–4 designate the four G1 sites (F2 labour, F3 shelter before each). Tab → Approve Work Plan → Advance one year.
6. Read and acknowledge the Annual Review. After that, N records a deliberate no-work year; advance and read the next review.
7. **Known blocker:** pressing C (Crop Tree) or X (Fell) on a Sitka currently fills much of the forest with a blue/red sheet (F-1). Stop and restart Play to clear it.
8. Stopping Play discards the session; nothing is saved. The historical five-rack scene is unchanged.

## 7. Evidence and next actions

Evidence: `/media/jer/Files/CCF/Verification/Sen1-873-Compact30-Playtest-20261120/` (captures/, logs, JSON witnesses, patches named above).

Next: (1) #872 decides a scoped F-1 repair grant (base `ForestTreeMarkingManager` marker scaling) and D-1 disposition; (2) separately assigned non-author, non-Codex-873/non-Claude-873 reviewer reviews `3841d4c1` + this result; (3) after F-1 repair, re-run GUI steps 6 and 8 with three Crop Trees; (4) old-world gates remain NOT RUN until separately scheduled. No main merge, no Windows build.
