# Sen-1 — #579 independent review: Compact30 DRAFT specs, world manifest, CG-TRIG/finance contracts and proposed D-083

**Verdict: NEEDS CHANGES — for the DRAFT documentation and canonical proposal ONLY.** 0 BLOCKING, 1 MAJOR, 5 MINOR. The package is close. One bounded author correction (M-1 required; m-1…m-5 recommended), followed by a short nonauthor delta re-check, should make it fit for #425/#424 owner final sign-off. **It is not fit for that sign-off at the reviewed head.**

This is a documents review. It is **not** a feature approval, a built scene, a save qualification, a specification approval, a canonical publication or permission to start Unity.

| Field | Value |
| --- | --- |
| Claim | [#579](https://github.com/jermur788/CCF/issues/579). Registered [RUNNING] before any review write ([comment](https://github.com/jermur788/CCF/issues/579#issuecomment-6912060743)). |
| Reviewer | Claude Opus 5.5 (`claude-opus-5-5`), Claude Code CLI on the owner's local Linux machine with git/gh. Not the #576 GPT-6 author, and no part of the candidate is mine. |
| Review branch | `review/sen-1-compact-beta-canonical-20261116-v1`. Unoccupied before creation (`git ls-remote`), created at the pinned subject head. |
| Pinned subject | `docs/sen-1-compact-beta-canonical-scope-20261116-v1` @ **`cc34ea971ea2da8d3b7adcebaeb1f01d0321ef54`** |
| Candidate base | `09bf0b42c23cec658242386a4f390fa86de0cfc0` (#576's registered `main` start) |
| Live `main` at review | `a7b01d1b373cfb3d5ccd9afdff5bfe9a3050a594`. Its only change since the base is 2 lines in `Docs/Project/coordination/active-tasks.md`, so there is no canonical drift. |
| Worktree | Short-lived docs-only checkout in session scratch, not ZX20 and not a Unity lane. Removed after push. |
| Not done | Unity, lease, C#/scene/asset/save/Prefs edits, candidate source edits, `active-tasks.md` writes, main merge, approvals. |

## Sources compared (exact)

| Source | Ref / blob |
| --- | --- |
| Owner 16 Nov ratification | `docs/sen-1-owner-30m-baseline-ratification-20261116-v1` @ `3e5a52c2` |
| #576 worker packet | `docs/sen-1-576-browser-handoff-20261116-v1` @ `a4518c17` |
| Parent #516 originals | `docs/scen1-parent-no-rack-g1-2plus2-draft-revision-20261114-v1` @ `bce49d7a`. Blobs: spec `9f485177`, plan `5f843dfa`, tasks `a117c1cb`, requirements `7092b09d`. These match the #576 result. |
| Child #515 originals | `docs/scen1-manager-513-2plus2-planting-access-verdict-20261114-v1` @ `2a24a969`. Blobs: spec `9a8e1407`, plan `12519df4`, tasks `1c625299`, requirements `cb7e050f`. These match the #576 result. |
| #508 source study | `research/scen1-eight-broadleaf-eligibility-20261114-v1` @ `6a1701ae`. Also the durable `/media/jer/Files/CCF/Verification/scen1-508-EntrancePathPlanting-20261114/ledger.json`, where `sha256sum -c SHA256SUMS` passed for all 9 entries. |
| #490 Year40 | `research/scen1-three-species-long-horizon-native-20261114-v1` @ `1bf40038` |
| Signed save | Spec at `200add4e` plus the external attestation on `docs/scen1-manager-save-v2-immutable-approval-20261115-v1` @ `c761d72e`. Signed blobs `8ac55102`/`4ba91d90`/`88052dc5`/`505ab550` are untouched by the candidate (the path is not in the diff). |
| Live canon | `origin/main:Docs/Project/decision-log.md`, `current-milestone.md` and `active-tasks.md` (read-only). Also #576 issue body edit history and #471 state. |

## Findings

### M-1 MAJOR — the G1 2+2 count still carries the "Manager reversible/provisional" authority tier in operative criteria and registers (S-1, S-2)

The owner ratified the 2+2 **count** on 16 Nov. Each file's new banner and the appended register now say **OWNER RATIFIED COUNT**. However, the carried-over operative text still assigns the count to Manager #515 as a reversible or provisional default. It does so in acceptance criteria, in an imperative instruction, and in a second OPEN-C register that now contradicts the appended one in the same `spec.md`:

| File @ `cc34ea97` | Line | Stale operative text |
| --- | --- | --- |
| `specs/scenario-one-compact-complete-loop-v1/checklists/requirements.md` | 17 | **R-007**: "#515 Manager provisional **G1 2 Oak +2 Beech** first lesson stated separately…". As written, a verifier must check that docs call the count Manager-provisional. |
| same | 134 | **R-107**: "FIRST PLANTING OWNER SEQUENCE / **#515 REVERSIBLE MANAGER COHORT**" |
| same | 144 | "Manager #515 selects reversible early G1 2 Oak +2 Beech" |
| `specs/scenario-one-compact-complete-loop-v1/spec.md` | 121, 128 | The original OPEN-C register still reads **OPEN-C2 "…are not approved"** and **"OPEN-C9 — PARTLY RESOLVED (MANAGER DRAFT) … reversible first-episode gameplay default"**. The appended register at line 144 says OPEN-C2 OWNER RATIFIED BASELINE and OPEN-C9 OWNER RATIFIED COUNT, so the one spec now holds two conflicting registers. |
| `specs/scenario-one-compact-complete-loop-v1/tasks.md` | 24, 40 | D-05 "(Manager #515 reversible DRAFT calibration…)"; C0-07 "#515 G1 2+2 is Manager provisional" |
| `specs/scenario-one-compact-complete-loop-v1/plan.md` | 42 | "2+2 is provisional cohort design" |
| `specs/scenario-one-compact-initial-planting-v1/plan.md` | 79 | Imperative: "**Label the current 2+2 G1 initial cohort as reversible Manager gameplay calibration**". This directly contradicts CIP-002 and the ratification. |
| `specs/scenario-one-compact-initial-planting-v1/spec.md` | 10 | "Manager provisional first lesson now G1 2 sessile Oak +2 Beech" |

**Why MAJOR rather than MINOR:** the banner's catch-all ("the 16 Nov owner ratification … govern[s] any conflicting *future* instruction") does not neutralise this. These lines are acceptance criteria and instructions that a future verifier or implementer is told to satisfy, and the #576 result §3 claims "targeted superseded wording corrected". The packet's S-2 requires the count to be classified OWNER RATIFIED BASELINE. A Manager-reversible label would also let a later Manager reopen a settled owner choice, which the owner record says not to do.

**Required bounded fix (author, not reviewer):**
- Retier those lines as "count OWNER RATIFIED 16 Nov; exact positions/fitness Manager candidate, native-unverified".
- In `spec.md`, either update the C2/C9 rows of the original register or label that register "14 Nov historical register — superseded by the 16 Nov register below".
- Do **not** change any R/CIP check scope, ID or checkbox. This is a provenance correction, not a weakening.

### m-1 MINOR — the child still declares a resolved one-rack parent conflict as an active BLOCKER (S-1)

`specs/scenario-one-compact-initial-planting-v1/plan.md:57` says: "**Parent reconciliation BLOCKER:** the previous parent compact full-loop DRAFT still assumes one extraction rack…".

`spec.md:81` says: "Parent DRAFT conflict … formulated around one extraction rack … Do not implement from two conflicting DRAFTs."

Both are now false on this branch, because the parent is the #516 no-rack DRAFT, and both still read as current gates. Mark them RESOLVED by the #576 consolidation, kept as history. Independent review of the consolidated pair remains a gate.

### m-2 MINOR — the G1 light 0.221 is mislabelled "pre-H1"

The packet, the manifest §D column header ("Source Year0 **pre-H1** validation proxy light") and the #576 result §1 (line 14, "for that *pre-H1 source model*") call 0.221 a pre-H1 value.

In #508's ledger, each G1 site's proxy is computed from `canopyAfter`, the canopy after the 14 H1 omissions. For cell 15, `canopyBase = canopyAfter = 0.7789`, so 1 − 0.7789 = 0.221. The value is therefore a **Year0 static source-equivalent proxy computed after the H1 omissions**, and H1 does not change cell 15. What remains unverified is the native, time-evolving (Year1/Year2+) light, plus understorey, browsing and survival.

Correct the label so nobody repeats a redundant static post-H1 calculation, or mistakes the static check for the missing native one.

### m-3 MINOR — the mechanical D-082→D-083 substitution produced awkward or misleading text

The global replacement `D-082` → `D-083 [proposed compact exception; D-082 remains Sitka experiment]` gives text such as:
- child `plan.md:16`: "Proposed first-demo D-083 [proposed compact exception; D-082 remains Sitka experiment] remains paused under … #425/#471";
- parent `plan.md:22`, `:46`: "the separate D-083 [proposed compact exception; D-082 remains Sitka experiment] canonical reconciliation";
- the repeated "D-083 …/#471" pairing.

#471 is the PAUSED stale one-rack/D-082 review. The #579 packet replaces its scope, so tying D-083 to #471 is misleading. Prefer "proposed D-083 (independent review #579; publication #425)", and keep one explicit note that D-082 is the unrelated Sitka lab.

### m-4 MINOR — D-083 narrows D-060/D-071/D-072 without saying so in the Decision Log row

D-083 calls itself "a new first-beta scope exception, not a repeal". In effect it:
- changes the target of D-060's "first external demo";
- narrows D-071/D-072 from "Scenario One" to "historical Enlarged80". The Milestone headings at `current-milestone.md:42`/`:50` are rewritten accordingly.

The D-071/D-072 rows still say "for Scenario One". The log's convention, as in D-060's "Supersedes…", is an explicit scope note. Add one sentence to D-083's last column, for example:

> Scope effect: the D-060 first-external-demo target becomes the qualified Compact world; D-071/D-072 apply to historical Enlarged80 only; D-059/D-060 duties are otherwise unchanged.

That way, readers of D-071 alone are not misled. The historical rows themselves need no edit.

### m-5 MINOR — the CG-TRIG draft leaves two lifecycle signals underspecified, including one that already exists in source

1. **Annual Review acknowledgement.** Signed spec §4.2 asks CG-TRIG to state "whether existing annual-review acknowledgement supplies that proof". Live `main` already has `ScenarioManagementEventType.AnnualReviewAcknowledged = 9` (`Assets/ForestPrototype/ScenarioOne/ScenarioManagementEvent.cs:16`), used by `ScenarioOneCompletionRules.NeedsReviewAcknowledgement` and `ScenarioCycleState.cs:21/98`.

   The draft (row "Open real Annual Review…", line 18; checklist line 40) proposes a *new* "Acknowledge this annual review" control "if required", without assessing this existing event. The signed spec forbids "a second fabricated event stream". CG-TRIG should evaluate binding `reviewAcknowledgedYear` to the existing event first, and propose a new control only if that event cannot express "opened and acknowledged the covering year".

2. **Assessment acknowledgement.** The precondition is left as "exact inspection source/display/event to be accepted" (line 14). It is the one step with no concrete candidate predicate. Owner/#424 need at least one proposed observable precondition to accept or reject.

This finding was observed by reading source on `main` only. Nothing was executed.

## Per-check disposition (S-1…S-8)

| Check | Result | Evidence |
| --- | --- | --- |
| **S-1** parent/child delta | **NEEDS CHANGES (M-1, m-1, m-3)** | All eight files diffed against the exact source blobs. The deltas are additive (banner, source trace, appended 16 Nov register, requirement-preservation note) plus targeted rewords. No deleted requirement text. Checklists: parent **115/115 unchecked, 0 checked, ID set identical**; child **33/33 unchecked, 0 checked, ID set identical**. Only R-004, R-115, CIP-002 and CIP-023 changed text, all as label/authority rewords with no scope reduction. No operative 4+4, three-in-rack, four-rack extrapolation, forced Crop/Fell or phantom rack marks. Remaining one-rack mentions are labelled #460 history, apart from the m-1 stale blocker. |
| **S-2** ratification | **NEEDS CHANGES (M-1); manifest PASS** | 30 m, ~12×10 H1, 14 pre-world omissions, winding pedestrian path and no rack are labelled OWNER RATIFIED BASELINE in the banners, appended registers and D-083. 189 is shown as research input and 175 as conditional throughout. **All 14 H1 IDs and x/z, the 8 one-metre-buffer IDs, the 8 Route A waypoints and the 4 G1 coordinates with their nearest-stem distances match #508 §3/§4/§5 and its durable `ledger.json` exactly.** No untraceable ID or coordinate. Manifest §E checkboxes are all unchecked and its status is UNSIGNED. |
| **S-3** save | **PASS** | Manifest §B literals match signed §4.1 exactly: `o1-historical` with no persisted ID; `o1-compact-v1` / `scenario-one-compact-v1` / `compact30-v1` / `scen1-compact-h1-g1-v1` / Compact30=2 as a dispatch reservation only; `forest-save-scenario-one-compact-v1.json`; O2 reserved. Envelope schema1/payload24, three root keys, neutral rack fields, SI-09 prefixes and the SI-10 freeze at first writer integration all match. The signed spec path is absent from the diff, so its bytes are untouched. ISO-58 stays BLOCKED, 63 ISO unchecked, and Compact is not activated. CG-TRIG evidence keys match signed §4.2 (`evidenceVersion`, two booleans, ≤4 `groupOrderIds`, `plantings` rows `workOrderId,speciesId,juvenileId,completionYear`, `reviewAcknowledgedYear` initially −1). No silent ISO-58 signature. |
| **S-4** teaching | **PASS WITH m-5** | Proposes player action → event → durable evidence → negative case for arrival, assessment, four owned-stock Pending orders, Approve, annual Completed, opened-vs-acknowledged review and derived completion. Covers cancellation/replacement (consistent with signed §4.2), stock/location failure, failed/partial years, no-advance, seven save/reload cut points, navigation skip, later work not substituting, and idempotent re-acknowledgement. Report-open, dismiss and skip explicitly do not count. No implementation claim. |
| **S-5** economy/completion | **PASS** | €12,000 traces to the #490 Year0 table; the €2,500 minimum and €36.92 → −€2,463.08 to #490 lines 51–53. €20 stock (2×€5.50+2×€4.50), €20 shelters and €30 labour (4×10 min×€45/h) are consistent with #508 §5. All are labelled gameplay calibration, not Irish data. A/B/C alternatives are shown with **none selected** and OPEN-C5 open. No mandatory three-cycle or Fell quota. The substantive classifier excludes the four G1 orders, no-work and annual ticks. Feasibility is marked native-pending. The non-profit framing traces to source parent S-11. |
| **S-6** canonical | **PASS WITH m-3, m-4** | `D-083` is absent everywhere on live `main` (`git grep`); `main`'s highest row is D-082. The D-082 row is **byte-identical** between `main` and the candidate. D-083 states the first closed-beta Compact exception explicitly. "Closed beta" traces to the Manager scope packet's record of the owner's 08:07 instruction. D-064/065/071/072, Legacy40, v1–v24, Reference Future v1 and `forest-save.json` are preserved, with no migration. Both canonical files are staged on the branch only, pending #425. |
| **S-7** claim/source | **PASS** | #576's issue-body edit history shows **[RUNNING] at 08:41:23Z** (GPT-6, branch, base `09bf0b42`, worktree NONE, allowlist) before the first candidate commit at 08:44:41Z, and [HANDED-OFF] at 08:56:50Z. Registration was a body edit, not a comment, and is preserved in GitHub edit history. `main…candidate`: **exactly 15 paths, all Markdown, all on the allowlist**. No Assets/Packages/ProjectSettings/Tools/CCF-Ops/coordination/save/build paths. #471 is still open and PAUSED, untouched. No claim of a user beta test. |
| **S-8** graphics | **PASS** | No candidate file treats #573 as Compact qualification. The result line 26 states #573 is E80-only and "not a Compact art benchmark". Specs and manifest require future 720p/1080p native views and claim no graphics evidence. |

## Fitness for #425/#424 sign-off

**Not yet.** After a bounded #576/#425 author correction of **M-1** (with m-1…m-5 recommended) and a **short nonauthor delta re-check limited to the changed lines**, I expect the package to be fit for owner final sign-off on the DRAFT specs and for #425 to publish D-083. No re-review of unchanged content is needed. Everything outside these findings checked clean.

Even after that, the following stay open, as the candidate itself states: the signed world manifest; CG-TRIG acceptance and ISO-58; the OPEN-C5 funding and completion choice; OPEN-C8; T-03B/T-04/T-05 and the 63 ISO native checks; native post-H1 G1 evidence; graphics; the personal Compact Editor test; and separate Windows-build authorisation.

## Reviewer limitations

- Source-text review only. Unity, Editor, scenes, saves, Prefs and native evidence were not run or inspected.
- #508 values were checked against its published result and durable ledger. I did not re-run its scripts.
- The m-5 source pointer comes from reading `main`, not from execution.
- Owner-intent wording was checked against the recorded owner and Manager documents. The owner was not consulted.
- Dashboard/Drive: NOT VERIFIED.
