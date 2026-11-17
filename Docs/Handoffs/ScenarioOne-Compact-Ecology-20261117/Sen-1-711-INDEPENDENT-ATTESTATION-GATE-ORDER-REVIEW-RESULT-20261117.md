# Sen-1 #711 — Independent non-OpenAI review of Manager #700's #709 gate-order corrigendum

**INDEPENDENT NON-OPENAI DOCUMENT REVIEW ONLY. Not a world signature, content approval, spec approval, pre-attestation permit, native test or production authority.**

**Verdict: APPROVED DOCUMENT CORRIGENDUM — 0 major, 3 minor, 5 notes.** The gate-order split, the OD-16 exclusion and the STOP on the scene-byte circularity are correct as written. The three minors are attribution and scope-wording precision inside the corrigendum itself; they are non-blocking and can be fixed in the successor checklist without re-review. Nothing here promotes a field, authorises the proposed lane or approves a spec.

## 1. Reviewer identity, scope and sources

| Item | Value |
| --- | --- |
| Actual reviewer | **DeepSeek V4.1 Flash** (`deepseek-v4.1-flash`), OpenCode Go provider through T3 Code on the owner's Linux host, with local Git and GitHub CLI readback. Not OpenAI. Not Anthropic. Not the #700 corrigendum author (OpenAI GPT-6 browser). Not the #709 reviewer (Anthropic Claude Opus 5.5). Not a signer, content author or rights holder. |
| Claim | [#711](https://github.com/jermur788/CCF/issues/711). Registered [RUNNING] before any edit ([registration](https://github.com/jermur788/CCF/issues/711#issuecomment-6962247399)). |
| Review branch / starting ref | `review/sen-1-711-attestation-gate-order-nonopenai-20261117-v1` from `origin/docs/sen-1-700-manager-intake-20261117-v1` = `b60bc89125290ee25a0a8db23a74002be88301fd` |
| Sole write | This new Markdown file |
| Reviewed document | Manager #700 corrigendum at `b60bc89125290ee25a0a8db23a74002be88301fd` (`Docs/Handoffs/ScenarioOne-Compact-Ecology-20261117/Sen-1-700-MANAGER-709-WORLD-ATTESTATION-GATE-ORDER-CORRIGENDUM-20261117.md`). Cited below as `700:§A`…`§D`. |
| #705 (closed author result) | `8676b4b264fedb4ef25e6ffb3cd7746495dc1cf3`, 31,218 B. Cited as `705:L<n>`. |
| #709 (independent review) | `74898b4c507aafd9df07f6a47b37b42b66cc957b`. Verified against, not substituted for, the frozen files. |
| Frozen #676 sources, opened directly | At `8617587ebb7e118183bdd3eec7de5b099c371fdd`: **[M]** blob `6c562f341bc76ba75753bfb474043f324218f958`, 308,748 B, SHA-256 `342e1afc4d39cf44c22e434dac3b7ad56d9902084bf3f61026e89086a561a677`; **[C]** blob `afd28d1b120f68071908f44474596731f3c1f7cc`, 82,840 B, SHA-256 `2441926a0a619b9731166149ca2121f2bb89f4a56a8d8ff827c39e9d77e86123`; **[W]** blob `fb02244baa1298d0d71a3cbc8bae961f6cd75ecf`, 88,048 B, SHA-256 `f553fff031246311d0000050b2a9ece8a81efd0c54f1006a34e74289fe8a0f26`. All three lengths and SHA-256 values independently reproduced. |
| Authority read | Decision Log at context commit `d435292b29e7e3c987e3df57d7b3555d82e84a8e` (D-032, D-083, D-084, D-085); live coordination at `main` `f52a083836a78f8499f92a0d31a2d06440c4b20e`; [P688] at `2a625018`; [G691] at `582c7b38`. |

No frozen #676 file, #705 report, #709 result, #700 corrigendum, spec, Decision Log, coordination file, source, art, asset, save, Prefs, Unity, Blender, Ops or Windows action. No world digest computed, no signature made. No `ops/workflow-foundation-v1` coordination copy used as authority; that copy is superseded.

## 2. What I independently reproduced

Static checks only (`git cat-file`, `sha256sum`, Python 3 `-I`, `json`). Nothing here is Unity, native or rights evidence.

| Check | Method | Result |
| --- | --- | --- |
| Frozen file identity | Blob size and SHA-256 of the three #676 blobs | M 308,748 B / `342e1afc…a677`; C 82,840 B / `2441926a…6123`; W 88,048 B / `f553fff0…0f26` — all three match #709 and `705:L11–13`. |
| Frozen field status | Counted M `/field_closure/*/after_status` | **SIGNED 0 / CANDIDATE 80 / BLOCKED 8**; before 0/36/52. Matches `700` and #709. |
| Eight blocked rows and blocker IDs | M `/field_closure`, `/signature_blockers` | Rows **10, 12, 30, 44, 73, 74, 75, 83** are BLOCKED. IDs: 10→B-W02-RIGHTS; 12→B-W02-SCALE; 30/44/73/74/75→B-W13-RENDER-DEPS; 83→B-W14-WHOLE. |
| Row 83 definition | M `/field_closure` row 83 | `identity.whole-world record / recipe`, pointer `/canonical_bytes`, evidence "OD-14 A; exact composition recipe but incomplete world_digest=null". Confirms B-W14 is a whole-binding blocker, not a gameplay test. |
| Signer and digest state | M `/attestation`, `/world_digest`, `/world_digest_status` | All four `owner_signature` entries null; `world_digest` null, status `UNAVAILABLE_INCOMPLETE_NOT_UNSIGNED_CANDIDATE_DIGEST`. No interim digest or signer exists in the corrigendum either. |
| Scene state | M `/scene` | `PROPOSED_NOT_CREATED_BLOCKED`, candidate path only, no GUID or scene bytes. This is the concrete fact behind the §12/B-W14 circularity. |
| Roster and cell identity | Rebuilt the roster CSV from M `/roster/rows` using `/roster/serialization` | 9,481 B, 176 lines, 175 unique IDs, all `sitka-spruce`, all age 20; SHA-256 **`dac06a3b…cca1` reproduced exactly** (matches M and D-085 OD-15). 36 cells CANDIDATE; cell checksum `2441926a…6123` is the companion hash, not a world digest. |
| B2 old polyline margin | M `/G1/sites/B2/polyline_distance_m` 1.0209756677655732 − (0.6 + 0.2 + 0.1) | **+0.1209756678 m**, i.e. `700:§D` "≈ +0.1209757 m", and exactly the figure D-085 OD-08 B quotes. |
| B2 smooth-route margin | M `/G1/sites/B2/distance_lower_bound_m` 1.0029378196342815 − 0.9 | **+0.1029378196 m**, i.e. `700:§D` "+0.1029378 m". Difference **0.01803785 m = 14.91 %**. `700:§D` "≈0.0180 m (≈14.9 %)" is correct. |
| B2 is unaffected by T1 | M `/path/anchors` and `/path/interpolation/pinned_derivatives_dx_dz` | B2 is at z = −2.33, inside segment A0→A1 (z −4.75…−2.25), whose Hermite blend uses m0 and m1 only. T1 sets m6 = m7 = −0.24, which changes only the A6→A7 and A7→A8 segments (z 10.25…13.5). So T1 cannot move B2; `700:§D` is right. (`705:L51`'s C0/T1 B2 difference of 7.6 × 10⁻⁸ m is numerical estimator noise, not geometry.) |
| H1 `must_not` contents | M `/H1/must_not` | Exactly ten entries: visible stumps; player Fell history; H1 harvest/timber/revenue/credit; grant/automatic cash; parked trucks/equipment; road/rack/corridor/historical planks; vehicle on footpath; public-road loading; simultaneous visitors and collection; historical buildables/crop picks. **No `kerb`, no `yard`, no `gate`.** `700:§D` is factually right. |
| Where kerb/yard/gate do appear | [P688] and [G691] full-text read | P688 line 13: "No road, gate, sign, vehicle, machinery, kerb, raised path, stumps…". G691 WF-06: "No parked or permanent vehicle, yard, gate or road." Both are DRAFT author proposals, not M and not owner directions. |
| D-083 closed-yard wording | Decision Log D-083 | "Occasional small truck collection is a staged closed-yard event, not a haulage/rights certificate." Confirms `700:§D`'s warning against reading "yard" as a physical H1 feature. |
| Manifest pointer breadth | M `/field_closure/*/manifest_pointer` | Rows 12 → `/visual_assignment`; 30 → `/terrain`; 73/74/75 → `/guards`; 44 → `/H1/surface`; 83 → `/canonical_bytes`. The narrower `/visual_assignment/scale_rule`, `/terrain/surface` and `/guards/states/1\|2\|3` in `705` are descendants, as `700:§D` says. |
| OD-16 deferral | W §11.2; D-085; M `/world_digest` and attestation | W §11.2: "OD-16 stays deferred outside this world-content definition." D-085: "**OD-16 (M-A-SCOPE)** … remains **owner-DEFERRED and undecided**". No OD-16 requirement appears in `700:§A`. |
| W §11.1 exclusion list | Direct quote from W §11.1 | "Accept exact nominated content/recipes/proposed numeric geometry and source/calibration inputs, **not measured ecology, native acceptance, source-start/spec approval or save-owner mapping**". |
| Public repo / binary exposure | GitHub read; M dependency traversal | Repository reads publicly at this revision without authentication; 416 tracked `.fbx` at `8617587e`, Sitka FBX family alone ≈93.96 MiB. Live board records #708's finding: repo PUBLIC, licence null, 104 selected files ≈107.7 MiB, total binary formats ≈1,118 MiB. `700:§A.2`'s disclosure requirement is factual, not speculative. |

## 3. The five assigned questions

| # | Question | Result |
| --- | --- | --- |
| 1 | Does the content-attestation vs post-signature split correctly exclude OD-16 and still retain exactly the necessary content-only native/asset/scene and rights evidence? | **PASS with m-3 and N-2.** `§A` keeps product-owner content choices, D-032 rights/custody, complete actual inputs and dependencies, the narrow blocker-specific native witnesses for rows 10/12/30/44/73–75/83, and the eight-row closure plus nonauthor exact-blob review. `§B` puts Work Plan, event-9, retained Crop Trees, §4.2 implementation/testing, v1–v24/Legacy40/E80/Reference continuity, gameplay/native/performance/Editor/Windows after the signature. OD-16 appears only in `§B` as deferred. No content-only item was moved out, and no gameplay item was left in. The only defect is the W §11.1 paraphrase attribution (m-3). |
| 2 | Is OD-16 genuinely deferred, with no invented threshold or premature requirement? | **PASS.** `§B` says OD-16 remains owner-DEFERRED, forbids inventing a numeric treatment threshold, and routes any later need to a separate owner decision. It matches D-085 and W §11.2 and does not condition the content signature on it. |
| 3 | Is the W §12 / B-W14 scene-byte paradox explicitly owner decision / STOP, with no premature permission in the proposed pre-attestation lane? | **PASS with m-2.** `§C` is headed "STOP, owner/Manager scope clarification needed", states "**NOT APPROVED HERE**", requires user approval plus explicit Manager [APPROVED TO START], and says "No present choice is implied." No branch, path, file list or permit exists in the corrigendum, and the lane is described conditionally throughout. Two wording issues remain (m-2), neither of which grants anything today. |
| 4 | Is the B2 margin comparison correct (old polyline +0.1209757 vs candidate smooth +0.1029378)? | **PASS.** Reproduced to the stated decimals, including the 0.0180 m / 14.9 % delta and the fact that T1 cannot change B2. The "analytically positive, not native collision clearance" framing is correct, and the instruction not to use the larger figure to justify more smoothing, widening or guard displacement follows D-085's own rationale. |
| 5 | Are the H1 attribution, manifest pointer scope, eight BLOCKED rows, absent signer and absent digest all correctly stated? | **PASS for the facts; m-1 for the replacement attribution; N-4 for pointer completeness.** M really lacks kerb/yard/gate; the three pointers really are narrower than M's; all eight rows stay BLOCKED, 80 stay CANDIDATE, 0 are SIGNED; the digest is null and all four signature fields are null. The corrigendum's proposed replacement owner for kerb/yard/gate is overstated (m-1). |

## 4. Findings

### m-1 — MINOR — Correct fact, imprecise replacement attribution for kerb/yard/gate

**Where:** `700:§D`, second bullet: "`kerb`, `yard`, `gate` appear as supplemental design exclusions in other proposal wording but are **not** literally listed in M `/H1/must_not`; distinguish them from explicit owner-directions/approved design rather than attributing to the manifest."

**Verified:** M `/H1/must_not` really contains none of the three, so the correction against `705:L47` and `705:L35` is right.

**Not verified, and partly wrong:** they are not "explicit owner-directions or approved design". Where they actually occur is DRAFT author proposal wording — P688 line 13 ("No road, gate, sign, vehicle, machinery, kerb, raised path, stumps…") and G691 WF-06 ("No parked or permanent vehicle, yard, gate or road"). G691 is itself a DRAFT four-file result that [R702] returned NEEDS CHANGES, and D-085 OD-06 says only "no permanent parked trucks or equipment". D-085 never says "no kerb", "no gate" or "no yard"; D-083 uses "closed-yard" for the abstract collection *event*.

**Required wording:** say "author-proposal exclusions in DRAFT P688/G691, not owner directions and not M `/H1/must_not`", and attribute to owner direction only the items D-085 actually states. Keep the instruction not to attribute them to M.

**Why it is material enough to record:** the corrigendum's purpose is precise source attribution, and the successor will be used by the owner. A successor that credits "no gate/yard/kerb" to owner direction would repeat the same class of error in the opposite direction.

### m-2 — MINOR — The proposed pre-attestation lane overstates what source art can close and omits two W §12 pre-production gates

**Where:** `700:§C`: "…restricted to isolated first-party art/scene manifest source inputs necessary to **close B-W14/B-W13/B-W02**"; and its precondition list: "approved applicable graphics/parent/planting spec (**or separately accepted content-only prototype authority**), D-032 rights/storage and public repo placement, exclusive one-writer source branch and physical Ops preflight, exact permitted files, native/isolated verification scope, and explicit Manager [APPROVED TO START]…".

**Two precision problems:**

1. **B-W02 cannot be closed by authoring bytes.** Row 10 (B-W02-RIGHTS) needs the D-032 owner rights/storage disposition, which #708 reviewed as still **D-032 OPEN** with the repository PUBLIC and licence null. Row 12 (B-W02-SCALE) needs all-LOD size/defect fidelity, which source-only art bytes do not establish. Rephrase the lane objective as supplying the source inputs needed to *progress* B-W14/B-W13 and the art side of B-W02, while rights closure stays with the owner D-032 decision and scale closure stays with its own all-LOD witness.
2. **The alternative to an approved spec, and the missing save decision.** W §12 pre-production requirements include "formally **APPROVE** the relevant Compact specs" and "obtain **separate signed save-policy/serialization/CG-TRIG decisions**". `§C` offers "or separately accepted content-only prototype authority" as an alternative to the approved spec and does not mention the save-policy/CG-TRIG decisions at all. Either keep both W §12 requirements inside the lane, or state explicitly that the owner is being asked to record a decision that supersedes W §12 for this bounded lane only.

**Not a permission finding:** the lane is labelled NOT APPROVED, requires user approval and Manager [APPROVED TO START], and names no authorised files. This is scope wording, not leaked authority. Fix it in the successor before the owner is asked to choose.

### m-3 — MINOR — W §11.1 is paraphrased with two limbs it does not contain

**Where:** `700` "Evidence and precise governing boundary": "Frozen #676 whole-world dossier [W] §11.1 says signature accepts exact nominated content/recipes/geometry/calibration input, **not gameplay or native acceptance, save-owner mapping, spec/source start or Windows**."

**Verified:** W §11.1 says "not **measured ecology, native acceptance, source-start/spec approval or save-owner mapping**". It does not say "gameplay" and does not say "Windows".

**Where the extra limbs come from:** the Windows exclusion is real but lives in M `/attestation/would_attest` ("release physical/source/Ops/Windows gates") and W §12 ("separate explicit owner Windows-build permission"). "Gameplay" is a fair gloss on measured ecology/native acceptance but is not the frozen wording.

**Required wording:** quote §11.1 verbatim, then cite M `/attestation` for the Windows limb. Since this corrigendum exists to correct attributions, its own quotes should be literal.

### Notes

- **N-1 — D-085's own margin label is loose.** D-085 OD-08 B calls +0.1209757 m the "core-plus-verge margin", but that number is distance minus **0.9 m**, i.e. core 0.6 + verge 0.2 + guard radius 0.1 — the *outer* margin. `700:§D`'s relabel to "outer path/guard margin" is arithmetically correct; present it as a label correction so the owner comparing +0.1209757 with M `core_verge_guard_margin_lower_m` 0.1029378 is not left to infer the discrepancy.
- **N-2 — The two-group successor checklist is a sequencing summary, not a replacement content list.** Carry forward explicitly the #705 items the corrigendum does not supersede: the 347 dependency witness (333 Git objects + 14 raw-local entries), the roster-only digest `dac06a3b…cca1`, the cell checksum `2441926a…6123` marked "not whole-world", the reserved identity tuple and profile/registry binding, and the exact canonical byte recipe. Otherwise the summary can be mistaken for the complete signing input set.
- **N-3 — Add an evidence floor to §A.4.** The open question "isolated, non-Unity source previews versus controlled Editor" should say that it may only *narrow*, never lower, the witness each row's own definition requires — in particular all-LOD fidelity for row 12, imported material/shader binding for 30/44/73–75, and authored scene bytes for 83. `§A.4` already forbids closure by proposal, estimate or Git presence; this closes the remaining gap.
- **N-4 — Pointer note is incomplete.** Rows 12/30/73–75 are correctly described. Row 44's pointer is `/H1/surface` (narrower than `/H1`) and row 83's is `/canonical_bytes`; a successor pointer table should cover those too.
- **N-5 — The public-repo/binary disclosure is independently corroborated.** Live board records #708: repository **PUBLIC**, licence null, chosen Sitka raw FBX/PNG publicly readable, 104 selected files ≈107.7 MiB, total binary formats ≈1,118 MiB. I reproduced the public read and counted 416 tracked `.fbx` at `8617587e` (Sitka FBX family ≈93.96 MiB). This is evidence for disclosure, not a rights decision, and it strengthens rather than corrects `§A.2`.

## 5. Not verified by this review

Unity, Editor, compiler, import, render, collision, walking, native cell readback, gameplay, save/load, package-cache shader mapping, FBX decoding, rights or licence existence, D-032 disposition, the [P698] guard construction, a full re-read of [P698] and [R702], and the 347-entry dependency byte chain (I re-witnessed frozen file identity and the roster digest, not every dependency blob). No harness, archived script or Player was run. Dashboard and Drive synchronization unverified. `ops/workflow-foundation-v1` coordination copy deliberately unused as authority.

## 6. Outcome

**Verdict: APPROVED DOCUMENT CORRIGENDUM (documentary quality only) — 0 major, 3 minor, 5 notes.** The corrigendum does what #709 asked: it separates content attestation from later gameplay/save qualification, keeps OD-16 deferred, keeps the necessary content-only rights/asset/scene/native witnesses, states the §12/B-W14 paradox as an owner decision and STOP, corrects the B2 figure, corrects the H1 manifest attribution and labels the narrower manifest pointers. The three minors are precision fixes inside the corrigendum, not gate-order defects, and none blocks use of the successor.

**Nothing was promoted.** World state remains **SIGNED 0 / CANDIDATE 80 / BLOCKED 8**, 36 CANDIDATE cells, `world_digest = null`, all four signature fields null, D-032 OPEN, graphics/parent/planting DRAFT. The proposed pre-attestation lane remains **unauthorised and un-approved**. No spec is approved and no scene, asset, source, save, Ops or Windows permission results from this review. The first owner decision remains whether to allow scoped pre-attestation art/scene content bytes or to revise the signing boundary; this review does not choose it.

```text
INTEGRATION DECISION (REVIEWER RECORD — DOCUMENT ONLY)
Status: APPROVED
Gate: G2 documentary evidence acceptance for the proposed corrigendum only.
      G1, G3, G4 and G5 NOT exercised. No canon, integration, spec or lane change.
Reviewed branch: docs/sen-1-700-manager-intake-20261117-v1
Reviewed HEAD: b60bc89125290ee25a0a8db23a74002be88301fd
Review branch / HEAD: review/sen-1-711-attestation-gate-order-nonopenai-20261117-v1 (this result commit)
Context commit: d435292b29e7e3c987e3df57d7b3555d82e84a8e
Reasons:
- Signature scope/order split matches W §11.1, W §11.2, D-085 and #709 M-1.
- OD-16 remains owner-deferred, with no threshold invented and no signature precondition.
- W §12 / B-W14 circularity is an explicit owner decision / STOP with no granted permission.
- B2 +0.1209757 vs +0.1029378 and the 0.0180 m / 14.9 % delta reproduced; T1 cannot move B2.
- M /H1/must_not has no kerb/yard/gate; three narrowed pointers confirmed; eight rows stay BLOCKED.
Required fixes (non-blocking, successor wording):
- m-1: attribute kerb/yard/gate to DRAFT P688/G691 author proposal, not owner direction.
- m-2: lane supplies inputs for B-W02, does not close it; keep W §12 spec-approval and signed save-policy gates or record an explicit superseding owner decision.
- m-3: quote W §11.1 verbatim; cite M /attestation for the Windows limb.
Integration target: Manager #700 successor checklist; no integration performed by this review.
SPEC CONFORMANCE: N/A (PACKET ONLY)
```

**Handoff:** [#711](https://github.com/jermur788/CCF/issues/711) HANDED-OFF for Manager #700. Owner action after this review remains the separate signing-boundary decision. Dashboard/Drive **unverified**.
