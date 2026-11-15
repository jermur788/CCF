# scen1 — #530 shared three-profile save DRAFT: bounded repair of #527 M-1, M-2 and m-1

**Result: REPAIRED IN DRAFT. This is a documentation-only change, and the DRAFT is still unapproved.** Three wording contradictions found by the independent #527 review are corrected. These are the only changes. The spec status stays **DRAFT — NOT APPROVED — NO IMPLEMENTATION AUTHORITY**. **B-01…B-15 and Q-01…Q-03 all remain OPEN**, and this repair decides none of them. Runtime checks were **NOT RUN**.

## Identity, authority and lane

- **Author:** Anthropic **Claude Opus 5.5** (`claude-opus-5-5`), running in the Claude Code CLI. The work was GitHub/docs-only, with no subagents. I did not author the #527 review. I am not the #527 reviewer, and I make no approval claim.
- **Delegation:** Overall Manager #425 issued a #530-specific, documentation-only authorisation: <https://github.com/jermur788/CCF/issues/530#issuecomment-6891804363>.
- **Binding packet:** [scen1-BROWSER-THREE-PROFILE-SAVE-527-BOUNDED-DOCS-REPAIR-PACKET-20261115.md](https://github.com/jermur788/CCF/blob/docs/scen1-527-save-draft-bounded-repair-dispatch-20261115-v1/Docs/Handoffs/ScenarioOne-Compact-Ecology-20261114/scen1-BROWSER-THREE-PROFILE-SAVE-527-BOUNDED-DOCS-REPAIR-PACKET-20261115.md).
- **RUNNING claim, posted before any tracked write:** <https://github.com/jermur788/CCF/issues/530#issuecomment-6891818793>.
- **Branch:** `docs/scen1-530-three-finding-save-draft-repair-20261115-v1`. It was new: `git ls-remote` showed it absent before the claim.
- **True starting HEAD:** `89bdab4b9f0b137f9837a33232a4eb2430e6e9f6`. This is the #524 amended DRAFT on `docs/shared-three-profile-save-policy-20261114-v1`, and `git ls-remote` confirmed it was unchanged immediately before the claim. The original #436 base is `504969298451327aa8a80cc7a4014e5e12953742`.
- **Worktree:** NONE. I built the commit with git plumbing and a temporary index, from edited copies kept in a scratch directory. No local checkout was switched, stashed or modified, and pre-existing unrelated local changes were not touched.
- **Unity/Editor/machine lease:** NONE.
- **Live board:** I read origin/main `Docs/Project/coordination/active-tasks.md` (main `fc18190b976f1363f1b5d1b0881810db2f8c6e39`) without editing it. No entry owns #530, the #524 branch or this output.
- **Evidence repaired against:** the [#527 independent report (pinned cb201378)](https://github.com/jermur788/CCF/blob/cb2013782238f89a6f9deb9d6e73db4a8a4605d7/Docs/Handoffs/ScenarioOne-Compact-Ecology-20261114/scen1-INDEPENDENT-THREE-PROFILE-SAVE-DRAFT-REVIEW-RESULT-20261115.md), whose verdict was NEEDS CHANGES (draft quality only) with 2 major findings, 1 minor finding and 2 advisories.

## Pinned read-only source references (main `fc18190b976f1363f1b5d1b0881810db2f8c6e39`)

I read every line below with `git show origin/main:<path>`. No source was modified, compiled or run.

| Fact | Pinned source |
| --- | --- |
| `CurrentVersion = 24`; field initialiser `version = CurrentVersion` | [ForestSaveData.cs:7](https://github.com/jermur788/CCF/blob/fc18190b976f1363f1b5d1b0881810db2f8c6e39/Assets/ForestPrototype/ForestSaveData.cs#L7), [ForestSaveData.cs:9](https://github.com/jermur788/CCF/blob/fc18190b976f1363f1b5d1b0881810db2f8c6e39/Assets/ForestPrototype/ForestSaveData.cs#L9) |
| CanLoad normalises a **parsed** `version <= 0` to 1 | [ForestSaveController.cs:249-250](https://github.com/jermur788/CCF/blob/fc18190b976f1363f1b5d1b0881810db2f8c6e39/Assets/ForestPrototype/ForestSaveController.cs#L249-L250) |
| Disk read deserialises (JsonUtility) **before** raw validators | [ForestSaveController.cs:267-269](https://github.com/jermur788/CCF/blob/fc18190b976f1363f1b5d1b0881810db2f8c6e39/Assets/ForestPrototype/ForestSaveController.cs#L267-L269) |
| LoadData normalises a **parsed** `version <= 0` to 1 | [ForestSaveController.cs:326-328](https://github.com/jermur788/CCF/blob/fc18190b976f1363f1b5d1b0881810db2f8c6e39/Assets/ForestPrototype/ForestSaveController.cs#L326-L328) |
| Newer version: warn + best-effort (historical, untouched) | [ForestSaveController.cs:346-347](https://github.com/jermur788/CCF/blob/fc18190b976f1363f1b5d1b0881810db2f8c6e39/Assets/ForestPrototype/ForestSaveController.cs#L346-L347) |
| `{}` parses as current-version defaults | [ForestSaveValidation.cs:6-8](https://github.com/jermur788/CCF/blob/fc18190b976f1363f1b5d1b0881810db2f8c6e39/Assets/ForestPrototype/ForestSaveValidation.cs#L6-L8) |
| Pre-v20: model 0 accepted, nonzero rejected | [ForestSaveValidation.Geometry.cs:14-17](https://github.com/jermur788/CCF/blob/fc18190b976f1363f1b5d1b0881810db2f8c6e39/Assets/ForestPrototype/ForestSaveValidation.Geometry.cs#L14-L17) |
| Raw geometry check returns early for `version < 20`; v20+ requires explicit integer | [ForestSaveValidation.Geometry.cs:27-34](https://github.com/jermur788/CCF/blob/fc18190b976f1363f1b5d1b0881810db2f8c6e39/Assets/ForestPrototype/ForestSaveValidation.Geometry.cs#L27-L34) |
| Every pre-v20 save read as Legacy40 | [StandGeometryModel.cs:60](https://github.com/jermur788/CCF/blob/fc18190b976f1363f1b5d1b0881810db2f8c6e39/Assets/ForestPrototype/StandGeometryModel.cs#L60) |
| F5 calls Save; Save captures and writes with no prompt | [ForestSaveController.cs:33-34](https://github.com/jermur788/CCF/blob/fc18190b976f1363f1b5d1b0881810db2f8c6e39/Assets/ForestPrototype/ForestSaveController.cs#L33-L34), [ForestSaveController.cs:39-44](https://github.com/jermur788/CCF/blob/fc18190b976f1363f1b5d1b0881810db2f8c6e39/Assets/ForestPrototype/ForestSaveController.cs#L39-L44) |
| F10 menu save asks for confirmation when the file exists | [UI/StandaloneSessionMenu.cs:124-126](https://github.com/jermur788/CCF/blob/fc18190b976f1363f1b5d1b0881810db2f8c6e39/Assets/ForestPrototype/UI/StandaloneSessionMenu.cs#L124-L126) |

Two small line-number differences from the #527 report affect no conclusion. The `version = CurrentVersion` initialiser is at ForestSaveData.cs line 9; #527 cited the declaration on line 7. The F10 confirmation is at StandaloneSessionMenu.cs line 125; #527 cited line 123, which is the adjacent load button.

## M-1 (MAJOR): raw-absent version key vs explicit parsed ≤ 0

**Before.** `specs/scenario-independent-save-isolation-v1/spec.md`, at base `89bdab4b`:

- line 109: `| Profile | Missing / ≤ 0 version | Known older | Newer than supported | Foreign identity |`
- line 111: `| Historical O1 | existing: treated as v1 [SOURCE] | …`

`checklists/requirements.md:82` (ISO-49): `Historical behaviour is byte-for-byte unchanged: version ≤ 0 → 1, pre-v20 → Legacy40, …`

**After**, on the #530 head:

- `spec.md:109` header: `| Profile | Raw-absent version key / explicit parsed ≤ 0 version (split by #530) | …`
- `spec.md:111` historical cell: `**Explicit present numeric 0 or negative:** normalised to v1 before validation [SOURCE OBSERVED]. **Raw-absent key:** not equivalent — parses as the initialised CurrentVersion (24), then faces the v20+ disk-route raw validators; never mapped to v1 by source [SOURCE OBSERVED]; any different future historical-slot treatment is [PENDING OWNER RULE] (see note below)`
- `spec.md:115`: a new paragraph, `**#530 version-key note (#527 M-1).**`, placed directly under the table. It cites the source facts, NOT RUN, and states that a raw-absent file is read as v24. On the disk route that file must pass the v20+ raw validators (for example an explicit integer `standGeometryModel`) or it is refused. On direct LoadData, only FindLoadProblem checks it. The note does **not** assert that every raw-absent historical file loads or maps to v1, and it chooses no new acceptance or rejection rule. Any change is [PENDING OWNER RULE] for #425/save authority, and the note does not alter B-03, B-04, B-05 or B-13. The compact/O2 strict refusal stays as PROPOSED DRAFT, and the historical newer-version warn + best-effort is untouched.
- `requirements.md:82` (ISO-49): `version ≤ 0 → 1` now reads `explicit parsed version ≤ 0 → 1`, plus the tag *(amended #530: three distinct historical-slot version cases, never merged:)*. Three indented sub-bullets follow on lines 83–85:
  - **raw `version` key absent:** [SOURCE OBSERVED] read as CurrentVersion (24) and validated as v24. The disk route applies the v20+ raw validators, so an absent `standGeometryModel` is refused. The file is not treated as v1. The test asserts current behaviour unchanged, or a later recorded owner ruling [PENDING OWNER RULE].
  - **explicit `"version": 0`:** [SOURCE OBSERVED] normalised to 1 by CanLoad and LoadData, then read under v1 rules subject to every other validator.
  - **explicit negative (for example -1):** [SOURCE OBSERVED] normalised to 1, exactly like the zero case.

ISO-49 keeps its number, its `[S/O]` tag and its unchecked box. The sub-bullets use the same indented-list form ISO-58 already uses, so they create no new checklist IDs. The compact/O2 rows of the FR-07 table are unchanged ("reject"), and so are ISO-11, ISO-24, ISO-46 and ISO-47.

## M-2 (MAJOR): F5 overwrite confirmation vs OPEN Q-03

**Before:**

- `spec.md:45` (W-02): `… affect the **currently selected** profile's slot only, with an understandable confirmation before a save replaces that profile's own existing current file.`
- `requirements.md:12` (ISO-03): `… fresh games do not delete a prior save. Later Save of a chapter requires truthful same-slot replacement confirmation.`
- `spec.md:181` (Q-03): `- Q-03: whether F5 should confirm replacement as F10 already does.`

**After:**

- `spec.md:45` (W-02): `… affect the **currently selected** profile's slot only. **Replacement confirmation (amended #530, #527 M-2):** the existing F10 menu save keeps its understandable confirmation before it replaces that profile's own existing current file [SOURCE OBSERVED]; source-current F5 saves directly, without a prompt [SOURCE OBSERVED]. Whether F5 must also confirm replacement is **OPEN Q-03** [PENDING OWNER RULE]: this workflow neither requires nor forbids an F5 prompt until Q-03 is recorded. Any user-initiated backup recovery still requires explicit confirmation (FR-06). Every save route, with or without a prompt, writes only the selected profile's own slot, reports truthfully and never replaces another profile's file (FR-05).` The rest of W-02, about no inheritance and no automatic cross-profile transfer, is unchanged.
- `requirements.md:12` (ISO-03): `… Later Save of a chapter writes only the selected profile's own slot, with truthful status and no cross-profile overwrite. *(amended #530, conditional on OPEN Q-03:)* the existing F10 menu save keeps its replacement confirmation when that profile's file exists; source-current F5 saves without a prompt [SOURCE OBSERVED]. The **F5-specific** confirmation clause is evaluated only against the recorded Q-03 ruling: if Q-03 requires an F5 prompt, an F5 overwrite without one FAILS; if Q-03 rules no F5 prompt, an unprompted F5 same-slot save PASSES this clause. Until Q-03 is recorded, that clause is neither PASS nor FAIL; the same-slot, truthful-status and F10-confirmation parts still apply.`
- `spec.md:183` (Q-03, shifted from line 181 by the added note): the text is unchanged, with the suffix *(#530: still OPEN; the F5-specific confirmation clauses in W-02 and ISO-03 are conditional on this ruling.)*

**FR-05 (spec.md:103) is unchanged.** It already says that F5 confirmation "is an owner UX question (Q-03)", so it is consistent with the repaired W-02/ISO-03. Its slot containment, pre-write identity validation, temp/backup containment, failure/no-success-message and no-save-during-preview requirements are all intact. Q-03 is not decided, and F5 is not described as having a prompt.

## m-1 (MINOR): pre-v20 geometry overstatement

**Before.** `spec.md:29`: `… A **v20+ disk** save with an omitted or non-integer standGeometryModel is **rejected** by ValidateGeometryJson, and a pre-v20 save that claims a model is rejected by ValidateGeometry. Only direct LoadData …`

**After.** `spec.md:29`: `… A **v20+ disk** save with an omitted or non-integer standGeometryModel is **rejected** by ValidateGeometryJson. **#530 correction (#527 m-1):** pre-v20 nonzero geometry is rejected by ValidateGeometry; an explicitly written geometry value of zero (Legacy40) is permitted; pre-v20 is interpreted as Legacy40. Only direct LoadData …`

No other text on line 29 changed. That includes the v20+ explicit-integer rule, the ForSave mapping and the direct-LoadData caveat, as well as the historical 0/1 semantics and the v20+ raw geometry/identity rules elsewhere. No validator or code change is proposed.

## Advisories A-1 and A-2

I added no text for either advisory. Both stay recorded only in the #527 report, as inputs to the owner decisions.

- **A-1:** binds the envelope payload-save-version to nested `payload.version`, with a mismatch test. It belongs with **B-13**, and B-02 for the envelope shape.
- **A-2:** says envelope root-key disjointness is not by itself a deserialiser-safety guard, and that the mandatory raw-marker preflight is decisive. The existing SI-08 / FR-02 step 4 / ISO-45 guard text is unchanged and still explicit.

Adding either one now would edge into schema design that the packet excludes.

## plan.md and tasks.md

I checked both files for directly contradictory wording, searching for confirmation/F5, ≤ 0 and missing-version phrasing, and pre-v20/claims-a-model phrasing. **None was found.** plan.md:22 says only "existing v1–v24 rules", and tasks.md:45 keeps compact/O2 strict refusal and the historical best-effort. **Neither file was changed.**

## Preserved open decisions and protected scope

- **B-01…B-15:** all 15 rows in spec §8.2 are byte-identical to base. **Q-01…Q-03:** all still OPEN. Only Q-03 gained a cross-reference suffix, and its question text is unchanged.
- **Unchanged:** the historical O1 newer-version warn + best-effort; the compact/O2 strict refusal (PROPOSED); the Legacy40/Enlarged80 and model 0 semantics, v1–v24 and the frozen Reference Future v1; D-082/#471, which stays PAUSED; and the compact parent/child DRAFTs.
- Both the new-profile and historical save routing remain **PROPOSED / DRAFT**, not implemented.

## Checklist integrity (63 / 63 unchecked)

Method: I ran these on the edited `checklists/requirements.md` before committing, and repeated them on the GitHub read-back recorded in the #530 HANDED-OFF comment.

- `grep -c '^- \[ \] \*\*ISO-'` gives **63**.
- `grep -c -E '^\s*- \[[xX]\]'` gives **0**.
- The extracted ID sequence is `01…63`, **unique and contiguous**.
- `diff` of every `- [ ] **ISO-NN [TAGS]**` prefix between base `89bdab4b` and the edited file is **identical**, so ISO-01…ISO-36 keep their original order and tags and ISO-37…ISO-63 are unchanged in position.
- Only ISO-03 and ISO-49 were reworded, and both carry the tag *(amended #530)*. The checklist header gained one sentence recording that. "Acceptance status: NOT EXECUTED. 0 checks marked passed" is unchanged.

## Changed files (relative to `89bdab4b`)

| Status | Path |
| --- | --- |
| M | `specs/scenario-independent-save-isolation-v1/spec.md`: header status line 4 (#530 attribution), line 29 (m-1), line 45 (M-2 W-02), lines 109/111 plus a new note at line 115 (M-1), line 183 (Q-03 cross-reference) |
| M | `specs/scenario-independent-save-isolation-v1/checklists/requirements.md`: header line 4, ISO-03 line 12 (M-2), ISO-49 lines 82–85 (M-1) |
| A | `Docs/Handoffs/ScenarioOne-Compact-Ecology-20261114/scen1-SHARED-SAVE-527-THREE-FINDING-BOUNDED-REPAIR-RESULT-20261115.md` (this report) |

There are no other changes. plan.md, tasks.md, C#/Assets/Packages, saves, PlayerPrefs, Reference archives, decision-log, current-milestone, the coordination board, the compact DRAFTs, and the #524 and #527 artefacts are all untouched.

## Expected spec status

The spec status is still **DRAFT — NOT APPROVED — NO IMPLEMENTATION AUTHORITY**, and the checklist status is still **NOT EXECUTED**. This repair grants no G4 integration and no G5 save/product/canonical approval.

## NOT RUN

The following were **not run**:

- Unity Editor or Player, and C# compile
- any save load/save, raw-JSON parsing experiment, or old-world/historical v1–v24 fixture load
- the Reference Future preview/hash
- PlayerPrefs reads or writes
- backup/recovery or failure injection
- Windows build

Every [SOURCE OBSERVED] statement comes from static reading of the pinned source only. Runtime and save compatibility are **NOT VERIFIED**. Dashboard/Drive/context-mirror sync is **NOT VERIFIED**.

## Next step

**At most one independent delta review**, limited to exactly these three repairs (M-1, M-2, m-1) and their direct interactions. This is not another full 63-item audit, and the reviewer must not be this author. After draft quality is approved, #425/owner/save authorities decide B-01…B-15 and Q-01…Q-03 separately.
