# Sen-1 #767 — Independent review of the #764 Compact30 world / route / Tier S signature-readiness dossier

**Verdict: APPROVED** — documentary evidence quality only. CRITICAL 0 / MAJOR 0 / MINOR 2 / NOTE 5.

This verdict covers how well the #764 document's evidence holds up. It is **not** an owner signature. It does not approve world content, sign the #743 instrument, provide the focused F-6 review of a filled #743 revision, give a physical-custody receipt, release a Unity lease, start Ops or start #718. The W §12 instrument is still **NOT PRESENTABLE** and #718 stays **HELD**.

## 1. Reviewer identity and independence

- **Reviewer:** Anthropic Claude Opus 5.5 (`claude-opus-5-5`), Claude Code harness inside T3 Code, on the user's Linux host. Role: NONAUTHOR reviewer for [#767](https://github.com/jermur788/CCF/issues/767).
- **Independence:** the #764 author was OpenAI GPT-6 working through the ChatGPT GitHub connector/browser. This review comes from a different provider and a different session. I did not author or edit #764, #743, #676 M/C, #685, #583, #622 or #731. Disclosure: the frozen #676 independent review (16783336) was also written by a Claude Opus 5.5 session. I cite it only as a prior witness. Wherever this review states a result, I recomputed it from the frozen bytes and did not rely on that review's conclusions.
- **Claim:** before writing, I confirmed through the REST comments endpoint that the `[RUNNING]` claim on #767 was registered (count 1).
- **Workspace:** `/media/jer/Files/CCF/Operations/CCF-767-WorldW12Review-20261118`, branch `review/sen-1-767-compact30-world-w12-20261118-v1`, from main `a69b3e2e90829505c792d95f5a3eba3e7d8d8132`. This file is the only write.

## 2. Frozen target and what I read

