# Sen-1 #632 — Independent review of the A-04a juvenile sessile Oak scratch prototype (botany and art source)

**Date:** 16 November 2026. **Issue:** [#632](https://github.com/jermur788/CCF/issues/632). **Reviews:** [#627 result](https://github.com/jermur788/CCF/blob/docs/sen-1-627-linux-oak-scratch-resource-profile-20261116-v1/Docs/Handoffs/ScenarioOne-Graphics/Sen-1-627-LINUX-OAK-JUVENILE-SCRATCH-AND-RESOURCE-PROFILE-20261116.md) (commit `c21bc77e`) and the [#425 Manager verdict](https://github.com/jermur788/CCF/blob/docs/sen-1-627-manager-verdict-632-review-20261116-v1/Docs/Handoffs/ScenarioOne-Graphics/Sen-1-425-MANAGER-627-OAK-SCRATCH-RESOURCE-VERDICT-AND-632-REVIEW-DISPATCH-20261116.md).

## Verdict

**NEEDS CHANGES. This verdict covers SCRATCH QUALITY only.**

- The geometry is clean, deterministic and correctly scaled.
- It already carries the two most useful sessile-oak leaf cues: a visible petiole and a wedge-shaped base with no auricles.
- **Two major problems:**
  - The plant's structure encodes a *single-season* plant, but the label calls it a *transplant*.
  - The modelled root collar is thinner than the published minimum for 60 cm bare-root oak.
- There are also seven minor and three nit findings (§5).

**Asset state is unchanged by this review:** A-04a is still SCRATCH AUTHORED, and it is not REVIEWED-ACCEPTED, INTEGRATED, VERIFIED or approved for distribution, Unity import or production. D-032 stays OPEN. #619 stays DRAFT.

**Corrected label for all future tasks:** "0.6 m sessile Oak (*Quercus petraea*) nursery transplant, **age not established**". Do not use "first-year" (§3).

## 1. Reviewer, lane and scope

| Item | Value |
|---|---|
| Reviewer | Anthropic Claude Opus 5.5 (`claude-opus-5-5`), Claude Code in T3 Code, local Linux `jer-laptop`. Registered [RUNNING] on #632 before any inspection ([comment](https://github.com/jermur788/CCF/issues/632#issuecomment-6924516292)). |
| **Independence disclosure** | #627 was authored by a **different session** of the same model, Claude Opus 5.5. This session did not write any #627 file. However, it shares the owner's persistent Claude memory, which holds a short operational note from the #627 session (Blender gotchas, not design reasoning). **Session independence: yes. Model-family independence: NO.** The Manager may want a non-Claude cross-check of the botanical findings. |
| Branch / worktree | `review/sen-1-632-oak-juvenile-botany-art-scratch-20261116-v1` at `/media/jer/Files/CCF/Operations/CCF-632-Oak-Botany-Art-Review-20261116` (Files ext4, docs-only, not a Unity lane) |
| Starting HEAD | `37cd047ed39fd649563f70602d458e2970f6780f` (fresh `origin/main`, unchanged since #627) |
| Tracked output | This one Markdown file. No binaries, images, meshes, `Assets/`, C#, scenes, saves, Prefs, `ProjectSettings/`, `Packages/` or CCF-Ops changes. |
| Not done | Blender GUI, any edit or save of #627 files, Unity (no lease queried or taken), imports, Windows build, benchmark re-runs, and cleanup or restoration of any user file. |

**What was actually run, disclosed in full:**

- `sha256sum -c` on the #627 manifest.
- Read-only `stat`/`ls` of `/tmp` and `~/.config/blender`.
- A Python OBJ parser (written to my session scratchpad) that measures gaps between leaves and stems.
- `blender --version`.
- **One background (`-b`) Blender rebuild** of the copied build script. It ran in my session scratchpad with `TMPDIR` and all `BLENDER_USER_*` variables pointed at that scratchpad. Issue step 4 allows this kind of determinism check when it writes nothing into another's source and does not change the shared Blender profile. Neither happened: the #627 directory, `~/.config/blender` (mtime still 2026-09-14) and `/tmp` were unchanged before and after.

## 2. Custody, hashes and determinism (step 1, step 4)

- `MANIFEST.sha256` itself hashes to `b8d7e81f242dd07367857a7cd85fc3712bfa2e5672d684e131bf2a57b4f1ef0c`, which **matches** #627.
- **35/35 manifest entries are OK**, covering the build script, `.blend`, 3 OBJ + 3 MTL, stats JSON, PROVENANCE, 5 PNG renders, telemetry and tools.
- Two files in the directory are not in the manifest: `blender-user/config/recent-files.txt` and `recent-searches.txt`. They are byproducts of the #627 GUI runs and harmless, but they should be listed in a future manifest.
- **Determinism, independently reproduced.** I rebuilt from a byte-identical copy of `tools/build_oak_juvenile.py` (`76edba3b…a3b`). LOD0/1/2 `.obj`, all three `.mtl` and `_stats.json` were **byte-identical** to the #627 files.
  - The rebuilt `.blend` is **not** byte-identical. That is expected for `.blend` files, which embed session and pointer data. I did not investigate it further.
  - Conclusion: the OBJ interchange copies and stats are reproducible from source. The `.blend` is reproducible as content but not as bytes.
- Triangle counts were confirmed by the rebuild: 5,171 / 2,188 / 222.
- **All five renders were viewed directly** (`01_hero_three_quarter`, `02_front_ortho_scale`, `03_top_plan`, `04_leaf_closeup`, `05_lod_lineup`). The findings below are based on those images and on the OBJ files, not on #627's descriptions.

## 3. Nursery age and stock type (step 2)

**Sources consulted:**
- **[L]** Leslie, A. (2024), *Best practice prescriptions for propagating and establishing pedunculate oak (Quercus robur) for timber production*. Future Trees Trust, funded by the Forestry Commission Tree Production Innovation Fund. [PDF](https://www.futuretrees.org/wp-content/uploads/2024/11/ftt-monograph-oak.pdf). I read p. 6–7 directly from the page image.
- Stock notation sources, cited by BS 3936-4: [Conservation Handbooks: transplanting in the nursery](https://conservationhandbooks.com/tree-planting-aftercare-introduction/propagation/transplanting-in-the-nursery/) and [British Hardwood Tree Nursery plant specification guide](https://www.britishhardwood.co.uk/plant-specification-guide).

**Established facts from these sources:**
- **Stock notation.** "1+1" means two years old: one year as a seedling and one in a transplant bed. "1u1" means two years in the seedbed with an undercut, not transplanted.
- **Normal age of bare-root oak.** [L] p. 7: "Normally producing bare-rooted stock of oak takes 2 years (1+1) but using exponential nutrient loading they can be produced within one year (Schmal et al. 2011)." [L] p. 6: lifting is "normally at the end of the first (1+1) or second growing season (1+2)".
- **Root-collar minimum for bare-root stock.** [L] Table 4, citing Morgan 1999, gives the minimum root-collar diameter (RCD) for oak/ash/gean/lime as **11 mm at 60 cm height** (5 / 6.5 / 8 / 9.5 / 11 mm at 20 / 30 / 40 / 50 / 60 cm).
- **Root-collar minimum for cell-grown stock.** [L] Table 5 (Morgan 1999) gives **8 mm** for the 60–90 cm class.
- **One-season seedling growth.** In an Irish seedbed trial ([L] p. 7, citing O'Reilly et al. 2008), extra nitrogen raised "the proportion of seedlings greater than 40cm" after one season. So a seedling taller than 40 cm after one season is a notable outcome, not the norm. I did **not** find a source for typical first-season heights and do not state one.

**Irish specification.** I did not find a public Irish (DAFM) height or age table for oak planting stock. Search results only point to the Forestry Standards Manual, the Afforestation Scheme Annex 2 §2.7, and per-approval size conditions, and I did not read their tables. **No Irish average or required stock type is claimed here.**

**Conclusion:**
- "First-year" is **unsupported**. A 0.6 m bare-root oak is *normally* 1+1, so two years old.
- A one-year 0.6 m oak is possible only under special nursery regimes (exponential nutrient loading) or, plausibly, as cell-grown stock. That last point is my inference.
- **The age therefore stays "not established"** until the Manager or the owner picks a stock type. That choice is a design decision; it cannot be read from the sources.

**Classification of the #627 values:**

| Value | Class | Basis |
|---|---|---|
| 0.60 m height | **Design choice**, consistent with standard UK size classes | [L]; British Hardwood guide |
| Stem base 9.2 mm (about 10.3 mm with the modelled flare) | **Calibration**, below the bare-root minimum (M-2) | [L] Tables 4–5 |
| Leaf blade 46–85 mm | **Calibration, unverified.** I found no authoritative source for *juvenile* leaf length. Mature-crown figures should not be transferred, because juvenile foliage differs ([P] below). | — |
| Petiole 10–18 mm | **Transferred** from mature-leaf descriptions; at the low end | [B], [W] |
| 30 leaves, 4 laterals, 0.24 m crown | **Calibration, unverified** | none found |
| Two flushes (spring + lammas) | **Established for oak**; timing is a design choice | [B], [P] |
| No auricles, cuneate base | **Established** sessile-oak character | [B], [P], [WW] |

## 4. Species, architecture and art (step 3)

**Botanical sources:**
- **[B]** Bean, *Trees and Shrubs Online*, [*Quercus petraea*](https://www.treesandshrubsonline.org/articles/quercus/quercus-petraea).
- **[P]** Potter (1994) method, as reproduced by Keele University Arboretum: [*A guide to the identification of Pedunculate and Sessile Oaks and their hybrids*](https://www.keele.ac.uk/media/k-microsites/k-arboretum/articles/oaks/separating-oaks.pdf).
- **[WW]** waldwissen.net, [Sessile oak or pedunculate oak](https://www.waldwissen.net/en/forest-ecology/forest-plants/deciduous/sessile-oak-or-pedunculate-oak).
- **[W]** University of Reading Whiteknights trees, [*Q. petraea*](https://whiteknights-trees.uk/species/rkh). Search-result level only.

**What passes:**
- **Pivot and scale.** The root collar is at the origin. The leader bud top is at 0.600 m, matching the 0.6 m marker in `02_front_ortho_scale`.
- **Petiole.** Present on every LOD0/LOD1 leaf. Petiole percentage P = 100 × petiole / blade is bounded by about 12 % and 39 % (ratio of the means about 22 %), which falls on the *Q. petraea* side of [P]'s scheme. Pedunculate oak has a 2–3 mm petiole ([W]).
- **Leaf base.** No auricles, so it would score +1 in [P].
- **Lobing.** Shallow and regular: the code puts the sinus-to-tip depth at 32–42 % of the half-width, which is "shallow" under [P]'s X ≤ 0.5Y rule.
- **Buds.** Clustered, elongated terminal buds (about 2.2 : 1). [B] says sessile-oak buds are "rather more elongated and acute".
- **Lammas flush.** Plausible for oak ([B] photographs lammas growth on *Q. petraea*).
- **Silhouette.** The top plan (`03`) reads as "young oak".

**Species recognisability limit (important for design).** [P] says trees under about 4 m "may still have predominantly juvenile foliage, which cannot be reliably identified" by leaf characters, and that lammas leaves "can be of abnormal shape".
- So **no 0.6 m model can make sessile vs pedunculate reliably recognisable from foliage**, and a real forester could not either.
- In game, species identity for planted stock should come from context: nursery label, planting UI, delivery paperwork. Leaf shape should only be *consistent with* sessile oak, which A-04a broadly is.
- This is a game-design note, not a change to the model.

**Visual observations from the actual renders:**
- `04_leaf_closeup`: the blade reads as a soft "cloud" or club. It is widest well above the middle and has big bulbous lobes. There is no midrib or vein detail, and visible faceting bands run along the V-fold.
- `02_front_ortho_scale` and `05_lod_lineup`: many leaves appear as scalloped half-moons. They are nearly edge-on, so only one raised margin shows.
- `03_top_plan`, `01`, `02`: the upper lammas cluster overlaps and interpenetrates.
- `05_lod_lineup`: LOD0 and LOD1 are almost indistinguishable at preview size. LOD2 leaves are rhombic plates, and several float off the stem (m-3).

**720p/1080p and in-game scale: NOT ASSESSED.** No in-engine or gameplay-camera screenshots exist; the #627 images are 1024–1600 px Blender EEVEE previews. Any claim about how A-04a reads at player distance would be speculation.

**Future URP integration notes. These are not native proof, and none was tested.**
- Leaves are single-sided. The MTL carries no two-sided flag. URP Lit needs *Render Face: Both* and correct back-face lighting.
- Faceted leaf shading near the camera will be more visible under real-time lighting than in these EEVEE previews.
- LOD2 floating leaves will show as popping at LOD transitions.

## 5. Findings

**Severity scale:** MAJOR must be fixed before A-04a is used as a reference for production art. MINOR should be fixed in the next scratch revision. NIT is optional.

| ID | Sev | Finding | Evidence | Exact correction |
|---|---|---|---|---|
| **M-1** | MAJOR | **The structure encodes a single-season plant labelled as a transplant.** The whole 0.6 m stem is this season's spring and lammas flush. Leaves start at 0.08 m on the "spring flush", which is coloured with a material named `Stem_Older`. A 1+1 or 1u1 plant would have a previous-season stem section: older wood, a bud-scar ring at last year's terminal, and leaves only on current-year shoots, including short shoots from lateral buds. | `build_oak_juvenile.py` lines 25, 288–308, 339–340; §3 | First, the Manager or owner records the stock type: **bare-root 1+1 (normal per [L])** or cell-grown or one-year. Then rebuild the structure to match. For 1+1: a lower leafless older-wood segment, a visible previous terminal bud-scar ring, and current growth above it. Rename `Stem_Older` and `Shoot_Current` so the names match the wood age. |
| **M-2** | MAJOR | **Root collar is below the published bare-root minimum.** The model is 9.2 mm (stats), about 10.3 mm with the 1.12 collar flare, and about 9.5 mm across the flats of the 8-sided ring. The minimum for 60 cm bare-root oak is 11 mm; for 60–90 cm cell-grown it is 8 mm. | [L] Tables 4–5; script line 332–334 | For bare-root, set the collar to **≥ 11 mm** (exact value is calibration). Make the stats report the actual collar diameter including flare. |
| m-1 | MINOR | **The leaf is too obovate.** The outline envelope peaks at about 67 % of blade length. [B] says *Q. petraea* is broadest "at or only slightly above the middle", less obovate than *Q. robur*, so the current shape leans pedunculate. | `_env` exponents 0.85/0.42 (peak at u ≈ 0.669); render `04` | Move the widest point to about 0.50–0.58 of blade length. Keep the wedge-shaped base. |
| m-2 | MINOR | **Too few lobe pairs.** The model has 4–5 pairs; *Q. petraea* commonly has 5–8 (ranges overlap with *Q. robur*, 3–6). | `leaf_params` n ∈ {4, 5, 5}; [W] (search-result level only, not read in full) | Use 5–7 regular shallow pairs. Keep the depth as it is. |
| m-3 | MINOR | **LOD2 leaves float off the stem.** Measured from the OBJ, the minimum blade-to-stem gap per leaf is **7.9 mm min / 17.4 mm median / 45.0 mm max** at LOD2, and **0.0 mm** at LOD0 and LOD1. Cause: the blade base stays at the petiole tip when petioles are dropped, and the coarse 7-sample stem polyline departs from the smooth curve. | OBJ connectivity check; render `05` | At LOD2, start each blade at a node on the LOD2 polyline, or keep a single-triangle petiole. |
| m-4 | MINOR | **LOD steps are poorly spaced.** LOD1 keeps 42 % of LOD0's triangles but is visually almost identical. LOD2 rhombic plates read as a generic shrub. | stats; render `05` | Aim for LOD1 around 1,000–1,200 tris. Make LOD2 a crossed-card or simplified-blade design that keeps a lobed silhouette, chosen at integration. |
| m-5 | MINOR | **No venation; faceted near-camera shading.** [WW] calls the vein pattern (no veins running to the sinuses in *Q. petraea*) one of the most reliable cues. | render `04` | For a future textured pass: midrib and lateral veins ending in the lobes, via texture or normal map. Increase blade subdivision across the fold, or soften the fold normals. |
| m-6 | MINOR | **Edge-on "half-moon" leaves in side views.** Elevation is −24° to +8° and every blade has a V-fold. Horizontal leaves are natural, but the fold hides one margin in side views. | renders `02`, `05` | Reduce the fold, widen the roll/elevation spread, and judge from the gameplay camera pitch (not yet defined) rather than from an eye-level orthographic view. |
| m-7 | MINOR | **The upper cluster overlaps and interpenetrates.** | renders `01`, `03`, `04` | Add a simple per-leaf clearance test or wider azimuth jitter in the last flush. |
| n-1 | NIT | Underside hairs and the autumn "cocoa" underside are not modelled. | [WW] | Defer to the seasonal-state art spec. |
| n-2 | NIT | The manifest omits the two `blender-user/config` byproducts. | §2 | List them, or keep them outside the manifest root. |
| n-3 | NIT | Petiole 10–18 mm sits at the low end of the mature range ([W] gives 13–25 mm; [B] gives about 10–32 mm). | §4 | Optionally 12–22 mm, as calibration. |

The 30-leaf count, 4 laterals and 0.24 m crown are left as calibration. I found no source to support or challenge them.

## 6. FBX and OBJ (step 4)

**Facts:**
- Ubuntu's Blender 4.0.2 FBX add-on needs numpy, which is not installed here. #627 used the built-in OBJ exporter.
- The OBJs are three separate files.
- Unity imports `.obj`, but a set of OBJ files cannot carry an auto-generated LODGroup. Unity builds a LODGroup automatically only from **one** model file whose meshes are named `_LOD0` / `_LOD1` / `_LOD2`.

**Options for a future, owner-approved integration task. None was done here.**
1. **Recommended:** use an official blender.org Blender 4.x portable archive (its bundled Python includes numpy), unpacked into an isolated task directory. Run it with the same isolation as §7, re-run the deterministic script, and export one FBX holding `…_LOD0/1/2`. This needs owner approval for the download, recorded with its version and SHA-256.
2. **Fallback, no download:** import the three OBJ files and assemble a LODGroup prefab by hand in Unity. Simple, but adds per-asset manual steps and serialized-asset churn.
3. **Not recommended:**
   - `pip install numpy` into the system or distro Python. It changes a shared system component.
   - Importing the `.blend` directly into Unity. That needs Blender on every importing machine and is not reproducible.
   - Adding Unity's FBX Exporter package. It is a package change and needs separate approval.

## 7. `/tmp/quit.blend` side effect (step 5)

**What happened, per #627 and the files.** Blender writes `/tmp/quit.blend` whenever a GUI session quits. #627 ran two GUI sessions, around 17:24 and 17:26 UTC on 2026-11-16. They used `--factory-startup` and redirected `BLENDER_USER_*`, but did **not** redirect `TMPDIR`.
- The first quit replaced any earlier `/tmp/quit.blend`.
- The second quit replaced the first.
- #627 then moved the R2 file to `gui-work/quit.blend.from-R2-session`. Its SHA-256 is `5506f5f1…199d`, which differs from the source `.blend`, as expected after live edits.

**Current state, checked read-only:** `/tmp/quit.blend` does **not exist**. Blender's *File → Recover → Last Session* therefore has nothing to offer right now. I did not alter, restore, copy or move anything.

**Was the owner's recovery snapshot lost? Unknown, and NOT recoverable by assumption.** The evidence narrows when a loss could have happened:
- The host booted at **2026-11-16 09:14:29** (Europe/Dublin = UTC in November). `systemd-tmpfiles-setup` runs `--create --remove --boot`, and `/usr/lib/tmpfiles.d/tmp.conf` holds `D /tmp 1777 root root 30d`. So `/tmp` was emptied at boot.
- Any lost snapshot must therefore come from an owner Blender GUI session that quit between **09:14 and about 17:24 UTC today**.
- The owner's `~/.config/blender/4.0/config` was last modified 2026-09-14, `recent-files.txt` is empty, and neither the user nor the system journal mentions Blender this boot.
- That is **no positive evidence** of owner Blender use today. It does **not** prove absence: quitting from an unsaved scene leaves no trace in these places.
- **Only the owner can say** whether they used Blender between 09:14 and 17:24 today. If they did, that session's recovery file is gone.

**Recommended isolation for all future Blender runs (GUI, render or background).** This was partly verified by my background rebuild.

```bash
T=/media/jer/Files/CCF/Verification/<task>/blender-iso
mkdir -p "$T"/{tmp,config,scripts,datafiles}
# Pre-flight: if /tmp/quit.blend exists, STOP and ask the owner. Do not copy, move or delete it.
test -e /tmp/quit.blend && { echo "owner recovery file present - stop"; exit 1; }
env TMPDIR="$T/tmp" BLENDER_USER_CONFIG="$T/config" \
    BLENDER_USER_SCRIPTS="$T/scripts" BLENDER_USER_DATAFILES="$T/datafiles" \
    blender --factory-startup -b ...   # use -b unless a GUI is the point of the task
```

- **Verified here (Blender 4.0.2, background mode):**
  - With `TMPDIR` set, `bpy.app.tempdir` resolved to a `blender_XXXXXX/` folder inside the task tmp dir.
  - Nothing appeared in `/tmp`, and no `quit.blend` was written.
  - `~/.config/blender` was unchanged.
- **Not verified:** that a **GUI** session writes `quit.blend` under `$TMPDIR` rather than `/tmp`. Blender's documented fallback is `$TMPDIR` and then `/tmp`, but the first GUI run under this wrapper must confirm it, with the pre-flight check above and a `stat /tmp/quit.blend` after quit.
- **GUI etiquette:**
  - No Blender GUI while the owner is using the desktop, unless they explicitly agree.
  - No focus-stealing windows.
  - #627 showed that an unfocused window throttles, so **valid GUI measurements need focus**. Schedule them with the owner rather than hiding the window.
  - Use a disposable working copy, as #627 did.

## 8. Resource conclusions (step 6)

#627's resource figures are **Blender-alone** only. **Unity and Blender running at the same time was NOT PERFORMED**, so the interim **TIME-SLICED** rule stands. I re-ran no benchmarks. My one background rebuild was not measured and does not count as resource evidence.

## 9. Next action for the Manager

1. **Decide the stock type** (M-1/M-2: bare-root 1+1 is the normal case per [L]) and record it. Use the label in this review's Verdict section until then.
2. If a revision is justified, dispatch **one** bounded A-04a scratch-revision packet with **one** art writer, covering M-1, M-2, m-1…m-7, in a new scratch directory. Leave the #627 directory as immutable evidence.
3. Commission a **separate, non-Claude delta review** of that revision if model-family independence is required.
4. FBX route (§6 option 1) and any Unity import remain **future, separately authorised** work, gated by D-032 and a signed #619.

**Owner question (for the Manager to relay; it does not block anything):** did you use Blender between 09:14 and 17:24 UTC on 16 November? If yes, that session's "Recover Last Session" snapshot was overwritten by #627 and cannot be restored.
