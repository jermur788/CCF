/* Canopy Ledger — Blind B interface prototype (CCF #870).
 *
 * PROTOTYPE — NOT THE REAL FORESTRY ENGINE.
 * All trees, measurements, growth and basal area below are deterministic MOCK
 * data generated in this file. Nothing is sent anywhere; there is no server.
 * The workflow (draft -> approve -> execute) only imitates the shape of the
 * real engine's rules so the interface can be evaluated.
 */
(function () {
  "use strict";

  // ---------- Constants ----------
  var STAND = 30;               // metres per side
  var GRID = 10;                // 10 x 10 jittered grid = 100 trees
  var HA = (STAND * STAND) / 10000;
  var MIN_SPAN = 5, MAX_SPAN = 40, FIT_SPAN = 33;

  // Placeholder list. The packet says the owner has chosen a predefined list
  // but does not give its wording, so these labels need owner confirmation.
  var REASONS = [
    { key: "thin", label: "Thinning — release a crop tree" },
    { key: "form", label: "Poor form (forked, leaning, coarse)" },
    { key: "damage", label: "Damage or disease" },
    { key: "wind", label: "Wind instability risk" },
    { key: "access", label: "Access / extraction route" },
    { key: "mature", label: "Mature — final harvest" },
    { key: "other", label: "Other (describe)…" }
  ];

  // ---------- Deterministic mock data ----------
  function mulberry32(seed) {
    return function () {
      seed |= 0; seed = (seed + 0x6d2b79f5) | 0;
      var t = Math.imul(seed ^ (seed >>> 15), 1 | seed);
      t = (t + Math.imul(t ^ (t >>> 7), 61 | t)) ^ t;
      return ((t ^ (t >>> 14)) >>> 0) / 4294967296;
    };
  }

  function pad3(n) { return String(n).padStart(3, "0"); }

  function makeTrees() {
    var rand = mulberry32(870);
    var cell = STAND / GRID;
    var list = [];
    for (var row = 0; row < GRID; row++) {
      for (var col = 0; col < GRID; col++) {
        var n = row * GRID + col + 1;
        var dbh = 14 + rand() * 26;               // 14–40 cm (mock)
        list.push({
          id: "T" + pad3(n),
          n: n,
          // IDs run west->east along rows starting at the north edge.
          east: (col + 0.5) * cell + (rand() - 0.5) * cell * 0.6,
          north: STAND - ((row + 0.5) * cell + (rand() - 0.5) * cell * 0.6),
          dbh: dbh,
          height: 4 + 0.62 * dbh + (rand() - 0.5) * 3, // mock allometry, not research-based
          felled: false,
          felledYear: null,
          felledPlan: null
        });
      }
    }
    return list;
  }

  // ---------- State ----------
  var state = {
    year: 1,
    trees: makeTrees(),
    selectedId: null,
    planSeq: 1,
    plan: null,
    history: [],
    view: { cx: STAND / 2, cy: STAND / 2, span: FIT_SPAN }
  };
  state.plan = newPlan();
  var byId = {};
  state.trees.forEach(function (t) { byId[t.id] = t; });

  function newPlan() {
    var p = { id: "P-" + pad3(state.planSeq), stage: "draft", orders: [] };
    state.planSeq += 1;
    return p;
  }

  function orderFor(treeId) {
    return state.plan.orders.find(function (o) { return o.treeId === treeId; }) || null;
  }

  function statusOf(t) {
    if (t.felled) return "felled";
    var o = orderFor(t.id);
    if (o && state.plan.stage === "approved") return "approved";
    if (o && state.plan.stage === "draft") return "pending";
    return "standing";
  }

  var STATUS_TEXT = {
    standing: "Standing",
    pending: "⚑ Pending order",
    approved: "✓ Approved for felling",
    felled: "✕ Stump"
  };

  // ---------- DOM helpers ----------
  var $ = function (id) { return document.getElementById(id); };
  var SVGNS = "http://www.w3.org/2000/svg";
  function svgEl(name, attrs) {
    var el = document.createElementNS(SVGNS, name);
    for (var k in attrs) el.setAttribute(k, attrs[k]);
    return el;
  }
  function svgY(north) { return STAND - north; }

  var toastTimer = null;
  function toast(msg, isError) {
    var el = $("toast");
    el.textContent = msg;
    el.className = "toast show" + (isError ? " error" : "");
    clearTimeout(toastTimer);
    toastTimer = setTimeout(function () { el.className = "toast"; }, 4200);
  }

  function log(kind, text) {
    state.history.push({ year: state.year, kind: kind, text: text });
    renderHistory();
  }

  // ---------- Map: grid and trees ----------
  function buildGrid() {
    var g = $("grid");
    // 5 m survey grid with metre labels along the south and west edges.
    for (var m = 0; m <= STAND; m += 5) {
      var cls = "grid-line major";
      g.appendChild(svgEl("line", { x1: m, y1: 0, x2: m, y2: STAND, "class": cls }));
      g.appendChild(svgEl("line", { x1: 0, y1: m, x2: STAND, y2: m, "class": cls }));
      var lx = svgEl("text", { x: m, y: STAND + 0.9, "class": "axis-label", "text-anchor": "middle" });
      lx.textContent = String(m);
      if (m === 0) lx.textContent = "0";
      g.appendChild(lx);
      var ly = svgEl("text", { x: -0.4, y: svgY(m) + 0.25, "class": "axis-label", "text-anchor": "end" });
      ly.textContent = m === 0 ? "" : String(m);
      g.appendChild(ly);
    }
  }

  function crownRadius(t) { return 0.45 + t.dbh * 0.022; } // mock: crown scales with DBH

  function buildTrees() {
    var host = $("trees");
    state.trees.forEach(function (t) {
      var g = svgEl("g", { "class": "tree", "data-id": t.id, transform: "translate(" + t.east.toFixed(3) + " " + svgY(t.north).toFixed(3) + ")" });
      var title = svgEl("title", {});
      g.appendChild(title);
      g.appendChild(svgEl("circle", { "class": "halo", r: crownRadius(t) + 0.45 }));
      g.appendChild(svgEl("circle", { "class": "ring", r: crownRadius(t) + 0.2 }));
      g.appendChild(svgEl("circle", { "class": "crown", r: crownRadius(t) }));
      g.appendChild(svgEl("circle", { "class": "bole", r: 0.12 }));
      var stump = svgEl("g", { "class": "stump-g" });
      stump.appendChild(svgEl("circle", { "class": "stump", r: Math.max(0.32, t.dbh / 70) }));
      stump.appendChild(svgEl("circle", { "class": "stump-ring", r: Math.max(0.2, t.dbh / 120) }));
      stump.appendChild(svgEl("circle", { "class": "stump-ring", r: 0.08 }));
      g.appendChild(stump);
      var label = svgEl("text", { "class": "tid", y: 0 });
      label.textContent = t.id.slice(1);
      g.appendChild(label);
      var glyph = svgEl("text", { "class": "glyph", y: -(crownRadius(t) + 0.55) });
      g.appendChild(glyph);
      host.appendChild(g);
      t.el = g;
    });
  }

  function renderTree(t) {
    var s = statusOf(t);
    var g = t.el;
    g.setAttribute("class", "tree " + s + (state.selectedId === t.id ? " selected" : ""));
    g.querySelector(".crown").style.display = t.felled ? "none" : "";
    g.querySelector(".bole").style.display = t.felled ? "none" : "";
    g.querySelector(".stump-g").style.display = t.felled ? "" : "none";
    g.querySelector(".halo").style.display = state.selectedId === t.id ? "" : "none";
    g.querySelector(".ring").style.display = (s === "pending" || s === "approved") ? "" : "none";
    g.querySelector(".glyph").textContent = s === "pending" ? "⚑" : s === "approved" ? "✓" : s === "felled" ? "✕" : "";
    g.querySelector("title").textContent = t.id + " — " + STATUS_TEXT[s].replace(/^[^A-Za-z]+/, "");
  }

  function renderAllTrees() { state.trees.forEach(renderTree); }

  // ---------- Map view: pan / zoom / fit ----------
  function clampView() {
    var v = state.view;
    v.span = Math.min(MAX_SPAN, Math.max(MIN_SPAN, v.span));
    v.cx = Math.min(STAND + 2, Math.max(-2, v.cx));
    v.cy = Math.min(STAND + 2, Math.max(-2, v.cy));
  }

  function applyView() {
    clampView();
    var v = state.view;
    $("map").setAttribute("viewBox", [v.cx - v.span / 2, v.cy - v.span / 2, v.span, v.span].map(function (n) { return n.toFixed(3); }).join(" "));
    $("zoom-readout").textContent = (FIT_SPAN / v.span).toFixed(1) + "×";
  }

  function pan(dir) {
    var step = state.view.span * 0.25;
    if (dir === "n") state.view.cy -= step;
    if (dir === "s") state.view.cy += step;
    if (dir === "e") state.view.cx += step;
    if (dir === "w") state.view.cx -= step;
    applyView();
  }

  function zoom(factor, focusX, focusY) {
    var v = state.view;
    var newSpan = Math.min(MAX_SPAN, Math.max(MIN_SPAN, v.span / factor));
    if (focusX !== undefined) {
      // Keep the point under the cursor still while zooming.
      var k = newSpan / v.span;
      v.cx = focusX + (v.cx - focusX) * k;
      v.cy = focusY + (v.cy - focusY) * k;
    }
    v.span = newSpan;
    applyView();
  }

  // Fit the whole stand. On wide maps, nudge it left so the floating
  // pan/zoom pad sits in empty margin rather than over the south-east trees.
  function fit() {
    var cx = STAND / 2;
    var svg = $("map"), tools = document.querySelector(".map-tools");
    var w = svg.clientWidth, h = svg.clientHeight;
    var floating = tools && getComputedStyle(tools).position === "absolute";
    if (w > h && floating) {
      var unitsPerPx = FIT_SPAN / h;
      var standPx = STAND / unitsPerPx;
      var freeRight = w - tools.offsetWidth - 24;      // usable width left of the pad
      if (freeRight >= standPx + 16) {
        var targetCentrePx = Math.max(standPx / 2 + 16, freeRight / 2);
        cx = STAND / 2 + (w / 2 - targetCentrePx) * unitsPerPx;
      }
    }
    state.view = { cx: cx, cy: STAND / 2, span: FIT_SPAN };
    applyView();
  }

  function ensureVisible(t) {
    var v = state.view, x = t.east, y = svgY(t.north), m = v.span * 0.12;
    var inside = x > v.cx - v.span / 2 + m && x < v.cx + v.span / 2 - m &&
                 y > v.cy - v.span / 2 + m && y < v.cy + v.span / 2 - m;
    if (!inside) { v.cx = x; v.cy = y; applyView(); }
  }

  function clientToMap(evt) {
    var svg = $("map");
    var pt = svg.createSVGPoint();
    pt.x = evt.clientX; pt.y = evt.clientY;
    return pt.matrixTransform(svg.getScreenCTM().inverse());
  }

  function wireMapPointer() {
    var svg = $("map");
    var drag = null;
    svg.addEventListener("pointerdown", function (e) {
      if (e.button !== 0) return;
      drag = { x: e.clientX, y: e.clientY, cx: state.view.cx, cy: state.view.cy, moved: false, id: e.pointerId };
    });
    svg.addEventListener("pointermove", function (e) {
      if (!drag) return;
      var dx = e.clientX - drag.x, dy = e.clientY - drag.y;
      if (!drag.moved && Math.hypot(dx, dy) < 5) return;
      if (!drag.moved) { drag.moved = true; svg.setPointerCapture(drag.id); svg.classList.add("dragging"); }
      var unitsPerPx = state.view.span / Math.min(svg.clientWidth, svg.clientHeight);
      state.view.cx = drag.cx - dx * unitsPerPx;
      state.view.cy = drag.cy - dy * unitsPerPx;
      applyView();
    });
    function end(e) {
      if (!drag) return;
      var wasDrag = drag.moved;
      drag = null;
      svg.classList.remove("dragging");
      if (wasDrag) return;
      var g = e.target.closest && e.target.closest(".tree");
      if (g) select(g.getAttribute("data-id"), { from: "map" });
    }
    svg.addEventListener("pointerup", end);
    svg.addEventListener("pointercancel", function () { drag = null; svg.classList.remove("dragging"); });
    svg.addEventListener("wheel", function (e) {
      e.preventDefault();
      var p = clientToMap(e);
      zoom(e.deltaY < 0 ? 1.2 : 1 / 1.2, p.x, p.y);
    }, { passive: false });

    svg.addEventListener("keydown", onMapKey);
  }

  // Arrow-key selection: pick the nearest tree roughly in the pressed direction.
  function neighbour(from, dir) {
    var vec = { ArrowUp: [0, 1], ArrowDown: [0, -1], ArrowLeft: [-1, 0], ArrowRight: [1, 0] }[dir];
    var best = null, bestScore = Infinity;
    state.trees.forEach(function (t) {
      if (t === from) return;
      var dx = t.east - from.east, dy = t.north - from.north;
      var along = dx * vec[0] + dy * vec[1];
      if (along <= 0.3) return;
      var across = Math.abs(dx * vec[1] - dy * vec[0]);
      if (across > along * 1.4) return;   // stay within a ~55° cone
      var score = along + across * 2;
      if (score < bestScore) { bestScore = score; best = t; }
    });
    return best;
  }

  function nearestToCentre() {
    var v = state.view, best = null, bd = Infinity;
    state.trees.forEach(function (t) {
      var d = Math.hypot(t.east - v.cx, svgY(t.north) - v.cy);
      if (d < bd) { bd = d; best = t; }
    });
    return best;
  }

  function onMapKey(e) {
    var k = e.key;
    if (k.indexOf("Arrow") === 0) {
      e.preventDefault();
      if (e.shiftKey) {
        pan({ ArrowUp: "n", ArrowDown: "s", ArrowLeft: "w", ArrowRight: "e" }[k]);
        return;
      }
      var cur = state.selectedId ? byId[state.selectedId] : null;
      var next = cur ? neighbour(cur, k) : nearestToCentre();
      if (next) select(next.id, { from: "keys" });
      else $("map-focus-readout").textContent = "No tree further in that direction.";
    } else if (k === "+" || k === "=") { e.preventDefault(); zoom(1.25); }
    else if (k === "-" || k === "_") { e.preventDefault(); zoom(1 / 1.25); }
    else if (k === "f" || k === "F" || k === "0") { e.preventDefault(); fit(); }
    else if (k === "Enter" || k === " ") {
      e.preventDefault();
      if (!state.selectedId) { var t = nearestToCentre(); if (t) select(t.id, { from: "keys" }); return; }
      var f = $("reason");
      if (!$("order-form").hidden) f.focus();
    } else if (k === "Escape") { select(null); }
  }

  // ---------- Selection ----------
  function select(id, opts) {
    opts = opts || {};
    var prev = state.selectedId;
    state.selectedId = id;
    if (prev && byId[prev]) renderTree(byId[prev]);
    if (id && byId[id]) {
      renderTree(byId[id]);
      ensureVisible(byId[id]);
      var t = byId[id];
      $("map-focus-readout").textContent = t.id + " · " + STATUS_TEXT[statusOf(t)].replace(/^[^A-Za-z]+/, "") + " · DBH " + t.dbh.toFixed(1) + " cm";
    } else {
      $("map-focus-readout").textContent = "";
    }
    renderTreeCard();
    renderTable();
  }

  // Accepts "T042", "t42", "42", " 042 ". Returns canonical ID or null.
  function parseId(raw) {
    var m = String(raw || "").trim().match(/^[tT]?\s*0*(\d{1,3})$/);
    if (!m) return null;
    var n = parseInt(m[1], 10);
    if (n < 1 || n > 100) return null;
    return "T" + pad3(n);
  }

  // ---------- Tree card + order form ----------
  function renderTreeCard() {
    var id = state.selectedId;
    var t = id ? byId[id] : null;
    $("tree-h").textContent = t ? "Tree " + t.id : "No tree selected";
    $("tree-empty-hint").hidden = !!t;
    $("tree-facts").hidden = !t;
    var form = $("order-form"), note = $("order-note");
    if (!t) { form.hidden = true; note.hidden = true; return; }

    var s = statusOf(t);
    $("f-status").innerHTML = "";
    var chip = document.createElement("span");
    chip.className = "status-chip " + s;
    chip.textContent = STATUS_TEXT[s];
    $("f-status").appendChild(chip);
    $("f-dbh").textContent = t.dbh.toFixed(1) + " cm";
    $("f-height").textContent = t.height.toFixed(1) + " m";
    $("f-pos").textContent = t.east.toFixed(1) + " E, " + t.north.toFixed(1) + " N";

    var o = orderFor(t.id);
    if (t.felled) {
      form.hidden = true; note.hidden = false;
      note.textContent = "Felled in year " + t.felledYear + " under " + t.felledPlan + ". A stump cannot be ordered again.";
    } else if (o) {
      form.hidden = true; note.hidden = false;
      note.textContent = "Already in " + state.plan.id + " (" + state.plan.stage + "): " + o.reasonText;
    } else if (state.plan.stage === "approved") {
      form.hidden = true; note.hidden = false;
      note.textContent = state.plan.id + " is approved and locked. Execute it before ordering more trees.";
    } else {
      form.hidden = false; note.hidden = true;
    }
  }

  function setFieldError(msg, field) {
    var m = $("reason-msg");
    m.textContent = msg || "";
    m.className = "field-msg" + (msg ? " error" : "");
    $("reason").removeAttribute("aria-invalid");
    $("reason-custom").removeAttribute("aria-invalid");
    if (field) field.setAttribute("aria-invalid", "true");
  }

  function onReasonChange() {
    var other = $("reason").value === "other";
    $("custom-wrap").hidden = !other;
    setFieldError("");
    if (other) $("reason-custom").focus();
  }

  // Validates and records one pending order. Each guard here is also a
  // "blocked command" the reviewer can trigger on purpose.
  function addOrder(e) {
    e.preventDefault();
    var t = state.selectedId ? byId[state.selectedId] : null;
    if (!t) { toast("Blocked: select a tree first.", true); return; }
    if (t.felled) { toast("Blocked: " + t.id + " is already a stump.", true); return; }
    if (state.plan.stage === "executed") {
      var old = state.plan.id;
      state.plan = newPlan();
      log("plan", "Started " + state.plan.id + " (previous " + old + " is closed).");
    }
    if (state.plan.stage === "approved") { toast("Blocked: " + state.plan.id + " is approved and locked.", true); return; }
    if (orderFor(t.id)) { toast("Blocked: " + t.id + " is already in the plan.", true); return; }

    var key = $("reason").value;
    if (!key) { setFieldError("Choose a felling reason.", $("reason")); $("reason").focus(); return; }
    var text;
    if (key === "other") {
      text = $("reason-custom").value.trim();
      if (text.length < 3) {
        setFieldError("Describe the reason (at least 3 characters). A blank Other reason is not allowed.", $("reason-custom"));
        $("reason-custom").focus();
        return;
      }
      text = "Other: " + text;
    } else {
      text = REASONS.find(function (r) { return r.key === key; }).label;
    }

    state.plan.orders.push({ treeId: t.id, reasonKey: key, reasonText: text });
    setFieldError("");
    $("reason").value = ""; $("reason-custom").value = ""; $("custom-wrap").hidden = true;
    log("order", t.id + " added to " + state.plan.id + " — " + text);
    toast(t.id + " added to " + state.plan.id + ". Nothing is felled yet.");
    renderTree(t); renderTreeCard(); renderPlan(); renderTable();
  }

  function removeOrder(treeId) {
    if (state.plan.stage !== "draft") { toast("Blocked: orders can only be removed while the plan is a draft.", true); return; }
    state.plan.orders = state.plan.orders.filter(function (o) { return o.treeId !== treeId; });
    log("order", treeId + " removed from " + state.plan.id + ".");
    renderTree(byId[treeId]); renderTreeCard(); renderPlan(); renderTable();
  }

  // ---------- Approve / Execute ----------
  function approve() {
    var p = state.plan;
    if (p.stage === "approved") { toast("Blocked: " + p.id + " is already approved.", true); return; }
    if (p.stage === "executed") { toast("Blocked: " + p.id + " was already executed. Add an order to start a new plan.", true); return; }
    if (p.orders.length === 0) { toast("Blocked: the plan has no orders to approve.", true); return; }
    var before = countFelled();
    p.stage = "approved";
    // Approval fells nothing; check that explicitly so the prototype proves the rule.
    var felledNow = countFelled() - before;
    log("approve", p.id + " approved with " + p.orders.length + " order(s). Trees felled by approval: " + felledNow + ".");
    toast(p.id + " approved. 0 trees felled — use Execute to fell.");
    renderAll();
  }

  function openExecute() {
    var p = state.plan;
    if (p.stage === "executed") { toast("Blocked: " + p.id + " has already been executed. Duplicate execution refused; nothing felled.", true); log("blocked", "Duplicate execution of " + p.id + " refused."); return; }
    if (p.stage !== "approved") { toast("Blocked: approve the plan before executing it.", true); return; }
    var ids = p.orders.map(function (o) { return o.treeId; });
    $("confirm-body").textContent = "Fell " + ids.length + " tree(s) under " + p.id + ": " + ids.join(", ") + ". In this prototype the result cannot be undone.";
    $("confirm-check").checked = false;
    $("confirm-go").disabled = true;
    $("confirm-dialog").showModal();
    $("confirm-check").focus();
  }

  function executePlan() {
    var p = state.plan;
    // Guarded again here, not only in the button handler, so a second call is a no-op.
    if (p.stage !== "approved") return;
    var felled = [];
    p.orders.forEach(function (o) {
      var t = byId[o.treeId];
      if (t.felled) return;
      t.felled = true; t.felledYear = state.year; t.felledPlan = p.id;
      felled.push(t.id);
    });
    p.stage = "executed";
    log("execute", p.id + " executed: " + felled.length + " tree(s) felled → stumps: " + felled.join(", ") + ".");
    toast(felled.length + " tree(s) felled. Standing count is now " + (state.trees.length - countFelled()) + ".");
    renderAll();
    felled.forEach(function (id) {
      var g = byId[id].el;
      g.classList.add("just-felled");
      setTimeout(function () { g.classList.remove("just-felled"); }, 1000);
    });
    bump("stat-standing"); bump("stat-felled");
  }

  function countFelled() { return state.trees.filter(function (t) { return t.felled; }).length; }

  function bump(id) {
    var el = $(id); el.classList.remove("bump"); void el.offsetWidth; el.classList.add("bump");
  }

  // ---------- Year +1 (fake growth) ----------
  function advanceYear() {
    var rand = mulberry32(870 * 1000 + state.year);
    state.trees.forEach(function (t) {
      if (t.felled) return;
      var g = 0.25 + rand() * 0.55;          // mock DBH growth, cm
      t.dbh += g;
      t.height += 0.15 + g * 0.5;            // mock height growth, m
    });
    state.year += 1;
    var pending = state.plan.stage !== "executed" && state.plan.orders.length ? " " + state.plan.id + " still " + state.plan.stage + "." : "";
    log("year", "Advanced to year " + state.year + " (mock growth applied)." + pending);
    toast("Year " + state.year + ". Mock growth applied; no trees were felled.");
    bump("stat-year");
    renderAll();
  }

  // ---------- Rendering of panels ----------
  function renderStats() {
    var felled = countFelled();
    $("stat-year").textContent = state.year;
    $("stat-standing").textContent = state.trees.length - felled;
    $("stat-felled").textContent = felled;
    var ba = state.trees.reduce(function (s, t) { return t.felled ? s : s + Math.PI * Math.pow(t.dbh / 200, 2); }, 0) / HA;
    $("stat-ba").textContent = ba.toFixed(1) + " m²/ha";
  }

  function setBlocked(btn, blocked, why) {
    // aria-disabled keeps the button focusable/clickable so a blocked click explains itself.
    btn.setAttribute("aria-disabled", blocked ? "true" : "false");
    btn.title = blocked ? why : "";
  }

  function renderPlan() {
    var p = state.plan;
    $("plan-id").textContent = "Plan " + p.id;
    $("plan-stage").textContent = { draft: "Draft", approved: "Approved · locked", executed: "Executed · closed" }[p.stage];
    $("plan-count").textContent = p.stage === "executed" ? 0 : p.orders.length;
    var order = ["draft", "approved", "executed"], idx = order.indexOf(p.stage);
    document.querySelectorAll(".stepper li").forEach(function (li, i) {
      li.className = i < idx ? "done" : i === idx ? "current" : "";
      if (i === idx) li.setAttribute("aria-current", "step"); else li.removeAttribute("aria-current");
    });
    var ul = $("orders");
    ul.innerHTML = "";
    p.orders.forEach(function (o) {
      var li = document.createElement("li");
      var b = document.createElement("button");
      b.className = "oid"; b.type = "button"; b.textContent = o.treeId;
      b.setAttribute("aria-label", "Select " + o.treeId);
      b.addEventListener("click", function () { select(o.treeId); });
      var r = document.createElement("span"); r.className = "oreason"; r.textContent = o.reasonText;
      li.appendChild(b); li.appendChild(r);
      if (p.stage === "draft") {
        var x = document.createElement("button");
        x.className = "remove"; x.type = "button"; x.textContent = "✕";
        x.setAttribute("aria-label", "Remove " + o.treeId + " from plan");
        x.addEventListener("click", function () { removeOrder(o.treeId); });
        li.appendChild(x);
      } else {
        var tag = document.createElement("span"); tag.className = "micro";
        tag.textContent = p.stage === "approved" ? "✓ locked" : "✕ felled";
        li.appendChild(tag);
      }
      ul.appendChild(li);
    });
    $("orders-empty").hidden = p.orders.length > 0;
    setBlocked($("btn-approve"), p.stage !== "draft" || p.orders.length === 0,
      p.stage !== "draft" ? "Plan is not a draft" : "No orders to approve");
    setBlocked($("btn-execute"), p.stage !== "approved",
      p.stage === "executed" ? "Already executed" : "Approve first");
  }

  function renderHistory() {
    var ol = $("history");
    ol.innerHTML = "";
    if (!state.history.length) {
      var li0 = document.createElement("li"); li0.textContent = "Nothing has happened yet."; ol.appendChild(li0); return;
    }
    state.history.slice().reverse().forEach(function (h) {
      var li = document.createElement("li");
      var y = document.createElement("span"); y.className = "hy"; y.textContent = "Y" + h.year;
      var k = document.createElement("span"); k.className = "hkind"; k.textContent = h.kind;
      li.appendChild(y); li.appendChild(k); li.appendChild(document.createTextNode(h.text));
      ol.appendChild(li);
    });
  }

  function renderTable() {
    var q = $("list-filter").value.trim().toLowerCase();
    var body = $("tree-rows");
    body.innerHTML = "";
    state.trees.forEach(function (t) {
      var s = statusOf(t), label = STATUS_TEXT[s].replace(/^[^A-Za-z]+/, "");
      if (q && t.id.toLowerCase().indexOf(q) < 0 && label.toLowerCase().indexOf(q) < 0 && s.indexOf(q) < 0 && !(q === "stump" && s === "felled")) return;
      var tr = document.createElement("tr");
      if (t.id === state.selectedId) tr.className = "is-selected";
      var td = document.createElement("td"), b = document.createElement("button");
      b.type = "button"; b.textContent = t.id;
      b.addEventListener("click", function () { select(t.id); });
      td.appendChild(b); tr.appendChild(td);
      [STATUS_TEXT[s], t.felled ? "—" : t.dbh.toFixed(1), t.felled ? "—" : t.height.toFixed(1)].forEach(function (v) {
        var c = document.createElement("td"); c.textContent = v; tr.appendChild(c);
      });
      body.appendChild(tr);
    });
  }

  function renderAll() { renderStats(); renderAllTrees(); renderTreeCard(); renderPlan(); renderTable(); }

  // ---------- Tabs ----------
  function wireTabs() {
    var tabs = Array.prototype.slice.call(document.querySelectorAll('[role="tab"]'));
    function activate(tab, focus) {
      tabs.forEach(function (t) {
        var on = t === tab;
        t.setAttribute("aria-selected", on ? "true" : "false");
        t.tabIndex = on ? 0 : -1;
        $(t.getAttribute("aria-controls")).hidden = !on;
      });
      if (focus) tab.focus();
    }
    tabs.forEach(function (tab, i) {
      tab.addEventListener("click", function () { activate(tab); });
      tab.addEventListener("keydown", function (e) {
        var j = null;
        if (e.key === "ArrowRight") j = (i + 1) % tabs.length;
        if (e.key === "ArrowLeft") j = (i - 1 + tabs.length) % tabs.length;
        if (e.key === "Home") j = 0;
        if (e.key === "End") j = tabs.length - 1;
        if (j !== null) { e.preventDefault(); activate(tabs[j], true); }
      });
    });
  }

  // ---------- Wiring ----------
  function init() {
    var sel = $("reason");
    REASONS.forEach(function (r) {
      var o = document.createElement("option"); o.value = r.key; o.textContent = r.label; sel.appendChild(o);
    });
    sel.addEventListener("change", onReasonChange);
    $("reason-custom").addEventListener("input", function () { if ($("reason-custom").value.trim()) setFieldError(""); });
    $("order-form").addEventListener("submit", addOrder);

    $("find-form").addEventListener("submit", function (e) {
      e.preventDefault();
      var raw = $("tree-search").value, id = parseId(raw), msg = $("find-msg");
      if (!id) {
        msg.className = "field-msg error";
        msg.textContent = raw.trim() ? "No tree “" + raw.trim() + "”. Valid IDs are T001–T100." : "Type a tree ID, e.g. T042 or 42.";
        $("tree-search").setAttribute("aria-invalid", "true");
        return;
      }
      $("tree-search").removeAttribute("aria-invalid");
      msg.className = "field-msg ok";
      msg.textContent = "Selected " + id + ".";
      select(id);
    });

    document.querySelectorAll("[data-pan]").forEach(function (b) {
      b.addEventListener("click", function () { pan(b.getAttribute("data-pan")); });
    });
    $("btn-fit").addEventListener("click", fit);
    $("btn-zoom-in").addEventListener("click", function () { zoom(1.25); });
    $("btn-zoom-out").addEventListener("click", function () { zoom(1 / 1.25); });
    $("btn-approve").addEventListener("click", approve);
    $("btn-execute").addEventListener("click", openExecute);
    $("btn-year").addEventListener("click", advanceYear);
    $("list-filter").addEventListener("input", renderTable);

    $("confirm-check").addEventListener("change", function () { $("confirm-go").disabled = !$("confirm-check").checked; });
    // The dialog buttons act directly instead of waiting for the dialog's
    // "close" event, which did not fire reliably in one test browser.
    function finishDialog(go) {
      var d = $("confirm-dialog");
      if (d.open) d.close();
      if (go && $("confirm-check").checked) executePlan();
      else toast("Execution cancelled. Nothing felled.");
      $("btn-execute").focus();
    }
    $("confirm-go").addEventListener("click", function () { finishDialog(true); });
    $("confirm-cancel").addEventListener("click", function () { finishDialog(false); });
    $("confirm-dialog").addEventListener("cancel", function (e) { e.preventDefault(); finishDialog(false); }); // Esc key

    buildGrid();
    buildTrees();
    wireMapPointer();
    wireTabs();
    fit();
    renderAll();
    renderHistory();

    // Small hook for manual console testing; not used by the UI.
    window.__canopyLedger = { state: state, executePlan: executePlan, approve: approve, parseId: parseId };
  }

  document.addEventListener("DOMContentLoaded", init);
})();
