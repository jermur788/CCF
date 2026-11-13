/* CCF Text Editor V1. All imported or edited copy is rendered as TEXT, never HTML. */
(() => {
  "use strict";
  const core=window.CCFContentCore;
  const seed=core.clone(window.CCF_CONTENT_SEED);
  let catalog=core.clone(seed),scenarioId="s1",pageId=null,variantId="all",query="",statusFilter="all";
  const $=(id)=>document.getElementById(id);
  const element=(tag,className,text)=>{const node=document.createElement(tag);if(className)node.className=className;if(text!==undefined)node.textContent=String(text);return node;};
  const add=(parent,tag,className,text)=>{const n=element(tag,className,text);parent.appendChild(n);return n;};
  const clear=(node)=>node.replaceChildren();
  const getPage=()=>catalog.pages.find((p)=>p.id===pageId)||null;
  const getScenario=()=>catalog.scenarios.find((s)=>s.id===scenarioId)||null;
  const pageEntries=(id)=>catalog.entries.filter((e)=>e.pageId===id);
  const visibleEntries=()=>pageEntries(pageId).filter((e)=>variantId==="all"||e.variantId==="default"||e.variantId===variantId);
  const dirty=(e)=>e.text!==e.baselineText;
  const totalChanged=()=>core.changed(catalog).length;
  const availablePages=()=>{
    const pages=catalog.pages.filter((p)=>statusFilter==="all"||p.status===statusFilter);
    const matching=pages.filter((p)=>{
      if(!query)return p.scenarioId===scenarioId;
      const source=(p.title+" "+p.section+" "+p.id+" "+(catalog.scenarios.find(s=>s.id===p.scenarioId)?.label||"")+" "+pageEntries(p.id).map(e=>e.text+" "+e.baselineText+" "+e.id).join(" ")).toLocaleLowerCase();
      return source.includes(query);
    });
    return matching.sort((a,b)=>a.order-b.order);
  };
  function notify(message){
    const n=$("notice");n.textContent=message;n.style.display="block";
  }
  function renderGlobal(){
    const n=totalChanged();
    $("dirty-status").textContent=n?n+" locally edited field"+(n===1?"":"s")+" · not approved":"No edits · source baseline";
    const cov=$("coverage");clear(cov);
    add(cov,"strong","",catalog.pages.length+" source pages · "+catalog.entries.length+" fields");
    add(cov,"div","",n+" fields changed. Prototype inventory is not all Unity text.");
    add(cov,"div","","No edits change the game; download and separately review the JSON.");
  }
  function renderScenarios(){
    const nav=$("scenario-list");clear(nav);
    catalog.scenarios.forEach((s)=>{
      const count=catalog.pages.filter((p)=>p.scenarioId===s.id).length;
      const b=add(nav,"button","scenario-button"+(s.id===scenarioId?" active":""),s.label);
      b.type="button";b.setAttribute("aria-current",s.id===scenarioId?"page":"false");
      add(b,"small","",count?count+" source pages · "+s.status:"NO SHIPPED TEXT YET · "+s.status);
      b.addEventListener("click",()=>{
        scenarioId=s.id;query="";$("search").value="";statusFilter="all";$("status-filter").value="all";
        const first=catalog.pages.find((p)=>p.scenarioId===scenarioId);
        pageId=first?.id||null;variantId="all";render();
      });
    });
  }
  function renderPages(){
    const nav=$("page-list");clear(nav);
    const pages=availablePages();$("pages-heading").textContent=query?"Search results · "+pages.length:"Pages · "+pages.length;
    if(!pages.length){
      add(nav,"div","empty-result",query?"No matching text in imported pages.":"No shipped text in this scenario. It is a placeholder, not a missing file.");return;
    }
    let group="";
    for(const p of pages){
      if(group!==p.section){group=p.section;add(nav,"div","page-group",group);}
      const b=add(nav,"button","page-button"+(pageId===p.id?" active":""),p.title);
      b.type="button";b.setAttribute("aria-current",pageId===p.id?"page":"false");
      const change=pageEntries(p.id).filter(dirty).length;
      add(b,"small","",p.status+(change?" · "+change+" edited":""));
      b.addEventListener("click",()=>{
        scenarioId=p.scenarioId;pageId=p.id;variantId="all";render();
        $("main").focus({preventScroll:true});
      });
    }
  }
  function appendMeta(container,text){add(container,"span","",text);}
  function renderVariants(){
    const sel=$("variant-filter");clear(sel);
    const ids=[...new Set(pageEntries(pageId).map((e)=>e.variantId))];
    const all=add(sel,"option","","All variants ("+ids.length+")");all.value="all";
    ids.forEach((id)=>{const o=add(sel,"option","",id);o.value=id;});
    if(variantId!=="all"&&!ids.includes(variantId))variantId="all";
    sel.value=variantId;
    sel.disabled=ids.length<2;
  }
  function renderHeader(){
    const p=getPage(),s=getScenario(),list=availablePages(),idx=list.findIndex((x)=>x.id===pageId);
    $("path").textContent=p?(s?.label||p.scenarioId)+"  /  "+p.section:"Select a scenario";
    $("page-title").textContent=p?p.title:(s?.label||"CCF source text");
    $("page-description").textContent=p?
      "Review source-extracted fields and conditional variants. Stable gameplay IDs are read-only; a draft export is not integrated or approved.":
      "NO SHIPPED TEXT YET. This chapter has no authored catalogue entries in the inspected main source. No future story or curriculum has been invented.";
    clear($("page-meta"));
    if(p){
      appendMeta($("page-meta"),"Source: "+p.status);
      appendMeta($("page-meta"),"Immutable page ID: "+p.id);
      if(p.protectedStepId)appendMeta($("page-meta"),"Protected lesson-progress key: "+p.protectedStepId);
      if(p.protectedLessonId)appendMeta($("page-meta"),"Protected teaching ID: "+p.protectedLessonId);
      appendMeta($("page-meta"),pageEntries(p.id).length+" field(s)");
    }
    $("previous").disabled=idx<=0;$("next").disabled=idx<0||idx>=list.length-1;
    $("page-number").textContent=idx<0?"":(idx+1)+" / "+list.length;
    $("variant-filter").disabled=!p;
    $("revert-page").disabled=!p||!pageEntries(pageId).some(dirty);
  }
  function displayField(entry){
    const card=add($("fields"),"article","field"+(dirty(entry)?" changed":""));
    const top=add(card,"div","field-top");
    const lbl=add(top,"label","",entry.fieldKey);
    const controls=add(top,"div","field-actions");
    const flag=add(controls,"span","flag",dirty(entry)?"EDITED":"ORIGINAL");
    const revert=add(controls,"button","","Revert field");
    revert.type="button";revert.disabled=!dirty(entry);revert.addEventListener("click",()=>{
      entry.text=entry.baselineText;render();
    });
    const info=add(card,"div","field-meta","Content ID (locked): "+entry.id+" · variant: "+entry.variantId+" · "+entry.status);
    if(entry.protectedGameplayKey)add(card,"div","field-meta","Protected gameplay identifier: "+entry.protectedGameplayKey+" (not editable)");
    const area=add(card,"textarea","");
    area.id="edit-"+catalog.entries.indexOf(entry);lbl.htmlFor=area.id;
    area.rows=Math.max(3,Math.min(12,entry.text.split("\n").length+Math.ceil(entry.text.length/95)));
    area.value=entry.text;area.setAttribute("spellcheck","true");
    area.addEventListener("input",()=>{
      entry.text=area.value;
      const changed=dirty(entry);card.classList.toggle("changed",changed);
      flag.textContent=changed?"EDITED":"ORIGINAL";revert.disabled=!changed;
      $("revert-page").disabled=!pageEntries(pageId).some(dirty);
      renderGlobal();renderComparison();
    });
    const path=entry.source?.path||"unknown";const line=entry.source?.line;
    add(card,"div","source",(entry.source?.branch||"?")+" · "+path+(line?" : "+line:"")+" · "+entry.source?.kind);
    if(entry.tokens.length)add(card,"div","field-meta","Required parameter tokens: "+entry.tokens.map((t)=>"{{"+t+"}}").join(", "));
    if(entry.note)add(card,"p","muted",entry.note);
  }
  function renderFields(){
    clear($("fields"));const p=getPage();if(!p){$("variant-filter").disabled=true;$("revert-page").disabled=true;return;}
    renderVariants();
    const items=visibleEntries();
    if(!items.length)add($("fields"),"p","intro","No fields in this variant.");
    else items.forEach(displayField);
  }
  function renderComparison(){
    const root=$("comparison-fields");clear(root);
    const items=visibleEntries();
    if(!items.length){add(root,"p","intro","Choose a page to compare source and current draft.");return;}
    for(const e of items){
      const block=add(root,"article","compare-item");
      add(block,"h3","",e.fieldKey+" · "+e.variantId+(dirty(e)?" · CHANGED":" · unchanged"));
      const cols=add(block,"div","compare-cols");
      const orig=add(cols,"div");add(orig,"h4","","Original source");add(orig,"pre","",e.baselineText);
      const edit=add(cols,"div");add(edit,"h4","","Current draft");add(edit,"pre","edited",e.text);
    }
  }
  function showValidation(result,prefix=""){
    const root=$("validation-results");clear(root);
    add(root,"strong","",prefix+(result.ok?"PASS: portable catalogue structure and tokens":"INVALID: do not export/apply"));
    add(root,"p","",result.entryCount+" entries / "+result.pageCount+" pages / "+result.changedCount+" edited fields. "+result.errors.length+" errors, "+result.warnings.length+" warnings.");
    if(result.errors.length){add(root,"h3","","Errors");const ul=add(root,"ul");result.errors.slice(0,100).forEach((s)=>add(ul,"li","",s));if(result.errors.length>100)add(root,"p","","More than 100 errors; showing the first 100.");}
    if(result.warnings.length){add(root,"h3","","Warnings");const ul=add(root,"ul");result.warnings.slice(0,100).forEach((s)=>add(ul,"li","",s));}
    add(root,"p","","Not checked: Unity compilation, built-game fonts/line breaks, conditional runtime routing, saves, or user acceptance.");
  }
  function render(){
    renderGlobal();renderScenarios();renderPages();renderHeader();renderFields();renderComparison();
  }
  function move(delta){
    const pages=availablePages(),idx=pages.findIndex((p)=>p.id===pageId),p=pages[idx+delta];if(!p)return;
    pageId=p.id;scenarioId=p.scenarioId;variantId="all";render();$("main").focus({preventScroll:true});
  }
  $("previous").addEventListener("click",()=>move(-1));
  $("next").addEventListener("click",()=>move(1));
  $("variant-filter").addEventListener("change",(e)=>{variantId=e.target.value;renderFields();renderComparison();});
  $("status-filter").addEventListener("change",(e)=>{statusFilter=e.target.value;
    const pages=availablePages();if(!pages.some(p=>p.id===pageId))pageId=pages[0]?.id||null;variantId="all";render();});
  $("search").addEventListener("input",(e)=>{query=e.target.value.trim().toLocaleLowerCase();
    const pages=availablePages();if(!pages.some(p=>p.id===pageId)){pageId=pages[0]?.id||null;scenarioId=pages[0]?.scenarioId||scenarioId;variantId="all";}render();});
  $("revert-page").addEventListener("click",()=>{
    const n=pageEntries(pageId).filter(dirty);if(!n.length)return;
    if(!window.confirm("Revert "+n.length+" changed field(s) on this page to the audited source text?"))return;
    n.forEach(e=>e.text=e.baselineText);render();notify("Current page restored; other edited pages preserved.");
  });
  $("validate-button").addEventListener("click",()=>{const result=core.validate(catalog,seed);showValidation(result);notify(result.ok?"Validation passed.":result.errors.length+" validation errors; see below.");});
  $("export-button").addEventListener("click",()=>{
    const result=core.validate(catalog,seed);showValidation(result);
    if(!result.ok){notify("Export blocked: fix validation errors first.");return;}
    const output=core.draft(catalog);
    const json=JSON.stringify(output,null,2)+"\n";
    const blob=new Blob([json],{type:"application/json;charset=utf-8"});
    const url=URL.createObjectURL(blob),link=document.createElement("a");
    link.href=url;link.download="ccf-text-draft-"+new Date().toISOString().slice(0,10)+".json";
    document.body.appendChild(link);link.click();link.remove();
    setTimeout(()=>URL.revokeObjectURL(url),1000);
    notify("Downloaded a local draft JSON file. This does not write into Unity or GitHub.");
  });
  $("import-button").addEventListener("click",()=>$("import-file").click());
  $("import-file").addEventListener("change",async(e)=>{
    const file=e.target.files?.[0];if(!file)return;
    try{
      const text=await file.text();if(text.length>3000000)throw new Error("File is too large (3 MB maximum).");
      const incoming=JSON.parse(text);
      const check=core.validate(incoming,seed);
      showValidation(check,"Import: ");
      if(!check.ok){notify("Import rejected; "+check.errors.length+" validation errors.");return;}
      if(totalChanged()&&!window.confirm("Replace your current unsaved draft with the imported file?"))return;
      catalog=core.clone(incoming);scenarioId="s1";pageId=catalog.pages[0]?.id||null;variantId="all";query="";statusFilter="all";$("search").value="";$("status-filter").value="all";
      render();notify("Imported "+file.name+"; "+core.changed(catalog).length+" edited fields restored.");
    }catch(error){showValidation({ok:false,entryCount:0,pageCount:0,changedCount:0,errors:[String(error.message||error)],warnings:[]},"Import: ");notify("Import failed. Original draft remains unchanged.");}
    finally{e.target.value="";}
  });
  document.addEventListener("keydown",(e)=>{
    if((e.ctrlKey||e.metaKey)&&e.key.toLowerCase()==="s"){e.preventDefault();$("export-button").click();}
    if(e.altKey&&!e.ctrlKey&&!e.metaKey){
      if(e.key==="ArrowLeft"){e.preventDefault();move(-1);}
      if(e.key==="ArrowRight"){e.preventDefault();move(1);}
    }
  });
  $("notice").addEventListener("click",()=>$("notice").style.display="none");
  pageId=catalog.pages.find((p)=>p.scenarioId==="s1")?.id||null;
  render();
})();
