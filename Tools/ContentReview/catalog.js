/* Generated mirror of catalog.json for file:// launch; keep byte-equivalent data. */
window.CCF_CONTENT_SEED = {
  "schemaVersion": "ccf.content.v1",
  "packageId": "ccf-s1-main-source-audit-20261113",
  "revision": 1,
  "provenance": {
    "repository": "jermur788/CCF",
    "sourceBranch": "main",
    "sourceCommit": "fc18190b976f1363f1b5d1b0881810db2f8c6e39",
    "comparisonCandidateBranch": "feature/scenario-one-objective1-ux-repair-20261112-v1",
    "extractedOn": "2026-11-13",
    "unityIntegrated": false,
    "reviewed": false,
    "coverageClaim": "representative source-extracted catalogue, not all runtime text"
  },
  "scenarios": [
    {
      "id": "s1",
      "label": "Scenario One · Inherited Sitka Plantation",
      "status": "main-source",
      "profile": "five-rack Enlarged80 / Legacy40 variants"
    },
    {
      "id": "s2",
      "label": "Scenario Two · Working Mixed Forest",
      "status": "no-shipped-text",
      "profile": "O2"
    },
    {
      "id": "s3",
      "label": "Scenario Three · New Farm Woodland",
      "status": "no-shipped-text",
      "profile": "O4"
    },
    {
      "id": "s4",
      "label": "Scenario Four · Neglected Broadleaf Wood",
      "status": "no-shipped-text",
      "profile": "O3"
    },
    {
      "id": "s5",
      "label": "Scenario Five · Exposed Upland Forest",
      "status": "no-shipped-text",
      "profile": "O7"
    },
    {
      "id": "s6",
      "label": "Scenario Six · Atlantic Oakwood",
      "status": "no-shipped-text",
      "profile": "O6"
    },
    {
      "id": "s7",
      "label": "Scenario Seven · Wet Woodland and River Valley",
      "status": "no-shipped-text",
      "profile": "O5"
    },
    {
      "id": "s8",
      "label": "Scenario Eight · Complete Irish Forest Estate",
      "status": "no-shipped-text",
      "profile": "O8"
    },
    {
      "id": "s1-compact",
      "label": "Scenario One · compact single-rack (DRAFT only)",
      "status": "draft-no-text",
      "profile": "proposal; not a shipped world"
    }
  ],
  "pages": [
    {
      "id": "s1.intro.p1",
      "scenarioId": "s1",
      "section": "Introduction",
      "title": "What is Continuous-Cover Forestry?",
      "order": 1,
      "status": "main-source"
    },
    {
      "id": "s1.intro.p2",
      "scenarioId": "s1",
      "section": "Introduction",
      "title": "How CCF works over time",
      "order": 2,
      "status": "main-source"
    },
    {
      "id": "s1.intro.p3",
      "scenarioId": "s1",
      "section": "Introduction",
      "title": "Scenario One — Transforming a Sitka Plantation",
      "order": 3,
      "status": "main-source"
    },
    {
      "id": "s1.objectives.topic1",
      "scenarioId": "s1",
      "section": "Objectives",
      "title": "1. Create access racks",
      "order": 4,
      "status": "main-source"
    },
    {
      "id": "s1.objectives.rack.read",
      "scenarioId": "s1",
      "section": "Objectives · steps",
      "title": "What an extraction rack is",
      "order": 5,
      "status": "main-source",
      "parentPageId": "s1.objectives.topic1",
      "protectedStepId": "rack.read",
      "reading": true,
      "requiresStepId": null
    },
    {
      "id": "s1.objectives.rack.create",
      "scenarioId": "s1",
      "section": "Objectives · steps",
      "title": "Create your first rack",
      "order": 6,
      "status": "main-source",
      "parentPageId": "s1.objectives.topic1",
      "protectedStepId": "rack.create",
      "reading": false,
      "requiresStepId": null
    },
    {
      "id": "s1.objectives.rack.layout",
      "scenarioId": "s1",
      "section": "Objectives · steps",
      "title": "Complete the rack layout",
      "order": 7,
      "status": "main-source",
      "parentPageId": "s1.objectives.topic1",
      "protectedStepId": "rack.layout",
      "reading": false,
      "requiresStepId": null
    },
    {
      "id": "s1.objectives.rack.approve",
      "scenarioId": "s1",
      "section": "Objectives · steps",
      "title": "Approve the rack work",
      "order": 8,
      "status": "main-source",
      "parentPageId": "s1.objectives.topic1",
      "protectedStepId": "rack.approve",
      "reading": false,
      "requiresStepId": null
    },
    {
      "id": "s1.objectives.rack.result",
      "scenarioId": "s1",
      "section": "Objectives · steps",
      "title": "Advance a year and see the racks",
      "order": 9,
      "status": "main-source",
      "parentPageId": "s1.objectives.topic1",
      "protectedStepId": "rack.result",
      "reading": false,
      "requiresStepId": null
    },
    {
      "id": "s1.objectives.topic2",
      "scenarioId": "s1",
      "section": "Objectives",
      "title": "2. Tree inspection and Crop Trees",
      "order": 10,
      "status": "main-source"
    },
    {
      "id": "s1.objectives.tree.inspect",
      "scenarioId": "s1",
      "section": "Objectives · steps",
      "title": "Inspect a living tree",
      "order": 11,
      "status": "main-source",
      "parentPageId": "s1.objectives.topic2",
      "protectedStepId": "tree.inspect",
      "reading": false,
      "requiresStepId": null
    },
    {
      "id": "s1.objectives.tree.read",
      "scenarioId": "s1",
      "section": "Objectives · steps",
      "title": "Understand the measurements",
      "order": 12,
      "status": "main-source",
      "parentPageId": "s1.objectives.topic2",
      "protectedStepId": "tree.read",
      "reading": true,
      "requiresStepId": "tree.inspect"
    },
    {
      "id": "s1.objectives.tree.crop",
      "scenarioId": "s1",
      "section": "Objectives · steps",
      "title": "Designate a Crop Tree",
      "order": 13,
      "status": "main-source",
      "parentPageId": "s1.objectives.topic2",
      "protectedStepId": "tree.crop",
      "reading": false,
      "requiresStepId": null
    },
    {
      "id": "s1.objectives.topic3",
      "scenarioId": "s1",
      "section": "Objectives",
      "title": "3. Felling",
      "order": 14,
      "status": "main-source"
    },
    {
      "id": "s1.objectives.fell.mark",
      "scenarioId": "s1",
      "section": "Objectives · steps",
      "title": "Propose a tree for felling",
      "order": 15,
      "status": "main-source",
      "parentPageId": "s1.objectives.topic3",
      "protectedStepId": "fell.mark",
      "reading": false,
      "requiresStepId": null
    },
    {
      "id": "s1.objectives.fell.plan",
      "scenarioId": "s1",
      "section": "Objectives · steps",
      "title": "Import the Fell mark into the Work Plan",
      "order": 16,
      "status": "main-source",
      "parentPageId": "s1.objectives.topic3",
      "protectedStepId": "fell.plan",
      "reading": false,
      "requiresStepId": null
    },
    {
      "id": "s1.objectives.fell.cost",
      "scenarioId": "s1",
      "section": "Objectives · steps",
      "title": "Read the harvest costs",
      "order": 17,
      "status": "main-source",
      "parentPageId": "s1.objectives.topic3",
      "protectedStepId": "fell.cost",
      "reading": true,
      "requiresStepId": "fell.plan"
    },
    {
      "id": "s1.objectives.fell.approve",
      "scenarioId": "s1",
      "section": "Objectives · steps",
      "title": "Approve a felling job when ready",
      "order": 18,
      "status": "main-source",
      "parentPageId": "s1.objectives.topic3",
      "protectedStepId": "fell.approve",
      "reading": false,
      "requiresStepId": null
    },
    {
      "id": "s1.objectives.fell.result",
      "scenarioId": "s1",
      "section": "Objectives · steps",
      "title": "Review a successful felling result",
      "order": 19,
      "status": "main-source",
      "parentPageId": "s1.objectives.topic3",
      "protectedStepId": "fell.result",
      "reading": false,
      "requiresStepId": null
    },
    {
      "id": "s1.objectives.topic4",
      "scenarioId": "s1",
      "section": "Objectives",
      "title": "4. Work Plan and Annual Review",
      "order": 20,
      "status": "main-source"
    },
    {
      "id": "s1.objectives.plan.open",
      "scenarioId": "s1",
      "section": "Objectives · steps",
      "title": "Open Work Plan",
      "order": 21,
      "status": "main-source",
      "parentPageId": "s1.objectives.topic4",
      "protectedStepId": "plan.open",
      "reading": false,
      "requiresStepId": null
    },
    {
      "id": "s1.objectives.plan.read",
      "scenarioId": "s1",
      "section": "Objectives · steps",
      "title": "Understand approval and time",
      "order": 22,
      "status": "main-source",
      "parentPageId": "s1.objectives.topic4",
      "protectedStepId": "plan.read",
      "reading": true,
      "requiresStepId": "plan.open"
    },
    {
      "id": "s1.objectives.review.open",
      "scenarioId": "s1",
      "section": "Objectives · steps",
      "title": "Open actual annual results",
      "order": 23,
      "status": "main-source",
      "parentPageId": "s1.objectives.topic4",
      "protectedStepId": "review.open",
      "reading": false,
      "requiresStepId": null
    },
    {
      "id": "s1.objectives.review.read",
      "scenarioId": "s1",
      "section": "Objectives · steps",
      "title": "Read and acknowledge the results",
      "order": 24,
      "status": "main-source",
      "parentPageId": "s1.objectives.topic4",
      "protectedStepId": "review.read",
      "reading": false,
      "requiresStepId": null
    },
    {
      "id": "s1.objectives.topic5",
      "scenarioId": "s1",
      "section": "Objectives",
      "title": "5. Stand Map",
      "order": 25,
      "status": "main-source"
    },
    {
      "id": "s1.objectives.map.open",
      "scenarioId": "s1",
      "section": "Objectives · steps",
      "title": "Open the map",
      "order": 26,
      "status": "main-source",
      "parentPageId": "s1.objectives.topic5",
      "protectedStepId": "map.open",
      "reading": false,
      "requiresStepId": null
    },
    {
      "id": "s1.objectives.map.select",
      "scenarioId": "s1",
      "section": "Objectives · steps",
      "title": "Select a square and read its information",
      "order": 27,
      "status": "main-source",
      "parentPageId": "s1.objectives.topic5",
      "protectedStepId": "map.select",
      "reading": false,
      "requiresStepId": null
    },
    {
      "id": "s1.objectives.map.light",
      "scenarioId": "s1",
      "section": "Objectives · steps",
      "title": "Read the Light layer",
      "order": 28,
      "status": "main-source",
      "parentPageId": "s1.objectives.topic5",
      "protectedStepId": "map.light",
      "reading": false,
      "requiresStepId": null
    },
    {
      "id": "s1.objectives.map.waypoint",
      "scenarioId": "s1",
      "section": "Objectives · steps",
      "title": "Set a waypoint",
      "order": 29,
      "status": "main-source",
      "parentPageId": "s1.objectives.topic5",
      "protectedStepId": "map.waypoint",
      "reading": false,
      "requiresStepId": null
    },
    {
      "id": "s1.objectives.map.return",
      "scenarioId": "s1",
      "section": "Objectives · steps",
      "title": "Return to the forest",
      "order": 30,
      "status": "main-source",
      "parentPageId": "s1.objectives.topic5",
      "protectedStepId": "map.return",
      "reading": false,
      "requiresStepId": null
    },
    {
      "id": "s1.objectives.map.arrive",
      "scenarioId": "s1",
      "section": "Objectives · steps",
      "title": "Walk to the destination",
      "order": 31,
      "status": "main-source",
      "parentPageId": "s1.objectives.topic5",
      "protectedStepId": "map.arrive",
      "reading": false,
      "requiresStepId": null
    },
    {
      "id": "s1.objectives.map.inspectsite",
      "scenarioId": "s1",
      "section": "Objectives · steps",
      "title": "Inspect the site on foot",
      "order": 32,
      "status": "main-source",
      "parentPageId": "s1.objectives.topic5",
      "protectedStepId": "map.inspectsite",
      "reading": false,
      "requiresStepId": null
    },
    {
      "id": "s1.objectives.map.regeneration",
      "scenarioId": "s1",
      "section": "Objectives · steps",
      "title": "Explore Regeneration",
      "order": 33,
      "status": "main-source",
      "parentPageId": "s1.objectives.topic5",
      "protectedStepId": "map.regeneration",
      "reading": false,
      "requiresStepId": null
    },
    {
      "id": "s1.objectives.map.browse",
      "scenarioId": "s1",
      "section": "Objectives · steps",
      "title": "Explore Browsing / protection",
      "order": 34,
      "status": "main-source",
      "parentPageId": "s1.objectives.topic5",
      "protectedStepId": "map.browse",
      "reading": false,
      "requiresStepId": null
    },
    {
      "id": "s1.objectives.map.marks",
      "scenarioId": "s1",
      "section": "Objectives · steps",
      "title": "Explore Fell & crop marks",
      "order": 35,
      "status": "main-source",
      "parentPageId": "s1.objectives.topic5",
      "protectedStepId": "map.marks",
      "reading": false,
      "requiresStepId": null
    },
    {
      "id": "s1.objectives.rack.map",
      "scenarioId": "s1",
      "section": "Objectives · steps",
      "title": "Plan a rack on the Stand Map",
      "order": 36,
      "status": "main-source",
      "parentPageId": "s1.objectives.topic5",
      "protectedStepId": "rack.map",
      "reading": false,
      "requiresStepId": null
    },
    {
      "id": "s1.objectives.map.compare",
      "scenarioId": "s1",
      "section": "Objectives · steps",
      "title": "Understand the limits of a map",
      "order": 37,
      "status": "main-source",
      "parentPageId": "s1.objectives.topic5",
      "protectedStepId": "map.compare",
      "reading": true,
      "requiresStepId": "map.inspectsite"
    },
    {
      "id": "s1.objectives.topic6",
      "scenarioId": "s1",
      "section": "Objectives",
      "title": "6. Pruning",
      "order": 38,
      "status": "main-source"
    },
    {
      "id": "s1.objectives.prune.read",
      "scenarioId": "s1",
      "section": "Objectives · steps",
      "title": "Understand eligibility",
      "order": 39,
      "status": "main-source",
      "parentPageId": "s1.objectives.topic6",
      "protectedStepId": "prune.read",
      "reading": true,
      "requiresStepId": "tree.crop"
    },
    {
      "id": "s1.objectives.prune.plan",
      "scenarioId": "s1",
      "section": "Objectives · steps",
      "title": "Plan an eligible pruning lift",
      "order": 40,
      "status": "main-source",
      "parentPageId": "s1.objectives.topic6",
      "protectedStepId": "prune.plan",
      "reading": false,
      "requiresStepId": null
    },
    {
      "id": "s1.objectives.prune.approve",
      "scenarioId": "s1",
      "section": "Objectives · steps",
      "title": "Approve the selected pruning work",
      "order": 41,
      "status": "main-source",
      "parentPageId": "s1.objectives.topic6",
      "protectedStepId": "prune.approve",
      "reading": false,
      "requiresStepId": null
    },
    {
      "id": "s1.objectives.prune.result",
      "scenarioId": "s1",
      "section": "Objectives · steps",
      "title": "Read the pruning result",
      "order": 42,
      "status": "main-source",
      "parentPageId": "s1.objectives.topic6",
      "protectedStepId": "prune.result",
      "reading": false,
      "requiresStepId": null
    },
    {
      "id": "s1.objectives.topic7",
      "scenarioId": "s1",
      "section": "Objectives",
      "title": "7. Fallen deadwood",
      "order": 43,
      "status": "main-source"
    },
    {
      "id": "s1.objectives.deadwood.read",
      "scenarioId": "s1",
      "section": "Objectives · steps",
      "title": "Understand the material choices",
      "order": 44,
      "status": "main-source",
      "parentPageId": "s1.objectives.topic7",
      "protectedStepId": "deadwood.read",
      "reading": true,
      "requiresStepId": "fell.plan"
    },
    {
      "id": "s1.objectives.deadwood.plan",
      "scenarioId": "s1",
      "section": "Objectives · steps",
      "title": "Choose Leave as deadwood",
      "order": 45,
      "status": "main-source",
      "parentPageId": "s1.objectives.topic7",
      "protectedStepId": "deadwood.plan",
      "reading": false,
      "requiresStepId": null
    },
    {
      "id": "s1.objectives.deadwood.result",
      "scenarioId": "s1",
      "section": "Objectives · steps",
      "title": "Review retained deadwood",
      "order": 46,
      "status": "main-source",
      "parentPageId": "s1.objectives.topic7",
      "protectedStepId": "deadwood.result",
      "reading": false,
      "requiresStepId": null
    },
    {
      "id": "s1.objectives.deadwood.visit",
      "scenarioId": "s1",
      "section": "Objectives · steps",
      "title": "Visit a retained fallen stem",
      "order": 47,
      "status": "main-source",
      "parentPageId": "s1.objectives.topic7",
      "protectedStepId": "deadwood.visit",
      "reading": false,
      "requiresStepId": null
    },
    {
      "id": "s1.objectives.topic8",
      "scenarioId": "s1",
      "section": "Objectives",
      "title": "8. Planting and protection",
      "order": 48,
      "status": "main-source"
    },
    {
      "id": "s1.objectives.plant.ground",
      "scenarioId": "s1",
      "section": "Objectives · steps",
      "title": "Read the ground report",
      "order": 49,
      "status": "main-source",
      "parentPageId": "s1.objectives.topic8",
      "protectedStepId": "plant.ground",
      "reading": false,
      "requiresStepId": null
    },
    {
      "id": "s1.objectives.plant.stock",
      "scenarioId": "s1",
      "section": "Objectives · steps",
      "title": "Buy nursery stock",
      "order": 50,
      "status": "main-source",
      "parentPageId": "s1.objectives.topic8",
      "protectedStepId": "plant.stock",
      "reading": false,
      "requiresStepId": null
    },
    {
      "id": "s1.objectives.plant.plan",
      "scenarioId": "s1",
      "section": "Objectives · steps",
      "title": "Mark a planting spot",
      "order": 51,
      "status": "main-source",
      "parentPageId": "s1.objectives.topic8",
      "protectedStepId": "plant.plan",
      "reading": false,
      "requiresStepId": null
    },
    {
      "id": "s1.objectives.plant.execution",
      "scenarioId": "s1",
      "section": "Objectives · steps",
      "title": "Read executor and shelter choices",
      "order": 52,
      "status": "main-source",
      "parentPageId": "s1.objectives.topic8",
      "protectedStepId": "plant.execution",
      "reading": true,
      "requiresStepId": "plant.plan"
    },
    {
      "id": "s1.objectives.plant.shelter",
      "scenarioId": "s1",
      "section": "Objectives · steps",
      "title": "Plan a shelter when appropriate",
      "order": 53,
      "status": "main-source",
      "parentPageId": "s1.objectives.topic8",
      "protectedStepId": "plant.shelter",
      "reading": false,
      "requiresStepId": null
    },
    {
      "id": "s1.objectives.plant.approve",
      "scenarioId": "s1",
      "section": "Objectives · steps",
      "title": "Approve planting work",
      "order": 54,
      "status": "main-source",
      "parentPageId": "s1.objectives.topic8",
      "protectedStepId": "plant.approve",
      "reading": false,
      "requiresStepId": null
    },
    {
      "id": "s1.objectives.plant.result",
      "scenarioId": "s1",
      "section": "Objectives · steps",
      "title": "Read planting results",
      "order": 55,
      "status": "main-source",
      "parentPageId": "s1.objectives.topic8",
      "protectedStepId": "plant.result",
      "reading": false,
      "requiresStepId": null
    },
    {
      "id": "s1.objectives.topic9",
      "scenarioId": "s1",
      "section": "Objectives",
      "title": "9. Vegetation clearance",
      "order": 56,
      "status": "main-source"
    },
    {
      "id": "s1.objectives.clear.read",
      "scenarioId": "s1",
      "section": "Objectives · steps",
      "title": "Read the footprint before planning",
      "order": 57,
      "status": "main-source",
      "parentPageId": "s1.objectives.topic9",
      "protectedStepId": "clear.read",
      "reading": true,
      "requiresStepId": "plant.ground"
    },
    {
      "id": "s1.objectives.clear.plan",
      "scenarioId": "s1",
      "section": "Objectives · steps",
      "title": "Plan a clearance area",
      "order": 58,
      "status": "main-source",
      "parentPageId": "s1.objectives.topic9",
      "protectedStepId": "clear.plan",
      "reading": false,
      "requiresStepId": null
    },
    {
      "id": "s1.objectives.clear.approve",
      "scenarioId": "s1",
      "section": "Objectives · steps",
      "title": "Approve clearance when ready",
      "order": 59,
      "status": "main-source",
      "parentPageId": "s1.objectives.topic9",
      "protectedStepId": "clear.approve",
      "reading": false,
      "requiresStepId": null
    },
    {
      "id": "s1.objectives.clear.result",
      "scenarioId": "s1",
      "section": "Objectives · steps",
      "title": "Read clearance results",
      "order": 60,
      "status": "main-source",
      "parentPageId": "s1.objectives.topic9",
      "protectedStepId": "clear.result",
      "reading": false,
      "requiresStepId": null
    },
    {
      "id": "s1.teaching.access",
      "scenarioId": "s1",
      "section": "Teaching sequence",
      "title": "1. Create access racks",
      "order": 61,
      "status": "main-source",
      "protectedLessonId": "access"
    },
    {
      "id": "s1.teaching.future-trees",
      "scenarioId": "s1",
      "section": "Teaching sequence",
      "title": "2. Choose promising trees to retain along the rack",
      "order": 62,
      "status": "main-source",
      "protectedLessonId": "future-trees"
    },
    {
      "id": "s1.teaching.release",
      "scenarioId": "s1",
      "section": "Teaching sequence",
      "title": "3. Identify the competitors that matter",
      "order": 63,
      "status": "main-source",
      "protectedLessonId": "release"
    },
    {
      "id": "s1.teaching.habitat",
      "scenarioId": "s1",
      "section": "Teaching sequence",
      "title": "4. Plan and carry out the intervention",
      "order": 64,
      "status": "main-source",
      "protectedLessonId": "habitat"
    },
    {
      "id": "s1.teaching.response",
      "scenarioId": "s1",
      "section": "Teaching sequence",
      "title": "5. Return and review",
      "order": 65,
      "status": "main-source",
      "protectedLessonId": "response"
    },
    {
      "id": "s1.teaching.recruitment",
      "scenarioId": "s1",
      "section": "Teaching sequence",
      "title": "6. Secure the next generation",
      "order": 66,
      "status": "main-source",
      "protectedLessonId": "recruitment"
    },
    {
      "id": "s1.teaching.establishment",
      "scenarioId": "s1",
      "section": "Teaching sequence",
      "title": "7. Return to the young trees",
      "order": 67,
      "status": "main-source",
      "protectedLessonId": "establishment"
    },
    {
      "id": "s1.teaching.transformation",
      "scenarioId": "s1",
      "section": "Teaching sequence",
      "title": "8. Continue the transformation",
      "order": 68,
      "status": "main-source",
      "protectedLessonId": "transformation"
    },
    {
      "id": "s1.help.walkinghud",
      "scenarioId": "s1",
      "section": "F1 & first-use help",
      "title": "WalkingHud",
      "order": 69,
      "status": "main-source"
    },
    {
      "id": "s1.help.treeinspection",
      "scenarioId": "s1",
      "section": "F1 & first-use help",
      "title": "TreeInspection",
      "order": 70,
      "status": "main-source"
    },
    {
      "id": "s1.help.standmap",
      "scenarioId": "s1",
      "section": "F1 & first-use help",
      "title": "StandMap",
      "order": 71,
      "status": "main-source"
    },
    {
      "id": "s1.help.workplan",
      "scenarioId": "s1",
      "section": "F1 & first-use help",
      "title": "WorkPlan",
      "order": 72,
      "status": "main-source"
    },
    {
      "id": "s1.help.annualreview",
      "scenarioId": "s1",
      "section": "F1 & first-use help",
      "title": "AnnualReview",
      "order": 73,
      "status": "main-source"
    },
    {
      "id": "s1.help.rack",
      "scenarioId": "s1",
      "section": "F1 & first-use help",
      "title": "Rack",
      "order": 74,
      "status": "main-source"
    },
    {
      "id": "s1.help.treemarks",
      "scenarioId": "s1",
      "section": "F1 & first-use help",
      "title": "TreeMarks",
      "order": 75,
      "status": "main-source"
    },
    {
      "id": "s1.controls.reference",
      "scenarioId": "s1",
      "section": "Controls",
      "title": "Controls card & key legend",
      "order": 76,
      "status": "main-source"
    },
    {
      "id": "s1.annual.review",
      "scenarioId": "s1",
      "section": "Annual Review",
      "title": "Annual review and historical year labels",
      "order": 77,
      "status": "main-source"
    },
    {
      "id": "s1.workplan.overview",
      "scenarioId": "s1",
      "section": "Work Plan",
      "title": "Work Plan summary / annual actions",
      "order": 78,
      "status": "main-source"
    },
    {
      "id": "s1.candidate.ux-443",
      "scenarioId": "s1",
      "section": "Unmerged Objective 1 candidate",
      "title": "Objective 1 UX candidate copy (not main)",
      "order": 79,
      "status": "unmerged-candidate"
    }
  ],
  "entries": [
    {
      "id": "s1.intro.p1.title",
      "pageId": "s1.intro.p1",
      "fieldKey": "title",
      "variantId": "default",
      "text": "What is Continuous-Cover Forestry?",
      "baselineText": "What is Continuous-Cover Forestry?",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/ScenarioOneOnboardingCopy.cs",
        "branch": "main",
        "line": 6,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": null,
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.intro.p1.lead",
      "pageId": "s1.intro.p1",
      "fieldKey": "lead",
      "variantId": "default",
      "text": "Continuous-cover forestry keeps a forest continuously wooded while it is managed. Instead of clear-felling the whole stand, trees are removed selectively and new generations establish beneath and between those that remain.",
      "baselineText": "Continuous-cover forestry keeps a forest continuously wooded while it is managed. Instead of clear-felling the whole stand, trees are removed selectively and new generations establish beneath and between those that remain.",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/ScenarioOneOnboardingCopy.cs",
        "branch": "main",
        "line": 11,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": null,
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.intro.p1.footer",
      "pageId": "s1.intro.p1",
      "fieldKey": "footer",
      "variantId": "default",
      "text": "CCF is a long-term process of repeated decisions, observation and adaptation.",
      "baselineText": "CCF is a long-term process of repeated decisions, observation and adaptation.",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/ScenarioOneOnboardingCopy.cs",
        "branch": "main",
        "line": 17,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": null,
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.intro.p1.renewal",
      "pageId": "s1.intro.p1",
      "fieldKey": "renewal",
      "variantId": "default",
      "text": "The forest is renewed gradually over time, creating a mixed-aged, more resilient and productive woodland.",
      "baselineText": "The forest is renewed gradually over time, creating a mixed-aged, more resilient and productive woodland.",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/ScenarioOneOnboardingCopy.cs",
        "branch": "main",
        "line": 15,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": null,
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.intro.p1.principle1",
      "pageId": "s1.intro.p1",
      "fieldKey": "principle1",
      "variantId": "default",
      "text": "Keep forest cover",
      "baselineText": "Keep forest cover",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/ScenarioOneOnboardingCopy.cs",
        "branch": "main",
        "line": 22,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": null,
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.intro.p1.principle2",
      "pageId": "s1.intro.p1",
      "fieldKey": "principle2",
      "variantId": "default",
      "text": "Manage repeatedly over time",
      "baselineText": "Manage repeatedly over time",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/ScenarioOneOnboardingCopy.cs",
        "branch": "main",
        "line": 22,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": null,
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.intro.p1.principle3",
      "pageId": "s1.intro.p1",
      "fieldKey": "principle3",
      "variantId": "default",
      "text": "Encourage regeneration",
      "baselineText": "Encourage regeneration",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/ScenarioOneOnboardingCopy.cs",
        "branch": "main",
        "line": 22,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": null,
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.intro.p1.principle4",
      "pageId": "s1.intro.p1",
      "fieldKey": "principle4",
      "variantId": "default",
      "text": "Build varied structure",
      "baselineText": "Build varied structure",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/ScenarioOneOnboardingCopy.cs",
        "branch": "main",
        "line": 22,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": null,
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.intro.p1.progressionHeading1",
      "pageId": "s1.intro.p1",
      "fieldKey": "progressionHeading1",
      "variantId": "default",
      "text": "BEFORE",
      "baselineText": "BEFORE",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/ScenarioOneOnboardingCopy.cs",
        "branch": "main",
        "line": 24,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": null,
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.intro.p1.progressionHeading2",
      "pageId": "s1.intro.p1",
      "fieldKey": "progressionHeading2",
      "variantId": "default",
      "text": "SELECTIVE TRANSFORMATION",
      "baselineText": "SELECTIVE TRANSFORMATION",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/ScenarioOneOnboardingCopy.cs",
        "branch": "main",
        "line": 24,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": null,
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.intro.p1.progressionHeading3",
      "pageId": "s1.intro.p1",
      "fieldKey": "progressionHeading3",
      "variantId": "default",
      "text": "DEVELOPED CCF STRUCTURE",
      "baselineText": "DEVELOPED CCF STRUCTURE",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/ScenarioOneOnboardingCopy.cs",
        "branch": "main",
        "line": 24,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": null,
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.intro.p1.progressionCaption1",
      "pageId": "s1.intro.p1",
      "fieldKey": "progressionCaption1",
      "variantId": "default",
      "text": "Even-aged Sitka plantation",
      "baselineText": "Even-aged Sitka plantation",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/ScenarioOneOnboardingCopy.cs",
        "branch": "main",
        "line": 26,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": null,
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.intro.p1.progressionCaption2",
      "pageId": "s1.intro.p1",
      "fieldKey": "progressionCaption2",
      "variantId": "default",
      "text": "Mature trees retained; openings allow regeneration",
      "baselineText": "Mature trees retained; openings allow regeneration",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/ScenarioOneOnboardingCopy.cs",
        "branch": "main",
        "line": 26,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": null,
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.intro.p1.progressionCaption3",
      "pageId": "s1.intro.p1",
      "fieldKey": "progressionCaption3",
      "variantId": "default",
      "text": "Productive trees, broadleaf diversity and new generations",
      "baselineText": "Productive trees, broadleaf diversity and new generations",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/ScenarioOneOnboardingCopy.cs",
        "branch": "main",
        "line": 26,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": null,
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.intro.p2.title",
      "pageId": "s1.intro.p2",
      "fieldKey": "title",
      "variantId": "default",
      "text": "How CCF works over time",
      "baselineText": "How CCF works over time",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/ScenarioOneOnboardingCopy.cs",
        "branch": "main",
        "line": 7,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": null,
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.intro.p2.lead",
      "pageId": "s1.intro.p2",
      "fieldKey": "lead",
      "variantId": "default",
      "text": "CCF is a cycle of decisions, observation and adaptation. Retain continuous forest cover as you create space for new generations and review how the stand develops.",
      "baselineText": "CCF is a cycle of decisions, observation and adaptation. Retain continuous forest cover as you create space for new generations and review how the stand develops.",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/ScenarioOneOnboardingCopy.cs",
        "branch": "main",
        "line": 12,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": null,
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.intro.p2.footer",
      "pageId": "s1.intro.p2",
      "fieldKey": "footer",
      "variantId": "default",
      "text": "Retained trees, regeneration and varied ages and sizes build a continuously covered forest over time.",
      "baselineText": "Retained trees, regeneration and varied ages and sizes build a continuously covered forest over time.",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/ScenarioOneOnboardingCopy.cs",
        "branch": "main",
        "line": 18,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": null,
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.intro.p2.process1Heading",
      "pageId": "s1.intro.p2",
      "fieldKey": "process1Heading",
      "variantId": "default",
      "text": "Observe the stand",
      "baselineText": "Observe the stand",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/ScenarioOneOnboardingCopy.cs",
        "branch": "main",
        "line": 29,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": null,
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.intro.p2.process2Heading",
      "pageId": "s1.intro.p2",
      "fieldKey": "process2Heading",
      "variantId": "default",
      "text": "Retain and select",
      "baselineText": "Retain and select",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/ScenarioOneOnboardingCopy.cs",
        "branch": "main",
        "line": 29,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": null,
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.intro.p2.process3Heading",
      "pageId": "s1.intro.p2",
      "fieldKey": "process3Heading",
      "variantId": "default",
      "text": "Create space and light",
      "baselineText": "Create space and light",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/ScenarioOneOnboardingCopy.cs",
        "branch": "main",
        "line": 29,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": null,
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.intro.p2.process4Heading",
      "pageId": "s1.intro.p2",
      "fieldKey": "process4Heading",
      "variantId": "default",
      "text": "Regenerate and establish",
      "baselineText": "Regenerate and establish",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/ScenarioOneOnboardingCopy.cs",
        "branch": "main",
        "line": 29,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": null,
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.intro.p2.process5Heading",
      "pageId": "s1.intro.p2",
      "fieldKey": "process5Heading",
      "variantId": "default",
      "text": "Review and repeat",
      "baselineText": "Review and repeat",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/ScenarioOneOnboardingCopy.cs",
        "branch": "main",
        "line": 29,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": null,
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.intro.p2.process1Body",
      "pageId": "s1.intro.p2",
      "fieldKey": "process1Body",
      "variantId": "default",
      "text": "Read its structure, regeneration and growing conditions.",
      "baselineText": "Read its structure, regeneration and growing conditions.",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/ScenarioOneOnboardingCopy.cs",
        "branch": "main",
        "line": 32,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": null,
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.intro.p2.process2Body",
      "pageId": "s1.intro.p2",
      "fieldKey": "process2Body",
      "variantId": "default",
      "text": "Keep productive trees and select competing trees for removal.",
      "baselineText": "Keep productive trees and select competing trees for removal.",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/ScenarioOneOnboardingCopy.cs",
        "branch": "main",
        "line": 33,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": null,
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.intro.p2.process3Body",
      "pageId": "s1.intro.p2",
      "fieldKey": "process3Body",
      "variantId": "default",
      "text": "Remove selected competitors to make suitable conditions for young trees.",
      "baselineText": "Remove selected competitors to make suitable conditions for young trees.",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/ScenarioOneOnboardingCopy.cs",
        "branch": "main",
        "line": 34,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": null,
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.intro.p2.process4Body",
      "pageId": "s1.intro.p2",
      "fieldKey": "process4Body",
      "variantId": "default",
      "text": "Encourage natural regeneration and establish other species for a diverse forest.",
      "baselineText": "Encourage natural regeneration and establish other species for a diverse forest.",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/ScenarioOneOnboardingCopy.cs",
        "branch": "main",
        "line": 35,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": null,
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.intro.p2.process5Body",
      "pageId": "s1.intro.p2",
      "fieldKey": "process5Body",
      "variantId": "default",
      "text": "Observe the response, adapt decisions and repeat management over many years.",
      "baselineText": "Observe the response, adapt decisions and repeat management over many years.",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/ScenarioOneOnboardingCopy.cs",
        "branch": "main",
        "line": 36,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": null,
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.intro.p3.title",
      "pageId": "s1.intro.p3",
      "fieldKey": "title",
      "variantId": "default",
      "text": "Scenario One — Transforming a Sitka Plantation",
      "baselineText": "Scenario One — Transforming a Sitka Plantation",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/ScenarioOneOnboardingCopy.cs",
        "branch": "main",
        "line": 8,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": null,
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.intro.p3.lead",
      "pageId": "s1.intro.p3",
      "fieldKey": "lead",
      "variantId": "default",
      "text": "You begin with an even-aged Sitka plantation. Your task is to retain the best Sitka, create the conditions for change, and work out how to establish both Oak and Beech so that the future forest contains all three.",
      "baselineText": "You begin with an even-aged Sitka plantation. Your task is to retain the best Sitka, create the conditions for change, and work out how to establish both Oak and Beech so that the future forest contains all three.",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/ScenarioOneOnboardingCopy.cs",
        "branch": "main",
        "line": 13,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": null,
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.intro.p3.footer",
      "pageId": "s1.intro.p3",
      "fieldKey": "footer",
      "variantId": "default",
      "text": "Establish and retain all three species while maintaining a productive, continuously covered forest.",
      "baselineText": "Establish and retain all three species while maintaining a productive, continuously covered forest.",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/ScenarioOneOnboardingCopy.cs",
        "branch": "main",
        "line": 19,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": null,
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.intro.p3.InitialState",
      "pageId": "s1.intro.p3",
      "fieldKey": "InitialState",
      "variantId": "default",
      "text": "Largely one species\nSimilar ages and sizes\nRegular plantation spacing",
      "baselineText": "Largely one species\nSimilar ages and sizes\nRegular plantation spacing",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/ScenarioOneOnboardingCopy.cs",
        "branch": "main",
        "line": 38,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": null,
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.intro.p3.TargetState",
      "pageId": "s1.intro.p3",
      "fieldKey": "TargetState",
      "variantId": "default",
      "text": "Productive Sitka retained\nBoth Oak and Beech established\nVaried ages and sizes\nRegeneration beneath and between trees\nContinuous cover and varied structure",
      "baselineText": "Productive Sitka retained\nBoth Oak and Beech established\nVaried ages and sizes\nRegeneration beneath and between trees\nContinuous cover and varied structure",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/ScenarioOneOnboardingCopy.cs",
        "branch": "main",
        "line": 39,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": null,
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.intro.p3.KeyIdea",
      "pageId": "s1.intro.p3",
      "fieldKey": "KeyIdea",
      "variantId": "default",
      "text": "Transformation is gradual. Repeat management cycles, observe the forest's response and adapt your next decisions.",
      "baselineText": "Transformation is gradual. Repeat management cycles, observe the forest's response and adapt your next decisions.",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/ScenarioOneOnboardingCopy.cs",
        "branch": "main",
        "line": 40,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": null,
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.objectives.topic1.title",
      "pageId": "s1.objectives.topic1",
      "fieldKey": "title",
      "variantId": "default",
      "text": "1. Create access racks",
      "baselineText": "1. Create access racks",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs",
        "branch": "main",
        "line": 25,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": null,
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.objectives.topic1.intro",
      "pageId": "s1.objectives.topic1",
      "fieldKey": "intro",
      "variantId": "default",
      "text": "Scenario One begins by opening machine routes through the plantation, before any selective thinning. Racks are permanent routes used to remove timber from the stand. Establishing them now gives machinery access for this and future thinnings. Nothing is cut until you approve the work and advance a year.",
      "baselineText": "Scenario One begins by opening machine routes through the plantation, before any selective thinning. Racks are permanent routes used to remove timber from the stand. Establishing them now gives machinery access for this and future thinnings. Nothing is cut until you approve the work and advance a year.",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs",
        "branch": "main",
        "line": 25,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": null,
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.objectives.rack.read.title",
      "pageId": "s1.objectives.rack.read",
      "fieldKey": "title",
      "variantId": "default",
      "text": "What an extraction rack is",
      "baselineText": "What an extraction rack is",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs",
        "branch": "main",
        "line": 26,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": "rack.read",
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.objectives.rack.read.body.legacy-or-fallback",
      "pageId": "s1.objectives.rack.read",
      "fieldKey": "body",
      "variantId": "legacy-or-fallback",
      "text": "A rack is a straight corridor of trees felled across the plantation so machinery can drive along it. It is permanent: it stays open for this and later thinnings. This plantation follows an authored rack plan of five evenly spaced racks across the planted rows (seven rows left standing between them). The status line and the Stand Map show how many are done.",
      "baselineText": "A rack is a straight corridor of trees felled across the plantation so machinery can drive along it. It is permanent: it stays open for this and later thinnings. This plantation follows an authored rack plan of five evenly spaced racks across the planted rows (seven rows left standing between them). The status line and the Stand Map show how many are done.",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs",
        "branch": "main",
        "line": 26,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": "rack.read",
      "format": "plain-text",
      "tokens": [],
      "note": "Legacy/general base wording; Rack V2 may replace at runtime"
    },
    {
      "id": "s1.objectives.rack.create.title",
      "pageId": "s1.objectives.rack.create",
      "fieldKey": "title",
      "variantId": "default",
      "text": "Create your first rack",
      "baselineText": "Create your first rack",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs",
        "branch": "main",
        "line": 27,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": "rack.create",
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.objectives.rack.create.body.legacy-or-fallback",
      "pageId": "s1.objectives.rack.create",
      "fieldKey": "body",
      "variantId": "legacy-or-fallback",
      "text": "Press B in the forest. Click the intended starting tree or ground point to fix the origin. Aim to pivot the violet corridor forward; the mouse wheel adjusts its direction. Click again to add the previewed trees to the Work Plan. Right-click chooses another start; Esc or B cancels.",
      "baselineText": "Press B in the forest. Click the intended starting tree or ground point to fix the origin. Aim to pivot the violet corridor forward; the mouse wheel adjusts its direction. Click again to add the previewed trees to the Work Plan. Right-click chooses another start; Esc or B cancels.",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs",
        "branch": "main",
        "line": 27,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": "rack.create",
      "format": "plain-text",
      "tokens": [],
      "note": "Legacy/general base wording; Rack V2 may replace at runtime"
    },
    {
      "id": "s1.objectives.rack.layout.title",
      "pageId": "s1.objectives.rack.layout",
      "fieldKey": "title",
      "variantId": "default",
      "text": "Complete the rack layout",
      "baselineText": "Complete the rack layout",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs",
        "branch": "main",
        "line": 28,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": "rack.layout",
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.objectives.rack.layout.body.legacy-or-fallback",
      "pageId": "s1.objectives.rack.layout",
      "fieldKey": "body",
      "variantId": "legacy-or-fallback",
      "text": "Open the rest of the authored rack plan: five racks in all. Use B in the forest, or open the Stand Map [M] and choose Plan rack, which shows the rows still to do. The status line counts the racks already in the Work Plan.",
      "baselineText": "Open the rest of the authored rack plan: five racks in all. Use B in the forest, or open the Stand Map [M] and choose Plan rack, which shows the rows still to do. The status line counts the racks already in the Work Plan.",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs",
        "branch": "main",
        "line": 28,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": "rack.layout",
      "format": "plain-text",
      "tokens": [],
      "note": "Legacy/general base wording; Rack V2 may replace at runtime"
    },
    {
      "id": "s1.objectives.rack.approve.title",
      "pageId": "s1.objectives.rack.approve",
      "fieldKey": "title",
      "variantId": "default",
      "text": "Approve the rack work",
      "baselineText": "Approve the rack work",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs",
        "branch": "main",
        "line": 29,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": "rack.approve",
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.objectives.rack.approve.body.legacy-or-fallback",
      "pageId": "s1.objectives.rack.approve",
      "fieldKey": "body",
      "variantId": "legacy-or-fallback",
      "text": "Open the Work Plan [Tab]. Each rack is one group with its trees, timber and income. Check the cost, then choose Approve Work Plan. Approval does not cut anything yet.",
      "baselineText": "Open the Work Plan [Tab]. Each rack is one group with its trees, timber and income. Check the cost, then choose Approve Work Plan. Approval does not cut anything yet.",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs",
        "branch": "main",
        "line": 29,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": "rack.approve",
      "format": "plain-text",
      "tokens": [],
      "note": "Legacy/general base wording; Rack V2 may replace at runtime"
    },
    {
      "id": "s1.objectives.rack.result.title",
      "pageId": "s1.objectives.rack.result",
      "fieldKey": "title",
      "variantId": "default",
      "text": "Advance a year and see the racks",
      "baselineText": "Advance a year and see the racks",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs",
        "branch": "main",
        "line": 30,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": "rack.result",
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.objectives.rack.result.body",
      "pageId": "s1.objectives.rack.result",
      "fieldKey": "body",
      "variantId": "default",
      "text": "Advance one year and read the Annual Review. Then walk a rack: it is now an open corridor of stumps across the plantation. Inspecting trees, Crop Trees and selective felling come next.",
      "baselineText": "Advance one year and read the Annual Review. Then walk a rack: it is now an open corridor of stumps across the plantation. Inspecting trees, Crop Trees and selective felling come next.",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs",
        "branch": "main",
        "line": 30,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": "rack.result",
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.objectives.topic2.title",
      "pageId": "s1.objectives.topic2",
      "fieldKey": "title",
      "variantId": "default",
      "text": "2. Tree inspection and Crop Trees",
      "baselineText": "2. Tree inspection and Crop Trees",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs",
        "branch": "main",
        "line": 31,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": null,
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.objectives.topic2.intro",
      "pageId": "s1.objectives.topic2",
      "fieldKey": "intro",
      "variantId": "default",
      "text": "Begin with trees worth retaining and favouring, then inspect their competitors. Being smaller or suppressed is not by itself a reason to remove a tree. There is no single correct tree picked by this lesson.",
      "baselineText": "Begin with trees worth retaining and favouring, then inspect their competitors. Being smaller or suppressed is not by itself a reason to remove a tree. There is no single correct tree picked by this lesson.",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs",
        "branch": "main",
        "line": 31,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": null,
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.objectives.tree.inspect.title",
      "pageId": "s1.objectives.tree.inspect",
      "fieldKey": "title",
      "variantId": "default",
      "text": "Inspect a living tree",
      "baselineText": "Inspect a living tree",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs",
        "branch": "main",
        "line": 32,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": "tree.inspect",
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.objectives.tree.inspect.body",
      "pageId": "s1.objectives.tree.inspect",
      "fieldKey": "body",
      "variantId": "default",
      "text": "Look at a tree and press E. Read its identity, age, height, DBH and competition. E closes inspection; F1 revisits the explanation.",
      "baselineText": "Look at a tree and press E. Read its identity, age, height, DBH and competition. E closes inspection; F1 revisits the explanation.",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs",
        "branch": "main",
        "line": 32,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": "tree.inspect",
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.objectives.tree.read.title",
      "pageId": "s1.objectives.tree.read",
      "fieldKey": "title",
      "variantId": "default",
      "text": "Understand the measurements",
      "baselineText": "Understand the measurements",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs",
        "branch": "main",
        "line": 33,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": "tree.read",
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.objectives.tree.read.body",
      "pageId": "s1.objectives.tree.read",
      "fieldKey": "body",
      "variantId": "default",
      "text": "DBH is trunk diameter at breast height. Competition describes neighbours restricting growth. The light figure is the light on the ground at the tree's base. Use the fields together with the tree and its neighbours, not as a command to remove it.",
      "baselineText": "DBH is trunk diameter at breast height. Competition describes neighbours restricting growth. The light figure is the light on the ground at the tree's base. Use the fields together with the tree and its neighbours, not as a command to remove it.",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs",
        "branch": "main",
        "line": 33,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": "tree.read",
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.objectives.tree.crop.title",
      "pageId": "s1.objectives.tree.crop",
      "fieldKey": "title",
      "variantId": "default",
      "text": "Designate a Crop Tree",
      "baselineText": "Designate a Crop Tree",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs",
        "branch": "main",
        "line": 34,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": "tree.crop",
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.objectives.tree.crop.body",
      "pageId": "s1.objectives.tree.crop",
      "fieldKey": "body",
      "variantId": "default",
      "text": "Aim at a living tree you want to retain and press C. A blue mark means retain/favour, not fell. Press C again to undo it. Choose a tree yourself after inspection; this designation is also used by crop-tree pruning.",
      "baselineText": "Aim at a living tree you want to retain and press C. A blue mark means retain/favour, not fell. Press C again to undo it. Choose a tree yourself after inspection; this designation is also used by crop-tree pruning.",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs",
        "branch": "main",
        "line": 34,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": "tree.crop",
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.objectives.topic3.title",
      "pageId": "s1.objectives.topic3",
      "fieldKey": "title",
      "variantId": "default",
      "text": "3. Felling",
      "baselineText": "3. Felling",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs",
        "branch": "main",
        "line": 35,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": null,
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.objectives.topic3.intro",
      "pageId": "s1.objectives.topic3",
      "fieldKey": "intro",
      "variantId": "default",
      "text": "Continuous-cover forestry needs repeated selective management. More felling is not automatically better: consider what remains, then return later to inspect and reassess. Learn the proposal → plan → approval → results sequence. Take time to compare trees; no cutting is required in year one.",
      "baselineText": "Continuous-cover forestry needs repeated selective management. More felling is not automatically better: consider what remains, then return later to inspect and reassess. Learn the proposal → plan → approval → results sequence. Take time to compare trees; no cutting is required in year one.",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs",
        "branch": "main",
        "line": 35,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": null,
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.objectives.fell.mark.title",
      "pageId": "s1.objectives.fell.mark",
      "fieldKey": "title",
      "variantId": "default",
      "text": "Propose a tree for felling",
      "baselineText": "Propose a tree for felling",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs",
        "branch": "main",
        "line": 36,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": "fell.mark",
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.objectives.fell.mark.body",
      "pageId": "s1.objectives.fell.mark",
      "fieldKey": "body",
      "variantId": "default",
      "text": "Aim at a living tree and press X. The red mark is only a proposal. X again removes it. Crop and Fell marks are alternatives on the same tree; check the label before proceeding.",
      "baselineText": "Aim at a living tree and press X. The red mark is only a proposal. X again removes it. Crop and Fell marks are alternatives on the same tree; check the label before proceeding.",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs",
        "branch": "main",
        "line": 36,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": "fell.mark",
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.objectives.fell.plan.title",
      "pageId": "s1.objectives.fell.plan",
      "fieldKey": "title",
      "variantId": "default",
      "text": "Import the Fell mark into the Work Plan",
      "baselineText": "Import the Fell mark into the Work Plan",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs",
        "branch": "main",
        "line": 37,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": "fell.plan",
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.objectives.fell.plan.body",
      "pageId": "s1.objectives.fell.plan",
      "fieldKey": "body",
      "variantId": "default",
      "text": "Press Tab. Your red Fell marks join the Work Plan as jobs when you open it. Inspect each job's timber estimate, contractor cost and material outcome. Removing a pending job lets you reconsider.",
      "baselineText": "Press Tab. Your red Fell marks join the Work Plan as jobs when you open it. Inspect each job's timber estimate, contractor cost and material outcome. Removing a pending job lets you reconsider.",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs",
        "branch": "main",
        "line": 37,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": "fell.plan",
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.objectives.fell.cost.title",
      "pageId": "s1.objectives.fell.cost",
      "fieldKey": "title",
      "variantId": "default",
      "text": "Read the harvest costs",
      "baselineText": "Read the harvest costs",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs",
        "branch": "main",
        "line": 38,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": "fell.cost",
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.objectives.fell.cost.body",
      "pageId": "s1.objectives.fell.cost",
      "fieldKey": "body",
      "variantId": "default",
      "text": "Compare total labour, timber income, net value and remaining cash. A small visit can be dominated by the contractor's minimum charge. Estimates are re-quoted when work resolves.",
      "baselineText": "Compare total labour, timber income, net value and remaining cash. A small visit can be dominated by the contractor's minimum charge. Estimates are re-quoted when work resolves.",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs",
        "branch": "main",
        "line": 38,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": "fell.cost",
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.objectives.fell.approve.title",
      "pageId": "s1.objectives.fell.approve",
      "fieldKey": "title",
      "variantId": "default",
      "text": "Approve a felling job when ready",
      "baselineText": "Approve a felling job when ready",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs",
        "branch": "main",
        "line": 39,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": "fell.approve",
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.objectives.fell.approve.body",
      "pageId": "s1.objectives.fell.approve",
      "fieldKey": "body",
      "variantId": "default",
      "text": "Approve Work Plan commits the planned jobs. Approval does not immediately cut trees. Check every pending job first: that button can approve more than your felling proposal.",
      "baselineText": "Approve Work Plan commits the planned jobs. Approval does not immediately cut trees. Check every pending job first: that button can approve more than your felling proposal.",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs",
        "branch": "main",
        "line": 39,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": "fell.approve",
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.objectives.fell.result.title",
      "pageId": "s1.objectives.fell.result",
      "fieldKey": "title",
      "variantId": "default",
      "text": "Review a successful felling result",
      "baselineText": "Review a successful felling result",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs",
        "branch": "main",
        "line": 40,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": "fell.result",
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.objectives.fell.result.body",
      "pageId": "s1.objectives.fell.result",
      "fieldKey": "body",
      "variantId": "default",
      "text": "Advance a year when your plan is ready. Open Annual Review and read WORK DONE, MONEY and FOREST. Successful felling removes the standing tree; failed work needs its failure reason checked. Walk back to inspect the change.",
      "baselineText": "Advance a year when your plan is ready. Open Annual Review and read WORK DONE, MONEY and FOREST. Successful felling removes the standing tree; failed work needs its failure reason checked. Walk back to inspect the change.",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs",
        "branch": "main",
        "line": 40,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": "fell.result",
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.objectives.topic4.title",
      "pageId": "s1.objectives.topic4",
      "fieldKey": "title",
      "variantId": "default",
      "text": "4. Work Plan and Annual Review",
      "baselineText": "4. Work Plan and Annual Review",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs",
        "branch": "main",
        "line": 41,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": null,
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.objectives.topic4.intro",
      "pageId": "s1.objectives.topic4",
      "fieldKey": "intro",
      "variantId": "default",
      "text": "The forest is where you decide what should happen. The Work Plan reviews execution and cost; Annual Review records what actually happened.",
      "baselineText": "The forest is where you decide what should happen. The Work Plan reviews execution and cost; Annual Review records what actually happened.",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs",
        "branch": "main",
        "line": 41,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": null,
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.objectives.plan.open.title",
      "pageId": "s1.objectives.plan.open",
      "fieldKey": "title",
      "variantId": "default",
      "text": "Open Work Plan",
      "baselineText": "Open Work Plan",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs",
        "branch": "main",
        "line": 42,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": "plan.open",
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.objectives.plan.open.body",
      "pageId": "s1.objectives.plan.open",
      "fieldKey": "body",
      "variantId": "default",
      "text": "Press Tab. Compare your open jobs, cash, expected timber income and available owner time. Opening the plan neither approves jobs nor advances time.",
      "baselineText": "Press Tab. Compare your open jobs, cash, expected timber income and available owner time. Opening the plan neither approves jobs nor advances time.",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs",
        "branch": "main",
        "line": 42,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": "plan.open",
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.objectives.plan.read.title",
      "pageId": "s1.objectives.plan.read",
      "fieldKey": "title",
      "variantId": "default",
      "text": "Understand approval and time",
      "baselineText": "Understand approval and time",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs",
        "branch": "main",
        "line": 43,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": "plan.read",
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.objectives.plan.read.body",
      "pageId": "s1.objectives.plan.read",
      "fieldKey": "body",
      "variantId": "default",
      "text": "Approve Work Plan commits the chosen jobs. Advance one year executes approved jobs and advances the forest. Check all pending jobs and costs first. Tutorials have no first-year deadline; take observations and decisions at your own pace.",
      "baselineText": "Approve Work Plan commits the chosen jobs. Advance one year executes approved jobs and advances the forest. Check all pending jobs and costs first. Tutorials have no first-year deadline; take observations and decisions at your own pace.",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs",
        "branch": "main",
        "line": 43,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": "plan.read",
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.objectives.review.open.title",
      "pageId": "s1.objectives.review.open",
      "fieldKey": "title",
      "variantId": "default",
      "text": "Open actual annual results",
      "baselineText": "Open actual annual results",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs",
        "branch": "main",
        "line": 44,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": "review.open",
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.objectives.review.open.body",
      "pageId": "s1.objectives.review.open",
      "fieldKey": "body",
      "variantId": "default",
      "text": "After an annual cycle, read WORK DONE for successes/failures, MONEY for costs/income and FOREST for changes. An empty year-zero review is not a completed cycle.",
      "baselineText": "After an annual cycle, read WORK DONE for successes/failures, MONEY for costs/income and FOREST for changes. An empty year-zero review is not a completed cycle.",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs",
        "branch": "main",
        "line": 44,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": "review.open",
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.objectives.review.read.title",
      "pageId": "s1.objectives.review.read",
      "fieldKey": "title",
      "variantId": "default",
      "text": "Read and acknowledge the results",
      "baselineText": "Read and acknowledge the results",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs",
        "branch": "main",
        "line": 45,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": "review.read",
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.objectives.review.read.body",
      "pageId": "s1.objectives.review.read",
      "fieldKey": "body",
      "variantId": "default",
      "text": "Use I've read the annual results after reading the three sections. The first acknowledgement unlocks further year advances. Keep using the results to choose what to inspect next; you need not finish every learning topic to continue.",
      "baselineText": "Use I've read the annual results after reading the three sections. The first acknowledgement unlocks further year advances. Keep using the results to choose what to inspect next; you need not finish every learning topic to continue.",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs",
        "branch": "main",
        "line": 45,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": "review.read",
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.objectives.topic5.title",
      "pageId": "s1.objectives.topic5",
      "fieldKey": "title",
      "variantId": "default",
      "text": "5. Stand Map",
      "baselineText": "5. Stand Map",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs",
        "branch": "main",
        "line": 46,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": null,
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.objectives.topic5.intro",
      "pageId": "s1.objectives.topic5",
      "fieldKey": "intro",
      "variantId": "default",
      "text": "Start with opening the map and reading one square. Practise navigation next; explore the other layers when you need them. The map finds places to investigate. It cannot make forestry decisions or carry out work remotely.",
      "baselineText": "Start with opening the map and reading one square. Practise navigation next; explore the other layers when you need them. The map finds places to investigate. It cannot make forestry decisions or carry out work remotely.",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs",
        "branch": "main",
        "line": 46,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": null,
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.objectives.map.open.title",
      "pageId": "s1.objectives.map.open",
      "fieldKey": "title",
      "variantId": "default",
      "text": "Open the map",
      "baselineText": "Open the map",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs",
        "branch": "main",
        "line": 47,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": "map.open",
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.objectives.map.open.body",
      "pageId": "s1.objectives.map.open",
      "fieldKey": "body",
      "variantId": "default",
      "text": "Press M, or choose Stand map on the HUD. North is up. The white square border shows your position; letter/number labels identify locations.",
      "baselineText": "Press M, or choose Stand map on the HUD. North is up. The white square border shows your position; letter/number labels identify locations.",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs",
        "branch": "main",
        "line": 47,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": "map.open",
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.objectives.map.select.title",
      "pageId": "s1.objectives.map.select",
      "fieldKey": "title",
      "variantId": "default",
      "text": "Select a square and read its information",
      "baselineText": "Select a square and read its information",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs",
        "branch": "main",
        "line": 48,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": "map.select",
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.objectives.map.select.body",
      "pageId": "s1.objectives.map.select",
      "fieldKey": "body",
      "variantId": "default",
      "text": "Click a square. The side panel shows its light, canopy and young growth. Read that diagnosis before choosing a destination.",
      "baselineText": "Click a square. The side panel shows its light, canopy and young growth. Read that diagnosis before choosing a destination.",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs",
        "branch": "main",
        "line": 48,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": "map.select",
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.objectives.map.light.title",
      "pageId": "s1.objectives.map.light",
      "fieldKey": "title",
      "variantId": "default",
      "text": "Read the Light layer",
      "baselineText": "Read the Light layer",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs",
        "branch": "main",
        "line": 49,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": "map.light",
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.objectives.map.light.body",
      "pageId": "s1.objectives.map.light",
      "fieldKey": "body",
      "variantId": "default",
      "text": "Choose Light. Values run from 0 (dark) to 1 (open), measured at ground level. Brighter squares have more ground light. This does not measure a particular tree's crown light.",
      "baselineText": "Choose Light. Values run from 0 (dark) to 1 (open), measured at ground level. Brighter squares have more ground light. This does not measure a particular tree's crown light.",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs",
        "branch": "main",
        "line": 49,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": "map.light",
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.objectives.map.waypoint.title",
      "pageId": "s1.objectives.map.waypoint",
      "fieldKey": "title",
      "variantId": "default",
      "text": "Set a waypoint",
      "baselineText": "Set a waypoint",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs",
        "branch": "main",
        "line": 50,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": "map.waypoint",
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.objectives.map.waypoint.body",
      "pageId": "s1.objectives.map.waypoint",
      "fieldKey": "body",
      "variantId": "default",
      "text": "Choose a square away from where you are standing, then Set waypoint. This adds a destination marker; it does not move you.",
      "baselineText": "Choose a square away from where you are standing, then Set waypoint. This adds a destination marker; it does not move you.",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs",
        "branch": "main",
        "line": 50,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": "map.waypoint",
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.objectives.map.return.title",
      "pageId": "s1.objectives.map.return",
      "fieldKey": "title",
      "variantId": "default",
      "text": "Return to the forest",
      "baselineText": "Return to the forest",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs",
        "branch": "main",
        "line": 51,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": "map.return",
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.objectives.map.return.body",
      "pageId": "s1.objectives.map.return",
      "fieldKey": "body",
      "variantId": "default",
      "text": "Press M or Esc, or use Back to forest. The HUD now gives the destination's compass direction and distance.",
      "baselineText": "Press M or Esc, or use Back to forest. The HUD now gives the destination's compass direction and distance.",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs",
        "branch": "main",
        "line": 51,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": "map.return",
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.objectives.map.arrive.title",
      "pageId": "s1.objectives.map.arrive",
      "fieldKey": "title",
      "variantId": "default",
      "text": "Walk to the destination",
      "baselineText": "Walk to the destination",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs",
        "branch": "main",
        "line": 52,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": "map.arrive",
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.objectives.map.arrive.body",
      "pageId": "s1.objectives.map.arrive",
      "fieldKey": "body",
      "variantId": "default",
      "text": "Follow the HUD direction and watch the distance decrease until it says you are here. Walk from outside the destination square; setting a waypoint on your own square is not this practice journey.",
      "baselineText": "Follow the HUD direction and watch the distance decrease until it says you are here. Walk from outside the destination square; setting a waypoint on your own square is not this practice journey.",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs",
        "branch": "main",
        "line": 52,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": "map.arrive",
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.objectives.map.inspectsite.title",
      "pageId": "s1.objectives.map.inspectsite",
      "fieldKey": "title",
      "variantId": "default",
      "text": "Inspect the site on foot",
      "baselineText": "Inspect the site on foot",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs",
        "branch": "main",
        "line": 53,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": "map.inspectsite",
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.objectives.map.inspectsite.body",
      "pageId": "s1.objectives.map.inspectsite",
      "fieldKey": "body",
      "variantId": "default",
      "text": "At the destination, look at the ground report or inspect a nearby tree with E. Compare what you see with the square diagnosis before choosing any management work.",
      "baselineText": "At the destination, look at the ground report or inspect a nearby tree with E. Compare what you see with the square diagnosis before choosing any management work.",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs",
        "branch": "main",
        "line": 53,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": "map.inspectsite",
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.objectives.map.regeneration.title",
      "pageId": "s1.objectives.map.regeneration",
      "fieldKey": "title",
      "variantId": "default",
      "text": "Explore Regeneration",
      "baselineText": "Explore Regeneration",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs",
        "branch": "main",
        "line": 54,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": "map.regeneration",
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.objectives.map.regeneration.body",
      "pageId": "s1.objectives.map.regeneration",
      "fieldKey": "body",
      "variantId": "default",
      "text": "Choose Regeneration. P counts living planted juveniles; the height is the tallest natural seedling cohort. Read the side panel for species and conditions. More light or an opening can create an opportunity for natural regeneration, but it needs seed and suitable conditions; success is not guaranteed. An empty square is information, not an instruction to plant.",
      "baselineText": "Choose Regeneration. P counts living planted juveniles; the height is the tallest natural seedling cohort. Read the side panel for species and conditions. More light or an opening can create an opportunity for natural regeneration, but it needs seed and suitable conditions; success is not guaranteed. An empty square is information, not an instruction to plant.",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs",
        "branch": "main",
        "line": 54,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": "map.regeneration",
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.objectives.map.browse.title",
      "pageId": "s1.objectives.map.browse",
      "fieldKey": "title",
      "variantId": "default",
      "text": "Explore Browsing / protection",
      "baselineText": "Explore Browsing / protection",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs",
        "branch": "main",
        "line": 55,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": "map.browse",
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.objectives.map.browse.body",
      "pageId": "s1.objectives.map.browse",
      "fieldKey": "body",
      "variantId": "default",
      "text": "Choose Browsing / protection. The map shows site browsing pressure and shelters expected to be effective next year. S counts shelters; a dash means none. Visit young growth and compare its condition with the ground report.",
      "baselineText": "Choose Browsing / protection. The map shows site browsing pressure and shelters expected to be effective next year. S counts shelters; a dash means none. Visit young growth and compare its condition with the ground report.",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs",
        "branch": "main",
        "line": 55,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": "map.browse",
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.objectives.map.marks.title",
      "pageId": "s1.objectives.map.marks",
      "fieldKey": "title",
      "variantId": "default",
      "text": "Explore Fell & crop marks",
      "baselineText": "Explore Fell & crop marks",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs",
        "branch": "main",
        "line": 56,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": "map.marks",
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.objectives.map.marks.body",
      "pageId": "s1.objectives.map.marks",
      "fieldKey": "body",
      "variantId": "default",
      "text": "Choose Fell & crop marks. Squares count Fell proposals; diamonds count Crop Trees. Red means proposed removal, blue means retain/favour. This is a view of decisions made in the forest, not a remote marking tool.",
      "baselineText": "Choose Fell & crop marks. Squares count Fell proposals; diamonds count Crop Trees. Red means proposed removal, blue means retain/favour. This is a view of decisions made in the forest, not a remote marking tool.",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs",
        "branch": "main",
        "line": 56,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": "map.marks",
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.objectives.rack.map.title",
      "pageId": "s1.objectives.rack.map",
      "fieldKey": "title",
      "variantId": "default",
      "text": "Plan a rack on the Stand Map",
      "baselineText": "Plan a rack on the Stand Map",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs",
        "branch": "main",
        "line": 57,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": "rack.map",
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.objectives.rack.map.body.legacy-or-fallback",
      "pageId": "s1.objectives.rack.map",
      "fieldKey": "body",
      "variantId": "legacy-or-fallback",
      "text": "Open the Stand Map [M], choose Plan rack, pick a square on the row you want and add the rack. The map uses the same rack tool as the forest. This practice is optional.",
      "baselineText": "Open the Stand Map [M], choose Plan rack, pick a square on the row you want and add the rack. The map uses the same rack tool as the forest. This practice is optional.",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs",
        "branch": "main",
        "line": 57,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": "rack.map",
      "format": "plain-text",
      "tokens": [],
      "note": "Legacy/general base wording; Rack V2 may replace at runtime"
    },
    {
      "id": "s1.objectives.map.compare.title",
      "pageId": "s1.objectives.map.compare",
      "fieldKey": "title",
      "variantId": "default",
      "text": "Understand the limits of a map",
      "baselineText": "Understand the limits of a map",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs",
        "branch": "main",
        "line": 58,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": "map.compare",
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.objectives.map.compare.body",
      "pageId": "s1.objectives.map.compare",
      "fieldKey": "body",
      "variantId": "default",
      "text": "A square summarises an area. Switch layers to compare light, young growth, protection and your marks, then walk there. A colour or a low number alone does not tell you which tree to fell.",
      "baselineText": "A square summarises an area. Switch layers to compare light, young growth, protection and your marks, then walk there. A colour or a low number alone does not tell you which tree to fell.",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs",
        "branch": "main",
        "line": 58,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": "map.compare",
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.objectives.topic6.title",
      "pageId": "s1.objectives.topic6",
      "fieldKey": "title",
      "variantId": "default",
      "text": "6. Pruning",
      "baselineText": "6. Pruning",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs",
        "branch": "main",
        "line": 59,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": null,
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.objectives.topic6.intro",
      "pageId": "s1.objectives.topic6",
      "fieldKey": "intro",
      "variantId": "default",
      "text": "Pruning removes lower branches from retained Crop Trees for future timber quality and a clearer stem. Pruning sets no price by itself: where Oak timber is marketed, the recorded pruned section can make a large log eligible for a higher grade, while Sitka and Beech have no such premium here, and fork or defect shape is not modelled. It costs contractor time and money. A tree may need to grow or recover before it is eligible.",
      "baselineText": "Pruning removes lower branches from retained Crop Trees for future timber quality and a clearer stem. Pruning sets no price by itself: where Oak timber is marketed, the recorded pruned section can make a large log eligible for a higher grade, while Sitka and Beech have no such premium here, and fork or defect shape is not modelled. It costs contractor time and money. A tree may need to grow or recover before it is eligible.",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs",
        "branch": "main",
        "line": 59,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": null,
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.objectives.prune.read.title",
      "pageId": "s1.objectives.prune.read",
      "fieldKey": "title",
      "variantId": "default",
      "text": "Understand eligibility",
      "baselineText": "Understand eligibility",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs",
        "branch": "main",
        "line": 60,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": "prune.read",
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.objectives.prune.read.body",
      "pageId": "s1.objectives.prune.read",
      "fieldKey": "body",
      "variantId": "default",
      "text": "The Work Plan reports how many Crop Trees are eligible. Sitka uses lifts to 2.5 m, 5 m and 6.5 m (at most three lifts, at least five years apart). When Oak and Beech timber markets are active, Oak and Beech use two lifts, to 3.5 m then 6.0 m, at least three years apart; otherwise they keep the three-lift schedule above. All are limited to a crown base below 60% of tree height. These are scenario rules, not a recommendation to prune every tree.",
      "baselineText": "The Work Plan reports how many Crop Trees are eligible. Sitka uses lifts to 2.5 m, 5 m and 6.5 m (at most three lifts, at least five years apart). When Oak and Beech timber markets are active, Oak and Beech use two lifts, to 3.5 m then 6.0 m, at least three years apart; otherwise they keep the three-lift schedule above. All are limited to a crown base below 60% of tree height. These are scenario rules, not a recommendation to prune every tree.",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs",
        "branch": "main",
        "line": 60,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": "prune.read",
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.objectives.prune.plan.title",
      "pageId": "s1.objectives.prune.plan",
      "fieldKey": "title",
      "variantId": "default",
      "text": "Plan an eligible pruning lift",
      "baselineText": "Plan an eligible pruning lift",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs",
        "branch": "main",
        "line": 61,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": "prune.plan",
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.objectives.prune.plan.body",
      "pageId": "s1.objectives.prune.plan",
      "fieldKey": "body",
      "variantId": "default",
      "text": "In Work Plan, find Pruning and Add eligible crop tree pruning tasks. This can add several eligible trees; review each target height and cost, and remove jobs you do not want. If none are eligible, return in a later year.",
      "baselineText": "In Work Plan, find Pruning and Add eligible crop tree pruning tasks. This can add several eligible trees; review each target height and cost, and remove jobs you do not want. If none are eligible, return in a later year.",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs",
        "branch": "main",
        "line": 61,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": "prune.plan",
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.objectives.prune.approve.title",
      "pageId": "s1.objectives.prune.approve",
      "fieldKey": "title",
      "variantId": "default",
      "text": "Approve the selected pruning work",
      "baselineText": "Approve the selected pruning work",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs",
        "branch": "main",
        "line": 62,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": "prune.approve",
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.objectives.prune.approve.body",
      "pageId": "s1.objectives.prune.approve",
      "fieldKey": "body",
      "variantId": "default",
      "text": "Check contractor cost and available cash, then approve pending work. A designated Crop Tree stays standing; the lift is executed during the annual work cycle.",
      "baselineText": "Check contractor cost and available cash, then approve pending work. A designated Crop Tree stays standing; the lift is executed during the annual work cycle.",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs",
        "branch": "main",
        "line": 62,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": "prune.approve",
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.objectives.prune.result.title",
      "pageId": "s1.objectives.prune.result",
      "fieldKey": "title",
      "variantId": "default",
      "text": "Read the pruning result",
      "baselineText": "Read the pruning result",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs",
        "branch": "main",
        "line": 63,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": "prune.result",
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.objectives.prune.result.body",
      "pageId": "s1.objectives.prune.result",
      "fieldKey": "body",
      "variantId": "default",
      "text": "After a successful lift, read Annual Review. Return to that tree and inspect its Pruning field for the lift count and clear-stem height. Do not expect another immediate lift.",
      "baselineText": "After a successful lift, read Annual Review. Return to that tree and inspect its Pruning field for the lift count and clear-stem height. Do not expect another immediate lift.",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs",
        "branch": "main",
        "line": 63,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": "prune.result",
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.objectives.topic7.title",
      "pageId": "s1.objectives.topic7",
      "fieldKey": "title",
      "variantId": "default",
      "text": "7. Fallen deadwood",
      "baselineText": "7. Fallen deadwood",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs",
        "branch": "main",
        "line": 64,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": null,
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.objectives.topic7.intro",
      "pageId": "s1.objectives.topic7",
      "fieldKey": "intro",
      "variantId": "default",
      "text": "Deadwood is a material outcome of felling. Keeping a fallen stem on site differs from selling it or keeping timber for construction.",
      "baselineText": "Deadwood is a material outcome of felling. Keeping a fallen stem on site differs from selling it or keeping timber for construction.",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs",
        "branch": "main",
        "line": 64,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": null,
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.objectives.deadwood.read.title",
      "pageId": "s1.objectives.deadwood.read",
      "fieldKey": "title",
      "variantId": "default",
      "text": "Understand the material choices",
      "baselineText": "Understand the material choices",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs",
        "branch": "main",
        "line": 65,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": "deadwood.read",
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.objectives.deadwood.read.body",
      "pageId": "s1.objectives.deadwood.read",
      "fieldKey": "body",
      "variantId": "default",
      "text": "Sell extracts timber for configured sale income. Keep for use retains usable construction timber. Leave as deadwood retains the fallen stem on site for the scenario's habitat record; it earns no timber sale income. Contractor work can still cost money.",
      "baselineText": "Sell extracts timber for configured sale income. Keep for use retains usable construction timber. Leave as deadwood retains the fallen stem on site for the scenario's habitat record; it earns no timber sale income. Contractor work can still cost money.",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs",
        "branch": "main",
        "line": 65,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": "deadwood.read",
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.objectives.deadwood.plan.title",
      "pageId": "s1.objectives.deadwood.plan",
      "fieldKey": "title",
      "variantId": "default",
      "text": "Choose Leave as deadwood",
      "baselineText": "Choose Leave as deadwood",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs",
        "branch": "main",
        "line": 66,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": "deadwood.plan",
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.objectives.deadwood.plan.body",
      "pageId": "s1.objectives.deadwood.plan",
      "fieldKey": "body",
      "variantId": "default",
      "text": "On a pending felling job, choose Leave as deadwood. Compare the new quote and cash balance before approving. This changes that job's outcome; it does not create a log until work succeeds.",
      "baselineText": "On a pending felling job, choose Leave as deadwood. Compare the new quote and cash balance before approving. This changes that job's outcome; it does not create a log until work succeeds.",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs",
        "branch": "main",
        "line": 66,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": "deadwood.plan",
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.objectives.deadwood.result.title",
      "pageId": "s1.objectives.deadwood.result",
      "fieldKey": "title",
      "variantId": "default",
      "text": "Review retained deadwood",
      "baselineText": "Review retained deadwood",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs",
        "branch": "main",
        "line": 67,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": "deadwood.result",
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.objectives.deadwood.result.body",
      "pageId": "s1.objectives.deadwood.result",
      "fieldKey": "body",
      "variantId": "default",
      "text": "Resolve the approved job in a later annual cycle and read FOREST in Annual Review. It reports newly retained volume and fallen logs on site. The scenario records decay over time; deadwood is not stored construction timber.",
      "baselineText": "Resolve the approved job in a later annual cycle and read FOREST in Annual Review. It reports newly retained volume and fallen logs on site. The scenario records decay over time; deadwood is not stored construction timber.",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs",
        "branch": "main",
        "line": 67,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": "deadwood.result",
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.objectives.deadwood.visit.title",
      "pageId": "s1.objectives.deadwood.visit",
      "fieldKey": "title",
      "variantId": "default",
      "text": "Visit a retained fallen stem",
      "baselineText": "Visit a retained fallen stem",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs",
        "branch": "main",
        "line": 68,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": "deadwood.visit",
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.objectives.deadwood.visit.body",
      "pageId": "s1.objectives.deadwood.visit",
      "fieldKey": "body",
      "variantId": "default",
      "text": "Walk back to within a few metres of a retained log and look at the site. Compare the standing-tree removal and the fallen stem with the recorded outcome. This lesson can wait until you choose a suitable habitat treatment.",
      "baselineText": "Walk back to within a few metres of a retained log and look at the site. Compare the standing-tree removal and the fallen stem with the recorded outcome. This lesson can wait until you choose a suitable habitat treatment.",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs",
        "branch": "main",
        "line": 68,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": "deadwood.visit",
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.objectives.topic8.title",
      "pageId": "s1.objectives.topic8",
      "fieldKey": "title",
      "variantId": "default",
      "text": "8. Planting and protection",
      "baselineText": "8. Planting and protection",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs",
        "branch": "main",
        "line": 69,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": null,
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.objectives.topic8.intro",
      "pageId": "s1.objectives.topic8",
      "fieldKey": "intro",
      "variantId": "default",
      "text": "Use ground observations to choose a site. Planting is not automatically an improvement; natural regeneration is not required for every useful management outcome. Planning a spot, paying for nursery stock and approving work are separate steps.",
      "baselineText": "Use ground observations to choose a site. Planting is not automatically an improvement; natural regeneration is not required for every useful management outcome. Planning a spot, paying for nursery stock and approving work are separate steps.",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs",
        "branch": "main",
        "line": 69,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": null,
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.objectives.plant.ground.title",
      "pageId": "s1.objectives.plant.ground",
      "fieldKey": "title",
      "variantId": "default",
      "text": "Read the ground report",
      "baselineText": "Read the ground report",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs",
        "branch": "main",
        "line": 70,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": "plant.ground",
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.objectives.plant.ground.body",
      "pageId": "s1.objectives.plant.ground",
      "fieldKey": "body",
      "variantId": "default",
      "text": "Look at the ground to see light, regeneration and browsing conditions. Use the map to locate another area if useful. Check existing growth before deciding whether planting is appropriate.",
      "baselineText": "Look at the ground to see light, regeneration and browsing conditions. Use the map to locate another area if useful. Check existing growth before deciding whether planting is appropriate.",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs",
        "branch": "main",
        "line": 70,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": "plant.ground",
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.objectives.plant.stock.title",
      "pageId": "s1.objectives.plant.stock",
      "fieldKey": "title",
      "variantId": "default",
      "text": "Buy nursery stock",
      "baselineText": "Buy nursery stock",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs",
        "branch": "main",
        "line": 71,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": "plant.stock",
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.objectives.plant.stock.body",
      "pageId": "s1.objectives.plant.stock",
      "fieldKey": "body",
      "variantId": "default",
      "text": "In Work Plan, open Nursery, choose a quantity and buy the species you intend to plant. Read the total purchase cost and cash warning before buying. Buying stock does not plant it. Contractor planting and shelter materials cost extra; using your own time for unsheltered planting has no external labour charge. Stock is consumed when successful planting work uses it.",
      "baselineText": "In Work Plan, open Nursery, choose a quantity and buy the species you intend to plant. Read the total purchase cost and cash warning before buying. Buying stock does not plant it. Contractor planting and shelter materials cost extra; using your own time for unsheltered planting has no external labour charge. Stock is consumed when successful planting work uses it.",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs",
        "branch": "main",
        "line": 71,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": "plant.stock",
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.objectives.plant.plan.title",
      "pageId": "s1.objectives.plant.plan",
      "fieldKey": "title",
      "variantId": "default",
      "text": "Mark a planting spot",
      "baselineText": "Mark a planting spot",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs",
        "branch": "main",
        "line": 72,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": "plant.plan",
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.objectives.plant.plan.body",
      "pageId": "s1.objectives.plant.plan",
      "fieldKey": "body",
      "variantId": "default",
      "text": "Back in the forest, press G for planting mode, select owned stock and click the intended ground spot. This creates a planned marker, not an immediately established tree. Esc leaves planting mode.",
      "baselineText": "Back in the forest, press G for planting mode, select owned stock and click the intended ground spot. This creates a planned marker, not an immediately established tree. Esc leaves planting mode.",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs",
        "branch": "main",
        "line": 72,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": "plant.plan",
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.objectives.plant.execution.title",
      "pageId": "s1.objectives.plant.execution",
      "fieldKey": "title",
      "variantId": "default",
      "text": "Read executor and shelter choices",
      "baselineText": "Read executor and shelter choices",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs",
        "branch": "main",
        "line": 73,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": "plant.execution",
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.objectives.plant.execution.body",
      "pageId": "s1.objectives.plant.execution",
      "fieldKey": "body",
      "variantId": "default",
      "text": "Work Plan lets planting use contractor labour or your limited annual time. Shelters add material cost and use the same executor as planting. Check browsing and your budget; changed approved work needs approval again. Review each order before advancing.",
      "baselineText": "Work Plan lets planting use contractor labour or your limited annual time. Shelters add material cost and use the same executor as planting. Check browsing and your budget; changed approved work needs approval again. Review each order before advancing.",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs",
        "branch": "main",
        "line": 73,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": "plant.execution",
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.objectives.plant.shelter.title",
      "pageId": "s1.objectives.plant.shelter",
      "fieldKey": "title",
      "variantId": "default",
      "text": "Plan a shelter when appropriate",
      "baselineText": "Plan a shelter when appropriate",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs",
        "branch": "main",
        "line": 74,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": "plant.shelter",
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.objectives.plant.shelter.body",
      "pageId": "s1.objectives.plant.shelter",
      "fieldKey": "body",
      "variantId": "default",
      "text": "Set Shelters ON in Work Plan and inspect the planting order's with shelter label. Do this only when you choose protection for the site; this optional practice can wait. The browsing layer later shows whether shelters remain effective.",
      "baselineText": "Set Shelters ON in Work Plan and inspect the planting order's with shelter label. Do this only when you choose protection for the site; this optional practice can wait. The browsing layer later shows whether shelters remain effective.",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs",
        "branch": "main",
        "line": 74,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": "plant.shelter",
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.objectives.plant.approve.title",
      "pageId": "s1.objectives.plant.approve",
      "fieldKey": "title",
      "variantId": "default",
      "text": "Approve planting work",
      "baselineText": "Approve planting work",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs",
        "branch": "main",
        "line": 75,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": "plant.approve",
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.objectives.plant.approve.body",
      "pageId": "s1.objectives.plant.approve",
      "fieldKey": "body",
      "variantId": "default",
      "text": "Check stock availability, labour/time, shelters and available cash. Approve the jobs you intend to carry out. Approval reserves a plan; establishment is recorded after the annual cycle succeeds.",
      "baselineText": "Check stock availability, labour/time, shelters and available cash. Approve the jobs you intend to carry out. Approval reserves a plan; establishment is recorded after the annual cycle succeeds.",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs",
        "branch": "main",
        "line": 75,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": "plant.approve",
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.objectives.plant.result.title",
      "pageId": "s1.objectives.plant.result",
      "fieldKey": "title",
      "variantId": "default",
      "text": "Read planting results",
      "baselineText": "Read planting results",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs",
        "branch": "main",
        "line": 76,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": "plant.result",
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.objectives.plant.result.body",
      "pageId": "s1.objectives.plant.result",
      "fieldKey": "body",
      "variantId": "default",
      "text": "Read Annual Review after successful planting, then revisit the marked site. Later years show growth and browsing response. A successful job does not guarantee that a juvenile survives indefinitely.",
      "baselineText": "Read Annual Review after successful planting, then revisit the marked site. Later years show growth and browsing response. A successful job does not guarantee that a juvenile survives indefinitely.",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs",
        "branch": "main",
        "line": 76,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": "plant.result",
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.objectives.topic9.title",
      "pageId": "s1.objectives.topic9",
      "fieldKey": "title",
      "variantId": "default",
      "text": "9. Vegetation clearance",
      "baselineText": "9. Vegetation clearance",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs",
        "branch": "main",
        "line": 77,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": null,
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.objectives.topic9.intro",
      "pageId": "s1.objectives.topic9",
      "fieldKey": "intro",
      "variantId": "default",
      "text": "Clearance removes competing vegetation inside the previewed square footprint. It is a planned contractor operation; standing trees remain.",
      "baselineText": "Clearance removes competing vegetation inside the previewed square footprint. It is a planned contractor operation; standing trees remain.",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs",
        "branch": "main",
        "line": 77,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": null,
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.objectives.clear.read.title",
      "pageId": "s1.objectives.clear.read",
      "fieldKey": "title",
      "variantId": "default",
      "text": "Read the footprint before planning",
      "baselineText": "Read the footprint before planning",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs",
        "branch": "main",
        "line": 78,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": "clear.read",
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.objectives.clear.read.body",
      "pageId": "s1.objectives.clear.read",
      "fieldKey": "body",
      "variantId": "default",
      "text": "Aim at ground with competing vegetation and use U to plan area clearance. Read the preview for the actual targets and area. Existing young growth inside the footprint may be removed, so inspect it before approving.",
      "baselineText": "Aim at ground with competing vegetation and use U to plan area clearance. Read the preview for the actual targets and area. Existing young growth inside the footprint may be removed, so inspect it before approving.",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs",
        "branch": "main",
        "line": 78,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": "clear.read",
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.objectives.clear.plan.title",
      "pageId": "s1.objectives.clear.plan",
      "fieldKey": "title",
      "variantId": "default",
      "text": "Plan a clearance area",
      "baselineText": "Plan a clearance area",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs",
        "branch": "main",
        "line": 79,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": "clear.plan",
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.objectives.clear.plan.body",
      "pageId": "s1.objectives.clear.plan",
      "fieldKey": "body",
      "variantId": "default",
      "text": "Use the forest preview to designate a square, then find Vegetation clearance in Work Plan. Check the square label, targeted vegetation, contractor cost and any validation problem. Do not clear a square just because the map is dark.",
      "baselineText": "Use the forest preview to designate a square, then find Vegetation clearance in Work Plan. Check the square label, targeted vegetation, contractor cost and any validation problem. Do not clear a square just because the map is dark.",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs",
        "branch": "main",
        "line": 79,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": "clear.plan",
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.objectives.clear.approve.title",
      "pageId": "s1.objectives.clear.approve",
      "fieldKey": "title",
      "variantId": "default",
      "text": "Approve clearance when ready",
      "baselineText": "Approve clearance when ready",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs",
        "branch": "main",
        "line": 80,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": "clear.approve",
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.objectives.clear.approve.body",
      "pageId": "s1.objectives.clear.approve",
      "fieldKey": "body",
      "variantId": "default",
      "text": "Approve the chosen work after checking what will be removed. Pending jobs can be removed if you change your mind. The plan is resolved at the annual cycle, not at designation.",
      "baselineText": "Approve the chosen work after checking what will be removed. Pending jobs can be removed if you change your mind. The plan is resolved at the annual cycle, not at designation.",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs",
        "branch": "main",
        "line": 80,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": "clear.approve",
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.objectives.clear.result.title",
      "pageId": "s1.objectives.clear.result",
      "fieldKey": "title",
      "variantId": "default",
      "text": "Read clearance results",
      "baselineText": "Read clearance results",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs",
        "branch": "main",
        "line": 81,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": "clear.result",
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.objectives.clear.result.body",
      "pageId": "s1.objectives.clear.result",
      "fieldKey": "body",
      "variantId": "default",
      "text": "Read the successful work result. Then revisit the footprint on foot to compare removed competing growth with the planned area. Standing trees are not part of this operation, and the site can develop new vegetation later.",
      "baselineText": "Read the successful work result. Then revisit the footprint on foot to compare removed competing growth with the planned area. Standing trees are not part of this operation, and the site can develop new vegetation later.",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs",
        "branch": "main",
        "line": 81,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": "clear.result",
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.objectives.rack.read.body.enlarged80-rack-v2",
      "pageId": "s1.objectives.rack.read",
      "fieldKey": "body",
      "variantId": "enlarged80-rack-v2",
      "text": "A rack is a straight corridor of removed trees that machinery drives along, so the rest of the stand can be reached without driving through it. It stays open for this and later thinnings. Spacing is conditional: around 16–20m between racks may be common in early transformation, around 30 m or more may be viable in developed CCF, and there is no universal rule. Scenario One recommends 16–20m and five racks for this plantation only.",
      "baselineText": "A rack is a straight corridor of removed trees that machinery drives along, so the rest of the stand can be reached without driving through it. It stays open for this and later thinnings. Spacing is conditional: around 16–20m between racks may be common in early transformation, around 30 m or more may be viable in developed CCF, and there is no universal rule. Scenario One recommends 16–20m and five racks for this plantation only.",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs",
        "branch": "main",
        "line": 113,
        "kind": "conditional"
      },
      "status": "main-source",
      "protectedGameplayKey": "rack.read",
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.objectives.rack.create.body.enlarged80-rack-v2",
      "pageId": "s1.objectives.rack.create",
      "fieldKey": "body",
      "variantId": "enlarged80-rack-v2",
      "text": "",
      "baselineText": "",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs",
        "branch": "main",
        "line": 116,
        "kind": "conditional"
      },
      "status": "main-source",
      "protectedGameplayKey": "rack.create",
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.objectives.rack.layout.body.enlarged80-rack-v2",
      "pageId": "s1.objectives.rack.layout",
      "fieldKey": "body",
      "variantId": "enlarged80-rack-v2",
      "text": "Plan five racks on the Stand Map [M] and choose Confirm Rack Plan. Then return to the forest and mark each rack entrance tree in orange: [P] spray marker, aim within 3.5 m, left-click. Tree Marks help explains the paint.",
      "baselineText": "Plan five racks on the Stand Map [M] and choose Confirm Rack Plan. Then return to the forest and mark each rack entrance tree in orange: [P] spray marker, aim within 3.5 m, left-click. Tree Marks help explains the paint.",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs",
        "branch": "main",
        "line": 117,
        "kind": "conditional"
      },
      "status": "main-source",
      "protectedGameplayKey": "rack.layout",
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.objectives.rack.approve.body.enlarged80-rack-v2",
      "pageId": "s1.objectives.rack.approve",
      "fieldKey": "body",
      "variantId": "enlarged80-rack-v2",
      "text": "Once the rack entrances are marked, open the Work Plan [Tab]. Check the rack removal work and its cost, then choose Approve Work Plan. Approval does not cut anything yet.",
      "baselineText": "Once the rack entrances are marked, open the Work Plan [Tab]. Check the rack removal work and its cost, then choose Approve Work Plan. Approval does not cut anything yet.",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs",
        "branch": "main",
        "line": 118,
        "kind": "conditional"
      },
      "status": "main-source",
      "protectedGameplayKey": "rack.approve",
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.objectives.rack.map.body.enlarged80-rack-v2",
      "pageId": "s1.objectives.rack.map",
      "fieldKey": "body",
      "variantId": "enlarged80-rack-v2",
      "text": "",
      "baselineText": "",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs",
        "branch": "main",
        "line": 119,
        "kind": "conditional"
      },
      "status": "main-source",
      "protectedGameplayKey": "rack.map",
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.teaching.access.title",
      "pageId": "s1.teaching.access",
      "fieldKey": "title",
      "variantId": "default",
      "text": "1. Create access racks",
      "baselineText": "1. Create access racks",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/ScenarioOneTeachingSequence.cs",
        "branch": "main",
        "line": 35,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": "access",
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.teaching.access.concept",
      "pageId": "s1.teaching.access",
      "fieldKey": "concept",
      "variantId": "default",
      "text": "A rack is a permanent access corridor. The harvesting machine drives along it, so the rows between racks can be thinned later without driving through standing trees. Planning access first is what makes careful, selective work possible: every later decision depends on it.",
      "baselineText": "A rack is a permanent access corridor. The harvesting machine drives along it, so the rows between racks can be thinned later without driving through standing trees. Planning access first is what makes careful, selective work possible: every later decision depends on it.",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/ScenarioOneTeachingSequence.cs",
        "branch": "main",
        "line": 35,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": "access",
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.teaching.access.action",
      "pageId": "s1.teaching.access",
      "fieldKey": "action",
      "variantId": "default",
      "text": "Press B, click the tree or ground point where a rack should start, aim along the rows and click again to add it to the Work Plan. Finish the marked routes on the Stand Map [M], then approve the work in the Work Plan [Tab] and advance one year.",
      "baselineText": "Press B, click the tree or ground point where a rack should start, aim along the rows and click again to add it to the Work Plan. Finish the marked routes on the Stand Map [M], then approve the work in the Work Plan [Tab] and advance one year.",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/ScenarioOneTeachingSequence.cs",
        "branch": "main",
        "line": 35,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": "access",
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.teaching.access.caption",
      "pageId": "s1.teaching.access",
      "fieldKey": "caption",
      "variantId": "default",
      "text": "Racks provide access while the rows between them remain standing.",
      "baselineText": "Racks provide access while the rows between them remain standing.",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/ScenarioOneTeachingSequence.cs",
        "branch": "main",
        "line": 35,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": "access",
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.teaching.access.hudAction",
      "pageId": "s1.teaching.access",
      "fieldKey": "hudAction",
      "variantId": "default",
      "text": "Press B, pick where a rack starts, aim forward and click to add it.",
      "baselineText": "Press B, pick where a rack starts, aim forward and click to add it.",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/ScenarioOneTeachingSequence.cs",
        "branch": "main",
        "line": 35,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": "access",
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.teaching.access.practice",
      "pageId": "s1.teaching.access",
      "fieldKey": "practice",
      "variantId": "default",
      "text": "Racks run straight and stay open for good, so plan the whole layout deliberately before you approve it. Take only the trees on each rack's line and leave the rows between racks standing: that standing forest is what you are keeping. Check the cost in the Work Plan before approving.",
      "baselineText": "Racks run straight and stay open for good, so plan the whole layout deliberately before you approve it. Take only the trees on each rack's line and leave the rows between racks standing: that standing forest is what you are keeping. Check the cost in the Work Plan before approving.",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/ScenarioOneTeachingSequence.cs",
        "branch": "main",
        "line": 35,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": "access",
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.teaching.future-trees.title",
      "pageId": "s1.teaching.future-trees",
      "fieldKey": "title",
      "variantId": "default",
      "text": "2. Choose promising trees to retain along the rack",
      "baselineText": "2. Choose promising trees to retain along the rack",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/ScenarioOneTeachingSequence.cs",
        "branch": "main",
        "line": 40,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": "future-trees",
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.teaching.future-trees.concept",
      "pageId": "s1.teaching.future-trees",
      "fieldKey": "concept",
      "variantId": "default",
      "text": "Inspect and compare promising trees to retain and grow on. Work along the whole teaching area beside the middle rack, rather than choosing a cluster in one place. Your choices are the retained trees whose relationships you will manage.",
      "baselineText": "Inspect and compare promising trees to retain and grow on. Work along the whole teaching area beside the middle rack, rather than choosing a cluster in one place. Your choices are the retained trees whose relationships you will manage.",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/ScenarioOneTeachingSequence.cs",
        "branch": "main",
        "line": 40,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": "future-trees",
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.teaching.future-trees.action",
      "pageId": "s1.teaching.future-trees",
      "fieldKey": "action",
      "variantId": "default",
      "text": "Inspect trees [E] and mark three Crop Trees [C]: one in the near, middle and far teaching zones. Keep them at least 8 m apart, with both rack sides represented between your three choices.",
      "baselineText": "Inspect trees [E] and mark three Crop Trees [C]: one in the near, middle and far teaching zones. Keep them at least 8 m apart, with both rack sides represented between your three choices.",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/ScenarioOneTeachingSequence.cs",
        "branch": "main",
        "line": 40,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": "future-trees",
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.teaching.future-trees.caption",
      "pageId": "s1.teaching.future-trees",
      "fieldKey": "caption",
      "variantId": "default",
      "text": "Distribute attention along the rack and compare individuals.",
      "baselineText": "Distribute attention along the rack and compare individuals.",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/ScenarioOneTeachingSequence.cs",
        "branch": "main",
        "line": 40,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": "future-trees",
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.teaching.future-trees.hudAction",
      "pageId": "s1.teaching.future-trees",
      "fieldKey": "hudAction",
      "variantId": "default",
      "text": "Choose one Crop Tree in each zone, 8 m apart, using both rack sides.",
      "baselineText": "Choose one Crop Tree in each zone, 8 m apart, using both rack sides.",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/ScenarioOneTeachingSequence.cs",
        "branch": "main",
        "line": 40,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": "future-trees",
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.teaching.future-trees.practice",
      "pageId": "s1.teaching.future-trees",
      "fieldKey": "practice",
      "variantId": "default",
      "text": "Three trees and 8 m spacing are teaching choices tied to this game's competition neighbourhood, not a forestry prescription. Spacing alone does not distribute your attention or prove crown position.",
      "baselineText": "Three trees and 8 m spacing are teaching choices tied to this game's competition neighbourhood, not a forestry prescription. Spacing alone does not distribute your attention or prove crown position.",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/ScenarioOneTeachingSequence.cs",
        "branch": "main",
        "line": 40,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": "future-trees",
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.teaching.release.title",
      "pageId": "s1.teaching.release",
      "fieldKey": "title",
      "variantId": "default",
      "text": "3. Identify the competitors that matter",
      "baselineText": "3. Identify the competitors that matter",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/ScenarioOneTeachingSequence.cs",
        "branch": "main",
        "line": 45,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": "release",
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.teaching.release.concept",
      "pageId": "s1.teaching.release",
      "fieldKey": "concept",
      "variantId": "default",
      "text": "Compare neighbour relationships and competition Share around each retained tree. Selective removal can help more than one tree, but being small is not a reason to fell. Retain useful structure and exercise restraint.",
      "baselineText": "Compare neighbour relationships and competition Share around each retained tree. Selective removal can help more than one tree, but being small is not a reason to fell. Retain useful structure and exercise restraint.",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/ScenarioOneTeachingSequence.cs",
        "branch": "main",
        "line": 45,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": "release",
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.teaching.release.action",
      "pageId": "s1.teaching.release",
      "fieldKey": "action",
      "variantId": "default",
      "text": "Inspect each Crop Tree [E]. Compare Share and position; select a listed competitor [1–5] and mark it [X]. Plan in-footprint removals supplying about 30% of each tree's measured competition, then review the Work Plan [Tab].",
      "baselineText": "Inspect each Crop Tree [E]. Compare Share and position; select a listed competitor [1–5] and mark it [X]. Plan in-footprint removals supplying about 30% of each tree's measured competition, then review the Work Plan [Tab].",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/ScenarioOneTeachingSequence.cs",
        "branch": "main",
        "line": 45,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": "release",
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.teaching.release.caption",
      "pageId": "s1.teaching.release",
      "fieldKey": "caption",
      "variantId": "default",
      "text": "Consider meaningful competitors and shared relationships; keep retained woodland.",
      "baselineText": "Consider meaningful competitors and shared relationships; keep retained woodland.",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/ScenarioOneTeachingSequence.cs",
        "branch": "main",
        "line": 45,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": "release",
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.teaching.release.hudAction",
      "pageId": "s1.teaching.release",
      "fieldKey": "hudAction",
      "variantId": "default",
      "text": "Compare Share, mark relevant competitors [X], then review the Work Plan [Tab].",
      "baselineText": "Compare Share, mark relevant competitors [X], then review the Work Plan [Tab].",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/ScenarioOneTeachingSequence.cs",
        "branch": "main",
        "line": 45,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": "release",
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.teaching.release.practice",
      "pageId": "s1.teaching.release",
      "fieldKey": "practice",
      "variantId": "default",
      "text": "The initial 30% target and 40% cumulative trunk-area safeguard are gameplay calibration. Neighbours outside the teaching footprint still compete, but their removal earns no lesson credit.",
      "baselineText": "The initial 30% target and 40% cumulative trunk-area safeguard are gameplay calibration. Neighbours outside the teaching footprint still compete, but their removal earns no lesson credit.",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/ScenarioOneTeachingSequence.cs",
        "branch": "main",
        "line": 45,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": "release",
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.teaching.habitat.title",
      "pageId": "s1.teaching.habitat",
      "fieldKey": "title",
      "variantId": "default",
      "text": "4. Plan and carry out the intervention",
      "baselineText": "4. Plan and carry out the intervention",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/ScenarioOneTeachingSequence.cs",
        "branch": "main",
        "line": 50,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": "habitat",
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.teaching.habitat.concept",
      "pageId": "s1.teaching.habitat",
      "fieldKey": "concept",
      "variantId": "default",
      "text": "Work across the teaching area while retaining forest structure. Selling timber brings income; leaving some felled wood forgoes that sale and adds deadwood habitat. Compare these outcomes in their actual positions.",
      "baselineText": "Work across the teaching area while retaining forest structure. Selling timber brings income; leaving some felled wood forgoes that sale and adds deadwood habitat. Compare these outcomes in their actual positions.",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/ScenarioOneTeachingSequence.cs",
        "branch": "main",
        "line": 50,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": "habitat",
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.teaching.habitat.action",
      "pageId": "s1.teaching.habitat",
      "fieldKey": "action",
      "variantId": "default",
      "text": "In Work Plan [Tab], assign both sold timber and fallen deadwood to the selected release trees. Check the retained stand and cumulative safeguard, approve the work, then advance one year.",
      "baselineText": "In Work Plan [Tab], assign both sold timber and fallen deadwood to the selected release trees. Check the retained stand and cumulative safeguard, approve the work, then advance one year.",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/ScenarioOneTeachingSequence.cs",
        "branch": "main",
        "line": 50,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": "habitat",
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.teaching.habitat.caption",
      "pageId": "s1.teaching.habitat",
      "fieldKey": "caption",
      "variantId": "default",
      "text": "Some felled wood is sold and some stays beside the retained forest.",
      "baselineText": "Some felled wood is sold and some stays beside the retained forest.",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/ScenarioOneTeachingSequence.cs",
        "branch": "main",
        "line": 50,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": "habitat",
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.teaching.habitat.hudAction",
      "pageId": "s1.teaching.habitat",
      "fieldKey": "hudAction",
      "variantId": "default",
      "text": "Compare sold and deadwood outcomes, approve the Work Plan [Tab], advance a year.",
      "baselineText": "Compare sold and deadwood outcomes, approve the Work Plan [Tab], advance a year.",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/ScenarioOneTeachingSequence.cs",
        "branch": "main",
        "line": 50,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": "habitat",
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.teaching.habitat.practice",
      "pageId": "s1.teaching.habitat",
      "fieldKey": "practice",
      "variantId": "default",
      "text": "Consider habitat and forgone sale together. No single stem size is automatically the right deadwood choice. Unrelated felling elsewhere cannot satisfy this lesson.",
      "baselineText": "Consider habitat and forgone sale together. No single stem size is automatically the right deadwood choice. Unrelated felling elsewhere cannot satisfy this lesson.",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/ScenarioOneTeachingSequence.cs",
        "branch": "main",
        "line": 50,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": "habitat",
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.teaching.response.title",
      "pageId": "s1.teaching.response",
      "fieldKey": "title",
      "variantId": "default",
      "text": "5. Return and review",
      "baselineText": "5. Return and review",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/ScenarioOneTeachingSequence.cs",
        "branch": "main",
        "line": 55,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": "response",
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.teaching.response.concept",
      "pageId": "s1.teaching.response",
      "fieldKey": "concept",
      "variantId": "default",
      "text": "Did your intervention create enough growing space? Revisit all three retained trees and review the surrounding stand along both sides of the rack. Good CCF management includes deciding not to cut.",
      "baselineText": "Did your intervention create enough growing space? Revisit all three retained trees and review the surrounding stand along both sides of the rack. Good CCF management includes deciding not to cut.",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/ScenarioOneTeachingSequence.cs",
        "branch": "main",
        "line": 55,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": "response",
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.teaching.response.action",
      "pageId": "s1.teaching.response",
      "fieldKey": "action",
      "variantId": "default",
      "text": "Read and acknowledge the Annual Review, then close and reopen each Crop Tree's inspection [E]. Inspect the surrounding retained stand in each zone on both sides. While inspecting, choose [J] Retain here, or mark justified additional competitors and choose [K] Release further. No additional felling is required.",
      "baselineText": "Read and acknowledge the Annual Review, then close and reopen each Crop Tree's inspection [E]. Inspect the surrounding retained stand in each zone on both sides. While inspecting, choose [J] Retain here, or mark justified additional competitors and choose [K] Release further. No additional felling is required.",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/ScenarioOneTeachingSequence.cs",
        "branch": "main",
        "line": 55,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": "response",
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.teaching.response.caption",
      "pageId": "s1.teaching.response",
      "fieldKey": "caption",
      "variantId": "default",
      "text": "Compare the response with your intention; retaining trees without another cut is success.",
      "baselineText": "Compare the response with your intention; retaining trees without another cut is success.",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/ScenarioOneTeachingSequence.cs",
        "branch": "main",
        "line": 55,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": "response",
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.teaching.response.hudAction",
      "pageId": "s1.teaching.response",
      "fieldKey": "hudAction",
      "variantId": "default",
      "text": "Revisit all three trees and retained stand: [J] retain here, [K] justified further release.",
      "baselineText": "Revisit all three trees and retained stand: [J] retain here, [K] justified further release.",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/ScenarioOneTeachingSequence.cs",
        "branch": "main",
        "line": 55,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": "response",
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.teaching.response.practice",
      "pageId": "s1.teaching.response",
      "fieldKey": "practice",
      "variantId": "default",
      "text": "Retain here is a successful decision. Additional work must stay within the teaching footprint and safeguard to count. Revisit after further executed work. Individual inspection decisions are not stored. Once this first review is complete, the lesson remembers completion after loading.",
      "baselineText": "Retain here is a successful decision. Additional work must stay within the teaching footprint and safeguard to count. Revisit after further executed work. Individual inspection decisions are not stored. Once this first review is complete, the lesson remembers completion after loading.",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/ScenarioOneTeachingSequence.cs",
        "branch": "main",
        "line": 55,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": "response",
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.teaching.recruitment.title",
      "pageId": "s1.teaching.recruitment",
      "fieldKey": "title",
      "variantId": "default",
      "text": "6. Secure the next generation",
      "baselineText": "6. Secure the next generation",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/ScenarioOneTeachingSequence.cs",
        "branch": "main",
        "line": 60,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": "recruitment",
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.teaching.recruitment.concept",
      "pageId": "s1.teaching.recruitment",
      "fieldKey": "concept",
      "variantId": "default",
      "text": "Young trees need suitable light and protection from browsing and vegetation. Inspect existing regeneration first. This scenario also asks you to introduce beech and sessile oak; planting gives a player-controlled route, but survival is not guaranteed.",
      "baselineText": "Young trees need suitable light and protection from browsing and vegetation. Inspect existing regeneration first. This scenario also asks you to introduce beech and sessile oak; planting gives a player-controlled route, but survival is not guaranteed.",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/ScenarioOneTeachingSequence.cs",
        "branch": "main",
        "line": 60,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": "recruitment",
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.teaching.recruitment.action",
      "pageId": "s1.teaching.recruitment",
      "fieldKey": "action",
      "variantId": "default",
      "text": "Use map layers to choose sites, then inspect them on foot. Buy nursery stock and plan beech and sessile oak. Review browsing and the optional shelter choice, approve planting, then advance one year.",
      "baselineText": "Use map layers to choose sites, then inspect them on foot. Buy nursery stock and plan beech and sessile oak. Review browsing and the optional shelter choice, approve planting, then advance one year.",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/ScenarioOneTeachingSequence.cs",
        "branch": "main",
        "line": 60,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": "recruitment",
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.teaching.recruitment.caption",
      "pageId": "s1.teaching.recruitment",
      "fieldKey": "caption",
      "variantId": "default",
      "text": "Inspect existing regeneration first. Consider suitable light, browsing and vegetation before planting; an optional shelter does not guarantee survival.",
      "baselineText": "Inspect existing regeneration first. Consider suitable light, browsing and vegetation before planting; an optional shelter does not guarantee survival.",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/ScenarioOneTeachingSequence.cs",
        "branch": "main",
        "line": 60,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": "recruitment",
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.teaching.recruitment.hudAction",
      "pageId": "s1.teaching.recruitment",
      "fieldKey": "hudAction",
      "variantId": "default",
      "text": "Check light and young trees, then plan beech and sessile oak planting in the Work Plan [Tab].",
      "baselineText": "Check light and young trees, then plan beech and sessile oak planting in the Work Plan [Tab].",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/ScenarioOneTeachingSequence.cs",
        "branch": "main",
        "line": 60,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": "recruitment",
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.teaching.establishment.title",
      "pageId": "s1.teaching.establishment",
      "fieldKey": "title",
      "variantId": "default",
      "text": "7. Return to the young trees",
      "baselineText": "7. Return to the young trees",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/ScenarioOneTeachingSequence.cs",
        "branch": "main",
        "line": 64,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": "establishment",
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.teaching.establishment.concept",
      "pageId": "s1.teaching.establishment",
      "fieldKey": "concept",
      "variantId": "default",
      "text": "Establishment is not the same as successful recruitment. Let another year pass, then compare survival, browsing and vegetation with the planting-year results. Failure is useful information too.",
      "baselineText": "Establishment is not the same as successful recruitment. Let another year pass, then compare survival, browsing and vegetation with the planting-year results. Failure is useful information too.",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/ScenarioOneTeachingSequence.cs",
        "branch": "main",
        "line": 64,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": "establishment",
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.teaching.establishment.action",
      "pageId": "s1.teaching.establishment",
      "fieldKey": "action",
      "variantId": "default",
      "text": "Read the planting results first. Advance one further year, read and acknowledge that Annual Review, then revisit the planted sites. Adapt any later work to what you find.",
      "baselineText": "Read the planting results first. Advance one further year, read and acknowledge that Annual Review, then revisit the planted sites. Adapt any later work to what you find.",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/ScenarioOneTeachingSequence.cs",
        "branch": "main",
        "line": 64,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": "establishment",
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.teaching.establishment.caption",
      "pageId": "s1.teaching.establishment",
      "fieldKey": "caption",
      "variantId": "default",
      "text": "The same planted sites are revisited after another year. Compare survival, browsing and vegetation with the planting-year results; this example includes failure despite a shelter.",
      "baselineText": "The same planted sites are revisited after another year. Compare survival, browsing and vegetation with the planting-year results; this example includes failure despite a shelter.",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/ScenarioOneTeachingSequence.cs",
        "branch": "main",
        "line": 64,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": "establishment",
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.teaching.establishment.hudAction",
      "pageId": "s1.teaching.establishment",
      "fieldKey": "hudAction",
      "variantId": "default",
      "text": "Advance a year, read the review, then revisit the planted trees.",
      "baselineText": "Advance a year, read the review, then revisit the planted trees.",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/ScenarioOneTeachingSequence.cs",
        "branch": "main",
        "line": 64,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": "establishment",
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.teaching.transformation.title",
      "pageId": "s1.teaching.transformation",
      "fieldKey": "title",
      "variantId": "default",
      "text": "8. Continue the transformation",
      "baselineText": "8. Continue the transformation",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/ScenarioOneTeachingSequence.cs",
        "branch": "main",
        "line": 68,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": "transformation",
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.teaching.transformation.concept",
      "pageId": "s1.teaching.transformation",
      "fieldKey": "concept",
      "variantId": "default",
      "text": "The woodland is not finished after a few interventions. Continue observing and adapting while keeping tree cover. The eight scenario goals remain separate from these lessons.",
      "baselineText": "The woodland is not finished after a few interventions. Continue observing and adapting while keeping tree cover. The eight scenario goals remain separate from these lessons.",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/ScenarioOneTeachingSequence.cs",
        "branch": "main",
        "line": 68,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": "transformation",
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.teaching.transformation.action",
      "pageId": "s1.teaching.transformation",
      "fieldKey": "action",
      "variantId": "default",
      "text": "Review All scenario goals below and the management history. Revisit the forest and plan later work only where justified; advance time and reassess until the scenario goals are met.",
      "baselineText": "Review All scenario goals below and the management history. Revisit the forest and plan later work only where justified; advance time and reassess until the scenario goals are met.",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/ScenarioOneTeachingSequence.cs",
        "branch": "main",
        "line": 68,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": "transformation",
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.teaching.transformation.caption",
      "pageId": "s1.teaching.transformation",
      "fieldKey": "caption",
      "variantId": "default",
      "text": "Retained forest, young trees and observed failure remain together. Reassess before any further work; the woodland is never finished.",
      "baselineText": "Retained forest, young trees and observed failure remain together. Reassess before any further work; the woodland is never finished.",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/ScenarioOneTeachingSequence.cs",
        "branch": "main",
        "line": 68,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": "transformation",
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.teaching.transformation.hudAction",
      "pageId": "s1.teaching.transformation",
      "fieldKey": "hudAction",
      "variantId": "default",
      "text": "Keep observing and adapting; all scenario goals are listed under [O].",
      "baselineText": "Keep observing and adapting; all scenario goals are listed under [O].",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/ScenarioOneTeachingSequence.cs",
        "branch": "main",
        "line": 68,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": "transformation",
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.help.treeinspection.title",
      "pageId": "s1.help.treeinspection",
      "fieldKey": "title",
      "variantId": "default",
      "text": "Tree Inspection · understand one tree",
      "baselineText": "Tree Inspection · understand one tree",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/MenuHelpView.Text.cs",
        "branch": "main",
        "line": 40,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": null,
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.help.treeinspection.summary.first-use",
      "pageId": "s1.help.treeinspection",
      "fieldKey": "summary",
      "variantId": "first-use",
      "text": "Start with a tree worth keeping, then see what competes with it. The card shows its size and how crowded it is; press V for more detail. Press C to mark a Crop Tree (keep and favour) or X to propose felling. Marks are only proposals: nothing is cut until you approve work in the Work Plan.",
      "baselineText": "Start with a tree worth keeping, then see what competes with it. The card shows its size and how crowded it is; press V for more detail. Press C to mark a Crop Tree (keep and favour) or X to propose felling. Marks are only proposals: nothing is cut until you approve work in the Work Plan.",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/MenuHelpView.Text.cs",
        "branch": "main",
        "line": 40,
        "kind": "conditional"
      },
      "status": "main-source",
      "protectedGameplayKey": null,
      "format": "plain-text",
      "tokens": [],
      "note": "Conditional screen text; first-use/full help may differ. Any C# constructed portions require adapter audit."
    },
    {
      "id": "s1.help.treeinspection.explanation.full-F1",
      "pageId": "s1.help.treeinspection",
      "fieldKey": "explanation",
      "variantId": "full-F1",
      "text": "Start with a tree worth keeping, then inspect its competitors. Inspect a tree with E (or left-click) to understand its size, growth, competition and management state. Use this before deciding which trees to favour or remove.\n\nDBH is trunk diameter measured at breast height. Competition shows whether neighbours restrict growth; the competition index (CI) is one number combining how large and how close the neighbouring trees are, and a higher number means more competition. Press V on the card for the rest: stem volume, light on the ground at the tree's base, recent growth, wind exposure and pruning.\n\nA Crop Tree is selected to retain and favour for future development. A Fell mark proposes removal. X and C change these marks; they do not execute work.\n\nCrop Trees are trees you choose to favour for the future. Nearby trees can reduce their growing space, but being small or suppressed does not by itself make a tree a problem. Inspect which neighbours contribute most to competition before deciding what to remove. On a Crop Tree's card, keys 1–5 show its listed neighbours in the forest.\n\nClose Help, then press E to close inspection and return to walking.",
      "baselineText": "Start with a tree worth keeping, then inspect its competitors. Inspect a tree with E (or left-click) to understand its size, growth, competition and management state. Use this before deciding which trees to favour or remove.\n\nDBH is trunk diameter measured at breast height. Competition shows whether neighbours restrict growth; the competition index (CI) is one number combining how large and how close the neighbouring trees are, and a higher number means more competition. Press V on the card for the rest: stem volume, light on the ground at the tree's base, recent growth, wind exposure and pruning.\n\nA Crop Tree is selected to retain and favour for future development. A Fell mark proposes removal. X and C change these marks; they do not execute work.\n\nCrop Trees are trees you choose to favour for the future. Nearby trees can reduce their growing space, but being small or suppressed does not by itself make a tree a problem. Inspect which neighbours contribute most to competition before deciding what to remove. On a Crop Tree's card, keys 1–5 show its listed neighbours in the forest.\n\nClose Help, then press E to close inspection and return to walking.",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/MenuHelpView.Text.cs",
        "branch": "main",
        "line": 40,
        "kind": "conditional"
      },
      "status": "main-source",
      "protectedGameplayKey": null,
      "format": "plain-text",
      "tokens": [],
      "note": "Conditional screen text; first-use/full help may differ. Any C# constructed portions require adapter audit."
    },
    {
      "id": "s1.help.standmap.title",
      "pageId": "s1.help.standmap",
      "fieldKey": "title",
      "variantId": "default",
      "text": "Stand Map · find where to look",
      "baselineText": "Stand Map · find where to look",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/MenuHelpView.Text.cs",
        "branch": "main",
        "line": 41,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": null,
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.help.standmap.summary.first-use",
      "pageId": "s1.help.standmap",
      "fieldKey": "summary",
      "variantId": "first-use",
      "text": "The map helps you decide where to look, not what to do. Choose a layer, select a square to read it, set a waypoint, then walk there and inspect the site yourself. It cannot mark, clear, plant or approve anything remotely.",
      "baselineText": "The map helps you decide where to look, not what to do. Choose a layer, select a square to read it, set a waypoint, then walk there and inspect the site yourself. It cannot mark, clear, plant or approve anything remotely.",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/MenuHelpView.Text.cs",
        "branch": "main",
        "line": 41,
        "kind": "conditional"
      },
      "status": "main-source",
      "protectedGameplayKey": null,
      "format": "plain-text",
      "tokens": [],
      "note": "Conditional screen text; first-use/full help may differ. Any C# constructed portions require adapter audit."
    },
    {
      "id": "s1.help.standmap.explanation.full-F1",
      "pageId": "s1.help.standmap",
      "fieldKey": "explanation",
      "variantId": "full-F1",
      "text": "The Stand Map is a plan of your property seen from above: every tree is drawn at the size of its crown, with the racks, the forest path and you, on a grid in metres. It helps you find patterns across the forest that are difficult to see among the trees. Open it with M when looking for a site to inspect. Underneath, the ground is divided into squares of 5 m × 5 m, each named by a letter and a number: point at the map to read the square and its figures.\n\nEach layer adds its colour over the same plan: use Light for darker/brighter areas, Regeneration for young growth, Browsing / protection for deer browsing (deer eating the shoots of young trees) and the shelters that protect planted saplings, and Fell & crop marks for your marked trees.\n\nSelect a square, read its information, then choose Set Waypoint. Close the map with M, Esc or Back to forest. Follow the HUD direction and distance, then inspect the site directly before deciding what to do.\n\nThe map helps you find where to look. It cannot mark, clear, plant or approve work remotely, and it does not make the forestry decision for you.",
      "baselineText": "The Stand Map is a plan of your property seen from above: every tree is drawn at the size of its crown, with the racks, the forest path and you, on a grid in metres. It helps you find patterns across the forest that are difficult to see among the trees. Open it with M when looking for a site to inspect. Underneath, the ground is divided into squares of 5 m × 5 m, each named by a letter and a number: point at the map to read the square and its figures.\n\nEach layer adds its colour over the same plan: use Light for darker/brighter areas, Regeneration for young growth, Browsing / protection for deer browsing (deer eating the shoots of young trees) and the shelters that protect planted saplings, and Fell & crop marks for your marked trees.\n\nSelect a square, read its information, then choose Set Waypoint. Close the map with M, Esc or Back to forest. Follow the HUD direction and distance, then inspect the site directly before deciding what to do.\n\nThe map helps you find where to look. It cannot mark, clear, plant or approve work remotely, and it does not make the forestry decision for you.",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/MenuHelpView.Text.cs",
        "branch": "main",
        "line": 41,
        "kind": "conditional"
      },
      "status": "main-source",
      "protectedGameplayKey": null,
      "format": "plain-text",
      "tokens": [],
      "note": "Conditional screen text; first-use/full help may differ. Any C# constructed portions require adapter audit."
    },
    {
      "id": "s1.help.workplan.title",
      "pageId": "s1.help.workplan",
      "fieldKey": "title",
      "variantId": "default",
      "text": "Work Plan · review and approve jobs",
      "baselineText": "Work Plan · review and approve jobs",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/MenuHelpView.Text.cs",
        "branch": "main",
        "line": 42,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": null,
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.help.workplan.summary.first-use",
      "pageId": "s1.help.workplan",
      "fieldKey": "summary",
      "variantId": "first-use",
      "text": "The Work Plan collects your marks and planting spots into jobs you can review and approve. Check each cost, then approve the plan. Approval alone changes nothing: Advance one year carries out the approved jobs, and jobs you have not approved wait.",
      "baselineText": "The Work Plan collects your marks and planting spots into jobs you can review and approve. Check each cost, then approve the plan. Approval alone changes nothing: Advance one year carries out the approved jobs, and jobs you have not approved wait.",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/MenuHelpView.Text.cs",
        "branch": "main",
        "line": 42,
        "kind": "conditional"
      },
      "status": "main-source",
      "protectedGameplayKey": null,
      "format": "plain-text",
      "tokens": [],
      "note": "Conditional screen text; first-use/full help may differ. Any C# constructed portions require adapter audit."
    },
    {
      "id": "s1.help.workplan.explanation.full-F1",
      "pageId": "s1.help.workplan",
      "fieldKey": "explanation",
      "variantId": "full-F1",
      "text": "The Work Plan collects decisions made in the forest into jobs you can review and approve. Open it with Tab after marking trees or planting/clearance sites; your marks join the plan when you open it.\n\nReview labour, material costs, cash after approved work and expected timber value. A contractor's minimum job charge can make a small harvest expensive. Some tasks let you choose contractor or landowner execution.\n\nReview or remove jobs, choose available executors and purchase nursery stock here. Approve Work Plan commits the plan; Advance one year resolves approved jobs and forest change. Approval alone does not immediately change the forest.\n\nDecide what should happen in the forest; review how it will happen and what it costs here. The residual stand is what remains standing after the planned work, and its summary (including basal area, the trunk cross-section area per hectare) describes what remains; it does not decide the forest structure for you. Use Tab or Back to forest to return.",
      "baselineText": "The Work Plan collects decisions made in the forest into jobs you can review and approve. Open it with Tab after marking trees or planting/clearance sites; your marks join the plan when you open it.\n\nReview labour, material costs, cash after approved work and expected timber value. A contractor's minimum job charge can make a small harvest expensive. Some tasks let you choose contractor or landowner execution.\n\nReview or remove jobs, choose available executors and purchase nursery stock here. Approve Work Plan commits the plan; Advance one year resolves approved jobs and forest change. Approval alone does not immediately change the forest.\n\nDecide what should happen in the forest; review how it will happen and what it costs here. The residual stand is what remains standing after the planned work, and its summary (including basal area, the trunk cross-section area per hectare) describes what remains; it does not decide the forest structure for you. Use Tab or Back to forest to return.",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/MenuHelpView.Text.cs",
        "branch": "main",
        "line": 42,
        "kind": "conditional"
      },
      "status": "main-source",
      "protectedGameplayKey": null,
      "format": "plain-text",
      "tokens": [],
      "note": "Conditional screen text; first-use/full help may differ. Any C# constructed portions require adapter audit."
    },
    {
      "id": "s1.help.annualreview.title",
      "pageId": "s1.help.annualreview",
      "fieldKey": "title",
      "variantId": "default",
      "text": "Annual Review · learn from the year",
      "baselineText": "Annual Review · learn from the year",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/MenuHelpView.Text.cs",
        "branch": "main",
        "line": 43,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": null,
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.help.annualreview.explanation.full-F1",
      "pageId": "s1.help.annualreview",
      "fieldKey": "explanation",
      "variantId": "full-F1",
      "text": "After advancing time, the Annual Review shows what work happened and how the forest and finances changed. Read it before planning the next cycle.\n\nWORK DONE: what did I do, and did any jobs fail?\nMONEY: what did it cost or earn, and what cash remains?\nFOREST: how did growth, light, regeneration and habitat respond?\n\nAverage forest-floor light is the mean of the light reaching the ground over every 5 m × 5 m square of the stand, from 0 (dark) to 1 (open). Areas with young regeneration counts the squares where young trees are present; the scenario objective Young trees present asks for a minimum number of them. Neither number tells you which tree to fell.\n\nOne thinning does not finish continuous-cover forestry. Walk back, inspect the forest response and reassess before the next intervention. Openings can create an opportunity for regeneration; seed, light, browsing and vegetation affect the outcome. Use these results to decide what to inspect next. This screen records outcomes; it does not choose or execute forestry work.\n\nClose Help and read the three sections. After your first results, choose I've read the annual results to unlock further year advances. Use Esc or Walk the forest to return and inspect the changes; Open Work Plan returns to planning.",
      "baselineText": "After advancing time, the Annual Review shows what work happened and how the forest and finances changed. Read it before planning the next cycle.\n\nWORK DONE: what did I do, and did any jobs fail?\nMONEY: what did it cost or earn, and what cash remains?\nFOREST: how did growth, light, regeneration and habitat respond?\n\nAverage forest-floor light is the mean of the light reaching the ground over every 5 m × 5 m square of the stand, from 0 (dark) to 1 (open). Areas with young regeneration counts the squares where young trees are present; the scenario objective Young trees present asks for a minimum number of them. Neither number tells you which tree to fell.\n\nOne thinning does not finish continuous-cover forestry. Walk back, inspect the forest response and reassess before the next intervention. Openings can create an opportunity for regeneration; seed, light, browsing and vegetation affect the outcome. Use these results to decide what to inspect next. This screen records outcomes; it does not choose or execute forestry work.\n\nClose Help and read the three sections. After your first results, choose I've read the annual results to unlock further year advances. Use Esc or Walk the forest to return and inspect the changes; Open Work Plan returns to planning.",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/MenuHelpView.Text.cs",
        "branch": "main",
        "line": 43,
        "kind": "conditional"
      },
      "status": "main-source",
      "protectedGameplayKey": null,
      "format": "plain-text",
      "tokens": [],
      "note": "Conditional screen text; first-use/full help may differ. Any C# constructed portions require adapter audit."
    },
    {
      "id": "s1.help.rack.title",
      "pageId": "s1.help.rack",
      "fieldKey": "title",
      "variantId": "default",
      "text": "Extraction racks · open machine routes",
      "baselineText": "Extraction racks · open machine routes",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/MenuHelpView.Text.cs",
        "branch": "main",
        "line": 44,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": null,
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.help.rack.summary.first-use",
      "pageId": "s1.help.rack",
      "fieldKey": "summary",
      "variantId": "first-use",
      "text": "Racks are permanent routes used to remove timber from the stand. Establishing them now gives machinery access for this and future thinnings. Click a tree or ground point where the rack should start. The start stays fixed while you aim. The preview extends forward only. Click again to add it to the Work Plan; right-click reselects the start, and Esc cancels. Nothing is cut until you approve the plan.",
      "baselineText": "Racks are permanent routes used to remove timber from the stand. Establishing them now gives machinery access for this and future thinnings. Click a tree or ground point where the rack should start. The start stays fixed while you aim. The preview extends forward only. Click again to add it to the Work Plan; right-click reselects the start, and Esc cancels. Nothing is cut until you approve the plan.",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/MenuHelpView.Text.cs",
        "branch": "main",
        "line": 44,
        "kind": "conditional"
      },
      "status": "main-source",
      "protectedGameplayKey": null,
      "format": "plain-text",
      "tokens": [],
      "note": "Conditional screen text; first-use/full help may differ. Any C# constructed portions require adapter audit."
    },
    {
      "id": "s1.help.rack.explanation.full-F1",
      "pageId": "s1.help.rack",
      "fieldKey": "explanation",
      "variantId": "full-F1",
      "text": "An extraction rack is a straight corridor of felled trees across the plantation. The harvesting machine drives along it, so the rest of the stand can be thinned later without driving through standing trees. Racks are permanent extraction routes: they stay open for this and future thinnings.\n\nPress B in the forest to create a rack. Click the intended starting tree or ground point: its ring and post mark the fixed origin. Aim to pivot the violet corridor forward from that point; the arrow shows the direction and violet rings mark the trees it would take. The mouse wheel adjusts the direction. Click or press Enter again to add it to the Work Plan; right-click reselects the start; Esc or B cancels. No work is added by choosing a start.\n\nThis plantation follows an authored rack plan: five evenly spaced racks across the planted rows, with seven rows left standing between racks. The status line counts how many are in the Work Plan. After your first rack, the Stand Map can plan the rest: open it with M and choose Plan rack, which marks the squares of a planned rack that are still to do (◇) and those already in the Work Plan (✓).\n\nA rack keeps any Crop Tree on its line. Racks are felled in one contractor visit together with any other felling you have planned, so the visit minimum is shared. A rack is still ordinary felling work: approve it in the Work Plan [Tab], then advance a year.",
      "baselineText": "An extraction rack is a straight corridor of felled trees across the plantation. The harvesting machine drives along it, so the rest of the stand can be thinned later without driving through standing trees. Racks are permanent extraction routes: they stay open for this and future thinnings.\n\nPress B in the forest to create a rack. Click the intended starting tree or ground point: its ring and post mark the fixed origin. Aim to pivot the violet corridor forward from that point; the arrow shows the direction and violet rings mark the trees it would take. The mouse wheel adjusts the direction. Click or press Enter again to add it to the Work Plan; right-click reselects the start; Esc or B cancels. No work is added by choosing a start.\n\nThis plantation follows an authored rack plan: five evenly spaced racks across the planted rows, with seven rows left standing between racks. The status line counts how many are in the Work Plan. After your first rack, the Stand Map can plan the rest: open it with M and choose Plan rack, which marks the squares of a planned rack that are still to do (◇) and those already in the Work Plan (✓).\n\nA rack keeps any Crop Tree on its line. Racks are felled in one contractor visit together with any other felling you have planned, so the visit minimum is shared. A rack is still ordinary felling work: approve it in the Work Plan [Tab], then advance a year.",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/MenuHelpView.Text.cs",
        "branch": "main",
        "line": 44,
        "kind": "conditional"
      },
      "status": "main-source",
      "protectedGameplayKey": null,
      "format": "plain-text",
      "tokens": [],
      "note": "Conditional screen text; first-use/full help may differ. Any C# constructed portions require adapter audit."
    },
    {
      "id": "s1.help.treemarks.title",
      "pageId": "s1.help.treemarks",
      "fieldKey": "title",
      "variantId": "default",
      "text": "Introducing tree marking",
      "baselineText": "Introducing tree marking",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/MenuHelpView.Text.cs",
        "branch": "main",
        "line": 45,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": null,
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.help.treemarks.explanation.full-F1",
      "pageId": "s1.help.treemarks",
      "fieldKey": "explanation",
      "variantId": "full-F1",
      "text": "Scenario 1 introduces 3 CCF markings. In this forest a paint mark records a management decision on the tree itself:\n\n•  — \n\n One spray paints vertical stripes on four sides of the entrance tree.\n\n\n\nAfter Confirm Rack Plan, walk to a rack entrance and press P to equip the spray marker. Aim at the first rack tree nearest the path, within 3.5 m, and left-click; P stows the marker. A tree carries only one of these marks: the spray will not mark a Fell or Crop Tree, and X or C cannot change an orange mark. Marking cuts nothing: the rack's removal work still needs approval in the Work Plan [Tab].",
      "baselineText": "Scenario 1 introduces 3 CCF markings. In this forest a paint mark records a management decision on the tree itself:\n\n•  — \n\n One spray paints vertical stripes on four sides of the entrance tree.\n\n\n\nAfter Confirm Rack Plan, walk to a rack entrance and press P to equip the spray marker. Aim at the first rack tree nearest the path, within 3.5 m, and left-click; P stows the marker. A tree carries only one of these marks: the spray will not mark a Fell or Crop Tree, and X or C cannot change an orange mark. Marking cuts nothing: the rack's removal work still needs approval in the Work Plan [Tab].",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/MenuHelpView.Text.cs",
        "branch": "main",
        "line": 45,
        "kind": "conditional"
      },
      "status": "main-source",
      "protectedGameplayKey": null,
      "format": "plain-text",
      "tokens": [],
      "note": "Conditional screen text; first-use/full help may differ. Any C# constructed portions require adapter audit."
    },
    {
      "id": "s1.help.rack.explanation.enlarged80-rack-v2",
      "pageId": "s1.help.rack",
      "fieldKey": "explanation",
      "variantId": "enlarged80-rack-v2",
      "text": "Plan extraction racks on the Stand Map [M]. Choose Plan Racks, create or edit the corridors, then choose Confirm Rack Plan. Back in the forest, mark each rack entrance tree in orange with the spray marker [P]. ",
      "baselineText": "Plan extraction racks on the Stand Map [M]. Choose Plan Racks, create or edit the corridors, then choose Confirm Rack Plan. Back in the forest, mark each rack entrance tree in orange with the spray marker [P]. ",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/MenuHelpView.Text.cs",
        "branch": "main",
        "line": 31,
        "kind": "conditional"
      },
      "status": "main-source",
      "protectedGameplayKey": null,
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.help.treemarks.TreeMarksOrangeText.enlarged80-rack-v2",
      "pageId": "s1.help.treemarks",
      "fieldKey": "TreeMarksOrangeText",
      "variantId": "enlarged80-rack-v2",
      "text": "Orange vertical stripes identify rack entrance trees associated with machine-path removal in Scenario 1.",
      "baselineText": "Orange vertical stripes identify rack entrance trees associated with machine-path removal in Scenario 1.",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/MenuHelpView.Text.cs",
        "branch": "main",
        "line": 27,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": null,
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.help.treemarks.TreeMarksSimplificationText.enlarged80-rack-v2",
      "pageId": "s1.help.treemarks",
      "fieldKey": "TreeMarksSimplificationText",
      "variantId": "enlarged80-rack-v2",
      "text": "The game asks you to mark the rack entrance tree as a teaching action. Real marking practice may mark additional trees along a route.",
      "baselineText": "The game asks you to mark the rack entrance tree as a teaching action. Real marking practice may mark additional trees along a route.",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/MenuHelpView.Text.cs",
        "branch": "main",
        "line": 28,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": null,
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.help.annualreview.AnnualReviewReadPrompt.enlarged80-rack-v2",
      "pageId": "s1.help.annualreview",
      "fieldKey": "AnnualReviewReadPrompt",
      "variantId": "enlarged80-rack-v2",
      "text": "Read WORK DONE, MONEY and FOREST. One thinning does not finish continuous-cover forestry: when you have read them, walk back, inspect how the forest responded and reassess before the next intervention.",
      "baselineText": "Read WORK DONE, MONEY and FOREST. One thinning does not finish continuous-cover forestry: when you have read them, walk back, inspect how the forest responded and reassess before the next intervention.",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/MenuHelpView.Text.cs",
        "branch": "main",
        "line": 78,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": null,
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.controls.reference.key1",
      "pageId": "s1.controls.reference",
      "fieldKey": "key1",
      "variantId": "default",
      "text": "P",
      "baselineText": "P",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/ControlsReference.cs",
        "branch": "main",
        "line": 11,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": null,
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.controls.reference.description1",
      "pageId": "s1.controls.reference",
      "fieldKey": "description1",
      "variantId": "default",
      "text": "Equip / stow the rack spray marker after confirming the plan; aim within 3.5 m and left-click",
      "baselineText": "Equip / stow the rack spray marker after confirming the plan; aim within 3.5 m and left-click",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/ControlsReference.cs",
        "branch": "main",
        "line": 11,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": null,
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.controls.reference.key2",
      "pageId": "s1.controls.reference",
      "fieldKey": "key2",
      "variantId": "default",
      "text": "M",
      "baselineText": "M",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/ControlsReference.cs",
        "branch": "main",
        "line": 16,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": null,
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.controls.reference.description2",
      "pageId": "s1.controls.reference",
      "fieldKey": "description2",
      "variantId": "default",
      "text": "Stand Map: Plan Racks, or find a place to inspect",
      "baselineText": "Stand Map: Plan Racks, or find a place to inspect",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/ControlsReference.cs",
        "branch": "main",
        "line": 16,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": null,
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.controls.reference.key3",
      "pageId": "s1.controls.reference",
      "fieldKey": "key3",
      "variantId": "default",
      "text": "W A S D",
      "baselineText": "W A S D",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/ControlsReference.cs",
        "branch": "main",
        "line": 26,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": null,
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.controls.reference.description3",
      "pageId": "s1.controls.reference",
      "fieldKey": "description3",
      "variantId": "default",
      "text": "Walk. Hold Shift to run, Space to jump",
      "baselineText": "Walk. Hold Shift to run, Space to jump",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/ControlsReference.cs",
        "branch": "main",
        "line": 26,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": null,
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.controls.reference.key4",
      "pageId": "s1.controls.reference",
      "fieldKey": "key4",
      "variantId": "default",
      "text": "Mouse",
      "baselineText": "Mouse",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/ControlsReference.cs",
        "branch": "main",
        "line": 27,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": null,
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.controls.reference.description4",
      "pageId": "s1.controls.reference",
      "fieldKey": "description4",
      "variantId": "default",
      "text": "Look around. Esc frees the mouse; click to take it back",
      "baselineText": "Look around. Esc frees the mouse; click to take it back",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/ControlsReference.cs",
        "branch": "main",
        "line": 27,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": null,
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.controls.reference.key5",
      "pageId": "s1.controls.reference",
      "fieldKey": "key5",
      "variantId": "default",
      "text": "E",
      "baselineText": "E",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/ControlsReference.cs",
        "branch": "main",
        "line": 28,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": null,
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.controls.reference.description5",
      "pageId": "s1.controls.reference",
      "fieldKey": "description5",
      "variantId": "default",
      "text": "Inspect the tree or object you are looking at. V on the card shows more detail",
      "baselineText": "Inspect the tree or object you are looking at. V on the card shows more detail",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/ControlsReference.cs",
        "branch": "main",
        "line": 28,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": null,
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.controls.reference.key6",
      "pageId": "s1.controls.reference",
      "fieldKey": "key6",
      "variantId": "default",
      "text": "Left-click",
      "baselineText": "Left-click",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/ControlsReference.cs",
        "branch": "main",
        "line": 29,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": null,
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.controls.reference.description6",
      "pageId": "s1.controls.reference",
      "fieldKey": "description6",
      "variantId": "default",
      "text": "Also inspects the tree you look at; after Esc the first click only retakes the mouse",
      "baselineText": "Also inspects the tree you look at; after Esc the first click only retakes the mouse",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/ControlsReference.cs",
        "branch": "main",
        "line": 29,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": null,
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.controls.reference.key7",
      "pageId": "s1.controls.reference",
      "fieldKey": "key7",
      "variantId": "default",
      "text": "B",
      "baselineText": "B",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/ControlsReference.cs",
        "branch": "main",
        "line": 30,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": null,
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.controls.reference.description7",
      "pageId": "s1.controls.reference",
      "fieldKey": "description7",
      "variantId": "default",
      "text": "Access rack: click a start, aim forward, click to add; right-click reselects, Esc cancels",
      "baselineText": "Access rack: click a start, aim forward, click to add; right-click reselects, Esc cancels",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/ControlsReference.cs",
        "branch": "main",
        "line": 30,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": null,
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.controls.reference.key8",
      "pageId": "s1.controls.reference",
      "fieldKey": "key8",
      "variantId": "default",
      "text": "X",
      "baselineText": "X",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/ControlsReference.cs",
        "branch": "main",
        "line": 31,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": null,
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.controls.reference.description8",
      "pageId": "s1.controls.reference",
      "fieldKey": "description8",
      "variantId": "default",
      "text": "Mark the tree for felling: a proposal only. X again removes the mark",
      "baselineText": "Mark the tree for felling: a proposal only. X again removes the mark",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/ControlsReference.cs",
        "branch": "main",
        "line": 31,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": null,
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.controls.reference.key9",
      "pageId": "s1.controls.reference",
      "fieldKey": "key9",
      "variantId": "default",
      "text": "C",
      "baselineText": "C",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/ControlsReference.cs",
        "branch": "main",
        "line": 32,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": null,
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.controls.reference.description9",
      "pageId": "s1.controls.reference",
      "fieldKey": "description9",
      "variantId": "default",
      "text": "Mark it as a Crop Tree to keep and favour; its competitors appear on the card",
      "baselineText": "Mark it as a Crop Tree to keep and favour; its competitors appear on the card",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/ControlsReference.cs",
        "branch": "main",
        "line": 32,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": null,
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.controls.reference.key10",
      "pageId": "s1.controls.reference",
      "fieldKey": "key10",
      "variantId": "default",
      "text": "1 – 5",
      "baselineText": "1 – 5",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/ControlsReference.cs",
        "branch": "main",
        "line": 33,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": null,
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.controls.reference.description10",
      "pageId": "s1.controls.reference",
      "fieldKey": "description10",
      "variantId": "default",
      "text": "On a Crop Tree's card, show its competitors in the forest (in planting mode, 1 and 2 pick the stock)",
      "baselineText": "On a Crop Tree's card, show its competitors in the forest (in planting mode, 1 and 2 pick the stock)",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/ControlsReference.cs",
        "branch": "main",
        "line": 33,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": null,
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.controls.reference.key11",
      "pageId": "s1.controls.reference",
      "fieldKey": "key11",
      "variantId": "default",
      "text": "G",
      "baselineText": "G",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/ControlsReference.cs",
        "branch": "main",
        "line": 34,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": null,
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.controls.reference.description11",
      "pageId": "s1.controls.reference",
      "fieldKey": "description11",
      "variantId": "default",
      "text": "Planting mode: click to mark a planting spot (buy saplings in the Work Plan first)",
      "baselineText": "Planting mode: click to mark a planting spot (buy saplings in the Work Plan first)",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/ControlsReference.cs",
        "branch": "main",
        "line": 34,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": null,
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.controls.reference.key12",
      "pageId": "s1.controls.reference",
      "fieldKey": "key12",
      "variantId": "default",
      "text": "U",
      "baselineText": "U",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/ControlsReference.cs",
        "branch": "main",
        "line": 35,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": null,
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.controls.reference.description12",
      "pageId": "s1.controls.reference",
      "fieldKey": "description12",
      "variantId": "default",
      "text": "Plan clearance of competing vegetation where you are looking",
      "baselineText": "Plan clearance of competing vegetation where you are looking",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/ControlsReference.cs",
        "branch": "main",
        "line": 35,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": null,
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.controls.reference.key13",
      "pageId": "s1.controls.reference",
      "fieldKey": "key13",
      "variantId": "default",
      "text": "M",
      "baselineText": "M",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/ControlsReference.cs",
        "branch": "main",
        "line": 36,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": null,
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.controls.reference.description13",
      "pageId": "s1.controls.reference",
      "fieldKey": "description13",
      "variantId": "default",
      "text": "Stand Map: find where to look",
      "baselineText": "Stand Map: find where to look",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/ControlsReference.cs",
        "branch": "main",
        "line": 36,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": null,
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.controls.reference.key14",
      "pageId": "s1.controls.reference",
      "fieldKey": "key14",
      "variantId": "default",
      "text": "Tab",
      "baselineText": "Tab",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/ControlsReference.cs",
        "branch": "main",
        "line": 37,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": null,
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.controls.reference.description14",
      "pageId": "s1.controls.reference",
      "fieldKey": "description14",
      "variantId": "default",
      "text": "Work Plan: review, approve and advance a year",
      "baselineText": "Work Plan: review, approve and advance a year",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/ControlsReference.cs",
        "branch": "main",
        "line": 37,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": null,
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.controls.reference.key15",
      "pageId": "s1.controls.reference",
      "fieldKey": "key15",
      "variantId": "default",
      "text": "O",
      "baselineText": "O",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/ControlsReference.cs",
        "branch": "main",
        "line": 38,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": null,
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.controls.reference.description15",
      "pageId": "s1.controls.reference",
      "fieldKey": "description15",
      "variantId": "default",
      "text": "Learning objectives and lessons",
      "baselineText": "Learning objectives and lessons",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/ControlsReference.cs",
        "branch": "main",
        "line": 38,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": null,
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.controls.reference.key16",
      "pageId": "s1.controls.reference",
      "fieldKey": "key16",
      "variantId": "default",
      "text": "F1",
      "baselineText": "F1",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/ControlsReference.cs",
        "branch": "main",
        "line": 39,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": null,
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.controls.reference.description16",
      "pageId": "s1.controls.reference",
      "fieldKey": "description16",
      "variantId": "default",
      "text": "Help for the screen you are on",
      "baselineText": "Help for the screen you are on",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/ControlsReference.cs",
        "branch": "main",
        "line": 39,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": null,
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.controls.reference.key17",
      "pageId": "s1.controls.reference",
      "fieldKey": "key17",
      "variantId": "default",
      "text": "Esc",
      "baselineText": "Esc",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/ControlsReference.cs",
        "branch": "main",
        "line": 40,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": null,
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.controls.reference.description17",
      "pageId": "s1.controls.reference",
      "fieldKey": "description17",
      "variantId": "default",
      "text": "Free the mouse, or close the open panel",
      "baselineText": "Free the mouse, or close the open panel",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/ControlsReference.cs",
        "branch": "main",
        "line": 40,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": null,
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.controls.reference.key18",
      "pageId": "s1.controls.reference",
      "fieldKey": "key18",
      "variantId": "default",
      "text": "F5 / F9",
      "baselineText": "F5 / F9",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/ControlsReference.cs",
        "branch": "main",
        "line": 41,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": null,
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.controls.reference.description18",
      "pageId": "s1.controls.reference",
      "fieldKey": "description18",
      "variantId": "default",
      "text": "Save / Load",
      "baselineText": "Save / Load",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/ControlsReference.cs",
        "branch": "main",
        "line": 41,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": null,
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.controls.reference.key19",
      "pageId": "s1.controls.reference",
      "fieldKey": "key19",
      "variantId": "default",
      "text": "F10",
      "baselineText": "F10",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/ControlsReference.cs",
        "branch": "main",
        "line": 42,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": null,
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.controls.reference.description19",
      "pageId": "s1.controls.reference",
      "fieldKey": "description19",
      "variantId": "default",
      "text": "Session menu: new scenario, save, load, quit",
      "baselineText": "Session menu: new scenario, save, load, quit",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/ControlsReference.cs",
        "branch": "main",
        "line": 42,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": null,
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.annual.review.yearHeading",
      "pageId": "s1.annual.review",
      "fieldKey": "yearHeading",
      "variantId": "default",
      "text": "Annual review · Year {{year}} complete",
      "baselineText": "Annual review · Year {{year}} complete",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/AnnualReviewView.cs",
        "branch": "main",
        "line": 132,
        "kind": "generated"
      },
      "status": "main-source",
      "protectedGameplayKey": null,
      "format": "plain-text",
      "tokens": [
        "year"
      ],
      "note": "Template derived from interpolated C#; year is authoritative game state."
    },
    {
      "id": "s1.annual.review.historicalHeading",
      "pageId": "s1.annual.review",
      "fieldKey": "historicalHeading",
      "variantId": "default",
      "text": "Annual review · Year {{year}} (earlier year; latest is Year {{latestYear}})",
      "baselineText": "Annual review · Year {{year}} (earlier year; latest is Year {{latestYear}})",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/AnnualReviewView.cs",
        "branch": "main",
        "line": 132,
        "kind": "generated"
      },
      "status": "main-source",
      "protectedGameplayKey": null,
      "format": "plain-text",
      "tokens": [
        "year",
        "latestYear"
      ],
      "note": ""
    },
    {
      "id": "s1.annual.review.emptyState",
      "pageId": "s1.annual.review",
      "fieldKey": "emptyState",
      "variantId": "default",
      "text": "Advance a year from the Work Plan to see the first annual results.",
      "baselineText": "Advance a year from the Work Plan to see the first annual results.",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/AnnualReviewView.cs",
        "branch": "main",
        "line": 135,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": null,
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.annual.review.workColumn",
      "pageId": "s1.annual.review",
      "fieldKey": "workColumn",
      "variantId": "default",
      "text": "Work done",
      "baselineText": "Work done",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/AnnualReviewView.cs",
        "branch": "main",
        "line": 156,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": null,
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.annual.review.moneyColumn",
      "pageId": "s1.annual.review",
      "fieldKey": "moneyColumn",
      "variantId": "default",
      "text": "Money",
      "baselineText": "Money",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/AnnualReviewView.cs",
        "branch": "main",
        "line": 157,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": null,
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.annual.review.forestColumn",
      "pageId": "s1.annual.review",
      "fieldKey": "forestColumn",
      "variantId": "default",
      "text": "Forest",
      "baselineText": "Forest",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/AnnualReviewView.cs",
        "branch": "main",
        "line": 158,
        "kind": "literal"
      },
      "status": "main-source",
      "protectedGameplayKey": null,
      "format": "plain-text",
      "tokens": [],
      "note": ""
    },
    {
      "id": "s1.workplan.overview.title",
      "pageId": "s1.workplan.overview",
      "fieldKey": "title",
      "variantId": "default",
      "text": "Work Plan · Year {{year}}",
      "baselineText": "Work Plan · Year {{year}}",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/WorkPlanView.cs",
        "branch": "main",
        "line": 120,
        "kind": "generated"
      },
      "status": "main-source",
      "protectedGameplayKey": null,
      "format": "plain-text",
      "tokens": [
        "year"
      ],
      "note": ""
    },
    {
      "id": "s1.workplan.overview.status",
      "pageId": "s1.workplan.overview",
      "fieldKey": "status",
      "variantId": "default",
      "text": "{{openCount}} open task(s), {{approvedCount}} approved · external cost {{externalCost}} · expected timber {{expectedTimber}} · net {{netValue}} · cash after approved work {{cashAfter}}",
      "baselineText": "{{openCount}} open task(s), {{approvedCount}} approved · external cost {{externalCost}} · expected timber {{expectedTimber}} · net {{netValue}} · cash after approved work {{cashAfter}}",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/WorkPlanView.cs",
        "branch": "main",
        "line": 124,
        "kind": "generated"
      },
      "status": "main-source",
      "protectedGameplayKey": null,
      "format": "plain-text",
      "tokens": [
        "openCount",
        "approvedCount",
        "externalCost",
        "expectedTimber",
        "netValue",
        "cashAfter"
      ],
      "note": "Tokens stand for C# state and formatting values. Plural handling remains unadapted in standalone."
    },
    {
      "id": "s1.workplan.overview.ownerTime",
      "pageId": "s1.workplan.overview",
      "fieldKey": "ownerTime",
      "variantId": "default",
      "text": "Your time next year: {{plannedTime}} planned of {{availableTime}}",
      "baselineText": "Your time next year: {{plannedTime}} planned of {{availableTime}}",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/WorkPlanView.cs",
        "branch": "main",
        "line": 121,
        "kind": "generated"
      },
      "status": "main-source",
      "protectedGameplayKey": null,
      "format": "plain-text",
      "tokens": [
        "plannedTime",
        "availableTime"
      ],
      "note": ""
    },
    {
      "id": "s1.candidate.ux-443.standMapTitle.candidate-443",
      "pageId": "s1.candidate.ux-443",
      "fieldKey": "standMapTitle",
      "variantId": "candidate-443",
      "text": "Stand Map",
      "baselineText": "Stand Map",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/MenuHelpView.Text.cs",
        "branch": "feature/scenario-one-objective1-ux-repair-20261112-v1",
        "line": 41,
        "kind": "conditional"
      },
      "status": "unmerged-candidate",
      "protectedGameplayKey": null,
      "format": "plain-text",
      "tokens": [],
      "note": "NOT on main; do not overwrite shipped baseline."
    },
    {
      "id": "s1.candidate.ux-443.standMapSummary.candidate-443",
      "pageId": "s1.candidate.ux-443",
      "fieldKey": "standMapSummary",
      "variantId": "candidate-443",
      "text": "Choose a layer, select a square to read it, set a waypoint, then walk there and inspect the site yourself. It cannot mark, clear, plant or approve anything remotely.",
      "baselineText": "Choose a layer, select a square to read it, set a waypoint, then walk there and inspect the site yourself. It cannot mark, clear, plant or approve anything remotely.",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/MenuHelpView.Text.cs",
        "branch": "feature/scenario-one-objective1-ux-repair-20261112-v1",
        "line": 64,
        "kind": "conditional"
      },
      "status": "unmerged-candidate",
      "protectedGameplayKey": null,
      "format": "plain-text",
      "tokens": [],
      "note": "NOT on main; do not overwrite shipped baseline."
    },
    {
      "id": "s1.candidate.ux-443.rackMarkCue.candidate-443",
      "pageId": "s1.candidate.ux-443",
      "fieldKey": "rackMarkCue",
      "variantId": "candidate-443",
      "text": ". Return to the forest and mark the first designated rack-removal tree at the headland-side entrance of each rack: [P] spray marker, aim within 3.5 m, left-click.",
      "baselineText": ". Return to the forest and mark the first designated rack-removal tree at the headland-side entrance of each rack: [P] spray marker, aim within 3.5 m, left-click.",
      "required": true,
      "source": {
        "path": "Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs",
        "branch": "feature/scenario-one-objective1-ux-repair-20261112-v1",
        "line": 102,
        "kind": "conditional"
      },
      "status": "unmerged-candidate",
      "protectedGameplayKey": null,
      "format": "plain-text",
      "tokens": [],
      "note": ""
    }
  ],
  "coverage": {
    "classifications": {
      "collected": 235,
      "conditional": 27,
      "generated": 5,
      "serialized": 0,
      "unlocated": 0,
      "outOfScope": 0
    },
    "notes": [
      "Only checked-in main C# literals from listed sources are source-extracted; no Unity runtime output is tested.",
      "Work Plan/Annual Review numeric sentences are displayed as proposed parameterized templates and still require a typed Unity adapter.",
      "Conditional Old Legacy40 and Enlarged80 Rack V2 variants are separate; unmerged #443 wording is candidate-only.",
      "Future O2–O8 and compact O1 are empty placeholders, not invented curriculum."
    ]
  }
};
