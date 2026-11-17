# Sen-1 #672 — Compact30 complete-world content: signature candidate and owner-gate register

**CANDIDATE / UNSIGNED / NOT A SIGNED WORLD / NOT UNITY-VERIFIED / NO IMPLEMENTATION AUTHORITY.**
**Stage W1 author document only. Verdict: SIGNATURE-BLOCKED.** No field, cell or world in this document is signed. No authorised content-owner attestation exists for it.

| Identity | Value |
| --- | --- |
| Date | 2026-11-17 (UTC). Raw-byte hash reads observed at 2026-11-17T09:01:16Z. |
| Author | Anthropic Claude Opus 5.5 (`claude-opus-5-5`), effort high, Claude Code inside T3 Code, host `jer-laptop` (Linux) |
| Custody | **Reversed pair.** The dispatched OpenAI `gpt-6.1-sol` worker failed at launch ("Your ChatGPT usage limit was reached", no output, no writes). Custody transferred to this author at [#672 comment 6943254376](https://github.com/jermur788/CCF/issues/672#issuecomment-6943254376); orchestrator ledger event E-003 on `docs/sen-1-672-deepseek-world-orchestration-20261117-v1` @ `cd174acc`. |
| Claim / parent / Manager | Stage-W1 content child ([claim comment 6943093772](https://github.com/jermur788/CCF/issues/672#issuecomment-6943093772), transferred) / [#672](https://github.com/jermur788/CCF/issues/672) / [#671](https://github.com/jermur788/CCF/issues/671) |
| Branch | `docs/sen-1-672-world-content-signature-20261117-v1` |
| Base = starting HEAD | `afd5484fc11697e6bde64f99d57d22e98f572f1f` (`origin/main`, re-fetched and unchanged before writing) |
| Own path (the only file written) | `Docs/Handoffs/ScenarioOne-Compact-Ecology-20261117/Sen-1-672-COMPACT30-COMPLETE-WORLD-CONTENT-SIGNATURE-CANDIDATE-AND-OWNER-GATES-20261117.md` |
| Checkout | `/media/jer/Files/CCF/Orchestration/CCF-672-WorldContent-20261117` (docs-only git tree; no `Library`, no Unity, no lease) |
| Helper (outside the repo, non-durable) | `/tmp/claude-1000/-media-jer-Files-CCF-Primary/7e124beb-8dc5-44bd-ad91-acc4786056b2/scratchpad/w1-672/w1_verify.py`, 189 lines, SHA-256 `6b2accd55a30c42ded0a377d50f12ae67174ceb9db3f0b639527ff4507dae287`, run with `python3 -I -B` (Python 3.12.3) |
| Correction round (one bounded round) | Author correction after the Stage-W2 independent review at `bd6208f2` (`Docs/Handoffs/ScenarioOne-Overnight-Gates-20261117/Sen-1-672-INDEPENDENT-WORLD-SIGNATURE-CANDIDATE-REVIEW-20261117.md`, OpenAI `gpt-6.1-sol`, NEEDS CHANGES 0/0/1/2) of this file at `3082b7d7`. Changes: F-01 (eight mixed-evidence rows relabelled UNS under C1, dependent counts only), F-02 (class vs signature wording, §8), F-03 (manifest-member limitation, §1.1). No hash, number, signature status or OD option changed. |

**Independence disclosure.** This author's model family (Claude Opus 5.5) also wrote the #657 roster annex whose digest this document reproduces, and wrote the #669 independent review of #667 and its delta. My roster reproduction uses a freshly written script. It is still **not** independent of the roster's origin. The Stage-W2 review must come from a different model family (the packet names GPT-6.1 Sol). This author must not review this document.

**Commit SHA and file SHA-256.** A file cannot contain its own commit hash or its own digest. Both are reported in the handoff reply and the #672 / parent-ledger record, not here.

---

## 0. Evidence legend, class-assignment rule and signature-status rule

### 0.1 Evidence classes

| Abbrev. | Class | Meaning |
| --- | --- | --- |
| SV | SOURCE-VERIFIED | The value is a byte-literal of an authenticated artifact (raw fixture, the #657 roster fence, signed save text) carried without computation. |
| MD | MODEL-DERIVED | The value is the output of documented arithmetic or geometry on SV inputs (membership tests, counts, distances, cell mappings). No Unity, field measurement or player observation. |
| OC | OWNER CHOICE | The value or rule is stated in a canonical owner decision (decision-log D-083 or D-084). This is accepted intent. It is never proof of exact content bytes or of native behaviour. |
| GCP | GAME-CALIBRATION-PROPOSAL | A proposed rule or boundary that supplies no missing datum and has no source or owner authority. Unsigned and unselected. |
| UNS | UNSUPPORTED / NOT IN SOURCE | The Compact requirement needs a datum that no authenticated byte, signed text or canonical owner decision supplies. |

### 0.2 Verification marks (separate from class)

| Mark | Meaning |
| --- | --- |
| **VN** | Verified now: bytes hashed or quantity recomputed by this author at this stage (§1–§2, §5). |
| **SC** | Source line re-read now by this author at the cited commit (C# at `afd5484f`; specs/annex/manifest at `1e0fba5f`; save text at `200add4e`). |
| **CARRIED** | Not re-verified at this stage. The prior location is cited. Never read as verified-now. |

### 0.3 Class-assignment rule (resolves #669 MINOR F-02)

#669 F-02 found that #667 at `d0520778` gave UNS to some fresh-default tree rows and GCP to others with the same kind of evidence. The #667 author's correction at `a94d3d91` relabelled the eight GCP rows as UNS (count 36 → 44), and the #669 delta re-check approved that recount. **But no written rule was stated.** This section states one. Every row in §3 then records which step fired.

Apply the steps **in order. The first step that matches decides the class.**

1. **C1 → UNS.** The row's Compact requirement needs a datum (number, string, enum, geometry, asset, footprint, pose or record) that no authenticated raw byte, signed text or canonical owner decision supplies. **The following never supply that datum, and never upgrade the class:**
   - a fresh C# field initializer or DTO default;
   - a source fallback (for example the flat `y = 0` or `Quaternion.identity`);
   - a calibration constant marked `[C]` or `[D]`;
   - a historical harness setting;
   - a DRAFT-spec proposal.

   Such a value is recorded only as a *candidate* in the row text.
2. **C2 → OC.** Otherwise, if decision-log D-083 or D-084 states the value or rule, the class is OC. Raw bytes carrying the same value are then cited as provenance only.
3. **C3 → MD.** Otherwise, if the value is produced or established by arithmetic or geometry on authenticated inputs, the class is MD.
4. **C4 → SV.** Otherwise, if the value is a byte-literal of an authenticated artifact or of signed text, the class is SV.
5. **C5 → GCP.** Any remaining row is a proposed rule with no missing datum and no source or owner authority. Its class is GCP.

**Effect on the disputed rows.** Every tree row whose candidate is a fresh source or DTO default lands at C1 → **UNS**. That covers `stageTimer`, `chopProgress`, `hasSimulation`, `equivalentSuppressedYears`, `pruningLifts`, `crownBaseHeightM`, `lastPruningYear`, the three mortality fields, `markType` and status. The suppression-class row and its persisted field both land at **UNS**, so they no longer disagree. Route A anchors are not named in D-083/D-084 (D-084 names only the four G1 positions), so they land at C4 → **SV**. The G1 positions land at C2 → **OC**.

**Mixed-evidence rows (Stage-W2 F-01 correction).** C1 is tested against the row's **whole** Compact requirement, before any owner decision or derivation is considered. Eight rows combine accepted owner intent or a source formula with a datum the row itself records as missing: **41** (boundary dispatch at +15 unsigned), **46** (visitor/parking placements and capacity), **47** (collection staging footprints and state controls), **61** (path ribbon/collider), **62** (exact verge width and fade recipe), **73** (effective guard mesh / complete footprint), **74** (distinct expired shape), **75** (failed Compact mesh/stakes/pose). Under C1 each whole requirement is **UNS**. Their D-083/D-084 owner intent, the cell formula, the anchors and the guard/path evidence remain recorded in the value and provenance columns, and in the Prov. type column, unchanged. Only the class of the incomplete whole requirement changes. All eight were already BLOCKED, so no signature status changes.

**Resulting counts.** **SV 21 · MD 4 · OC 10 · GCP 1 · UNS 52 = 88.** This deliberately differs from the `a94d3d91` labels (SV 21 · MD 5 · OC 17 · GCP 1 · UNS 44): the difference is exactly the eight mixed-evidence rows above (MD −1, OC −7, UNS +8). The row-by-row step column in §3 lets anyone recount.

### 0.4 Signature-status rule

| Status | Rule |
| --- | --- |
| **SIGNED BY AUTHORISED CONTENT OWNER** | Requires a recorded attestation by an identified, authorised content owner over exact bytes. **None exists. 0 rows and 0 cells carry this status.** |
| **CANDIDATE** | The row's required value is fully specified in this document from authenticated evidence or an owner decision. Only attestation, the W14 binding and later native witnesses are missing. |
| **BLOCKED** | The row needs authored data that does not exist, or an owner choice among alternatives that has not been made. Each BLOCKED row names a blocker. |

CANDIDATE is not "approved" and not "complete". **The world is signable only when no BLOCKED row remains** (§10).

### 0.5 Citation aliases

| Alias | Exact source |
| --- | --- |
| ANN | `1e0fba5f:Docs/Handoffs/ScenarioOne-Compact-Ecology-20261116/Sen-1-657-AUTHORED-COMPACT30-ROSTER-PROVENANCE-AND-OWNER-SIGNATURE-CANDIDATE-20261116.md` (blob `1ecf429c`, identical at `55f79067`) |
| MAN | `1e0fba5f:Docs/Handoffs/ScenarioOne-Compact-Ecology-20261116/Sen-1-COMPACT-30M-WORLD-MANIFEST-CANDIDATE-20261116.md` |
| P / C / G | `1e0fba5f:specs/scenario-one-compact-complete-loop-v1/spec.md` / `…-initial-planting-v1/spec.md` / `…-graphics-v1/spec.md` (all DRAFT) |
| W667 | `a94d3d91:Docs/Handoffs/ScenarioOne-Overnight-Gates-20261117/Sen-1-667-COMPACT30-COMPLETE-WORLD-CONTENT-PROVENANCE-AND-SIGNATURE-READY-CANDIDATE-20261117.md` |
| R669 | `9f571297:Docs/Handoffs/ScenarioOne-Overnight-Gates-20261117/Sen-1-667-INDEPENDENT-COMPACT-WORLD-AND-SAVE-GATE-DOSSIER-REVIEW-20261117.md` |
| SAVE | `200add4e:specs/scenario-independent-save-isolation-v1/spec.md` (blob `8ac55102`, the immutable signed contract) |
| DL | `afd5484f:Docs/Project/decision-log.md` (D-083 = line 149, D-084 = line 151) |
| SD / SC / T / SP / ST | `Assets/ForestPrototype/` `ForestSaveData.cs` / `ForestSaveController.cs` / `ForestTree.cs` / `ForestTreeSpawner.cs` / `ForestStartingStand.cs` at `afd5484f` |
| EC / CELL / US / UC | `ForestEcologyController.cs` / `ForestEcologyCell.cs` / `ScenarioOne/ScenarioOneUnderstorey.cs` / `ScenarioOne/UnderstoreyCompetition.cs` |
| M / PV / PLAYER / MK / DEF / WO / UI / SSP | `ScenarioOne/ScenarioOneManager.cs` / `ScenarioProtectionVisuals.cs` / `ForestPlayer.cs` / `ForestTreeMarkingManager.cs` / `ScenarioOne/ScenarioOneDefinition.cs` / `ScenarioOne/ScenarioOneWorkOrder.cs` / `UI/ScenarioOneUiRoot.cs` / `ScenarioSiteProfiles.cs` |
| CB / L / EH / OM | Raw C-B-30 / #508 ledger / executed harness / #508 `omissions-2.0.json` (paths in §1); `/x/y` = JSON pointer |

**Location fact (verified now).** The three Compact spec families, ANN and MAN are **not on `main` (`afd5484f`)**. They exist on the unmerged lineage `55f79067` → `1e0fba5f` → `a94d3d91`. `git merge-base 1e0fba5f afd5484f` = `a5541b8a`. `git diff 1e0fba5f afd5484f -- Assets` is **empty**, so C# line citations made at `1e0fba5f` (W667, R669) still resolve at this base.

---

## 1. Immutable source inventory

**Method.** `sha256sum` and Python `hashlib`. For Git objects: `git rev-parse <commit>:<path>` plus `git show <commit>:<path> | sha256sum`. In the #508 folder, `sha256sum -c SHA256SUMS` returned **9/9 OK** (ledger, omissions, seven scripts). **No archived script or harness was executed.** Script rows are integrity-only byte reads, not endorsements.

### 1.1 Raw local evidence (Files ext4; not in Git)

`N = /media/jer/Files/CCF/Verification/CompactOneRackNativeFeasibility460-20261113T145748Z/native-20261113T152528Z`
`E = /media/jer/Files/CCF/Verification/scen1-508-EntrancePathPlanting-20261114`

| File | Bytes | SHA-256 (VN) | Proves | Does not prove |
| --- | ---: | --- | --- | --- |
| `N/C-B-30.json` | 128518 | `3e7c37ad010d66382722bbd932db6c93b9de1b78914a38c48b4f1a7e1f9d5a27` | The 189-row fixture, its metadata, 113 anchors and harness traces. Equals `N/evidence-manifest.json` `"C-B-30.json"` and `L /inputs/C-B-30.json`. | A Compact world, terrain, ecology, presentation, or any native scene. The fixture declares `TEMPORARY RUNTIME DERIVATIVE: NOT A SAVE/PRODUCTION MODEL`. |
| `N/evidence-manifest.json` | 6135 | `cd35e159c0d306f5833104e697d9657b45956559d2f52d0f42ca8a6ace6838d7` | Run-level hash register (C-B-30 entry matches) | Anything beyond integrity |
| `N/harness-provenance.json` | 534 | `99a6484e45aff980a41fd4c25d605358c7a3090a8280fb8523ae6461ae36b284` | `executed_harness_sha256` = EH hash. It also states that the later seed instrumentation was **not recompiled**, so the initial seed supply is a source derivation, not a measured seed CSV. | That any seed state is measured |
| `N/executed-harness.cs` | 25321 | `3c0ebae88448d27b0aae4d0345b37a28026f8c9cc3b417d13920288853019863` | `InRack` at line 151 is a position predicate (SC by grep). The reachability walk uses a capsule of r 0.30 m / h 1.80 m (lines 48, 210, 227). | That any of its settings are Compact settings |
| `E/ledger.json` | 24746 | `aa10f54f2fac9ccfa23d11a450f29985c39bf966944c665cd822b0ef49b0bcf3` | H1 rect, inside, buffer, Route A samples, G1 sites, static canopy proxies, track, buildables | Native light, survival or walk |
| `E/omissions-2.0.json` | 3316 | `dcc5a53ed6bdf41e1224c36c962560d57d7a88a744a26cfdde25ecc4ce6bd2ab` | Lists `clr` 37 and `mort` 30 IDs: 67 unique, disjoint from the 189, and 189 + 67 = 256 = 16² lattice (VN count) | **Why** each slot was excluded. That causation stays author-reported by #508; no generator was rerun. |
| `E/SHA256SUMS` | 754 | `f86940c9f461bfe02eabb1f1dcb9016107c9dcdf87015f52fa67d34951e11fd3` | Checksum register; `-c` gives 9/9 OK | — |
| `E/scripts/explore.py` | 1457 | `a647fcb935a7a2155eb1a1fc6c2f270e19c82c4848e2dab04d4a9e5d4ea9b33a` | integrity only | algorithm correctness |
| `E/scripts/groups.py` | 1087 | `6e2f552f3d5a38f8c686515dda6a94c8b5f5f0e857a0ef14d442e6006bba6273` | integrity only | — |
| `E/scripts/ledger.py` | 9318 | `deffea859d4b2427778810beb28a4e8807d376661a9f702e14a4e4bdee71a5ab` | integrity only | — |
| `E/scripts/lightgroups.py` | 1600 | `af2499963ad3496e5505a1a72f7356c397d9750c43ee12cbf23272e2f9ecba94` | integrity only | — |
| `E/scripts/recon.py` | 2089 | `8d36154632aaebf13b08d4f61d14b2372a4166bbbf45b6a10f2706cfa6b4048d` | integrity only | — |
| `E/scripts/sites.py` | 1468 | `c7df2ac7590cf6f4397f4df8d86c616980580ff36774d5cc0bfc73c3127c7d6e` | integrity only | — |
| `E/scripts/slide.py` | 554 | `8bf70fe6fd5572bc41fb1c9e5804f29d4ad1ff36eebf36d0b3c6841f822751b7` | integrity only | — |

All 14 hashes equal W667:51–64 (CARRIED values, now reproduced).

**Manifest-member limitation (Stage-W2 F-03).** This author checked only the `N/evidence-manifest.json` entries for the inputs used here (C-B-30 and the executed harness). A full reviewer sweep of all 62 native-manifest members found 61 matching and one mismatch: `N/closure-receipt.json` (377 bytes) has manifest expectation `db414df73114bbbb84a87a3e98e065fe52222256b880a2067a1aeb59e47e3c29` and current bytes `e715ff7eff8a15a6232745916b836624464ad3de4e8ac437745953f8844f7dc6`. That receipt is not an input to the roster or geometry calculation, and every hash this document publishes was reproduced as correct. Blanket integrity of every manifest member must not be inferred from the manifest's own digest. The cause and date of the mismatch are not established. No archived evidence file or checksum register was repaired or edited.

**Absent input.** `N/ledger.json` **does not exist** (`ls`: "No such file or directory"). No hash is claimed for it and no substitute was opened. The authenticated ledger is `E/ledger.json`.

### 1.2 Git-held inputs read (read-only `git show`)

| Commit : path | Blob | Bytes | SHA-256 | Used for |
| --- | --- | ---: | --- | --- |
| `afd5484f`: SD | `56065ea1` | 16120 | `093cbea99aae9844d2cd3b0cda0260a16d5478a4206e0bf6534838eafa3b3d0e` | TreeSaveData, cell DTOs, v24 |
| `afd5484f`: SC | `e903dae9` | 25924 | `ac4c4cb64c26ae0fcaaac7bb693f4104dc5faa312755e77e10953079d494d8be` | capture fields, model selectors, single filename |
| `afd5484f`: T | `2e4ada0a` | 28561 | `0582c138d70124285e07bfa7a5c54ee76864c9a0cea592a5598bdac59a1f852d` | stage / mark / mortality / suppression / pruning defaults |
| `afd5484f`: SP | `7ed9a4b3` | 9668 | `7205311459081649c35174e0c157f176765b0ac2ca7aef608a0c2ea1803b14e7` | identity rotation, visual assignment |
| `afd5484f`: ST | `29af0628` | 23553 | `bc305a2f92f9ad68ef56699ee49423d12f3650165a17fd6d62fbfba3c7e9190b` | legacy lattice y = 0, ±(size/2 − 0.6) clamp |
| `afd5484f`: EC | `0290a7be` | 105786 | `5db7231f2164eb18bb3a0d38c769758d65803ce23048e7537b3471e94a19a3ea` | Hegyi, shade reach, `GetCellIndex` |
| `afd5484f`: CELL | `5e82a9a8` | 10043 | `d3ab0479056f7e035471edad989075df2f097f9b7bf62e2cbc058a19500cc844` | site-driver defaults, derived seed rain |
| `afd5484f`: US | `128a03ef` | 3324 | `fda9fea6b96a4bca5fbae36dab04fb0f36984ddb8a55ec41ee23a370c962b3db` | six flora proxies, `Initially` |
| `afd5484f`: UC | `a2fac341` | 10657 | `1d8c92895ae85f9191cb6de07ac5d30c845c2beef2d59927f8d8c019ef7f03cd` | bramble/bracken initialisation |
| `afd5484f`: M | `4a704569` | 141289 | `2e414b02652517664399422b9941313fb03f34dfe800c9dbba7efc87f1575f90` | exact-planting admission, shelter creation |
| `afd5484f`: PV | `a0a81bf8` | 6072 | `c9d1240a67c12b1cb11c06964d20a9322b4b3ad08abbd963e1b22b0e207fd2ec` | tube 1.2 m / r 0.10, failed pose |
| `afd5484f`: PLAYER | `2dd8b369` | 55507 | `a79b64e56893cb452b596be2cb9501c7319859ba9aef043b6eda5e4375f8858e` | CharacterController, reach, ground normal |
| `afd5484f`: MK | `7bb8c995` | 35062 | `7e61a9274ec4827a52d44715d7045d72f85b854405cb52ef8cd32c6c345e8eec` | 8 m marking reach |
| `afd5484f`: DEF | `b29a5e44` | 10713 | `05417363975cec5fdd284e9cba2983e02a758d2f12e7ab2cb818e627f220b4aa` | browse 0.2 `[C]`; starting cash 1200000 default (line 30) |
| `afd5484f`: WO | `f1a34b44` | 3975 | `f352a7e75acf284bd345e3fef655aec0618451950d08dfce0b6da3980ea6d560` | order position / cell / shelter fields (lines 38–54) |
| `afd5484f`: UI | `27090145` | 23526 | `3c3e7f84d407fb9518ad944ba0ca3f84393bb33c4aaec1db1f772142f8d3fce7` | read for completeness; one-shot first-review hold (48 / 462). No world-content field depends on it. |
| `afd5484f`: SSP | `d336bcc2` | 2087 | `78f5ab00e3b759f0228ad096a22fa4b84ac3cfd744dcc04392f0153484f63df8` | **dormant** Scenario One site profile: moisture / drainage / fertility / exposure = Reference, rooting = Unknown; not persisted |
| `afd5484f`: `ForestSiteProfile.cs` | `3c73cc9e` | 5853 | `497bef8582424f34267f3a0d9c568b3ddf6640448bf8daed064032b450d5c0ec` | descriptor classes (moisture, drainage/aeration) |
| `afd5484f`: DL | `eb0c38af` | 95512 | `9a2e8b2fdf7d4e6e67b5168b73fef164200e80806ce1d4b92d11dda0d4b9f1c4` | D-079 / D-081 / D-082 / D-083 / D-084 |
| `afd5484f`: `Docs/Project/current-milestone.md` | `c5fabd7b` | 29255 | `3c8ef3b7337bc7039cdce00c0d39b064e5bd0627b7e78210c973dfeea4fa9ef4` | Compact section, lines 13–25 |
| `afd5484f`: `Docs/Project/AgentWorkflow.md` | `d58e16ab` | 28316 | `3e3db30aa18049b292fecc1c067d6faae094c645ca1600fd72a8d941941da4e3` | review-independence boundary |
| `1e0fba5f`: P | `3342a125` | 130785 | `2b81e24084ee47ce6825d620875b954c84dbe233def4d20c3cd684e8ffffe0bc` | D84-G / T / F / M (DRAFT) |
| `1e0fba5f`: C | `3658f782` | 51395 | `36e8f8c598c50a29bc37f136af43356dddcb78b8f2d8447790d9f8216166bf69` | C84-1..3, H1 staging (DRAFT) |
| `1e0fba5f`: G | `50d86b77` | 34842 | `5a5e0d9de7b6d83013b2bd5e4ee743fd9bd0832d0d5b901dc5d47a2730c6c27b` | GA-2 / GA-3, FR-01..20, GQ-01..07 (DRAFT) |
| `1e0fba5f`: ANN | `1ecf429c` | 32346 | `10bbc4464c5a183185c68a4946b49e03726dae9a9e272324f5ef7c42f2a998bd` | roster fence (lines 89–264), §6 / §7 |
| `1e0fba5f`: MAN | `3c0dda8f` | 15128 | `1b1d84476c63402acfe6791e84c0452054a23a2cfe1d184ca19e489548f5b07f` | reserved identity table, staging, witness list |
| `a94d3d91`: W667 | `45410f9a` | 59094 | `2dc1c652c441743f2419ab589e93182ea31acba200df9e70d045d8d27d613c2a` | 88-row matrix, 36-cell table, W01–W14 |
| `9f571297`: R669 | `80a5e6c3` | 22603 | `fb239a0151010eb44f951ddb12eafbb0e5f5d99bf1bceae0e26a104ad6ff2629` | F-02, verified recomputations |
| `200add4e`: SAVE | `8ac55102` | 69553 | `9efda18bfb0b8d2f4cc426dbb387a5f2f4cd0cedf84702c7f96129201d349ed7` | lines 148 / 159 / 160 / 182 re-read; **not modified or reinterpreted** |
| `59280842`: orchestrator packet | `8bf496fe` | 12459 | `25aa11ab2863e9dc1006d494363eeabf57a931054b23b7249d9accfb7e3596d6` | binding Stage-W1 scope |
| `af3f3253`: content-owner packet | `b15c679b` | 5928 | `f28ffa396c7a3791e2eed940e1299b9c74d7643ded9eceafc48527070c288a28` | binding field/signature requirements |

The packets are at `origin/docs/sen-1-672-673-dual-deepseek-orchestrators-20261117-v1` (`59280842`) and `origin/docs/sen-1-overall-manager-successor-20261117-v1` (`af3f3253`).

**GitHub issue text read** (`gh api`, read-only): #672 body and comments 6942711106, 6942831310, **6942962077** (owner clarification, 2026-11-17T08:35:01Z), 6943072773, 6943093772 and 6943254376. These are decision receipts, not canonical Git.

---

## 2. Recomputed 189 → 175 audit and roster-digest reproduction (all VN, MD)

**Fixture metadata (SV, CB).**

| Field | Value |
| --- | --- |
| `id` | `C-B-30` |
| `size` | 30.0 |
| `lattice` | 16 |
| `target` | 189 |
| `seed` | 20260914 |
| `cells` | 36 |
| `rackZ` | −0.25 |
| `rackWidth` | `2.4000000953674316` (float of 2.4) |
| `generatorMethod` | "unmodified ForestStartingStand.Generate legacy lattice branch with scratch instance inputs" |

The 189 IDs are unique, all `sitka-spruce`, all age 20. Harness labels: north 99 / south 71 / rack 19. Maximum |x| and |z| is 14.3999996 m. This is the ST:113–116 clamp at size/2 − 0.6, so **no tree lies on the ±15 m boundary**. Raw totals for the 189: BA 3.6501191814 m², volume 22.2125049420 m³.

**Membership rule.** The 175 are the CB rows minus exactly those whose raw x/z lie in the **inclusive** H1 rectangle x = [−9, 3], z = [−15, −5]. `L /H1/rect` equals this rectangle.

| Recomputed item | Result (VN) | Matches |
| --- | --- | --- |
| H1 inside (14) | `P0004 P0005 P0006 P0207 P0306 P0307 P0403 P0404 P0405 P0406 P0407 P0503 P0505 P0507`. All south-labelled; no rack-labelled row. | `L /H1/inside` exactly |
| 1 m outside buffer (8), Euclidean rectangle distance ≤ 1.0 | `P0003 P0009 P0103 P0109 P0209 P0504 P0506 P0508`, all retained | `L /H1/buffer1m` exactly |
| Retained rows | **175** | — |
| Retained BA / volume (raw precision, before roster rounding) | **3.3540142137 m²** (3.3540142137153026) / **20.3413430341 m³** (20.34134303405881) | W667:94, R669:112 |
| Retained former rack-labelled stems | **19**: P0700–P0707, P0710–P0715, P0800, P0802, P0804, P0806, P0815 | W667:100–118 positions (3-decimal x/z identical) |
| 36-cell occupancy | §4. Sum 175, no tree outside cells 0–35, raw = roster-rounded vector. | W667:227–262 |

**Roster digest reproduction (VN).** I generated the roster from CB using ANN §4's exact serialisation:

- header `ccf-compact30-authored-roster-candidate;v=0-UNSIGNED;fields=id,species,x_m,z_m,age_y,dbh_cm,height_m,crown_m;order=id-ascending;lf`;
- one row per tree, ASCII-ascending by `id`;
- x/z `.3f`, DBH `.2f`, height `.2f`, crown `.3f`, age integer;
- comma-separated, no spaces or quotes;
- UTF-8, LF, no BOM, one final LF.

Result: **9481 bytes, 176 LF-terminated lines (header + 175), SHA-256 `dac06a3b7775f884fac9f8df6b95178cbaf9d912b44687082a161eaf0426cca1` — EQUAL to the stated digest.**

Separately, I extracted ANN's first `` ```text `` fence (ANN:89–264) byte-for-byte. It is 9481 bytes, 175 data rows, no BOM, no CR, eight columns on every row, and its IDs are strictly ascending and unique. Its SHA-256 is the same `dac06a3b…`, and **its bytes equal the generated bytes**. The 189-row reference export (ANN header with `harness_position_label`) gives 11359 bytes, `e649b2aac5df85230b71372e0977cbf723990f1aabb5881a3914855575cccfbe`, equal to ANN:82.

**Observation (new, minor).** CB `/cropIds` = `["P0000","P1504"]`, but the per-row `crop` flag is **false on all 189 rows**. ANN:40 says the `crop` field "marks" the two picks. In the bytes, the picks are recorded only at `/cropIds`. Neither column is authored into the roster (ANN:67), so the digest is unaffected. Both P0000 and P1504 are retained in the 175 with no mark proposed.

**Other raw provenance (VN).**

- CB `/anchors`: 113 entries (104 at r 1.5 m, 9 at r 2.2 m); 62 lie inside ±15 m.
- `L /track`: 52 planks, 27.5202551894 m, from (−0.928509, −14.75) to (−2.714831, 10.75).
- `L /buildables`: 8 positions at z 9.0–12.8.
- OM: 37 geometry + 30 mortality = 67 lattice slots outside the 189.

These explain inherited empty regions. They are **provenance only, never new Compact objects**.

**No-carry statement.** None of the following travels into the proposed new world:

- any historical 52-plank track or rack object;
- an approach clone;
- the Year-1 `access-clearance` trace (CB `/traces[0]` felled the 19 rack-labelled stems in the historical run; that trace is **not** imported);
- any Fell event or mark, timber sale or revenue from the 14 H1 omissions;
- any grant or automatic cash;
- any historical crop pick.

The 19 former rack-labelled stems **remain standing** as ordinary Sitka. The label is dropped from authored data (ANN:66–67). This is the D-083/D-084 boundary, not a claim that a scene exists.

---

## 3. Field-by-field world matrix — all 88 #667 fields

The rows below are #667's 88 rows (W667:131–218), in the same order and under the same field names.

- **Step** is the §0.3 rule step that fired.
- **Prov. type:** OWNER-CONCEPT = accepted owner conceptual choice; SRC-AUTHORED = source-verified authored value; MISSING = missing authored value.
- Every row carries a signature status. **No row is SIGNED.**
- Rows 41, 46, 47, 61, 62, 73, 74 and 75 are UNS (C1) as whole requirements (§0.3 mixed-evidence rows). Their Prov. type still records the accepted owner concept or source-authored formula that remains valid evidence for part of the requirement.

<!-- FIELD-MATRIX-BEGIN -->
| # | Field | Required value / rule (candidate defaults are not values) | Class (step) | Verif. | Provenance | Prov. type | Signature | Blocker |
| ---: | --- | --- | --- | --- | --- | --- | --- | --- |
| 1 | tree.id | ANN fence column `id`; 175 unique, strictly ASCII-ascending | SV (C4) | VN | ANN:89–264; CB → roster byte equality §2 | SRC-AUTHORED | CANDIDATE | W14 |
| 2 | tree.species | `sitka-spruce` on all 175 rows | SV (C4) | VN | ANN:89–264; CB `/trees/*/species` | SRC-AUTHORED | CANDIDATE | W14 |
| 3 | tree.x | `x_m`, metres, three fixed decimals | SV (C4) | VN | ANN fence; CB `/trees/*/x` rounding §2 | SRC-AUTHORED | CANDIDATE | W14 |
| 4 | tree.z | `z_m`, metres, three fixed decimals | SV (C4) | VN | ANN fence; CB `/trees/*/z` | SRC-AUTHORED | CANDIDATE | W14 |
| 5 | tree.age | `age_y` = 20 on all rows | SV (C4) | VN | ANN fence; CB `/trees/*/age` | SRC-AUTHORED | CANDIDATE | W14 |
| 6 | tree.DBH | `dbh_cm`, two fixed decimals | SV (C4) | VN | ANN fence; CB `/trees/*/dbh` | SRC-AUTHORED | CANDIDATE | W14 |
| 7 | tree.height | `height_m`, two fixed decimals | SV (C4) | VN | ANN fence; CB `/trees/*/height` | SRC-AUTHORED | CANDIDATE | W14 |
| 8 | tree.crown radius | `crown_m`, three fixed decimals | SV (C4) | VN | ANN fence; CB `/trees/*/crown` | SRC-AUTHORED | CANDIDATE | W14 |
| 9 | tree.y / terrain rule | No y in roster; one grounding rule must bind every tree | UNS (C1) | SC | ANN:303; SD:211 `position` Vector3; ST:113–116 legacy y = 0 is a fallback, not a value | MISSING | BLOCKED | W01 |
| 10 | tree.prefab / LOD variant | No per-tree visual identity; pin asset set + deterministic variant rule | UNS (C1) | SC | ANN:304; T:44–64; SP:164–177 | MISSING | BLOCKED | W02 / W13 |
| 11 | tree.orientation / yaw | None in CB; SP:148 `Quaternion.identity` is source behaviour only | UNS (C1) | SC | SP:145–148; ANN:304 | MISSING | BLOCKED | W02 |
| 12 | tree.visual scale / defect flags | None; biology must stay truthful | UNS (C1) | SC | T:44–64; ANN:304 | MISSING | BLOCKED | W02 |
| 13 | tree.life stage (`stage`) | Candidate engine default `Mature` (T:72); not proof of biological maturity at age 20 | UNS (C1) | SC | T:5–10, 72, 135–152; SD:191 | MISSING | BLOCKED | W03 |
| 14 | tree.status | Standing/living for all 175, encoded only via stage/mortality fields; no new status field | UNS (C1) | SC | ANN:65–69; T:81–87; SD:185–212 | MISSING | BLOCKED | W03 |
| 15 | tree.suppression class | No authored category; small DBH is not past suppression | UNS (C1) | SC | T:102–118; EC:52, 932; SD:198 | MISSING | BLOCKED | W03 |
| 16 | tree.harvest / CropTree mark (`markType`) | Candidate `None` = 0; harness `/cropIds` P0000 / P1504 not inherited; both saved ID lists must agree | UNS (C1) | VN / SC | T:18–23, 75; SD:33–34, 206; CB `/cropIds`; ANN:40, 67 | MISSING | BLOCKED | W03 / OD-17 |
| 17 | tree.mortality (`biologicallyDead`) | Candidate false; no deaths or H1 stumps inserted | UNS (C1) | SC | T:32, 81–82; SD:208 | MISSING | BLOCKED | W03 / W05 |
| 18 | tree.mortalityCause | Candidate `""` (T:33 initializer) | UNS (C1) | SC | T:33; SD:209 | MISSING | BLOCKED | W03 |
| 19 | tree.mortalityYear | Candidate −1 (T:34 initializer) | UNS (C1) | SC | T:34; SD:210 | MISSING | BLOCKED | W03 |
| 20 | tree.stageTimer | Candidate 0 (C# default, T:74) | UNS (C1) | SC | T:74; SD:192; SC:144 | MISSING | BLOCKED | W03 |
| 21 | tree.chopProgress | Candidate 0; no prior player chops | UNS (C1) | SC | T:73; SD:193; SC:145 | MISSING | BLOCKED | W03 |
| 22 | tree.hasSimulation | Candidate true (capture writes true, SC:146); DTO default false (SD:195) | UNS (C1) | SC | SC:146; SD:195 | MISSING | BLOCKED | W03 |
| 23 | tree.equivalentSuppressedYears | Candidate 0 recorded history; T:103 "no inferred pre-spawn history" | UNS (C1) | SC | T:102–118; SD:198 | MISSING | BLOCKED | W03 |
| 24 | tree.pruningLifts | Candidate 0 (unpruned) | UNS (C1) | SC | SD:202–203; T:121–129 | MISSING | BLOCKED | W03 |
| 25 | tree.crownBaseHeightM | Must follow a pinned source initialisation or authored values; no guessed per-tree heights | UNS (C1) | SC | SD:204; SC:153; T:124–129, 175 | MISSING | BLOCKED | W03 |
| 26 | tree.lastPruningYear | Candidate −1 (SD:205 initializer) | UNS (C1) | SC | SD:205 | MISSING | BLOCKED | W03 |
| 27 | tree.ordering / determinism | ASCII id ascending; UTF-8, LF, no BOM, fixed decimals, one final LF | SV (C4) | VN | ANN:71–77; fence checks §2 | SRC-AUTHORED | CANDIDATE | W14 |
| 28 | terrain.height source | No terrain bytes or heightfield | UNS (C1) | SC | ANN:303; ST:113–116; PV:85–87 fallback | MISSING | BLOCKED | W01 |
| 29 | terrain.normal / up vector | None; PLAYER:449–450 planting aim needs normal·up > 0.7 | UNS (C1) | SC | ANN:303; PLAYER:447–451 | MISSING | BLOCKED | W01 |
| 30 | terrain.surface / material | No Compact surface asset/material | UNS (C1) | SC | G:71–72 (FR-07 / 08), G:77 (FR-13); ANN:310 | MISSING | BLOCKED | W05 / W13 |
| 31 | terrain.bounds | 30 × 30 m, x/z ∈ [−15, +15], 900 m², no external forest donors | OC (C2) | SC / VN | DL:149 (D-083); CB `/size` = 30; ANN:290 | OWNER-CONCEPT | CANDIDATE | W04 (edge policy) |
| 32 | terrain.collider | None signed; visual ribbon is not movement proof | UNS (C1) | SC | ANN:303, 309; G:68–70 | MISSING | BLOCKED | W01 / W07 |
| 33 | cell.occupancy | 36-vector in §4, sum 175; stem counts, not canopy | MD (C3) | VN | CB `/trees` minus H1; §2, §4 | SRC-AUTHORED | CANDIDATE | W04 (ecology separate) |
| 34 | cell.ground-flora proxies | mosses, ferns, grasses, forbs, shrubs, fungi initial values/rule | UNS (C1) | SC | US:7–16, 27–52 | MISSING | BLOCKED | W04 |
| 35 | cell.brambleCover | 36 initial values | UNS (C1) | SC | US:17–18; UC:24–33; SD:67 | MISSING | BLOCKED | W04 |
| 36 | cell.brackenCover | 36 initial values; not inferred from fern meshes | UNS (C1) | SC | US:17–19; UC:30–33 | MISSING | BLOCKED | W04 |
| 37 | cell.seed state / cohorts | Cohorts must be initialised; seed rain is derived yearly and never saved | UNS (C1) | SC | CELL:91–99; SD:244–272; harness-provenance seed caveat §1 | MISSING | BLOCKED | W04 |
| 38 | cell.browse state | No per-cell export; DEF:58 background 0.2 is `[C]` calibration | UNS (C1) | SC | DEF:56–58, 135 | MISSING | BLOCKED | W04 |
| 39 | cell.deadwood state | No initial deadwood inventory; no H1 felling implied | UNS (C1) | SC | SD:68–69 | MISSING | BLOCKED | W04 |
| 40 | cell.canopy / light base | Only #508 static proxies for 10 cells (§4); not a native grid | UNS (C1) | VN | `L /H1/canopyCellsChanged`, `/H1/sites`; EC:914–918 | MISSING | BLOCKED | W04 |
| 41 | cell.coordinate formula | `floor((z+15)/5)·6 + floor((x+15)/5)`; G1 → 15. Boundary dispatch at +15 unsigned (EC:1963–1973 returns −1 when x or z = +15). | UNS (C1) | VN / SC | ANN:291; `L /H1/sites` cell 15; EC:1963–1973 | SRC-AUTHORED | BLOCKED | W04 |
| 42 | cell.per-cell initial values | All 36 rows in §4; zero stems ≠ zero ecology | UNS (C1) | SC | §4; SD:244–272; US:7–19 | MISSING | BLOCKED | W04 |
| 43 | cell.site drivers / opening / suitability | SiteProductivity / SoilStability / EstablishmentSuitability (C# default 1.0, `[C]`), recentOpening; dormant Reference site profile | UNS (C1) | SC | CELL:47–50; SD:255–259; SSP:22–27 | MISSING | BLOCKED | W04 |
| 44 | H1.surfaces / edge / stump depiction | None; GQ-05 undecided | UNS (C1) | SC | ANN:310; G:149 | MISSING | BLOCKED | W05 |
| 45 | H1.spawn pose | No xyz/yaw or camera pose | UNS (C1) | SC | ANN:310; MAN:80 | MISSING | BLOCKED | W07 |
| 46 | H1.visitor / parking staging | Visitors/cars separated from timber collection; placements/capacity unknown | UNS (C1) | SC | DL:149; C:93, 108; MAN:35 | OWNER-CONCEPT | BLOCKED | W06 |
| 47 | H1.collection area | Occasional small-truck closed-yard staging; no driving or road certification | UNS (C1) | SC | DL:149; C:100, 108; MAN:35 | OWNER-CONCEPT | BLOCKED | W06 |
| 48 | H1.clearing geometry | x = [−9, 3], z = [−15, −5]; 120 m²; 14 omissions before Year 0 | MD (C3) | VN | `L /H1/rect`, `/H1/inside`; §2 | SRC-AUTHORED | CANDIDATE | W05 / W14 |
| 49 | H1.outside buffer | Eight listed retained stems within 1 m; no extra omission | MD (C3) | VN | `L /H1/buffer1m`; §2 | SRC-AUTHORED | CANDIDATE | W05 |
| 50 | H1.must NOT contain | §6 list | OC (C2) | SC | DL:149, 151; P:130 (FA-2); C:108 | OWNER-CONCEPT | CANDIDATE | W06 (native negative later) |
| 51 | footpath.anchor A0 | (2.99, −4.75) | SV (C4) | VN | `L /H1/path/samples[0]`; ANN:294 | SRC-AUTHORED | CANDIDATE | W01 / W08 (y, projection) |
| 52 | footpath.anchor A1 | (2.80, −2.25) | SV (C4) | VN | `L /H1/path/samples[1]` | SRC-AUTHORED | CANDIDATE | W01 / W08 |
| 53 | footpath.anchor A2 | (2.06, 0.25) | SV (C4) | VN | `…/samples[2]` | SRC-AUTHORED | CANDIDATE | W01 / W08 |
| 54 | footpath.anchor A3 | (0.90, 2.75) | SV (C4) | VN | `…/samples[3]` | SRC-AUTHORED | CANDIDATE | W01 / W08 |
| 55 | footpath.anchor A4 | (−0.43, 5.25) | SV (C4) | VN | `…/samples[4]` | SRC-AUTHORED | CANDIDATE | W01 / W08 |
| 56 | footpath.anchor A5 | (−1.68, 7.75) | SV (C4) | VN | `…/samples[5]` | SRC-AUTHORED | CANDIDATE | W01 / W08 |
| 57 | footpath.anchor A6 | (−2.59, 10.25) | SV (C4) | VN | `…/samples[6]` | SRC-AUTHORED | CANDIDATE | W01 / W08 |
| 58 | footpath.anchor A7 | (−2.71, 10.75) | SV (C4) | VN | `…/samples[7]` | SRC-AUTHORED | CANDIDATE | W01 / W08 / W10 |
| 59 | footpath.spline / interpolation | No algorithm/tangents; polyline is only the checked baseline | UNS (C1) | SC | ANN:294, 308; G:68 | MISSING | BLOCKED | W08 |
| 60 | footpath.terrain projection | No height-offset or normal-alignment recipe | UNS (C1) | SC | ANN:308; ST:115; PV:85–87 precedents only | MISSING | BLOCKED | W01 / W08 |
| 61 | footpath.width / collider policy | 1.2 m symmetric walking-core target; ribbon/collider unauthored | UNS (C1) | SC | DL:151 (D-084); P:28; G:18, 68–70 | OWNER-CONCEPT | BLOCKED | W07 / W08 |
| 62 | footpath.visual verge | Optional ≤ 0.20 m per side, display-only; exact width and fade recipe absent | UNS (C1) | SC | DL:151; P:28; G:18 | OWNER-CONCEPT | BLOCKED | W08 |
| 63 | footpath.off-path refusal | No Compact path/H1 planting refusal; source checks cell/stem/duplicate only | UNS (C1) | SC | M:105–165; `L /negativeControls` offline | MISSING | BLOCKED | W09 |
| 64 | north.A7 end behaviour | Inherited gap beside A7 must be declared; alternatives §5.3, neither chosen | UNS (C1) | VN / SC | ANN:309; CB `/anchors`; `L /buildables`; §5.3 | MISSING | BLOCKED | W10 |
| 65 | G1.O1 | sessile-oak; x = 1.17, z = −0.47; cell 15; exact species/x/z match to `L /H1/sites` | OC (C2) | VN | DL:151; P:26; C:15; `L /H1/sites` | OWNER-CONCEPT | CANDIDATE | W01 / W12 / W14 |
| 66 | G1.O2 | sessile-oak; x = 1.57, z = −2.73; cell 15; exact match | OC (C2) | VN | same | OWNER-CONCEPT | CANDIDATE | W01 / W12 / W14 |
| 67 | G1.B1 | beech; x = 3.43, z = −0.07; cell 15; exact match | OC (C2) | VN | same | OWNER-CONCEPT | CANDIDATE | W01 / W12 / W14 |
| 68 | G1.B2 | beech; x = 3.83, z = −2.33; cell 15; exact match | OC (C2) | VN | same | OWNER-CONCEPT | CANDIDATE | W01 / W12 / W14 |
| 69 | G1.source-cell association | All four in cell 15 by coordinate formula, not list order | MD (C3) | VN | `L /H1/sites`; §5.4 | SRC-AUTHORED | CANDIDATE | W04 (native mapping) |
| 70 | G1.composition | Exactly 2 sessile Oak + 2 European Beech by real player orders; none at Year 0 | OC (C2) | SC | DL:149, 151; C:19 | OWNER-CONCEPT | CANDIDATE | — (native T1 witness) |
| 71 | G1.site-match tolerance | No signed tolerance; question in §5.5 | UNS (C1) | SC | P:96; M:167–175 (0.5 m duplicate); M:133–138 (0.7 m stem guard) | MISSING | BLOCKED | W12 |
| 72 | G1.guard absent state | Shelter optional per tree; no record/visual = no protection | OC (C2) | SC | DL:151 (per-tree shelter choice, CIP-016); C:17, 19; M:1908–1914; PV:64–70 | OWNER-CONCEPT | CANDIDATE | W11 / W13 |
| 73 | G1.guard effective state | Honest upright protection from real shelter state; full footprint r ≤ 0.10 m | UNS (C1) | SC | DL:151; PV:11–12, 64–70; C:17 | OWNER-CONCEPT | BLOCKED | W11 |
| 74 | G1.guard expired state | Distinct from effective and failed by more than colour; no efficacy invention | UNS (C1) | SC | DL:151 ("honestly distinguishable"); C:17; G:20; PV:99–100 (colour only) | OWNER-CONCEPT | BLOCKED | W11 |
| 75 | G1.guard failed state | Honestly damaged; full visual/physical/stake footprint r ≤ 0.10 m; current pose not selected | UNS (C1) | VN / SC | DL:151; P:27, 30; G:20; PV:91–95; §5.6 | OWNER-CONCEPT | BLOCKED | W11 |
| 76 | player.capsule / collider | CharacterController required; no Compact radius/height/step/slope/layer | UNS (C1) | SC | PLAYER:4; ANN:308–310; G:125; EH walk capsule 0.30 / 1.80 m is harness-only | MISSING | BLOCKED | W07 |
| 77 | player.core | 1.2 m is a route width, not a capsule diameter or proof of turning | OC (C2) | SC | DL:151; P:28–31; G:68 | OWNER-CONCEPT | CANDIDATE | W07 / W08 (witness) |
| 78 | player.verge | ≤ 0.20 m visual-only; never enlarges the collidable corridor | OC (C2) | SC | DL:151; P:28; G:18 | OWNER-CONCEPT | CANDIDATE | W08 |
| 79 | player.spawn | No position/orientation; must agree with H1 and a safe controller pose | UNS (C1) | SC | ANN:310; PLAYER:4 | MISSING | BLOCKED | W07 |
| 80 | player.interaction reach | Source defaults 3.5 m (PLAYER:17) / 8 m (MK:10); Compact serialized values unbound | UNS (C1) | SC | PLAYER:17; MK:10 | MISSING | BLOCKED | W07 |
| 81 | identity.contentKey | `scen1-compact-h1-g1-v1`, the signed spec's "Compact O1, PROPOSED" registry row; not a digest | SV (C4) | SC | SAVE:148; MAN:19 | SRC-AUTHORED | CANDIDATE | W14 |
| 82 | identity.content version / label | `Compact30-H1-RouteA-G1-content-v1` (draft label, not a registry key); roster header `v=0-UNSIGNED` | SV (C4) | SC | ANN:11, 71, 319 | SRC-AUTHORED | CANDIDATE | W14 |
| 83 | identity.whole-world record / recipe | No canonical manifest bytes or digest recipe selected | UNS (C1) | SC | ANN:312; SAVE:182 | MISSING | BLOCKED | W14 |
| 84 | identity.chapter / profile / world / geometry | `scenario-one` / `o1-compact-v1` / `scenario-one-compact-v1` / `compact30-v1`; numeric 2 is a dispatch reservation only | SV (C4) | SC | SAVE:148, 159–160; MAN:19 | SRC-AUTHORED | CANDIDATE | W14 |
| 85 | identity.historical refusal | Strict selected-profile match before mutation; no cross-slot fallback | SV (C4) | CARRIED (SAVE:173–181 and 188–210 not re-read; W667:215, R669:89) | SAVE:173–182, 188–210; D-079 / D-081 | SRC-AUTHORED | CANDIDATE | W14 (engineering) |
| 86 | identity.model selectors / seed | CB seed 20260914 is provenance; `rngModelVersion`, `regenerationModel`, `growthModel`, `stormModel` unbound for Compact | UNS (C1) | VN / SC | CB `/seed`; SD:13–27; SC:116–122 | MISSING | BLOCKED | W04 / W14 |
| 87 | identity.initial management / world objects | €12,000 = 1200000 c is D-084 intent (DEF:30 default matches). Empty work/history/plantings and neutral rack fields are proposed. Carried/stored wood and buildables inventories are absent. | UNS (C1) | SC | DL:151; DEF:30; SD:11, 33–49; SAVE:160; P:129 | MISSING | BLOCKED | W14 / W06 |
| 88 | identity.signable content vs engineering | Content owner signs authored bytes/rules only; import/controller/router/runtime proof stays engineering | GCP (C5) | SC | ANN:329–337; SAVE:182; G:151 | MISSING (proposal) | CANDIDATE | W13 / W14 |
<!-- FIELD-MATRIX-END -->

**Recount.**

| Measure | SV | MD | OC | GCP | UNS | Total |
| --- | ---: | ---: | ---: | ---: | ---: | ---: |
| Rows | 21 | 4 | 10 | 1 | 52 | 88 |
| CANDIDATE | 21 | 4 | 10 | 1 | 0 | 36 |
| BLOCKED | 0 | 0 | 0 | 0 | 52 | 52 |
| SIGNED | 0 | 0 | 0 | 0 | 0 | 0 |

- **Row lists by class:** SV = 1–8, 27, 51–58, 81, 82, 84, 85. MD = 33, 48, 49, 69. OC = 31, 50, 65–68, 70, 72, 77, 78. GCP = 88. UNS = the other 52 rows.
- **BLOCKED outside UNS:** none. Every BLOCKED row is UNS (C1); every SV/MD/OC/GCP row is CANDIDATE.
- **Mixed-evidence UNS rows (§0.3):** 41 (formerly MD), 46, 47, 61, 62, 73, 74, 75 (formerly OC). Owner intent / formula evidence is retained in their provenance.

---

## 4. All 36 ecology cells — occupancy and full initial-state requirement

**Coordinate rule (MD).** `cell = floor((z+15)/5)·6 + floor((x+15)/5)`, giving 6 × 6 cells of 5 m.

- **Source dispatch** (`EC:1963–1973`, SC): `GetCellIndex` uses `origin = −size/2` and returns −1 when the cell index is outside [0, 5]. Each axis is therefore **half-open, [−15, +15)**, and a position exactly at +15 m maps to −1.
- **Grid-edge policy (UNSIGNED, W04).** Whether Compact adopts this half-open rule for every object type is unsigned. That includes the ground, path ends, planting requests and decorative instances. No roster tree is affected, because the maximum |x| and |z| is 14.400 m.
- **No external donors.** The 30 m world has no imaginary external seed or forest donors (D-083 bounds).

**Full initial state required for EVERY cell.** None of this is supplied by any source. A cell with zero adult stems still needs every item.

| Item | Requirement |
| --- | --- |
| (a) Flora | the six US proxies (mosses, ferns, grasses, forbs, shrubs, fungi) and `lastUpdatedYear` |
| (b) Competitors | `brambleCover`, `brackenCover` |
| (c) Regeneration cohorts | species, density, height, establishYear, origin, originYear (SD:262–272); empty is a value that must be stated |
| (d) Seed rain | a pinned derivation recipe; seed rain is never saved (CELL:91–94) |
| (e) Browse | background pressure and protection inputs |
| (f) Deadwood | records (SD:68–69) |
| (g) Canopy / light | initial canopy and light, derived from the 175 retained crowns by a pinned model, not copied from a proxy |
| (h) Site drivers | SiteProductivity, SoilStability, EstablishmentSuitability, recentOpening; plus whether the dormant SSP Reference profile applies |
| (i) Coordinate rule and edge policy | as stated above |

`recentOpening` must not be manufactured by running a Fell for the pre-world H1.

**Source initialisation recipe that a signer could pin or reject (GCP — not selected).**

| Input | Source | Note |
| --- | --- | --- |
| Six flora proxies | `ScenarioOneUnderstorey.Initially` (US:47–52) → `Target` from cell light, site, stability, opening (US:27–45) | `[D]` provisional response shapes |
| Bramble / bracken | `UnderstoreyCompetition.Initialize` sets both to `Target(cell) × InitialFraction 0.25` (UC:14, 24–33) | `[C]` / `[D]` calibration |
| Site drivers | CELL:48–50 C# defaults 1.0 | `[C]` simple defaults, not measured soil |
| Browse | DEF:58 0.2 | `[C]` gameplay calibration, not a deer density |

Adopting this recipe is a content choice (OD-04). It does not change any row's class.

**Static #508 canopy proxy.** `L /H1/canopyCellsChanged` gives [before H1, after H1] for 9 cells. The G1 sites give `canopyBase = canopyAfter = 0.7789` for cell 15. These are offline #508 script outputs (SV as recorded values). They are **not** an initial-state record, not a native light grid, and not a Year-1+ witness. The ledger's `meanCanopyAfter` is 0.9206 and CB `/states[0].meanCanopy` (189 trees) is 0.9798.

| Cell | x range (m) | z range (m) | Stems (MD, VN) | Layout facts (MD, VN) | #508 static canopy proxy (SV recorded) | Full initial ecology (a)–(i) | Signature | Blocker |
| ---: | --- | --- | ---: | --- | --- | --- | --- | --- |
| 0 | −15…−10 | −15…−10 | 4 | — | — | UNS | BLOCKED | W04 |
| 1 | −10…−5 | −15…−10 | 2 | H1 overlap 20 m² | 0.9645 → 0.8875 | UNS | BLOCKED | W04 |
| 2 | −5…0 | −15…−10 | 0 | H1 overlap 25 m²; zero stems ≠ zero ecology | 0.932 → 0 | UNS | BLOCKED | W04 / W05 |
| 3 | 0…5 | −15…−10 | 3 | H1 overlap 15 m² | 0.9971 → 0.9964 | UNS | BLOCKED | W04 |
| 4 | 5…10 | −15…−10 | 7 | — | — | UNS | BLOCKED | W04 |
| 5 | 10…15 | −15…−10 | 5 | x = +15 edge | — | UNS | BLOCKED | W04 |
| 6 | −15…−10 | −10…−5 | 6 | — | 0.9994 → 0.9991 | UNS | BLOCKED | W04 |
| 7 | −10…−5 | −10…−5 | 0 | H1 overlap 20 m²; zero stems ≠ zero ecology | 0.9957 → 0.5318 | UNS | BLOCKED | W04 / W05 |
| 8 | −5…0 | −10…−5 | 0 | H1 overlap 25 m²; zero stems ≠ zero ecology | 0.999 → 0.3982 | UNS | BLOCKED | W04 / W05 |
| 9 | 0…5 | −10…−5 | 1 | H1 overlap 15 m² | 0.9138 → 0.878 | UNS | BLOCKED | W04 |
| 10 | 5…10 | −10…−5 | 7 | — | — | UNS | BLOCKED | W04 |
| 11 | 10…15 | −10…−5 | 7 | x = +15 edge | — | UNS | BLOCKED | W04 |
| 12 | −15…−10 | −5…0 | 8 | — | — | UNS | BLOCKED | W04 |
| 13 | −10…−5 | −5…0 | 5 | — | 0.9955 → 0.9889 | UNS | BLOCKED | W04 |
| 14 | −5…0 | −5…0 | 3 | — | 0.984 → 0.9699 | UNS | BLOCKED | W04 |
| 15 | 0…5 | −5…0 | 2 | **All four G1 sites**; Route A 4.854 m (A0, A1); nearest stems P0707, P0710 (former rack-labelled) | 0.7789 unchanged (proxy light ≈ 0.221) | UNS | BLOCKED | W04 |
| 16 | 5…10 | −5…0 | 3 | — | — | UNS | BLOCKED | W04 |
| 17 | 10…15 | −5…0 | 8 | x = +15 edge | — | UNS | BLOCKED | W04 |
| 18 | −15…−10 | 0…5 | 7 | — | — | UNS | BLOCKED | W04 |
| 19 | −10…−5 | 0…5 | 6 | — | — | UNS | BLOCKED | W04 |
| 20 | −5…0 | 0…5 | 6 | Route A 0.632 m | — | UNS | BLOCKED | W04 |
| 21 | 0…5 | 0…5 | 4 | Route A 4.933 m (A2, A3) | — | UNS | BLOCKED | W04 |
| 22 | 5…10 | 0…5 | 6 | — | — | UNS | BLOCKED | W04 |
| 23 | 10…15 | 0…5 | 7 | x = +15 edge | — | UNS | BLOCKED | W04 |
| 24 | −15…−10 | 5…10 | 8 | — | — | UNS | BLOCKED | W04 |
| 25 | −10…−5 | 5…10 | 5 | — | — | UNS | BLOCKED | W04 |
| 26 | −5…0 | 5…10 | 2 | Route A 5.473 m (A4, A5) | — | UNS | BLOCKED | W04 |
| 27 | 0…5 | 5…10 | 3 | — | — | UNS | BLOCKED | W04 |
| 28 | 5…10 | 5…10 | 7 | — | — | UNS | BLOCKED | W04 |
| 29 | 10…15 | 5…10 | 8 | x = +15 edge | — | UNS | BLOCKED | W04 |
| 30 | −15…−10 | 10…15 | 9 | z = +15 edge | — | UNS | BLOCKED | W04 |
| 31 | −10…−5 | 10…15 | 5 | z = +15 edge | — | UNS | BLOCKED | W04 |
| 32 | −5…0 | 10…15 | 2 | z = +15 edge; Route A 0.780 m (A6, **A7 terminus**); inherited buildables gap | — | UNS | BLOCKED | W04 / W10 |
| 33 | 0…5 | 10…15 | 4 | z = +15 edge | — | UNS | BLOCKED | W04 |
| 34 | 5…10 | 10…15 | 6 | z = +15 edge | — | UNS | BLOCKED | W04 |
| 35 | 10…15 | 10…15 | 9 | x and z = +15 edge | — | UNS | BLOCKED | W04 |

**Cell checks (VN).**

- Occupancy sum = 175. Zero-stem cells are 2, 7 and 8.
- H1 overlap totals 120 m², in cells 1, 2, 3, 7, 8 and 9.
- Route A per-cell lengths (cells 15, 20, 21, 26, 32) sum to 16.672 m, equal to the route length to sampling precision.

**Cell counts.** 36 cells: occupancy MD 36, initial ecology UNS 36. Signature: BLOCKED 36, CANDIDATE 0, SIGNED 0.

---

## 5. Terrain, hydrology, path, A7, G1 and guards

### 5.1 Terrain elevation, normal, slope, hydrology, collider

| Item | Evidence | Status |
| --- | --- | --- |
| Elevation | No raw terrain bytes exist (CB/L have only x/z). Source precedents: ST:113–116 legacy y = 0; PV:85–87 samples `Terrain.activeTerrain` or falls back to 0. | UNS → W01 |
| Normal / slope | None. PLAYER:449–450 accepts planting only on surfaces with normal·up > 0.7, so any slope directly affects planting aim. | UNS → W01 |
| Hydrology | **No spatial hydrology exists in source.** A water-dispersal note says "no water or spatial carrier in the simulation" (`SpeciesConfigurationPolicy.cs:125–127`, read now, not in the dispatch list). SSP:22–27 holds a **dormant, non-persisted** Scenario One site profile with moisture and drainage/aeration both = `Reference` (SSP:5 "DORMANT and NOT PERSISTED"). A grep found no reader of it outside its own file (SC). Compact shares chapter id `scenario-one`, so whether that dormant profile applies to Compact is an open content question (OD-04). | UNS → W01 / W04 |
| Collider | None authored. The rendered path ribbon is not movement proof (G:68–70). | UNS → W01 / W07 |

**W01 options (none chosen).** (a) An explicit flat source plane (y = 0, up = +Y) declared as authored content. (b) A separately authored heightfield with pinned bytes, transform and one sampling rule shared by trees, path, H1, G1 and the player. Under (b), every y in §3 rows 9, 51–58 and 65–68 depends on those bytes. Either way, slope and drainage must not be inferred from x/z.

### 5.2 Route A — anchors, core, verge, projection, interpolation, refusal

**Anchors (SV, `L /H1/path/samples`, VN).**

| Anchor | x, z (m) | Next segment length (m, MD) |
| --- | --- | ---: |
| A0 | 2.99, −4.75 | 2.507210 |
| A1 | 2.80, −2.25 | 2.607221 |
| A2 | 2.06, 0.25 | 2.756012 |
| A3 | 0.90, 2.75 | 2.831766 |
| A4 | −0.43, 5.25 | 2.795085 |
| A5 | −1.68, 7.75 | 2.660470 |
| A6 | −2.59, 10.25 | 0.514198 |
| A7 | −2.71, 10.75 | end |

**Route length (MD, VN): 16.6719614516 m** (16.671961451565554).

- `L /H1/path/lengthM` = 16.6857879905 is the historical track sampling, a different geometric object.
- No anchor lies inside H1. A0 sits 0.25 m north of the H1 edge (z = −5).
- A4 coincides with a raw r 2.2 m clearance anchor at (−0.43, 5.25) in CB `/anchors`. This is provenance only.

**Nearest retained stem (MD, VN).** Clamped point-to-segment distance over the eight-anchor polyline gives **P1106 at 1.6004984013 m**. Its trunk-edge margin to a 1.2 m core is d − DBH/200 − 0.6 = **+0.9194443718 m**. No retained stem centre lies within 0.8 m of the centreline. The ledger's 1.592 m (`/H1/path/minTrunkCentreDist`) is measured against the whole historical track.

**Core / verge (OC rules, D-084; rows 77–78 OC; rows 61–62 UNS as whole requirements because ribbon/collider and exact verge recipe are absent).**

- **Core:** a 1.2 m symmetric pedestrian walking-core target. It is a route width, not a capsule diameter or a turning proof.
- **Verge:** optional, at most 0.20 m per side, display-only. It has no collider, removes no tree, and has no effect on ecology, clearance or planting eligibility (P:28; G:18).
- **Not set:** the exact verge width (0–0.20 m) and the fade/material recipe (W08).
- **G:80 conflict:** the earlier #577 scratch W preview used a 0.35 m soft edge, which is incompatible with this maximum.

**Projection and interpolation (UNS, W08).** No algorithm, tangents or projection recipe exists. The straight polyline is the only checked baseline. A smooth spline would move the centreline and invalidate every margin below unless its deviation bounds are pinned and margins recomputed. G:22 / P:29 note that a 0.10 m adverse shift leaves B2 at +0.021 m (CARRIED).

**Off-path / inside-H1 planting refusal (UNS, W09).**

- Source `TryDesignateExactPlanting` (M:105–165, SC) checks only five things: execution method, offer/species, stock, a 0.5 m open-order/juvenile duplicate (M:167–175), and an in-grid cell plus the 0.7 m living-stem centre distance (M:127–139).
- It has **no** Route A, verge or H1 exclusion.
- The refusal set (on-path, inside-H1, outside-world) and the visible reasons must be authored.
- Refusal must stay a *planting* refusal. It must not stop players from walking off-path to assess the stand (T1-B).

### 5.3 A7 northern terminus — alternatives only, **no choice made**

**Facts (MD, VN).**

- A7 (−2.71, 10.75) is in cell 32. The last segment A6–A7 is 0.514 m.
- A7 lies **0.253–0.258 m** from the historical buildable positions (−2.75, 11.0) and (−2.771994, 11.0) in `L /buildables`.
- The nearest retained stem to A7 is **P1506 at 3.661 m**, then P1507 3.884 m and P1106 4.035 m.
- A7 is 4.25 m from the north boundary (z = +15).
- Eight of the nine r 2.2 m raw clearance anchors cluster at z ≈ 9.0–12.8. They explain the inherited gap.

| Alternative | What is authored | Consequences | Still required either way |
| --- | --- | --- | --- |
| **A — declare the existing gap and end the path at A7** | Visible path end at A7 with a modest end treatment. The gap is declared inherited layout. | Simplest geometry. Retained canopy can be compared at the gap without claiming player treatment. Risk: an unexplained-looking gap needs a text/visual cue. | Keep the roster unchanged; no imported buildables; no new felling, planting or credit. End-cap footprint, surface and sightline (W08 / W10). |
| **B — a bounded observation stop in the same gap** | A clearly bounded stop at A7 with no building, no extra clearance and no vehicle access | Gives a reason to reach the north end and can teach retained-canopy comparison. Risk: more content; may distract from G1 or imply amenities. No dimensions, coordinates or asset IDs are assumed here. | Same, plus an exact footprint/material from the content owner, and a native witness of no roster change |

Refilling the gap with invented Sitka would change the exact roster. That needs a separate content proposal and is not a default. Both rows are **GAME-CALIBRATION-PROPOSALS**, not ecological predictions.

### 5.4 G1 — four 2 Oak + 2 Beech sites (OC; geometry MD, VN)

Margins are route distance − 0.6 − 0.10 for core, and a further − 0.20 for core + verge. They assume an upright r 0.10 m envelope, a flat plane and the straight polyline. They exclude terrain, spline deviation, mesh, stakes and capsule turning.

| Site | Species | x, z | Cell | Segment | Route distance (m) | Core margin (m) | Core + verge margin (m) | Nearest retained stems (m) | In H1 | `L /H1/sites` exact match |
| --- | --- | --- | ---: | --- | ---: | ---: | ---: | --- | --- | --- |
| O1 | sessile-oak | 1.17, −0.47 | 15 | A1–A2 | 1.0577547 | +0.3577547 | +0.1577547 | P0707 2.431; P0807 3.007; P0710 3.670 | no | yes |
| O2 | sessile-oak | 1.57, −2.73 | 15 | A0–A1 | 1.2628382 | +0.5628382 | +0.3628382 | P0508 2.066; P0707 3.247; P0710 3.620 | no | yes |
| B1 | beech | 3.43, −0.07 | 15 | A1–A2 | 1.2228347 | +0.5228347 | +0.3228347 | P0710 1.692; P0810 2.244; P0909 3.052 | no | yes |
| B2 | beech | 3.83, −2.33 | 15 | A0–A1 | 1.0209757 | +0.3209757 | **+0.1209757** | P0710 1.579; P0510 3.063; P0508 3.636 | no | yes |

- **Spacing:** O1–O2, O1–B1, O2–B2 and B1–B2 are each 2.2951253 m; O1–B2 and O2–B1 are each 3.2457973 m.
- **Clearance:** all four clear the source 0.7 m living-stem guard and the 0.5 m duplicate radius against each other.
- **Former rack-labelled neighbours:** P0707 and P0710 were rack-labelled in the fixture and remain standing. Deleting them would change these distances and the cell-15 light.

### 5.5 Site-match tolerance — the question, not an answer (W12)

The P:96 transaction leaves "site-match tolerance" to the content signature. Two bounded alternatives exist. **Neither is chosen.**

1. **Zero planar displacement (candidate).**
   - The player explicitly selects one of the four authored site markers.
   - The order stores the exact approved x/z, and terrain y comes from the signed W01 rule.
   - G1 membership requires exact canonical x/z equality.
   - All §5.4 margins stay valid for the actual planted point.
   - This needs a site-selection control that does **not** exist in source today.
2. **A separately justified finite tolerance t.**
   - A value would need its own justification, and the whole tolerance disk would need re-checking against core/verge, stems, other sites and the grid edge.
   - Every margin shrinks by up to t. For example, B2's core + verge margin becomes 0.1209757 − t.
   - **No value is proposed here.**

**Not tolerances.** The source 0.5 m duplicate radius (M:167–175) prevents duplicate markers. The 0.7 m centre distance (M:133–138) keeps planting off living stems. **Neither may be repurposed** as a teaching-site tolerance. Proximity must never create the order↔juvenile pairing (P:107, PJ-6).

### 5.6 Guard states and the failed-shelter envelope

**Source (PV, SC).**

- Tube: 1.2 m high, r 0.10 m (PV:11–12). The collider is destroyed (PV:81–83).
- Failed pose: `Quaternion.Euler(0, 0, 28)` plus offset (+0.25, −0.1, 0) m (PV:91–95).
- Expired differs only by colour (PV:99–100).
- Shelters are created per installed order with `failedYear = −1` and `effectiveYears = 8` (M:1908–1914).

**Failed envelope (MD, VN).** I sampled both end-cap circles of the posed cylinder at 10,000 angles each. The plan-view hull's extreme points lie on the caps, and the plan footprint does not depend on the rotation's sign convention.

| Site | Minimum route distance (m) | Intrusion into 0.6 m half-core (m) | Intrusion into 0.8 m half-core + verge (m) |
| --- | ---: | ---: | ---: |
| O1 | 0.4586424 | **0.1413576** | 0.3413576 |
| O2 | 0.6443177 | none (−0.0443) | 0.1556823 |
| B1 | 1.1031602 | none | none |
| B2 | 0.9010174 | none | none |

- O1 reproduces the reported 0.141 m defect. O2 with a verge reproduces G:146's "0.155 m into O2 when 0.20 m verge applies".
- This is a **visual-envelope defect, not evidence of a player collision**, because the collider is destroyed.
- D-084 already requires a Compact-only damaged state within r ≤ 0.10 m. The old renderer stays unchanged for historical worlds.

---

## 6. H1 arrival content

| Element | Content | Class / status |
| --- | --- | --- |
| Pre-existing omissions | The 14 IDs in §2, removed **before Year 0** as authored history. No event, Fell mark, order, cash, timber volume or harvest record. Their notional sale volume (1.6173 m³, recomputed now from CB `saleVolume`, VN; equal to ANN:45) is never sold or credited. | MD / CANDIDATE |
| Buffered retained stems | The 8 IDs in §2 stay standing. A local visual/collider hazard goes back to the owner; it never auto-erases trees. | MD / CANDIDATE |
| Surfaces / edge / stumps | No asset or rule. GQ-05 (G:149) leaves stump/history look to the owner; "no fresh stumps" is only a Manager default. Options: (a) a flat apron with edge transition and no stumps; (b) an old-stump context with supplied provenance, never player felling. | UNS / BLOCKED, W05 |
| Spawn pose | No xyz, yaw or camera pose exists anywhere. `originalPlayerPosition` must not be recycled without a value (MAN:80). | UNS / BLOCKED, W07 |
| Visitor mode vs closed-yard collection | D-083: an occasional small truck in a **staged closed-yard** event. C:93 / 108: no simultaneous parked cars/visitors and loading; the truck backs in if movement is shown, leaves forwards, never uses the footpath, and there is no public-road loading. Exact footprints, capacities and state controls are absent. | UNS (C1; D-083 owner intent recorded) / BLOCKED, W06 |
| **Must not contain** | Year-1 harvest or revenue from H1; any grant or automatic cash; any rack, extraction corridor or historical plank; a truck on the footpath; public-road loading; simultaneous visitors and collection; any imported historical buildable; any inherited crop pick or Fell mark | OC / CANDIDATE |

No parking count, vehicle envelope or road/rights certification is proposed or implied.

---

## 7. Tree residual state, regeneration initialisation and presentation

### 7.1 Tree state beyond the CSV

ANN authors only `id, species, x, z, age, dbh, height, crown`. TreeSaveData (SD:185–212) also persists:

- stage, stageTimer, chopProgress, hasSimulation;
- equivalentSuppressedYears;
- pruningLifts, crownBaseHeightM, lastPruningYear;
- markType;
- biologicallyDead, mortalityCause, mortalityYear;
- position.

Each extra field needs either an explicit per-tree value or a pinned, versioned constructor recipe. The obvious candidate is "fresh source spawn state", which gives stage `Mature`, zero timers, no marks, no deaths, zero recorded suppression, and unpruned trees with a crown base from the source initialiser. **It stays UNS (§0.3 C1) until a complete authenticated value or pinned recipe supplies the missing datum.** Signature is a separate axis (§0.4): an attestation without a complete recipe supplies no value, and a complete recipe that is still unsigned would change the class but not make the row signed.

**Honest biology.**

- The engine's `Mature` stage enum (T:72) is a legacy mechanics value. It is not a claim that 20-year-old Sitka is biologically mature.
- Zero recorded suppression means "recording starts at world creation" (T:103). It does not mean the real stand was never suppressed.
- Small DBH must not be relabelled as suppressed.
- The 175 retained trees start living. No mortality, damage, defect or stump is introduced to make the stand look varied.

### 7.2 Seed, juvenile, browse and understorey initialisation

- **Planted juveniles:** none at Year 0. G1 juveniles arise only from real Completed orders (OC row 70).
- **Natural regeneration:** cohorts must be explicitly empty or authored per cell. Seed rain is a yearly derivation from living trees (CELL:91–94) and is never saved. No seed-bank field may be invented.
- **Browse:** background pressure and shelter inputs per §4.
- **Understorey:** flora proxies and competitors per §4.

The harness-provenance caveat applies: the historical run's seed supply is a source derivation, not a measured seed CSV.

**No Oak or Beech natural recruitment is guaranteed by Year 40 (D-083).**

### 7.3 Presentation

| Item | State of evidence | Blocker |
| --- | --- | --- |
| Tree prefab / LOD / material | The source assigns visuals per species with ID-based pruning-base picks (SP:164–177) and optional recent/plantation variants (SP:169–170). **No prefab GUID, LOD set or material was inspected or assigned for Compact in this stage.** | W02 / W13 |
| Tree collider | Source trunk is a primitive cylinder (SP:150–153). Compact collider policy is unauthored. | W02 / W07 |
| Guard meshes and stakes, all four states (absent / effective / expired / failed) | None exists. All must stay within r ≤ 0.10 m in plan and be distinguishable by more than colour, and the 0.6 m juvenile must stay inspectable (C:17; G:20, 79). | W11 |
| Ground / H1 / path materials | Must be Compact-only instances; never edit `Forest Ground.mat` / `Forest Path.mat` (G:71–72). | W05 / W13 |
| **GQ-07 scene architecture** | (A) a separate Compact scene/hierarchy without historical `Dirt Path` planks or their consumers (the *preferred DRAFT*, not approved); (B) shared `ForestTest` with profile gating (G:151). All eight Route A anchors coincide with historical plank centres (G:73, FR-09, CARRIED). **Must be decided before any world/graphics signature.** | W13 |
| Licensed first-party presentation rights | D-032 is OPEN (G:82, FR-18). No production binary, licence or redistribution right is granted here. Locally present Ultimate Nature or other partial packs are not reproducible sources (G:76). | W13 |

---

## 8. Identity and content binding

| Item | Value | Status |
| --- | --- | --- |
| Reserved registry (SAVE:148, SV) | chapter `scenario-one`, profile `o1-compact-v1`, world `scenario-one-compact-v1`, geometry `compact30-v1` (numeric **2** = dispatch reservation only, never 0 / 1), content key `scen1-compact-h1-g1-v1`, slot `forest-save-scenario-one-compact-v1.json` | Signed text labels this row "Compact O1, PROPOSED". It is enabled only after a signed world manifest is accepted. |
| Draft content label (ANN:11) | `Compact30-H1-RouteA-G1-content-v1` | A label, **not** a registry key. Roster header `v=0-UNSIGNED` is serialisation, not a payload version. |
| Roster digest | `dac06a3b7775f884fac9f8df6b95178cbaf9d912b44687082a161eaf0426cca1` | Covers the 175-row roster **only**. It must never stand in for a world digest. |
| **Whole-world digest** | **None exists. None is invented.** | W14 |

**Proposed canonical manifest contents (UNSIGNED; carried from W667:313, restructured).** Each item has a named byte identity:

1. the roster block (by digest);
2. per-tree extra state or a pinned constructor recipe (§7.1);
3. presentation references and the assignment rule;
4. terrain / bounds / grid / edge policy;
5. all 36 cell initial states or a pinned derivation (model selectors, species parameters, source commit, expected readback);
6. H1 surfaces, staging modes and footprints;
7. path anchors, interpolation, projection, core, verge and end treatment;
8. the four G1 site identities, species, terrain positions and site-match rule;
9. guard/stake state envelopes;
10. player controller, spawn and reach;
11. initial cash, inventories, orders, history and neutral rack fields;
12. dependency file digests;
13. the reserved profile/geometry/content key binding.

**Serialisation:** UTF-8, LF, fixed key order, explicit units, finite decimal encodings, no ambient defaults.

**Digest policy.** A whole-content SHA-256 is computed **once**, after every input above is supplied and reviewed and the recipe is accepted. A seed alone never replaces explicit rows or a pinned derivation.

**Materiality and refusal (SAVE:182 SI-10, quoted, SC).**

> New-profile saved identity meanings immutable from **first integration to main of a writer for that profile** … any change interpreting persisted coordinates/cells, species identities, starting roster/placement, model/default semantics, teaching evidence or schema meaning requires a new versioned binding; display-only text/visual changes that cannot reinterpret saved facts do not.

After that trigger, a material content change needs a new reviewed content version and an explicit migration/refusal policy (ANN:337).

**Separation from the signed save contract.**

- The contract at `200add4e` (blob `8ac55102`) is **immutable**. Nothing here changes, extends or reinterprets it.
- No field is added to `ForestSaveData`, `ccfWorldPayload` or §4.2.
- The world payload stays the existing v24 shape with neutral rack fields (SAVE:160).
- A content signature cannot choose M-A-SCOPE, change the save contract, approve a DRAFT spec or release ISO-58.

---

## 9. Consolidated OWNER-DECISION REGISTER

Each question below is one only the owner (or a designated authority) can answer. **No option is presented as chosen.** "Stays blocked regardless" lists what the answer alone does not release.

| ID | Question | Options (none chosen) | Unlocks | Consequences | Stays blocked regardless |
| --- | --- | --- | --- | --- | --- |
| OD-00 | Who is the authorised content owner whose attestation turns CANDIDATE into SIGNED? | (a) The product owner personally. (b) A named designee with recorded delegation. | Any SIGNED status at all | Without it, nothing in this document can be signed | Native qualification; save admission |
| OD-01 (W01) | Terrain and hydrology for Compact | (a) Declared flat plane y = 0 with no hydrology. (b) An authored heightfield (and optional drainage layer) with pinned bytes and sampling rule. | Rows 9, 28, 29, 32, 60; y for anchors and G1 | (b) moves every y and may change slope-dependent planting aim (PLAYER:449–450) | Collider/walk witness (W07) |
| OD-02 (W02) | Tree presentation assignment | (a) A deterministic ID-based rule over a pinned first-party asset set. (b) An explicit per-ID variant/yaw/scale table. | Rows 10–12 | Asset set depends on OD-13 rights | Native visual acceptance |
| OD-03 (W03) | Extra tree state at world start | (a) A pinned fresh-source constructor recipe. (b) Explicit per-tree values for every TreeSaveData field. | Rows 13–26 | (a) is simpler but ties content to a source commit | Two-construction / reload witness |
| OD-04 (W04) | Initial ecology for 36 cells | (a) Pin the source initialisation recipe (§4) plus model selectors. (b) Supply a reviewed 36-cell table. Sub-choices: grid edge half-open [−15, 15) or other; whether the dormant SSP `Reference` site profile applies to Compact. | Rows 34–43, 86; all 36 cells | (a) inherits `[C]`/`[D]` calibrations as content; (b) needs an author with ecological review | Year-1+ ecology evidence |
| OD-05 (W05) | H1 surface, edge and stump look (GQ-05) | (a) A flat apron with soft edge and no stumps. (b) An old-stump context with provenance. | Row 44 | (b) must never imply player felling or revenue | Native view from spawn |
| OD-06 (W06) | H1 staging content | (a) Mode rules only; vehicles omitted until authored. (b) Approved stage-only props with measured envelopes. | Rows 46, 47, 87 (inventories part) | No road, rights or haulage certification either way | Vehicle/visitor exclusion witness |
| OD-07 (W07) | Spawn pose, controller and reach | Supply exact spawn xyz/yaw and capsule/step/slope settings; keep or change the source reach defaults (3.5 m / 8 m) | Rows 45, 76, 79, 80 | Must agree with OD-01 ground | Native walk H1 → A7 → each G1 |
| OD-08 (W08) | Path interpolation and verge | (a) Piecewise-linear centreline. (b) Smooth interpolation with pinned algorithm and deviation bounds. Verge width in 0–0.20 m plus fade recipe. | Rows 59, 61, 62 | (b) requires recomputing all §5.4 margins; B2 core + verge is only +0.121 m today | Capsule turning witness |
| OD-09 (W09) | Planting refusal geometry | (a) Explicit site selection checked against signed sites, plus path/H1 exclusion. (b) Free-position admission with signed exclusion footprints. | Row 63 | Must not forbid walking off-path | Native refusal negatives |
| OD-10 (W10) | **A7 north terminus** | (A) Declared gap and path end at A7. (B) A bounded observation stop in the same gap (§5.3). | Row 64 | See §5.3 | Native visibility and roster-unchanged witness |
| OD-11 (W11) | Guard meshes for absent / effective / expired / failed | Approve artist-supplied meshes and stakes within r ≤ 0.10 m, distinguishable beyond colour | Rows 73–75 | Depends on OD-13 rights | Removal of the 0.141 m O1 defect, Compact-only |
| OD-12 (W12) | **G1 site-match tolerance** | (a) Zero planar displacement via explicit site selection. (b) A separately justified finite tolerance (value to be supplied with clearance recomputation). | Row 71 | (b) shrinks margins by t; the 0.5 m / 0.7 m guards are excluded as tolerances | Boundary-perturbation witness |
| OD-13 (W13) | **GQ-07 scene architecture** and D-032 rights | (A) A separate Compact scene/hierarchy. (B) Shared `ForestTest` with profile gating. Plus a rights/storage decision for any new binaries. | Rows 10, 30, 72, 88; all presentation | Must precede any world/graphics signature (G:151) | Old-world regression and performance |
| OD-14 (W14) | Content binding | Accept or amend the §8 manifest contents, canonical recipe, digest policy and materiality/refusal policy. Adopt or replace the draft label. | Rows 1–8, 27, 81–84, 83 | The save owner must separately approve the serialisation mapping | Byte-identical construction / readback |
| OD-15 | Roster adoption | (a) Sign `dac06a3b…` verbatim: 175 rows including the 8 buffers and 19 former rack-labelled stems. (b) Return an amendment, which needs a new roster, a new digest and new reviews. | Rows 1–8, 27, 33, 48, 49 | (b) changes G1 neighbour distances and cell-15 light if any nearby stem changes | Native census readback |
| OD-16 | **M-A-SCOPE** — does one treated stem satisfy the D-084 substantive-intervention requirement? | **DEFERRED by the owner. No option, threshold or count is proposed here.** | — (completion predicate, not world content) | Not a world field. Recorded so it is not silently settled by content. | Remains deferred |
| OD-17 | Crop-tree intent at world start | The 2026-11-17 08:35Z owner receipt (comment 6942962077) confirms **three crop trees selected/retained and favoured** by the player through genuine Work Plan management. It is not three fellings, not a cap, and not a treatment on each. Question: should the authored world (a) start with **no** marks (candidate `markType = None` on all 175), or (b) also author candidate crop-tree suggestions without marking them? | Row 16 | Neither option creates a mark or Fell order. The receipt is recorded only as **crop-tree selection intent**; it does not settle OD-16. | Teaching/completion specs stay DRAFT |

---

## 10. Blocker summary

| Group | Rows (§3) / cells | OD | Missing author | Confidence in scoping | Risk if left |
| --- | --- | --- | --- | --- | --- |
| W01 Terrain / hydrology | 9, 28, 29, 32, 60 (+ y for 51–58, 65–68) | OD-01 | Terrain author | High (no terrain bytes anywhere) | High: every y and slope check depends on it |
| W02 Tree presentation | 10, 11, 12 | OD-02 | Artist / presentation author | High | Medium |
| W03 Tree initial state | 13–26 | OD-03 | Simulation-state author | High | Medium: silent defaults could become persisted meaning under SI-10 |
| W04 Ecology / model | 31 (edge), 33 (ecology), 34–43, 69 (mapping), 86; **all 36 cells** | OD-04 | Ecology / model author | High | High: largest unsigned surface |
| W05 H1 surface | 44 (+ 17, 48, 49 context) | OD-05 | H1 content author | High | Low–medium |
| W06 H1 staging | 46, 47, 50 (native), 87 | OD-06 | Staging / product author | High | Low–medium |
| W07 Player / navigation | 32, 45, 61, 76, 77, 79, 80 | OD-07 | Navigation author | High | High: no capsule/spawn data |
| W08 Path geometry | 51–58 (projection), 59–62, 77, 78 | OD-08 | Path author | High | High: B2 verge margin is 0.121 m |
| W09 Planting refusal | 63 | OD-09 | Planting / control author | High | Medium |
| W10 North terminus | 58, 64; cell 32 | OD-10 | Content owner | High | Low |
| W11 Guards | 72–75 | OD-11 | Protection / visual author | High (known 0.141 m defect) | Medium |
| W12 Site match | 65–68, 71 | OD-12 | Content + teaching owner | High | Medium |
| W13 Scene / rights | 10, 30, 72, 88 | OD-13 | Manager + content / rights authorities | High (G:151 says blocking) | High: architecture precedes signature |
| W14 Binding | 1–8, 27, 48, 65–68, 81–88 | OD-14, OD-15 | Content owner + save owner | High | High: no world identity without it |

The W01–W14 IDs are group identifiers, not 14 independent dispatches. The owner-level prerequisites OD-00 (who signs) and OD-17 (crop-tree start intent) sit outside the W groups. OD-16 (M-A-SCOPE) is deferred and outside world content.

**Count summary.**

| Measure | Value |
| --- | --- |
| Fields | 88 |
| Fields by class | SV 21 · MD 4 · OC 10 · GCP 1 · UNS 52 |
| Fields by status | SIGNED 0 · CANDIDATE 36 · BLOCKED 52 |
| Cells | 36 |
| Cells by class | occupancy MD 36 / initial ecology UNS 36 |
| Cells by status | SIGNED 0 · CANDIDATE 0 · BLOCKED 36 |
| Unsupported table rows (fields + cells) | 52 + 36 = **88**. W667:342's 44 + 36 = 80 used the earlier labels; the +8 is exactly the mixed-evidence rows 41, 46, 47, 61, 62, 73, 74, 75 (§0.3), all already BLOCKED |
| Owner-decision items | 18 (OD-00 to OD-17), one of which (OD-16) is deferred and not offered for decision |

**Minimum set the owner must attest to make the world signable.** All four steps are required.

1. Name the signing authority (OD-00).
2. Attest the 36 CANDIDATE rows as exact values: roster digest, H1 rectangle / 14 / 8, the eight anchors, bounds, the four G1 sites, composition, the must-not list, the guard-absent rule, core/verge rules, identity literals, and the GCP signing boundary.
3. Decide OD-01 to OD-15 and OD-17, and supply or approve the authored data that clears **every** BLOCKED row (52 fields + 36 cells).
4. Accept the §8 manifest recipe so that one whole-world digest can be computed, with the save owner separately approving the serialisation mapping.

**Attesting step 2 alone does not make the world signable.**

---

## 11. What this document does not do; NOT RUN; remaining verification

**Does not:**

- sign anything;
- claim owner or content-owner authority;
- promote the 175 roster to a signed or complete world;
- approve or edit a DRAFT spec;
- change or reinterpret the signed save contract at `200add4e`;
- decide M-A-SCOPE, A7, the W12 tolerance, the admission policy or GQ-07;
- create a scene, asset, save, Pref, `.meta`, package, coordination entry or canonical-doc change;
- touch Legacy40, Enlarged80, Rack V2/MFM, frozen Reference Future v1 or genuine v1–v24 saves;
- create a new evidence path on ZX20;
- write in `/media/jer/Files/CCF-Primary` (read-only `git show` only).

**NOT RUN:**

- Unity, the Editor, the compiler, typechecks or tests;
- any archived harness or #508 script;
- native tree reconstruction or import;
- walking, capsule, slope or collision checks;
- planting, stock, cash or approval settlement;
- annual ecology, regeneration or survival;
- rendering or 720p / 1080p captures;
- performance measurement;
- save/load, replay, profile refusal or old-save/Reference regression;
- player tests, Windows builds, Ops or leases;
- independent review;
- Manager acceptance.

**Actually performed:**

- read-only source and document reads;
- raw byte hashing (14 files) and `sha256sum -c` (9/9);
- detection of the absent ledger path;
- byte-equal roster reproduction (generated and extracted);
- 189-export digest reproduction;
- H1, buffer, rack-label, totals, occupancy, route, nearest-stem, G1 and failed-envelope arithmetic (MODEL-DERIVED);
- per-cell H1 and route overlap;
- an OM count check.

**Remaining verification (future, later stages):**

- the Stage-W2 independent nonauthor review (different model family);
- the owner decisions in §9;
- the native witnesses named in W667 §5 and MAN:79–84;
- save-owner approval of serialisation.

---

## 12. Handoff block

| Item | Value |
| --- | --- |
| Branch | `docs/sen-1-672-world-content-signature-20261117-v1` |
| Base / starting HEAD | `afd5484fc11697e6bde64f99d57d22e98f572f1f` |
| Commit SHA | Reported in the handoff reply and the #672 / parent ledger (cannot self-reference) |
| File | `Docs/Handoffs/ScenarioOne-Compact-Ecology-20261117/Sen-1-672-COMPACT30-COMPLETE-WORLD-CONTENT-SIGNATURE-CANDIDATE-AND-OWNER-GATES-20261117.md` |
| File SHA-256 / line count | Reported in the handoff reply (cannot self-reference) |
| Expected `git diff --stat afd5484f..HEAD` | exactly one added file (this one); nothing else |
| Roster reproduction | `dac06a3b…` reproduced byte-for-byte (generated = extracted fence) |
| Field counts | SV 21 · MD 4 · OC 10 · GCP 1 · UNS 52; CANDIDATE 36 · BLOCKED 52 · SIGNED 0 |
| Cell counts | 36 BLOCKED (occupancy MD; ecology UNS) |
| Owner questions | OD-00 to OD-17 (OD-16 M-A-SCOPE deferred, not offered) |
| Verdict | **SIGNATURE-BLOCKED** |
| Unresolved | W01–W14; OD-00; the GQ-07 architecture; D-032 rights; A7; W12; M-A-SCOPE (deferred) |
| Next | Changed-lines-only Stage-W2 re-check of this correction round by a non-Claude model family (GPT-6.1 Sol per packet), or `[PAUSED — CAPACITY]` if unavailable. This author must not perform the re-check. |
