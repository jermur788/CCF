# Implementation notes — Scenario One UI Visual System V2

Source baseline: `13988874cc36413d911243f4d3a6524436237bab`.
Initial source checkpoint: `cf25d1b8d6ec5fa52356d70781cbf446da1f4075`.
Context: `d5f11f865dc9123ac3ff0bab571e0402ad03d276`.
Status: VERIFIED — independent G2/G3 APPROVED at `36eafce8656cc52c370c383f496ac6433cc5a028`.
Final verdict: **UI VISUAL SYSTEM V2 QUALIFIED — READY FOR THREE-PAGE ONBOARDING**.
This is feature-candidate qualification; no G4 integration, human playtest or Windows build is claimed.
Evidence root: `/media/jer/Files/CCF/Verification/ScenarioOneUiVisualSystemV2-20261025/`.

## Canonical palette

Values live in `Assets/ForestPrototype/UI/Resources/ScenarioOneUi.uss`; Painter2D/world-feedback values mirror them in UiKit. Image coordinates refer to the immutable1536×1024 approved reference. Sampling counts/bounds are preserved in evidence `reference-sampling.json`. Texture, alpha and antialiasing mean the sampled rendered pixels are not themselves flat-source colours.

| Token | RGB / RGBA | Intended role | Reference region / derivation |
|---|---|---|---|
| panel-bg |21,28,30,.94|HUD/translucent panels|Legend interior, x1280–1500/y680–870; most common21–22,28–29,30–31|
| frame-bg |21,28,30,1|Modal/readable base|Title box x50–245/y24–80 and Legend interior|
| surface-section |27,35,38,1|Quiet secondary surface|Slight lifted slate relative to Legend base; calibrated separation|
| panel-border / frame-border |109,125,132,.46|1-unit panel frame|Thin cool edge of Legend/Minimap; alpha calibrated against base|
| frame-rule |109,125,132,.30|Quiet separators|Legend rule x1197–1505/y411–414, composited near54–58/61–64/63–66|
| backdrop |7,12,16,.64|Modal environment scrim|Dark surrounding map forest; readability calibration, no scene change|
| ink |235,237,233|Primary off-white|Griffin title/Legend type; neutral contrast calibration|
| ink-muted |174,185,189|Secondary text|Legend supporting labels and survey labels, cooled for legibility|
| ink-faint |151,165,171|Metadata/captions|Survey labels and grey contours, increased contrast for720p|
| accent |110,163,157|Active progress/focus/competitor listings|Lightened blue-green relative to active Map tab and cool water/linework; contrast calibration|
| active-bg |56,89,75|Active navigation/primary control|Map tab x60–128/y90–120, modal values56/88–90/71–73|
| active-hover / active-pressed |65,102,88 /44,72,64|Interactive feedback|Lighter/darker calibrated variants of the active Map tab|
| btn-bg / hover / active |30,39,42 /41,53,57 /25,34,37|Calm inactive controls|Inactive tab x140–470/y90–120, cooled slight slate lift|
| btn-border |109,125,132,.40|Control boundary|Same canonical panel edge with reduced alpha|
| disabled-bg / border / ink |27,34,37 /66,79,85 /146,160,166|Locked/disabled information remains visible|Variants of panel slate and survey-grey; contrast calibrated|
| good |151,188,165|Positive money/forestry charts|Muted forestry green, compatible with reference active tab; semantic calibration|
| warn / bad |229,182,106 /221,145,135|Warnings/errors only|Amber related to waypoint marker; red is retained semantic warning vocabulary, not sampled decorative reference colour|
| waypoint |239,180,78|Waypoint text/arrow|Amber pins near x746/y410 and x1370/y182|
| select / select-ink |222,164,58 /29,32,31|Inspected-tree ring/edge/selected competitor tag|Existing tree-selection hue retained for semantic continuity; close to reference waypoint family; dark ink prevents low-contrast numbers|
| crop / fell / rack |70,130,230 /214,70,58 /182,118,232|Existing management marks|Existing gameplay-feedback semantics preserved; crop related to reference blue player marker. Fell/rack are not sampled from reference and are used only as semantic marks|
| chip-bg |174,185,189,.09|Restrained badge background|Same grey family as labels/rules; semantic border identifies each badge|

Opaque token contrast checks: ink14.65:1; muted8.61:1; faint6.81:1; disabled6.41:1 against frame; ink on active6.60:1; selected dark number on ochre7.41:1. These are static calculations. Actual selected competitor-row text on rendered RGB(60,66,67) is8.68:1 at both resolutions; the selected world-number tag is7.41:1. Both were independently checked.

## Typography and assets

Major `.title` labels use unmodified Liberation Serif Regular via existing UiKit.Text builder and `FontDefinition.FromFont`; dense body/control/value text keeps the runtime default sans. Cash values using `money` plus `title` are deliberately excluded from serif. Existing26-unit major-title scale,20-unit section scale,18-unit body and15–17-unit secondary/caption scales remain. Compact720p reading rules remain. Major titles use regular weight and0.2-unit tracking; section hierarchy uses weight/spacing and off-white rather than gold.

