# Stand Map Species Canopy Differentiation — Task Packet

**Claim:** #232  
**Parent:** #91 / Browser Overall #200 / local orchestration #203  
**Status:** **PREPARED / BLOCKED ON #231 FINAL TIER-3**  
**Classification:** PACKET ONLY — bounded player-facing map presentation repair  
**Difficulty:** Medium — narrow visual scope, but it touches the authoritative Stand Map/minimap canopy raster and therefore needs rendered regression evidence at both target resolutions.  
**Preferred executor after release:** SOL/CODEX or Claude Sonnet high-reasoning local Unity/code lane; one primary writer.  
**Independent reviewer:** separate cross-provider reviewer.  
**User action required now:** none. Asset/reference design may proceed while #231 runs; production edits may not.

## User-directed intent

The top-down Stand Map currently makes Sitka spruce read too much like a deciduous crown. The player should be able to identify Sitka as a conifer at a glance from the map, without relying only on labels or colour.

Minimum product requirement:

- Sitka spruce must read immediately as **coniferous** in the full Stand Map at normal viewing scale.
- The distinction should remain visible in the minimap where pixel scale permits.
- Silhouette/structure must carry the distinction; colour may reinforce it but must not be the sole cue.
- Oak and Beech may receive additional differentiation if it remains a bounded presentation change and does not compromise legibility.

## Current implementation truth

The map does **not** currently depend on external tree PNG assets. `Assets/ForestPrototype/UI/Map/StandMapCanopyRaster.cs` procedurally generates cached crown sprites from real tree position, crown radius, state and `MapTreeKind`.

The current generator already branches on `MapTreeKind.Conifer`, with different lobe counts, texture parameters and radial spokes, but the resulting conifer/broadleaf difference is not sufficiently obvious in observed player-facing viewing.

`StandMapCanopyLayer` then presents the raster for both the full Stand Map and minimap. The same model/world-to-map transform is used for both.

## Preferred implementation boundary

Prefer strengthening the existing procedural crown generator over introducing a new texture/asset loading pipeline.

A suitable bounded solution may change only the presentation parameters/shape mathematics needed to make conifers unmistakable. Preserve:

- exact tree positions;
- crown-radius scaling from the simulation;
- species/state identity;
- full-map and minimap world-to-map transforms;
- tree draw ordering and standing-dead/stump semantics;
- canopy-version redraw behavior;
- sprite caching/performance characteristics;
- Rack/candidate/proposal/Fell/Crop Tree overlays and their visual authority;
- all gameplay, save, ecology, economy, growth, prescription and Work Plan semantics.

Do not create a second tree-location or canopy model.

## Visual target

For **Sitka / conifer** crowns, prefer a recognisably coniferous top-down language such as:

- stronger radial/whorled branch structure;
- more pointed/star-like or layered perimeter rather than a rounded broadleaf blob;
- denser central leader/top tuft;
- darker/cooler needle mass where compatible with the existing palette;
- enough per-tree variation to avoid cloned stamps while preserving the conifer family resemblance.

For **Oak**, preserve a broader, strongly lobed irregular crown.

For **Beech**, preserve a smoother rounded/oval broadleaf crown with less coarse lobing than Oak if this can be achieved without expanding scope.

The player must not need to infer species from colour alone.

## Reference asset/design work allowed before #231 returns

Browser/design work may generate transparent top-down reference crowns or a species comparison sheet now. Those references are design targets only unless a later Manager decision explicitly chooses texture-backed implementation.

Do **not** add generated raster assets to the live Tier-3 production candidate while #231 is running.

## Release rule

#231 is currently qualifying one exact Scenario One candidate. This task must not modify production until #231 returns and Browser Overall disposes that result.

If #231 passes, Browser Overall will repin #232 to the resulting exact integrated lineage. Because any accepted canopy presentation change creates a new player-test candidate, the affected rendered/integration gates must be rerun before the user playtest token is issued.

If #231 finds a blocking product defect first, #232 remains queued behind that disposition unless Browser Overall explicitly resequences it.

## Required verification after production release

### Pure/offline

- existing Stand Map canopy/raster tests remain green;
- add or strengthen species-shape assertions only where they test durable presentation invariants rather than brittle individual pixels;
- prove tree position and crown-radius mapping are unchanged;
- prove deterministic repeat for identical model/seed inputs;
- prove no new simulation/gameplay state is introduced.

### Unity/rendered

Capture the full Stand Map at **1280×720 and 1920×1080** on a mixed-species state where Sitka, Oak and Beech are all visible.

Required visual acceptance:

- Sitka crowns are immediately recognisable as conifers in isolation and in a mixed stand;
- Sitka does not read as the same broadleaf silhouette with only a colour shift;
- Oak and Beech remain readable broadleaf crowns and do not accidentally acquire conifer-like radial spikes;
- marks/overlays remain legible over all crown types;
- rack gaps/access lines remain readable;
- no critical clipping, moiré or visual noise at either target resolution;
- minimap remains coherent and performant.

Rerun proportionate map/UI gates, including Stand Map canopy/raster verification, Combined HUD/minimap, PARITY-07 rendered coverage where applicable, Rack map-planning/rendered gates if canopy presentation overlaps their screenshots, and a Tier-3 rendered delta check sufficient to prove the new exact candidate remains player-test-ready.

### Performance

Do not materially regress canopy redraw behavior. Preserve the cached-sprite architecture or provide measured evidence that any replacement remains within the existing presentation budget.

### Independent review

Fresh cross-provider read-only review of:

- exact production diff;
- rendered mixed-species evidence at both target resolutions;
- species legibility without colour-only dependence;
- unchanged gameplay/model/position/crown-radius semantics;
- map/minimap and overlay compatibility;
- scope discipline.

Verdict: `APPROVED / NEEDS CHANGES / REJECTED`.

## Hard exclusions

- no forestry model change;
- no species growth/calibration change;
- no save/schema change;
- no Rack V2 geometry or persistence change;
- no MFM logic or candidate/proposal semantics change;
- no ecology/economy/timber/deadwood/regeneration change;
- no protected Reference Future / Legacy40 rebaseline;
- no assertion weakening;
- no Windows build;
- #194 remains NO ACTION.

## Stop conditions

STOP and return to Browser Overall if:

- the change requires a new asset-loading architecture or material pipeline larger than a bounded map presentation repair;
- the existing procedural renderer cannot produce the desired legibility without disturbing authoritative tree geometry/scale;
- a map/minimap/rack/overlay regression appears;
- a protected verification gate becomes red for a genuine product reason;
- performance materially regresses;
- independent review finds a blocker.

## Required handoff after release

Publish:

`Docs/Handoffs/StandMap-Species-Canopy-Differentiation/HANDOFF.md`

Include exact base/final lineage, changed files, before/after screenshots at both target resolutions, mixed-species and minimap evidence, gate counts, performance/redraw evidence, independent-review verdict and cleanup state.

Success token:

`STAND MAP SPECIES CANOPY DIFFERENTIATION COMPLETE — READY FOR MANAGER REVIEW`
