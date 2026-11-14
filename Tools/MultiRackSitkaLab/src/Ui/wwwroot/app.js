'use strict';
/*
 * Synthetic Sitka test stand (flat) — browser front end.
 * This file draws and formats what the Core API returns. It performs NO forestry arithmetic:
 * no basal area, density, Dq, removal fraction or area is ever computed here. The only maths is
 * screen geometry (metres → pixels) for drawing, and number formatting.
 */
(() => {
  const SVGNS = 'http://www.w3.org/2000/svg';
  const $ = (id) => document.getElementById(id);
  const svg = (name, attrs = {}) => {
    const el = document.createElementNS(SVGNS, name);
    for (const [k, v] of Object.entries(attrs)) el.setAttribute(k, v);
    return el;
  };

  const S = {
    state: null,
    treesById: new Map(),
    selected: new Set(),
    lastClicked: null,
    view: { scale: 1, ox: 0, oy: 0, fitted: false },
    previewSeq: 0,
    saves: [],
  };

  // ---------- API ----------
  async function api(path, body) {
    const opt = body === undefined
      ? { method: 'GET' }
      : { method: 'POST', headers: { 'Content-Type': 'application/json', 'X-Sitka-Lab': '1' }, body: JSON.stringify(body) };
    const res = await fetch(path, opt);
    let data = null;
    try { data = await res.json(); } catch { data = { ok: false, code: 'BadResponse', message: 'The server returned a non-JSON response (' + res.status + ').' }; }
    data.httpStatus = res.status;
    return data;
  }

  function say(ok, code, text) {
    const m = $('message');
    m.className = 'message ' + (ok ? 'ok' : 'err');
    m.dataset.code = code || '';
    m.textContent = (ok ? 'OK' : 'Refused') + (code && code !== 'ok' ? ' [' + code + ']' : '') + ': ' + text;
  }

  // ---------- formatting only ----------
  const f = (v, d) => (v === null || v === undefined ? 'not applicable' : Number(v).toFixed(d));
  const short = (h) => (h ? h.slice(0, 10) + '…' : '');
  const STATUS_LABEL = { live: 'live', dead: 'dead', felled: 'felled (stump)' };

  // ---------- state intake ----------
  function setState(st) {
    S.state = st;
    S.treesById = new Map(st.trees.map((t) => [t.treeId, t]));
    for (const id of [...S.selected]) if (!S.treesById.has(id)) S.selected.delete(id);
    renderAll();
  }

  async function refresh() {
    const st = await api('/api/state');
    setState(st);
    await refreshLists();
    await updatePreview();
  }

  async function refreshLists() {
    const fx = await api('/api/fixtures');
    const sel = $('fixture-select');
    const cur = sel.value;
    sel.replaceChildren(...fx.map((x) => { const o = document.createElement('option'); o.value = x.fixtureId; o.textContent = `${x.fixtureId} (${x.treeCount} trees, ${x.areaHa.toFixed(3)} ha)`; return o; }));
    sel.value = cur && fx.some((x) => x.fixtureId === cur) ? cur : (S.state ? S.state.fixtureId : fx[0].fixtureId);
    const sv = await api('/api/saves');
    fillSaves(sv.files || []);
  }

  function fillSaves(files) {
    S.saves = files;
    const sel = $('load-select');
    sel.replaceChildren(...files.map((n) => { const o = document.createElement('option'); o.value = n; o.textContent = n; return o; }));
    if (!files.length) { const o = document.createElement('option'); o.value = ''; o.textContent = '(no lab files yet)'; sel.append(o); }
  }

  // ---------- rendering ----------
  function renderAll() {
    const st = S.state;
    $('app-title').textContent = st.title;
    $('banner-source').textContent = st.sourceBanner;
    $('banner-growth').textContent = st.growthBanner;
    $('banner-model').textContent = `Model: ${st.modelName} · v${st.modelVersion}`;
    $('forest-line').textContent =
      `Forest ${st.forestId} · fixture ${st.fixtureId} · inventory year ${st.inventoryYear} · experiment step ${st.experimentStep} · forest revision ${st.forestRevision} · ` +
      `flat site (slope ${st.slopeDeg}°) · events ${st.eventCount} · state SHA-256 ${short(st.stateHash)}`;
    $('btn-advance').textContent = st.advanceStepLabel;
    document.title = st.title;
    renderMetrics(st.metrics);
    renderMap();
    renderSelection();
    renderPlans();
    renderEvents();
    renderMarkedHint();
  }

  function setMetric(id, text, raw) { const el = $(id); el.textContent = text; el.dataset.raw = raw === null || raw === undefined ? '' : String(raw); }

  function renderMetrics(m) {
    setMetric('m-area-m2', f(m.areaM2, 1), m.areaM2);
    setMetric('m-area-ha', f(m.areaHa, 4), m.areaHa);
    setMetric('m-n', String(m.eligibleLiveN), m.eligibleLiveN);
    setMetric('m-nha', f(m.nPerHa, 2), m.nPerHa);
    setMetric('m-ba', f(m.baM2, 4), m.baM2);
    setMetric('m-baha', f(m.baM2PerHa, 4), m.baM2PerHa);
    setMetric('m-dq', m.dqCm === null ? 'not applicable (no eligible adults)' : f(m.dqCm, 2) + ' cm', m.dqCm);
    $('m-below').textContent = m.liveBelowThreshold; $('m-dead').textContent = m.dead; $('m-felled').textContent = m.felled;
    $('m-threshold').textContent = S.state.adultThresholdMm;
  }

  // ----- map -----
  const mapEl = () => $('map');
  function mapSize() { const r = mapEl().getBoundingClientRect(); return { w: Math.max(200, r.width), h: Math.max(200, r.height) }; }
  const toScreen = (xM, yM) => ({ x: S.view.ox + (xM - S.state.minXMetres) * S.view.scale, y: S.view.oy + (S.state.maxYMetres - yM) * S.view.scale });
  const toMetres = (sx, sy) => ({ x: S.state.minXMetres + (sx - S.view.ox) / S.view.scale, y: S.state.maxYMetres - (sy - S.view.oy) / S.view.scale });

  function fitView() {
    const { w, h } = mapSize(); const st = S.state; const pad = 28;
    const pw = Math.max(0.001, st.maxXMetres - st.minXMetres), ph = Math.max(0.001, st.maxYMetres - st.minYMetres);
    const scale = Math.min((w - 2 * pad) / pw, (h - 2 * pad) / ph);
    S.view.scale = scale;
    S.view.ox = (w - pw * scale) / 2;
    S.view.oy = (h - ph * scale) / 2;
    S.view.fitted = true;
  }

  function visible(t) {
    if (t.status === 'dead') return $('show-dead').checked;
    if (t.status === 'felled') return $('show-felled').checked;
    if (t.belowThreshold) return $('show-below').checked;
    return true;
  }

  const RING_LETTER = { MARKED: 'M', DRAFT: 'D', PROPOSED: 'P', APPROVED: 'A', PENDING_WORK: 'W' };

  function glyph(t) {
    const ringKey = t.planState && t.planState !== 'EXECUTED' ? t.planState : (t.markState === 'PLAYER_MARKED' ? 'MARKED' : null);
    const g = svg('g', {
      class: 'tree', 'data-tree-id': t.treeId, 'data-status': t.status, 'data-mark': t.markState,
      'data-plan': t.planId || '', 'data-plan-state': t.planState || '', 'data-ring': ringKey || '',
    });
    const title = svg('title');
    title.textContent = `${t.treeId} · ${STATUS_LABEL[t.status]}${t.belowThreshold ? ' (below threshold)' : ''} · DBH ${t.dbhCm.toFixed(1)} cm · height ${t.heightM.toFixed(1)} m` +
      (ringKey ? ` · ${ringKey}${t.planId ? ' ' + t.planId : ''}` : '');
    g.append(title);
    if (t.status === 'dead') g.append(svg('path', { d: 'M-4.5,-4.5L4.5,4.5M4.5,-4.5L-4.5,4.5', class: 'shape dead' }));
    else if (t.status === 'felled') g.append(svg('rect', { x: -3.5, y: -3.5, width: 7, height: 7, class: 'shape felled' }));
    else if (t.belowThreshold) g.append(svg('circle', { r: 3, class: 'shape below' }));
    else g.append(svg('circle', { r: 5, class: 'shape live' }));
    // transparent hit target so small glyphs are easy to click
    g.append(svg('circle', { r: 6, fill: 'transparent', stroke: 'none', class: 'hit' }));
    if (ringKey) {
      g.append(svg('circle', { r: 8.5, class: 'ring ring-' + ringKey }));
      const tx = svg('text', { x: 9, y: -7, class: 'letter' }); tx.textContent = RING_LETTER[ringKey]; g.append(tx);
    }
    if (S.selected.has(t.treeId)) { g.classList.add('selected'); g.append(svg('rect', { x: -11, y: -11, width: 22, height: 22, class: 'sel' })); }
    return g;
  }

  function renderMap() {
    const st = S.state;
    if (!S.view.fitted) fitView();
    const layer = $('layer-trees');
    layer.replaceChildren();
    const frag = document.createDocumentFragment();
    for (const t of st.trees) {
      if (!visible(t)) continue;
      const g = glyph(t);
      const p = toScreen(t.xMetres, t.yMetres);
      g.setAttribute('transform', `translate(${p.x.toFixed(2)},${p.y.toFixed(2)})`);
      frag.append(g);
    }
    layer.append(frag);
    drawBoundaryAndScale();
  }

  function repositionOnly() {
    for (const g of $('layer-trees').children) {
      const t = S.treesById.get(g.dataset.treeId); if (!t) continue;
      const p = toScreen(t.xMetres, t.yMetres);
      g.setAttribute('transform', `translate(${p.x.toFixed(2)},${p.y.toFixed(2)})`);
    }
    drawBoundaryAndScale();
  }

  function drawBoundaryAndScale() {
    const st = S.state;
    const poly = svg('polygon', { class: 'plot', points: st.polygonMetres.map((p) => { const s = toScreen(p.xMetres, p.yMetres); return `${s.x.toFixed(2)},${s.y.toFixed(2)}`; }).join(' ') });
    poly.append(Object.assign(svg('title'), { textContent: 'Stand boundary (polygon, metres)' }));
    $('layer-boundary').replaceChildren(poly);
    // scale bar: pick a round metre length that is 70–160 px wide (display geometry only)
    const nice = [1, 2, 5, 10, 20, 50, 100, 200, 500, 1000];
    let metres = nice[0];
    for (const n of nice) { metres = n; if (n * S.view.scale >= 70) break; }
    const px = metres * S.view.scale; const { h } = mapSize();
    const g = svg('g', { class: 'scale' });
    g.append(svg('line', { x1: 14, y1: h - 18, x2: 14 + px, y2: h - 18 }), svg('line', { x1: 14, y1: h - 24, x2: 14, y2: h - 12 }), svg('line', { x1: 14 + px, y1: h - 24, x2: 14 + px, y2: h - 12 }));
    const tx = svg('text', { x: 14, y: h - 28 }); tx.textContent = `${metres} m`; tx.id = 'scale-label'; g.append(tx);
    $('layer-scale').replaceChildren(g);
  }

  // ----- selection / inspector / preview -----
  function renderSelection() {
    const n = S.selected.size;
    const ids = [...S.selected].sort();
    $('sel-summary').textContent = n === 0 ? 'No trees selected.' : `${n} tree${n === 1 ? '' : 's'} selected: ${ids.slice(0, 12).join(', ')}${n > 12 ? ` … (+${n - 12} more)` : ''}`;
    $('sel-summary').dataset.ids = ids.join(',');
    const tId = ids.length ? ids[ids.length - 1] : null;
    const t = S.lastClicked && S.selected.has(S.lastClicked) ? S.treesById.get(S.lastClicked) : (tId ? S.treesById.get(tId) : null);
    renderInspector(t);
  }

  function renderInspector(t) {
    const el = $('inspector-body');
    if (!t) { el.textContent = 'Click a tree on the plot.'; el.dataset.treeId = ''; return; }
    el.dataset.treeId = t.treeId;
    const rows = [
      ['Tree ID', t.treeId], ['Species', t.species], ['Position', `x ${t.xMetres.toFixed(3)} m, y ${t.yMetres.toFixed(3)} m`],
      ['DBH', `${t.dbhCm.toFixed(1)} cm`], ['Height', `${t.heightM.toFixed(2)} m`],
      ['Age at inventory', `${t.ageAtInventoryYears} years (never changes in P0)`],
      ['Biological status', STATUS_LABEL[t.status] + (t.belowThreshold ? ' — below adult threshold: shown but not actionable' : '')],
      ['Mark state', t.markState],
      ['Plan', t.planId ? `${t.planId} (${t.planState})` : 'none'],
      ['Eligible living adult', t.eligible ? 'yes' : 'no'],
      ['Provenance', 'SYNTHETIC TEST FOREST — engineering fixture'],
    ];
    const tbl = document.createElement('table'); tbl.className = 'kv';
    for (const [k, v] of rows) { const tr = tbl.insertRow(); const th = document.createElement('th'); th.textContent = k; const td = tr.insertCell(); td.textContent = v; td.dataset.field = k; tr.prepend(th); }
    el.replaceChildren(tbl);
  }

  async function updatePreview() {
    const box = $('preview');
    const ids = [...S.selected].sort();
    const seq = ++S.previewSeq;
    if (!ids.length) { box.className = 'preview'; box.textContent = 'Select trees to see Core-computed removal preview.'; box.dataset.valid = ''; return; }
    box.className = 'preview'; box.dataset.valid = 'pending'; box.textContent = 'Updating Core preview…'; // never show a stale answer for a new selection
    const p = await api('/api/preview', { treeIds: ids });
    if (seq !== S.previewSeq) return; // a newer selection superseded this answer
    renderPreview(box, p);
  }

  function renderPreview(box, p, title) {
    box.dataset.valid = String(p.valid);
    if (!p.valid) {
      box.className = 'preview bad';
      box.textContent = `${title ? title + ': ' : ''}Not a valid removal: ${p.invalidReason}${p.invalidIds && p.invalidIds.length ? ' — ' + p.invalidIds.slice(0, 8).join(', ') : ''}.`;
      return;
    }
    box.className = 'preview';
    box.dataset.removedN = p.removedN; box.dataset.removedBa = p.removedBaM2; box.dataset.afterN = p.after.eligibleLiveN; box.dataset.afterBa = p.after.baM2;
    box.textContent = `${title ? title + ': ' : ''}if removed → N ${p.before.eligibleLiveN} → ${p.after.eligibleLiveN} (−${p.removedN}); ` +
      `BA ${f(p.before.baM2, 4)} → ${f(p.after.baM2, 4)} m² (−${f(p.removedBaM2, 4)} m², ${p.removedBaFractionOfLiveStand === null ? 'n/a' : f(p.removedBaFractionOfLiveStand * 100, 2) + '% of live-stand BA'}); ` +
      `BA/ha ${f(p.after.baM2PerHa, 4)}; Dq ${p.after.dqCm === null ? 'not applicable' : f(p.after.dqCm, 2) + ' cm'}. (Preview only — nothing changes until Execute Test Work.)`;
  }

  async function select(ids, additive, clicked) {
    const t0 = performance.now();
    if (!additive) S.selected.clear();
    for (const id of ids) { if (additive && S.selected.has(id)) S.selected.delete(id); else S.selected.add(id); }
    S.lastClicked = clicked || ids[ids.length - 1] || null;
    renderMap(); renderSelection();
    await updatePreview();
    const ms = performance.now() - t0;
    const pl = $('perf-line'); pl.dataset.ms = ms.toFixed(1); pl.textContent = `Interaction latency (selection + inspector + Core preview): ${ms.toFixed(1)} ms`;
    window.__sitkaLastInteractionMs = ms;
  }

  function renderMarkedHint() {
    const free = S.state.trees.filter((t) => t.markState === 'PLAYER_MARKED' && !t.planId);
    $('marked-hint').textContent = `${free.length} marked tree${free.length === 1 ? '' : 's'} not yet in a plan. Marking changes nothing in the forest.`;
    $('marked-hint').dataset.freeMarked = free.map((t) => t.treeId).join(',');
  }

  // ----- plans -----
  const ACTIONS = [
    ['propose', 'Propose', 'canPropose'], ['approve', 'Approve Experiment Plan', 'canApprove'], ['queue', 'Queue Test Work', 'canQueue'],
    ['execute', 'Execute Test Work', 'canExecute'], ['reject', 'Reject', 'canReject'], ['cancel', 'Cancel', 'canCancel'],
  ];

  function renderPlans() {
    const list = $('plan-list');
    const plans = S.state.plans;
    if (!plans.length) { list.replaceChildren(Object.assign(document.createElement('p'), { className: 'hint', textContent: 'No plans yet.' })); return; }
    const frag = document.createDocumentFragment();
    for (const p of [...plans].reverse()) {
      const d = document.createElement('div'); d.className = 'plan st-' + p.planState; d.dataset.planId = p.planId; d.dataset.planState = p.planState;
      const head = document.createElement('div');
      head.innerHTML = '';
      const strong = document.createElement('strong'); strong.textContent = p.planId + ' ';
      const stSpan = document.createElement('span'); stSpan.className = 'state'; stSpan.textContent = p.planState;
      head.append(strong, stSpan, document.createTextNode(` · ${p.treeIds.length} trees · base revision ${p.baseForestRevision === null ? '—' : p.baseForestRevision}`));
      const ids = document.createElement('div'); ids.className = 'ids'; ids.textContent = p.treeIds.join(', ');
      d.append(head, ids);
      if (p.preview) { const pv = document.createElement('div'); pv.className = 'preview'; renderPreview(pv, p.preview, 'Core preview'); d.append(pv); }
      const acts = document.createElement('div'); acts.className = 'acts';
      for (const [a, label, flag] of ACTIONS) {
        const b = document.createElement('button'); b.type = 'button'; b.textContent = label; b.dataset.action = a; b.dataset.plan = p.planId; b.id = `plan-${p.planId}-${a}`;
        if (!p.actions[flag]) { b.classList.add('invalid-now'); b.title = 'Not valid in the current state — the server will refuse and explain (and, for Execute, record an audit-only event).'; }
        acts.append(b);
      }
      d.append(acts);
      frag.append(d);
    }
    list.replaceChildren(frag);
  }

  function renderEvents() {
    $('event-count').textContent = `(${S.state.eventCount} total, newest first)`;
    const ol = $('event-list');
    ol.replaceChildren(...[...S.state.recentEvents].reverse().map((e) => {
      const li = document.createElement('li'); li.value = e.eventSeq;
      li.textContent = `${e.eventType}${e.planId ? ' ' + e.planId : ''}${e.reason ? ' [' + e.reason + ']' : ''}${e.treeIds.length ? ' (' + e.treeIds.length + ' trees)' : ''} ${short(e.prevStateHash)}→${short(e.postStateHash)}${e.prevStateHash === e.postStateHash ? ' (audit-only)' : ''}`;
      return li;
    }));
  }

  // ---------- actions ----------
  async function doAction(path, body, okMsg) {
    const r = await api(path, body);
    if (r.state) { setState(r.state); await updatePreview(); }
    say(!!r.ok, r.code, r.message || okMsg || '');
    return r;
  }

  const ids = () => [...S.selected].sort();

  function wire() {
    $('btn-mark').addEventListener('click', () => ids().length ? doAction('/api/marks/mark', { treeIds: ids() }) : say(false, 'EmptySelection', 'Select one or more trees first.'));
    $('btn-unmark').addEventListener('click', () => ids().length ? doAction('/api/marks/unmark', { treeIds: ids() }) : say(false, 'EmptySelection', 'Select one or more trees first.'));
    $('btn-create-plan').addEventListener('click', async () => {
      const free = S.state.trees.filter((t) => t.markState === 'PLAYER_MARKED' && !t.planId).map((t) => t.treeId);
      if (!free.length) return say(false, 'EmptySelection', 'No marked trees outside plans. Mark trees first.');
      const r = await doAction('/api/plans/create', { treeIds: free });
      if (r.ok) say(true, 'ok', `Created ${r.planId} (DRAFT) from ${free.length} marked trees. Nothing in the forest changed.`);
    });
    $('btn-advance').addEventListener('click', () => doAction('/api/step/advance', {}));
    $('plan-list').addEventListener('click', (e) => {
      const b = e.target.closest('button[data-action]'); if (!b) return;
      doAction(`/api/plans/${encodeURIComponent(b.dataset.plan)}/${b.dataset.action}`, {});
    });
    $('btn-reset').addEventListener('click', () => { $('reset-confirm').hidden = false; });
    $('btn-reset-no').addEventListener('click', () => { $('reset-confirm').hidden = true; });
    $('btn-reset-yes').addEventListener('click', async () => {
      $('reset-confirm').hidden = true;
      S.selected.clear(); S.view.fitted = false;
      await doAction('/api/fixture/load', { fixtureId: $('fixture-select').value, confirm: true });
    });
    $('btn-save').addEventListener('click', async () => {
      const r = await api('/api/saves/save', { name: $('save-name').value });
      if (r.files) fillSaves(r.files);
      say(!!r.ok, r.code, r.message);
    });
    $('btn-load').addEventListener('click', async () => {
      const name = $('load-select').value;
      if (!name) return say(false, 'NotListed', 'No lab file selected.');
      S.selected.clear(); S.view.fitted = false;
      await doAction('/api/saves/load', { fileName: name });
    });
    $('btn-replay').addEventListener('click', async () => {
      const r = await api('/api/replay');
      const out = $('replay-result');
      out.dataset.ok = String(r.ok);
      out.textContent = r.ok ? `Replay OK: ${r.eventCount} events re-derived from the fixture; final SHA-256 ${short(r.finalHash)} ${r.matchesCurrentState ? 'matches' : 'DOES NOT match'} the current state.` : `Replay FAILED: ${r.error}`;
      say(!!r.ok, r.ok ? 'ok' : 'ReplayMismatch', out.textContent);
    });
    $('btn-export').addEventListener('click', async () => {
      const res = await fetch('/api/export'); const text = await res.text();
      const url = URL.createObjectURL(new Blob([text], { type: 'application/json' }));
      const a = document.createElement('a'); a.href = url; a.download = `${S.state.fixtureId}-provenance-export.json`; document.body.append(a); a.click(); a.remove();
      setTimeout(() => URL.revokeObjectURL(url), 2000);
      say(true, 'ok', `Exported provenance manifest + ${S.state.eventCount} events + stand summary (${text.length} bytes).`);
    });
    $('id-form').addEventListener('submit', (e) => {
      e.preventDefault();
      const id = $('id-input').value.trim();
      if (!S.treesById.has(id)) return say(false, 'TreeMissing', `No tree with ID "${id}" in this forest.`);
      select([id], false, id);
    });
    $('sel-clear').addEventListener('click', () => select([], false, null));
    for (const id of ['show-below', 'show-dead', 'show-felled']) $(id).addEventListener('change', renderMap);

    // map interaction: click selects, shift/ctrl/meta-click toggles, drag pans, wheel zooms
    const map = mapEl();
    let drag = null;
    map.addEventListener('pointerdown', (e) => { drag = { x: e.clientX, y: e.clientY, ox: S.view.ox, oy: S.view.oy, moved: false }; map.setPointerCapture(e.pointerId); });
    map.addEventListener('pointermove', (e) => {
      const r = map.getBoundingClientRect(); const m = toMetres(e.clientX - r.left, e.clientY - r.top);
      $('cursor-readout').textContent = `x ${m.x.toFixed(2)} m, y ${m.y.toFixed(2)} m`;
      if (!drag) return;
      const dx = e.clientX - drag.x, dy = e.clientY - drag.y;
      if (!drag.moved && Math.hypot(dx, dy) > 5) drag.moved = true;
      if (drag.moved) { S.view.ox = drag.ox + dx; S.view.oy = drag.oy + dy; repositionOnly(); }
    });
    map.addEventListener('pointerup', (e) => {
      const d = drag; drag = null;
      try { map.releasePointerCapture(e.pointerId); } catch { /* not captured */ }
      if (!d || d.moved) return;
      const el = document.elementFromPoint(e.clientX, e.clientY);
      const g = el && el.closest ? el.closest('.tree') : null;
      if (g) select([g.dataset.treeId], e.shiftKey || e.ctrlKey || e.metaKey, g.dataset.treeId);
      else if (!(e.shiftKey || e.ctrlKey || e.metaKey)) select([], false, null);
    });
    map.addEventListener('wheel', (e) => { e.preventDefault(); zoomAt(e.deltaY < 0 ? 1.25 : 0.8, e.clientX, e.clientY); }, { passive: false });
    $('zoom-in').addEventListener('click', () => zoomAt(1.4));
    $('zoom-out').addEventListener('click', () => zoomAt(1 / 1.4));
    $('zoom-fit').addEventListener('click', () => { fitView(); repositionOnly(); });
    window.addEventListener('resize', () => { if (S.state) { fitView(); repositionOnly(); } });
  }

  function zoomAt(factor, cx, cy) {
    const r = mapEl().getBoundingClientRect();
    const px = cx === undefined ? r.width / 2 : cx - r.left, py = cy === undefined ? r.height / 2 : cy - r.top;
    const m = toMetres(px, py);
    S.view.scale = Math.min(2000, Math.max(0.05, S.view.scale * factor));
    S.view.ox = px - (m.x - S.state.minXMetres) * S.view.scale;
    S.view.oy = py - (S.state.maxYMetres - m.y) * S.view.scale;
    repositionOnly();
  }

  wire();
  refresh().then(() => say(true, 'ok', 'Ready. This is a SYNTHETIC TEST FOREST with NO BIOLOGICAL GROWTH.')).catch((e) => say(false, 'LoadFailed', String(e)));
})();
