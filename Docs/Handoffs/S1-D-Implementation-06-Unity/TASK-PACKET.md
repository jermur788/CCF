# S1-D T-06 — Unity Qualification Task Packet

## Task

**S1-D T-06 — qualify management-event ID save hardening in Unity before integration.**

This is a Tier-2 bounded feature qualification. Production and verification logic are read-only unless the Manager explicitly returns a defect for repair.

## Authority and exact candidate

- Approved spec authority: `specs/scenario-one-repeated-interventions-v1/` (RD-12 / FR-E3 / AC-7).
- Implementation production commit: `8e05b5a24d7c3c74d063feb28002a6d7f28db438`.
- Reviewed code candidate containing the authorised U4 repair: `29bb6cbd6bf5dd81499a1e45bb68666f34969309`.
- Independent execution result: `Docs/Handoffs/S1-D-Implementation-06-Review/INDEPENDENT-EXECUTION-CONFIRMATION-RESULT.md` at `6b8ef4de5d89cce55e14cdf78f865191bb14c2be`.
- Manager approval of static/executable review: `Docs/Handoffs/S1-D-Implementation-06-Review/MANAGER-REVIEW-INDEPENDENT-EXECUTION.md`.
- Required production validator blob: `Assets/ForestPrototype/ForestSaveValidation.cs` = `28b786d49fc728e50a1009bc0c42fd0f7c4f33bb`.

Qualification must run against exact code candidate `29bb6cb` or a docs-only descendant proven to preserve the same production and verification blobs. Do not silently rebase or absorb unrelated work.

## Queue / Unity safety

RACK-V2-08 claim #49 retains machine-wide Unity priority. This packet is **WAITING** until the Overall Manager releases the Unity lane after #49 or explicitly reroutes it.

Before Unity:

1. refresh current `main`, this packet, live `Docs/Project/coordination/active-tasks.md`, and the qualification claim;
2. use a dedicated clean worktree rooted at the reviewed candidate;
3. record actual model/provider, branch/worktree and starting HEAD in the claim;
4. verify the production validator blob and U4 verification source match the reviewed candidate;
5. verify no other owner holds the Unity lock, then acquire it normally;
6. no Windows build.

## Required Unity qualification

### Q1 — T-06 runtime loader parity and no mutation

Run the existing disposable gate from the candidate worktree while holding the Unity lock:

```sh
python3 -I Tools/Verification/ScenarioOneRepeatedInterventions/EventIdIntegrity/run_save_event_id_integrity.py --i-hold-the-unity-lock --label run-1
```

Require:

- valid positive unique histories accepted;
- zero, negative and duplicate IDs rejected;
- `CanLoad`, `LoadData` and disk `Load()` agree on verdict/reason for every case;
- rejection occurs before world mutation;
- ecological year, tree count, player/world snapshot and full Scenario One save snapshot remain unchanged on rejection;
- `forest-save.json`, `.bak` and `.tmp` bytes remain unchanged on rejection;
- both Legacy40 and Enlarged80 coverage;
- historical v19 valid/invalid cases as encoded by the harness;
- baseline restore succeeds;
- player save digests unchanged after the run;
- no temporary copied verification source or `.meta` remains under `Assets/ForestPrototype/`.

Expected success token from the harness is the canonical `SAVE_EVENT_ID_VERIFY_PASS ...`; record exact observed counts rather than assuming the source-count estimate.

### Q2 — Existing save hardening preservation

Run `Tools/Verification/SaveHardeningVerification.cs` using its checked-in header instructions/copy-run-remove pattern.

Require canonical:

`SAVE_HARDENING_VERIFY_PASS`

Do not weaken or rebaseline any existing assertion.

### Q3 — Cell-index hardening preservation

Run:

```sh
python3 Tools/Verification/SaveCellIndexHardening/run_save_cell_index_hardening.py
```

Require the canonical success results including:

- `SAVE_CELL_INDEX_HARDENING_VERIFY_PASS`
- `SAVE_CELL_STARTUP_PASS`

If the suite's accepted event fixtures collide with live baseline event IDs, stop and report the exact fixture/baseline state. Do not renumber production history or silently rewrite the test contract.

### Q4 — U4 repaired duplicate-ID runtime case

Run the relevant U4 gate on the reviewed `U4Qualification.cs` repair. At minimum establish that `SYN duplicate event IDs`:

- is refused by `LoadData`;
- leaves year unchanged;
- leaves the captured Scenario One/world snapshot unchanged;
- does not bypass `ForestSaveValidation`.

Preserve S1/S2/S4 semantics and N3/rules-level duplicate chronology coverage. The helper does not currently assert the exact rejection reason; record the observed reason if available, but do not broaden production or verification code merely to add it during qualification.

### Q5 — Unity import / compile

Require a clean candidate import/compile with no new candidate-caused compiler errors. This gate should also provide the deferred real Unity import/compile observation for applicable S1-D T-02 script/meta introduced earlier.

Known unrelated/baseline issues may be reported only with exact evidence and must not be used to waive a new error in T-06 or the U4 repair.

## Preservation checks

After Unity qualification, rerun the already-passing offline/static suites if cheap or if any Unity-side file changed unexpectedly:

- T-06: 75 checks + 7/7 mutants;
- N3: 88 checks;
- T-02: 93 checks + 7/7 mutants.

No production or verification code edits are authorised by this packet. Any candidate-caused failure returns:

`S1-D T-06 UNITY QUALIFICATION NEEDS CHANGES — MANAGER REVIEW REQUIRED`

## Output

Publish:

`Docs/Handoffs/S1-D-Implementation-06-Unity/HANDOFF.md`

Include:

- actual executor/model/provider;
- branch/worktree, starting HEAD and exact qualified candidate;
- Unity lock acquisition/release evidence;
- exact commands and success/failure tokens;
- observed T-06 case counts and loader parity results;
- no-mutation/world/disk evidence;
- SAVE_HARDENING and save-cell-index results;
- U4 duplicate-ID runtime result;
- Unity compile/import result;
- cleanup proof for disposable scripts/meta;
- repo/worktree cleanliness and unrelated-work accounting;
- unresolved risks;
- explicit statement that no Windows build was made.

### Success token

`S1-D T-06 UNITY QUALIFIED — READY FOR MANAGER INTEGRATION REVIEW`

Do not open or merge an integration PR from this qualification task unless the Manager separately authorises it.
