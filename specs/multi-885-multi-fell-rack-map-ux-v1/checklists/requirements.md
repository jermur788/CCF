# Requirements — verify implementation presence; not an extended test plan
- [ ] R1 One retained Crop Tree can have **2+ actual neighbouring competitors** draft-selected simultaneously; selection toggles and visible count/IDs remain clear.
- [ ] R2 One **Add selected N** action queues each exact valid tree to the same existing pending Work Plan, no unintended Fell until separate Approve/Execute. Crop itself cannot be added.
- [ ] R3 Every submitted add uses the latest server-issued fence; partial success/refusal/unknown outcome stops and reports honestly. No claim of atomicity, duplicate requests or silent retry.
- [ ] R4 Clicking either area in the **mini two-area diagram** switches main map/inspector/list, with pointer and keyboard support; current area is unmistakable.
- [ ] R5 Compass + zoom + area selector occupy **one coherent on-map cluster**, not separate detached placements; readable at normal desktop and browser zoom.
- [ ] R6 No changing area accidentally adds work or changes a Crop selection; existing selected trees and work orders are area-scoped.
- [ ] R7 All non-presentation bytes in `PlayerMultiApi.cs` and all Core/API/fixture/P0/save/Unity/main unchanged. Owner demo and #877/#882 untouched.
- [ ] R8 A real source SHA and direct GitHub handoff identifies which F1–F3 implemented; never assert independently verified or owner accepted without that evidence.


- [ ] **R9 (F4)** The `/multi` legend is **visible by default at the right of the map** in desktop mode and can be hidden/shown with a clear accessible button; remains reachable at small widths.
- [ ] **R10 (F4)** Every legend entry pairs a real, recognisable **map-matching icon/glyph/swatches** with readable words for living, Crop, competitor/candidate/reference, pending, approved, stump/felled, naturally dead and boundary. It is not merely a textual dropdown.
- [ ] **R11 (F4)** Legend remains separate from the grouped navigation dock and does not block crowns, control targets, or the Work Plan.
