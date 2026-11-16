# Manager continuation handoff: CCF #611 Windows Phase-0

**Date:** 16 November 2026. **Status:** BLOCKED / UNQUALIFIED; #611 remains OPEN. **Manager action:** choose the next execution route; do not repeat the resolved setup/licensing investigation.

**Publication and re-verification (2026-11-16T17:24Z, this commit).** This Markdown was drafted by OpenAI `gpt-6.1-sol`, High reasoning, Codex harness in T3 Code, in the run that ended on a provider usage limit before it could publish. It is published here, substantively unchanged, by opencode `deepseek-v4.1-flash` (T3 Code) on the same machine `DESKTOP-DL1BQI7`, which re-checked live state first: Smart App Control still `VerifiedAndReputablePolicyState=1` and `SmartAppControlState=On`; a fresh direct assembly load of `Bee.Stevedore.Program.dll` still fails with `0x800711C7`; no `Unity` process running; physical lease guard `0` (free, last event `normal-release` 16:37:18Z); experiment worktree clean at `e994f0ae4e96cff9693180bd8026ed4b25abb96d`; `#611` OPEN, `#621` CLOSED; `origin/main` `37cd047ed39fd649563f70602d458e2970f6780f`. Nothing was installed, imported or launched to produce this publication; no production asset, scene, package file or save was touched, and no captures exist.

## Paste into the manager chat

Please resume ownership coordination for Scenario One #611. The owner authorised a one-off local Windows material experiment, explicitly accepted installed Unity 6000.6.9f1 for this task, authorised minimal Windows Ops/worktree/profile setup, and confirmed Unity Personal eligibility and acceptance of its terms. Those approvals supersede #621's earlier read-only restrictions only within this narrow task. The original interim D-032 allowance remains local-only; it is not general asset/production approval.

The worker implemented and tested task-local Windows Ops (35 PASS / 0 FAIL), registered an exclusive worktree and separate Library, created an isolated standard Windows account, completed official Unity sign-in and Personal activation, and repeatedly verified all 41 v2 source texture hashes. These prerequisites no longer block #611.

The remaining blocker is Windows Smart App Control. Installed Unity 6000.6.9f1 cannot load its unsigned `Bee.Stevedore.Program.dll`; the Editor exits before the material harness compiles or executes. No native captures exist and the material remains UNQUALIFIED. Smart App Control is still On. No security settings have been changed. The Unity lease is free and no native Editor is running.

The owner asked whether another solution exists instead of downloading Unity 6000.6.0f1. No route has subsequently been selected. Do not treat that question as permission to install the older Editor or disable security. Options are: explicitly approve a system-wide Smart App Control change after checking restoration support; move the material inspection to Linux under its normal admission and any required hash-verified source-transfer authorisation; try the original official 6000.6.0f1 installer alongside the current Editor after approval, with no guarantee of success; or wait for a supported Unity fix. Microsoft documents no individual-app Smart App Control exception.

Preserve the prepared Windows setup and all evidence. Keep #611 open until the native material experiment and required evidence are complete. #621 is already CLOSED as a historical read-only feasibility result. Update the manager-owned live coordination record to reflect resolved prerequisites and the current OS blocker; its older row still describes superseded pre-authorisation gates. Production assets/scenes/saves, historical worlds, asset commits and Windows Player builds remain prohibited.

## Registration and authority

