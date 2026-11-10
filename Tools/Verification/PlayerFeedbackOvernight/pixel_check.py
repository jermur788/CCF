#!/usr/bin/env python3
"""Offline draw-plan raster witness ONLY: synthetic source-backed fixture, not a Unity screenshot.
Both positive and removed-feeder mutation must be distinguishable. No scene/save access.
"""
import json, math, pathlib
from PIL import Image,ImageDraw
out=pathlib.Path('/tmp/ccf363-preflight')
def draw(side,items,omit=False):
 im=Image.new('RGB',(side,side),(26,45,31)); d=ImageDraw.Draw(im)
 for a in items:
  pts=[(p['X'],p['Y']) for p in a['Points']]
  if len(pts)<2: continue
  # remove the source feeder strokes, leaving background, original path and endpoint pads as negative control
  if omit and len(pts)==2 and abs(pts[0][0]-(4.6/80*side))<.1 and abs(pts[0][1]-(67.4/80*side))<.1 and abs(pts[1][0]-(37.563834/80*side))<.1: continue
  color=a['Colour']; rgb=tuple(color[k] for k in ['R','G','B'])
  if a['Filled']:
   if color['A']>0:d.polygon(pts,fill=rgb)
  else:
   width=max(1,round(a['WidthPx']));dash=a['DashPx'];gap=a['GapPx']
   if a['Closed']:pts.append(pts[0])
   for p,q in zip(pts,pts[1:]):
    length=math.dist(p,q)
    if not length:continue
    if dash>0 and gap>0:
     n=0
     while n<length:
      b=min(n+dash,length);d.line([(p[0]+(q[0]-p[0])*v/length,p[1]+(q[1]-p[1])*v/length) for v in (n,b)],fill=rgb,width=width);n+=dash+gap
    else:d.line([p,q],fill=rgb,width=width)
 return im
def witness(im,side):
 y=round(67.4/80*side);xs=range(round(10/80*side),round(30/80*side))
 light=lambda p:sum(p)/3
 differences=[abs(light(im.getpixel((x,y+3)))-light(im.getpixel((x,y+24)))) for x in xs]
 return sum(differences)/len(differences)
for side,label in [(480,'1280x720'),(800,'1920x1080')]:
 items=json.loads((out/f'plan-{side}.json').read_text()); im=draw(side,items); bad=draw(side,items,True)
 score=witness(im,side); neg=witness(bad,side)
 assert score>80 and neg<5,(score,neg)
 assert len(im.getcolors(side*side))>3 and im.getbbox(), 'content-bearing nonblack image required'
 im.save(out/f'offline-feeder-{label}.png');im.convert('L').save(out/f'offline-feeder-{label}-gray.png')
 print(f'{label} offline map side={side}px: feeder/background mean luminance difference={score:.2f}; removed-feeder mutation={neg:.2f}; PASS (not Unity evidence)')
