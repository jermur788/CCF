# #484 Tier 1 focal boundary CI — RESEARCH HANDOFF (offline; no Unity)

**Verdict: HANDED OFF — DATA GATE PASSED, calculation complete, Unity NOT RUN by design.** Numbers, tables, assumptions and limits are in the companion [result report](../../Research/ScenarioOne-Compact-Ecology-20261114/FOCAL-BOUNDARY-CI-TIER1-RESULT-20261114.md). This handoff gives the registration record, the packet's required answers, and the verification matrix.

```text
RESEARCH HANDOFF
Task: CCF #484 — Tier 1 offline focal competition-boundary sensitivity (Manager #424 packet, 14 Nov 2026)
Worker: Anthropic Claude Sonnet 5.5 (claude-sonnet-5-5), Claude Code inside local T3 Code, Linux.
        NOT the GPT-6.1 Sol / Codex worker the packet recommended. No sub-agents. Reasoning effort unverified.
Role: offline research / pure-Python analysis. No Unity, no machine lease, no saves/Prefs/HOME/XDG.
Branch: research/scenario-one-focal-ci-offline-20261114-v1
Base / starting HEAD: fc18190b976f1363f1b5d1b0881810db2f8c6e39 (origin/main, fresh at start)
Context commit: fc18190b976f1363f1b5d1b0881810db2f8c6e39 (packet baseline unchanged)
Worktree: /media/jer/Files/CCF/Operations/CCF-484-FocalCI-Offline-20261114 (created clean; sole writer)
Registration: https://github.com/jermur788/CCF/issues/484#issuecomment-6874426598 (before any file edit; issue set [RUNNING])
Tool commit: 1e38659e97a96ed0107ff7854b1ed0dc3a6c4093 (script, tests, results). Later commits on the branch add only
        the two Markdown documents and a README wording fix; the final branch HEAD SHA is in the #484 completion comment.
GITHUB HANDOFF PATH: Docs/Handoffs/ScenarioOne-Compact-Ecology-20261114/TIER1-FOCAL-BOUNDARY-CI-HANDOFF-20261114.md
EXTERNAL EVIDENCE (read-only, untouched): /media/jer/Files/CCF/Verification/CompactOneRackNativeFeasibility460-20261113T145748Z/native-20261113T152528Z/
```

## Files changed — new files only, no existing file edited

* `Tools/Research/ScenarioOneFocalBoundaryCI/README.md`
* `Tools/Research/ScenarioOneFocalBoundaryCI/focal_boundary_ci.py`
* `Tools/Research/ScenarioOneFocalBoundaryCI/test_focal_boundary_ci.py`
* `Tools/Research/ScenarioOneFocalBoundaryCI/results/results.json`
* `Tools/Research/ScenarioOneFocalBoundaryCI/results/focal-rows.csv`
* `Tools/Research/ScenarioOneFocalBoundaryCI/results/external-stems.csv`
* `Tools/Research/ScenarioOneFocalBoundaryCI/results/run-meta.json`
* `Docs/Research/ScenarioOne-Compact-Ecology-20261114/FOCAL-BOUNDARY-CI-TIER1-RESULT-20261114.md`
* `Docs/Handoffs/ScenarioOne-Compact-Ecology-20261114/TIER1-FOCAL-BOUNDARY-CI-HANDOFF-20261114.md` (this file)

No `Assets/`, `Packages/`, `ProjectSettings/`, `specs/`, Decision Log, milestone, `active-tasks.md`, saves/Prefs, frozen fixture/hash, or #460 evidence file was touched. No raw #460 files or tree arrays were copied into the repository (only the eight focal rows and the 101 hypothetical-stem rows). Serialized Unity files touched: none. Assets added: none. Simulation/save impact: none. Player-facing result: none.

## The six answers the packet requires

