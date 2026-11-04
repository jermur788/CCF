# Post-overnight continuation — DeepSeek DPC return to the Overall Manager

**Date:** 2026-11-04. **Parent:** [POST-OVERNIGHT-REVIEW-AND-ROUTING-PACKET.md](POST-OVERNIGHT-REVIEW-AND-ROUTING-PACKET.md).
**Claims:** [#269](https://github.com/jermur788/CCF/issues/269), [#270](https://github.com/jermur788/CCF/issues/270), [#271](https://github.com/jermur788/CCF/issues/271).
**Ledger:** [ROUTING-LEDGER.md](ROUTING-LEDGER.md) EVENTS 031–042 on this branch.
**Overall status: ALL THREE CLAIMS EXECUTED, REVIEWED BY THE OPPOSITE MODEL FAMILY, AND HANDED OFF. NOTHING IS INTEGRATED, ACCEPTED, UNITY-VERIFIED OR BUILT.**

## 1. Executive summary

| Claim | Delivered | Independent review | State |
|---|---|---|---|
| **#269** isolate reviewed UI deltas | [PR #273](https://github.com/jermur788/CCF/pull/273) — 13 whitelisted files + handoff, **base = the playtest candidate** | OpenAI GPT-6.1 — **APPROVE-WITH-CHANGES** | **STAGED / REVIEWED (source)** |
| **#270** final review of three DRAFT UX specs | [FINAL-DRAFT-SPEC-REVIEW-HANDOFF.md](FINAL-DRAFT-SPEC-REVIEW-HANDOFF.md) — 3 verdicts, 19 Manager decisions | Each spec reviewed by the opposite family of its author | **REVIEWED, all remain DRAFT** |
| **#271** MFM A vs constrained C | [PR #274](https://github.com/jermur788/CCF/pull/274) — matched offline study | OpenAI GPT-6.1 — **APPROVE-WITH-CHANGES** | **RESEARCH, NO-COMPARISON** |

`main` was **not** modified by any claim. The published candidate `integration/scenario-one-player-test-ready-v1` stayed at **`72b4efbd`** and was not moved. PRs **#261/#262/#264 were never merged, rebased or retargeted**. No Unity, no Editor, no Library import, no Player or Windows build was run.

## 2. What actually happened, with the numbers

### #269 — staging result

Branch `integration/scenario-one-overnight-ui-qualification-v1`, head **`d3ba4fce`**, PR **#273** based on `72b4efbd`.

- Diff vs candidate = **exactly the 13 whitelisted paths + the staging handoff**; 3 files from #261, 6 from #262, 4 from #264.
- Each of the 13 is **byte-identical** to its source head (verified independently by the reviewer, file by file).
- `run_offline.py` → `RACK_V2_HELP_TEACHING_OFFLINE_PASS checks=97`, then `FAIL failed=2`.
- `check_baseline.py --rotation-state post-fix --instruction-state post-fix` → `PASS=9 FAIL=1 NOT-TESTABLE=2`.
- The single FAIL is **check 08**: the Objective 1 primary button still calls `ui.CloseAll()`. **This is the user's unfixed complaint and it was deliberately left failing.**
- The two `run_offline.py` failures (S10, S11) are **branch-scope guards** against historical base `6d3228c`; the named files belong to candidate history, not to this delta. They were not weakened, skipped or relabelled.
- Stager, DPC and reviewer independently produced the **same three tallies**.

### #270 — spec review result

| Spec | Author family | Reviewer | Verdict |
|---|---|---|---|
| #257 input settings | Claude | GPT-6.1 | APPROVE-WITH-CHANGES |
| #258 next-action guidance | Codex | Claude Sonnet 5.5 | NEEDS CHANGES (3 High) |
| #259 tree-mark language | Codex | Claude Sonnet 5.5 | APPROVE-WITH-CHANGES |

**The dominant finding across all three is the same:** each spec promises behaviour the shipped UI does not have — non-focusable buttons, no screen-reader mechanism, colour sampling that can hide a stroke, and a recognition test a scorer can memorise. That, not any individual wording, is what the Manager should act on.

### #271 — MFM comparison result

Seven new files, PR **#274**, legacy baseline preserved, **NO-COMPARISON** verdict.

- Matched caps 20/15/12/9 m and both ownership rules applied identically to A, three C constructions and the legacy layout; the reviewer found **no option-specific policy exception**.
- Uncovered ground: legacy 0/0/0/3.11%; **A lateral 36.25→49.03%**, A nearest 14.13→38.22%; C-S45 17.85–19.05% / 3.06–14.77%; C-H45 41.13–42.21% / 21.53–37.12%; C-J45 56.31–65.17% / 11.99–40.80%.
- **The ordering is cap-dependent.** A is better at 20/15 m; C-H45 is better at 12/9 m (41.29% vs 44.77%, and 42.21% vs 49.03%). There is no global winner in the numbers.
- **The product-relevant finding:** #251's "C cannot hold 45°" depends on a **16 m gap that is D-068 Manager guidance, not an enforced rule**. On this spine, 45° fits four racks at ≥16 m, or five at a max equal gap of **13.70 m** (13.698 m on the z-only straight-spine approximation, **13.7475 m** with actual endpoints).
- Three C constructions exist and each fails differently: C-S45 needs a Rack V2 behaviour change (no legal headland entrance for two racks); C-H45 is legal but one direction family and drops the spine; C-J45 is illegal on the 2.4 m clearance (centrelines 1.414 m apart).
- Reproducible: two runs byte-identical; a separate rederivation script that imports neither study script reproduces the headline geometry.

## 3. Routing record (what the user asked to be logged)

- **Providers/models actually used:** Claude Sonnet 5.5 Medium (**#269** staging, `claudeAgent_claude_b_pro`; **#271** study and **#258** review, `claudeAgent`), OpenAI GPT-6.1 Medium (`gpt-6.1-sol`; **#257** and **#259** reviews, **#269** and **#271** reviews).
- **Escalations:** **none.** Medium was sufficient for every lane; no Opus 5.5, no high/xhigh effort. #271 explicitly reported medium was enough for a defensible matched comparison.
- **Retries:** one — both writer children failed at app-managed sandbox provisioning before any task work; relaunched on the packet's pre-created lanes with no repository change (EVENT 033).
- **Limits/quota/reset:** **UNKNOWN throughout.** No rate-limit event, no cooldown, no invented reset schedule.
- **Errors corrected in public, not hidden:** (a) I routed #259 to a same-family reviewer — the reviewer caught it and a Claude review was dispatched (EVENT 035); (b) I published an over-strong "78.10 should be 78.077696" correction — it is a matrix constant-set difference, now recorded with a ±0.05 tolerance (EVENT 039); (c) I described the snap script as `/tmp`-only — it is actually committed inside the handoff; only re-run artefacts were ephemeral (EVENT 038); (d) I repeated "C-H45 is not better than A" — false at 12/9 m (EVENT 041).
- That is four self-corrections in one session, three of them found by the independence gate working as designed.

## 4. What the Manager must decide

**Spec product decisions (19, D-01…D-19)** are consolidated in [FINAL-DRAFT-SPEC-REVIEW-HANDOFF.md](FINAL-DRAFT-SPEC-REVIEW-HANDOFF.md) §5. The highest-consequence ones:

1. **D-07** whether the forest UI becomes keyboard-focusable at all (today it deliberately is not).
2. **D-08** whether destructive consent buttons stop being the primary-styled choice.
3. **D-14** pink rack **or** move the Fell colour — "orange-first with pink fallback" is nominal because orange-vs-orange fails the floor by construction.
4. **D-15** whether a mark edge/backing is mandatory (without it, 3:1 is unreachable on mid-light bark).
5. **D-01** the tester isolation model — per-device storage isolates nothing if testers share an OS account.

**MFM decision:** whether the 45° full-spine target is a **requirement** (then a fallback C must be defined and qualified) or a **goal to be scoped** (then the target changes, not the geometry).

**Hygiene decision:** whether the two staged handoffs (4,884 lines / 450,266 bytes and 3,889 lines / 322,835 bytes with copied transcripts) remain permanent documentation or are compacted with provenance links. DPC did not touch them.

## 5. Exact Git state left behind

| Ref | Head | Purpose | Merged? |
|---|---|---|---|
| `docs/v2-routing-continuation-20261104` | see PR #272 | ledger, review handoffs, evidence | **No — PR #272** |
| `integration/scenario-one-overnight-ui-qualification-v1` | `d3ba4fce` | staged UI delta | **No — PR #273** (base = candidate) |
| `docs/mfm-a-vs-c-offline-20261104-v1` | `4d0abd1b` | MFM study | **No — PR #274** |
| `integration/scenario-one-player-test-ready-v1` | `72b4efbd` | user playtest candidate | unchanged |
| `main` | `db43dbe5` | — | unchanged by these claims |
| #261 / #262 / #264 | `9304d318` / `693e24c8` / `6efba8fd` | original wrong-base PRs | **open, untouched, not merged** |

Worktrees `/media/jer/Files/CCF/Operations/CCF-Overnight-UIQualification-20261104` and `CCF-MFM-A-vs-C-Docs-20261104` remain in place for follow-up; both are source-only, no Library. Disk was 103 GiB free (Files) / 54 GiB (ZX20) at the last check.

## 6. What is still NOT proven

- No Unity compile, import, render, text fit, input dispatch, physical rack drag, save/load or Player build.
- No user acceptance, and the personal playtest remains incomplete.
- No production C# equivalence for any MFM number; Python port only.
- No accessibility evidence of any kind: no screen-reader run, no colour-vision participants, no perceptual study.
- No decision-grade MFM comparison (no fixed fallback target, no complete masks, no competition-inclusive coverage).
- Method B inventory provenance (73/61) is still owed by its originator.

## 7. Recommended next safe actions

1. Manager merges **#272** (docs only) to make the ledger and handoffs canonical.
2. Manager triages **#273** as a *source-staging* artifact only; it must not enter the playtest candidate without a separate Unity qualification packet with fresh lock/disk/process preflight (15 GiB floor; the user's warm `CCF-U1` Library must not be borrowed).
3. Authors revise #257/#258/#259 against the findings; DPC does not edit specs.
4. Manager answers the MFM 45° scope question before any Option A spec work resumes.
5. No Windows build until the user completes the playtest and explicitly authorises it.

**Nothing in this return authorises implementation, integration or a build.**
