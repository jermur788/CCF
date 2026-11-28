// S0c provenance: dom.js [L1] — section(s) 1 from multi.js
import { EXPLANATIONS, SVG_NS, byId, state } from './store.js';
  // ---- small DOM helpers ----------------------------------------------------------------------------------------------

  export function setAttributes(node, attributes) {
    for (const [name, value] of Object.entries(attributes || {})) {
      if (value !== null && value !== undefined && value !== false) node.setAttribute(name, String(value));
    }
  }

  export function el(tag, attributes, ...children) {
    const node = document.createElement(tag);
    setAttributes(node, attributes);
    for (const child of children) {
      if (child === null || child === undefined) continue;
      node.append(child instanceof Node ? child : String(child));
    }
    return node;
  }

  export function svg(tag, attributes, textValue) {
    const node = document.createElementNS(SVG_NS, tag);
    setAttributes(node, attributes);
    if (textValue !== undefined) node.textContent = String(textValue);
    return node;
  }

  export function useIcon(id, size) {
    const box = svg('svg', { viewBox: '0 0 32 32', width: size || 16, height: size || 16, 'aria-hidden': 'true', focusable: 'false' });
    box.append(svg('use', { href: '#' + id, x: 0, y: 0, width: 32, height: 32 }));
    return box;
  }

  export function pair(term, value, className) {
    return el('div', className ? { class: className } : null, el('dt', null, term), el('dd', null, value));
  }

  export function label(name) {
    return state.labels && typeof state.labels[name] === 'string' ? state.labels[name] : '';
  }

  export function fill(template, values) {
    return template.replace(/\{(\w+)\}/g, (match, name) => (name in values ? String(values[name]) : match));
  }

  export function fixed(value, digits) {
    return typeof value === 'number' ? value.toFixed(digits) : String(value);
  }

  export function lightIndexText(value) {
    return typeof value === 'number' && value > 0 && value < 0.0005 ? '< 0.001' : fixed(value, 3);
  }

  export function plural(n, one, many) { return n + ' ' + (n === 1 ? one : many); }
  export function listIds(ids) { return ids.length === 0 ? 'none' : ids.join(', '); }
  export const n3 = (value) => Math.round(value * 1000) / 1000;

  export function expandable(full) {
    const value = String(full);
    return el('details', { class: 'expand' },
      el('summary', null, el('span', { class: 'mono' }, value.slice(0, 12) + '…'), el('span', { class: 'sr-only' }, ' show the full value')),
      el('code', { class: 'full' }, value));
  }

  export function technicalPlan(planId, executionKey) {
    return el('details', { class: 'technical-plan' }, el('summary', null, 'Technical reference'),
      el('p', null, 'PlanId ', el('code', null, planId)), el('p', null, 'Execution key ', el('code', null, executionKey)));
  }

  export function table(caption, headers, rows) {
    return el('div', { class: 'table-wrap', role: 'region', 'aria-label': caption, tabindex: '0' },
      el('table', null, el('caption', null, caption),
        el('thead', null, el('tr', null, ...headers.map((header) => el('th', { scope: 'col' }, header)))),
        el('tbody', null, ...rows.map((cells) => el('tr', null, el('th', { scope: 'row' }, cells[0]),
          ...cells.slice(1).map((cell) => el('td', null, cell)))))));
  }

  // ---- Shared single-level accessible pinned explanation component (WCAG 2.2 SC 1.4.13; Safe Hybrid) ------------------

  const explainCtrl = {
    trigger: null,
    key: null,
    pinned: false,
    closeTimer: 0,
    wired: false,
  };

  export function explainTrigger(key, ariaLabel) {
    const info = EXPLANATIONS[key];
    return el('button', {
      type: 'button',
      class: 'explain-btn',
      'data-explain': key,
      'aria-expanded': 'false',
      'aria-controls': 'explain-popover',
      'aria-label': ariaLabel || (info ? 'Explain: ' + info.title : 'Show explanation'),
      title: info ? info.title + ' (hover, focus, or click to pin)' : 'Explanation',
    }, '?');
  }

  export function isExplainOpen() {
    const pop = byId('explain-popover');
    return Boolean(pop && !pop.hidden);
  }

  export function isExplainPinned() {
    return isExplainOpen() && explainCtrl.pinned;
  }

  function cancelExplainClose() {
    if (explainCtrl.closeTimer) {
      window.clearTimeout(explainCtrl.closeTimer);
      explainCtrl.closeTimer = 0;
    }
  }

  function scheduleExplainClose() {
    if (explainCtrl.pinned) return;
    cancelExplainClose();
    explainCtrl.closeTimer = window.setTimeout(() => {
      explainCtrl.closeTimer = 0;
      if (!explainCtrl.pinned) closeExplain(false);
    }, 180);
  }

  function positionExplainPopover(triggerEl) {
    const pop = byId('explain-popover');
    if (!pop || !triggerEl) return;
    const rect = triggerEl.getBoundingClientRect();
    const popW = Math.min(320, window.innerWidth - 16);
    const popH = pop.offsetHeight || 120;
    let left = rect.left;
    if (left + popW > window.innerWidth - 8) left = window.innerWidth - popW - 8;
    if (left < 8) left = 8;
    let top = rect.bottom + 6;
    if (top + popH > window.innerHeight - 8) {
      top = Math.max(8, rect.top - popH - 6);
    }
    pop.style.left = Math.round(left) + 'px';
    pop.style.top = Math.round(top) + 'px';
  }

  export function openExplain(triggerEl, pin) {
    const pop = byId('explain-popover');
    if (!pop || !triggerEl) return false;
    const key = triggerEl.getAttribute('data-explain');
    const info = key ? EXPLANATIONS[key] : null;
    if (!info) return false;
    cancelExplainClose();
    if (explainCtrl.trigger && explainCtrl.trigger !== triggerEl) {
      explainCtrl.trigger.setAttribute('aria-expanded', 'false');
    }
    explainCtrl.trigger = triggerEl;
    explainCtrl.key = key;
    explainCtrl.pinned = Boolean(pin);
    byId('explain-title').textContent = info.title;
    byId('explain-body').textContent = info.body;
    const tagEl = byId('explain-tag');
    if (tagEl) {
      tagEl.textContent = info.tag || '';
      tagEl.hidden = !info.tag;
    }
    const pinTag = byId('explain-pin-tag');
    if (pinTag) pinTag.hidden = !explainCtrl.pinned;
    const hint = byId('explain-hint');
    if (hint) {
      hint.textContent = explainCtrl.pinned
        ? 'Pinned · Press Esc or × to close'
        : 'Hoverable · Click or press Enter to pin · Esc to dismiss';
    }
    pop.setAttribute('data-explain-key', key);
    pop.setAttribute('data-pinned', String(explainCtrl.pinned));
    pop.hidden = false;
    triggerEl.setAttribute('aria-expanded', 'true');
    positionExplainPopover(triggerEl);
    return true;
  }

  export function closeExplain(returnFocus) {
    const pop = byId('explain-popover');
    cancelExplainClose();
    if (!pop || pop.hidden) return false;
    const prevTrigger = explainCtrl.trigger;
    pop.hidden = true;
    pop.setAttribute('data-pinned', 'false');
    pop.removeAttribute('data-explain-key');
    if (prevTrigger) prevTrigger.setAttribute('aria-expanded', 'false');
    explainCtrl.trigger = null;
    explainCtrl.key = null;
    explainCtrl.pinned = false;
    if (returnFocus && prevTrigger && prevTrigger.isConnected && !prevTrigger.closest('[hidden]') && !prevTrigger.disabled) {
      prevTrigger.focus();
    }
    return true;
  }

  export function initExplainPopover() {
    if (explainCtrl.wired) return;
    explainCtrl.wired = true;
    const pop = byId('explain-popover');
    const host = typeof document.addEventListener === 'function' ? document : document.documentElement;
    if (!pop || !host || typeof host.addEventListener !== 'function') return;

    host.addEventListener('mouseover', (event) => {
      const btn = event.target && event.target.closest ? event.target.closest('[data-explain]') : null;
      if (btn) {
        if (explainCtrl.pinned && explainCtrl.trigger !== btn) return;
        openExplain(btn, explainCtrl.pinned && explainCtrl.trigger === btn);
        return;
      }
      if (pop.contains && pop.contains(event.target)) {
        cancelExplainClose();
      }
    });

    host.addEventListener('mouseout', (event) => {
      const btn = event.target && event.target.closest ? event.target.closest('[data-explain]') : null;
      const inPop = Boolean(pop.contains && pop.contains(event.target));
      if (!btn && !inPop) return;
      const related = event.relatedTarget;
      if (related && ((pop.contains && pop.contains(related)) || (explainCtrl.trigger && explainCtrl.trigger.contains && explainCtrl.trigger.contains(related)))) {
        return;
      }
      scheduleExplainClose();
    });

    host.addEventListener('focusin', (event) => {
      const btn = event.target && event.target.closest ? event.target.closest('[data-explain]') : null;
      if (btn) {
        if (explainCtrl.pinned && explainCtrl.trigger !== btn) return;
        openExplain(btn, explainCtrl.pinned && explainCtrl.trigger === btn);
        return;
      }
      if (pop.contains && pop.contains(event.target)) {
        cancelExplainClose();
      } else if (!explainCtrl.pinned && isExplainOpen()) {
        closeExplain(false);
      }
    });

    host.addEventListener('click', (event) => {
      const closeBtn = event.target && event.target.closest ? event.target.closest('#explain-close') : null;
      if (closeBtn) {
        event.stopPropagation();
        closeExplain(true);
        return;
      }
      const btn = event.target && event.target.closest ? event.target.closest('[data-explain]') : null;
      if (btn) {
        event.preventDefault();
        event.stopPropagation();
        if (explainCtrl.pinned && explainCtrl.trigger === btn) {
          closeExplain(false);
        } else {
          openExplain(btn, true);
        }
        return;
      }
      if (isExplainOpen() && !pop.contains(event.target)) {
        closeExplain(false);
      }
    });

    pop.addEventListener('mouseenter', cancelExplainClose);
    pop.addEventListener('mouseleave', scheduleExplainClose);
    window.addEventListener('resize', () => {
      if (isExplainOpen() && explainCtrl.trigger && explainCtrl.trigger.isConnected) {
        positionExplainPopover(explainCtrl.trigger);
      }
    });
  }

