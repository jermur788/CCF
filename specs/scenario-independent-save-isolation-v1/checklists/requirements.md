# Independent Scenario Save Isolation V1 — Requirement/Acceptance Checklist

**Status: DRAFT — NOT APPROVED — NO IMPLEMENTATION AUTHORITY.**
**Amended by #524 (14 November 2026) for three profiles: historical O1, compact O1 (PROPOSED), O2.** The original ISO-01–ISO-36 keep their numbers and order and stay unchecked; contradictions are revised in place and tagged *(amended #524)*. New items ISO-37–ISO-63 are appended. All boxes are **unchecked**; writing this checklist is not verification. Execution is blocked until specification approval, new source ownership/claim, and relevant Unity gates. Evidence must name exact candidate, fixture origin/version, before/after world/file comparison and failure conditions. Every new literal named below (compact/O2 filenames, IDs, envelope, prefixes) is **PROPOSED / NEEDS APPROVAL** (spec section 8.2 B-01). **#530 bounded repair (15 November 2026):** only ISO-03 (F5 confirmation conditional on OPEN Q-03) and ISO-49 (raw-absent vs explicit 0/negative version cases) are reworded, tagged *(amended #530)*; no item is renumbered, added, removed or checked.

Legend: **S** static/source, **O** offline deterministic/negative, **U** later Unity/Editor Tier 2, **A** archived/reference integrity. Tier 1 FAST for isolated S/O; Tier 2 FEATURE for integrated load/scene/IO; Tier 3 only later for a nominated release candidate.

## A. Slot selection and preservation

