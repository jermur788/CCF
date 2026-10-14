# Exact UX follow-up549eae3 source review

**RESULT B — bounded authorised UX source accepted as the qualification candidate, with stated follow-ups/limits.** Candidate branch `ux/scenario-one-small-fixes-84e881a`; exact candidate HEAD `549eae3318f1bb4d01a2853384b58f282f7f4e73`. This review output branch is separate; its new documentation/tooling commit is not a replacement production candidate. No merge/integration or canonical publication. **Unity run by reviewer: NO.**

## Ancestry and complete diff

`84e881aa6d0e96c5752f09492d5db37d2cc8925e` is an ancestor of exact549eae3. Complete24-file diff inspected, with source-file SHA256 inventory and full-diff digest in Evidence/source-review.json. Production changes are precisely five UI files; everything else is Docs/Verification or Tools/Verification. No ecology/growth/density/species/manager gameplay/economy math/save/objective/scenes/prefabs/ProjectSettings/Packages delta. No unrelated mechanic was found; ResultC exclusion is not triggered.

| Production file | Change | Review |
|---|---|---|
| UI/Resources/ScenarioOneUi.uss | HUD heading wraps; stacked sentence-value rows; inspection cap72→80%; separate estimate-panel styles | Matches count/overflow/Deadwood layout scope; no resolution-policy change |
| UI/ScenarioOneUiRoot.cs | Attach EstimatePanel; share side-panel placement with competitor panel | Display-only wiring, no marking/approval/advance/save change |
| UI/TreeInspectionView.cs | Move existing timber estimate text into separate right-side panel | Same estimate/market predicate and text; Crop Tree assessment returns before estimate so panels are mutually exclusive |
| UI/UiKit.cs | Optional valueBelow=false argument; stacked CSS class if true | Existing callers retain default behaviour; no state mutation |
| UI/WorkPlanOverview.cs | Pass valueBelow:true for Deadwood sentence | Same names/numbers/calculation, presentation placement only |

Harness changes: source UX qualification files/evidence inherited fromcc54d6a, added inspection/HUD/Deadwood/control-scroll checks, fresh-device startup display/state assertion, scroll-aware viewport/reachability/back-button checks, explicit P3 presentation expected-hash update in run_s1a_gates.py and S1A_README. Startup checks retain hidden forest/state/hash/no-disk-save assertions; scrolling rows are checked for reachability and final row/Back visibility rather than impossible all-at-once visibility. No acceptance path deletes a failed ecological assertion.

## Independent P3 proof

P3 gate source hashes `leaving + "\n" + money`, built from displayed labels, using FNV-style64-bit char iteration. It is not a hash of simulation/tree/save state. The wording change originates at84e881a; the follow-up changes its explicit expected presentation hash and keeps layout text neutral.

- Old expected text hash: **F7C2FC966816BBAD**.
- New exact text hash: **AF7405918195F0DE**.
- Read-only world hash in retained gate markers: **EBE7A228F630AE41**, unchanged.
- Retained residual values:336→306 trees, BA41.10→36.23, volume39.87→34.68.

Reviewer independently decoded `P3_HASHED_TEXT_B64` from existing `Build/U1Gates/p3-proof/p3-proof.log` without running Unity. The exact displayed text is now retained in Evidence/p3-hashed-text.txt. Python port iterates UTF16 code units to match C# char hashing. It reproducesAF7405918195F0DE. Replacing **only** the new residual-stand/basal-area subtitle with its pre-UX wording produces exactlyF7C2FC966816BBAD. All other hashed text remains byte-for-byte the same. No simulation/save code changes occur in84e881a→549eae3. This establishes a presentation-derived change without silently rebaselining ecological/reference anchors. The branch itself declares the expected-hash change explicitly.

Reproduce: `python3 Tools/Verification/ScenarioOneUx/Review/check_followup.py --out <external.json>`. Review covers exact pinned commits, not a moving branch.

