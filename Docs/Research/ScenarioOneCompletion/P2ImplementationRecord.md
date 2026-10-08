# P2 — Crop Tree competitor reasoning: implementation record

**Branch:** `task/crop-tree-competitor-reasoning` · **Worktree:** `/home/jer/CCF-crop-tree-competitors` · **Base:** `origin/main` @ `1fbefd8a40c69dd90abb83f2149e24b546a184cf` (Regeneration Model 2 integrated, save v18).
**Source design:** `task/scenario-one-completion-readiness` @ `6a0e01f` (`CropTreeCompetitorDesign.md`, `CropTreeReleaseMetric.md`, `Packets/P2-CropTreeCompetitors.md`).
**Status:** implemented on the task branch; **Unity verification pending**. Sol's Unity sessions were running for the whole task, so no Unity Editor was launched (RAM rule). Verification done: a full type-check of the game assembly with Unity's bundled Roslyn, plus an offline logic check on the .NET runtime.

## Teaching intent

*Start with the tree you want to keep. Then ask which neighbours actually compete with it.* The interface describes each neighbour's contribution to the existing competition index. It never says which tree to fell. The forest-wide rule "small / suppressed / largest / highest = remove" is not encoded anywhere.

## What changed

| File | Change |
|---|---|
| `UI/CropTreeCompetition.cs` (new) | Pure, read-only breakdown of the existing Hegyi competition index into per-neighbour contributions; release preview without Fell-marked trees; Crop Tree release summary; distribution sentence; share bands |
| `UI/CompetitorAssessment.cs` (new) | Caches the breakdown for the inspected tree and the release summary. Recalculates only when the inspected tree, the marks or the ecological year change (signature checked 4× per second). Digit keys 1–5 select a listed neighbour. Temporary rings and number labels while a Crop Tree is inspected |
| `UI/TreeInspectionView.cs` | Crop Tree card: "Competitors of this Crop Tree" section in place of the felling estimate (a kept tree needs no felling estimate, and the card cannot scroll). Card widens to 440 px only for this section. Other trees: one line pointing to positive selection |
| `UI/WalkingHudView.cs` | The thinning forecast now sits on its own panel (readability audit V1), with a new Crop Tree release line |
| `UI/ScenarioOneUiRoot.cs` | Creates the assessment, updates it while walking, clears it otherwise, places labels in `LateUpdate`, destroys runtime objects on teardown |
| `UI/MenuHelpView.cs` | Tree Inspection first-use help (F1) carries the core message verbatim, plus one sentence about keys 1–5 |
| `UI/Resources/ScenarioOneUi.uss` | Competitor row, tag and forecast-panel styles |
| `.meta` | New GUIDs for the two new scripts (minimal script meta, same format as `UiKit.cs.meta`) |

Untouched: `ForestEcologyController`, `ScenarioOneManager`, `ForestTreeMarkingManager`, `ForestPlayer`, objectives, economy, save, regeneration, understorey, growth, storms, scenes, prefabs, packages.

## Authoritative competition API

