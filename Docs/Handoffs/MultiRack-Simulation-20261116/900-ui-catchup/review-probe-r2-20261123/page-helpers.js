(() => {
  if (window.__rv) return 'already';
  // ---- the page's own fetch is wrapped; the reviewer's side-channel calls use window.__real and are never counted ----
  window.__real = window.fetch.bind(window);
  window.__reqs = [];        // every request the PAGE itself made (relative URL as the page wrote it)
  window.__mode = null;      // one-shot fault for the next api/multi/hit: 'hold-ans' | 'hold-req' | 'guard' | 'transport'
  window.__held = 0;         // number of held/forced hit-test calls reached
  window.__open = null;      // releases the current hold
  window.__mutateView = null;// one-shot: function(json) applied to the next api/multi/view answer
  window.fetch = async (input, init) => {
    const url = String(input);
    const method = (init && init.method) || 'GET';
    window.__reqs.push(method + ' ' + url);
    if (url.endsWith('api/multi/hit') && window.__mode) {
      const mode = window.__mode;
      window.__mode = null;
      const gate = new Promise((resolve) => { window.__open = resolve; });
      if (mode === 'hold-ans') { const r = await window.__real(input, init); window.__held += 1; await gate; return r; }
      if (mode === 'hold-req') { window.__held += 1; await gate; return window.__real(input, init); }
      if (mode === 'guard') { window.__held += 1; await gate; return new Response(JSON.stringify({ error: { code: 'ProbeGuard', message: 'probe guard answer' } }), { status: 403, headers: { 'Content-Type': 'application/json' } }); }
      if (mode === 'transport') { window.__held += 1; await gate; throw new TypeError('probe transport failure'); }
    }
    const r = await window.__real(input, init);
    if (url.endsWith('api/multi/view') && window.__mutateView) {
      const f = window.__mutateView; window.__mutateView = null;
      const j = await r.json(); f(j);
      return new Response(JSON.stringify(j), { status: r.status, headers: { 'Content-Type': 'application/json' } });
    }
    return r;
  };

  const rv = {};
  rv.view = async () => (await window.__real('api/multi/view', { cache: 'no-store' })).json();
  rv.hash = (s) => { let h1 = 0xdeadbeef, h2 = 0x41c6ce57; for (let i = 0; i < s.length; i++) { const ch = s.charCodeAt(i); h1 = Math.imul(h1 ^ ch, 2654435761); h2 = Math.imul(h2 ^ ch, 1597334677); }
    h1 = Math.imul(h1 ^ (h1 >>> 16), 2246822507) ^ Math.imul(h2 ^ (h2 >>> 13), 3266489909); h2 = Math.imul(h2 ^ (h2 >>> 16), 2246822507) ^ Math.imul(h1 ^ (h1 >>> 13), 3266489909);
    return (4294967296 * (2097151 & h2) + (h1 >>> 0)).toString(16); };
  rv.fence = (v) => ({ sessionId: v.sessionId, source: v.source, planStateRevision: v.planStateRevision });
  rv.snap = async () => { const p = await rv.view(); const v = p.view;
    return { year: v.source.ecologicalYear, rev: v.planStateRevision, work: v.source.workRevision, pending: v.pending.map((o) => o.areaId + ':' + o.treeId + ':' + o.reason),
      approved: v.approved ? v.approved.orders.map((o) => o.treeId) : null, events: v.events.length, crops: v.areas.map((a) => a.cropTreeIds), living: v.areas.map((a) => a.stand.livingCount),
      hash: rv.hash(JSON.stringify(v)) }; };
  rv.post = async (path, body) => { const r = await window.__real(path, { method: 'POST', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify(body) });
    let j = null; try { j = await r.json(); } catch { j = null; } return { status: r.status, json: j }; };
  rv.hit = async (areaId, u, v) => { const p = await rv.view(); const r = await rv.post('api/multi/hit', { fence: rv.fence(p.view), areaId, u, v }); return r.json && r.json.candidates ? r.json.candidates.map((c) => c.treeId) : null; };
  rv.cropMark = async (areaId, treeId) => { const p = await rv.view(); return rv.post('api/multi/crop/mark', { fence: rv.fence(p.view), areaId, treeId }); };
  rv.topFive = async (areaId, treeId) => { const p = await rv.view(); const r = await rv.post('api/multi/competition', { fence: rv.fence(p.view), areaId, treeId });
    return r.json && r.json.competition ? r.json.competition.topFive.map((n) => n.treeId) : null; };
  rv.screenOf = (u, v) => { const m = document.getElementById('map').getScreenCTM(); const x = m.a * u + m.c * v + m.e, y = m.b * u + m.d * v + m.f;
    const e = document.elementFromPoint(x, y);
    return { x, y, inView: x > 2 && y > 2 && x < innerWidth - 2 && y < innerHeight - 2, onMap: !!(e && e.closest && e.closest('#map')),
      onCrown: !!(e && e.closest && e.closest('#crowns')), onRank: !!(e && e.closest && e.closest('[data-rank-toggle]')), top: e ? (e.id || e.localName) : null }; };
  rv.inspected = () => (document.getElementById('inspector-body').hidden ? null : document.getElementById('insp-id').textContent);
  rv.area = () => document.getElementById('map-subtitle').textContent.slice(0, 6);
  rv.reach = (e) => { if (!e) return false; const b = e.getBoundingClientRect(); if (b.width === 0 || b.height === 0) return false;
    const x = b.left + b.width / 2, y = b.top + b.height / 2; if (x < 0 || y < 0 || x > innerWidth || y > innerHeight) return false;
    const t = document.elementFromPoint(x, y); return !!t && (e === t || e.contains(t)); };
  rv.fold = () => {
    const pane = document.getElementById('pane-tree'); const pr = pane.getBoundingClientRect();
    const rows = [...document.querySelectorAll('#competition-content .comp-row')];
    const dr = document.querySelector('.add-dock').getBoundingClientRect();
    const add = document.getElementById('add-button'); const ar = add.getBoundingClientRect();
    const ref = document.querySelector('.ref-card').getBoundingClientRect();
    const rr = rows.map((r) => r.getBoundingClientRect());
    const g = document.getElementById('ground-panel'); const gr = g.getBoundingClientRect();
    const closeBtn = document.getElementById('inspect-close');
    const cb = closeBtn.getBoundingClientRect();
    const blocked = [...document.querySelectorAll('#competition-content [data-draft]')].filter((i) => i.disabled).length;
    return { blocked, closeRight: Math.round(cb.right), paneClientRight: Math.round(pr.left + pane.clientWidth), rowsReachableCount: rows.filter((r) => rv.reach(r)).length, n: rows.length, scrollTop: pane.scrollTop, paneTop: Math.round(pr.top), paneBottom: Math.round(pr.bottom), innerHeight,
      refTop: Math.round(ref.top), refBottom: Math.round(ref.bottom), firstTop: rows.length ? Math.round(rr[0].top) : null, lastBottom: rows.length ? Math.round(rr[rr.length - 1].bottom) : null,
      rowH: rows.length ? Math.round(rr[0].height) : null, dockTop: Math.round(dr.top), dockBottom: Math.round(dr.bottom),
      addTop: Math.round(ar.top), addBottom: Math.round(ar.bottom), addH: Math.round(ar.height), addText: add.textContent.trim(),
      formHidden: document.getElementById('add-form').hidden, addDisabled: add.disabled,
      rowsReachable: rows.every((r) => rv.reach(r)), addReachable: rv.reach(add), closeReachable: rv.reach(closeBtn), refReachable: rv.reach(document.getElementById('insp-id')),
      groundHidden: g.hidden, groundParent: g.parentNode ? g.parentNode.id : null, groundTop: Math.round(gr.top), groundBottom: Math.round(gr.bottom) };
  };
  rv.ranks = () => [...document.querySelectorAll('#rank-layer [data-rank-toggle]')].map((b) => { const r = b.getBoundingClientRect();
    return { id: b.dataset.rankToggle, rank: Number(b.dataset.rank), pressed: b.getAttribute('aria-pressed'), disabled: b.getAttribute('aria-disabled'), label: b.getAttribute('aria-label'),
      w: Math.round(r.width), h: Math.round(r.height), x: r.left + r.width / 2, y: r.top + r.height / 2, tag: b.localName }; });
  rv.rows = () => [...document.querySelectorAll('#competition-content [data-draft]')].map((i) => ({ id: i.dataset.draft, checked: i.checked, disabled: i.disabled }));
  rv.state = () => ({ insp: rv.inspected(), area: rv.area(), announce: document.getElementById('announce').textContent, alert: document.getElementById('alert').textContent,
    alertShown: document.getElementById('alert').classList.contains('shown'), add: document.getElementById('add-button').textContent.trim(),
    ranksHidden: document.getElementById('rank-layer').hidden, zoom: document.getElementById('zoom-state').textContent, vb: document.getElementById('map').getAttribute('viewBox'),
    year: document.getElementById('year-value') ? document.getElementById('year-value').textContent : null,
    light: document.getElementById('light-toggle').getAttribute('aria-pressed'), ground: document.getElementById('ground-mode').getAttribute('aria-pressed') });
  window.__rv = rv;
  return 'installed';
})()
