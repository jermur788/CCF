# Sen-1 #719 — Independent review of the Compact30 pre-attestation content-byte authority (#717) and D-032 sheet

**Verdict: NEEDS CHANGES (documentary policy and permission completeness).** 0 blocking defects of principle, 6 MAJOR, 8 minor, 4 notes.

#717's structure is correct. Choice A is the right shape and the safer option. It correctly treats the owner's "do scene in parallel" as not being a W §12 supersession, and it preserves every hard gate the packet lists. The proposed replacement text, though, cannot be signed as written:

- It names only three of the five W §12 ordering clauses that a content-byte phase must displace.
- It uses an open-ended "only to the extent necessary" qualifier.
- It removes the formal spec approval step without moving the content choices that the DRAFT graphics spec still holds into the grant.
- It is not bound to #718's exact five-file allowlist, which postdates it, or to separate art tiers.
- It has no provision for the authoring/readback tool and Unity import side effects that the scene writer will in practice need.

The D-032 sheet is complete in coverage but not staged. It does not tell the owner which answers are needed before production, before signature, or before any distribution. It also omits the scene/placement files themselves and the public-push consequence of any content branch.

Corrected owner-ready text is in §6 and §7. **No owner choice is made here.**

**Status: REVIEW ONLY.** I did not sign anything, act for the owner, modify #717 or any other source document, or touch Assets, Unity, art, source, specs, the Decision Log, active-tasks, Ops, saves or Windows. D-032 stays OPEN. Frozen #676 stays **SIGNED 0 / CANDIDATE 80 / BLOCKED 8**, `world_digest=null`. #690 is out of scope. Manager #700 alone decides disposition.

## 1. Reviewer registration and inputs

