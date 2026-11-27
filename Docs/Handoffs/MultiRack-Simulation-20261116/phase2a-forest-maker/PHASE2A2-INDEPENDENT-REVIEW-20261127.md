# CCF Multi 2D — Phase 2A-2 Forest Maker UI, Multi-Lane View, Lane-Filtered Reads & Save Schema v3 — Independent Review

- **Date:** 2026-11-27
- **Reviewer:** Claude Opus 5.5 (independent, read-only; did not write any part of the change)
- **Review branch:** `review/multi-2d-phase2a2-forest-maker-ui-v3-20261127-v1` (this file is the only change on it)
- **Integrated candidate reviewed:** `origin/integration/2d-sim` @ `afcc2ba089543face9bbffa0d20f9184d1667e99` (merge; first parent `e8e8c8c7…`, second parent feature `2f3ab616…`)
- **Feature:** `feat/multi-2d-phase2a2-forest-maker-ui-v3-20261127-v1` @ `2f3ab616` (code head `9100c0b9`), base `fa69d6d3`
- **Protected `origin/main`:** observed `bdeb6a324acaf4129c10fb0846faac8b64974360` (matches the required value)
- **Contracts:** `specs/multi-2d-phase2a-forest-maker-v1/{spec,plan,tasks}.md` (Step 2A-2, P2A-07..P2A-10, P2A-REG, §11); `specs/multi-2d-s0-seam-extraction-v1/frozen-contracts-s0b.md` (C-V3, C-FENCE, §12 checklist)
- **Review mode:** Owner Proportionate Verification override of 2026-11-27 (no `selftest.sh`, no legacy browser sweep / Stage B suites)

---

## 0. Order of events

The combine onto `integration/2d-sim` was pushed under the owner's Proportionate Verification override **before** this review was done. That order does not change any finding below. Every finding is about the code and evidence, and all of it applies to the integrated tip. It does change what follows from the findings. Nothing can be held back from integration any more, so each condition below needs a follow-up commit on `integration/2d-sim` before the Playable Milestone 1 closeout. The merge tree differs from the feature head only by `Docs/…/2D_Simulator_Strategic_Roadmap.md` (+68 lines, from trunk `e8e8c8c7`). I checked this with `git diff --stat 2f3ab616 afcc2ba0`, so the review applies to the feature code without change.

---

## 1. Method

1. I made a clean worktree at `afcc2ba0`: `/home/jer/Development/CCF-HP-Worktrees/CCF-Multi-2D-Phase2A2-Review-20261127`. Before reading any file I set `core.autocrlf false` and `core.eol lf`, then ran `git checkout-index -f -a`. `git status` was clean.
2. I checked the scope and the fences with `git diff --name-status`/`--name-only` from the base `fa69d6d3` to the feature `2f3ab616`.
3. I read the full source of: `src/Core/Saves/{SaveV3Codec,SaveV3Format,FeatureBlockRegistry,GenesisRecipeBlockCodec}.cs`, the Core diff under `src/Core/MultiArea/**`, `src/Ui/{MultiSessionHost,MultiViewBudget,MultiApi.Saves,MultiApi.Woodland,MultiApi.Stand,MultiApi.Commands}.cs`, the relevant parts of `MultiApi.View.cs`, `MultiApi.History.cs`, `MultiRoutes.cs` and `PlayerApi.cs`, the browser modules `multi/api.js` (diff), `multi/dialogs/woodland.js`, `multi/dialogs/saves.js` (diff) and `multi/map/minimap.js` (accessibility parts), the tests `tests/Core.Tests/Phase2A2Tests.cs` and `tests/phase2a-ui/drive-phase2a.mjs` (overflow/economy parts), and gate commit `81b2c6db` in full.
4. I cross-checked every number in the handoff against the evidence root `/home/jer/Development/CCF-Multi-2D-Phase2A2-Evidence-20261127`, read-only.
5. I ran **no** builds, tests or browser suites. See §2 for why.

## 2. Exact commands run

All were run through `wsl.exe bash -lc` and were read-only, except the worktree creation and this report's commit.

