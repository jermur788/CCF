# Focused UI Unity qualification — handoff (#275)

**Task:** [#275](https://github.com/jermur788/CCF/issues/275) · **Packet:** [`FOCUSED-UI-UNITY-QUALIFICATION-PACKET-20261104.md`](FOCUSED-UI-UNITY-QUALIFICATION-PACKET-20261104.md)
**State:** **HANDED-OFF — qualified, with one bounded non-production follow-up and the deliberate Objective 1 defects preserved.**
**Integration recommendation:** **NEEDS CHANGES** — not in the staged feature bytes (they are qualified) but because the committed `RackV2EditingVerification` gate still asserts the pre-#256 handle-snap behaviour and would fail after integration. That test-expectation update needs a Manager decision; nothing in it was silently rewritten here.
**Worker:** T3 Code harness / `deepseek-v4.1-flash` (provider `opencode-go`, OpenCode). Single agent; no sub-agents.
**Unity lease:** `b744ee32ea7141ff885848fa2726e9b5`, owner `T3-Code-deepseek-v4.1-flash`, task `FOCUSED-UI-UNITY-QUALIFICATION-275`, acquired 2026-11-04T07:00:24Z, **released normally** (receipt in §9).

---

## 1. Identity and ancestry

| Item | Value |
|---|---|
| Original user candidate (base of the staging ref) | `72b4efbd1c653fa2231f632a79030a83ff76f833` — confirmed ancestor of the qualified head |
| Reviewed staging head (#269 / PR #273) | `d3ba4fce3994d5c59b51786c6671dd654791e7d1` — unchanged, still the Files worktree's head |
| Qualification lane | `/media/jer/ZX20/CCF-UIQualification-20261104` |
| Qualification branch | `qual/scenario-one-overnight-ui-20261104-v1` |
| **Qualified head (Unity-tested)** | `d5fdf1c941065e9c543a090fe8039cafdd97729e` |
| Head delta vs the reviewed staging head | exactly one docs-only commit (`d5fdf1c9`, Phase A below); `git diff --stat d3ba4fce..d5fdf1c9` = 2 files, both `Docs/Handoffs/**` |
| `git diff --stat 72b4efbd..d5fdf1c9` | **14 files**, +903 / −19 (was 14 files / +9,522 / −19 before Phase A compaction) |
| Unity | `6000.6.0f1` (f7f8ed4d1e24), `/media/jer/ZX20/Unity/6000.6.0f1/Editor/Unity` |

### Why a second worktree (ownership, not preference)

The packet's lane `/media/jer/Files/CCF/Operations/CCF-Overnight-UIQualification-20261104` is on durable storage and is not a registered Unity lane. The deployed machine-wide lease (`CCF-Ops/scripts/unity-lock`) admits **only** registered lanes on `/media/jer/ZX20` (`on_storage()` + `lanes.json` + `unity: true`); it refuses anything else before it will write a record. #275 explicitly permits "a separate clean qualification worktree at the same exact commit if required for ownership", so:

- a clean worktree was created at **exactly** `d3ba4fce` on ZX20;
- lane `UIQual275` was registered in `CCF-Ops/lanes.json` (one entry added; no other config touched);
- the packet's Files worktree was left untouched, clean, at `d3ba4fce`, and is still the reviewable staging ref.

No branch was rewritten, no stash/clean/reset was used, and the user's warm `CCF-U1` Library was not read or reused.

## 2. Pre-admission receipts (all before any Unity launch)

Fetched live `Docs/Project/coordination/active-tasks.md` (2026-11-04 revision, single writer #200). #203 is a RUNNING local-orchestration umbrella that owns no Unity lane; no other live claim, no Editor, no verification process.

```text
lane      /media/jer/ZX20/CCF-UIQualification-20261104
branch    qual/scenario-one-overnight-ui-20261104-v1
HEAD      d5fdf1c941065e9c543a090fe8039cafdd97729e
status    (clean; no output from git status --porcelain -uall)
process   no Unity Editor process; no run_gate/native_input/pointer_driver/Xephyr
disk      ZX20 55,739,023,360 B free (51.9 GiB)   Files 105,583,767,552 B free (98.3 GiB)
lease     free -> acquired b744ee32ea7141ff885848fa2726e9b5 (unity_editor_pids: [])
```

Admission floor: a cold Library import of this project measured **6.6 GiB on disk** here; post-import ZX20 free was **48 GiB**, so the 15 GiB floor was never approached. Full receipts: `preflight.txt`, `phase-a-byte-identity-inventory.json` in the evidence root.

### Licence note (cost ~35 minutes; recorded so the next worker does not lose it)

The first three import attempts failed with `No valid Unity Editor license found`, `Found 0 entitlement groups`, licensing-client warning `LicenseGroupOfflineValidityPeriodIsExpired`. Cause: the editor was reading the **stale** `~/.config/unity3d/Unity/licenses/UnityEntitlementLicense.xml` (UpdateDate 2026-10-15), whose offline validity had expired, while the Unity Hub had already refreshed its own copy (`~/.var/app/com.unity.UnityHub/config/...`, UpdateDate 2026-12-04). The shipped `run_gate.py` already selects the newest licence by mtime, so every gate run was fine; only my ad-hoc manual import picked the old one. With the Hub copy, import succeeded immediately (`Licensing::Client Successfully resolved entitlement details`). No licence file, account or Hub state was modified.

## 3. Phase A — docs-only hygiene (committed separately, `d5fdf1c9`)

Manager ruling: remove/compact the duplicated payload, preserve permanent provenance. Both files are inside the 13-path staging inventory; they are now links-plus-results documents and name their own byte-identical originals.

| File | Before | After | Original still available at |
|---|---|---|---|
| `Docs/Handoffs/Overnight-Continuation-20261104/OBJECTIVE-COPY-HANDOFF.md` | 4,884 lines / 450,266 B | **44 lines** | `693e24c8…:Docs/…/OBJECTIVE-COPY-HANDOFF.md`, PR [#262](https://github.com/jermur788/CCF/pull/262), sha256 `5dba7304308664e22e6770adcf157806ed85d895b1c9a9cbbc2134afb6b8d74d` |
| `Docs/Handoffs/Overnight-Continuation-20261104/RACK-HANDLE-HANDOFF.md` | 3,889 lines / 322,835 B | **110 lines** | `9304d318…:Docs/…/RACK-HANDLE-HANDOFF.md`, PR [#261](https://github.com/jermur788/CCF/pull/261), sha256 `9e9b092f6ec210c526cfa38bc907bb87d1fd2a96b5d18d8032d896c3a63f843b` |

Preserved verbatim: status, source PR + source head, before/after copy, the complete production diff, every offline result, every explicit non-proof, the manual Unity script, the V2 routing appendix and the publication receipts. Removed: pasted AGENTS/AgentWorkflow/context dumps and raw command transcripts. Both files point to the #269 staging proof at `d3ba4fce` (PR #273) and to the independently archived review evidence on `main` (`Docs/Handoffs/Overnight-Continuation-20261104/review-269-evidence/`).

**New byte-identity inventory** (`phase-a-byte-identity-inventory.json`): of the **13** staged paths, **11 are still byte-identical** to their source heads; the **2 changed** paths are exactly those two handoffs. The 14-path file count of `72b4efbd..HEAD` is unchanged. No production C#, verification tooling, serialized asset, `.meta`, package or settings byte changed.

`git diff --check` clean. This is a separately reviewable commit (`d5fdf1c9`), as ruled.

## 4. Compile / import

Cold import + full script compile in the qualification lane:

```text
Scripting: domain reloads=1, domain reload time=4306 ms, compile time=98034 ms
Project Asset Count: scripts=1601, non-scripts=4050
error CS: 0        "No valid Unity license": none after the licence fix
```

No new compile error, no warning escalation attributable to the delta.

## 5. Unity Tier-2 gate A — `RackV2Objective1EndToEnd` (documented harness, real OS input)

```text
CCF_GATE_LANE=<lane> python3 -I -B <evidence>/harness/rgate275/run_gate.py \
  --name RackV2Objective1EndToEnd --method RackV2Objective1EndToEnd.Begin \
  --files RackV2Objective1/RackV2Objective1EndToEnd.cs \
  --native --normal-startup --timeout 2400 \
  --pass-token RACKV2_O1E2E_PASS --fail-token RACKV2_O1E2E_FAIL --out <evidence>/objective1
→ name=RackV2Objective1EndToEnd exit=0 reaped=False seconds=312 pass=True
→ RACKV2_O1E2E_PASS checks=173 shots=24
```

The staged bytes are the lane's own `Tools/Verification/RackV2Objective1/RackV2Objective1EndToEnd.cs` (unchanged from the reviewed head). The runner copy differs from the shipped `run_gate.py` in exactly one constant: `TASK = "RACK-V2-12"` → `"FOCUSED-UI-UNITY-QUALIFICATION-275"`, so the lock check names this task instead of borrowing another task's name (sha256 `9ace0c23…`, diff recorded in the evidence tree). Harness staging, licence isolation, private Xephyr display and post-run removal are the documented behaviour; no staged file or `.meta` was left in `Assets/ForestPrototype/`.

Measured / asserted (exact log values):

| Area | Result |
|---|---|
| Fresh world | Enlarged80 Rack Planning V2, 1,344 living stems, no racks/work; Objective 1 current |
| Objective 1 copy | purpose sentence, `16–20m`, "Plan Racks" present; no "18 m is the ideal", no "always required" |
| Real input | `P` refused before a confirmed plan; `W` walked **7.33 m**; `M` opened the Stand Map; `M` closed it; `Tab` opened the Work Plan |
| Planning | five suggested routes; real `Delete`; four-rack confirm disabled with a reason and nothing changed; Undo restored the deleted rack; move kept the plan confirmable; spacing shown |
| Confirm | five racks confirmed; **166** Pending rack-owned orders created; nothing felled, nothing marked |
| Tree Marks help | opened by itself on reaching field marking; lists orange, red, blue; closes |
| Counter/cue | `Rack entrances marked 0 / 5`; cue on the first unmarked rack |
| Approval before marking | **refused** — button disabled, `ApproveWorkPlan()` refused, nothing changed, Work Plan explains why (deliberate D-064/OPEN-2 defect, preserved) |
| Marking | real `P` + left-click marked all five entrances (`PO0203, PO1204, PO2003, PO2803, PO3803`); unrelated/removal/repeat spray refused; `5 / 5`; nothing approved by marking |
| Approve + execute | exactly the frozen rack trees approved; one year; exactly those trees felled; cash change = quoted net; work Completed and locked; `rack.result` |
| Lesson 2 | teaching area = the central confirmed player rack; Crop Tree marking works |
| Persistence | real `Save()`, `CanLoad()`, `Load()`; V2 validation; exact rack/order/lock signature; Objective 2 and `5 / 5` after load |

## 6. Unity Tier-2 gate B — physical create / move / **round-handle** input with and without Shift

The packet's physical rotation requirement is **not** covered by gate A. It is also no longer covered by the committed `RackV2EditingVerification`, which asserts the pre-#256 behaviour (see §8). A disposable focused gate was therefore derived **from that committed gate without editing it** (96 changed lines, all in the handle/Shift section plus two extra real-input cases), staged from outside the repository and removed afterwards.

```text
CCF_GATE_LANE=<lane> python3 -I -B <evidence>/harness/focused/run_gate.py --gate focused \
  --out <evidence>/focused-handle-3 --sizes 1280x720,1920x1080 --timeout 2400
→ gate=focused exit=0 reaped=False seconds=533 pass-marker=True
→ FOCUSED_RACK_HANDLE_UNITY_PASS checks=356 shots=19
```

Real desktop pointer and keyboard on a private Xephyr display; calibration validated to <0.0001 px at both sizes. 77 checks at 1280×720, 90 at 1920×1080, plus the shared Part-A facade checks and the harvested-rack capture.

| Case (real input) | 1280×720 | 1920×1080 |
|---|---|---|
| Draw from the headland, Shift released | 1.1 m off level over 70 m **soft-snapped** (far z == entrance z) | same |
| Draw with a real Shift keypress | stayed **0.795°** (not snapped) | stayed off-snap (PASS) |
| Move body, Shift released | entrance stayed on the headland, 12 m slide, length/angle kept, undo exact | same |
| Move body with a real Shift keypress | entrance still on the headland, length/angle kept, undo exact | same |
| **Round-handle reshape, Shift released** | 1.2° release stayed **1.324°** — no snap to level | 1.2° release stayed **1.234°** |
| Ghost vs release (handle) | committed rack == last ghost while the button was down | same |
| Edit card wording | "Rotated rack 1." — no "along the suggested direction"/"parallel to rack"/"to the stand edge" | same |
| Large handle rotation | **12.154°** free, entrance did not move | **12.131°** |
| **Round-handle reshape, real Shift held** | **1.854°**, still unsnapped | **1.762°** |
| Undo after each case | exact geometry restored | exact |

Also re-proved from the committed Part A on the same head (unchanged by #256): every refusal (too short, off-headland, outside stand, duplicate/overlap, non-finite) leaves the whole save byte-identical; undo is exactly one level and restores ids and the id allocator; confirmed-plan and executed-rack edits refused; the approved-work warning changes nothing; reset to the suggestion restores exact geometry.

**Ghost / committed geometry / selection / reopen**: gate B asserts ghost-vs-release equality for draw, move and reshape, the selection state, and the harvested (locked) rack having no handle. Reopen/reload after save is asserted in gate A's save/load round trip.

## 7. Rendered review (Objective 1 help and Tree Marks, 1280×720 and 1920×1080)

24 captures (`c01`–`c12` × 2 sizes) from gate A, 19 from gate B. Directly inspected at both sizes:

- `c01-objective-1-before-planning-*` — full Objective 1 panel legible at both sizes; "around 16–20m" appears exactly once; "Plan Racks on the Stand Map [M]"; no universal-spacing claim. **The highlighted primary button is still "Back to forest [O / Esc]"** (see FAIL 08).
- `c04-edited-plan-spacing-*` — the #256 sentence renders fully: "Drag a rack to slide it along the headland. Its round handle rotates freely. Hold Shift while drawing to turn snapping off." Round handle visible on the selected rack; "Nearest rack 2: 17.8 m — within the recommended range for this stand"; suggested/planned dashed-vs-solid legend correct.
- `c06-tree-marks-help-*` — "Introducing tree marking" heading, the Scenario 1 intro sentence, and the three rows (Orange vertical stripes / Red — Fell / Blue — Crop Tree) all render without clipping at 720p.
- `c05-confirmed-map-*` / `c09-five-entrances-marked-*` — Full Stand Map and the HUD minimap show the **same** five-rack plan and the same trail; the `5 / 5` counter and orange entrance stripe are visible in the world view.
- `c07-workplan-before-marking-*` — "Approve Work Plan" greyed out with "Mark the rack entrances in the forest before approving the rack work: 0 / 5 marked."
- `c10-workplan-approved-*` — 166 approved, €10287.17 after approved work, 1344→1178 stems.

No unintended rack-persistence regression was observed: the confirmed plan, the 166 orders and the 5/5 marking survive the real save/load.

## 8. Residual failures and verification debt (not claimed as passes)

1. **FAIL 08 — Objective 1 primary CTA — REAL and unresolved.** The source-static checker still reports `FAIL 08 objective-primary-cta`: the Objective 1 primary button invokes `ui.CloseAll()` / "Back to forest", not `ui.ShowMap()`. The rendered 720p/1080p captures confirm the *visual* prominence of that button. This is deliberate Scope Guard territory (#258 is DRAFT); it was **not** fixed, not relaxed and not rebaselined here.
2. **D-064 / OPEN-2 field marking before approval — deliberate defect, preserved.** Gate A asserts the refusal rather than the "correct" flow, exactly as the packet requires.
3. **Committed `RackV2EditingVerification` now encodes pre-#256 behaviour.** `Tools/Verification/RackV2Editing/RackV2EditingVerification.cs` still contains, in its real-pointer rotate section:
   - `Check(... Near(Rack(1).Far.Z, Rack(1).Entrance.Z, 0.0001f), "… a near-level release snaps back to level")`
   - and a Shift case built on the same assumption.

   With #256, a handle (Reshape) release is deliberately never snapped, so that gate would fail at its first rotate assertion. This is a **test-expectation** conflict created by an approved behaviour change, not a product regression — but it must be resolved before merge. It was **not** edited here (the packet forbids making tests green), and the focused gate in §6 asserts the approved behaviour instead of weakening it.
4. **NOT-TESTABLE 09** (`map/minimap/trail consistency`) remains not statically testable; §7 gives rendered same-plan evidence only, not a coordinate-parity proof.
5. **`16–20 m` with a space** still appears in the Stand Map card (`StandMapView.cs:603`) because that file was outside #255's two-file scope. Rendered, not a regression, still outstanding.

## 9. Offline gates (re-run at the qualified head)

```text
python3 Tools/Verification/RackV2HelpTeaching/run_offline.py                     → exit 1
  RACK_V2_HELP_TEACHING_OFFLINE_PASS checks=97
  FAIL S10 …, FAIL S11 …   → RACK_V2_HELP_TEACHING_FAIL failed=2 (historical base 6d3228c)
python3 Tools/Verification/RackV2HelpTeaching/run_offline.py --base 72b4efbd…   → exit 1
  PASS S11; FAIL S10 only (combined-footprint: #256/#260 paths outside the #255 allow-list)
  → RACK_V2_HELP_TEACHING_FAIL failed=1
python3 Tools/Verification/OvernightPlaytestFeedback/check_baseline.py \
  --rotation-state post-fix --instruction-state post-fix                          → exit 1
  SOURCE-STATIC TALLY PASS=9 FAIL=1 NOT-TESTABLE=2
  FAIL 08 objective-primary-cta (preserved)
  NOT-TESTABLE 06 round-handle-input, 09 map-minimap-trail-consistency
```

With the explicit real base, S10/S11 read exactly as predicted at #269 — branch-scope guards, not regressions, and not weakened. Raw output in `offline/help-default.stdout.txt`, `offline/help-base-72b4efbd.stdout.txt`, `offline/baseline-postfix.stdout.txt`.

## 10. Exceptions, edits and honesty notes

- **Only one genuine exception** in 845 live checks: `ArgumentOutOfRangeException` in `UnityEditor.Search.SearchDatabase.EnumerateAll` at Editor startup (QuickSearch indexer, before any CCF check ran). It is the same environment class of startup exception recorded for #243. It is **not** a CCF product defect and did not affect any assertion; it is logged, not waived.
- The `RenderChainCommand:ExecuteNonDrawMesh` frames in the logs are part of `Debug.Log` stack traces from the harness's own `Check()`, not renderer exceptions.
- Unity rewrote `.vscode/settings.json` and five `Assets/ForestPrototype/Art/SectionFive/**/*.mat` files after each run (known URP/Editor side effect). Every file was restored byte-exactly and `git status` was clean before every gate; the lane is clean now.
- Two focused-gate attempts failed **before** the passing run, both from mid-drag sync timing in my derived harness (a ghost captured at 2/3 of the drag, then compared to the release). The assertions were not weakened: the sync was moved to the final waypoint. Both failed runs are retained in the evidence tree.
- Nothing here is a Windows/Player build; no playtest branch, save, spec, CTA, colour, focus policy, settings, MFM topology, Rack V2 schema or Reference Future file was touched.
- Environment observation, unrelated to this task: while it ran, `CCF-Primary` was relocated from ZX20 to `/media/jer/Files/CCF-Primary` by something outside this task. I did not perform or authorise that move; this session's working directory was moved to the qualification lane afterwards.
- Reference-check note: the **published** remote branch `origin/integration/scenario-one-player-test-ready-v1` is still `72b4efbd1c653fa2231f632a79030a83ff76f833`, unchanged. The shared bare store's *local* branch of the same name points at a divergent `185200f5` line last moved on 2026-10-30 (merge commits, not an ancestor of `72b4efbd`). That is pre-existing local state, was not created or touched by this task, and is not the published candidate — but anyone comparing locally will see the difference.

## 11. Lease release receipt

```text
$ CCF-Ops/scripts/unity-lock release \
    --owner T3-Code-deepseek-v4.1-flash \
    --task-id FOCUSED-UI-UNITY-QUALIFICATION-275 \
    --lock-id b744ee32ea7141ff885848fa2726e9b5 \
    --reason "Tier-2 qualification complete at d5fdf1c9; lane clean; no Editor" --json
{
  "status": "released",
  "forced": false,
  "previous": {
    "lock_id": "b744ee32ea7141ff885848fa2726e9b5",
    "owner": "T3-Code-deepseek-v4.1-flash",
    "task_id": "FOCUSED-UI-UNITY-QUALIFICATION-275",
    "lane": "/media/jer/ZX20/CCF-UIQualification-20261104",
    "branch": "qual/scenario-one-overnight-ui-20261104-v1",
    "head": "d5fdf1c941065e9c543a090fe8039cafdd97729e",
    "start_timestamp": "2026-11-04T07:00:24.275283+00:00",
    "eta": "2026-11-04T12:00:23Z"
  },
  "unity_editor_pids": []
}
$ CCF-Ops/scripts/unity-lock status --json
{ "status": "free", "metadata": null, "unity_editor_pids": [] }
```

No forced release, no Editor process, lane clean at `d5fdf1c9`. Receipt kept at `lease-release.json` in the evidence root.

## 12. Evidence index

Evidence root (durable storage, not in Git): `/media/jer/Files/CCF/Verification/FocusedUIUnityQualification-20261104T070007Z/`

| Path | Contents |
|---|---|
| `preflight.txt` | lane/ancestry/process/disk/lease receipts at admission |
| `phase-a-byte-identity-inventory.json` | 13-path inventory, 11 unchanged / 2 compacted, per-file blobs |
| `import-graphical.log` | successful import + compile (0 `error CS`); earlier failed-licence logs retained alongside |
| `objective1/` | gate A log, `result.json`, `evidence/` (24 PNGs), staged-source hashes, licence-copy-removal record |
| `focused-handle-3/` | gate B log, `result.json`, `evidence/` (19 PNGs, `shots.json`, `pointer-trace.txt`) |
| `focused-handle/`, `focused-handle-2/` | the two harness-timing failures, retained unedited |
| `harness/` | `FocusedRackHandleInputVerification.cs` (sha256 `85d26bb9…`), the two patched runner copies, `pointer_driver.py`, `native_input.py` |
| `offline/` | the three offline stdout captures |

## 13. What the Manager must decide

1. **Accept the qualified head** `d5fdf1c9` as the exact ref for the staged #255/#256/#260 deltas (`d3ba4fce` + docs-only hygiene).
2. **Authorise the bounded test-expectation update** in `Tools/Verification/RackV2Editing/RackV2EditingVerification.cs` (handle-rotate expectations only) so the committed gate matches the approved #256 behaviour. Until then, integration leaves a failing committed gate.
3. **Keep FAIL 08, D-064/OPEN-2 and the `16–20 m` spacing display as recorded residuals** — none is caused by this staging round and none was changed.
4. The published playtest branch `integration/scenario-one-player-test-ready-v1` and the user's save remain untouched; the user still owes the personal playtest before any Windows authorisation.
