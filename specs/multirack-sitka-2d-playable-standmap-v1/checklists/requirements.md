# Multi — Playable 2D Stand Map: delta requirements checklist (DRAFT; ALL NOT RUN)

- **Spec:** [../spec.md](../spec.md) MULTI-STANDMAP-PLAYABLE-2D-V1, **DRAFT**.
- **Relationship:** the approved Stage 5 checklist ([26 R + 24 X](https://github.com/jermur788/CCF/blob/docs/multi-730-837-g2-contract-decision-draft-20261119-v1/specs/multirack-sitka-2d-intervention-workflow-v1/checklists/requirements.md)) remains the **gameplay acceptance authority**, with all 50 unchecked. This file adds **14 PM-R and 16 PM-X** items for the new architecture only. Each item names the Stage 5 ids it supports. No box here can mark a Stage 5 item PASS.
- **Status:** **30 items, all unchecked, NOT RUN.** Phase I ran only the environment proof (base 66/66) recorded in the Phase I note, which is not a test of this delta.

## PM-R — architecture requirements

- [ ] **PM-R01 — owner G5 recorded** before any `src/Ui/**` write; UI-A or UI-B exactly as decided (spec §3). *Supports R-22, R-23, R-25.*
- [ ] **PM-R02 — #730 acceptance recorded** of this delta, including Q-1…Q-6 and the §6 ABI, before any production write. *R-25, X-24.*
- [ ] **PM-R03 — map path B:** no Scenario One file is compiled, linked, copied or edited, and `StandMapSource.cs` is unused. Conventions are used as design reference only, with no visual-match claim. *R-02, R-24.*
- [ ] **PM-R04 — single engine:** the UI and adapter hold no tree, counter, RNG or clock state of their own; every actual value comes from a `PlayerSessionView`. *R-14, R-18, R-20.*
- [ ] **PM-R05 — same-lock view:** frame, token, pending, approved, history and plan-state come from one `WorkRoot` under one `sync` acquisition. *R-03, R-13.*
- [ ] **PM-R06 — fences:** session + full token + plan-state are checked before mutation for Add, Remove, Approve, Cancel, Execute, Advance and the hit-test. *R-13, R-17, R-26.*
- [ ] **PM-R07 — revision rules** exactly as spec §6.3, including the Execute increment (Q-2) and the ABA case. *R-12, R-17.*
- [ ] **PM-R08 — server-minted identities:** PlanId and ExecutionKey are minted by the engine side; callers cannot supply a plan object or PlanId. *R-12, R-15.*
- [ ] **PM-R09 — A-2 replay rule** (spec §6.5): same key + PlanId + original source + session gives `Replayed`; any mismatch refuses; zero mutation. *R-15.*
- [ ] **PM-R10 — exact selection identity** S-1…S-8; no nearest-centre or seed-derived identity. *R-05, R-09, X-04.*
- [ ] **PM-R11 — truthful labels** from `PlayerLabels` and the view: synthetic experiment, model/fixture/year/revisions, G1 DISABLED — Manual/NotCovered, money UNAVAILABLE (never €0), session-only. *R-02, R-07, R-21, R-23.*
- [ ] **PM-R12 — no forbidden elements:** no rack, corridor, entrance, marking, proposal, sample, equivalent-area, economy, save or persistence code, control or text. *R-01, R-07, R-21, R-23, X-03, X-23.*
- [ ] **PM-R13 — protected scope:** the path list in tasks.md only. Fixture, goldens, P0 pin, `AnnualRules.cs`, `WorkContract.cs`, `AnnualEngine.cs`, CLI, `.sln`, Core and test csproj, `Tools/MultiRackSitkaLab/**` and `Assets/**` unchanged. *R-24, X-22.*
- [ ] **PM-R14 — accessibility** A-1…A-7 (spec §8); screen reader reported RUN or NOT RUN truthfully. *R-22, X-21.*

## PM-X — tests (positive and negative)

- [ ] **PM-X01 — base preserved:** on the pushed candidate in a pristine LF clone, the 66 base tests pass unchanged and the ten digests are character-identical. **Negative:** any base edit or digest drift is FAIL, never rebaselined. *X-17, X-22.*
- [ ] **PM-X02 — atomic view** under concurrent Execute and Advance (PS-01). **Negative:** a torn view (frame and token work revisions differ) is FAIL. *X-13.*
- [ ] **PM-X03 — plan-state rules** (PS-02, PS-03). **Negative:** a missing or double increment, or a stale ABA fence accepted. *X-11, X-15.*
- [ ] **PM-X04 — fence refusals** for all commands (PS-04, PS-10, PS-11, MS-06), each with zero mutation. *X-02, X-15.*
- [ ] **PM-X05 — no Fell before Execute** (PS-05): Add, Remove, Approve and Cancel make zero Fell and leave the token unchanged. *X-08, X-11.*
- [ ] **PM-X06 — one real Fell** (PS-06): exact IDs, typed cause, work revision +1, year and annual revision unchanged, count/BA from the source. *X-12, X-19.*
- [ ] **PM-X07 — replay** (PS-07, PS-08, PS-09). **Negative:** a PlanId or session mismatch is accepted, or a second Fell occurs. *X-14.*
- [ ] **PM-X08 — all-live and fault** (PS-12, PS-16): whole-batch refusal or rollback with zero mutation. *X-13, X-19.*
- [ ] **PM-X09 — consent and Year+1** (PS-13, PS-14, PS-15): deny → unchanged; consent → zero-Fell cancel plus one genuine year; survivors equal `AnnualRules.Calculate(post-Fell)`; second cycle works. *X-15, X-16, X-18.*
- [ ] **PM-X10 — immutability and identity** (PS-17, PS-18): read-only collections, 100 exact IDs, one scope. *X-01.*
- [ ] **PM-X11 — no unsupported API** (PS-19): no proposal, comparator, sample, seam or plan-object member. *X-07 / X-09 (N/A — G1 DISABLED evidence), X-23.*
- [ ] **PM-X12 — scene geometry and order** (MS-01, MS-02, MS-05). *X-03, X-04.*
- [ ] **PM-X13 — hit-test identity** (MS-03, MS-04), plus the in-browser S-5 cross-check witnessed. **Negative:** bare ground selects nothing; a stale fence selects nothing. *X-04.*
- [ ] **PM-X14 — UI process envelope** (`ui-smoke.sh`, UI-A): 127.0.0.1-only, port ≠ 5187, capability/Host/Origin/type/size guards, full API loop, no files written. *X-23.*
- [ ] **PM-X15 — human-operable run** at 1280 px, 360 px and 200 %, keyboard-only, with real screenshots or recordings and provenance (plan P3). **Negative:** a refusal is announced, an optimistic Fell never appears, "outcome unconfirmed" offers only a re-read. *X-03, X-21, R-26.*
- [ ] **PM-X16 — independent Tier-2** by a non-author (GPT-6.1 Sol), fresh clone, exact SHA, verdict recorded. *X-24, AC-12 of Stage 5.*

## Documentary verdict

- Phase I: documentation and environment proof only. **No production source, no UI, no runtime of this delta.**
- Owner G5: **PENDING**. #730 acceptance: **PENDING**. All 30 PM items and all 50 Stage 5 items: **unchecked / NOT RUN**.
