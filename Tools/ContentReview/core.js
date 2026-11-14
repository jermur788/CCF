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
  const schema = Object.freeze({
    package: ["schemaVersion","packageId","revision","provenance","scenarios","pages","entries","coverage","authoringStatus","exportedAt"],
    provenance: ["repository","sourceBranch","sourceCommit","comparisonCandidateBranch","comparisonCandidateCommit","extractedOn","unityIntegrated","reviewed","coverageClaim"],
    scenario: ["id","label","profile","status"],
    page: ["id","scenarioId","section","title","order","status","parentPageId","protectedStepId","protectedLessonId","reading","requiresStepId"],
    entry: ["id","pageId","fieldKey","variantId","text","baselineText","required","source","status","protectedGameplayKey","format","tokens","note"],
    source: ["path","branch","commit","line","kind"],
    coverage: ["classifications","notes"],
    classifications: ["collected","conditional","generated","serialized","unlocated","outOfScope"]
  });
  const isRecord = (obj) => obj !== null && typeof obj === "object" && !Array.isArray(obj);
  const disallowedUnicode = /[\u0000-\u0008\u000b\u000c\u000e-\u001f\u007f-\u009f\u00ad\u034f\u061c\u180e\u200b-\u200f\u202a-\u202e\u2060-\u206f\uFEFF]/;
  function validate(pkg, seed) {
    const errors = [], warnings = [];
    const count = (obj, name) => Array.isArray(obj?.[name]) ? obj[name].length : 0;
    const result = () => ({
      errors, warnings, ok:errors.length===0,
      changedCount:Array.isArray(pkg?.entries) ? pkg.entries.filter(e=>isRecord(e)&&typeof e.text==="string"&&e.text!==e.baselineText).length : 0,
      entryCount:count(pkg,"entries"), pageCount:count(pkg,"pages")
    });
    function closed(obj, kind, where) {
      if (!isRecord(obj)) {errors.push(where+" must be an object.");return false;}
      const allowed=new Set(schema[kind]);
      for (const key of Object.keys(obj)) if (!allowed.has(key)) errors.push("Unknown "+where+" field: "+key);
      return true;
    }
    if (!closed(pkg,"package","package")) return result();
    if (pkg.schemaVersion!==VERSION) errors.push("Unsupported schema version. Expected "+VERSION+".");
    for (const kind of ["scenarios","pages","entries"]) if (!Array.isArray(pkg[kind])) errors.push(kind+" must be an array.");
    if (!closed(pkg.provenance,"provenance","provenance")) errors.push("Source provenance is required.");
    if (closed(pkg.coverage,"coverage","coverage")) {
      closed(pkg.coverage.classifications,"classifications","coverage.classifications");
      if (!Array.isArray(pkg.coverage.notes)||pkg.coverage.notes.some(n=>typeof n!=="string")) errors.push("coverage.notes must contain strings.");
      for (const field of schema.classifications) {
        const v=pkg.coverage.classifications?.[field];
        if (!(v===null||(Number.isSafeInteger(v)&&v>=0))) errors.push("Invalid or unmeasured coverage value: "+field);
      }
    }
    if (!Number.isSafeInteger(pkg.revision)||pkg.revision<1||pkg.revision>=Number.MAX_SAFE_INTEGER) errors.push("Invalid draft revision.");
    if (pkg.authoringStatus!==undefined&&pkg.authoringStatus!=="draft-local-export") errors.push("Unsupported approval/authoring status.");
    if (pkg.exportedAt!==undefined&&(typeof pkg.exportedAt!=="string"||!/^\d{4}-\d\d-\d\dT/.test(pkg.exportedAt))) errors.push("Invalid export timestamp.");
    if (!Array.isArray(pkg.scenarios)||!Array.isArray(pkg.pages)||!Array.isArray(pkg.entries)) return result();

    const unique=(items,kind)=>{const seen=new Set();for(const item of items){
      if (!isRecord(item)||typeof item.id!=="string"||!item.id) {errors.push("Invalid "+kind+" ID.");continue;}
      if(seen.has(item.id))errors.push("Duplicate "+kind+" ID: "+item.id);else seen.add(item.id);
    }};
    pkg.scenarios.forEach((s,i)=>closed(s,"scenario","scenarios["+i+"]"));
    pkg.pages.forEach((p,i)=>closed(p,"page","pages["+i+"]"));
    pkg.entries.forEach((e,i)=>{
      if (closed(e,"entry","entries["+i+"]")) {
        closed(e.source,"source","entries["+i+"].source");
        if (!Array.isArray(e.tokens)||e.tokens.some(t=>typeof t!=="string"))errors.push("Invalid token signature: "+e.id);
      }
    });
    unique(pkg.scenarios,"scenario");unique(pkg.pages,"page");unique(pkg.entries,"entry");
    const scenarioIds=new Set(pkg.scenarios.map(s=>s?.id)),pageIds=new Set(pkg.pages.map(p=>p?.id));
    for(const p of pkg.pages)if(isRecord(p)&&!scenarioIds.has(p.scenarioId))errors.push("Orphan page: "+p.id);
    for(const e of pkg.entries)if(isRecord(e)&&!pageIds.has(e.pageId))errors.push("Orphan entry: "+e.id);

    if (isRecord(seed)) {
      if(pkg.packageId!==seed.packageId)errors.push("Protected package identity changed.");
      if(!same(pkg.provenance,seed.provenance))errors.push("Protected source provenance / approval facts changed.");
      if(!same(pkg.coverage,seed.coverage))errors.push("Protected source coverage metadata changed.");
      if(!same(pkg.scenarios,seed.scenarios))errors.push("Scenario identities/status changed. Text editor cannot modify scenario routing.");
      if(!same(pkg.pages,seed.pages))errors.push("Page order, gameplay IDs or hierarchy changed. This editor only changes text.");
      if(pkg.entries.length!==seed.entries.length)errors.push("Entry count changed; adding/removing fields needs schema review.");
      if(!same(pkg.entries.map(e=>e?.id),seed.entries.map(e=>e?.id)))errors.push("Entry field order / identities changed.");
      const entriesById=new Map(pkg.entries.filter(isRecord).map(e=>[e.id,e]));
      for(const baseline of seed.entries) {
        const e=entriesById.get(baseline.id);
        if(!e){errors.push("Missing required content ID: "+baseline.id);continue;}
        for(const prop of ["pageId","fieldKey","variantId","baselineText","required","source","status","protectedGameplayKey","tokens","format","note"]){
          if(!same(e[prop],baseline[prop]))errors.push("Protected "+prop+" modified for "+baseline.id);
        }
      }
    }
    for (const e of pkg.entries) {
      if (!isRecord(e))continue;
      if(typeof e.text!=="string"){errors.push("Non-text entry "+e.id);continue;}
      if(e.required&&!e.text.trim())errors.push("Required text is blank: "+e.id);
      if(disallowedUnicode.test(e.text))errors.push("Disallowed control/invisible/bidi character: "+e.id);
      if(e.text.length>10000)errors.push("Over 10,000 characters: "+e.id);
      else if(e.text.length>3000)warnings.push("Long text; check layout after Unity integration: "+e.id);
      if(/<[A-Za-z/!][^>]*>/.test(e.text))errors.push("Rich/HTML markup is not supported in V1: "+e.id);
      const expected=tokens(e.baselineText||""),actual=tokens(e.text);
      if(!same(expected,actual))errors.push("Missing/extra/reordered placeholder(s): "+e.id+" (expected "+expected.join(", ")+", found "+actual.join(", ")+")");
      if(e.text.includes("{{")&&e.text.replace(tokenPattern,"").includes("{{"))errors.push("Malformed placeholder: "+e.id);
      if(e.text.includes("}}")&&e.text.replace(tokenPattern,"").includes("}}"))errors.push("Unmatched placeholder close: "+e.id);
    }
    return result();
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
