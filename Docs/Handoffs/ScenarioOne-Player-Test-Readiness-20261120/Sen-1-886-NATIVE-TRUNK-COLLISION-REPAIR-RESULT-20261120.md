# Sen-1 #886 — Compact30 trunk collision repair: native author result

**IMPLEMENTED / AUTHOR TARGETED VERIFIED / READY FOR OWNER IMMEDIATE EDITOR TEST / NOT INDEPENDENTLY REVIEWED / NOT MERGED.** STOP for owner feedback before any review, integration or Windows build.

Issue: https://github.com/jermur788/CCF/issues/886. Binding packet: `f89524f6c5e8bacae68c09a14595759a7bb71ee3`, `Sen-1-886-SOL61-COMPACT30-TRUNK-COLLISION-OWNER-FIRST-REPAIR-PACKET-20261120.md` on its published docs branch. Owner directly authorised this worker to register the proposed #886 lane and Ops board row in this native conversation on 2026-11-20. No broader Ops source/deployment authority was inferred.

## Exact candidate and custody

- Actor: **OpenAI Codex, gpt-6.1-sol, high reasoning**, through native T3 Code / Codex on jer-laptop, UID1000. Requested and executed model agree with the runtime identity supplied to this worker. Role: primary author, not independent reviewer. No delegates.
- Source parent: `fce41bd3b77a054bf82d2521648d15ec18f81141` (#876), no #857 import or main merge.
- Locked canonical context: `7e24ba0eee05b9af1055c88e423e629fe409bbf1`. Fresh live GitHub coordination inspected at main `90f2aefd5112c57ab97f55a2b29207aad0374ccb`; #886 has the two-file prospective reservation, subject to physical admission. Stale/dirty Primary was not used as live November authority or edited.
- Branch: `fix/sen1-886-compact30-tree-collision-20261120-v1`.
- Owner-test project: **`/media/jer/ZX20/CCF-Sen1-886-TreeCollision-20261120`**, newly created, isolated, registered `Sen1-886-TreeCollision`, own freshly imported Library; no Library copied.
- Production fix commit: `b7a64e76c471b9384f28b7f6e0c071ccd1806b81`. Source/test candidate: **`f1b04939614a467574b92000f49610e3bd81f5ac`**. This result's later documentation commit changes only this Markdown; production bytes are identical to all fixed-candidate runs below.
- Collision qualification HEAD: `078dd68311e3698797aa0d042ac56a1b49e27f79`; final restart/marker HEAD: `f1b04939614a467574b92000f49610e3bd81f5ac`. Production files identical at these HEADs. No uncommitted production edit was tested.
- Physical storage: ZX20 UUID `18D6-896E`, exFAT, persistent by-id identity verified; initially ~270 GiB free against 15 GiB minimum. Files is ext4 UUID `2283b135-bf37-42d6-921f-2fb861d0a8d4`, initially ~23 GiB free. Evidence lives on Files, not ZX20.

Deployed machine-wide lease owner `Codex-886-native`, task `SEN1-886-TREE-COLLISION`; every acquire required clean exact branch/HEAD and every release was normal, never forced. ETAs were work estimates only, explicitly not expiry under D-091:

| Phase | Exact leased HEAD | Lock ID | Disposition |
|---|---|---|---|
| Baseline | `2a1a6e02ea892e5bac1c2508901f52dff6f642ca` | `c760ce5c46e1441183cbe6cdd7d22395` | Normal release, no Editor remaining |
| First fixed author run | `56c04854547a6d4a63d21f4259ec88f522b61014` | `848663bcbaa94145bf33a21591a7d10a` | Normal release |
| Corrected interior-fixture qualification | `078dd68311e3698797aa0d042ac56a1b49e27f79` | `d9cc7b137c8f4c918b2b9d372bb978cd` | Normal release |
| Final Play Mode restart and #876 markers | `f1b04939614a467574b92000f49610e3bd81f5ac` | `f9dabe514cf64880bb06dfe918b9406a` | Normal release; final status FREE, no Editor PID |

`/media/jer/Files/CCF/Verification/Sen1-886-TreeCollision-20261120/` contains G0/grant, registration backups/witnesses, all acquire/release JSON, full logs, collider CSVs, captures, drift patches and cleanup witnesses. The lane is left clean and available for the owner's normally registered Editor session; author execution has stopped.

## Small production change

1. **`Compact30Bootstrap.cs`:** create one `Trunk collision` child per bound authored living tree, matching the tree's layer and cancelling its authored root scale. Its vertical capsule is sized in metres: radius `dbh_cm / 200`, height `max(1.2, height_m)`, centre at half height. All 175 measured roots are upright, positively and uniformly scaled; the child is unit-scaled in world space. No minimum radius/buffer is introduced. Parent hierarchy still resolves the actual `ForestTree` for inspection/marking rays.
2. **`Compact30Session.cs`, visible stump only:** disable/destroy the primitive's rounded capsule and attach a `MeshCollider` using that same built-in cylinder mesh. This is a static low stump, with no Rigidbody, package, new asset or fallen-log mechanic. The living tree's old colliders are still disabled by the existing felling handler. This costs one small mesh collider per visible felled stump and fits the visible cylinder more closely than the primitive capsule.

Source inspection corrected the packet's estimated stump width: actual `localScale=(0.3,0.175,0.3)` gives **0.30 m diameter × 0.35 m height**, not 0.60 m width. Dimensions and appearance remain unchanged. The actual player's radius is 0.30 m, height 1.80 m, stepOffset 0.25 m and skinWidth 0.03 m. It stops at this stump and walks around it; this repair does not silently make 0.35 m wood step over a 0.25 m step limit.

Production delta is **18 inserted / 3 removed lines in exactly those two allowlisted files**. New verification files are only under `Tools/Verification/Sen1CompactCollision/`; this result is the sole new handoff. No scene, prefab, mesh, material, `.meta`, package, ProjectSettings, UI, forest simulation, save or reference changes are committed. #876 marker source, Compact scene and roster, ForestPlayer and ForestTree retain their exact parent hashes.

## Native observed before/after

Unity **6000.6.0f1**, actual Compact30 scene objects and PhysX CharacterController.Move, not an offline estimate. Every initial root had exactly one enabled nontrigger layer-0 capsule; no extra decorative mesh collider was found. Baseline root scales actually span ~8.07–15.90, broader than the packet's static sample. Baseline capsules span **2.018–3.975 m radius**; candidate radii span **0.0491–0.1111 m**, matching the 9.82–22.21 cm authoritative DBHs. Renderer bases and collider bases are at ground y=0 within the measured tolerances.

| Stem / scale sample | DBH cm | Root scale | Radius before → after (m) | Collider height before → after (m) | Player-centre contact distance before → after (m) |
|---|---:|---:|---:|---:|---:|
| P0400 / smallest | 10.65 | 8.070 | 2.018 → 0.05325 | 65.125 → 8.07 | 2.272 → 0.383 |
| P0603 / median | 15.87 | 11.620 | 2.905 → 0.07935 | 135.024 → 11.62 | 2.908 → 0.409 |
| P0000 / largest | 22.21 | 15.900 | 3.975 → 0.11105 | 252.810 → 15.90 | 3.524 → 0.441 |
| P0401 / additional position | 11.99 | 9.590 | 2.398 → 0.05995 | 91.968 → 9.59 | 2.564 → 0.390 |
| P1000 / additional position | 18.81 | 14.170 | 3.542 → 0.09405 | 200.787 → 14.17 | 3.289 → 0.424 |

Candidate contact equals stem radius + 0.30 m player radius + ~0.03 m controller skin. For attributed contact and ray measurements, other tree colliders are temporarily disabled then restored. **Five further real between-tree walks use all scene colliders**, including the scenery, without early blocking. The sample includes perimeter and interior stems; physical fence collision remains valid.

## Actual verification and failed attempts

- **PASS: collision qualification 393/393, Editor exit 0.** Exhaustive 175 radius/height/base/centre/solid/layer/duplicate checks; five actual controller contacts; exact tree selection at both 3.5 m and 8 m using the production self-filter; five clear natural scene walks; three Crop callback toggle/unmark/re-mark sequences; separate Fell callback toggle/unmark/re-mark; real H1 arrival and assessment; Work Plan approval and real Year-1 felling; exactly one completed Fell order; living obstruction gone; visible low stump and physical bounds agree; controller stops 0.4500 m from stump centre then walks around; repeat Fell creates no duplicate; no old breast-height obstruction; real mortality transition disables a nonliving stem's obstruction; survivors retain one live collider. No invented missing stem is bound by the test.
- **PASS: true stop/re-enter Play Mode in one Editor.** Both entries have exactly 175 distinct living unmarked stems, one solid collider each, no stale stump. Editor exit 0.
- **PASS: existing, unchanged #876 Compact marker contract 110/110.** No weakened assertions; world mark sizes/positions, three Crop marks, Fell marks, repeat mark/unmark, moved roots and nonuniform scale witnesses. Editor exit 0.
- **PASS: actual import/compile**, zero `error CS` in baseline retry and each fixed/restart/marker log. Known QuickSearch `ArgumentOutOfRangeException` occurs once per log, including baseline; known LOD-name/import warnings remain. These were not newly repaired or waived by #886.
- **PASS: rendered author inspection** of native 1280×720 `qualification/standing-trunk.png` and `qualification/felled-stump.png`. They show the existing forest and a low cylinder at the felled tree location. This is author visual evidence, not owner acceptance.
- **Preserved initial failure:** the first fresh launch stopped before the harness because .NET named-pipe/socket creation was denied on exFAT TMPDIR. Retry used task-specific Files ext4 `Operations/UnityTmp-886` and compiled/imported successfully. No package or Ops tool change solved this.
- **Preserved baseline diagnostic:** 368 checks, 189 failures, intentionally demonstrates oversized capsules and distant controller contact. Nine of those failures were an author-test ray self-filter mistake: the player's own collider was included. The corrected harness now mirrors production self-exclusion; those nine failures are not claimed as a game defect.
- **Preserved first fixed run:** 390 checks, 3 failures. All 175 fixed world-dimension checks and five contact checks passed; the stencil selected only two unobstructed natural gaps, and the stump fixture P0010 started outside the fence. The author corrected fixtures to select five genuinely clear gaps and an interior stem at (1.45,6.60); acceptance thresholds were retained. The initial stump capture was empty because of the capture fixture; a separate temporary camera and interior site produced the qualified visible-stump image. Production code was not changed after `b7a64e76`.

See the new verification README for exact copy/run/remove commands and environment. The tests drive the **actual CharacterController** deterministically with ForestPlayer.Update temporarily disabled and exercise C/X public callbacks; **they do not prove real physical keyboard delivery or player feel**. The existing marking gate proves visual-transform contracts, not native input. Those are explicit owner-test items below. No old-save/reference/determinism runtime qualification or Windows build was run; historical code paths are unchanged and no broad regression was added.

## Preservation and cleanup

All disposable copied harnesses and generated `.meta` files were removed from `Assets/ForestPrototype/`. Final `git diff --check` passes and worktree is clean after this result commit. Witnessed Editor drift was preserved in evidence patches then restored by exact path only in this task-owned initially clean lane: `.vscode/settings.json`, three toolchain package upgrades in manifest/lock (1.1.0→1.1.1), and five SectionFive materials during the first fixed run. None is committed.

Private `XDG_CONFIG_HOME` was used from the first Editor launch. Before/after hashes cover 95 shared Unity files; shared game/save/prefs bytes are unchanged. Only the running Hub licensing client's two logs and entitlement refresh XML changed. Task-local copied entitlement material was removed from evidence. No save controller was instantiated and no test called a save API. Primary's unrelated tracked/untracked changes and #876's dirty settings/material/package files were left untouched. Author lock is normally released, with no remaining Editor process.

## Owner immediate Editor walkthrough — next gate, before review

1. In Unity Hub, add/open **`/media/jer/ZX20/CCF-Sen1-886-TreeCollision-20261120`**, Unity **6000.6.0f1**. Follow the normal physical Editor-session registration/lease; do not open another worker's lane. Editor may reproduce its known package/settings/material import drift; do not commit that.
2. Choose **CCF → Player Test → Play Compact30**, then focus the Game view. This opens the exact Compact scene; no harness is left to autostart or exit the Editor. Stop/Play starts a disposable fresh world; Compact persistence is not qualified here.
3. Walk towards small and large trunks and through several gaps, including near the footpath. Expected: body meets the trunk at normal close range, no 2–4 m invisible barrier. Crowns and needles do not form a solid fence. Real perimeter/deer-fence obstruction is still expected.
4. Aim at visible trunks: **C** selects/unselects three Crop Trees; **X** toggles a separate Fell mark. **E** inspection within ordinary 3.5 m reach should identify that tree. Blue/orange discs, bands and diamonds should retain #876 world sizes. Marking alone does not fell a tree.
5. Acknowledge H1 with **Y**, open current assessment with **J**, acknowledge with **Y**. Add/approve the separate Fell in Work Plan and advance Year 1 using normal controls. Expected: living trunk disappears, exactly one low visible stump remains; no invisible standing trunk/log. The 0.35 m stump blocks close contact, can be walked around, and can be jumped over with normal movement; it is not silently traversed at ground level.
6. Stop and re-enter Play: the fresh stand should have no duplicate obstruction or leftover stump. Report whether walking, aiming and stump behaviour now read coherently.

**Await owner pass/fail feedback.** Only afterwards may Manager #884 route a specific repair or independent review. No independent reviewer was dispatched; no merge/main acceptance, Windows build, historical-save qualification or external-demo readiness is claimed. Current manager-owned coordination may link this published handoff; this worker did not edit the authoritative shared coordination file or claim dashboard/Drive publication.
