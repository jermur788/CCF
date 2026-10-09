# S1-B private compiled-build rights audit

Scope: compiled private Windows candidate only; no standalone asset/source distribution. Final included-assets.txt and package-manifest.json must be reviewed before distribution. This record does not infer rights from missing documentation.

| Component | Classification | Evidence / boundary |
|---|---|---|
| Project-created procedural/generated art | CLEAR FOR PRIVATE TEST | User explicitly confirmed ownership/permission for private compiled builds on 2026-10-09. Per-family READMEs under Assets/ForestPrototype/Art record procedural/generated creation. This is owner evidence, not independent proof of every source input. |
| Ultimate Nature – Starter, Innerverse Interactive | CLEAR FOR PRIVATE TEST as embedded asset | Owner confirmed licence for private compiled builds; existing project setup identifies the locally acquired pack, excluded from Git. Imported from the user's existing Unity Asset Store cache. Do not ship its raw package/source. Standard Asset Store EULA permits qualifying embedded game use, not raw redistribution. |
| Unity 6000.6.0f1 runtime | CLEAR FOR PRIVATE TEST subject to existing licence/tier compliance | Unity Editor Software Terms permit runtime distribution under their conditions. No subscription/tier change is made. Preserve supplied runtime notices. |
| Unity/package dependencies and open-source components | REQUIRES ATTRIBUTION / NOTICE | Build packaging conservatively copies installed package LICENSE and Third Party Notices files into Notices, including some Editor-only packages. Runtime assembly inclusion must be checked against the build report. |
| Any unknown external asset, restricted/review-only content or unverified custom licence discovered in included assets | UNCLEAR — BLOCK DISTRIBUTION UNTIL RESOLVED | Do not remove/replace major assets to force clearance. Return to Manager with exact paths/evidence. |

Sources checked 2026-10-09: [Unity Editor Software Terms](https://unity.com/legal/editor-terms-of-service/software), [Asset Store EULA](https://unity.com/legal/as-terms), [Asset Store EULA FAQ](https://assetstore.unity.com/browse/eula-faq). The cached pack archive contained its setup README and no separate custom licence/notice file; owner confirmation remains the permission evidence. Final candidate inclusion was audited below. No third-party audio was found in the tracked Assets inspection. Preview-only Blender cameras/layouts are not intended Player dependencies; final report establishes actual inclusion.

## Completed candidate audit

Compiled candidate: `CCF-S1-20261009-B06-b3ba9b8`, source `b3ba9b8900b1f62b721aee6508aba6d64db93bfd`.

The actual build report has 3,462 source-path entries (including MonoScript metadata, not 3,462 loose assets). Sources are CCF project code/art/settings/ForestTest, five embedded Ultimate Nature source entries, installed Unity packages and Unity built-ins. The pack entries are stump model/prefab, shared palette material/texture and mushroom model. All six required pack prefabs/metas remain byte-identical to the cached licensed package even where they are not Player dependencies. No third-party audio or review-only/reference image/source layout was identified as an included visual/audio dependency. The embedded project art is covered by the owner's explicit private compiled-build rights confirmation.

Classification: **CLEAR FOR PRIVATE TEST** for owner-confirmed project/pack embedded content and the existing licensed Unity runtime; **REQUIRES ATTRIBUTION / NOTICE** for runtime/package components, satisfied by the 40 supplied notice files. No unresolved included-content rights blocker was identified. This is a scoped evidence audit, not a new legal guarantee or public-distribution approval.

The tester payload has no loose verification/source scripts, source Unity package, licence credentials, tester saves, PDBs or DontShip backups. Four installed Collections test-only DLLs are filtered from the build; every remaining managed assembly was checked for references to them (zero). Unity retains some MonoScript metadata paths for those package sources; these are not executable test assemblies or raw C# payloads. The generated performance-test JSON resources are excluded. Installed package manifests/asmdefs are unchanged.

Effective build-program service flags are false for UnityConnect, PerformanceReporting, Analytics, CrashReporting and Insights in both normal and diagnostic Players. A temporary supported Engine Diagnostics opt-out is applied during building and the canonical source setting is restored. There is no added CCF telemetry or automated bug-report upload; manual local Player.log diagnostics remain available. This does not certify OS/Wine network behaviour.

The unused demo HDR and its demo skybox are absent from the build report. The HDR/meta are restored in task-local source with their original hashes; the licensed package and other checkouts were preserved. The private package remains held from beginner testers until Manager review, combined independent review and native Windows technical smoke pass. Wine UI text failed to render; native Windows font/readability checks are an explicit remaining technical gate.
