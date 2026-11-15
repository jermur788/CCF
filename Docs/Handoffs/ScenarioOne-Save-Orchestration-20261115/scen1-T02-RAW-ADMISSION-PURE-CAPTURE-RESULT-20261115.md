# T-02 raw admission and pure capture — source handoff

**Task #551, parent #544. Source handoff for independent review only. No integration or feature qualification.**

| Record | Actual value |
| --- | --- |
| Worker | OpenAI `gpt-6.1-sol`, HIGH reasoning effort, T3 Code through Codex; implementation sub-agent, sole source writer; no delegated workers |
| Branch | `feature/scen1-shared-three-profile-save-v2` |
| Worktree | `/media/jer/Files/CCF/Operations/CCF-Scen1-SharedSave-T01-20261115` |
| Shared Git store | `/media/jer/Files/CCF/Repository.git` |
| Starting HEAD | `da962372a7d45d8d049e9ad0dd2a657e5c339a2a` |
| Preserved partial checkpoint | `f14751b3bf793ff138419649000255d81638e7e4`, committed and pushed FIRST as instructed; exactly the three inherited partial files, 432 additions / 52 removals |
| Final source commit | `84a631d04350f7e50ab0a28e7ec8d2ec61658af0`; this result is the following documentation commit; its delivery SHA is recorded in the #551 receipt |
| Main refreshed before source commit | `a9b89ab83a34da9c68e39d90e2fb73085ea0943a`; zero `Assets/` delta from `fc18190b976f1363f1b5d1b0881810db2f8c6e39` |
| Authority | Signed four-file contract at `200add4e6e5116268b79ef63131ff9b836f14c5d`, its external immutable D-A attestation, T-01 Manager construction packet, live main reservation, #544 two-menu grant and latest direct user instructions |
| Execution | Offline source/.NET only. No Unity, Editor, Player, game run, package installation, real save/pref write, #443 merge/copy, board change or Primary access |
| Durable external evidence | `/media/jer/Files/CCF/Verification/SharedSaveT02-551-20261115/resume-3/` |

Project documents were obtained using `git show da962372:<path>`; live coordination was read from fetched `origin/main`. The stale Primary checkout was not consulted or modified. The signed four-file contract is **not present on this source branch**; it was read from the exact signed commit, together with its external attestation. No contract file was added or edited. The task-specific offline ownership reconciliation was applied without claiming physical CCF-Ops mirror synchronization. Prior sessions were stopped, and no real competing source writer was observed. The menu files initially had no local modifications. Fresh #551 [RUNNING] receipt: https://github.com/jermur788/CCF/issues/551#issuecomment-6902605096.

## Narrower allowlist actually used

Only these five C# files and this one Markdown changed relative to the T-01 base. Optional ceiling paths were unnecessary; no new helper asset or `.meta` was created.

| File under `Assets/ForestPrototype/` | Methods / bounded changes |
| --- | --- |
| `ForestSaveController.cs` | Nested `CapturedSource` / `AdmittedSave`; `Save`, `TrySave`, `CanOperateOnSelectedWorld`, `CaptureData` and unchanged-body `CaptureWorldData`; `Load`, `TryLoad`, `CanLoad`, `TryPrepareDiskLoad`, `TryReadSave`; `TryPrepareReference`, `ExtractReferenceWorld`; `LoadData`, `TryPrepareCapture`, `CanRestoreCapture`, `RestoreCapturedData`, `ApplyAdmittedData`. Existing writer body unchanged. |
| `ForestSaveValidation.cs` | Add `ValidateSelectedJson`, `ValidateHistoricalIdentityJson`, `ValidateEnvelopeStructure`, `ValidateExactKeys`, `ValidateNewShape`. All 25 pre-existing methods unchanged. |
| `ScenarioOne/ScenarioOneManager.cs` | `CaptureSaveData`, `InitializeNewScenario`, both public `RestoreSaveData` overloads, internal `RestoreAdmittedSaveData`, narrow preview entry/return admission. `Start` plus session-only `worldSetupComplete` flag were added to the declared method list in progress before that edit to avoid early canopy-sensitive initialization. |
| `UI/StandaloneSessionMenu.cs` | Only MainCard Load callback and `SaveAndClose`: admit before `Close`. |
| `UI/StandaloneSessionMenu.Startup.cs` | Only `ContinueSavedForest` and its explanatory comment: same-source admission before help/menu teardown; refusal retains current card and focus rather than rebuilding it. No button text, layout, keybinding, navigation or new UI. |
| Result | Exactly `Docs/Handoffs/ScenarioOne-Save-Orchestration-20261115/scen1-T02-RAW-ADMISSION-PURE-CAPTURE-RESULT-20261115.md`. |

## Implementation against the five requirements

1. **Raw new-envelope structure and identity.** `ForestSaveValidation.cs:63` loads original raw JSON with recursive duplicate-property errors, exact three root keys, exact seven identity keys, case-sensitive registered identity, signed numeric types/ranges, explicit tuple `(1,24)`, nested version/scenario agreement, recursive unknown-key/type checks and defined enum values. O2 teaching must be a present raw `null`; Compact requires an object, reserved geometry 2 and neutral historical rack/latch fields. `ValidateNewShape` at line 155 is **new-format-only** and is never used to tighten historical payloads. `ValidateSelectedJson` at line 22 keeps operational new profiles unavailable after structural inspection. Structural diagnostic success is not operational admission; no new authored geometry or typed world success is invented.
2. **Historical preservation and explicit foreign rejection.** `ForestSaveValidation.cs:32` observes raw nonempty string `scenarioOne.scenarioId` and rejects explicit foreign values, including ones hidden by later duplicate properties. Missing/null/empty remain observationally unchanged; there is no provenance inference or new historical required-field rule. The three reserved root markers together recognize the new disjoint-root envelope in a historical slot. A lone otherwise-unknown historical property is not treated as proof of foreignness. `ForestSaveController.cs:48` retains the six original raw validators in their original order; line 75 reparses the immutable source and only then maps parsed version <=0 to1 before the unchanged semantic/species checks at line 509. No new raw-version-presence requirement, future-version refusal, normalizer change or historical DTO field was introduced. The future warning remains at line 587. Native absent-field behavior is unmeasured.
3. **One source-bound boundary and typed closure.** `AdmittedSave` at controller line 31 holds private readonly raw text, context, registered profile and selected primary path. It exposes no mutable admitted payload. Before apply it reparses its own bytes and checks current context/definition/semantics. The brief mutable applying-manager guard authorizes only the matching management subrestore during that full-world apply (`TryGetManagementData`, line 111). The public management DTO overloads now refuse at `ScenarioOneManager.cs:1458`; first management mutation is behind the admission check at line 1473. Public controller `LoadData`, line 537, accepts only an unchanged original same-session `CaptureData` object recorded privately by reference, with matching source text, session and path; a clone, new DTO, mutation or stale context/path cannot manufacture provenance. No public raw/trusted-boolean escape was added. Menu actions use `TryLoad`/`TrySave` callbacks after admission. File replacement after a CanLoad observation is reread at the action; after action admission the apply consumes the admitted text without rereading replacement bytes. The internal archive route authenticates original embedded raw text against the frozen hash at line 436, rather than current-DTO reserialization. The internal capture return seam is restricted to registered same-session source evidence.
4. **Pure capture.** Manager `CaptureSaveData`, line 1405, no longer reaches `EnsureUnderstoreyGrid` or competition invalidation. The entire world capture body is otherwise byte-preserved after its private rename (`ForestSaveController.cs:255`). Capturing records private provenance but changes no world/scene/UI/pref/file data. Initial startup keeps existing `Start -> EnsureBaselineSnapshot -> RecordEcologicalSnapshot -> EnsureUnderstoreyGrid` timing after stand construction. A session-only flag (`ScenarioOneManager.cs:571`) permits explicit later reset initialization at line 671; it prevents the inherited partial patch from seeding understorey during Awake before canopy setup. Capture never repairs an inconsistent world. Save checks active context/definition/preview before capture and validates captured bytes before IO.
5. **Availability and F-1.** `ForestSaveProfiles.cs`, `ForestSaveSessionContext.cs` and envelope declarations are byte-preserved. Compact/O2 remain `IsAvailable=false`; all operational paths use `CurrentPaths` / `GetSelectedPaths`, which enforces `CanUse`. The only production `ForestSaveProfiles.GetPaths` call remains inside the guarded session-context accessor. No alternate path parameter, slot fallback, new world, geometry dispatch or availability switch was introduced.