```
git -C /home/jer/Development/CCF-HP fetch origin
git rev-parse origin/main origin/integration/2d-sim origin/feat/multi-2d-phase2a2-forest-maker-ui-v3-20261127-v1
git -C /home/jer/Development/CCF-HP worktree add -b review/multi-2d-phase2a2-forest-maker-ui-v3-20261127-v1 <review-wt> afcc2ba089543face9bbffa0d20f9184d1667e99
git config core.autocrlf false; git config core.eol lf; git checkout-index -f -a
git log --format='%H %P %s' -1 afcc2ba0; git log --oneline e8e8c8c7..afcc2ba0; git merge-base 2f3ab616 fa69d6d3
git diff --stat e8e8c8c7 afcc2ba0; git diff --stat 2f3ab616 afcc2ba0
git diff --name-only fa69d6d3 2f3ab616 -- <each fenced path>; git diff --name-status fa69d6d3 2f3ab616
git diff fa69d6d3 9100c0b9 -- src/Core/MultiArea src/Ui/... build/2d-gate/...; git show 81b2c6db
grep/sed over src/Ui/*.cs, src/Ui/multi/**, tests/**, specs/**
cat/grep over evidence: phase2a-edge-final/run.log, phase2a-edge-final-vp1280/run.log, gate-final/{gate-summary.json,gate.log,g2-*-tests.log,g3-anchors.json},
  combine-fast/{edge-merge,edge-merge-2}/run.log, combine-fast/*.log (+ mtimes), m0-post/results.jsonl, selftest-final.console.log
```

**Not run, and why.** The override allows clean builds, Core.Tests, Phase1.Save.Tests and at most one targeted Edge run. It also says not to re-run evidence that already passed on the same tree unless there is a specific doubt. `combine-fast/` holds a clean build (0 warnings / 0 errors for `.sln` and Ui), `312/312` Core and `50/50` Save on the merged tree, and the code head has the same results in `gate-final/`. I had no specific doubt about those results. My one substantive finding (Major-1) is in routes the Phase 2A-2 Edge suite does not exercise, so re-running that suite would not have tested it. Major-1 rests on code reading, not on a run. No processes were started, and no protected listener was bound or probed.

---

## 3. Observed results (evidence cross-check)

| Claim (handoff) | Observed | Match |
|---|---|---|
| Clean builds 0/0 | `combine-fast/build-{sln,ui}.log`: `0 Warning(s)`, `0 Error(s)`; gate-final G1 PASS | ✅ |
| Core 312/312 | `gate-final/g2-core-tests.log` and `combine-fast/core-tests.log`: `312/312 passed, 0 failed` | ✅ |
| Save 50/50 | both logs: `50/50 passed, 0 failed` | ✅ |
| Full gate PASS on `9100c0b9` | `gate-summary.json`: `headCommit 9100c0b9…`, `verdict PASS`, G0–G7 + B PASS | ✅ |
| G3 9/9 | `gate.log:27` `G3: 9/9 frozen SHA-256 anchors verified` | ✅ |
| G4 line | `gate.log:29` matches the handoff text exactly | ✅ |
| G5 5/5, G7 158 paths | `gate.log:31,35` | ✅ |
| Edge 1920: 36 passed | `phase2a-edge-final/run.log:52` `OVERALL: PASS (36 passed, 0 failed)` = 1 static + 20 full@1920 + 4 restart@**1280** + 5 overflow@**1280** + 6 Z-checks | ✅ count; ⚠ viewport wording (Minor-6) |
| Edge 1280 full: 20 passed | `phase2a-edge-final-vp1280/run.log:24` | ✅ |
| Restart "PASS at both viewports" | restart ran **only** at 1280×720 (`run.log:31–37`); the vp1280 run has only the `full` mode | ❌ (Minor-6) |
| Stand bytes "Edge, 1280×720" | `run.log` line is tagged `[1920x1080] vp1920` | ⚠ (Minor-6) |
| M0 table | `m0-post/results.jsonl`: 4 rows that exactly match every byte figure in §2.4 | ✅ |
| Y15 3-lane headroom ~15.8 KB | 524,288 − 508,505 = 15,783 B | ✅ |
| Forced overflow 503 (`bytes=1084530, budget=400000`, 171→171) | `run.log:42–45` | ✅ (only for the routes tested; see Major-1) |
| Selftest stopped at C26/29 | `selftest-final.console.log` ends during the full cases with no per-case verdicts; the stop point cannot be confirmed from the log | ⚠ disclosed |
| Combine-time targeted Edge run | `combine-fast/edge-merge/run.log`: **`OVERALL: FAIL (9 passed, 2 failed)`** (page TypeError, restart timeout, overflow body undefined). `build-ui.log` mtime 12:22:51 is **after** that run (12:20:07), and the rerun `edge-merge-2` (12:23:27) is `PASS (36/0)` | ⚠ undisclosed (Minor-6). Most likely cause is a stale `SitkaP1.Ui.dll` (Ui csproj is not in the `.sln`), not a code defect |