Reference raster does not identify its exact font family. Liberation Serif is a restrained Times-compatible editorial face available locally, chosen to reproduce the supplied serif title character without applying serif to small dense controls. This is not a claim of exact font-family identity.

Font source: `/usr/share/fonts/truetype/liberation/LiberationSerif-Regular.ttf`.
Font size:393,692 bytes; single face, no large asset family or package.
Font SHA-256: `705903ae1382f0150f115d6f10a30f5b2f3d6ade649c474afe6366c39174fb65`.
Licence confirmation: `/usr/share/doc/fonts-liberation/copyright`, Files:* SIL OFL1.1 explicitly permits embedding/redistribution with notice. Licence/copyright copied beside the unmodified font. No external download or new package. Unity6000.6.0f1 generated and imported the new metadata. Font GUID `9a6199e57e95fb29ca7c37b52627e93e` is retained, includeFontData1; no existing GUID was replaced.

## Scope and deviations

No material product/layout/palette deviation intentionally taken. Font rasterisation/unknown exact source font is recorded above. Strong visual fidelity passed independent G2 review of all32 final captures at both sizes plus16 representative later-stage captures; all94 comparison-sheet hashes/native actual pixels were independently checked.

The existing onboarding illustration content/palette, copy and page sequence are excluded by the approved spec and remain unchanged. Its gold labels/forestry greens are illustration content, not defaults for UI headings. Neutral overlaid labels in the separate Objective diagrams now consume shared off-white/muted USS tokens; their mesh geometry and semantic forestry colours are unchanged. Map cartography/data/transforms/symbols/terrain imagery remain untouched; shell-only styles use shared tokens. World selection and mark semantics remain distinct.

Only presentation files, the supplied spec/references, one licensed font and disposable verification tools change. No gameplay, save/schema, forestry model, progression, annual simulation, economy, package or scene change is authorised or made.

## Verification and reviews

External runtime type gate158sources/116references:0errors at5376319. Initial relative-source invocation failed because external compiler paths resolved relative to its output directory; rerun with absolute --src passed, without source/tool changes. It is not a Unity gate.

Independent source review round1 (GPT6.1 medium) found selected competitor tag text contrast1.88:1; cf25d1b adds dark selected text. Independent round2 (DeepSeek V4.1 Flash Go) confirms7.41:1, correct cascade, no remaining source blocker: SOURCE GATE APPROVED. No runtime/render acceptance implied.

User authorised exact preservation/temporary restore of four pre-existing U1 settings/material files for clean Unity verification, then restoration of the originals. Copies/hashes remain in evidence `preserved-user-files/` and `preserved-user-files.json`.

Actual qualification completed:

| Gate | Result | Evidence directory |
|---|---|---|
| Unity import/compile |PASS, exit0,92s; font imported/metas retained|01-import-compile|
| Main final Editor render/nav/console |PASS,32captures,355checks,0fail,exit0,183.16s|05-final-render|
| Existing later-stage UI fixture |PASS,62captures,888checks,0fail,exit0,370.04s; all8Objectives, WorkPlan0/5/7|04-later-stages|
| Driver cleanup regression |2/2 synthetic cases; denied cleanup cannot PASS; no realUnity/licence used|runner-cleanup-fault-check|
| Independent G2/G3 final review |APPROVED, exact36eafce; all32final and16later images read;94sheets verified|ui-v2-rendered-review-02-result.md|

Pure UI navigation and competitor restoration retain world hash30B1C4735B673ED2; deliberate advance adds exactly one ecological year and annual report, then review preserves hashA4518DA0E7C48664. These are local same-configuration mutation checks, not replacement protected simulation anchors.

Earlier failed clips/contrast and generic-driver exit143 are preserved, with fixes and rerenders. No assertion or acceptance threshold was weakened. The final runner isolates owned processes and requires both harness and copied-licence cleanup. Normal machine-wide locks were released; disposable script/meta removed. Four original user files are restored from exact hashed copies after verification. Attributed Editor material/settings side effects are archived and excluded from commits.

No material spec deviation remains. The exact raster font identity was unavailable; the licensed regular serif choice and contrast calibrations are documented above. Existing onboarding copy/sequence/illustration, semantic forestry art, map content and all nonvisual code remain unchanged. A higher-resolution/native-OS/Player-performance pass and human playtest are outside this qualification and are not claimed. Unity can update host Editor window layout/preferences during these graphical tests; game save/preferences used isolated config.

The durable `HANDOFF.md` records source/result checkpoints, exact replay commands/manual steps, restoration checks, backup and final dashboard publication/readback. The permanent source notes do not stand in for that operational receipt.