Preview changes are admission-only: historical capability is checked before entry; both archive and pure return snapshot are admitted before applying the preview. Public Save/Load/LoadData refuse throughout preview. Refused return retains its snapshot, guard, year and prior status, with no false Returned message. This does not implement staged-world exception recovery.

## Route and first-mutation map

“First mutation” below excludes refusal diagnostic messages/logs and private provenance bookkeeping. It includes menu/help/focus, simulation, scene/input, prefs and file effects. The successful route remains in-place application: T-03 exception atomicity is **UNVERIFIED**.

| Route / caller | Admission before effects | Earliest permitted mutation / refusal behavior |
| --- | --- | --- |
| F5, controller Update `:141`, or public Save `:147` | `TrySave :150`: selected available context, definition and preview guard; pure capture; raw + typed admission | Selected temp creation at controller `:208` (menu callback absent). Admission refusal writes no file. Existing T-04 IO behavior is outside this claim. |
| Menu Save, MainCard `:124`, `SaveAndClose :145` | Same `TrySave`; existing replacement confirmation unchanged | `StandaloneSessionMenu.Close :94` runs only from onAdmitted, then writer. Refused capture/context leaves menu, focus, scene/input and files intact. |
| F9, Update `:143`, or public Load `:374` | `TryLoad :381 -> TryPrepareDiskLoad :391 -> TryReadSave :410 -> AdmittedSave.Apply :94` | World begins at ecology `StandApproachExpected :595` / geometry `:596`; if no ecology, carried wood `:601`, then tree/object/management restoration. Refusal stays before all of them. |
| CanLoad and startup offer (`StandaloneSessionMenu.Startup.cs:124`) | Same selected read/prepare, no reusable permission token | None; no close, help dismissal, capture or apply. |
| Startup Continue (`.Startup.cs:89`) | A new TryLoad admission at click; same immutable bytes used by Apply | After admission only: help dismissal `:95`, CloseHelp `:96`, menu Close `:97`, then world apply. Refusal only logs; current card/focus/help and continue-state fields stay intact. |
| Session menu Load (`StandaloneSessionMenu.cs:123`) | TryLoad before Close | Close `:94`, then admitted world apply. No teardown on refusal. |
| In-session Continue/F10 | Existing `Close`; no save is read/applied | Explicit user navigation closes menu; it is not a disk-load route. |
| Public LoadData (`controller:537`) | Preview/active-context check, private original-capture identity/text/path check, then same AdmittedSave semantic boundary | Same first world mutation `:595`; unsupported arbitrary DTOs return false before any apply. |
| Public management subrestore (`manager:1458,1461`) | Unsupported standalone DTO origin always refuses | No mutation, including null/pre-v10 inputs; no TeachingWorldVersion increment, cache clear, browsing clear or initialization. |
| Internal management subrestore (`manager:1466`, controller caller `:757`) | Ticket must currently be applying this manager and still match context | TeachingWorldVersion `:1473`, then existing normalization/restoration. Null/inactive tickets refuse first. |
| Reference entry, WorkPlanView `:584` / public manager `:675` | Historical-only profile, existing archive identity, raw frozen hash, semantic admission, pure return-source check | First preview world mutation is the same admitted apply `controller:595`; Work Plan/preview guard/viewpoint publish only after successful application. Cache reads/refusal feedback are not world mutations. |
| Preview return, manager Update `:590` / public End `:729` | Original same-session snapshot through internal RestoreCapturedData `controller:544` | Same admitted apply; guard/snapshot are cleared only after success. Refused return retains retry state. |
| Backup | **No backup loader exists in this candidate.** TryReadSave consults backup existence only for an unreadable-file diagnostic. Shared raw/typed policy is the required seam for T-04. | No auto load/promotion/rename; unavailable backup recovery is not claimed implemented or qualified. Writer can still rotate backup on a successful save under existing T-04 code. |
| Start New / Restart | Existing early historical-only context from T-01; neither route consumes a DTO/file | StartNew hands over the already generated stand; Restart closes and reloads its scene. New-profile staging/bootstrap is T-03, not delivered here. |
| Direct verification scripts | Same public APIs; no special harness boolean or bypass | Cloned/mutated/constructed DTO calls now refuse. Unchanged original captures can use LoadData outside preview. Existing direct management calls refuse. These tests need separately authorised migration; their old outcomes are not represented as green. |

A complete literal caller index follows in the appendix, including Editor and non-Editor parse branches. It includes references in read-only Tools/Verification; no such script was run or edited. Reflection, dynamic calls, external assemblies and actual Unity callbacks are not proved by lexical inventory.

## Offline evidence and test matrix

**Status convention:** “observed” means the named external check actually ran; it is not Unity verification or an ISO acceptance result. All 63 signed ISO checkboxes remain unchecked. No Unity test, native serialization equivalence or complete save compatibility verdict is reported.

Commands executed in the source worktree, with all outputs under the external directory above:

```bash
python3 /media/jer/Files/CCF/Verification/SharedSaveT02-551-20261115/resume-3/prepare.py
dotnet run --project /media/jer/Files/CCF/Verification/SharedSaveT02-551-20261115/resume-3/generator/generator.csproj -- /media/jer/Files/CCF/Operations/CCF-Scen1-SharedSave-T01-20261115 /media/jer/Files/CCF/Verification/SharedSaveT02-551-20261115/resume-3
dotnet run --project /media/jer/Files/CCF/Verification/SharedSaveT02-551-20261115/resume-3/harness.csproj -c Release
python3 /media/jer/Files/CCF/Verification/SharedSaveT02-551-20261115/resume-3/audit.py
git diff --check da962372
```