**1. How many original focal IDs were verified?** **8 of 8** (4 per candidate), each found in the hash-verified original Year-0 `trees[]` with ID, species (Sitka), X/Z, DBH, recorded native CI and edge distance, and each recorded CI reproduced offline to ≤ 7e-7 absolute: C-A-20 **P0101, P0604, P0504, P0708**; C-B-30 **P0000, P1504, P0807, P0209**. Stage: Year 0 immediately after unmodified `ForestStartingStand.Generate`, before access felling. The #460 anchors P0101/P0604 (CI 5.4487 / 5.4590) and P0000/P1504 (1.8469 / 3.4087) match the #460 report, so the stage matches.

**2. What was measured versus hypothetically assumed?**
*Measured (read-only original data):* all 82 and 189 trees' IDs, positions, DBHs, recorded CI. *Recomputed offline:* source Hegyi CI (float64 and float32 emulation). *Assumed (invented, labelled):* every external stem — a thinned continuation of the source 1.9 m lattice outside the property at the source 2,100 stems/ha target, Sitka age 20, DBH = the candidate's own mean (15.5696 / 15.4336 cm), no jitter, plus one sensitivity (expected-value stocking).

**3. Where is the actual external forest unknown?** Everywhere outside the property. #460's candidates are disposable scratch Unity worlds in an otherwise empty scene: no input contains any real neighbouring tree, ownership boundary, species, age, spacing, road or gap. Open land beyond the boundary is equally valid, and then the finite result is correct.

**4. Model difference per focal (Hegyi CI, finite → with assumed surround):**

| Candidate | Focal | Edge (m) | Finite CI | + external | Context CI | Ratio |
|---|---|---:|---:|---:|---:|---:|
| 20 m | P0101 | 2.75 | 5.4487 | +3.0490 (23 stems) | 8.4977 | 1.560 |
| 20 m | P0604 | 7.07 | 5.4590 | +0.3159 (3 stems) | 5.7749 | 1.058 |
| 20 m | P0504 | 8.83 | 6.0594 | +0.0000 (0) | 6.0594 | 1.000 |
| 20 m | P0708 | 2.81 | 6.9917 | +3.8179 (19 stems) | 10.8096 | 1.546 |
| 30 m | P0000 | 0.60 | 1.8469 | +4.2892 (29 stems) | 6.1360 | 3.322 |
| 30 m | P1504 | 0.60 | 3.4087 | +2.8817 (19 stems) | 6.2904 | 1.845 |
| 30 m | P0807 | 13.65 | 6.0762 | +0.0000 (0) | 6.0762 | 1.000 |
| 30 m | P0209 | 3.60 | 6.1785 | +1.2469 (8 stems) | 7.4254 | 1.202 |

The effect is large within ~4 m of an edge, small at ~7 m, and exactly zero beyond ~8 m. The expected-value sensitivity agrees within 0.2–12 % for the five near-edge stems (−24 % for P0604, whose result rests on three stems near the cutoff). The zero-external-stem negative control reproduces the finite CI bit-for-bit for all eight focal trees in both arithmetics.

**5. Does a native Tier 2 merit preparing?** **Conditionally yes — only under the "plot is a management subset of a larger forest" premise.** The change for near-edge sampled trees (ΔCI 1.2–4.3 against interior CI ≈ 6) is large enough to alter a training explanation of competition if neighbours really exist. Under the "isolated property, open land" premise it is irrelevant and the recommendation is to **stop** and keep the small property as a declared bounded training abstraction. Deciding which premise applies is a product/ownership decision for #424 and the owner, not for this worker. If chosen, the Tier 2 packet should be a separately authorised, exact-candidate, exclusive-lock native study with **real** neighbouring trees in a new authorised world (not meshes), reusing these eight IDs, and testing canopy/light/growth effects — none of which this Tier 1 measures. This handoff authorises none of it and starts nothing.

