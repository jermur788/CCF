/**
 * Clean Woodland view: the no-MapLibre style and every Clean Woodland fallback.
 *
 * It draws the same Layers 3-8 as Satellite (see overlay-layers.js) over the plain #15251a
 * canvas, north-up in local metres. This module only works out the metres -> screen transform:
 * a uniform scale (so circles stay circles) plus a translation that centres the focus box.
 */

import { focusBox } from './frame-projector.js';

const PADDING_M = 4;

/**
 * @returns {{a:number,b:number,c:number,d:number,e:number,f:number}|null}
 *   Affine matrix for SVG transform="matrix(a b c d e f)", mapping overlay units (X = xM, Y = -zM)
 *   to stage pixels; null when the frame has no drawable area.
 */
export function cleanWoodlandMatrix(frame, focusAreaId, widthPx, heightPx) {
  const box = focusBox(frame, focusAreaId);
  if (!box) return null;
  const spanX = Math.max(1, box.maxXM - box.minXM) + 2 * PADDING_M;
  const spanZ = Math.max(1, box.maxZM - box.minZM) + 2 * PADDING_M;
  const scale = Math.min(Math.max(1, widthPx) / spanX, Math.max(1, heightPx) / spanZ);
  const centreX = (box.minXM + box.maxXM) / 2;
  const centreY = -(box.minZM + box.maxZM) / 2;
  return { a: scale, b: 0, c: 0, d: scale, e: widthPx / 2 - scale * centreX, f: heightPx / 2 - scale * centreY };
}
