# PLAYER-TEST-OVERNIGHT-01 — Overnight Result — 2026-10-30

**Parent claim:** #91  
**Orchestrator:** DeepSeek V4.1 Flash (OpenCode / T3)  
**Run window:** 2026-10-30T00:36Z – approximately 04:40Z  
**Browser lane:** #162  
**Local/Unity umbrella:** #106  
**No Windows build.**

## Return condition

**`SCENARIO ONE PLAYER-TEST CANDIDATE READY — USER PLAYTEST REQUIRED` was NOT reached.**

The programme did not hit a product/design stop. It stopped at a verification-infrastructure and serialized-Unity boundary:

- the v24 schema bump exposed a family of stale `CurrentVersion == 23` legacy harness pins across several gates;
- two reconciliation waves fixed the #124 and PARITY-08 sets, but further untouched pins remain and some required PARITY-08 checks (model-0 completion replay, Sitka timber anchors) still have no Unity runner;
- #151 and #156 have offline candidates but their rendered Unity gates have not run;
- #126 has not received its independent #128 review and is not integrated;
- one intermittent Unity Editor exit hang after PASS remains a rerun risk (never waived).

Safe work remains, so this is a **session-capacity/verification-debt stop**, not a hard product decision. Exact next actions are below.

---

## What was achieved

| Result | Exact evidence |
|---|---|
| **#143 S1-D T-04P integrated** | Production `2f476a52`; docs successor `bccd2b90`; candidate `b9bf11aa`; independent review APPROVED; eight #143 Unity gates PASS; hash dump byte-identical to accepted MFM-07 base. |
| **#124 S1-D T-04 accepted/integrated** | Offline review APPROVED; T-04 guide Unity `S1D_T04_GUIDE_UNITY_PASS checks=93`; stale legacy harnesses reconciled by #164; integrated at **`185200f5`**; production unchanged. |
| **#164 v24 harness reconciliation PASS/integrated** | TeachingCopy 61, RackLesson 36, teaching-latch 60 ×3 clean, S1-C history 279, T-04 529, T-06 170; all exit 0; verification-only. |
| **#126 PARITY-08 offline PASS** | Found a real defect: unknown species IDs were never validated; unknown tree species loaded as Sitka and unknown regeneration cohorts were silently dropped, while `CanLoad` said loadable. Added pre-mutation `ValidateSpecies` shared by `CanLoad`/`LoadData`; no schema bump; 23/23 new checks + mutants killed. |
| **#126 Unity required set clean via #165** | Compile v24, hardening incl. no-mutation unknown-species rejection, save 159, hash dump byte-identical, Reference Future, teaching-latch 60, approval 106, N3, interaction, progress, completion, completion-model1, density-save — all clean exit 0 on `3eb9c954`. |
| **#151 S1-D T-05 offline candidate** | Production `98ac98b5`, verification `38904535`, docs `4ddf4aa5`; 219/219 checks + 7/7 mutations; independent review APPROVED (offline/static). Unity not run. |
| **#156 MFM-08 offline candidate** | Production `d55ec86a`, docs `d70cec17`; MFM-08 106/106; MFM-04/05/07/NonUI unchanged; no excluded/serialized files. Unity not run. |

### Exact current lineage

- Scenario One integration branch: **`185200f5`** (production tree identical to `bccd2b90` / `2f476a52`; schema v24).
- #143: merge `2f476a52`, docs `bccd2b90`.
- #124: verification `c838aca`, docs `99b7d4b`, integrated at `185200f5`.
- #164: `edba1c35`, integrated at `185200f5`.
- #126: candidate **`3eb9c954`**, branch `verify/parity-08-save-compatibility-v1`; **not integrated**.
- #165: `508c2d07`, branch `verify/s1d-v24-legacy-gate-reconciliation-2`; **not integrated**.
- #151: docs `4ddf4aa5`, production `98ac98b5`; not integrated.
- #156: docs `d70cec17`, production `d55ec86a`; not integrated.
- Docs/main carried by the orchestration branch `orchestration/player-test-overnight-20261030`.

