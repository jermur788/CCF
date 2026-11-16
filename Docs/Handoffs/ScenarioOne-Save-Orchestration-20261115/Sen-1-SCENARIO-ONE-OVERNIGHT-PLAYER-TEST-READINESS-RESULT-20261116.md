# Sen-1 — Scenario One overnight player-test readiness result

**Issued:** 16 November 2026, under the [Sen-1 overnight player-test readiness amendment](https://github.com/jermur788/CCF/blob/docs/sen-1-overnight-player-test-amendment-20261115-v1/Docs/Handoffs/ScenarioOne-Save-Orchestration-20261115/Sen-1-OVERNIGHT-PLAYER-TEST-READINESS-AMENDMENT-20261115.md). **Orchestrator:** DeepSeek V4.1 Flash 1017 (`opencode-go/deepseek-v4.1-flash`), effort **high**, **orchestrator only** — no C# authored, no source review performed, no Unity run, no self-approval. **Ledger:** [scen1-ORCHESTRATION-EVIDENCE-LEDGER-20261115.md](https://github.com/jermur788/CCF/blob/orchestration/scen1-save-v2-post-signature-20261115-v1/Docs/Handoffs/ScenarioOne-Save-Orchestration-20261115/scen1-ORCHESTRATION-EVIDENCE-LEDGER-20261115.md), EVENTS 001–046 continuous, inherited from closed #535.

**One-line answer: the five-rack Scenario One is CONDITIONAL for a personal playtest pending three owner/manager answers; the shared-save source is reviewed and accepted as a source stage but is NOT in any playable candidate.**

---

## 1. Exact candidates, stage claims, reviews and heads

| Stage | Candidate | Independent review | Status claim |
| --- | --- | --- | --- |
| T-02 raw admission + pure capture | `2353a792` on `feature/scen1-shared-three-profile-save-v2` | **#557** Opus 5.5, cross-provider, **NEEDS CHANGES** 0B/1M/4m/3n | superseded by T-02R |
| T-02R refusal + admission repair | `460fd51c` (source `5e21c944`) | **#569** Sonnet 5.5, nonauthor, **APPROVED** 0B/0M/2m/4n | **T-02 SOURCE PROVISIONALLY ACCEPTED FOR T-03A ONLY** |
| T-03A safe bootstrap / staged seam | `09ade4c4` (source `b730459a`) | **#571** Sonnet 5.5, nonauthor, **APPROVED narrow** 0B/0M/4m/4n | **fail-closed interim seam; requirements 1 and 2 NOT fully met, T-03B owed** |
| Candidate A — five-rack personal test | `55863f1b` on `integration/scenario-one-personal-playtest-d073-20261112-v1` | prior #426/#435/#427 evidence | **CONDITIONAL, not READY** |
| Candidate C — compact world | none | none | **PAUSED, not begun, never described as playable** |

**Signed contract:** `specs/scenario-independent-save-isolation-v1/{spec.md,plan.md,tasks.md,checklists/requirements.md}` at **`200add4e`**, plus the external [D-A attestation](https://github.com/jermur788/CCF/blob/docs/scen1-manager-save-v2-immutable-approval-20261115-v1/Docs/Handoffs/ScenarioOne-Save-Orchestration-20261115/scen1-MANAGER-SHARED-SAVE-V2-IMMUTABLE-SPEC-SIGNATURE-20261115.md). **Note for reviewers:** the blobs still carry their pre-signature "APPROVAL CANDIDATE V2 — NOT APPROVED" banner. That banner is frozen historical text; the external immutable attestation is the approval. Reviewer #571 correctly flagged the conflict rather than guessing, which is the right behaviour.

**ISO checklist: 63 of 63 items UNCHECKED, 0 ticked** — verified from the signed checklist at `200add4e`. No ISO item is claimed by this report.

---

## 2. Five-rack player-test readiness — **CONDITIONAL**

Candidate **A** (`55863f1b`) is the only artefact that could be played tonight. Its branch, HEAD, Unity lane and launcher (`1d43336e…`) are unchanged since the 12 Nov conditional GO.

| Gate | Status | Evidence basis |
| --- | --- | --- |
| Functional player path | PREPARED-ONLY | prior qualification; **not re-run tonight** |
| Rendered / visual | PREPARED-ONLY | #427 at 720p/1080p, #437 proof, user's own first view |
| Save / Reference / legacy | PREPARED-ONLY | prior passes; **known-weak writer, see §5** |
| Editor lane + lease | RAN (read-only snapshot) | lock free, 0 Editor PIDs, lane clean, 52 GiB free |
| Personal-test safety | **CONDITIONAL** | needs the three decisions in §3 |

**Tests actually run tonight: none in Unity.** The readiness matrix was a read-only documentation inventory and says so. Everything above is inherited evidence or live read-only observation.

### The structural fact that decides the night's ceiling

**Candidate B (the reviewed save source) is a sibling of Candidate A, not a newer version.**

```
git rev-list --count 55863f1b ^2353a792  ->  74    (A's commits absent from B)
git rev-list --count 2353a792 ^55863f1b  ->  378   (B's commits absent from A)
merge-base                               ->  50042bbe
```

**No test result transfers between A and B in either direction.** A candidate that is both playable *and* carries the new save system requires an authorised merge plus fresh qualification. That was not attempted and is not claimed.

---

## 3. Minimum concrete next user action

**Answer three questions; none requires a code change from you.**

1. **Second-session re-GO.** The 12 Nov conditional GO covered **one** session and that session happened. A second session needs your re-confirmation. A one-session GO is not treated as standing permission.
2. **Launcher: option (a) or (b).** The launcher's allowed-drift list (lines 110–121) omits `Berry_Calyx.mat` and `Bilberry_Berries.mat`, both of which appear in the **actual** 12 Nov drift file. A normal exit will likely emit `STOP: unexpected lane changes` and the next `--check-only` will refuse.
   - **(a)** widen the list by exactly those two material paths — the better permanent fix, but it **changes the launcher's bytes and voids the check your GO covered**.
   - **(b)** keep the launcher byte-identical and treat an exit STOP as expected and documented, with the recovery step in the pack.
   - **Orchestrator recommendation: (b)** for tonight, (a) through the normal amendment path later.
3. **Physical board sync (#553).** Last written 13 Nov; its Candidate A row still reads "NOT STARTED". Blocks a READY *claim*, not the playtest. Needs the physical board owner.

**Then, if you re-GO:** paste-ready pack at [Sen-1-FIVE-RACK-PLAYER-TEST-READINESS-MATRIX-20261116.md](https://github.com/jermur788/CCF/blob/sen-1/five-rack-readiness-matrix-20261116-v1/Docs/Handoffs/ScenarioOne-Save-Orchestration-20261115/Sen-1-FIVE-RACK-PLAYER-TEST-READINESS-MATRIX-20261116.md). Two things it adds that the old runbook lacked: **F10 → "Reset tutorial/help progress…"** first (the throwaway profile still holds 12 Nov prefs, so first-time help will not reappear), and a **save-error stop sign** — do not trust "Your previous save is unchanged" if a save ever fails.

---

## 4. Hard blockers, and the next authorised tasks

**Hard blockers to a READY verdict: 3**, all authority/artefact gates rather than defects — the two decisions above plus the physical board.

**Next authorised work, in order:**
1. **T-03B** — close T-03A's unmet requirements 1 and 2. Requires a **Manager-widened packet**: `ForestStartingStand.cs`, `ForestEcologyController.cs`, `ScenarioOneManager` preview code, geometry consumers, preference consumers. **Not authorised by me**, and not absorbed by reinterpretation.
2. **T-04A** — offline writer/recovery primitives, only after T-03B's review, from the signed T-04 text and the 71-row matrix already prepared.
3. **Cross-provider re-check** of T-02R and T-03A when an OpenAI pool returns — author and reviewers were all Anthropic tonight, so independence is same-provider and must be described that way.

---

## 5. What passed, what failed, and what is simply unverified

**Passed (offline, source-level, on exact heads):**
- T-02R: harness **460/0**, mutants **27/28** with every kill a real assertion failure, Roslyn typecheck against real Unity 6000.6 assemblies **0 errors** both define sets; four silent-success assertions **replaced, not deleted**.
- T-03A: harness **748/0** (original 460 unchanged), ordering proof **18/0**, mutants **15/16** — reviewer found **T7 a genuine coverage gap** and wrote a probe that kills it (**752/0**); its own 8 mutants all killed; typecheck **0/0** both define sets including the 9 `/Editor/` files.

**Failed or overstated, caught by review rather than shipped:**
- **M-1** (first review): silent refusals on all three menu routes — a real regression introduced during T-02 and **undisclosed in its result doc**. Fixed in T-02R.
- **T7 mutant survivor**: a refused apply must keep the "saving is off" flag and no committed test reached that branch. Reviewer's probe closes it; the test is **not yet in the branch**.
- **T-03A ordering claim is partly cosmetic**: `ForestStartingStand.Awake` and `ForestEcologyController.Awake` still build world content regardless of the manager's gate. Unreachable today, but the doc's "refused selection → nothing constructed" **overstates** it.
- **Stale live message**: `ScenarioOneManager.cs:717-719` still says "rejected before anything in the world changed", which `Apply` returning false after a partial change no longer guarantees.

**Unverified, and not to be read as passing:** any Unity compile, Editor or Player run; real `ApplyAdmittedData` or `Destroy` timing; engine callback order; native `JsonUtility` behaviour including the parser differential; real saves, PlayerPrefs and keyboard flows; the m-3 Editor verification-script migration; T-04 and T-05 entirely.

**A capacity lesson worth recording:** OpenAI A refused on quota (reset 2026-11-18T13:43); **B hit its usage limit mid-repair and again mid-T-03A**; Claude A refused on **billing, not quota**. I also briefly recorded B as "recovered" because a dispatch was accepted — **that was wrong and is corrected in EVENT 044**: a queued dispatch is not an allowed run. Only a provider refusal proves unavailability and only a completed run proves availability.

---

## 6. Claim closure, physical state, ledger and mirrors

- **#551** open, superseded by T-02R; **#563** handed off, open for Manager; **#569** and **#571** handed off, open for Manager verdict; **#570** handed off PARTIAL, open. **#557** handed off. **#548** handed off, open, one semantic residual (off-storage lane: *reported* vs *rejected*) left as an owner decision; **no further ops rounds funded**.
- **CCF-Ops, as actually observed:** `unity-lock status` = **free**, **0 Editor PIDs**, positive identity PASS, all negatives rejected, board lint PASS. **#553 physical board still unsynced and stale.** The lease tooling is verified usable; **no Unity lease was acquired by this run and no Editor was launched**.
- **User's protected work:** `AGENTS.md` and `Docs/Operations/StorageArchitecture.md` in the primary anchor remain uncommitted and untouched, hashes verified unchanged mid-run.
- **Ledger:** continuous, EVENTS 001–046, appended never rewritten. **Event header timestamps are approximate; git commit times and GitHub comment times are authoritative** — the two disagree by up to ~30 minutes in places and the issue record should be trusted.
- **Dashboard ingestion / Drive mirrors: NOT VERIFIED.** Nothing was checked, so nothing is claimed.

---

## 7. Throughput rows for the project ledger owner (#424/#425) — append-only

All times UTC from git commit metadata or GitHub comment `createdAt`.

| Stage | Start | End | Elapsed | Provider/model | Cycles | Note |
| --- | --- | --- | --- | --- | --- | --- |
| T-02 source (3rd attempt) | 2026-11-15T22:41Z | 2026-11-15T23:08Z | ~27 min | OpenAI B `gpt-6.1-sol` high | 2 prior for-cause stops, 0 lost work | checkpoint `f14751b3` preserved both stopped attempts |
| T-02 review | 23:11Z | 23:34Z | ~23 min | Claude B Opus 5.5 | 1 | cross-provider; mutation-tested the tests (11/12) |
| T-02R repair | 23:53Z | 2026-11-16T00:14Z | ~21 min | Opus 5.5 (fallback) | 1 of 2 permitted | OpenAI A/B both refused; 4-file allowlist |
| T-02R delta review | 00:16Z | 00:28Z | **~12 min** | Sonnet 5.5 | 1 | delta-scoped; rebuild + mutants + typecheck |
| T-03A source | 00:37Z | 01:52Z | ~75 min | Opus 5.5 (fallback) | 1 | two background-wait stalls cost visibility, not work |
| T-03A review | 01:52Z | 02:14Z | ~22 min | Sonnet 5.5 | 1 | completed author's PARTIAL mutants (12/13) and both typechecks |
| Ops durability | 22:46Z | 23:34Z | ~48 min | OpenAI B `gpt-6.1-sol` high | 3 gate rounds | live tooling already worked; defect was unversioned drift |
| Readiness matrix (P2) | 23:53Z | 00:01Z | ~8 min | Opus 5.5 | 0 | read-only, no Unity |
| T-04 matrix (P1B) | 23:53Z | 00:04Z | ~11 min | Sonnet 5.5 | 0 | 71 rows, nothing executed |

**Throughput observations, labelled as observations:** delta-scoping the second review cut review time from ~23 min to ~12 min while still rebuilding the harness — scoping to the delta saved roughly half a review cycle. Substituting providers cost one dispatch per event and **zero rework** across four switches. The two background-wait stalls were the only real time lost, and both were resolved by steering with a "commit first" instruction.

---

## 8. What this report is not

Not a Unity qualification, not integration, not acceptance, not a Windows build authorisation, and not a playtest. **No Windows build was produced or attempted.** The compact world remains paused and undecided; its decision card is at [Sen-1-COMPACT-WORLD-OPEN-DECISIONS-CARD-20261116.md](https://github.com/jermur788/CCF/blob/sen-1/compact-open-decisions-card-20261116-v1/Docs/Handoffs/ScenarioOne-Save-Orchestration-20261115/Sen-1-COMPACT-WORLD-OPEN-DECISIONS-CARD-20261116.md) (10 identifiers, not the 8 the amendment assumed, and a live **D-082 number collision** with the Sitka annual-growth decision that needs a different number assigned before anyone uses it).

**Scope note, stated because hiding it would be wrong:** #425 recorded an owner re-scope of overnight work at 16 Nov 00:00 to the standalone 2D Sitka lab (#565) and explicitly said #424/#544 were **not touched**. This run continued under the direct instruction to this session plus the Sen-1 amendment, and #565 is a disjoint lane, so no capacity or file conflict occurred. If that reading is wrong, this report is the evidence to correct against.
