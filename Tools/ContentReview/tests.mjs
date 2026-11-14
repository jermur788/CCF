#!/usr/bin/env node
/* Run from anywhere: node Tools/ContentReview/tests.mjs (Node 18+). No Unity. */
import assert from "node:assert/strict";
import {readFileSync} from "node:fs";
import {dirname,join} from "node:path";
import {fileURLToPath} from "node:url";
import vm from "node:vm";
import {createRequire} from "node:module";
import {execFileSync} from "node:child_process";
const dir=dirname(fileURLToPath(import.meta.url)),require=createRequire(import.meta.url);
const core=require(join(dir,"core.js"));
const read=(f)=>readFileSync(join(dir,f),"utf8");
const seed=JSON.parse(read("catalog.json"));
let passes=0;
function check(name,fn){fn();passes++;process.stdout.write("PASS "+name+"\n");}
const v=(p)=>core.validate(p,seed);
const mutated=(action)=>{const d=core.clone(seed);action(d);return d;};
check("baseline has source provenance and source pages",()=>{assert.equal(seed.schemaVersion,core.VERSION);assert.ok(seed.pages.length>=70);assert.ok(seed.entries.length>=200);assert.ok(v(seed).ok);});
check("embedded offline catalogue exactly equals source JSON",()=>{
  const context={window:{}};vm.runInNewContext(read("catalog.js"),context,{timeout:3000});
  assert.deepEqual(JSON.parse(JSON.stringify(context.window.CCF_CONTENT_SEED)),seed);
});
check("source variants remain separate",()=>{
 assert.ok(seed.entries.some(e=>e.variantId==="enlarged80-rack-v2"));
 assert.ok(seed.entries.some(e=>e.variantId==="legacy-or-fallback"));
 assert.ok(seed.entries.some(e=>e.status==="unmerged-candidate"));
 assert.ok(seed.entries.filter(e=>e.status==="unmerged-candidate").every(e=>e.source.branch!=="main"));
});
check("future scenarios and compact draft do not invent text",()=>{
  assert.equal(seed.scenarios.filter(s=>s.id.startsWith("s")&&!["s1","s1-compact"].includes(s.id)).length,7);
  assert.ok(seed.pages.every(p=>p.scenarioId==="s1"));
});
check("Unicode, apostrophes, arrows, newline and slash preserve JSON round trip",()=>{
 const p=mutated(d=>{d.entries[0].text="Fadó → O'Brien & oak\\beech\nTá an choill beo.";});
 assert.ok(v(p).ok);assert.equal(JSON.parse(JSON.stringify(p)).entries[0].text,p.entries[0].text);
});
check("missing token rejected",()=>{
 const p=mutated(d=>{const e=d.entries.find(e=>e.tokens.length);assert.ok(e);e.text=e.text.replace(/\{\{[\w]+\}\}/,"");});
 assert.ok(!v(p).ok);assert.ok(v(p).errors.some(x=>x.includes("placeholder")));
});
check("extra token rejected",()=>{const p=mutated(d=>d.entries[0].text+=" {{unknown}}");assert.ok(!v(p).ok);});
check("reordered tokens rejected",()=>{
 const p=mutated(d=>{const e=d.entries.find(e=>core.tokens(e.text).length>1);assert.ok(e);let t=core.tokens(e.text);e.text=e.text.replace("{{"+t[0]+"}}","%%TEMP%%").replace("{{"+t[1]+"}}","{{"+t[0]+"}}").replace("%%TEMP%%","{{"+t[1]+"}}");});
 assert.ok(!v(p).ok);
});
check("protected step identity mutation rejected",()=>{
 const p=mutated(d=>d.pages.find(p=>p.protectedStepId).protectedStepId="tampered");
 assert.ok(!v(p).ok);assert.ok(v(p).errors.some(x=>x.includes("Page order")));
});
check("source provenance mutation rejected",()=>{const p=mutated(d=>d.entries[0].source.path="Assets/Evil.cs");assert.ok(!v(p).ok);});
check("untrusted approval stamp rejected",()=>{const p=mutated(d=>d.provenance.reviewed=true);assert.ok(!v(p).ok);});
check("package ID, note and coverage tampering rejected",()=>{
 for(const change of [d=>d.packageId="unknown",d=>d.entries[0].note="touched",d=>d.coverage.notes.push("fake")]){
  assert.ok(!v(mutated(change)).ok);
 }
});
check("duplicate content ID rejected",()=>{const p=mutated(d=>d.entries[1].id=d.entries[0].id);assert.ok(!v(p).ok);});
check("empty required text rejected",()=>{const p=mutated(d=>d.entries[0].text="   ");assert.ok(!v(p).ok);});
check("control characters rejected",()=>{const p=mutated(d=>d.entries[0].text="bad\u0000copy");assert.ok(!v(p).ok);});
check("markup rejected",()=>{const p=mutated(d=>d.entries[0].text="<img src=x onerror=alert(1)>");assert.ok(!v(p).ok);});
check("malformed dynamic token rejected",()=>{const p=mutated(d=>d.entries[0].text+=" {{bad");assert.ok(!v(p).ok);});
check("draft export is reversible, not approved",()=>{
 const p=mutated(d=>d.entries[0].text+=" Revised");
 const exp=core.draft(p);
 assert.equal(exp.authoringStatus,"draft-local-export");
 assert.equal(exp.provenance.unityIntegrated,false);
 assert.equal(exp.provenance.reviewed,false);
 assert.equal(exp.revision,p.revision+1);
 assert.equal(core.changed(exp).length,1);
 assert.ok(v(JSON.parse(JSON.stringify(exp))).ok);
});
check("app never uses innerHTML to display draft text",()=>{
 assert.ok(!/\.innerHTML\s*=|insertAdjacentHTML/.test(read("app.js")));
 assert.ok(read("app.js").includes("textContent"));
});
check("all browser scripts parse",()=>{
 for(const file of ["core.js","catalog.js","app.js"])execFileSync(process.execPath,["--check",join(dir,file)],{stdio:"pipe"});
});

