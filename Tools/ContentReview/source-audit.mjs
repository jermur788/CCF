#!/usr/bin/env node
/* Read-only lexical drift scan for an actual CCF checkout. Does not launch Unity or modify files. */
import {readFileSync,readdirSync,existsSync,statSync} from "node:fs";
import {createHash} from "node:crypto";
import {resolve,dirname,join,relative} from "node:path";
import {fileURLToPath} from "node:url";
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
result.notes.push("A source blob change or new lexical candidate requires human extraction, not automatic replacement.");
result.notes.push("UnmappedSource also includes non-player-facing C# helpers; serialized candidates are not certified visible text.");
if(process.argv.includes("--json"))process.stdout.write(JSON.stringify(result,null,2)+"\n");
else{
 process.stdout.write("CCF Text Editor source audit\nBaseline files unchanged: "+result.verified.length+"\nChanged: "+result.changed.length+"\nMissing: "+result.missing.length+"\nUnmapped candidate source files: "+result.unmappedSource.length+"\nSerialized text candidate assets: "+result.serializedTextCandidates.length+"\n");
 for(const [name,arr] of [["Changed",result.changed],["Missing",result.missing],["Unmapped",result.unmappedSource],["Serialized",result.serializedTextCandidates]])if(arr.length)process.stdout.write(name+": "+JSON.stringify(arr.slice(0,30))+"\n");
}
if(result.changed.length||result.missing.length)process.exitCode=1;