---

## 4. Per-area verdicts

### Q1 — C-V3 conformance: **PASS** (one undisclosed spec reconciliation, Minor-4)

- **Envelope:** the exact top-level keys `format, schemaVersion, header, payload, blocks, checksum` are checked for both set and order (`SaveV3Codec.cs:19–27,114–120`), and so is the checksum key order (`:122–131`).
- **Blocks:** keys must be strictly ordinal-sorted (`:148–155`). Each block must have exactly `[blockVersion, data]` in order (`:165–169`), and `blockVersion` must be a positive 32-bit integer (`:171–174`).
- **Fail-closed:** an unknown key gives `SaveBlockUnknown`, a reserved key gives its slot refusal, and an unreadable version gives `SaveBlockVersionUnsupported` (`FeatureBlockRegistry.cs:73–87`). A missing `genesisRecipe` on a forest-maker header (`SaveV3Codec.cs:207–212`), `economyEnabled` set with no economy block (`:214–218`), and caller-required blocks (`:220–228`) are all refused. Nothing is defaulted.
- **Checksum and canonical form:** the whole file is re-encoded canonically and compared byte for byte (`:194–199`), and the checksum covers every byte before the checksum object, blocks included (`:201–204`, `:481–488`). The genesisRecipe data is re-encoded canonically as well (`GenesisRecipeBlockCodec.cs:107–110`). Header and payload also pass every frozen v2 gate through `Phase1SaveCodec.Decode` on a rebuilt inner envelope (`SaveV3Codec.cs:137–142`).
- **Size cap:** 64 MiB, the same as v2 (`SaveV3Format.cs:32`), enforced on encode (`SaveV3Codec.cs:81–83`), on decode (`:100–102`) and on slot read (`SaveV3Slots.cs:223`).
- **genesisRecipe payload:** `{recipe, recipeHash, generatorNote, economyEnabled}`, verified on decode for hash and note.
- **Economy key:** reserved only; no codec exists (`FeatureBlockRegistry.cs:28–31`). Encoding refuses with `SaveEconomyRequiresV3` (`SaveV3Codec.cs:48–55`, `GenesisRecipeBlockCodec.cs:31–33`).
- **v2 isolation:** zero files changed under `src/Core/Phase1/**`. v2 decoding is delegated unchanged (`SaveV3Codec.cs:312–316,323–325`), so v2 messages are unchanged by construction. The G4 line confirms the golden save is still 24,703 B and `phase1V3Refs=0`.
- **Spec reconciliation:** Phase 2A spec §6.1–6.2 and `plan.md:54–55` prescribe putting `recipe` inside the v2 header and editing `Phase1SaveCodec`. The frozen C-V3 prescribes a top-level `blocks` envelope and forbids any Phase1 edit. The implementation followed C-V3, which I judge correct, but the handoff does not record the deviation (Minor-4).

### Q2 — Self-contained v3 load and atomic host replacement: **PASS**

- **Load order:** v2 gates and block decode (`SaveV3Codec.Decode`) → economy and model vector `1/1/1/1/1` and lineage → recipe validity → recipe hash → regenerate and compare experimentId, content hash, generator id/version/seed, areas, seeds and hectare bits (`SaveV3Codec.cs:238–295`) → a fresh `MultiAreaContinuation`, `RequireSameOptions`, restore, block decode, approval converted to a Pending draft, `RequireLegalRestoredOrders`, new session id, caches cleared (`MultiAreaContinuation.cs:612–649`).
- **Order vs spec:** steps 3 and 4 run in the opposite order to spec §6.3 (model/lineage is checked before regeneration). This affects only which refusal code wins when several checks fail. It does not affect safety.
- **Host swap:** the candidate is fully built before `SwapLocked`, all under `multiHost.SyncLock` (`MultiApi.Saves.cs:164–205`). A refusal leaves `current` untouched. Every mutating route takes the same host lock (`MultiApi.Commands.cs:21,66,111,169`; `MultiApi.History.cs:60,223`; `MultiApi.View.cs:478,505,542`; `MultiApi.Woodland.cs:141` via `AtomicReplace`).
- **Old fences:** the new session id means old fences are refused, and the Edge suite asserts `PlayerSessionMismatch` for a pre-swap fence.
- **No singleton, service locator or DI:** `MultiSessionHost` is an ordinary instance field of `PlayerApi` (`PlayerApi.cs:74`).
- **Gap:** omitting the confirmation flag skips the unsaved-progress guard (Minor-1).