### Protected anchors

No hash/reference/protected anchor was rebaselined. The #143 and #126 hash dumps are byte-identical to the accepted baselines. Completion-rule normalized hash and Reference Future resources are unchanged.

---

## Open state / blockers

### #126 PARITY-08 (highest-value open item)

- Offline/static PASS; Unity required set clean via #165.
- **Not done:** independent #128 review on the exact candidate + Unity evidence.
- **Residual gaps:** model-0 completion replay has no runner mode; Sitka timber anchors have no Unity runner actually driven on an Editor; #165 added `progress`/`density-save` to the runner isolated-save allowlist (manager may accept or reject); other stale v23 pins remain in harnesses outside the #126 set.
- **Not integrated.** No parity schema field or anchor change.

### #151 S1-D T-05

- Offline/static APPROVED (independent Claude A).
- Unity rendered three-cycle gate at 1280×720 / 1920×1080 not run.
- Tiny authorised API overlap (`AnnualReviewView.TryShowYear` + `ScenarioOneUiRoot.TryShowReviewYear`) is implemented and reviewed offline.
- Not integrated.

### #156 MFM-08

- Offline candidate only; Unity Tier-2 import/render/interaction not run.
- Two API questions remain for Manager/Unity review: rack-sample geometry read and living-tree stem list; card height at 1280×720; `Painter2D` path; tab placement.
- Not integrated.

### Final Tier-3

Not reachable in this session. It still requires #126/#128, #151 and #156 Unity/review/integration, MFM-09 preparation/implementation, and one exact combined Tier-3 candidate.

### Unity/harness risk