- The quantity is `ForestEcologyController` competition index CI = Σ over living neighbours with horizontal distance ≤ `HegyiCutoffMeters` (8 m) of `HegyiTerm(neighbourDbh, targetDbh, distance)`. It is the same quantity DBH growth uses (`1 / (1 + CI/Ci50)`).
- The breakdown uses the same pair set as `UpdateCompetition` (living = not stump and not biologically dead, XZ position, `Vector2.Distance`, the same cutoff), sums in the same ordinal tree-id order, and calls the existing public `ForestEcologyController.HegyiTerm`. **No second competition model.**
- **Timing nuance (documented, not changed):** `GetCompetitionIndex` returns a cached value. It refreshes after any felling or death, but otherwise holds from the start of the last annual step (before that year's diameter growth). The breakdown always computes from current trees, which is the value next year's growth step will use. Between annual steps the card's existing "Competition (CI …)" stat and the section's "Competition now" can therefore differ slightly when no tree died that year. The gate reconciles against a forced recompute (`InvalidateCompetition`, harness only).

## Ranking, list size and bands

- **Ranking:** contribution descending, ties by tree id (deterministic).
- **List size: top 5 + remainder line.** Offline test on the 16 Year-0 fixture Crop Trees (Unity-exported stand):

| Shown | Median share covered | Gap between last shown and next (median) | Judgement |
|---|---|---|---|
| Top 3 | 23 % | 1 percentage point | Cuts arbitrarily: entries 3–5 are nearly equal |
| **Top 5** | **32 %** | 0 | Shows the leaders; the "n other trees together x %" line makes the long tail explicit |
| Top 8 | 44 % | 0 | Rows 6–8 are almost always Low or Moderate and add little; does not fit the card at 1280 × 720 |

- **Bands (UI abstraction, stated on the card):** share of *this tree's own* competition. **Strong ≥ 10 %**, **Moderate ≥ 5 %**, **Low < 5 %**. Shown as text plus square glyphs (■■■ / ■■ / ■) plus the percentage. Never colour alone, and no red.
- **Distribution sentence** (describes, never prescribes): "spread across many trees; no single neighbour dominates" / "a few neighbours contribute most" / "one neighbour contributes a large share".

## World correspondence

- While a **Crop Tree** card is open (and only then): a thin pale-gold ring at breast height (1.3 m) around each listed neighbour, plus a round number label (1–5) matching the list. Labels are screen-projected UI and are hidden behind the camera.
- **Keys 1–5** select a listed neighbour: its ring becomes white and thicker, its label inverts, and its row is marked ▶ in the accent colour. The same key clears the selection. Keys are ignored in planting mode (where 1–2 choose stock).
- **Why keys, not hover:** Tree Inspection exists only while the cursor is locked (`ForestPlayer` closes inspection when the mouse is released), so list rows cannot be hovered or clicked.
- Everything disappears when the card closes, on any modal screen, in the Reference preview, or when the tree is not a Crop Tree. No permanent floating numbers. Rings/labels can be switched off with `CompetitorAssessment.ShowWorldMarkers` if the manual review finds they read as instructions.

## Release preview

On the Crop Tree card: "Competition now → after your Fell marks: 8.15 → 7.56 (−7 %) (P0707 with P0607 marked)". "n Fell-marked neighbours supply x % of it. Growth held back: about 62 % now, 60 % after. Estimate if nothing else changes." With no Fell-marked neighbour: the current value plus "Marks you add with [X] show their effect here." The same index and the existing growth response are used; production growth is not altered.

HUD forecast panel (when ≥ 1 Fell mark): "Crop Trees 16: competition around them 6.09 → 4.89 (−20 %) · 16 lose at least a tenth · Fell marks within 8 m of a Crop Tree: 30 of 30 (13 near more than one)". With no Crop Trees: "Crop Trees: none chosen yet. Keep trees with [C] to see how your Fell marks affect them." **"Meaningful competitors removed" is not implemented** (rejected in the readiness study).

## Suppressed-tree case

- **Synthetic fixture (offline):** Crop Tree A (25 cm). B: 7 cm at 1.6 m, ranked 8 of 11, 8.0 %. C: 30 cm at 2.8 m, ranked 1, 20.0 %, Strong. C contributes 2.5× B. **Finding:** in this sparse 11-tree fixture B's share band reads *Moderate*, not Low. Bands are relative to the tree's own total, so a close small tree in an open stand is a moderate share. The assertion checks the requirement (B ranks low and below C), not a tuned band.
- **Real stand (Unity gate and offline):** Crop Tree P0707. P0710 (10.7 cm, suppressed, 6.0 m) ranks 43 of 48 at 1.1 %, Low. P0706 (20.5 cm, 1.5 m) ranks 1 at 7.9 %.

## Multi-Crop case

Each Fell-marked stem is counted **once**. The summary reports how many are near a Crop Tree and how many are near more than one. On the card, a listed neighbour that is also within 8 m of other Crop Trees is noted ("Also within 8 m of other Crop Trees: #2 (1)"). Offline fixture: 3 Fell marks (one shared between two Crop Trees, one beside one, one far away) → fells 3, near 2, several 1. Year 0, T2 marks: 30 fells, all near a Crop Tree, 13 near more than one.

## Performance

| Stand | Inspect (one tree) | Mark-change summary (all Crop Trees) |
|---|---|---|
| 336 stems / 16 Crop Trees | 0.02 ms | 0.3 ms |
| 1,300 / 41 | 0.04 ms | 2.1 ms |
| 3,000 / 82 | 0.08 ms | 7.9 ms |
| 5,000 / 122 | 0.14 ms | 21 ms |

Offline .NET timings, synthetic random stands (`offline-check-output.txt`; they vary run to run). Work runs only on inspection or mark/year change, never per frame. Per frame: at most 5 label projections and 5 ring updates, plus a mark signature string every 0.25 s. There is no new per-frame O(n²). (The pre-existing marking forecast's O(n²) refresh is unchanged.) The Unity gate logs `P2_PERF` on the real stand, including the scene scan.

## Verification

| Check | Mode | Result |
|---|---|---|
| Game assembly (all non-Editor sources) type-check, Unity Roslyn + Unity 6000.6 references | offline compiler | **0 errors** (one pre-existing warning in `ForestEcologyController`) |
| Same + harness with Editor defines | offline compiler | **0 errors** |
| `run_offline_check.py` | offline .NET | **P2_OFFLINE_PASS**: reconcile (1e-5), shares sum to 1, suppressed/larger ordering, outside-radius excluded, marked removal preview, unrelated removal no change, determinism, multi-crop counting, also-near count, Year-0 CI vs Unity export 327/336 within 0.01 (the 9 others: 2-dp rounding at the cutoff), P0707 top 5, T2/T5 release matching the Unity harness |
| `CropTreeCompetitorVerification` | **Unity batch: NOT RUN** | Checks 1–10 against live `ForestTree`s and `GetCompetitionIndex` |
| `CropTreeCompetitorVerification` | **Unity interactive: NOT RUN** | Card fit and tags at 1280 / 1600 / 1920, with captures |
| Regression (completion, lifecycle, Reference, Interaction, MenuTutorial, Clearance) | **NOT RUN** | Presentation-only change; must stay identical |

### Runner (preferred)

`Tools/Verification/ScenarioOneCompletion/P2/run_p2_gates.py` refuses to start if any Unity Editor is running. It stages and always removes the harness, uses an isolated config (`Build/P2/config`) and an 8 GB memory cap, and runs batch ×2 and interactive ×1, comparing determinism hashes. It writes `Build/P2/results.json`. Then run the regression suite with the existing runner:

```bash
python3 Tools/Verification/ScenarioOneCompletion/P2/run_p2_gates.py
```

```bash
python3 Tools/Verification/UnderstoreyRecruitment/run_regression.py
```

### Manual commands (equivalent; one Editor at a time)

```bash
cp Tools/Verification/ScenarioOneCompletion/P2/CropTreeCompetitorVerification.cs Assets/ForestPrototype/
```

Batch (logic gate):

```bash
XDG_CONFIG_HOME=<isolated> <Unity 6000.6.0f1>/Editor/Unity -batchmode -nographics -projectPath /home/jer/CCF-crop-tree-competitors -executeMethod CropTreeCompetitorVerification.Begin -logFile <log>
```

Interactive (layout + captures):

```bash
DISPLAY=:0 XDG_CONFIG_HOME=<isolated> CCF_ACCEPTANCE_OUTPUT=Docs/Verification/ScenarioOneP2 <Unity>/Editor/Unity -projectPath /home/jer/CCF-crop-tree-competitors -executeMethod CropTreeCompetitorVerification.Begin -logFile <log>
```

Then remove the copy and its `.meta`:

```bash
rm Assets/ForestPrototype/CropTreeCompetitorVerification.cs Assets/ForestPrototype/CropTreeCompetitorVerification.cs.meta
```

Run the batch gate twice (separate processes) and compare `P2_DETERMINISM_HASH`. Then run the regression gates and confirm the anchors are unchanged. After interactive runs, revert any Editor normalisation of `.vscode/settings.json` or `Art/SectionFive` materials.

Expected tokens: `P2_RECONCILE_PASS`, `P2_SUPPRESSED_CASE_PASS`, `P2_RELEASE_PREVIEW_PASS`, `P2_MULTI_CROP_PASS`, `P2_DETERMINISM_HASH`, `P2_NO_STATE_PASS`, `P2_SAVE_PASS`, `P2_PERF`, `P2_RENDERED_PASS` ×3 (interactive), `P2_COMPETITOR_VERIFY_PASS`.

### Manual smoke (human, after the gates)

1. New game. Inspect a tree (E): one line invites keeping it as a Crop Tree. Press C: within a moment the card shows the competitor list, five rings and labels.
2. Read the distribution sentence and bands. Press 1–5: the matching ring turns white; press again to clear.
3. Turn to a listed neighbour and press X. The card's "now → after" and the HUD forecast panel update. Press X again: the preview restores.
4. Walk away or press E: rings and labels vanish. Walk around: no leftover markers.
5. Repeat at 1280 × 720, 1600 × 900 and 1920 × 1080. Check the card fits and labels are readable over the forest.

## Overlaps

- **Sol:** none. Sol's active work (storms worktree) owns ecology/save/manager. The storm packet locks `UI/*` and `ForestTreeMarkingManager` to the UI owner. The future storm UI (W5) will edit `TreeInspectionView` after this.
- **P1 copy branch (`1a36ea9`, not on main) — not integrated here, by instruction.** `git merge-tree` (P2 HEAD × P1) reports one textual conflict: **`MenuHelpView`** (both rewrite the Tree Inspection paragraph). `TreeInspectionView` and `WalkingHudView` auto-merge, but semantically:
  - P1 adds a Crop Tree line ("Inspect its neighbours to see which really compete with it."). It would sit above P2's section lead and duplicate it. On a P2 Crop Tree card, drop P1's line or merge it into P2's lead.
  - P1 reorders the footer (`[C] Crop Tree [X] Fell … [F1] Help`). P2's Crop Tree branch returns early with its own footer. Apply P1's order and `[F1] Help` to the P2 footer too.
  - P1 reorders the marking-summary keys in `WalkingHudView` (no interaction with the forecast panel). P1 changes the forecast *string* in `ForestTreeMarkingManager` (untouched by P2). The two lines share the new panel.
  - `MenuHelpView`: keep P1's positive-selection paragraph, then add P2's core message and the keys 1–5 sentence once (avoid repeating "start with a tree worth keeping").
  - Resolve deliberately during P1-INT against final main + P2, then re-run P1's copy gate and this gate.

## Known limitations

- Unity gates and the human smoke have not run (see above).
- Screen-projected labels draw over intervening trunks. That is intended for correspondence, but needs review in dense views.
- The wind and competition labels in the stat grid are unchanged (outside P2).
- The "Competition now" vs cached stat difference between annual steps (see API timing nuance).