### Q3 — C-FENCE lane-filtered reads: **PASS for the stand routes; FAIL for the "never over-budget 200" guarantee across all view-returning routes (Major-1)**

- **`/api/multi/stand`:** one snapshot under one host lock (`MultiApi.Stand.cs:186–213`). The fence check happens inside that same critical section, and the fence object returned is `FenceOf(snap.View)`, the same `MultiFenceMembers` shape. A stale fence gives 409 with the existing codes (`:198–212`), an unknown lane gives 409 `MultiAreaUnknownArea` (`:221–230`), and an over-budget body gives 503 (`:161–162,242–243`).
- **Lock note:** the Core session lock is taken more than once per response (snapshot plus history index), but always inside the single host lock that every mutator also holds, so the frame is consistent.
- **Client composition:** `fetchMultiStandCFence` compares fences byte for byte and retries, but **nothing in the shipped UI calls it** (Minor-3). The live lane view renders from the full `/api/multi/view` under the 2.5 MiB scaled budget.
- **Major-1:** six view-returning routes still serialize the Tier-3 over-budget body and send it with HTTP 200 or 409.

### Q4 — UI contract: **PASS (on the writer's Edge evidence plus source reading)**

- **Modal:** a native `<dialog>` opened with `showModal()`, which makes the background inert, handles Esc and gives the implicit dialog role. It is labelled with `aria-labelledby`/`aria-describedby` (`multi.html:921`), and focus returns to the opener (`commands.js:329–335`). The Edge suite asserts Esc closes the dialog and focus returns to `#woodland-open`.
- **Preview:** shows only values the server sent (`woodland.js:89–101`). The bounds are display-only `min`/`max` attributes from Core (`:172–197`).
- **No client forestry math:** the source grep finds no numeric forestry constants in `woodland.js` or `minimap.js`. `expandCompactLanePayload` (`api.js`) does coordinate un-packing for presentation only.
- **Unsaved guard (woodland/new):** `confirmDiscardUnsaved` is a required member and the guard is enforced in `AtomicReplace`.
- **Money On/Off:** works at Year 0. On is not persistable (see Minor-6).
- **Lane views:** 2/3-lane views, mini-map click and keyboard (`role=button`, `tabindex=0`, Enter/Space at `minimap.js:65–67,111–112`), the 1-lane toggle, the two-area layout (`multi=false`, "Both areas") and the ≥3-area-only lane view are all asserted in the Edge logs at 1920 and 1280.

### Q5 — G4 gate-lock retirement (`81b2c6db`): **Legitimate retirement, not a weakening. Coverage is unproven (Minor-5).**

- **Dropped:** only "zero `IFeatureBlockCodec` implementations while `SchemaVersion == 2`". That was an explicitly temporal S0b lock, and S0b §4 item 4 says to register a codec "once Phase 2A implements C-V3". The rule is kept behind `saveV3.enabled=false`.
- **Kept or tightened:**
  - "No `SchemaVersion = 3` anywhere in `src/`" is still enforced whenever v3 is on. The regex `\bSchemaVersion\s*=\s*3\b` does not match `SchemaVersionV3 = 3`.
  - The v2 golden checks (PASS line, bytes, file SHA, embedded checksum, `SchemaVersion = 2`) are untouched.
  - `src/Core/Phase1/**` is still compared against both the base and T0.
- **Added:** codecs only under `src/Core/Saves/`; every codec registered in `CreateDefault()`; `economy` constant present, reserved and with no codec matched by key expression or class name; exactly one `genesisRecipe` codec with version ≥ 1; zero v3 symbols under `src/Core/Phase1/**`.
- **Residual weakness:** the checks are regex-based, `selftest.sh` gained no mutation case for any new assertion, and selftest was not completed (Minor-5).

### Q6 — Scope and fences: **PASS**

