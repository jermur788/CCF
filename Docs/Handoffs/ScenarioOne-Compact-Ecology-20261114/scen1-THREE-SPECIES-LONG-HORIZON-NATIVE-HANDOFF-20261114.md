# scen1 — three-species long-horizon native research HANDOFF (#490)

**Verdict: NATIVE RESEARCH RUN PASS (scope: measurement only) — HANDED OFF.** The data gate passed. The Unity run ended with Editor exit 0 and 142/142 fixture checks; both arms reached elapsed Year 40 on genuine annual ticks. Custody was clean and the lease was released normally. This is **not** VERIFIED, INTEGRATED or ACCEPTED status for Scenario One. Full numbers are in the companion [result](../../Research/ScenarioOne-Compact-Ecology-20261114/scen1-THREE-SPECIES-LONG-HORIZON-NATIVE-RESULT-20261114.md).

```text
RESEARCH HANDOFF
Task: CCF #490 — scen1 bounded 30 m three-species Year-40 native research witness (Manager #424 packet, 14 Nov 2026)
Worker: Anthropic Claude Opus 5.5 (claude-opus-5-5), Claude Code in local T3 Code, Linux; no sub-agents
Role: local source/Unity research executor; research only
Branch: research/scen1-three-species-long-horizon-native-20261114-v1
Base / starting HEAD / context commit: fc18190b976f1363f1b5d1b0881810db2f8c6e39 (origin/main, fresh)
Worktree: /media/jer/ZX20/CCF-scen1-LongHorizon-20261114 (clean; inactive Library/Temp/Logs/obj and Editor-generated csproj/slnx/UserSettings deleted)
Registration + pre-registered method: https://github.com/jermur788/CCF/issues/490#issuecomment-6874806702
GITHUB HANDOFF PATH: Docs/Handoffs/ScenarioOne-Compact-Ecology-20261114/scen1-THREE-SPECIES-LONG-HORIZON-NATIVE-HANDOFF-20261114.md
GITHUB HANDOFF COMMIT: branch HEAD recorded in the #490 completion comment
EXTERNAL EVIDENCE: /media/jer/Files/CCF/Verification/scen1-ThreeSpeciesLongHorizon490-20261114T085531Z/ (50 MB, 121 files, evidence-manifest.json)
```

## Files changed (tracked) — exactly two new Markdown files

* `Docs/Research/ScenarioOne-Compact-Ecology-20261114/scen1-THREE-SPECIES-LONG-HORIZON-NATIVE-RESULT-20261114.md`
* `Docs/Handoffs/ScenarioOne-Compact-Ecology-20261114/scen1-THREE-SPECIES-LONG-HORIZON-NATIVE-HANDOFF-20261114.md`

Serialized Unity files touched: none in the final diff. Assets added: none. Save/simulation impact: none. Player-facing result: none. No production C#, specs, Decision Log, milestone or `active-tasks.md` edits.

## Deviation reported

The packet proposed the worktree `/media/jer/Files/CCF/Operations/CCF-485-LongHorizon-20261114`. I used `/media/jer/ZX20/CCF-scen1-LongHorizon-20261114` instead and reported this in the registration before any Unity work.

**Why:** `/media/jer/Files` is a rotational HDD (sdb). The repository storage rule places active Unity working sets and their Library on the ZX20 SSD, and durable evidence on Files. The proposed path was never created.

## Machine and host safety record

