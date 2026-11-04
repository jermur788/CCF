# Rack V2 committed editing-gate expectation repair — handoff (#277)

**Task:** [#277](https://github.com/jermur788/CCF/issues/277) · **Packet:** [`RACK-V2-EDITING-GATE-REPAIR-PACKET-20261104.md`](RACK-V2-EDITING-GATE-REPAIR-PACKET-20261104.md)
**State:** **HANDED-OFF — gate repaired, verified at both resolutions, independently reviewed. Do not merge without a separate Manager integration verdict.**
**Worker:** T3 Code / `deepseek-v4.1-flash` (provider `opencode-go`, OpenCode), single agent. Packet routing named GPT-6.1 Medium with Claude Sonnet 5.5 Medium review; the observed-capacity substitution is recorded on the issue.
**Unity lease:** `1d0312f81bfd41fe83371114a2f6bdbc`, task `RACK-V2-EDITING-GATE-REPAIR-277`, acquired 2026-11-04T08:50:54Z, **released normally** (receipt in §10).
**Independent review:** Claude Sonnet 5.5 Medium, exact diff at `17288a41`, verdict **APPROVED WITH NOTES** — no blocking finding; test strength preserved and increased (§8).

---

## 1. Identity and scope

| Item | Value |
|---|---|
| Starting head | `55b7e5b6499af6c268792a6641ad3c97e502a50d` (equals `origin/qual/scenario-one-overnight-ui-20261104-v1`, the #276 head) |
| Unity-tested predecessor | `d5fdf1c941065e9c543a090fe8039cafdd97729e` — confirmed ancestor |
| Fix branch | `fix/rack-v2-editing-verification-free-rotation-20261104` |
| Test/source head (Unity-tested) | `17288a419b883718738cfc4e452d883a82bda580` |
| Handoff commit | docs-only commit after `17288a41` on the same branch; the test source at the final branch head is byte-identical to `17288a41` |
| Lane | `/media/jer/ZX20/CCF-UIQualification-20261104` (registered `UIQual275`, `unity: true`) |
| Unity | `6000.6.0f1` (f7f8ed4d1e24) |
| Source diff | `17288a41` = **1 file**, `Tools/Verification/RackV2Editing/RackV2EditingVerification.cs`, +58/−17; no other tracked file |
| Test source sha256 | `2bf4abbd7ad60fdf2cb7d9040caa67004c36c134c9dcc0d32343616d84814615` |

**No production, geometry, save/schema, scene, prefab, material, `.meta`, package, ProjectSettings, Reference Future, spec, CTA, colour, focus-policy or MFM file was edited.** No merge, no Windows/Player build, no user-playtest branch change, no acceptance claim.

## 2. Why the test failed before the repair

The committed gate still encoded the pre-#256 belief that a round-handle (Reshape) release 1.2° off level would soft-snap back to level, and that one Undo would then return a level rack. Production #256 deliberately makes a Reshape rotation free at every angle and ignores Shift; Shift only disables the **Create** soft snap.

The unmodified committed gate was run first at 1280×720 on the exact starting head. It failed at the first stale rotate assertion:

```text
gate=editing exit=1 reaped=False seconds=799 licence-sha=46f0748b1de0 pass-marker=False
RACK_V2_EDITING_UNITY_FAIL checks=178 shots=4 exception=System.InvalidOperationException:
1280x720: rotating by the handle keeps the entrance, and a near-level release snaps back to level
```

Part A had already completed at that point (`part A complete: facade semantics verified, refusals leave the save byte-identical`). This is a verification-expectation defect, not a production regression. The failed baseline run is retained, unedited, at `gate-baseline-720/`.

## 3. What changed (only the Part-B rotation/Undo/Shift block)

File: `Tools/Verification/RackV2Editing/RackV2EditingVerification.cs`.

The old block contained:

- a positive `Near(Rack(1).Far.Z, Rack(1).Entrance.Z, 0.0001f)` assertion that a near-level **no-Shift** handle release snaps level;
- a large rotation, then a single Undo and a positive “undo returned rack 1 to level” assertion — which is wrong once the first rotation stayed free;
- a conditional Shift case that printed `PASS` only when a 0.4–3° angle was seen, otherwise `Note(... UNVERIFIED)`.

The repaired block:

1. **Real Shift, positive PASS/FAIL.** Starts from the known-level rack, drags the round handle to a near-level point with the real Shift key held, and asserts in the live sync that `Keyboard.current.shiftKey.isPressed`. It then asserts the entrance is fixed, the released angle is free (0.4–3°), the far end did **not** return to level, the committed rack equals the last captured handle ghost, and the card says `Rotated rack 1.` with no “suggested”/“parallel”/“edge” snap claim. The old conditional `Note()` is gone, so missing key input fails rather than being reported as unverified.
2. **Exact Undo of the Shift rotation.** Its real prior state was the level rack; Undo now asserts exact entrance/far geometry and that only one level was available.
3. **No-Shift near-level free rotation.** Same positive unsnapped-angle assertion, fixed entrance, `!Near(far.Z, entrance.Z)`, last ghost == committed rack, and plain rotation wording with no snap claim. This replaces the old “snaps back to level” expectation.
4. **Large rotation and exact Undo.** The 6–14° free rotation and identity checks are kept. Undo now asserts the exact **actual prior** near-level free geometry plus `PlanSig() == planBeforeLargeRotation`, and that only one level was available. No manufactured “reset to level” edit and no hidden state change.
5. **Helper `CapturedRotatingGhost`.** Captures the live handle ghost at the final waypoint (sync at step 14, after the harness repeats the last waypoint and sleeps) so ghost-vs-commit and the real Shift key can be checked while the button is still down.

Ordering note: the Shift experiment runs first from the known-level rack, so its Undo legitimately restores level; the no-Shift near-level experiment then starts from level; the large rotation's Undo restores the free near-level state. This tests each Undo against the state that actually preceded it rather than assuming a level reset.

**Angle tolerances** are taken from #275's recorded real-pointer observations, not manufactured angles: near-level observed 1.234–1.854°, large observed 12.131–12.154°, old soft-snap window 4°. The gate uses 0.4–3° for near-level and the unchanged 6–14° for large; a snap to level or to a tidy angle would fail.

**Preserved unchanged:** all Part-A facade assertions; Create soft snap without Shift; Move headland/length/angle; every refusal and whole-save byte-identity check; overlap refusal; one-level undo and edit-history checks; reset/suggestion identity; confirmed-plan and executed-rack locks; ghost-vs-release for draw/move/reshape; outside-rack-plan save identity; S10/S11, the source-static CTA checker and D-064/OPEN-2. Create-with-Shift is not part of this committed gate and was not added; it remains covered by #275's separate derived harness evidence, and this task deliberately did not widen the gate.

## 4. Verification — committed real-pointer gate at both resolutions

Exact command, after committing the test change:

```text
cd /media/jer/ZX20/CCF-UIQualification-20261104
python3 -I -B Tools/Verification/RackV2Editing/run_gate.py \
  --gate editing --out <evidence>/gate-fix-both \
  --sizes 1280x720,1920x1080 --timeout 3000
```

```text
gate=editing exit=0 reaped=False seconds=412 licence-sha=46f0748b1de0 pass-marker=True
RACK_V2_EDITING_UNITY_PASS checks=356 shots=17
```

- Real desktop pointer and keyboard on a private Xephyr display; measured calibration error **0 px** at both 1280×720 and 1920×1080.
- **0** `error CS`; **0** `RACK_V2_EDITING_UNITY_FAIL`.
- 17 PNG captures retained; no black captures (the gate refuses them).
- 356 checks total, versus 178 checks reached by the old gate before its first rotate failure.

| Real-input case | 1280×720 | 1920×1080 |
|---|---|---|
| Shift held, near-level round-handle release | **1.165924°**, unsnapped, real key asserted pressed | **1.16671526°**, unsnapped, real key asserted pressed |
| Shift edit-card wording | `Rotated rack 1.` — no snap claim | `Rotated rack 1.` — no snap claim |
| Undo of Shift rotation | level rack restored exactly; one level only | same |
| No Shift, near-level round-handle release | **1.165924°**, free, not level, ghost == commit | **1.16671526°**, free, not level, ghost == commit |
| No-Shift edit-card wording | `Rotated rack 1.` — no snap claim | `Rotated rack 1.` — no snap claim |
| Large round-handle rotation | **12.0605011°**, entrance fixed, id 1 | **12.0914745°**, entrance fixed, id 1 |
| Undo of large rotation | exact prior free near-level geometry + plan signature; one level only | same |
| Create (no Shift) | still soft-snaps to suggested direction | same |
| Move | still on headland, length/angle kept, exact Undo | same |
| Part A refusals / save identity / locks | PASS | PASS |

## 5. Static verification

```text
git diff --check HEAD^ HEAD                                  → clean
python3 -I -B Tools/Verification/RackV2Editing/run_offline.py → RACK_EDITING_OFFLINE_PASS checks=61
dotnet build                                                 → 0 warnings, 0 errors
```

The offline check exercises the unchanged pure geometry/validation rules; the changed verification file itself was compiled and executed by the Unity gate above. No full Tier-3, Objective 1 end-to-end, or unrelated gate was rerun, because the change is confined to the committed editing gate and touches no production byte.

## 6. Evidence and rendered artefacts

Evidence root (durable Files storage):
`/media/jer/Files/CCF/Verification/RackV2EditingGateRepair-277-20261104T084945Z/`

| Path | Contents |
|---|---|
| `preflight.txt` | lane/branch/HEAD/ancestry/clean/process/disk receipts |
| `lease-before.json`, `lease-acquire.json`, `lease-release.json`, `lease-after.json` | machine-wide lease lifecycle; final status FREE |
| `lanes-json-repair.diff`, `lanes.json.before-277` | the operational registry-path repair described in §7 |
| `gate-baseline-720/` | unmodified committed gate, 1280×720, expected stale-assertion FAIL retained |
| `gate-fix-both/` | repaired gate, both sizes: log, summary, result evidence, 17 PNGs, pointer trace, shots.json |
| `offline/` | offline rack-editing PASS and build output |

## 7. Operational preflight repair (disclosed)

The deployed machine-wide lease at `/media/jer/ZX20/CCF-Ops` refused every acquisition with:

```text
ERROR: git failed (128): fatal: not a git repository: '/home/jer/CCF/.git'
```

Its `lanes.json` still listed the legacy Git store at the pre-relocation path. The storage architecture records that the legacy store moved to `/media/jer/Files/CCF-Legacy/.git`, which exists. I backed up the registry and changed **only** that `git_stores[].git_dir` value; the diff is in the evidence root. No lane registration, lane path, board row, Unity flag, storage threshold, lock state or product authority was changed. The `UIQual275` lane was already registered, clean and available, and the branch has no other writable checkout.

## 8. Independent review

- Reviewer: **Claude Sonnet 5.5 Medium**, separate delegated task, read-only (no Unity, no edits, no build).
- Reviewed exact head: `17288a419b883718738cfc4e452d883a82bda580`.
- Verdict: **APPROVED WITH NOTES — no blocking findings.**
- Explicit finding: test strength was **preserved and increased** — the inverted snap assertion became a positive unsnapped-angle check; Shift went from conditional to mandatory; Undo gained exact geometry and plan-signature assertions; no original safety assertion was weakened or removed; scope was the single verification file.

Non-blocking notes carried forward:

1. A future comment could state explicitly that Shift has no effect on a Reshape (it proves the real key path and the free rotation); the new header comment only partly says this.
2. The status check uses `StartsWith("Rotated rack 1")` plus negative substring checks; pinning the exact string `Rotated rack 1.` would be slightly stricter, but the current check already rejects snap wording.
3. `StartsWith`/`Contains` are culture-sensitive; harmless for this ASCII text.
4. Rack length is implicit in the 1.5 m handle offset giving ~1.2°; the bounds are deliberately generous around that.

## 9. Residuals and honesty notes

- This is a **Tier-2 verification repair**, not a product acceptance or a Player build. No Windows build was made or authorised.
- #275's deliberate residuals remain **unchanged and unwaived**: source-static **FAIL 08** (`objective-primary-cta`), **D-064 / OPEN-2** field-marking-before-approval behaviour, and the `16–20 m` spacing display in the Stand Map card.
- The only non-CCF exception in the verified run is the known `UnityEditor.Search.SearchDatabase.EnumerateAll` `ArgumentOutOfRangeException` at Editor startup (QuickSearch indexer, before any CCF check). It did not affect an assertion; it is logged, not waived.
- The verified branch was not merged into the active playtest candidate, staging or `main`. The published personal-playtest branch and the user's save were untouched.
- Handoff commit is docs-only after the tested head; no Unity rerun is claimed for it.

## 10. Cleanup and lease release

After the run, Unity had rewritten its known side-effect files (`.vscode/settings.json` and three `SectionFive` forbs materials). All four were restored byte-exactly with `git checkout --`; the lane is clean including untracked files, the disposable staged gate and its `.meta` are gone, and no Editor/Xephyr/pointer process remains.

```text
$ /media/jer/ZX20/CCF-Ops/scripts/unity-lock release \
    --owner T3-Code-deepseek-v4.1-flash \
    --task-id RACK-V2-EDITING-GATE-REPAIR-277 \
    --lock-id 1d0312f81bfd41fe83371114a2f6bdbc \
    --reason "CCF #277 committed rack-editing gate repaired and verified at 17288a41 at 1280x720 and 1920x1080; lane clean, side effects restored, no Editor" --json
→ status=released, forced=false, unity_editor_pids=[]
→ subsequent status: free, metadata null, unity_editor_pids []
```

## 11. Pull request and stack

Stacked PR on `qual/scenario-one-overnight-ui-20261104-v1` (PR #276), **not** on `main`, staging or the published playtest branch. Head branch: `fix/rack-v2-editing-verification-free-rotation-20261104`. Do not self-merge. Manager integration verdict is still required.

## 12. Dashboard synchronization

Not performed by this local worker and **not claimed**. The Manager/browser lane (#200) remains the owner of the live coordination record and dashboard publication. No generated dashboard or coordination-state file was hand-edited. `Docs/Project/coordination/active-tasks.md` was refreshed read-only from `origin/main`; this handoff does not edit it.

## 13. What the Manager must decide

1. Accept `17288a41` as the exact test repair head and review the docs-only handoff commit on top.
2. Confirm the operational `lanes.json` legacy-store path repair is acceptable as recorded, or fold it into the ops record under #200/#203.
3. Decide the integration order for PR #276 / #273 now that the committed editing gate matches #256. **No merge has been performed by this task.**
4. Keep FAIL 08, D-064/OPEN-2 and the `16–20 m` presentation residual as recorded; none is caused by, or was changed by, this repair.