Zero diff under `Assets/`, `Packages/`, `ProjectSettings/`, `Docs/Project/`, `src/Ui/wwwroot/`, `src/Core/SpeciesWave2/`, `src/Core/WoodlandImport/`, `src/Core/NaturalProcesses/`, `src/Core/Phase1/`, `specs/**` and `.p0ref`. No `[x]` checklist ticks were added.

Changes to Core biology and rules: none. `MultiAreaRoot` gains a nullable `GenesisRecipe` as its **last** positional member, satisfying the §12 checklist item 1. `MultiAreaExperiment` gains a `BadgeOverride` init property that is never part of the manifest. Continuation changes are additive. G3 still verifies 9/9 anchors.

Edits under `MultiArea/**` touch `MultiAreaContinuation.cs`, which the 2A-1 plan listed as untouched. The 2A-2 spec §7.3 explicitly allows the `BadgeOverride` and host changes, and the edits are additive.

### Q7 — Handoff honesty: **Mostly accurate. Several overclaims and omissions (Minor-6, Minor-3).**

These disclosures are accurate:
- the skipped and incomplete selftest, including the admission that it would normally be required because a gate script changed;
- the 15.8 KB headroom;
- the economy-save refusal;
- the 2A-1 minors;
- that anchors 10–11 are test-pinned only.

Overclaims and omissions are listed in Minor-6 and Minor-3.

### Q8 — Residual risk: see §7.

---

## 5. Findings

### Critical
None.

### Major

**Major-1. The "never an over-budget body" guarantee holds only on some routes.**
Files: `MultiApi.History.cs:79–86,103–107`; `MultiApi.View.cs:487–492,517–522,554–559`; `MultiApi.Commands.cs:153–159`.

Spec §8 and P2A-10 require that the server **never** returns a view body above the active budget, returning 503 `MultiViewPayloadBudgetExceeded` instead. `SerializeBoundedMultiView` still produces a Tier-3 over-budget object (`MultiApi.View.cs:276–285`). Five handlers send it with HTTP 200 or 409 and only `payloadBudgetExceeded:true` set (stderr also logs it):

- `api/multi/skip` / advance-years (`SkipMultiYearsAsync`)
- `api/multi/cell`
- `api/multi/competition`
- `api/multi/hit`
- `api/multi/economy/session` (Money switch)

These handlers call `BoundSnapshotForResponse` (`MultiApi.View.cs:411–420`), which throws the raw bytes away, and never check the flag. This lane edited every one of those call sites to pass the new scaled budget, but did not add the 503 branch. Only `/view`, `/stand`, `load`, `woodland/new` and the generic `MultiCommandAsync` refuse correctly, and the forced-overflow test (`drive-phase2a.mjs:379–405`) covers only `woodland/new` and `GET /view`.

The handoff's "The silent-overflow fallback is gone: over-budget now returns typed HTTP 503" (§2.4) is therefore true only in part. The fallback is no longer silent, but it can still be over budget.

*Trigger:* not reachable with the two presets (the largest view is about 1.69 MB against a 2.5 MiB budget). It is plausible on a custom woodland near the 3,000-stem cap at later years, or under the test budget override.

*Fix:* in each handler, branch on `bounded.PayloadBudgetExceeded` and return 503, as `MultiCommandAsync` does, and extend the overflow test to cover at least `skip` and `hit`.

### Minor

**Minor-1. The v3 load's unsaved-progress guard is opt-in.**
Files: `MultiApi.Saves.cs:194`, `MultiApi.Saves.cs:257`, `MultiRoutes.cs:51`.

`ConfirmDiscardUnsaved` is `bool?`, the check is `== false`, and `confirmDiscardUnsaved` is not a required member of `MultiLoadMembers`. A load request that omits the field therefore discards unsaved progress with no refusal. Spec §7.2 says v3 load goes "behind the same unsaved-progress confirmation", and `woodland/new` requires the member. The shipped UI always sends `true` (`saves.js:179`) and relies on the pre-existing load confirmation, so the server-side guard is never exercised by the real client.

*Fix:* treat a missing value as unconfirmed (`!= true`) and add a test.

**Minor-2. A 503 can be sent after a committed mutation.**
Files: `MultiApi.Woodland.cs:141–183`, `MultiApi.Saves.cs:196–224`, `MultiApi.Commands.cs:21–41`.

