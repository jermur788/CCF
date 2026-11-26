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

### Original Publisher IPT DwC-A Download — `PENDING REAL ARCHIVE DOWNLOAD` (Fallback Triggered Honestly)
- **Registered `DWC_ARCHIVE` endpoint (from `api.gbif.org/v1/dataset/2e102194-f384-4712-89a4-5db7a3fc409a`):**
  `http://dynids.toulouse.inra.fr:8180/ipt/archive.do?r=trems_integrate`
- **Observed network result (2026-11-26):**
  - `curl -fsSL --max-time 60 -o /tmp/trems_integrate.zip 'http://dynids.toulouse.inra.fr:8180/ipt/archive.do?r=trems_integrate'` → **`curl: (28) Connection timed out after 60001 milliseconds`** (`HTTP_CODE=000`, `SIZE=0`).
  - Alternate probes (`https://dynids.toulouse.inra.fr/ipt/archive.do?r=trems_integrate`, `http://dynids.toulouse.inrae.fr:8180/...`, `https://dynids.toulouse.inrae.fr/...`) all returned **`URLError: <urlopen error timed out>`**.
  - **Independent corroboration from GBIF's own crawl history (`api.gbif.org/v1/dataset/2e102194-f384-4712-89a4-5db7a3fc409a/process?limit=3`):**
    - `2026-11-25T20:11:40.149+00:00` → `finishReason: ABORT`
    - `2026-11-18T20:05:09.633+00:00` → `finishReason: ABORT`
    - `2026-11-11T19:10:37.125+00:00` → `finishReason: ABORT`
    - Last successful GBIF crawl recorded on verbatim records: **`2026-10-30T23:26:12.556+00:00` (`crawlId: 420`)**.
- **Consequence (honest boundary):** Because the publisher IPT ZIP was unreachable at qualification time, we do **NOT** have an original publisher ZIP SHA-256 or byte-level copy of the original publisher `meta.xml`. Original publisher ZIP SHA-256 and publisher `meta.xml` header/delimiter/encoding verification remain **`PENDING REAL ARCHIVE DOWNLOAD`**.

### Real CC BY 4.0 Verbatim Record Retrieval via GBIF API — `SUCCEEDED`
- `https://api.gbif.org/v1/dataset/2e102194-f384-4712-89a4-5db7a3fc409a` (HTTP 200), `/document` EML (`77,489` bytes, HTTP 200), `/v1/occurrence/search`, and `/v1/occurrence/{key}/verbatim` (HTTP 200) were all directly reachable and queried on `2026-11-26`.
- Every field encoding and plot fixture documented below was observed directly from GBIF's stored **verbatim** publisher records (`crawlId: 420`, `2026-10-30`).

---

## 2. Observed Verbatim Field, Coordinate, Extension, and Status Encodings

### Core Terms Observed on `/v1/occurrence/{key}/verbatim`
| DwC Term URI | Observed Example | Notes |
|---|---|---|
| `http://rs.tdwg.org/dwc/terms/occurrenceID` | `24839_IPlus_Donadea_MHF_2`, `33321_IPlus_Mersch` | Unique tree key: `{identificationID}_{plotName}[_{surveyIndex}]`. |
| `http://rs.tdwg.org/dwc/terms/scientificName` | `Picea abies`, `Fagus sylvatica`, `Quercus petraea` | Verbatim binomial (GBIF backbone interpreted view appends taxonomic authors such as `Fagus sylvatica L.`). Both forms are handled by `WoodlandSpeciesMapper`. |
| `http://rs.tdwg.org/dwc/terms/verbatimCoordinates` | `"34 , 48.82"`, `"18.62 , 83.47"`, `"71.28 , 16.07"` | **Local Cartesian metres `"X , Y"`** (comma-separated with surrounding spaces, invariant decimal point `.`). |
| `http://rs.tdwg.org/dwc/terms/decimalLatitude` / `decimalLongitude` | `53.3241216`, `-6.74506` (`coordinateUncertaintyInMeters: 500`) | Plot-level WGS-84 location (shared across every tree in a plot); **not** per-tree coordinates. Stored in `ExtraAttributes` (`plotDecimalLatitude`/`plotDecimalLongitude`) and never confused with per-tree local coordinates. |
| `http://rs.tdwg.org/dwc/terms/country` / `countryCode` | `Ireland` / `IE`, `Luxembourg` / `LU` | Country metadata. |
| `http://rs.tdwg.org/dwc/terms/habitat` | `Ash admixed with Norway spruce`, `Oak-beech` | Stand description preserved in `ExtraAttributes`. |
| `http://rs.tdwg.org/dwc/terms/eventDate` / `verbatimElevation` | `2019`, `80` | Survey year and plot elevation preserved in `ExtraAttributes`. |

### Extension Observed: `http://rs.tdwg.org/dwc/terms/MeasurementOrFact`
Joined to the core record by `occurrenceID`. Each tree carries multiple `MeasurementOrFact` rows:

