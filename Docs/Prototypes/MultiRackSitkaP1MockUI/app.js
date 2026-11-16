/*
 * Multi P1 #655 — ISOLATED STATIC MOCK, NOT A SIMULATOR.
 * Every record is hand-authored and synthetic. This file never imports
 * P0/P1 fixtures, invokes a Host, advances ecological time, saves state,
 * executes work, promotes a source-review status or predicts biology.
 */
"use strict";

const DOMAIN = Object.freeze({
  DERIVED: "derived",
  ORIGINAL: "original",
  UNITY: "unity"
});
const MOCK_THRESHOLD_MM = 170; // illustrative only; never an authoritative P1 runtime rule
const MOCK_AREA_HA = 0.09;      // illustrative 30 m x 30 m square
const MOCK_PROVENANCE = "Hand-authored synthetic MOCK fixture in app.js. Not a P0/P1 catalog, Irish field sample, real source record, or Unity tree.";

const MOCK_TREES = Object.freeze([
  { id: "MOCK-001", x: 3,  y: 3,  dbhMm: 120, heightM: 8.8,  crownM: 2.1, age: 28, status: "living" },
  { id: "MOCK-002", x: 8,  y: 3,  dbhMm: 145, heightM: 10.5, crownM: 2.4, age: 28, status: "living" },
  { id: "MOCK-003", x: 13, y: 3,  dbhMm: 160, heightM: 12.0, crownM: 2.7, age: 28, status: "living" },
  { id: "MOCK-004", x: 19, y: 4,  dbhMm: 170, heightM: 13.2, crownM: 2.9, age: 28, status: "living" },
  { id: "MOCK-005", x: 25, y: 3,  dbhMm: 180, heightM: 14.0, crownM: 3.0, age: 28, status: "living" },
  { id: "MOCK-006", x: 5,  y: 10, dbhMm: 200, heightM: 15.2, crownM: 3.2, age: 28, status: "living" },
  { id: "MOCK-007", x: 10, y: 12, dbhMm: 220, heightM: 17.8, crownM: 3.5, age: 28, status: "living" },
  { id: "MOCK-008", x: 16, y: 10, dbhMm: 235, heightM: 18.5, crownM: 3.6, age: 28, status: "living" },
  { id: "MOCK-009", x: 24, y: 10, dbhMm: 250, heightM: 19.7, crownM: 3.8, age: 28, status: "living" },
  { id: "MOCK-010", x: 5,  y: 19, dbhMm: 270, heightM: 20.6, crownM: 4.0, age: 28, status: "living" },
  { id: "MOCK-011", x: 13, y: 21, dbhMm: 290, heightM: 21.8, crownM: 4.2, age: 28, status: "living" },
  { id: "MOCK-012", x: 23, y: 21, dbhMm: 310, heightM: 22.3, crownM: 4.4, age: 28, status: "living" },
  { id: "MOCK-013", x: 7,  y: 26, dbhMm: 260, heightM: 20.0, crownM: 3.6, age: 28, status: "dead" },
  { id: "MOCK-014", x: 21, y: 26, dbhMm: 190, heightM: 15.9, crownM: 3.1, age: 28, status: "dead" }
].map(Object.freeze));

const byId = new Map(MOCK_TREES.map(function (tree) { return [tree.id, tree]; }));
let selectedId = "MOCK-007";
let selectedDomain = DOMAIN.DERIVED;
let selectedTab = "overview";

function el(id) {
  return document.getElementById(id);
}
function setText(id, value) {
  el(id).textContent = String(value);
}
function isActionable(tree) {
  return tree.status === "living" && tree.dbhMm >= MOCK_THRESHOLD_MM;
}
function basalAreaOf(tree) {
  const diameterMetres = tree.dbhMm / 1000;
  return Math.PI * diameterMetres * diameterMetres / 4;
}
function basalAreaOfGroup(trees) {
  return trees.reduce(function (sum, tree) { return sum + basalAreaOf(tree); }, 0);
}
function labelStatus(tree) {
  return tree.status === "living" ? "Living (mock)" : "Dead (mock; cause/year unknown)";
}
function labelEligibility(tree) {
  if (tree.status !== "living") return "No — dead mock stem";
  return isActionable(tree) ? "Yes — above/equal mock 170 mm" : "No — below mock 170 mm";
}

/* These are geometric summaries of the fixed synthetic fixture, not forest dynamics. */
function renderMetrics() {
  const living = MOCK_TREES.filter(function (tree) { return tree.status === "living"; });
  const actionable = living.filter(isActionable);
  const livingBa = basalAreaOfGroup(living);
  const actionBa = basalAreaOfGroup(actionable);
  setText("living-n", living.length);
  setText("living-ba", livingBa.toFixed(3));
  setText("living-nha", (living.length / MOCK_AREA_HA).toFixed(1));
  setText("living-baha", (livingBa / MOCK_AREA_HA).toFixed(3));
  setText("action-n", actionable.length);
  setText("action-ba", actionBa.toFixed(3));
}

