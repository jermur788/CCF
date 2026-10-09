# S1-B private compiled-build rights audit

Scope: compiled private Windows candidate only; no standalone asset/source distribution. Final included-assets.txt and package-manifest.json must be reviewed before distribution. This record does not infer rights from missing documentation.

| Component | Classification | Evidence / boundary |
|---|---|---|
| Project-created procedural/generated art | CLEAR FOR PRIVATE TEST, subject to included-asset audit | User explicitly confirmed ownership/permission for private compiled builds on 2026-10-09. Per-family READMEs under Assets/ForestPrototype/Art record procedural/generated creation. This is owner evidence, not independent proof of every source input. |
| Ultimate Nature – Starter, Innerverse Interactive | CLEAR FOR PRIVATE TEST as embedded asset; confirm package-specific licence/notice on import | Owner confirmed licence for private compiled builds; existing project setup identifies the locally acquired pack, excluded from Git. Imported from the user's existing Unity Asset Store cache. Do not ship its raw package/source. Standard Asset Store EULA permits qualifying embedded game use, not raw redistribution. |
| Unity 6000.6.0f1 runtime | CLEAR FOR PRIVATE TEST subject to existing licence/tier compliance | Unity Editor Software Terms permit runtime distribution under their conditions. No subscription/tier change is made. Preserve supplied runtime notices. |
| Unity/package dependencies and open-source components | REQUIRES ATTRIBUTION / NOTICE | Build packaging conservatively copies installed package LICENSE and Third Party Notices files into Notices, including some Editor-only packages. Runtime assembly inclusion must be checked against the build report. |
| Any unknown external asset, restricted/review-only content or unverified custom licence discovered in included assets | UNCLEAR — BLOCK DISTRIBUTION UNTIL RESOLVED | Do not remove/replace major assets to force clearance. Return to Manager with exact paths/evidence. |

Sources checked 2026-10-09: [Unity Editor Software Terms](https://unity.com/legal/editor-terms-of-service/software), [Asset Store EULA](https://unity.com/legal/as-terms), [Asset Store EULA FAQ](https://assetstore.unity.com/browse/eula-faq). Current pack-specific metadata/notices and final build inclusion remain to be audited. No third-party audio was found in the tracked Assets inspection. Preview-only Blender cameras/layouts are not intended Player dependencies; final report establishes actual inclusion.
