# scen1 — Independent #496 review of the #495 Option A planting-led compact DRAFT delta

**14 November 2026 | Verdict: NEEDS CHANGES (narrow, documentation-only) — DRAFT QUALITY ONLY.**

Option A itself is recorded correctly and the draft does not approve anything the owner did not. The verdict rests on **one source-premise defect (N-1)** about hidden mortality/deadwood that runs through all four files, plus **one stale-header defect (N-2)**. Both are small, bounded author repairs. Nothing here authorises implementation, Unity, a canonical D-082 change or a Windows build.

## 1. Registration and independence

| | |
|---|---|
| Claim | [#496](https://github.com/jermur788/CCF/issues/496), registered `[RUNNING]` before any write ([registration comment](https://github.com/jermur788/CCF/issues/496#issuecomment-6877102383)) |
| Actual reviewer | Anthropic **Claude Sonnet 5.5** (`claude-sonnet-5-5`), Claude Code CLI harness. This is **not** the suggested Opus-High / GPT-6-High; it was registered as-is. |
| Independence | Different session from the #495 GPT-6 author. No earlier involvement in #495/#465/#467/#490. **Disclosure:** the #484 research worker was also a Sonnet 5.5 session. This is a different session and I reviewed only how #484 is cited, not #484's method. |
| Starting main HEAD | `fc18190b976f1363f1b5d1b0881810db2f8c6e39` |
| Reviewed candidate | `docs/scen1-planting-led-compact-draft-20261114-v1` @ `a16a987c46bd93689ac60f2a5f621c4c9862d3a4` |
| Baseline | `docs/compact-one-rack-463-three-findings-repair-20261113-v1` @ `06dcb22d5f5011ac4f911eb3b8a2fc544e1055f4` (#467 APPROVED FOR OWNER/SAVE-DECISION PREPARATION) |
| Review branch | `docs/scen1-496-option-a-planting-delta-review-20261114-v1` (checked absent, then created from main HEAD above) |
| Worktree / Unity | Worktree **NONE** written. Read-only git object access only. No Unity, Editor, machine lease or Windows build. |

## 2. What I did and did not do

**Done (static only):**
- Read the full #495 handoff and the #496 issue, and read #495 comment history.
- Diffed candidate against baseline: exactly the four modified DRAFTs plus one new handoff.
- Read all four candidate files, with the Option A hunks in full.
- Machine-counted the checklist rows.
- Read #484 result, #490 result, the Manager #490 verdict and #467 review.
- Read current-main source: `ForestTree.cs:551–565`, `ForestEcologyController.cs:429–437, 1062–1150`, `ScenarioOneManager.cs:512–560`.
- Read-only inspection of the local #490 evidence directory (`/media/jer/Files/CCF/Verification/scen1-ThreeSpeciesLongHorizon490-20261114T085531Z/`): executed harness (SHA-256 `c7291f7a…65126cb`, equal to the pin in the #490 report), snapshots and scenario-state JSON.

**Not done:** Unity or any native replay, #490 142-check re-execution, PNG pixel inspection, old-save/Reference hash replay, the full 115-requirement test suite, canonical/D-082 review, #471/#436 branch review. Each remains NOT VERIFIED.

**Limit on owner authority.** I could verify Option A only through Manager attestations (packet §1, #424/#425 comments of 10:16 UTC). The owner's own words are not recorded in the repository. This is consistent with the Manager's 10:03 UTC A/B question, but I did not see the owner's message.

## 3. Packet checklist results

| # | Question | Result |
|---|---|---|
| 1 | Is Option A recorded as accepted planting/protection, with "no natural Oak/Beech by Year40" a permitted outcome rather than a guarantee? Is D-059 parity kept? | **PASS.** Spec §1A and §9, S-12A, R-105/R-106, plan "Accepted product boundary" and tasks D-05 say so. They state that natural Oak/Beech is *not required*, not that it is *prevented*. S-12, S-12A and R-105 keep seed, regeneration, browsing and mortality mechanics and D-059/D-060. A grep of the four files found **no** unchanged baseline line that still *requires* natural broadleaf recruitment. |
| 2 | Any line that inadvertently approves 30 m, 40-year completion, guaranteed survival, an arbitrary planting count, hedge donors, financed work, twin-run saves/ideal guided marks or forced Fell? | **PASS, with wording notes N-4/N-5.** §1A "Not approved" and OPEN-C2/C3/C5/C6/C9/C10 are explicit. Four things are labelled DRAFT inference, not owner acceptance: the "small cohort", S-12A's "at least two options", R-107's "plurality" and the three reassessment opportunities. R-113 and tasks "Deferral" forbid compulsory Fell. R-114/OPEN-C10 keep the twin run unratified. Hedge donors are deferred / spec required. |
| 3 | Are #484/#490 correctly tiered? | **MOSTLY PASS. One substantive miss (N-1) and two small tier slips (N-3).** See §4. |
| 4 | Are S-12A, §1A, plan, D-05, C0-05..08, R-049 and R-105..115 consistent and non-conflicting with old semantics, D-077, three-species staging and S1 history? | **PASS except N-1/N-2.** S-13/S-14 are untouched and consistent: "no 30/40-year completion rule" and the D-077 Year25/100 stops coexist. D-077 P-1 keeps the old two-cycle predicate. R-049's rewrite is faithful to #484, whose own text says a finite result is correct if the land beyond is open. |
| 5 | Exactly four modified files plus one added handoff, all 115 unchecked, nothing else changed? | **PASS.** See §5. |
| 6 | Bounded next step. | See §7. |

## 4. Evidence-tier audit (#484 / #490 claims vs the sources)

| Draft claim | Source check | Tier |
|---|---|---|
| #484 reproduced Year-0 finite 8 m Hegyi CI for 8 focal trees; hypothetical exterior stems matter only under a larger-forest premise | Matches the #484 result: 8/8 IDs, CI reproduced to ≤7e-7, premise explicitly unsettled | OK |
| 40 real annual ticks, two arms, 142 checks worker-reported, early #460 replay equality | Matches the #490 result; the "worker-reported" qualifier is preserved | OK |
| One Oak and one Beech survived and promoted around Years 10/11; Y40 18.1/19.4 m, 12.3/15.0 cm; age 42 | Matches (Oak Y10, Beech Y11; DBH 12.28/15.00; height 18.10/19.40) | OK |
| Zero natural Oak/Beech cohorts; small seed potential only at Y40 | Matches (seed potential 0.003/0.010; 146/143 cohort bands all Sitka) | OK |
| ~100 original Sitka died per arm | Matches (106/100) | OK |
| Y5 release gave +1.34/+2.69 cm to crop Sitka and barely changed the broadleaves | Matches (Oak 12.22 vs 12.28 cm) | OK |
| Oak crown radius ~0.45 m at 18.1 m | Matches. See N-3 for the Oak-specific scope. | OK |
| One defective Y0 image; old-save/Reference hashes NOT RUN; Bramble/bracken NOT analysed | Matches | OK |
| **Sitka deaths leave no deadwood outcome; `ApplyMortality` "hides … rather than providing … deadwood"; a presentation limitation** | **Not supported as a game-behaviour claim.** See N-1. | **Mis-tiered** |

## 5. Scope and integrity verification (static)

- `git diff --name-status baseline candidate` = `A` the #495 handoff and `M` for `spec.md`, `plan.md`, `tasks.md` and `checklists/requirements.md` only. **0** `Assets/`, `Packages/`, `ProjectSettings/`, `Tools/`, `Docs/Project/` or coordination paths.
- Checklist rows: **115** `- [ ] R-…` rows, IDs contiguous 001–115 with no gaps or duplicates, **0** `[x]` anywhere in the four files. R-105..R-115 are the 11 new rows.
- R-001..R-104 are byte-identical to baseline **except R-049**, which was deliberately rewritten (finite-edge logic) and is faithful.
- OPEN-C1..C8 are unchanged. OPEN-C9 and OPEN-C10 are added once each. No ID collisions in tasks (D-05, C0-05..C0-08 appear once each).
- The F-1/F-2/F-3 repair text (S-10, S-11, plan Stage C/D, R-039/R-040) is untouched by the delta; I did not reopen #467.
- Branch relation: the candidate is 25 ahead of and **3 behind** main. The three main commits (`d39c0244`, `ee457db3`, `fc18190b`) touch only `Docs/Project/coordination/active-tasks.md`, which the candidate does not change, so there is **no path overlap**. A two-dot diff against current main shows `active-tasks.md` only because of that lag; it is not an author edit. A three-dot diff confirms the author did not touch it.
- Protected old-world classes (v1–v24, Legacy40/Enlarged80, five-rack, model-0, Reference Future v1): **text protections are preserved** (S-03, S-22, R-115). **Nothing was run or hash-verified**, by the author or by me.

## 6. Findings

### N-1 (MEDIUM — required): the "hidden mortality / no deadwood" premise is not established for the game and is probably a research-harness artefact

**Where it appears:** spec.md:23 (§1A, "ApplyMortality hides … rather than providing an observed standing/fallen deadwood outcome … a recorded modelling/presentation limitation"); plan.md:22 ("without falsely displaying a fallen/deadwood log for a tree hidden via `ApplyMortality`"); tasks.md:34 (C0-07 "hidden self-thinning objects") and tasks.md:58; checklists/requirements.md:132 (R-111); spec §9 and OPEN-C9 treat it as a "quality gate".

**What I found:**
1. On main `fc18190b`, `ScenarioOneManager.OnEnable` subscribes `OnTreeBiologicalDeath` to `ForestTree.MortalityApplied`. For growth model ≥ 1 (`SiteClassDensity`; new games use model 2) the handler creates a `ScenarioDeadwoodRecord` for each biological death (tree ID, species, position, volume, fallen year) and spawns a fallen-log visual. Its source comment says "a biological death leaves its stem as fallen deadwood … ApplyMortality raises this event once per death". `ForestEcologyController.ApplyAdultDensityMortality` reaches `ApplyMortality("self-thinning", year)` through `KillDensityVictims`, so it triggers that path.
2. `ForestTree` also stores `MortalityCause` and `MortalityYear` (persisted via `RestoreMortality`).
3. The #490 exported state has **0** deadwood records at Y9–Y40 (`deadwoodRecords` empty, `nextDeadwoodId 1`) in both arms, despite ~100 self-thinning deaths. Snapshots list **no** dead trees (all `dead:false`), consistent with the inactive objects dropping out of enumeration.
4. The executed #490 harness (SHA-pinned in the report) does `manager.enabled=false` at `Scen1LongHorizon490Native.cs:77`, then drives years with `manager.AdvanceYear()`. Disabling the component runs `OnDisable`, which **unsubscribes** the deadwood handler.

**Assessment.** The zero-deadwood result is *consistent with* a fixture side effect and **cannot be read as what a player would see**. I did not run Unity, so I do not claim to have proved it. It remains an unreconciled discrepancy between source reading and the native measurement. The draft's literal statement that `ForestTree.ApplyMortality` alone does not create a log is true. The *inference* that Sitka mortality therefore has no deadwood outcome, and must be handled as a presentation limitation, is not supported. The same wording appears in the #490 result §1.5 and the Manager #490 verdict ("without a per-tree … resolver"). That is outside this branch, but #424 should be told.

**Required repair (author-only, docs):**
- Restate the tier: *measured* = aggregate losses only. In the #490 fixture the Scenario manager component was disabled, so deadwood and per-death records are **not evidence about normal play**. *Source reading* = a normal growth-model ≥ 1 game appears to create one deadwood record and log visual per biological death and keeps tree-level cause/year. *Unverified natively.*
- Remove or soften the assertions at spec.md:23, plan.md:22, tasks.md:34 and tasks.md:58 ("falsely displaying", "hidden self-thinning objects", "must be handled as a presentation limitation").
- Rewrite R-111 so it asks for a **small native check with the manager enabled** (are deadwood records and logs created per self-thinning death? is cause recoverable?) before any mortality/deadwood gate is treated as real.
- Keep the existing guard against fabricated logs. Do **not** order a #490 rerun; fold the check into the C0-08 witness only if C0-07 keeps it relevant.

### N-2 (LOW — required, repeat class of #463 F-2): stale spec header and sequencing text

- spec.md:3 still reads "Date: 13 November 2026 … #454 is documentation author only".
- spec.md:5 (Evidence basis) lists only #460 and states that #462/#465 editors "ran no new fixture"; it does not mention #484/#490/#495.
- spec.md:6 (Implementation gate) and S-21 do not include the #495 → #496 → #425 reconciliation step.
- tasks.md:7 (C0-02) still names only OPEN-C1–C8. tasks.md:33 (C0-06) says "no new full 104-item re-review" although the checklist is now 115.
- The checklist header (requirements.md:2) *was* updated, so the four files are now inconsistent in what they say about provenance.

**Repair:** update those lines. No status promotion.

### N-3 (LOW — advisory): small tier and precision slips

- plan.md:18 says "Manager reviewed GitHub evidence but not every raw local image". The Manager #490 verdict says it did **not** independently inspect any PNG pixels. The #490 report itself says only about 6 of 20 captures were individually inspected, and one of those 6 ("via 1080p twin statistics") is weak. Say that explicitly.
- spec.md:23 reads "One Year2 protected Oak and one Beech". In #490 the Oak was owner-**sheltered** and the Beech contractor-planted and **unsheltered**, yet both survived. That bears directly on any "protection matters" lesson, so state it.
- spec.md:77 (S-12A) says "extremely narrow crown behaviour" for broadleaves generally. #490 shows the narrow crown is **Oak-specific** (radius 0.29–0.45 m). The Beech crown was 0.63–1.04 m. Also, the Oak's modelled crown radius *shrank* from 0.69 m (Y10) to 0.33 m (Y20) as it was suppressed; that is worth naming.

### N-4 (LOW — advisory): hedgerow wording

spec.md:21 says "Hedgerows **may** have seed sources." The Manager verdict notes "a generic hedgerow does not automatically contain fertile Oak/Beech; it may carry other species instead". Label this as an **unverified assumption needing research** (AGENTS.md ecology rule), not a hedged fact. The conclusion (no mechanism approved; spec required) is already correct.

### N-5 (LOW — advisory): label inferences inside the requirement lines

- R-107 ("bounded credible plurality") and S-12A ("at least two … options … must be compared") are Manager DRAFT inferences, not owner acceptance. §1A says so, but the requirement rows themselves should repeat "DRAFT inference — not owner-accepted count/design".
- The §1A block heading "Owner-approved site framing" over-labels material that is partly the author's "provisional interpretation". The owner "permits" the bounded-woodland framing; retitle it "Owner-permitted".

## 7. Bounded next step

1. **#424 (author-only, one short pass, docs only):** apply N-1 and N-2 (required). Fold in N-3 to N-5 if convenient. **No** change to the checklist count, check state, OPEN IDs, owner decisions or canonical files.
2. **Then one short non-author delta check** of only those lines, not a re-run of #460/#484/#490 and not 115 generic tests.
3. **After APPROVED as DRAFT quality:** #424 may prepare an owner-facing cohort/site/decision-clarity design from #490 evidence without promising planting success, and #425 reconciles D-082/Option A in canonical documents through its own paused #471 gate. OPEN-C1–C10 and #436 one-router remain owner/save decisions.
4. **Tell #424** that the #490 result §1.5 and the Manager verdict carry the same deadwood premise. They are not under review here and were not edited.

**Not authorised by this review:** implementation, a new Unity worktree or native run, closing D-082, any canonical Decision Log / Current Milestone / coordination edit, or a Windows build. This is **documentation quality only**, not product approval.

## 8. Publishing evidence

This review adds exactly one new Markdown path on `docs/scen1-496-option-a-planting-delta-review-20261114-v1`. GitHub readback and the one-file comparison against the starting main HEAD are recorded on [#496](https://github.com/jermur788/CCF/issues/496). Dashboard and Drive/context mirror: **NOT VERIFIED**; #424 retains operational sync.