/* SVG is an illustration of the authored XY pairs. It does not compute light,
   canopy competition, spatial growth, stocking rules or any forest outcome. */
function buildMap() {
  const svgNS = "http://www.w3.org/2000/svg";
  const points = el("map-points");
  for (const tree of MOCK_TREES) {
    const dot = document.createElementNS(svgNS, "circle");
    const isBelow = tree.status === "living" && !isActionable(tree);
    dot.setAttribute("cx", String(67 + 374 * tree.x / 30));
    dot.setAttribute("cy", String(432 - 374 * tree.y / 30));
    dot.setAttribute("r", "9");
    dot.setAttribute("tabindex", "0");
    dot.setAttribute("role", "button");
    dot.setAttribute("aria-label", "Select synthetic tree " + tree.id + "; " + labelStatus(tree) + ", DBH " + tree.dbhMm + " mm");
    dot.dataset.treeId = tree.id;
    dot.classList.add("map-tree");
    if (tree.status === "dead") dot.classList.add("dead");
    if (isBelow) dot.classList.add("below");
    const title = document.createElementNS(svgNS, "title");
    title.textContent = tree.id + " • " + tree.dbhMm + " mm • " + labelStatus(tree);
    dot.appendChild(title);
    dot.addEventListener("click", function () { selectTree(tree.id); });
    dot.addEventListener("keydown", function (event) {
      if (event.key === "Enter" || event.key === " " || event.key === "Spacebar") {
        event.preventDefault();
        selectTree(tree.id);
      }
    });
    dot.addEventListener("mouseenter", function () { hoverMapTree(tree.id, true); });
    dot.addEventListener("mouseleave", function () { hoverMapTree(tree.id, false); });
    dot.addEventListener("focus", function () { hoverMapTree(tree.id, true); });
    dot.addEventListener("blur", function () { hoverMapTree(tree.id, false); });
    points.appendChild(dot);
  }
  renderSelection();
}
function hoverMapTree(id, on) {
  const dot = Array.from(el("map-points").children).find(function (item) { return item.dataset.treeId === id; });
  if (dot) dot.dataset.hover = on ? "true" : "false";
  const tree = byId.get(id);
  setText("map-hint", on
    ? tree.id + " — " + labelStatus(tree) + " / DBH " + tree.dbhMm + " mm. Activate to inspect."
    : "Activate a circle or table “Select” button. Tab and Enter/Space work on map markers.");
}
function selectTree(id) {
  if (!byId.has(id)) return;
  selectedId = id; // UI selection only; never changes the mock tree record
  renderSelection();
}
function renderSelection() {
  const tree = byId.get(selectedId);
  if (!tree) return;
  setText("tree-id", tree.id);
  setText("tree-status", labelStatus(tree));
  setText("tree-species", "Sitka spruce (mock)");
  setText("tree-dbh", tree.dbhMm + " mm (" + (tree.dbhMm / 10).toFixed(1) + " cm)");
  setText("tree-height", tree.heightM.toFixed(1) + " m");
  setText("tree-crown", tree.crownM.toFixed(1) + " m");
  setText("tree-age", tree.age + " years (mock)");
  setText("tree-xy", tree.x.toFixed(1) + " m E, " + tree.y.toFixed(1) + " m N");
  setText("tree-eligibility", labelEligibility(tree));
  setText("tree-provenance", MOCK_PROVENANCE);
  for (const dot of el("map-points").children) {
    const selected = dot.dataset.treeId === selectedId;
    dot.classList.toggle("selected", selected);
    dot.setAttribute("aria-pressed", selected ? "true" : "false");
  }
  for (const row of el("tree-rows").children) {
    const selected = row.dataset.treeId === selectedId;
    row.classList.toggle("selected-row", selected);
    const select = row.querySelector("button");
    if (select) select.setAttribute("aria-pressed", selected ? "true" : "false");
  }
  const inFilteredList = Array.from(el("tree-rows").children).some(function (row) {
    return row.dataset.treeId === selectedId;
  });
  if (!inFilteredList) {
    setText("filter-hint", "Selected " + selectedId + " is outside the current filter. The map and inspector still show the selected synthetic tree.");
  } else {
    setText("filter-hint", "Selection synced: " + selectedId + " on the map, table and inspector. Filtering/sorting does not change any tree.");
  }
}
function cell(row, text) {
  const td = document.createElement("td");
  td.textContent = String(text);
  row.appendChild(td);
}
function renderTable() {
  const search = el("search-tree").value.trim().toUpperCase();
  const filter = el("filter-tree").value;
  const sort = el("sort-tree").value;
  const rows = MOCK_TREES.filter(function (tree) {
    if (search && !tree.id.includes(search)) return false;
    if (filter === "living") return tree.status === "living";
    if (filter === "dead") return tree.status === "dead";
    if (filter === "actionable") return isActionable(tree);
    if (filter === "below") return tree.status === "living" && !isActionable(tree);
    return true;
  }).slice();
  rows.sort(function (a, b) {
    if (sort === "id-desc") return b.id.localeCompare(a.id);
    if (sort === "dbh-asc") return a.dbhMm - b.dbhMm || a.id.localeCompare(b.id);
    if (sort === "dbh-desc") return b.dbhMm - a.dbhMm || a.id.localeCompare(b.id);
    return a.id.localeCompare(b.id);
  });

  const tbody = el("tree-rows");
  tbody.replaceChildren();
  if (!rows.length) {
    const tr = document.createElement("tr");
    const td = document.createElement("td");
    td.colSpan = 6;
    td.className = "empty-row";
    td.textContent = "No synthetic records match this filter. Clear the search or change the filter.";
    tr.appendChild(td);
    tbody.appendChild(tr);
  }
  for (const tree of rows) {
    const tr = document.createElement("tr");
    tr.dataset.treeId = tree.id;
    const chooseCell = document.createElement("td");
    const button = document.createElement("button");
    button.type = "button";
    button.className = "select-tree";
    button.textContent = "Select " + tree.id;
    button.setAttribute("aria-label", "Select synthetic tree " + tree.id + " for inspection");
    button.addEventListener("click", function () { selectTree(tree.id); });
    chooseCell.appendChild(button);
    tr.appendChild(chooseCell);
    cell(tr, tree.status === "living" ? "Living" : "Dead (mock)");
    cell(tr, tree.dbhMm);
    cell(tr, tree.heightM.toFixed(1));
    cell(tr, tree.x + ", " + tree.y);
    cell(tr, labelEligibility(tree));
    tbody.appendChild(tr);
  }
  setText("filter-count", rows.length + " / " + MOCK_TREES.length + " mock records");
  renderSelection();
}
function setDomain(domain) {
  if (!Object.values(DOMAIN).includes(domain)) return;
  selectedDomain = domain;
  for (const choice of document.querySelectorAll(".source-choice")) {
    choice.classList.toggle("active", choice.querySelector("input").value === domain);
  }
  const derived = domain === DOMAIN.DERIVED;
  el("derived-content").hidden = !derived;
  el("domain-placeholder").hidden = derived;
  if (derived) {
    setText("source-state", "MOCK VIEW • P1 MANIFEST PendingIndependentReview");
  } else if (domain === DOMAIN.ORIGINAL) {
    setText("source-state", "INSPECT-ONLY DOMAIN • NO ORIGINAL FIXTURE LOADED");
    setText("placeholder-label", "OriginalPolygonExploratoryV1 • SOURCE NOT LOADED");
    setText("placeholder-title", "Original source inspection is not supplied");
    setText("placeholder-body", "The original polygon and its IDs must be read unchanged from an independently trusted P0 fixture. This standalone mock cannot inspect that source, cannot infer its area, and will not turn its geometry into a 30 m square. Original annual work is refused.");
  } else {
    setText("source-state", "UNAVAILABLE • FUTURE SOURCE-LEGAL PARITY");
    setText("placeholder-label", "DerivedUnitySquareParityV1 • NOT AVAILABLE");
    setText("placeholder-title", "Unity parity has not been demonstrated");
    setText("placeholder-body", "Future 40 m and 80 m candidates require separate owner/legal source approval and real verification. The 30 m mock is not a Unity parity fixture. No Unity data, import or numerical result exists in this page.");
  }
}
function setTab(tab) {
  if (tab !== "overview" && tab !== "comparison") return;
  selectedTab = tab;
  const overview = tab === "overview";
  el("overview-screen").hidden = !overview;
  el("comparison-screen").hidden = overview;
  el("tab-overview").classList.toggle("active", overview);
  el("tab-comparison").classList.toggle("active", !overview);
  if (overview) {
    el("tab-overview").setAttribute("aria-current", "page");
    el("tab-comparison").removeAttribute("aria-current");
  } else {
    el("tab-comparison").setAttribute("aria-current", "page");
    el("tab-overview").removeAttribute("aria-current");
  }
}
function init() {
  renderMetrics();
  buildMap();
  renderTable();
  setDomain(selectedDomain);
  setTab(selectedTab);
  el("search-tree").addEventListener("input", renderTable);
  el("filter-tree").addEventListener("change", renderTable);
  el("sort-tree").addEventListener("change", renderTable);
  for (const radio of document.querySelectorAll('input[name="domain"]')) {
    radio.addEventListener("change", function () {
      if (radio.checked) setDomain(radio.value);
    });
  }
  el("tab-overview").addEventListener("click", function () { setTab("overview"); });
  el("tab-comparison").addEventListener("click", function () { setTab("comparison"); });
}
init();
