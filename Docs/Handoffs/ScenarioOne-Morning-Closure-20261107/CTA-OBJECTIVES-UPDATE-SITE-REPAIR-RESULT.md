# #330 — Objectives visibility Update-site checker repair (author result)

**Status: AUTHOR COMPLETE — SOURCE CANDIDATE PUBLISHED FOR #331 INDEPENDENT REVIEW.** This is the author's source-only evidence handoff. It is **not** Manager source acceptance, integration or Unity qualification. Independent [#331 review](https://github.com/jermur788/CCF/issues/331) is mandatory before any Manager G2/G3 verdict; this author approves nothing.

Binding packet: [Manager #329 CTA Objectives Update-site closure packet](https://github.com/jermur788/CCF/blob/main/Docs/Handoffs/ScenarioOne-Morning-Closure-20261107/MANAGER-329-CTA-OBJECTIVES-UPDATE-SITE-CLOSURE-PACKET-20261107.md). Claim thread: [#330](https://github.com/jermur788/CCF/issues/330) ([RUNNING receipt](https://github.com/jermur788/CCF/issues/330#issuecomment-6714844307), 2026-11-08T00:07:58Z).

## Identity

| Item | Exact value |
|---|---|
| Author | T3 Code/OpenCode session; **Xiaomi MiMo-V2.6-Pro** (`xiaomi-token-plan-ams/mimo-v2.6-pro`), effort setting not exposed to the session (recorded **UNKNOWN**, not guessed) |
| Author clock | 2026-11-08T00:07:00Z–2026-11-08T00:3xZ (host UTC clock; evidence-file mtimes retained) |
| Source lane | worktree `/media/jer/Files/CCF/Operations/CCF-CTAUpdateSite-20261107`, branch `fix/cta-objectives-update-site-20261107-v1` |
| Immutable base | #328 CTA published head `6adb20318b80e8a81560bc58a63c8bdbfa15d7ca` (= origin head; publication added only author handoff vs tested `288ba3863793e3775327fb7322dc06af28f7835a`); #325 base `c83fae1af4ac1449cba0304e44b0df49b91f8919` |
| Tested source | `990ec43ec6bf5ec7063fb530f80b82e37501c582` (the tooling commit; publication adds only this handoff Markdown) |
| Common Git store | `/media/jer/Files/CCF/Repository.git` |
| Evidence root (`E/`) | `/media/jer/Files/CCF/Verification/CTAObjectivesUpdateSite-20261107/` |

## Exact defect closed (A329-01) and the bounded assertion added

The #329 reviewer's witness moved the unchanged Objectives panel visibility statement

`learning.Root.style.display = !preview && screen == UiScreen.Objectives ? DisplayStyle.Flex : DisplayStyle.None;`

out of `ScenarioOneUiRoot.Update()` into an **uncalled** `private void ReviewUnusedPanel(bool preview)`. All 27 pinned symbol sites survived and the full checker returned a false green `10/0/2 exit 0`. The lexical stage pins the statement text anywhere in the root class body; the Roslyn stage counted a class-wide multiset without enclosing-method identity.

**Repair (report-then-rule, bounded; no general call-graph engine):**

1. `ObjectiveCtaRoslynBinding.cs` (report-only) now additionally reports (a) every root member named `Update` with kind/parameter-count/return-type/generic/containing-type facts, and (b) every candidate Objectives visibility assignment — matched by the reviewed left target `learning.Root.style.display` (whitespace/`@`-insensitive, `this.`/`global::`-tolerant) **or** by the reviewed conditional (`DisplayStyle.Flex : DisplayStyle.None` arms with a `UiScreen.Objectives` test) — with the binding of the `learning` receiver, the two `DisplayStyle` arms, the `UiScreen.Objectives` test, the **nearest enclosing callable** (method/local function/anonymous function/constructor/other member) and the enclosing method.
2. `objective_cta_roslyn_binding.py` (rules) fails closed unless: exactly **one** candidate exists (missing/duplicated/reshaped → fail); its assignment target is the reviewed receiver and `learning` binds to the source field `ScenarioOneUiRoot.learning`; the arms bind to metadata `UnityEngine.UIElements.DisplayStyle` `Flex`/`None` fields; the condition tests `UiScreen.Objectives` exactly once, bound to `ScenarioOneUiRoot.UiScreen.Objectives`; and the statement's enclosing callable **is** the root's single plain `void Update()` member — exactly one `Update` member, 0 parameters, `void`, non-generic — with no local function, lambda or other callable (or other type's `Update`) in between.
3. `test_objective_cta_roslyn_binding.py`: 10 new synthetic rule cases (A-21…A-30) and 5 new real-Roslyn negatives + 2 benign overlays (X330-N01…N05, X330-B10/B11), with end-to-end expectations where applicable.
4. Tiny `check_baseline.py` check-08 diagnostic clause and `README.md` claim corrections (rule 6 added; the "counts do not enforce method identity" limitation now scoped to the non-Update sites).

A same-string occurrence inside `ReviewUnusedPanel` (or a comment, or another type's `Update`) is **not** satisfaction; moved source stays rejected through the method-ancestry cause, not a diagnostic wording change. Other source UI methods are untampered.

## Diff scope (source commit only)

Exactly the five allowed paths under `Tools/Verification/OvernightPlaytestFeedback/`: `ObjectiveCtaRoslynBinding.cs`, `objective_cta_roslyn_binding.py`, `test_objective_cta_roslyn_binding.py`, plus the necessary tiny `check_baseline.py` check-08 diagnostic and `README.md` claim correction. **0 files** outside those; no Assets, production C#, scenes, Packages, ProjectSettings, saves, Workbench, PR #300 ancestry, `baseline-output.txt`, `roslyn-reference-manifest.txt` or `objective_cta_source_check.py` (lexical stage byte-unchanged). `git diff --check` clean.

## Test plan evidence (proportionate, falsifying, finite)

Raw outputs under `E/out/` (see `E/out/commands.txt` for exact commands; all checker/binding invocations ran from an SDK-pinned cwd `E/sdk8`, `global.json` `8.0.133`/`rollForward: disable`).

**1. Verbatim A329-01 reconstruction, old false green, new rejection.** The #329 reviewer's exact patch was copied and hash-verified (`0dc449573e5e5260b3f15b8557363261a00713b8e6bb97bfd632e817e95f91ba`) and applied to a disposable sparse clone of the exact CTA source at `6adb2031` (`E/cta-site`); the applied diff is byte-identical to the reviewer patch. With the **old** tool files: exit **0**, `SOURCE-STATIC TALLY PASS=10 FAIL=0 NOT-TESTABLE=2`, check 08 PASS, `0` C#9 compile errors, 27 bound sites (Flex 9 / None 13 / frameCount 3 / unscaledTime 2) — and stdout SHA-256 `f9f0e3f964044442e1568d89135da2f42b478f05fb29c504e168f41abec6fdf8` is **byte-identical** to the #329 recorded false-green stdout. With the **new** tool files on the same mutated source: exit **1**, `PASS=9 FAIL=1 NOT-TESTABLE=2`, only check 08, for the method/site cause: `[update site] … the Objectives visibility update must be inside the actual ScenarioOneUiRoot.Update() method, but it sits in MethodDeclaration ReviewUnusedPanel of ScenarioOneUiRoot`. C#9 compile stays clean (`errors=0`, 189 files) — the rejection is the method association, not a syntax/inventory side effect. Method symbol/syntax evidence (`E/out/update-site-evidence-{pristine,mutated}.json`): pristine candidate `updateSites[0].callable = MethodDeclaration Update` (void, 0 params) at line 235; mutated `MethodDeclaration ReviewUnusedPanel(bool)` at line 693.

**2. Bounded negative/benign set (real Roslyn overlays + full checker).**
- `X330-N01` A329-01 verbatim relocation → rejected, method/site cause (semantic stage; full checker 9/1/2 only check 08).
- `X330-N02` identical assignment duplicated into an uncalled method → rejected (Roslyn: 2 candidates; full checker: lexical "expected exactly one reviewed occurrence").
- `X330-N03` update hidden in an uncalled **local function** inside `Update` → rejected, `LocalFunction ZHidePanel` cause (semantic stage).
- `X330-N04` update moved into a **decoy type's own `Update()`** in the same file → rejected: receiver binds to `ScenarioOneUiRoot.ZDecoyPanel.learning`, and `MethodDeclaration Update of ScenarioOneUiRoot.ZDecoyPanel` is not the root's `Update`.
- `X330-N05` assignment removed from `Update` with its symbols left in another method → rejected fail-closed: the reviewed assignment is missing (0 candidates), and at full-checker level lexically (missing reviewed occurrence).
- Benign: `X330-B10` the same assignment inside a nested block of `Update` (legitimate syntax) → accepted 10/0/2; `X330-B11` a comment quoting the statement and `Update()` plus an unrelated uncalled method → accepted 10/0/2 (comments/method-name text create no sites).
All five negatives are valid C#9 with 0 compile errors except where noted; each is rejected for the intended cause, and the real `Update` site is semantic (enclosing method symbol), not a textual method name.

**3. Preserved controls (no weakening; unchanged corpora reused byte-unchanged).**
- Old lexical suite `test_objective_cta_source_check.py` (byte-identical files): **884/884 negatives rejected, false greens 0, wrong causes 0** (this includes the original 47+10 N-1/N-2 families), **112/112 benign accepted** (32+32+48), plus the five #325 benign 5/5; historical BASE still rejected. Exit 0.
- Roslyn suite (final publication run `E/out/roslyn-suite-final.stdout`): A **30/30** synthetic negatives; B **25/25** negatives rejected for intended cause incl. the preserved `A326-N06/N07/N08`, `L326-U01` and `R321-S05/S06/S07` witnesses, **10/10** benign overlays; C **18/18** fail-closed degradations (missing/corrupt/extra references, manifest tamper, wrong SDK/compiler, no dotnet, topology, compile error, tool crash, unreadable output); D end-to-end on the **real** `check_baseline.py`: candidate **10/0/2 exit 0**, **11** negatives **9/1/2** with only check 08 for the intended cause, **7** benign overlays 10/0/2, historical source **6/4/2 exit 1** with the Roslyn stage unreachable. Exit 0 (`ALL CHECKS PASSED`).
- Six real `RackEditing.SnapFar` cases PASS in every full-checker run under the cwd-pinned SDK 8.0.133 (host trace: `dotnet` selects `/usr/lib/dotnet/sdk/8.0.133`).
- **320** source-reference hashes unchanged (`roslyn-reference-manifest.txt` byte-unchanged, manifest SHA-256 `978123241d3d…`); C#9 language level, pinned compiler DLL hashes and unsupported-topology fail-closed behaviour re-verified.

**4. No silent rebaseline; no new semantic gap.** No pin, negative or fixture was weakened or re-baselined. Two test *expectation strings* in the new X330 cases were corrected after the first full suite run to match the actual (correct) rejection diagnostics — the mutations and their required rejections were unchanged (disclosed as author loop count 1 in the #332 ledger). One implementation defect (a reporter helper parameter type) was caught by the pristine gate before any test ran and fixed. No further semantic gap emerged; none was hunted.

## Commands and exits (summary; exact ledger in `E/out/commands.txt`)

| Command (cwd `E/sdk8`) | Exit | Result |
|---|---|---|
| `git apply` reviewer patch (verbatim A329-01) | 0 | applied diff SHA-256 = `0dc44957…` (identical to #329 pin) |
| old `check_baseline.py --rotation-state post-fix --instruction-state post-fix` (mutated) | 0 | 10/0/2 — false green; stdout SHA-256 `f9f0e3f9…` = #329 pin |
| old `objective_cta_roslyn_binding.py` (mutated) | 0 | 189 files, errors=0, 27 sites |
| new `check_baseline.py …` (mutated) | 1 | 9/1/2, only check 08, `[update site]` method/site cause |
| new `objective_cta_roslyn_binding.py` (mutated) | 1 | single UNMET `[update site]` |
| new `check_baseline.py …` (pristine candidate) | 0 | 10/0/2 incl. 6 SnapFar; evidence row `Objectives visibility update x1 -> ScenarioOneUiRoot.Update()` |
| `test_objective_cta_roslyn_binding.py` (full, final) | 0 | see item 3 |
| `test_objective_cta_source_check.py` (full) | 0 | 884/884, 112/112, historical rejected |

## Classification and limits

Source/offline evidence only: Roslyn = SDK 8.0.133 C# 9 compiler against hash-pinned references (a player-like single-`Assembly-CSharp` model), **not** Unity compilation, import, analyzers or generators. Nothing about native UI, rendering, input, focus, play mode, saves or performance is tested. Only `ScenarioOneUiRoot.cs` is judged for the pinned sites. The method-site rule binds the Objectives visibility update to the actual `Update()` member; it is not a general reachability/call-graph analysis (`Update` being called by Unity every frame is assumed by name/shape, not proven). **NOT RUN:** all Unity/Editor/lease gates, Workbench C–G, integrated A–O (15 gates), Legacy40/Reference save/load, Windows/Player builds. #287 remains NOT UNITY QUALIFIED; no integration or source acceptance is implied by this handoff.

## Publication

Branch `fix/cta-objectives-update-site-20261107-v1`: source commit `990ec43ec6bf5ec7063fb530f80b82e37501c582` (tested), then one handoff-only commit adding this file (publication adds only author handoff vs tested commit, verified by diff). [HANDED-OFF] on #330; the source lane is released read-only for the #331 independent reviewer, who must use its own lane and may never author or approve this patch. Manager #200 decides the CTA source gate afterwards.
