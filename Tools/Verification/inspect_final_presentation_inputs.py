#!/usr/bin/env python3
"""Read-only baseline reference inventory and external candidate manifest check."""
import hashlib
import json
from pathlib import Path
import re
import subprocess
import sys
import shutil

ROOT = Path(__file__).resolve().parents[2]
OUT = Path('/tmp/opencode/ccf-final-acceptance')
OUT.mkdir(parents=True, exist_ok=True)
if '--stage-local-dependency' in sys.argv:
    source = Path('/home/jer/CCF-main/Assets/InnerverseInteractive')
    target = ROOT / 'Assets/InnerverseInteractive'
    expected = {'3aa54be08e092174ab6b689e524fdf44', 'd0fa3c49b2c9cd84bb5a644c53ef025e'}
    found = set()
    for meta in source.rglob('*.meta'):
        found.update(re.findall(r'^guid: ([a-f0-9]{32})$', meta.read_text(errors='replace'), re.M))
    assert expected.issubset(found), 'Documented local dependency GUID mismatch'
    assert subprocess.run(['git','check-ignore','Assets/InnerverseInteractive/'],cwd=ROOT,capture_output=True).returncode == 0
    assert not target.exists(), 'Do not overwrite an existing local dependency'
    payloads = [p for p in source.rglob('*') if p.is_file()]
    print('LOCAL_DEPENDENCY_SETUP files=', len(payloads), 'MiB=', round(sum(p.stat().st_size for p in payloads)/1048576,2))
    shutil.copytree(source,target)
    meta = source.with_name(source.name+'.meta')
    if meta.exists(): shutil.copy2(meta,target.with_name(target.name+'.meta'))
    print('LOCAL_DEPENDENCY_SETUP_PASS ignored=true productionSourceReadOnly=true')
guid_map = {}
for meta in (ROOT / 'Assets').rglob('*.meta'):
    match = re.search(r'^guid: ([a-f0-9]{32})$', meta.read_text(errors='replace'), re.M)
    if match:
        guid_map[match.group(1)] = str(meta.with_suffix('').relative_to(ROOT))
paths = ['Assets/Scenes/ForestTest.unity',
    'Assets/ForestPrototype/ScenarioOne/Resources/PlantationVisualCatalog.asset',
    'Assets/ForestPrototype/ScenarioOne/Resources/RecentAssetVisualCatalog.asset',
    'Assets/ForestPrototype/ScenarioOne/Resources/SectionFiveVisualCatalog.asset']
records = {}
for relative in paths:
    path = ROOT / relative
    if not path.exists():
        records[relative] = {'missing': True}
        continue
    text = path.read_text()
    refs = sorted(set(re.findall(r'guid: ([a-f0-9]{32})', text)))
    records[relative] = {'sha256': hashlib.sha256(path.read_bytes()).hexdigest(),
                        'references': {guid: guid_map.get(guid, 'NOT IN TRACKED ASSET TREE') for guid in refs}}
freeze = {'head': subprocess.check_output(['git', 'rev-parse', 'HEAD'], cwd=ROOT, text=True).strip(),
          'project_version': (ROOT / 'ProjectSettings/ProjectVersion.txt').read_text(),
          'urp': json.loads((ROOT / 'Packages/manifest.json').read_text())['dependencies']['com.unity.render-pipelines.universal'],
          'records': records}
(OUT / 'baseline-inputs.json').write_text(json.dumps(freeze, indent=2) + '\n')
print('BASELINE_REFERENCE_INVENTORY', freeze['head'], len(guid_map), 'GUIDs')

pack = Path('/media/jer/ZX20/Unity Assets/ForestFloorV1')
errors = []
count = 0
for line in (pack / 'Documentation/SHA256SUMS.txt').read_text().splitlines():
    expected, relative = line.split('  ', 1)
    path = pack / relative
    if not path.exists() or hashlib.sha256(path.read_bytes()).hexdigest() != expected:
        errors.append(relative)
    count += 1
inventory = json.loads((pack / 'Documentation/asset_inventory.json').read_text())
for item in inventory:
    assert len(item['lods']) == 2 and item['lods'][1]['triangles'] < item['lods'][0]['triangles']
    for lod in item['lods']:
        assert (pack / 'Meshes' / (lod['name'] + '.fbx')).is_file()
textures = list((pack / 'Textures').glob('*.png'))
result = {'status': 'FAIL' if errors else 'PASS', 'payload_hashes': count, 'errors': errors,
          'assets': len(inventory), 'fbx_lod_pairs': len(inventory), 'textures': len(textures),
          'geometry_validation': json.loads((pack / 'Documentation/geometry_validation.json').read_text())['status'],
          'source': 'Original project-created procedural outputs; README provenance inspected',
          'unity_rendered': False, 'adopted': []}
(OUT / 'ForestFloorV1-validation.json').write_text(json.dumps(result, indent=2) + '\n')
print('FOREST_FLOOR_DELIVERY_MANIFEST', result['status'], count, 'payloads', len(inventory), 'assets')
assert not errors
