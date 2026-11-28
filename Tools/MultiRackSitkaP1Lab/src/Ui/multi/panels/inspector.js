// S0c provenance: panels/inspector.js [L4] — section(s) 12 from multi.js
import { fetchScotsPineTree } from '../api.js';
import { el, fill, fixed, label, pair } from '../dom.js';
import { SCOTS_PINE_SLOT_IDS, areaName, baseStatusText, byId, currentLens, hooks, itemOf, pruneStatusOf, referenceIn, s4Area, s4Tree, scotsPineEcology, scotsPineTreeKey, selectedId, speciesLabel, speciesOf, state, statusText, treeIn } from '../store.js';
  // ---- Tree inspector, Prune box (species-aware) and Competition ------------------------------------------------------

  export function renderOverlap() {
    const box = byId('overlap');
    const overlap = state.overlap;
    if (!overlap || overlap.areaId !== state.areaId || state.mapMode !== 'tree') {
      box.hidden = true;
      byId('overlap-list').replaceChildren();
      return;
    }
    box.hidden = false;
    byId('overlap-heading').textContent = fill(label('CrownsOverlap'), { n: overlap.candidates.length });
    byId('overlap-list').replaceChildren(...overlap.candidates.map((candidate, index) => {
      const current = candidate.treeId === selectedId();
      return el('li', null, el('button', { type: 'button', class: 'small', 'data-pick': candidate.treeId,
        'data-focus-key': 'pick:' + candidate.treeId, 'aria-label': (current ? 'Selected ' : 'Select ') + candidate.treeId },
        current ? 'Selected' : 'Select'), ' ', el('span', { class: 'mono' }, candidate.treeId),
        ' (' + speciesLabel(speciesOf(candidate.treeId)) + ') — ' + statusText(candidate) + (index === 0 ? ' (topmost)' : ''));
    }));
  }

  function recordedReason(areaId, treeId) {
    const view = state.view;
    const pending = view.pending.find((order) => order.areaId === areaId && order.treeId === treeId);
    if (pending) return pending.reason + ' (pending order)';
    const approved = view.approved ? view.approved.orders.find((order) => order.areaId === areaId && order.treeId === treeId) : null;
    if (approved) return approved.reason + ' (approved order)';
    for (const event of view.events) {
      for (const change of event.areas) {
        const fell = change.fells.find((record) => record.treeId === treeId);
        if (fell) return fell.reason + ' (executed Fell, work revision ' + fell.workRevision + ')';
      }
    }
    return null;
  }

  function keyFact(term, value, unit) {
    return el('div', null, el('dt', null, term), el('dd', null, value, ' ', el('small', null, unit)));
  }

  export function renderInspector() {
    const areaId = state.areaId;
    const id = selectedId();
    const tree = treeIn(areaId, id);
    const item = tree ? itemOf(areaId, id) : null;
    byId('inspector-empty').hidden = Boolean(tree && item);
    byId('inspector-body').hidden = !(tree && item);
    if (!tree || !item) return;
    const sp = speciesOf(tree.treeId);
    const spName = speciesLabel(sp);
    const isReference = referenceIn(areaId) === id;
    byId('insp-role').textContent = (isReference ? 'Reference tree' : 'Selected tree') + ' · ' + spName;
    byId('insp-area').textContent = areaName(areaId);
    byId('insp-id').textContent = tree.treeId;
    const chip = byId('insp-chip');
    chip.textContent = item.isCrop ? 'C · ' + label('CropRetained') : baseStatusText(item);
    chip.setAttribute('class', 'chip s-' + item.status + (item.isCrop ? ' crop' : ''));
    chip.setAttribute('title', chip.textContent);
    chip.hidden = !item.isCrop && item.status === 'Living';
    byId('insp-summary').textContent = spName + ' · ' + baseStatusText(item);
    byId('insp-key').replaceChildren(
      keyFact('DBH', fixed(tree.dbhCm, 1), 'cm'),
      keyFact('Height', fixed(tree.heightM, 1), 'm'),
      keyFact('Crown r', fixed(tree.crownRadiusM, 2), 'm'));
    byId('inspect-close').setAttribute('aria-label', 'Close inspection of ' + tree.treeId + ' (Escape)');
    byId('insp-pinned').textContent = isReference ? 'Reference stays pinned while choosing' : '';
    const facts = [
      pair('Species', spName + ' (' + sp + ')'),
      pair('DBH', fixed(tree.dbhCm, 2) + ' cm'),
      pair('Height', fixed(tree.heightM, 2) + ' m'),
      pair('Crown radius', fixed(tree.crownRadiusM, 3) + ' m'),
      pair('Age', tree.ageYears + ' years'),
      pair('Position', 'X ' + fixed(tree.xm, 2) + ' m east, Z ' + fixed(tree.zm, 2) + ' m north', 'wide'),
    ];
    if (tree.death) {
      facts.push(pair('Death year', tree.death.ecologicalYear));
      facts.push(pair('Cause', tree.death.cause + ' (' + tree.death.mechanism + ')'));
    }
    const s4 = s4Tree(areaId, tree.treeId);
    const ps = pruneStatusOf(areaId, tree.treeId);
    const dw = s4Area(areaId) ? s4Area(areaId).canopy.fallenDeadwood.find((d) => d.treeId === tree.treeId) : null;
    if (s4) {
      facts.push(pair('Life state', s4.lifeState + (s4.deathCause ? ' · ' + s4.deathCause + ', year ' + s4.deathYear : '')));
    } else if (tree.death && state.stage4) {
      const lifeState = tree.death.lifeState || (item.status === 'Felled' ? 'FelledStump' : 'BiologicallyDead');
      facts.push(pair('Life state', lifeState === 'BiologicallyDead'
        ? 'BiologicallyDead · ' + tree.death.cause + ', year ' + tree.death.ecologicalYear
        : lifeState));
    }
    if (tree.isLiving && ps) {
      facts.push(pair('Pruning lifts', ps.lifts + ' / ' + ps.maxLifts + ' (' + ps.minIntervalYears + '-yr wait)' +
        (s4 && s4.pruningLifts > 0 ? ' · crown base ' + fixed(s4.crownBaseHeightM, 1) + ' m · last lift year ' + s4.lastPruningYear : ''), 'wide'));
    }
    if (dw) {
      facts.push(pair('Fallen deadwood', dw.deadwoodId + ' · ' + spName + ' · vol ' + fixed(dw.estimatedVolumeM3, 3) + ' m³ · direction schematic, not simulated', 'wide'));
    }
    const reason = recordedReason(areaId, tree.treeId);
    if (reason !== null) facts.push(pair('Recorded reason', reason, 'wide'));
    facts.push(...scotsPineFacts(areaId, tree));
    byId('insp-facts').replaceChildren(...facts);
    const crop = byId('crop-button');
    crop.textContent = label(item.isCrop ? 'MultiUnmarkCrop' : 'MultiMarkCrop');
    crop.hidden = !tree.isLiving;
    byId('crop-note').textContent = item.isCrop ? label('MultiCropFellRefused') + '.' :
      item.status === 'PendingFell' || item.status === 'ApprovedFell' ? 'Remove the pending Fell or cancel its approved plan before designating Crop.' :
        tree.isLiving ? label('MultiCropSeparate') + '.' : 'Dead trees cannot retain a Crop designation.';
    const addSelf = item.status === 'Living' && !item.isCrop;
    byId('add-reference-wrap').hidden = !addSelf;
    byId('add-reference').textContent = 'Fell this tree instead';
    byId('add-reference').setAttribute('aria-label', 'Add ' + tree.treeId + ' itself to the Fell plan');
    byId('insp-remove-wrap').hidden = item.status !== 'PendingFell';
    byId('insp-remove').textContent = label('MultiRemoveFromPlan');
    byId('insp-approved-note').hidden = item.status !== 'ApprovedFell';
    byId('insp-threshold').textContent = label('MultiThresholdInformational');
    byId('insp-threshold').hidden = !tree.isLiving;
    renderPruneBox(areaId, tree, item, ps);
  }

  function renderPruneBox(areaId, tree, item, ps) {
    const box = byId('prune-box');
    box.hidden = !tree.isLiving || !ps;
    if (box.hidden) return;
    byId('prune-lifts').textContent = speciesLabel(ps.speciesId) + ' · ' + ps.lifts + ' / ' + ps.maxLifts + ' lifts (' + ps.minIntervalYears + '-yr wait)';
    let status;
    if (ps.planState === 'PendingPrune') status = 'Pending Prune — no crown is cut until the approved plan is executed.';
    else if (ps.planState === 'ApprovedPrune') status = 'Approved Prune — the crown is cut only when the plan is executed.';
    else if (ps.eligible) status = 'Eligible now: next lift raises the crown base to ' + fixed(ps.nextTargetM, 1) + ' m.';
    else status = 'Not eligible: ' + ps.refusalReason + (ps.nextEligibleYear !== undefined && ps.nextEligibleYear !== null
      ? ' (next lift possible in year ' + ps.nextEligibleYear + ')' : '') + '.';
    byId('prune-status').textContent = status;
    const add = byId('prune-add');
    add.textContent = label('MultiAddToPrunePlan');
    add.hidden = Boolean(ps.planState);
    const remove = byId('prune-remove');
    remove.hidden = ps.planState !== 'PendingPrune';
    remove.textContent = 'Remove Prune from Work Plan';
    remove.setAttribute('aria-label', 'Remove the pending Prune of ' + tree.treeId);
    byId('prune-why').textContent = pruneAvailability(areaId, item, ps).why || label('MultiPrunePendingNote') + '.';
  }

  export function pruneAvailability(areaId, item, ps) {
    if (!ps || !item) return { ok: false, why: 'Select a living tree.' };
    if (!ps.eligible) return { ok: false, why: 'Not eligible (engine): ' + ps.refusalReason + '.' };
    if (ps.planState) return { ok: false, why: 'Already in the Work Plan as a Prune.' };
    if (item.status === 'PendingFell' || item.status === 'ApprovedFell') return { ok: false, why: 'This tree has a Fell order; one order per tree.' };
    if (state.view.approved) return { ok: false, why: 'The plan is approved, so editing is locked. Cancel the approved plan to change it.' };
    return { ok: true, why: null };
  }

  export function renderPruneGate() {
    const areaId = state.areaId;
    const id = selectedId();
    const ps = id ? pruneStatusOf(areaId, id) : null;
    byId('prune-add').disabled = !pruneAvailability(areaId, id ? itemOf(areaId, id) : null, ps).ok;
  }

  // ---- Scots pine & site moisture (Part 2): inspector rows and live lens slots, every value read from Core ----------------

  async function requestScotsPineTree(areaId, treeId, key) {
    const req = ++state.scotsPineTreeRequest;
    state.scotsPineTree = { key: null, pending: key };
    const outcome = await fetchScotsPineTree(areaId, treeId);
    if (req !== state.scotsPineTreeRequest) return;
    state.scotsPineTree = outcome.kind === 'ok' ? { key, tree: outcome.tree }
      : { key, refusal: outcome.kind === 'refused' ? outcome.refusal : { code: 'Transport', message: outcome.detail } };
    if (hooks.renderAll) hooks.renderAll();
  }

  // The selected tree's Core site/pine detail when it is current for this exact root, else null (and a read is started).
  function scotsPineTreeDetail(areaId, treeId) {
    if (!scotsPineEcology() || !treeId) return null;
    const key = scotsPineTreeKey(areaId, treeId);
    const cached = state.scotsPineTree;
    if (cached && cached.key === key) return cached;
    if (!cached || cached.pending !== key) requestScotsPineTree(areaId, treeId, key);
    return { pending: key };
  }

  function scotsPineFacts(areaId, tree) {
    const detail = scotsPineTreeDetail(areaId, tree.treeId);
    if (!detail) return [];
    if (detail.pending) return [pair('Site moisture', 'Reading the Core site detail…', 'wide')];
    if (detail.refusal) return [pair('Site moisture', 'Unavailable (' + detail.refusal.code + ')', 'wide')];
    const t = detail.tree;
    const rows = [
      pair('Site moisture', t.moistureLabel + (t.habitatCaution ? ' · habitat-protection caution' : ''), 'wide'),
      pair('Moisture modifiers', t.modifierIsNeutral
        ? 'NeutralLegacy — no moisture modifier (establishment 1.00 · growth 1.00)'
        : speciesLabel(t.speciesId) + ': establishment ' + fixed(t.establishmentModifier, 2) + ' · growth ' + fixed(t.growthModifier, 2) +
          ' (separate modifiers, [Calibration])', 'wide'),
    ];
    if (t.isScotsPine) {
      rows.push(pair('GROWFOR growth', t.withinGrowthDomain
        ? 'Inside the domain · DBH +' + fixed(t.siteAdjustedDbhIncrementCm, 3) + ' cm/yr (potential ' + fixed(t.potentialDbhIncrementCm, 3) + ' cm/yr)'
        : t.growthNote, 'wide'));
      rows.push(pair('Pine pruning', t.pruningLifts + ' / 2 lifts · pruned to ' + fixed(t.prunedHeightM, 1) + ' m' +
        (t.nextLiftTargetM !== null && t.nextLiftTargetM !== undefined ? ' · next lift to ' + fixed(t.nextLiftTargetM, 1) + ' m' : ' · 5–6 m target reached'), 'wide'));
      rows.push(pair('Timber grade now', t.timberSortment + ' · €' + fixed(t.timberPriceEurPerTonne, 0) + '/t · ' + fixed(t.timberVolumeM3, 3) + ' m³ → ' +
        fixed(t.timberGreenTonnes, 3) + ' green t · gross €' + fixed(t.timberValueEur, 2) + ' [Calibration]', 'wide'));
      rows.push(pair('Grade reason', t.timberReason + (t.tightEarlyStocking ? ' Tight early stocking recorded.' : ''), 'wide'));
    }
    return rows;
  }

  function zoneModifierText(site, speciesId) {
    return site.modifiers.filter((m) => m.speciesId === speciesId).map((m) => {
      const zone = site.zones.find((z) => z.moistureClass === m.moistureClass);
      return (zone ? zone.label : m.moistureClass) + ': establishment ' + fixed(m.establishment, 2) + ' · growth ' + fixed(m.growth, 2);
    }).join(' | ');
  }

  function selectedPineDetail() {
    const id = selectedId();
    const cached = state.scotsPineTree;
    return id && cached && cached.tree && cached.key === scotsPineTreeKey(state.areaId, id) && cached.tree.isScotsPine ? cached.tree : null;
  }

  function liveSlotLines(slotId, sp) {
    const site = sp.site;
    const last = sp.accounts.length > 0 ? sp.accounts[sp.accounts.length - 1] : null;
    const pine = selectedPineDetail();
    const lines = [];
    switch (slotId) {
      case 'scots-pine-profile':
        lines.push('Living Scots pine adults: ' + sp.livingPineAdults + ' · planted pine saplings active: ' + sp.activePlantedPine);
        if (last) lines.push('Year ' + last.yearBefore + '→' + last.yearAfter + ': ' + last.adultsGrowing + ' grew inside the GROWFOR domain, ' +
          last.adultsOutsideDomain + ' outside it (no DBH growth modelled)');
        lines.push('Pruning: 2 lifts — 2.5 m then 5.5 m (5–6 m clear-bole target), 5 years apart');
        if (pine) lines.push('Selected ' + pine.treeId + ': ' + (pine.withinGrowthDomain ? 'DBH +' + fixed(pine.siteAdjustedDbhIncrementCm, 3) + ' cm/yr' : 'outside the growth domain'));
        break;
      case 'soil-moisture-overlay':
        lines.push('Site: ' + site.wholeStandLabel);
        if (!site.isNeutral) {
          lines.push('Scots pine — ' + zoneModifierText(site, 'scots-pine'));
          lines.push('Sitka spruce — ' + zoneModifierText(site, 'sitka-spruce'));
        } else {
          lines.push('NeutralLegacy: every species keeps establishment 1.00 · growth 1.00 (no moisture effect)');
        }
        lines.push('Next year: ' + (sp.nextYearIsGoodSeedYear ? 'good' : 'poor') + ' Scots pine seed year (local seed only)');
        lines.push('Natural pine recruits active: ' + sp.recruits.length + (last ? ' · year ' + last.yearAfter + ': ' + last.recruited +
          ' recruited in ' + last.eligibleCells + ' eligible gap cells, ' + last.promoted + ' promoted' : ''));
        lines.push(sp.plant.note);
        break;
      case 'scots-pine-timber-grader':
        lines.push('Sawlog €' + fixed(sp.timber.sawlogEurPerTonne, 0) + '/t · Small roundwood €' + fixed(sp.timber.smallRoundwoodEurPerTonne, 0) +
          '/t · Pulp/chip €' + fixed(sp.timber.pulpChipEurPerTonne, 0) + '/t at ' + fixed(sp.timber.greenDensityTPerM3, 2) + ' green t/m³');
        lines.push(sp.timber.rule);
        if (pine) lines.push('Selected ' + pine.treeId + ': ' + pine.timberSortment + ' · gross €' + fixed(pine.timberValueEur, 2));
        break;
      case 'sensitive-wet-ground':
        lines.push(site.habitatCaution ? site.habitatCautionText : 'No stagnant poor bog on this site — no habitat-protection caution.');
        lines.push('Scots pine deadwood logs: ' + sp.pineDeadwood);
        break;
      default:
        return null;
    }
    return lines;
  }

  export function renderLensSlots() {
    const host = byId('lens-slots-list');
    const summary = byId('lens-slots-summary');
    if (!host || !summary) return;
    const lens = currentLens();
    const slots = lens.extensionSlots || [];
    const sp = scotsPineEcology();
    summary.textContent = lens.label + ' lens · Part 2 slots (' + slots.length + ')';
    if (slots.length === 0) {
      const rows = [];
      if (sp) {
        rows.push(el('li', { 'data-site-moisture': sp.site.mode, 'data-extension-lens': lens.id },
          el('div', { class: 'slot-row-head' }, el('strong', null, 'Site moisture'), el('span', { class: 'tag' }, 'Core')),
          el('p', { class: 'small-note', 'data-site-label': 'true' }, sp.site.wholeStandLabel),
          sp.site.habitatCaution ? el('p', { class: 'warning slot-caution', role: 'note' }, sp.site.habitatCautionText) : null));
      }
      rows.push(el('li', { class: 'small-note' },
        'Woodland overview combines active layers. Switch to Trees, Regeneration, Harvesting, or Habitat to see their Part 2 feature slots.'));
      host.replaceChildren(...rows);
      return;
    }
    host.replaceChildren(...slots.map((slot) => {
      const lines = sp && SCOTS_PINE_SLOT_IDS.includes(slot.id) ? liveSlotLines(slot.id, sp) : null;
      return el('li', { 'data-extension-slot': slot.id, 'data-extension-lens': lens.id, 'data-slot-live': lines ? 'true' : 'false' },
        el('div', { class: 'slot-row-head' },
          el('strong', null, slot.feature),
          el('span', { class: 'tag' }, lines ? 'Live · Core' : 'Part 2 slot')),
        el('p', { class: 'small-note' }, slot.label),
        lines ? el('ul', { class: 'slot-live' }, ...lines.map((line, i) => el('li',
          i === 0 && slot.id === 'sensitive-wet-ground' && sp.site.habitatCaution ? { class: 'warning slot-caution', role: 'note' } : null, line))) : null);
    }));
  }