/* #474 F-02–F-13: committed fail-closed cases, not unpublished V8-only checks. */
check("closed package schema rejects misleading approval flags B01",()=>{
 const d=mutated(p=>{p.approved=true;p.integrated=true;p.approvedBy="Manager #424";});
 assert.ok(!v(d).ok);assert.ok(v(d).errors.some(x=>x.includes("Unknown package field: approved")));
});
check("closed entry schema rejects B02 approval flags",()=>{
 const d=mutated(p=>{p.entries[0].approved=true;p.entries[0].reviewedBy="owner";});
 assert.ok(!v(d).ok);assert.ok(v(d).errors.some(x=>x.includes("Unknown entries[0] field")));
});
check("closed nested schemas reject forged source and approval metadata",()=>{
 for(const mutate of [
  p=>p.provenance.approvedBy="owner",
  p=>p.entries[0].source.approved=true,
  p=>p.scenarios[0].certified=true,
  p=>p.pages[0].approved=true,
  p=>p.coverage.approval="yes",
  p=>p.coverage.classifications.extra=1
 ])assert.ok(!v(mutated(mutate)).ok);
});
check("field array order B03 remains immutable",()=>{
 const d=mutated(p=>{[p.entries[0],p.entries[1]]=[p.entries[1],p.entries[0]];});
 assert.ok(!v(d).ok);assert.ok(v(d).errors.some(x=>x.includes("field order")));
});
check("bidi zero-width C1 and BOM B04 B05 reject",()=>{
 for(const cp of [0x0085,0x009b,0x200b,0x200c,0x200d,0x202e,0x202c,0x2060,0x2064,0xfeff]){
  const d=mutated(p=>p.entries[0].text="Tree"+String.fromCodePoint(cp)+"growth");
  assert.ok(!v(d).ok,"accepted U+"+cp.toString(16));
 }
});
check("common Irish accents, Unicode arrows, newline and tab remain valid",()=>{
 const d=mutated(p=>p.entries[0].text="Tá Fadó — O’Brien's → \\ path\nplanting\tOak");
 assert.ok(v(d).ok);
});
check("invalid schema and early-invalid object always have numeric counts",()=>{
 for(const d of [null,[],{},mutated(p=>p.schemaVersion="ccf.content.v0"),mutated(p=>p.entries={})]){
  const a=v(d);assert.ok(!a.ok);
  assert.ok(Number.isFinite(a.entryCount)&&Number.isFinite(a.pageCount)&&Number.isFinite(a.changedCount));
 }
});
check("unsafe revision number and authoring-status promotions rejected",()=>{
 for(const change of [p=>p.revision=0,p=>p.revision=1.5,p=>p.revision=Number.MAX_SAFE_INTEGER,p=>p.authoringStatus="approved"]){
  assert.ok(!v(mutated(change)).ok);
 }
});
check("protected scenarios page order and page routing rejected",()=>{
 for(const change of [
  p=>p.scenarios[0].label="Fake",
  p=>p.pages[0].order=99,
  p=>p.pages[0].scenarioId="s2",
  p=>[p.pages[0],p.pages[1]]=[p.pages[1],p.pages[0]]
 ])assert.ok(!v(mutated(change)).ok);
});
check("modified baseline/candidate and source metadata rejected",()=>{
 for(const change of [
  p=>p.entries[0].baselineText="altered",
  p=>p.entries[0].status="reviewed-not-integrated",
  p=>p.entries[0].source.line=999,
  p=>p.provenance.comparisonCandidateCommit="spoofed"
 ])assert.ok(!v(mutated(change)).ok);
});
check("added/removed entry rejected",()=>{
 const added=mutated(p=>p.entries.push({...p.entries[0],id:"unexpected.new"}));
 const removed=mutated(p=>p.entries.pop());
 assert.ok(!v(added).ok);assert.ok(!v(removed).ok);
});
check("text over 10000 characters rejects and 3000 warns",()=>{
 const long=mutated(p=>p.entries[0].text="a".repeat(10001));
 assert.ok(!v(long).ok);
 const medium=mutated(p=>p.entries[0].text="a".repeat(3300));
 assert.ok(v(medium).ok);assert.ok(v(medium).warnings.length>0);
});
check("coverage unknown values are null, never deceptive measured zero",()=>{
 for(const key of ["serialized","unlocated","outOfScope"])assert.equal(seed.coverage.classifications[key],null);
});
check("Walking HUD title and candidate source commit are traceable",()=>{
 const e=seed.entries.find(e=>e.id==="s1.help.walkinghud.title");
 assert.equal(e.baselineText,"Walking HUD · your forest at a glance");
 assert.equal(e.source.line,46);
 assert.ok(/^[a-f0-9]{40}$/.test(seed.provenance.comparisonCandidateCommit));
 assert.equal(seed.entries.filter(e=>e.status==="unmerged-candidate").length,3);
 assert.ok(seed.entries.filter(e=>e.status==="unmerged-candidate").every(e=>e.source.commit===seed.provenance.comparisonCandidateCommit));
});
check("source-provenance repair covers exact C# line anchors",()=>{
 const at=(id,line)=>assert.equal(seed.entries.find(e=>e.id===id).source.line,line,id);
 at("s1.help.treeinspection.explanation.full-F1",106);
 at("s1.annual.review.workColumn",146);
 at("s1.workplan.overview.title",128);
 at("s1.teaching.access.practice",39);
});
check("app source uses beforeunload, accurate status search and safe large-file preflight",()=>{
 const js=read("app.js");
 assert.ok(js.includes("beforeunload"));
 assert.ok(js.includes("file.size>3000000"));
 assert.ok(js.includes("navStatus.textContent"));
 assert.ok(!js.includes('p.id+" "+(catalog.scenarios'));
});
process.stdout.write("\n"+passes+" deterministic static checks passed. NOT Unity or rendered-browser qualification.\n");
