#!/usr/bin/env node
/* Run from anywhere: node Tools/ContentReview/tests.mjs (Node 18+). No Unity. */
import assert from "node:assert/strict";
import {readFileSync,existsSync,mkdtempSync,mkdirSync,writeFileSync,rmSync} from "node:fs";
import {tmpdir} from "node:os";
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
/* #487 N-01: Tree Marks composed help is source-exact constituents plus a read-only derived preview. */
const TM="s1.help.treemarks.",F1=TM+"explanation.full-F1";
const tmFields=[TM+"explanationIntro.full-F1",TM+"TreeMarkEntries0Label.full-F1",TM+"TreeMarkEntries0Meaning.full-F1",TM+"TreeMarkEntries1Label.full-F1",TM+"TreeMarkEntries1Meaning.full-F1",TM+"TreeMarkEntries2Label.full-F1",TM+"TreeMarkEntries2Meaning.full-F1",TM+"explanationSprayLine.full-F1",TM+"explanationClosing.full-F1"];
const OLD_FLAT_SLOT="•  — ";
const entryOf=(d,id)=>d.entries.find(e=>e.id===id);
check("revision 3 seed: Tree Marks full-F1 is one derived read-only entry with its original ID",()=>{
 assert.equal(seed.revision,3);assert.equal(seed.entries.length,278);
 const derived=seed.entries.filter(e=>e.derived!==undefined);
 assert.deepEqual(derived.map(e=>e.id),[F1]);
 const f1=derived[0];
 assert.equal(f1.derived.readOnly,true);assert.equal(f1.text,f1.baselineText);
 assert.ok(!f1.baselineText.includes(OLD_FLAT_SLOT));
 assert.ok(f1.baselineText.includes("• Orange vertical stripes — Rack / machine-path removal\n• Red — Fell\n• Blue — Crop Tree / future timber\n\nOrange vertical stripes identify"));
 assert.equal(core.compose(f1.derived,seed.entries,"baselineText").text,f1.baselineText);
 assert.deepEqual(core.compose(f1.derived,seed.entries,"baselineText").missing,[]);
});
check("Tree Marks constituents are nine editable source-exact entries and the two existing constants keep their IDs",()=>{
 for(const id of tmFields){const e=entryOf(seed,id);assert.ok(e,id);assert.equal(e.derived,undefined);assert.equal(e.required,true);assert.equal(e.source.path,"Assets/ForestPrototype/UI/MenuHelpView.Text.cs");assert.ok(e.source.line>=21&&e.source.line<=99,id);}
 assert.equal(entryOf(seed,TM+"TreeMarkEntries1Label.full-F1").baselineText,"Red");
 assert.equal(entryOf(seed,TM+"TreeMarkEntries2Meaning.full-F1").baselineText,"Crop Tree / future timber");
 assert.equal(entryOf(seed,TM+"TreeMarksOrangeText.enlarged80-rack-v2").source.line,27);
 assert.equal(entryOf(seed,TM+"TreeMarksSimplificationText.enlarged80-rack-v2").source.line,28);
 assert.equal(entryOf(seed,TM+"title").baselineText,"Introducing tree marking");
 for(const e of seed.entries.filter(e=>e.pageId==="s1.help.treemarks"&&e.derived===undefined))assert.ok(!/•\s*(?:—|$)/m.test(e.baselineText),"blank bullet slot in "+e.id);
});
check("derived read-only entry cannot be edited, even together with its baseline or by stripping its derived metadata",()=>{
 const edited=mutated(d=>entryOf(d,F1).text+=" Edited");
 assert.ok(!v(edited).ok);assert.ok(v(edited).errors.some(x=>x.includes("Derived read-only field was edited")));
 assert.ok(!v(mutated(d=>{const e=entryOf(d,F1);e.text=e.baselineText="Flattened replacement";})).ok);
 assert.ok(!v(mutated(d=>{delete entryOf(d,F1).derived;entryOf(d,F1).text+=" now editable";})).ok);
 assert.ok(!v(mutated(d=>{delete entryOf(d,F1).derived;})).ok);
});
check("fake editable flattened Tree Marks copy with blank bullet slots is rejected",()=>{
 const old=seed.entries.find(e=>e.id===F1).baselineText;
 const flat=old.replace(/• Orange[^\n]*\n• Red[^\n]*\n• Blue[^\n]*/,OLD_FLAT_SLOT.trim());
 assert.ok(flat.includes("•  —"));
 for(const change of [d=>{const e=entryOf(d,F1);delete e.derived;e.text=e.baselineText=flat;},d=>{entryOf(d,F1).text=flat;},d=>{entryOf(d,tmFields[1]).text=OLD_FLAT_SLOT.trim();entryOf(d,F1).derived.readOnly=false;}])
  assert.ok(!v(mutated(change)).ok);
});
check("derived metadata is closed, read-only and cannot be forged or added to ordinary entries",()=>{
 for(const change of [
  d=>entryOf(d,F1).derived.readOnly=false,
  d=>entryOf(d,F1).derived.approved=true,
  d=>entryOf(d,F1).derived.parts[0].approved=true,
  d=>entryOf(d,F1).derived.parts=[],
  d=>entryOf(d,F1).derived.parts[0]={entryId:tmFields[0],glue:"x"},
  d=>entryOf(d,F1).derived.parts[0]={},
  d=>entryOf(d,F1).derived.parts[0].entryId="missing.id",
  d=>{const p=entryOf(d,F1).derived.parts;[p[0],p[2]]=[p[2],p[0]];},
  d=>entryOf(d,F1).derived.parts.push({glue:"\n"}),
  d=>entryOf(d,F1).derived.parts[1].line=-1,
  d=>entryOf(d,F1).derived=null,
  d=>entryOf(d,F1).derived=true,
  d=>entryOf(d,tmFields[0]).derived=clonePart(entryOf(d,F1).derived),
  d=>d.entries[0].derived={readOnly:true,composedBy:"x",parts:[{glue:"x"}]},
  d=>entryOf(d,F1).editable=true,
  d=>entryOf(d,F1).kind="editable",
  d=>entryOf(d,F1).derived.parts.push({entryId:F1})
 ])assert.ok(!v(mutated(change)).ok,String(change));
 function clonePart(x){return JSON.parse(JSON.stringify(x));}
});
check("derived structure and composition are enforced on their own, even without a bundled seed",()=>{
 const alone=(p)=>core.validate(p);
 assert.ok(alone(seed).ok);
 const expectError=(change,text)=>{const r=alone(mutated(change));assert.ok(!r.ok,text);assert.ok(r.errors.some(x=>x.includes(text)),text+" in "+JSON.stringify(r.errors));};
 expectError(d=>entryOf(d,F1).derived.readOnly=false,"must stay read-only");
 expectError(d=>entryOf(d,F1).derived.approved=true,"Unknown entries[");
 expectError(d=>entryOf(d,F1).derived.parts[0].approved=true,"Unknown entries[");
 expectError(d=>entryOf(d,F1).derived.parts[0]={entryId:tmFields[0],glue:"x"},"exactly one of entryId or glue");
 expectError(d=>entryOf(d,F1).derived.parts[0]={},"exactly one of entryId or glue");
 expectError(d=>entryOf(d,F1).derived.parts[0].entryId="missing.id","missing or derived entry");
 expectError(d=>entryOf(d,F1).derived.parts.push({entryId:F1}),"missing or derived entry");
 expectError(d=>{const e=entryOf(d,tmFields[1]);e.baselineText=e.text="Green";},"not the composition of its source constituents");
 expectError(d=>entryOf(d,F1).text+="x","Derived read-only field was edited");
 expectError(d=>entryOf(d,F1).derived.parts[1].glue="​","Disallowed control/invisible character in derived glue");
 expectError(d=>entryOf(d,F1).derived.parts=[],"needs composition parts");
 expectError(d=>entryOf(d,F1).derived.composedBy="","composedBy");
});
check("editing a constituent stays valid, recomposes the preview and never edits the derived entry",()=>{
 const d=mutated(p=>{entryOf(p,tmFields[3]).text="Red paint";entryOf(p,tmFields[8]).text=entryOf(p,tmFields[8]).text.replace("Marking cuts nothing","Marking cuts nothing at all");});
 const r=v(d);assert.ok(r.ok,r.errors.join("; "));assert.equal(r.changedCount,2);
 const live=core.compose(entryOf(d,F1).derived,d.entries,"text").text;
 assert.ok(live.includes("• Red paint — Fell"));assert.ok(live.includes("Marking cuts nothing at all"));
 assert.notEqual(live,entryOf(d,F1).baselineText);
 assert.equal(entryOf(d,F1).text,entryOf(d,F1).baselineText);
 const exp=core.draft(d);
 assert.deepEqual(entryOf(exp,F1),entryOf(seed,F1));
 const back=JSON.parse(JSON.stringify(exp));
 assert.ok(v(back).ok);assert.equal(core.changed(back).length,2);
 assert.deepEqual(back.entries.map(e=>e.id),seed.entries.map(e=>e.id));
});
check("immutable identifiers: all 269 revision-2 content IDs are retained and only nine IDs are added",()=>{
 const ids=new Set(seed.entries.map(e=>e.id));
 assert.equal(ids.size,278);assert.equal(tmFields.filter(id=>ids.has(id)).length,9);
 assert.equal(seed.entries.filter(e=>!tmFields.includes(e.id)).length,269);
 for(const id of [F1,TM+"title",TM+"TreeMarksOrangeText.enlarged80-rack-v2",TM+"TreeMarksSimplificationText.enlarged80-rack-v2"])assert.ok(ids.has(id),id);
 // the Tree Marks page keeps its identity, order and immutable page metadata
 assert.deepEqual(seed.pages.find(p=>p.id==="s1.help.treemarks"),{id:"s1.help.treemarks",scenarioId:"s1",section:"F1 & first-use help",title:"TreeMarks",order:75,status:"main-source"});
 assert.equal(seed.pages.length,79);
});
check("exports from the revision-2 269-entry seed are rejected, not silently accepted",()=>{
 const old=mutated(d=>{
  d.revision=3;d.entries=d.entries.filter(e=>!tmFields.includes(e.id));
  const f=entryOf(d,F1);delete f.derived;f.text=f.baselineText="Scenario 1 introduces 3 CCF markings.\n\n•  — \n\n One spray paints vertical stripes on four sides of the entrance tree.";
 });
 assert.equal(old.entries.length,269);
 const r=v(old);assert.ok(!r.ok);assert.ok(r.errors.some(x=>x.includes("Entry count changed")));
});
check("app renders derived entries as read-only previews with no textarea or revert control",()=>{
 const js=read("app.js");
 assert.ok(js.includes("function displayDerived"));
 assert.ok(js.indexOf("if(isDerived(entry)){displayDerived(entry);return;}")>0);
 assert.ok(js.indexOf("if(isDerived(entry)){displayDerived(entry);return;}")<js.indexOf('add(card,"textarea"'));
 const body=js.slice(js.indexOf("function displayDerived"),js.indexOf("function displayField"));
 assert.ok(!/textarea|"input"|Revert field|contentEditable/.test(body));
 assert.ok(read("catalog.js").includes('"derived"'));
});
{
 const repo=join(dir,"..",".."),cs=join(repo,"Assets","ForestPrototype","UI","MenuHelpView.Text.cs");
 if(!existsSync(cs))process.stdout.write("SKIP Tree Marks source audit (no full CCF checkout at "+repo+"): NOT RUN\n");
 else{
  const audit=(root)=>{let out;try{out=execFileSync(process.execPath,[join(dir,"source-audit.mjs"),root,"--json"],{stdio:["ignore","pipe","pipe"],maxBuffer:64*1024*1024}).toString();}catch(e){out=String(e.stdout||"");}return JSON.parse(out).treeMarks;};
  check("source audit verifies every Tree Marks fragment, the two constants and the derived composition against the C# statement",()=>{
   const r=audit(repo);assert.ok(r.ok,JSON.stringify(r.errors));assert.equal(r.verified.length,12);
  });
  check("source audit fails closed when the C# marks, glue or statement shape drift",()=>{
   const original=readFileSync(cs,"utf8"),tmp=mkdtempSync(join(tmpdir(),"ccf-tm-"));
   try{
    const run=(change)=>{const text=change(original);assert.notEqual(text,original);const file=join(tmp,"Assets","ForestPrototype","UI");mkdirSync(file,{recursive:true});writeFileSync(join(file,"MenuHelpView.Text.cs"),text);return audit(tmp);};
    for(const change of [
     t=>t.replace('"Red", "Fell"','"Red", "Felled"'),
     t=>t.replace('"• " + entry[0]','"- " + entry[0]'),
     t=>t.replace("One spray paints","Two sprays paint"),
     t=>t.replace('+ TreeMarksSimplificationText + "\\n\\n"','+ TreeMarksSimplificationText'),
     t=>t.replace('new[] { "Blue", "Crop Tree / future timber" }','new[] { "Blue", "Crop Tree / future timber" },\n        new[] { "Green", "Retain" }'),
     t=>t.replace("Marking cuts nothing","Marking cuts something")
    ]){const r=run(change);assert.ok(!r.ok,"drift not detected for "+String(change));assert.ok(r.errors.length>0);}
   }finally{rmSync(tmp,{recursive:true,force:true});}
  });
 }
}
process.stdout.write("\n"+passes+" deterministic static checks passed. NOT Unity or rendered-browser qualification.\n");