The swap or command commits under the lock, and the budget check happens afterwards. The forced-overflow test shows this: `woodland/new` returns 503, yet `GET /view` then also returns 503 because the server now holds the 5-lane stand (`drive-phase2a.mjs:386–398`). The page keeps the previous stand, whose fence is now stale. The next action is refused, and the user recovers with the re-read button. State is not corrupted, but the response looks like a refusal while describing a committed change.

*Fix:* either say in the banner that the change committed, or check the budget for a replacement candidate before swapping.

**Minor-3. The C-FENCE client composer is dead code in the shipped UI.**
File: `multi/api.js:107`.

`fetchMultiStandCFence` is never imported or called. The UI only calls `syncStandOverview` (`api.js:87`) for the mini-map, from `commands.js:499`, `saves.js:188` and `woodland.js:244`. The active-lane-plus-neighbours view renders from the full `/api/multi/view` body, about 1.59–1.69 MB per response for the presets (`m0-post/results.jsonl`), under the 2.5 MiB scaled budget. The server-side lane read is correct, but only the tests use it.

Two handoff statements overstate this:
- "Browser composes a frame only from byte-identical fences."
- "the lane-filtered read is what brings it under [512 KiB]" (§2.4).

This is not a contract violation, because the full view respects the scaled budget. But the 3-lane/512 KiB figures describe an API the player's page does not use.

**Minor-4. The Phase 2A spec deviation is not recorded.**
Files: `spec.md:134–142`, `plan.md:54–55` versus the implementation.

The spec and plan put `recipe` in the v3 **header**, add `SchemaVersionV3` to `Phase1SaveFormat`, and make `Phase1SaveCodec` accept 2 or 3. The implementation follows the frozen C-V3 instead: a top-level `blocks.genesisRecipe` and zero Phase1 edits. That is the safer reading, and it satisfies stop condition §11-4. The recipe-swap protection the spec wanted ("checksum keeps covering header‖payload") is kept, because the outer v3 checksum covers the blocks.

The handoff calls the format "frozen C-V3 envelope" but never says it deviates from §6.1–6.2 and P2A-07's "§6.2 header". The owner should record the reconciliation in the decision log or a spec erratum.

**Minor-5. The new G4 assertions have no selftest coverage.**
Files: `build/2d-gate.sh` (G4 Python block); `selftest.sh` unchanged.

The retirement is legitimate (Q5). However, no selftest mutation proves the new positive assertions fail closed: a codec outside `Saves/`, an unregistered codec, an economy codec, a v3 symbol inside Phase1. Detection is regex-based, so for example a codec whose `BlockKey` returns another constant expression would get past the reserved-key check unless its class name contains "economy". Combined with the incomplete selftest run, the gate change is unproven.

*Fix:* add four mutation cases to `selftest.sh` and complete selftest at closeout.

**Minor-6. Handoff overclaims and omissions.**

- (a) §2.2 says "Money On/Off at Year 0 carried into the session **and round-tripped through v3**". Only Money **Off** can round-trip. Money On refuses to save (`SaveEconomyRequiresV3`), as §2.1 and §4.3 of the same handoff say.
- (b) §3 says the restart test was "PASS at both viewports". The restart and overflow modes ran only at 1280×720 (`phase2a-edge-final/run.log:31–48`), and the 1280 run directory has only the `full` mode.
- (c) §2.4 says "Measured (Edge, 1280×720)" for the stand bytes. The evidence line is tagged `[1920x1080]`.
- (d) §6 does not mention that the first combine-time targeted Edge run **failed** (`combine-fast/edge-merge/run.log`: `OVERALL: FAIL (9 passed, 2 failed)`). It passed only after the Ui project was rebuilt (`build-ui.log` 12:22:51, then `edge-merge-2` PASS 36/0). The likely cause is a stale embedded-UI DLL, but the record should say so.

**Minor-7. P2A-07 test gaps.**
File: `tests/Core.Tests/Phase2A2Tests.cs`.

- There is no save → load → **re-save canonical byte equality** check. The continuation is compared by tree DBH, height, crown, living state and counts over Years 2–5, which is strong but not "identical canonical bytes".
- There is no refusal case for an unsupported generator **version**. Only an unsupported `generatorNote` is tested, at `:186–187`.
- No Core-level test shows that a refused v3 load leaves the host session unchanged. That is a host/UI concern and is covered only indirectly by the Edge suite.