- Handoff drafter: OpenAI `gpt-6.1-sol`, High reasoning, Codex harness / T3 Code. Publisher of this branch: opencode `deepseek-v4.1-flash`, T3 Code. Machine: `DESKTOP-DL1BQI7` for both.
- Documentation-only publication branch: `docs/sen-1-611-windows-manager-handoff-20261116-v1`.
- Documentation starting HEAD: `37cd047ed39fd649563f70602d458e2970f6780f` (current `origin/main` at publication); this branch adds only this Markdown. Its final commit is recorded in the publication receipt and GitHub commit history.
- Previous detailed result: [continuation result at exact commit e060ba9](https://github.com/jermur788/CCF/blob/e060ba931c0618c0a88e71012f88405831cc2c10/Docs/Handoffs/ScenarioOne-Graphics/Sen-1-611-WINDOWS-NATIVE-URP-PHASE0-CONTINUATION-RESULT-20261116.md). Earlier stop history remains preserved there.
- Primary checkout: `C:\Users\hp\Documents\GitHub\CCF`, local `main` at `04defad6f28c33156cc986dd0685dcb1998073a4`, 18 commits behind live `origin/main` `37cd047ed39fd649563f70602d458e2970f6780f` at publication; the checkout was left untouched and this branch was published from a separate task-local worktree.
- Experiment branch: `experiment/sen-1-611-compact-wornearth-urp-phase0-20261116-v1`, exact baseline/HEAD `e994f0ae4e96cff9693180bd8026ed4b25abb96d`; no implementation commits or repinning.
- [Original owner-approved packet](https://github.com/jermur788/CCF/blob/docs/sen-1-611-owner-approved-phase0-executable-20261116-v1/Docs/Handoffs/ScenarioOne-Graphics/Sen-1-611-OWNER-APPROVED-LOCAL-ONLY-UNITY-URP-PHASE0-EXECUTION-PACKET-20261116.md) and [Windows continuation packet](https://github.com/jermur788/CCF/blob/docs/sen-1-611-windows-editor-installed-continuation-20261116-v1/Docs/Handoffs/ScenarioOne-Graphics/Sen-1-611-WINDOWS-UNITY-EDITOR-INSTALLED-LOCAL-PHASE0-CONTINUATION-20261116.md), as amended by the owner's direct approvals above.
- Live `main` coordination blob fetched for this handoff: `4365a601d3f976e932e33353ee5d9c2cd1beaeb3`. #424 owns that record; this worker did not edit it. Dashboard/Drive synchronisation remains NOT VERIFIED.

## What exists locally

Abbreviate `C:\Users\Public\Documents\CCF-Phase0-611-Windows` as **R**.

| Item | Verified state |
| --- | --- |
| Registered worktree | `R\Worktrees\CCF-Sen-1-611-CompactWornEarth-URP-Phase0`; clean tracked status rechecked for this handoff |
| Physical task Ops | `R\Ops\board\active.md`, `lanes.json`, `windows_ops.py`, native lock/history under `state\unity`; 35 qualification checks passed |
| Isolated account | Standard, nonadministrator `CCFPhase0611`, profile `C:\Users\CCFPhase0611`; authenticated/licensed separately |
| Isolated project | Ignored disposable `LocalPreview611` within the registered worktree, short alias `R\Preview`, its own Library; no production game scripts |
| Editor / URP | Installed Unity 6000.6.9f1; disposable project resolved URP 17.6.0 |
| Source provenance | 41/41 v2 hashes verified before actual runs; manifest SHA256 `ecc97ca6580c5477eab23cc706464c80316da9d97d67e783a0f53cbe53615673` |
| Harness | `R\Ops\MaterialExperiment.cs`, copied only into the ignored project; prepared, not compiled/executed |
| Current admission | Lease free, metadata null, no native Editors; rechecked for this handoff |

This Windows Ops implementation is owner-authorised and worker-tested for this task. It has not received independent qualification or permanent shared Ops acceptance. Credentials/licence tokens are private and must not be published or copied into the manager chat.

## Exact failure and comparability limits

Blocked file: `C:\Program Files\Unity\Hub\Editor\6000.6.9f1\Editor\Data\Tools\BuildPipeline\Bee.Stevedore.Program.dll`.

- SHA256: `379f5d3265c951fb06b2a04f1a544a1d8aa7e60345d31d00394de7f1f71f5a47`; 125,952 bytes; no Authenticode signature.
- Native log: `System.IO.FileLoadException`, `An Application Control policy has blocked this file. (0x800711C7)`.
- Code Integrity events 3033/3077/3118 corroborate the load denial; Smart App Control reports On.
- The signed Editor executable does not establish a signature for every bundled DLL. No Zone.Identifier exists on this DLL, so `Unblock-File` does not address this failure. No trust/signature/policy bypass was attempted.
- [Microsoft FAQ](https://support.microsoft.com/en-us/windows/security/threat-malware-protection/smart-app-control-frequently-asked-questions): no individual-app bypass. The current FAQ says recent Windows updates allow enabling Smart App Control without a clean install, **if available for the device**. Restoration on this PC has not been demonstrated; do not promise it before verification or infer permission to disable it.

An earlier licensed attempt opened the full isolated CCF checkout and Unity automatically changed its two package files. The worker stopped only its owned Editor, retained the package diff/copies, and restored the two attributed files from exact backups. A disposable standalone project was then prepared with copied CCF render/quality settings and minimal URP dependencies to avoid repeating dependency migration. This provides a prospective material-only comparison, not full CCF integration qualification. No production checkout packages were changed.

## Verification, preservation and missing gates

The last complete custody checkpoint, `2026-11-16T16:41:55.488048Z`, verified **5,862 tracked files unchanged**, both Git statuses clean, hp EditorPrefs unchanged, and hp CCF save directory still absent. Clean tracked statuses were checked again for this handoff. Source maps remain outside production. Existing worlds and shared materials were not intentionally edited. Only ignored experiment files and external task-local setup/evidence were created; no texture asset commits, gameplay runs or Player build occurred.

Evidence root: `R\Evidence`. Key records: `ops-qualification.json`, `profile-custody.json`, `license-activation-outcome.json`, `source-hashes-before.json`, `protection-checkpoint.json`, `isolated-package-upgrade.diff`, `unity-smart-app-control-failure.log`, `native-smart-app-control-failure.json`, `smart-app-control-events.json`, lease receipts and `setup-and-stop-evidence-sha256.json`. Raw logs/screenshots remain local; this handoff is not an uploaded evidence archive.

The intended local harness covers five maps, normal GL/Y+ handling, a deliberately flipped-green negative control, normal/height off/on comparisons, three camera views and 720p/1080p: **18 planned PNGs, zero produced**. Once execution is possible, compilation/API correctness, actual rendering, visual defects/repetition/colour cast, controls, final source/custody checks and Manager acceptance still require verification. The final native verdict must be SUITABLE FOR LATER DESIGN REVIEW, NEEDS CHANGES or UNQUALIFIED; it is not production approval.

## Manager next action

1. Read the linked detailed report and preserve its history. Record setup/licence resolved, Smart App Control blocked, no active lease, zero captures; keep #611 OPEN.
2. Obtain the owner's execution-route choice. No new Editor installation or security reduction is authorised by the latest question. A Linux continuation needs the applicable host/lease/source availability and transfer gates; do not transfer files on assumption.
3. If Windows execution is authorised and made possible, revalidate actual model/branch/HEAD, 41 source hashes, task user, own Library, clean lane, free disk and exclusive lease before any Editor launch/import. Preserve the original packets' remaining gates. Diagnose/fix only ignored experiment code if necessary.
4. Capture and inspect the native evidence, verify controls and custody, release the lease after Editor exit, publish a dated GitHub result, then request/record the appropriate Manager verdict. Do not claim integration or close the task merely because Unity launches.

**Assessment:** Ready for Manager triage; native material acceptance NOT READY. No production implementation change is proposed by this handoff.
