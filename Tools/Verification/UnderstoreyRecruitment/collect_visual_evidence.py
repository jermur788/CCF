#!/usr/bin/env python3
"""Publish selected full-frame audit captures; no scene or source-art edits."""
from pathlib import Path
import csv,hashlib,shutil
from PIL import Image
R=Path(__file__).resolve().parents[3];B=R/'Build/UnderstoreyRecruitment';E=R/'Docs/Research/ScenarioOneAssets/Evidence'
E.mkdir(exist_ok=True)
selected={'plantation-1280','map-1280','workplan-1280','heavy-understorey','cleared-patch','isolated-natural-sitka-spruce','isolated-natural-sessile-oak','isolated-natural-beech','isolated-planted-beech','thinned-area','deadwood','inspection'}
rows=[]
for source in sorted((B/'evidence').glob('*.png')):
 with Image.open(source) as im:
  delivered=''
  if source.stem in selected:
   target=E/(source.stem+'.jpg');im.convert('RGB').save(target,quality=90,optimize=True);delivered=str(target.relative_to(R))
  rows.append(dict(source=str(source.relative_to(R)),sha256=hashlib.sha256(source.read_bytes()).hexdigest(),width=im.width,height=im.height,delivered=delivered,scope='synthetic audit world; capture execution is not visual adequacy or FPS proof'))
with (E/'CaptureManifest.csv').open('w') as f:
 w=csv.DictWriter(f,list(rows[0]),lineterminator='\n');w.writeheader();w.writerows(rows)
for name in ['runtime_renderers.csv','juvenile_display_heights.csv']:shutil.copyfile(B/'evidence'/name,E/name)
shutil.copyfile(B/'UnderstoreyVisualAudit.result.json',E/'visual_capture_result.json')
print(f'{len(rows)} raw captures inventoried; {len(selected)} distinct full-frame JPEGs retained')
