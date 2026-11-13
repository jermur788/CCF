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
process.stdout.write("\n"+passes+" deterministic static checks passed. NOT Unity or rendered-browser qualification.\n");
