# Sen-1 #741-A — merged installed README projection and source/current-live trace matrix (RESULT)

**STATUS: DOCUMENTATION ONLY. NOTHING DEPLOYED. NOT REVIEWED.** This is a subworker result for claim [#742](https://github.com/jermur788/CCF/issues/742) under #741. It proposes README bytes for a future five-file projection. It installs nothing, edits no live Ops file, and is not an approval. The independent focused delta review required by Manager #731 (projection decision §2 item 5) has not happened.

## 1. Actor and scope

| Field | Value |
|---|---|
| Model / harness | Anthropic Claude Sonnet 5.5 (`claude-sonnet-5-5`), Claude Code / Agent SDK in T3 Code, host `jer-laptop`; session id `e9ecca11-9365-4b16-ae16-03a01e7d2009` (taken from the session scratchpad path) |
| Branch / start | `docs/sen-1-741-a-merged-readme-projection-20261118-v1` @ `e8e84027faa45d42316cd01bda34ad36393fd069` |
| Worktree | `/media/jer/Files/CCF/Operations/CCF-741-A-MergedReadme-20261118` |
| Scratch | `/media/jer/Files/CCF/Verification/Sen1-741-A-MergedReadme-20261118/README.md` (pure proposed bytes) |
| Files written | the three new files in the #742 allowlist, and the scratch file above. No other write. |
| Read-only inputs | live `/media/jer/ZX20/CCF-Ops` (README, `environment.sh`, `lanes.json`, `deployment.json`, `roles/unity-operator.md`, `scripts/` listing); source worktree `/media/jer/Files/CCF/Operations/CCF-OpsReadiness-652-20261116` (observed HEAD `4c7acc051e262c3fe301049b51ee7fc48a9f99a2`, clean); Manager documents on `docs/sen-1-731-chas-manager-intake-20261118-v1` @ `6667669d9aa8d800641b9671228c1253a6c20f65`, read through the GitHub API |
| Not done | no `unity-lock`, `lane-status`, `packet-lint`, `verify-deployment`, self-test or Unity run; no board, lanes, lock-state or `deployment.json` write |

## 2. Inputs and outputs (SHA-256)

| Item | Path / locator | SHA-256 |
|---|---|---|
| Baseline: live README (re-hashed at start and end of this task, unchanged) | `/media/jer/ZX20/CCF-Ops/README.md` | `249be391e172e9d80f7d3d3419af961850b84c40fb2f15ba7b1d0469649eb517` |
| Additions: reviewed D-091 source README | `…/CCF-OpsReadiness-652-20261116/CCF-Ops/README.md` | `a1911b01ece918d4eb52bfde230fe197e34f839a8c41d79f67555d9a560598e4` |
| **Proposed merged installed README (pure bytes, 93 lines, 16 424 bytes)** | scratch `README.md`; also the part of the attachment after `<!-- BEGIN-PROPOSED-README-BYTES -->` | **`bde3c6cf286b79685d51e5a4f2882fede49a57cc3475d1b6d8f99320ccdd8881`** |
| Attachment 1: NOT-DEPLOYED README (banner + marker + pure bytes) | `attachments/Sen-1-741-A-MERGED-INSTALLED-README.proposed-NOT-DEPLOYED.md` | `ccfbf11f3a2cf3d3be00a011a1d36b205b53f12071d7ea9e7638c79633e20fe2` |
| Attachment 3: merge decisions | `attachments/Sen-1-741-A-README-MERGE-DECISIONS-20261118.md` | `9b6f9d43d6ae2eb0bfd8ad1ebf3338649a870e43e640ccc42fd875681f53d920` |
| This result document | this file | reported in the #742 `[HANDED-OFF]` comment (a file cannot contain its own hash) |
| Live `environment.sh` (retained) | `/media/jer/ZX20/CCF-Ops/environment.sh` | `3775cba7ed446c61ae6fa2977da5cf0984ff7d888213ad9fa33396b1aa11c954` |
| Live `roles/unity-operator.md` (old; replaced in the projection) | `/media/jer/ZX20/CCF-Ops/roles/unity-operator.md` | `120f1deb006aa56a9d4b9643917f855548b5da80432a7166442ecc6163eee815` |
| Source `roles/unity-operator.md` (D-091, equals the Manager's pinned hash) | source worktree | `e44e7eb89d33a82d4e3dc5987572565e394d12a60ec82733d3dfd4bb103206db` |
| Source `scripts/_ccf.py` (Option A, equals the Manager's pinned hash) | source worktree | `24a321466e6dae61ba950c4cbc4f918e12f7d6be8a6af628341c5aa8efe0b435` |
| Source `DeploymentProvenance.md` (source-only) | source worktree | `a08fb8a5d38e82ab3dad1b6b1e0df7280cf28e1751fd1359b61cc2947b288db4` |
| Source `scripts/verify-deployment` (source-only) | source worktree | `ee001216fb020ae33ed54f4c317e2c8e19ce4bdb407f4e6bb8ae457248a02386` |
| Manager projection decision packet | `Sen-1-731-652-DEPLOYMENT-PROJECTION-DECISION-AND-STAGED-REHEARSAL-PACKET-20261118.md` @ `6667669d…` | `926e7bda7badb9c10e3100487e625b006c6e6dba5f09667183c1e540f28601ba` |
| #741 orchestration packet | `Sen-1-741-DEEPSEEK-ONE-CYCLE-OPS-DEPLOYMENT-AND-COMPACT-UNITY-READINESS-ORCHESTRATION-20261118.md` @ `6667669d…` | `d1aad79d28c4dac075b6ff9280384148fd39e347f7a1c0faed3ecbf18344d2f2` |
| #652 preflight result (read at blob `8506db60…`) | `Sen-1-652-D091-LIVE-DEPLOYMENT-PREFLIGHT-AND-CANDIDATE-PROJECTION-RESULT-20261118.md` | `427ccc3ed7c7d445338366f9eda41f9e16cac94a0f62b234ea13598d240c257d` |

Both README input hashes equal the values in the #742 brief. The observed source HEAD (`4c7acc05`) differs from the `4d03f5b5` named in the preflight; the README bytes still match the pinned hash and I did not investigate the head difference (observation O-3).

## 3. Method

1. Hashed both inputs and compared them with the pinned values.
2. Built the proposed file mechanically from the live bytes by exact-string substitution (each substitution asserted to match the expected number of times), so nothing in the live baseline changed by accident. The build script lives only in my session scratchpad. The result is reproducible: starting from the live README, apply D-10, D-12, D-13, D-15, D-16/17 and D-19–21 of the decision log.
3. Compared the proposed file with live by line and by sentence. A line-level diff shows exactly four replaced live lines (24, 38, 47, 53), the insertion of two paragraphs after line 53, and an appended block at the end. All other 76 live lines are byte-identical.
4. Checked each source sentence absent from the proposal and each live sentence absent from the proposal (§5).

## 4. Trace matrix (per proposed line)

Legend: **LIVE** = identical to live baseline bytes; **LIVE+EDIT** = live line with the stated edit; **LIVE+SRC** = live line with sentences taken verbatim from the D-091 source; **SRC** = verbatim from D-091 source; **NEW** = new wording for this projection. Blank lines are omitted. Proposed line = live line for lines 1–53; from the Post-integration heading on, proposed line = live line + 4. Decision IDs are in `attachments/Sen-1-741-A-README-MERGE-DECISIONS-20261118.md`.

| Proposed line(s) | Content | Provenance | Decision |
|---|---|---|---|
| 1 | Title `CCF-Ops foundation — G4 corrections` | LIVE (= source) | D-23 |
| 3 | Versioned source, live deployment, explicit-copy rules (keeps the `CCF-NonUnity` source worktree and `deployment.json` wording) | LIVE (source L-3 differs and is not used) | D-05, D-22 |
| 5 | Persistent lanes / disposable threads / role chain | LIVE (= source) | D-23 |
| 7 | `## Storage and lanes` | LIVE | D-23 |
| 9 | ZX20 UUID `18D6-896E`; **persistent `/dev/disk/by-id` path compared by block-device identity**; **duplicate visible UUID rejection** | LIVE (source regresses both) | D-01, D-02 |
| 11–14, 16 | U1/U2/Integration/NonUnity lane list and Unity flags | LIVE (= source) | D-23 |
| 18 | Canonical repository moved 2026-10-21, old ZX20 store removed, not a backup, in-flight work, bundles | LIVE (source S-18 is a reduced rewrite) | D-06 |
| 20 | Distinct branch per lane, no per-task worktree creation, no Library copying; durable `Builds`/`Archives` and **`/media/jer/Files/CCF/Verification`**; scratch `ZX20/CCF-Temp`; StorageArchitecture pointer | LIVE (source S-20 is a rewrite) | D-07, D-08, D-09 |
| 22 | `## Dual Git stores and assignment` | LIVE | D-23 |
| 24 | Dual-store assignment rule, `lane-status --preflight` command, migration note, detached-candidate rule | **LIVE+EDIT**: both `/home/jer/CCF/.git` → `/media/jer/Files/CCF-Legacy/.git` (matches live `lanes.json` `git_stores[1]`) | D-10, D-11 |
| 26–36 | `## Commands` block | LIVE (= source) | D-23 |
| 38 | `lane-status` semantics and exit codes | **LIVE+SRC**: live sentences retained; source Option A block (five sentences from "Configuration rejects arbitrary roots…" to "…remain ZX20-only.") inserted after the live "Off-storage non-Unity lanes…STOP warning." sentence | D-12 |
| 40 | `packet-lint` semantics | LIVE (= source) | D-23 |
| 42, 44 | `## Machine-wide Unity ownership`, state path/flock paragraph | LIVE (= source) | D-23 |
| 46, 48–51 | `unity-lock` example block except line 47; `--pid` comment (48); release and force-release examples | LIVE (= source) | D-14, D-23 |
| 47 | Acquire example | **LIVE+EDIT**: `--eta FUTURE_ISO_TIMESTAMP_WITH_TIMEZONE` → `--no-expiry` (= source S-47) | D-13 |
| 53 | Acquire fail-closed rules, release, force, guard | **LIVE+SRC**: live text; the clause "`ID; force requires prior`" is replaced by the source's release-refusal sentences (refuses while any Editor or recorded PID exists incl. reused PID; unknown PID needs worker-stopped verification) followed by "Force requires prior…" | D-15 |
| 55 | Owner D-091 custody paragraph (no-expiry, `eta:null`+`no_expiry:true`, legacy ETA readable, advisory overdue, no auto transfer) | SRC verbatim | D-16 |
| 57 | Safe handoff paragraph (event-driven, `HANDED-OFF`/`RELEASED` in receipts, `DONE-PENDING-LEDGER`, never migrate held lock JSON, old-script rollback rule) | SRC verbatim | D-17 |
| 59, 61 | `## Post-integration Unity gates` and paragraph | LIVE (= source) | D-23 |
| 63, 65, 67 | Board, ledger, authority transition | LIVE (= source) | D-23 |
| 69, 71, 73–77, 79 | Route identity placeholders | LIVE (= source) | D-23 |
| 81, 83 | `## Offline checks / pilot`; self-test paragraph ending "Full receipt belongs in `/media/jer/Files/CCF/Verification/WorkflowFoundation`." | LIVE (source S-83 would substitute the retired ZX20-side spelling of the receipt folder) | D-03, D-04 |
| 85 | Reviewer A/B/C paragraph | LIVE (= source) | D-23 |
| 87 | `## Installed deployment projection` | NEW | D-19 |
| 89 | Deployment-only document; not the reviewed source README; provenance authority = `deployment.json` | NEW | D-19 |
| 91 | `DeploymentProvenance.md` and `scripts/verify-deployment` source-only and not installed; five-file list; `environment.sh`/`AGENTS.md` installed-only overrides; never copy source `deployment.json`/`lanes.json`/`board/`/`ledger/`/`state/` | NEW | D-20 |
| 93 | Option A authority sentence reconciled with the uninstalled `DeploymentProvenance.md` | NEW wording around the SRC authority-document name and the source clause "Unity admission, physical lock state and writable preflight remain verified ZX20-only" | D-21 |

## 5. Deliberate exclusions

Source sentences left out (all have a decision row):

| Source text (S-line) | Why left out | Decision |
|---|---|---|
| S-3 "verify source branch/HEAD against the deployment receipt before updating" | Live wording is the baseline and is more specific | D-22 |
| S-9 "stores device, UUID…", dropping the unique-visible-UUID clause | Regression of live facts | D-01, D-02 |
| S-18 shortened canonical-repository sentence | Loses the in-flight-work warning | D-06 |
| S-20 "Durable builds, archives and verification output belong under …{Builds,Archives,Verification}… environment.sh checks scratch and bounded durable output directories…" plus one further sentence telling readers not to recreate the retired ZX20 build/verification directories | The `environment.sh` claim describes a file that is not installed (P-2); the rest duplicates live L-20 | D-07, D-08, D-09 |
| S-24 shortened Dual Git stores paragraph | Loses the assignment rule and preflight command; its safety statements are carried by M-38 and M-93 | D-11 |
| S-83 sentence naming the retired ZX20-side spelling of the WorkflowFoundation receipt folder | Retired path | D-04 |
| S-87 sentences that say `DeploymentProvenance.md` is "retained unchanged" without saying it is absent from installed Ops | Would imply an installed file | D-21 |

Live sentences not present in the proposal: only the four replaced by an adopted source or correction: the two `/home/jer/CCF/.git` sentences (path corrected), the `--eta` acquire example line, and the "Normal release needs owner+task+lock ID; force requires prior…" sentence (expanded). No other live sentence was dropped.

Not added although available: the #652 preflight §7 sentence that the recorded `--pid` must be the real Editor PID, never a launcher or terminal-shell PID (D-18; see R-3).

## 6. Checks run on the proposed bytes

| Check | Result |
|---|---|
| Extraction of the proposed bytes from the attachment (`sed '1,/^<!-- BEGIN-PROPOSED-README-BYTES -->$/d' … \| sha256sum`) | `bde3c6cf…8881`, equal to the scratch file |
| `grep -E` over the proposed file for the two retired ZX20 directory names (written with bracket classes such as `CCF-V[e]rification` in the real command, so this document does not itself contain them), `/home/jer/CCF`, `installed on`, `has been installed/deployed`, `was installed/deployed`, `Do not recreate` and `Builds/V[e]rification` | exit 1, no match |
| Positive checks (read back) | `by-id` wording present; "duplicates are rejected" present; `/media/jer/Files/CCF/Verification/WorkflowFoundation` present; `/media/jer/Files/CCF-Legacy/.git` twice; `--no-expiry` present; release-refusal sentences present; D-091 custody and handoff paragraphs present; Option A block present |
| Live Ops has no `DeploymentProvenance.md` and no `scripts/verify-deployment` | confirmed by `ls` (no such file) |
| Live `lanes.json` legacy store | `git_stores[1].git_dir = /media/jer/Files/CCF-Legacy/.git`; `storage.device` is a `/dev/disk/by-id/…-part2` path |
| Text cross-check of the candidate `_ccf.py` and `unity-lock` for the Option A claims | present as code (`on_durable_ccf`, Files-CCF-tree error, escape check, "not Unity-capable" rejection before creating state/guard). **Not executed.** |

## 7. Observations and residual risks

| ID | Item |
|---|---|
| O-1 | Live L-20 still says "the non-Unity lane is on the Files HDD" and "No per-task worktree creation", while live `lanes.json` now lists 23 lanes including many per-task non-Unity lanes on Files. I kept the live sentence because the brief says to keep live statements and the Manager asked for minimal additions. A reviewer may want it brought up to date; that is a separate decision. |
| O-2 | The Manager's brief says no retired ZX20 build/verification directory names may appear in the README, and the proposed README is clean. But the D-091 `roles/unity-operator.md` (`e44e7eb8…`, one of the five files to be installed) and the live copy both direct receipts/logs and Player builds/staging/archives to the retired ZX20 verification and builds directories (line 7). Several other installed documents (for example `G4BCorrections.md`, `WorkflowFoundationVerification.md`, `pilot/README.md`, ledger files, `board/active.md`) contain the same retired names. I did not touch these and the Manager froze the role file as reviewed. This is a conflict between the README's accurate paths and an installed role file. |
| O-3 | Source worktree HEAD observed `4c7acc05`, not `4d03f5b5` named in the preflight. README hash matches the pin; the other pinned hashes I rechecked (`_ccf.py`, `unity-operator.md`) also match. |
| O-4 | The README keeps live line 3's claim that `/media/jer/Files/CCF/Operations/CCF-NonUnity` is the versioned-source worktree. I only confirmed it is a registered non-Unity lane in `lanes.json`; I did not check its branch or HEAD, or whether `deployment.json` (`source_branch ops/workflow-foundation-v1`) still points to it. |
| R-1 | The Option A sentences (M-38) and "verified ZX20-only" (M-93) are carried from the reviewed source README. I did not run `lane-status`, `unity-lock`, the 233-check self-test or `verify-deployment`. The statement matches the code only by text inspection. |
| R-2 | Lines 87–93 are new bytes and need the focused independent delta review the Manager requires. The reviewer should be a different session from me and from the custodians. |
| R-3 | D-18: the real-Editor-PID rule from preflight §7 is not in the proposal. If the reviewer or Manager wants it, it is one added sentence in M-53 or M-55; the hash will change. |
| R-4 | The README does not mention the `deployment.json` regeneration or the date of install, by design. The prospective receipt (task B) must be generated against the exact proposed hash above, or against any later hash if bytes change after review. |
| R-5 | Dashboard and Drive mirrors were not inspected. |

## 8. Handoff

Proposed README hash to pin in task B: `bde3c6cf286b79685d51e5a4f2882fede49a57cc3475d1b6d8f99320ccdd8881`. Commit, blob URLs and result-document hash are in the #742 `[HANDED-OFF]` comment. No live Ops, board, lanes, state, Unity, game, Compact30 or Windows action was taken.
