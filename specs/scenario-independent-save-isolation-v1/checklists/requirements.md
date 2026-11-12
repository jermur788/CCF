# Independent Scenario Save Isolation V1 — Requirement/Acceptance Checklist

**Status: DRAFT — NOT APPROVED — NO IMPLEMENTATION AUTHORITY.**
All boxes are **unchecked**; writing this checklist is not verification. Execution is blocked until specification approval, new source ownership/claim, and relevant Unity gates. Evidence must name exact candidate, fixture origin/version, before/after world/file comparison and failure conditions.

Legend: **S** static/source, **O** offline deterministic/negative, **U** later Unity/Editor Tier 2, **A** archived/reference integrity. Tier 1 FAST for isolated S/O; Tier 2 FEATURE for integrated load/scene/IO; Tier 3 only later for a nominated release candidate.

## A. Slot selection and preservation

- [ ] **ISO-01 [S/O/U]** O1 selected save path remains exactly Application.persistentDataPath/forest-save.json, not renamed or moved; pre-existing O1 file and .bak/.tmp are not rewritten by selecting/starting O2.
- [ ] **ISO-02 [S/O/U]** O2 uses one fixed distinct path, proposed forest-save-scenario-two.json, with .tmp/.bak derived from only that path. No collision, cross-slot temp or backup promotion, path traversal, user-supplied file key or implicit O1 fallback.
- [ ] **ISO-03 [U]** New O1 and O2 choices are explicit and select the correct authored forest/definition and save slot; fresh games do not delete a prior save. Later Save of a chapter requires truthful same-slot replacement confirmation.
- [ ] **ISO-04 [S/O/U]** F5, F9, F10, startup Continue/New, and any direct public LoadData/scene selection routes share the selected identity and preflight contract; no bypassing entrypoint. Same-file time-of-check/change is revalidated.
- [ ] **ISO-05 [O/U]** With only O1 on disk, O2 "Continue" is unavailable with an accurate no-O2-save reason while O1 remains available; with only O2, the converse holds; both-present and neither-present cases are correct.

## B. Identity/version rejection BEFORE state mutation

- [ ] **ISO-06 [S/O/U]** A valid new save stores an explicit allowlisted chapter ID; raw JSON key presence/type and parsed value agree; no JsonUtility-created default is misread as an explicit saved ID.
- [ ] **ISO-07 [O/U]** Copied/renamed O1 save placed in the O2 slot refuses, even when O1 and O2 contain equal tree IDs/year numbers; vice versa for O2 in O1.
- [ ] **ISO-08 [O/U]** Both CanLoad and actual Load and direct LoadData reject an explicitly foreign ID, O1/O2 nested ID conflict, wrong target definition or selected chapter, **before** ecology geometry mutation, tree destruction/spawning or management restore.
- [ ] **ISO-09 [O/U]** Missing/null/empty/whitespace/unknown/case-variant scenario ID, duplicate/conflicting raw JSON ID fields and malformed identity envelope reject for new format; v1–v9 O1-only historical missing ID follows the documented narrow exception and is never O2.
- [ ] **ISO-10 [O/U]** Authentic O1 v10–v24 old saves with available valid identity load under version-aware compatibility; missing nested identity is not blindly assumed authentic. Unknown provenance fails closed rather than being repaired into O2 or O1.
- [ ] **ISO-11 [O/U]** Unsupported newer schema/version, invalid model, malformed JSON, valid "{}" default-deserialisation trap, truncated file, duplicate tree IDs, invalid species, invalid cell indices and inconsistent serialized state refuse. Do not silently drop or relabel unknown species or future fields.
- [ ] **ISO-12 [O/U]** Wrong-load and corrupt-load refusal leaves **all** inspected sentinels identical: active scene and selection, geometry/track, species/trees/marks/pruning/deadwood/juveniles/cells, carried and stored timber, cash, inventory, Work Plan approval/pending orders, execution choices, ecological year, annual reviews, management-event IDs/history/objective achievements, browsing/protection, soundscape and visible presentation.
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
- [ ] **ISO-24 [S/O/U]** Unknown-field/future-version handling is conservative: no unsupported newer save loads best-effort or autosaves destructively; acceptable old formats preserve documented field/default semantics.

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

## Acceptance status

**NOT EXECUTED. 0 checks marked passed.** The spec package must first be reviewed and approved; engineering/Unity work requires separate claims and ownership. Any requirement not supported by genuine fixture/source/runtime evidence remains **UNVERIFIED**, not presumed passed.
