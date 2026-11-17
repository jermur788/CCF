# Sen-1 #709 — Independent review of #705 Compact30 world blockers and unsigned attestation evidence

**INDEPENDENT NON-OPENAI DOCUMENT REVIEW ONLY. Not a world signature, content approval, spec approval, native test or production authority.**

**Verdict: NEEDS CHANGES (narrow) — 1 major, 3 minor, 5 notes.** All facts, counts, hashes and geometry I checked are correct. The change needed is to the scope and order of the proposed owner-attestation gate, not to any blocker status or number.

## 1. Reviewer identity, scope and sources

| Item | Value |
| --- | --- |
| Actual reviewer | **Anthropic Claude Opus 5.5** (`claude-opus-5-5`), Claude Code CLI on the owner's Linux host. Repository read through local Git plus GitHub REST. Not OpenAI. Not the #705 author (OpenAI ChatGPT GPT-6 browser). Not a signer or content author. |
| Claim | [#709](https://github.com/jermur788/CCF/issues/709). Registered [RUNNING] before any edit ([registration comment](https://github.com/jermur788/CCF/issues/709#issuecomment-6961837849)). |
| Review branch / starting ref | `review/sen-1-709-world-attestation-nonopenai-20261117-v1` from origin/main `c91fc12faa477fa34de64f0a0d03df1a0c4efacc` |
| Sole write | This new Markdown file |
| Reviewed document | #705 result at `8676b4b264fedb4ef25e6ffb3cd7746495dc1cf3` (branch `docs/sen-1-705-world-blocker-evidence-worker-20261117-v1`), 31,218 bytes. Cited below as `705:L<n>`. |
| Frozen #676 sources (opened directly, not via #705) | Commit `8617587ebb7e118183bdd3eec7de5b099c371fdd`: **[M]** manifest blob `6c562f34…f958`, 308,748 B, SHA-256 `342e1afc4d39cf44c22e434dac3b7ad56d9902084bf3f61026e89086a561a677`; **[C]** cell companion blob `afd28d1b…f7cc`, 82,840 B, SHA-256 `2441926a0a619b9731166149ca2121f2bb89f4a56a8d8ff827c39e9d77e86123`; **[W]** dossier blob `fb02244baa1298d0d71a3cbc8bae961f6cd75ecf`, 88,048 B, SHA-256 `f553fff031246311d0000050b2a9ece8a81efd0c54f1006a34e74289fe8a0f26` |
| Other authority read | [R676] at `16783336` (F-1, F-2, N-3, N-6); [A685] at `75b3d8ea` (§§F-2, LOD, eleven materials); [P688] at `2a625018` (§§5–6); Decision Log D-032/D-083/D-084/D-085 at `e371b181` and current main `c91fc12f` (no D-083..085 change between them); source assets at `ab09111681bf1acce3823d6fe3343642a851f3d9` |
| Physical read (read-only) | The 14 raw-local dependency files under `/media/jer/Files/CCF/Verification/…`, hashed only. Nothing was executed. |

No frozen #676 file, #705 report, world JSON, spec, decision log, coordination file, source, asset, save, Prefs, Unity, Blender, Ops or Windows action. No world digest was computed and no signature was made.

## 2. What I independently reproduced

All checks are static (Python 3 `-I`, `git cat-file`, `sha256sum`). Nothing here is Unity or native evidence.

| Check | Method | Result |
| --- | --- | --- |
| Frozen file identity | `git ls-tree 8617587e`; sha256 of blob bytes | M/C blobs match `705:L11–12`. C SHA-256 = `2441926a…6123`, which matches `705:L80` and M `/cells/sha256`. |
| 88-row closure | M `/field_closure` | Rows 1–88 all present. After: **CANDIDATE 80 / BLOCKED 8 / SIGNED 0**. Before: 36/52/0. BLOCKED rows **10, 12, 30, 44, 73, 74, 75, 83** match `705:L22`. |
| Blocker IDs per row | M `/field_closure/*/blocker_ids`, `/signature_blockers` | 10→B-W02-RIGHTS, 12→B-W02-SCALE, 30/44/73/74/75→B-W13-RENDER-DEPS, 83→B-W14-WHOLE. This matches `705:L41`. |
| Shader/script GUID correction | `git show ab091116:<mat>` for the material YAML | In every one of the 11 closure materials, `933532a4fcc9baf4fa0491de14d08ed7` is the `m_Shader` reference and `d0353a89b1f911e48b9e16bdc9f2e058` is an `m_Script` reference. Each material's own `.meta` GUID is different (for example Bark.mat `c2792981…`, Sitka_Needles.mat `dd52816f…`). No tracked `.meta` defines either GUID. M `/signature_blockers/2` calls them "material GUIDs", which is wrong; #705's correction (`705:L41`) is right. |
| 22 references / 11 materials | M `/visual_assignment/unresolved_asset_guids` | 22 entries: 11 distinct tree-visual materials × 2 GUIDs, 11 of each. This supports applying B-W13 to rows 10 and 12 ([R676 F-2], [A685 §F-2]). |
| 347 dependency ledger, **physically witnessed** | For each M `/dependencies` entry, `git rev-parse <commit>:<path>` plus byte length plus SHA-256; raw files read from disk | **333/333 Git objects match** (blob, bytes, SHA-256) across 7 commits (ab091116 ×314, 1e0fba5f ×14, five others ×1). **14/14 raw-local files exist on this host and match bytes and SHA-256.** This is a custody/integrity witness only. It says nothing about rights. |
| Roster digest | Rebuilt the CSV from M `/roster/rows` using M `/roster/serialization` | 9,481 bytes, 176 lines, SHA-256 **`dac06a3b…cca1` reproduced exactly**. 175 unique IDs, ASCII-sorted, all `sitka-spruce`, all age 20. |
| 189 → 14 → 175 census | Raw `C-B-30.json` (189 trees, seed 20260914, target 189) compared with the roster | The roster is a subset of the 189. **Source minus roster equals exactly the 14 IDs** in `705:L45` and M `/H1/pre_world_omission_ids`. x/z for all 175 match the raw source to 3 dp. |
| Buffers / former rack stems | Roster lookup; raw `side` field | All 8 buffer IDs are retained. No retained stem lies inside the H1 rectangle. All **19** `side=rack` source stems are retained. |
| P0109 H1 clearance | Distance from rectangle to stem centre minus DBH/200 | **0.02525 m** reproduced (`705:L47`, [P688 §6]). |
| 36 cells | M grid formula applied to every roster stem | All 175 stems sit in the stated cell index. No duplicates. Occupancy sums to 175. Zero-stem cells are 2, 7 and 8. 36/36 CANDIDATE. `native_readback_status` NOT_RUN. C `owner_signature` all null. |
| C0 Hermite | Re-derived tangents from the anchors; sampled 4,000 points per segment | Tangents equal M `/path/interpolation/pinned_derivatives_dx_dz`. Maximum same-Z departure **0.0940741 m** (cap 0.095). Minimum radius **0.62536 m at A7 (z = 10.75)**, consistent with [P688]'s bound of ≥0.625284 m. |
| T1 (m6 = m7 = −0.24) | Same method | Maximum departure **0.1881481 m**, which **fails the 0.095 cap** and fits under the proposed 0.190. Sampled minimum radius 1.619 m is ≥ [P688]'s conservative bound of 1.488681 m. |
| G1 margins | Curve-to-site distance − 0.8 − 0.1 | O1 0.185015, O2 0.390959, B1 0.279673, **B2 0.102938** (C0 and T1 are the same because T1 only changes segments 6–7). This matches M `/G1/sites/*/core_verge_guard_margin_lower_m` and `705:L51`. |
| Guards | M `/guards` | Tube outer radius 0.065. Stake corner = √(0.0925² + 0.01²) = **0.0930389703 m**. Panel heights 1.2 / 0.85–1.2 / 0.28–0.35 and stake heights 1.25 / 0.45 match `705:L36–38`. W §8: old O1 intrusion **0.1413575720592738 m** against the original polyline. |
| LOD thresholds | Four unpruned prefabs at ab091116 | `screenRelativeHeight` is 0.55 / 0.2 / 0.04 in all four (`705:L32`). 107 roster stems are below 12 m (pole) and 68 are at or above 12 m (mature). |
| Initial state | M `/initial_management`, `/player`, `/G1` | `cashCents` 1,200,000. Every list is empty, including `cropTreeIds`, `crop_suggestions`, `plantedJuveniles`, `shelters` and `harvest_history`. Spawn (2.0, 0.1, −8.0). Reach 3.5 m / 8 m. `initial_juveniles` empty, `planar_tolerance_m` 0. |
| Signature / digest | M `/world_digest`, `/attestation`, `/canonical_bytes` | `world_digest` null (`UNAVAILABLE_INCOMPLETE_NOT_UNSIGNED_CANDIDATE_DIGEST`). All four `owner_signature` fields null. `705:L75–98` has **13 unchecked items** and fills nothing. |

## 3. PASS / FAIL by evidence category

| # | Category (packet §) | Result | Note |
| --- | --- | --- | --- |
| 1 | Source identity and frozen state (§1) | **PASS** | [W] blob not stated (N-1) |
| 2 | Eight rows × eight columns A–H, statuses (§2) | **PASS** | Cells are faithful to M/A685/P688/P698. Pointer labels: m-3 |
| 3 | B-W02 / B-W13 / B-W14 attribution and GUID correction (§2) | **PASS** | Verified in material YAML |
| 4 | 347 references compared with physical files (§2) | **PASS**, now physically witnessed | N-2 |
| 5 | 189 / 14 / 175 / 8 buffers / 36 cells (§3) | **PASS** | Reproduced from raw C-B-30 |
| 6 | No invented scene count, Fell, stump or harvest history; three *retained* Crop Trees; zero Year0 Oak/Beech; four real Work Plan plantings (§3) | **PASS** | `705:L22, 45, 53, 55` |
| 7 | 30×30 m no-rack scene; 12×10 m H1; 1.2 m core plus ≤0.20 m visual-only verge (§4) | **PASS** with m-2 | Scene path labelled author-proposed (`705:L47`) |
| 8 | C0 vs T1, 0.190 cap unapproved, A8 unsigned, G1 immutable, B2 margin, O1 intrusion not silently fixed, 0.625 vs 1.49 m labelled analytic (§4) | **PASS** with m-1 | `705:L49–51, 102` correctly attribute C0/T1 numbers to [P688] |
| 9 | 13 unchecked items, nulls, no whole-world digest, roster/cell checksums not relabelled (§5) | **PASS** | `705:L45, 80, 92–98` |
| 10 | Checklist faithfully expresses the necessary owner signature without unsanctioned or circular prerequisites; future packets executable (§5–6) | **FAIL** | M-1 |
| 11 | §4.2 / #677 not implemented; Legacy40, E80, Reference Future, v1–v24 and OD-16 respected (§5) | **PASS** | `705:L55, 87` |
| 12 | Unsupported numeric, schema, SHA or rights claims (§6) | **PASS** with m-2 | No rights asserted. Numbers are attributed. |

## 4. Findings

### M-1 — MAJOR — The owner-attestation gate is ordered and scoped against the frozen #676 signature meaning, and the conflict is not stated

**Where in #705:**
- §4 packet 5 (`705:L71`): "Verify four real planting orders, event-9 review, substantive later management, M-A class, save/load, signed §4.2 isolation, and Legacy40/… protections on exact candidate. **Only after** every blocker is closed … may Manager request the product owner's personal attestation."
- §5 item 10 (`705:L86`) requires, before signature: "four genuine exact planting orders, real event-9 review, later Approved→Completed treatment and reviewed adaptive act/no-work, **resolved OD-16 classifier**".
- §5 item 11 (`705:L87`): "immutable signed three-profile §4.2 contract **implemented and tested**".

**What the frozen sources say:**
- [W §11.1], "Meaning of future signature": "Accept exact nominated content/recipes/proposed numeric geometry and source/calibration inputs, **not measured ecology, native acceptance, source-start/spec approval or save-owner mapping**."
- [W §11.2]: "**OD-16 stays deferred outside this world-content definition.**"
- M `/attestation/would_attest`: "Does not assert measured ecology/native behaviour, sign save-owner serialization, approve DRAFT specs or release physical/source/Ops/Windows gates."
- [W §12]: "Before production source/scene/asset/save work: resolve exact content blockers, obtain independent review **and owner's complete exact-byte world attestation**; formally APPROVE the relevant Compact specs …". Gameplay, save/load and compatibility qualification come **after** that.
- D-085 records OD-16 as "**owner-DEFERRED and undecided**".

**Why it matters:**
1. Item 10 makes the world signature depend on resolving OD-16, which the owner has deferred and which W explicitly places outside world content. Unless the owner gives up that deferral, the signature cannot be given.
2. Packet 5 and items 10–11 require gameplay and save qualification on the exact candidate before attestation. W §12 requires attestation before the production source/scene/save work that would make that qualification possible. Followed literally, the two documents point at each other and neither can go first. #705 does not mention this conflict.
3. Some native evidence is legitimately a precondition, because the blocked rows cannot close without it. Examples: OD-11 "must remove the existing 0.141 m path intrusion", authored scene bytes for B-W14, and LOD/scale fidelity for B-W02-SCALE. W itself is not fully consistent here: B-W14 needs a scene that W §12 places after attestation. #705 settles that tension silently and puts *everything* before attestation.

**Required correction (Manager corrigendum or successor checklist; #705 itself stays frozen):**
- Split the checklist into:
  - **(a) Content-attestation preconditions.** Rows 10/12/30/44/73/74/75/83 closed. Rights and custody settled. Exact asset and scene bytes. Only the native evidence each blocked row needs, such as guard bounds, O1 non-intrusion, LOD/scale fidelity and scene binding. The full successor bytes reviewed by someone other than the author.
  - **(b) Later qualification gates the attestation does not assert.** Work Plan, event-9, M-A/OD-16, §4.2 implementation, save/load and historical compatibility.
- Remove "resolved OD-16 classifier" from the attestation preconditions.
- Ask the owner or Manager to rule explicitly how a pre-attestation, non-integrated Compact scene/asset authoring lane may produce bytes for blocked row 83 without contradicting W §12.

### m-1 — MINOR — The B2 margin is not compared with the owner-recorded figure

D-085 OD-08 records the owner's reasoning: "the existing G1 B2 core-plus-verge margin of only **+0.1209757 m** means unbounded smoothing is unsafe". That figure is the original-polyline value (M B2 `polyline_distance_m` 1.0209757 − 0.8 − 0.1). #705 (`705:L51`) and M use the smooth-curve value **+0.1029378 m**, which is about **0.018 m smaller**. The smoothing the owner chose under OD-08 B already uses up about 15% of the margin the owner cited. #705 should say this next to the C0/T1 decision so the owner decides on the curve-based figure. T1 does not change B2.

### m-2 — MINOR — H1 exclusions that M does not support

`705:L47` lists H1 exclusions as "path for vehicles, road/**kerb**/stumps/**yard**/**gate**", and `705:L35` row 44 includes "yard". M `/H1/must_not` contains visible stumps, Fell history, H1 harvest/revenue, grant/automatic cash, parked trucks/equipment, road/rack/corridor/historical planks, vehicle on footpath, public-road loading, simultaneous visitors and collection, and historical buildables/crop picks. It has no kerb, yard or gate. D-083 describes collection as "a staged **closed-yard** event", so "no yard" reads against that wording, even though the event is abstract and has no physical footprint (M `/H1/staging`). Quote M `/H1/must_not` verbatim, or label the extra items as reviewer-proposed.

### m-3 — MINOR — Pointer labels

`705:L28` says "Each row cites the frozen manifest pointer". Rows 12, 30 and 73–75 actually cite narrower pointers: `/visual_assignment/scale_rule`, `/terrain/surface` and `/guards/states/1|2|3`. M `/field_closure/*/manifest_pointer` gives `/visual_assignment`, `/terrain` and `/guards`. The narrower pointers are valid (states 1/2/3 = effective/expired/failed), but they should be labelled as narrower than M's pointer.

### Notes

- **N-1.** [W] at `705:L13` has no blob ID. Checklist item 2 (`705:L78`) asks for "actual full byte lengths and SHA-256" but gives none for M or W. The values are in §1 above, for successor use only. They are not signing inputs.
- **N-2.** `705:L63` ("14 raw-local source files cannot be reauthenticated by copied hashes") is correct for a browser worker. On the owner's host on 2026-11-17, all 14 raw files and all 333 Git objects matched. Future packet 1 (`705:L67`) can therefore concentrate on rights, creator and licence evidence rather than byte recovery. A custodian re-witness is still needed at signing time.
- **N-3.** M is internally inconsistent: `/visual_assignment/blocker_ids` lists only B-W02 while `/visual_assignment/dependency_completeness` is "BLOCKED: unresolved runtime shader/package references". This supports #705's B-W13 correction for rows 10 and 12.
- **N-4.** At ab091116, 93 tracked `.mat` files reference shader `933532a4…`, but only 11 are in the #676 closure. Any new Compact URP Lit material (rows 30, 44, 73–75) will reference the same unresolved package shader. That makes B-W13 apply to those rows for a reason beyond production rights, which #705 states correctly.
- **N-5.** The R676 F-1 caveat (cells 7, 8 and 15 differ by 1 ulp; the 2e-6 tolerance is not a native PASS) is carried accurately (`705:L47`). Cells 7 and 8 have no stems, so their canopy comes from neighbouring crowns. I did not re-run this. It is attributed to R676.

## 5. Not run or not verified by this review

Unity, Editor, compiler, import, render, collision, walking, native cell readback, gameplay, save/load, package-cache shader mapping, FBX decoding, rights or licence existence, the [P698] guard construction beyond M's stated bounds, a full re-read of [P698] and [G691]/[R702], and the D-032 decision. Dashboard and Drive sync are unverified. No harness or archived script was executed.

## 6. Outcome

**Verdict: NEEDS CHANGES (narrow), document quality only.** The factual reliability of #705 is high. I reproduced every count, hash and geometry figure I checked, and confirmed the physical custody of all 347 references. Fixing M-1 needs a Manager corrigendum or successor checklist that separates content attestation from later qualification and removes the OD-16 dependency. m-1 to m-3 can go in the same corrigendum. Nothing here promotes any field. The world stays **SIGNED 0 / CANDIDATE 80 / BLOCKED 8**, 36 cells are CANDIDATE, the digest is null and all 13 attestation items are unchecked. #705 stays closed and unchanged. The claim goes to [#709](https://github.com/jermur788/CCF/issues/709) HANDED-OFF, open for Manager #700.
