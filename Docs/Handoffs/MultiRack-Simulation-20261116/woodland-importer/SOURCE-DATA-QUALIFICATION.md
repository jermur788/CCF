# Woodland Importer V1 — Source Data Qualification (`trems_integrate`)

- **Date (UTC):** `2026-11-26`
- **Target Dataset:** *Spatially explicit database of tree related microhabitats (TreMs)* (`trems_integrate`, Integrate+ / INRAE)
- **DOI:** `10.15468/ocof3v`
- **GBIF Dataset Key:** `2e102194-f384-4712-89a4-5db7a3fc409a`
- **Licence:** **CC BY 4.0** (`http://creativecommons.org/licenses/by/4.0/legalcode`)
- **Citation:**
  > Kraus D, Schuck A, Bebi P, Blaschke M, Bütler R, Flade M, Heintz W, Krumm F, Lachat T, Larrieu L, Lehnerova L, Levin M, Mergner U, Pach M, Paillet Y, Pyttel P, Rydkvist T, Santopuoli G, Sever K, Sturm K, Vandekerkhove K, Winter S, Witz M, Winnock M, Marcandella M, Roth I, Jakob A, Dickele R, Hofmann G, Ruis-Eckhardt D, Boschen S, Schölmerich U, Leder B, Guericke M, Merkel H, Löffler D, Schusser S, Runkel M, Steinmetz A, Marx K H, Mongelluzzi A, Wilshusen F, Boutteaux J, Duchamp L, Dericbourg N, Rouyer E, Csikos V, Sweeny K, Steichen D, Leytem M, Konczal S, Stereńczak K, Kazimirovic M, Šebeň V, Baiges Zapater T, van der Maaten-Theunissen M, Pommer A, Nord-Larsen T, Fuhr M, Delebeque L, Navarro L M, Lasala D, Waez-Mousavi S, Sefidi K, Abellanas B, Zudin S (2022). *Spatially explicit database of tree related microhabitats (TreMs)*. Version 1.26. Institut national de recherche pour l’agriculture, l’alimentation et l’environnement (INRAE). Occurrence dataset https://doi.org/10.15468/ocof3v accessed via GBIF.org on 2026-11-26.

---

## 1. Publisher DwC-A Archive Retrieval Status (`WI-T00` / `WI-T01`)

### Original Publisher IPT DwC-A & GBIF-Mediated Bulk Download — `PENDING REAL ARCHIVE DOWNLOAD` (Fallback Triggered Honestly)
1. **Registered publisher `DWC_ARCHIVE` endpoint (`api.gbif.org/v1/dataset/2e102194-f384-4712-89a4-5db7a3fc409a`):**
   - `http://dynids.toulouse.inra.fr:8180/ipt/archive.do?r=trems_integrate` → **`curl: (28) Connection timed out after 60001 milliseconds`** (`HTTP_CODE=000`, `SIZE=0`).
   - Alternate probes (`https://dynids.toulouse.inra.fr/ipt/archive.do?r=trems_integrate`, `http://dynids.toulouse.inrae.fr:8180/ipt/archive.do?r=trems_integrate`, `https://dynids.toulouse.inrae.fr/ipt/archive.do?r=trems_integrate`) all returned **`URLError: <urlopen error timed out>`**.
   - **Independent corroboration from GBIF's own crawl history (`api.gbif.org/v1/dataset/2e102194-f384-4712-89a4-5db7a3fc409a/process?limit=3`):**
     - `2026-11-25T20:11:40.149+00:00` → `finishReason: ABORT`
     - `2026-11-18T20:05:09.633+00:00` → `finishReason: ABORT`
     - `2026-11-11T19:10:37.125+00:00` → `finishReason: ABORT`
     - Last successful GBIF crawl recorded on verbatim records: **`2026-10-30T23:26:12.556+00:00` (`crawlId: 420`)**.
