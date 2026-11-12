# #437 — D-073 personal-playtest candidate lane and isolated launch proof

**12 November 2026 · Worker assessment: PASS for this bounded candidate-preparation task, with the launcher-check correction and verification limits below.** This is a handoff to parent [#412](https://github.com/jermur788/CCF/issues/412), not Manager conditional personal GO, user acceptance, full Tier-3 qualification, main integration or Windows/Player permission. Claim: [#437](https://github.com/jermur788/CCF/issues/437). Worker: **OpenAI Codex gpt-6.1-sol, high reasoning effort**, T3 Code on Predator (actual Linux hostname `jer-laptop`). No subagents.

The candidate is isolated and pinned; the user's valued saves and game PlayerPrefs were unchanged. One normal Editor session proved the normal title → all three introduction pages → rendered forest → native F5 path. **The original wrapper returned cleanup failure because my extra 81-file check included Unity's global Editor window layout. That result remains failure in the raw receipt.** All required 37-file valued-data and 4,011-file production gates pass. The final launcher corrects that extra check to protect the complete game-data inventory; its Editor command/profile/lease/cleanup restoration logic is unchanged. It passed fresh `--check-only`, but its complete final-byte Editor lifecycle was not rerun.

## Exact candidate and custody

| Item | Verified identity/result |
|---|---|
| Qualified production source | `339517ed2d49bb3d5716fc9cf029d9ef0eaeee72` |
| Candidate branch | `integration/scenario-one-personal-playtest-d073-20261112-v1` |
| Exact candidate HEAD | `55863f1b7719011bba16b3f3ab377e379c0b988a` (qualified feature tip; later commit adds handoff only) |
| Editor lane | `/media/jer/ZX20/CCF-S1-ResearchCopy-V1-Qual` — `ResearchCopyQual391` |
| Starting lane | Clean `qualify/scenario-one-d073-fr10-visible-provenance-unity-20261112-v1` at `ab92cbdf0f00f4875e6ae2485c22a1dab0fd693f`, warm Library |
| Candidate creation | Plain new branch at exact feature tip; normal push; `git switch` in the explicitly authorised lane; no merge or candidate commit |
| Ancestry from protected #382 tip | `b62446caf40fc2ac939d55a6bea6e201f41de700...55863f1b`: **0 behind / 22 ahead**; ancestor test exit 0; linear lineage |
| Production equality | `git diff --stat 339517ed 55863f1b -- Assets Packages ProjectSettings Tools` empty |
| Live preflight | Mounted read/write exFAT ZX20, 72,746,139,648 bytes free initially (>15 GiB); no Editor; lock FREE; required lane branch/HEAD clean; both new branch names absent locally and on origin |
| End state | Editor exited 0; no Editor PID; lock FREE; lane clean at candidate HEAD; no disposable helper/meta; 37 valued host files and all 4,011 production files identical; private profile completely empty |
| Result branch/worktree | `docs/fable-412-d073-personal-playtest-result-20261112-v1`; `/media/jer/Files/CCF/Operations/CCF-412-D073-PersonalPlaytest-Result`; created from `origin/docs/manager-d073-player-test-release-20261112-v1` at `daeef0b02a4be772988e5048919d75952f6d3abb` |

No game source or tests changed, no Player/Windows build, no main mutation. The older #382 candidate branch remains at `b62446ca`; its source worktree and protected Editor lane were not opened, checked out or written. U1/U2/U3 and the dirty coordination anchor were not operated on. The physical Ops board was read/linted (PASS, 24 live tasks) and its bytes remained unchanged; the parent owns its transition. No stash, hard reset, clean or force push.

Read inputs: Manager packet Phase 3/stop rules; independent #421 readiness matrix A/B; #427 full report including follow-up and inspected drivers/manifests; #382 and older conditional-GO packet; candidate AGENTS, AgentWorkflow, project context and live main coordination. `Docs/Operations/StorageArchitecture.md` is absent from both candidate and origin/main; I did not read the user's uncommitted local version. The explicit current storage paths in this task governed all writes. Earlier read-only input snapshots are retained in the evidence directory.

## Required upstream approvals (inherited, not rerun here)

- [#426](https://github.com/jermur788/CCF/issues/426): independent Opus G2 **APPROVED**, source `339517ed`, zero Blocker/Major.
- [#427](https://github.com/jermur788/CCF/issues/427): targeted Tier-2 **PASS**, qualified by the preserved original failures and follow-up. No suite repeated here.
- [#435](https://github.com/jermur788/CCF/issues/435): independent evidence **APPROVED**, report `fb047b29847f32b729bb3dbe34664a03bf5cd814`.
- Latest #427 parent correction read: two first-run native clicks were lost during the pointer-displacement window (foldout close and Later year), not one; the exact mover remains unknown. Missed foldout/year clicks remain a personal-test watch item. Original failures remain failures, not silently relabelled.

## Complete host backup and valued-data protection

**Backup:** `/media/jer/Files/CCF/Archives/PersonalPlaytest-D073-HostBackup-20261112/` (new, mode 0700; separate from every game's rotating `.bak`). Both entire existing roots were copied byte-for-byte:

1. `/home/jer/.config/unity3d/`
2. `/home/jer/.var/app/com.unity.UnityHub/config/unity3d/`

**81 files / 104,484,850 bytes**, no source symlinks. `originals.json` and `copies.json` record original absolute path, size and SHA-256 for every file. Original hashes were taken before copying and retaken after copying; all originals/copies matched file-for-file. Both manifest files have SHA-256 `bd80976387ed0a0d7776d29c3a42f247345e8b2b88b3f23e5c8e91c790225271`. Backup copies were checked again after the Editor and by the final launcher admission. No source save/config contents are published.

The 37 valued #427 paths are the complete file inventory of the host DefaultCompany game-data trees plus the Flatpak Unity global `prefs`. They all match the #427 baseline, the new baseline, the independent backup and the final after-manifest. The native `DefaultCompany/CCF/forest-save.json` was absent before/after; its valued `.bak` remained unchanged. The Flatpak `DefaultCompany/CCF/forest-save.json` and both game `prefs` files remained byte-identical. No new host game-data files appeared (so no host `.tmp`, replacement save or newly created save escaped the file-hash check).

**Known extra host write, preserved honestly:** Unity wrote `/home/jer/.config/unity3d/Preferences/Editor-5.x/Layouts/current/default-6000.dwlt`, outside the 37 valued game-data paths. Both the post-F5 and final post-exit inventories still contain exactly 81 host files: only that global window-layout file differs; none was added or removed (`host81-final-difference.json`). Its pre-session bytes are in the full backup. I did **not** restore or otherwise modify the host original. Thus **all 81 originals are not claimed unchanged**. XDG isolation here is game save/PlayerPrefs isolation as in #427, not a filesystem sandbox for every global Editor preference.

My original launcher had an additional full-host81 equality check. That check alone failed cleanup (`editor_exit=0`, `cleanup_result=1` in `proof1/result.txt`); all 37 required checks say OK, the backup copies pass, the source is clean and the lease was normally released. The final launcher replaces the extra host81 comparison with both **37 exact hashes + complete game-data inventory**. The backup-copy81 check remains. The exact executed script, final script and their diff are retained. No second Editor session was run to hide or retry this tooling result.

## Actual isolated paths and F5/PlayerPrefs proof

Profile root: `/media/jer/Files/CCF/Operations/PersonalPlaytest-D073/profile/`.

Actual launched process environment (`proof1/process-environment.json`):

```text
HOME=/home/jer                         # unchanged, matching #427
DISPLAY=:87                          # private Xephyr proof only
XDG_CONFIG_HOME=/media/jer/Files/CCF/Operations/PersonalPlaytest-D073/profile/config
TMPDIR=/tmp/ccf437
CCF437_PROOF_DIR=/media/jer/Files/CCF/Verification/D073-PersonalPlaytest-437-20261112/proof1
CCF_TEST_DIRECT_ENTRY absent
CCF_STARTUP_SCREEN absent
```

Unity's actual log reports engine `6000.6.0f1 (f7f8ed4d1e24)` and the following in Edit Mode and again with `playing=True`, active scene `Assets/Scenes/ForestTest.unity`:

```text
Application.persistentDataPath=/media/jer/Files/CCF/Operations/PersonalPlaytest-D073/profile/config/unity3d/DefaultCompany/CCF
ForestSaveController.SavePath=.../CCF/forest-save.json
ForestSaveController.TempSavePath=.../CCF/forest-save.json.tmp
ForestSaveController.BackupSavePath=.../CCF/forest-save.json.bak
PlayerPrefs file=.../CCF/prefs
walkingHelpIntroduced: 0 before intro, 1 after Begin
```

The save properties above were reflected from the actual loaded `ForestSaveController`, not reconstructed only from environment variables. A filename-only `strace` around the launch independently observed actual Editor opens of the private `prefs` with `O_WRONLY|O_CREAT|O_TRUNC`, and native F5's private `forest-save.json.tmp` open followed by successful rename to the private `forest-save.json`. **Zero game save/Prefs write syscalls into either real host game-data root** were found; the filtered receipt is empty by design. No file read/write contents were traced.

Before F5 there were no `forest-save*` files in the profile. The one native F5 created `/media/jer/Files/CCF/Operations/PersonalPlaytest-D073/profile/config/unity3d/DefaultCompany/CCF/forest-save.json`, **1,211,575 bytes**, SHA-256 `758a6b711ffd15b74317060733fc04b6cf44cce22e9407ea08fb3d2e35370692`. No `.bak` was needed in this fresh slot; no `.tmp` remained. The visible game message said saved v24, 1,344 trees. F9 was **not pressed**; inspected `ForestSaveController.Update/Load` and the actual reflected shared `SavePath` establish its destination, not a new behavioural load test.

The proof profile (including test save, prefs and analytics) was discarded after exit; the directory was recreated empty, mode 0700. It contains **no save, preference or licence copy** for the user's first session. Future personal sessions retain their own disposable progress; the launcher never copies it into host saves.

## One short launch-proof session

The same launcher path was used with worker-only `--proof`; the private Xephyr/openbox display was external to the launcher. Ordinary user invocation inherits the real desktop `DISPLAY` and uses no Xephyr, batch mode or direct-entry override. Original #427 evidence was read-only.

- Normal lease: **`9e7c1a7c8d3b49ef95f61fff7216f548`**, owner **USER personal playtest #412**, task `CCF-437-D073-PERSONAL-PLAYTEST`, acquired `2026-11-12T20:11:10.355524+00:00`.
- Launch: licensed Hub-managed Unity 6000.6.0f1, exact candidate project, `-openfile .../Assets/Scenes/ForestTest.unity -job-worker-count 2 -logFile .../proof1/Editor.log`. No Player build.
- Same #427 licence handling: newest existing entitlement directory copied into private XDG config, removed after Editor exit; originals not edited by the launcher. Short TMPDIR; private GTK `gtk-menu-bar-accel=` so F10 is not swallowed. No licence-198 failure/retry.
- One task-created disposable `Ccf437LaunchProof.cs`, staged only after lease acquisition, sets the Game-view size, logs paths and captures the Game image; normal scene/editor stop/exit uses Editor APIs. It does not generate a world, set player state, save/load, bypass startup or invoke UI callbacks. Source/helper and generated `.meta` removed after exit. A nine-name check covering #427 and #437 disposable scripts/metas finds none under Assets.
- Native X11 input (`xdotool`, actions/timestamps in `proof1/native-actions.jsonl`): Ctrl+P to Play; click **Start New Scenario**; Next to page 2; Back to page 1; Next to page 2; Next to page 3; Begin; one F5; Ctrl+P to leave Play Mode. Then normal `EditorApplication.Exit(0)` via the disposable operator helper. No Editor signal/kill, forced release, fixture load, time advance, harvest or gameplay exploration.
- All three intro pages legible in exact captures; Back/Next/Begin work. After Begin: recognisable textured ground, lit tree trunks/canopy/foliage, HUD, Year 0, Cash €12,000 and minimap. No black/blank intro or forest.
- Editor exit **0**, log contains `CCF437_NORMAL_EXIT` and normal subsystem shutdown; 0 C# compiler errors. This is not a zero-warning/clean-console certification: inherited compiler obsolescence warnings and ordinary licensing/Vulkan/Editor diagnostics remain in the log.
- Four actual drift paths were restored within the ten-path allowlist: `.vscode/settings.json` and the three Woodland_Forbs materials. Each restore used path-limited `git checkout --`; changed bytes and full diff retained at `proof1/drift/` and `Editor-drift.patch`. No unexpected dirty path remained. No host restore was performed.
- Release receipt says **`status: released`, `forced: false`, `unity_editor_pids: []`**. Final live check says FREE/no Editor.

The filename tracer outlived the already-finished launcher because it followed lingering non-Editor descendants. Its `-o PROG` mode ignored SIGTERM; after independently confirming Editor exited and lease FREE, I stopped **only our strace process** with SIGKILL (no `--kill-on-exit`, no Editor alive). The display driver then closed only its own openbox/Xephyr. This is separate from the Editor's normal exit 0 and launcher's original cleanup result 1; `trace-detach.json` preserves it. No Editor was killed and no lease forced.

### Exact-pixel rendered evidence

All files below are in `/media/jer/Files/CCF/Verification/D073-PersonalPlaytest-437-20261112/proof1/`. Captures are Game-frame PNGs at 1280×720, **not scaled crops of the 1920×1080 nested desktop**. All six below were visually inspected. The complete screenshot manifest also includes title/desktop opening-state captures; black desktop/Editor loading frames precede Play and are not intro/forest failures or accepted game captures.

| File | Pixels | SHA-256 |
|---|---|---|
| `forest-after-begin-1280x720.png` | 1280 × 720 | `f127bd423f2786fe0ec580b3b2f600bbbe61bfa0aa0bc78eed350c7fee0e9d96` |
| `forest-after-f5-1280x720.png` | 1280 × 720 | `a119402403f613e146dcda0e22609ae7cdb776968a0e499fe3a8f4a9006e31ef` |
| `intro-back-to-page1-1280x720.png` | 1280 × 720 | `dd04c1532cdd448bc7df4b789826197bd6cfd9f3b86cb6bf86c4e10d90e0679e` |
| `intro-page1-1280x720.png` | 1280 × 720 | `dd04c1532cdd448bc7df4b789826197bd6cfd9f3b86cb6bf86c4e10d90e0679e` |
| `intro-page2-1280x720.png` | 1280 × 720 | `2c45a37608ff2b8213250a97eefce85f716167a202e81db4b6ebc706fc47aeae` |
| `intro-page3-1280x720.png` | 1280 × 720 | `fbc7339ee5805a9815c75611e2953d32782c77d4cd52c5913380a47499376e6b` |

No new 1080p intro/forest run was requested or performed; inherited #427 provenance qualification at both sizes remains separate evidence. The proof Game view displayed at 0.44 scale within the nested desktop; the saved Game frames are exact 1280×720. Full-size viewing was used to assess text legibility.

## Launcher and learner instructions

**Final launcher:** `/media/jer/Files/CCF/Operations/PersonalPlaytest-D073/launch-personal-playtest.sh`  
**SHA-256:** `1d43336ea6830b6fb053ebc94b439a289e72da2bf1dcb4bd7ed1684b53623b29`  
**README:** `/media/jer/Files/CCF/Operations/PersonalPlaytest-D073/README.md`  
**README SHA-256:** `292dec6a539927731212413e8d977c2a171cb8b3604c1fb6a1981a66a420d764`

Plain Bash with existing host tools. It pins the exact branch/commit, checks the clean lane and 4,011 bytes, host37 hashes+complete inventory, all full-backup81 copies, mount/15 GiB, no Editor/FREE lock, existing entitlement and graphical desktop. It rejects profile symlinks, acquires the real lease, copies the private licence/config, launches interactively, waits even if the terminal receives a signal, and only cleans/releases after Editor exit. It restores only the ten authorised tracked drift paths, preserves/reports anything else and leaves receipt files. Its host baselines are immutable admissions: if valued data later changes legitimately, it stops for operator review instead of silently accepting a different baseline.

**Tests actually performed:** `bash -n`; original and final `--check-only` on real desktop `:0` PASS; final missing-DISPLAY refusal exit1 PASS; exact branch/hash/backup/inventory end-state rechecks; one Editor session described above. Final complete Editor launch after the small check-only/cleanup-inventory correction **NOT rerun**; this limitation is explicit. The final launch command/environment and all Unity ownership/restoration code are the same as the proved session.

**Before the human starts:** parent #412 must update/lint the physical Ops row for this candidate and named human owner, reconcile coordination, and obtain **Manager conditional GO for this exact candidate**. I did not grant GO or edit that board. This handoff fulfills worker preparation, not that final release gate.

1. Open a terminal on Predator after GO. Run:

   ```bash
   /media/jer/Files/CCF/Operations/PersonalPlaytest-D073/launch-personal-playtest.sh --check-only
   ```

   Expect `CHECK PASS` and the private save path above. If it says STOP, leave files alone and give the message to the operator.
2. Run:

   ```bash
   /media/jer/Files/CCF/Operations/PersonalPlaytest-D073/launch-personal-playtest.sh
   ```

   Keep the terminal open. Use this launcher every time; a direct Hub/project launch bypasses these safeguards.
3. In Unity click **Play**, **Start New Scenario**, read all three introduction pages, then **Begin**. Ground and trees should be visible. If the intro or forest is black/unreadable/blank, **STOP, exit Play Mode without saving, close Unity and report it**. Do not press F5 on a black screen.
4. The world is fresh and disposable. F5/F9 use only the private profile. For the broader personal test follow the #421 route: Objectives; Stand Map's five racks and western headland/southern feeder; field entrance marks; tree inspection/Crop/Fell; separate Work Plan Approve and Advance; Annual Review/history/RECORDED-vs-DERIVED; optional MFM/second intervention. This worker did not test that route and does not claim user acceptance.
5. To finish: exit Play Mode, then close Unity normally. Do not save scene/project edits. Let the terminal finish checking valued data/restoring known Editor drift/releasing the lease. It prints receipts. Share unexpected results; do not force-release, kill Unity, discard dirty files or copy private saves over host data.

## Manifests, hashes and final checks

Evidence root: `/media/jer/Files/CCF/Verification/D073-PersonalPlaytest-437-20261112/`. The 4,011-file method is the same SHA-256-by-relative-path method as #427: load its exact path-key list and hash the candidate lane's file bytes, emitting JSON in the same order. I additionally verified this set equals **all** tracked `.vscode/settings.json`, `Assets`, `Packages`, `ProjectSettings` paths (4,011), with no missing/extra path. `Tools` is not part of this manifest. The qualify branch's 16 verification-only additions disappear on the candidate as expected; inherited candidate Tools remain, and the separate qualified-source Tools diff is empty. There is no branch-dependent path mismatch hidden in the hash comparison.

`production-before.json` and `production-after.json` match #427's `production-before.json` file-for-file; host37 before/after match its `host-before.json`. Backup manifests include sizes/hashes only. `artifact-sha256.json` indexes local evidence and support files; report publication receipts are separate later files.

| Evidence file (relative to evidence root) | SHA-256 |
|---|---|
| `prelaunch.json` | `9d8784bb13dfb1f1e81e4fc5a51ff08e391682f264634ffde8e1f7055a70f31f` |
| `host-before.json` | `6de491bc580ce729e203baf1561d57afe53c550f0cce4770ecfec547a2acc712` |
| `host-after.json` | `6de491bc580ce729e203baf1561d57afe53c550f0cce4770ecfec547a2acc712` |
| `production-before.json` | `219704058261afe3f315d80756f44c60ea12ec3c802b39ec3f8162757a9eeff9` |
| `production-after.json` | `219704058261afe3f315d80756f44c60ea12ec3c802b39ec3f8162757a9eeff9` |
| `backup-originals.json` | `bd80976387ed0a0d7776d29c3a42f247345e8b2b88b3f23e5c8e91c790225271` |
| `backup-copies.json` | `bd80976387ed0a0d7776d29c3a42f247345e8b2b88b3f23e5c8e91c790225271` |
| `backup-result.json` | `232a0d5f837647885217a88fb9b8c3177b2353878b642a133f0eecbc1ec195a5` |
| `end-state.json` | `da1f4fb6176588f571c1aa0c3ce9969e4b7ee4aac9350f22aed395b56290c7ca` |
| `screenshot-manifest.json` | `4c0b85aaf0fccca8787a999fb5f1311e3bc76a73c2cc9aac3c1308beaa6f82e7` |
| `disposable-removal-check.json` | `116ab6f0a1ff3abd6b7c1e1ba942a31983c5fda68dc962ada7db52b5eb32b8a9` |
| `proof1/Editor.log` | `37af900f4c43cbf07fa4e468e3d24842ff0ca82c3ec91e595d716b4afcccba24` |
| `proof1/acquire.json` | `d515ed4299dda29f123bfd06ae410ef27c709a3b038e134df9ab8b72612b854d` |
| `proof1/release.json` | `b81627aa8a5102897e97e43f03649492687231545d3e9b011455d6a685b5e6cc` |
| `proof1/process-environment.json` | `8ce6d23a8dbe811497f3979c4bdd1367137a94c74d9b92a655055638033091de` |
| `proof1/private-save-prefs-access.txt` | `d8990a880b88492a563e1e496588c0e213b645874d59d019a401b296eb40abf7` |
| `proof1/host-game-write-syscalls.txt` | `e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855` |
| `proof1/f5-save-receipt.json` | `7bc91d3be2bebc8bfb1db2f90ae963878cf89fef17250b3fcd2dbebc77f11e21` |
| `proof1/launch-personal-playtest-executed.sh` | `77c3dba6e9e3533027812392031293d80b272bfc0b51dc4282adee64fe16a70c` |
| `launcher-after-proof.diff` | `55276a1ca57dd391f5b16c0211be1af98ea07289709467929b115e584bbd7072` |
| `check-only-final.txt` | `f4bb0a1da94645c48f79b46e5e3898ab21a2f91efe2f6693094976488c8aac59` |
| `check-only-no-display-refusal.txt` | `f4e737f7d338b5e1bb181b1ffb7cebfe330307120172a5f9ba5dd9342b377e57` |

Backup `originals.json` and `copies.json` SHA-256: `bd80976387ed0a0d7776d29c3a42f247345e8b2b88b3f23e5c8e91c790225271` (both identical). The actual executed launcher SHA is `77c3dba6e9e3533027812392031293d80b272bfc0b51dc4282adee64fe16a70c`; the final launcher SHA above differs only by the disclosed inventory-check correction (`launcher-after-proof.diff`).

End state rechecked: candidate clean at exact HEAD; no Editor; FREE lock; source 4,011 exact; valued37 exact; complete backup copies exact; profile empty; nine disposable names/metas absent; own nested display closed; physical board unmodified. Result docs worktree is left in place, clean after this report commit/push, for the parent to append its consolidated result.

## Not verified / remaining authority

- **Human personal playtest NOT STARTED/NOT ACCEPTED.** Physical human-owner registration and final Manager conditional GO remain parent/Manager work. No automatic dashboard/Drive ingestion or downstream sync was verified.
- No normal-desktop interactive Editor session: worker used private display; only final `--check-only` used real desktop. No final-byte complete launcher rerun after the inventory-check correction. Forced-signal/live-Editor refusal/low-disk/wrong-branch branches were inspected, not deliberately induced against the live lane.
- No native F9 load, F10 accessibility run, walking/harvesting/resource/construction/regeneration, year advancement, old-save/Reference runtime retest, Annual Review/D-073 interaction or long loop in this child. Their existing upstream evidence is inherited, with disclosed #381/#427 limits; not a new all-green Tier3 claim.
- No 1080p intro/forest proof, Player/Windows build, native Windows input/performance, main integration or external-demo release.
- The 37 game save/Prefs originals are unchanged; **one global Editor layout outside that set changed** and is left intact with its original backed up. No claim that the Editor made zero writes outside private XDG config.

## Appendix — full final launch script

The following is the complete final local script, SHA-256 `1d43336ea6830b6fb053ebc94b439a289e72da2bf1dcb4bd7ed1684b53623b29`. Its support manifests and learner README are beside it in Operations and copied into the evidence directory. The disposable proof source stays only in evidence and is never left under Assets.

```bash
#!/usr/bin/env bash
# Protected D-073 personal Editor lane. Run from a terminal; keep it open.
# No game source changes, builds, host-save writes or automatic save copying.
set -Eeuo pipefail
umask 077
readonly OPS=/media/jer/Files/CCF/Operations/PersonalPlaytest-D073
readonly LANE=/media/jer/ZX20/CCF-S1-ResearchCopy-V1-Qual
readonly BRANCH=integration/scenario-one-personal-playtest-d073-20261112-v1
readonly HEAD_SHA=55863f1b7719011bba16b3f3ab377e379c0b988a
readonly UNITY=/media/jer/ZX20/Unity/6000.6.0f1/Editor/Unity
readonly LOCK=/media/jer/ZX20/CCF-Ops/scripts/unity-lock
readonly PROFILE="$OPS/profile"
readonly CONFIG="$PROFILE/config"
readonly OWNER='USER personal playtest #412'
readonly TASK=CCF-437-D073-PERSONAL-PLAYTEST
readonly EVIDENCE=/media/jer/Files/CCF/Verification/D073-PersonalPlaytest-437-20261112
readonly STAGED="$LANE/Assets/ForestPrototype/Ccf437LaunchProof.cs"
check_only=0
proof=0
case "${1:-}" in
  '') ;;
  --check-only) check_only=1 ;;
  --proof) proof=1 ;; # Worker-only, same launch/profile/lease; private display supplied externally.
  *) echo 'Usage: launch-personal-playtest.sh [--check-only]' >&2; exit 2 ;;
esac
[[ $# -le 1 ]] || { echo 'Too many arguments' >&2; exit 2; }
fail() { echo "STOP: $*" >&2; exit 1; }
for program in git jq sha256sum findmnt df stat cp date cmp find sort; do
  command -v "$program" >/dev/null || fail "Missing command: $program"
done
[[ -x "$UNITY" && -x "$LOCK" ]] || fail 'Expected Editor or lock tool missing.'
[[ $(findmnt -n -o TARGET -T /media/jer/ZX20) == /media/jer/ZX20 ]] || fail 'ZX20 is not mounted at its expected path.'
[[ $(findmnt -n -o FSTYPE -T /media/jer/ZX20) == exfat ]] || fail 'Unexpected ZX20 filesystem.'
[[ $(df -B1 --output=avail /media/jer/ZX20 | tail -n1) -ge 16106127360 ]] || fail 'ZX20 has less than 15 GiB free.'
[[ $(git -C "$LANE" branch --show-current) == "$BRANCH" ]] || fail 'Lane is on a different branch.'
[[ $(git -C "$LANE" rev-parse HEAD) == "$HEAD_SHA" ]] || fail 'Candidate commit changed.'
[[ -z $(git -C "$LANE" status --porcelain=v1 -uall) ]] || fail 'Lane contains changes. Ask the operator; do not discard them.'
[[ ! -e "$STAGED" && ! -e "$STAGED.meta" ]] || fail 'Disposable proof helper is already present.'
sha256sum --check --status "$OPS/production4011.sha256" || fail 'Protected production bytes changed.'
sha256sum --check --status "$OPS/host37.sha256" || fail 'Valued host files differ from the protected backup baseline.'
sha256sum --check --status "$OPS/backup81.sha256" || fail 'Complete backup copy failed its hash check.'
# Unity's global Editor window layout uses real HOME even with private XDG config.
# It is backed up, but is not a game save/PlayerPrefs file. Protect the exact
# complete game-data inventory instead of rejecting harmless window-layout drift.
host_inventory() {
  find /home/jer/.config/unity3d/DefaultCompany \
    /home/jer/.var/app/com.unity.UnityHub/config/unity3d/DefaultCompany \
    /home/jer/.var/app/com.unity.UnityHub/config/unity3d/prefs -type f -print | LC_ALL=C sort
}
cmp -s "$OPS/host37-paths.txt" <(host_inventory) || fail 'Host game-data inventory changed (new/missing file).'
state=$("$LOCK" status --json)
[[ $(jq -r .status <<<"$state") == free && $(jq '.unity_editor_pids | length' <<<"$state") == 0 ]] || fail 'Unity lock held or another Editor running.'
[[ -n ${DISPLAY:-} ]] || fail 'No graphical desktop DISPLAY; use a terminal on Predator.'
[[ ! -L "$PROFILE" && ! -L "$CONFIG" ]] || fail 'Private profile/config must not be a symlink.'
[[ -d "$PROFILE" ]] || fail 'Private profile directory missing.'
# Refuse symlinks in this dedicated profile; no path can lead back into host data.
[[ -z $(find "$PROFILE" -type l -print -quit) ]] || fail 'Symlink found in private profile.'
licence_source=''
for candidate in /home/jer/.config/unity3d/Unity/licenses /home/jer/.var/app/com.unity.UnityHub/config/unity3d/Unity/licenses; do
  if [[ -f "$candidate/UnityEntitlementLicense.xml" ]]; then
    if [[ -z $licence_source || "$candidate/UnityEntitlementLicense.xml" -nt "$licence_source/UnityEntitlementLicense.xml" ]]; then
      licence_source=$candidate
    fi
  fi
done
[[ -n $licence_source ]] || fail 'No existing licensed Unity entitlement found.'
if (( proof )); then
  [[ ${DISPLAY:-} != :0 && ${DISPLAY:-} != :1 ]] || fail 'Proof requires a private nested display.'
  [[ ! -e "$EVIDENCE/proof1" ]] || fail 'Proof evidence already exists; no automatic retry.'
  sha256sum --check --status "$OPS/proof-helper.sha256" || fail 'Proof helper hash mismatch.'
fi
printf 'CHECK PASS\nCandidate: %s @ %s\nLane: %s\nPrivate config: %s\nExpected saves and PlayerPrefs: %s/unity3d/DefaultCompany/CCF/\nFull backup: %s\nEditor: %s\nDisplay: %s\n' \
  "$BRANCH" "$HEAD_SHA" "$LANE" "$CONFIG" "$CONFIG" \
  /media/jer/Files/CCF/Archives/PersonalPlaytest-D073-HostBackup-20261112 "$UNITY" "$DISPLAY"
echo 'The real paths were verified in the #437 proof; direct Hub launches bypass this protection.'
if (( check_only )); then exit 0; fi

run="$OPS/runs/$(date -u +%Y%m%dT%H%M%SZ)-$$"
if (( proof )); then run="$EVIDENCE/proof1"; fi
mkdir -p "$run"
lease=''
editor_pid=''
staged=0
licence_copied=0
cleanup() {
  local original_code=$? result=0 now
  trap - EXIT
  # If acquisition failed, we own nothing and must not restore any lane files.
  [[ -n $lease ]] || exit "$original_code"
  set +e
  # Never release another owner's lease, and never release while an Editor lives.
  if [[ -n $lease ]]; then
    now=$("$LOCK" status --json)
    if [[ $(jq '.unity_editor_pids | length' <<<"$now") != 0 ]]; then
      echo "STOP: Editor still running; lease $lease retained. Close Editor normally, then operator must release it." >&2
      exit 1
    fi
    if (( staged )); then rm -- "$STAGED"; [[ ! -f "$STAGED.meta" ]] || rm -- "$STAGED.meta"; fi
    if (( licence_copied )); then rm -r -- "$CONFIG/unity3d/Unity/licenses"; fi
  fi
  echo 'Valued host-file verification (all 37 must say OK):'
  cat "$OPS/host37.sha256"
  sha256sum --check "$OPS/host37.sha256" | tee "$run/host37-check.txt" || result=1
  host_inventory >"$run/host-game-inventory.txt"
  cmp -s "$OPS/host37-paths.txt" "$run/host-game-inventory.txt" || result=1
  sha256sum --check "$OPS/backup81.sha256" >"$run/backup81-check.txt" || result=1
  # Archive and restore only the ten explicitly authorised Editor-drift paths.
  # Any other dirty path is left alone and makes this run fail its clean gate.
  local path
  local -a allowed=(
    '.vscode/settings.json'
    'Assets/ForestPrototype/Art/SectionFive/Woodland_Forbs_01/Materials/Forb_Flower_Centre.mat'
    'Assets/ForestPrototype/Art/SectionFive/Woodland_Forbs_01/Materials/Forb_Leaves.mat'
    'Assets/ForestPrototype/Art/SectionFive/Woodland_Forbs_01/Materials/Forb_Stems.mat'
    'Assets/ForestPrototype/Art/SectionFive/Bilberry_Type_Cover_01/Materials/Bilberry_Leaves.mat'
    'Assets/ForestPrototype/Art/SectionFive/Bilberry_Type_Cover_01/Materials/Bilberry_Stems.mat'
    'Assets/ForestPrototype/Art/SectionFive/Deadwood_Fungi_01/Materials/Bracket_Cap.mat'
    'Assets/ForestPrototype/Art/SectionFive/Deadwood_Fungi_01/Materials/Bracket_Pores.mat'
    'Assets/ForestPrototype/Art/SectionFive/Deadwood_Fungi_01/Materials/Fungi_Cap.mat'
    'Assets/ForestPrototype/Art/SectionFive/Deadwood_Fungi_01/Materials/Fungi_Underside.mat'
  )
  git -C "$LANE" diff --binary >"$run/Editor-drift.patch"
  for path in "${allowed[@]}"; do
    if ! git -C "$LANE" diff --quiet -- "$path"; then
      mkdir -p "$run/drift/$(dirname "$path")"
      [[ ! -f "$LANE/$path" ]] || cp -p -- "$LANE/$path" "$run/drift/$path"
      git -C "$LANE" checkout -- "$path" || result=1
      echo "Restored known Editor drift: $path"
    fi
  done
  git -C "$LANE" status --short | tee "$run/lane-status.txt"
  [[ -z $(git -C "$LANE" status --porcelain=v1 -uall) ]] || { echo 'STOP: unexpected lane changes preserved; contact operator.' >&2; result=1; }
  sha256sum --check --status "$OPS/production4011.sha256" || result=1
  # Retain ownership through restoration/hash checks, release even if a check fails.
  "$LOCK" release --owner "$OWNER" --task-id "$TASK" --lock-id "$lease" \
    --reason 'Owned Editor exited; cleanup checked, normal release by guarded personal launcher.' --json >"$run/release.json" || result=1
  cat "$run/release.json"
  "$LOCK" status --json >"$run/final-lock.json"
  printf 'editor_exit=%s\ncleanup_result=%s\nlease=%s\n' "$original_code" "$result" "$lease" | tee "$run/result.txt"
  echo "Receipts: $run"
  (( original_code == 0 && result == 0 )) || exit 1
  exit 0
}
trap cleanup EXIT
# Signals do not kill Unity or release a live lease. Keep waiting for normal close.
trap 'echo "Close the Unity Editor normally; this terminal must wait before releasing the lease." >&2' INT TERM HUP
"$LOCK" acquire --owner "$OWNER" --task-id "$TASK" --lane "$LANE" --branch "$BRANCH" --head "$HEAD_SHA" \
  --eta "$(date -u -d '+4 hours' --iso-8601=seconds)" \
  --notes 'User-owned isolated D073 personal test, #437 child of #412; no Player build; host backup verified.' \
  --json >"$run/acquire.json"
lease=$(jq -er '.metadata.lock_id' "$run/acquire.json")
cat "$run/acquire.json"
mkdir -p "$CONFIG/gtk-3.0" "$CONFIG/unity3d/Unity" /tmp/ccf437
printf '[Settings]\ngtk-menu-bar-accel=\n' >"$CONFIG/gtk-3.0/settings.ini"
[[ ! -e "$CONFIG/unity3d/Unity/licenses" ]] || fail 'Private licence copy unexpectedly remains.'
cp -a -- "$licence_source" "$CONFIG/unity3d/Unity/licenses"
licence_copied=1
args=("$UNITY" -projectPath "$LANE" -openfile "$LANE/Assets/Scenes/ForestTest.unity" -job-worker-count 2 -logFile "$run/Editor.log")
if (( proof )); then
  cp -p -- "$EVIDENCE/Ccf437LaunchProof.cs" "$STAGED"
  staged=1
  export CCF437_PROOF_DIR="$run"
else
  unset CCF437_PROOF_DIR
fi
# Match #427's successful licensing/config isolation. HOME remains the real home;
# actual persistentDataPath and prefs were proved inside private XDG_CONFIG_HOME.
# No direct-entry/test startup overrides: the normal title -> three-page intro.
env -u TMP -u TEMP -u XDG_CACHE_HOME -u CCF_TEST_DIRECT_ENTRY -u CCF_STARTUP_SCREEN \
  XDG_CONFIG_HOME="$CONFIG" TMPDIR=/tmp/ccf437 "${args[@]}" &
editor_pid=$!
printf '%s\n' "$editor_pid" >"$run/unity.pid"
echo 'Use Play in Unity. Start New Scenario, read all three pages, then Begin.'
echo 'If intro or forest is black/blank: STOP, exit Play Mode without saving, close Unity.'
echo 'To finish: exit Play Mode, then close the Unity Editor; keep this terminal open.'
editor_code=0
while true; do
  wait "$editor_pid" && editor_code=0 || editor_code=$?
  kill -0 "$editor_pid" 2>/dev/null || break
done
if (( editor_code == 198 )); then echo 'STOP: Unity licence exit 198. No retry.' >&2; fi
exit "$editor_code"
```