Final assertion log: `run-7-final.log`, **352 assertions completed, zero failures**. `generator-final.log` records C#9 syntax checking under both symbol sets and the caller index. `audit-2.log` records static preservation checks. `tested-source-sha256.json` binds the executed source bytes to the source commit listed above. Earlier failed scaffold runs remain preserved: `run-2.log` lacked generated modifier whitespace; `run-3.log` lacked menu-shell symbols; `run-4.log` exposed the harness's character-vs-UTF8 FNV error. These were external harness corrections, not production fixes or accepted failed criteria. `audit-1.log` assumed the signed spec existed on this branch; corrected audit checks the immutable ref and continued absence instead. Earlier-session evidence is preserved separately and is not added to this session's assertion count.

The harness runs on .NET8 with external Newtonsoft.Json13.0.3; the installed Unity package/runtime was not loaded. It directly compiles the production validators, registry/context and DTO, with additional field types derived from source. `JsonUtility` is an explicit Newtonsoft fields-only **adapter**, not Unity. Controller boundary methods are source-extracted; engine CaptureWorldData and ApplyAdmittedData are replaced with explicit adapters. Manager capture uses its source-extracted method and real clone methods; public management guards and the admitted-management guard are extracted. MainCard/SaveAndClose/ContinueSavedForest and EndReferencePreview are source-extracted, with UI close/help, pose and world application explicitly stubbed. Update/OnGUI, full manager restoration, Resources/Unity archive admission and engine application do not run. Species lookup is stubbed; fixture differential species checking is permissive on both sides, not actual authored-species qualification.

| Case / criterion portion | Actual offline result | Unity/native status |
| --- | --- | --- |
| New raw exact roots, identities, unknown/case/missing/null/wrong types | Expected structural accept/refusal observed on original raw JSON | UNVERIFIED engine deserialization |
| Recursive duplicates, actual integer types/overflow, enums | Expected raw refusals observed, including nested records and signed64 overflow | UNVERIFIED engine paths |
| O2 explicit raw null versus absent/object/string/boolean/number/array | Structural distinction observed; availability remains false | O2 world unavailable; no native roundtrip claim |
| Canonical new envelopes to historical and six cross-profile placements | Historical raw denial observed; cross-profile diagnostic/availability refusal observed | No three-world/runtime placement claim |
| Historical explicit foreign identity, including duplicate-shadowed string | Raw denial observed before extracted-route mutation callbacks | UNVERIFIED native full-world sentinel |
| Historical absent/null/empty identity and raw version boundary controls | In-memory synthetic preflight preserves these cases; no missing-ID historical fixture manufactured | Authentic missing-ID v10–24 and native defaults UNVERIFIED |
| Missing versus0/negative/future99 historical version | Baseline/candidate raw-before-normalize differential agrees through same adapter | Exact Unity JsonUtility experiment UNVERIFIED |
| Genuine available fixture raw/typed differential | 15 rows agree on old raw validators and typed validators; all return no problem through the adapter | Actual load/restored-world facts UNVERIFIED |
| Same populated manager state capture | 13 management-bearing rows produce equal baseline/candidate adapter bytes; input unchanged | Full native same-state serialization UNVERIFIED |
| Capture purity on empty model2 understorey | Extracted capture does not call setup or alter fields; source call chain inspected | Native initial Awake/Start/reset/year progression UNVERIFIED |
| Public LoadData / manager direct routes | New, cloned, modified, foreign, zero-version untrusted DTOs; null/inactive tickets refuse as expected | Existing engine verification gates not run; migration required |
| Context and destination | Forged registry record, stale context/ticket and changed persistent root refused; cross-slot fallback absent | Actual profile switching/staging UNVERIFIED |
| Menu Load/Save/Continue denial | Extracted callers retain model menu/help/continue state; admission callback not invoked | Native focus/input/rendering/OnEnable behavior UNVERIFIED |
| File changes between observation/admission/apply | Changed-since-CanLoad source refuses at action; replacement after admission applies original admitted wood37 through adapter | Native file race/apply transaction UNVERIFIED |
| Preview | Public Save/Load/LoadData refuse; extracted failed return retains snapshot/guard/year/status; successful restricted return releases them after adapter | Full native entry/pose/return/exception recovery UNVERIFIED |
| Backup | Missing primary never auto-loads/promotes valid/invalid/foreign backup; bad backup does not hide valid primary | Explicit validated backup recovery NOT IMPLEMENTED (T-04) |
| Refusal sentinels | Model state/scene object identities+fields, selected context/profile, synthetic preference presence/values, UI flags and actual sandbox file hashes/presence unchanged around tested denials | Full FR-04 scene/ecology/pose/prefs sentinel UNVERIFIED |
| Availability / F-1 | New profiles stay false; only guarded context accessor calls pure GetPaths | Native startup selection UNVERIFIED |
| Preserved files and diff | Protected source/resources/normalizers/registry/meta anchors unchanged; no broad YAML/package/settings changes; diff check clean | Import/GUID behavior not run |
| C#9 | Five changed source files parse under Editor and non-Editor symbols | NOT Unity compilation/import |
| T-03 staging; T-04 atomic writer/recovery; T-05 ISO qualification | Not executed or claimed | UNVERIFIED / deferred |

Negative inputs exercised include: missing each root/identity key; explicit null in every root/identity position; wrong-case and unknown keys; wrong string/object/array/bool/number types; empty/whitespace/foreign/case-variant/trailing-space identity; mismatched nested chapter/profile/world/content/geometry; schema0/negative/future, payload counter0/negative/23/25, absent/0/negative/float/string/null/future nested version; int32 and int64 overflow; recursive duplicates; unknown enum; unknown Compact geometry; historical geometry in Compact; nonneutral rack/latch; null order records; malformed/trailing JSON; forged registry identity; canonical new envelope at a historical selected path; public direct LoadData with arbitrary/cloned/modified/foreign input; stale session/path/ticket; menu refusal; file replacement; no backup or cross-slot fallback. Missing/null/empty historical-ID and unknown-field controls are **synthetic in-memory JSON**, not provenance fixtures.

## Historical provenance and preservation

`fixtures-before.json`, `fixtures-after.json` and `fixture-results.json` contain each original path, hash, raw/typed result and adapter typed-state digest. No original fixture was modified or re-saved. Original file hashes were compared before and after. v12 raw snippets were extracted from the unchanged compressed archive; all four original FNV hashes match. Neither year/version headers nor frozen hash helpers were rewritten.