| `measurementType` | `measurementRemarks` | `measurementUnit` | Observed `measurementValue` | Importer Mapping |
|---|---|---|---|---|
| `Tree diameter` | `diameter` | `cm` | Numeric cm (e.g. `24`, `43.4`, `62.9`) | `ImportedTreeRecord.DbhCm` (`FieldProvenance.Observed`) |
| `tree height` | `height` | `m` | Numeric m (e.g. `14`, `22`, `32.2`) | `ImportedTreeRecord.HeightM` (`FieldProvenance.Observed`) |
| `Tree status (dead or living)` | `status` | *(empty)* | **`"1"` = Living**, **`"0"` = Dead** | `ImportedTreeRecord.VitalStatus` (`Alive` / `Dead`). In `IPlus_Donadea_MHF`, `259` trees have `"1"` and `12` have `"0"`; in `IPlus_Mersch`, `131` have `"1"` and `3` have `"0"`. |
| `Tree related micro-habitat` | Code (e.g. `EP33`, `DE11`, `CV31`) | *(empty)* | Count (e.g. `1`) | Preserved verbatim in `ExtraAttributes["trem:{code}"]`. |

### Envelope Note for Committed Subset Fixtures
Because the publisher IPT `meta.xml` could not be downloaded, the two committed subset fixtures wrap the genuine verbatim CC BY 4.0 rows in a standard tab-delimited UTF-8 DwC-A (`meta.xml`, `occurrence.txt`, `measurementorfact.txt`, `fieldsTerminatedBy="\t"`, `linesTerminatedBy="\n"`, `fieldsEnclosedBy=""`, `ignoreHeaderLines="1"`, join key `<id index="0"/>` ↔ `<coreid index="0"/>`). When the INRAE IPT returns online, `meta.xml` delimiters/quote/encoding should be verified byte-for-byte against the live ZIP.

---

## 3. Qualified Plots (Committed Offline Subsets under `tests/Core/WoodlandImport/Fixtures/Qualified/`)

### 3.1 Irish Plot First — `IPlus_Donadea_MHF` (Donadea Forest Park, Co. Kildare, Ireland)
- **Full plot observed in GBIF (`country=IE`):** `271` total records (`259` living `status=1`, `12` dead `status=0`), habitat `"Ash admixed with Norway spruce"`, `53.3241216°N, -6.74506°E`, elevation `80 m`, survey `2019`.
  - Full-plot species mix: `Picea abies` (118), `Fraxinus excelsior` (102), `Ulmus glabra` (30), `Fagus sylvatica` (16), `Quercus robur` (2), `Crataegus monogyna` (2), `Sorbus aucuparia` (1).
- **Committed subset fixture:** `ie-donadea-mhf-subset.dwca.zip`
  - **Byte size:** `2,137` bytes
  - **Fixture SHA-256:** `4ee2f4a125d1a06474d7086ccf5a6ca96735e16124ad3e4f9a11647ddcee853d`
  - **Subset rows:** `20` verbatim trees spanning **all 7 observed Irish species** (`Fagus sylvatica`: 12, `Fraxinus excelsior`: 3, `Picea abies`: 1, `Ulmus glabra`: 1, `Quercus robur`: 1, `Sorbus aucuparia`: 1, `Crataegus monogyna`: 1), `18` living + `2` dead, `X ∈ [2.97, 47.73] m`, `Y ∈ [0.00, 98.35] m`.
  - **Importer readiness (Option A #1 & #4):**
    - Without boundary: `WoodlandReadiness.InventoryOnly` (`BoundaryMissing` + `UnsupportedLivingSpecies`), all 20 trees retained verbatim.
    - With supplied `50 × 100 m` (`0.50 ha`) surveyed boundary: **remains `WoodlandReadiness.InventoryOnly`** because living unmodelled species (`Fraxinus excelsior`, `Picea abies`, `Ulmus glabra`, `Quercus robur`, `Sorbus aucuparia`, `Crataegus monogyna`) block simulation admission without silent deletion or species substitution.

### 3.2 Compatible Mixed European Plot — `IPlus_Mersch` (Mersch, Luxembourg)
- **Full plot observed in GBIF (`49.745348°N, 6.141345°E`, `LU`):** `134` total records (`131` living `status=1`, `3` dead `status=0`), habitat `"Oak-beech"`.
  - Full-plot species mix: `Fagus sylvatica` (120 living, 3 dead) + `Quercus petraea` (11 living) — **100% `ModelledAtBase`** (`beech` + `sessile-oak`), zero unmodelled living species!
- **Committed subset fixture:** `lu-mersch-oak-beech-subset.dwca.zip`
  - **Byte size:** `2,145` bytes
  - **Fixture SHA-256:** `3c275b0115b6330ebabdad422ea9a25451489c93949dd5d80be1fb5e8b57b1ae`
  - **Subset rows:** `20` verbatim trees (`11` living `Quercus petraea` + `6` living `Fagus sylvatica` + `3` dead `Fagus sylvatica`), `X ∈ [7.17, 98.56] m`, `Y ∈ [6.18, 92.65] m`.
  - **Importer readiness (Option A #3 & #4):**
    - Without surveyed boundary: `WoodlandReadiness.InventoryOnly` (sole issue: `BoundaryMissing` — boundary is never inferred).
    - With supplied `100 × 100 m` (`1.00 ha`) surveyed boundary: **`WoodlandReadiness.CandidateForSimulationIntegration`** (`0` issues, all 20 trees retained, `17` living `ModelledAtBase` + `3` dead).

*(Dataset-wide target-species counts observed in GBIF on 2026-11-26 for reference: `Fagus sylvatica` 13,554; `Pinus sylvestris` 4,046; `Quercus petraea` 3,151; `Alnus glutinosa` 87; `Picea sitchensis` 60; `Betula pubescens` 0. `DE IPlus_Berbketal` [548 trees] was also verified as a pure Beech + Sessile Oak plot in the same dataset.)*
