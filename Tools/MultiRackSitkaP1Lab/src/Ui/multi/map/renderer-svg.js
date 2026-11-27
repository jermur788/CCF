// S0c provenance: map/renderer-svg.js [L3] — section(s) 8, 9, 11 from multi.js
import { announce } from '../api.js';
import { el, fixed, label, n3, plural, setAttributes, svg } from '../dom.js';
import { SAPLING_SYMBOL, SLATE, SPECIES_SYMBOL, SPOT_PREVIEW_RADIUS_M, allAreas, approvedClears, approvedPlantings, approvedPrunes, areaName, areaView, blockedReason, byId, compactCellsFor, currentArea, currentJuvenile, currentLens, currentReport, draftOf, groundCell, hooks, isMultiLaneStand, itemOf, lightPaint, opacityText, pendingPlantings, pruneStatusOf, referenceIn, s4Area, sceneOf, selectedId, speciesLabel, speciesOf, state, visibleLaneIds } from '../store.js';
import { MAP_RENDERER_METHODS } from './contract.js';
  // ---- the map: 1 SVG user unit = 1 metre; U east, V south ------------------------------------------------------------

  let lastMultiFrame = null;

  function visibleAreasNow() {
    if (!state.view || !isMultiLaneStand()) return null;
    const ids = visibleLaneIds(allAreas(), state.areaId, state.singleLaneMode, window.innerWidth);
    const list = ids.map((id) => areaView(id)).filter(Boolean);
    return list.length > 0 ? list : null;
  }

  function multiLaneBounds(visAreas) {
    const pts = visAreas.flatMap((a) => a.polygon || []);
    const minX = Math.min(...pts.map((p) => p.xm));
    const maxX = Math.max(...pts.map((p) => p.xm));
    const minZ = Math.min(...pts.map((p) => p.zm));
    const maxZ = Math.max(...pts.map((p) => p.zm));
    return { minX, maxX, minZ, maxZ, w: Math.max(1, maxX - minX), h: Math.max(1, maxZ - minZ) };
  }

  export function resolveMapPoint(svgX, svgY) {
    if (!lastMultiFrame) {
      return { area: currentArea(), u: svgX, v: svgY };
    }
    const worldX = svgX;
    const worldZ = lastMultiFrame.maxZ - svgY;
    const hit = lastMultiFrame.entries.find((e) =>
      worldX >= e.minX - 1e-6 && worldX <= e.maxX + 1e-6 && worldZ >= e.minZ - 1e-6 && worldZ <= e.maxZ + 1e-6)
      || lastMultiFrame.entries.find((e) => e.area.areaId === state.areaId)
      || lastMultiFrame.entries[0];
    return { area: hit.area, u: svgX - hit.offX, v: svgY - hit.offY };
  }

  function fullBox(area) {
    const vis = visibleAreasNow();
    if (vis) {
      const b = multiLaneBounds(vis);
      return { x: b.minX - 3.6, y: -5.2, w: b.w + 7.2, h: b.h + 9.2 };
    }
    const side = area.extent.sideM;
    return { x: -3.4, y: -2.2, w: side + 6.0, h: side + 5.4 };
  }

  function mapFrame(area) {
    const full = fullBox(area);
    const pane = byId('map-pane');
    const dock = byId('map-dock');
    const W = Math.max(pane.clientWidth, 1);
    const H = Math.max(pane.clientHeight, 1);
    const dockRight = dock.offsetLeft + dock.offsetWidth + 10;
    const dockBottom = dock.offsetTop + dock.offsetHeight + 10;
    const besideScale = Math.min((W - dockRight) / full.w, H / full.h);
    const belowScale = Math.min(W / full.w, (H - dockBottom) / full.h);
    const beside = besideScale >= belowScale;
    return { full, W, H, fit: Math.max(beside ? besideScale : belowScale, 1e-3),
      reserveLeft: beside ? dockRight : 0, reserveTop: beside ? 0 : dockBottom };
  }

  function viewAxis(centre, usable, reserve, start, length) {
    if (usable >= length - 1e-9) return { pos: start - reserve - (usable - length) / 2, back: false, forward: false };
    const lo = start - reserve;
    const hi = start + length - usable - reserve;
    const pos = Math.min(hi, Math.max(lo, centre - usable / 2 - reserve));
    return { pos, back: pos > lo + 1e-6, forward: pos < hi - 1e-6 };
  }

  export function viewBox(area) {
    const frame = mapFrame(area);
    const full = frame.full;
    const scale = frame.fit * state.zoom;
    const w = frame.W / scale, h = frame.H / scale;
    const rl = frame.reserveLeft / scale, rt = frame.reserveTop / scale;
    const uw = w - rl, uh = h - rt;
    const centre = state.centre || { u: full.x + full.w / 2, v: full.y + full.h / 2 };
    const ax = viewAxis(centre.u, uw, rl, full.x, full.w);
    const ay = viewAxis(centre.v, uh, rt, full.y, full.h);
    return { x: ax.pos, y: ay.pos, w, h, scale, uw, uh, rl, rt,
      west: ax.back, east: ax.forward, north: ay.back, south: ay.forward };
  }

  export function usableCentre(box) { return { u: box.x + box.rl + box.uw / 2, v: box.y + box.rt + box.uh / 2 }; }

  const FOCUS_SCALE = 2;
  const FOCUS_GAP_PX = 0;
  const FOCUS_TOUCH = .8;
  function spreadFocus(areaId, report, mpp, box) {
    const ids = [report.targetTreeId, ...report.topFive.map((n) => n.treeId)];
    const base = ids.map((id) => itemOf(areaId, id)).filter(Boolean);
    if (!base.length || base[0].treeId !== report.targetTreeId) return new Map();
    const lo = { u: box.x + box.rl + 34 * mpp, v: box.y + box.rt + 34 * mpp };
    const hi = { u: box.x + box.w - 34 * mpp, v: box.y + box.h - 48 * mpp };
    const nodes = base.map((item) => ({ ...item, radiusM: item.radiusM * FOCUS_SCALE }));
    const within = (x, a, b) => (a > b ? (a + b) / 2 : Math.min(Math.max(x, a), b));
    for (let round = 0; round < 8; round++) {
      separate(nodes, FOCUS_GAP_PX * mpp);
      let held = false;
      for (const n of nodes.slice(1)) {
        const u = within(n.u, lo.u + n.radiusM, hi.u - n.radiusM), v = within(n.v, lo.v + n.radiusM, hi.v - n.radiusM);
        if (u !== n.u || v !== n.v) { n.u = u; n.v = v; held = true; }
      }
      if (!held) break;
    }
    return new Map(nodes.map((node) => [node.treeId, node]));
  }

  function separate(nodes, pad) {
    const anchor = nodes[0];
    for (let pass = 0; pass < 80; pass++) {
      let moved = false;
      for (let i = 0; i < nodes.length; i++) for (let j = i + 1; j < nodes.length; j++) {
        const a = nodes[i], b = nodes[j];
        let dx = b.u - a.u, dy = b.v - a.v, d = Math.hypot(dx, dy);
        if (d < 1e-6) { dx = Math.cos(j * 2.399); dy = Math.sin(j * 2.399); d = 0; } else { dx /= d; dy /= d; }
        const need = (a.radiusM + b.radiusM) * FOCUS_TOUCH + pad - d;
        if (need <= 0) continue;
        moved = true;
        const shareA = a === anchor ? 0 : b === anchor ? 1 : .5;
        a.u -= dx * need * shareA; a.v -= dy * need * shareA;
        b.u += dx * need * (1 - shareA); b.v += dy * need * (1 - shareA);
      }
      if (!moved) break;
    }
    return nodes;
  }

  const crownCache = new Map();

  function canopyHash(a, b, salt) {
    let h = Math.imul(a ^ 0x9e3779b1, 0x85ebca77) ^ Math.imul(b ^ salt, 0xc2b2ae3d);
    h = Math.imul(h ^ (h >>> 15), 0x2c1b3c6d);
    h = Math.imul(h ^ (h >>> 12), 0x297a2d39);
    return (h ^ (h >>> 15)) >>> 0;
  }
  function canopySeed(id) {
    let seed = 7;
    for (const ch of String(id || 'legend')) seed = canopyHash(seed, ch.charCodeAt(0), 7);
    return seed;
  }
  function canopyRandom(seed, channel) { return (canopyHash(seed, channel, 2) & 0xffffff) / 16777216; }

  function groundNoise(x, y, cells, seed) {
    const fx = x * cells, fy = y * cells;
    const ix = Math.floor(fx), iy = Math.floor(fy);
    const smooth = (t) => t * t * (3 - 2 * t);
    const tx = smooth(fx - ix), ty = smooth(fy - iy);
    const at = (a, b) => (canopyHash(a % cells, b % cells, seed) & 0xffffff) / 16777216;
    const lerp = (a, b, t) => a + (b - a) * t;
    return lerp(lerp(at(ix, iy), at(ix + 1, iy), tx), lerp(at(ix, iy + 1), at(ix + 1, iy + 1), tx), ty);
  }

  function initFloorDefs() {
    const defs = byId('map').querySelector('defs');
    if (byId('slate-ground')) return;
    [12.8, 9.7].forEach((period, index) => {
      const tile = svg('pattern', { id: 'slate-' + index, patternUnits: 'userSpaceOnUse', width: period, height: period });
      const seed = index === 0 ? 0x51ed270b : 0x9e3779b9;
      const size = 24, cell = period / size;
      for (let y = 0; y < size; y++) for (let x = 0; x < size; x++) {
        const n = 255 * (.65 * groundNoise(x / size, y / size, 14, seed) +
          .35 * groundNoise(x / size, y / size, 5, seed ^ 0xabcd)) - 128;
        const colour = SLATE.map((v) => Math.round(v + n / 14));
        tile.append(svg('rect', { x: n3(x * cell), y: n3(y * cell), width: n3(cell + .005), height: n3(cell + .005),
          fill: 'rgb(' + colour.join(',') + ')' }));
      }
      defs.append(tile);
    });
    const floor = svg('pattern', { id: 'slate-ground', patternUnits: 'userSpaceOnUse', width: 124.16, height: 124.16 });
    floor.append(svg('rect', { width: 124.16, height: 124.16, fill: 'url(#slate-0)' }),
      svg('rect', { width: 124.16, height: 124.16, fill: 'url(#slate-1)', opacity: .4 }));
    defs.append(floor);
  }

  // Strictly circular crown geometry (spec §5.6): x = u - r, y = v - r, width = height = 2 * r, where r = item.radiusM.
  // Zero elliptical stretch; zero DBH->radius maths in JS. Deterministic seeded rotation around (u, v) is allowed.
  function crownDecoration(item, sp) {
    const u = item.u, v = item.v, r = item.radiusM;
    if (item.status === 'Felled') {
      return [
        svg('circle', { class: 'stump-disc deco', cx: n3(u), cy: n3(v), r: n3(r) }),
        svg('circle', { class: 'stump-ring deco', cx: n3(u), cy: n3(v), r: n3(r * .65) }),
        svg('circle', { class: 'stump-ring deco', cx: n3(u), cy: n3(v), r: n3(r * .32) }),
      ];
    }
    const seed = canopySeed(item.treeId);
    const turn = Math.round(canopyRandom(seed, 21) * 360);
    const dead = item.status === 'NaturalDead';
    const symbolId = dead ? 'ccf-standing-dead' : (SPECIES_SYMBOL[sp] || 'ccf-sitka');
    const diam = n3(2 * r);
    return [
      svg('circle', { class: 'deco', cx: n3(u + r * .16), cy: n3(v + r * .18), r: n3(r * 1.08),
        fill: 'url(#canopy-shadow)', opacity: dead ? .4 : .85 }),
      svg('use', {
        class: 'deco crown-art',
        href: '#' + symbolId,
        x: n3(u - r),
        y: n3(v - r),
        width: diam,
        height: diam,
        transform: 'rotate(' + turn + ' ' + n3(u) + ' ' + n3(v) + ')',
        'data-radius-m': n3(r),
        'data-species': sp,
      }),
    ];
  }

  function renderMap() {
    const area = currentArea();
    if (!area) return;
    const ws = byId('workspace');
    if (ws) ws.setAttribute('data-lens', state.lens);
    const visAreas = visibleAreasNow();
    const mapPane = byId('map-pane');
    if (mapPane) mapPane.setAttribute('data-lens', state.lens);
    if (visAreas) {
      if (mapPane) mapPane.classList.add('multi-lane-exterior');
      renderMultiLaneMap(area, visAreas);
      return;
    }
    lastMultiFrame = null;
    if (mapPane) mapPane.classList.remove('multi-lane-exterior');
    const laneBorders = byId('lane-borders');
    if (laneBorders) laneBorders.replaceChildren();
    const side = area.extent.sideM;
    const box = viewBox(area);
    const mpp = 1 / box.scale;
    byId('map').setAttribute('viewBox', [box.x, box.y, box.w, box.h].map((n) => n.toFixed(4)).join(' '));
    setAttributes(byId('ground'), { x: 0, y: 0, width: side, height: side });
    const toU = (x) => x - area.extent.minXM;
    const toV = (z) => area.extent.minZM + side - z;
    const outline = area.polygon.map((p) => toU(p.xm) + ',' + toV(p.zm)).join(' ');
    byId('area-outline').setAttribute('points', outline);
    byId('area-clip-shape').setAttribute('points', outline);
    drawGrid(area, mpp, box.scale);

    const inTreeMode = state.mapMode === 'tree';
    const report = inTreeMode ? currentReport() : null;
    const reference = referenceIn(area.areaId);
    const draft = new Set(draftOf(area.areaId));
    const focusIds = report ? new Set([report.targetTreeId, ...report.topFive.map((n) => n.treeId)]) : null;
    const crowns = [];
    const focusCrowns = [];
    const marks = [];
    const ranks = [];
    const spread = focusIds ? spreadFocus(area.areaId, report, mpp, box) : new Map();
    const items = new Map(sceneOf(area.areaId).map((item) => [item.treeId, spread.get(item.treeId) || item]));
    for (const [treeId, shown] of spread) {
      const real = items.get(treeId) === shown ? itemOf(area.areaId, treeId) : null;
      if (real && Math.hypot(shown.u - real.u, shown.v - real.v) > 2 * mpp) {
        marks.push(svg('path', { class: 'spread-leader', d: 'M ' + n3(real.u) + ' ' + n3(real.v) + ' L ' + n3(shown.u) + ' ' + n3(shown.v) }),
          svg('circle', { class: 'spread-origin', cx: n3(real.u), cy: n3(real.v), r: n3(2.5 * mpp) }));
      }
    }
    for (const raw of sceneOf(area.areaId).slice().sort((a, b) => a.paintOrder - b.paintOrder)) {
      const item = items.get(raw.treeId);
      const sp = speciesOf(item.treeId);
      const focused = item !== raw;
      const dimmed = focusIds && !focused;
      const cacheKey = area.areaId + '|' + item.treeId;
      const drawKey = [item.status, sp, n3(item.u), n3(item.v), n3(item.radiusM)].join('|');
      let cached = crownCache.get(cacheKey);
      if (!cached || cached.drawKey !== drawKey) {
        const group = svg('g', { 'data-tree-id': item.treeId, 'data-species': sp });
        // Transparent true-radius hit disc preserved underneath crown art
        group.append(svg('circle', { class: 'crown', cx: n3(item.u), cy: n3(item.v), r: n3(item.radiusM) }));
        group.append(...crownDecoration(item, sp));
        cached = { drawKey, node: group };
        crownCache.set(cacheKey, cached);
      }
      const className = 'tree s-' + item.status + (dimmed ? ' dimmed' : '');
      if (cached.node.getAttribute('class') !== className) cached.node.setAttribute('class', className);
      (focused ? focusCrowns : crowns).push(cached.node);
      marks.push(...statusMarks(item, mpp), ...pruneMarks(area.areaId, item, mpp));
    }
    if (report) {
      const taken = [];
      const focus = [...spread.values()];
      for (const f of focus) {
        if (f.isCrop) taken.push(tileCentre(f, TILE.crop, mpp));
        if (f.status === 'PendingFell' || f.status === 'ApprovedFell') taken.push(tileCentre(f, TILE.status, mpp));
      }
      const target = items.get(report.targetTreeId);
      if (target) {
        taken.push(tileCentre(target, TILE.reference, mpp));
        for (const dx of [-30, -10, 10, 30]) taken.push([target.u + dx * mpp, target.v - target.radiusM - 22 * mpp]);
      }
      const free = (x, y) => taken.every(([tx, ty]) => Math.hypot(tx - x, ty - y) >= 34 * mpp) &&
        focus.every((f) => Math.hypot(f.u - x, f.v - y) >= f.radiusM * .55);
      report.topFive.forEach((entry, rank) => {
        const item = items.get(entry.treeId);
        if (!item) return;
        const chosen = draft.has(entry.treeId);
        const fits = (a) => { const [x, y] = tileCentre(item, a, mpp); return free(x, y) && (!chosen || free(x - 23 * mpp, y)); };
        const angle = RANK_ANGLES.find(fits) ?? TILE.rank;
        const [x, y] = tileCentre(item, angle, mpp);
        taken.push([x, y]);
        if (chosen) taken.push([x - 23 * mpp, y]);
        marks.push(underline(item, chosen ? 'ring-draft' : 'ring-competitor', mpp, 0));
        ranks.push({ treeId: entry.treeId, rank: rank + 1, x, y, chosen });
      });
    }
    crowns.push(...focusCrowns);
    const host = byId('crowns');
    const unchanged = host.childNodes.length === crowns.length && crowns.every((node, index) => host.childNodes[index] === node);
    if (!unchanged) host.replaceChildren(...crowns);

    renderOverlayCells(area.areaId, mpp);
    renderClearCells(area.areaId);
    renderSpotPatches(area.areaId);
    renderDeadwoodLayer(area.areaId, mpp);
    renderPlantedLayer(area.areaId, mpp);
    renderDraftPreview(area.areaId, mpp);
    renderGroundMark(area.areaId);
    byId('marks').replaceChildren(...marks);
    renderRankLayer(ranks, box);

    const chosen = inTreeMode && !currentJuvenile() && selectedId() ? items.get(selectedId()) : null;
    const lit = chosen && !state.lock ? (chosen.treeId === reference ? referenceMarks(chosen, mpp, true) : inspectMarks(chosen, mpp, true)) : [];
    byId('selection').replaceChildren(...lit);

    byId('map-desc').textContent = 'North-up map of ' + areaName(area.areaId) + ', ' + area.areaM2Exact + ' square metres. ' +
      'Ecological year ' + state.view.source.ecologicalYear + '. Living trees ' + area.stand.livingCount + '.';
    byId('map-subtitle').textContent = area.label + ' · ' + fixed(area.areaHa, 4) + ' ha';
    byId('map-note').textContent = areaName(area.areaId) + ' · ' + (state.zoom === 1 ? 'Fit whole area' :
      fixed(state.zoom, 2) + '× Fit · view ' + fixed(box.uw, 1) + ' × ' + fixed(box.uh, 1) + ' m');
    byId('zoom-state').textContent = state.zoom === 1 ? 'Fit' : fixed(state.zoom, 2) + '×';
    renderScale(box.scale);
    renderMapStatus(reference, draft.size);
  }

  // Phase 2A-2 multi-lane view (>=3 lanes): renders active lane + visible neighbours (2 at outer edges, 3 in interior,
  // or 1 when singleLaneMode is active) in a unified world-metre SVG frame with drawn/labelled lane borders and open
  // grassland outside the woodland perimeter (zero fabricated outside trees).
  function renderMultiLaneMap(activeArea, visAreas) {
    const b = multiLaneBounds(visAreas);
    const box = viewBox(activeArea);
    const mpp = 1 / box.scale;
    byId('map').setAttribute('viewBox', [box.x, box.y, box.w, box.h].map((n) => n.toFixed(4)).join(' '));

    const entries = visAreas.map((a) => {
      const xs = a.polygon.map((p) => p.xm);
      const zs = a.polygon.map((p) => p.zm);
      return {
        area: a,
        offX: a.extent.minXM,
        offY: b.maxZ - (a.extent.minZM + a.extent.sideM),
        minX: Math.min(...xs),
        maxX: Math.max(...xs),
        minZ: Math.min(...zs),
        maxZ: Math.max(...zs),
      };
    });
    lastMultiFrame = { maxZ: b.maxZ, entries };

    // Union polygon of visible lanes for ground fill and clipping.
    const unionPts = [
      n3(b.minX) + ',0',
      n3(b.maxX) + ',0',
      n3(b.maxX) + ',' + n3(b.h),
      n3(b.minX) + ',' + n3(b.h),
    ].join(' ');
    setAttributes(byId('ground'), { x: n3(b.minX), y: 0, width: n3(b.w), height: n3(b.h) });
    byId('area-outline').setAttribute('points', unionPts);
    byId('area-clip-shape').setAttribute('points', unionPts);

    // Draw open-grassland exterior surround + world-metre grid + axis labels.
    drawMultiLaneGrid(b, mpp, box.scale);

    // Draw lane borders and lane labels (active lane emphasized).
    const borderNodes = [];
    for (const e of entries) {
      const isActive = e.area.areaId === state.areaId;
      const pts = e.area.polygon.map((p) => n3(p.xm) + ',' + n3(b.maxZ - p.zm)).join(' ');
      borderNodes.push(svg('polygon', {
        class: 'lane-border' + (isActive ? ' lane-border-active' : ' lane-border-neighbour'),
        'data-lane-border': e.area.areaId,
        points: pts,
      }));
      const cx = (e.minX + e.maxX) / 2;
      const lbl = e.area.label + (isActive ? ' (active)' : '');
      const pillW = (lbl.length * 6.4 + 14) * mpp;
      const pillH = 18 * mpp;
      const py = -14 * mpp;
      const lg = svg('g', { class: 'lane-label-pill' + (isActive ? ' active' : ''), 'data-lane-label': e.area.areaId });
      lg.append(
        svg('rect', { x: n3(cx - pillW / 2), y: n3(py - pillH / 2), width: n3(pillW), height: n3(pillH), rx: n3(4 * mpp) }),
        svg('text', { x: n3(cx), y: n3(py + 0.5 * mpp), 'font-size': n3(10.5 * mpp) }, lbl));
      borderNodes.push(lg);
    }
    const laneBordersHost = byId('lane-borders');
    if (laneBordersHost) laneBordersHost.replaceChildren(...borderNodes);

    const inTreeMode = state.mapMode === 'tree';
    const report = inTreeMode ? currentReport() : null;
    const reference = referenceIn(activeArea.areaId);
    const draft = new Set(draftOf(activeArea.areaId));
    const focusIds = report ? new Set([report.targetTreeId, ...report.topFive.map((n) => n.treeId)]) : null;

    const crownGroups = [];
    const marksGroups = [];
    const ranks = [];
    let activeItems = null;
    let activeEntry = entries.find((e) => e.area.areaId === activeArea.areaId) || entries[0];

    for (const e of entries) {
      const aid = e.area.areaId;
      const isActive = aid === activeArea.areaId;
      const spread = (isActive && focusIds) ? spreadFocus(aid, report, mpp, {
        x: box.x - e.offX, y: box.y - e.offY, w: box.w, h: box.h, rl: box.rl, rt: box.rt,
      }) : new Map();
      const items = new Map(sceneOf(aid).map((item) => [item.treeId, spread.get(item.treeId) || item]));
      if (isActive) activeItems = items;

      const laneCrowns = [];
      const laneFocusCrowns = [];
      const laneMarks = [];

      if (isActive) {
        for (const [treeId, shown] of spread) {
          const real = items.get(treeId) === shown ? itemOf(aid, treeId) : null;
          if (real && Math.hypot(shown.u - real.u, shown.v - real.v) > 2 * mpp) {
            laneMarks.push(
              svg('path', { class: 'spread-leader', d: 'M ' + n3(real.u) + ' ' + n3(real.v) + ' L ' + n3(shown.u) + ' ' + n3(shown.v) }),
              svg('circle', { class: 'spread-origin', cx: n3(real.u), cy: n3(real.v), r: n3(2.5 * mpp) }));
          }
        }
      }

      for (const raw of sceneOf(aid).slice().sort((a, c) => a.paintOrder - c.paintOrder)) {
        const item = items.get(raw.treeId);
        const sp = speciesOf(item.treeId);
        const focused = isActive && item !== raw;
        const dimmed = (isActive && focusIds && !focused) || (!isActive && focusIds !== null);
        const cacheKey = aid + '|' + item.treeId;
        const drawKey = [item.status, sp, n3(item.u), n3(item.v), n3(item.radiusM)].join('|');
        let cached = crownCache.get(cacheKey);
        if (!cached || cached.drawKey !== drawKey) {
          const group = svg('g', { 'data-tree-id': item.treeId, 'data-area-id': aid, 'data-species': sp });
          group.append(svg('circle', { class: 'crown', cx: n3(item.u), cy: n3(item.v), r: n3(item.radiusM) }));
          group.append(...crownDecoration(item, sp));
          cached = { drawKey, node: group };
          crownCache.set(cacheKey, cached);
        }
        const className = 'tree s-' + item.status + (dimmed ? ' dimmed' : '') + (isActive ? '' : ' neighbour-tree');
        if (cached.node.getAttribute('class') !== className) cached.node.setAttribute('class', className);
        (focused ? laneFocusCrowns : laneCrowns).push(cached.node);
        laneMarks.push(...statusMarks(item, mpp), ...pruneMarks(aid, item, mpp));
      }

      if (isActive && report) {
        const taken = [];
        const focus = [...spread.values()];
        for (const f of focus) {
          if (f.isCrop) taken.push(tileCentre(f, TILE.crop, mpp));
          if (f.status === 'PendingFell' || f.status === 'ApprovedFell') taken.push(tileCentre(f, TILE.status, mpp));
        }
        const target = items.get(report.targetTreeId);
        if (target) {
          taken.push(tileCentre(target, TILE.reference, mpp));
          for (const dx of [-30, -10, 10, 30]) taken.push([target.u + dx * mpp, target.v - target.radiusM - 22 * mpp]);
        }
        const free = (x, y) => taken.every(([tx, ty]) => Math.hypot(tx - x, ty - y) >= 34 * mpp) &&
          focus.every((f) => Math.hypot(f.u - x, f.v - y) >= f.radiusM * 0.55);
        report.topFive.forEach((entry, rank) => {
          const item = items.get(entry.treeId);
          if (!item) return;
          const chosen = draft.has(entry.treeId);
          const fits = (a) => { const [x, y] = tileCentre(item, a, mpp); return free(x, y) && (!chosen || free(x - 23 * mpp, y)); };
          const angle = RANK_ANGLES.find(fits) ?? TILE.rank;
          const [x, y] = tileCentre(item, angle, mpp);
          taken.push([x, y]);
          if (chosen) taken.push([x - 23 * mpp, y]);
          laneMarks.push(underline(item, chosen ? 'ring-draft' : 'ring-competitor', mpp, 0));
          ranks.push({ treeId: entry.treeId, rank: rank + 1, x: x + e.offX, y: y + e.offY, chosen });
        });
      }

      laneCrowns.push(...laneFocusCrowns);
      const tx = 'translate(' + n3(e.offX) + ' ' + n3(e.offY) + ')';
      const cg = svg('g', { 'data-lane-crowns': aid, transform: tx });
      cg.append(...laneCrowns);
      crownGroups.push(cg);

      const mg = svg('g', { 'data-lane-marks': aid, transform: tx });
      mg.append(...laneMarks);
      marksGroups.push(mg);
    }

    byId('crowns').replaceChildren(...crownGroups);
    byId('marks').replaceChildren(...marksGroups);

    // Multi-lane translated ground/ecology layers (overlay, clears, spot patches, deadwood, planted, draft, ground mark).
    renderTranslatedLayer('overlay-cells', entries, (aid) => buildOverlayCellNodes(aid, mpp));
    renderTranslatedLayer('clear-cells', entries, (aid) => buildClearCellNodes(aid));
    renderTranslatedLayer('spot-patches', entries, (aid) => buildSpotPatchNodes(aid));
    renderTranslatedLayer('deadwood', entries, (aid) => buildDeadwoodNodes(aid, mpp));
    renderTranslatedLayer('planted-layer', entries, (aid) => buildPlantedNodes(aid, mpp));
    renderTranslatedLayer('draft-preview', entries, (aid) => buildDraftPreviewNodes(aid, mpp));
    renderTranslatedLayer('ground-mark', entries, (aid) => buildGroundMarkNodes(aid));

    renderRankLayer(ranks, box);

    const chosen = inTreeMode && !currentJuvenile() && selectedId() && activeItems ? activeItems.get(selectedId()) : null;
    const lit = chosen && !state.lock
      ? (chosen.treeId === reference ? referenceMarks(chosen, mpp, true) : inspectMarks(chosen, mpp, true))
      : [];
    if (lit.length > 0 && activeEntry) {
      const sg = svg('g', { transform: 'translate(' + n3(activeEntry.offX) + ' ' + n3(activeEntry.offY) + ')' });
      sg.append(...lit);
      byId('selection').replaceChildren(sg);
    } else {
      byId('selection').replaceChildren();
    }

    const visNames = visAreas.map((a) => a.label).join(', ');
    byId('map-desc').textContent = 'North-up multi-lane map showing ' + visNames + ' (active: ' + areaName(activeArea.areaId) + '). ' +
      'Ecological year ' + state.view.source.ecologicalYear + '. Open grassland outside perimeter.';
    byId('map-subtitle').textContent = activeArea.label + ' · ' + (visAreas.length > 1 ? visAreas.length + ' lanes visible' : '1 lane') +
      ' · ' + fixed(activeArea.areaHa, 4) + ' ha';
    byId('map-note').textContent = visNames + ' · ' + (state.zoom === 1 ? 'Fit visible lanes' :
      fixed(state.zoom, 2) + '× Fit · view ' + fixed(box.uw, 1) + ' × ' + fixed(box.uh, 1) + ' m');
    byId('zoom-state').textContent = state.zoom === 1 ? 'Fit' : fixed(state.zoom, 2) + '×';
    renderScale(box.scale);
    renderMapStatus(reference, draft.size);
  }

  function renderTranslatedLayer(hostId, entries, buildFn) {
    const host = byId(hostId);
    if (!host) return;
    const groups = [];
    for (const e of entries) {
      const nodes = buildFn(e.area.areaId);
      if (!nodes || nodes.length === 0) continue;
      const g = svg('g', { transform: 'translate(' + n3(e.offX) + ' ' + n3(e.offY) + ')' });
      g.append(...nodes);
      groups.push(g);
    }
    host.replaceChildren(...groups);
  }

  function drawMultiLaneGrid(b, mpp, scale) {
    const step = 5 * scale >= 26 ? 5 : 10;
    const reach = 120;
    const lines = [];
    const labels = [];
    const font = n3(10 * mpp);
    // Open grassland surround outside the outer stand perimeter (zero fabricated trees).
    lines.push(svg('rect', {
      class: 'grassland-exterior',
      x: n3(b.minX - reach),
      y: n3(-reach),
      width: n3(b.w + 2 * reach),
      height: n3(b.h + 2 * reach),
    }));
    for (let x = Math.ceil((b.minX - reach) / step) * step; x <= b.maxX + reach; x += step) {
      lines.push(svg('line', { class: 'grid-line', x1: n3(x), y1: -reach, x2: n3(x), y2: n3(b.h + reach) }));
      if (x >= b.minX - 1e-9 && x <= b.maxX + 1e-9) {
        labels.push(svg('text', { class: 'grid-label', x: n3(x), y: n3(b.h + 14 * mpp), 'text-anchor': 'middle', 'font-size': font }, x));
      }
    }
    for (let z = Math.ceil((b.minZ - reach) / step) * step; z <= b.maxZ + reach; z += step) {
      const y = b.maxZ - z;
      lines.push(svg('line', { class: 'grid-line', x1: n3(b.minX - reach), y1: n3(y), x2: n3(b.maxX + reach), y2: n3(y) }));
      if (z >= b.minZ - 1e-9 && z <= b.maxZ + 1e-9) {
        labels.push(svg('text', { class: 'grid-label', x: n3(b.minX - 7 * mpp), y: n3(y + 3.5 * mpp), 'text-anchor': 'end', 'font-size': font }, z));
      }
    }
    labels.push(svg('text', { class: 'grid-label', x: n3((b.minX + b.maxX) / 2), y: n3(b.h + 30 * mpp), 'text-anchor': 'middle', 'font-size': font },
      'X metres east · Z metres north · Open grassland outside perimeter'));
    byId('grid').replaceChildren(...lines);
    byId('grid-in').replaceChildren(...lines.slice(1).map((line) => line.cloneNode(false)));
    byId('furniture').replaceChildren(...labels);
  }

  function buildOverlayCellNodes(areaId, mpp) {
    const s4 = s4Area(areaId);
    if (state.overlay === 'off' || !s4) return [];
    const nodes = [];
    if (state.overlay === 'light') {
      for (const cell of s4.ground) {
        const paint = lightPaint(cell.lightIndex);
        if (!paint) continue;
        nodes.push(svg('polygon', { class: 'light-cell', 'data-cell': cell.column + ':' + cell.row, 'data-light': cell.lightIndex,
          fill: paint.colour, 'fill-opacity': paint.opacity, points: cell.polygon.map((p) => n3(p.u) + ',' + n3(p.v)).join(' ') }));
      }
    } else if (state.overlay === 'undergrowth') {
      const compact = compactCellsFor(areaId);
      for (const cell of s4.ground) {
        const row = compact.get(cell.column + ':' + cell.row);
        if (!row) continue;
        const bramble = row[4], bracken = row[5];
        const pts = cell.polygon.map((p) => n3(p.u) + ',' + n3(p.v)).join(' ');
        if (bramble > 0) {
          nodes.push(svg('polygon', { class: 'undergrowth-cell', 'data-kind': 'bramble', fill: 'url(#ccf-bramble-hatch)',
            'fill-opacity': opacityText(Math.min(0.95, 0.2 + 0.75 * bramble)), points: pts }));
        }
        if (bracken > 0) {
          nodes.push(svg('polygon', { class: 'undergrowth-cell', 'data-kind': 'bracken', fill: 'url(#ccf-bracken-hatch)',
            'fill-opacity': opacityText(Math.min(0.95, 0.2 + 0.75 * bracken)), points: pts }));
        }
      }
    } else if (state.overlay === 'seedlings') {
      const compact = compactCellsFor(areaId);
      for (const cell of s4.ground) {
        const row = compact.get(cell.column + ':' + cell.row);
        if (!row || row[7] <= 0) continue;
        const mask = row[6];
        const tags = [];
        if (mask & 1) tags.push('S');
        if (mask & 2) tags.push('O');
        if (mask & 4) tags.push('B');
        const gSize = Math.min(1.6, Math.max(0.9, 18 * mpp));
        nodes.push(svg('polygon', { fill: '#355c2b', 'fill-opacity': '0.32', stroke: '#7bc45c', 'stroke-opacity': '0.55',
          'stroke-width': n3(1 * mpp), points: cell.polygon.map((p) => n3(p.u) + ',' + n3(p.v)).join(' ') }));
        nodes.push(svg('use', { href: '#ccf-natural-seedlings', x: n3(cell.centreU - gSize / 2), y: n3(cell.centreV - gSize * 0.6),
          width: n3(gSize), height: n3(gSize) }));
        if (tags.length > 0) {
          nodes.push(svg('text', { class: 'seedling-tag', x: n3(cell.centreU), y: n3(cell.centreV + gSize * 0.45),
            'font-size': n3(Math.min(0.75, 9 * mpp)) }, tags.join('·')));
        }
      }
    }
    return nodes;
  }

  function renderOverlayCells(areaId, mpp) {
    byId('overlay-cells').replaceChildren(...buildOverlayCellNodes(areaId, mpp));
  }

  // Rule 1: ClearUndergrowth highlights the whole 2.5 m cell (sparing seedlings inside), never a tree-crown badge.
  function buildClearCellNodes(areaId) {
    if (!state.ecology || !s4Area(areaId)) return [];
    const nodes = [];
    const drawClear = (orders, cls) => {
      for (const o of orders) {
        if (o.areaId !== areaId) continue;
        const cell = groundCell(areaId, o.column, o.row);
        if (!cell) continue;
        nodes.push(svg('polygon', { class: cls, 'data-clear-cell': o.column + ':' + o.row,
          points: cell.polygon.map((p) => n3(p.u) + ',' + n3(p.v)).join(' ') }));
      }
    };
    drawClear(state.ecology.pendingClear || [], 'clear-cell-pending');
    drawClear(state.ecology.approvedClear || [], 'clear-cell-approved');
    return nodes;
  }

  function renderClearCells(areaId) {
    byId('clear-cells').replaceChildren(...buildClearCellNodes(areaId));
  }

  // Rule 2: SpotClear (~1 m2): circular dashed ring drawn to exact world scale r = patch.radiusM (~0.564 m)
  // over bare soil/twigs with NO seedling inside, rendered in its own layer BENEATH planted PJ saplings.
  function buildSpotPatchNodes(areaId) {
    if (!state.ecology) return [];
    const nodes = [];
    for (const p of (state.ecology.patches || [])) {
      if (p.areaId !== areaId) continue;
      const r = p.radiusM;
      nodes.push(svg('use', { href: '#ccf-spot-clear', 'data-patch-id': p.patchId, 'data-radius-m': n3(r),
        x: n3(p.u - r), y: n3(p.v - r), width: n3(2 * r), height: n3(2 * r) }));
    }
    const drawQueuedSpot = (orders, cls, stateTag) => {
      for (const o of orders) {
        if (o.areaId !== areaId || o.kind !== 'SpotClear') continue;
        const r = SPOT_PREVIEW_RADIUS_M;
        const g = svg('g', { 'data-spot-order': o.orderId, 'data-order-state': stateTag });
        g.append(svg('title', null, o.orderId + ' · ' + stateTag + ' SpotClear (~1 m², r ≈ 0.564 m, no seedling)'));
        g.append(svg('circle', { class: cls, cx: n3(o.u), cy: n3(o.v), r: n3(r) }));
        nodes.push(g);
      }
    };
    drawQueuedSpot(pendingPlantings(), 'spot-order-pending', 'Pending');
    drawQueuedSpot(approvedPlantings(), 'spot-order-approved', 'Approved');
    return nodes;
  }

  function renderSpotPatches(areaId) {
    byId('spot-patches').replaceChildren(...buildSpotPatchNodes(areaId));
  }

  function buildDeadwoodNodes(areaId, mpp) {
    const s4 = s4Area(areaId);
    if (!s4) return [];
    const w = 26 * mpp, h = 10 * mpp;
    return s4.canopy.fallenDeadwood.map((record) => {
      const sp = speciesLabel(speciesOf(record.treeId));
      const g = svg('g', { class: 'log', 'data-deadwood-id': record.deadwoodId, 'data-species': speciesOf(record.treeId) });
      g.append(svg('title', null, record.deadwoodId + ' · ' + sp + ' fallen log (year ' + record.deathYear + ', direction schematic, not simulated)'));
      g.append(svg('use', { href: '#ccf-log', x: n3(record.u - w / 2), y: n3(record.v - h / 2), width: n3(w), height: n3(h) }));
      return g;
    });
  }

  function renderDeadwoodLayer(areaId, mpp) {
    byId('deadwood').replaceChildren(...buildDeadwoodNodes(areaId, mpp));
  }

  // Planted juveniles (PJ), active/expired shelters, and queued Plant/Shelter orders ABOVE SpotClear patches.
  function buildPlantedNodes(areaId, mpp) {
    if (!state.ecology) return [];
    const nodes = [];
    const sSize = Math.max(0.75, 18 * mpp);
    const selectedPj = currentJuvenile();
    for (const j of (state.ecology.planted || [])) {
      if (j.areaId !== areaId || !j.isActive) continue;
      const sym = SAPLING_SYMBOL[j.speciesId] || 'ccf-sapling-oak';
      const g = svg('g', { class: 'planted-pj', 'data-juvenile-id': j.juvenileId, 'data-area-id': areaId, 'data-species': j.speciesId });
      g.append(svg('title', null, j.juvenileId + ' · ' + speciesLabel(j.speciesId) + ' planted sapling, H ' + fixed(j.heightM, 2) + ' m (click to inspect)'));
      g.append(svg('circle', { class: 'pj-hit', cx: n3(j.u), cy: n3(j.v), r: n3(Math.max(0.45, sSize * 0.6)) }));
      g.append(svg('use', { href: '#' + sym, x: n3(j.u - sSize / 2), y: n3(j.v - sSize / 2), width: n3(sSize), height: n3(sSize) }));
      if (selectedPj && selectedPj.juvenileId === j.juvenileId) {
        g.append(svg('circle', { class: 'pj-selected-ring', cx: n3(j.u), cy: n3(j.v), r: n3(sSize * 0.65) }));
      }
      nodes.push(g);
    }
    const shSize = Math.max(0.95, 22 * mpp);
    for (const sh of (state.ecology.shelters || [])) {
      if (sh.areaId !== areaId) continue;
      const sym = sh.isEffective ? 'ccf-shelter-active' : 'ccf-shelter-expired';
      const g = svg('g', { class: 'shelter-mark', 'data-shelter-id': sh.shelterId, 'data-juvenile-target': sh.juvenileId,
        'data-effective': String(sh.isEffective) });
      g.append(svg('title', null, sh.shelterId + ' (' + (sh.isEffective ? 'Active shelter · expires year ' + sh.expiryYear : 'Expired shelter · expired year ' + sh.expiryYear) + ')'));
      g.append(svg('use', { href: '#' + sym, x: n3(sh.u - shSize / 2), y: n3(sh.v - shSize / 2), width: n3(shSize), height: n3(shSize) }));
      nodes.push(g);
    }
    const qSize = Math.max(0.75, 18 * mpp);
    const drawQueuedPlanting = (orders, approved) => {
      const ringCls = approved ? 'plant-order-ring-approved' : 'plant-order-ring-pending';
      const prefix = approved ? '✓' : '◷';
      const stateWord = approved ? 'Approved' : 'Pending';
      for (const o of orders) {
        if (o.areaId !== areaId || o.kind === 'SpotClear') continue;
        const iconId = o.kind === 'Plant' ? 'ccf-action-plant' : 'ccf-action-shelter';
        const dx = o.kind === 'Shelter' ? qSize * 0.42 : 0;
        const g = svg('g', { 'data-planting-order': o.orderId, 'data-order-kind': o.kind, 'data-order-state': stateWord });
        g.append(svg('title', null, o.orderId + ' · ' + prefix + ' ' + stateWord + ' ' + o.kind +
          (o.speciesId ? ' (' + speciesLabel(o.speciesId) + ')' : '')));
        g.append(svg('circle', { class: ringCls, cx: n3(o.u + dx), cy: n3(o.v), r: n3(qSize * 0.56) }));
        g.append(svg('use', { href: '#' + iconId, x: n3(o.u + dx - qSize * 0.44), y: n3(o.v - qSize * 0.44),
          width: n3(qSize * 0.88), height: n3(qSize * 0.88) }));
        nodes.push(g);
      }
    };
    drawQueuedPlanting(pendingPlantings(), false);
    drawQueuedPlanting(approvedPlantings(), true);
    return nodes;
  }

  function renderPlantedLayer(areaId, mpp) {
    byId('planted-layer').replaceChildren(...buildPlantedNodes(areaId, mpp));
  }

  // Provisional local Draft previews (dashed slate outline, snap OFF, zero engine order).
  function buildDraftPreviewNodes(areaId, mpp) {
    const nodes = [];
    const arm = Math.max(0.32, 7 * mpp);
    if (state.mapMode === 'plant' && state.plantDraft && state.plantDraft.areaId === areaId) {
      const d = state.plantDraft;
      const wantSpot = Boolean(byId('plant-opt-spot') && byId('plant-opt-spot').checked);
      const wantShelter = Boolean(byId('plant-opt-shelter') && byId('plant-opt-shelter').checked);
      const g = svg('g', { id: 'draft-plant-preview', 'data-draft-kind': 'plant' });
      if (wantSpot) {
        g.append(svg('circle', { class: 'draft-ring', 'data-draft-spot-ring': 'true', cx: n3(d.u), cy: n3(d.v), r: n3(SPOT_PREVIEW_RADIUS_M) }));
      }
      g.append(svg('circle', { class: 'draft-ring', cx: n3(d.u), cy: n3(d.v), r: n3(Math.max(0.28, 8 * mpp)) }));
      g.append(svg('path', { class: 'draft-cross', d: 'M ' + n3(d.u - arm) + ' ' + n3(d.v) + ' L ' + n3(d.u + arm) + ' ' + n3(d.v) +
        ' M ' + n3(d.u) + ' ' + n3(d.v - arm) + ' L ' + n3(d.u) + ' ' + n3(d.v + arm) }));
      const labelText = 'Draft Plant' + (wantShelter ? ' + Shelter' : '') + (wantSpot ? ' + Spot' : '');
      g.append(svg('text', { class: 'draft-label', x: n3(d.u), y: n3(d.v - Math.max(0.75, 15 * mpp)), 'font-size': n3(Math.max(0.45, 10 * mpp)) }, labelText));
      nodes.push(g);
    } else if (state.mapMode === 'spot' && state.spotDraft && state.spotDraft.areaId === areaId) {
      const d = state.spotDraft;
      const g = svg('g', { id: 'draft-spot-preview', 'data-draft-kind': 'spot' });
      g.append(svg('circle', { class: 'draft-ring', 'data-draft-spot-ring': 'true', cx: n3(d.u), cy: n3(d.v), r: n3(SPOT_PREVIEW_RADIUS_M) }));
      g.append(svg('path', { class: 'draft-cross', d: 'M ' + n3(d.u - arm * 0.6) + ' ' + n3(d.v) + ' L ' + n3(d.u + arm * 0.6) + ' ' + n3(d.v) +
        ' M ' + n3(d.u) + ' ' + n3(d.v - arm * 0.6) + ' L ' + n3(d.u) + ' ' + n3(d.v + arm * 0.6) }));
      g.append(svg('text', { class: 'draft-label', x: n3(d.u), y: n3(d.v - Math.max(0.75, 15 * mpp)), 'font-size': n3(Math.max(0.45, 10 * mpp)) }, 'Draft Spot Clear (~1 m²)'));
      nodes.push(g);
    }
    return nodes;
  }

  function renderDraftPreview(areaId, mpp) {
    byId('draft-preview').replaceChildren(...buildDraftPreviewNodes(areaId, mpp));
  }

  function buildGroundMarkNodes(areaId) {
    const c = state.cellDetail;
    const poly = state.mapMode === 'cell' && c && !c.missing && c.areaId === areaId ? c.polygon : null;
    const points = poly ? poly.map((p) => n3(p.u) + ',' + n3(p.v)).join(' ') : null;
    return points ? [svg('polygon', { class: 'ground-pick-halo', points }), svg('polygon', { class: 'ground-pick', points })] : [];
  }

  function renderGroundMark(areaId) {
    byId('ground-mark').replaceChildren(...buildGroundMarkNodes(areaId));
  }

  const RAMP_SAMPLES = [0, 0.002, 0.005, 0.01, 0.02, 0.05, 0.1, 0.2, 0.35, 0.5, 0.65, 0.8, 0.9, 1];
  export function renderOverlayLegend() {
    const box = byId('overlay-legend');
    if (state.overlay === 'off') { box.hidden = true; box.replaceChildren(); return; }
    box.hidden = false;
    if (state.overlay === 'light') {
      const ramp = svg('svg', { id: 'light-ramp', class: 'light-ramp', viewBox: '0 0 100 10', preserveAspectRatio: 'none', role: 'img',
        'aria-label': 'Light Guide colour ramp: shaded to bright' });
      const grad = svg('linearGradient', { id: 'light-ramp-fill', x1: 0, y1: 0, x2: 1, y2: 0 });
      for (const value of RAMP_SAMPLES) {
        const paint = lightPaint(value);
        grad.append(svg('stop', { offset: value, 'data-l': value, 'stop-color': paint ? paint.colour : '#7f3b4a',
          'stop-opacity': paint ? paint.opacity : 0 }));
      }
      const defs = svg('defs');
      defs.append(grad);
      ramp.append(defs, svg('rect', { x: 0, y: 0, width: 100, height: 10, fill: 'rgb(' + SLATE.join(',') + ')' }),
        svg('rect', { x: 0, y: 0, width: 100, height: 10, fill: 'url(#light-ramp-fill)' }));
      box.replaceChildren(
        el('p', { class: 'overlay-legend-title' }, 'Light Guide'),
        el('span', { class: 'ramp-end', 'aria-hidden': 'true' }, 'Shaded'),
        ramp,
        el('span', { class: 'ramp-end', 'aria-hidden': 'true' }, 'Bright'),
        el('p', { class: 'overlay-legend-note' }, 'Modelled light · not calibrated PAR'));
    } else if (state.overlay === 'undergrowth') {
      const swBramble = svg('svg', { class: 'overlay-swatch', viewBox: '0 0 4 2.5', 'aria-hidden': 'true' });
      swBramble.append(svg('rect', { width: 4, height: 2.5, fill: '#1b2430' }), svg('rect', { width: 4, height: 2.5, fill: 'url(#ccf-bramble-hatch)' }));
      const swBracken = svg('svg', { class: 'overlay-swatch', viewBox: '0 0 4 2.5', 'aria-hidden': 'true' });
      swBracken.append(svg('rect', { width: 4, height: 2.5, fill: '#1b2430' }), svg('rect', { width: 4, height: 2.5, fill: 'url(#ccf-bracken-hatch)' }));
      box.replaceChildren(
        el('p', { class: 'overlay-legend-title' }, 'Undergrowth'),
        el('span', null, swBramble, ' Bramble'),
        el('span', null, swBracken, ' Bracken'),
        el('p', { class: 'overlay-legend-note' }, 'Hatch opacity scales with cell cover (0–1). ClearUndergrowth: Whole 2.5 m cell · spares seedlings.'));
    } else if (state.overlay === 'seedlings') {
      const swSeed = svg('svg', { class: 'overlay-swatch', viewBox: '-24 -24 48 48', 'aria-hidden': 'true' });
      swSeed.append(svg('use', { href: '#ccf-natural-seedlings', x: -24, y: -24, width: 48, height: 48 }));
      box.replaceChildren(
        el('p', { class: 'overlay-legend-title' }, 'Natural Seedlings'),
        el('span', null, swSeed, ' Cell presence (S Sitka · O Oak · B Beech)'),
        el('p', { class: 'overlay-legend-note' }, label('MultiNaturalSeedlingsNote') || 'Cell-level presence only — Density is abstract occupancy, not a literal stem count'));
    }
  }

  function renderScale(scale) {
    const metres = [0.5, 1, 2, 5, 10, 20, 50, 100].find((m) => m * scale >= 56) || 100;
    const width = Math.max(8, Math.round(metres * scale));
    const bar = byId('scale-bar');
    setAttributes(bar, { width, height: 9, viewBox: '0 0 ' + width + ' 9' });
    byId('scale-path').setAttribute('d', 'M 1 1 V 8 H ' + (width - 1) + ' V 1');
    byId('scale-text').textContent = metres + ' m';
  }

  function renderMapStatus(reference, drafts) {
    const box = byId('map-status');
    const view = state.view;
    let text = '';
    if (state.mapMode === 'cell') text = 'Inspect Cell mode: click any 2.5 m cell inside the area · Esc returns to Select Tree (draft Fell ticks kept)';
    else if (state.mapMode === 'plant') text = 'Plant mode (snap OFF): click map for provisional Draft preview · Esc returns to Select Tree (draft Fell ticks kept)';
    else if (state.mapMode === 'spot') text = 'Spot Clear mode (snap OFF): click map for seedling-free 0.564 m Draft preview · Esc returns to Select Tree (draft Fell ticks kept)';
    else if (state.batch) text = 'Adding ' + (state.batch.confirmed.length + 1) + ' of ' + state.batch.ids.length + ' to the Fell plan…';
    else if (state.queueRunning) text = 'Queueing planting Work Plan orders in sequence…';
    else if (view.approved) {
      const summary = hooks.workTotals(view.approved.orders.length, approvedPrunes().length, approvedClears().length, approvedPlantings());
      text = summary + ' approved · nothing executed yet';
    }
    else if (currentJuvenile()) text = 'Sapling ' + currentJuvenile().juvenileId + ' · ' + speciesLabel(currentJuvenile().speciesId);
    else if (reference) text = 'Reference ' + reference + ' · ' + plural(drafts, 'draft choice', 'draft choices');
    box.hidden = text === '';
    box.textContent = text;
  }

  // ---- status marks ---------------------------------------------------------------------------------------------------

  function markPath(cls, d) { return svg('path', { class: 'ring ' + cls, d }); }
  function at(item, dx, dy) { return n3(item.u + dx) + ' ' + n3(item.v + dy); }

  function cornerFrame(item, cls, mpp) {
    const h = item.radiusM + 7 * mpp, arm = 10 * mpp;
    let d = '';
    for (const [sx, sy] of [[-1, -1], [1, -1], [1, 1], [-1, 1]]) {
      d += 'M ' + at(item, sx * h, sy * (h - arm)) + ' L ' + at(item, sx * h, sy * h) + ' L ' + at(item, sx * (h - arm), sy * h) + ' ';
    }
    return markPath(cls, d);
  }

  function sideBrackets(item, cls, mpp) {
    const x = item.radiusM + 7 * mpp, y = 9 * mpp, arm = 5 * mpp;
    let d = '';
    for (const s of [-1, 1]) {
      d += 'M ' + at(item, s * (x - arm), -y) + ' L ' + at(item, s * x, -y) + ' L ' + at(item, s * x, y) + ' L ' + at(item, s * (x - arm), y) + ' ';
    }
    return markPath(cls, d);
  }

  function underline(item, cls, mpp, extraPx) {
    const y = item.radiusM * .8 + (extraPx || 0) * mpp, half = 10 * mpp;
    return markPath(cls, 'M ' + at(item, -half, y) + ' L ' + at(item, half, y));
  }

  const TILE = { rank: -45, status: 45, crop: 135, reference: -135 };
  const RANK_ANGLES = [-45, 45, -135, 135, 0, 180, -90, 90];

  function tileCentre(item, angleDeg, mpp) {
    const angle = angleDeg * Math.PI / 180, distance = item.radiusM + 9 * mpp;
    return [item.u + Math.cos(angle) * distance, item.v + Math.sin(angle) * distance];
  }

  function badge(item, angleDeg, text, kind, mpp, extra) {
    const [cx, cy] = tileCentre(item, angleDeg, mpp);
    const g = svg('g', { class: 'badge b-' + kind + (extra ? ' ' + extra : '') });
    g.append(svg('rect', { class: 'badge-bg', x: n3(cx - 10 * mpp), y: n3(cy - 10 * mpp), width: n3(20 * mpp), height: n3(20 * mpp), rx: n3(2 * mpp) }),
      svg('text', { class: 'badge-text', x: n3(cx), y: n3(cy + .5 * mpp), 'font-size': n3(10.5 * mpp) }, text));
    return g;
  }

  function tag(x, y, text, mpp) {
    const width = (text.length * 6.4 + 14) * mpp, height = 19 * mpp;
    const g = svg('g', { class: 'map-tag' });
    g.append(svg('rect', { x: n3(x - width / 2), y: n3(y - height / 2), width: n3(width), height: n3(height), rx: n3(4 * mpp) }),
      svg('text', { x: n3(x), y: n3(y + .5 * mpp), 'font-size': n3(11 * mpp) }, text));
    return g;
  }

  function tick(item, mpp, angle) {
    const [rx, ry] = tileCentre(item, angle ?? TILE.rank, mpp);
    const cx = rx + 11 * mpp, cy = ry - 11 * mpp;
    const p = (dx, dy) => n3(cx + dx * mpp) + ' ' + n3(cy + dy * mpp);
    return [svg('circle', { class: 'tick-box', cx: n3(cx), cy: n3(cy), r: n3(7 * mpp) }),
      svg('path', { class: 'tick', d: 'M ' + p(-3, 0) + ' L ' + p(-1, 2.5) + ' L ' + p(3.5, -3) })];
  }

  function statusMarks(item, mpp) {
    const marks = [];
    // Rule 5: Retained crop tree framed by double gold ring + small C badge (#ccf-crop-ring), never sawn lumber.
    if (item.isCrop) {
      const span = Math.max(item.radiusM * 1.22, item.radiusM + 5 * mpp);
      marks.push(svg('use', { class: 'deco crop-ring', href: '#ccf-crop-ring',
        x: n3(item.u - span), y: n3(item.v - span), width: n3(2 * span), height: n3(2 * span) }));
    }
    // Rule 3: Pending = dashed gold outline + clock/P; Approved = solid pale-blue badge + check/A.
    if (item.status === 'PendingFell') {
      marks.push(underline(item, 'ring-pending', mpp, 4), badge(item, TILE.status, '◷P', 'pending', mpp));
    } else if (item.status === 'ApprovedFell') {
      marks.push(underline(item, 'ring-approved', mpp, 4), underline(item, 'ring-approved', mpp, 8),
        badge(item, TILE.status, '✓A', 'approved', mpp));
    } else if (item.status === 'NaturalDead') {
      marks.push(badge(item, TILE.status, 'D', 'dead', mpp));
    } else if (item.status === 'Felled') {
      const d = Math.min(item.radiusM * .45, 6 * mpp);
      marks.push(svg('path', { class: 'stump-x', d: 'M ' + n3(item.u - d) + ' ' + n3(item.v - d) + ' L ' + n3(item.u + d) + ' ' +
        n3(item.v + d) + ' M ' + n3(item.u - d) + ' ' + n3(item.v + d) + ' L ' + n3(item.u + d) + ' ' + n3(item.v - d) }),
        badge(item, TILE.status, 'X', 'executed', mpp));
    }
    return marks;
  }

  function competitorMarks(item, rank, chosen, mpp, angle) {
    const marks = [underline(item, chosen ? 'ring-draft' : 'ring-competitor', mpp, 0)];
    marks.push(badge(item, angle ?? TILE.rank, String(rank), 'rank', mpp, chosen ? 'chosen' : null));
    if (chosen) marks.push(...tick(item, mpp, angle));
    return marks;
  }

  function renderRankLayer(ranks, box) {
    const layer = byId('rank-layer');
    const active = document.activeElement;
    const focusKey = active && layer.contains(active) ? active.getAttribute('data-focus-key') : null;
    const planLocked = state.view.approved !== null;
    const frozenNow = state.lock !== null || state.batch !== null;
    const buttons = ranks.map((r) => {
      const why = blockedReason(state.areaId, r.treeId) ||
        (planLocked ? 'The plan is approved, so editing is locked' : frozenNow ? 'Please wait' : null);
      const button = el('button', { type: 'button', class: 'rank-toggle', 'data-rank-toggle': r.treeId, 'data-rank': r.rank,
        'data-focus-key': 'rank:' + r.treeId, 'aria-pressed': String(r.chosen && !why), 'aria-disabled': why ? 'true' : null,
        'aria-label': 'Competitor ' + r.rank + ', tree ' + r.treeId + ', Fell draft' + (why ? ' (' + why + ')' : ''),
        title: 'Competitor ' + r.rank + ' · ' + r.treeId + (why ? ' · ' + why : r.chosen ? ' · in the Fell draft (click to remove)' : ' · click to add to the Fell draft') },
      String(r.rank));
      if (button.style) {
        button.style.left = n3((r.x - box.x) * box.scale) + 'px';
        button.style.top = n3((r.y - box.y) * box.scale) + 'px';
      }
      return button;
    });
    layer.replaceChildren(...buttons);
    layer.hidden = buttons.length === 0;
    if (focusKey) {
      const again = layer.querySelector('[data-focus-key="' + CSS.escape(focusKey) + '"]');
      if (again) again.focus();
    }
  }

  export function onRankToggle(event) {
    const button = event.target.closest('[data-rank-toggle]');
    if (!button) return;
    event.stopPropagation();
    const treeId = button.getAttribute('data-rank-toggle');
    if (button.getAttribute('aria-disabled') === 'true') {
      announce('Competitor ' + button.getAttribute('data-rank') + ', ' + treeId + ', cannot be chosen: ' +
        (blockedReason(state.areaId, treeId) || (state.view.approved ? 'the plan is approved, so editing is locked' : 'please wait')) + '.');
      return;
    }
    hooks.toggleDraft(treeId, button.getAttribute('aria-pressed') !== 'true');
  }

  function referenceMarks(item, mpp, withTag) {
    const marks = [cornerFrame(item, 'ring-halo', mpp), cornerFrame(item, 'ring-reference', mpp),
      badge(item, TILE.reference, 'R', 'reference', mpp)];
    if (withTag) marks.push(tag(item.u, item.v - item.radiusM - 22 * mpp, 'Reference' + (item.isCrop ? ' · Crop' : ''), mpp));
    return marks;
  }

  function inspectMarks(item, mpp, withTag) {
    const marks = [sideBrackets(item, 'ring-halo', mpp), sideBrackets(item, 'ring-inspect', mpp),
      badge(item, TILE.reference, 'I', 'inspect', mpp)];
    if (withTag) marks.push(tag(item.u, item.v - item.radiusM - 22 * mpp, 'Selected', mpp));
    return marks;
  }

  function pruneMarks(areaId, item, mpp) {
    const ps = pruneStatusOf(areaId, item.treeId);
    if (!ps || !ps.planState) return [];
    return ps.planState === 'PendingPrune'
      ? [underline(item, 'ring-pending', mpp, 4), badge(item, TILE.status, '◷✂', 'pending', mpp)]
      : [underline(item, 'ring-approved', mpp, 4), underline(item, 'ring-approved', mpp, 8), badge(item, TILE.status, '✓✂', 'approved', mpp)];
  }

  // ---- grid and legend ------------------------------------------------------------------------------------------------

  function drawGrid(area, mpp, scale) {
    const side = area.extent.sideM;
    const minX = area.extent.minXM;
    const minZ = area.extent.minZM;
    const step = 5 * scale >= 26 ? 5 : 10;
    const reach = 80;
    const lines = [];
    const labels = [];
    const font = n3(10 * mpp);
    for (let x = Math.ceil((minX - reach) / step) * step; x <= minX + side + reach; x += step) {
      const u = x - minX;
      lines.push(svg('line', { class: 'grid-line', x1: n3(u), y1: -reach, x2: n3(u), y2: side + reach }));
      if (x >= minX - 1e-9 && x <= minX + side + 1e-9) {
        labels.push(svg('text', { class: 'grid-label', x: n3(u), y: n3(side + 14 * mpp), 'text-anchor': 'middle', 'font-size': font }, x));
      }
    }
    for (let z = Math.ceil((minZ - reach) / step) * step; z <= minZ + side + reach; z += step) {
      const v = minZ + side - z;
      lines.push(svg('line', { class: 'grid-line', x1: -reach, y1: n3(v), x2: side + reach, y2: n3(v) }));
      if (z >= minZ - 1e-9 && z <= minZ + side + 1e-9) {
        labels.push(svg('text', { class: 'grid-label', x: n3(-7 * mpp), y: n3(v + 3.5 * mpp), 'text-anchor': 'end', 'font-size': font }, z));
      }
    }
    labels.push(svg('text', { class: 'grid-label', x: n3(side / 2), y: n3(side + 30 * mpp), 'text-anchor': 'middle', 'font-size': font },
      'X metres east · Z metres north'));
    byId('grid').replaceChildren(...lines);
    byId('grid-in').replaceChildren(...lines.map((line) => line.cloneNode(false)));
    byId('furniture').replaceChildren(...labels);
  }

  function legendIcon(draw) {
    const icon = svg('svg', { viewBox: '-1.9 -1.9 3.8 3.8', 'aria-hidden': 'true', focusable: 'false' });
    const mpp = .075;
    const item = { u: 0, v: 0, radiusM: 1, treeId: 'legend', status: 'Living', isCrop: false };
    icon.append(svg('circle', { class: 'swatch-ground', cx: 0, cy: 0, r: 1.85 }));
    icon.append(...draw(item, mpp));
    return icon;
  }

  function legendUse(symbolId, span) {
    const s = span || 1.35;
    return [svg('use', { href: '#' + symbolId, x: -s, y: -s, width: 2 * s, height: 2 * s })];
  }

  function legendEntries() {
    return [
      { key: 'sitka', name: 'Sitka spruce', short: 'Sitka', description: 'Radial conifer crown', draw: () => legendUse('ccf-sitka', 1.25) },
      { key: 'oak', name: 'Sessile oak', short: 'Oak', description: 'Lobed broadleaf crown', draw: () => legendUse('ccf-oak', 1.25) },
      { key: 'beech', name: 'European beech', short: 'Beech', description: 'Dense rounded broadleaf', draw: () => legendUse('ccf-beech', 1.25) },
      { key: 'crop', name: 'Retained Crop · C', short: 'Crop · C', description: 'Double gold ring + C badge', draw: (item, mpp) => [...legendUse('ccf-sitka', 1.05),
        ...statusMarks(Object.assign({}, item, { isCrop: true }), mpp)] },
      { key: 'reference', name: 'Reference · R / Comp. 1–5', short: 'Ref · R', description: 'Pinned reference & top 5', draw: (item, mpp) => [...legendUse('ccf-sitka', 1.0), ...referenceMarks(item, mpp, false)] },
      { key: 'pending', name: 'Pending · ◷', short: 'Pending · ◷', description: 'Dashed gold outline; queued', draw: (item, mpp) => [...legendUse('ccf-sitka', 1.0),
        ...statusMarks(Object.assign({}, item, { status: 'PendingFell' }), mpp)] },
      { key: 'approved', name: 'Approved · ✓', short: 'Approved · ✓', description: 'Solid pale-blue; awaiting Execute', draw: (item, mpp) => [...legendUse('ccf-sitka', 1.0),
        ...statusMarks(Object.assign({}, item, { status: 'ApprovedFell' }), mpp)] },
      { key: 'felled', name: 'Felled stump · X', short: 'Stump · X', description: 'Executed work (History Log)', draw: (item, mpp) => [...crownDecoration(Object.assign({}, item, { status: 'Felled' }), 'sitka-spruce'),
        ...statusMarks(Object.assign({}, item, { status: 'Felled' }), mpp)] },
      { key: 'standing-dead', name: 'Standing dead · D', short: 'Dead · D', description: 'Natural mortality crown', draw: (item, mpp) => [...legendUse('ccf-standing-dead', 1.15),
        ...statusMarks(Object.assign({}, item, { status: 'NaturalDead' }), mpp)] },
      { key: 'fallen-log', name: 'Fallen log (schematic)', short: 'Fallen log', description: 'Direction schematic, not simulated', draw: () => [svg('use', { href: '#ccf-log', x: -1.45, y: -0.55, width: 2.9, height: 1.1 })] },
      { key: 'shelter-active', name: 'Active shelter (< 8 yr)', short: 'Shelter <8yr', description: 'Cyan mesh tube protecting PJ', draw: () => legendUse('ccf-shelter-active', 1.35) },
      { key: 'shelter-expired', name: 'Expired shelter (8+ yr)', short: 'Expired ×', description: 'Dashed grey + X · map state only', draw: () => legendUse('ccf-shelter-expired', 1.35) },
      { key: 'spot-clear', name: 'SpotClear (~1 m²)', short: 'SpotClear', description: 'Cleared soil + twigs, no seedling', draw: () => legendUse('ccf-spot-clear', 1.25) },
      { key: 'clear-undergrowth', name: 'ClearUndergrowth', short: 'Clear cell', description: 'Whole 2.5 m cell · spares seedlings', draw: () => legendUse('ccf-action-clear', 1.35) },
      { key: 'seedlings', name: 'Natural seedlings', short: 'Seedlings', description: 'Cell-level presence glyph only', draw: () => legendUse('ccf-natural-seedlings', 1.35) },
      { key: 'boundary', name: 'Area boundary', short: 'Boundary', description: 'Actual management extent', draw: null },
    ];
  }

  function boundarySwatch() {
    const boundary = svg('svg', { viewBox: '-1.9 -1.9 3.8 3.8', 'aria-hidden': 'true', focusable: 'false' });
    boundary.append(svg('rect', { class: 'legend-exterior', x: -1.85, y: -1.85, width: 3.7, height: 3.7, rx: .3 }),
      svg('polygon', { class: 'legend-area', points: '-1.3,-1 1.2,-1.35 1.35,1.2 -1.2,1.3' }));
    return boundary;
  }

  export function renderLegend() {
    const items = legendEntries().map((entry) => el('li', { 'data-legend-key': entry.key },
      entry.draw ? legendIcon(entry.draw) : boundarySwatch(),
      el('span', null, el('span', { class: 'legend-name' }, entry.name), el('span', { class: 'legend-desc' }, entry.description))));
    byId('legend').replaceChildren(...items);
  }

  export function renderCompactLegend() {
    const host = byId('legend-compact-items');
    if (!host) return;
    const lens = currentLens();
    const byKey = new Map(legendEntries().map((e) => [e.key, e]));
    const keys = lens.legendKeys.slice();
    if (state.overlay === 'seedlings' && !keys.includes('seedlings')) keys.unshift('seedlings');
    if (state.overlay === 'undergrowth' && !keys.includes('clear-undergrowth')) keys.push('clear-undergrowth');
    const chips = keys.map((key) => {
      const entry = byKey.get(key);
      if (!entry) return null;
      return el('span', { class: 'legend-chip', 'data-legend-chip': key, title: entry.name + ' — ' + entry.description },
        entry.draw ? legendIcon(entry.draw) : boundarySwatch(),
        el('span', { class: 'legend-chip-label' }, entry.short));
    }).filter(Boolean);
    host.replaceChildren(...chips);
    const ctx = byId('legend-context-label');
    if (ctx) ctx.textContent = lens.label + ' key:';
    const toggle = byId('legend-toggle');
    if (toggle) {
      toggle.setAttribute('aria-expanded', String(Boolean(state.legendExpanded)));
      toggle.textContent = state.legendExpanded ? 'Hide full legend ▴' : 'Full legend ▾';
    }
    const panel = byId('legend-panel');
    if (panel) panel.hidden = !state.legendExpanded;
  }

  export function setLegendExpanded(expanded, moveFocus) {
    state.legendExpanded = Boolean(expanded);
    state.legendHidden = !state.legendExpanded;
    const ws = byId('workspace');
    if (ws) ws.setAttribute('data-legend', state.legendExpanded ? 'expanded' : 'compact');
    const panel = byId('legend-panel');
    if (panel) panel.hidden = !state.legendExpanded;
    const toggle = byId('legend-toggle');
    if (toggle) {
      toggle.setAttribute('aria-expanded', String(state.legendExpanded));
      toggle.textContent = state.legendExpanded ? 'Hide full legend ▴' : 'Full legend ▾';
    }
    const hideBtn = byId('legend-hide');
    if (hideBtn) hideBtn.setAttribute('aria-expanded', String(state.legendExpanded));
    if (moveFocus) {
      const target = state.legendExpanded ? hideBtn : toggle;
      if (target && !target.hidden) target.focus();
    }
    announce(state.legendExpanded ? 'Full map legend expanded (16 symbols). Press Escape to collapse.' : 'Full map legend collapsed to compact contextual key.');
  }

  export function setLegendHidden(hidden, moveFocus) {
    setLegendExpanded(!hidden, moveFocus);
  }


  // S0c P5 svgRenderer adapter (conforms to MAP_RENDERER_METHODS in ./contract.js)
  export const svgRenderer = Object.freeze({
    mount() { return initFloorDefs(); },
    render() { return renderMap(); },
    setSelection() {},
    focusArea() {},
    destroy() {},
  });
