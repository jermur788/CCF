#!/usr/bin/env node
/* Read-only lexical drift scan for an actual CCF checkout. Does not launch Unity or modify files. */
import {readFileSync,readdirSync,existsSync,statSync} from "node:fs";
import {createHash} from "node:crypto";
import {resolve,dirname,join,relative} from "node:path";
import {fileURLToPath} from "node:url";
import {createRequire} from "node:module";
const here=dirname(fileURLToPath(import.meta.url));
const root=resolve(process.argv[2]||join(here,"../.."));
const sourceMap=JSON.parse(readFileSync(join(here,"source-map.json"),"utf8"));
const blob=(text)=>createHash("sha1").update(Buffer.concat([Buffer.from("blob "+Buffer.byteLength(text)+"\0"),Buffer.from(text)])).digest("hex");
const count=(text,re)=>(text.match(re)||[]).length;
const result={root,baseline:sourceMap.auditedMain,verified:[],changed:[],missing:[],unmappedSource:[],serializedTextCandidates:[],notes:[]};
const known=new Set(sourceMap.files.map(s=>s.path));
for(const s of sourceMap.files){
 const file=join(root,s.path);
 if(!existsSync(file)){result.missing.push(s.path);continue;}
 const data=readFileSync(file),text=data.toString("utf8");
 const actualBlob=blob(data);
 const actualStrings=count(text,/"(?:\\.|[^"\\])*"/g);
 const actualInterpolation=count(text,/\$"/g);
 const check={path:s.path,baselineBlob:s.sourceBlob,blob:actualBlob,baselineLexical:s.quotedLiteralCandidates,lexical:actualStrings,baselineInterpolations:s.interpolatedStringStarts,interpolations:actualInterpolation};
 if(actualBlob!==s.sourceBlob)result.changed.push(check);else result.verified.push(s.path);
}
function walk(dir,callback,depth=0){
 if(depth>7||!existsSync(dir))return;
 for(const d of readdirSync(dir,{withFileTypes:true})){
  const target=join(dir,d.name);
  if(d.isDirectory())walk(target,callback,depth+1);
  else if(d.isFile())callback(target);
 }
}
for(const dir of ["Assets/ForestPrototype/UI","Assets/ForestPrototype/ScenarioOne"]){
 walk(join(root,dir),file=>{
  if(!file.endsWith(".cs"))return;
  const p=relative(root,file).replaceAll("\\","/");
  if(!known.has(p))result.unmappedSource.push(p);
 });
}
for(const dir of ["Assets/Scenes","Assets/ForestPrototype/UI/Resources","Assets/ForestPrototype/ScenarioOne/Resources"]){
 walk(join(root,dir),file=>{
  if(!/\.(unity|prefab|uxml|asset)$/.test(file))return;
  if(statSync(file).size>4_000_000)return;
  const t=readFileSync(file,"utf8");
  const n=count(t,/\bm_Text\s*:|\btext\s*=|\btext\s*:/g);
  if(n)result.serializedTextCandidates.push({path:relative(root,file),lexicalFieldCandidates:n});
 });
}
/* #487 N-01: the Tree Marks F1 help is composed by C#. Verify every editable fragment, the two existing constants and the
   derived read-only composition against the actual statement, not against a copy. Read-only; fails closed on any mismatch. */
