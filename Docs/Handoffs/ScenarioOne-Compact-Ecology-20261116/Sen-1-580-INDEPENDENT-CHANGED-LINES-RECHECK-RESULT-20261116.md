# Sen-1 — #584 independent changed-lines recheck of the #580 correction

**Verdict: APPROVED (changed-lines document quality only). 0 blockers, 0 major, 0 minor, 3 non-blocking nits.**

This verdict covers only the wording changes made under [#580](https://github.com/jermur788/CCF/issues/580) to fix the six [#579](https://github.com/jermur788/CCF/issues/579) findings. It does **not** approve either Compact specification. It does not approve the world manifest, CG-TRIG, D-083 publication, Compact source, save routing, Unity or any Windows build. Both Compact four-file specs remain DRAFT. D-083 stays staged for review only, under #425.

## 1. Registration and scope

| Item | Value |
| --- | --- |
| Reviewer | Anthropic Claude Opus 5.5 (`claude-opus-5-5`), Claude Code CLI, owner's local Linux machine. Not the #576/#580 author (OpenAI GPT-6). Same model as the original #579 reviewer, so each correction was checked against the #579 report's own wording. |
| Registration | [#584 comment](https://github.com/jermur788/CCF/issues/584#issuecomment-6913118561), posted before any write. Issue title set to `[RUNNING]`. |
| Base (reviewed #576 head) | `cc34ea971ea2da8d3b7adcebaeb1f01d0321ef54` |
| Repaired head (starting HEAD) | `974ae8b9a8f2ca5ef24c068c5506523a30b2f377`, the live tip of `docs/sen-1-580-compact-draft-six-findings-repair-20261116-v1`, fetched after the #580 result was published |
| Review branch | `review/sen-1-580-compact-delta-recheck-20261116-v1`. I confirmed it was unoccupied, then created it at `974ae8b9`. |
| Worktree | temporary docs-only checkout in the session scratchpad, created at write time and removed afterwards. No Unity Library. |
| Output | this file only |
| Method | `git diff --word-diff cc34ea97..974ae8b9` over all 11 paths, plus targeted residual greps across the 10 edited files at the repaired head. #576-unchanged DRAFT content was not re-reviewed. #508, the 115/33 cases, ISO-63, Unity and tests were **not run**. |

## 2. Inventory

`cc34ea97` is an ancestor of `974ae8b9`. Exactly **11 Markdown paths** changed, all on the #580 allowlist, with no unexpected paths:

- parent `specs/scenario-one-compact-complete-loop-v1/`: `spec.md`, `plan.md`, `tasks.md`, `checklists/requirements.md`
- child `specs/scenario-one-compact-initial-planting-v1/`: `spec.md`, `plan.md`
- `Docs/Handoffs/ScenarioOne-Compact-Ecology-20261116/`: world manifest candidate, CG-TRIG DRAFT, the #576 result, and the new #580 result
- `Docs/Project/decision-log.md` (D-083 row only)

The following are byte-identical between base and head: child `checklists/requirements.md`, child `tasks.md`, `Docs/Project/current-milestone.md` and `specs/scenario-independent-save-isolation-v1/` (the signed save contract). No Assets, Packages, ProjectSettings, Tools, coordination, save or Prefs path appears.

## 3. The six findings

| #579 finding | Recheck result | Evidence (repaired head line numbers) |
| --- | --- | --- |
| **M-1 MAJOR**: the owner-ratified 2+2 count was labelled Manager reversible or provisional | **RESOLVED** (see nit N-1) | R-007, R-102, R-105, R-107 and R-115 plus the decision-update paragraph (requirements 17/126/132/134/142/144) now say "16 Nov OWNER-RATIFIED … count". Only the sites, year and protection remain #515 candidates. Parent spec §8 is retitled: OPEN-C2 is an "OWNER RATIFIED DESIGN BASELINE" with exact roster, colliders and walkability "NOT YET VERIFIED", and OPEN-C9 reads "COUNT OWNER RATIFIED; SITES/OUTCOMES OPEN" (121/128). Spec 10, 111 and 134, plan 11/34/40/42, tasks D-05 (24), owner-acceptance paragraph (35) and C0-07 (40), child spec 9–10/59/67/90/97 and child plan 15/22/52/79 are consistent. This matches the owner source ([ratification](https://github.com/jermur788/CCF/blob/docs/sen-1-owner-30m-baseline-ratification-20261116-v1/Docs/Handoffs/ScenarioOne-Compact-Ecology-20261116/Sen-1-OWNER-RATIFIED-30M-NO-RACK-BETA-DESIGN-BASELINE-20261116.md) items 6 and 22: "the initial 2+2 count/direction within OPEN-C9", with exact G1 coordinates, light and protection not closed). No Manager site, year, light or protection condition was promoted to owner-ratified. |
| **m-1**: stale "parent one-rack BLOCKER" | **RESOLVED** | Child plan 57 and child spec 81 now say the historical one-rack conflict was "RESOLVED at DRAFT level" by #516 (no-rack parent) and #576 (consolidation). Both keep a "Still BLOCKED/OPEN" list: #579/#580 review, formal approval, signed H1/G1 native world, Work Plan handling and saves. Child spec 12 and child plan 70 agree. Five-rack Rack V2 is stated as unaffected. Nothing is marked APPROVED. |
| **m-2**: G1 0.221 called "pre-H1" | **RESOLVED and independently confirmed** | Manifest 62 header now reads "static H1-adjusted canopy proxy (cell 15 unchanged)", and the new paragraph at 69 gives canopyBase = canopyAfter = 0.7789 and 1 − 0.7789 ≈ 0.221. I re-verified this against the durable #508 ledger (`/media/jer/Files/CCF/Verification/scen1-508-EntrancePathPlanting-20261114/`): `sha256sum -c SHA256SUMS` passed for all three files, and `ledger.json` `/H1/sites[0..3]` all show `cell 15, canopyBase 0.7789, canopyAfter 0.7789`. Manifest 71, #576 result 14, parent tasks 35 and R-107 all correctly say this is not native Year1/Year2 light or survival. A grep for `pre-H1` across all ten edited files returns nothing. |
| **m-3**: garbled D-082→D-083 substitution and the #471 tie | **RESOLVED** (see nit N-3) | Every `[proposed compact exception; D-082 remains Sitka experiment]` bracket is gone (grep returns nothing). Each is replaced by "proposed D-083 Compact-first-beta exception", in most places tied to "(review #579; obsolete #471 paused)". D-082 is still described only as the standalone Sitka experiment, and its Decision Log row is unchanged. No #471 mutation is claimed. |
| **m-4**: D-083 did not state its effect on D-060/D-071/D-072 | **RESOLVED** | A script check confirmed that the D-083 row at head equals the base row plus exactly one inserted sentence: "Scope effect: D-060's first-external-demo target becomes the qualified Compact world; D-071 and D-072 govern historical Enlarged80 only; D-059 and D-060 comparable three-species modelling and complete-loop duties otherwise remain unchanged." Every other Decision Log line (D-082, D-071, D-072, D-059, D-060 included) is byte-identical to base. The row status stays "STAGED FOR INDEPENDENT REVIEW, NOT IMPLEMENTATION APPROVAL". |
| **m-5**: CG-TRIG ignored the existing `AnnualReviewAcknowledged=9` event | **RESOLVED, and every source citation checked** | The CG-TRIG DRAFT (lines 14, 18, 21, 23–28, 46) now prefers the existing event 9 and `MarkAnnualReviewSeen()`, and adds no event or button by default. It names the generic `year >= latest` chronology as too weak for the signed exact-year `reviewAcknowledgedYear = max(completionYear)`. If the UI cannot prove the player opened and acknowledged the covering report, it says to STOP for an owner-signed binding: no fabricated event and no automatic credit. ISO-58 stays BLOCKED. The assessment step is a concrete **proposed** predicate: the player opens the current Compact stand or diagnostic view and explicitly confirms, with wrong-profile, closed-view and save/reload negatives. It is not claimed as existing UI. Checked against `origin/main`: `ScenarioManagementEvent.cs:16` `AnnualReviewAcknowledged = 9`; `ScenarioOneManager.cs:391–401` `MarkAnnualReviewSeen()` (returns early when there are no reports, sets `annualReviewSeen`, records event 9 at `CurrentYear` through `ShouldRecordAcknowledgement`); `ScenarioOneCompletionRules.cs:51–68` (`e.year >= latest.Year` and the idempotence rule); `ScenarioCycleState.cs:21–23, 95–101` (`e.year >= interventionYear`). All four match the DRAFT's description. |

## 4. Preservation checks

- **Parent checklist:** the ordered `R-001…R-115` ID and checkbox sequence has identical md5 at base and head (`3bc6d3b8…`). 115 unchecked, 0 checked. The changed rows reword the authority labels only. No criterion, negative test or status was weakened or deleted.
- **Child checklist:** `CIP-001…CIP-033` identical (`61407936…`). 33 unchecked, 0 checked. The file is byte-identical.
- **H1/G1 source data:** the manifest changed in only three places: the G1 table header (62), one new explanatory paragraph (69) and one sentence (71). All 14 H1 omission rows, 8 buffer IDs, 8 Route A waypoints and 4 G1 coordinate/light rows are untouched. The four G1 coordinates quoted in the #576 result are unchanged.
- **Signed save and historical worlds:** the signed save contract directory is unchanged and absent from the delta. No evidence key, schema, identity, path or Prefs wording was altered. CG-TRIG explicitly forbids changing signed keys. Enlarged80, Rack V2/MFM, Legacy40, v1–v24 saves and Reference Future v1 protections are only restated or kept.
- **Authority:** no OPEN-C1/C4–C8/C10 item is closed. The 175 conditional count and the 189 source originals keep their evidence tiers. No main merge, coordination edit or approval claim was found.

## 5. Non-blocking nits

These are optional. Each could be folded into #425's canonical publication edit without another review round.

- **N-1, parent `spec.md:35`:** "**OPEN-C9** first episode and provisional G1 2+2 are partly resolved at DRAFT level…" still calls the 2+2 "provisional". This contradicts §8 OPEN-C9 at line 128 ("COUNT OWNER RATIFIED"). #579 did not list this line, and the #580 hunk at line 35 changed only the D-083 reference. The #580 result's M-1 row lists `spec.md` line 35 as repaired, which overstates the change. Suggested text: "OPEN-C9 G1 2+2 count is owner ratified; sites, stock, position and finance proof remain outstanding." It does not block approval because every requirement row and the governing §8 register are correct.
- **N-2, child `spec.md:12`:** "independent #579 NEEDS CHANGES delta recheck and approval remain" is compressed to the point of being ambiguous. It should read "the #580 changed-lines recheck of #579's NEEDS CHANGES".
- **N-3, child `plan.md:54`:** "proposed D-083 Compact-first-beta first-demo canonical…" is missing the word "exception" that every other substitution uses.

**Observation (not a defect):** `MarkAnnualReviewSeen()` records event 9 at `CurrentYear`. Whether the covering report for completion year Y is acknowledged while `CurrentYear` is Y or Y+1 depends on when the UI shows that report. This decides whether the signed exact-year binding can work without amendment. The DRAFT's trace-or-STOP gate already covers this, and #424 should trace it first when CG-TRIG is taken up.

## 6. Disposition

#579's six findings are **resolved for changed-lines document quality**. The #580 DRAFT wording is fit to go forward to #425 for canonical/D-083 publication decisions and to #424/the owner for CG-TRIG, OPEN-C5, OPEN-C8 and world-spec acceptance. This review grants no spec approval, no source or Unity release, no save activation and no Windows build. Dashboard/Drive: NOT VERIFIED.