| Item | Result |
|---|---|
| Lease before | `unity-lock status` free; no Editor PIDs; no bwrap |
| Temporary Ops lane | `Scen1LongHorizon490` appended to `CCF-Ops/lanes.json` (required by `unity-lock`, as #460 did). Before `1b6d70eb…` → after `93968b23…`. **Restored byte-exact** to `1b6d70eb…` after release. |
| Lease | `56191e2fcf8b45e2b95e8e5cfc8ba8a5`, owner `Claude Opus 5.5 scen1-490`, acquired 08:57:07Z; **released normally, not forced**; status free afterwards with no Editor PIDs (`lock-acquired.json`, `lock-release.json`) |
| Isolation | bwrap read-only host root; private `/home/jer`; private XDG config/data/state; tmpfs `/tmp`. Writable only for the own worktree and the evidence root. Namespace proof: `PRIVATE_NAMESPACE_PASS`, host root read-only. In-Editor: `persistentDataPath` is inside evidence, and the PlayerPrefs and EditorPrefs probes passed (`runtime-isolation.json`). No save/load was invoked; the save controller was disabled. |
| Licence | Hub entitlement copied into the private profile for the run (sha prefix `63c1ddbf`), then **deleted**. No licence file remains in evidence. |
| Host custody | All 82 protected Unity profile files, 5 worktree records (HEAD/status/diff) and 89 #460 evidence files (size+mtime): **before = after** (`custody/comparison.json`) |
| Editor side effects | The same 8 tracked files as #460 (`.vscode/settings.json`, 5 SectionFive `.mat`, `Packages/manifest.json`, `Packages/packages-lock.json`). Patch archived (`cleanup/editor-side-effects.patch` `85835af7…`); restored by exact path. Worktree back to clean `fc18190b`. |
| Harness removal | `Scen1LongHorizon490Native.cs` (`c7291f7a…`) and its generated `.meta` (`98b7bcd2…`) removed; hashes in `cleanup/removed-harness.sha256` |
| Inactive caches | Worktree Library (6.6 GB), Logs and generated project files deleted; private-profile `.cache` (735 MB) and `.dotnet` deleted, as #460 did |
| Log | One Editor-internal QuickSearch indexing `ArgumentOutOfRangeException` at startup, before the fixture began; not research-relevant |

## Verification matrix

| Check | Result |
|---|---|
| #460 raw gate (pinned hashes, manifest, 189 / age 20 / seed / models) | **PASS** |
| Initial 189 trees equal #460 in both arms | **PASS** |
| Arm B Year 0–8 states equal #460; Arm A Year 0–4 equal #460 and B | **PASS** |
| 40 genuine `AdvanceYear` ticks per arm; no skip, no forged acknowledgement, no outcome block | **PASS** (outcome `Active` at Year 40) |
| Harvests: quote = settlement (Year 1 both arms; Year 5 Arm B) | **PASS** |
| No NaN/Infinity in outputs; no harness exception | **PASS** |
| Year 0/5/9/10/20/25/30/40 snapshots (trees, juveniles, cohorts, cells) both arms | **PASS** |
| Year 20/40 captures at 720p and 1080p, both arms | **PASS with defects labelled**: Arm B Year 0 overhead reference is DEFECTIVE (no trees rendered); Arm B eye-level views are partly occluded |
| Per-tree mortality cause | **NOT MEASURED** (source: engine `self-thinning`; dead objects leave no record) |
| Bramble/bracken analysis | **NOT ANALYSED** (raw exported) |
| Old-save / Reference Future replay hashes, integrated suite | **NOT RUN** (not authorised) |
| Windows build | **NOT RUN** (forbidden) |
| GitHub read-back of the two files and additive diff | see #490 completion comment |
| Dashboard / Drive | **NOT VERIFIED** |

## Key findings for #424

1. The engine is numerically and procedurally stable for 40 years in a 30 m finite three-species woodland.
2. Planted Oak and Beech survive and promote (Years 10 and 11). At Year 40 (age 42) they are about 12–15 cm DBH and 18–19 m tall, suppressed and narrow-crowned. Their visual models change to mature broadleaf forms.
3. There is **no natural broadleaf regeneration in 40 years** without exterior donors; the planted trees only begin seed potential at Year 40.
4. Unmanaged Sitka self-thins from 170 to about 65 original stems.
5. One modest Year 5 release gives the crop trees +1.3 to +2.7 cm DBH by Year 40 and costs €2,463 net.

## Limitations

* This is one deterministic seed and one fixture. Arm differences after Year 5 include state-dependent divergence. Model outputs are not Irish field validation.
* The broadleaf results rest on two individual trees.
* Game "stage" is not biological stage.
* Screenshots are research overviews, not a player test.
* The Year 0/9 overhead references were optional; one of them is defective.

## Decisions needed (product/owner, not this worker)

1. Whether a bounded 30 m woodland whose broadleaf continuity depends on a few planted individuals over 40 years is an acceptable teaching premise, or whether exterior/hedgerow seed mechanics need a separate approved spec.
2. Whether self-thinning dominance and late, suppressed broadleaf development fit the intended Scenario One teaching arc.
3. Whether any follow-up (other seeds, more planted individuals, Year 60+) is warranted. It would need a new packet.

## Worker assessment

**Ready for review: Yes.** Not self-reviewed. **Recommended next step:** #424 issues APPROVED RESEARCH / NEEDS CHANGES / REJECTED. Nothing further starts automatically.

**DASHBOARD / DRIVE: NOT VERIFIED.**