- [ ] **ISO-01 [S/O/U]** O1 selected save path remains exactly Application.persistentDataPath/forest-save.json, not renamed or moved; pre-existing O1 file and .bak/.tmp are not rewritten by selecting/starting O2 *(amended #524)* or compact O1.
- [ ] **ISO-02 [S/O/U]** O2 uses one fixed distinct path, proposed forest-save-scenario-two.json, with .tmp/.bak derived from only that path. No collision, cross-slot temp or backup promotion, path traversal, user-supplied file key or implicit O1 fallback. *(amended #524: the same holds for the proposed compact slot; see ISO-37.)*
- [ ] **ISO-03 [U]** New O1 and O2 choices *(amended #524: historical O1, compact O1 and O2 choices)* are explicit and select the correct authored forest/definition and save slot; fresh games do not delete a prior save. Later Save of a chapter writes only the selected profile's own slot, with truthful status and no cross-profile overwrite. *(amended #530, conditional on OPEN Q-03:)* the existing F10 menu save keeps its replacement confirmation when that profile's file exists; source-current F5 saves without a prompt [SOURCE OBSERVED]. The **F5-specific** confirmation clause is evaluated only against the recorded Q-03 ruling: if Q-03 requires an F5 prompt, an F5 overwrite without one FAILS; if Q-03 rules no F5 prompt, an unprompted F5 same-slot save PASSES this clause. Until Q-03 is recorded, that clause is neither PASS nor FAIL; the same-slot, truthful-status and F10-confirmation parts still apply.
- [ ] **ISO-04 [S/O/U]** F5, F9, F10, startup Continue/New, and any direct public LoadData/scene selection routes share the selected identity and preflight contract; no bypassing entrypoint. *(amended #524: plus preview entry/return and backup recovery, one shared preflight; see ISO-50.)* Same-file time-of-check/change is revalidated.
- [ ] **ISO-05 [O/U]** With only O1 on disk, O2 "Continue" is unavailable with an accurate no-O2-save reason while O1 remains available; with only O2, the converse holds; both-present and neither-present cases are correct. *(amended #524: extended to all eight three-slot presence combinations in ISO-39.)*

## B. Identity/version rejection BEFORE state mutation

- [ ] **ISO-06 [S/O/U]** A valid new save *(amended #524: a new-profile compact or O2 save, inside its envelope; historical saves gain no field)* stores an explicit allowlisted chapter ID; raw JSON key presence/type and parsed value agree; no JsonUtility-created default is misread as an explicit saved ID.
- [ ] **ISO-07 [O/U]** Copied/renamed O1 save placed in the O2 slot refuses, even when O1 and O2 contain equal tree IDs/year numbers; vice versa for O2 in O1.
- [ ] **ISO-08 [O/U]** Both CanLoad and actual Load and direct LoadData reject an explicitly foreign ID, O1/O2 nested ID conflict, wrong target definition or selected chapter, **before** ecology geometry mutation, tree destruction/spawning or management restore.
- [ ] **ISO-09 [O/U]** Missing/null/empty/whitespace/unknown/case-variant scenario ID, duplicate/conflicting raw JSON ID fields and malformed identity envelope reject for new format; v1–v9 O1-only historical missing ID follows the documented narrow exception and is never O2 *(amended #524)* or compact.
- [ ] **ISO-10 [O/U]** Authentic O1 v10–v24 old saves with available valid identity load under version-aware compatibility; missing nested identity is not blindly assumed authentic. Unknown provenance fails closed rather than being repaired into O2 or O1. *(amended #524: the final rule for a historical-slot v10–v24 file with the raw nested scenarioId key absent is BLOCKED on owner/save decision B-04. Current source accepts it; this item is evaluated against the recorded B-04 ruling and may neither silently tighten nor silently ratify it.)*
- [ ] **ISO-11 [O/U]** Unsupported newer schema/version *(amended #524: for compact/O2 only; genuine historical O1 newer versions keep warn/best-effort per ISO-47)*, invalid model, malformed JSON, valid "{}" default-deserialisation trap, truncated file, duplicate tree IDs, invalid species, invalid cell indices and inconsistent serialized state refuse. Do not silently drop or relabel unknown species or future fields.
- [ ] **ISO-12 [O/U]** Wrong-load and corrupt-load refusal leaves **all** inspected sentinels identical *(amended #524: plus active profile, all learning/help PlayerPrefs namespaces and all three slots' bytes; see ISO-54)*: active scene and selection, geometry/track, species/trees/marks/pruning/deadwood/juveniles/cells, carried and stored timber, cash, inventory, Work Plan approval/pending orders, execution choices, ecological year, annual reviews, management-event IDs/history/objective achievements, browsing/protection, soundscape and visible presentation.
- [ ] **ISO-13 [U]** Startup Continue refuses without prematurely closing the screen/dismissing teaching; direct in-forest load refusal does not yield false success or switch worlds. A later failed scene transition cannot leave a partially restored target in place.

## C. Old Scenario One and frozen archive invariants

- [ ] **ISO-14 [O/U]** Genuine historical O1 save versions **v1 through v24** can be read by their intended old-format semantics; missing scenarioOne in v1–v9 remains supported; no data rewrite simply from loading. Record fixture provenance for every sampled version, and report any unavailable fixture instead of claiming blanket empirical proof.
- [ ] **ISO-15 [O/U]** Legacy40 old saves use their original 40×40 geometry/cell indices; Enlarged80 saves retain their saved 80×80 geometry/stand approach and five-parallel-rack constraints. No O2 selection changes O1 geometry policy.
- [ ] **ISO-16 [O/A/U]** Frozen Reference Future v1 archive bytes, schedule, milestone and world hashes remain unchanged; authenticated v12 preview at 0/20/50/100 and return to current O1 world work; no new scenario ID inserted into frozen records.
- [ ] **ISO-17 [O/U]** Current O1 v21 Rack V2, v22 objective history, v23 prescription and v24 first guided rack review persist correctly, without reset/backfill/hash drift; O1 cash, annual reports, repeated work and world identity round-trip accurately.
- [ ] **ISO-18 [O/A]** Before/after byte/hash manifest of real O1 slot and historical fixtures proves no unsolicited migration/renaming/deletion; existing protection fixtures/baselines are not rewritten to match altered outputs.

## D. Save transaction, backup and recovery

- [ ] **ISO-19 [O/U]** New O2 first Save, second Save and reload use O2 canonical path; O1 current/backup bytes remain unchanged, and O2 save does not import O1 wood, trees, orders, economy or age.
- [ ] **ISO-20 [O/U]** Interrupted temp write, failed rename/replace, unsupported File.Replace fallback and write permissions/read-only storage report failure and retain the last valid slot or an explicitly recoverable previous version; no "saved" false positive or corrupt current/backup due to cleanup.
- [ ] **ISO-21 [O/U]** A malformed primary with valid **same-scenario** .bak offers accurate opt-in recovery only after full identity/version/content preflight; no automatic substitution. Before explicit recovery both file bytes persist untouched. Recovery itself is safe and does not destroy the only good copy.
- [ ] **ISO-22 [O/U]** Malformed/foreign .bak, valid primary/foreign .bak, backup in wrong chapter slot, stale .tmp, and both primary and backup invalid produce no cross-load, no silent backup promotion and no destructive cleanup.
- [ ] **ISO-23 [S/O/U]** Snapshot of source and target identities is validated before the save writes .tmp; no live world from scenario A can be written into B's slot (including after chapter switch and after Reference Preview).
- [ ] **ISO-24 [S/O/U]** Unknown-field/future-version handling is conservative: no unsupported newer save loads best-effort or autosaves destructively *(amended #524: "no best-effort" applies to compact/O2; for genuine historical O1 the existing load-only warn/best-effort is preserved, and the "never autosave destructively" part applies to every profile)*; acceptable old formats preserve documented field/default semantics.

## E. Independent chapters and phased O2 status

- [ ] **ISO-25 [U]** O1 and Gate 1 O2 start as genuinely different authored worlds; O2 contains real irregular/mixed-age cohort evidence, not a renamed O1 stand. Actual O2 scene/spec integration is separately approved.
- [ ] **ISO-26 [U]** Each chapter independently advances multiple ecological years including a work and no-work year, logs a Work Plan decision, persists age/annual review and history, leaves/returns/loads its slot, and recovers the same chapter facts.
- [ ] **ISO-27 [O/U]** Cross-world equal or colliding tree IDs, work-order IDs, management-event IDs, cell indices and year labels do not imply shared objects, money, inventory, objectives or history. Duplicates **within** a save still reject under existing integrity rules.
- [ ] **ISO-28 [S/U]** Gate 1 result is explicitly **INTERNAL ENGINEERING PROOF ONLY**, never external/player-facing O2 completion or an O1 personal playtest surrogate. No new species declared ready by this work.
- [ ] **ISO-29 [S]** Gate 2/complete O2 still requires Sitka, sessile oak, beech, Scots pine, downy birch, common alder, with the latter three separately qualified as real site-appropriate persisted/visualized educational biology; currently HOLD. No silent roster reduction.
- [ ] **ISO-30 [S]** No generic eight-slot engine, estate-local O8 finance, O2 species model, new O1 racks, global progress, Work Plan redesign, gameplay economy transfer, Windows build or Reference Future rewrite sneaks into this feature.

## F. Evidence, review and release

- [ ] **ISO-31 [S]** Static inspection enumerates every mutation-capable load entrypoint and confirms preflight sequencing. Actual source paths/functions and unresolved edge cases are cited; no test-run claims from code inspection alone.
- [ ] **ISO-32 [O]** Focused offline negative cases and legacy fixtures pass on the exact implementation candidate; failure and original baseline defects are reported, not hidden by weakening tests.
- [ ] **ISO-33 [U]** Exact-head Tier 2 Editor qualification checks active scene, authored O2 identity, save/reload both slots, cross-load refusal/atomicity, annual review, recovery and Reference Preview; input/rendered observations are captured where relevant. One focused feature qualification rather than blanket Tier 3 per packet.
- [ ] **ISO-34 [S/U]** Independent reviewer examines persistence, identity and legacy test evidence, issues APPROVED/NEEDS CHANGES/REJECTED; Manager separately decides integration and verification. No inference from pushed files alone.
- [ ] **ISO-35 [S]** Every production packet has approved spec, disjoint branch/worktree, owner and live lock verified with #424, claim [RUNNING], exact candidate, allowed paths, appropriate Unity lease, GitHub Markdown handoff and [HANDED-OFF] closure. No Windows build before user personally tests O1 and explicitly authorises one.
- [ ] **ISO-36 [S]** Final handoff states passing/failing/unknown checks, next action, user action, dashboard and Drive status. No approval or completed O2 assertion from DRAFT docs.

## G. Three-profile registry and slots (added #524)

- [ ] **ISO-37 [S/O]** One fixed compile-time table maps exactly three profiles to three slots: historical forest-save.json (unchanged), proposed forest-save-scenario-one-compact-v1.json and proposed forest-save-scenario-two.json. Each .tmp/.bak derives only from its own primary; all nine paths are pairwise distinct, including under case-insensitive comparison; no user-derived names or traversal.
- [ ] **ISO-38 [O/U]** Three-by-three containment matrix: selecting, starting, saving, loading or failing in any one profile leaves the other two profiles' primary/.tmp/.bak bytes identical, by before/after hash.
- [ ] **ISO-39 [O/U]** Continue availability is correct and independent per profile for all eight presence combinations (none, each single, each pair, all three). A missing or corrupt compact/O2 file never hides or alters historical O1 Continue, and vice versa.

## H. Envelope and raw identity (added #524)

- [ ] **ISO-40 [S/O]** A compact (and O2) file requires the envelope marker. Each envelope identity key (schema, chapter, profile, world, geometry, content, payload version) appears exactly once with the stated type and an exact case-sensitive allowlisted value. Missing, null, empty, whitespace, case-variant, duplicate or conflicting keys reject before mutation. The nested management scenarioId equals the envelope chapter.
- [ ] **ISO-41 [O/U]** Genuine historical forest-save.json bytes (any sampled v1–v24) copied or renamed into the compact slot, or into the O2 slot, reject for missing envelope, before mutation.
- [ ] **ISO-42 [O/U]** A compact or O2 envelope file copied into the historical slot rejects as foreign **before** any historical version or best-effort rule and before mutation.
- [ ] **ISO-43 [O/U]** Compact↔O2 cross-placement rejects in both directions, including when payload tree IDs, work-order IDs and years coincide.
- [ ] **ISO-44 [O/U]** A compact envelope whose payload geometry is Legacy40, Enlarged80 or unknown, whose content key or world ID belongs to another profile, or whose geometry key is unrecognised, rejects. Unknown compact geometry never falls through to 40 m / 64 cells or 80 m / 256 cells.
- [ ] **ISO-45 [S/O]** Envelope root keys are disjoint from every ForestSaveData root field name, so an envelope-shaped file cannot deserialise as a plausible historical save (for example, no root "version" that would read as v1).

## I. Profile-specific version policy (added #524)

- [ ] **ISO-46 [O/U]** Compact/O2: a missing, unknown or newer envelope schema or payload save version rejects before mutation, with an "unsupported newer version" or "unreadable" message; never best-effort and never auto-saved.
- [ ] **ISO-47 [O/U]** A genuine historical-slot file with version greater than the supported current version and no foreign marker keeps the existing warn + best-effort load behaviour (not globally tightened). The test proves the existing warning remains and that no automatic save rewrites the file. Any later change requires the separate B-05 decision.
- [ ] **ISO-48 [O/U]** In the historical slot, a raw-present nested scenarioOne.scenarioId not exactly "scenario-one" rejects before mutation (PROPOSED, pending B-03 ratification). A raw-absent nested ID follows the recorded B-04 ruling; v1–v9 without scenarioOne load as today.
- [ ] **ISO-49 [S/O]** Historical behaviour is byte-for-byte unchanged: explicit parsed version ≤ 0 → 1, pre-v20 → Legacy40, v20+ explicit integer geometry on the disk path, v15/v21/v22/v23 normalisers, the v24 one-way latch and model 0 defaults. No new field is written into historical saves, and the serialized output for the same world is identical before and after implementation. *(amended #530: three distinct historical-slot version cases, never merged:)*
  - **raw `version` key absent:** [SOURCE OBSERVED] parses as the initialised CurrentVersion (24) and is validated as v24, so the disk route applies the v20+ raw validators (for example an absent `standGeometryModel` is refused); it is **not** treated as v1. The test asserts current behaviour unchanged, or the later recorded owner ruling if one is made [PENDING OWNER RULE];
  - **explicit `"version": 0`:** [SOURCE OBSERVED] normalised to 1 by CanLoad and LoadData, then read under v1 rules subject to every other validator;
  - **explicit negative `version` (for example -1):** [SOURCE OBSERVED] normalised to 1 exactly as the zero case.

## J. Shared preflight coverage (added #524)

- [ ] **ISO-50 [S/O/U]** One shared preflight serves Start/New admission, Continue/CanLoad, Save/F5, F9, F10 load and save, direct public LoadData, Reference preview entry and return, and backup recovery. CanLoad and Load agree per profile, and a file changed between check and commit is re-validated or refused.
- [ ] **ISO-51 [S/O]** Direct public LoadData cannot load player-supplied bytes without the profile and raw-text checks. Trusted internal modes (captured preview return, hash-verified archive) are enumerated and unreachable with arbitrary player files.

## K. Preview and Reference across three profiles (added #524)

- [ ] **ISO-52 [S/O/U]** Reference Future preview is not offered in compact or O2 unless separately approved (B-07). The compact profile cannot satisfy ScenarioReferenceArchive.IdentityMatches merely by sharing the scenario-one chapter/definition. Historical preview from Legacy40 and Enlarged80 worlds is unchanged; return restores the exact captured world and active profile; F5/F9 remain suppressed during preview.
- [ ] **ISO-53 [O/A]** Envelope/profile metadata never enters archive bytes, historical WorldHash / LegacyCompatibleWorldHash inputs, or the stripping helpers. Any compact anchors are new, separate anchors, never a rebaseline of existing ones.

## L. Failure atomicity and backup across three slots (added #524)

- [ ] **ISO-54 [O/U]** For every rejection in ISO-40–ISO-48, and every save-failure injection per profile, compare: all three primaries/.tmp/.bak bytes; the `CCF.Learning.v1.*` / `CCF.MenuHelp.v1.*` key values; the compact and O2 prefix key values; and the full ISO-12 live-state sentinel, including the active profile.
- [ ] **ISO-55 [O/U]** Per-profile backup: a compact .bak is validated against compact identity only. A historical or O2 file in compact's .bak position (and every other mismatched pairing) rejects; there is no cross-profile promotion, and recovery is explicit only.
- [ ] **ISO-56 [O/U]** A save failure in one profile (temp write error, replace failure, copy fallback interruption) leaves the other two slots' bytes unchanged and shows no "Saved" message.

## M. Learning/help namespaces and compact teaching evidence (added #524)

- [ ] **ISO-57 [S/O/U]** `CCF.Learning.v1.*` and `CCF.MenuHelp.v1.*` values are unchanged by every compact/O2 action, including a compact or O2 learning/help reset. The historical reset does not touch compact/O2 keys. The proposed `CCF.Learning.scen1-compact-v1.*`, `CCF.MenuHelp.scen1-compact-v1.*` and O2 `scenario-two-v1` prefixes are disjoint.
- [ ] **ISO-58 [O/U]** Compact early teaching completion comes only from compact-save evidence:
  - arrival/assessment;
  - four G1 `PlantJuvenile` exact-position orders (2 sessile oak + 2 European beech), each created Pending with owned stock, Approved through the Work Plan and resolved Completed by the real annual resolution, with stable workOrderIds and PlantedJuvenile records;
  - then the first subsequent genuine annual review opened and acknowledged.

  Pending markers, Failed/rejected orders, quotes, prefs, the stage 7 / v24 latch and historical rack keys earn no credit. A midway save/load resumes correctly, and later juvenile death does not erase the record.
- [ ] **ISO-59 [O/U]** Compact teaching evidence is never written to or read from historical saves; compact never sets `firstGuidedRackReviewComplete`; historical saves are never scored by compact criteria.

## N. Provenance, approval and status (added #524)

- [ ] **ISO-60 [S/A]** The fixture manifest labels each fixture as genuine-historical (with build/commit/date provenance) or synthetic, covering v1–v24, Legacy40, Enlarged80, model 0, Rack V2, MFM v23, v24 teaching and Reference Future v1. Compact/O2 fixtures are produced only by the approved candidate. Unavailable versions are reported, not assumed (B-06).
- [ ] **ISO-61 [S]** Every new literal (filenames, profile/world IDs, envelope keys, geometry key/number, content key, prefixes) traces to an APPROVED decision record before it appears in code. The compact profile is not presented as the first external demo until D-082 is accepted.
- [ ] **ISO-62 [S]** O2 remains Gate 1 internal-only with three species; the six-species complete O2 stays HOLD; no Scots pine, downy birch or common alder implementation enters under this save feature.
- [ ] **ISO-63 [S]** Registry immutability: after first shipped use, a material geometry/content/schema change receives a new versioned identity. Display titles and chapter numbers are never routing keys, and only one router/preflight implementation exists (no compact-only save controller).

## Acceptance status

**NOT EXECUTED. 0 checks marked passed.** 63 items (ISO-01–ISO-36 original, ISO-37–ISO-63 added by #524), all unchecked and NOT RUN. The spec package must first be reviewed and approved; engineering/Unity work requires separate claims and ownership. Any requirement not supported by genuine fixture/source/runtime evidence remains **UNVERIFIED**, not presumed passed.