## Tests and attributed qualification evidence

This review ran source ancestry/complete-path/diff/ordinal-hash checks, offlineUX176/176, and production approximate type-check with7 known baseline errors/no new errors. It did not run Unity/render/input/performance. Original549eae3 handoff reports **A on Linux Editor**:191 hard checks, source-specific1280×720/1920×1080 renders/virtual input and corrected startup gates; that remains the original worker’s evidence. It does not qualify later integrated growth/completion/UI source. Windows, physical mouse and Player are explicitly unverified.

Minimum supported external demo remains1280×720;1920×1080 required;1024×768 compatibility-only. The source did not redesign the compatibility size or promote it to the external minimum. Text-size/fit evidence and ignored screenshot locations are indexed by the source handoff; no new screenshot/runtime claim here.

## Review flags / later qualification

1. `make_startup_diagnostic.py` is an obsolete one-off generator after the startup assertion wording changed: its exact-string count assertion fails against the final source. It is not the authoritative startup gate and has no production effect. Keep as historical diagnostic or retire/update in a separate tooling packet; do not use it as current acceptance evidence.
2. Worst-case pruned-Oak inspection layout has about8–10 reference units to the HUD forecast in source evidence. Recheck after calibrated growth/full-loop copy; source fit is not a durable guarantee against added rows/wrapping.
3. Qualification runner exempts `.vscode/settings.json` and broad SectionFive .mat-path rewrites when recording unexpected changes. Before future cleanup, inspect actual diffs against known normalisation and preserve user state; do not treat every material change as harmless automatically.
4. Source input was virtual/callback-based and private-display Editor; later physical input, clean Player/performance and native Windows remain mandatory. Integrated N3 changes acknowledgement-footer availability, so rerun legacy/new review UI and completion summary explicitly.
5. Existing source caveats remain: partial approvals leave pending work silently; H-5 species-objective explanation is still open. This review does not change gameplay/copy beyond authorised scope.

## Candidate disposition and integration hold

Designate exact549eae3318f1bb4d01a2853384b58f282f7f4e73 the **reviewed UX qualification candidate** for the specified fixes/P3 handling. Source is local branch; pushed review branch carries that exact source in its ancestry, making the commit retrievable without moving another worker’s branch. This bounded acceptance does not mean the full external demo is qualified. DensityU3 needs Manager acceptance of itsB flags; exact production growth and N3 candidates need their later qualification; no final integration branch was created. Apply approved sources only through assigned integrator and full Job7 gate sequence. No canonical publication until integrated evidence exists.

## Complete changed-file inventory