function decodeCs(raw){
 const body=raw.slice(1,-1);let out="";
 for(let i=0;i<body.length;i++){
  const c=body[i];
  if(c!=="\\"){out+=c;continue;}
  const n=body[++i];
  const simple={n:"\n",t:"\t",r:"\r",'"':'"',"\\":"\\","'":"'","0":"\0"};
  if(n in simple)out+=simple[n];
  else if(n==="u"){out+=String.fromCharCode(parseInt(body.slice(i+1,i+5),16));i+=4;}
  else throw new Error("Unsupported C# escape \\"+n);
 }
 return out;
}
function treeMarksCheck(){
 const out={ok:false,verified:[],errors:[]};
 const fail=(m)=>out.errors.push(m);
 const csPath="Assets/ForestPrototype/UI/MenuHelpView.Text.cs",file=join(root,csPath);
 if(!existsSync(file)){fail(csPath+" not found; full checkout required.");return out;}
 const core=createRequire(import.meta.url)(join(here,"core.js"));
 const seed=JSON.parse(readFileSync(join(here,"catalog.json"),"utf8"));
 const src=readFileSync(file,"utf8").replace(/\r\n/g,"\n");
 const lineAt=(i)=>src.slice(0,i).split("\n").length;
 const litRe=/"(?:\\.|[^"\\])*"/g;
 const lit=(m,base)=>({value:decodeCs(m[0]),line:lineAt(base+m.index)});
 // TreeMarkEntries pairs
 const t0=src.indexOf("public static readonly string[][] TreeMarkEntries"),t1=src.indexOf("};",t0);
 if(t0<0||t1<0){fail("TreeMarkEntries declaration not found.");return out;}
 const pairs=[...src.slice(t0,t1).matchAll(/new\[\]\s*\{\s*("(?:\\.|[^"\\])*")\s*,\s*("(?:\\.|[^"\\])*")\s*\}/g)].map((m)=>({
  label:decodeCs(m[1]),meaning:decodeCs(m[2]),
  labelLine:lineAt(t0+m.index+m[0].indexOf(m[1])),meaningLine:lineAt(t0+m.index+m[0].lastIndexOf(m[2]))}));
 if(pairs.length!==3)fail("Expected exactly 3 TreeMarkEntries pairs, found "+pairs.length+".");
 const constant=(name)=>{
  const i=src.indexOf("public const string "+name),m=i<0?null:src.slice(i).match(/=\s*("(?:\\.|[^"\\])*")\s*;/);
  return m?{value:decodeCs(m[1]),line:lineAt(i)}:null;
 };
 const orange=constant("TreeMarksOrangeText"),simpl=constant("TreeMarksSimplificationText");
 if(!orange||!simpl){fail("TreeMarks constants not found.");return out;}
 // the composition statement inside Explanation(Menu.TreeMarks)
 const r0=src.indexOf("case Menu.TreeMarks:\n"),r1=src.indexOf("case Menu.Rack:",r0);
 if(r0<0||r1<0){fail("Tree Marks composition statement not found.");return out;}
 const lits=[...src.slice(r0,r1).matchAll(litRe)].map((m)=>lit(m,r0));
 if(lits.length!==10){fail("Composition statement shape changed: expected 10 string literals, found "+lits.length+". Re-extract by hand.");return out;}
 if(pairs.length!==3)return out;
 const [intro,sep,bullet,dash,after,spray,simplGlue,c1,c2,c3]=lits;
 const expected=intro.value+pairs.map((p)=>bullet.value+p.label+dash.value+p.meaning).join(sep.value)+after.value
  +orange.value+spray.value+simpl.value+simplGlue.value+c1.value+c2.value+c3.value;
 const byId=new Map(seed.entries.map((e)=>[e.id,e]));
 const P="s1.help.treemarks.";
 const want=[
  [P+"explanationIntro.full-F1",intro.value.replace(/\n$/,""),intro.line],
  ...pairs.flatMap((p,i)=>[[P+"TreeMarkEntries"+i+"Label.full-F1",p.label,p.labelLine],[P+"TreeMarkEntries"+i+"Meaning.full-F1",p.meaning,p.meaningLine]]),
  [P+"explanationSprayLine.full-F1",spray.value.replace(/^ /,"").replace(/\n\n$/,""),spray.line],
  [P+"explanationClosing.full-F1",c1.value+c2.value+c3.value,c1.line],
  [P+"TreeMarksOrangeText.enlarged80-rack-v2",orange.value,orange.line],
  [P+"TreeMarksSimplificationText.enlarged80-rack-v2",simpl.value,simpl.line]
 ];
 for(const [id,text,line] of want){
  const e=byId.get(id);
  if(!e){fail("Missing catalogue entry: "+id);continue;}
  if(e.baselineText!==text)fail("Source mismatch for "+id+": catalogue baseline differs from MenuHelpView.Text.cs.");
  else if(e.source.path!==csPath||e.source.line!==line)fail("Line anchor mismatch for "+id+": catalogue "+e.source.line+", source "+line+".");
  else if(e.derived!==undefined)fail("Constituent must not be derived: "+id);
  else out.verified.push(id);
 }
 const f1=byId.get(P+"explanation.full-F1");
 if(!f1||!f1.derived)fail("Composed full-F1 entry must exist and be derived/read-only.");
 else{
  if(f1.baselineText!==expected||f1.text!==expected)fail("Derived full-F1 text is not the composition produced by the C# statement.");
  const built=core.compose(f1.derived,seed.entries,"baselineText");
  if(built.missing.length||built.text!==expected)fail("Derived recipe does not recompose to the C# result.");
  for(const [n,part] of f1.derived.parts.entries())
   if(typeof part.glue==="string"&&!lits.some((l)=>l.line===part.line&&l.value.includes(part.glue)))fail("Glue part "+n+" is not found in the C# literal on line "+part.line+".");
  const listed=f1.derived.parts.filter((p)=>p.entryId).map((p)=>p.entryId);
  const order=[want[0][0],want[1][0],want[2][0],want[3][0],want[4][0],want[5][0],want[6][0],P+"TreeMarksOrangeText.enlarged80-rack-v2",want[7][0],P+"TreeMarksSimplificationText.enlarged80-rack-v2",want[8][0]];
  if(JSON.stringify(listed)!==JSON.stringify(order))fail("Derived recipe does not reference the constituents in C# order.");
  if(!out.errors.length)out.verified.push(P+"explanation.full-F1 (derived)");
 }
 for(const e of seed.entries.filter((x)=>x.pageId==="s1.help.treemarks"&&x.derived===undefined))
  if(/•\s*(?:—|$)|—\s*$/m.test(e.text))fail("Editable entry presents a blank bullet/dash slot: "+e.id);
 out.ok=out.errors.length===0;return out;
}
result.treeMarks=treeMarksCheck();
if(!result.treeMarks.ok)process.exitCode=1;
result.notes.push("A source blob change or new lexical candidate requires human extraction, not automatic replacement.");
result.notes.push("UnmappedSource also includes non-player-facing C# helpers; serialized candidates are not certified visible text.");
if(process.argv.includes("--json"))process.stdout.write(JSON.stringify(result,null,2)+"\n");
else{
 process.stdout.write("CCF Text Editor source audit\nBaseline files unchanged: "+result.verified.length+"\nChanged: "+result.changed.length+"\nMissing: "+result.missing.length+"\nUnmapped candidate source files: "+result.unmappedSource.length+"\nSerialized text candidate assets: "+result.serializedTextCandidates.length+"\nTree Marks composed copy: "+(result.treeMarks.ok?"VERIFIED ("+result.treeMarks.verified.length+" entries against MenuHelpView.Text.cs)":"MISMATCH "+JSON.stringify(result.treeMarks.errors))+"\n");
 for(const [name,arr] of [["Changed",result.changed],["Missing",result.missing],["Unmapped",result.unmappedSource],["Serialized",result.serializedTextCandidates]])if(arr.length)process.stdout.write(name+": "+JSON.stringify(arr.slice(0,30))+"\n");
}
if(result.changed.length||result.missing.length)process.exitCode=1;