| Fixture row | Provenance | Original source SHA-256 |
| --- | --- | --- |
| v8 host archive | Archived host bytes; original gameplay construction not established | `ccb01a0125d4d55174b073cc7a0b45221b4e9e2f17bce6f8f0c4820914c5d303` |
| v9 host archive | Archived host bytes; original gameplay construction not established | `c0169790c8bba169ffd27c486e6a83bf67d6c938e170da58d42031bbcdf862a3` |
| v20 host archive | Archived host save-backup bytes | `7effe9b72c741d372efbb8acc1115d7be57a3514300da1d3d2de42801e565075` |
| v20-y5 | Earlier Unity writer output; T05 manifest source /media/jer/Files/CCF/Verification/ScenarioOneRackFirstUx-20261017/ux-run1/config/unity3d/DefaultCompany/CCF/forest-save.json | `af448bb297491d96bb18457ab2173694b2a0eafc2b8674d4438f25c9101eff00` |
| v21-y1 | Earlier Unity writer output; T05 manifest source /media/jer/Files/CCF/Verification/RackV2-CombinedRepair01-20261027T1845Z/unity-regression-model0-final/config/unity3d/DefaultCompany/CCF/forest-save.json | `a3aa334ffa585cc6db1f908860aa382ce3ae73d1e3a89bd2da2975369c5ba4d7` |
| v21-y0-rack | Earlier Unity writer output; T05 manifest source /media/jer/Files/CCF/Verification/RackV2-CombinedRepair01-20261027T1845Z/unity-combined-write-final/config/unity3d/DefaultCompany/CCF/forest-save.json | `2b89ee6303a3fcad3b9f5331dd74c2dc879bd37fb3865b94b5762ebc6d5e465a` |
| v22 recorded year3 | Genuine generated output; S1-C-07 Manager review and T05 capture report | `95e5dd37d7f546d78cb94cda8e5af1641b576c925cc37313ce88aa0a54167b9e` |
| v19 StormCoreVerification/resolved_storm.json | Tracked storm verification writer output; no user-save provenance | `5fc059ddaedcba659ca1ce96959db8c2012142da5c7683c594aba67fbac2015b` |
| v19 StormRngWrite/world-0.json | Tracked storm verification writer output; no user-save provenance | `041aaf5eff48885268a29d2f1b077cd9f648722976732e1cab38be5716b08920` |
| v19 StormRngWrite/world-1.json | Tracked storm verification writer output; no user-save provenance | `a304e38022c033ce6c3bdee380c14681d20b20de46308a70a57d115da61b3fdc` |
| v19 StormSalvageVerification/salvage_resolved.json | Tracked storm verification writer output; no user-save provenance | `17e916e0681d483e34d9bd7fe4b1ca101c90819480ea1a326c55e04d092a25f4` |
| v12 frozen year0 | Frozen generated archive; original embedded raw JSON, not reserialized | `789e3eddcf00d3d4d0fd18d078f7ab37a06d44aa9c8672cff564445b2bb041fc` |
| v12 frozen year20 | Frozen generated archive; original embedded raw JSON, not reserialized | `789e3eddcf00d3d4d0fd18d078f7ab37a06d44aa9c8672cff564445b2bb041fc` |
| v12 frozen year50 | Frozen generated archive; original embedded raw JSON, not reserialized | `789e3eddcf00d3d4d0fd18d078f7ab37a06d44aa9c8672cff564445b2bb041fc` |
| v12 frozen year100 | Frozen generated archive; original embedded raw JSON, not reserialized | `789e3eddcf00d3d4d0fd18d078f7ab37a06d44aa9c8672cff564445b2bb041fc` |

Host archive provenance is tied to the existing PersonalPlaytest-D073 backup manifests; this does not reconstruct how those original sessions were played. The v20/v21 writer copies are tied to the T05 fixture manifest and its original writer paths. The v22 row is tied to the published S1-C-07 Manager review and T05 capture report. v19 rows are tracked storm harness outputs, not claimed user saves. v12 is frozen authored-run output, not a fabricated save.

| Genuine-version coverage | Available evidence |
| --- | --- |
| v1–v7 | UNVERIFIED / no provenance-established fixture used |
| v8, v9 | One archived host row each; original gameplay construction remains unknown |
| v10–v11 | UNVERIFIED / no fixture used |
| v12 | Four frozen generated milestones |
| v13–v18 | UNVERIFIED / no fixture used |
| v19 | Four tracked storm verification outputs |
| v20 | One archived host backup and one earlier writer output |
| v21 | Two earlier writer outputs |
| v22 | One recorded generated year3 output |
| v23–v24 | UNVERIFIED / no genuine fixture used; current synthetic DTOs do not fill these cells |

This is **partial v1–v24 coverage**, not a complete historical compatibility finding. No genuine missing-ID v10–v24 fixture or raw-absent-version fixture was invented. Historical normalizers, geometry/storm/RackV2 validators, reference hash code, historical DTO and resources are byte-preserved; no native restore/migration equivalence is asserted.

## Not verified and residual risk

- No Unity compilation/import, Editor/Player/runtime/game run, native JsonUtility behavior or serialization equivalence; no movement/interaction, harvest-once/resource/construction, regeneration/year development, save/reload persistence, rendering, input or performance observation.
- No full FR-04 native sentinel. Offline world application, UI effects, species resolution and preference storage are adapters. No real PlayerPrefs/historical preference file was opened or written. The sentinel preference keys are synthetic probes in memory, not a census of real preferences.
- **T-03 remains necessary:** admitted data is applied in place. An engine exception after the first mutation is not rolled back; menu/help may already have been released after admission. No staged destination, full transactional preview, new-world bootstrap or three-world witness is delivered.
- **T-04 remains necessary:** the original unsupported-atomic-replace File.Copy fallback and overbroad IO-failure preservation message remain unchanged. No atomicity/crash/power-loss/permissions/commit-point safety or explicit validated backup loader is claimed. Do not release this branch as a finished safe writer.
- New-profile structural diagnostics do not enable either world. Exact authored geometry dispatch and any successful new-profile typed apply remain unavailable pending T-03/dependencies. CG-TRIG interactions/evidence-history credit and Compact/O2 content were not invented. No full teaching-evidence reconciliation or complete O2 is claimed.
- Existing direct verification callers that construct, clone or mutate DTOs, or call standalone management restore, now fail closed by design. Their assertions need an explicitly authorised migration to admitted isolated raw sources/restricted origins; they were not edited, run or labelled successful. The appendix identifies callers; it does not certify all old harnesses still work.
- First-setup/reset timing is source-reasoned; actual callback order and ecology/capture contents need native comparison. Early or externally reconfigured inconsistent worlds are not repaired by capture.
- Complete genuine corpus, authentic missing-ID history, independent exact-head review, Manager acceptance/integration, dashboard ingestion/Drive mirror and physical Ops mirror synchronization remain UNVERIFIED. All ISO-01–ISO-63 remain unchecked, including static/offline support portions recorded here.

## Deferred native checks for the next authorised verifier

These are proposed exact observations, **not runs or a Unity release**. Use Unity 6000.6.0f1 only in a separately assigned exact-source lane with current lawful lease, its own Library and disposable save/pref directory on Files. Never aim the old direct-DTO harnesses at a real save.

