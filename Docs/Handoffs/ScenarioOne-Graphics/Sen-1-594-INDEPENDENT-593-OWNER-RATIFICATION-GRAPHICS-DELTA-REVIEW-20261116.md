# Sen-1 — #594 independent review of the #593 owner-ratified Compact30 scene and graphics design delta

- **Claim:** [#594](https://github.com/jermur788/CCF/issues/594). Parent: Scenario One Manager [#424](https://github.com/jermur788/CCF/issues/424). Author under review: [#593](https://github.com/jermur788/CCF/issues/593) (#424 GPT-6 browser worker).
- **Reviewer (actual):** Anthropic **Claude Opus 5.5** (`claude-opus-5-5`), Claude Code CLI on the owner's local Windows machine, using git and the GitHub REST API. I am not the #593 author and did not author the #587/#590/#592 DRAFT.
- **Registration:** [#594 pre-edit registration comment](https://github.com/jermur788/CCF/issues/594#issuecomment-6916470910), posted at 2026-11-16 12:30:40 UTC **before** this file was created. The review branch was created and pushed unmodified before the comment.
- **Review branch:** `review/sen-1-594-owner-compact-scene-visual-delta-20261116-v1`. **Starting HEAD:** `ff7b1f1e3b132040acbed8fef6bea4b45bf84b8a` (the final #593 tip).
- **Worktree:** a separate local sparse docs-only worktree with only `Docs/Handoffs/ScenarioOne-Graphics/` and `specs/scenario-one-compact-graphics-v1/` checked out. No Unity Editor was opened and no lease was taken.
- **Write allowlist:** this one file.
- **Verdict scope:** DRAFT document quality of the material owner-ratification delta **only**. This verdict does **not** approve the specification, the implementation, the assets, the save behaviour or the native result.

## Verdict

**APPROVED FOR OWNER-DESIGN DRAFT QUALITY ONLY**, with **0 blockers, 0 majors, 4 minors (all nonblocking) and 3 nits**.

The #593 delta records the seven owner choices accurately as product/style direction. It does not elevate any of them to approved technical, asset, save or native status. Every implementation-relevant gate remains in force: GQ-07 source/scene bridge, GQ-02 failed-guard geometry, the signed manifest, D-032, ISO-58/63, Compact profile activation and owner rendered acceptance. All 20 FR, all 12 AC and all 66 unchecked checklist items survive unchanged in count and ID order.

The four minors are precision issues. None of them creates false authority or removes a gate, and every task they touch is still BLOCKED. They should be folded into the next amendment pass (the minors marked "graphics" into this four-file DRAFT, M2 into the staged parent bridge) **before** #424 marks the specification APPROVED. They do not need another review cycle on their own.

`SPEC-SEN1-COMPACT-GRAPHICS-V1` remains **DRAFT — NOT APPROVED**.

## Baseline, candidate and diff integrity (packet check 8)

| Check | Result | Evidence |
| --- | --- | --- |
| Candidate tip unmoved | **PASS** | `origin/docs/sen-1-593-owner-compact-look-scene-ratification-20261116-v1` = `ff7b1f1e3b13…` at review start. This matches the #593 closure comment and the #594 issue pin. |
| Baseline | **PASS** | #592 head `6e4ba15893cc785d4439d1bb0509c3db200c312f` = `origin/docs/sen-1-592-591-review-minor-tidy-20261116-v1`, and it is an ancestor of `ff7b1f1e`. It matches the #593 [RUNNING] claim. |
| Changed file set | **PASS** | `git diff --name-status 6e4ba158 ff7b1f1e` shows exactly **M** `specs/scenario-one-compact-graphics-v1/{spec.md,plan.md,tasks.md,checklists/requirements.md}` and **A** the #593 result plus the #594 packet. That is +145/−51 lines. There are no source, scene, asset, binary, `.meta`, ProjectSettings, save or main-branch changes. |
| Protected sibling DRAFTs untouched | **PASS** | The parent/child #580 branch head is still `974ae8b9a8f2…` (last commit 09:48 UTC, before the owner's 12:09 instruction). No #593 commits touch it. |
| Status headers | **PASS** | spec.md:4 `DRAFT — NOT APPROVED / NO PRODUCTION, ASSET OR UNITY AUTHORITY`. plan.md:4 and tasks.md:4 `DRAFT — NO IMPLEMENTATION AUTHORITY`. requirements.md:4 `DRAFT / ALL UNCHECKED / NOT APPROVED`. These are byte-identical to the base. |
| 20 FR / 12 AC | **PASS** | Defined `- **FR-01..FR-20` and `- **AC-01..AC-12` bullets are counted identically on base and tip, and the FR ID sequence 01–20 is unbroken. |
| 66 unchecked / 0 checked | **PASS** | requirements.md has 66 `- [ ]` and 0 `- [x]` on both base and tip. The checklist ID sequence (`FR-*`, `WF-*`, `SA-*`, `AC-*`, `G573-*`, `NEG-*`, `V-*`) is **identical** between base and tip (mechanical `diff` of the extracted IDs gives no output). |
| Task release table | **PASS** | tasks.md:132–140: only T-01 is `YES docs/review only`. T-02..T-06, T-08 and T-09 are BLOCKED/**NO**, and T-07 is DEFERRED/**NO**. |

## Packet checks 1–7

### 1. Authority: "all of these" = the seven listed choices only — **PASS** (see N3)

- spec.md:16 names exactly seven choices: separate Compact30 scene; 1.2 m footpath starting target subject to clearance; worn-earth W; realistically proportioned partly see-through/mesh guards with path-safe failed states; no fresh stumps; no unnecessary boundary scenery; neutral daylight. It then states: "not approved exact meshes, collision, final guard dimensions, art/licences, content manifest, save system or Unity rendering".
- The same seven appear consistently in four places: the #593 issue body, the #593 result table, the #594 issue/packet and the owner summary relayed in this dispatch. I found no extra item in any of them.
- D-032 is not extended: spec.md:4/header D-032 **OPEN**, FR-18 (spec.md:68), GQ-03 (spec.md:133), V-02 (requirements.md:89) and the #593 result "Critical non-decisions".
- No professional forestry standard is claimed. FR-15 (spec.md:65) leaves "forestry/protection feasibility" as a technical/biological gate.
- See **M3** for one attribution blur ("optional" guards) and **N3** for the evidence limitation.

### 2. GQ-07 separate scene with a required parent source bridge — **PASS** (see M2)

- FR-09 (spec.md:59) records "OWNER-selected separate Compact30 scene; BLOCKING TECHNICAL CONTRACT". It requires the Compact scene to **exclude** `Dirt Path`, `Dirt Path X###`, `Approach Track ###`, cutting/headland/feeder/rack objects and their runtime clones/templates. It keeps implementation BLOCKED until a "separately reviewed parent-world/technical contract" defines all of the following: scene entry/routing, the `ForestStartingStand` no-tree corridor, `StandMapSource`, `ScenarioOneManager.PrescriptionApproval`, tree/vegetation sources, player state, signed world/profile/save binding and old-world regression.
- Option B (`Renderer.enabled`) is explicitly "**NOT the selected design** … never an implicit alternative permitted to an implementer". This is repeated in AC-05 (spec.md:114), GQ-07 (spec.md:137), plan.md:15 and :27, the T-02 scope (tasks.md:29) and AC-05/G573-M2 (requirements.md:56, :68).
- No claim is made that the scene exists. plan.md:27 says "owner look selection does not make a Unity scene", and the save section (spec.md:83) says it is "to be safely created only after an approved scene/source contract".
- The parent bridge is **staged, not performed**. The #593 result's "Immediate follow-on" section lists it as a separate one-writer docs task. GQ-07 makes it a precondition of spec APPROVED, and T-02/T-04 dependencies (tasks.md:32, :58) require it "independently reviewed and accepted in parent Compact spec". The parent DRAFT was not silently edited or marked reviewed.

### 3. GQ-02 guards: principle approved, geometry BLOCKED — **PASS** (see M3)

- FR-15 (spec.md:65) separates the approved principle (realistic, partly see-through, juvenile visible, failed state off the path) from the unapproved specifics: material, height, radius, stake, openings, failed/expired envelope and browse meaning. It labels the current 28° / +0.25 m world-x failed visual a "**known visual source FAIL**". It forbids changing `failedYear`, browse efficacy, juvenile IDs/height or the 2+2 count.
- GQ-02 (spec.md:132): "No G1 path-clearance PASS yet". The same blocker appears in AC-02 (spec.md:111), the requirements FR-15 line (requirements.md:26), plan Phase 4 (plan.md:35: "current 28°/+0.25m source visual does not"), T-02 scope (tasks.md:29) and the T-06 scope/dependencies.
- There is no fake clearance PASS anywhere. The O1 −0.141 m core intrusion is retained in FR-05 (spec.md:55), GQ-02 and plan.md:18. The O2 0.155 m figure with verge is retained in FR-05 and plan.md:18 (see N1).

### 4. FR-04 / W path — **PASS** (see M4)

- FR-04 (spec.md:54) presents 1.2 m as the "first Compact design target". It retains the 1.8 m B2 **+0.021 m** no-verge figure and the 2.4 m O1/B2 centre plus all-four-guard collisions. It says "**Do not approve the 1.2m footprint until exact signed spline/mesh/collider/player and failed-guard envelopes are source/native tested**".
- FR-16 (spec.md:66) selects W and keeps the #577 scratch ~0.35 m soft edge as "inconsistent" with the 1.2 m core and the ≤0.20 m illustrative fringe. It requires "Adapt/taper W" and says the Blender render/ratios are not approved material or geometry. plan.md:33 says it is "not approved for direct import".
- N2 is an archived alternative, not a pending vote (FR-16; plan.md:33; T-01 tasks.md:16 "do not ask owner to choose W/N2 again"). A W/N2 comparison returns only "unless the chosen look fails". That is proportionate.

### 5. H1 / atmosphere — **PASS** (see M1)

- No fresh stumps and no phantom income: FR-02 (spec.md:52, unchanged), FR-17 (spec.md:67), GQ-05 (spec.md:135), plan Phase 3/5 and WF-01 (requirements.md:35).
- Minimal boundary and neutral daylight: FR-17, GQ-05 and plan Phase 5 (plan.md:37). T-07 is reclassified "DEFERRED / NOT IN FIRST BETA" (tasks.md:138), with T-07 scope reduced to a basic exposure check.
- No renderer-proof claim: FR-17 says "exact light/material/render parameters remain native/technical", and GQ-05 says "Native camera/light/scene parameters remain unverified".
- M1 notes that the separate-scene choice changes what "no global RenderSettings change" means.

### 6. Historic worlds, saves and rights — **PASS**

- The save section (spec.md:83) keeps ISO-58/ISO-63, workstream #544 T-03B/T-04/T-05 as separate, "**Compact profile activation stays BLOCKED**", a new manifest/content version for material geometry changes, and Legacy40/E80/MFM/Reference v1/v1–v24/`forest-save.json` unchanged. The separate-scene sentence adds a protection ("MUST not mutate historical scenes, profiles, frozen saves or global shared resources"). It does not remove one.
- D-032 remains OPEN in all four files and in the #593 result. #577 preproduction is still "input, not implementation permission" (tasks.md:9), and T-03 depends on D-032/licence (tasks.md:45, :134).

### 7. Consistency and proportionality — **PASS** (see N2)

- The counts and IDs are preserved (see the integrity table above). T-02..T-09 are still BLOCKED or DEFERRED behind an actual spec APPROVED, the bridge and leases.
- "Owner selected" is consistently kept separate from "owner rendered acceptance": requirements FR-16 says "not final rendered visual acceptance", AC-03 (spec.md:112) and AC-12 (spec.md:121) still require owner/user visual acceptance, and T-09 is unchanged.
- The final approval paragraph (spec.md:139) correctly lists the remaining preconditions: this targeted review, the parent bridge, the failed-guard solution, D-032, the signed manifest and explicit Manager authorisation.

## Findings

### Minor (nonblocking)

**M1 — With a separate scene, "global RenderSettings" no longer means global, so "neutral daylight" is underspecified (graphics DRAFT).**
FR-17 (spec.md:67) says "avoid modifying global URP/RenderSettings/camera defaults". The plan Camera/URP row (plan.md:20) still describes "Old scene Main Camera `m_ClearFlags: 2` Solid Colour … global `RenderSettings`" and was not updated by #593.

In Unity, `RenderSettings` (fog, skybox, ambient) and the lighting settings are **serialized per scene**. Read-only inspection of `Assets/Scenes/ForestTest.unity` (identical at `ff7b1f1e`) shows the following: `RenderSettings` at line 14, `m_Fog: 0` (:17), `m_AmbientMode: 1` (:27), the built-in default skybox material (:29), `m_LightingSettings: {fileID: 0}` (:97) and camera `m_ClearFlags: 2` (:4326).

A newly created Unity scene does **not** inherit any of these. A new scene uses Unity's own defaults (skybox camera clear, skybox ambient). So "neutral daylight" in Compact30 has to be **authored** in the new scene. It could also be misread as "touch nothing", which would silently give a different look from ForestTest.

The project-global items that must stay untouched are the URP asset, `DefaultVolumeProfile`, `QualitySettings` and `GraphicsSettings` (plan.md:48).

*Shortest correction:* in FR-17/GQ-05 and plan.md:20/:37, add one sentence along these lines. "The Compact30 scene's own serialized RenderSettings, lighting settings, directional light and camera clear are explicitly authored to a recorded neutral-daylight baseline (for example, matched to ForestTest's solid-colour clear, trilight ambient and fog off, or a reviewed neutral equivalent). Project-global URP/Volume/Quality/Graphics assets stay untouched." The plan.md:20 row should say that the ForestTest camera values are the **reference**, not the Compact camera.

**M2 — The staged parent bridge does not name the concrete scene-registration and routing touchpoints (parent bridge amendment, not the graphics DRAFT).**
FR-09 and GQ-07 require "runtime scene routing/initialisation" only generically. Read-only source inspection (identical at `ff7b1f1e`) found these touchpoints:

- `ProjectSettings/EditorBuildSettings.asset:7–10` lists **only** `Assets/Scenes/ForestTest.unity`. A separate scene cannot load in a Player build without editing this file. ProjectSettings is single-writer and protected by plan.md:48.
- `Assets/ForestPrototype/UI/StandaloneSessionMenu.cs:27–38` bootstraps a `DontDestroyOnLoad` session host once via `RuntimeInitializeOnLoadMethod(AfterSceneLoad)`.
- `StandaloneSessionMenu.cs:145–149` `Restart()` reloads `SceneManager.GetActiveScene().buildIndex`. That is a same-scene reload with no profile→scene mapping.

None of these is named in any of the four files, or in the #593 "Immediate follow-on" bullet list.

*Shortest correction:* when #424 dispatches the parent source/scene-bridge amendment, list these three touchpoints explicitly. That covers build-scene registration ownership (ProjectSettings single-writer), profile-to-scene selection at startup/restart/load, and persistence of the session host across scene loads. Optionally add a one-line pointer in GQ-07. No graphics DRAFT edit is strictly required.

**M3 — "Optional" guards are attributed to the 12:09 owner approval in two places, although protect-or-not is a pre-existing child-spec rule (graphics DRAFT).**
spec.md:16 and FR-15 (spec.md:65) correctly separate the owner-approved guard *look* from "The player retains a genuine optional management choice". The #593 result also says protect-or-not "**remains**" optional.

However, requirements.md:26 says "**Owner approved optional realistic partly see-through/mesh or vented protective guards**". T-01 scope (tasks.md:16) and plan Phase 4 heading (plan.md:35) fold "optional" into the owner choice in the same way.

The optionality actually comes from the child planting DRAFT. `specs/scenario-one-compact-initial-planting-v1/spec.md:25` at `974ae8b9` says "Present shelter for every sapling as an editable per-tree recommendation … user may reject". The owner summary in this dispatch ("realistic, partly see-through tree guards, with damaged guards kept off the path") does not itself say "optional".

The practical effect is nil today. However, owner-authority records should not merge provenance, because a later change to child-spec protection defaults would otherwise look like overriding an owner decision.

*Shortest correction:* in requirements.md:26 (and optionally tasks.md:16 and plan.md:35), reword to "Owner approved realistic partly see-through/mesh guards with path-safe failed state; protect-or-not remains the existing per-tree player choice (child planting DRAFT)".

**M4 — GQ-04's "small evidence-based adjustment allowed" does not say who authorises the adjustment (graphics DRAFT).**
GQ-04 (spec.md:134) and FR-04 (spec.md:54) say the owner allowed "small path/shelter adjustment if needed". FR-06 (spec.md:56, unchanged) correctly requires the worker to **STOP** and present alternatives rather than silently move planted coordinates. FR-04 ends with "until a reviewed layout amendment is warranted".

An implementer reading GQ-04 alone could take "adjustment allowed" as implementer discretion.

*Shortest correction:* append "via FR-06 STOP and a reviewed, versioned manifest amendment; never at implementer discretion" to GQ-04 (and optionally FR-04).

### Nits

- **N1** — The GQ-02 row (spec.md:132) dropped the base text's "0.155 m into O2 when 0.20 m illustrative verge applies". The figure is still in FR-05 (spec.md:55) and plan.md:18, so no information is lost. Re-adding it would keep GQ-02 self-contained.
- **N2** — The T-03 release row (tasks.md:134) says "owner look APPROVED". "Owner look **direction** APPROVED" would avoid any reading as final rendered acceptance (T-09). The guard art type (mesh versus vented tube) should also match the protection that the simulation actually models. This is an ecological-honesty point already gated by GQ-02's "owner-approved protection meaning" and FR-15's forestry/protection feasibility; it is noted here so the T-03 brief carries it.
- **N3 — Evidence limitation.** The owner's 12:09 words ("Okay, I approve all of these.") and #424's immediately preceding recommendation text are recorded only through #424's transcription. I could not independently see that conversation. The four GitHub descriptions agree with each other, and the owner summary given to me in this dispatch matches the seven items. Archiving the verbatim recommendation text in the #593 result, or linking to it, would make future authority checks reproducible.

## Not done / not verified

- No Unity Editor, C#, asset, scene, `.meta`, binary, save, Prefs, network asset download, Windows build, ISO-63, full #588/#591 rerun or 66-item acceptance run.
- No change to the four-file DRAFT, the parent/child #580 DRAFTs, canonical #585/D-083, main or coordination files.
- The source observations in M1 and M2 come from read-only inspection of tracked text files at a commit identical to `ff7b1f1e`. They are not runtime behaviour.
- Dashboard/Drive: **NOT VERIFIED**.

## Return to #424

**APPROVED FOR OWNER-DESIGN DRAFT QUALITY ONLY**, with 0 blockers, 0 majors, 4 nonblocking minors and 3 nits. The spec stays **DRAFT / NOT APPROVED**.

Recommended next steps:

1. Fold M1, M3, M4 and the nits into the next graphics four-file amendment.
2. Carry M2 into the staged parent Compact scene/source-bridge amendment dispatch.
3. Keep GQ-07, GQ-02, D-032, the signed manifest, ISO-58/63 and Compact activation as blocking gates.
