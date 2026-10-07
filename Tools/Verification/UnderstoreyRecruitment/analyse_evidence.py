from pathlib import Path
import csv,json,shutil,gzip,collections,re
R=Path(__file__).resolve().parents[3]; E=R/'Docs/Research/UnderstoreyRecruitment/Evidence'; B=R/'Build/UnderstoreyRecruitment/evidence';E.mkdir(exist_ok=True)
for n in ['gap_fixtures.csv','cell_species_ledger.csv','recolonisation_runtime.csv','performance.csv','repeat_hashes.csv','baseline_years.csv']:shutil.copyfile(B/n,E/n)
rows=list(csv.DictReader((B/'baseline_years.csv').open())); groups=collections.defaultdict(list)
for r in rows:
 if r['repeat']=='0':groups[r['run']].append(r)
out=[]
for label,rs in groups.items():
 for y in [10,25,50,100]:
  prior=[r for r in rs if int(r['year'])<=y];r=next(r for r in rs if int(r['year'])==y)
  out.append(dict(run=label,year=y,species=r['species'],remaining=float(r['remaining']),promoted=sum(int(t['exact_recruits']) for t in prior),accepted=sum(float(t['accepted']) for t in prior),browse_loss=sum(float(t['browse_loss']) for t in prior),light_loss=sum(float(t['light_loss']) for t in prior),clearance_cells=sum(int(t['clearance_cells']) for t in prior),living_adults=int(r['living_adults']),deadwood_records=int(r['deadwood_records']),ferns=float(r['ferns']),grasses=float(r['grasses']),shrubs=float(r['shrubs']),first_recruit_year=next((int(t['year']) for t in prior if int(t['exact_recruits'])>0),'none'),competition_loss='not implemented',planting_count=0,shelter_count=0,cash_delta='not settled'))
with (E/'baseline_horizons.csv').open('w') as f:
 w=csv.DictWriter(f,list(out[0]),lineterminator='\n');w.writeheader();w.writerows(out)
# Retain raw virtual sweep reproducibly compressed. The script regenerates CSV.
if (E/'candidate_sensitivity.csv').exists():
 with (E/'candidate_sensitivity.csv').open('rb') as src,gzip.GzipFile(str(E/'candidate_sensitivity.csv.gz'),'wb',mtime=0) as dst:shutil.copyfileobj(src,dst)
 candidate=list(csv.DictReader((E/'candidate_sensitivity.csv').open()));(E/'candidate_sensitivity.csv').unlink()
else:
 with gzip.open(E/'candidate_sensitivity.csv.gz','rt') as f:candidate=list(csv.DictReader(f))
summary=[]
for process in sorted({r['process'] for r in candidate}):
 for strength in [.15,.35,.6]:
  subset=[r for r in candidate if r['process']==process and float(r['strength'])==strength and r['year']=='20' and r['initial_cover']=='1' and r['browse']=='0' and r['clearance']=='none' and r['recruitment_source']=='0']
  summary.append(dict(process=process,strength=strength,grade='C',min_existing_virtual_abundance=min(float(r['virtual_abundance']) for r in subset),max_existing_virtual_abundance=max(float(r['virtual_abundance']) for r in subset),scope='relative response; no production light mortality or promotion'))
with (E/'candidate_ranges.csv').open('w') as f:
 w=csv.DictWriter(f,list(summary[0]),lineterminator='\n');w.writeheader();w.writerows(summary)
# Imported inventory + source metadata + explicit adequacy unknown rather than false affirmative.
A=R/'Docs/Research/ScenarioOneAssets';src={r['path']:r for r in csv.DictReader((A/'SourceAssetRegister.csv').open())};items=list(csv.DictReader((B/'AssetInventory.csv').open()))
def family(p):
 p=p.lower()
 for key,label in [('pruning','pruning'),('sitka','Sitka tree'),('sessile','oak tree'),('beech','beech tree'),('deadwood','deadwood'),('fallen','deadwood'),('bracken','bracken'),('bramble','bramble'),('bilberry','bilberry'),('fern','fern'),('moss','moss'),('fung','fungi'),('grass','graminoid'),('rush','graminoid'),('forb','herbs'),('shelter','shelter'),('ground','ground/track'),('audio','audio'),('ui','UI')]:
  if key in p:return label
 return 'other/support'
for r in items:
 s=src.get(r['path'],{});meta=R/(r['path']+'.meta');text=meta.read_text(errors='replace') if meta.exists() else '';settings={}
 for key in ['enableMipMap','sRGBTexture','alphaIsTransparency','filterMode','wrapU','wrapV','textureCompression','isReadable','meshCompression','useFileScale','globalScale']:
  match=re.search(r'^\s*'+key+r':\s*(.+)$',text,re.M);settings[key]=match.group(1).strip() if match else 'not explicitly recorded'
 r.update(import_settings=json.dumps(settings,sort_keys=True),rereview_after_import='preserve source GUID; active render and interaction needs runtime confirmation')
 r.update(source_note=s.get('source_note','not in art register; inspect adjacent source documentation'),source_bytes=s.get('bytes',''),guid=s.get('guid',''),texture_max_size=s.get('texture_max_size',''),mechanic_purpose=family(r['path']),visual_state='not individually rendered; family review in reports',technical_state='imported; hierarchy measured; runtime-generated geometry needs runtime audit',variant_relationship='family/path grouping; mesh overlap not deduplicated',license_status='unverified; source note is not a rights clearance',pivot_origin='not measured; verify ground-centred in creation acceptance')
with (A/'AssetInventory.csv').open('w') as f:
 w=csv.DictWriter(f,list(items[0]),lineterminator='\n');w.writeheader();w.writerows(items)
AE=A/'Evidence';AE.mkdir(exist_ok=True);shutil.copyfile(B/'runtime_renderers.csv',AE/'runtime_renderers.csv')
print(json.dumps({'horizons':len(out),'candidate_ranges':len(summary),'imported_assets':len(items),'baseline_rows':len(rows)}))
