# Storm save audit and approved compact v19 schema

Current main is save18 at1fbefd8; the next free schema version is19. Audit precedes production persistence changes. The user explicitly approved the additional compact `cropTreesLost` event count on8 October2026.

Existing authoritative state already stores each victim ID, mortality cause/year, frozen dimensions and position; fallen-deadwood records store original/remaining volume, dimensions, decay and tree link. Saved cell recentOpening records exposure and decay. Existing work orders and management events record salvage selection, financial settlement and resolved material volume. No duplicate victim-ID list, per-tree opening history, per-log bearing, salvage state or annual summary fields are needed.

The Manager packet authorizes stormModel and minimal resolved storm-event information needed for history/determinism. Approved compact additions: root integer `stormModel`; `scenarioOne.stormEvents` list of `{year:int, severity:float, directionDegrees:float, cropTreesLost:int}`. Severity is a [C] intensity, not a measured wind speed/probability. Direction is the bearing wind blows towards (north +Z=0°, east +X=90°); a tree-ID-keyed offset derives the fall bearing.

The crop-loss aggregate is irrecoverable: `ForestTree.ApplyMortality` clears `markType` before its death event; marking changes have no saved history. It is necessary resolved-event history for the packet's Crop Trees lost review requirement. Other event damage counts/volumes/areas are derived from existing victim records; retained and salvaged volume is derived from deadwood and work history. Storms with zero victims still retain severity/direction/history.

Legacy saves without the root field remain stormModel0; explicit model1 requires v19 and a scenario block. Reference remains0. New games remain0. Event years must be past/current, unique and ordered; intensities finite in(0,1], bearings in[0,360), crop counts nonnegative and no larger than derived victims. Model0 cannot carry causal storm events. All validation must precede world mutation.

Historical hash comparisons will strip only absent/inert new storm fields under stormModel0 and use the exact original byte layout; actual model1 world hashes retain all new fields. Frozen Reference archives are untouched. No other new persistent field is planned; a need for one triggers the packet's stop condition.

## Existing-field salvage semantics

No additional field was required. ScenarioWorkType appends SalvageDeadwood and the management treatment enum appends WindthrowSalvaged without renumbering existing values. Existing targetTreeId/outcome/resolvedYear/expectedVolume/material and financial history are reused. The one deadwood record stays after extraction with remainingVolumeM3=0; the wind-specific annual path preserves zero instead of reconstituting the normal natural-decay floor. It keeps root-plate display and original event volume reconstructable. Salvaged volume is derived from completed existing orders; all other event quantities are derived from victim/deadwood state. Neither runtime candidate profiles nor forced pending events are saved. Loads reset overrides to the provisional fixed profile.

## Final hardening and causal evidence

Core107 checks PASS, including atomic malformed JSON/schema rejection, missing/duplicate windthrow deadwood, invalid stored volumes and exact wind model event/victim associations. Every wind victim must have one existing finite positive original-volume/dimension deadwood record; remaining volume may be zero after salvage and may never exceed the original. Null history is rejected before mutation. No extra state was added to achieve this.

Independent process replay reproduces resolved84796FAEF211823C and future52037B943B62DD39. Separate saved RNG0 and RNG1 coverage is recorded after its dedicated two-process gate. Recruitment120 checks PASS: actual canopy/light/opening changes, Model2 vegetation competition and exact/natural juvenile survival/promotion paths, three species, zero-victim event and both competition strengths. Production Model2 communicates through existing shared state and is unchanged. The new harness restores unsaved SiteProductivity and calibration configuration.

Separate-process saved RNG0/1 gates both PASS throughyear10, including reversed iteration preview. RNG0 resolved3CDFDAB9E065695C / futureD48FE1364778BB0F; RNG1 resolved5699BD84874A3059 / future6CEAC0BB5401BEAD. These are test-fixture model1 world hashes, not newly accepted canonical/reference anchors. Reproducibility is verified under the recorded Editor/configuration, not an untested cross-platform promise.

Version19 hashes include the approved new fields. Model2's full modern fixture startEBE7A228F630AE41 / altered-site one-year671A28E277E0FDB1 and completion84CD51EB6A951D9E differ from their old version18 byte streams because of the schema. Final compatibility gates explicitly reconstruct only inert old layouts; active storm worlds are refused by historical comparators. The exact v18 model2 layout anchorsFA855239CDDA32D8 /02334804F65C0234 /702766DECE591E21 must remain unchanged. Clean authored one-year8333BAA4126E8A09 is independently verified and distinct from the known altered-site fixture.
