# Sen-1 #667 / #668 — Compact30 complete-world content and provenance candidate

**CANDIDATE / UNSIGNED / NOT A SIGNED WORLD / NOT UNITY-VERIFIED / NO IMPLEMENTATION AUTHORITY.**
**PRE-IMPLEMENTATION / OWNER-AND-SAVE-OWNER-DECISION-READY evidence dossier. Verdict: SIGNATURE-NOT-READY.**

| Identity | Value |
| --- | --- |
| Date / author | 2026-11-17 UTC; OpenAI GPT-6.1 Sol (`gpt-6.1-sol`), reasoning effort high, T3 Code/Codex |
| Claim / parent | [#668](https://github.com/jermur788/CCF/issues/668) / [#667](https://github.com/jermur788/CCF/issues/667) |
| Base, verified before writing | `1e0fba5ffd5a07704eed3e44c34b3a960f61ac60` |
| Branch | `docs/sen-1-667-compact-world-save-evidence-20261117-v1` |
| Own path | `Docs/Handoffs/ScenarioOne-Overnight-Gates-20261117/Sen-1-667-COMPACT30-COMPLETE-WORLD-CONTENT-PROVENANCE-AND-SIGNATURE-READY-CANDIDATE-20261117.md` |
| Context-source commit | `a5541b8a193883792db290539b7a3b877b589ff2`; inspected canonical files at the dispatched base |
| Static implementation pin | The base above; `git diff 500ef7acecd39724b8ebd4cec1ebdfc00be45867 HEAD -- Assets` was empty before authoring |
| Scope | Exactly this new file and the companion CGTRIG/MAE dossier; no inherited document or source change |

The smallest change is two evidence documents. No engineering architecture is selected by writing them. The latest direct user dispatch authorises the prepared local checkout, commit and ordinary push; it is later and more specific than the earlier GitHub-only reservation. Live main coordination blob `c0edc881eef83fb4f9cbc9386a3451ddc3ce6e1d` and the actual #668 claim were read. The [pre-edit receipt](https://github.com/jermur788/CCF/issues/668#issuecomment-6934662071) records this scope; no physical Ops permission or Unity lease is inferred.

## Evidence legend and citation scope

| Label | Meaning |
| --- | --- |
| SOURCE-VERIFIED | Bytes, text or C# actually read; authentication/method below. Static C# establishes source behaviour only. |
| MODEL-DERIVED | Arithmetic on authenticated inputs; no Unity, field measurement or player observation. |
| GAME-CALIBRATION-PROPOSAL | Technically implementable candidate default; unsigned and unselected. |
| OWNER CHOICE (D-083/D-084) | Accepted intent, never proof of exact content bytes or native qualification. |
| UNSUPPORTED / NOT IN SOURCE | No complete Compact value/rule in inspected sources. A suggested default does not upgrade this label. |

Citation aliases below resolve to **exact repository path:line**, at the base unless another commit is specified. Ranges only locate the inspected evidence; they do not claim every field in a range is already implemented for Compact.

| Alias | Exact source |
| --- | --- |
| ANN | `Docs/Handoffs/ScenarioOne-Compact-Ecology-20261116/Sen-1-657-AUTHORED-COMPACT30-ROSTER-PROVENANCE-AND-OWNER-SIGNATURE-CANDIDATE-20261116.md`, fetched from GitHub at `55f790677e452805d9dedf08d243c5ba81e2aab2` |
| MAN | `Docs/Handoffs/ScenarioOne-Compact-Ecology-20261116/Sen-1-COMPACT-30M-WORLD-MANIFEST-CANDIDATE-20261116.md` |
| P / C / G | `specs/scenario-one-compact-complete-loop-v1/spec.md` / `specs/scenario-one-compact-initial-planting-v1/spec.md` / `specs/scenario-one-compact-graphics-v1/spec.md` |
| SD / SC | `Assets/ForestPrototype/ForestSaveData.cs` / `Assets/ForestPrototype/ForestSaveController.cs` |
| T / SP / ST | `Assets/ForestPrototype/ForestTree.cs` / `Assets/ForestPrototype/ForestTreeSpawner.cs` / `Assets/ForestPrototype/ForestStartingStand.cs` |
| EC / CELL / US | `Assets/ForestPrototype/ForestEcologyController.cs` / `Assets/ForestPrototype/ForestEcologyCell.cs` / `Assets/ForestPrototype/ScenarioOne/ScenarioOneUnderstorey.cs` |
| M / PV / PLAYER | `Assets/ForestPrototype/ScenarioOne/ScenarioOneManager.cs` / `Assets/ForestPrototype/ScenarioProtectionVisuals.cs` / `Assets/ForestPrototype/ForestPlayer.cs` |
| SAVE | `200add4e6e5116268b79ef63131ff9b836f14c5d:specs/scenario-independent-save-isolation-v1/spec.md`; read using `git show ...` piped to `nl -ba` |
| CB / L / EH | Raw C-B-30 / #508 ledger / executed-harness paths in the inventory below; JSON pointers supplement byte authentication |

Canonical intent: `Docs/Project/decision-log.md:139` D-079; `:143` D-081; `:146` D-082 (separate experiment); `:149` D-083; `:151` D-084. Current Milestone's Compact section preserves historical worlds and DRAFT gates. These were read alongside AgentWorkflow, Game Brief, Overview, Research Index and live coordination. The three Compact spec families, their plans/tasks/checklists and the corrected P:14–20 / 33–203 were inspected as DRAFT inputs; this is not another full checklist acceptance review.

## 1. Immutable source inventory and checks actually performed

Read-only Python `Path.read_bytes`, `json.loads` and `hashlib.sha256` were used; `sha256sum` independently reproduced the five primary raw hashes. In the #508 folder, the inspected `SHA256SUMS` names only ledger, omissions and seven Python files; `sha256sum -c SHA256SUMS` returned **9/9 OK**. None of those scripts was executed. Their inventory rows are **integrity-only byte reads**, not endorsements of their algorithms. Read times below are observed hash-read times, not inferred historical creation times.

| Source actually read | Bytes | Computed SHA-256 | UTC read time | Check / comparison with #657 |
| --- | ---: | --- | --- | --- |
| `/media/jer/Files/CCF/Verification/CompactOneRackNativeFeasibility460-20261113T145748Z/native-20261113T152528Z/C-B-30.json` | 128518 | `3e7c37ad010d66382722bbd932db6c93b9de1b78914a38c48b4f1a7e1f9d5a27` | 2026-11-17T01:10:04.639610Z | Matches #657 and manifest / ledger input |
| `/media/jer/Files/CCF/Verification/CompactOneRackNativeFeasibility460-20261113T145748Z/native-20261113T152528Z/evidence-manifest.json` | 6135 | `cd35e159c0d306f5833104e697d9657b45956559d2f52d0f42ca8a6ace6838d7` | 2026-11-17T01:10:04.654276Z | Computed; integrity/provenance check only where noted below |
| `/media/jer/Files/CCF/Verification/CompactOneRackNativeFeasibility460-20261113T145748Z/native-20261113T152528Z/harness-provenance.json` | 534 | `99a6484e45aff980a41fd4c25d605358c7a3090a8280fb8523ae6461ae36b284` | 2026-11-17T01:10:04.654788Z | Computed; integrity/provenance check only where noted below |
| `/media/jer/Files/CCF/Verification/CompactOneRackNativeFeasibility460-20261113T145748Z/native-20261113T152528Z/executed-harness.cs` | 25321 | `3c0ebae88448d27b0aae4d0345b37a28026f8c9cc3b417d13920288853019863` | 2026-11-17T01:10:04.655214Z | Computed; integrity/provenance check only where noted below |
| `/media/jer/Files/CCF/Verification/scen1-508-EntrancePathPlanting-20261114/ledger.json` | 24746 | `aa10f54f2fac9ccfa23d11a450f29985c39bf966944c665cd822b0ef49b0bcf3` | 2026-11-17T01:10:04.655355Z | Matches #657 and SHA256SUMS |
| `/media/jer/Files/CCF/Verification/scen1-508-EntrancePathPlanting-20261114/SHA256SUMS` | 754 | `f86940c9f461bfe02eabb1f1dcb9016107c9dcdf87015f52fa67d34951e11fd3` | 2026-11-17T01:11:02.999631Z | Computed; integrity/provenance check only where noted below |
| `/media/jer/Files/CCF/Verification/scen1-508-EntrancePathPlanting-20261114/scripts/explore.py` | 1457 | `a647fcb935a7a2155eb1a1fc6c2f270e19c82c4848e2dab04d4a9e5d4ea9b33a` | 2026-11-17T01:11:02.999735Z | Computed; integrity/provenance check only where noted below |
| `/media/jer/Files/CCF/Verification/scen1-508-EntrancePathPlanting-20261114/scripts/groups.py` | 1087 | `6e2f552f3d5a38f8c686515dda6a94c8b5f5f0e857a0ef14d442e6006bba6273` | 2026-11-17T01:11:02.999797Z | Computed; integrity/provenance check only where noted below |
| `/media/jer/Files/CCF/Verification/scen1-508-EntrancePathPlanting-20261114/scripts/ledger.py` | 9318 | `deffea859d4b2427778810beb28a4e8807d376661a9f702e14a4e4bdee71a5ab` | 2026-11-17T01:11:02.999879Z | Computed; integrity/provenance check only where noted below |
| `/media/jer/Files/CCF/Verification/scen1-508-EntrancePathPlanting-20261114/scripts/lightgroups.py` | 1600 | `af2499963ad3496e5505a1a72f7356c397d9750c43ee12cbf23272e2f9ecba94` | 2026-11-17T01:11:02.999933Z | Computed; integrity/provenance check only where noted below |
| `/media/jer/Files/CCF/Verification/scen1-508-EntrancePathPlanting-20261114/scripts/recon.py` | 2089 | `8d36154632aaebf13b08d4f61d14b2372a4166bbbf45b6a10f2706cfa6b4048d` | 2026-11-17T01:11:02.999984Z | Computed; integrity/provenance check only where noted below |
| `/media/jer/Files/CCF/Verification/scen1-508-EntrancePathPlanting-20261114/scripts/sites.py` | 1468 | `c7df2ac7590cf6f4397f4df8d86c616980580ff36774d5cc0bfc73c3127c7d6e` | 2026-11-17T01:11:03.000033Z | Computed; integrity/provenance check only where noted below |
| `/media/jer/Files/CCF/Verification/scen1-508-EntrancePathPlanting-20261114/scripts/slide.py` | 554 | `8bf70fe6fd5572bc41fb1c9e5804f29d4ad1ff36eebf36d0b3c6841f822751b7` | 2026-11-17T01:11:03.000129Z | Computed; integrity/provenance check only where noted below |
| `/media/jer/Files/CCF/Verification/scen1-508-EntrancePathPlanting-20261114/omissions-2.0.json` | 3316 | `dcc5a53ed6bdf41e1224c36c962560d57d7a88a744a26cfdde25ecc4ce6bd2ab` | 2026-11-17T01:11:03.000302Z | Computed; integrity/provenance check only where noted below |

**Absent input:** the dispatch's `.../native-20261113T152528Z/ledger.json` does not exist (`FileNotFoundError` during the initial read). No hash is claimed for it. The authenticated ledger is the separate #508 path above, exactly the path ANN:25 identifies. No substitute historical save/profile/Ops file was opened. This missing duplicate path does not prevent the derivation from CB plus L.

The manifest's `C-B-30.json` entry equals the computed CB hash. L `/inputs/C-B-30.json` equals it too. The EH hash equals `/executed_harness_sha256` in `harness-provenance.json`, whose `matches_launch_hash` is true. That provenance file explicitly says later seed instrumentation was **not recompiled** in the historical run: seed initialization cannot be promoted to a directly measured seed CSV. The recorded bool is provenance text, not a fresh execution witness. ANN:24–35 records the matching primary values; it does not give full independent file hashes for every supplementary row, so those comparisons are **not applicable**, not invented matches.

| GitHub canonical source / derived artifact | Bytes / rows | Computed SHA-256 | UTC read / method |
| --- | --- | --- | --- |
| [Full ANN text at frozen #657](https://github.com/jermur788/CCF/blob/55f790677e452805d9dedf08d243c5ba81e2aab2/Docs/Handoffs/ScenarioOne-Compact-Ecology-20261116/Sen-1-657-AUTHORED-COMPACT30-ROSTER-PROVENANCE-AND-OWNER-SIGNATURE-CANDIDATE-20261116.md), Git blob `1ecf429cc7d759146d8481bd7c2567b3a4bbc75d` | 32346 bytes | `10bbc4464c5a183185c68a4946b49e03726dae9a9e272324f5ef7c42f2a998bd` | 2026-11-17T01:10:49.495583Z; connected GitHub `fetch_file`, then re-read returned UTF-8 text and extracted its fenced roster |
| ANN:89–264 canonical roster, header + 175 records | 9481 bytes / 175 | `dac06a3b7775f884fac9f8df6b95178cbaf9d912b44687082a161eaf0426cca1` | Same read; matches ANN:81 and independently generated raw-to-175 bytes |
| Source189 canonical export, reference only | 11359 bytes / 189 | `e649b2aac5df85230b71372e0977cbf723990f1aabb5881a3914855575cccfbe` | Rebuilt from authenticated CB using ANN's source header/serialization; matches ANN:82 |

Reproduction was custom read-only arithmetic, not execution of the archived harness. Extract exactly ANN §5's first `text` fence, preserving header and final LF. Check UTF-8/no BOM/LF, eight columns, strictly ASCII-ascending IDs and 175 unique IDs. Independently generate the same bytes from CB after the H1 test: x/z fixed three decimals, DBH/height two, crown three, integer age, comma separators, exact ANN:89 header, one final LF (ANN:71–77). The generated and fetched roster byte strings were **equal**. Hashing the whole Markdown is a separate operation from hashing that block.

**What this proves:** identity of the printed tree roster and its stated raw derivation, including precise rounding. **What it does not prove:** a terrain, scene, prefab/collider set, initial ecology, whole-world key, content-owner approval, new generator equivalence, post-H1 survival or a playable no-rack world. No full-world SHA-256 exists here.

## 2. Recomputed 189 → 175 audit

CB `/geometryClass` is `TEMPORARY RUNTIME DERIVATIVE: NOT A SAVE/PRODUCTION MODEL`; `/generatorMethod` names the unmodified legacy lattice generator with scratch instance inputs. Actual CB values: size 30, lattice 16, target 189, seed 20260914, cells 36, rackZ −0.25, rackWidth **2.4000000953674316** (float representation of the reported 2.4). EH:127–145 shows the temporary setup; it is not an authored Compact profile.

The 189 unique `/trees` rows are all `sitka-spruce`, age 20; labels north 99 / south 71 / rack 19. Summing raw `ba` / `volume` gives **3.6501191814 m² / 22.2125049420 m³** (display rounded 3.6501 / 22.2125). These are source model totals, not ecological field observations.

Membership is **CB rows minus exactly those whose raw x/z lie in inclusive H1 rectangle x=[−9,3], z=[−15,−5]**. The 14 resulting IDs equal L `/H1/inside`:

`P0004 P0005 P0006 P0207 P0306 P0307 P0403 P0404 P0405 P0406 P0407 P0503 P0505 P0507`.

All 14 have source label south; no rack-labelled row is omitted. Outside-H1 Euclidean distance to that rectangle ≤1m returns exactly these eight retained buffer IDs, equal L `/H1/buffer1m`:

`P0003 P0009 P0103 P0109 P0209 P0504 P0506 P0508`.

The 175 retained rows have raw BA **3.3540142137 m²**, volume **20.3413430341 m³**. These totals precede rounding of the authored roster, and are not promised recomputed totals after native import. The two harness crop picks do not become authored marks (ANN:40 / 67; gap W03).

EH:151 defines `InRack`; EH:162 assigns `side` **after** generation using `abs(z-rackZ) <= rackWidth/2` (and the stand x bound). It is a position label, not a generator exclusion. The full 19 are retained as ordinary living Sitka; values below are roster-precision x/z from authenticated CB:

| Retained source ID | x (m) | z (m) | Evidence class |
| --- | ---: | ---: | --- |
| P0700 | -14.400 | -0.716 | SOURCE-VERIFIED CB `/trees`; rounding MODEL-DERIVED |
| P0701 | -12.794 | -1.150 | SOURCE-VERIFIED CB `/trees`; rounding MODEL-DERIVED |
| P0702 | -11.312 | -0.677 | SOURCE-VERIFIED CB `/trees`; rounding MODEL-DERIVED |
| P0703 | -8.928 | -1.111 | SOURCE-VERIFIED CB `/trees`; rounding MODEL-DERIVED |
| P0704 | -7.446 | -0.638 | SOURCE-VERIFIED CB `/trees`; rounding MODEL-DERIVED |
| P0705 | -5.062 | -1.069 | SOURCE-VERIFIED CB `/trees`; rounding MODEL-DERIVED |
| P0706 | -2.677 | -0.599 | SOURCE-VERIFIED CB `/trees`; rounding MODEL-DERIVED |
| P0707 | -1.196 | -1.030 | SOURCE-VERIFIED CB `/trees`; rounding MODEL-DERIVED |
| P0710 | 4.790 | -1.076 | SOURCE-VERIFIED CB `/trees`; rounding MODEL-DERIVED |
| P0711 | 7.309 | -0.645 | SOURCE-VERIFIED CB `/trees`; rounding MODEL-DERIVED |
| P0712 | 8.924 | -1.118 | SOURCE-VERIFIED CB `/trees`; rounding MODEL-DERIVED |
| P0713 | 11.443 | -0.684 | SOURCE-VERIFIED CB `/trees`; rounding MODEL-DERIVED |
| P0714 | 13.425 | -0.998 | SOURCE-VERIFIED CB `/trees`; rounding MODEL-DERIVED |
| P0715 | 14.400 | -0.568 | SOURCE-VERIFIED CB `/trees`; rounding MODEL-DERIVED |
| P0800 | -14.400 | 0.861 | SOURCE-VERIFIED CB `/trees`; rounding MODEL-DERIVED |
| P0802 | -11.231 | 0.822 | SOURCE-VERIFIED CB `/trees`; rounding MODEL-DERIVED |
| P0804 | -7.097 | 0.783 | SOURCE-VERIFIED CB `/trees`; rounding MODEL-DERIVED |
| P0806 | -2.963 | 0.744 | SOURCE-VERIFIED CB `/trees`; rounding MODEL-DERIVED |
| P0815 | 14.400 | 0.896 | SOURCE-VERIFIED CB `/trees`; rounding MODEL-DERIVED |

Near G1, P0707 (−1.196,−1.030) remains nearest O1; P0710 (4.790,−1.076) remains nearest B1/B2. L `/H1/sites` and ANN:52–57 corroborate this; deleting the 19 would change competition and light. **No historical 52-plank/rack object, approach clone, Year-1 access clearance, Fell event, timber sale, H1 revenue or grant travels into the proposed new world.** This is the D-083/D-084 boundary (P:24–35 / 128–132), not a claim that a scene has been authored.

Raw source `/anchors` has 113 strings: 104 radius 1.5m and nine radius 2.2m. L `/track` records 52 planks / 27.5202551894m, from (−0.928509,−14.75) to (−2.714831,10.75). These explain inherited empty regions; they are provenance, never new Compact objects. ANN:35's reported 37 geometry / 30 mortality lattice-slot split is **AUTHOR-REPORTED (#508/#657), not independently re-derived here**; no rerun of the generator was performed or needed to adopt explicit rows.

## 3. Field-by-field world manifest matrix

Every row has an evidence class. **These are field requirements and gaps, not invented new JSON keys.** Existing field spellings are SD/SC source names; plain-language rows name content that must be supplied. A SOURCE-VERIFIED roster value remains UNSIGNED. An UNSUPPORTED row stays unsupported even when a candidate is provided in §5. W01…W14 refer to the finite signature blockers below.

<!-- FIELD-MATRIX-BEGIN -->
| Field | Required value / rule | Evidence class | Source (alias:path:line or authenticated byte check) | Gap / owner choice |
| --- | --- | --- | --- | --- |
| tree.id | Exact ANN roster column `id`; 175 unique, ordered IDs | SOURCE-VERIFIED | ANN:67–89 / 90–264; CB → roster byte equality | Roster only; exact content signature pending W14 |
| tree.species | Exact ANN roster column `species`; sitka-spruce only | SOURCE-VERIFIED | ANN:67–89 / 90–264; CB → roster byte equality | Roster only; exact content signature pending W14 |
| tree.x | Exact ANN roster column `x_m`; metres, three fixed decimals | SOURCE-VERIFIED | ANN:67–89 / 90–264; CB → roster byte equality | Roster only; exact content signature pending W14 |
| tree.z | Exact ANN roster column `z_m`; metres, three fixed decimals | SOURCE-VERIFIED | ANN:67–89 / 90–264; CB → roster byte equality | Roster only; exact content signature pending W14 |
| tree.age | Exact ANN roster column `age_y`; 20 years | SOURCE-VERIFIED | ANN:67–89 / 90–264; CB → roster byte equality | Roster only; exact content signature pending W14 |
| tree.DBH | Exact ANN roster column `dbh_cm`; centimetres, two fixed decimals | SOURCE-VERIFIED | ANN:67–89 / 90–264; CB → roster byte equality | Roster only; exact content signature pending W14 |
| tree.height | Exact ANN roster column `height_m`; metres, two fixed decimals | SOURCE-VERIFIED | ANN:67–89 / 90–264; CB → roster byte equality | Roster only; exact content signature pending W14 |
| tree.crown radius | Exact ANN roster column `crown_m`; metres, three fixed decimals | SOURCE-VERIFIED | ANN:67–89 / 90–264; CB → roster byte equality | Roster only; exact content signature pending W14 |
| tree.y / terrain rule | No y in raw roster; must bind ground height consistently for every tree | UNSUPPORTED / NOT IN SOURCE | ANN:301–305; SD:211; ST:113–116 source flat y=0 | W01 |
| tree.prefab / LOD variant | No per-tree visual identity in CB; pin reusable asset set and deterministic variant rule | UNSUPPORTED / NOT IN SOURCE | ANN:304; T:44–64; SP:164–176 | W02 / W13 |
| tree.orientation / yaw | CB has none; source spawner root uses Quaternion.identity | UNSUPPORTED / NOT IN SOURCE | SP:145–148; ANN:304 | W02; identity rotation is a candidate only |
| tree.visual scale / defect flags | No per-tree Compact assignment; biology must remain truthful | UNSUPPORTED / NOT IN SOURCE | T:44–64; ANN:304 | W02; do not create biological defects from art |
| tree.life stage (`stage`) | Engine enum Mature=0 exists; it is not proof of biological maturity at age20 | UNSUPPORTED / NOT IN SOURCE | T:5–10 / 72 / 135–139; SD:191 | W03; candidate fresh-source engine stage |
| tree.status | Standing/living intended for all 175; no separate saved status field should be invented | GAME-CALIBRATION-PROPOSAL | ANN:65–69; T:81–87; SD:185–212 | W03; encode through actual stage/mortality fields |
| tree.suppression class | No authored suppression category; runtime competition is not a recorded pre-world history | UNSUPPORTED / NOT IN SOURCE | T:102–118; EC:52 / 932; SD:198 | W03; do not infer past suppression from small DBH |
| tree.harvest / CropTree mark (`markType`) | Propose None=0, no marks inherited from harness crop picks | GAME-CALIBRATION-PROPOSAL | T:18–23; ANN:40 / 67; SD:206 | W03; both marked ID lists must agree |
| tree.mortality (`biologicallyDead`) | Propose false; no deaths or H1 stumps silently inserted | GAME-CALIBRATION-PROPOSAL | T:32–34 / 81–82; SD:208 | W03 / W05 |
| tree.mortalityCause | Propose empty string, matching fresh source field default | GAME-CALIBRATION-PROPOSAL | T:33; SD:209 | W03 |
| tree.mortalityYear | Propose −1, matching fresh source default | GAME-CALIBRATION-PROPOSAL | T:34; SD:210 | W03 |
| tree.stageTimer | Not in CB; source fresh private float default is zero | UNSUPPORTED / NOT IN SOURCE | T:72–75; SD:192; SC:143–145 | W03; candidate zero requires full fresh-state rule |
| tree.chopProgress | Not in CB; no prior player chops proposed | UNSUPPORTED / NOT IN SOURCE | T:73; SD:193; SC:145 | W03; candidate zero |
| tree.hasSimulation | Source capture writes true; Compact construction still absent | GAME-CALIBRATION-PROPOSAL | SC:146; SD:195 | W03; do not treat DTO initializer as measured state |
| tree.equivalentSuppressedYears | Not in CB; candidate zero recorded history at new-world start | GAME-CALIBRATION-PROPOSAL | T:102–118; SD:198 | W03; no claim that real trees were never suppressed |
| tree.pruningLifts | No inherited pruning record; candidate fresh unpruned state | UNSUPPORTED / NOT IN SOURCE | SD:203; T:121–128 | W03 |
| tree.crownBaseHeightM | Must use source initialization rule or explicitly authored value; no invented per-tree heights | UNSUPPORTED / NOT IN SOURCE | SD:204; SC:152–154; T:124–128 | W03 |
| tree.lastPruningYear | Candidate −1 from existing DTO default; no prior pruning credit | GAME-CALIBRATION-PROPOSAL | SD:205 | W03 |
| tree.ordering / determinism | Roster ASCII id ascending; UTF-8 LF canonical decimals and final LF | SOURCE-VERIFIED | ANN:71–77; canonical byte check §1 | Construction/restore order must be witnessed; W14 |
| terrain.height source | No terrain bytes or Compact heightfield authored | UNSUPPORTED / NOT IN SOURCE | ANN:301–303; ST:113–116; PV:85–87 fallback | W01; flat source-plane candidate or separate heightfield |
| terrain.normal / up vector | No normals in CB; flat-plane up is an explicit candidate | UNSUPPORTED / NOT IN SOURCE | ANN:303; PLAYER:447–451 tests surface normal | W01; never infer slope from x/z |
| terrain.surface / material | No exact Compact surface asset/material binding | UNSUPPORTED / NOT IN SOURCE | G:63–84 (FR-03/13/16/18); ANN:309–313 | W05 / W13 |
| terrain.bounds | 30×30m; x/z [−15,+15], 900m²; no imaginary external forest donors | OWNER CHOICE (D-083/D-084) | D-083 decision-log:149; ANN:290; CB /size | Grid edge admission still needs W04 |
| terrain.collider | No Compact collider mesh/rules signed | UNSUPPORTED / NOT IN SOURCE | ANN:303 / 309; G:68–70 | W01 / W07; visual ribbon is not movement proof |
| cell.occupancy | 36 counts from retained roster and floor-grid, listed below; not canopy percentages | MODEL-DERIVED | CB `/trees`; L `/H1/rect`; ANN:60 / 293 | All19 retained; does not initialise ecology W04 |
| cell.ground-flora proxies | mosses, ferns, grasses, forbs, shrubs, fungi all require initial values/rule | UNSUPPORTED / NOT IN SOURCE | US:7–19 / 27–51; ANN:307 | W04; no botanical decimals copied from art |
| cell.brambleCover | Missing Compact initial 36 values; independent competition state | UNSUPPORTED / NOT IN SOURCE | US:17–19; SD:67; ANN:307 | W04; pin source Model2 initialization or author grid |
| cell.brackenCover | Missing Compact initial 36 values; not inferred from visible fern meshes | UNSUPPORTED / NOT IN SOURCE | US:17–19; ANN:307 | W04 |
| cell.seed state / cohorts | Seed rain is runtime derived, not a persisted seed bank; no source supports inventing a bank field | UNSUPPORTED / NOT IN SOURCE | CELL:91–98; SD:245–271; provenance seed-instrumentation caveat §1 | W04; initialise cohorts and pin deterministic derived seed recipe |
| cell.browse state | No Compact 36-cell browse-state export; current definition default pressure0.2 is game calibration only | UNSUPPORTED / NOT IN SOURCE | ScenarioOne/ScenarioOneDefinition.cs:58 / 135 under Assets/ForestPrototype; ANN:307 | W04; pin background/protection recipe, no fake per-cell counts |
| cell.deadwood state | No full Compact initial deadwood record inventory; no H1 felling implied | UNSUPPORTED / NOT IN SOURCE | SD:68–69; ANN:307 | W04; empty-record candidate is not historical evidence |
| cell.canopy / light base | Only static ledger proxy at selected sites and changed-cell entries; not a full native Compact grid | UNSUPPORTED / NOT IN SOURCE | L `/H1/sites`, `/H1/canopyCellsChanged`; EC:914–918 | W04; cell15 canopy0.7789 ⇒ light≈0.221 is a proxy |
| cell.coordinate formula | floor((z+15)/5)*6 + floor((x+15)/5); 6×6 of 5m; G1 →15 | MODEL-DERIVED | ANN:60 / 291; L `/H1/sites`; arithmetic §4 | W04 must settle exact boundary dispatch; +15 needs explicit outside rule |
| cell.per-cell initial values | See all36 rows below; ecology unsupported even where adult occupancy is zero | UNSUPPORTED / NOT IN SOURCE | ANN:307; SD:245–271; US:7–19 | W04 |
| cell.site drivers / opening / suitability | Complete initial SiteProductivity, SoilStability, recentOpening, establishmentSuitability recipe absent | UNSUPPORTED / NOT IN SOURCE | CELL:47–50; SD:255–259 | W04; source defaults are candidates, not measured soil |
| H1.surfaces / edge / stump depiction | No Compact surface or stump-history asset selected; no fresh stumps is only a draft default | UNSUPPORTED / NOT IN SOURCE | ANN:310; G:149 (GQ-05) | W05 |
| H1.spawn pose | No exact xyz/yaw or camera pose in CB/ANN | UNSUPPORTED / NOT IN SOURCE | ANN:310; MAN:80 | W07; do not recycle originalPlayerPosition without a value |
| H1.visitor / parking staging | Visitors/cars separated from timber collection; exact placements/capacity unknown | OWNER CHOICE (D-083/D-084) | MAN:35; C:91–108; D-083:149 | W06; no invented parking count |
| H1.collection area | Occasional small-truck CLOSED-yard staging; no literal driving/road certification | OWNER CHOICE (D-083/D-084) | C:100 / 108; MAN:35 | W06; exact footprint and state controls absent |
| H1.clearing geometry | Exact proposed x=[−9,3], z=[−15,−5];120m²; 14 omissions before Year0 | MODEL-DERIVED | L `/H1/rect`, `/H1/inside`; §2 audit | W05 / W14; approximate concept accepted, exact bytes unsigned |
| H1.outside buffer | Eight listed retained trees within1m; no extra omission | MODEL-DERIVED | L `/H1/buffer1m`; raw rectangle-distance check §2 | W05; local hazard returns to owner, never auto-erases trees |
| H1.must NOT contain | No Year1 harvest/revenue, grant, rack, footpath truck, public-road loading or simultaneous visitors/collection | OWNER CHOICE (D-083/D-084) | D-083:149; D-084:151; P:128–132; C:108 | W06 native negative later |
| footpath.anchor A0 | (2.99, -4.75) x/z metres, this exact order | SOURCE-VERIFIED | L `/H1/path/samples`; ANN:294 | W08 y/projection and geometry still unsigned |
| footpath.anchor A1 | (2.80, -2.25) x/z metres, this exact order | SOURCE-VERIFIED | L `/H1/path/samples`; ANN:294 | W08 y/projection and geometry still unsigned |
| footpath.anchor A2 | (2.06, 0.25) x/z metres, this exact order | SOURCE-VERIFIED | L `/H1/path/samples`; ANN:294 | W08 y/projection and geometry still unsigned |
| footpath.anchor A3 | (0.90, 2.75) x/z metres, this exact order | SOURCE-VERIFIED | L `/H1/path/samples`; ANN:294 | W08 y/projection and geometry still unsigned |
| footpath.anchor A4 | (-0.43, 5.25) x/z metres, this exact order | SOURCE-VERIFIED | L `/H1/path/samples`; ANN:294 | W08 y/projection and geometry still unsigned |
| footpath.anchor A5 | (-1.68, 7.75) x/z metres, this exact order | SOURCE-VERIFIED | L `/H1/path/samples`; ANN:294 | W08 y/projection and geometry still unsigned |
| footpath.anchor A6 | (-2.59, 10.25) x/z metres, this exact order | SOURCE-VERIFIED | L `/H1/path/samples`; ANN:294 | W08 y/projection and geometry still unsigned |
| footpath.anchor A7 | (-2.71, 10.75) x/z metres, this exact order | SOURCE-VERIFIED | L `/H1/path/samples`; ANN:294 | W08 y/projection and geometry still unsigned |
| footpath.spline / interpolation | No algorithm/tangents present; straight polyline is only the checked baseline | UNSUPPORTED / NOT IN SOURCE | ANN:294 / 308; G:68 | W08 |
| footpath.terrain projection | No height offset or normal-alignment recipe authored | UNSUPPORTED / NOT IN SOURCE | ANN:308; ST:115 / PV:85–87 are source precedents only | W01 / W08 |
| footpath.width / collider policy | 1.2m symmetric walking core; no vehicle route; actual ribbon/collider still unauthored | OWNER CHOICE (D-083/D-084) | P:28–31; G:68–70 | W08 / W07; ground collider vs ribbon needs explicit design |
| footpath.visual verge | Optional ≤0.20m per side, display-only, no collider/ecology/tree removal | OWNER CHOICE (D-083/D-084) | P:28; G:69 | W08; exact fade/material recipe absent |
| footpath.off-path refusal | No signed Compact path/H1 planting-refusal implementation or footprint recipe | UNSUPPORTED / NOT IN SOURCE | M:105–164 checks cell/stem/duplicate, not RouteA; L negativeControls are offline | W09; distinguish planting refusal from freedom to inspect off-path |
| north.A7 end behaviour | Historical empty gap beside A7 must be declared; alternatives §4, neither chosen | UNSUPPORTED / NOT IN SOURCE | ANN:309; CB `/anchors`; L `/buildables` | W10; no new gap-felling story |
| G1.O1 | sessile-oak; x=1.17, z=-0.47; floor-grid cell15; L G1 site matched by exact species/x/z | OWNER CHOICE (D-083/D-084) | P:26; C:15; L `/H1/sites`; ANN:52–55 | No pre-world juvenile; W01 / W12 / W14 |
| G1.O2 | sessile-oak; x=1.57, z=-2.73; floor-grid cell15; L G1 site matched by exact species/x/z | OWNER CHOICE (D-083/D-084) | P:26; C:15; L `/H1/sites`; ANN:52–55 | No pre-world juvenile; W01 / W12 / W14 |
| G1.B1 | beech; x=3.43, z=-0.07; floor-grid cell15; L G1 site matched by exact species/x/z | OWNER CHOICE (D-083/D-084) | P:26; C:15; L `/H1/sites`; ANN:52–55 | No pre-world juvenile; W01 / W12 / W14 |
| G1.B2 | beech; x=3.83, z=-2.33; floor-grid cell15; L G1 site matched by exact species/x/z | OWNER CHOICE (D-083/D-084) | P:26; C:15; L `/H1/sites`; ANN:52–55 | No pre-world juvenile; W01 / W12 / W14 |
| G1.source-cell association | All four in cell15, associated with their actual coordinate formula, not list order | MODEL-DERIVED | L `/H1/sites`; ANN:52–60 | W04 native GetCellIndex mapping absent |
| G1.composition | Exactly2 sessile Oak +2 European Beech by real player orders; none at Year0 | OWNER CHOICE (D-083/D-084) | D-083:149; D-084:151; C:19 | Pairing/teaching are in companion; no guaranteed survival |
| G1.site-match tolerance | No signed tolerance; candidate exact authored-site selection described §4 | UNSUPPORTED / NOT IN SOURCE | P:96 explicitly leaves tolerance to content signature; M:167–174 is duplicate radius only | W12; do not repurpose0.5m duplicate guard |
| G1.guard absent state | Optional per tree: no shelter record/visual means no shelter protection | OWNER CHOICE (D-083/D-084) | C:17 / 19; M:1908–1914; PV:64–70 | W11 / W13; absence must stay distinguishable |
| G1.guard effective state | Honest upright protection from real shelter state; complete target footprint r≤0.10m | OWNER CHOICE (D-083/D-084) | C:17; PV:11–12 / 64–70; ANN:297 | W11 actual mesh/stakes and player visibility |
| G1.guard expired state | Distinct from effective/failed by shape/text as well as colour; no efficacy invention | OWNER CHOICE (D-083/D-084) | C:17; G:20; PV:68–70 / 99–100 | W11; source currently colour-only expired cylinder |
| G1.guard failed state | Honestly damaged Compact-only representation, full visual/physical/stakes r≤0.10m | OWNER CHOICE (D-083/D-084) | P:27–30; G:20 / 79; PV:91–94 current defect | W11; current28°/+0.25X/−0.1Y pose is NOT selected Compact geometry |
| player.capsule / collider | CharacterController required; no exact Compact radius/height/step/slope/layer settings authored | UNSUPPORTED / NOT IN SOURCE | PLAYER:4; ANN:308–310; G:125 | W07; no invented capsule dimensions |
| player.core | 1.2m target is a route width, not a capsule diameter or proof of turning | OWNER CHOICE (D-083/D-084) | P:28–31; G:68 | W07 / W08 actual walking witness |
| player.verge | ≤0.20m visual-only; never enlarges collidable walking corridor | OWNER CHOICE (D-083/D-084) | P:28; G:69 | W08 |
| player.spawn | Exact position/orientation unsupported; must agree with H1 and safe controller pose | UNSUPPORTED / NOT IN SOURCE | ANN:310; PLAYER:4 | W07 |
| player.interaction reach | Source defaults3.5m ordinary /8m marking; actual Compact serialized values unbound | UNSUPPORTED / NOT IN SOURCE | PLAYER:17; Assets/ForestPrototype/ForestTreeMarkingManager.cs:10 | W07; candidate retain defaults, no visibility guarantee |
| identity.contentKey | Signed registry reserves scen1-compact-h1-g1-v1; it is not a digest of this document | SOURCE-VERIFIED | SAVE:148–160; MAN:19 | W14 authored binding unavailable |
| identity.content version / label | ANN reserved label Compact30-H1-RouteA-G1-content-v1; roster v=0-UNSIGNED only | SOURCE-VERIFIED | ANN:11 / 71 / 319–327 | W14 no new registry literal/version invented |
| identity.whole-world record / recipe | No complete canonical manifest bytes or hash recipe selected; proposed contents §6 | UNSUPPORTED / NOT IN SOURCE | ANN:312; SAVE:182 (SI-10) | W14; never substitute roster hash |
| identity.chapter / profile / world / geometry | scenario-one / o1-compact-v1 / scenario-one-compact-v1 / compact30-v1; geometry2 reserved | SOURCE-VERIFIED | SAVE:148 / 159–160; MAN:19 | W14; reservation does not activate Compact |
| identity.historical refusal | Strict selected-profile matching before mutation; no cross-slot import/fallback | SOURCE-VERIFIED | SAVE:173–182 / 188–210; D-079 / D-081 | Engineering not present at base SC:9–19 / 324–357; companion S01 |
| identity.model selectors / seed | CB seed20260914 is provenance; Compact rngModelVersion, regenerationModel, growthModel, stormModel need explicit binding | UNSUPPORTED / NOT IN SOURCE | CB /seed; EH:132–136; SD:13–32; SC:116–122 | W04 / W14; no automatic borrowing of scratch model configuration |
| identity.initial management / world objects | Pin initial cash1200000c owner intent; empty work/history/plantings and neutral rack state are proposed start rules, plus actual carried/stored wood/buildables inventories | UNSUPPORTED / NOT IN SOURCE | D-084:151; SD:11 / 33–49 / 55–109; SAVE:160 | W14 / W06; no phantom timber stock or old structures |
| identity.signable content vs engineering | Content owner signs exact authored bytes/rules only; import/controller/router/runtime proof remain engineering | GAME-CALIBRATION-PROPOSAL | ANN:329–346; SAVE:163 / 182; G:151 | W13 / W14; no approval supplied here |
<!-- FIELD-MATRIX-END -->

## 4. All 36 cells, geometry alternatives and site admissibility

Occupancy is MODEL-DERIVED from CB minus H1 using the formula in §3; sum =175. **For every row, flora, competitor covers, cohort/derived-seed initialization, browse/protection/deadwood and site drivers remain UNSUPPORTED / NOT IN SOURCE as a complete Compact initial record (W04).** Zero adult stems never means zero ecological state. `recentOpening` must not be manufactured by running a Fell for pre-world H1. No static cell15 proxy is a Year1/Year2 witness.

| Cell | Standing roster stems | Initial ecology evidence class / missing value |
| --- | ---: | --- |
| 0 | 4 | UNSUPPORTED / NOT IN SOURCE — W04 full initial ecology; occupancy MODEL-DERIVED |
| 1 | 2 | UNSUPPORTED / NOT IN SOURCE — W04 full initial ecology; occupancy MODEL-DERIVED |
| 2 | 0 | UNSUPPORTED / NOT IN SOURCE — W04 full initial ecology; occupancy MODEL-DERIVED |
| 3 | 3 | UNSUPPORTED / NOT IN SOURCE — W04 full initial ecology; occupancy MODEL-DERIVED |
| 4 | 7 | UNSUPPORTED / NOT IN SOURCE — W04 full initial ecology; occupancy MODEL-DERIVED |
| 5 | 5 | UNSUPPORTED / NOT IN SOURCE — W04 full initial ecology; occupancy MODEL-DERIVED |
| 6 | 6 | UNSUPPORTED / NOT IN SOURCE — W04 full initial ecology; occupancy MODEL-DERIVED |
| 7 | 0 | UNSUPPORTED / NOT IN SOURCE — W04 full initial ecology; occupancy MODEL-DERIVED |
| 8 | 0 | UNSUPPORTED / NOT IN SOURCE — W04 full initial ecology; occupancy MODEL-DERIVED |
| 9 | 1 | UNSUPPORTED / NOT IN SOURCE — W04 full initial ecology; occupancy MODEL-DERIVED |
| 10 | 7 | UNSUPPORTED / NOT IN SOURCE — W04 full initial ecology; occupancy MODEL-DERIVED |
| 11 | 7 | UNSUPPORTED / NOT IN SOURCE — W04 full initial ecology; occupancy MODEL-DERIVED |
| 12 | 8 | UNSUPPORTED / NOT IN SOURCE — W04 full initial ecology; occupancy MODEL-DERIVED |
| 13 | 5 | UNSUPPORTED / NOT IN SOURCE — W04 full initial ecology; occupancy MODEL-DERIVED |
| 14 | 3 | UNSUPPORTED / NOT IN SOURCE — W04 full initial ecology; occupancy MODEL-DERIVED |
| 15 | 2 | UNSUPPORTED / NOT IN SOURCE — W04 full initial ecology; occupancy MODEL-DERIVED |
| 16 | 3 | UNSUPPORTED / NOT IN SOURCE — W04 full initial ecology; occupancy MODEL-DERIVED |
| 17 | 8 | UNSUPPORTED / NOT IN SOURCE — W04 full initial ecology; occupancy MODEL-DERIVED |
| 18 | 7 | UNSUPPORTED / NOT IN SOURCE — W04 full initial ecology; occupancy MODEL-DERIVED |
| 19 | 6 | UNSUPPORTED / NOT IN SOURCE — W04 full initial ecology; occupancy MODEL-DERIVED |
| 20 | 6 | UNSUPPORTED / NOT IN SOURCE — W04 full initial ecology; occupancy MODEL-DERIVED |
| 21 | 4 | UNSUPPORTED / NOT IN SOURCE — W04 full initial ecology; occupancy MODEL-DERIVED |
| 22 | 6 | UNSUPPORTED / NOT IN SOURCE — W04 full initial ecology; occupancy MODEL-DERIVED |
| 23 | 7 | UNSUPPORTED / NOT IN SOURCE — W04 full initial ecology; occupancy MODEL-DERIVED |
| 24 | 8 | UNSUPPORTED / NOT IN SOURCE — W04 full initial ecology; occupancy MODEL-DERIVED |
| 25 | 5 | UNSUPPORTED / NOT IN SOURCE — W04 full initial ecology; occupancy MODEL-DERIVED |
| 26 | 2 | UNSUPPORTED / NOT IN SOURCE — W04 full initial ecology; occupancy MODEL-DERIVED |
| 27 | 3 | UNSUPPORTED / NOT IN SOURCE — W04 full initial ecology; occupancy MODEL-DERIVED |
| 28 | 7 | UNSUPPORTED / NOT IN SOURCE — W04 full initial ecology; occupancy MODEL-DERIVED |
| 29 | 8 | UNSUPPORTED / NOT IN SOURCE — W04 full initial ecology; occupancy MODEL-DERIVED |
| 30 | 9 | UNSUPPORTED / NOT IN SOURCE — W04 full initial ecology; occupancy MODEL-DERIVED |
| 31 | 5 | UNSUPPORTED / NOT IN SOURCE — W04 full initial ecology; occupancy MODEL-DERIVED |
| 32 | 2 | UNSUPPORTED / NOT IN SOURCE — W04 full initial ecology; occupancy MODEL-DERIVED |
| 33 | 4 | UNSUPPORTED / NOT IN SOURCE — W04 full initial ecology; occupancy MODEL-DERIVED |
| 34 | 6 | UNSUPPORTED / NOT IN SOURCE — W04 full initial ecology; occupancy MODEL-DERIVED |
| 35 | 9 | UNSUPPORTED / NOT IN SOURCE — W04 full initial ecology; occupancy MODEL-DERIVED |

### A7 north terminus — explicit alternatives only, no owner decision

1. **Candidate A — declared existing open gap and path end at A7.** Keep all roster stems, no imported historical buildables, new felling or extra planting. End the route visibly at its existing last anchor with a modest authored end treatment. The gap stays honestly visible as inherited authored layout; teaching can compare canopy/light at the gap without claiming player treatment. Simplest geometry, but an unexplained-looking gap needs text/visual clarity. Exact end cap, surface and sightline require W08/W10.
2. **Candidate B — declared observation/viewpoint use of the same gap at A7.** Preserve the same roster and A7 point, author a clearly bounded observation stop in the already open area without a new building, extra clearance or vehicle access. It gives a deliberate reason to reach the north end and can teach retained-canopy comparison; it adds visual content and may distract from G1 or imply amenities unless restrained. No dimensions, additional coordinates, asset IDs or work credit are assumed. Content owner must supply exact footprint/material; source and native witnesses must prove no roster change and honest visibility.

Both are **GAME-CALIBRATION-PROPOSALS** grounded in ANN:309 / L `/buildables` / raw historical clearance anchors. Neither is an ecological prediction or selected design. Refilling the gap with invented Sitka would alter the exact roster and require a different explicit content proposal; it is not a default in this dossier.

### Proposed G1 site-match rule — W12, unsigned

**Candidate default:** offer explicit selection of one of the four authored site markers, store the exact approved x/z as the ordinary order position, use that site's signed terrain y, and require exact canonical x/z equality for teaching-site membership. This is a **zero planar displacement** proposal, not an empirical tolerance. It is implementable by reusing `TryDesignateExactPlanting(itemId, position, method, installShelter)` (M:105–164) after a new site-selection control; it does not claim that control exists. A free cursor that misses the site would not gain G1 credit. Geometry calculations below would then remain about the actual selected site, rather than a silently moved one.

An alternative is a **separately authored finite planar tolerance**, accepted only after recomputing whole tolerance-disk clearance to path/verge/stems/other sites and grid edges. No unsupported radius is proposed. The source's **0.5m** rule (M:167–174) prevents duplicates, and **0.7m** (M:133–138) prevents planting near live stems; neither is a teaching-site tolerance. The present B2 verge margin is only **0.1209757m** in the flat-polyline model. A nominal0.5m tolerance would allow otherwise inadmissible displacement and is not justified. Load checks must validate signed site membership separately from the explicit order↔juvenile pairing; proximity must never manufacture that link (P:96 / 107).

### Recomputed source-plane geometry — no native PASS

Route length =sum Euclidean adjacent-anchor lengths=**16.6719614516m**. Point-to-segment distance uses clamped projection, not the historical whole track. From raw retained trees the nearest centre is **P1106,1.6004984013m**; subtract DBH/200 and half-core0.6m to obtain **+0.9194443718m** minimum trunk-edge margin. Roster-rounded coordinates give slightly different last decimals in #660; no source disagreement is concealed.

| Site | Species | x,z | Cell / association | Route distance | Core / core+verge margins for r0.10m |
| --- | --- | --- | --- | ---: | --- |
| O1 | sessile-oak | 1.17,−0.47 |15; L G1 oak matching species/x/z |1.057755|+0.358 / +0.158|
| O2 | sessile-oak | 1.57,−2.73 |15; L G1 oak matching species/x/z |1.262838|+0.563 / +0.363|
| B1 | beech | 3.43,−0.07 |15; L G1 beech matching species/x/z |1.222835|+0.523 / +0.323|
| B2 | beech | 3.83,−2.33 |15; L G1 beech matching species/x/z |1.020976|+0.321 / +0.121|

Margins =route distance−0.6−0.10, then optionally−0.20. They exclude terrain, spline deviation, mesh/stakes and capsule turning. PV:79–94 creates a1.2m-high/r0.10m cylinder, destroys its collider, and applies failed pose28° Z with +0.25m X/−0.1m Y. A source-plane continuous-cylinder envelope sampled at10,000 circumference angles at both ends gives O1 minimum route distance0.4586424m, hence **0.1413576m intrusion** into the0.6m half-core, reproducing the reported≈0.141m defect. This is MODEL-DERIVED, not a rendered mesh measurement or collision witness. The destroyed collider means this is an established visual-envelope defect, **not evidence of an actual player collision**. A new Compact damaged representation inside r≤0.10m still has to be authored, legible, measured and accepted; old-world visuals stay protected.

## 5. Finite gap register — fourteen signature blockers, each with candidate defaults

Every candidate below is **GAME-CALIBRATION-PROPOSAL**, never a selected value. Each row names the content-owner signature scope and a future native witness. Compatibility applies to Legacy40, Enlarged80, their save files and frozen Reference Future v1; none may be changed to make a candidate easier.

| Gap / needed author | Candidate default(s), source basis and reason | Compatibility / exact content signature scope | Later witness — NOT RUN |
| --- | --- | --- | --- |
| W01 Terrain author | (a) Explicit flat source plane y=0/up, reflecting ST:113–116 and PV:85–87 fallback; or (b) separately authored heightfield with pinned source bytes/transform and common sampling rule. No terrain value claimed from CB. | Compact-only height/normal/projection/collider inputs; nonflat ground changes signed positions and must bind before identity freeze. Old scenes unchanged. | Trees/path/H1/G1 grounded; slope/normal/reach and walk/collision at all anchors, exact y readback. |
| W02 Tree-presentation author | Existing first-party visual catalog with deterministic ID-based assignment and root identity rotation (SP:145–176); alternatively an explicit per-ID variant/yaw table supplied by artist. No uninspected prefab GUID assigned. | Sign exact prefab/LOD/material references, assignment rule, scales/yaws/defect meaning; do not change biology or old shared prefabs. Rights gate W13 separate. |175 IDs retain correct position/DBH/height/crown; species/stage honest at720p/1080p; reproducible variants/LOD. |
| W03 Simulation initial-state author | Capture and pin fresh-source tree state for the exact175 construction; candidate no prior chops/marks/pruning/suppression-history/death. SD:185–212 and T:72–75/102–128 define fields; crown base must follow actual initialization, not guessed0. | Sign every extra TreeSaveData initial field or a deterministic, versioned constructor recipe; source engine Mature is not biological adulthood. No historical-state backfill. |Same initial records/counters in two constructions and save/reload; no H1/Fell history or crop flags inherited. |
| W04 Ecology/source-model author | Derive all36 initial cells with pinned installed model/species/source parameters after explicit H1 omission, using actual initialization including US:47–51; or supply a reviewed complete36-cell table. Runtime seed rain remains derived (CELL:91–98), not a new saved bank. | Sign model selectors/seed/site drivers/cohorts/competitor/understorey/browse/protection/deadwood start rules and grid-edge policy. Raw scratch selectors are not automatically Compact defaults. |36 complete cells;175 occupancy; cell15 true evolving light/browse/competition, real G1 outcomes including loss; deterministic reload. |
| W05 H1 content author | Explicit pre-existing flat apron bounded by L rectangle with edge transition and no fresh stumps; alternatively source-authored old-stump context with provenance supplied, not player felling. GQ-05 is undecided (G:149). | Sign surface/material/edge/stump bytes and exact omitted/buffer IDs; no new omission, revenue or rewritten historical stand. |View from spawn and path mouth; all8 buffers still present; no footprint overlap/extra removal. |
| W06 H1 staging/product author | Minimal visitor mode and mutually exclusive closed collection mode, with vehicles omitted until separately authored; alternatively approved stage-only small-truck/car props with measured envelopes. C:100/108 supports simple staging, not capacity or safety certification. | Sign mode rules and exact content when supplied; never a real road/rights/licence approval, invented car count or public-road loading. |Players/visitors absent during collection; no path vehicle, fake money or hidden timber grants. |
| W07 Player/navigation author | Reuse ForestPlayer and CharacterController with an explicit H1 pose/settings record; use source3.5m interaction /8m marking defaults as candidates (PLAYER:17; marking manager:10). Exact safe pose/controller dimensions still need an author, not guessed coordinates. | Sign initial pose, capsule parameters, interaction settings and boundary behaviour; keep old controller/scene settings untouched. |Native walk/turn from H1 throughA7 to eachG1; planting/inspection reachable; no ground penetration. |
| W08 Path/geometry author | Source-screened piecewise-linear centreline at the eight anchors on the chosen ground; alternatively a deterministic smooth interpolation with supplied algorithm/tangents and measured deviation bounds. | Sign centreline/projection,1.2m core, exact visual verge≤0.20m, surfaces/ground-collider policy and end geometry. Existing margins certify neither smooth spline nor collision. |Full route at both resolutions; correct visible/core/mask footprints; every guard state/stake and capsule turning. |
| W09 Planting/control author | Explicit teaching-site selection with membership checked against signed sites, plus H1/path exclusion from new footpath geometry; or source-owned free-position admission using signed footprints. M:105–174 does not supply those Compact exclusions. | Sign which off-path/inside-H1/on-path requests refuse and visible reason. Do not accidentally prohibit walking off-path for stand assessment or change historical admission. |On-path/inside-H1/outside-world/near-stem/duplicate refusal with no order/cash/stock mutation; legitimateG1 admitted. |
| W10 North-terminus content owner | Choose A7 path-end/declared gap OR A7 observation stop, exactly the alternatives in §4. Both preserve175 roster and omit historical buildables. | Sign the intended visible gap/end footprint and teaching explanation; no implicit new felling, replacement trees or new management credit. |Player sees honest end, no phantom buildables or unreachable destination; compare roster before/after. |
| W11 Protection/visual author | Source upright envelope for effective state plus distinct expired look; failed state split/compressed within same r≤0.10m, visibly damaged (G:20). Alternative specific damaged shape must be artist-supplied, not current tilted pose. Include optional absent state and all stakes. | Sign exact meshes/footprints for absent/effective/expired/failed and text/shape distinctions; no change to efficacy/failedYear or old PV pose. |Actual geometry bounds and juvenile visibility all4 sites/all4 states;0.141m O1 defect removed in Compact only. |
| W12 Content + teaching owner | Exact authored-site selection / zero planar displacement is proposed §4; alternatively a separately justified finite tolerance, value still absent. | Sign site equality/tolerance and admissibility consequences; never confuse duplicate radius with tolerance or infer pairing. |Boundary/equality perturbations including B2 fringe; reject wrong site/species/profile with no credit. |
| W13 Overall Manager + content/rights authorities | Preferred DRAFT GQ-07 separate Compact hierarchy with no old planks/consumers; alternate shared-scene profile gating only after full source reconciliation (G:151). Reuse verified first-party assets under explicit D-032 disposition (G:147); no licence granted here. | Architecture choice must precede content signature. Pin scene inclusion/exclusion and dependency/material/source provenance. No old shared material, tree/corridor or Reference change. |No52-plank/approach/headland/rack semantics in Compact, old worlds unchanged; asset provenance, performance and native visual acceptance. |
| W14 Content owner + save owner | Full manifest covering §3 plus §6 and reference to signed reserved registry strings; no digest until every input is supplied. Candidate canonical recipe below, not a registered new key. | Sign exact complete bytes/version/binding and materiality/refusal policy; save owner approves serialization mapping. Reserved name/roster digest alone cannot bind a world. |Byte-identical construction/manifest readback; strict foreign/history/mismatched-key refusal, exact old-save/Reference anchors. |

## 6. Proposed full-world serialization and signable boundary

**UNSIGNED recipe proposal, not save-schema extension:** keep an independently canonical content artifact covering roster; complete extra per-tree start state and visual references; terrain/bounds/grid; all36 cell state/derivation inputs and model/species versions; H1 surface/staging; all path anchors/interpolation/projection/core/verge/end; four G1 site identities/species/terrain positions/match rule; all guard/stake state envelopes; player/controller/spawn/reach; initial world-object/inventory/cash/orders/history/neutral rack rules; exact dependency hashes and the signed profile/geometry/content binding. This describes manifest contents, **not new fields inserted into historical ForestSaveData or §4.2**.

Candidate canonicalization: retain ANN's exact roster block as a named input; require a documented UTF-8/LF canonical manifest representation with fixed ordering, explicit units, finite numeric encodings and no ambient defaults, plus byte digests of dependencies. Compute a whole-content SHA-256 only once that recipe and all bytes are reviewed. If a derived grid/visual assignment is used instead of explicit rows, pin its algorithm/source commit/configuration and expected initial readback; a seed alone cannot replace this. No claimed hash is invented tonight.

Use the already signed reserved `contentKey=scen1-compact-h1-g1-v1` / `geometryKey=compact30-v1` only after a separately accepted binding exists (SAVE:148–160). ANN's `Compact30-H1-RouteA-G1-content-v1` is a draft revision label, **not a registry key**, and roster `v=0-UNSIGNED` is a serialization header, **not a new payload version**. Signed SI-10 (SAVE:182) freezes persisted meaning at the **first integration to main of a writer**, irrespective of release. Later material changes require a new reviewed version/binding and explicit migration/refusal policy; display-only changes cannot silently reinterpret persisted facts.

Historical profile refusal is a signed requirement (SAVE:188–210), not a tested property here. New-profile admission must compare selected chapter/profile/world/geometry/content keys and exact schema1/payload24 before any scene/world/Prefs/file mutation, without fallback to Legacy40/Enlarged80. Genuine historical v1–v24 routes retain their accepted semantics and location; no automatic migration, copying or reserialization. The base SC:9–19 still uses a single historical filename; this dossier does not mistake the unmerged #544 save branch for implementation at this base.

**The content-owner signature would cover:** the complete finite authored contents and canonical recipe just listed, with all W01…W14 resolved and exact byte identities attached. It would not attest Unity import, collider behaviour, player visibility, ecological validity, asset redistribution rights beyond separately documented rights, save admission or runtime determinism. Engineering must implement and witness the signed interpretation. A content signature cannot choose M-A-SCOPE, change the immutable save contract, approve a DRAFT spec or release ISO-58.

## 7. Signature readiness and conflicts observed

**SIGNATURE-NOT-READY — 14 finite content blocker groups W01…W14.** Each row in §5 specifies the exact missing source/author, signature scope and witness needed; proposals do not close those blockers. No signature is solicited for an incomplete bundle. Raw authentication and roster reproduction are complete evidence tasks; no native content/source approval gate is declared passed.

Conflicts / distinctions retained:

- Dispatch's native-folder ledger path is absent; ANN:25 and actual L locate the authenticated ledger elsewhere (§1). No false hash or read time supplied for the absent path.
- L `/H1/path/lengthM`=16.6857879905 and whole-track nearest-centre1.592 describe the historical track sampling; the eight rounded source anchors yield16.6719614516 and raw retained-tree nearest1.6004984013. ANN:47–58 distinguishes these. They are different geometric objects; this dossier uses the explicit eight-anchor polyline and does not relabel it a spline.
- Earlier #657/663 one-tree recommendations remain historical proposals (P:148); the latest direct owner instruction and #665 review explicitly leave **M-A-SCOPE DEFERRED/UNDECIDED**. No quota or completion classifier is selected.
- Signed SAVE retains pre-signature banners; its external signature applies only to exact commit200add4e, not to new content or triggers. The companion quotes the boundary and authority. #660 G-1 names the missing world fields; #663 corrected teaching/pairing text and #665 accepted that changed-lines document quality only. None signs this world.

## 8. What this document does not do / remaining verification

No scene, Unity/Editor, Ops deployment or lease, Player build/Windows, production C#/YAML/.meta edit, package/binary asset, deletion, personal save/Prefs access, new save key, schema amendment, content signature, approval, DRAFT-status change, checkbox completion, main merge, force push or self-review. All175 roster rows and three spec families stay UNSIGNED/DRAFT respectively. **ISO-58/CG-TRIG BLOCKED; #652 deployment HELD.**

**NOT RUN:** compilation/typechecks, archived harnesses/scripts, native tree reconstruction/import, walking/interaction/capsule/slope, real planting/stock/cash/approval/replacement settlement, annual ecology/regeneration/survival, all guard-state rendered/physics views,720p/1080p native captures, performance, save/load/replay/crash/backup/profile refusal, genuine v1–v24 and frozen Reference regression, user playtest, Windows build, independent review, Manager acceptance, dashboard/published-state/Drive/context-mirror synchronization. The concrete future witnesses are in §5 and the companion. These are not instructions to run Unity tonight.

**Actually performed:** source/document reads, raw byte hashes and nine-entry checksum verification, canonical GitHub roster byte reproduction, offline membership/occupancy/plane geometry arithmetic, and bounded Git scope/format checks. This is an author handoff for an independent nonauthor reviewer, never a self-approved world.

**Count basis:** field matrix contains 88 field rows, of which 36 are labelled UNSUPPORTED / NOT IN SOURCE. The separate36-cell table adds36 explicitly incomplete ecology rows. Total unsupported table rows under this count = 72; unique world signature blocker groups =14 (W01…W14). The companion adds8 save/decision blocker groups S01…S08; combined dossier blocker-register entries =22, with cross-dependencies explicitly named, not22 independent runtime failures.