- **Actual reviewer:** Anthropic **Claude Opus 5.5** (`claude-opus-5-5`), Claude Code CLI on the owner's Linux host (T3 Code harness), reading GitHub via `git`/`gh`. One writer, no delegation. This is a different provider family from #716/#717 (OpenAI GPT-6) and #718 (OpenAI GPT-6.2 Sol). I hold no source or Unity custody on any Compact30 lane. I am not the #690 writer session (`8b161722…`).
- **Registered before any write:** [#719 registration](https://github.com/jermur788/CCF/issues/719#issuecomment-6965170267).
- **Branch:** `review/sen-1-719-compact30-authority-20261118-v1` from `main` @ `3e4563425a9151ee3c595651be06a683bde2db79`. This file is the only change.
- **Packet:** [`docs/sen-1-700-manager-intake-20261117-v1` @ `e9c5015e`](https://github.com/jermur788/CCF/blob/e9c5015e11eba05935bd52c62a68613247738028/Docs/Handoffs/ScenarioOne-Compact-Ecology-20261117/Sen-1-719-INDEPENDENT-COMPACT30-W12-D032-CONTENT-PRODUCTION-AUTHORITY-REVIEW-PACKET-20261118.md).
- **Read in full:** #717 sheet (branch `docs/sen-1-717-scene-authority-owner-decisions-gpt6-20261117-v1`); #716 eight-blocker result (branch `docs/sen-1-716-browser-eight-blockers-gpt6-20261117-v1`); #718 at `db5da692`; Manager intake verdict and #711 acceptance (#700 branch); frozen #676 W §§1, 11, 11.1, 11.2, 12 and 14 at `8617587e` (the #676 branch head is still `8617587e`, so it is unchanged since being frozen); #708 at `47be4f9b`; #704 issue thread (still NEEDS CHANGES, no corrected dossier yet); main Decision Log D-032/D-084/D-085 and active-tasks at `3e456342`; `Docs/Verification/S1B/RightsAudit.md`; the #690 source permit path list.

### 1.1 Checks I actually ran (read-only)

| Check | Result |
| --- | --- |
| The five #718 paths and the reserved art parents (`Assets/ForestPrototype/Scenes{,.meta}`, `Assets/ForestPrototype/Art/ScenarioOneCompact30{,.meta}`) on `main` `3e456342`, D-073 `55863f1b` and **every fetched `origin/*` branch** | **Absent everywhere.** Parent `Assets/ForestPrototype/` exists. All existing scenes live under `Assets/Scenes/` (overview lines 59–62). |
| #690 permit path list vs the five paths | **No overlap.** #690 is C# under `Assets/ForestPrototype/` (save/stand/manager/UI) plus `Tools/Verification/…` and one handoff Markdown. Note that it edits `ScenarioOneManager.cs`, `ForestStartingStand.cs` and `ForestSaveController.cs`, the bootstrap components the inert scene must not carry (m-9). |
| Installed URP 17.6.0 `Shaders/Lit.shader` and `Editor/AssetVersion.cs` SHA-256 and `.meta` GUIDs under `/media/jer/ZX20/Unity/6000.6.0f1/…/BuiltInPackages/com.unity.render-pipelines.universal` | **Match #718** (`1ec00dad…8628` → `933532a4…`; `96ed27e1…344c` → `d0353a89…`). Read with `sha256sum`/`grep` only, without launching Unity. This is static file identity, not import or compile proof. |
| Four-family unpruned+pruning FBX count/bytes at D-073 | **84 files, 69,546,784 bytes**, matching #718. |
| `SS_Stump_01.prefab` referenced by the historical spawner | Its model GUID resolves to tracked first-party `Art/SectionFive/Woodland_Assets/SS_Stump_01/SS_Stump_01_LOD0.fbx`. It is not Ultimate Nature. |
| `UserSettings/`, `Library/` | Git-ignored. |
| Repo visibility / licence (`gh api repos/jermur788/CCF`) | **public**, licence **null**. |

**Not run:** Unity, Blender, import or any native check, the archive `.blend` hashes, the 333+14 dependency re-pin, legal analysis. I give no legal opinion anywhere below.

## 2. Packet item 1 — W §12 circularity and Choice A vs B

### 2.1 What frozen W §12 actually says

The exact frozen sentence at `8617587e` §12 is one chain of **six ordered prerequisites** before "production source/scene/asset/save work":

| # | Clause (verbatim fragment) | Can it precede content-byte production? |
| --- | --- | --- |
| W12-a | "resolve exact content blockers" | **No.** Rows 12, 30, 44, 73–75 and 83 need the native bytes to exist. |
| W12-b | "obtain independent review" | **No** for the whole-world review: it reviews the completed successor. |
| W12-c | "and owner's complete exact-byte world attestation" | **No.** This is the circularity. |
| W12-d | "formally APPROVE the relevant Compact specs" | Possible, but slow. The DRAFT graphics spec holds open content choices. |
| W12-e | "obtain separate signed save-policy/serialization/CG-TRIG decisions" | Irrelevant to an inert scene with no save binding. |
| W12-f | "obtain actual prospective source and physical Ops custody/lease plus a Manager source-start packet for the exact candidate" | **Yes, and it must stay.** |

The same paragraph adds "ISO-58 remains BLOCKED and #652 live deployment HELD".

I confirm the circularity: W12-a/b/c cannot be satisfied without bytes that W §12 forbids producing. #711 (accepted) rules that only an explicit owner supersession resolves it. The 2026-11-17 owner request to build the scene in parallel (active-tasks line 8) is a scheduling instruction. It names no clause, file, scope or expiry, so it is **not** a supersession. #717 §2 and §3 say the same; I concur.

### 2.2 Choice A vs B

- **Choice A (narrow content-bytes exception): RECOMMENDED for presentation.** It keeps D-085 OD-14's single final manifest and the owner's single final personal signature. It leaves W §12 fully in force for everything outside the named phase. It adds no new identity concept.
- **Choice B (interim recipe identity plus final attestation): NOT RECOMMENDED.** It needs every decision A needs. #717 itself says B must also state the spec/save exception or close those gates. On top of that it needs an exhaustive exclusion list, an interim-vs-final materiality rule and an extra reconciliation review. It also creates a second signed-looking identity that someone could later mistake for the OD-14 manifest. It solves nothing A does not.
- **NEITHER:** STOP. No scene or art bytes. Rows 12, 30, 44, 73–75 and 83 stay permanently open. The Compact world cannot be signed.

The defects are in A's **text**, not its concept. See F-1 to F-5.

## 3. Packet item 2 — D-032 decision completeness

#717 §5 DR-1 to DR-6, and its separable rights (a)–(e), cover the subject matter the packet lists:

- the public repo and its missing licence;
- the already-public selected Sitka exports;
- the fact that tracked/public/archived bytes are not ownership evidence;
- internal use, compiled private beta, raw handoff, public Git and promotion;
- package/URP/third-party notices;
- new #688/#698 originality and custody;
- "20.5 / 3.25 MiB are not accepted";
- no LFS rewrite, no inferred right to publish binaries or `.blend` masters.

It makes no legal claim and does not treat the 2026-10-09 audit as a Compact licence. Its gaps are about **staging and omissions**, not wrong facts: F-6, m-1, m-2, m-3 and m-7.

## 4. Packet item 3 — #718 five-file inventory vs the eight blockers

### 4.1 Path status

All five paths are absent on current main, D-073 and every fetched branch (§1.1), and none overlaps #690. The five paths are:

1. `Assets/ForestPrototype/Scenes.meta`
2. `…/Scenes/ScenarioOneCompact30.unity`
3. `…/Scenes/ScenarioOneCompact30.unity.meta`
4. `…/Scenes/ScenarioOneCompact30Placement.json`
5. `…/Scenes/ScenarioOneCompact30Placement.json.meta`

`Scenes.meta` is a **new folder** meta and must have exactly one writer. The separate `Assets/ForestPrototype/Art/ScenarioOneCompact30.meta` parent, shared by #688 and #698, is **not** in the five and needs one named first writer in exactly one art allowlist (m-5). Absence must be **re-checked at the actual future parent immediately before the physical grant**. Today's check is a snapshot.

### 4.2 What a five-file fixture can and cannot close

| Row / blocker | Five-file scene alone | Needs additionally | Not closeable by any production |
| --- | --- | --- | --- |
| 10 B-W02-RIGHTS | Nothing | — | D-032 rights-holder and owner receipts only |
| 12 B-W02-SCALE | **Contributes** the scene where all-LOD height, DBH and crown can be measured natively | Native lease and witness. If the mesh does not fit, any fit adapter is a source change, which is **outside A** → STOP. A static fixture with baked transforms may not reproduce runtime `ForestTree` height scaling (m-6). | — |
| 30 terrain surface | **Cannot**: a ground material must be a real `.mat` asset, which is not among the five. Scene-embedded materials must be forbidden (F-5). | #688 ground mesh/material/mask deliverables, imported natively | — |
| 44 H1 surface | Cannot | #688 H1 apron/soft-edge deliverables | — |
| 73/74/75 guard effective/expired/failed | **Cannot**: G1 is empty at Year 0, so the Compact scene must contain **no** guards | #698 meshes/materials **and** a separately allocated witness mechanism (a preview scene or disposable fixture) outside the five | — |
| 83 B-W14-WHOLE | **Contributes** the scene/GUID/registry binding | All of the above closed, a complete successor M+C, independent exact-byte review, then the owner's personal signature | — |

**Conclusion:** the five-file fixture fully closes **zero** rows. It is a necessary input to rows 12 and 83 only. A grant limited to the five files must say so explicitly, and must say that rows 30, 44 and 73–75 need separately listed #688/#698 art and a guard witness mechanism (F-4).

## 5. Findings

### MAJOR

**F-1 — Replacement text displaces only three of the five W §12 clauses that must move.** #717 §3 table row A and §3.1 supersede W12-c, plus W12-d/e "to the extent necessary". They never name W12-a ("resolve exact content blockers") or W12-b ("obtain independent review"). Both precede production in the frozen sentence and both depend on the bytes, so a literal reader can still hold the phase blocked. **Fix:** the supersession must state that, for the named phase only, W12-a, W12-b and W12-c **move to after** content-byte production as the closure path. W12-f stays a **prior** condition, rendered as `[APPROVED TO START — CONTENT BYTES ONLY]`. See §6 text.

**F-2 — "only to the extent necessary for that phase" cannot be signed.** No one is named to judge necessity, so the scope stays open-ended. **Fix:** make it categorical and file-scoped. For the files on the attached allowlist, W12-d and W12-e are not prerequisites. For every other file, act or state, including any source, runtime, profile, save, gameplay, integration, build or distribution, W12-d and W12-e remain fully in force.

**F-3 — Bypassing spec approval also bypasses the content choices it would have settled.** Graphics DRAFT GQ-01 (W palette/native look), GQ-08 (H1/path single-plane approach), GQ-09 (C0 vs T1/0.190 m cap), GQ-10 (A8 endpoint) and GQ-11 (shelter silhouettes/inspect cue) are owner/spec choices the produced bytes will embody. The §8 form covers C0/T1/A8, G1 and H1 look, but not GQ-01 or GQ-11. §3.1 does not condition the grant on any of them. **Fix:** the A grant must record the owner's answer to each, **or** mark the affected bytes PROVISIONAL with a stated rework rule (any later change re-authors those bytes before the successor manifest). Later formal spec approval must then confirm or supersede those answers, not inherit them silently.

**F-4 — The allowlist is not bound to #718's five files or split into separately activatable tiers.** #717 predates #718. Its A scope column bundles scene, H1/path terrain, guards and materials into one undifferentiated class, and §3.1 refers to "specifically listed" files without a list. The packet requires #688/#698 art to stay separate from the five-file scene set. **Fix:** three tiers, each with its own exact path list, single writer and activation:

| Tier | Contents | Approved paths |
| --- | --- | --- |
| **S** (scene) | The five #718 paths verbatim | Listed above |
| **H** (#688 entrance/path art) | Ground, H1 and path art | Exact paths to be enumerated under `Art/ScenarioOneCompact30/EntrancePath/`; none approved yet |
| **J** (#698 juvenile/guard art) | Juvenile and guard art | Exact paths to be enumerated under `Art/ScenarioOneCompact30/JuvenilesProtection/`; none approved yet |

The grant must also name the guard witness mechanism. Granting S alone must state that rows 30, 44 and 73–75 remain open (§4.2).

**F-5 — The grant does not say how the work is actually done, so a writer must either stall or quietly widen scope.** Placing 175 ID-bound trees from a pinned manifest and reading them back realistically needs a temporary Editor authoring/readback script. #718 says this "would need a separate explicit allocation", and no such script is on any list. Opening a fresh worktree in Unity can also rewrite tracked `ProjectSettings/*` or `Packages/packages-lock.json`. D-073 already differs from main in those package lines. Finally, unsaved materials or meshes can be serialized **into** a `.unity` file. **Fix:** the grant must:

- (i) either allocate one named disposable tool under the documented `Tools/Verification/` copy/run/remove workflow, with zero residue under `Assets/ForestPrototype/` and no auto-start, or forbid tooling and accept manual authoring;
- (ii) make any change to a tracked file outside the tier lists a **STOP-and-revert** condition, never a commit;
- (iii) forbid scene-embedded materials, meshes and textures;
- (iv) define "inert": no `Game State` bootstrap, no `ForestSaveController`, `ScenarioOneManager`, `ForestStartingStand` or `ForestEcologyController` instances, no profile/slot/identity registration, no build-settings entry, no annual or save callbacks, and no serialized 36-cell ecological state (OD-03/OD-04 require the source constructor).

**F-6 — D-032 is not staged, and two exposure routes are missing.** §5 and §7 make "all rights-holder grants, D-032 public storage decision" a single prerequisite without separating what each step needs. As a result the owner cannot answer the minimum question needed to unblock production. Two items are missing:

- (a) a rights/custody line for the **new scene and placement files** themselves. B-W14 binds "actual separate Compact scene/art **and its rights**".
- (b) the plain consequence that, while the repository is public, **pushing any branch** containing produced bytes to `jermur788/CCF` publishes them. Merging is not required for exposure.

**Fix:** stage D-032 as P (before production), S (before the world signature, row 10) and X (before each distribution channel). See §7.

### minor

- **m-1** — The 2026-10-09 S1B audit records the owner's explicit confirmation of ownership/permission for **project-created procedural/generated art** for private compiled builds. #717 DR-1 correctly says this is not a Compact licence, but dismisses it as "a different build". Present it as an existing receipt the owner may **explicitly extend** (e.g. to internal Compact production), not as irrelevant.
- **m-2** — Fold #718 evidence into DR-1 and DR-4 with correct grading. DR-1: the nine archive `.blend` candidates under `/media/jer/Files/CCF/Archives/UnityAssets` and the 84/84 FBX↔archive byte match are **custody leads, not title**. DR-4: the installed URP `Lit.shader`/`AssetVersion.cs` identities (re-confirmed in §1.1) are **static provenance**, not import, compile or notice closure. Also carry the 66.33 MiB FBX-only figure beside #708's ~107.7 MiB.
- **m-3** — DR-4 should require that the dependency closure of every tier be checked for Asset Store/third-party content, specifically Ultimate Nature. The historical spawner's `SS_Stump_01` is first-party (§1.1), so no live violation was found. The rule should still be explicit.
- **m-4** — `ScenarioOneCompact30Placement.json` must be **derived from, and SHA-256-bound to, the independently reviewed successor M/C**. It must never become a second authority for roster, positions or cells.
- **m-5** — The folder choice `Assets/ForestPrototype/Scenes/` (new) differs from the existing `Assets/Scenes/` convention. It is acceptable, since it isolates Compact from historical scenes, but the Manager should confirm it explicitly. `Art/ScenarioOneCompact30.meta` needs one first writer in Tier H **or** J, not both.
- **m-6** — Row-12 witnesses taken from a static fixture count only if they measure the same presentation path the runtime uses (`ForestTree.ModelForHeight`/`ApplyVisualOverride`), or if a later runtime re-witness is required. Otherwise the fixture certifies transforms nobody ships.
- **m-7** — DR-5 option (iii), changing repository visibility, should carry #716's sentence: a later visibility change does not retract existing public exposure, clones or forks.
- **m-8** — §3.1 has no expiry placeholder or lapse rule. The exception should name a UTC expiry, a supervising person, and what happens to partly produced bytes on expiry (left unmerged, branch retained, no push unless P-3 allows).

### notes

- **N-1** — #717 correctly preserves: the eight BLOCKED fields 10/12/30/44/73/74/75/83; `world_digest=null`; the 175 roster `dac06a3b…` and 36 candidate cells; the 14 pre-layout H1 omissions (not felled, no revenue); four empty G1 sites at Year 0; OD-16 deferred; v1–v24 and historical worlds untouched; T1/0.190 m/A8 and guard look unapproved; the single-writer/lease/preflight/Manager-grant chain; #652 HELD; no Windows. No change needed.
- **N-2** — #690 is now `[APPROVED TO START — CONDITIONAL]` for a different Claude session, with a manually supervised **2026-11-19 00:00Z** expiry. It is NonUnity, but any future Compact content lane must re-verify board, lane and worktree disjointness and the machine-wide Unity lock at its own start.
- **N-3** — Because #652 is HELD, an A grant cannot be exercised until either #652 is released or the owner approves an alternative physical route. That is a separate owner/Manager decision and belongs in the form (it is in §7 P-5).
- **N-4** — The B2 margin figures (+0.1029378 m on C0/T1 vs +0.1209757 m straight) are analytic only and cited consistently across #716, #717 and #718.

## 6. Edited owner-ready authority choice — NOT EFFECTIVE, NOT SIGNED

> **Owner selects exactly one:** ☐ **A** (recommended) ☐ **B** (not recommended: needs everything A needs plus an exhaustive interim exclusion list and an interim/final rule) ☐ **NEITHER** (STOP: no Compact scene/art bytes; rows 12, 30, 44, 73–75 and 83 cannot close)

**Choice A — replacement text for the owner to sign (fill every ⟨…⟩):**

> I, the product owner, make a **limited ordering exception to frozen #676 W §12** for one named phase, **"Compact30 content bytes ⟨tier S / S+H / S+H+J⟩"**, expiring **⟨UTC date-time⟩**, supervised by **⟨person⟩**.
>
> 1. **Displaced ordering, this phase only.** For the files on the attached allowlist, W §12's requirements to *"resolve exact content blockers, obtain independent review and owner's complete exact-byte world attestation"* are moved to **after** content-byte production, as that production's closure path. *"Formally APPROVE the relevant Compact specs"* and *"obtain separate signed save-policy/serialization/CG-TRIG decisions"* are **not prerequisites for the allowlisted files only**.
> 2. **Retained before any byte is written:** W §12's *"actual prospective source and physical Ops custody/lease plus a Manager source-start packet for the exact candidate"*, in the form of a Manager **[APPROVED TO START — CONTENT BYTES ONLY]** naming one actual writer (provider/model/session/host), branch, starting HEAD, disjoint worktree, current Ops board row, a sole machine-wide Unity lock, `lane-status --preflight` READY/exit 0, and this expiry. Also retained: D-032 stage-P answers (§7); ISO-58 BLOCKED; #652 HELD unless ⟨alternative physical route⟩ is separately approved.
> 3. **Allowlist (exhaustive):** Tier S = the five #718 paths, verbatim and re-checked absent. ⟨Tier H exact paths or "not granted"⟩. ⟨Tier J exact paths or "not granted"⟩. ⟨Guard witness mechanism and its paths, or "not granted — rows 73–75 stay open"⟩. ⟨One disposable `Tools/Verification/` authoring/readback tool by exact name under copy/run/remove, or "no tooling"⟩. Any change to any other tracked file is STOP-and-revert, never committed.
> 4. **Content the bytes embody:** route ⟨C0 / T1 with 0.190 m cap⟩, A8 ⟨coordinates / provisional⟩, palette/look ⟨GQ-01 answer / provisional⟩, H1/path approach ⟨GQ-08⟩, shelter silhouettes ⟨GQ-11 / not in this phase⟩. Provisional items must be re-authored before the successor manifest if later changed.
> 5. **Inert:** the scene has no `Game State` bootstrap, no save/manager/stand/ecology controller instances, no profile/slot/identity registration, no build-settings entry, no serialized 36-cell state, no embedded materials/meshes, no guards or juveniles at Year 0, no marks, and no H1 stumps or Fell history. The placement JSON is derived from and hash-bound to the reviewed successor M/C.
> 6. **Not approved by this exception:** any DRAFT spec; save/serialization implementation; Work Plan, planting, Annual Review or any runtime; new source scripts other than item 3's disposable tool; profile activation; main integration; any distribution beyond D-032 answers; a Compact beta; any Windows build; any change to historical scenes, worlds, saves v1–v24 or frozen Reference Future v1.
> 7. **Still required afterwards, in order:** the eight rows closed on real evidence; a complete successor M+C under a new name; independent nonauthor exact-byte and blocker-native review; **my personal exact-byte world signature**; formal approval of the parent/planting/graphics four-file specs (confirming or superseding item 4); signed serialization bridge; separate source-start for runtime work. Any material deviation from item 4 or the allowlist stops the phase and returns to me.
>
> Signed ⟨name⟩ ⟨UTC date⟩ ⟨link⟩. Recorded by Manager ⟨#⟩ in Decision Log ⟨D-⟩. Independent nonauthor review of this filled text ⟨link⟩ before any start grant.

**If B is chosen**, the owner must additionally attach: the exhaustive excluded-byte list; the interim identity name/version; its relationship to the final OD-14 manifest; a forbidden-inference rule (the interim identity is never the world signature); and the same items 2–7 above.

## 7. Edited D-032 rights/storage checklist — staged, UNSIGNED

Each line takes an owner answer: **Yes / No / Restricted to ⟨…⟩ / Not now**, plus the receipt link. "No" or "Not now" at a stage blocks only that stage. Facts: the repository is **PUBLIC** with no licence; the four Sitka families (84 FBX, 66.33 MiB; ~107.7 MiB with textures/media) are already public in ordinary Git. Tracked, public or archived bytes are not ownership evidence.

**Stage P — before any content byte is produced**

- **P-1** Four Sitka families: does the owner confirm rights to **use them internally** in Compact production (extending the 2026-10-09 private-build confirmation, or a new statement)? Name the creator/rights holder, including any AI tool or provider used to generate them and under whose terms, as the owner states it.
- **P-2** New scene/placement files (Tier S): owner states who creates them and claims what rights.
- **P-3** **Push channel:** may the content branch be pushed to the **public** `jermur788/CCF` remote, which publishes its bytes, or must it stay local / go to ⟨access-controlled remote⟩ until stage X? Answer per tier.
- **P-4** Tiers H/J, if granted: creator, input references and textures, editable-master custody location (Files CCF archive, not ZX20), originality statement, and the same push-channel answer.
- **P-5** Physical route: #652 release or a named alternative, and who is custodian.
- **P-6** Per-tier byte caps for this phase (the proposed 20.5 / 3.25 MiB are **not** accepted unless written here), and no LFS, `.gitattributes`, history rewrite or force-push.

**Stage S — before the owner's world signature (closes row 10 together with native rows)**

- **S-1** Per-family rights-holder receipt for each Sitka family, Tier H/J asset and the scene: creator, input/derivative chain, editable master path and SHA-256 (or "unavailable"), export SHA-256 and bytes, `.meta` GUID.
- **S-2** Third-party/package closure: Unity 6000.6.0f1 and URP/Core/ShaderGraph 17.6.0 notices actually read; dependency closure shows no Asset Store/Ultimate Nature or other third-party content, or lists it with its licence.
- **S-3** The channels the signed world is intended for (at minimum internal use plus ⟨compiled private beta?⟩). The signature does not grant channels not listed.

**Stage X — before each distribution channel, separately**

- **X-1** Invited compiled beta. **X-2** Raw FBX/PNG to collaborators. **X-3** Editable `.blend` to collaborators. **X-4** Continued or future public Git for each family and tier (and disposition of the already-public bytes: confirm / restrict going forward). **X-5** Promotional stills/video.
- **X-6** Repository visibility: (i) stay public, (ii) access-controlled repo/archive for new or retained masters, or (iii) change visibility, noting that this does not retract past exposure, clones or forks.
- **X-7** Measured per-file, per-family and aggregate budgets, retention, backup custodian and clone impact.

Some answers have legal consequences, and nothing here substitutes for them. If the owner cannot state rights for an input, that input is excluded or replaced by an approved original. The owner does not fill this in on anyone else's behalf.

## 8. Disposition requested

**NEEDS CHANGES.** F-1 to F-6 should be fixed in a corrected authority sheet before the owner is asked to sign. §6 and §7 are offered as that text, so a short author/Manager correction suffices; a re-derivation is not needed. A delta re-check by a non-OpenAI, nonauthor reviewer should confirm that the six MAJOR items are closed. Minor items may be closed in the same pass.

What stays unchanged in the meantime:

- Production remains **HELD**.
- #718 stays PHASE1 BLOCKED.
- #704 still owns the corrected D-032 dossier, whose F-1/F-2 public-repo corrections should feed §7.
- Frozen #676 is untouched: 0 / 80 / 8, digest null.
- No owner decision is made here.

Return to **Manager #700**. Dashboard/Drive synchronization **UNVERIFIED**.