**6. Unknowns for long-term Sitka/Oak/Beech visual and biological growth:** every dynamic. Not measured here: crown/canopy/light overlap across the boundary (#460: potential crown edge-crossing for 29/82 and 78/189 stems), seed supply (kernels exceed these widths), wind and edge mortality, browsing, the effect of a CI shift on actual DBH/height trajectories (the source `1/(1+CI/5)` multiplier is shown for scale only, not as a prediction), Oak/Beech behaviour under the shared 8 m radius (no broadleaf-specific radius evidence), mixed or older surrounds, Year-25/40 outcomes, the two-versus-three species question, and any rendered or player-facing appearance. The study stand is pure Sitka age 20.

## Verification matrix

| Check | Result |
|---|---|
| Unit tests, `python3 -I -B -m unittest discover -s Tools/Research/ScenarioOneFocalBoundaryCI -p 'test_*.py'` | **PASS 30/30** (28 pure; 2 against the real evidence) |
| Real run against original evidence, exit 0; three consecutive runs byte-identical (`results.json`, `focal-rows.csv`, `external-stems.csv`) | **PASS** |
| Raw evidence SHA-256: `native-result.json`, `executed-harness.cs` = #460 report pins; all four used files = manifest | **PASS** (61/62 manifest entries match; sole mismatch `closure-receipt.json` was written after the manifest and is not an input) |
| Evidence custody: size + mtime of all 89 files, hashes of four key files before = after | **PASS** |
| Recorded CI recreated offline for all 82 + 189 stems | **PASS ≤ 2.4e-6 absolute**; float32 emulation bit-equal for 54/82 and 112/189 |
| Hegyi term hand example, floors, 8 m cutoff (7.9999 / 8.0 in, 8.0001 out), pair exclusion, target-vs-neighbour DBH direction | **PASS** |
| Property-boundary classification; 20 m vs 30 m identity and candidate scoping | **PASS** |
| Zero-external negative control (unit + 8/8 live focal × 2 arithmetics); beyond-cutoff control; interior stems exactly zero | **PASS** |
| NaN / inf / non-positive DBH / duplicate ID / missing field rejection | **PASS** |
| Only allowed new paths in the diff vs `fc18190b`; GitHub read-back | see #484 completion comment |
| Hegyi bit-equality against Unity | **NOT RUN** — approximation only; no native comparison |
| Unity Editor, game, native witness, Year-25/40 simulation, 3D captures | **NOT RUN** (out of scope) |
| Protected Legacy/Reference replay hashes | **NOT RUN** |
| Dashboard / Drive mirrors | **NOT VERIFIED** |

## Known limitations

See result report §6. In short: all external stems are hypothetical; on-property DBH was generated for a finite stand and held fixed; unjittered lattice with uniform DBH; P0604 is cutoff-limited; eight stems are a probe rather than a sample (the 30 m anchors are boundary stems and bound the high end); the offline arithmetic approximates but is not proven bit-equal to Unity. A first-run unit test caught a thinning-hash bias, which was fixed before any result was computed; no parameter was changed after seeing a focal result.

## Unrelated work preserved

`/media/jer/Files/CCF-Primary` still carries two pre-existing modifications (`AGENTS.md`, `Docs/Operations/StorageArchitecture.md`) on branch `ops/workflow-foundation-v1`; they were observed, not touched. Only the new worktree above was written to. The shared bare store gained one new branch ref and one worktree registration. No stash, reset, clean, or other worker's worktree/lane was accessed; `active-tasks.md` was read from `origin/main` and not edited.

## Decisions needed (for #424 / the owner)

1. Which premise does Scenario One intend for a compact plot: **a management subset of a larger forest**, or **a bounded property with open land beyond it**? This determines whether the finite boundary is correct and therefore whether Tier 2 is relevant.
2. If the first premise: authorise (or decline) a separate Tier 2 native packet. Nothing here pre-authorises it.

## Worker assessment

**Ready for review: Yes.** Review should check: the focal-selection rule (§3.2 of the report) against the code; the hypothetical-context definition (§3.3) for anything that looks like fishing; and the claim boundaries. This worker did not review its own work. **Recommended next step:** #424 records research acceptance or NEEDS CHANGES; no next stage starts automatically.

**DASHBOARD / DRIVE: NOT VERIFIED.**