1. After ordinary ForestTest startup/baseline setup, capture the full FR-04 sentinel twice; call CaptureData/CaptureSaveData repeatedly and compare scene/ecology/understorey/competition, marks/orders/cash/year, UI/profile, prefs and all nine files. Repeat after explicit InitializeNewScenario in model2. Expected: only explicit setup initializes; capture changes none of the sentinel.
2. With copies of each known genuine fixture in the selected historical test path, compare baseline and candidate actual raw admission and restored-world facts, then compare same-state JsonUtility.ToJson(data,true) bytes. Run separate raw missing-version, version0 and negative-version experiments; record actual deserializer output before raw gates and normalization. Expected: preserved baseline outcomes, no forced missing-to-v1 mapping or automatic file rewrite.
3. At startup and F10, place canonical new envelopes and explicit-foreign legacy-shaped controls in historical primary. Click Continue/Load/Save as applicable; also call public LoadData with an arbitrary DTO and both public manager RestoreSaveData overloads. Expected: denial before world/UI active selection/help/prefs/file changes; no Loaded/Saved. Test genuine absent/null/empty controls only as labelled synthetic inputs, not provenance evidence.
4. Observe CanLoad on valid selected bytes, replace them before clicking Continue, then repeat with replacement after action admission using a bounded authorised hook. Expected: first case revalidates/refuses with unchanged screen; second applies the admitted bytes only. Native UI focus/input/OnEnable effects must be recorded.
5. From historical Legacy40 and Enlarged80, enter each hash-valid frozen milestone, deny a return before apply, then retry with the original source available. Expected: pure return snapshot, failed return retains guard/snapshot and blocks public disk/typed loads and saves; successful return restores exact world and pose before clearing guard. Engine exceptions require T-03 staging rather than a false success claim.
6. Keep Compact/O2 unavailable. T-03/T-04/T-05, new-world enables, writer interruption/recovery and complete ISO qualification require their own released packets. No Windows build or personal-playtest acceptance follows this source handoff.

## Complete literal caller appendix

Generated from current `Assets/ForestPrototype/**/*.cs` and `Tools/Verification/**/*.cs` under both Editor/non-Editor symbols, deduplicated by file/symbol/line. The index includes Save/Load instance receivers as written and excludes unrelated uppercase static resource/prefs APIs. It is a source inventory, not an execution claim. Public APIs can also have external/dynamic callers; these remain unobserved and encounter the same guards. F5/F9 unqualified controller calls and the production route table above complete the entry map.

