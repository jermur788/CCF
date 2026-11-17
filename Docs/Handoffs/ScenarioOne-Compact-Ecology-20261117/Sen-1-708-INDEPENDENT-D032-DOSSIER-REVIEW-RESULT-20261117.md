# Sen-1 #708 — independent review of the #704 D-032 rights and binary-storage dossier

**Verdict: NEEDS CHANGES.** The dossier is careful and mostly accurate. Its evidence labels hold up, and its #676 crosswalk is correct. But it treats the repository as private when GitHub reports it as **public**, and it never records that the four selected Sitka families are already tracked as ordinary-Git binaries. Both facts change the owner choices in §4 (ST-1, ST-3, ST-4, ST-6) and §5, so the owner should not get the sheet until they are corrected. The one minor item and one nit below do not block on their own.

**Status: REVIEW ONLY.** This review does not sign D-032, grant any licence, approve storage policy, approve asset production, change canon, or touch `Assets/**`, packages, binaries, source, Unity, Ops, saves or Windows. D-032 stays **OPEN**. Manager #700 alone decides document quality.

## 1. Reviewer registration and what was reviewed

- **Actual reviewer:** Anthropic **Claude Opus 5.5** (`claude-opus-5-5`), Claude Code CLI on the owner's Linux host through the T3 Code harness. I read GitHub through `git`/`gh`, not a web browser. One writer, no delegation. Independent of the author (#704 was OpenAI GPT-6 browser) and not in the OpenAI/GPT family.
- **Registration before any write:** [#708 RUNNING receipt](https://github.com/jermur788/CCF/issues/708#issuecomment-6961828648).
- **Review branch:** `review/sen-1-708-d032-nonopenai-20261117-v1`, from `main` @ `c91fc12faa477fa34de64f0a0d03df1a0c4efacc`. This file is the only change.
- **Packet:** [`docs/sen-1-708-709-independent-review-dispatch-20261117-v1` @ `7139c195`](https://github.com/jermur788/CCF/blob/7139c195ac879e74504704d50c8d77ecbdf42b15/Docs/Handoffs/ScenarioOne-Compact-Ecology-20261117/Sen-1-708-INDEPENDENT-D032-RIGHTS-STORAGE-DOSSIER-REVIEW-PACKET-20261117.md).
- **Target:** [#704 result @ `9fb4fdc8`](https://github.com/jermur788/CCF/blob/9fb4fdc83bf3eeb9bfda87d246fb810ab8083bee/Docs/Handoffs/ScenarioOne-Compact-Ecology-20261117/Sen-1-704-D032-ASSET-RIGHTS-STORAGE-DECISION-DOSSIER-RESULT-20261117.md), 35,705 bytes, read in full.
- **Live board when I registered:** `main:Docs/Project/coordination/active-tasks.md` blob `4b7bb718…` showed #708 READY/NOT RUNNING, with #700 as the only coordination writer. I did not edit the board.

**Labels used here:** **SOURCE-BACKED** = I reproduced it myself from the pinned Git objects or the GitHub API. **AUTHOR REPORTED** = taken from a named earlier report without re-running it. **PROPOSED** = an option or wording for the owner or Manager to decide. **UNSUPPORTED** = no source supports it.

## 2. Branch isolation (packet Q1)

**SOURCE-BACKED.** `git diff --stat e371b181..9fb4fdc8` shows **one new file, 121 lines**, and nothing else. The branch has two commits on top of base `e371b181` (the #704/#705 reservation commit, which is also the merge base with `main`):

- `295b5a5f` adds the dossier;
- `9fb4fdc8` changes one line, removing the "Oak/Beach" spelling note in §6.

The [#704 RUNNING comment](https://github.com/jermur788/CCF/issues/704#issuecomment-6961273107) is timestamped 21:17:41Z. The first commit is 21:23:12Z, so registration came first. #704 is still **open**, titled `[HANDED-OFF — MANAGER REVIEW]`. **PASS.**

## 3. Findings

### F-1 — BLOCKING: the repository is public, but the dossier treats it as private

**Where:** §4 ST-1 option A ("ordinary private Git") and its "What is still required" column ("target private repo"); §4 ST-3 options (c) and (d), and the proposed default "(c)+(d) NO / UNAUTHORISED until express rights"; §5 closing paragraph ("may each still expose unlicensed source bytes **to collaborators**"); §3 matrix, the `private compiled` framing for the four Sitka rows.

**Evidence (SOURCE-BACKED, 2026-11-17):**

- `gh api repos/jermur788/CCF` returns `"private": false, "visibility": "public", "license": null`.
- At `main` `c91fc12f`, these exported binaries are tracked in ordinary Git, so anyone can read and clone them:
  - all 21 state/LOD FBX for each of Sitka Pole 01, Young 02 and Mature 01;
  - the Benchmark family's 21 FBX, its six texture PNGs (for example `Textures/Sitka_NeedleSprigs_BaseColorAlpha.png`, 4,639,024 bytes, and `Textures/Sitka_Bark_BaseColor_v2.png`, 3,813,680 bytes) and its preview PNGs.
- No `.blend` file is tracked anywhere in the tree (0 matches), and `.gitignore` excludes `/Assets/InnerverseInteractive/`, so Ultimate Nature content is not in the public tree.

**Why it matters:** The dossier presents raw FBX/PNG distribution (ST-3 c) and public distribution (ST-3 d) as future permissions, defaulting to NO. In practice the selected families' raw exports are already publicly downloadable. A repo with no licence file is not an open-source grant, and I draw no legal conclusion here. But the owner cannot make an informed ST-1/ST-3 choice without knowing about the existing exposure. Also, any ST-1 A choice ("ordinary Git") for the new #688 PNGs or #698 FBX/`.blend` would publish them publicly as soon as they are pushed to this repository. That conflicts with the dossier's own "(c)+(d) NO" default. I did not check whether the repository has always been public.

**Required change (PROPOSED wording, not a decision):**

1. Add a DIRECT SOURCE EVIDENCE statement to §2 or §3: the repository is public with no licence, and the four families' FBX/PNG exports are already publicly readable. No `.blend` masters are tracked.
2. Replace "private Git" in ST-1 with "this repository (currently public)". Add the missing choice: keep existing and new art binaries in the public repo, or move them to separate access-controlled custody, or change repository visibility. Present these as owner choices only, with no recommendation pressure.
3. In ST-3, record that (c)/(d) are already exercised in practice for existing exports. The owner may then confirm, limit or reverse this, but the dossier must not present it as never having happened.
4. Extend the §5 sentence from "to collaborators" to "to collaborators or, in a public repository, to anyone".

### F-2 — MATERIAL: the existing binary footprint is missing from the storage options

**Where:** §4 ST-1 (proposed default "A for modest, measured files"), ST-4 (budgets cover only new #688/#698 files), ST-6 ("stay ordinary Git … no `.gitattributes` change"); §2's negative `.gitattributes` evidence.

**Evidence (SOURCE-BACKED, `git ls-tree -l` at `c91fc12f`):** the four selected families are already ordinary-Git binaries:

| Family | Files | FBX/PNG bytes |
|---|---|---|
| Sitka Pole 01 | 24 | 10.66 MiB |
| Sitka Young 02 | 23 | 7.17 MiB |
| Sitka Mature 01 | 23 | 37.16 MiB |
| Sitka Mature Benchmark 01 | 34 | 52.68 MiB |
| **Total** | **104** | **≈107.7 MiB** |

Across the whole tree there are 678 FBX/PNG/JPG/TGA/PSD/EXR/TIFF/audio/font files, totalling about **1,118 MiB**. The largest blob is 25.30 MiB (`Sessile_Oak_Mature_01_Unpruned_LOD0.fbx`). No blob is 50 MiB or larger. The full tree has **zero** `.gitattributes` files at any depth, not just a missing root file, and no fetched ref contains `.gitattributes` in its history.

**Why it matters:** The dossier's proposed ceilings (≤20.5 MiB for #688, ≤3.25 MiB for #698) are labelled correctly as unapproved requests. But without this baseline, the owner cannot compare those ceilings with what the reused families already cost. The baseline also shows that ST-6 option A ("stay ordinary Git") is the status quo for about 108 MiB of selected art, not a new policy. This review does not recommend any storage class.

**Required change:** add the baseline as DIRECT SOURCE EVIDENCE to §4 ST-1/ST-4/ST-6. Strengthen §2's `.gitattributes` statement to "no `.gitattributes` anywhere in the tracked tree at `<ref>`", while keeping the existing caveat that this is not a `git lfs` installation check.

### F-3 — MINOR: source-lineage leads are left out of the matrix

**Where:** §3 rows for Mature Benchmark 01, Pole/Young/Mature, and Young 02; §4 ST-2.

**Evidence (SOURCE-BACKED):**

- The [Benchmark README](https://github.com/jermur788/CCF/blob/c91fc12faa477fa34de64f0a0d03df1a0c4efacc/Assets/ForestPrototype/Art/SitkaMatureBenchmark/Docs/README.md) describes the Benchmark as "a refinement of the mature Sitka family, with the same 26 m authoring scale as `Sitka_Mature_02`", which is a new revision of its architecture. So the Benchmark's chain of title may run through **Sitka Mature 02**, which is not one of the four selected families.
- The [CropTreePruning README](https://github.com/jermur788/CCF/blob/c91fc12faa477fa34de64f0a0d03df1a0c4efacc/Assets/ForestPrototype/Art/CropTreePruning/Docs/README.md) gives its delivery location as `/media/jer/ZX20/Unity Assets/Crop_Tree_Pruning`, and says the editable `.blend` sources hold every state. That is an author-reported lead on where the original masters are, and AGENTS.md classifies ZX20 as active-work storage, not durable custody.
- Young 02's manifest `source` field points to `Tree_Variation_Additions/…`, a different origin folder from the other two. The dossier quotes this path but does not draw the lineage point from it.

**Required change:** in each affected row, add these as AUTHOR REPORTED leads to verify, not proof. Add Sitka Mature 02 as a possible upstream input that needs its own receipt. In ST-2, note that the README places the masters on ZX20, which matters when choosing durable custody.

### N-1 — NIT: Unity documentation link

§3 and §4 cite the Japanese Unity Manual (`docs.unity3d.com/jp/current/…`). Please use the English page, [`docs.unity3d.com/Manual/assets-direct-reference.html`](https://docs.unity3d.com/Manual/assets-direct-reference.html). Both returned HTTP 200 when I checked.

## 4. Claims I checked that hold up (packet Q2–Q5)

| #704 claim (section) | My check | Grade |
|---|---|---|
| D-032 is OPEN for large first-party FBX/PNG/Blender storage, LFS undecided; D-085 OD-13 A gives a separate Compact scene, with no new LFS or dependency without approval (§1) | `main` decision-log line 78 (D-032 row) and line 153 (D-085 OD-13 A) | SOURCE-BACKED — accurate |
| The S1B RightsAudit is limited to candidate `CCF-S1-20261009-B06-b3ba9b8`: owner confirmation 2026-10-09, five embedded Ultimate Nature entries, 40 notices, no standalone source distribution, no third-party audio (§2) | `main:Docs/Verification/S1B/RightsAudit.md`, read in full | SOURCE-BACKED — accurate, and correctly not read as a Compact grant |
| Four manifest blob IDs: Pole `94e53f06…`, Young `f663cb5a…`, Mature `9448a226…`, Benchmark `2dff0740…` (§3) | `git rev-parse` at `main` matches all four exactly (last changed in `03ef176a`) | SOURCE-BACKED |
| Manifest `source` paths and unpruned LOD0 heights of 8.000 / 10.500 / 28.011 m; Benchmark ~26 m / 0.48 m DBH; bark and atlas bake `.blend` files (§3) | Read from the manifests and READMEs | SOURCE-BACKED |
| The manifests contain no creator, licence or permission fields (§2) | Pole manifest top-level keys: base, source, exports, pruning thresholds, branches, ground shift, portable textures, production status, species, visual stage, prunable flag | SOURCE-BACKED |
| Prefab GUIDs `6fc4c8b0…`, `c54cc46c…`, `b3a74f27…`, `69a172cf…` (§3) | Match #685 lines 40–43 | AUTHOR REPORTED (#685) — quoted correctly and labelled correctly |
| URP / Core / ShaderGraph 17.6.0 are `builtin` in the lock file; shader `933532a4…` and script `d0353a89…` references; "does not authenticate installed bytes" (§3, ST-5) | `packages-lock.json` (core, universal, universal-config, shadergraph all 17.6.0 `builtin`); `ProjectVersion.txt` 6000.6.0f1 (`f7f8ed4d1e24`); #685 material ledger | SOURCE-BACKED; the dossier rightly does not claim installed-package proof |
| 93/94 materials share the shader reference and 94/94 share the script reference (§3) | #685 line 225 attributes this to the published review | AUTHOR REPORTED — labelled correctly |
| #688: two original PNGs (2048² and 512²), no donor; ≤20.5 MiB is an unapproved request; old soil PNG is 1,254², 3,538,063 bytes (§3, ST-4) | #688 @ `2a625018` lines 40, 154–155, 160–164 | AUTHOR REPORTED / PROPOSED — accurate |
| The 2048² RGBA texture is 16 MiB raw (ST-4) | 2048 × 2048 × 4 bytes = 16 MiB | SOURCE-BACKED arithmetic |
| #698: `JuvenilesProtection.blend`, `build_shelter.py`, `SourceManifest.md`, three FBX, five prefabs, five texture-free URP Lit materials, donor and third-party packs excluded; 3.25 MiB total (§3, ST-4) | #698 @ `7a8e575e` lines 15–21 and 63 ("2 MiB … 1 MiB … 0.25 MiB, 3.25 MiB total before evidence") | AUTHOR REPORTED / PROPOSED — accurate |
| The ≤20.5 / ≤3.25 MiB figures are not treated as measured or approved caps (§3, ST-4) | Called "unapproved request" and "not a granted limit" in every place they appear | PASS |
| There is no root `LICENSE` or root `.gitattributes`, and this is "not an exhaustive scan" (§2) | Confirmed, and my full-tree scan strengthens it (F-2) | SOURCE-BACKED; the negative evidence is described properly |
| #676 frozen candidate: fields 0/80/8, cells 0/36/0, `world_digest=null`, `CANDIDATE/NOT_SIGNED/SIGNATURE_BLOCKED` (§7) | I parsed the JSON @ `8617587e`: `counts.fields_after` and `cells_after` match, `world_digest` is null, and `document_status` matches | SOURCE-BACKED |
| Crosswalk rows 10/12/30/44/73/74/75/83 against blockers B-W02-RIGHTS [10], B-W02-SCALE [12], B-W13-RENDER-DEPS [30, 44, 73, 74, 75], B-W14-WHOLE [83]; all stay BLOCKED, and none are closed (§7) | The `signature_blockers` list in #676 has exactly these four IDs and row sets, and #685 rows 21–28 agree. The F-2 note that a future revision should add a render blocker to rows 10/12 is attributed correctly | SOURCE-BACKED — PASS |
| LiberationSerif has its own licence, separate from any repository grant (§3) | #685 @ `75b3d8ea` line 34: "the LiberationSerif font licence is unrelated". Also checked directly: `main` tracks `Assets/ForestPrototype/UI/Resources/Fonts/LiberationSerif-Regular.ttf` and `LiberationSerif-LICENSE.txt` (`License: SIL-OFL-1.1`). Across the whole tree, the only other licence or notice file is `Tools/Build/S1B/Notices/UnityPlayer-Windows-Mono-6000.6.0f1.pdf` | AUTHOR REPORTED (#685) — attributed correctly; optional: also cite the tracked licence file |
| Ultimate Nature is not in Git and has a different rights holder (§3) | `.gitignore` excludes `/Assets/InnerverseInteractive/`; only `Docs/UltimateNatureSetup.md` is tracked | SOURCE-BACKED |
| GitHub, LFS and Unity Asset Store policy links (§4) | All eight URLs returned HTTP 200 on 2026-11-17. I checked that they resolve, not their full wording | Links resolve |
| §5 receipt contract, §6 unsigned template, §8 STOP conditions | Separate yes/no/conditions per channel and per family; the owner is never asked to grant third-party rights; marked NOT PROVIDED everywhere; no false PASS | PASS (except the §5 wording in F-1) |

**Owner sheet (Q5):** ST-1 to ST-7 give discrete choices, each row decided separately, and "no decision" keeps the gate blocked. The proposed defaults are marked PROPOSAL and are conditional on real bytes and owner decisions; none is presented as already signed off. The tone does not pressure the owner. The evidence classes (the §5 receipt fields and the §8 local-only STOP list) can each be closed one at a time. The only gap in the sheet is the missing public-visibility choice (F-1).

**What I found no problems with:** no false licence guarantee; Git presence and GUIDs are never treated as ownership; the B06 compiled grant and Compact closed-beta use are kept apart; paid and third-party packages are kept separate from first-party rights; `.meta` files stay in ordinary Git and historical GUIDs are preserved; there is no implied LFS, history rewrite or package edit.

## 5. Not run or not verified

- No Unity, Blender, FBX decoding, package cache, installed `git lfs`, LFS quota or billing, build inclusion report, native render or import.
- No access to original `.blend` masters, including the README's ZX20 location.
- No legal analysis of what a public repository with no licence permits.
- Repository visibility history was not checked; I report visibility as of today only.
- External policy pages were checked only for HTTP status, not re-read for wording.
- No dashboard ingest or Drive mirror sync.

## 6. Handoff

The author needs to fix **F-1 and F-2** before Manager #700 accepts the document. F-3 and N-1 can go into the same correction pass. Once the author publishes a corrected revision, Manager can send it for an independent delta review limited to those findings.

Nothing in this review is a D-032 decision, a licence or rights receipt, a storage approval, or production authority. The choices about repository visibility and art custody that F-1 raises belong to the **product owner**, through Manager #700.