- Intermittent Unity Editor exit hang after a PASS marker: 2/5 teaching-latch launches (#124/#164), 1/2 progress launches (#126), 1 completion-model1 launch (#165). A hung receipt is a failure and was rerun, never waived.
- ZX20 free space hovered around 19.5–20 GiB: above the 15 GiB mandatory floor, at/below the 20 GiB preferred headroom. Every Unity acquisition must fresh-check.

---

## Exact next actions (ordered)

1. **Review and integrate #165** (`508c2d07`, verification-only) after a read-only manager/orchestrator review; decide whether its `progress`/`density-save` allowlist additions are acceptable. If rejected, those gates have no runner-clean receipt and must be handled explicitly.
2. **Close #126 residuals** by either (a) adding a real Unity model-0 completion replay and Sitka timber anchor run, or (b) obtaining explicit Manager acceptance of those gaps with the existing offline/Unity evidence. Do not silently waive them.
3. **Run #128 independent review** of exact #126 candidate `3eb9c954` + the #165 clean receipts. Reviewer must be cross-provider from the Claude Opus implementer (DeepSeek or OpenAI when restored).
4. **Integrate #126** only after #128 APPROVED and residuals dispositioned; then continue the accepted parity dependency chain toward PARITY-09.
5. **Run #151 Unity** rendered three-cycle gate at 1280×720 and 1920×1080; then independent/Manager review and integration.
6. **Run #156 Unity Tier-2** import/render/interaction; then independent/Manager review and integration.
7. **Prepare MFM-09** in browser before local implementation; run it after accepted/integrated MFM-08 + S1-D T-04/T-05.
8. **Final Tier-3** on one exact combined candidate; then the user's personal Scenario One playtest. No Windows build until explicit post-playtest authorisation.

Recommended immediate harness policy: perform one systematic sweep for remaining `CurrentVersion == 23` / pre-v24 pins before further Unity work, so each gate is fixed once rather than rediscovered task by task.

---

# Orchestration Decision Appendix

## 1. Orchestration policy

- **Orchestrator:** DeepSeek V4.1 Flash (`opencode-go/deepseek-v4.1-flash`), T3/OpenCode, full-access.
- **User routing priorities (direct instruction):**
  - DeepSeek is the overnight orchestrator and default worker/reviewer/integrator.
  - Use DeepSeek for every subtask it can competently perform.
  - OpenAI B (`codex_c75d8114…`, `gpt-6.1-sol`) was to be used first after reauthentication, with about 13% of its five-hour window remaining; reuse only when the provider actually restores capacity.
  - Claude Sonnet 5.5 Medium for materially harder code/UI/Unity/review.
  - Escalate to Claude Opus 5.5 Low/Medium only for genuine high-consequence difficulty or prior failure.
  - OpenAI A was believed token-empty by the user; not used.
  - No quota/auth bypass. Record UNKNOWN when telemetry is unavailable.
- **Concurrency:** one writer per subsystem/worktree; Unity machine-wide serialized; browser work separated from local Code/Unity work.
- **Review:** independent review for high-risk save/reference/determinism work; reviewer read-only; repairs return to the owning implementation claim.
- **Protected decisions:** no product/design changes, no anchor rebaseline, no validator/gate weakening, no Windows build before the user's playtest.

## 2. Material routing decisions

| Work unit | Previous | Selected | Alternatives | Why / rule | Capacity evidence at decision | Source | Reset | Prior outcome | Result |
|---|---|---|---|---|---|---|---|---|---|
| #161 #143 review | OpenAI gpt-6.1-sol implementer | DeepSeek V4.1 Flash | Claude Sonnet/Opus | Cross-provider independence; browser/static sufficient | DeepSeek available; Claude UNKNOWN | T3 | n/a | Implementation complete | **APPROVED** |
| #151 initial implementation | none | OpenAI B gpt-6.1-sol high | DeepSeek, Claude | Direct user instruction to use reauthenticated B first | ~13% of five-hour window | user | UNKNOWN | None | Pushed `98ac98b5`; cut off mid-turn by subscription limit |
| #156 offline implementation | none | Claude B Sonnet 5.5 medium | DeepSeek | Architecture-sensitive UI; user policy | Credentials present; quota UNKNOWN | T3 | UNKNOWN | None | Offline candidate `d55ec86a` |
| #124 offline implementation | none | DeepSeek V4.1 Flash max | Claude A | Default capable worker; bounded curriculum verification | DeepSeek available | T3 | n/a | #143 integrated | Offline PASS; no production change |
| #151 resume | OpenAI B (exhausted) | DeepSeek V4.1 Flash max | Claude A/B | Capacity failover; preserve pushed/uncommitted work | OpenAI B exhausted; DeepSeek available | provider/T3 | OpenAI B UNKNOWN | OpenAI B cut off | Offline candidate `4ddf4aa5`; Claude A review APPROVED |
| #124 Unity | none | Claude B Sonnet 5.5 medium | DeepSeek | Unity capability and context | Claude B available; quota UNKNOWN | T3 | UNKNOWN | Offline review approved | Initial NEEDS WORK on stale harnesses; T-04 guide 93 passed |
| #164 harness reconciliation | Claude B | Claude B Sonnet 5.5 medium | DeepSeek | Same Unity context; verification-only | Claude B available | T3 | UNKNOWN | #124 harness failures | **PASS**, integrated |
| #126 PARITY-08 implementation | none | Claude Opus 5.5 low | Claude Sonnet, DeepSeek | 5/5 save/determinism consequence; user permits Opus Low | Claude A available; quota UNKNOWN | T3 | UNKNOWN | #124 accepted | Offline PASS; found real unknown-species defect |
| #126 Unity | none | Claude B Sonnet 5.5 medium | DeepSeek | Serialized save/reference qualification | Claude B available | T3 | UNKNOWN | Offline PASS | Partial; all required species/v24 gates clean; four historical gates stale |
| #165 gate reconciliation 2 | Claude B | Claude B Sonnet 5.5 medium | DeepSeek | Continuity and verification-only scope | Claude B available | T3 | UNKNOWN | #126 partial | **PASS**; clean `3eb9c954` receipts |
| #151 review | DeepSeek implementer | Claude A Sonnet 5.5 medium | Opus | Cross-provider independence; offline evidence | Claude A available | T3 | UNKNOWN | Offline candidate | **APPROVED** (offline/static) |
| #124 review | DeepSeek implementer | Claude A Sonnet 5.5 medium | Opus | Cross-provider independence; offline evidence | Claude A available | T3 | UNKNOWN | Offline candidate | **APPROVED** (offline/static) |

## 3. Capacity events

| Provider/pool | Timestamp (approx) | State | Reported reset/window | Source | Routing change | Next model |
|---|---|---|---|---|---|---|
| OpenAI B | 00:36Z | ~13% of five-hour window remaining; just reauthenticated | Five-hour window; reset **UNKNOWN** | user | Yes — used first for #151 | gpt-6.1-sol high |
| OpenAI A | 00:36Z | Believed token-empty; not tested | UNKNOWN | user | Yes — not used | none |
| DeepSeek | 00:36Z onward | Available / busy | n/a | T3/execution | Orchestrator and default worker | DeepSeek |
| Claude A | 00:59Z onward | Available; no quota error; remaining UNKNOWN | UNKNOWN | T3/worker | Used for #124/#151 reviews | Sonnet 5.5 medium |
| Claude B | 00:36Z onward | Available; no quota error; remaining UNKNOWN | UNKNOWN | T3/worker | Used for #156/#124 Unity/#164/#126 Unity/#165 | Sonnet 5.5 medium |
| OpenAI B | 01:0xZ (#151 resume) | **COOLDOWN/QUOTA** — direct error: "The ChatGPT user has reached their Subscription Sharing usage limit." | Five-hour subscription; reset **UNKNOWN** | provider | Yes — #151 re-routed to DeepSeek | DeepSeek V4.1 Flash |
| Unity lane | 01:02Z–04:31Z | Serialized; free/held cycles | n/a | lock service/process list | No provider change | Claude B Unity |
| Unity exit hang | multiple | Intermittent Editor process hang after PASS | n/a | Unity logs | Reruns only; no waiver | Same |

No quota value or reset time was invented. Where telemetry was absent, the ledger says **UNKNOWN**.

## 4. Worker outcomes

| Task | Provider/model | Outcome | Evidence | Handoff/commit | Rework | Accepted? | Failure classification |
|---|---|---|---|---|---|---|---|
| #143 implementation | OpenAI gpt-6.1-sol high | Implemented + Unity PASS | 8 gates, hash dump identical | `b9bf11aa` / `d8168926` | None | **Yes**, integrated `2f476a52` | none |
| #161 review | DeepSeek V4.1 Flash | APPROVED | Independent offline rerun + Unity log review | `INDEPENDENT-REVIEW.md` | None | Yes | none |
| #151 initial | OpenAI B gpt-6.1-sol high | Cut off after pushing production + creating untracked verification | `98ac98b5`, untracked `PreviousProposed/` | #151 | Resumed by DeepSeek | Offline accepted; Unity pending | provider capacity |
| #151 resume | DeepSeek V4.1 Flash max | Completed offline/static | 219/219, 7/7 mutations, regressions | `4ddf4aa5` | None | Offline APPROVED by Claude A | none |
| #156 offline | Claude B Sonnet 5.5 medium | Offline candidate | 106/106, MFM-04/05/07/NonUI | `d55ec86a` / `d70cec17` | Token corrected to provisional | Offline candidate only; Unity pending | over-claimed token (manager corrected) |
| #124 offline | DeepSeek V4.1 Flash max | No production change needed; verification | 62 checks + 5/5 mutations; 16,384-row oracle | `c838aca` / `99b7d4b` | None | Offline APPROVED | none |
| #124 Unity | Claude B Sonnet 5.5 medium | NEEDS WORK on stale harnesses, no production regression | T-04 guide 93; stale v23/pre-D-066 gates | #124 comment `6513087927` | → #164 | Not as-is | stale harness + intermittent hang |
| #164 | Claude B Sonnet 5.5 medium | PASS, verification-only | TeachingCopy 61, RackLesson 36, teaching-latch 60 ×3, S1-C 279/529/170 | `edba1c35`, integrated `185200f5` | None | **Yes**, integrated | none |
| #126 offline | Claude Opus 5.5 low | Offline PASS; found real unknown-species fallback defect | 23/23 + mutants, MFM 172, timber/isolated density | `3eb9c954` | None | Offline candidate; Unity partial | none (real production defect repaired) |
| #126 Unity | Claude B Sonnet 5.5 medium | Partial; required species/v24 gates clean; four historical gates stale | compile/save/hardening/reference/latch/approval/N3/interaction clean | `RESULT.md`, #126 comment `6514614077` | → #165 | Not yet | stale v23 pins + runner rule + hang |
| #165 | Claude B Sonnet 5.5 medium | PASS, clean reruns | progress/completion/completion-model1/density-save exit 0 | `508c2d07` | None | Not integrated yet | none |
| #151 review | Claude A Sonnet 5.5 medium | APPROVED (offline/static) | 219/219 rerun; scope/API/history verified | #151 comment `6512416568` | None | Offline accepted | none |
| #124 review | Claude A Sonnet 5.5 medium | APPROVED (offline/static) | 62 + 5/5 mutations; production unchanged | #124 comment `6512358394` | None | Accepted with Unity via #164 | none |

**A worker saying "completed" was not treated as acceptance.** Several workers posted success tokens that were corrected to provisional because the Unity gate was not run (#124, #151, #156).

## 5. End-of-session assessment

**Worked well**

- DeepSeek orchestration kept the full dependency graph, evidence ledger and routing state current while multiple providers ran.
- Cross-provider reviews were genuine: DeepSeek reviewed OpenAI’s #143; Claude A reviewed DeepSeek’s #124/#151; Claude Opus’s #126 remains for #128.
- #126 found a real production defect (unknown species fallback) rather than only running a stale test.
- Bounded verification-only claims (#164/#165) isolated harness debt from production risk.

**Wasteful or incorrect**

- Starting #151 on OpenAI B with only ~13% of a five-hour window; the provider cut off mid-turn. Work was preserved, but the same task then needed a DeepSeek resume.
- Not performing a systematic v24 harness sweep before Unity qualification. Stale v23 pins were rediscovered one gate at a time (#164, then #165), consuming serialized Unity time.
- Letting #124/#151/#156 workers post success tokens before Unity; corrected, but it created avoidable status noise.

**Repeated failure patterns**

- Stale `CurrentVersion == 23` / pre-v24 assumptions across legacy harnesses.
- Intermittent Unity Editor exit hang after PASS (2/5, 1/2, 1/7 in different chains). A hung receipt was never accepted.
- One-off worker turns ending while a Unity chain continued; the orchestrator had to reattach and inspect receipts directly.

**Where a cheaper model would probably have sufficed**

- #164/#165 harness pin edits and Unity reruns were mostly mechanical; DeepSeek could likely have done them, though Claude B’s continuity reduced rediscovery.
- #151 offline verification was performed by DeepSeek after the OpenAI cut-off and was adequate.

**Where escalation was justified**

- #126 PARITY-08: 5/5 save validation/determinism, real production defect, no schema bump; Claude Opus 5.5 Low was justified.
- #143/#126 save/hash review independence was correctly preserved.

**Missing telemetry**

- No provider-reported remaining quota or reset time for Claude A/B or OpenAI B; all reset times remain **UNKNOWN**.
- No native Unity hang stack (gdb could not attach); only CPU/process/log evidence.
- No single harness inventory mapping every gate to its schema assumptions; this caused repeated rediscovery.

**Recommended routing-policy changes**

- Before any further Unity qualification on v24, run one systematic harness-pin audit and fix all `CurrentVersion == 23` / pre-v24 assumptions in one verification claim.
- Do not assign a task to a provider with a reported low window unless the work is checkpointable in small commits; prefer DeepSeek for salvageable offline work.
- Treat a hung Unity receipt as failure by default and budget one clean rerun per gate.
- Keep verification-only reconciliation claims separate from feature claims; they are low-risk but acceptance-critical.

## 6. Evidence rule

The ledger and this result keep three categories distinct:

- **Observed fact:** provider error text, Git SHAs, Unity gate logs/receipts, offline check output, file diffs, lock/process state.
- **Orchestrator claim:** what DeepSeek believed and why it chose a route, including “no production regression found” and “candidate is ready”.
- **Retrospective inference:** conclusions drawn after outcomes, e.g. “stale v24 pins were the dominant cause of Unity churn”.

No orchestrator claim was promoted to observed fact. A worker’s completion message was not treated as acceptance.

## 7. Mandatory final summary

- **Total delegated attempts:** 12 substantive child attempts (including resumes/reviews).
- **Completed / failed / cancelled:** 8 materially completed (some partial by scope); 1 provider-capacity failure (#151 OpenAI B); 1 Unity NEEDS WORK (#124 initial); 1 Unity partial (#126); 2 verification reconciliations passed (#164/#165); no child was cancelled by the orchestrator.
- **Capacity-related failures:** 1 (OpenAI B subscription limit). Work salvaged on DeepSeek.
- **Retries/salvages:** #151 OpenAI→DeepSeek; #124 Unity→#164 harness reconciliation; #126 Unity→#165 harness reconciliation; multiple clean reruns after intermittent Unity exit hangs.
- **Material routing decisions:** 12 (table above).
- **Decisions backed by direct capacity evidence:** OpenAI B 13% (user), OpenAI B exhaustion (provider), Claude/DeepSeek availability (T3/worker). **Decisions made with inferred/unknown capacity:** all Claude remaining quotas and reset times; OpenAI A token state.
- **Review findings:** #143 APPROVED; #124 APPROVED (offline) + Unity via #164; #151 APPROVED (offline); #126 review still pending; #164/#165 verification repairs PASS.
- **Unresolved tasks:** #126 residuals + #128; #151 Unity; #156 Unity; MFM-09; final Tier-3.
- **Exact next action:** review/integrate #165, disposition #126 residuals, run #128, integrate #126, then #151 and #156 Unity/review/integration, then MFM-09 and final Tier-3.

**Game-development specifics**

- **Unity/editor lanes:** U1 `/media/jer/ZX20/CCF-U1`, strictly serialized. Locks used: #143 stale lock released after completion evidence; #124 `cf2e5588bc7147278f6bab399de4898f`; #126 `80143256eb8b4f22a335d0d3cee2ff77`; #165 `3e26cffb…` (prefix only recorded by its worker). All released normally; U1 restored clean at `b9bf11aa`.
- **Serialized Unity operations:** compile/import, teaching-latch, save/provenance, hash dump, hardening, Reference Future, approval, N3, interaction, progress, completion, completion-model1, density-save. No parallel Unity.
- **Worktrees/branches touched:** orchestration (`orchestration/player-test-overnight-20261030`), integration (`integration/scenario-one-player-test-ready-v1`), #151 (`feature/s1-d-t05-previous-proposed-v1`), #156 (`feature/mfm-08-map-3d-review-ux-v1`), #124 (`feature/s1-d-t04-guide-monotonicity-v2`), #164 (`verify/s1d-v24-harness-reconciliation-v1`), #126 (`verify/parity-08-save-compatibility-v1`), #165 (`verify/s1d-v24-legacy-gate-reconciliation-2`).
- **Rendered/runtime qualification:** #124 T-04 guide 93 checks; #164 TeachingCopy 61, RackLesson 36, teaching-latch 60 ×3, S1-C history 279, T-04 529, T-06 170; #126 save/hardening/reference/latch/approval/N3/interaction; #165 progress/completion/completion-model1/density-save. #151 and #156 rendered gates **not run**.
- **Save/reference/determinism independent review:** #143 reviewed by DeepSeek (APPROVED); #126 pending #128; no hash/reference/anchor rebaseline.
- **Protected anchors changed:** none.

**No Windows build was produced or authorised.**
