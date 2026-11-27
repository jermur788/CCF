// S0c provenance: panels/inspector.js [L4] — section(s) 12 from multi.js
import { el, fill, fixed, label, pair } from '../dom.js';
import { areaName, baseStatusText, byId, currentLens, itemOf, pruneStatusOf, referenceIn, s4Area, s4Tree, selectedId, speciesLabel, speciesOf, state, statusText, treeIn } from '../store.js';
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

  export function renderLensSlots() {
    const host = byId('lens-slots-list');
    const summary = byId('lens-slots-summary');
    if (!host || !summary) return;
    const lens = currentLens();
    const slots = lens.extensionSlots || [];
    summary.textContent = lens.label + ' lens · Part 2 slots (' + slots.length + ')';
    if (slots.length === 0) {
      host.replaceChildren(el('li', { class: 'small-note' },
        'Woodland overview combines active layers. Switch to Trees, Regeneration, Harvesting, or Habitat to see their Part 2 feature slots.'));
      return;
    }
    host.replaceChildren(...slots.map((slot) => el('li', { 'data-extension-slot': slot.id, 'data-extension-lens': lens.id },
      el('div', { class: 'slot-row-head' },
        el('strong', null, slot.feature),
        el('span', { class: 'tag' }, 'Part 2 slot')),
      el('p', { class: 'small-note' }, slot.label))));
  }