- `Assets/ForestPrototype/UI/Resources/ScenarioOneUi.uss` — source SHA256 `b85ec1db613736bf820ecc610967bb1cfa4a199944878bc344e9e0d71d28b745`.
- `Assets/ForestPrototype/UI/ScenarioOneUiRoot.cs` — source SHA256 `47acf7a2b788598ad347503dd7d4a800e287d7353f7b8c7eaaf01de14d1c60e7`.
- `Assets/ForestPrototype/UI/TreeInspectionView.cs` — source SHA256 `49929d8e31faa4522d202fa1d00a53f9ac5615acdfb9eb0d06e2b0d6b83bca60`.
- `Assets/ForestPrototype/UI/UiKit.cs` — source SHA256 `89c0f016b2f482df7528bbdf1c342928cf594873bbcd9ef23d715a2789de210d`.
- `Assets/ForestPrototype/UI/WorkPlanOverview.cs` — source SHA256 `279c04daf63d61aff3237d66e20eb416e50ba6e420101ada2590caa69ad979bb`.
- `Docs/Verification/ScenarioOneUxFixes/Handoff.md` — source SHA256 `f7de40ac39668c976eefdc29ea8097dda5b9d8e1529e4e640a83197d43750765`.
- `Docs/Verification/ScenarioOneUxFixes/evidence/before-fix-failures.txt` — source SHA256 `511d3290c17323d2b2bf25b6695a8b9dd85b5c38ae2419e61bc9509eed79fe3c`.
- `Docs/Verification/ScenarioOneUxFixes/evidence/final-hard-checks.txt` — source SHA256 `2fc1e3632e8657de50a73266900e934c36f236ce0e12cbe798e60d6c8c393d2a`.
- `Docs/Verification/ScenarioOneUxFixes/evidence/p3-anchor-proof.txt` — source SHA256 `05d31dc771f908ff1a23629cef77e9f7463dd2a193a321da3f5643fadfc2eacc`.
- `Docs/Verification/ScenarioOneUxFixes/evidence/text-sizes.csv` — source SHA256 `46b31398a1e42046f76fe5991ff3685b6eafdc0704eb61e66c03ef9e56f53a14`.
- `Docs/Verification/ScenarioOneUxQualification/Handoff.md` — source SHA256 `e0676738c09e52ae99dafd2eee49a28844a586f345aa58d801e90bcde74e5b6a`.
- `Docs/Verification/ScenarioOneUxQualification/README.md` — source SHA256 `672807d70a7f69b0d77e7b2d352e56569bee3048a78baf041e524618722acd9f`.
- `Docs/Verification/ScenarioOneUxQualification/evidence/gate-results.md` — source SHA256 `edc06d9bd8bcce02b8fff10decb3fb2f720535b504a0a7bb5191d54ee28b3c70`.
- `Docs/Verification/ScenarioOneUxQualification/evidence/harness-checks.txt` — source SHA256 `24b524878a8ac55baa82896f9e9676e6d7e4e60816311faa8031fbe452fec6ed`.
- `Docs/Verification/ScenarioOneUxQualification/evidence/perf-cpu-only.txt` — source SHA256 `e64cf106e56749ca997ce4ff8d7d602f28458fc5fdb25abc5f8f00fffe10df8b`.
- `Docs/Verification/ScenarioOneUxQualification/evidence/startup-gate-diagnosis.md` — source SHA256 `513e053b56c30d4aecbc7febea358c0c563ff6030fb7c590fa541b72ac3a2308`.
- `Docs/Verification/ScenarioOneUxQualification/evidence/text-sizes.csv` — source SHA256 `07ef0ad45907fab3aadfad9afe96951d4a257016ad29d070b3eb690607057985`.
- `Docs/Verification/ScenarioOneUxQualification/evidence/ui-screenshots-index.md` — source SHA256 `ad2c0ac4c7fa2ca9c9b7b01190289a19551a640485239de084ada5e89697eb28`.
- `Tools/Verification/S1A_README.md` — source SHA256 `ade9fe2b637a3ecb4bdab8e08da12216b8976080f6bb9cbd67e38fc702d988a6`.
- `Tools/Verification/ScenarioOneUx/Qualification/UxQualification.cs` — source SHA256 `2d63e622a808b502dfd6b93f2539e81a0f64ae929352aa6815d309aeb7f6e27a`.
- `Tools/Verification/ScenarioOneUx/Qualification/make_startup_diagnostic.py` — source SHA256 `f0e9613ff7204cd97c98b7a21f376578a0c085c2ee26fd00f00c9ff1e30fe92a`.
- `Tools/Verification/ScenarioOneUx/Qualification/run_qualification.py` — source SHA256 `ef00a663d6949cb9c5021143c2406690da57dea351655cc2bf62c7c76567b53d`.
- `Tools/Verification/Startup/StartupScreenVerification.cs` — source SHA256 `905d6b062300f128daad4b181f7f6ad663c202ae7161d69e1395922520098d8f`.
- `Tools/Verification/run_s1a_gates.py` — source SHA256 `8ed9d3c26b366920921e0dab0c89950e67f34eefedb8d69c28584d62873fd72b`.
