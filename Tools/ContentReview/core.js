/* CCF portable content contract: no DOM, filesystem, network or third-party dependencies. */
(function (root, factory) {
  const api = factory();
  if (typeof module === "object" && module.exports) module.exports = api;
  else root.CCFContentCore = api;
})(typeof globalThis === "object" ? globalThis : this, function () {
  "use strict";
  const VERSION = "ccf.content.v1";
  const tokenPattern = /\{\{([a-zA-Z_][\w]*)\}\}/g;
  const clone = (value) => JSON.parse(JSON.stringify(value));
  const tokens = (text) => [...String(text).matchAll(tokenPattern)].map((m) => m[1]);
  const same = (a,b) => JSON.stringify(a) === JSON.stringify(b);
  const changed = (catalog) => catalog.entries.filter((e) => e.text !== e.baselineText);
  const position = (list, id) => list.findIndex((v) => v.id === id);
  function validate(pkg, seed) {
    const errors = [], warnings = [];
    if (!pkg || typeof pkg !== "object") return { errors:["Not a JSON catalogue object."],warnings,ok:false };
    if (pkg.schemaVersion !== VERSION) errors.push("Unsupported schema version. Expected "+VERSION+".");
    for (const kind of ["scenarios","pages","entries"]) if (!Array.isArray(pkg[kind])) errors.push(kind+" must be an array.");
    if (errors.length) return { errors,warnings,ok:false };
    const unique = (items,kind) => {const seen=new Set();for(const item of items){if(!item||typeof item.id!=="string"||!item.id)errors.push("Invalid "+kind+" ID.");else if(seen.has(item.id))errors.push("Duplicate "+kind+" ID: "+item.id);else seen.add(item.id);}};
    unique(pkg.scenarios,"scenario");unique(pkg.pages,"page");unique(pkg.entries,"entry");
    const scenarioIds=new Set(pkg.scenarios.map((s)=>s.id)),pageIds=new Set(pkg.pages.map((p)=>p.id));
    for(const p of pkg.pages)if(!scenarioIds.has(p.scenarioId))errors.push("Orphan page: "+p.id);
    for(const e of pkg.entries)if(!pageIds.has(e.pageId))errors.push("Orphan entry: "+e.id);
    if(seed){
      if(pkg.packageId!==seed.packageId)errors.push("Protected package identity changed.");
      if(!same(pkg.provenance,seed.provenance))errors.push("Protected source provenance / approval facts changed.");
      if(!same(pkg.coverage,seed.coverage))errors.push("Protected source coverage metadata changed.");
      if(!Number.isSafeInteger(pkg.revision)||pkg.revision<1)errors.push("Invalid draft revision.");
      if(pkg.authoringStatus!==undefined&&pkg.authoringStatus!=="draft-local-export")errors.push("Unsupported approval/authoring status.");
      if(!same(pkg.scenarios,seed.scenarios))errors.push("Scenario identities/status changed. Text editor cannot modify scenario routing.");
      if(!same(pkg.pages,seed.pages))errors.push("Page order, gameplay IDs or hierarchy changed. This editor only changes text.");
      if(pkg.entries.length!==seed.entries.length)errors.push("Entry count changed; adding/removing fields needs schema review.");
      for(const baseline of seed.entries){
        const e=pkg.entries.find((v)=>v.id===baseline.id);
        if(!e){errors.push("Missing required content ID: "+baseline.id);continue;}
        for(const prop of ["pageId","fieldKey","variantId","baselineText","required","source","status","protectedGameplayKey","tokens","format","note"]){
          if(!same(e[prop],baseline[prop]))errors.push("Protected "+prop+" modified for "+baseline.id);
        }
      }
    }
    for(const e of pkg.entries){
      if(typeof e.text!=="string"){errors.push("Non-text entry "+e.id);continue;}
      if(e.required && !e.text.trim())errors.push("Required text is blank: "+e.id);
      if(/[\u0000-\u0008\u000b\u000c\u000e-\u001f\u007f]/.test(e.text))errors.push("Disallowed control character: "+e.id);
      if(e.text.length>10000)errors.push("Over 10,000 characters: "+e.id);
      else if(e.text.length>3000)warnings.push("Long text; check layout after Unity integration: "+e.id);
      if(/<[A-Za-z/!][^>]*>/.test(e.text))errors.push("Rich/HTML markup is not supported in V1: "+e.id);
      const expected=tokens(e.baselineText||""),actual=tokens(e.text);
      if(!same(expected,actual))errors.push("Missing/extra/reordered placeholder(s): "+e.id+" (expected "+expected.join(", ")+", found "+actual.join(", ")+")");
      if(e.text.includes("{{")&& e.text.replace(tokenPattern,"").includes("{{"))errors.push("Malformed placeholder: "+e.id);
      if(e.text.includes("}}")&& e.text.replace(tokenPattern,"").includes("}}"))errors.push("Unmatched placeholder close: "+e.id);
    }
    return {errors,warnings,ok:errors.length===0,changedCount:changed(pkg).length,entryCount:pkg.entries.length,pageCount:pkg.pages.length};
  }
  function draft(pkg) {
    const out=clone(pkg);
    out.revision=(Number.isInteger(pkg.revision)?pkg.revision:0)+1;
    out.authoringStatus="draft-local-export";
    out.exportedAt=new Date().toISOString();
    out.provenance.unityIntegrated=false;
    out.provenance.reviewed=false;
    return out;
  }
  return {VERSION,clone,tokens,changed,position,validate,draft};
});
