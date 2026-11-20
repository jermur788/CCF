# CCF #890 — Fab / Quixel Megascans 4×4 ground-asset shortlist

**Date:** 2026-11-20. **Manager:** #887 GPT-6 3D Asset Manager. **Status:** RESEARCH / DIRECT PRODUCT LINKS VERIFIED by public search, not licensed/downloaded/imported/Unity rendered. **Owner intent:** inspect up to four candidates each for forest soil/needle floor, moss/organic ground, root assemblies, branches/debris; decide which to download for reusable Compact30 + future scenarios. **Earlier procurement report:** https://github.com/jermur788/CCF/blob/docs/889-free-paid-ground-assets-deep-dive-20261120-v1/Docs/Research/ScenarioOneAssets/CCF-889-FREE-PAID-GROUND-ASSET-DEEP-DIVE-RANKING-20261120.md

## A — Needle litter/ground surface (texture/material assets; all Quixel direct listing links)

1. Pine Forest Floor — https://www.fab.com/listings/470a0512-bcf7-47f4-b59e-3e8e0e57e99d (4096px/m, 2×2m scan, albedo, normal, displacement, roughness)
2. Pine Needles — https://www.fab.com/listings/9cb56a1b-e6db-4ca4-8063-d60314467db6
3. Forest Floor — https://www.fab.com/listings/cf8af196-e3f2-4ac3-8de5-a5de611b9b55
4. Pine Forest Floor alternative scan — https://www.fab.com/listings/7de54e63-0c8f-425a-bd76-975741b9dd35

**Priority:** these alter appearance but do NOT create walkable physical unevenness; must be used on composable heightfield/terrain geometry.

## B — Moss/organic ground (texture/material assets)

1. Forest Moss — https://www.fab.com/listings/dae3ce0c-4f40-4659-bf9a-a4f7d5583018
2. Mossy Forest Floor — https://www.fab.com/listings/e75504fd-9631-4bf2-b5de-4e0b04f59cef
3. Forest Moss alternative scan — https://www.fab.com/listings/7c4ebad6-e8d0-4811-ae0a-b853aae80f4c
4. Mossy Forest Floor alternative scan — https://www.fab.com/listings/73c4ac19-dc59-4e30-8700-09b44c5a6f4a

**Priority:** conditionally placed under source-supported light/wetness/cover state, not lush uniform plantation carpet. Duplicate listing titles mean use exact listing UUID, not only text search.

## C — Exposed tree roots and root-flare geometry (3D, fit and scale before use)

1. Forest European Broadleaf Ground Root S — https://www.fab.com/listings/7340c012-f00a-4597-a839-657d95faffe2 (measured ~2.88×2.8×0.35m, open mesh, FBX/gltf; unsuitable as directly attached Sitka trunk without material/shape adaptation; valuable modular ground-root prototype)
2. Forest European Broadleaf Scatter Root M — https://www.fab.com/listings/86943776-d493-497b-a286-cbfc20df8537
3. Ground Roots — https://www.fab.com/listings/ffa458cc-e918-49de-8885-f87a1fbb04b8
4. Forest American Tree Root Mixed Small 03 — https://www.fab.com/listings/7e94f243-63a9-4732-836a-760dd54f1549

**Warning:** real roots must align with actual authoritative tree bases and surface collider; avoid implying European broadleaf root morphology is validated for Sitka. On exact Unity prototype, require proof of material matching and physical/visual ground shape.

## D — Dead branches, small sticks and scattered tree litter (mix of decals and scan assets)

1. Fallen Branches — https://www.fab.com/listings/874d948f-b6a2-4846-bdeb-49840d70658b (verified **DECAL** with texture set/gltf, not a solid fallen-log mesh)
2. Stems and Sticks — https://www.fab.com/listings/b64f5773-e61c-470f-b4a8-c6145a123a3c
3. Tree Debris — https://www.fab.com/listings/f4d638a7-35bc-4198-a5e4-866afd70b928
4. Dead Leaves and Sticks — https://www.fab.com/listings/b500f91c-7e9f-4b7d-9c1d-a0043e4a3ee4

**Warning:** inspect listing format and prop class before import; decals are visual ground scatter, not volumetric collision, and should not be used as stumps or artificially new forestry causal deadwood.

## Licensing, price and source files — verification caveats

- These are **16 separately identified public Fab listings** under Quixel Megascans; none are downloaded nor entitlement-verified. Fab may show a **Select a License** selector in logged-out view and hide final current price; cannot assert all sixteen FREE or promise to claim them from browser. Even though Quixel announced free Megascans return in Nov 2026, actual entitlement/checkout MUST be confirmed on each listing in owner's account. For example open Pine Forest Floor listing shows licence selector, while Fallen Branches listing shows **Free**.
- Crucial discrepancy: several current exact listing pages show **'Allows usage with AI: No'** (e.g. Pine Forest Floor and Forest European Broadleaf Ground Root S); *do not interpret commercial modification permission as blanket permission for AI training or AI image-to-3D inputs*. Previously communicated Quixel company-wide Nov 2026 AI announcement does not override visibly contradictory listing fields without licence-specific resolution. Manual material/Blender edits and authorised Unity use are separate from AI-training permissions; resolve licence before sending asset data to models.
- Fab official download/Unity guide: https://dev.epicgames.com/documentation/fab/exporting-assets-from-fab-in-launcher — Unity 6 export supported, FBX recommended for 3D scans, many ground materials need URP Lit material remap, and specialised Quixel texture-set/gltf formats should be checked.
- Fab official acquisition: https://dev.epicgames.com/documentation/fab/purchasing-and-downloading-assets-in-fab; https://www.fab.com/eula . Epic account login, acquisition and licence acceptance are user's explicit operations; **this connected browser/GitHub asset manager has no Fab-authenticated entitlement/download connector nor authorised local Unity filesystem access**, so cannot acquire and deliver purchased/claimed assets into their PC/project. User uses Fab in Launcher for batch download/export, or manual listing Download, then an authorised native worker can import via scoped task. Never ask for credentials or use unofficial download API/token scripts.
- Useful first purchase/download subset: A1 Pine Forest Floor, B1 Forest Moss, C1 European Broadleaf Ground Root S and D1 Fallen Branches, subject to correct price/rights and current actual material/look; if D1 needs real geometry instead use D2 Stems and Sticks. The advertised asset output is not necessarily the texture/mesh quality in actual CCF 6000.6 URP. No D-032 asset binary policy approval and no source/Unity import.
