// Phase 2A-2: Whole-woodland interactive mini-map (spec §7.3; visible only when areas >= 3).
// Presentation only (D-020): draws lane polygons and low-detail stems verbatim from server payloads; zero geometry invention.
import { el, n3, svg } from '../dom.js';
import { byId, state, visibleLaneIds } from '../store.js';

const STEM_COLOUR = ['#5ea878', '#8ec24f', '#b5a84e'];

function minimapAreas() {
  if (state.standOverview && Array.isArray(state.standOverview.areas) && state.standOverview.areas.length > 0) {
    return state.standOverview.areas;
  }
  return state.view ? state.view.areas : [];
}

export function renderMinimap(onSelectLane) {
  const host = byId('woodland-minimap');
  if (!host) return;
  const areas = minimapAreas();
  if (areas.length < 3) {
    host.hidden = true;
    host.replaceChildren();
    return;
  }
  host.hidden = false;

  const allPts = areas.flatMap((a) => a.polygon || []);
  if (allPts.length === 0) { host.replaceChildren(); return; }
  const minX = Math.min(...allPts.map((p) => p.xm));
  const maxX = Math.max(...allPts.map((p) => p.xm));
  const minZ = Math.min(...allPts.map((p) => p.zm));
  const maxZ = Math.max(...allPts.map((p) => p.zm));
  const w = Math.max(1, maxX - minX);
  const h = Math.max(1, maxZ - minZ);
  const pad = Math.max(w, h) * 0.08;

  const visible = new Set(visibleLaneIds(areas, state.areaId, state.singleLaneMode));
  const mapSvg = svg('svg', {
    class: 'minimap-svg',
    viewBox: [minX - pad, -pad, w + 2 * pad, h + 2 * pad].map(n3).join(' '),
    role: 'group',
    'aria-label': 'Whole woodland mini-map (' + areas.length + ' lanes); click or press Enter on a lane to switch',
  });

  // Open grassland surround outside the woodland perimeter (zero trees outside).
  mapSvg.append(svg('rect', {
    class: 'minimap-grassland',
    x: n3(minX - pad),
    y: n3(-pad),
    width: n3(w + 2 * pad),
    height: n3(h + 2 * pad),
  }));

  for (const area of areas) {
    const isActive = area.areaId === state.areaId;
    const isVis = visible.has(area.areaId);
    const pts = area.polygon.map((p) => n3(p.xm) + ',' + n3(maxZ - p.zm)).join(' ');
    const laneXs = area.polygon.map((p) => p.xm);
    const laneZs = area.polygon.map((p) => maxZ - p.zm);
    const cx = (Math.min(...laneXs) + Math.max(...laneXs)) / 2;
    const cy = Math.min(...laneZs) + (Math.max(...laneZs) - Math.min(...laneZs)) * 0.14;

    const g = svg('g', {
      class: 'minimap-lane' + (isActive ? ' active' : isVis ? ' neighbour' : ''),
      'data-minimap-lane': area.areaId,
      role: 'button',
      tabindex: '0',
      'aria-pressed': String(isActive),
      'aria-label': area.label + (isActive ? ' (active)' : isVis ? ' (visible neighbour)' : '') +
        ', ' + (area.stand ? area.stand.livingCount + ' living trees' : ''),
    });
    g.append(svg('polygon', { class: 'minimap-poly', points: pts }));

    // Low-detail stems directly from Core overview minimapStems (or full area.trees fallback).
    const stems = area.minimapStems || null;
    if (stems) {
      for (const st of stems) {
        if (!st.l) continue;
        g.append(svg('circle', {
          class: 'minimap-stem',
          cx: n3(st.x),
          cy: n3(maxZ - st.z),
          r: n3(Math.max(0.75, st.r * 0.72)),
          fill: STEM_COLOUR[st.s] || STEM_COLOUR[0],
        }));
      }
    } else if (area.trees) {
      for (const tr of area.trees) {
        if (!tr.isLiving) continue;
        g.append(svg('circle', {
          class: 'minimap-stem',
          cx: n3(tr.xm),
          cy: n3(maxZ - tr.zm),
          r: n3(Math.max(0.75, tr.crownRadiusM * 0.72)),
          fill: STEM_COLOUR[0],
        }));
      }
    }

    const shortLetter = area.areaId.startsWith('MA-') ? area.areaId.slice(3) : area.label;
    g.append(svg('text', {
      class: 'minimap-label',
      x: n3(cx),
      y: n3(cy),
      'font-size': n3(Math.max(3.6, w * 0.065)),
    }, shortLetter));

    g.addEventListener('click', (event) => {
      event.stopPropagation();
      if (onSelectLane) onSelectLane(area.areaId);
    });
    g.addEventListener('keydown', (event) => {
      if (event.key === 'Enter' || event.key === ' ') {
        event.preventDefault();
        event.stopPropagation();
        if (onSelectLane) onSelectLane(area.areaId);
      }
    });
    mapSvg.append(g);
  }

  const head = el('div', { class: 'minimap-head' },
    el('span', { class: 'minimap-title' }, 'Woodland (' + areas.length + ' lanes)'),
    el('span', { class: 'minimap-mode' }, state.singleLaneMode ? '1 lane' : visible.size + ' shown'));
  host.replaceChildren(head, mapSvg);
}