**Minor-8. The test budget override can raise frozen budgets in the shipped binary.**
Files: `MultiViewBudget.cs:21,25–26,40–41,49–56`; `PlayerApi.cs:76`.

Any positive `CCF_TEST_MAX_VIEW_BYTES` replaces the budget with no clamp, including values above 512 KiB for MA-1 or above the 2.5 MiB ceiling. Spec §8 says the ceiling "may be lowered … but never raised".

*Fix:* clamp the override to `min(override, computed budget)`.

### Nit

**Nit-1. Stale player-visible text.** `MultiAreaContinuation.cs:658` and `:683` still say "C-V3, deferred to Phase 2A". C-V3 now exists; economy persistence is deferred to Phase 2B's economy block.

**Nit-2. The saves list is hidden in Money-On sessions** (pre-existing; `MultiApi.Saves.cs:20–29`). Now that v3 saves load self-contained and swap the stand, a player in a Money-On session cannot see or load a saved forest without first turning Money Off or starting a new woodland.

---

## 6. Conditions (all for a follow-up commit on `integration/2d-sim` before the Playable Milestone 1 closeout)

1. **Fix Major-1** by returning 503 on `skip`, `cell`, `competition`, `hit` and `economy/session`, and extend the forced-overflow test to at least `skip` and `hit`.
2. **Fix Minor-1:** a v3 load with the confirmation omitted must be refused while there is unsaved progress.
3. **Fix Minor-8:** clamp the test budget override so it can never raise a budget.
4. **Complete `build/2d-gate/selftest.sh`** (already deferred by the owner), with mutation cases added for the new G4 assertions (Minor-5).
5. **Record the C-V3-versus-spec §6.2 reconciliation** (Minor-4), and correct the handoff statements in Minor-6 and Minor-3 through an addendum. The existing handoff should not be rewritten.
6. **Measure a max-cap custom woodland** (about 3,000 stems) at Year 0 and Year 15 against the 2.5 MiB `/view` budget before closeout (§7.1).

Minor-2, Minor-7, Nit-1 and Nit-2 are recommended but are not conditions.

---

## 7. Residual risk (Playable Milestone 1 closeout)

1. **Large custom woodlands are unmeasured.** M0 covers only the two 1,344-stem presets. The raw `/view` is about 1,180 B per stem at Year 0 and grows by Year 15. A custom stand near the 3,000-stem cap could be about 3.5–3.8 MB raw against a 2.5 MiB budget. Whether Tier-1/2 compaction brings it under is **not verified**. If it does not, `/view` returns 503 and the page cannot show that woodland. Because of Major-1, the skip/cell/hit routes would also send over-budget bodies.
2. **Lane-read headroom.** The 3-lane read sits 15.8 KB under 512 KiB at Year 15. Today this affects only the API and tests (Minor-3), but it becomes player-facing as soon as the UI switches to lane reads.
3. **Money On cannot be saved.** This is intended and fails closed, but players will see it. A player who starts a woodland with Money On cannot save it at all until Phase 2B.
4. **Gate integrity.** The gate script changed and selftest is incomplete (Condition 4).
5. **Embedded-UI build trap.** `SitkaP1.Ui.csproj` is not in the `.sln`. The first combine-time Edge failure shows that a stale DLL makes browser evidence go wrong. Closeout scripts must build the Ui project explicitly.

---

## 8. Not verified by this reviewer

- I did not run any build, Core/Save test, gate, selftest or browser suite. Build and test results come from the writer and coordinator logs listed in §3 (on the same trees). The Edge, UI-smoke, M0 and gate results are writer evidence.
- Major-1 was found by code reading. I did not trigger it at runtime.
- I did not personally check accessibility behaviour (focus trap, screen-reader output) in a browser. That rests on the native `<dialog>` semantics and the writer's Edge assertions.
- I did not check the selftest cases completed before the stop (the log has no per-case verdicts), nor that C11 (Phase1 edit makes G4 fail) still fires under the new G4.
- I did not check whether the owner's 2A-2 handoff packet (not in the repo) explicitly authorised the Money On/Off toggle, given that spec §2 lists economy and money as Phase 2B scope.
- No Unity, Windows, 3D or real-device verification. That is out of scope.

---

## 9. Finding counts

Critical 0 · Major 1 · Minor 8 · Nit 2

INTEGRATION DECISION: APPROVED WITH CONDITIONS