**Target under review:** commit `2c58e476894d7398aee6297285600d57e98f0706` (on `origin/docs/sen-1-764-compact30-signature-readiness-20261118-gpt6-v1`; parent `7d24af79`, the dossier's stated start main), path `Docs/Handoffs/ScenarioOne-Compact-Ecology-20261118/Sen-1-764-COMPACT30-WORLD-ROUTE-AND-OWNER-SIGNATURE-READINESS-RESULT-20261118.md`. The file is 139 lines. My SHA-256 of the blob bytes is `bf7415dca44f04f2aa82ce3940404ad6c4db8621c8d73fb26312114f03f8aa21`.

I read every source below in full through `git show <rev>:<path>`. I checked out nothing and edited nothing.

| Source | Commit (full) |
| --- | --- |
| #676 manifest M (JSON, parsed) | `8617587ebb7e118183bdd3eec7de5b099c371fdd` |
| #676 36-cell companion C (JSON, parsed) | `8617587e…` |
| #676 construction + owner-signature candidate | `8617587e…` |
| #676 independent review | `16783336451d327802e1bbe9ab6fcd1675e8c9c7` |
| #685 final asset/render/LOD dossier (and its git lineage bb7a4c56 → 96bf137d review → 75b3d8ea) | `75b3d8eadcd0ed09eb1357c3ecabc854737ff3e7` |
| #743 instrument, revision 3 (and history 5d67e845 → d9824565 → a68e905a; reviews b79b6919 and c6fdeac2) | `a68e905aff5a04290c3f08abfa0e1084fdb55394` |
| #583 Route A / G1 clearance | `a9da04b129a8383e68dd2db79900e4b428e1fd63` |
| #622 H1/route/G1 guard design | `4aedf2bfd2bae19bdd647755ca152e2f921a8c37` |
| #731 filled W §12 selections (historical) | `afddcb7f49b0085247cccdd4414e0b4421c16e53` |
| #731 parallel-lanes plan (lane C brief) | `0a44cdaa266ea61ca52ca6fa6f1de774053b6528` |
| Decision Log D-083 … D-091 | worktree `a69b3e2e`; `git diff 7d24af79 a69b3e2e -- Docs/Project/decision-log.md` is empty, so it is identical to the dossier's start main |

Method: Python 3 (`-I -B`) scripts in a session scratchpad outside the repository. I parsed both JSONs, rebuilt the roster CSV from the serialization recipe in M, reimplemented FNV-1a `PickBase`, recomputed cell occupancy from the stated grid formula, and rebuilt the C0 Hermite curve from the anchors alone using M's stated slope rule. I measured G1 distances with 200,000 samples per segment, plus point-to-chord refinement. I used `sha256sum` on bytes from `git show`.

## 3. Matrix over the #767 items

| # | Item | Result | Evidence and my own values |
| --- | --- | --- | --- |
| 1 | World/roster arithmetic | **PASS** (part UNVERIFIED) | M terrain `[30,30]`, bounds `[-15,15,-15,15]`, 900 m², `y=0 exactly`, slope 0, no hydrology. Roster has 175 rows, all `sitka-spruce`, x/z in [−14.4, +14.4]. `initial_management.racks=[]`, `no_rack_owned_orders=true`. M.H1 omission IDs match the dossier's 14 exactly, and none of them is in the roster. The 8 buffer IDs are in the roster, and they are **exactly** the 8 roster stems within 1 m of the H1 rectangle (distances 0.115–0.266 m); there are no others. No roster stem lies inside H1. The 19 former rack-labelled IDs (P0700–P0707, P0710–P0715, P0800/02/04/06/15) are all in the roster, as K.md:182 and R676:233 also record. No plank content is present (H1.must_not includes "historical52planks"). **UNVERIFIED:** the 189 source count, and the fact that the 14 omissions are exactly the 189∩H1 intersection. Both need C-B-30.json, which is not a git object here. |
| 2 | Identity/geometry | **PASS** | M.identity: `profileId o1-compact-v1`, `worldId scenario-one-compact-v1`, `geometryKey compact30-v1`, `numeric_geometry_id 2`, `numeric_id_status RESERVATION_ONLY_UNAVAILABLE`. |
| 3 | Sitka prefabs and GUIDs | **PASS** | I reimplemented PickBase (FNV-1a32, offset 2166136261, prime 16777619, `% len`; pole bases if height < 12 m, otherwise mature bases; min height 8.07 m). Result: Pole_01 **57**, Young_02 **50**, Mature_01 **31**, Mature_Benchmark_01 **37**, sum **175**. The GUIDs in M (`6fc4c8b0…`, `c54cc46c…`, `b3a74f27…`, `69a172cf…`) equal the #743 §4 list. M `yaw_degrees 0` and `Quaternion.identity`. The dossier claims no random yaw, which is correct. On "no proxy-art substitutions", see N-3. |
| 4 | 36 ecology cells | **PASS** | I recomputed occupancy from the 175 rows: empty cells **[2,7,8]**, no out-of-grid stems, and every cell's `tree_ids`/`occupancy_count` equals C (36/36). C grid origin (−15,−15), 5 m, 6×6, half-open edge. Selectors RNG1/regen2/growth2/storm0 and seed 20260914 appear in both M and C. C.numeric_policy says `native_bitwise_parity: NOT VERIFIED` and gives a 0.000002 tolerance. The dossier's F-1 one-ULP caveat for cells 7, 8, 15 matches R676:89. The dossier makes **no** native bitwise claim; it says the opposite. I did not recompute the 360 float values (R676 did); see §6. |
| 5 | Route and G1 | **PASS** | From the anchors alone, using M's slope rule (endpoint m=chord, interior m=mean of chords), I get slopes matching M, terminal dx/dz **0.221818181818182** (claimed +0.221818181818182). Max same-Z departure **0.0940740741 m ≤ 0.095** (claimed 0.094074074074). Min radius of curvature **0.62536 m** at A7 (claimed ~0.625). Length 19.50900 m (M 19.509002). A8 setback 15 − 13.5 = **1.50 m**. All four G1 sites are in cell 15 by the grid formula. Site distances, mine vs claimed: see table below. Every one agrees to 6 dp. **No mismatch.** |
| 6 | H1 | **PASS** | M.H1 rect `[-9,3,-15,-5]`, area 120 (12×10). Surface is "same continuous y0 collider; no step, apron slab, road or visible stump". Staging `OpenArrival`, zero parked spaces/vehicles. must_not covers stumps, Fell history, trucks/equipment, road/rack, construction (buildables). |
| 7 | Signature state | **PASS** | From the JSON: `counts.fields_after` SIGNED 0 / CANDIDATE 80 / BLOCKED 8. A direct tally of the 88 `field_closure` rows (rows 1–88 contiguous) gives the same 80/8. C counts after are 0/36/0, and each of the 36 cells is `CANDIDATE`. `world_digest: null`, status `UNAVAILABLE_INCOMPLETE_NOT_UNSIGNED_CANDIDATE_DIGEST`, attestation `UNFILLED_NOT_SIGNABLE`. Blocked rows are exactly **10, 12, 30, 44, 73, 74, 75, 83**, mapped to B-W02-RIGHTS [10], B-W02-SCALE [12], B-W13-RENDER-DEPS [30,44,73,74,75] and B-W14-WHOLE [83], as in the dossier's §3 table. "Eleven source materials" matches R685:68. |
| 8 | SHA-256 claims | **PASS where rehashable; UNVERIFIED otherwise** | See §4. All 5 SHA-256 values I could rebuild from git objects match. 3 raw-source hashes were not rehashed. The dossier states that a git blob SHA is not a SHA-256 of file bytes (§1, line 11), and so does this review. |
| 9 | W §12 F-1…F-7 | **PASS** | The dossier §5 states each role and its timing consistently with #743 §§2, 5, 8 and 9. F-2 is the operative physical Ops route, a presentation STOP. F-4 is the named human custody owner plus event-driven release, also a presentation STOP. F-3a is pre-start. F-3b is after production and before public push/acceptance, and is not a prerequisite of F-7. F-7 requires F-1/2/3a/4/5/6 + READY + lease *eligibility*, and the lock is taken after F-7. The dossier repeats the presentation rule (#743 line 4 and §9 B-1) in §4 (line 78) and §6 (line 116). |
| 10 | #676 review F-2 ≠ W §12 F-2 | **PASS** (N-1) | The dossier draws this distinction explicitly (§3 line 72; §5 line 97). Its description of #676 review F-2 (rows 10 and 12 should also carry B-W13) matches R676:99/447/522. I found no conflation. One unprefixed "(F-1)" in §2 is a style note (N-1). |
| 11 | D-091 | **PASS** | §4 says "No arbitrary lease duration per D-091". F-4 says never substitute 4/8/12 h or a far-future date, and a stale heartbeat triggers inquiry only. §6 says "D-091 no time limits". Nothing in the dossier implies timeout expiry or silent reclamation. It also avoids repeating the superseded "supervised UTC expiry" wording that still appears in D-086/D-088/D-089. |
| 12 | Tier S scope | **PASS** | The five public paths in dossier §4 match D-088 and #743 §3 rows 1–5 byte for byte. The two temporary paths match #743 rows 6–7. The dossier says no guard and no Tier H/J (§2 guard row, §4). Its rights statements are bounded by D-087/D-088, and it explicitly denies third-party/URP licensing. It invents no mesh, collider or Unity evidence. |
| 13 | Unity/physical | **PASS** | §8 "NOT RUN / NOT CLAIMED" lists native compile/import/scene/LOD/collision/navigation, Ops install/board/lock/preflight and rendered checks. The header and §8 keep #718 HELD and the instrument NOT PRESENTABLE, and state that no Unity or Windows work is dispatched. The dossier never claims a signed world. |

**G1 site distances: my recomputation next to the dossier's figures** (smooth C0 centreline; core + r0.10 guard means d − 0.70; core + 0.20 verge + r0.10 means d − 0.90):

| Site | Mine: distance | Claimed | Mine: core+guard | Claimed | Mine: core+verge+guard | Claimed |
| --- | ---: | ---: | ---: | ---: | ---: | ---: |
| O1 (+1.17, −0.47) | 1.085015 | 1.085015 | +0.385015 | +0.385015 | +0.185015 | +0.185015 |
| O2 (+1.57, −2.73) | 1.290959 | 1.290959 | +0.590959 | +0.590959 | +0.390959 | +0.390959 |
| B1 (+3.43, −0.07) | 1.179673 | 1.179673 | +0.479673 | +0.479673 | +0.279673 | +0.279673 |
| B2 (+3.83, −2.33) | 1.002938 | 1.002938 | +0.302938 | +0.302938 | **+0.102938** | +0.102938 (limiting) |

For contrast, my straight-polyline distances are O1 1.057755, O2 1.262838, B1 1.222835 and B2 1.020976. These equal M `polyline_distance_m`. They are different numbers from the C0 figures, as the dossier warns.

## 4. SHA-256 rehash results

| Object | Bytes obtained from | Claimed | Rehash |
| --- | --- | --- | --- |
| #676 M JSON | `git show 8617587e:…MANIFEST-CANDIDATE…json`, 308,748 B | `342e1afc…a561a677` | **PASS**, 308,748 B |
| #676 C JSON | `git show 8617587e:…36-CELL…json`, 82,840 B | `2441926a…77e86123` (also M `/cells/sha256`) | **PASS** |
| #676 construction MD | `git show 8617587e:…CONSTRUCTION…md`, 88,048 B | `f553fff0…e8a0f26` | **PASS** |
| #743 revision 3 | `git show a68e905a:…INSTRUMENT…md`, 34,090 B | `7ad64693…ec47d297` | **PASS**. Revision 1 at 5d67e845 is 26,817 B, as the dossier says. |
| Roster CSV (digest scope ROSTER_ONLY) | Rebuilt from M.roster rows and the serialization recipe; not a stored file | `dac06a3b…0426cca1`, 9,481 B, 176 lines | **PASS**. I rebuilt it as 9,481 B / 176 lines with an identical SHA-256. This is a reconstruction from M, not a rehash of an original native file. |
| C-B-30.json | Not in this repository | `3e7c37ad…1f9d5a27` | **UNVERIFIED**. Not rehashed; reported value not accepted. |
| #508 ledger.json | Not in this repository | `aa10f54f…0b0bcf3` | **UNVERIFIED**. Not rehashed. |
| omissions-2.0.json | Not in this repository | `dcc5a53e…c4ce6bd2ab` | **UNVERIFIED**. Not rehashed. |

The dossier labels these three raw-source hashes as "not accessible/rehashed physically", which is correct. I deliberately did not read the native Files archive: it is outside this docs-only grant. **A git blob SHA (for example M's blob `6c562f34…`) is a SHA-1 over a `blob <len>\0` header plus content. It is not a SHA-256 of the file bytes, and none of the PASS rows above relies on one.**

## 5. Findings

### m-1 — MINOR — The §1 preamble overstates how independent the #743 hash is

**Where:** §1, line 11: "SHA-256 values below are **reported and independently reproduced in the cited original #676 review or #743 author GitHub receipt**". Row "Latest bounded Tier S exception instrument", line 22: "latest SHA-256 per author receipt".
**Problem:** an author's own receipt does not count as independent reproduction. The #676 review does reproduce the three #676 hashes (R676:189–191). The #743 value 7ad64693… comes only from its author. (The c6fdeac2 delta re-check hashed revision 2 `b9b03702…`, not revision 3.) The value is correct, since I reproduced it, but the sentence misdescribes the evidence.
**Fix:** reword along these lines: "#676 hashes were independently reproduced in the #676 review; the #743 revision-3 hash is author-reported (independently rehashed later by #767)".

### m-2 — MINOR — The guard-row intrusion figure is a polyline number presented beside C0 without a label

**Where:** §2 "Guard concept" row, line 41: "The old O1 damaged pose intrudes **0.14136 m** into the 1.2 m core".
**Problem:** K.md:355 and R676:338 both measure 0.1413575721 m against the **original 8-anchor straight polyline**, not the smoothed C0 centreline. The dossier's own §2 note (line 59) says not to mix polyline numbers with smoothed C0. Unlabelled, the sentence reads as a C0 core intrusion. The historical renderer is not used for Compact either way, so nothing is materially wrong, but it is a clarity defect inside a signature-readiness dossier.
**Fix:** add "(measured against the original polyline, per #676 K.md §guards; not recomputed against C0)".

### N-1 — NOTE — An unprefixed "F-1" sits in the cells row

§2 Cells row, line 38: "1-ulp precision issue (F-1)". This is the #676 review F-1. The dossier separates the two code spaces elsewhere, so this is not a conflation. Still, in a document where the W §12 F-1 means "sole producer", writing "#676 review F-1" would be safer.

### N-2 — NOTE — The A0 overlap wording is loose

§4 anchors paragraph, line 48: "A0 overlaps the northern H1 edge by path-core radius". A0 is 0.25 m north of z = −5, so the 0.60 m core reaches 0.35 m into H1, not 0.60 m. M's wording is "A0 is only .25m north of H1 edge and radius.6 overlaps apron". Suggest: "the core (r 0.60) overlaps H1 by 0.35 m".

### N-3 — NOTE — "No proxy-art substitutions" needs scoping

§2 Sitka row, line 39. M.visual_assignment.proxy_policy keeps the authoritative cylinder trunk proxy (diameter dbh/100, height H) and strips the polished mesh colliders. The dossier's phrase is correct if it means no proxy art replaces the prefab families. It should not be read as "no proxy at all". Suggest: "no proxy-art substitution for the four prefab families; the source trunk proxy remains (M proxy_policy)".

### N-4 — NOTE — The F-6 exclusion list is wider than the instrument's

§5 F-6, line 107, also excludes the "#764 author". #743 line 204 requires only "a different session from this author, producer and custodians". The extra exclusion is a reasonable recommendation, but it should be labelled as #764's recommendation, not as an instrument requirement.

### N-5 — NOTE — Prior-witness counts relayed without being rerun

The dossier relays "347/347 dependencies, 360/360 cell values, 13 owner receipts" from #676 (§1 line 19). These match R676:390/540/549 and are correctly attributed as prior witness results. I did not rerun the 347 dependency hashes or the 360 float values (see §6). The dossier does not claim to have rerun them either.

### Items checked with no finding

- #731 precursor: §2 (signature table line 54) says "A8 NOT SELECTED" while the status line and D-090 say provisional. The dossier describes this accurately.
- #743 lineage is described correctly: three commits, two NEEDS CHANGES reviews (b79b6919, c6fdeac2), the last revision a68e905a not re-reviewed, and the internal "REVISION 2" header still present at revision 3.
- #685: the final correction 75b3d8ea was not independently delta-reviewed. #686 was cancelled under the owner waiver (58951f2e), which matches the dossier.
- The player spawn (2, 0.1, −8), yaw 16.94154°, radius 0.30 m and skin 0.03 m all match M.

## 6. Not verified, and why

- **The 189 source count and the claim that the 14 omissions are exactly 189∩H1:** these need C-B-30.json / #508 ledger / omissions-2.0.json. None is a git object here, and the native archive is outside this grant. Their hashes are UNVERIFIED (§4).
- **The 360 cell float values:** I did not reimplement the binary32 canopy/flora/competitor model. Only occupancy, tree IDs and empty cells were recomputed. The F-1 one-ULP claim is checked against R676/R685 text only.
- **The 347 dependency hashes, the 13 owner-receipt bodies, and the live GitHub branch/claim state of #764:** not rerun or fetched beyond the #767 claim check.
- **Rights, licences, URP package GUID attribution, LOD thresholds and DBH/crown fidelity:** these cannot be settled from documents. They stay blocked as the dossier states.
- **Anything native or physical:** no Unity, compile, scene, collision, capsule, rendering, Ops, board, lease, preflight or custody. None was in scope or performed.
- **Owner intent beyond the Decision Log text:** D-083–D-091 were read as text. I did not inspect the owner receipt comments.

## 7. Fix requests (minimal; non-blocking for this documentary verdict)

1. **m-1:** reword §1 line 11 so the #743 hash is labelled author-reported (now independently rehashed by #767), not "independently reproduced".
2. **m-2:** label the 0.14136 m O1 intrusion in §2 as measured against the original polyline.
3. **N-1 to N-4:** optional wording clarifications as described above.

These fixes belong to the #764 author or to Manager #731's intake. The frozen #764 bytes must not be edited in place. Any change needs a new commit, and the changed lines need a fresh nonauthor check if Manager requires one.

**Unchanged by this review:** the world is still SIGNED 0 / CANDIDATE 80 / BLOCKED 8 with `world_digest = null`. The #743 instrument is still UNSIGNED and NOT PRESENTABLE until F-2 and F-4 are recorded in its exact bytes and that filled revision has its own focused nonauthor review. #718 stays HELD. No owner signature, custody holder, physical route, reviewer for F-6 or digest has been invented or implied.
