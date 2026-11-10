#!/usr/bin/env python3
"""#379 tagged prose composition regression; source/consumer evidence, not pixel fit."""
import argparse
from pathlib import Path
import subprocess
import sys
sys.dont_write_bytecode = True
from run_checks import member


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--src', type=Path, required=True)
    parser.add_argument('--out', type=Path, required=True)
    args = parser.parse_args()
    args.out.mkdir(parents=True, exist_ok=True)
    source = (args.src / 'Assets/ForestPrototype/UI/AnnualReviewView.cs').read_text()
    styles = (args.src / 'Assets/ForestPrototype/UI/Resources/ScenarioOneUi.uss').read_text()
    for selector, rules in {
        '.annual-text-entry': ['flex-direction: column', 'flex-shrink: 0', 'min-width: 0'],
        '.annual-text-body': ['align-self: stretch', 'white-space: normal'],
        '.annual-text-source': ['flex-direction: row', 'flex-wrap: wrap'],
    }.items():
        block = styles[styles.index(selector + ' {'):].split('}', 1)[0]
        assert all(rule in block for rule in rules), selector
    print('PASS annual-scoped column/name/amount style contracts (not render proof)')
    method = member(source, 'private static Label AddTagged(')
    tag = member(source, 'private static Label Tag(')
    code = r'''using System;using System.Collections.Generic;
enum HistoryProvenance {Recorded,Derived,Current}
class Style {public int flexShrink,marginTop;}
class VisualElement {public Style style=new Style();public string text="";public List<string> classes=new List<string>();public List<VisualElement> children=new List<VisualElement>();public void Add(VisualElement e)=>children.Add(e);public void AddToClassList(string c)=>classes.Add(c);}
class Label:VisualElement {}
static class ScenarioOneTeachingSequence {public static string SourceQualifier(HistoryProvenance k)=>"qualifier:"+k;}
static class HistoryProvenanceText {public static string Label(HistoryProvenance k)=>k.ToString();}
static class UiKit {public static VisualElement Box(params string[] c){var e=new VisualElement();e.classes.AddRange(c);return e;}public static Label Add(VisualElement p,string t,params string[] c){var e=new Label{text=t};e.classes.AddRange(c);p.Add(e);return e;}public static Label Chip(VisualElement p,string t,string c)=>Add(p,t,c);}
class Probe {
static string ChipStyle(HistoryProvenance k)=>"chip";
/*TAG*/
/*METHOD*/
static void Need(bool ok,string label){if(!ok)throw new Exception("FAIL "+label);}
static void Main(){int cases=0;foreach(var kind in new[]{HistoryProvenance.Recorded,HistoryProvenance.Derived,HistoryProvenance.Current})foreach(var text in new[]{"Standing trees 1195 → 1195","Harvest job · year 3: harvest orders settled together as one job.","Areas with young regeneration: 73 (was 47) — Sitka spruce 73"})foreach(var cls in new[]{"body","muted"})foreach(var show in new[]{true,false}){
var parent=UiKit.Box();var label=AddTagged(parent,kind,text,cls);var entry=parent.children[0];Need(entry.classes.Contains("annual-text-entry")&&entry.children.Count==2,"INDEPENDENT_SOURCE_BODY");var meta=entry.children[0];Need(meta.classes.Contains("annual-text-source")&&meta.children.Count==2,"METADATA");Need(meta.children[0].text=="qualifier:"+kind,"QUALIFIER");var chip=meta.children[1];Need(chip.text==kind.ToString()&&chip.classes.Contains("prov")&&chip.classes.Contains("prov-"+kind.ToString().ToLowerInvariant()),"CHIP_HOOKS");Need(Object.ReferenceEquals(label,entry.children[1])&&label.text==text&&label.classes.Contains(cls)&&label.classes.Contains("annual-text-body"),"FULL_BODY_RETURN");Need(meta.children[0].text.Length>0&&(!show||chip.text.Length>0),"TOGGLE_PRESERVES_METADATA");cases++;}Console.WriteLine("PASS exact AddTagged/Tag consumer "+cases+" cases (toggle uses existing prov hook; no render proof)");}}
'''.replace('/*METHOD*/', method).replace('/*TAG*/', tag)

    def run(name, content):
        out = args.out / name
        out.mkdir(exist_ok=True)
        (out / 'Probe.cs').write_text(content)
        (out / 'probe.csproj').write_text('<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><OutputType>Exe</OutputType><TargetFramework>net8.0</TargetFramework></PropertyGroup></Project>')
        result = subprocess.run(['dotnet', 'run', '--project', str(out / 'probe.csproj'), '-v', 'q'], capture_output=True, text=True)
        (out / 'result.log').write_text(result.stdout + result.stderr)
        return result

    result = run('positive', code)
    assert result.returncode == 0, result.stdout + result.stderr
    print(result.stdout.strip())
    # Restore the old competing horizontal siblings; must fail structure assertion.
    bad = method[:method.index('{') + 1] + '''
        var row = UiKit.Box("row"); Tag(row, kind);
        var label = UiKit.Add(row, text, textClass); parent.Add(row); return label;
    }'''
    result = run('old-width-competition', code.replace(method, bad))
    assert result.returncode != 0 and 'FAIL INDEPENDENT_SOURCE_BODY' in result.stderr and 'error CS' not in result.stdout
    print('KILLED compiled old-width-competition by INDEPENDENT_SOURCE_BODY')


if __name__ == '__main__':
    main()
