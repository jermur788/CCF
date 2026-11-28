/**
 * Pure-Node synthetic georeferenced PNG generator and PNG RGBA pixel reader (zero dependencies).
 * Produces a synthetic local test image (clearly distinct from #15251a)
 * and decodes captured CDP PNG screenshots for pixel verification.
 */

import { deflateSync, inflateSync } from 'node:zlib';

const CRC_TABLE = (() => {
  const tbl = new Uint32Array(256);
  for (let n = 0; n < 256; n++) {
    let c = n;
    for (let k = 0; k < 8; k++) {
      c = (c & 1) ? (0xedb88320 ^ (c >>> 1)) : (c >>> 1);
    }
    tbl[n] = c >>> 0;
  }
  return tbl;
})();

function crc32(buf) {
  let c = 0xffffffff;
  for (let i = 0; i < buf.length; i++) {
    c = CRC_TABLE[(c ^ buf[i]) & 0xff] ^ (c >>> 8);
  }
  return (c ^ 0xffffffff) >>> 0;
}

function pngChunk(type, data) {
  const typeBuf = Buffer.from(type, 'ascii');
  const lenBuf = Buffer.alloc(4);
  lenBuf.writeUInt32BE(data.length, 0);
  const crcInput = Buffer.concat([typeBuf, data]);
  const crcBuf = Buffer.alloc(4);
  crcBuf.writeUInt32BE(crc32(crcInput), 0);
  return Buffer.concat([lenBuf, typeBuf, data, crcBuf]);
}

/**
 * Deterministic synthetic 256x256 RGB PNG used as "local georeferenced imagery" in tests.
 * It covers local -24..104 m on both axes. Outside the woodland (an outer 48 px band) it is a
 * uniform rgb(196, 168, 92), so screenshot pixels there can be compared with an exact blend.
 * Inside the woodland footprint it carries a visible checker so it is plainly an image.
 */
export function generateSyntheticWoodlandPng(width = 256, height = 256) {
  const sig = Buffer.from([137, 80, 78, 71, 13, 10, 26, 10]);
  const ihdr = Buffer.alloc(13);
  ihdr.writeUInt32BE(width, 0);
  ihdr.writeUInt32BE(height, 4);
  ihdr[8] = 8; // bit depth
  ihdr[9] = 2; // colour type RGB
  const stride = 1 + width * 3;
  const raw = Buffer.alloc(stride * height);
  const inner = (p, size) => p >= size * 0.1875 && p < size * 0.8125;
  for (let y = 0; y < height; y++) {
    raw[y * stride] = 0; // filter: none
    for (let x = 0; x < width; x++) {
      const o = y * stride + 1 + x * 3;
      const shade = inner(x, width) && inner(y, height) ? (((x >> 4) + (y >> 4)) % 2 === 0 ? 18 : -18) : 0;
      raw[o] = 196 + shade;
      raw[o + 1] = 168 + shade;
      raw[o + 2] = 92 + shade;
    }
  }
  return Buffer.concat([sig, pngChunk('IHDR', ihdr), pngChunk('IDAT', deflateSync(raw)), pngChunk('IEND', Buffer.alloc(0))]);
}

function paethPredictor(a, b, c) {
  const p = a + b - c;
  const pa = Math.abs(p - a);
  const pb = Math.abs(p - b);
  const pc = Math.abs(p - c);
  if (pa <= pb && pa <= pc) return a;
  if (pb <= pc) return b;
  return c;
}

/**
 * Decode an 8-bit RGB or RGBA PNG buffer into `{ width, height, rgba: Uint8Array }`.
 * Handles all 5 PNG scanline filters (None, Sub, Up, Average, Paeth).
 */
export function decodePngRgba(buf) {
  if (buf.length < 24 || buf[0] !== 137 || buf[1] !== 80 || buf[2] !== 78 || buf[3] !== 71) {
    throw new Error('Invalid PNG signature');
  }
  let offset = 8;
  let width = 0;
  let height = 0;
  let bitDepth = 0;
  let colorType = 0;
  const idatChunks = [];

  while (offset + 8 <= buf.length) {
    const length = buf.readUInt32BE(offset);
    const type = buf.toString('ascii', offset + 4, offset + 8);
    const dataStart = offset + 8;
    const dataEnd = dataStart + length;
    if (type === 'IHDR') {
      width = buf.readUInt32BE(dataStart);
      height = buf.readUInt32BE(dataStart + 4);
      bitDepth = buf[dataStart + 8];
      colorType = buf[dataStart + 9];
    } else if (type === 'IDAT') {
      idatChunks.push(buf.subarray(dataStart, dataEnd));
    } else if (type === 'IEND') {
      break;
    }
    offset = dataEnd + 4;
  }

  if (bitDepth !== 8 || (colorType !== 2 && colorType !== 6)) {
    throw new Error(`Unsupported PNG format: bitDepth=${bitDepth}, colorType=${colorType}`);
  }
  const bpp = colorType === 6 ? 4 : 3;
  const stride = width * bpp;
  const inflated = inflateSync(Buffer.concat(idatChunks));
  const recon = new Uint8Array(height * stride);

  for (let y = 0; y < height; y++) {
    const filter = inflated[y * (stride + 1)];
    const srcRow = y * (stride + 1) + 1;
    const dstRow = y * stride;
    for (let i = 0; i < stride; i++) {
      const rawByte = inflated[srcRow + i];
      const a = i >= bpp ? recon[dstRow + i - bpp] : 0;
      const b = y > 0 ? recon[dstRow - stride + i] : 0;
      const c = (y > 0 && i >= bpp) ? recon[dstRow - stride + i - bpp] : 0;
      let val;
      if (filter === 0) val = rawByte;
      else if (filter === 1) val = (rawByte + a) & 0xff;
      else if (filter === 2) val = (rawByte + b) & 0xff;
      else if (filter === 3) val = (rawByte + ((a + b) >> 1)) & 0xff;
      else if (filter === 4) val = (rawByte + paethPredictor(a, b, c)) & 0xff;
      else throw new Error(`Unknown PNG filter ${filter}`);
      recon[dstRow + i] = val;
    }
  }

  const rgba = new Uint8Array(width * height * 4);
  if (bpp === 4) {
    rgba.set(recon);
  } else {
    for (let i = 0, j = 0; i < recon.length; i += 3, j += 4) {
      rgba[j] = recon[i];
      rgba[j + 1] = recon[i + 1];
      rgba[j + 2] = recon[i + 2];
      rgba[j + 3] = 255;
    }
  }
  return { width, height, rgba };
}

/** Mean RGB of a decoded RGBA image inside a pixel box. */
export function meanRgbInBox(img, box) {
  const x0 = Math.max(0, Math.floor(box.x));
  const y0 = Math.max(0, Math.floor(box.y));
  const x1 = Math.min(img.width, Math.ceil(box.x + box.w));
  const y1 = Math.min(img.height, Math.ceil(box.y + box.h));
  const sum = [0, 0, 0];
  let count = 0;
  for (let y = y0; y < y1; y++) {
    for (let x = x0; x < x1; x++) {
      const i = (y * img.width + x) * 4;
      sum[0] += img.rgba[i];
      sum[1] += img.rgba[i + 1];
      sum[2] += img.rgba[i + 2];
      count++;
    }
  }
  return count ? sum.map((v) => v / count) : [NaN, NaN, NaN];
}
