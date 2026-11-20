/* CCF Multi #869 — Blind A: independent offline UI-only demonstration.
   This is NOT the protected C# forestry engine. No fetch, storage, saves or external data. */
"use strict";

(() => {
  const $ = (id) => document.getElementById(id);
  const COUNT = 100;
  const SIZE_METRES = 30;
  const PAN_STEP = 2.5;

  function makeTrees() {
    return Array.from({ length: COUNT }, (_, index) => {
      const n = index + 1;
      const row = Math.floor(index / 10);
      const column = index % 10;
      const jitterX = (((n * 17) % 9) - 4) * 0.14;
      const jitterY = (((n * 13) % 9) - 4) * 0.14;
      return {
        id: "S" + String(n).padStart(3, "0"),
        x: 1.5 + column * 3 + jitterX,
        y: 1.5 + row * 3 + jitterY,
        dbh: 15 + ((n * 7) % 140) / 10,
        height: 8 + ((n * 11) % 120) / 10,
        felledYear: null
      };
    });
  }

  let trees = makeTrees();
  let selectedId = "S045";
  let pending = new Map();
  let approved = null;
  let year = 1;
  let planSerial = 0;
  let executedPlans = new Set();
  let events = [{ year: 1, text: "Mock stand opened. All 100 synthetic trees standing." }];
  const view = { x: 0, y: 0, zoom: 1 };
  let drag = null;

  const canvas = $("forestMap");
  const ctx = canvas.getContext("2d");

  function treeById(id) { return trees.find((tree) => tree.id === id); }
  function isFelled(tree) { return tree.felledYear !== null; }
  function stateOf(tree) {
    if (isFelled(tree)) return "Felled";
    if (approved && pending.has(tree.id)) return "Approved";
    if (pending.has(tree.id)) return "Pending";
    return "Standing";
  }
  function record(message) {
    events.unshift({ year, text: message });
    events = events.slice(0, 80);
  }
  function notify(message, tone = "") {
    const el = $("feedback");
    el.textContent = message;
    el.className = "feedback" + (tone ? " " + tone : "");
  }

  function renderRoster() {
    const container = $("treeList");
    const oldScroll = container.scrollTop;
    const needle = $("treeSearch").value.trim().toUpperCase();
    const filtered = trees.filter((tree) => tree.id.includes(needle) || stateOf(tree).toUpperCase().includes(needle));
    $("matchCount").textContent = filtered.length + " shown";
    const fragment = document.createDocumentFragment();

    for (const tree of filtered) {
      const state = stateOf(tree);
      const button = document.createElement("button");
      button.type = "button";
      button.className = "tree-row";
      button.dataset.id = tree.id;
      button.setAttribute("aria-pressed", String(selectedId === tree.id));
      button.setAttribute("aria-label", tree.id + ", " + state + ", " + tree.dbh.toFixed(1) + " centimetres DBH, " + tree.height.toFixed(1) + " metres height");

      const symbol = document.createElement("span");
      symbol.className = "tree-symbol " + state.toLowerCase();
      symbol.setAttribute("aria-hidden", "true");
      symbol.textContent = state === "Standing" ? "▲" : state === "Pending" ? "!" : state === "Approved" ? "✓" : "◎";

      const nameWrap = document.createElement("span");
      const name = document.createElement("span");
      name.className = "id";
      name.textContent = tree.id;
      const subtitle = document.createElement("span");
      subtitle.className = "status";
      subtitle.textContent = state;
      nameWrap.append(name, subtitle);

      const location = document.createElement("span");
      location.className = "location";
      location.textContent = "E" + tree.x.toFixed(0) + " N" + tree.y.toFixed(0);
      button.append(symbol, nameWrap, location);
      button.addEventListener("click", () => selectTree(tree.id, true));
      fragment.append(button);
    }

    if (filtered.length === 0) {
      const empty = document.createElement("p");
      empty.className = "hint";
      empty.style.padding = "10px";
      empty.textContent = "No matching trees. Enter S001–S100 to find an exact ID.";
      fragment.append(empty);
    }
    container.replaceChildren(fragment);
    container.scrollTop = oldScroll;
  }

  function renderSelected() {
    const tree = treeById(selectedId);
    const state = stateOf(tree);
    $("selectedId").textContent = tree.id;
    $("selectedDbh").textContent = tree.dbh.toFixed(1) + " cm";
    $("selectedHeight").textContent = tree.height.toFixed(1) + " m";
    $("selectedGrid").textContent = "E " + tree.x.toFixed(1) + " / N " + tree.y.toFixed(1) + " m";
    $("selectedStatus").textContent = state === "Felled" ? "Felled · stump" : state;
    $("selectedStatus").className = "status-pill " + state.toLowerCase();
    $("addOrder").disabled = isFelled(tree) || pending.has(tree.id) || approved !== null;
    $("addOrder").title = isFelled(tree) ? "Cannot plan an already felled tree" :
      pending.has(tree.id) ? "This tree already has an order" :
      approved ? "Withdraw approval before changing the plan" : "Record a reason for this selected tree";
  }

  function renderPlan() {
    const count = pending.size;
    $("orderCount").textContent = count + (count === 1 ? " order" : " orders");
    $("planState").textContent = approved ? "APPROVED · NOT EXECUTED" : "DRAFT";
    $("planState").className = "plan-state" + (approved ? " approved" : "");
    $("approvePlan").disabled = count === 0 || approved !== null;
    $("executePlan").disabled = approved === null;
    $("revisePlan").hidden = approved === null;
    $("approvePlan").textContent = approved ? "1 · Approved — 0 felled" : "1 · Approve (0 felled)";
    $("executePlan").textContent = approved ? "2 · Execute " + approved.ids.length + " approved order" + (approved.ids.length === 1 ? "" : "s") : "2 · Execute approved work";

    const list = $("orderList");
    const fragment = document.createDocumentFragment();
    if (count === 0) {
      const empty = document.createElement("li");
      empty.className = "empty";
      empty.textContent = "No tree orders yet.";
      fragment.append(empty);
    }
    for (const [id, reason] of pending) {
      const item = document.createElement("li");
      const idNode = document.createElement("span");
      idNode.className = "order-id";
      idNode.textContent = id;
      const reasonNode = document.createElement("span");
      reasonNode.className = "order-reason";
      reasonNode.textContent = reason;
      item.append(idNode, reasonNode);
      if (!approved) {
        const remove = document.createElement("button");
        remove.className = "remove-order";
        remove.type = "button";
        remove.textContent = "Remove";
        remove.setAttribute("aria-label", "Remove pending order for " + id);
        remove.addEventListener("click", () => {
          if (approved || !pending.has(id)) {
            notify("Cannot remove an order after approval. Withdraw approval first.", "error");
            return;
          }
          pending.delete(id);
          record("Draft order removed for " + id + ".");
          notify(id + " removed from draft plan.");
          renderAll();
        });
        item.append(remove);
      }
      fragment.append(item);
    }
    list.replaceChildren(fragment);
  }

  function renderHistory() {
    $("yearValue").textContent = String(year);
    const list = $("historyList");
    const fragment = document.createDocumentFragment();
    for (const event of events) {
      const item = document.createElement("li");
      const prefix = document.createElement("span");
      prefix.className = "event-year";
      prefix.textContent = "Y" + event.year + " · ";
      item.append(prefix, document.createTextNode(event.text));
      fragment.append(item);
    }
    list.replaceChildren(fragment);
  }

  function renderCounts() {
    $("standingCount").textContent = String(trees.filter((tree) => !isFelled(tree)).length);
    $("felledCount").textContent = String(trees.filter(isFelled).length);
    $("plannedCount").textContent = String(pending.size);
  }

  function renderAll() {
    renderRoster();
    renderSelected();
    renderPlan();
    renderHistory();
    renderCounts();
    paintMap();
  }

  function selectTree(id, restoreRosterFocus = false) {
    const tree = treeById(id);
    if (!tree) {
      notify("Invalid tree ID. Choose S001–S100.", "error");
      return false;
    }
    selectedId = id;
    renderAll();
    if (restoreRosterFocus) {
      const row = Array.from($("treeList").querySelectorAll("button")).find((button) => button.dataset.id === id);
      if (row) row.focus({ preventScroll: true });
    }
    notify(tree.id + " selected — " + stateOf(tree) + ".");
    return true;
  }

  function selectExact(rawInput) {
    const text = rawInput.trim().toUpperCase();
    const match = /^(?:S)?(\d{1,3})$/.exec(text);
    if (!match) {
      notify("Enter a complete ID such as S045 (S001–S100).", "error");
      return;
    }
    const number = Number(match[1]);
    if (!Number.isInteger(number) || number < 1 || number > COUNT) {
      notify("Tree ID out of range. Valid IDs are S001–S100.", "error");
      return;
    }
    const id = "S" + String(number).padStart(3, "0");
    $("treeSearch").value = id;
    selectTree(id);
    const row = Array.from($("treeList").querySelectorAll("button")).find((button) => button.dataset.id === id);
    if (row) row.scrollIntoView({ block: "nearest" });
  }

  function currentReason() {
    const kind = $("reasonSelect").value;
    if (!kind) return { error: "Choose a predefined felling reason, or Other / custom." };
    if (kind !== "Other") return { value: kind };
    const custom = $("reasonCustom").value.trim().replace(/\s+/g, " ");
    if (!custom) return { error: "Other / custom requires a non-empty written reason." };
    if (custom.length > 140) return { error: "Custom reason must be 140 characters or fewer." };
    return { value: "Other: " + custom };
  }

  function addOrder() {
    const tree = treeById(selectedId);
    if (approved) { notify("Plan is approved. Withdraw approval before editing.", "error"); return; }
    if (!tree || isFelled(tree)) { notify("Cannot order felling of an invalid or already felled tree.", "error"); return; }
    if (pending.has(tree.id)) { notify("Duplicate order blocked: " + tree.id + " is already in the plan.", "error"); return; }
    const reason = currentReason();
    if (reason.error) { notify(reason.error, "error"); return; }
    pending.set(tree.id, reason.value);
    record("Draft felling reason recorded for " + tree.id + ": " + reason.value + ".");
    $("reasonSelect").value = "";
    $("reasonCustom").value = "";
    $("customWrap").hidden = true;
    notify("Draft order added for " + tree.id + ". No tree has been felled.", "success");
    renderAll();
  }

  function approvePlan() {
    if (approved) { notify("Already approved. Execution is a separate action.", "error"); return; }
    if (pending.size === 0) { notify("Cannot approve an empty Work Plan.", "error"); return; }
    const ids = [...pending.keys()];
    if (ids.some((id) => !treeById(id) || isFelled(treeById(id)) || !pending.get(id))) {
      notify("Approval refused: invalid, felled, or reasonless tree order.", "error"); return;
    }
    approved = { id: "DEMO-" + year + "-" + (++planSerial), year, ids };
    record("Work Plan " + approved.id + " approved for " + ids.length + " tree" + (ids.length === 1 ? "" : "s") + ". Fell count unchanged.");
    notify("Approved " + ids.length + " order" + (ids.length === 1 ? "" : "s") + ". ZERO trees felled. Execute separately.", "success");
    renderAll();
  }

  function withdrawApproval() {
    if (!approved) { notify("No approval exists to withdraw.", "error"); return; }
    record("Approval withdrawn from " + approved.id + "; orders returned to draft.");
    approved = null;
    notify("Approval withdrawn. You may now revise the draft plan.");
    renderAll();
  }

  function executePlan() {
    if (!approved) { notify("Execution blocked: approve a non-empty Work Plan first.", "error"); return; }
    const receipt = approved;
    if (executedPlans.has(receipt.id)) {
      notify("Duplicate execution blocked: " + receipt.id + " already executed.", "error"); return;
    }
    const current = [...pending.keys()];
    const valid = receipt.year === year &&
      receipt.ids.length > 0 &&
      current.length === receipt.ids.length &&
      receipt.ids.every((id, index) => {
        const tree = treeById(id);
        return id === current[index] && tree && !isFelled(tree) && !!pending.get(id);
      });
    if (!valid) {
      notify("Execution refused: approved order IDs/reasons no longer match current state.", "error"); return;
    }
    const answer = window.confirm("DEMO ONLY: execute " + receipt.ids.length + " approved felling order" +
      (receipt.ids.length === 1 ? "" : "s") + "? This replaces tree symbols with stumps in the mock map.");
    if (!answer) { notify("Execution cancelled — stand remains unchanged."); return; }

    for (const id of receipt.ids) treeById(id).felledYear = year;
    executedPlans.add(receipt.id);
    pending.clear();
    approved = null;
    record("Executed " + receipt.id + ": " + receipt.ids.length + " mock fell" +
      (receipt.ids.length === 1 ? "" : "s") + "; stumps displayed. No real forestry model invoked.");
    notify(receipt.ids.length + " DEMO tree" + (receipt.ids.length === 1 ? "" : "s") + " felled. Stumps and counts updated. Re-execution blocked.", "success");
    renderAll();
  }

  function advanceYear() {
    if (pending.size || approved) {
      notify("Year advance blocked while orders are pending or approved. Execute or remove the draft orders first.", "error");
      return;
    }
    year += 1;
    record("Mock year advanced. No tree growth, recruitment or ecological calculation has run.");
    notify("Demo timeline advanced to Year " + year + ". Tree measurements are unchanged; no real simulation.", "success");
    renderAll();
  }

  function resetDemo() {
    if (!window.confirm("Reset the entire in-memory demo? All orders, stumps and history will be cleared.")) {
      notify("Reset cancelled."); return;
    }
    trees = makeTrees();
    selectedId = "S045";
    pending = new Map();
    approved = null;
    year = 1;
    planSerial = 0;
    executedPlans = new Set();
    events = [{ year: 1, text: "Mock stand reset. 100 synthetic Sitka standing." }];
    view.x = 0; view.y = 0; view.zoom = 1;
    $("treeSearch").value = "";
    $("reasonSelect").value = "";
    $("reasonCustom").value = "";
    $("customWrap").hidden = true;
    notify("Deterministic demo reset to 100 standing trees, Year 1.", "success");
    renderAll();
  }

  function dimensions() {
    const rect = canvas.getBoundingClientRect();
    const w = rect.width || 1, h = rect.height || 1;
    const scale = Math.max(0.1, Math.min((w - 50) / SIZE_METRES, (h - 50) / SIZE_METRES)) * view.zoom;
    return { w, h, scale };
  }
  function worldToScreen(x, y, dim) {
    return {
      x: dim.w / 2 + (x - SIZE_METRES / 2 + view.x) * dim.scale,
      y: dim.h / 2 - (y - SIZE_METRES / 2 + view.y) * dim.scale
    };
  }
  function paintMap() {
    if (!ctx) return;
    const dim = dimensions();
    if (dim.w < 2 || dim.h < 2) return;
    const ratio = Math.min(window.devicePixelRatio || 1, 3);
    const pxW = Math.round(dim.w * ratio), pxH = Math.round(dim.h * ratio);
    if (canvas.width !== pxW || canvas.height !== pxH) {
      canvas.width = pxW; canvas.height = pxH;
    }
    ctx.setTransform(ratio, 0, 0, ratio, 0, 0);
    ctx.clearRect(0, 0, dim.w, dim.h);

    const ground = ctx.createLinearGradient(0, 0, dim.w, dim.h);
    ground.addColorStop(0, "#e5ecdc");
    ground.addColorStop(1, "#c5d7c1");
    ctx.fillStyle = ground;
    ctx.fillRect(0, 0, dim.w, dim.h);

    // Authored survey-paper contours, illustrative and not terrain data.
    ctx.strokeStyle = "rgba(60, 104, 81, .17)";
    ctx.lineWidth = 1.2;
    for (let i = -2; i < 9; i++) {
      const offset = i * 71;
      ctx.beginPath();
      ctx.moveTo(-70, offset - 28);
      ctx.bezierCurveTo(dim.w * .3, offset + 85, dim.w * .63, offset - 107, dim.w + 70, offset + 12);
      ctx.stroke();
    }
    const southWest = worldToScreen(0, 0, dim);
    const northEast = worldToScreen(SIZE_METRES, SIZE_METRES, dim);
    const left = southWest.x, top = northEast.y;
    const side = SIZE_METRES * dim.scale;

    ctx.save();
    ctx.fillStyle = "rgba(222, 233, 204, .60)";
    ctx.fillRect(left, top, side, side);
    ctx.beginPath();
    ctx.rect(left, top, side, side);
    ctx.clip();
    for (let metres = 0; metres <= SIZE_METRES; metres += 5) {
      const vertical = worldToScreen(metres, 0, dim).x;
      const horizontal = worldToScreen(0, metres, dim).y;
      ctx.beginPath();
      ctx.strokeStyle = metres % 10 === 0 ? "rgba(37, 83, 68, .23)" : "rgba(37, 83, 68, .12)";
      ctx.lineWidth = metres % 10 === 0 ? 1.1 : .7;
      ctx.moveTo(vertical, top); ctx.lineTo(vertical, top + side);
      ctx.moveTo(left, horizontal); ctx.lineTo(left + side, horizontal);
      ctx.stroke();
    }
    ctx.restore();
    ctx.strokeStyle = "#557967";
    ctx.lineWidth = 2;
    ctx.strokeRect(left, top, side, side);
    ctx.fillStyle = "#335b4e";
    ctx.font = "700 10px Trebuchet MS, sans-serif";
    for (let metres = 0; metres <= SIZE_METRES; metres += 10) {
      const x = worldToScreen(metres, 0, dim).x;
      if (x >= 0 && x <= dim.w) ctx.fillText(String(metres), x - 5, Math.min(dim.h - 5, southWest.y + 16));
    }

    // Draw in ID order, then selected label last for a predictable highlight.
    for (const tree of trees) drawTree(tree, dim);
    const tree = treeById(selectedId);
    if (tree) drawSelection(tree, dim);
    $("zoomReadout").textContent = Math.round(view.zoom * 100) + "% view";
  }
  function drawTree(tree, dim) {
    const p = worldToScreen(tree.x, tree.y, dim);
    if (p.x < -22 || p.y < -22 || p.x > dim.w + 22 || p.y > dim.h + 22) return;
    const state = stateOf(tree);
    ctx.save();
    ctx.translate(p.x, p.y);

    if (state === "Felled") {
      ctx.fillStyle = "#ead3ba";
      ctx.strokeStyle = "#75442b";
      ctx.lineWidth = 2.1;
      ctx.beginPath(); ctx.arc(0, 0, 8.8, 0, Math.PI * 2); ctx.fill(); ctx.stroke();
      ctx.lineWidth = 1.2;
      ctx.beginPath(); ctx.arc(0, 0, 4.4, 0, Math.PI * 2); ctx.stroke();
      ctx.beginPath(); ctx.moveTo(-3.3, -3.3); ctx.lineTo(3.3, 3.3); ctx.stroke();
    } else if (state === "Pending") {
      ctx.fillStyle = "#ffd49a"; ctx.strokeStyle = "#81360e"; ctx.lineWidth = 2;
      ctx.beginPath(); ctx.arc(0, 0, 10, 0, Math.PI * 2); ctx.fill(); ctx.stroke();
      ctx.fillStyle = "#63310f"; ctx.font = "800 14px Arial"; ctx.textAlign = "center"; ctx.textBaseline = "middle"; ctx.fillText("!", 0, 1);
    } else if (state === "Approved") {
      ctx.fillStyle = "#255970"; ctx.strokeStyle = "#fbf8e7"; ctx.lineWidth = 2;
      ctx.beginPath(); ctx.moveTo(0, -11); ctx.lineTo(11, 0); ctx.lineTo(0, 11); ctx.lineTo(-11, 0); ctx.closePath(); ctx.fill(); ctx.stroke();
      ctx.fillStyle = "#fff"; ctx.font = "bold 12px Arial"; ctx.textAlign = "center"; ctx.textBaseline = "middle"; ctx.fillText("✓", 0, 1);
    } else {
      ctx.fillStyle = "#2e6854"; ctx.strokeStyle = "#eff8d9"; ctx.lineWidth = 2;
      ctx.beginPath(); ctx.arc(0, 0, 9.5, 0, Math.PI * 2); ctx.fill(); ctx.stroke();
      ctx.fillStyle = "#ecf5d8";
      ctx.beginPath(); ctx.moveTo(0, -6); ctx.lineTo(5.5, 4); ctx.lineTo(-5.5, 4); ctx.closePath(); ctx.fill();
    }
    ctx.restore();
  }
  function drawSelection(tree, dim) {
    const p = worldToScreen(tree.x, tree.y, dim);
    if (p.x < -25 || p.x > dim.w + 25 || p.y < -25 || p.y > dim.h + 25) return;
    ctx.save();
    ctx.strokeStyle = "#6e2e17"; ctx.lineWidth = 3;
    ctx.beginPath(); ctx.arc(p.x, p.y, 16, 0, Math.PI * 2); ctx.stroke();
    const bubbleWidth = 52;
    const x = Math.min(Math.max(4, p.x + 17), dim.w - bubbleWidth - 4);
    const y = Math.max(4, p.y - 30);
    ctx.fillStyle = "#173e39";
    ctx.fillRect(x, y, bubbleWidth, 23);
    ctx.fillStyle = "#fffceb";
    ctx.font = "bold 12px Trebuchet MS, sans-serif";
    ctx.textAlign = "center"; ctx.textBaseline = "middle";
    ctx.fillText(tree.id, x + bubbleWidth / 2, y + 12);
    ctx.restore();
  }

  function navigate(dx, dy) { view.x += dx; view.y += dy; paintMap(); }
  function zoomBy(multiplier) {
    view.zoom = Math.max(.65, Math.min(3.5, Math.round(view.zoom * multiplier * 100) / 100));
    paintMap();
  }
  function fit() { view.x = 0; view.y = 0; view.zoom = 1; paintMap(); }

  function findTreeAt(clientX, clientY) {
    const rect = canvas.getBoundingClientRect();
    const x = clientX - rect.left, y = clientY - rect.top;
    const dim = dimensions();
    let nearest = null, nearestDistance = Infinity;
    for (const tree of trees) {
      const p = worldToScreen(tree.x, tree.y, dim);
      const distance = Math.hypot(p.x - x, p.y - y);
      if (distance < nearestDistance) { nearestDistance = distance; nearest = tree; }
    }
    if (nearest && nearestDistance <= 16) selectTree(nearest.id);
    else notify("No tree at that point. Click a symbol or use exact-ID search.", "error");
  }

  canvas.addEventListener("pointerdown", (event) => {
    if (event.button !== 0 && event.pointerType === "mouse") return;
    drag = { id: event.pointerId, x: event.clientX, y: event.clientY, startX: event.clientX, startY: event.clientY, moved: false };
    canvas.setPointerCapture(event.pointerId);
  });
  canvas.addEventListener("pointermove", (event) => {
    if (!drag || drag.id !== event.pointerId) return;
    const dx = event.clientX - drag.x, dy = event.clientY - drag.y;
    if (Math.hypot(event.clientX - drag.startX, event.clientY - drag.startY) > 5) drag.moved = true;
    if (drag.moved) {
      const scale = dimensions().scale;
      view.x += dx / scale;
      view.y -= dy / scale;
      canvas.classList.add("dragging");
      paintMap();
    }
    drag.x = event.clientX; drag.y = event.clientY;
  });
  canvas.addEventListener("pointerup", (event) => {
    if (!drag || drag.id !== event.pointerId) return;
    const moved = drag.moved;
    drag = null;
    canvas.classList.remove("dragging");
    if (!moved) findTreeAt(event.clientX, event.clientY);
  });
  canvas.addEventListener("pointercancel", () => { drag = null; canvas.classList.remove("dragging"); });
  canvas.addEventListener("wheel", (event) => {
    event.preventDefault();
    zoomBy(event.deltaY < 0 ? 1.12 : 1 / 1.12);
  }, { passive: false });
  canvas.addEventListener("keydown", (event) => {
    let handled = true;
    switch (event.key) {
      case "ArrowUp": navigate(0, -PAN_STEP); break;
      case "ArrowDown": navigate(0, PAN_STEP); break;
      case "ArrowLeft": navigate(PAN_STEP, 0); break;
      case "ArrowRight": navigate(-PAN_STEP, 0); break;
      case "+": case "=": zoomBy(1.2); break;
      case "-": zoomBy(1 / 1.2); break;
      case "Home": fit(); break;
      default: handled = false;
    }
    if (handled) event.preventDefault();
  });
  canvas.tabIndex = 0;

  $("panNorth").addEventListener("click", () => navigate(0, -PAN_STEP));
  $("panSouth").addEventListener("click", () => navigate(0, PAN_STEP));
  $("panEast").addEventListener("click", () => navigate(-PAN_STEP, 0));
  $("panWest").addEventListener("click", () => navigate(PAN_STEP, 0));
  $("zoomIn").addEventListener("click", () => zoomBy(1.2));
  $("zoomOut").addEventListener("click", () => zoomBy(1 / 1.2));
  $("fitMap").addEventListener("click", fit);

  $("findForm").addEventListener("submit", (event) => { event.preventDefault(); selectExact($("treeSearch").value); });
  $("treeSearch").addEventListener("input", renderRoster);
  document.addEventListener("keydown", (event) => {
    if (event.key === "/" && !["INPUT", "TEXTAREA", "SELECT"].includes(document.activeElement.tagName)) {
      event.preventDefault(); $("treeSearch").focus();
    }
  });
  $("reasonSelect").addEventListener("change", () => {
    const other = $("reasonSelect").value === "Other";
    $("customWrap").hidden = !other;
    $("reasonCustom").required = other;
    if (other) $("reasonCustom").focus();
  });
  $("addOrder").addEventListener("click", addOrder);
  $("approvePlan").addEventListener("click", approvePlan);
  $("executePlan").addEventListener("click", executePlan);
  $("revisePlan").addEventListener("click", withdrawApproval);
  $("advanceYear").addEventListener("click", advanceYear);
  $("resetDemo").addEventListener("click", resetDemo);

  if (typeof ResizeObserver !== "undefined") {
    new ResizeObserver(paintMap).observe($("mapFrame"));
  } else {
    window.addEventListener("resize", paintMap);
  }
  renderAll();
})();