| Caller file | Invocations (symbol: exact lines) |
| --- | --- |
| `Assets/ForestPrototype/Editor/RecentDeliveriesAssetSetup.cs` | `Load`: 49,50,51,52,53,54,103,206 |
| `Assets/ForestPrototype/Editor/SectionFiveAssetSetup.cs` | `Load`: 39,40,41,42,43,44,80,91,97 |
| `Assets/ForestPrototype/ForestSaveController.cs` | `CaptureData`: 157; `CaptureSaveData`: 287; `Load`: 144; `RestoreAdmittedSaveData`: 757; `RestoreCapturedData`: 540; `Save`: 142; `TryLoad`: 376; `TryPrepareDiskLoad`: 383,389; `TryReadSave`: 400; `TrySave`: 147 |
| `Assets/ForestPrototype/ScenarioHabitatPresentationVerification.cs` | `CaptureData`: 115,174; `EndReferencePreview`: 171; `TryBeginReferencePreview`: 129 |
| `Assets/ForestPrototype/ScenarioOne/ScenarioOneManager.cs` | `CanRestoreCapture`: 702; `CaptureData`: 701; `EndReferencePreview`: 590; `RestoreCapturedData`: 734; `RestoreSaveData`: 1459; `TryPrepareReference`: 696 |
| `Assets/ForestPrototype/ScenarioOneInteractionVerification.cs` | `CaptureData`: 85,91,129,313,330,399,404,448,455; `CaptureSaveData`: 287; `EndReferencePreview`: 88; `LoadData`: 133,143,315,331,378,437,452; `RestoreSaveData`: 289; `TryBeginReferencePreview`: 86 |
| `Assets/ForestPrototype/UI/StandaloneSessionMenu.cs` | `TryLoad`: 123; `TrySave`: 145 |
| `Assets/ForestPrototype/UI/StandaloneSessionMenu.Startup.cs` | `CanLoad`: 124; `TryLoad`: 93 |
| `Assets/ForestPrototype/UI/WorkPlanView.cs` | `TryBeginReferencePreview`: 584 |
| `Tools/Verification/BatchRecomputeVerification.cs` | `CaptureData`: 60; `LoadData`: 68,79 |
| `Tools/Verification/BrowsingProtectionVerification.cs` | `CaptureData`: 121,136,827,853,968,970,980; `CaptureSaveData`: 825; `LoadData`: 118,819,839,844,961,976; `RestoreSaveData`: 416,717,753,788,825,1025 |
| `Tools/Verification/CCFBeechVerification.cs` | `saves.Load`: 44,49; `saves.Save`: 17 |
| `Tools/Verification/CCFIntegrationVerificationTemp.cs` | `saves.Load`: 183,202; `saves.Save`: 171 |
| `Tools/Verification/CCFOakPlayerPlantingVerification.cs` | `saves.Load`: 159; `saves.Save`: 155 |
| `Tools/Verification/CCFOakVerification.cs` | `saves.Load`: 337; `saves.Save`: 330,333,349 |
| `Tools/Verification/CCFPlantingVerification.cs` | `saves.Load`: 435,456; `saves.Save`: 433,452 |
| `Tools/Verification/ClearanceVerification.cs` | `CaptureData`: 89,156,170,226,228; `CaptureSaveData`: 118,242; `LoadData`: 235,297,309; `RestoreSaveData`: 124,248,284; `saves.Load`: 227; `saves.Save`: 227 |
| `Tools/Verification/Enlarged80Calibration/AreaCalibrationReview.cs` | `CaptureData`: 83; `LoadData`: 87,102,108 |
| `Tools/Verification/Enlarged80Calibration/AreaCalibrationVerification.cs` | `CaptureSaveData`: 58,59; `RestoreSaveData`: 64,78,80 |
| `Tools/Verification/JuvenileMortalityFoundationVerification.cs` | `CaptureData`: 168,266,301,336,381; `LoadData`: 267,340,348,355,368,379; `RestoreSaveData`: 203,256,276,289 |
| `Tools/Verification/MenuTutorialVerification.cs` | `CaptureData`: 126,138,477,479; `EndReferencePreview`: 318; `LoadData`: 104,446; `saves.Load`: 298,303,582; `saves.Save`: 298,303,582; `TryBeginReferencePreview`: 316 |
| `Tools/Verification/Mfm09ExtensionFlow/Mfm09ExtensionGate.cs` | `CaptureData`: 277,311,404,428,598,604; `LoadData`: 119; `saves.Load`: 463,603; `saves.Save`: 457,599 |
| `Tools/Verification/Mfm12IntegratedQualification/Mfm11CalibrationImpactGate.cs` | `CaptureData`: 105,220,289,571,577,580; `LoadData`: 186,316,588; `saves.Load`: 579; `saves.Save`: 578 |
| `Tools/Verification/MfmApprovalBoundary/MfmApprovalBoundaryUnityGate.cs` | `CanLoad`: 386; `CaptureData`: 115,163,381,390,409; `LoadData`: 71,410; `saves.Load`: 387; `saves.Save`: 382 |
| `Tools/Verification/MfmApprovalBoundary/OfflineLogic/ApprovalBoundaryOfflineCheck.cs` | `Load`: 57 |
| `Tools/Verification/MfmCoverageWorld/MfmCoverageWorldUnityGate.cs` | `CaptureData`: 115,163; `LoadData`: 71 |
| `Tools/Verification/MfmCoverageWorld/OfflineLogic/CoverageWorldOfflineCheck.cs` | `Load`: 45 |
| `Tools/Verification/MfmPrescriptionCore/OfflineLogic/PrescriptionCoreOfflineCheck.cs` | `Load`: 70 |
| `Tools/Verification/MfmRackSample/MfmRackSampleGeometryVerification.cs` | `CaptureData`: 299,315,440; `LoadData`: 302,316,443 |
| `Tools/Verification/MfmSaveProvenanceUnity/MfmSaveProvenanceUnityCheck.cs` | `CanLoad`: 130,198,214,273,348; `CaptureData`: 88,92,159,167,187,219; `LoadData`: 77,252,341,353; `saves.Load`: 135,199,274,349; `saves.Save`: 191,269 |
| `Tools/Verification/MfmTreatmentReviewUnity/MfmTreatmentReviewUnityGate.cs` | `CaptureData`: 149,373,890,891,894; `LoadData`: 128,892 |
| `Tools/Verification/MfmUnity/MfmUnityCompileCheck.cs` | `CaptureData`: 97,239; `LoadData`: 68 |
| `Tools/Verification/ModelDefault/ModelDefaultVerification.cs` | `CaptureData`: 227,324,344,361,382,388,392,393,397,419,434; `EndReferencePreview`: 431; `LoadData`: 87,347; `saves.Load`: 169; `saves.Save`: 329; `TryBeginReferencePreview`: 424 |
| `Tools/Verification/Parity07Unity/Parity07UnityRender.cs` | `CaptureData`: 217,783; `LoadData`: 163 |
| `Tools/Verification/RackV2Combined/RackV2CombinedVerification.cs` | `CanLoad`: 528; `CaptureData`: 353,401; `saves.Load`: 529; `saves.Save`: 341,524 |
| `Tools/Verification/RackV2Combined/RackV2LegacyProgressionRegression.cs` | `CanLoad`: 198; `saves.Load`: 199; `saves.Save`: 196 |
| `Tools/Verification/RackV2ConfirmMinimap/RackV2ConfirmMinimapVerification.cs` | `CaptureData`: 189,194,450; `LoadData`: 238,457 |
| `Tools/Verification/RackV2Editing/RackV2EditingVerification.cs` | `CaptureData`: 184,189,514; `LoadData`: 233,521 |
| `Tools/Verification/RackV2FieldMarking/RackV2FieldMarkingVerification.cs` | `CaptureData`: 59; `LoadData`: 248,255,257,276,298,324; `RestoreSaveData`: 328 |
| `Tools/Verification/RackV2Headland/RackV2HeadlandQualification.cs` | `CaptureData`: 98,106,108; `LoadData`: 98,106 |
| `Tools/Verification/RackV2MapPlanning/RackV2MapPlanningVerification.cs` | `CaptureData`: 186,197,208 |
| `Tools/Verification/RackV2NoCompetingAuthor/GuidedRackV2Verification.cs` | `CaptureData`: 17 |
| `Tools/Verification/RackV2Objective1/RackV2Objective1EndToEnd.cs` | `CanLoad`: 636; `CaptureData`: 99,444,455; `LoadData`: 461; `saves.Load`: 637; `saves.Save`: 630 |
| `Tools/Verification/RackV2Persistence/RackV2PersistenceVerification.cs` | `CanLoad`: 228,238; `CaptureData`: 183,202,274; `LoadData`: 278; `saves.Load`: 229,240; `saves.Save`: 218 |
| `Tools/Verification/RackV2Runtime/RackV2RuntimeVerification.cs` | `CaptureData`: 258,267,273; `LoadData`: 261,282 |
| `Tools/Verification/RackV2Save/RackV2SaveVerification.cs` | `CaptureData`: 123,137,146,167,174,191,195,217,236,243,262,282,292,384,388,403,410; `LoadData`: 134,158,186,208,230,247,387; `RestoreSaveData`: 254,408,412 |
| `Tools/Verification/RackV2Spacing/RackV2SpacingVerification.cs` | `CaptureData`: 184,189,514; `LoadData`: 233,521 |
| `Tools/Verification/RackV2SuggestedLayout/SuggestedLayoutVerification.cs` | `CaptureData`: 63,64,75,77; `EndReferencePreview`: 160; `TryBeginReferencePreview`: 154 |
| `Tools/Verification/RegenerationBudget/RegenerationModelVerification.cs` | `CaptureData`: 468,500,523,525,529,697; `EndReferencePreview`: 557,727; `LoadData`: 488,503,526,534,538,542,549,700,727; `TryBeginReferencePreview`: 554 |
| `Tools/Verification/RngModelPolicyVerification.cs` | `CaptureData`: 77,131,212,214,220; `EndReferencePreview`: 62,152; `LoadData`: 62,93,129,138,145,195,217; `TryBeginReferencePreview`: 149 |
| `Tools/Verification/S1B/SessionMenuVerification.cs` | `CaptureData`: 46,51,57; `EndReferencePreview`: 59; `TryBeginReferencePreview`: 58 |
| `Tools/Verification/S1B/StandalonePlayerSmoke.cs` | `CaptureData`: 75,84; `saves.Save`: 73 |
| `Tools/Verification/S1CManagementHistory/AnnualReviewDrillDownVerification.cs` | `CaptureData`: 566; `CaptureSaveData`: 169,307 |
| `Tools/Verification/S1CManagementHistory/AnnualReviewGuidanceVerification.cs` | `CanLoad`: 270; `CaptureData`: 740; `CaptureSaveData`: 172,274,297,481; `LoadData`: 271 |
| `Tools/Verification/S1CManagementHistory/AnnualReviewProvenanceVerification.cs` | `CanLoad`: 311; `CaptureData`: 781; `CaptureSaveData`: 172,315,338,522; `LoadData`: 312 |
| `Tools/Verification/S1CManagementHistory/AnnualReviewYearBrowsingVerification.cs` | `CaptureData`: 743 |
| `Tools/Verification/S1CObjectiveHistoryUnity/S1CObjectiveHistoryRenderedVerification.cs` | `CanLoad`: 96; `LoadData`: 97 |
| `Tools/Verification/S1CObjectiveHistoryUnity/S1CObjectiveHistoryUnityVerification.cs` | `CanLoad`: 190,214,335,359,510,545,616,704; `CaptureData`: 185,194,206,255,265,311,531,578,912; `LoadData`: 341,361,516,550,615; `saves.Load`: 191,215,346,363,524,546,561,590,616,636,705; `saves.Save`: 187,211,263,541,635 |
| `Tools/Verification/SaveCellIndexHardening/SaveCellIndexHardeningVerification.cs` | `CanLoad`: 97; `CaptureData`: 78,145; `LoadData`: 100; `saves.Load`: 106,116 |
| `Tools/Verification/SaveHardeningVerification.cs` | `CanLoad`: 169; `LoadData`: 135,138,145,170; `saves.Load`: 118,126,131; `saves.Save`: 106,111 |
| `Tools/Verification/ScenarioOneCombined/CombinedHudComposition.cs` | `CaptureData`: 105; `LoadData`: 111 |
| `Tools/Verification/ScenarioOneCompletion/N3/N3CompletionVerification.cs` | `CaptureData`: 62,72,82; `CaptureSaveData`: 54,102; `LoadData`: 41,76; `RestoreSaveData`: 71 |
| `Tools/Verification/ScenarioOneCompletion/N3/OfflineLogic/N3OfflineCheck.cs` | `Save`: 56,58,60,62,63,74,81 |
| `Tools/Verification/ScenarioOneCompletion/P2/CropTreeCompetitorVerification.cs` | `CaptureData`: 229; `CaptureSaveData`: 114; `LoadData`: 234 |
| `Tools/Verification/ScenarioOneCompletion/P3/WorkPlanResidualVerification.cs` | `CaptureData`: 103 |
| `Tools/Verification/ScenarioOneCompletionVerification.cs` | `CaptureData`: 141,357,369,392,423,424,425,428,429,436,442,451,513; `EndReferencePreview`: 385; `LoadData`: 371,373,448,470,482,511; `TryBeginReferencePreview`: 381 |
| `Tools/Verification/ScenarioOneDeadwoodVerification.cs` | `CaptureSaveData`: 185,201; `saves.Load`: 188,210; `saves.Save`: 186 |
| `Tools/Verification/ScenarioOneEcologyViabilityDiagnostic.cs` | `CaptureData`: 88; `LoadData`: 74,93 |
| `Tools/Verification/ScenarioOneEconomyIntegrationVerification.cs` | `CaptureData`: 51,68,178,180,184,214,215,222,228,231; `EndReferencePreview`: 183; `LoadData`: 66,79,178,185,225,229; `TryBeginReferencePreview`: 181 |
| `Tools/Verification/ScenarioOneNonUi/OfflineLogic/CompetitorKeyboardOfflineCheck.cs` | `Load`: 26 |
| `Tools/Verification/ScenarioOneNonUi/OfflineLogic/RackBayOfflineCheck.cs` | `Load`: 118 |
| `Tools/Verification/ScenarioOneNonUi/UnityGates/CompetitorKeyboardVerification.cs` | `CaptureData`: 254; `LoadData`: 255 |
| `Tools/Verification/ScenarioOneNonUi/UnityGates/RackLessonVerification.cs` | `CaptureData`: 277,292,302; `LoadData`: 280,298,305 |
| `Tools/Verification/ScenarioOneNonUi/UnityGates/StandApproachVerification.cs` | `CaptureData`: 166; `LoadData`: 176,185,189 |
| `Tools/Verification/ScenarioOnePlantingVerification.cs` | `CaptureSaveData`: 174,211,243; `RestoreSaveData`: 218,241; `saves.Load`: 178,253,287,301; `saves.Save`: 175,249,279,297 |
| `Tools/Verification/ScenarioOnePresentationReview.cs` | `CaptureData`: 256,341,352,365,382; `EndReferencePreview`: 100,364; `LoadData`: 102,293,346,348,355,356; `TryBeginReferencePreview`: 361 |
| `Tools/Verification/ScenarioOneProfessionalMap/MapRedesignShots.cs` | `CaptureData`: 286 |
| `Tools/Verification/ScenarioOneProfessionalMap/ProfessionalMapVerification.cs` | `CaptureData`: 598; `LoadData`: 603 |
| `Tools/Verification/ScenarioOneProgressVerification.cs` | `CaptureData`: 86,92,102; `CaptureSaveData`: 217,222,230,235,241; `EndReferencePreview`: 99; `RestoreSaveData`: 232; `saves.Load`: 158,220,238; `saves.Save`: 157,218,236; `TryBeginReferencePreview`: 89 |
| `Tools/Verification/ScenarioOnePruningVerification.cs` | `CaptureSaveData`: 125,133; `saves.Load`: 128,142; `saves.Save`: 126 |
| `Tools/Verification/ScenarioOneRackUx/GuidedOnboardingVerification.cs` | `CaptureData`: 148,161,197,310,317; `LoadData`: 154 |
| `Tools/Verification/ScenarioOneRackUx/RackUxRack.cs` | `CaptureData`: 492; `LoadData`: 493 |
| `Tools/Verification/ScenarioOneRemovalVerification.cs` | `CaptureSaveData`: 182,189; `saves.Load`: 185; `saves.Save`: 183 |
| `Tools/Verification/ScenarioOneRepeatedInterventions/EventIdIntegrity/EventIdIntegrityOfflineCheck.cs` | `Save`: 70,71,72,73,74,75,76,79,81,89,91,93,95,97,99,100,101,104,105,114,116,118,120,122,125,128,136,149,154 |
| `Tools/Verification/ScenarioOneRepeatedInterventions/EventIdIntegrity/SaveEventIdIntegrityVerification.cs` | `CanLoad`: 117; `CaptureData`: 92,169; `LoadData`: 123; `saves.Load`: 133,144 |
| `Tools/Verification/ScenarioOneRepeatedInterventions/TeachingCapGateSetup.cs` | `CaptureData`: 14; `LoadData`: 19 |
| `Tools/Verification/ScenarioOneRepeatedInterventions/TeachingCapUnityVerification.cs` | `CaptureData`: 159,177,181,183,184,225; `LoadData`: 57 |
| `Tools/Verification/ScenarioOneTeachingCopyVerification.cs` | `CaptureData`: 86,119 |
| `Tools/Verification/ScenarioOneThreePageOnboarding/OnboardingVerification.cs` | `CanLoad`: 146; `CaptureData`: 101,138,153; `saves.Save`: 144 |
| `Tools/Verification/ScenarioOneUx/Qualification/UxQualification.cs` | `CaptureData`: 152,322; `EndReferencePreview`: 170; `LoadData`: 170; `saves.Save`: 629 |
| `Tools/Verification/ScenarioOneUxPedagogy/UxPedagogyShots.cs` | `CaptureData`: 95; `LoadData`: 470,500 |
| `Tools/Verification/ScenarioOneVisualSystem/VisualSystemFlowShots.cs` | `CaptureData`: 178,183; `saves.Save`: 179 |
| `Tools/Verification/ScenarioOneVisualSystemV2/VisualSystemV2Shots.cs` | `CaptureData`: 882 |
| `Tools/Verification/ScenarioReferenceVerification.cs` | `CaptureData`: 358,398,478,491,524,577; `EndReferencePreview`: 487; `LoadData`: 354,505; `TryBeginReferencePreview`: 482 |
| `Tools/Verification/ScenarioTwoSpeciesFoundation/UnityLogic/ScenarioTwoSpeciesFoundationVerification.cs` | `CaptureData`: 191; `EndReferencePreview`: 387; `LoadData`: 387 |
| `Tools/Verification/SequentialObjectives/SequentialObjectivesVerification.cs` | `CaptureData`: 184,186,199,202,211,221,347,348,366,374; `LoadData`: 200,214; `saves.Save`: 297,335 |
| `Tools/Verification/SitkaGrowthMortality/SitkaGrowthModelVerification.cs` | `CaptureData`: 90,101,276,320; `EndReferencePreview`: 127,343; `LoadData`: 103,106,109,111,117,120,279,343; `TryBeginReferencePreview`: 124 |
| `Tools/Verification/StandExpansion/StandExpansionAudit.cs` | `CaptureData`: 324; `EndReferencePreview`: 407; `LoadData`: 361; `TryBeginReferencePreview`: 388 |
| `Tools/Verification/StandGeometry/Enlarged80Verification.cs` | `CaptureData`: 78,272,293,308,313; `EndReferencePreview`: 339; `LoadData`: 281,301,318; `TryBeginReferencePreview`: 334 |
| `Tools/Verification/StandGeometry/StandGeometryVerification.cs` | `CaptureData`: 69,122,161,211; `EndReferencePreview`: 240,263; `LoadData`: 147,159,173,178,206,214,247,274; `TryBeginReferencePreview`: 237,257 |
| `Tools/Verification/StandMap/StandMapParity.cs` | `CaptureData`: 439; `LoadData`: 440 |
| `Tools/Verification/Startup/StartupScreenVerification.cs` | `CanLoad`: 359,396,622,698; `CaptureData`: 159,268,394,397,408,429,433; `LoadData`: 375,419,615,649,682,710; `saves.Load`: 701; `saves.Save`: 373,610,680,731 |
| `Tools/Verification/TeachingPersistence/PersistenceUnityVerification.cs` | `CanLoad`: 308,335; `CaptureData`: 278,287,294,300,313,318; `CaptureSaveData`: 291,293,311; `LoadData`: 281,321,323,333; `saves.Load`: 308,335; `saves.Save`: 306,335 |
| `Tools/Verification/ThreeSpecies/BroadleafAdultGrowthVerification.cs` | `CaptureData`: 344,413,415,419; `EndReferencePreview`: 454; `LoadData`: 416,454 |
| `Tools/Verification/ThreeSpecies/BroadleafTimberEconomyVerification.cs` | `CaptureData`: 58,71,156,205; `LoadData`: 69,80,159,206 |
| `Tools/Verification/ThreeSpecies/MixedDensityVerification.cs` | `CaptureData`: 97,512,565; `EndReferencePreview`: 230,592; `LoadData`: 195,204,219,515,592; `TryBeginReferencePreview`: 227 |
| `Tools/Verification/ThreeSpecies/Parity06ResidualVerification.cs` | `CaptureData`: 70,84,142,165; `LoadData`: 82,92,144,152,166 |
| `Tools/Verification/ThreeSpecies/Qualification/DensityQualification.cs` | `CanLoad`: 980; `CaptureData`: 121,949,956,993,1282; `EndReferencePreview`: 1323; `LoadData`: 960,964,972,975,978,997,1323; `saves.Load`: 983; `saves.Save`: 979 |
| `Tools/Verification/ThreeSpecies/ScenarioHistoryVerification.cs` | `CaptureData`: 62,75,186; `LoadData`: 73,111,188 |
| `Tools/Verification/U4/U4Qualification.cs` | `CanLoad`: 668; `CaptureData`: 104,152,153,630,646; `LoadData`: 157,649; `saves.Load`: 669; `saves.Save`: 661 |
| `Tools/Verification/U4Guidance/U4GuidanceSmoke.cs` | `CaptureData`: 127,381,382; `LoadData`: 386 |
| `Tools/Verification/UnderstoreyRecruitment/Model2Matrix.cs` | `CaptureData`: 30,113; `LoadData`: 44 |
| `Tools/Verification/UnderstoreyRecruitment/Model2PedagogyVerification.cs` | `CaptureData`: 58,79 |
| `Tools/Verification/UnderstoreyRecruitment/Model2TargetedEconomy.cs` | `CaptureData`: 31,97; `LoadData`: 45 |
| `Tools/Verification/UnderstoreyRecruitment/Model2Verification.cs` | `CaptureData`: 34,36,51,55,57,58,70,74,77,80,81,92; `LoadData`: 39,40,47,50,55,57,58,59,66,68,69,72,77,81,89,92 |
| `Tools/Verification/WindthrowV1/StormCoreVerification.cs` | `CaptureData`: 22,36,42,44,59; `LoadData`: 39,45,72,75,77,79,81,83,85,87,95,98,103 |
| `Tools/Verification/WindthrowV1/StormForcedMatrix.cs` | `CaptureData`: 43,67; `LoadData`: 54,62,82,87,95,100 |
| `Tools/Verification/WindthrowV1/StormLongRunCalibration.cs` | `CaptureData`: 68; `LoadData`: 79,98 |
| `Tools/Verification/WindthrowV1/StormLongRunResume.cs` | `CaptureData`: 68; `LoadData`: 80,99 |
| `Tools/Verification/WindthrowV1/StormPerformanceVerification.cs` | `CaptureData`: 41; `LoadData`: 53 |
| `Tools/Verification/WindthrowV1/StormRecruitmentVerification.cs` | `CaptureData`: 27,32,38; `LoadData`: 32,36,41,50,79 |
| `Tools/Verification/WindthrowV1/StormReplayVerification.cs` | `CaptureData`: 25,27; `LoadData`: 24 |
| `Tools/Verification/WindthrowV1/StormRngReplay.cs` | `CaptureData`: 21,25,26; `LoadData`: 25,28 |
| `Tools/Verification/WindthrowV1/StormRngWrite.cs` | `CaptureData`: 21,28,29; `LoadData`: 24,31 |
| `Tools/Verification/WindthrowV1/StormSalvageVerification.cs` | `CaptureData`: 24,26,29,57,68; `LoadData`: 57,68,72,101 |
| `Tools/Verification/WindthrowV1/StormUiVerification.cs` | `CaptureData`: 49,62,65,104; `LoadData`: 54,85,90,102,110 |
| `Tools/Verification/WindthrowV1/WindDisplayAudit.cs` | `CaptureData`: 47; `LoadData`: 69 |
| `Tools/Verification/WindthrowV1/WindthrowVisualVerification.cs` | `CaptureData`: 53,56,77; `LoadData`: 76,85 |
