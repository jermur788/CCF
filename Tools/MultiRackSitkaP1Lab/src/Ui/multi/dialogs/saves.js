// S0c provenance: dialogs/saves.js [L4] — section(s) 4 from multi.js
import { announce, applyPayload, clearAlert, currentFence, showAlert, syncStandOverview } from '../api.js';
import { openDialog } from '../commands.js';
import { el, fill, label } from '../dom.js';
import { byId, hooks, isEconomyEnabled, resetSessionUiState, state } from '../store.js';
  // ---- Phase 1 Save/Load (server-managed named slots; no path, no upload, no download) --------------------------------

  const SAVE_STATE = {
    valid: ['Ready to load', 'state-valid'],
    incompatible: ['Different forest, model or engine', 'state-incompatible'],
    corrupt: ['Corrupt — not loadable', 'state-corrupt'],
    missing: ['Missing file', 'state-missing'],
  };

  function savedSlot(slot) { return (state.saves || []).find((row) => row.slot === slot) || null; }

  function saveStatus(text, kind) {
    const box = byId('saves-status');
    box.textContent = text;
    box.className = 'save-status' + (kind ? ' ' + kind : '');
  }

  // Save/Load routes answer their own payload shapes (no composite view on a save, a composite view on a load), so they
  // use this small JSON helper instead of call().
  export async function jsonCall(path, method, body) {
    const init = { method, cache: 'no-store', credentials: 'omit', redirect: 'error' };
    if (body !== undefined) {
      init.headers = { 'Content-Type': 'application/json' };
      init.body = JSON.stringify(body);
    }
    let response;
    try { response = await fetch(path, init); } catch (failure) {
      return { kind: 'transport', detail: String((failure && failure.message) || failure) };
    }
    let payload = null;
    try { payload = await response.json(); } catch { payload = null; }
    const okStatus = Boolean(response.ok) || (response.status >= 200 && response.status < 300);
    if (okStatus && payload) return { kind: 'ok', status: response.status, payload };
    if (payload && payload.view && payload.refusal) return { kind: 'refused', status: response.status, payload };
    if (payload && (payload.refusal || payload.error)) return { kind: 'guard', status: response.status, payload };
    return { kind: 'transport', status: response.status, detail: 'HTTP ' + response.status };
  }

  export function noticeOf(outcome) {
    if (outcome.payload) {
      if (outcome.payload.refusal) return outcome.payload.refusal;
      if (outcome.payload.error) return outcome.payload.error;
    }
    return { code: 'Transport', message: outcome.detail || 'no confirmed answer from the local server' };
  }

  export async function openSaves() {
    byId('saves-title').textContent = label('MultiSavesTitle') || 'Saved games';
    byId('saves-intro').textContent = label('MultiSavesIntro');
    byId('save-slot-hint').textContent = label('MultiSavesNameHint');
    byId('save-overwrite-text').textContent = label('MultiSavesOverwrite');
    byId('save-do').textContent = label('MultiSavesSave');
    byId('load-confirm').hidden = true;
    byId('load-confirm-text').textContent = '';
    saveStatus('', null);
    openDialog(byId('saves-dialog'), byId('save-slot'));
    await refreshSaves();
  }

  async function refreshSaves() {
    const outcome = await jsonCall('api/multi/saves', 'GET');
    if (outcome.kind !== 'ok' || !outcome.payload) {
      const notice = noticeOf(outcome);
      saveStatus('Could not read the saved games list (' + notice.code + ').', 'bad');
      return;
    }
    state.saves = outcome.payload.slots || [];
    if (outcome.payload.refusal) {
      saveStatus(outcome.payload.refusal.code + ': ' + outcome.payload.refusal.message +
        ' Save v3 economy persistence is deferred to Phase 2B-2b; switch Money Off to use Save v2 slots.', 'warn');
    }
    renderSaves();
  }

  function renderSaves() {
    const rows = (state.saves || []).map((slot) => {
      const [word, stateClass] = SAVE_STATE[slot.status] || [slot.status, 'state-corrupt'];
      const actions = [];
      if (slot.canLoad) {
        actions.push(el('button', { type: 'button', class: 'small', 'data-load': slot.slot,
          'aria-label': 'Load saved slot ' + slot.slot }, label('MultiSavesLoad') || 'Load'));
      }
      if (slot.backupExists && slot.recoverable) {
        actions.push(el('button', { type: 'button', class: 'small', 'data-recover': slot.slot,
          'aria-label': 'Restore the verified backup of ' + slot.slot }, label('MultiSavesRecover') || 'Restore backup'));
      }
      return el('tr', null,
        el('td', { class: 'slot-cell' }, el('code', null, slot.slot),
          slot.schemaVersion === 3 ? el('span', { class: 'tag slot-ver' }, 'v3') : null),
        el('td', { class: 'num' }, slot.ecologicalYear === null || slot.ecologicalYear === undefined ? '–' : String(slot.ecologicalYear)),
        el('td', { class: 'num' }, slot.savedUtc || '–'),
        el('td', { class: stateClass }, word,
          slot.refusalCode ? el('span', { class: 'note' }, ' (' + slot.refusalCode + ')') : null,
          slot.backupExists && slot.backupLoadable ? el('span', { class: 'note' }, ' · backup available') : null),
        el('td', null, el('div', { class: 'slot-actions' }, ...actions)));
    });
    byId('saves-rows').replaceChildren(...(rows.length > 0 ? rows
      : [el('tr', null, el('td', { colspan: 5, class: 'save-empty' },
        isEconomyEnabled()
          ? 'Save v2 slots are unavailable while Money is On (Save v3 deferred to Phase 2B-2b).'
          : (label('MultiSavesEmpty') || 'No saved slots yet.')))]));
    updateSaveControls();
  }

  function saveNameValid(name) { return /^[a-z0-9][a-z0-9-]{0,47}$/.test(name); }

  export function updateSaveControls() {
    const name = byId('save-slot').value.trim();
    const existing = saveNameValid(name) ? savedSlot(name) : null;
    byId('save-overwrite-wrap').hidden = !existing;
    if (!existing) byId('save-overwrite').checked = false;
    byId('save-do').disabled = !saveNameValid(name) || state.busy || isEconomyEnabled();
  }

  export async function onSave() {
    if (!state.view || state.busy) return;
    const slot = byId('save-slot').value.trim();
    if (!saveNameValid(slot)) {
      saveStatus('A slot name is 1–48 lowercase letters, digits or hyphens, starting with a letter or digit. Nothing was written.', 'bad');
      return;
    }
    if (state.lock) {
      saveStatus('Outcome unconfirmed — re-read the current state before saving. Nothing was written.', 'bad');
      return;
    }
    const existing = savedSlot(slot);
    if (existing && !byId('save-overwrite').checked) {
      saveStatus('Slot "' + slot + '" already holds a save. Tick "' + label('MultiSavesOverwrite') + '" to replace it. Nothing was written.', 'warn');
      return;
    }
    state.busy = true;
    document.documentElement.classList.add('busy');
    const outcome = await jsonCall('api/multi/save', 'POST', { fence: currentFence(), slot, replaceExisting: Boolean(existing) });
    state.busy = false;
    document.documentElement.classList.remove('busy');
    if (outcome.kind === 'ok' && outcome.payload && outcome.payload.saved) {
      const saved = outcome.payload.saved;
      const durable = saved.status === 'durablyCommitted';
      saveStatus('Saved "' + saved.slot + '" · year ' + saved.ecologicalYear + ' · ' + saved.savedUtc + ' · ' +
        (durable ? label('MultiSavesDurable') : label('MultiSavesDurabilityUnconfirmed')), durable ? 'ok' : 'warn');
      state.hasUnsavedProgress = false;
      byId('save-overwrite').checked = false;
      announce('Saved slot ' + saved.slot + ' in year ' + saved.ecologicalYear + '.');
      await refreshSaves();
      return;
    }
    const notice = noticeOf(outcome);
    saveStatus('Not saved (' + notice.code + '): ' + notice.message, 'bad');
  }

  export function askLoad(slot) {
    const row = savedSlot(slot);
    if (!row || !row.canLoad || state.busy) return;
    byId('recover-confirm').hidden = true;
    state.recoverSlot = null;
    state.loadSlot = slot;
    byId('load-confirm-text').textContent =
      fill(label('MultiSavesLoadConfirm'), { slot: slot });
    byId('load-confirm').hidden = false;
    byId('load-do').focus();
  }

  // A load replaces the whole live forest, so every local selection, draft, preview, in-flight request token and pending
  // confirmation of the old session is dropped here BEFORE the new payload is applied.
  function resetForLoad() {
    resetSessionUiState();
  }

  export async function onLoadConfirmed() {
    const slot = state.loadSlot;
    if (!slot || !state.view || state.busy) return;
    state.busy = true;
    document.documentElement.classList.add('busy');
    const outcome = await jsonCall('api/multi/load', 'POST', { fence: currentFence(), slot, confirmDiscardUnsaved: true });
    state.busy = false;
    document.documentElement.classList.remove('busy');
    byId('load-confirm').hidden = true;
    if (outcome.kind === 'ok' && outcome.payload && outcome.payload.view) {
      resetForLoad();
      applyPayload(outcome.payload, false);
      byId('saves-dialog').close();
      if (state.view && state.view.areas && state.view.areas.length >= 3) {
        await syncStandOverview();
      }
      const info = outcome.payload.restored;
      const draft = info && info.draftOrderCount > 0 ? fill(label('MultiSavesPendingDraft'), { n: info.draftOrderCount }) : null;
      const areaScopeText = state.view.areas.length >= 3 ? ('all ' + state.view.areas.length + ' lanes') : 'both areas';
      clearAlert();
      showAlert([
        el('p', { class: 'alert-title' }, 'Loaded "' + slot + '"'),
        el('p', null, 'Year ' + state.view.source.ecologicalYear + ', ' + areaScopeText + ' restored from the save; the session has a new authority, so every earlier token is refused.'),
        draft ? el('p', { class: 'warning' }, draft) : null,
        el('p', null, 'Advance or approve and execute work to continue from here.'),
      ]);
      announce('Loaded ' + slot + ' in year ' + state.view.source.ecologicalYear + '.' + (draft ? ' ' + draft : ''));
      hooks.refreshReviewPreview();
      if (state.historyOpen) hooks.loadHistoryIndex(null, false);
      if (byId('side').getAttribute('data-pane') === 'details') hooks.refreshStructure();
      return;
    }
    if (outcome.kind === 'refused' && outcome.payload && outcome.payload.view) {
      // The server replaced nothing: it answered with the unchanged live session, so the page keeps working authority.
      applyPayload(outcome.payload, false);
      const notice = noticeOf(outcome);
      saveStatus('Load refused (' + notice.code + '): ' + notice.message + ' The current forest is unchanged.', 'bad');
      return;
    }
    const notice = noticeOf(outcome);
    saveStatus('Load not confirmed (' + notice.code + '): ' + notice.message + ' Re-read the current state before trying again.', 'bad');
  }

  export function askRecover(slot) {
    const row = savedSlot(slot);
    if (!row || !row.recoverable || state.busy) return;
    byId('load-confirm').hidden = true;
    state.loadSlot = null;
    state.recoverSlot = slot;
    byId('recover-confirm-text').textContent = fill(label('MultiSavesRecoverConfirm'), { slot: slot });
    byId('recover-confirm').hidden = false;
    byId('recover-do').focus();
  }

  export async function onRecoverConfirmed() {
    const slot = state.recoverSlot;
    if (!slot || state.busy) return;
    state.busy = true;
    document.documentElement.classList.add('busy');
    const outcome = await jsonCall('api/multi/saves/recover', 'POST', { slot });
    state.busy = false;
    document.documentElement.classList.remove('busy');
    byId('recover-confirm').hidden = true;
    state.recoverSlot = null;
    if (outcome.kind === 'ok') {
      saveStatus('Restored the verified backup of "' + slot + '" over its unreadable primary file. Load it to play that forest.', 'ok');
      await refreshSaves();
      return;
    }
    const notice = noticeOf(outcome);
    saveStatus('Backup not restored (' + notice.code + '): ' + notice.message, 'bad');
  }