2. **GBIF-mediated DwC-A occurrence download API:**
   - `POST https://api.gbif.org/v1/occurrence/download/request` (`format: "DWCA"`, `DATASET_KEY = 2e102194-f384-4712-89a4-5db7a3fc409a`, unauthenticated) → **`HTTP 403`** with body **`Access is denied`** (GBIF bulk download creation requires authenticated GBIF user credentials, which are not present in this lane).
   - `GET https://www.gbif.org/dataset/2e102194-f384-4712-89a4-5db7a3fc409a` → **`curl: (28) Operation timed out after 15001 milliseconds with 0 bytes received`** (`HTTP_CODE=000`).
   - `GET https://api.gbif.org/v1/occurrence/download/dataset/2e102194-f384-4712-89a4-5db7a3fc409a?limit=100` → **`curl: (28) Operation timed out after 20001 milliseconds with 475136 bytes received`**.
- **Consequence (honest boundary):** Neither the original publisher IPT ZIP nor a freshly generated GBIF-mediated bulk DwC-A ZIP could be downloaded without credentials on `2026-11-26`. Original publisher ZIP SHA-256 and byte-level publisher `meta.xml` verification remain **`PENDING REAL ARCHIVE DOWNLOAD`**.

### Real CC BY 4.0 Occurrence Record Retrieval via GBIF API — `SUCCEEDED`
- `https://api.gbif.org/v1/dataset/2e102194-f384-4712-89a4-5db7a3fc409a` (HTTP 200), `/document` EML (`77,489` bytes, HTTP 200), `/v1/occurrence/search` (GBIF interpreted view), and `/v1/occurrence/{key}/verbatim` (GBIF stored verbatim publisher fields from `crawlId: 420`, `2026-10-30`) were directly queried on `2026-11-26`.
- Most fixture columns below come directly from `/v1/occurrence/{key}/verbatim`; explicit exceptions (`countryCode` from GBIF's interpreted record, dropped verbatim columns, and coordinate-system assumptions) are documented transparently in §2.

---

## 2. Observed Verbatim Fields, GBIF-Interpreted Additions, Dropped Columns, and Assumptions (F-09)

### Core Terms in the Subset Fixtures
| Column / DwC Term | Source Provenance | Observed Example | Notes & Assumptions |
|---|---|---|---|
| `occurrenceID` | Verbatim (`/verbatim`) | `24839_IPlus_Donadea_MHF_2`, `33321_IPlus_Mersch` | Unique tree key: `{identificationID}_{plotName}[_{surveyIndex}]`. Note (N3): `IntegrateMarteloscopeReader` intentionally retains any trailing `_{surveyIndex}` (e.g. `_2` in `IPlus_Donadea_MHF_2`) as part of the plot-survey key so distinct survey epochs of the same physical plot are never silently merged into one Year-0 stand. |
| `scientificName` | Verbatim (`/verbatim`) | `Picea abies`, `Fagus sylvatica`, `Quercus petraea` | Verbatim binomial (GBIF interpreted view appends taxonomic authors; `WoodlandSpeciesMapper` handles both). |
| `verbatimCoordinates` | Verbatim (`/verbatim`) | `"34 , 48.82"`, `"18.62 , 83.47"`, `"71.28 , 16.07"` | **INFERENCE / ASSUMPTION (not verified publisher schema):** the importer treats these two numbers as plot-local Cartesian metres `(X, Y)` because their range (`0–100`) matches typical Integrate+ marteloscope plot dimensions. **Contradiction to note:** the verbatim records themselves carry `verbatimCoordinateSystem = "decimal degrees"` (likely a publisher mapping artefact, since 48.82/83.47 cannot be WGS-84 degrees for Ireland/Luxembourg), and **X/Y axis order and compass orientation are unverified** without the publisher's field schema. |
| `country` | Verbatim (`/verbatim`) | `Ireland`, `Luxembourg` | Verbatim country name. |
| `countryCode` | **GBIF-interpreted** (`/search`, **not** verbatim) | `IE`, `LU` | Verbatim records carry `country` only; ISO `countryCode` was populated from GBIF's interpreted occurrence record. |
| `decimalLatitude` / `decimalLongitude` | Verbatim (`/verbatim`) | `53.3241216`, `-6.74506` | Plot-level WGS-84 location (shared across every tree in a plot; verbatim `coordinateUncertaintyInMeters: 500`). Stored in `ExtraAttributes` (`plotDecimalLatitude`/`plotDecimalLongitude`) and never confused with per-tree local coordinates. |
| `habitat`, `eventDate`, `verbatimElevation` | Verbatim (`/verbatim`) | `Ash admixed with Norway spruce`, `2019`, `80` | Stand context preserved in `ExtraAttributes`. |

### Extension Observed: `http://rs.tdwg.org/dwc/terms/MeasurementOrFact`
Joined to the core record by `occurrenceID`. Observed verbatim rows:

| `measurementType` | `measurementRemarks` | `measurementUnit` | Observed `measurementValue` | Importer Mapping |
|---|---|---|---|---|
| `Tree diameter` | `diameter` | `cm` | Numeric cm (e.g. `24`, `43.4`, `62.9`) | `ImportedTreeRecord.DbhCm` (`FieldProvenance.Observed`) |
| `tree height` | `height` | `m` | Numeric m (e.g. `14`, `22`, `32.2`) | `ImportedTreeRecord.HeightM` (`FieldProvenance.Observed`) |
| `Tree status (dead or living)` | `status` | *(empty)* | **`"1"` = Living**, **`"0"` = Dead** | `ImportedTreeRecord.VitalStatus` (`Alive` / `Dead`). In `IPlus_Donadea_MHF`, `259` trees have `"1"` and `12` have `"0"`; in `IPlus_Mersch`, `131` have `"1"` and `3` have `"0"`. |
| `Tree related micro-habitat` | Code (e.g. `EP33`, `DE11`, `CV31`) | *(empty)* | Count (e.g. `1`) | Preserved verbatim in `ExtraAttributes["trem:{code}"]`. |

### CC BY 4.0 §3(a)(1)(B) Statement of Modifications Made to Subset Fixtures
The committed fixtures (`ie-donadea-mhf-subset.dwca.zip` and `lu-mersch-oak-beech-subset.dwca.zip`) are modified subsets of the INRAE TreMs dataset (`10.15468/ocof3v`, CC BY 4.0). Exact changes made (also recorded in `Fixtures/Qualified/ATTRIBUTION.json`):
1. Selected 20-tree subsets out of 271 (`IE Donadea`) and 134 (`LU Mersch`) records to keep committed test fixtures small (~2.1 KB each).
2. **Dropped verbatim columns** not consumed by Woodland Importer V1, including `measurementAccuracy` (e.g. `5%`, `20 cm`), `identificationID`, `basisOfRecord`, `coordinateUncertaintyInMeters`, `verbatimCoordinateSystem`, `geodeticDatum`, `kingdom`, `genus`, `specificEpithet`, `taxonRank`, `accessRights`, and `language`.
3. Populated the `countryCode` column from GBIF's interpreted occurrence record (verbatim records contain `country` only).
4. Sorted rows deterministically and packaged them into a **reconstructed** tab-delimited UTF-8 DwC-A ZIP envelope (`meta.xml`, `occurrence.txt`, `measurementorfact.txt`, `fieldsTerminatedBy="\t"`, `linesTerminatedBy="\n"`, `fieldsEnclosedBy=""`, `ignoreHeaderLines="1"`, join `<id index="0"/>` ↔ `<coreid index="0"/>`) because the publisher IPT `meta.xml` could not be downloaded.
5. No species names, coordinates, diameters, heights, vital statuses, or microhabitat codes were altered.

---

## 3. Qualified Plots (Committed Offline Subsets under `tests/Core/WoodlandImport/Fixtures/Qualified/`)

> **Important boundary honesty disclosure (F-09):** Neither `IPlus_Donadea_MHF` nor `IPlus_Mersch` provides a surveyed boundary polygon in GBIF. The `50 × 100 m` (`0.50 ha`) and `100 × 100 m` (`1.00 ha`) polygons used in `FixtureQualificationTests.cs` are **constructed test-harness constants with no cited survey source for either plot's real dimensions or orientation**. Without a genuinely surveyed boundary polygon, **both** plots honestly remain `WoodlandReadiness.InventoryOnly` under Option A #3.

### 3.1 Irish Plot First — `IPlus_Donadea_MHF` (Donadea Forest Park, Co. Kildare, Ireland)
- **Full plot observed in GBIF (`country=IE`):** `271` total records (`259` living `status=1`, `12` dead `status=0`), habitat `"Ash admixed with Norway spruce"`, plot WGS-84 `53.3241216°N, -6.74506°E`, elevation `80 m`, survey `2019`.
  - Full-plot species mix: `Picea abies` (118), `Fraxinus excelsior` (102), `Ulmus glabra` (30), `Fagus sylvatica` (16), `Quercus robur` (2), `Crataegus monogyna` (2), `Sorbus aucuparia` (1).
- **Committed subset fixture:** `ie-donadea-mhf-subset.dwca.zip`
  - **Byte size:** `2,137` bytes
  - **Fixture SHA-256:** `4ee2f4a125d1a06474d7086ccf5a6ca96735e16124ad3e4f9a11647ddcee853d`
  - **Subset rows:** `20` verbatim trees spanning **all 7 observed Irish species** (`Fagus sylvatica`: 12, `Fraxinus excelsior`: 3, `Picea abies`: 1, `Ulmus glabra`: 1, `Quercus robur`: 1, `Sorbus aucuparia`: 1, `Crataegus monogyna`: 1), `18` living + `2` dead, assumed local `X ∈ [2.97, 47.73]`, `Y ∈ [0.00, 98.35]`.
  - **Importer readiness (Option A #1 & #4):**
    - Without boundary: `WoodlandReadiness.InventoryOnly` (`BoundaryMissing` + `UnsupportedLivingSpecies`), all 20 trees retained verbatim.
    - Under the constructed `50 × 100 m` test boundary constant: **remains `WoodlandReadiness.InventoryOnly`** because living unmodelled species (`Fraxinus excelsior`, `Picea abies`, `Ulmus glabra`, `Quercus robur`, `Sorbus aucuparia`, `Crataegus monogyna`) block simulation admission without silent deletion or species substitution.

### 3.2 Compatible Mixed European Plot — `IPlus_Mersch` (Mersch, Luxembourg)
- **Full plot observed in GBIF (`49.745348°N, 6.141345°E`, `LU`):** `134` total records (`131` living `status=1`, `3` dead `status=0`), habitat `"Oak-beech"`.
  - Full-plot species mix: `Fagus sylvatica` (120 living, 3 dead) + `Quercus petraea` (11 living) — **100% `ModelledAtBase`** (`beech` + `sessile-oak`), zero unmodelled living species.
- **Committed subset fixture:** `lu-mersch-oak-beech-subset.dwca.zip`
  - **Byte size:** `2,145` bytes
  - **Fixture SHA-256:** `3c275b0115b6330ebabdad422ea9a25451489c93949dd5d80be1fb5e8b57b1ae`
  - **Subset rows:** `20` verbatim trees (`11` living `Quercus petraea` + `6` living `Fagus sylvatica` + `3` dead `Fagus sylvatica`), assumed local `X ∈ [7.17, 98.56]`, `Y ∈ [6.18, 92.65]`.
  - **Importer readiness (Option A #3 & #4):**
    - Without boundary (true observed state): **`WoodlandReadiness.InventoryOnly`** (sole issue: `BoundaryMissing` — boundary is never inferred).
    - Under the constructed `100 × 100 m` test-harness polygon constant: **`WoodlandReadiness.CandidateForSimulationIntegration`** (`0` issues, all 20 trees retained, `17` living `ModelledAtBase` + `3` dead). Note plainly: this `CandidateForSimulationIntegration` outcome depends on the constructed test polygon and does **not** claim an externally surveyed boundary for Mersch has been verified.

*(Dataset-wide target-species counts observed in GBIF on 2026-11-26 for reference: `Fagus sylvatica` 13,554; `Pinus sylvestris` 4,046; `Quercus petraea` 3,151; `Alnus glutinosa` 87; `Picea sitchensis` 60; `Betula pubescens` 0. `DE IPlus_Berbketal` [548 trees] was also observed as a pure Beech + Sessile Oak plot in the same dataset.)*
