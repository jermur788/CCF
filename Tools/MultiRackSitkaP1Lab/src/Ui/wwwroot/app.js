// #849 playable 2D Stand Map page (spec MULTI-STANDMAP-PLAYABLE-2D-V1 §4, §5, §7, §8).
//
// The server's view is the only authority. This page draws, lists and inspects only what the latest view says, sends
// exact tree IDs with the fence of the view it displayed, and itself converts nothing but screen pixels into map metres.
// It computes no forestry statistic (counts, basal area, Dq, deaths, growth), keeps no copy of felled trees, counters,
// clocks or randomness, uses no web storage, and loads nothing from another origin. Every text node is set with
// textContent, never parsed as HTML.
'use strict';

(function () {
  const SVG_NS = 'http://www.w3.org/2000/svg';
  const STATUS_ORDER = ['Living', 'PendingFell', 'ApprovedFell', 'Felled', 'NaturalDead'];
  const STATUS_LABEL = {
    Living: 'StatusLiving',
    PendingFell: 'StatusPendingFell',
    ApprovedFell: 'StatusApprovedFell',
    Felled: 'StatusFelled',
    NaturalDead: 'StatusNaturalDead',
  };
  const STATUS_CUE = {
    Living: 'crown disc',
    PendingFell: 'crown with a dashed ring',
    ApprovedFell: 'crown with a solid double ring',
    Felled: 'stump marker with a cross; the canopy gap is open',
    NaturalDead: 'hatched grey crown with a dashed outline',
  };

  const state = {
    view: null, // the latest PlayerSessionView from the server
    scene: [], // the PlayerMapScene items the server built from that same view, in paint order
    labels: null, // Core's PlayerLabels, read from the server
    selectedId: null, // the UI-only draft selection: one exact TreeId, never an authority
    overlap: null, // { candidates, ambiguous } from the latest map hit-test; any newer view clears it
    busy: false,
    // The last answered Execute: its key, the work revision of the fence sent and the revision the server recorded.
    // Used only to label the receipt of that same execution key as "w → w+1".
    lastExecution: null,
    lastWasReplay: false, // whether the latest committed command was an Execute replay
    opener: null, // the element that had focus before a dialog opened
  };

  const byId = (id) => document.getElementById(id);

  // ---- small DOM helpers ----------------------------------------------------------------------------------------------

  function setAttributes(node, attributes) {
    for (const [name, value] of Object.entries(attributes || {})) {
      if (value !== null && value !== undefined && value !== false) node.setAttribute(name, String(value));
    }
  }

  // Builds an HTML element. Strings and numbers become text nodes; null and undefined children are skipped.
  function el(tag, attributes, ...children) {
    const node = document.createElement(tag);
    setAttributes(node, attributes);
    for (const child of children) {
      if (child === null || child === undefined) continue;
      node.append(child instanceof Node ? child : String(child));
    }
    return node;
  }

  function svg(tag, attributes, textValue) {
    const node = document.createElementNS(SVG_NS, tag);
    setAttributes(node, attributes);
    if (textValue !== undefined) node.textContent = String(textValue);
    return node;
  }

  function pair(term, value) {
    return el('div', null, el('dt', null, term), el('dd', null, value));
  }

  function label(name) {
    return state.labels && typeof state.labels[name] === 'string' ? state.labels[name] : '';
  }

  // Fills {n} and {y} in a PlayerLabels template with values taken from the view.
  function fill(template, values) {
    return template.replace(/\{(\w+)\}/g, (match, name) => (name in values ? String(values[name]) : match));
  }

  // Display rounding only: the value itself always comes from the server view.
  function fixed(value, digits) {
    return typeof value === 'number' ? value.toFixed(digits) : String(value);
  }

  function statusText(item) {
    const template = label(STATUS_LABEL[item.status]);
    return template ? fill(template, { y: item.deathYear }) : String(item.status);
  }

  // The status words without their year, for the legend and the filter.
  function statusName(status) {
    return label(STATUS_LABEL[status]).replace(/\s*Y\{y\}$/, '');
  }

  // ---- talking to the local session process ---------------------------------------------------------------------------

  // The fence is echoed exactly as the latest view gave it; the page never edits it.
  function currentFence() {
    const view = state.view;
    return { sessionId: view.sessionId, source: view.source, planStateRevision: view.planStateRevision };
  }

  // Relative URLs keep every request on this page's own origin and capability path.
  async function call(path, body) {
    const init = { method: body === undefined ? 'GET' : 'POST', cache: 'no-store', credentials: 'omit', redirect: 'error' };
    if (body !== undefined) {
      init.headers = { 'Content-Type': 'application/json' };
      init.body = JSON.stringify(body);
    }
    let response;
    try {
      response = await fetch(path, init);
    } catch (failure) {
      return { kind: 'transport', detail: String((failure && failure.message) || failure) };
    }
    let payload = null;
    try {
      payload = await response.json();
    } catch {
      payload = null;
    }
    if (response.status === 200 && payload && payload.view) return { kind: 'ok', payload };
    if (response.status === 409 && payload && payload.view && payload.refusal) return { kind: 'refused', payload };
    if (payload && payload.error) {
      const unconfirmed = payload.error.code === 'OutcomeUnconfirmed';
      return { kind: unconfirmed ? 'unconfirmed' : 'guard', status: response.status, error: payload.error };
    }
    return { kind: 'transport', detail: 'HTTP ' + response.status };
  }

  // Shows a newly received view. A selection is kept only if its exact ID is in the new view (spec §5 S-8).
  function applyPayload(payload, keepOverlap) {
    state.view = payload.view;
    state.scene = payload.scene;
    if (payload.labels) state.labels = payload.labels;
    if (!keepOverlap) state.overlap = null;
    if (state.selectedId !== null && !state.view.frame.trees.some((tree) => tree.treeId === state.selectedId)) {
      state.selectedId = null;
    }
    buildStatusFilter();
    renderAll();
  }

  // Runs one fenced command. Nothing on the page changes until the server answers; then the answer is drawn as it is.
  async function run(path, body, what, onCommitted, options) {
    if (state.busy || !state.view) return;
    state.busy = true;
    document.documentElement.classList.add('busy');
    const outcome = await call(path, body);
    state.busy = false;
    document.documentElement.classList.remove('busy');
    settle(outcome, what, onCommitted, options || {});
  }

  function settle(outcome, what, onCommitted, options) {
    if (outcome.kind === 'ok') {
      clearAlert();
      state.lastWasReplay = Boolean(outcome.payload.execution && outcome.payload.execution.replayed);
      applyPayload(outcome.payload, false);
      if (onCommitted) onCommitted(outcome.payload);
      if (options.focusAfter) keepFocusOr(options.focusAfter);
    } else if (outcome.kind === 'refused') {
      applyPayload(outcome.payload, false);
      const field = options.failingField ? options.failingField(outcome.payload.refusal.code) : null;
      showRefusal(what, outcome.payload.refusal, field);
    } else if (outcome.kind === 'guard') {
      showGuardRefusal(what, outcome.status, outcome.error);
    } else {
      showUnconfirmed(what);
    }
  }

  // ---- alerts and announcements ---------------------------------------------------------------------------------------

  function clearAlert() {
    const box = byId('alert');
    box.replaceChildren();
    box.classList.remove('shown');
  }

  // Refusals and failures go to the alert region and move focus there, or to the failing field (spec §8 A-5).
  function showAlert(parts, focusTarget) {
    const box = byId('alert');
    box.replaceChildren(...parts.filter((part) => part !== null));
    box.classList.add('shown');
    (focusTarget || box).focus();
  }

  function announce(message) {
    const region = byId('announce');
    region.textContent = '';
    window.setTimeout(() => {
      region.textContent = message;
    }, 50);
  }

  function rereadButton() {
    const button = el('button', { type: 'button' }, 'Re-read current state');
    button.addEventListener('click', () => reread(true));
    return button;
  }

  function recoveryFor(code) {
    switch (code) {
      case 'PlayerSourceStale':
      case 'PlayerPlanStateStale':
      case 'PlayerSessionMismatch':
        return 'The page now shows the current view. Review it, then try again.';
      case 'WorkReasonRequired':
        return 'Write a reason for this tree, then add it again.';
      case 'WorkPlanAlreadyApproved':
        return 'Cancel the approved plan before editing the Work Plan.';
      case 'WorkPendingConsentRequired':
        return 'Choose whether to keep the orders or cancel them before the year advances.';
      case 'WorkWouldLeaveNoLivingStems':
        return label('AllLiveBatchRefused') + '. Cancel the approved plan and approve a smaller batch.';
      default:
        return 'Review the current view, then choose again.';
    }
  }

  function showRefusal(what, refusal, focusTarget) {
    showAlert([
      el('p', { class: 'alert-title' }, 'Refused: ' + what),
      el('p', null, el('code', null, refusal.code), ' — ' + refusal.message),
      el('p', null, label('NothingChanged') + '. Current ecological year ' + state.view.frame.ecologicalYear + '.'),
      el('p', null, recoveryFor(refusal.code)),
    ], focusTarget);
  }

  function showGuardRefusal(what, status, error) {
    showAlert([
      el('p', { class: 'alert-title' }, 'Refused by the local page server: ' + what),
      el('p', null, el('code', null, error.code), ' (HTTP ' + status + ') — ' + error.message),
      el('p', null, 'The request did not reach the engine.'),
      rereadButton(),
    ]);
  }

  // After a transport failure or a fault the outcome is unknown: the page claims neither success nor refusal, and its
  // only offer is a re-read of the current state (spec §7.1 item 7).
  function showUnconfirmed(what) {
    showAlert([
      el('p', { class: 'alert-title' }, label('OutcomeUnconfirmed') || 'No answer from the local session process'),
      el('p', null, what + ': no confirmed result was received. Do not assume that it happened, or that it did not.'),
      rereadButton(),
    ]);
  }

  function keepFocusOr(id) {
    const active = document.activeElement;
    const usable = active && active !== document.body && active.isConnected && !active.closest('[hidden]');
    if (!usable) byId(id).focus();
  }

  // ---- rendering ------------------------------------------------------------------------------------------------------

  function renderAll() {
    if (!state.view || !state.labels) return;
    const active = document.activeElement;
    const focusKey = active && active.getAttribute ? active.getAttribute('data-focus-key') : null;
    const items = new Map(state.scene.map((item) => [item.treeId, item]));
    renderStatus();
    renderMap(items);
    renderOverlap();
    renderInspector(items);
    renderPlan();
    renderTable(items);
    renderResultAndHistory();
    // Re-created buttons get their focus back by a stable key (an exact tree ID), so a redraw is not a focus reset.
    if (focusKey) {
      const again = document.querySelector('[data-focus-key="' + CSS.escape(focusKey) + '"]');
      if (again) again.focus();
    }
  }

  function expandable(full) {
    const value = String(full);
    return el('details', { class: 'expand' },
      el('summary', null, el('span', { class: 'mono' }, value.slice(0, 12) + '…'), el('span', { class: 'sr-only' }, ' show the full value')),
      el('code', { class: 'full' }, value));
  }

  // Status bar (S0): every value here is read from the view or from PlayerLabels.
  function renderStatus() {
    const view = state.view;
    const source = view.source;
    const frame = view.frame;
    const stand = frame.stand;
    document.title = 'Stand Map — ' + label('Title');
    byId('page-title').textContent = label('Title');
    byId('disclosure').textContent = label('Disclosure');
    byId('facts').replaceChildren(
      pair('Domain', source.domain),
      pair('Fixture', source.fixtureId),
      pair('Fixture file SHA-256', expandable(source.fixtureFileSha256)),
      pair('Fixture content hash', expandable(source.fixtureContentHash)),
      pair('Models', 'RNG v' + source.rngModelVersion + ' · growth model v' + source.growthModelVersion + ' · algorithm ' + source.p1AlgorithmVersion),
      pair('Ecological year', frame.ecologicalYear),
      pair('Annual revision', frame.annualRevision),
      pair('Work revision', frame.workRevision),
      pair('Plan-state revision', view.planStateRevision),
      pair('Session', expandable(view.sessionId)),
      pair('Living trees', stand.livingCount),
      pair('Stems per ha', fixed(stand.stemsPerHa, 1)),
      pair('Basal area', fixed(stand.basalAreaM2PerHa, 2) + ' m²/ha'),
      pair('Quadratic mean DBH (Dq)', fixed(stand.quadraticMeanDbhCm, 1) + ' cm'),
      pair('Area', fixed(view.extent.areaHa, 2) + ' ha'));
    byId('flags').replaceChildren(...['ManagementArea', 'ComparisonExtension', 'Money', 'SessionOnly'].map((name) => el('li', null, label(name))));
  }

  // Map (spec §4): the server's scene, drawn in its paint order. One SVG user unit is one metre; U runs east from the west
  // edge and V south from the north edge, so the scene's numbers are used as they are.
  function renderMap(items) {
    const view = state.view;
    const crowns = [];
    const marks = [];
    const ordered = state.scene.slice().sort((a, b) => a.paintOrder - b.paintOrder);
    for (const item of ordered) {
      const group = svg('g', { class: 'tree s-' + item.status, 'data-tree-id': item.treeId });
      group.append(svg('circle', { class: 'crown', cx: item.u, cy: item.v, r: item.radiusM }));
      crowns.push(group);
      // Status marks sit in an overlay that ignores the pointer, so a mark never changes which crown is hit.
      if (item.status === 'PendingFell') {
        marks.push(svg('circle', { class: 'ring-pending', cx: item.u, cy: item.v, r: item.radiusM + 0.25 }));
      } else if (item.status === 'ApprovedFell') {
        marks.push(svg('circle', { class: 'ring-approved', cx: item.u, cy: item.v, r: item.radiusM + 0.2 }));
        marks.push(svg('circle', { class: 'ring-approved', cx: item.u, cy: item.v, r: item.radiusM + 0.5 }));
      } else if (item.status === 'Felled') {
        const d = 0.24;
        marks.push(svg('path', {
          class: 'stump-x',
          d: 'M ' + (item.u - d) + ' ' + (item.v - d) + ' L ' + (item.u + d) + ' ' + (item.v + d) +
            ' M ' + (item.u - d) + ' ' + (item.v + d) + ' L ' + (item.u + d) + ' ' + (item.v - d),
        }));
      }
    }
    byId('crowns').replaceChildren(...crowns);
    byId('marks').replaceChildren(...marks);

    const chosen = state.selectedId === null ? null : items.get(state.selectedId);
    byId('selection').replaceChildren(...(chosen ? [
      svg('circle', { class: 'select-halo', cx: chosen.u, cy: chosen.v, r: chosen.radiusM + 0.45 }),
      svg('circle', { class: 'select-ring', cx: chosen.u, cy: chosen.v, r: chosen.radiusM + 0.45 }),
    ] : []));

    const approvedOrders = view.approved ? view.approved.orders.length : 0;
    byId('map-desc').textContent = 'North-up map. ' + label('ManagementArea') + '. Ecological year ' + view.frame.ecologicalYear +
      '. Living trees ' + view.frame.stand.livingCount + '; pending orders ' + view.pending.length + '; approved orders ' +
      approvedOrders + '. ' + (state.selectedId !== null ? 'Selected tree ' + state.selectedId + '. ' : '') +
      'Use the tree table to select a tree by keyboard.';
    renderLegend(items);
  }

  function swatch(kind) {
    const box = svg('svg', { class: 'swatch', viewBox: '-1.6 -1.6 3.2 3.2', 'aria-hidden': 'true', focusable: 'false' });
    const crown = (status, r) => {
      const group = svg('g', { class: 'tree s-' + status });
      group.append(svg('circle', { class: 'crown', cx: 0, cy: 0, r }));
      return group;
    };
    if (kind === 'Living' || kind === 'NaturalDead') box.append(crown(kind, 1.1));
    if (kind === 'PendingFell') box.append(crown(kind, 0.9), svg('circle', { class: 'ring-pending', cx: 0, cy: 0, r: 1.25 }));
    if (kind === 'ApprovedFell') {
      box.append(crown(kind, 0.75), svg('circle', { class: 'ring-approved', cx: 0, cy: 0, r: 1.0 }),
        svg('circle', { class: 'ring-approved', cx: 0, cy: 0, r: 1.35 }));
    }
    if (kind === 'Felled') {
      box.append(crown(kind, 0.55), svg('path', { class: 'stump-x', d: 'M -0.35 -0.35 L 0.35 0.35 M -0.35 0.35 L 0.35 -0.35' }));
    }
    if (kind === 'Selected') {
      box.append(crown('Living', 0.8), svg('circle', { class: 'select-halo', cx: 0, cy: 0, r: 1.2 }),
        svg('circle', { class: 'select-ring', cx: 0, cy: 0, r: 1.2 }));
    }
    if (kind === 'Boundary') box.append(svg('line', { class: 'boundary-line', x1: -1.4, y1: 0, x2: 1.4, y2: 0 }));
    return box;
  }

  // The legend names only what is drawn now (spec §4.2).
  function renderLegend(items) {
    const present = new Set(state.scene.map((item) => item.status));
    const entries = STATUS_ORDER.filter((status) => present.has(status))
      .map((status) => el('li', null, swatch(status), statusName(status) + ' — ' + STATUS_CUE[status]));
    if (state.selectedId !== null && items.has(state.selectedId)) {
      entries.push(el('li', null, swatch('Selected'), 'Selected — thick white ring'));
    }
    entries.push(el('li', null, swatch('Boundary'), 'Management area boundary — dashed line'));
    byId('legend').replaceChildren(...entries);
  }

  // Grid lines go under the crowns; their labels, the north arrow and the scale bar go above them.
  function drawGridAndFurniture() {
    const lines = [];
    const labels = [];
    for (let metres = 0; metres <= 30; metres += 5) {
      lines.push(svg('line', { class: 'grid-line', x1: metres, y1: 0, x2: metres, y2: 30 }));
      lines.push(svg('line', { class: 'grid-line', x1: 0, y1: metres, x2: 30, y2: metres }));
      // Grid labels count east and north from the south-west corner; V runs south, so north = 30 - V.
      labels.push(svg('text', { class: 'grid-label', x: metres, y: 31.6, 'text-anchor': 'middle' }, metres));
      labels.push(svg('text', { class: 'grid-label', x: -0.5, y: 30 - metres + 0.4, 'text-anchor': 'end' }, metres));
    }
    labels.push(svg('text', { class: 'grid-label', x: 15, y: 33.0, 'text-anchor': 'middle' }, 'metres east'));
    labels.push(svg('text', { class: 'grid-label', x: -3.1, y: 15, 'text-anchor': 'middle', transform: 'rotate(-90 -3.1 15)' }, 'metres north'));
    byId('grid').replaceChildren(...lines);
    byId('furniture').replaceChildren(...labels,
      svg('path', { class: 'north-arrow', d: 'M 32.3 4.4 L 32.3 1.8 M 32.3 0.9 L 31.65 2.1 L 32.95 2.1 Z' }),
      svg('text', { class: 'furniture-label', x: 32.3, y: 0.25, 'text-anchor': 'middle' }, 'N'),
      svg('path', { class: 'scale-bar', d: 'M 0 34.6 L 5 34.6 M 0 34.05 L 0 35.15 M 5 34.05 L 5 35.15' }),
      svg('text', { class: 'furniture-label', x: 5.6, y: 35.0 }, '5 m'));
  }

  // Overlapping crowns at the last clicked point (spec §5 S-4, S-5), topmost first, each with its exact ID.
  function renderOverlap() {
    const box = byId('overlap');
    const overlap = state.overlap;
    if (!overlap) {
      box.hidden = true;
      byId('overlap-list').replaceChildren();
      return;
    }
    box.hidden = false;
    byId('overlap-heading').textContent = overlap.ambiguous
      ? label('SelectionAmbiguous')
      : fill(label('CrownsOverlap'), { n: overlap.candidates.length });
    byId('overlap-list').replaceChildren(...overlap.candidates.map((candidate, index) => {
      const current = candidate.treeId === state.selectedId;
      const button = el('button', {
        type: 'button', class: 'small', 'data-pick': candidate.treeId, 'data-focus-key': 'pick:' + candidate.treeId,
        'aria-label': (current ? 'Selected ' : 'Select ') + candidate.treeId,
      }, current ? 'Selected' : 'Select');
      return el('li', null, button, ' ', el('span', { class: 'mono' }, candidate.treeId),
        ' — height ' + fixed(candidate.heightM, 1) + ' m, DBH ' + fixed(candidate.dbhCm, 1) + ' cm, ' + statusText(candidate) +
        (index === 0 ? ' (topmost)' : ''));
    }));
  }

  function recordedReason(treeId) {
    const view = state.view;
    const pending = view.pending.find((order) => order.treeId === treeId);
    if (pending) return pending.reason + ' (pending order)';
    const approved = view.approved ? view.approved.orders.find((order) => order.treeId === treeId) : null;
    if (approved) return approved.reason + ' (approved order)';
    for (const event of view.frame.events) {
      const fell = event.fells.find((record) => record.treeId === treeId);
      if (fell) return fell.reason + ' (executed Fell, work revision ' + fell.workRevision + ')';
    }
    return null;
  }

  // Inspector (S1): the selected exact ID in full, its measurements with units, and only the actions its state allows.
  function renderInspector(items) {
    const view = state.view;
    const tree = state.selectedId === null ? null : view.frame.trees.find((candidate) => candidate.treeId === state.selectedId);
    const item = tree ? items.get(tree.treeId) : null;
    byId('inspector-empty').hidden = Boolean(tree && item);
    byId('inspector-body').hidden = !(tree && item);
    if (!tree || !item) return;

    byId('insp-id').textContent = tree.treeId;
    const facts = [
      pair('Status', statusText(item)),
      // The server's camelCase JSON names AnnualTree.XM and ZM "xm" and "zm".
      pair('Position', 'X ' + fixed(tree.xm, 3) + ' m (east), Z ' + fixed(tree.zm, 3) + ' m (north), from the centre of the area'),
      pair('Map position', fixed(item.u, 3) + ' m east of the west edge, ' + fixed(item.v, 3) + ' m south of the north edge'),
      pair('Age', tree.ageYears + ' years'),
      pair('DBH', fixed(tree.dbhCm, 2) + ' cm'),
      pair('Height', fixed(tree.heightM, 2) + ' m'),
      pair('Crown radius', fixed(tree.crownRadiusM, 3) + ' m'),
    ];
    if (tree.death) {
      facts.push(pair('Death year', tree.death.ecologicalYear));
      facts.push(pair('Cause', tree.death.cause + ' (' + tree.death.mechanism + ')'));
    }
    const reason = recordedReason(tree.treeId);
    if (reason !== null) facts.push(pair('Recorded reason', reason));
    byId('insp-facts').replaceChildren(...facts);

    byId('insp-threshold').textContent = label('ThresholdInformational');
    byId('insp-threshold').hidden = !tree.isLiving;
    byId('add-form').hidden = item.status !== 'Living';
    byId('add-button').textContent = label('AddToWorkPlan');
    byId('insp-remove-wrap').hidden = item.status !== 'PendingFell';
    byId('insp-remove').textContent = label('RemoveFromWorkPlan');
    byId('insp-approved-note').hidden = item.status !== 'ApprovedFell';
  }

  // Work Plan (S2-S4): the engine's pending orders and its approved plan with the minted PlanId and execution key.
  function renderPlan() {
    const view = state.view;
    const pending = view.pending;
    byId('pending-empty').hidden = pending.length > 0;
    byId('pending-list').replaceChildren(...pending.map((order) => el('li', null,
      el('span', { class: 'mono' }, order.treeId), ' — ' + order.reason + ' ',
      el('button', {
        type: 'button', class: 'small', 'data-remove': order.treeId, 'data-focus-key': 'remove:' + order.treeId,
        'aria-label': label('RemoveFromWorkPlan') + ': ' + order.treeId,
      }, label('RemoveFromWorkPlan')))));

    const approve = byId('approve');
    approve.textContent = label('ApproveWholePlan');
    approve.hidden = pending.length === 0 || view.approved !== null;

    const plan = view.approved;
    byId('approved-empty').hidden = plan !== null;
    byId('approved-box').hidden = plan === null;
    if (plan === null) {
      // Nothing of an executed or cancelled plan stays in the page, not even hidden.
      byId('approved-facts').replaceChildren();
      byId('approved-list').replaceChildren();
      return;
    }
    byId('approved-facts').replaceChildren(
      pair('PlanId', el('code', null, plan.planId)),
      pair('Execution key', el('code', null, plan.executionKey)),
      pair('Approved against', 'ecological year ' + plan.source.ecologicalYear + ', annual revision ' + plan.source.annualRevision +
        ', work revision ' + plan.source.workRevision),
      pair('Orders', plan.orders.length));
    byId('approved-list').replaceChildren(...plan.orders.map((order) => el('li', null,
      el('span', { class: 'mono' }, order.treeId), ' — ' + order.reason)));
    byId('cancel-approved').textContent = label('CancelApprovedPlan');
    byId('execute').textContent = label('ExecuteApprovedPlan');
  }

  function buildStatusFilter() {
    const select = byId('status-filter');
    if (select.options.length > 0 || !state.labels) return;
    select.append(el('option', { value: 'all' }, 'All statuses'));
    for (const status of STATUS_ORDER) select.append(el('option', { value: status }, statusName(status)));
  }

  // Tree table (spec §5 S-6): the authoritative keyboard and screen-reader path, in fixture order, with exact IDs.
  function renderTable(items) {
    const view = state.view;
    const query = byId('tree-search').value;
    const wanted = byId('status-filter').value || 'all';
    const rows = [];
    for (const tree of view.frame.trees) {
      const item = items.get(tree.treeId);
      if (!item) continue;
      if (query !== '' && !tree.treeId.includes(query)) continue;
      if (wanted !== 'all' && item.status !== wanted) continue;
      const selected = tree.treeId === state.selectedId;
      const button = el('button', {
        type: 'button', class: 'small', 'data-select': tree.treeId, 'data-focus-key': 'select:' + tree.treeId,
        'aria-label': (selected ? 'Selected ' : 'Select ') + tree.treeId,
      }, selected ? 'Selected' : 'Select');
      rows.push(el('tr', { class: selected ? 'current' : null, 'aria-current': selected ? 'true' : null },
        el('td', null, button),
        el('th', { scope: 'row', class: 'mono' }, tree.treeId),
        el('td', null, statusText(item)),
        el('td', null, fixed(tree.dbhCm, 1)),
        el('td', null, fixed(tree.heightM, 1)),
        el('td', null, fixed(tree.crownRadiusM, 2)),
        el('td', null, fixed(tree.xm, 2)),
        el('td', null, fixed(tree.zm, 2))));
    }
    byId('tree-rows').replaceChildren(...rows);
    byId('rows-shown').textContent = rows.length + ' rows shown' + (query !== '' || wanted !== 'all' ? ' (filtered)' : '');
  }

  function table(caption, headers, rows) {
    return el('div', { class: 'table-wrap', role: 'region', 'aria-label': caption, tabindex: '0' },
      el('table', null,
        el('caption', null, caption),
        el('thead', null, el('tr', null, ...headers.map((header) => el('th', { scope: 'col' }, header)))),
        el('tbody', null, ...rows.map((cells) => el('tr', null,
          el('th', { scope: 'row' }, cells[0]), ...cells.slice(1).map((cell) => el('td', null, cell)))))));
  }

  // Result and year (S5-S7): the latest executed work event and the session history, previous actual versus current.
  function renderResultAndHistory() {
    const view = state.view;
    const events = view.frame.events;
    const receipt = [];
    if (events.length === 0) {
      receipt.push(el('p', null, 'No work has been executed in this session.'));
    } else {
      const event = events[events.length - 1];
      if (state.lastWasReplay) receipt.push(el('p', { class: 'warning' }, label('AlreadyExecuted')));
      // "w → w+1" only when this page holds both revisions for this exact key; otherwise only the recorded one.
      const seen = state.lastExecution;
      const revision = seen && seen.executionKey === event.executionKey
        ? pair('Work revision', seen.workRevisionBefore + ' → ' + seen.workRevisionAfter)
        : pair('Work revision after this event', event.workRevision);
      receipt.push(el('dl', { class: 'pairs' },
        pair('Execution key', el('code', null, event.executionKey)),
        pair('PlanId', el('code', null, event.planId)),
        pair('Ecological year', event.ecologicalYear),
        revision,
        pair('Living trees', event.before.livingCount + ' → ' + event.after.livingCount),
        pair('Basal area', fixed(event.before.basalAreaM2PerHa, 2) + ' → ' + fixed(event.after.basalAreaM2PerHa, 2) + ' m²/ha'),
        pair('Stems per ha', fixed(event.before.stemsPerHa, 1) + ' → ' + fixed(event.after.stemsPerHa, 1))));
      receipt.push(el('h4', null, 'Felled in this event'));
      receipt.push(el('ul', { class: 'list' }, ...event.fells.map((fell) => el('li', null,
        el('span', { class: 'mono' }, fell.treeId), ' — ' + fell.cause + ' (' + fell.mechanism + ') — ' + fell.reason))));
    }
    byId('receipt').replaceChildren(...receipt);
    byId('advance').textContent = label('AdvanceYear');
    byId('proposed-note').textContent = 'Proposed values: ' + label('NotCovered') + '. ' + label('ComparisonExtension') + '.';

    const history = [];
    const years = view.annualHistory;
    if (years.length === 0) {
      history.push(el('p', null, 'No ecological year has been advanced in this session.'));
    } else {
      history.push(table('Committed ecological years',
        ['Year', 'Living trees', 'Basal area (m²/ha)', 'Natural deaths', 'Unexecuted work cancelled'],
        years.map((record) => [
          record.yearBefore + ' → ' + record.yearAfter,
          record.before.livingCount + ' → ' + record.after.livingCount,
          fixed(record.before.basalAreaM2PerHa, 2) + ' → ' + fixed(record.after.basalAreaM2PerHa, 2),
          record.naturalDeaths.length === 0 ? 'none'
            : record.naturalDeaths.map((death) => death.treeId + ' (' + death.cause + ', ' + death.mechanism + ')').join('; '),
          record.cancelledUnexecutedWork ? 'yes, at zero Fell' : 'no',
        ])));
    }
    if (events.length > 0) {
      history.push(table('Executed work events',
        ['Year', 'Work revision', 'Felled', 'Living trees', 'Basal area (m²/ha)', 'PlanId'],
        events.map((event) => [
          String(event.ecologicalYear),
          String(event.workRevision),
          event.fells.map((fell) => fell.treeId + ' (' + fell.cause + ')').join('; '),
          event.before.livingCount + ' → ' + event.after.livingCount,
          fixed(event.before.basalAreaM2PerHa, 2) + ' → ' + fixed(event.after.basalAreaM2PerHa, 2),
          event.planId,
        ])));
    }
    byId('history').replaceChildren(...history);
  }

  // ---- selection --------------------------------------------------------------------------------------------------------

  function chooseTree(treeId, fromTable) {
    if (!state.view || !state.view.frame.trees.some((tree) => tree.treeId === treeId)) return;
    state.selectedId = treeId;
    if (fromTable) state.overlap = null;
    renderAll();
    announce('Selected tree ' + treeId + '.');
    if (fromTable) byId('inspector-heading').focus();
  }

  // Map click (spec §5 S-2 to S-5): the page only converts the pointer to map metres; the server's hit-test on the current
  // view decides which exact IDs are under it. Bare ground never picks the nearest tree.
  async function onMapClick(event) {
    if (state.busy || !state.view) return;
    const matrix = byId('map').getScreenCTM();
    if (!matrix) return;
    const point = new DOMPoint(event.clientX, event.clientY).matrixTransform(matrix.inverse());
    const target = event.target instanceof Element ? event.target.closest('[data-tree-id]') : null;
    const drawnId = target ? target.getAttribute('data-tree-id') : null;

    state.busy = true;
    const outcome = await call('api/hit', { fence: currentFence(), u: point.x, v: point.y });
    state.busy = false;

    if (outcome.kind === 'ok') {
      clearAlert();
      const candidates = outcome.payload.candidates;
      let message;
      if (candidates.length === 0) {
        state.selectedId = null;
        state.overlap = null;
        message = 'No crown at this point — nothing selected.';
      } else if (drawnId !== candidates[0].treeId) {
        // S-5: the browser's topmost drawn crown and the server's topmost candidate disagree, so nothing is selected.
        state.selectedId = null;
        state.overlap = { candidates, ambiguous: true };
        message = label('SelectionAmbiguous') + '.';
      } else {
        state.selectedId = candidates[0].treeId;
        state.overlap = candidates.length > 1 ? { candidates, ambiguous: false } : null;
        message = 'Selected tree ' + state.selectedId + '.' +
          (candidates.length > 1 ? ' ' + fill(label('CrownsOverlap'), { n: candidates.length }) + '.' : '');
      }
      applyPayload(outcome.payload, true);
      announce(message);
    } else if (outcome.kind === 'refused') {
      // A stale fence: redraw from the fresh view and select nothing (S-3).
      state.selectedId = null;
      applyPayload(outcome.payload, false);
      showRefusal('Map selection', outcome.payload.refusal, null);
    } else if (outcome.kind === 'guard') {
      showGuardRefusal('Map selection', outcome.status, outcome.error);
    } else {
      showAlert([el('p', { class: 'alert-title' }, 'Map selection could not be checked with the local session; nothing was selected.'),
        rereadButton()]);
    }
  }

  function onSearchKey(event) {
    if (event.key !== 'Enter') return;
    event.preventDefault();
    const typed = byId('tree-search').value;
    if (state.view && state.view.frame.trees.some((tree) => tree.treeId === typed)) {
      clearAlert();
      chooseTree(typed, true);
    } else {
      showAlert([el('p', { class: 'alert-title' }, 'No exact match — the selection is unchanged'),
        el('p', null, 'No tree in the current view has the exact case-sensitive ID "' + typed + '".')], byId('tree-search'));
    }
  }

  // ---- commands -------------------------------------------------------------------------------------------------------

  function onAdd(event) {
    event.preventDefault();
    const treeId = state.selectedId;
    if (treeId === null) return;
    run('api/pending/add', { fence: currentFence(), treeId, reason: byId('reason').value }, 'Add ' + treeId + ' to the Work Plan',
      (payload) => {
        byId('reason').value = '';
        announce('Done: ' + label('AddToWorkPlan') + ' (' + treeId + '). Plan-state revision ' + payload.view.planStateRevision +
          '; living trees ' + payload.view.frame.stand.livingCount + '.');
      },
      { failingField: (code) => (code === 'WorkReasonRequired' ? byId('reason') : null), focusAfter: 'plan-heading' });
  }

  function removePending(treeId) {
    if (treeId === null) return;
    run('api/pending/remove', { fence: currentFence(), treeId }, 'Remove ' + treeId + ' from the Work Plan',
      (payload) => announce('Done: ' + label('RemoveFromWorkPlan') + ' (' + treeId + '). Plan-state revision ' +
        payload.view.planStateRevision + '.'),
      { focusAfter: 'plan-heading' });
  }

  function onApprove() {
    run('api/plan/approve', { fence: currentFence() }, label('ApproveWholePlan'),
      (payload) => announce('Done: ' + label('ApproveWholePlan') + '. PlanId ' + payload.view.approved.planId +
        '. Living trees ' + payload.view.frame.stand.livingCount + '.'),
      { focusAfter: 'plan-heading' });
  }

  function onCancelApproved() {
    run('api/plan/cancel', { fence: currentFence() }, label('CancelApprovedPlan'),
      (payload) => announce('Done: ' + label('CancelApprovedPlan') + '. Its orders were not returned to pending. Plan-state revision ' +
        payload.view.planStateRevision + '.'),
      { focusAfter: 'plan-heading' });
  }

  function openDialog(dialog, initialFocus) {
    state.opener = document.activeElement;
    dialog.showModal();
    initialFocus.focus();
  }

  // Escape or any close returns focus to the control that opened the dialog, if it is still shown (spec §8 A-1).
  function returnFocus() {
    const opener = state.opener;
    state.opener = null;
    if (opener && opener.isConnected && !opener.closest('[hidden]')) opener.focus();
  }

  function openExecuteDialog() {
    const view = state.view;
    const plan = view ? view.approved : null;
    if (!plan) return;
    byId('execute-title').textContent = label('ExecuteApprovedPlan');
    byId('execute-text').textContent = fill(label('ExecuteConfirmation'), { n: plan.orders.length, y: view.frame.ecologicalYear }) + '.';
    byId('execute-ids').replaceChildren(...plan.orders.map((order) => el('li', null,
      el('span', { class: 'mono' }, order.treeId), ' — ' + order.reason)));
    // A pre-warning only (D-093): the engine refuses such a batch as a whole, whatever this page shows.
    const living = view.frame.trees.filter((tree) => tree.isLiving).map((tree) => tree.treeId);
    const chosen = new Set(plan.orders.map((order) => order.treeId));
    const warning = byId('execute-warning');
    warning.textContent = label('AllLiveBatchRefused');
    warning.hidden = !(living.length > 0 && living.every((treeId) => chosen.has(treeId)));
    byId('execute-confirm').textContent = label('ExecuteApprovedPlan');
    // Focus starts on Back, so a repeated click or key press can never confirm by accident (spec §8 A-5).
    openDialog(byId('execute-dialog'), byId('execute-back'));
  }

  // After an unconfirmed outcome the page offers only a re-read. That shows either the receipt (it did commit) or the
  // plan still approved (it did not), and Execute then echoes the same engine-minted key, so it can never fell twice.
  function sendExecute(request) {
    run('api/plan/execute', request, label('ExecuteApprovedPlan'), (payload) => {
      const execution = payload.execution;
      const event = execution.event;
      // Before: the work revision of the fence this page sent. After: the revision the server recorded for this key.
      state.lastExecution = {
        executionKey: execution.executionKey,
        workRevisionBefore: request.fence.source.workRevision,
        workRevisionAfter: execution.workRevision,
      };
      renderResultAndHistory(); // the receipt was drawn with the view, before these revisions were known
      if (execution.replayed) {
        announce(label('AlreadyExecuted') + '. Execution key ' + execution.executionKey + '.');
        return;
      }
      announce('Done: ' + label('ExecuteApprovedPlan') + '. Felled ' + event.fells.map((fell) => fell.treeId).join(', ') +
        ' in ecological year ' + event.ecologicalYear + '. Work revision ' + request.fence.source.workRevision + ' → ' +
        execution.workRevision + '. Living trees ' + event.before.livingCount + ' → ' + event.after.livingCount +
        '; basal area ' + fixed(event.before.basalAreaM2PerHa, 2) + ' → ' + fixed(event.after.basalAreaM2PerHa, 2) + ' m²/ha.');
    }, { focusAfter: 'receipt-heading' });
  }

  function onExecuteConfirmed() {
    const view = state.view;
    const plan = view ? view.approved : null;
    byId('execute-dialog').close();
    if (!plan) return;
    // The engine minted the PlanId and the key; the page only echoes them with the fence it displayed.
    sendExecute({ fence: currentFence(), planId: plan.planId, executionKey: plan.executionKey });
  }

  function sendAdvance(consent) {
    run('api/year/advance', { fence: currentFence(), consentToCancelWork: consent }, label('AdvanceYear'), (payload) => {
      const annual = payload.annual;
      const record = payload.view.annualHistory[payload.view.annualHistory.length - 1];
      // The engine's own list of this year's natural deaths, named by exact ID; the page counts nothing itself (C-5).
      const deaths = annual.deaths.map((death) => death.treeId);
      announce('Done: ' + label('AdvanceYear') + ': year ' + annual.yearBefore + ' → ' + annual.yearAfter + '. Living trees ' +
        annual.before.livingCount + ' → ' + annual.after.livingCount + '; natural deaths: ' +
        (deaths.length === 0 ? 'none' : deaths.join(', ')) + '.' +
        (record && record.cancelledUnexecutedWork ? ' Unexecuted work was cancelled at zero Fell.' : ''));
    }, { focusAfter: 'year-heading' });
  }

  function onAdvance() {
    const view = state.view;
    if (!view) return;
    if (view.pending.length > 0 || view.approved !== null) openConsentDialog();
    else sendAdvance(false);
  }

  function openConsentDialog() {
    const view = state.view;
    const pendingIds = view.pending.map((order) => order.treeId);
    const approvedIds = view.approved ? view.approved.orders.map((order) => order.treeId) : [];
    byId('consent-title').textContent = label('AdvanceYear');
    byId('consent-text').textContent = 'The Work Plan has unexecuted work (' +
      (pendingIds.length > 0 ? 'pending: ' + pendingIds.join(', ') : 'no pending orders') + '; ' +
      (approvedIds.length > 0 ? 'approved: ' + approvedIds.join(', ') : 'no approved plan') +
      '). Advancing the year needs your explicit choice.';
    byId('consent-keep').textContent = fill(label('ConsentKeepOrders'), { y: view.frame.ecologicalYear });
    byId('consent-advance').textContent = label('ConsentCancelAndAdvance');
    openDialog(byId('consent-dialog'), byId('consent-keep'));
  }

  function onConsentKeep() {
    const view = state.view;
    byId('consent-dialog').close();
    announce(label('NothingChanged') + ' — ' + fill(label('ConsentKeepOrders'), { y: view.frame.ecologicalYear }) + '.');
  }

  function onConsentAdvance() {
    byId('consent-dialog').close();
    sendAdvance(true);
  }

  async function reread(announceIt) {
    if (state.busy) return;
    state.busy = true;
    const outcome = await call('api/view');
    state.busy = false;
    if (outcome.kind === 'ok') {
      clearAlert();
      state.lastWasReplay = false;
      applyPayload(outcome.payload, false);
      if (announceIt) {
        announce('Re-read: ecological year ' + state.view.frame.ecologicalYear + ', work revision ' + state.view.frame.workRevision +
          ', plan-state revision ' + state.view.planStateRevision + '.');
      }
    } else if (outcome.kind === 'guard') {
      showGuardRefusal('Re-read', outcome.status, outcome.error);
    } else {
      showAlert([el('p', { class: 'alert-title' }, 'The local session process did not answer the read.'), rereadButton()]);
    }
  }

  // ---- start ----------------------------------------------------------------------------------------------------------

  function wire() {
    byId('reread').addEventListener('click', () => reread(true));
    byId('map').addEventListener('click', onMapClick);
    byId('overlap-list').addEventListener('click', (event) => {
      const button = event.target.closest('[data-pick]');
      if (button) chooseTree(button.getAttribute('data-pick'), false);
    });
    byId('tree-rows').addEventListener('click', (event) => {
      const button = event.target.closest('[data-select]');
      if (button) chooseTree(button.getAttribute('data-select'), true);
    });
    byId('tree-search').addEventListener('input', renderAll);
    byId('tree-search').addEventListener('keydown', onSearchKey);
    byId('status-filter').addEventListener('change', renderAll);
    byId('add-form').addEventListener('submit', onAdd);
    byId('insp-remove').addEventListener('click', () => removePending(state.selectedId));
    byId('pending-list').addEventListener('click', (event) => {
      const button = event.target.closest('[data-remove]');
      if (button) removePending(button.getAttribute('data-remove'));
    });
    byId('approve').addEventListener('click', onApprove);
    byId('cancel-approved').addEventListener('click', onCancelApproved);
    byId('execute').addEventListener('click', openExecuteDialog);
    byId('execute-back').addEventListener('click', () => byId('execute-dialog').close());
    byId('execute-confirm').addEventListener('click', onExecuteConfirmed);
    byId('advance').addEventListener('click', onAdvance);
    byId('consent-keep').addEventListener('click', onConsentKeep);
    byId('consent-advance').addEventListener('click', onConsentAdvance);
    byId('execute-dialog').addEventListener('close', returnFocus);
    byId('consent-dialog').addEventListener('close', returnFocus);
  }

  wire();
  drawGridAndFurniture();
  reread(false);
})();
